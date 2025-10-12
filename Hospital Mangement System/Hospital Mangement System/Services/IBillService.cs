using Hospital_Management_System.DTOs;

namespace Hospital_Management_System.Services
{
    public interface IBillService
    {
        Task<IEnumerable<BillDto>> GetAllBillsAsync();
        Task<BillDto?> GetBillByIdAsync(int id);
        Task<IEnumerable<BillDto>> GetBillsByPatientAsync(int patientId);
        Task<BillDto?> CreateBillAsync(CreateBillDto createBillDto);
        Task<BillDto?> UpdateBillAsync(int id, UpdateBillDto updateBillDto);
        Task<bool> ProcessPaymentAsync(int billId, PaymentDto paymentDto);
        Task<bool> DeleteBillAsync(int id);
        Task<bool> BillExistsAsync(int id);
        Task<IEnumerable<BillDto>> GetOverdueBillsAsync();
        Task<decimal> GetTotalOutstandingAmountAsync();
    }
}
