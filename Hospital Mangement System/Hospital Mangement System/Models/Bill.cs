using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital_Management_System.Models
{
    public class Bill : BaseEntity
    {
        [Required]
        [StringLength(50)]
        public string BillNumber { get; set; } = string.Empty;

        [Required]
        public DateTime BillDate { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Paid, Overdue, Cancelled

        public decimal SubTotal { get; set; }

        public decimal TaxAmount { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal PaidAmount { get; set; }

        public decimal RemainingAmount { get; set; }

        [StringLength(100)]
        public string? PaymentMethod { get; set; }

        public DateTime? PaymentDate { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        [StringLength(100)]
        public string? InsuranceProvider { get; set; }

        [StringLength(50)]
        public string? InsuranceNumber { get; set; }

        public decimal? InsuranceCoverage { get; set; }

        // Foreign Keys
        public int PatientId { get; set; }

        // Navigation properties
        [ForeignKey("PatientId")]
        public virtual Patient? Patient { get; set; }

        public virtual ICollection<BillItem>? BillItems { get; set; }
    }
}
