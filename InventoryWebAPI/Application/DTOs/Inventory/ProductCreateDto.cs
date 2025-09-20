using System.ComponentModel.DataAnnotations;

namespace InventoryWebAPI.Application.DTOs.Inventory
{
    public class ProductCreateDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int Stock { get; set; }
        public string? ImageBase64 { get; set; }
        [Required]
        public Guid CategoryId { get; set; }
    }
}