using Hydra.Api.Core.Entities;
using Hydra.Api.Core.Interfaces;
using MongoDB.Driver;

namespace Hydra.Api.Infrastructure.Data;

public class UserRepository : IUserRepository
{
    private readonly MongoDbContext _context;

    public UserRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByWalletAddressAsync(string walletAddress)
    {
        return await _context.Users.Find(u => u.WalletAddress == walletAddress).FirstOrDefaultAsync();
    }

    public async Task<User> CreateUserAsync(User user)
    {
        await _context.Users.InsertOneAsync(user);
        return user;
    }
}
