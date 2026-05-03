using Hydra.Api.Core.Entities;

namespace Hydra.Api.Core.Interfaces;

public interface IPostRepository
{
    Task<Post> CreatePostAsync(Post post);
    Task<Post?> GetByContentHashAsync(string contentHash);
    Task UpdateStatusAsync(string contentHash, string status);
    Task<List<Post>> GetLatestPostsAsync(int count = 50);
}
