using ShopSiloAppFSD.Enums;

namespace ShopSiloAppFSD.DTO
{
    public class OrderDto
    {
        public int OrderID { get; set; }
        public decimal TotalAmount { get; set; }
        public int UserID { get; set; }
        public int SellerID { get; set; }
        public int ShippingAddressID { get; set; }
        public int BillingAddressID { get; set; }
        public string? TrackingNumber { get; set; }        
        public int? DiscountID { get; set; }
        public DateTime? OrderDate { get; set; }
        public OrderStatus OrderStatus { get; set; }
        // List of Order Items
        public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
    }
}
