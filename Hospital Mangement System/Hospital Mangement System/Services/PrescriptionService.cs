using Microsoft.EntityFrameworkCore;
using Hospital_Management_System.Data;
using Hospital_Management_System.DTOs;
using Hospital_Management_System.Models;
using AutoMapper;

namespace Hospital_Management_System.Services
{
    public class PrescriptionService : IPrescriptionService
    {
        private readonly HospitalDbContext _context;
        private readonly IMapper _mapper;

        public PrescriptionService(HospitalDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PrescriptionDto>> GetAllPrescriptionsAsync()
        {
            var prescriptions = await _context.Prescriptions
                .Include(p => p.Patient)
                .Include(p => p.Doctor)
                .Include(p => p.PrescriptionItems)
                .Where(p => !p.IsDeleted)
                .OrderByDescending(p => p.PrescriptionDate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<PrescriptionDto>>(prescriptions);
        }

        public async Task<PrescriptionDto?> GetPrescriptionByIdAsync(int id)
        {
            var prescription = await _context.Prescriptions
                .Include(p => p.Patient)
                .Include(p => p.Doctor)
                .Include(p => p.PrescriptionItems)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

            return prescription == null ? null : _mapper.Map<PrescriptionDto>(prescription);
        }

        public async Task<IEnumerable<PrescriptionDto>> GetPrescriptionsByPatientAsync(int patientId)
        {
            var prescriptions = await _context.Prescriptions
                .Include(p => p.Patient)
                .Include(p => p.Doctor)
                .Include(p => p.PrescriptionItems)
                .Where(p => p.PatientId == patientId && !p.IsDeleted)
                .OrderByDescending(p => p.PrescriptionDate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<PrescriptionDto>>(prescriptions);
        }

        public async Task<IEnumerable<PrescriptionDto>> GetPrescriptionsByDoctorAsync(int doctorId)
        {
            var prescriptions = await _context.Prescriptions
                .Include(p => p.Patient)
                .Include(p => p.Doctor)
                .Include(p => p.PrescriptionItems)
                .Where(p => p.DoctorId == doctorId && !p.IsDeleted)
                .OrderByDescending(p => p.PrescriptionDate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<PrescriptionDto>>(prescriptions);
        }

        public async Task<PrescriptionDto> CreatePrescriptionAsync(CreatePrescriptionDto createPrescriptionDto)
        {
            // Verify patient exists
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.Id == createPrescriptionDto.PatientId && !p.IsDeleted);

            if (patient == null)
                throw new InvalidOperationException("Patient not found.");

            // Verify doctor exists
            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.Id == createPrescriptionDto.DoctorId && !d.IsDeleted);

            if (doctor == null)
                throw new InvalidOperationException("Doctor not found.");

            var prescription = new Prescription
            {
                PrescriptionDate = createPrescriptionDto.PrescriptionDate,
                ValidUntil = createPrescriptionDto.ValidUntil,
                Instructions = createPrescriptionDto.Instructions,
                Notes = createPrescriptionDto.Notes,
                PatientId = createPrescriptionDto.PatientId,
                DoctorId = createPrescriptionDto.DoctorId,
                Status = "Active",
                CreatedAt = DateTime.UtcNow
            };

            _context.Prescriptions.Add(prescription);
            await _context.SaveChangesAsync();

            // Add prescription items
            foreach (var itemDto in createPrescriptionDto.PrescriptionItems)
            {
                var prescriptionItem = new PrescriptionItem
                {
                    MedicineName = itemDto.MedicineName,
                    Dosage = itemDto.Dosage,
                    Frequency = itemDto.Frequency,
                    Duration = itemDto.Duration,
                    Instructions = itemDto.Instructions,
                    Quantity = itemDto.Quantity,
                    UnitPrice = itemDto.UnitPrice,
                    TotalPrice = itemDto.Quantity * itemDto.UnitPrice,
                    PrescriptionId = prescription.Id,
                    MedicineId = itemDto.MedicineId,
                    CreatedAt = DateTime.UtcNow
                };

                _context.PrescriptionItems.Add(prescriptionItem);
            }

            await _context.SaveChangesAsync();

            // Load related entities for response
            await _context.Entry(prescription)
                .Reference(p => p.Patient)
                .LoadAsync();

            await _context.Entry(prescription)
                .Reference(p => p.Doctor)
                .LoadAsync();

            await _context.Entry(prescription)
                .Collection(p => p.PrescriptionItems)
                .LoadAsync();

            return _mapper.Map<PrescriptionDto>(prescription);
        }

        public async Task<PrescriptionDto?> UpdatePrescriptionAsync(int id, UpdatePrescriptionDto updatePrescriptionDto)
        {
            var prescription = await _context.Prescriptions
                .Include(p => p.Patient)
                .Include(p => p.Doctor)
                .Include(p => p.PrescriptionItems)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

            if (prescription == null)
                return null;

            // Update only provided fields
            if (updatePrescriptionDto.PrescriptionDate.HasValue)
                prescription.PrescriptionDate = updatePrescriptionDto.PrescriptionDate.Value;

            if (updatePrescriptionDto.ValidUntil.HasValue)
                prescription.ValidUntil = updatePrescriptionDto.ValidUntil.Value;

            if (updatePrescriptionDto.Instructions != null)
                prescription.Instructions = updatePrescriptionDto.Instructions;

            if (updatePrescriptionDto.Notes != null)
                prescription.Notes = updatePrescriptionDto.Notes;

            if (!string.IsNullOrEmpty(updatePrescriptionDto.Status))
                prescription.Status = updatePrescriptionDto.Status;

            prescription.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return _mapper.Map<PrescriptionDto>(prescription);
        }

        public async Task<bool> DispensePrescriptionAsync(int id)
        {
            var prescription = await _context.Prescriptions
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

            if (prescription == null)
                return false;

            prescription.IsDispensed = true;
            prescription.DispensedDate = DateTime.UtcNow;
            prescription.Status = "Dispensed";
            prescription.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeletePrescriptionAsync(int id)
        {
            var prescription = await _context.Prescriptions
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

            if (prescription == null)
                return false;

            prescription.IsDeleted = true;
            prescription.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> PrescriptionExistsAsync(int id)
        {
            return await _context.Prescriptions
                .AnyAsync(p => p.Id == id && !p.IsDeleted);
        }
    }
}
