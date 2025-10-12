using Hospital_Management_System.DTOs;

namespace Hospital_Management_System.Services
{
    public interface IAppointmentService
    {
        Task<IEnumerable<AppointmentDto>> GetAllAppointmentsAsync();
        Task<AppointmentDto?> GetAppointmentByIdAsync(int id);
        Task<IEnumerable<AppointmentDto>> GetAppointmentsByPatientAsync(int patientId);
        Task<IEnumerable<AppointmentDto>> GetAppointmentsByDoctorAsync(int doctorId);
        Task<IEnumerable<AppointmentDto>> GetAppointmentsByDateAsync(DateTime date);
        Task<IEnumerable<AppointmentDto>> GetAppointmentsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<AppointmentDto>> SearchAppointmentsAsync(AppointmentSearchDto searchDto);
        Task<AppointmentDto> CreateAppointmentAsync(CreateAppointmentDto createAppointmentDto);
        Task<AppointmentDto?> UpdateAppointmentAsync(int id, UpdateAppointmentDto updateAppointmentDto);
        Task<bool> CancelAppointmentAsync(int id);
        Task<bool> CompleteAppointmentAsync(int id, string diagnosis, string treatment);
        Task<bool> DeleteAppointmentAsync(int id);
        Task<bool> AppointmentExistsAsync(int id);
        Task<bool> IsTimeSlotAvailableAsync(int doctorId, DateTime appointmentDate, TimeSpan appointmentTime, int? excludeAppointmentId = null);
        Task<IEnumerable<TimeSpan>> GetAvailableTimeSlotsAsync(int doctorId, DateTime appointmentDate);
    }
}
