using System.ComponentModel.DataAnnotations;

namespace Hospital_Management_System.DTOs
{
    public class ScheduleDto
    {
        public int Id { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string? Notes { get; set; }
        public bool IsAvailable { get; set; }
        public int DoctorId { get; set; }
        public string? DoctorName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateScheduleDto
    {
        [Required]
        public DayOfWeek DayOfWeek { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        [StringLength(100)]
        public string? Notes { get; set; }

        [Required]
        public int DoctorId { get; set; }
    }

    public class UpdateScheduleDto
    {
        public DayOfWeek? DayOfWeek { get; set; }

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        [StringLength(100)]
        public string? Notes { get; set; }

        public bool? IsAvailable { get; set; }
    }
}
