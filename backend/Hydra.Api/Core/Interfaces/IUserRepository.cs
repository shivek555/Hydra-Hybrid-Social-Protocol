using Hydra.Api.Core.Entities;

namespace Hydra.Api.Core.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByWalletAddressAsync(string walletAddress);
    Task<User> CreateUserAsync(User user);
}
