using System.ComponentModel.DataAnnotations;

namespace Hospital_Management_System.DTOs
{
    public class NursingUnitDto
    {
        public int Id { get; set; }
        public string UnitId { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public string? Wing { get; set; }
        public string? Lead { get; set; }
        public int Nurses { get; set; }
        public string? Coverage { get; set; }
        public string? Ratio { get; set; }
        public string? Focus { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateNursingUnitDto
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
    }

    public class UpdateNursingUnitDto
    {
        [StringLength(50)]
        public string? UnitId { get; set; }

        [StringLength(200)]
        public string? Unit { get; set; }

        [StringLength(200)]
        public string? Wing { get; set; }

        [StringLength(100)]
        public string? Lead { get; set; }

        public int? Nurses { get; set; }

        [StringLength(50)]
        public string? Coverage { get; set; }

        [StringLength(50)]
        public string? Ratio { get; set; }

        [StringLength(1000)]
        public string? Focus { get; set; }

        public bool? IsActive { get; set; }
    }
}

