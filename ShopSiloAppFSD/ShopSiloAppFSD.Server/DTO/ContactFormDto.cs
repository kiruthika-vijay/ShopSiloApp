using System.ComponentModel.DataAnnotations;

namespace ShopSiloAppFSD.Server.DTO
{
    public class ContactFormDto
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Message is required.")]
        [StringLength(2000, ErrorMessage = "Message cannot exceed 2000 characters.")]
        public string Message { get; set; }
    }
}

