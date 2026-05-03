using Hydra.Api.Core.Entities;
using Hydra.Api.Core.Interfaces;

namespace Hydra.Api.Core.Services;

public class UniversalFeedService
{
    private readonly IPostRepository _postRepository;

    public UniversalFeedService(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<List<Post>> GetFeedAsync()
    {
        return await _postRepository.GetLatestPostsAsync(50);
    }
}
