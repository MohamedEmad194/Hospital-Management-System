using Microsoft.EntityFrameworkCore;
using Hospital_Management_System.Data;
using Hospital_Management_System.DTOs;
using Hospital_Management_System.Models;
using AutoMapper;

namespace Hospital_Management_System.Services
{
    public class MedicalRecordService : IMedicalRecordService
    {
        private readonly HospitalDbContext _context;
        private readonly IMapper _mapper;

        public MedicalRecordService(HospitalDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MedicalRecordDto>> GetAllMedicalRecordsAsync()
        {
            var records = await _context.MedicalRecords
                .Include(mr => mr.Patient)
                .Include(mr => mr.Doctor)
                .Where(mr => !mr.IsDeleted)
                .OrderByDescending(mr => mr.RecordDate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<MedicalRecordDto>>(records);
        }

        public async Task<MedicalRecordDto?> GetMedicalRecordByIdAsync(int id)
        {
            var record = await _context.MedicalRecords
                .Include(mr => mr.Patient)
                .Include(mr => mr.Doctor)
                .FirstOrDefaultAsync(mr => mr.Id == id && !mr.IsDeleted);

            return record == null ? null : _mapper.Map<MedicalRecordDto>(record);
        }

        public async Task<IEnumerable<MedicalRecordDto>> GetMedicalRecordsByPatientAsync(int patientId)
        {
            var records = await _context.MedicalRecords
                .Include(mr => mr.Patient)
                .Include(mr => mr.Doctor)
                .Where(mr => mr.PatientId == patientId && !mr.IsDeleted)
                .OrderByDescending(mr => mr.RecordDate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<MedicalRecordDto>>(records);
        }

        public async Task<IEnumerable<MedicalRecordDto>> GetMedicalRecordsByDoctorAsync(int doctorId)
        {
            var records = await _context.MedicalRecords
                .Include(mr => mr.Patient)
                .Include(mr => mr.Doctor)
                .Where(mr => mr.DoctorId == doctorId && !mr.IsDeleted)
                .OrderByDescending(mr => mr.RecordDate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<MedicalRecordDto>>(records);
        }

        public async Task<MedicalRecordDto> CreateMedicalRecordAsync(CreateMedicalRecordDto createMedicalRecordDto)
        {
            // Verify patient exists
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.Id == createMedicalRecordDto.PatientId && !p.IsDeleted);

            if (patient == null)
                throw new InvalidOperationException("Patient not found.");

            // Verify doctor exists
            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.Id == createMedicalRecordDto.DoctorId && !d.IsDeleted);

            if (doctor == null)
                throw new InvalidOperationException("Doctor not found.");

            var record = _mapper.Map<MedicalRecord>(createMedicalRecordDto);
            record.CreatedAt = DateTime.UtcNow;

            _context.MedicalRecords.Add(record);
            await _context.SaveChangesAsync();

            // Load related entities for response
            await _context.Entry(record)
                .Reference(mr => mr.Patient)
                .LoadAsync();

            await _context.Entry(record)
                .Reference(mr => mr.Doctor)
                .LoadAsync();

            return _mapper.Map<MedicalRecordDto>(record);
        }

        public async Task<MedicalRecordDto?> UpdateMedicalRecordAsync(int id, UpdateMedicalRecordDto updateMedicalRecordDto)
        {
            var record = await _context.MedicalRecords
                .Include(mr => mr.Patient)
                .Include(mr => mr.Doctor)
                .FirstOrDefaultAsync(mr => mr.Id == id && !mr.IsDeleted);

            if (record == null)
                return null;

            // Update only provided fields
            if (updateMedicalRecordDto.RecordDate.HasValue)
                record.RecordDate = updateMedicalRecordDto.RecordDate.Value;

            if (!string.IsNullOrEmpty(updateMedicalRecordDto.RecordType))
                record.RecordType = updateMedicalRecordDto.RecordType;

            if (updateMedicalRecordDto.Symptoms != null)
                record.Symptoms = updateMedicalRecordDto.Symptoms;

            if (updateMedicalRecordDto.Diagnosis != null)
                record.Diagnosis = updateMedicalRecordDto.Diagnosis;

            if (updateMedicalRecordDto.Treatment != null)
                record.Treatment = updateMedicalRecordDto.Treatment;

            if (updateMedicalRecordDto.Prescription != null)
                record.Prescription = updateMedicalRecordDto.Prescription;

            if (updateMedicalRecordDto.Notes != null)
                record.Notes = updateMedicalRecordDto.Notes;

            if (updateMedicalRecordDto.LabResults != null)
                record.LabResults = updateMedicalRecordDto.LabResults;

            if (updateMedicalRecordDto.ImagingResults != null)
                record.ImagingResults = updateMedicalRecordDto.ImagingResults;

            if (updateMedicalRecordDto.VitalSigns != null)
                record.VitalSigns = updateMedicalRecordDto.VitalSigns;

            if (updateMedicalRecordDto.BloodPressure != null)
                record.BloodPressure = updateMedicalRecordDto.BloodPressure;

            if (updateMedicalRecordDto.Temperature != null)
                record.Temperature = updateMedicalRecordDto.Temperature;

            if (updateMedicalRecordDto.HeartRate != null)
                record.HeartRate = updateMedicalRecordDto.HeartRate;

            if (updateMedicalRecordDto.Weight != null)
                record.Weight = updateMedicalRecordDto.Weight;

            if (updateMedicalRecordDto.Height != null)
                record.Height = updateMedicalRecordDto.Height;

            record.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return _mapper.Map<MedicalRecordDto>(record);
        }

        public async Task<bool> DeleteMedicalRecordAsync(int id)
        {
            var record = await _context.MedicalRecords
                .FirstOrDefaultAsync(mr => mr.Id == id && !mr.IsDeleted);

            if (record == null)
                return false;

            record.IsDeleted = true;
            record.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MedicalRecordExistsAsync(int id)
        {
            return await _context.MedicalRecords
                .AnyAsync(mr => mr.Id == id && !mr.IsDeleted);
        }
    }
}
