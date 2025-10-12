using Microsoft.EntityFrameworkCore;
using Hospital_Management_System.Data;
using Hospital_Management_System.DTOs;
using Hospital_Management_System.Models;
using AutoMapper;

namespace Hospital_Management_System.Services
{
    public class PatientService : IPatientService
    {
        private readonly HospitalDbContext _context;
        private readonly IMapper _mapper;

        public PatientService(HospitalDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PatientDto>> GetAllPatientsAsync()
        {
            var patients = await _context.Patients
                .Where(p => !p.IsDeleted)
                .OrderBy(p => p.FirstName)
                .ThenBy(p => p.LastName)
                .ToListAsync();

            return _mapper.Map<IEnumerable<PatientDto>>(patients);
        }

        public async Task<PatientDto?> GetPatientByIdAsync(int id)
        {
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

            return patient == null ? null : _mapper.Map<PatientDto>(patient);
        }

        public async Task<PatientDto?> GetPatientByNationalIdAsync(string nationalId)
        {
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.NationalId == nationalId && !p.IsDeleted);

            return patient == null ? null : _mapper.Map<PatientDto>(patient);
        }

        public async Task<PatientDto> CreatePatientAsync(CreatePatientDto createPatientDto)
        {
            // Check if patient already exists
            if (await PatientExistsByNationalIdAsync(createPatientDto.NationalId))
            {
                throw new InvalidOperationException("Patient with this National ID already exists.");
            }

            if (await PatientExistsByEmailAsync(createPatientDto.Email))
            {
                throw new InvalidOperationException("Patient with this email already exists.");
            }

            var patient = _mapper.Map<Patient>(createPatientDto);
            patient.CreatedAt = DateTime.UtcNow;

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            return _mapper.Map<PatientDto>(patient);
        }

        public async Task<PatientDto?> UpdatePatientAsync(int id, UpdatePatientDto updatePatientDto)
        {
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

            if (patient == null)
                return null;

            // Check email uniqueness if email is being updated
            if (!string.IsNullOrEmpty(updatePatientDto.Email) && 
                updatePatientDto.Email != patient.Email &&
                await PatientExistsByEmailAsync(updatePatientDto.Email))
            {
                throw new InvalidOperationException("Patient with this email already exists.");
            }

            // Update only provided fields
            if (!string.IsNullOrEmpty(updatePatientDto.FirstName))
                patient.FirstName = updatePatientDto.FirstName;

            if (!string.IsNullOrEmpty(updatePatientDto.LastName))
                patient.LastName = updatePatientDto.LastName;

            if (!string.IsNullOrEmpty(updatePatientDto.Email))
                patient.Email = updatePatientDto.Email;

            if (!string.IsNullOrEmpty(updatePatientDto.PhoneNumber))
                patient.PhoneNumber = updatePatientDto.PhoneNumber;

            if (updatePatientDto.PhoneNumber2 != null)
                patient.PhoneNumber2 = updatePatientDto.PhoneNumber2;

            if (updatePatientDto.DateOfBirth.HasValue)
                patient.DateOfBirth = updatePatientDto.DateOfBirth.Value;

            if (!string.IsNullOrEmpty(updatePatientDto.Gender))
                patient.Gender = updatePatientDto.Gender;

            if (updatePatientDto.Address != null)
                patient.Address = updatePatientDto.Address;

            if (updatePatientDto.EmergencyContactName != null)
                patient.EmergencyContactName = updatePatientDto.EmergencyContactName;

            if (updatePatientDto.EmergencyContactPhone != null)
                patient.EmergencyContactPhone = updatePatientDto.EmergencyContactPhone;

            if (updatePatientDto.InsuranceProvider != null)
                patient.InsuranceProvider = updatePatientDto.InsuranceProvider;

            if (updatePatientDto.InsuranceNumber != null)
                patient.InsuranceNumber = updatePatientDto.InsuranceNumber;

            if (updatePatientDto.MedicalHistory != null)
                patient.MedicalHistory = updatePatientDto.MedicalHistory;

            if (updatePatientDto.Allergies != null)
                patient.Allergies = updatePatientDto.Allergies;

            if (updatePatientDto.CurrentMedications != null)
                patient.CurrentMedications = updatePatientDto.CurrentMedications;

            if (updatePatientDto.IsActive.HasValue)
                patient.IsActive = updatePatientDto.IsActive.Value;

            patient.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return _mapper.Map<PatientDto>(patient);
        }

        public async Task<bool> DeletePatientAsync(int id)
        {
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

            if (patient == null)
                return false;

            patient.IsDeleted = true;
            patient.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<PatientDto>> SearchPatientsAsync(string searchTerm)
        {
            var patients = await _context.Patients
                .Where(p => !p.IsDeleted && (
                    p.FirstName.Contains(searchTerm) ||
                    p.LastName.Contains(searchTerm) ||
                    p.NationalId.Contains(searchTerm) ||
                    p.Email.Contains(searchTerm) ||
                    p.PhoneNumber.Contains(searchTerm)
                ))
                .OrderBy(p => p.FirstName)
                .ThenBy(p => p.LastName)
                .ToListAsync();

            return _mapper.Map<IEnumerable<PatientDto>>(patients);
        }

        public async Task<bool> PatientExistsAsync(int id)
        {
            return await _context.Patients
                .AnyAsync(p => p.Id == id && !p.IsDeleted);
        }

        public async Task<bool> PatientExistsByNationalIdAsync(string nationalId)
        {
            return await _context.Patients
                .AnyAsync(p => p.NationalId == nationalId && !p.IsDeleted);
        }

        public async Task<bool> PatientExistsByEmailAsync(string email)
        {
            return await _context.Patients
                .AnyAsync(p => p.Email == email && !p.IsDeleted);
        }
    }
}
