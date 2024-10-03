using ShopSiloAppFSD.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopSiloAppFSD.Models
{
    public class PaymentTransaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TransactionId { get; set; } // Auto-increment primary key

        [ForeignKey("User")]
        public int UserId { get; set; } // Foreign key to the User entity

        [Required]
        [StringLength(255)]
        public string? RazorpayPaymentId { get; set; } // Razorpay Payment ID for the transaction

        [StringLength(255)]
        public string? RazorpayOrderId { get; set; } // Optional Razorpay Order ID (if available)

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Amount { get; set; } // Payment amount

        [Required]
        [StringLength(50)]
        public PaymentStatus PaymentStatus { get; set; } // Payment status (e.g., "Success", "Failed")

        public DateTime Timestamp { get; set; } = DateTime.UtcNow; // Timestamp of the transaction
        public bool IsActive { get; set; } = true;

        // Navigation property to the User entity (if needed)
        public virtual User? User { get; set; }
        public int OrderID { get; set; }
        [ForeignKey("OrderID")]
        public virtual Order? Order { get; set; }
    }
}