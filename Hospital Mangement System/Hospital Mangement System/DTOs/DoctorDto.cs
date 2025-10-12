using System.ComponentModel.DataAnnotations;

namespace Hospital_Management_System.DTOs
{
    public class DoctorDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string NationalId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? PhoneNumber2 { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string LicenseNumber { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string? SubSpecialization { get; set; }
        public int YearsOfExperience { get; set; }
        public string? Education { get; set; }
        public string? Certifications { get; set; }
        public string? Languages { get; set; }
        public decimal ConsultationFee { get; set; }
        public TimeSpan WorkingHoursStart { get; set; }
        public TimeSpan WorkingHoursEnd { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsActive { get; set; }
        public int DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateDoctorDto
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

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
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

        [Required]
        [Range(0, 50)]
        public int YearsOfExperience { get; set; }

        [StringLength(1000)]
        public string? Education { get; set; }

        [StringLength(1000)]
        public string? Certifications { get; set; }

        [StringLength(500)]
        public string? Languages { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal ConsultationFee { get; set; }

        [Required]
        public TimeSpan WorkingHoursStart { get; set; }

        [Required]
        public TimeSpan WorkingHoursEnd { get; set; }

        [Required]
        public int DepartmentId { get; set; }
    }

    public class UpdateDoctorDto
    {
        [StringLength(100)]
        public string? FirstName { get; set; }

        [StringLength(100)]
        public string? LastName { get; set; }

        [EmailAddress]
        [StringLength(256)]
        public string? Email { get; set; }

        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [StringLength(20)]
        public string? PhoneNumber2 { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [StringLength(10)]
        public string? Gender { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        [StringLength(50)]
        public string? LicenseNumber { get; set; }

        [StringLength(100)]
        public string? Specialization { get; set; }

        [StringLength(100)]
        public string? SubSpecialization { get; set; }

        [Range(0, 50)]
        public int? YearsOfExperience { get; set; }

        [StringLength(1000)]
        public string? Education { get; set; }

        [StringLength(1000)]
        public string? Certifications { get; set; }

        [StringLength(500)]
        public string? Languages { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? ConsultationFee { get; set; }

        public TimeSpan? WorkingHoursStart { get; set; }

        public TimeSpan? WorkingHoursEnd { get; set; }

        public bool? IsAvailable { get; set; }

        public bool? IsActive { get; set; }

        public int? DepartmentId { get; set; }
    }
}
