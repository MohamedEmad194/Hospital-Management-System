using Hospital_Management_System.DTOs;

namespace Hospital_Management_System.Services
{
    public interface IMedicineService
    {
        Task<IEnumerable<MedicineDto>> GetAllMedicinesAsync();
        Task<MedicineDto?> GetMedicineByIdAsync(int id);
        Task<IEnumerable<MedicineDto>> SearchMedicinesAsync(string searchTerm);
        Task<MedicineDto> CreateMedicineAsync(CreateMedicineDto createMedicineDto);
        Task<MedicineDto?> UpdateMedicineAsync(int id, UpdateMedicineDto updateMedicineDto);
        Task<bool> DeleteMedicineAsync(int id);
        Task<bool> MedicineExistsAsync(int id);
        Task<IEnumerable<MedicineDto>> GetLowStockMedicinesAsync();
    }
}
