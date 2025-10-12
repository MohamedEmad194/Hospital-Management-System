using System.ComponentModel.DataAnnotations;

namespace Hospital_Management_System.Models
{
    public class Department : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(100)]
        public string? HeadOfDepartment { get; set; }

        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [StringLength(100)]
        public string? Location { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<Doctor>? Doctors { get; set; }
        public virtual ICollection<Room>? Rooms { get; set; }
        public virtual ICollection<Staff>? StaffMembers { get; set; }
    }
}
