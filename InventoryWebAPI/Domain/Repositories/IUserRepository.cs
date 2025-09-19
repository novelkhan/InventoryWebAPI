using InventoryWebAPI.Domain.Entities;

namespace InventoryWebAPI.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> FindByEmailAsync(string email);
        Task<User?> FindByIdAsync(string userId);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> CheckPasswordAsync(User user, string password);
        Task<bool> CreateUserAsync(User user, string password);

        // RefreshToken সম্পর্কিত
        Task<RefreshToken?> GetRefreshTokenByUserIdAsync(string userId);
        Task<RefreshToken?> GetRefreshTokenAsync(string userId, string token);
        Task AddRefreshTokenAsync(RefreshToken refreshToken);
    }
}