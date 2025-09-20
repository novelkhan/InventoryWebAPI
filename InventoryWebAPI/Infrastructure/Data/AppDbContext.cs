using InventoryWebAPI.Domain.Entities;
using InventoryWebAPI.Domain.Entities.Inventory;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InventoryWebAPI.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            //add-migration AddingUserToDatabase -o Infrastructure/Data/Migrations
            //add-migration AddingProductToDatabase -o Infrastructure/Data/Migrations
        }


        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}