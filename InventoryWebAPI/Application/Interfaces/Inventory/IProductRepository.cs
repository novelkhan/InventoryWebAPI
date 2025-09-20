using InventoryWebAPI.Domain.Entities.Inventory;

namespace InventoryWebAPI.Application.Interfaces.Inventory
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(Guid id);
        Task<IEnumerable<Product>> GetAllAsync(Guid? categoryId, decimal? minPrice, decimal? maxPrice, string? search, int page, int limit);
        Task<int> GetTotalCountAsync(Guid? categoryId, decimal? minPrice, decimal? maxPrice, string? search);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Guid id);
    }
}