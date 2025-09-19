using InventoryWebAPI.Application.Interfaces;

namespace InventoryWebAPI.Infrastructure.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        Task<int> CommitAsync();
    }
}