using System.ComponentModel.DataAnnotations;

namespace Hospital_Management_System.DTOs
{
    public class PrescriptionDto
    {
        public int Id { get; set; }
        public DateTime PrescriptionDate { get; set; }
        public DateTime ValidUntil { get; set; }
        public string? Instructions { get; set; }
        public string? Notes { get; set; }
        public bool IsDispensed { get; set; }
        public DateTime? DispensedDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public int PatientId { get; set; }
        public string? PatientName { get; set; }
        public int DoctorId { get; set; }
        public string? DoctorName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<PrescriptionItemDto>? PrescriptionItems { get; set; }
    }

    public class CreatePrescriptionDto
    {
        [Required]
        public DateTime PrescriptionDate { get; set; }

        [Required]
        public DateTime ValidUntil { get; set; }

        [StringLength(1000)]
        public string? Instructions { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required]
        public List<CreatePrescriptionItemDto> PrescriptionItems { get; set; } = new();
    }

    public class UpdatePrescriptionDto
    {
        public DateTime? PrescriptionDate { get; set; }

        public DateTime? ValidUntil { get; set; }

        [StringLength(1000)]
        public string? Instructions { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        [StringLength(20)]
        public string? Status { get; set; }
    }

    public class PrescriptionItemDto
    {
        public int Id { get; set; }
        public string MedicineName { get; set; } = string.Empty;
        public string? Dosage { get; set; }
        public string? Frequency { get; set; }
        public string? Duration { get; set; }
        public string? Instructions { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public int PrescriptionId { get; set; }
        public int? MedicineId { get; set; }
        public string? MedicineGenericName { get; set; }
    }

    public class CreatePrescriptionItemDto
    {
        [Required]
        [StringLength(100)]
        public string MedicineName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Dosage { get; set; }

        [StringLength(50)]
        public string? Frequency { get; set; }

        [StringLength(100)]
        public string? Duration { get; set; }

        [StringLength(500)]
        public string? Instructions { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        public int? MedicineId { get; set; }
    }
}
