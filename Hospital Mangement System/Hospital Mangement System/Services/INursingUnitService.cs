using Hospital_Management_System.DTOs;
using Hospital_Management_System.Models;

namespace Hospital_Management_System.Services
{
    public interface INursingUnitService
    {
        Task<IEnumerable<NursingUnitDto>> GetAllNursingUnitsAsync();
        Task<NursingUnitDto?> GetNursingUnitByIdAsync(int id);
        Task<NursingUnitDto?> GetNursingUnitByUnitIdAsync(string unitId);
        Task<IEnumerable<NursingUnitDto>> SearchNursingUnitsAsync(string searchTerm);
        Task<NursingUnitDto> CreateNursingUnitAsync(CreateNursingUnitDto createDto);
        Task<NursingUnitDto?> UpdateNursingUnitAsync(int id, UpdateNursingUnitDto updateDto);
        Task<bool> DeleteNursingUnitAsync(int id);
        Task<bool> NursingUnitExistsAsync(int id);
        Task<bool> SeedNursingUnitsAsync();
    }
}

