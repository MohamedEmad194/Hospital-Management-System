using Microsoft.EntityFrameworkCore;
using Hospital_Management_System.Data;
using Hospital_Management_System.DTOs;
using Hospital_Management_System.Models;
using AutoMapper;

namespace Hospital_Management_System.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly HospitalDbContext _context;
        private readonly IMapper _mapper;

        public DepartmentService(HospitalDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DepartmentDto>> GetAllDepartmentsAsync()
        {
            var departments = await _context.Departments
                .Where(d => !d.IsDeleted)
                .OrderBy(d => d.Name)
                .ToListAsync();

            if (departments.Count == 0)
                return Enumerable.Empty<DepartmentDto>();

            var departmentIds = departments.Select(d => d.Id).ToList();

            // Batch all counts into 3 grouped queries instead of N×3 individual round-trips
            var doctorCounts = await _context.Doctors
                .Where(d => departmentIds.Contains(d.DepartmentId) && !d.IsDeleted)
                .GroupBy(d => d.DepartmentId)
                .Select(g => new { DepartmentId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.DepartmentId, x => x.Count);

            var roomCounts = await _context.Rooms
                .Where(r => departmentIds.Contains(r.DepartmentId) && !r.IsDeleted)
                .GroupBy(r => r.DepartmentId)
                .Select(g => new { DepartmentId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.DepartmentId, x => x.Count);

            var staffCounts = await _context.Staff
                .Where(s => departmentIds.Contains(s.DepartmentId) && !s.IsDeleted)
                .GroupBy(s => s.DepartmentId)
                .Select(g => new { DepartmentId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.DepartmentId, x => x.Count);

            return departments.Select(department =>
            {
                var dto = _mapper.Map<DepartmentDto>(department);
                dto.DoctorCount = doctorCounts.TryGetValue(department.Id, out var dc) ? dc : 0;
                dto.RoomCount = roomCounts.TryGetValue(department.Id, out var rc) ? rc : 0;
                dto.StaffCount = staffCounts.TryGetValue(department.Id, out var sc) ? sc : 0;
                return dto;
            }).ToList();
        }

        public async Task<DepartmentDto?> GetDepartmentByIdAsync(int id)
        {
            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);

            if (department == null)
                return null;

            var dto = _mapper.Map<DepartmentDto>(department);
            
            // Get counts
            dto.DoctorCount = await _context.Doctors
                .CountAsync(d => d.DepartmentId == department.Id && !d.IsDeleted);
            
            dto.RoomCount = await _context.Rooms
                .CountAsync(r => r.DepartmentId == department.Id && !r.IsDeleted);
            
            dto.StaffCount = await _context.Staff
                .CountAsync(s => s.DepartmentId == department.Id && !s.IsDeleted);

            return dto;
        }

        public async Task<DepartmentDto> CreateDepartmentAsync(CreateDepartmentDto createDepartmentDto)
        {
            var department = _mapper.Map<Department>(createDepartmentDto);
            department.CreatedAt = DateTime.UtcNow;

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            var dto = _mapper.Map<DepartmentDto>(department);
            dto.DoctorCount = 0;
            dto.RoomCount = 0;
            dto.StaffCount = 0;

            return dto;
        }

        public async Task<DepartmentDto?> UpdateDepartmentAsync(int id, UpdateDepartmentDto updateDepartmentDto)
        {
            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);

            if (department == null)
                return null;

            // Update only provided fields
            if (!string.IsNullOrEmpty(updateDepartmentDto.Name))
                department.Name = updateDepartmentDto.Name;

            if (updateDepartmentDto.Description != null)
                department.Description = updateDepartmentDto.Description;

            if (updateDepartmentDto.HeadOfDepartment != null)
                department.HeadOfDepartment = updateDepartmentDto.HeadOfDepartment;

            if (updateDepartmentDto.PhoneNumber != null)
                department.PhoneNumber = updateDepartmentDto.PhoneNumber;

            if (updateDepartmentDto.Location != null)
                department.Location = updateDepartmentDto.Location;

            if (updateDepartmentDto.IsActive.HasValue)
                department.IsActive = updateDepartmentDto.IsActive.Value;

            department.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var dto = _mapper.Map<DepartmentDto>(department);
            
            // Get counts
            dto.DoctorCount = await _context.Doctors
                .CountAsync(d => d.DepartmentId == department.Id && !d.IsDeleted);
            
            dto.RoomCount = await _context.Rooms
                .CountAsync(r => r.DepartmentId == department.Id && !r.IsDeleted);
            
            dto.StaffCount = await _context.Staff
                .CountAsync(s => s.DepartmentId == department.Id && !s.IsDeleted);

            return dto;
        }

        public async Task<bool> DeleteDepartmentAsync(int id)
        {
            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);

            if (department == null)
                return false;

            department.IsDeleted = true;
            department.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DepartmentExistsAsync(int id)
        {
            return await _context.Departments
                .AnyAsync(d => d.Id == id && !d.IsDeleted);
        }
    }
}
