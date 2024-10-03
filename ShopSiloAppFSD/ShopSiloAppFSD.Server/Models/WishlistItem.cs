using ShopSiloAppFSD.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopSiloAppFSD.Models
{
    public class WishlistItem
    {
        [Key]
        public int WishlistItemID { get; set; }

        public int ProductID { get; set; } // Assuming each item is linked to a product

        public int? WishlistID { get; set; } // Foreign key property

        [ForeignKey("WishlistID")]
        public virtual Wishlist? Wishlist { get; set; } // Navigation property
        [ForeignKey("ProductID")]
        public virtual Product? Product { get; set; }
    }
}
