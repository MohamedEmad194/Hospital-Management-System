using System.ComponentModel.DataAnnotations;

namespace Hospital_Management_System.Models
{
    public class NursingUnit : BaseEntity
    {
        [Required]
        [StringLength(50)]
        public string UnitId { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Unit { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Wing { get; set; }

        [StringLength(100)]
        public string? Lead { get; set; }

        public int Nurses { get; set; }

        [StringLength(50)]
        public string? Coverage { get; set; }

        [StringLength(50)]
        public string? Ratio { get; set; }

        [StringLength(1000)]
        public string? Focus { get; set; }

        public bool IsActive { get; set; } = true;
    }
}

