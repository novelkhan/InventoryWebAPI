namespace InventoryWebAPI.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        Inventory.IProductRepository Products { get; }
        Inventory.ICategoryRepository Categories { get; }
        Task<int> CommitAsync();
    }
}