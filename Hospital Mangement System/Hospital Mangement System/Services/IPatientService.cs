using Hospital_Management_System.DTOs;

namespace Hospital_Management_System.Services
{
    public interface IPatientService
    {
        Task<IEnumerable<PatientDto>> GetAllPatientsAsync();
        Task<PatientDto?> GetPatientByIdAsync(int id);
        Task<PatientDto?> GetPatientByNationalIdAsync(string nationalId);
        Task<PatientDto> CreatePatientAsync(CreatePatientDto createPatientDto);
        Task<PatientDto?> UpdatePatientAsync(int id, UpdatePatientDto updatePatientDto);
        Task<bool> DeletePatientAsync(int id);
        Task<IEnumerable<PatientDto>> SearchPatientsAsync(string searchTerm);
        Task<bool> PatientExistsAsync(int id);
        Task<bool> PatientExistsByNationalIdAsync(string nationalId);
        Task<bool> PatientExistsByEmailAsync(string email);
    }
}
