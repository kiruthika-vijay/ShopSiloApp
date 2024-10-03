namespace ShopSiloAppFSD.Server.DTO
{
    public class PaymentVerificationModel
    {
        public string OrderId { get; set; } // Razorpay order ID
        public string PaymentId { get; set; } // Razorpay payment ID
        public string Signature { get; set; } // Razorpay signature
    }
}
