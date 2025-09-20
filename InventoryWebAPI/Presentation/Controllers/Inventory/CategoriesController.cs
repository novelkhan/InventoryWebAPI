using InventoryWebAPI.Application.DTOs.Inventory;
using InventoryWebAPI.Application.Interfaces;
using InventoryWebAPI.Domain.Entities.Inventory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryWebAPI.Presentation.Controllers.Inventory
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public CategoriesController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoryCreateDto dto)
        {
            if (await _uow.Categories.NameExistsAsync(dto.Name))
                return Conflict("Category name already exists");

            var category = new Category
            {
                Name = dto.Name,
                Description = dto.Description
            };

            await _uow.Categories.AddAsync(category);
            await _uow.CommitAsync();

            return CreatedAtAction(nameof(GetById), new { id = category.Id }, MapToCategoryDto(category));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _uow.Categories.GetAllAsync();
            var categoryDtos = categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                ProductCount = c.Products.Count,
                CreatedAt = c.CreatedAt
            }).ToList();

            return Ok(categoryDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var category = await _uow.Categories.GetByIdAsync(id);
            if (category == null)
                return NotFound("Category not found");

            return Ok(MapToCategoryDto(category));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CategoryUpdateDto dto)
        {
            var category = await _uow.Categories.GetByIdAsync(id);
            if (category == null)
                return NotFound("Category not found");

            if (await _uow.Categories.NameExistsAsync(dto.Name, id))
                return Conflict("Category name already exists");

            category.Name = dto.Name;
            category.Description = dto.Description;

            await _uow.Categories.UpdateAsync(category);
            await _uow.CommitAsync();

            return Ok(MapToCategoryDto(category));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var category = await _uow.Categories.GetByIdAsync(id);
            if (category == null)
                return NotFound("Category not found");

            if (await _uow.Categories.HasProductsAsync(id))
                return Conflict("Cannot delete category with associated products");

            await _uow.Categories.DeleteAsync(id);
            await _uow.CommitAsync();

            return NoContent();
        }

        private CategoryDto MapToCategoryDto(Category category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ProductCount = category.Products.Count,
                CreatedAt = category.CreatedAt
            };
        }
    }
}