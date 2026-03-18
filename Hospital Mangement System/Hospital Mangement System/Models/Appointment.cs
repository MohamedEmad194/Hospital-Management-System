using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital_Management_System.Models
{
    public class Appointment : BaseEntity
    {
        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        public TimeSpan AppointmentTime { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "Scheduled"; // Scheduled, Confirmed, Completed, Cancelled, NoShow

        [StringLength(500)]
        public string? Reason { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        [StringLength(1000)]
        public string? Diagnosis { get; set; }

        [StringLength(500)]
        public string? Treatment { get; set; }

        public decimal? ConsultationFee { get; set; }

        public bool IsFollowUp { get; set; } = false;

        // Foreign Keys
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public int? RoomId { get; set; }

        // Navigation properties
        [ForeignKey("PatientId")]
        public virtual Patient? Patient { get; set; }

        [ForeignKey("DoctorId")]
        public virtual Doctor? Doctor { get; set; }

        [ForeignKey("RoomId")]
        public virtual Room? Room { get; set; }
    }
}
