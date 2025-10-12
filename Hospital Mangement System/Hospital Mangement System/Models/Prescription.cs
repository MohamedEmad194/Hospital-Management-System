using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital_Management_System.Models
{
    public class Prescription : BaseEntity
    {
        [Required]
        public DateTime PrescriptionDate { get; set; }

        [Required]
        public DateTime ValidUntil { get; set; }

        [StringLength(1000)]
        public string? Instructions { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        public bool IsDispensed { get; set; } = false;

        public DateTime? DispensedDate { get; set; }

        [StringLength(100)]
        public string? Status { get; set; } = "Active"; // Active, Dispensed, Expired, Cancelled

        // Foreign Keys
        public int PatientId { get; set; }
        public int DoctorId { get; set; }

        // Navigation properties
        [ForeignKey("PatientId")]
        public virtual Patient? Patient { get; set; }

        [ForeignKey("DoctorId")]
        public virtual Doctor? Doctor { get; set; }

        public virtual ICollection<PrescriptionItem>? PrescriptionItems { get; set; }
    }
}
