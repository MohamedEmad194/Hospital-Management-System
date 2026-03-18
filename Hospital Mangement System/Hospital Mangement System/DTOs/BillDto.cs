using System.ComponentModel.DataAnnotations;

namespace Hospital_Management_System.DTOs
{
    public class BillDto
    {
        public int Id { get; set; }
        public string BillNumber { get; set; } = string.Empty;
        public DateTime BillDate { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public string? PaymentMethod { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string? Notes { get; set; }
        public string? InsuranceProvider { get; set; }
        public string? InsuranceNumber { get; set; }
        public decimal? InsuranceCoverage { get; set; }
        public int PatientId { get; set; }
        public string? PatientName { get; set; }
        public string? PatientEmail { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<BillItemDto>? BillItems { get; set; }
    }

    public class CreateBillDto
    {
        [Required]
        public DateTime BillDate { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        [StringLength(100)]
        public string? InsuranceProvider { get; set; }

        [StringLength(50)]
        public string? InsuranceNumber { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? InsuranceCoverage { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        public List<CreateBillItemDto> BillItems { get; set; } = new();
    }

    public class UpdateBillDto
    {
        public DateTime? BillDate { get; set; }

        public DateTime? DueDate { get; set; }

        [StringLength(20)]
        public string? Status { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        [StringLength(100)]
        public string? PaymentMethod { get; set; }

        public DateTime? PaymentDate { get; set; }

        [StringLength(100)]
        public string? InsuranceProvider { get; set; }

        [StringLength(50)]
        public string? InsuranceNumber { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? InsuranceCoverage { get; set; }
    }

    public class BillItemDto
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? Category { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string? Notes { get; set; }
        public int BillId { get; set; }
    }

    public class CreateBillItemDto
    {
        [Required]
        [StringLength(200)]
        public string Description { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Category { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        [StringLength(100)]
        public string? Notes { get; set; }
    }

    public class PaymentDto
    {
        [Required]
        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(100)]
        public string PaymentMethod { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}
