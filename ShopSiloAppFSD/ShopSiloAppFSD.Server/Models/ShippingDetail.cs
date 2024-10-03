using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ShopSiloAppFSD.Enums;

namespace ShopSiloAppFSD.Models
{
    public class ShippingDetail
    {
        [Key]
        public int ShippingID { get; set; }

        [Required]
        [StringLength(50)]
        public string? ShippingMethod { get; set; }

        [Required]
        [StringLength(100)]
        public string? TrackingNumber { get; set; }

        [Required]
        [StringLength(20)]
        public ShippingStatus ShippingStatus { get; set; }
        public bool IsActive { get; set; } = true;

        public int OrderID { get; set; }

        [ForeignKey("OrderID")]
        public virtual Order? Order { get; set; }
    }
}

