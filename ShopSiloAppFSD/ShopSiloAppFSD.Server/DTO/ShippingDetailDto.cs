using ShopSiloAppFSD.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShopSiloAppFSD.DTO
{
    public class ShippingDetailDto
    {
        public int ShippingID { get; set; }
        public string ShippingMethod { get; set; } = string.Empty;
        public string TrackingNumber { get; set; } = string.Empty;
        public ShippingStatus ShippingStatus { get; set; }
        public int OrderID { get; set; }
    }
}



