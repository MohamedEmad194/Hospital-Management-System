using System.ComponentModel.DataAnnotations;

namespace Hospital_Management_System.DTOs
{
    public class MedicalRecordDto
    {
        public int Id { get; set; }
        public DateTime RecordDate { get; set; }
        public string RecordType { get; set; } = string.Empty;
        public string? Symptoms { get; set; }
        public string? Diagnosis { get; set; }
        public string? Treatment { get; set; }
        public string? Prescription { get; set; }
        public string? Notes { get; set; }
        public string? LabResults { get; set; }
        public string? ImagingResults { get; set; }
        public string? VitalSigns { get; set; }
        public string? BloodPressure { get; set; }
        public string? Temperature { get; set; }
        public string? HeartRate { get; set; }
        public string? Weight { get; set; }
        public string? Height { get; set; }
        public int PatientId { get; set; }
        public string? PatientName { get; set; }
        public int DoctorId { get; set; }
        public string? DoctorName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateMedicalRecordDto
    {
        [Required]
        public DateTime RecordDate { get; set; }

        [Required]
        [StringLength(100)]
        public string RecordType { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Symptoms { get; set; }

        [StringLength(1000)]
        public string? Diagnosis { get; set; }

        [StringLength(1000)]
        public string? Treatment { get; set; }

        [StringLength(1000)]
        public string? Prescription { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        [StringLength(1000)]
        public string? LabResults { get; set; }

        [StringLength(1000)]
        public string? ImagingResults { get; set; }

        [StringLength(100)]
        public string? VitalSigns { get; set; }

        [StringLength(100)]
        public string? BloodPressure { get; set; }

        [StringLength(100)]
        public string? Temperature { get; set; }

        [StringLength(100)]
        public string? HeartRate { get; set; }

        [StringLength(100)]
        public string? Weight { get; set; }

        [StringLength(100)]
        public string? Height { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        public int DoctorId { get; set; }
    }

    public class UpdateMedicalRecordDto
    {
        public DateTime? RecordDate { get; set; }

        [StringLength(100)]
        public string? RecordType { get; set; }

        [StringLength(1000)]
        public string? Symptoms { get; set; }

        [StringLength(1000)]
        public string? Diagnosis { get; set; }

        [StringLength(1000)]
        public string? Treatment { get; set; }

        [StringLength(1000)]
        public string? Prescription { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        [StringLength(1000)]
        public string? LabResults { get; set; }

        [StringLength(1000)]
        public string? ImagingResults { get; set; }

        [StringLength(100)]
        public string? VitalSigns { get; set; }

        [StringLength(100)]
        public string? BloodPressure { get; set; }

        [StringLength(100)]
        public string? Temperature { get; set; }

        [StringLength(100)]
        public string? HeartRate { get; set; }

        [StringLength(100)]
        public string? Weight { get; set; }

        [StringLength(100)]
        public string? Height { get; set; }
    }
}
