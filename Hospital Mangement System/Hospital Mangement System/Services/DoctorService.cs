using Microsoft.EntityFrameworkCore;
using Hospital_Management_System.Data;
using Hospital_Management_System.DTOs;
using Hospital_Management_System.Models;
using AutoMapper;

namespace Hospital_Management_System.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly HospitalDbContext _context;
        private readonly IMapper _mapper;

        public DoctorService(HospitalDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DoctorDto>> GetAllDoctorsAsync()
        {
            var doctors = await _context.Doctors
                .Include(d => d.Department)
                .Where(d => !d.IsDeleted)
                .OrderBy(d => d.FirstName)
                .ThenBy(d => d.LastName)
                .ToListAsync();

            return _mapper.Map<IEnumerable<DoctorDto>>(doctors);
        }

        public async Task<DoctorDto?> GetDoctorByIdAsync(int id)
        {
            var doctor = await _context.Doctors
                .Include(d => d.Department)
                .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);

            return doctor == null ? null : _mapper.Map<DoctorDto>(doctor);
        }

        public async Task<DoctorDto?> GetDoctorByNationalIdAsync(string nationalId)
        {
            var doctor = await _context.Doctors
                .Include(d => d.Department)
                .FirstOrDefaultAsync(d => d.NationalId == nationalId && !d.IsDeleted);

            return doctor == null ? null : _mapper.Map<DoctorDto>(doctor);
        }

        public async Task<DoctorDto?> GetDoctorByLicenseNumberAsync(string licenseNumber)
        {
            var doctor = await _context.Doctors
                .Include(d => d.Department)
                .FirstOrDefaultAsync(d => d.LicenseNumber == licenseNumber && !d.IsDeleted);

            return doctor == null ? null : _mapper.Map<DoctorDto>(doctor);
        }

        public async Task<IEnumerable<DoctorDto>> GetDoctorsByDepartmentAsync(int departmentId)
        {
            var doctors = await _context.Doctors
                .Include(d => d.Department)
                .Where(d => d.DepartmentId == departmentId && !d.IsDeleted)
                .OrderBy(d => d.FirstName)
                .ThenBy(d => d.LastName)
                .ToListAsync();

            return _mapper.Map<IEnumerable<DoctorDto>>(doctors);
        }

        public async Task<IEnumerable<DoctorDto>> GetAvailableDoctorsAsync()
        {
            var doctors = await _context.Doctors
                .Include(d => d.Department)
                .Where(d => !d.IsDeleted && d.IsAvailable && d.IsActive)
                .OrderBy(d => d.FirstName)
                .ThenBy(d => d.LastName)
                .ToListAsync();

            return _mapper.Map<IEnumerable<DoctorDto>>(doctors);
        }

        public async Task<DoctorDto> CreateDoctorAsync(CreateDoctorDto createDoctorDto)
        {
            // Check if doctor already exists
            if (await DoctorExistsByNationalIdAsync(createDoctorDto.NationalId))
            {
                throw new InvalidOperationException("Doctor with this National ID already exists.");
            }

            if (await DoctorExistsByEmailAsync(createDoctorDto.Email))
            {
                throw new InvalidOperationException("Doctor with this email already exists.");
            }

            if (await DoctorExistsByLicenseNumberAsync(createDoctorDto.LicenseNumber))
            {
                throw new InvalidOperationException("Doctor with this license number already exists.");
            }

            // Verify department exists
            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.Id == createDoctorDto.DepartmentId && d.IsActive);

            if (department == null)
            {
                throw new InvalidOperationException("Department not found or inactive.");
            }

            var doctor = _mapper.Map<Doctor>(createDoctorDto);
            doctor.CreatedAt = DateTime.UtcNow;

            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();

            // Load department for response
            await _context.Entry(doctor)
                .Reference(d => d.Department)
                .LoadAsync();

            return _mapper.Map<DoctorDto>(doctor);
        }

        public async Task<DoctorDto?> UpdateDoctorAsync(int id, UpdateDoctorDto updateDoctorDto)
        {
            var doctor = await _context.Doctors
                .Include(d => d.Department)
                .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);

            if (doctor == null)
                return null;

            // Check email uniqueness if email is being updated
            if (!string.IsNullOrEmpty(updateDoctorDto.Email) && 
                updateDoctorDto.Email != doctor.Email &&
                await DoctorExistsByEmailAsync(updateDoctorDto.Email))
            {
                throw new InvalidOperationException("Doctor with this email already exists.");
            }

            // Check license number uniqueness if license number is being updated
            if (!string.IsNullOrEmpty(updateDoctorDto.LicenseNumber) && 
                updateDoctorDto.LicenseNumber != doctor.LicenseNumber &&
                await DoctorExistsByLicenseNumberAsync(updateDoctorDto.LicenseNumber))
            {
                throw new InvalidOperationException("Doctor with this license number already exists.");
            }

            // Verify department exists if department is being updated
            if (updateDoctorDto.DepartmentId.HasValue)
            {
                var department = await _context.Departments
                    .FirstOrDefaultAsync(d => d.Id == updateDoctorDto.DepartmentId.Value && d.IsActive);

                if (department == null)
                {
                    throw new InvalidOperationException("Department not found or inactive.");
                }
            }

            // Update only provided fields
            if (!string.IsNullOrEmpty(updateDoctorDto.FirstName))
                doctor.FirstName = updateDoctorDto.FirstName;

            if (!string.IsNullOrEmpty(updateDoctorDto.LastName))
                doctor.LastName = updateDoctorDto.LastName;

            if (!string.IsNullOrEmpty(updateDoctorDto.Email))
                doctor.Email = updateDoctorDto.Email;

            if (!string.IsNullOrEmpty(updateDoctorDto.PhoneNumber))
                doctor.PhoneNumber = updateDoctorDto.PhoneNumber;

            if (updateDoctorDto.PhoneNumber2 != null)
                doctor.PhoneNumber2 = updateDoctorDto.PhoneNumber2;

            if (updateDoctorDto.DateOfBirth.HasValue)
                doctor.DateOfBirth = updateDoctorDto.DateOfBirth.Value;

            if (!string.IsNullOrEmpty(updateDoctorDto.Gender))
                doctor.Gender = updateDoctorDto.Gender;

            if (updateDoctorDto.Address != null)
                doctor.Address = updateDoctorDto.Address;

            if (!string.IsNullOrEmpty(updateDoctorDto.LicenseNumber))
                doctor.LicenseNumber = updateDoctorDto.LicenseNumber;

            if (!string.IsNullOrEmpty(updateDoctorDto.Specialization))
                doctor.Specialization = updateDoctorDto.Specialization;

            if (updateDoctorDto.SubSpecialization != null)
                doctor.SubSpecialization = updateDoctorDto.SubSpecialization;

            if (updateDoctorDto.YearsOfExperience.HasValue)
                doctor.YearsOfExperience = updateDoctorDto.YearsOfExperience.Value;

            if (updateDoctorDto.Education != null)
                doctor.Education = updateDoctorDto.Education;

            if (updateDoctorDto.Certifications != null)
                doctor.Certifications = updateDoctorDto.Certifications;

            if (updateDoctorDto.Languages != null)
                doctor.Languages = updateDoctorDto.Languages;

            if (updateDoctorDto.ConsultationFee.HasValue)
                doctor.ConsultationFee = updateDoctorDto.ConsultationFee.Value;

            if (updateDoctorDto.WorkingHoursStart.HasValue)
                doctor.WorkingHoursStart = updateDoctorDto.WorkingHoursStart.Value;

            if (updateDoctorDto.WorkingHoursEnd.HasValue)
                doctor.WorkingHoursEnd = updateDoctorDto.WorkingHoursEnd.Value;

            if (updateDoctorDto.IsAvailable.HasValue)
                doctor.IsAvailable = updateDoctorDto.IsAvailable.Value;

            if (updateDoctorDto.IsActive.HasValue)
                doctor.IsActive = updateDoctorDto.IsActive.Value;

            if (updateDoctorDto.DepartmentId.HasValue)
                doctor.DepartmentId = updateDoctorDto.DepartmentId.Value;

            doctor.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Reload department for response
            await _context.Entry(doctor)
                .Reference(d => d.Department)
                .LoadAsync();

            return _mapper.Map<DoctorDto>(doctor);
        }

        public async Task<bool> DeleteDoctorAsync(int id)
        {
            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);

            if (doctor == null)
                return false;

            doctor.IsDeleted = true;
            doctor.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<DoctorDto>> SearchDoctorsAsync(string searchTerm)
        {
            var doctors = await _context.Doctors
                .Include(d => d.Department)
                .Where(d => !d.IsDeleted && (
                    d.FirstName.Contains(searchTerm) ||
                    d.LastName.Contains(searchTerm) ||
                    d.NationalId.Contains(searchTerm) ||
                    d.Email.Contains(searchTerm) ||
                    d.Specialization.Contains(searchTerm) ||
                    d.LicenseNumber.Contains(searchTerm)
                ))
                .OrderBy(d => d.FirstName)
                .ThenBy(d => d.LastName)
                .ToListAsync();

            return _mapper.Map<IEnumerable<DoctorDto>>(doctors);
        }

        public async Task<bool> DoctorExistsAsync(int id)
        {
            return await _context.Doctors
                .AnyAsync(d => d.Id == id && !d.IsDeleted);
        }

        public async Task<bool> DoctorExistsByNationalIdAsync(string nationalId)
        {
            return await _context.Doctors
                .AnyAsync(d => d.NationalId == nationalId && !d.IsDeleted);
        }

        public async Task<bool> DoctorExistsByEmailAsync(string email)
        {
            return await _context.Doctors
                .AnyAsync(d => d.Email == email && !d.IsDeleted);
        }

        public async Task<bool> DoctorExistsByLicenseNumberAsync(string licenseNumber)
        {
            return await _context.Doctors
                .AnyAsync(d => d.LicenseNumber == licenseNumber && !d.IsDeleted);
        }
    }
}
