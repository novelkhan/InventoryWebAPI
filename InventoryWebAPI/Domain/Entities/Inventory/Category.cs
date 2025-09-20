using System.ComponentModel.DataAnnotations;

namespace InventoryWebAPI.Domain.Entities.Inventory
{
    public class Category
    {
        [Key]
        public Guid Id { get; set; }
        [Required, MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(200)]
        public string Description { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}