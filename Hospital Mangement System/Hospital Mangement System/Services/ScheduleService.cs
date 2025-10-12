using Microsoft.EntityFrameworkCore;
using Hospital_Management_System.Data;
using Hospital_Management_System.DTOs;
using Hospital_Management_System.Models;
using AutoMapper;

namespace Hospital_Management_System.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly HospitalDbContext _context;
        private readonly IMapper _mapper;

        public ScheduleService(HospitalDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ScheduleDto>> GetAllSchedulesAsync()
        {
            var schedules = await _context.Schedules
                .Include(s => s.Doctor)
                .Where(s => !s.IsDeleted)
                .OrderBy(s => s.DayOfWeek)
                .ThenBy(s => s.StartTime)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ScheduleDto>>(schedules);
        }

        public async Task<ScheduleDto?> GetScheduleByIdAsync(int id)
        {
            var schedule = await _context.Schedules
                .Include(s => s.Doctor)
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);

            return schedule == null ? null : _mapper.Map<ScheduleDto>(schedule);
        }

        public async Task<IEnumerable<ScheduleDto>> GetSchedulesByDoctorAsync(int doctorId)
        {
            var schedules = await _context.Schedules
                .Include(s => s.Doctor)
                .Where(s => s.DoctorId == doctorId && !s.IsDeleted)
                .OrderBy(s => s.DayOfWeek)
                .ThenBy(s => s.StartTime)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ScheduleDto>>(schedules);
        }

        public async Task<ScheduleDto> CreateScheduleAsync(CreateScheduleDto createScheduleDto)
        {
            // Verify doctor exists
            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.Id == createScheduleDto.DoctorId && !d.IsDeleted);

            if (doctor == null)
                throw new InvalidOperationException("Doctor not found.");

            // Check for overlapping schedules
            var overlappingSchedule = await _context.Schedules
                .FirstOrDefaultAsync(s => s.DoctorId == createScheduleDto.DoctorId &&
                                        s.DayOfWeek == createScheduleDto.DayOfWeek &&
                                        !s.IsDeleted &&
                                        s.IsAvailable &&
                                        ((createScheduleDto.StartTime >= s.StartTime && createScheduleDto.StartTime < s.EndTime) ||
                                         (createScheduleDto.EndTime > s.StartTime && createScheduleDto.EndTime <= s.EndTime) ||
                                         (createScheduleDto.StartTime <= s.StartTime && createScheduleDto.EndTime >= s.EndTime)));

            if (overlappingSchedule != null)
                throw new InvalidOperationException("Schedule overlaps with existing schedule.");

            var schedule = _mapper.Map<Schedule>(createScheduleDto);
            schedule.CreatedAt = DateTime.UtcNow;

            _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync();

            // Load doctor for response
            await _context.Entry(schedule)
                .Reference(s => s.Doctor)
                .LoadAsync();

            return _mapper.Map<ScheduleDto>(schedule);
        }

        public async Task<ScheduleDto?> UpdateScheduleAsync(int id, UpdateScheduleDto updateScheduleDto)
        {
            var schedule = await _context.Schedules
                .Include(s => s.Doctor)
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);

            if (schedule == null)
                return null;

            // Check for overlapping schedules if time is being updated
            if (updateScheduleDto.DayOfWeek.HasValue || 
                updateScheduleDto.StartTime.HasValue || 
                updateScheduleDto.EndTime.HasValue)
            {
                var dayOfWeek = updateScheduleDto.DayOfWeek ?? schedule.DayOfWeek;
                var startTime = updateScheduleDto.StartTime ?? schedule.StartTime;
                var endTime = updateScheduleDto.EndTime ?? schedule.EndTime;

                var overlappingSchedule = await _context.Schedules
                    .FirstOrDefaultAsync(s => s.DoctorId == schedule.DoctorId &&
                                            s.DayOfWeek == dayOfWeek &&
                                            s.Id != id &&
                                            !s.IsDeleted &&
                                            s.IsAvailable &&
                                            ((startTime >= s.StartTime && startTime < s.EndTime) ||
                                             (endTime > s.StartTime && endTime <= s.EndTime) ||
                                             (startTime <= s.StartTime && endTime >= s.EndTime)));

                if (overlappingSchedule != null)
                    throw new InvalidOperationException("Schedule overlaps with existing schedule.");
            }

            // Update only provided fields
            if (updateScheduleDto.DayOfWeek.HasValue)
                schedule.DayOfWeek = updateScheduleDto.DayOfWeek.Value;

            if (updateScheduleDto.StartTime.HasValue)
                schedule.StartTime = updateScheduleDto.StartTime.Value;

            if (updateScheduleDto.EndTime.HasValue)
                schedule.EndTime = updateScheduleDto.EndTime.Value;

            if (updateScheduleDto.Notes != null)
                schedule.Notes = updateScheduleDto.Notes;

            if (updateScheduleDto.IsAvailable.HasValue)
                schedule.IsAvailable = updateScheduleDto.IsAvailable.Value;

            schedule.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return _mapper.Map<ScheduleDto>(schedule);
        }

        public async Task<bool> DeleteScheduleAsync(int id)
        {
            var schedule = await _context.Schedules
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);

            if (schedule == null)
                return false;

            schedule.IsDeleted = true;
            schedule.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ScheduleExistsAsync(int id)
        {
            return await _context.Schedules
                .AnyAsync(s => s.Id == id && !s.IsDeleted);
        }
    }
}
