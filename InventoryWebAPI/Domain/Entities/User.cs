using Microsoft.AspNetCore.Identity;

namespace InventoryWebAPI.Domain.Entities
{
    public class User : IdentityUser
    {
        //The required properties by default come with IdentityUser
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}