using Microsoft.EntityFrameworkCore;
using Hospital_Management_System.Data;
using Hospital_Management_System.DTOs;
using Hospital_Management_System.Models;
using AutoMapper;

namespace Hospital_Management_System.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly HospitalDbContext _context;
        private readonly IMapper _mapper;

        public AppointmentService(HospitalDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AppointmentDto>> GetAllAppointmentsAsync()
        {
            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Room)
                .Where(a => !a.IsDeleted)
                .OrderByDescending(a => a.AppointmentDate)
                .ThenBy(a => a.AppointmentTime)
                .ToListAsync();

            return _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
        }

        public async Task<AppointmentDto?> GetAppointmentByIdAsync(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Room)
                .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);

            return appointment == null ? null : _mapper.Map<AppointmentDto>(appointment);
        }

        public async Task<IEnumerable<AppointmentDto>> GetAppointmentsByPatientAsync(int patientId)
        {
            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Room)
                .Where(a => a.PatientId == patientId && !a.IsDeleted)
                .OrderByDescending(a => a.AppointmentDate)
                .ThenBy(a => a.AppointmentTime)
                .ToListAsync();

            return _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
        }

        public async Task<IEnumerable<AppointmentDto>> GetAppointmentsByDoctorAsync(int doctorId)
        {
            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Room)
                .Where(a => a.DoctorId == doctorId && !a.IsDeleted)
                .OrderByDescending(a => a.AppointmentDate)
                .ThenBy(a => a.AppointmentTime)
                .ToListAsync();

            return _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
        }

        public async Task<IEnumerable<AppointmentDto>> GetAppointmentsByDateAsync(DateTime date)
        {
            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Room)
                .Where(a => a.AppointmentDate.Date == date.Date && !a.IsDeleted)
                .OrderBy(a => a.AppointmentTime)
                .ToListAsync();

            return _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
        }

        public async Task<IEnumerable<AppointmentDto>> GetAppointmentsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Room)
                .Where(a => a.AppointmentDate.Date >= startDate.Date && 
                           a.AppointmentDate.Date <= endDate.Date && 
                           !a.IsDeleted)
                .OrderBy(a => a.AppointmentDate)
                .ThenBy(a => a.AppointmentTime)
                .ToListAsync();

            return _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
        }

        public async Task<IEnumerable<AppointmentDto>> SearchAppointmentsAsync(AppointmentSearchDto searchDto)
        {
            var query = _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Room)
                .Where(a => !a.IsDeleted);

            if (searchDto.StartDate.HasValue)
                query = query.Where(a => a.AppointmentDate.Date >= searchDto.StartDate.Value.Date);

            if (searchDto.EndDate.HasValue)
                query = query.Where(a => a.AppointmentDate.Date <= searchDto.EndDate.Value.Date);

            if (searchDto.PatientId.HasValue)
                query = query.Where(a => a.PatientId == searchDto.PatientId.Value);

            if (searchDto.DoctorId.HasValue)
                query = query.Where(a => a.DoctorId == searchDto.DoctorId.Value);

            if (!string.IsNullOrEmpty(searchDto.Status))
                query = query.Where(a => a.Status == searchDto.Status);

            if (searchDto.RoomId.HasValue)
                query = query.Where(a => a.RoomId == searchDto.RoomId.Value);

            var appointments = await query
                .OrderBy(a => a.AppointmentDate)
                .ThenBy(a => a.AppointmentTime)
                .ToListAsync();

            return _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
        }

        public async Task<AppointmentDto> CreateAppointmentAsync(CreateAppointmentDto createAppointmentDto)
        {
            // Verify patient exists
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.Id == createAppointmentDto.PatientId && !p.IsDeleted);

            if (patient == null)
                throw new InvalidOperationException("Patient not found.");

            // Verify doctor exists and is available
            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.Id == createAppointmentDto.DoctorId && !d.IsDeleted && d.IsAvailable);

            if (doctor == null)
                throw new InvalidOperationException("Doctor not found or not available.");

            // Verify room exists and is available (if specified)
            if (createAppointmentDto.RoomId.HasValue)
            {
                var room = await _context.Rooms
                    .FirstOrDefaultAsync(r => r.Id == createAppointmentDto.RoomId.Value && !r.IsDeleted && r.IsAvailable);

                if (room == null)
                    throw new InvalidOperationException("Room not found or not available.");
            }

            // Check if time slot is available
            if (!await IsTimeSlotAvailableAsync(createAppointmentDto.DoctorId, createAppointmentDto.AppointmentDate, createAppointmentDto.AppointmentTime))
            {
                throw new InvalidOperationException("Time slot is not available.");
            }

            var appointment = _mapper.Map<Appointment>(createAppointmentDto);
            appointment.CreatedAt = DateTime.UtcNow;
            appointment.ConsultationFee = doctor.ConsultationFee;

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            // Load related entities for response
            await _context.Entry(appointment)
                .Reference(a => a.Patient)
                .LoadAsync();

            await _context.Entry(appointment)
                .Reference(a => a.Doctor)
                .LoadAsync();

            if (appointment.RoomId.HasValue)
            {
                await _context.Entry(appointment)
                    .Reference(a => a.Room)
                    .LoadAsync();
            }

            return _mapper.Map<AppointmentDto>(appointment);
        }

        public async Task<AppointmentDto?> UpdateAppointmentAsync(int id, UpdateAppointmentDto updateAppointmentDto)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Room)
                .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);

            if (appointment == null)
                return null;

            // Check if time slot is available for updates
            if (updateAppointmentDto.DoctorId.HasValue && 
                updateAppointmentDto.AppointmentDate.HasValue && 
                updateAppointmentDto.AppointmentTime.HasValue)
            {
                if (!await IsTimeSlotAvailableAsync(
                    updateAppointmentDto.DoctorId.Value, 
                    updateAppointmentDto.AppointmentDate.Value, 
                    updateAppointmentDto.AppointmentTime.Value, 
                    id))
                {
                    throw new InvalidOperationException("Time slot is not available.");
                }
            }

            // Update only provided fields
            if (updateAppointmentDto.AppointmentDate.HasValue)
                appointment.AppointmentDate = updateAppointmentDto.AppointmentDate.Value;

            if (updateAppointmentDto.AppointmentTime.HasValue)
                appointment.AppointmentTime = updateAppointmentDto.AppointmentTime.Value;

            if (!string.IsNullOrEmpty(updateAppointmentDto.Status))
                appointment.Status = updateAppointmentDto.Status;

            if (updateAppointmentDto.Reason != null)
                appointment.Reason = updateAppointmentDto.Reason;

            if (updateAppointmentDto.Notes != null)
                appointment.Notes = updateAppointmentDto.Notes;

            if (updateAppointmentDto.Diagnosis != null)
                appointment.Diagnosis = updateAppointmentDto.Diagnosis;

            if (updateAppointmentDto.Treatment != null)
                appointment.Treatment = updateAppointmentDto.Treatment;

            if (updateAppointmentDto.ConsultationFee.HasValue)
                appointment.ConsultationFee = updateAppointmentDto.ConsultationFee.Value;

            if (updateAppointmentDto.IsFollowUp.HasValue)
                appointment.IsFollowUp = updateAppointmentDto.IsFollowUp.Value;

            if (updateAppointmentDto.PatientId.HasValue)
                appointment.PatientId = updateAppointmentDto.PatientId.Value;

            if (updateAppointmentDto.DoctorId.HasValue)
                appointment.DoctorId = updateAppointmentDto.DoctorId.Value;

            if (updateAppointmentDto.RoomId.HasValue)
                appointment.RoomId = updateAppointmentDto.RoomId.Value;

            appointment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return _mapper.Map<AppointmentDto>(appointment);
        }

        public async Task<bool> CancelAppointmentAsync(int id)
        {
            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);

            if (appointment == null)
                return false;

            appointment.Status = "Cancelled";
            appointment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CompleteAppointmentAsync(int id, string diagnosis, string treatment)
        {
            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);

            if (appointment == null)
                return false;

            appointment.Status = "Completed";
            appointment.Diagnosis = diagnosis;
            appointment.Treatment = treatment;
            appointment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAppointmentAsync(int id)
        {
            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);

            if (appointment == null)
                return false;

            appointment.IsDeleted = true;
            appointment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AppointmentExistsAsync(int id)
        {
            return await _context.Appointments
                .AnyAsync(a => a.Id == id && !a.IsDeleted);
        }

        public async Task<bool> IsTimeSlotAvailableAsync(int doctorId, DateTime appointmentDate, TimeSpan appointmentTime, int? excludeAppointmentId = null)
        {
            var query = _context.Appointments
                .Where(a => a.DoctorId == doctorId &&
                           a.AppointmentDate.Date == appointmentDate.Date &&
                           a.AppointmentTime == appointmentTime &&
                           !a.IsDeleted &&
                           a.Status != "Cancelled");

            if (excludeAppointmentId.HasValue)
                query = query.Where(a => a.Id != excludeAppointmentId.Value);

            return !await query.AnyAsync();
        }

        public async Task<IEnumerable<TimeSpan>> GetAvailableTimeSlotsAsync(int doctorId, DateTime appointmentDate)
        {
            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.Id == doctorId && !d.IsDeleted && d.IsAvailable);

            if (doctor == null)
                return Enumerable.Empty<TimeSpan>();

            var bookedSlots = await _context.Appointments
                .Where(a => a.DoctorId == doctorId &&
                           a.AppointmentDate.Date == appointmentDate.Date &&
                           !a.IsDeleted &&
                           a.Status != "Cancelled")
                .Select(a => a.AppointmentTime)
                .ToListAsync();

            var availableSlots = new List<TimeSpan>();
            var startTime = doctor.WorkingHoursStart;
            var endTime = doctor.WorkingHoursEnd;
            var slotDuration = TimeSpan.FromMinutes(30); // 30-minute slots

            for (var time = startTime; time < endTime; time = time.Add(slotDuration))
            {
                if (!bookedSlots.Contains(time))
                {
                    availableSlots.Add(time);
                }
            }

            return availableSlots;
        }
    }
}
