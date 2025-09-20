using InventoryWebAPI.Application.DTOs.Inventory;
using InventoryWebAPI.Application.Interfaces;
using InventoryWebAPI.Domain.Entities.Inventory;
using InventoryWebAPI.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryWebAPI.Presentation.Controllers.Inventory
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public ProductsController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductCreateDto dto)
        {
            var category = await _uow.Categories.GetByIdAsync(dto.CategoryId);
            if (category == null)
                return NotFound("Category not found");

            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Stock = dto.Stock,
                ImageBase64 = dto.ImageBase64,
                CategoryId = dto.CategoryId
            };

            await _uow.Products.AddAsync(product);
            await _uow.CommitAsync();

            return CreatedAtAction(nameof(GetById), new { id = product.Id }, MapToProductDto(product));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] Guid? categoryId, [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice, [FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            if (page < 1 || limit < 1)
                return BadRequest("Invalid pagination parameters");

            var products = await _uow.Products.GetAllAsync(categoryId, minPrice, maxPrice, q, page, limit);
            var totalCount = await _uow.Products.GetTotalCountAsync(categoryId, minPrice, maxPrice, q);

            var productDtos = products.Select(MapToProductDto).ToList();

            return Ok(new
            {
                TotalCount = totalCount,
                Page = page,
                Limit = limit,
                Data = productDtos
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var product = await _uow.Products.GetByIdAsync(id);
            if (product == null)
                return NotFound("Product not found");

            return Ok(MapToProductDto(product));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ProductUpdateDto dto)
        {
            var product = await _uow.Products.GetByIdAsync(id);
            if (product == null)
                return NotFound("Product not found");

            var category = await _uow.Categories.GetByIdAsync(dto.CategoryId);
            if (category == null)
                return NotFound("Category not found");

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.Stock = dto.Stock;
            product.ImageBase64 = dto.ImageBase64;
            product.CategoryId = dto.CategoryId;

            await _uow.Products.UpdateAsync(product);
            await _uow.CommitAsync();

            return Ok(MapToProductDto(product));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var product = await _uow.Products.GetByIdAsync(id);
            if (product == null)
                return NotFound("Product not found");

            await _uow.Products.DeleteAsync(id);
            await _uow.CommitAsync();

            return NoContent();
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string q, [FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            if (string.IsNullOrWhiteSpace(q))
                return BadRequest("Search query is required");

            return await GetAll(null, null, null, q, page, limit);
        }

        private ProductDto MapToProductDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                ImageBase64 = product.ImageBase64,
                CategoryId = product.CategoryId,
                CategoryName = product.Category.Name,
                CreatedAt = product.CreatedAt
            };
        }
    }
}