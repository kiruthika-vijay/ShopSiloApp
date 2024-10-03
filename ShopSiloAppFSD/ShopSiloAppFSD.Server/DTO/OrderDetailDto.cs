using ShopSiloAppFSD.DTO;
using ShopSiloAppFSD.Enums;

namespace ShopSiloAppFSD.Server.DTO
{
    public class OrderDetailDto
    {
        public int OrderID { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public int UserID { get; set; }
        public int SellerID { get; set; }
        public string? TrackingNumber { get; set; }
        public string? SellerName { get; set; } // Add Seller Name
        public int ShippingAddressID { get; set; }
        public int BillingAddressID { get; set; }
        public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
    }
}
