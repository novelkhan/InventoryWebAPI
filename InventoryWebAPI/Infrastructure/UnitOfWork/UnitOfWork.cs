using InventoryWebAPI.Application.Interfaces;
using InventoryWebAPI.Application.Interfaces.Inventory;
using InventoryWebAPI.Infrastructure.Data;

namespace InventoryWebAPI.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public UnitOfWork(AppDbContext context,
            IUserRepository userRepository,
            IProductRepository productRepository,
            ICategoryRepository categoryRepository)
        {
            _context = context;
            _userRepository = userRepository;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public IUserRepository Users => _userRepository;
        public IProductRepository Products => _productRepository;
        public ICategoryRepository Categories => _categoryRepository;

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}