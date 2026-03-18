using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital_Management_System.Models
{
    public class Staff : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string NationalId { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(256)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [StringLength(20)]
        public string? PhoneNumber2 { get; set; }

        public DateTime DateOfBirth { get; set; }

        [StringLength(10)]
        public string Gender { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Address { get; set; }

        [Required]
        [StringLength(100)]
        public string Position { get; set; } = string.Empty;

        [StringLength(50)]
        public string? EmployeeId { get; set; }

        public DateTime HireDate { get; set; }

        public decimal Salary { get; set; }

        [StringLength(100)]
        public string? Qualification { get; set; }

        [StringLength(500)]
        public string? Skills { get; set; }

        public TimeSpan WorkingHoursStart { get; set; }
        public TimeSpan WorkingHoursEnd { get; set; }

        public bool IsActive { get; set; } = true;

        // Foreign Keys
        public int DepartmentId { get; set; }
        public string? UserId { get; set; }

        // Navigation properties
        [ForeignKey("DepartmentId")]
        public virtual Department? Department { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}
