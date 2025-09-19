using System.ComponentModel.DataAnnotations;

namespace InventoryWebAPI.Application.DTOs.Auth
{
    public class RegisterDto
    {
        [Required, MaxLength(50)]
        public string Username { get; set; }
        [Required]
        [RegularExpression("^\\w+@[a-zA-Z_]+?\\.[a-zA-Z]{2,3}$", ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
        [Required]
        [StringLength(15, MinimumLength = 6, ErrorMessage = "Password must be at least {2}, and maximum {1} characters")]
        public string Password { get; set; }
    }
}