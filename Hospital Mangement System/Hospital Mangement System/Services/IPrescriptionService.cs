using Hospital_Management_System.DTOs;

namespace Hospital_Management_System.Services
{
    public interface IPrescriptionService
    {
        Task<IEnumerable<PrescriptionDto>> GetAllPrescriptionsAsync();
        Task<PrescriptionDto?> GetPrescriptionByIdAsync(int id);
        Task<IEnumerable<PrescriptionDto>> GetPrescriptionsByPatientAsync(int patientId);
        Task<IEnumerable<PrescriptionDto>> GetPrescriptionsByDoctorAsync(int doctorId);
        Task<PrescriptionDto> CreatePrescriptionAsync(CreatePrescriptionDto createPrescriptionDto);
        Task<PrescriptionDto?> UpdatePrescriptionAsync(int id, UpdatePrescriptionDto updatePrescriptionDto);
        Task<bool> DispensePrescriptionAsync(int id);
        Task<bool> DeletePrescriptionAsync(int id);
        Task<bool> PrescriptionExistsAsync(int id);
    }
}
