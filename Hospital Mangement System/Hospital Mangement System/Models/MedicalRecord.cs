using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital_Management_System.Models
{
    public class MedicalRecord : BaseEntity
    {
        [Required]
        public DateTime RecordDate { get; set; }

        [Required]
        [StringLength(100)]
        public string RecordType { get; set; } = string.Empty; // Consultation, Diagnosis, Treatment, Surgery, Lab

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

        // Foreign Keys
        public int PatientId { get; set; }
        public int DoctorId { get; set; }

        // Navigation properties
        [ForeignKey("PatientId")]
        public virtual Patient? Patient { get; set; }

        [ForeignKey("DoctorId")]
        public virtual Doctor? Doctor { get; set; }
    }
}
