using ShopSiloAppFSD.Enums;
using ShopSiloAppFSD.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShopSiloAppFSD.Models
{
    public class Order
    {
        [Key]
        public int OrderID { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        public decimal TotalAmount { get; set; }

        [Required]
        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;

        public int UserID { get; set; }
        [ForeignKey("UserID")]
        public virtual User? User { get; set; }

        public int SellerID { get; set; }
        [ForeignKey("SellerID")]
        public virtual Seller? Seller { get; set; }

        public int ShippingAddressID { get; set; }
        [ForeignKey("ShippingAddressID")]
        public virtual Address? ShippingAddress { get; set; }

        public int BillingAddressID { get; set; }
        [ForeignKey("BillingAddressID")]
        public virtual Address? BillingAddress { get; set; }
        public bool IsActive { get; set; } = true;

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public virtual ICollection<PaymentTransaction> Payments { get; set; } = new List<PaymentTransaction>();

        public virtual ICollection<SalesData> SalesData { get; set; } = new List<SalesData>();

        public virtual ICollection<ShippingDetail> ShippingDetails { get; set; } = new List<ShippingDetail>();

        public int? DiscountID {  get; set; }
        public virtual Discount? Discount { get; set; }

        // New property for tracking information
        [StringLength(100)]
        public string? TrackingNumber { get; set; }
    }
}
