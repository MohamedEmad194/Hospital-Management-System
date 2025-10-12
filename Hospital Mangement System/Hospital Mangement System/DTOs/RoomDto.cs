using System.ComponentModel.DataAnnotations;

namespace Hospital_Management_System.DTOs
{
    public class RoomDto
    {
        public int Id { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public string RoomType { get; set; } = string.Empty;
        public string? Floor { get; set; }
        public string? Building { get; set; }
        public string? Description { get; set; }
        public int Capacity { get; set; }
        public decimal? HourlyRate { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsActive { get; set; }
        public int DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateRoomDto
    {
        [Required]
        [StringLength(20)]
        public string RoomNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string RoomType { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Floor { get; set; }

        [StringLength(100)]
        public string? Building { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Capacity { get; set; } = 1;

        [Range(0, double.MaxValue)]
        public decimal? HourlyRate { get; set; }

        [Required]
        public int DepartmentId { get; set; }
    }

    public class UpdateRoomDto
    {
        [StringLength(20)]
        public string? RoomNumber { get; set; }

        [StringLength(50)]
        public string? RoomType { get; set; }

        [StringLength(100)]
        public string? Floor { get; set; }

        [StringLength(100)]
        public string? Building { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Range(1, int.MaxValue)]
        public int? Capacity { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? HourlyRate { get; set; }

        public bool? IsAvailable { get; set; }

        public bool? IsActive { get; set; }

        public int? DepartmentId { get; set; }
    }
}
