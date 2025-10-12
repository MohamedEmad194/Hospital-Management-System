using System.ComponentModel.DataAnnotations;

namespace Hospital_Management_System.DTOs
{
    public class MedicineDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? GenericName { get; set; }
        public string? DosageForm { get; set; }
        public string? Strength { get; set; }
        public string? Manufacturer { get; set; }
        public string? Description { get; set; }
        public string? Indications { get; set; }
        public string? Contraindications { get; set; }
        public string? SideEffects { get; set; }
        public string? DosageInstructions { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int MinimumStockLevel { get; set; }
        public string? Unit { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? BatchNumber { get; set; }
        public bool RequiresPrescription { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateMedicineDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(100)]
        public string? GenericName { get; set; }

        [StringLength(50)]
        public string? DosageForm { get; set; }

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

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int MinimumStockLevel { get; set; }

        [StringLength(20)]
        public string? Unit { get; set; }

        public DateTime? ExpiryDate { get; set; }

        [StringLength(50)]
        public string? BatchNumber { get; set; }

        public bool RequiresPrescription { get; set; } = true;
    }

    public class UpdateMedicineDto
    {
        [StringLength(100)]
        public string? Name { get; set; }

        [StringLength(100)]
        public string? GenericName { get; set; }

        [StringLength(50)]
        public string? DosageForm { get; set; }

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

        [Range(0, double.MaxValue)]
        public decimal? Price { get; set; }

        [Range(0, int.MaxValue)]
        public int? StockQuantity { get; set; }

        [Range(0, int.MaxValue)]
        public int? MinimumStockLevel { get; set; }

        [StringLength(20)]
        public string? Unit { get; set; }

        public DateTime? ExpiryDate { get; set; }

        [StringLength(50)]
        public string? BatchNumber { get; set; }

        public bool? RequiresPrescription { get; set; }

        public bool? IsActive { get; set; }
    }
}
