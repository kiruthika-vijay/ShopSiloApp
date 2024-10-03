using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShopSiloAppFSD.Models
{
    public class Address
    {
        [Key]
        public int AddressID { get; set; }

        [StringLength(255)]
        public string? AddressLine1 { get; set; }

        [StringLength(255)]
        public string? AddressLine2 { get; set; }

        [StringLength(50)]
        public string? City { get; set; }

        [StringLength(50)]
        public string? State { get; set; }

        [StringLength(20)]
        public string? PostalCode { get; set; }

        [StringLength(50)]
        public string? Country { get; set; }

        public bool IsBillingAddress { get; set; } // Indicates if it's a billing address

        public bool IsShippingAddress { get; set; } // Indicates if it's a shipping address
        public bool IsActive { get; set; } = true;
        public int CustomerID { get; set; }

        [ForeignKey("CustomerID")]
        public virtual CustomerDetail? CustomerDetail { get; set; }
    }
}

