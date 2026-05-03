using Hydra.Api.Core.Entities;
using Hydra.Api.Core.Interfaces;
using MongoDB.Driver;

namespace Hydra.Api.Infrastructure.Data;

public class PostRepository : IPostRepository
{
    private readonly MongoDbContext _context;

    public PostRepository(MongoDbContext context)
    {
        _context = context;
        
        var indexOptions = new CreateIndexOptions { Unique = true };
        var indexKeys = Builders<Post>.IndexKeys.Ascending(p => p.ContentHash);
        _context.Posts.Indexes.CreateOne(new CreateIndexModel<Post>(indexKeys, indexOptions));
    }

    public async Task<Post> CreatePostAsync(Post post)
    {
        await _context.Posts.InsertOneAsync(post);
        return post;
    }

    public async Task<Post?> GetByContentHashAsync(string contentHash)
    {
        return await _context.Posts.Find(p => p.ContentHash == contentHash).FirstOrDefaultAsync();
    }

    public async Task UpdateStatusAsync(string contentHash, string status)
    {
        var update = Builders<Post>.Update.Set(p => p.Status, status);
        await _context.Posts.UpdateOneAsync(p => p.ContentHash == contentHash, update);
    }

    public async Task<List<Post>> GetLatestPostsAsync(int count = 50)
    {
        return await _context.Posts.Find(_ => true)
            .SortByDescending(p => p.Timestamp)
            .Limit(count)
            .ToListAsync();
    }
}
