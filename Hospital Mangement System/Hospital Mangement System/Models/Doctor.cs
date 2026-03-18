using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital_Management_System.Models
{
    public class Doctor : BaseEntity
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
        [StringLength(50)]
        public string LicenseNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Specialization { get; set; } = string.Empty;

        [StringLength(100)]
        public string? SubSpecialization { get; set; }

        public int YearsOfExperience { get; set; }

        [StringLength(1000)]
        public string? Education { get; set; }

        [StringLength(1000)]
        public string? Certifications { get; set; }

        [StringLength(500)]
        public string? Languages { get; set; }

        public decimal ConsultationFee { get; set; }

        public TimeSpan WorkingHoursStart { get; set; }
        public TimeSpan WorkingHoursEnd { get; set; }

        public bool IsAvailable { get; set; } = true;
        public bool IsActive { get; set; } = true;

        // Foreign Keys
        public int DepartmentId { get; set; }
        public string? UserId { get; set; }

        // Navigation properties
        [ForeignKey("DepartmentId")]
        public virtual Department? Department { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        public virtual ICollection<Appointment>? Appointments { get; set; }
        public virtual ICollection<MedicalRecord>? MedicalRecords { get; set; }
        public virtual ICollection<Prescription>? Prescriptions { get; set; }
        public virtual ICollection<Schedule>? Schedules { get; set; }
    }
}
