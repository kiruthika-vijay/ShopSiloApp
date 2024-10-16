using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopSiloAppFSD.Models
{
    public class SalesData
    {
        [Key]
        public int SalesDataID { get; set; }  // Primary Key

        public int OrderID { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime OrderDate { get; set; }

        public decimal TotalAmount { get; set; }

        // Optionally add relationships if needed
        [ForeignKey("OrderID")]
        public virtual Order? Order { get; set; }
    }
}
