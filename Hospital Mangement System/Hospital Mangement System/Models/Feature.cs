using System.ComponentModel.DataAnnotations;

namespace Hospital_Management_System.Models
{
    public class Feature : BaseEntity
    {
        [Required]
        [StringLength(10)]
        public string Icon { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string TitleEn { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string TitleAr { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string DescriptionEn { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string DescriptionAr { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Color { get; set; }

        public int DisplayOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;
    }
}

