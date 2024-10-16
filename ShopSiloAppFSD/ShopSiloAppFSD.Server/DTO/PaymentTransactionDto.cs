using ShopSiloAppFSD.Enums;
using System.ComponentModel.DataAnnotations;

namespace ShopSiloAppFSD.DTO
{
    public class PaymentTransactionDto
    {
        public int TransactionId { get; set; }
        public int UserId { get; set; }
        public string RazorpayPaymentId { get; set; }
        public decimal Amount { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public DateTime Timestamp { get; set; }
        public int OrderID { get; set; }
    }
}
