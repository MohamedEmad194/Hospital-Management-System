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

            var departmentDtos = new List<DepartmentDto>();

            foreach (var department in departments)
            {
                var dto = _mapper.Map<DepartmentDto>(department);
                
                // Get counts
                dto.DoctorCount = await _context.Doctors
                    .CountAsync(d => d.DepartmentId == department.Id && !d.IsDeleted);
                
                dto.RoomCount = await _context.Rooms
                    .CountAsync(r => r.DepartmentId == department.Id && !r.IsDeleted);
                
                dto.StaffCount = await _context.Staff
                    .CountAsync(s => s.DepartmentId == department.Id && !s.IsDeleted);

                departmentDtos.Add(dto);
            }

            return departmentDtos;
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
