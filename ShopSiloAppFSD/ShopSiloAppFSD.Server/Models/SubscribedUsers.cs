using System.ComponentModel.DataAnnotations;

namespace ShopSiloAppFSD.Server.Models
{
    public class SubscribedUsers
    {
        [Key]
        public int SubscribedUserID { get; set; }
        [Required]
        public string Email { get; set; }
    }
}
