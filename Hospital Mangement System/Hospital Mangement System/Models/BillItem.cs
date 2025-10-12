using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital_Management_System.Models
{
    public class BillItem : BaseEntity
    {
        [Required]
        [StringLength(200)]
        public string Description { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Category { get; set; } // Consultation, Medicine, Lab, Surgery, etc.

        public int Quantity { get; set; } = 1;

        public decimal UnitPrice { get; set; }

        public decimal TotalPrice { get; set; }

        [StringLength(100)]
        public string? Notes { get; set; }

        // Foreign Keys
        public int BillId { get; set; }

        // Navigation properties
        [ForeignKey("BillId")]
        public virtual Bill? Bill { get; set; }
    }


}
