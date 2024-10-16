using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopSiloAppFSD.Models
{
    public class Discount
    {
        [Key]
        public int DiscountID { get; set; }

        [StringLength(50)]
        public string? DiscountCode { get; set; }

        [StringLength(255)]
        public string? Description { get; set; }

        public decimal DiscountPercentage { get; set; } // Storing the DiscountPercentage as decimal (i.e) 5% => 0.05
        [Column(TypeName = "datetime")]
        public DateTime ExpiryDate { get; set; }
        public bool IsActive { get; set; }

        public decimal MinimumPurchaseAmount { get; set; }
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }

}
