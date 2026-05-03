using Hydra.Api.Core.Entities;
using MongoDB.Driver;

namespace Hydra.Api.Infrastructure.Data;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IConfiguration configuration)
    {
        var client = new MongoClient(configuration.GetConnectionString("MongoDb") ?? "mongodb://localhost:27017");
        _database = client.GetDatabase("HydraDb");

        var users = _database.GetCollection<User>("Users");
        
        // Ensure unique index on wallet_address
        var userIndexOptions = new CreateIndexOptions { Unique = true };
        var userIndexKeys = Builders<User>.IndexKeys.Ascending(u => u.WalletAddress);
        users.Indexes.CreateOne(new CreateIndexModel<User>(userIndexKeys, userIndexOptions));

        var messages = _database.GetCollection<Message>("Messages");

        // Ensure compound index for fast retrieval of latest messages per chat
        var messageIndexKeys = Builders<Message>.IndexKeys
            .Ascending(m => m.ConversationId)
            .Descending(m => m.Timestamp);
        messages.Indexes.CreateOne(new CreateIndexModel<Message>(messageIndexKeys));
    }

    public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
    public IMongoCollection<Chat> Chats => _database.GetCollection<Chat>("Chats");
    public IMongoCollection<Message> Messages => _database.GetCollection<Message>("Messages");
    public IMongoCollection<Post> Posts => _database.GetCollection<Post>("Posts");
}
