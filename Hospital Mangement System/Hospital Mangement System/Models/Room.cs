using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital_Management_System.Models
{
    public class Room : BaseEntity
    {
        [Required]
        [StringLength(20)]
        public string RoomNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string RoomType { get; set; } = string.Empty; // Consultation, Surgery, ICU, Ward, Emergency

        [StringLength(100)]
        public string? Floor { get; set; }

        [StringLength(100)]
        public string? Building { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public int Capacity { get; set; } = 1;

        public decimal? HourlyRate { get; set; }

        public bool IsAvailable { get; set; } = true;

        public bool IsActive { get; set; } = true;

        // Foreign Keys
        public int DepartmentId { get; set; }

        // Navigation properties
        [ForeignKey("DepartmentId")]
        public virtual Department? Department { get; set; }

        public virtual ICollection<Appointment>? Appointments { get; set; }
    }
}
