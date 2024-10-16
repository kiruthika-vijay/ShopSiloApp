using ShopSiloAppFSD.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShopSiloAppFSD.Models
{
    public class Wishlist
    {
        [Key]
        public int WishListID { get; set; }

        public int UserID { get; set; }
        [ForeignKey("UserID")]
        public virtual User? User { get; set; }

        public virtual ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();
    }
}
