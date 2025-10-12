using Hospital_Management_System.DTOs;

namespace Hospital_Management_System.Services
{
    public interface IDoctorService
    {
        Task<IEnumerable<DoctorDto>> GetAllDoctorsAsync();
        Task<DoctorDto?> GetDoctorByIdAsync(int id);
        Task<DoctorDto?> GetDoctorByNationalIdAsync(string nationalId);
        Task<DoctorDto?> GetDoctorByLicenseNumberAsync(string licenseNumber);
        Task<IEnumerable<DoctorDto>> GetDoctorsByDepartmentAsync(int departmentId);
        Task<IEnumerable<DoctorDto>> GetAvailableDoctorsAsync();
        Task<DoctorDto> CreateDoctorAsync(CreateDoctorDto createDoctorDto);
        Task<DoctorDto?> UpdateDoctorAsync(int id, UpdateDoctorDto updateDoctorDto);
        Task<bool> DeleteDoctorAsync(int id);
        Task<IEnumerable<DoctorDto>> SearchDoctorsAsync(string searchTerm);
        Task<bool> DoctorExistsAsync(int id);
        Task<bool> DoctorExistsByNationalIdAsync(string nationalId);
        Task<bool> DoctorExistsByEmailAsync(string email);
        Task<bool> DoctorExistsByLicenseNumberAsync(string licenseNumber);
    }
}
