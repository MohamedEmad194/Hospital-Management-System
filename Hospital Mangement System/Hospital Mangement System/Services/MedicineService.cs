using Microsoft.EntityFrameworkCore;
using Hospital_Management_System.Data;
using Hospital_Management_System.DTOs;
using Hospital_Management_System.Models;
using AutoMapper;

namespace Hospital_Management_System.Services
{
    public class MedicineService : IMedicineService
    {
        private readonly HospitalDbContext _context;
        private readonly IMapper _mapper;

        public MedicineService(HospitalDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MedicineDto>> GetAllMedicinesAsync()
        {
            var medicines = await _context.Medicines
                .Where(m => !m.IsDeleted)
                .OrderBy(m => m.Name)
                .ToListAsync();

            return _mapper.Map<IEnumerable<MedicineDto>>(medicines);
        }

        public async Task<MedicineDto?> GetMedicineByIdAsync(int id)
        {
            var medicine = await _context.Medicines
                .FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted);

            return medicine == null ? null : _mapper.Map<MedicineDto>(medicine);
        }

        public async Task<IEnumerable<MedicineDto>> SearchMedicinesAsync(string searchTerm)
        {
            var medicines = await _context.Medicines
                .Where(m => !m.IsDeleted && (
                    m.Name.Contains(searchTerm) ||
                    m.GenericName.Contains(searchTerm) ||
                    m.Manufacturer.Contains(searchTerm)
                ))
                .OrderBy(m => m.Name)
                .ToListAsync();

            return _mapper.Map<IEnumerable<MedicineDto>>(medicines);
        }

        public async Task<MedicineDto> CreateMedicineAsync(CreateMedicineDto createMedicineDto)
        {
            var medicine = _mapper.Map<Medicine>(createMedicineDto);
            medicine.CreatedAt = DateTime.UtcNow;

            _context.Medicines.Add(medicine);
            await _context.SaveChangesAsync();

            return _mapper.Map<MedicineDto>(medicine);
        }

        public async Task<MedicineDto?> UpdateMedicineAsync(int id, UpdateMedicineDto updateMedicineDto)
        {
            var medicine = await _context.Medicines
                .FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted);

            if (medicine == null)
                return null;

            // Update only provided fields
            if (!string.IsNullOrEmpty(updateMedicineDto.Name))
                medicine.Name = updateMedicineDto.Name;

            if (updateMedicineDto.GenericName != null)
                medicine.GenericName = updateMedicineDto.GenericName;

            if (updateMedicineDto.DosageForm != null)
                medicine.DosageForm = updateMedicineDto.DosageForm;

            if (updateMedicineDto.Strength != null)
                medicine.Strength = updateMedicineDto.Strength;

            if (updateMedicineDto.Manufacturer != null)
                medicine.Manufacturer = updateMedicineDto.Manufacturer;

            if (updateMedicineDto.Description != null)
                medicine.Description = updateMedicineDto.Description;

            if (updateMedicineDto.Indications != null)
                medicine.Indications = updateMedicineDto.Indications;

            if (updateMedicineDto.Contraindications != null)
                medicine.Contraindications = updateMedicineDto.Contraindications;

            if (updateMedicineDto.SideEffects != null)
                medicine.SideEffects = updateMedicineDto.SideEffects;

            if (updateMedicineDto.DosageInstructions != null)
                medicine.DosageInstructions = updateMedicineDto.DosageInstructions;

            if (updateMedicineDto.Price.HasValue)
                medicine.Price = updateMedicineDto.Price.Value;

            if (updateMedicineDto.StockQuantity.HasValue)
                medicine.StockQuantity = updateMedicineDto.StockQuantity.Value;

            if (updateMedicineDto.MinimumStockLevel.HasValue)
                medicine.MinimumStockLevel = updateMedicineDto.MinimumStockLevel.Value;

            if (updateMedicineDto.Unit != null)
                medicine.Unit = updateMedicineDto.Unit;

            if (updateMedicineDto.ExpiryDate.HasValue)
                medicine.ExpiryDate = updateMedicineDto.ExpiryDate.Value;

            if (updateMedicineDto.BatchNumber != null)
                medicine.BatchNumber = updateMedicineDto.BatchNumber;

            if (updateMedicineDto.RequiresPrescription.HasValue)
                medicine.RequiresPrescription = updateMedicineDto.RequiresPrescription.Value;

            if (updateMedicineDto.IsActive.HasValue)
                medicine.IsActive = updateMedicineDto.IsActive.Value;

            medicine.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return _mapper.Map<MedicineDto>(medicine);
        }

        public async Task<bool> DeleteMedicineAsync(int id)
        {
            var medicine = await _context.Medicines
                .FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted);

            if (medicine == null)
                return false;

            medicine.IsDeleted = true;
            medicine.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MedicineExistsAsync(int id)
        {
            return await _context.Medicines
                .AnyAsync(m => m.Id == id && !m.IsDeleted);
        }

        public async Task<IEnumerable<MedicineDto>> GetLowStockMedicinesAsync()
        {
            var medicines = await _context.Medicines
                .Where(m => !m.IsDeleted && m.StockQuantity <= m.MinimumStockLevel)
                .OrderBy(m => m.StockQuantity)
                .ToListAsync();

            return _mapper.Map<IEnumerable<MedicineDto>>(medicines);
        }
    }
}
