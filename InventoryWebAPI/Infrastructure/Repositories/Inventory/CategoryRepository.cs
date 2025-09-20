using InventoryWebAPI.Application.Interfaces.Inventory;
using InventoryWebAPI.Domain.Entities.Inventory;
using InventoryWebAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InventoryWebAPI.Infrastructure.Repositories.Inventory
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Category?> GetByIdAsync(Guid id)
        {
            return await _context.Categories.FindAsync(id);
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories
                .Select(c => new Category
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    CreatedAt = c.CreatedAt,
                    Products = c.Products
                })
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task AddAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
        }

        public async Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
        }

        public async Task DeleteAsync(Guid id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
                _context.Categories.Remove(category);
        }

        public async Task<bool> HasProductsAsync(Guid id)
        {
            return await _context.Products.AnyAsync(p => p.CategoryId == id);
        }

        public async Task<bool> NameExistsAsync(string name, Guid? excludeId = null)
        {
            var query = _context.Categories.Where(c => c.Name == name);
            if (excludeId.HasValue)
                query = query.Where(c => c.Id != excludeId.Value);
            return await query.AnyAsync();
        }
    }
}