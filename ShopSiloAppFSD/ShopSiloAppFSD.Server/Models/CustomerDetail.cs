using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace ShopSiloAppFSD.Models
{
    public class CustomerDetail
    {
        [Key]
        public int CustomerID { get; set; }

        [StringLength(50)]
        public string? FirstName { get; set; }

        [StringLength(50)]
        public string? LastName { get; set; }

        [StringLength(20)]
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        [ForeignKey("CustomerID")]
        public virtual User? User { get; set; }

        public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();
    }
}
