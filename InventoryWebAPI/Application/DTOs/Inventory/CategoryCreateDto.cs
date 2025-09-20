using System.ComponentModel.DataAnnotations;

namespace InventoryWebAPI.Application.DTOs.Inventory
{
    public class CategoryCreateDto
    {
        [Required, MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(200)]
        public string Description { get; set; }
    }
}