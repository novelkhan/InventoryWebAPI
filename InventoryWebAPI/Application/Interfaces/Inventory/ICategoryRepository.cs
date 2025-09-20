using InventoryWebAPI.Domain.Entities.Inventory;

namespace InventoryWebAPI.Application.Interfaces.Inventory
{
    public interface ICategoryRepository
    {
        Task<Category?> GetByIdAsync(Guid id);
        Task<IEnumerable<Category>> GetAllAsync();
        Task AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(Guid id);
        Task<bool> HasProductsAsync(Guid id);
        Task<bool> NameExistsAsync(string name, Guid? excludeId = null);
    }
}