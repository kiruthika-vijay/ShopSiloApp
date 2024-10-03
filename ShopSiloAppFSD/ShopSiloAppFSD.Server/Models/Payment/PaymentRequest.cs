namespace ShopSiloAppFSD.Server.Models.Payment
{
    public class PaymentResult
    {
        public string PaymentStatus { get; set; }
        public string OrderId { get; set; }
        public string PaymentId { get; set; }
        public string Signature { get; set; }
    }
}
