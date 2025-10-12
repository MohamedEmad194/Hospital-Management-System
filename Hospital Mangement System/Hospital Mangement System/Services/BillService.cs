using Microsoft.EntityFrameworkCore;
using Hospital_Management_System.Data;
using Hospital_Management_System.DTOs;
using Hospital_Management_System.Models;
using AutoMapper;

namespace Hospital_Management_System.Services
{
    public class BillService : IBillService
    {
        private readonly HospitalDbContext _context;
        private readonly IMapper _mapper;

        public BillService(HospitalDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BillDto>> GetAllBillsAsync()
        {
            var bills = await _context.Bills
                .Include(b => b.Patient)
                .Include(b => b.BillItems)
                .Where(b => !b.IsDeleted)
                .OrderByDescending(b => b.BillDate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<BillDto>>(bills);
        }

        public async Task<BillDto?> GetBillByIdAsync(int id)
        {
            var bill = await _context.Bills
                .Include(b => b.Patient)
                .Include(b => b.BillItems)
                .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);

            return bill == null ? null : _mapper.Map<BillDto>(bill);
        }

        public async Task<IEnumerable<BillDto>> GetBillsByPatientAsync(int patientId)
        {
            var bills = await _context.Bills
                .Include(b => b.Patient)
                .Include(b => b.BillItems)
                .Where(b => b.PatientId == patientId && !b.IsDeleted)
                .OrderByDescending(b => b.BillDate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<BillDto>>(bills);
        }

        public async Task<BillDto?> CreateBillAsync(CreateBillDto createBillDto)
        {
            // Verify patient exists
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.Id == createBillDto.PatientId && !p.IsDeleted);

            if (patient == null)
                throw new InvalidOperationException("Patient not found.");

            var bill = new Bill
            {
                BillNumber = await GenerateBillNumberAsync(),
                BillDate = createBillDto.BillDate,
                DueDate = createBillDto.DueDate,
                Notes = createBillDto.Notes,
                InsuranceProvider = createBillDto.InsuranceProvider,
                InsuranceNumber = createBillDto.InsuranceNumber,
                InsuranceCoverage = createBillDto.InsuranceCoverage,
                PatientId = createBillDto.PatientId,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            _context.Bills.Add(bill);
            await _context.SaveChangesAsync();

            // Add bill items
            decimal subTotal = 0;
            foreach (var itemDto in createBillDto.BillItems)
            {
                var billItem = new BillItem
                {
                    Description = itemDto.Description,
                    Category = itemDto.Category,
                    Quantity = itemDto.Quantity,
                    UnitPrice = itemDto.UnitPrice,
                    TotalPrice = itemDto.Quantity * itemDto.UnitPrice,
                    Notes = itemDto.Notes,
                    BillId = bill.Id,
                    CreatedAt = DateTime.UtcNow
                };

                _context.BillItems.Add(billItem);
                subTotal += billItem.TotalPrice;
            }

            // Calculate totals
            bill.SubTotal = subTotal;
            bill.TaxAmount = subTotal * 0.15m; // 15% tax
            bill.DiscountAmount = 0;
            bill.TotalAmount = bill.SubTotal + bill.TaxAmount - bill.DiscountAmount;
            bill.RemainingAmount = bill.TotalAmount;

            await _context.SaveChangesAsync();

            // Load related entities for response
            await _context.Entry(bill)
                .Reference(b => b.Patient)
                .LoadAsync();

            await _context.Entry(bill)
                .Collection(b => b.BillItems)
                .LoadAsync();

            return _mapper.Map<BillDto>(bill);
        }

        public async Task<BillDto?> UpdateBillAsync(int id, UpdateBillDto updateBillDto)
        {
            var bill = await _context.Bills
                .Include(b => b.Patient)
                .Include(b => b.BillItems)
                .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);

            if (bill == null)
                return null;

            // Update only provided fields
            if (updateBillDto.BillDate.HasValue)
                bill.BillDate = updateBillDto.BillDate.Value;

            if (updateBillDto.DueDate.HasValue)
                bill.DueDate = updateBillDto.DueDate.Value;

            if (!string.IsNullOrEmpty(updateBillDto.Status))
                bill.Status = updateBillDto.Status;

            if (updateBillDto.Notes != null)
                bill.Notes = updateBillDto.Notes;

            if (updateBillDto.PaymentMethod != null)
                bill.PaymentMethod = updateBillDto.PaymentMethod;

            if (updateBillDto.PaymentDate.HasValue)
                bill.PaymentDate = updateBillDto.PaymentDate.Value;

            if (updateBillDto.InsuranceProvider != null)
                bill.InsuranceProvider = updateBillDto.InsuranceProvider;

            if (updateBillDto.InsuranceNumber != null)
                bill.InsuranceNumber = updateBillDto.InsuranceNumber;

            if (updateBillDto.InsuranceCoverage.HasValue)
                bill.InsuranceCoverage = updateBillDto.InsuranceCoverage;

            bill.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return _mapper.Map<BillDto>(bill);
        }

        public async Task<bool> ProcessPaymentAsync(int billId, PaymentDto paymentDto)
        {
            var bill = await _context.Bills
                .FirstOrDefaultAsync(b => b.Id == billId && !b.IsDeleted);

            if (bill == null)
                return false;

            bill.PaidAmount += paymentDto.Amount;
            bill.RemainingAmount = bill.TotalAmount - bill.PaidAmount;
            bill.PaymentMethod = paymentDto.PaymentMethod;
            bill.PaymentDate = DateTime.UtcNow;

            if (bill.RemainingAmount <= 0)
            {
                bill.Status = "Paid";
            }

            bill.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteBillAsync(int id)
        {
            var bill = await _context.Bills
                .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);

            if (bill == null)
                return false;

            bill.IsDeleted = true;
            bill.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> BillExistsAsync(int id)
        {
            return await _context.Bills
                .AnyAsync(b => b.Id == id && !b.IsDeleted);
        }

        public async Task<IEnumerable<BillDto>> GetOverdueBillsAsync()
        {
            var bills = await _context.Bills
                .Include(b => b.Patient)
                .Include(b => b.BillItems)
                .Where(b => !b.IsDeleted && 
                           b.Status != "Paid" && 
                           b.DueDate < DateTime.UtcNow)
                .OrderBy(b => b.DueDate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<BillDto>>(bills);
        }

        public async Task<decimal> GetTotalOutstandingAmountAsync()
        {
            return await _context.Bills
                .Where(b => !b.IsDeleted && b.Status != "Paid")
                .SumAsync(b => b.RemainingAmount);
        }

        private async Task<string> GenerateBillNumberAsync()
        {
            var year = DateTime.UtcNow.Year;
            var month = DateTime.UtcNow.Month.ToString("D2");
            var day = DateTime.UtcNow.Day.ToString("D2");

            var lastBill = await _context.Bills
                .Where(b => b.BillNumber.StartsWith($"BILL{year}{month}{day}"))
                .OrderByDescending(b => b.BillNumber)
                .FirstOrDefaultAsync();

            int sequence = 1;
            if (lastBill != null)
            {
                var lastSequence = lastBill.BillNumber.Substring(12); // After BILLYYYYMMDD
                if (int.TryParse(lastSequence, out int lastSeq))
                {
                    sequence = lastSeq + 1;
                }
            }

            return $"BILL{year}{month}{day}{sequence:D4}";
        }
    }
}
