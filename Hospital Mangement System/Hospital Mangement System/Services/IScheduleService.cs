using Hospital_Management_System.DTOs;

namespace Hospital_Management_System.Services
{
    public interface IScheduleService
    {
        Task<IEnumerable<ScheduleDto>> GetAllSchedulesAsync();
        Task<ScheduleDto?> GetScheduleByIdAsync(int id);
        Task<IEnumerable<ScheduleDto>> GetSchedulesByDoctorAsync(int doctorId);
        Task<ScheduleDto> CreateScheduleAsync(CreateScheduleDto createScheduleDto);
        Task<ScheduleDto?> UpdateScheduleAsync(int id, UpdateScheduleDto updateScheduleDto);
        Task<bool> DeleteScheduleAsync(int id);
        Task<bool> ScheduleExistsAsync(int id);
    }
}
