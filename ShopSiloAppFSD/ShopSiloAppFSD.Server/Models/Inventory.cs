using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShopSiloAppFSD.Models
{
    public class Inventory
    {
        [Key]
        public int InventoryID { get; set; }

        public int Quantity { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime LastUpdatedDate { get; set; } = DateTime.Now;

        public int ProductID { get; set; }
        [ForeignKey("ProductID")]
        public virtual Product? Product { get; set; }
        public bool IsActive { get; set; } = true;
    }
}

