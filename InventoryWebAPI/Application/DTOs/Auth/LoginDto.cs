using System.ComponentModel.DataAnnotations;

namespace InventoryWebAPI.Application.DTOs.Auth
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}