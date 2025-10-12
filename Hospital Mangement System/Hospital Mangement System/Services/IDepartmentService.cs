using Hospital_Management_System.DTOs;

namespace Hospital_Management_System.Services
{
    public interface IDepartmentService
    {
        Task<IEnumerable<DepartmentDto>> GetAllDepartmentsAsync();
        Task<DepartmentDto?> GetDepartmentByIdAsync(int id);
        Task<DepartmentDto> CreateDepartmentAsync(CreateDepartmentDto createDepartmentDto);
        Task<DepartmentDto?> UpdateDepartmentAsync(int id, UpdateDepartmentDto updateDepartmentDto);
        Task<bool> DeleteDepartmentAsync(int id);
        Task<bool> DepartmentExistsAsync(int id);
    }
}
