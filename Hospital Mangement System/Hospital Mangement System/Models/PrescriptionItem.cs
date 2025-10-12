using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital_Management_System.Models
{
    public class PrescriptionItem : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string MedicineName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Dosage { get; set; }

        [StringLength(50)]
        public string? Frequency { get; set; } // Once daily, Twice daily, etc.

        [StringLength(100)]
        public string? Duration { get; set; } // 7 days, 2 weeks, etc.

        [StringLength(500)]
        public string? Instructions { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal TotalPrice { get; set; }

        // Foreign Keys
        public int PrescriptionId { get; set; }
        public int? MedicineId { get; set; }

        // Navigation properties
        [ForeignKey("PrescriptionId")]
        public virtual Prescription? Prescription { get; set; }

        [ForeignKey("MedicineId")]
        public virtual Medicine? Medicine { get; set; }
    }
}
