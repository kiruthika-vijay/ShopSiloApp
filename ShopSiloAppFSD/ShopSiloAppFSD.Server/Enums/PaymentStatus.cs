namespace ShopSiloAppFSD.Enums
{
    public enum PaymentStatus
    {
        Pending, // Payment initialized but not captured
        Success, // Payment successful and captured        
        Failed, // Payment failed due to some issues
        Refunded // Payment amount refunded
    }
}
