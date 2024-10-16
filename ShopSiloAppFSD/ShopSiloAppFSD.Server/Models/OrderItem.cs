using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShopSiloAppFSD.Models
{
    public class OrderItem
    {
        [Key]
        public int OrderItemID { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public int OrderID { get; set; }
        [ForeignKey("OrderID")]
        public virtual Order? Order { get; set; }

        public int ProductID { get; set; }
        [ForeignKey("ProductID")]
        public virtual Product? Product { get; set; }
    }

}
