using InventoryWebAPI.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InventoryWebAPI.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            //add-migration AddingUserToDatabase -o Infrastructure/Data/Migrations
        }


        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}