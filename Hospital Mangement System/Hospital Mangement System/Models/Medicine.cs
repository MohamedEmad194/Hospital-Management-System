using System.ComponentModel.DataAnnotations;

namespace Hospital_Management_System.Models
{
    public class Medicine : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(100)]
        public string? GenericName { get; set; }

        [StringLength(50)]
        public string? DosageForm { get; set; } // Tablet, Syrup, Injection, etc.

        [StringLength(100)]
        public string? Strength { get; set; }

        [StringLength(100)]
        public string? Manufacturer { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(1000)]
        public string? Indications { get; set; }

        [StringLength(1000)]
        public string? Contraindications { get; set; }

        [StringLength(1000)]
        public string? SideEffects { get; set; }

        [StringLength(1000)]
        public string? DosageInstructions { get; set; }

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public int MinimumStockLevel { get; set; }

        [StringLength(20)]
        public string? Unit { get; set; } // mg, ml, tablet, etc.

        public DateTime? ExpiryDate { get; set; }

        [StringLength(50)]
        public string? BatchNumber { get; set; }

        public bool RequiresPrescription { get; set; } = true;

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<PrescriptionItem>? PrescriptionItems { get; set; }
    }
}
