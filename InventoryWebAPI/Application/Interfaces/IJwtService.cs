using InventoryWebAPI.Domain.Entities;
using System.Threading.Tasks;

namespace InventoryWebAPI.Application.Interfaces
{
    public interface IJwtService
    {
        Task<string> CreateJWT(User user);
        RefreshToken CreateRefreshToken(User user);
    }
}