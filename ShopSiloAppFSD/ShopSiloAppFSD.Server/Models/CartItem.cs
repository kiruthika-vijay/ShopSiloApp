using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShopSiloAppFSD.Models
{
    public class CartItem
    {
        [Key]
        public int CartItemID { get; set; }

        public int Quantity { get; set; } = 1;

        public decimal Price { get; set; }

        public int CartID { get; set; }
        [ForeignKey("CartID")]
        public virtual ShoppingCart? ShoppingCart { get; set; }

        public int ProductID { get; set; }
        [ForeignKey("ProductID")]
        public virtual Product? Product { get; set; }
    }

}
