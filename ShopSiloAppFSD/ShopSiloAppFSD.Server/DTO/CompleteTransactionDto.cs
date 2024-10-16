using ShopSiloAppFSD.DTO;
using ShopSiloAppFSD.Enums;

namespace ShopSiloAppFSD.Server.DTO
{
    public class CompleteTransactionDto
    {
        public int UserId { get; set; }
        public string RazorpayPaymentId { get; set; } // Payment ID from Razorpay
        public string RazorpayOrderId { get; set; } // Order ID from Razorpay
        public decimal Amount { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public int ShippingAddressID { get; set; }
        public int BillingAddressID { get; set; }
        public int SellerID { get; set; }

        // Add a list of order items
        public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
    }

}
