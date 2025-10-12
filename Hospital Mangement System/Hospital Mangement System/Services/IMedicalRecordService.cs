using Hospital_Management_System.DTOs;

namespace Hospital_Management_System.Services
{
    public interface IMedicalRecordService
    {
        Task<IEnumerable<MedicalRecordDto>> GetAllMedicalRecordsAsync();
        Task<MedicalRecordDto?> GetMedicalRecordByIdAsync(int id);
        Task<IEnumerable<MedicalRecordDto>> GetMedicalRecordsByPatientAsync(int patientId);
        Task<IEnumerable<MedicalRecordDto>> GetMedicalRecordsByDoctorAsync(int doctorId);
        Task<MedicalRecordDto> CreateMedicalRecordAsync(CreateMedicalRecordDto createMedicalRecordDto);
        Task<MedicalRecordDto?> UpdateMedicalRecordAsync(int id, UpdateMedicalRecordDto updateMedicalRecordDto);
        Task<bool> DeleteMedicalRecordAsync(int id);
        Task<bool> MedicalRecordExistsAsync(int id);
    }
}
