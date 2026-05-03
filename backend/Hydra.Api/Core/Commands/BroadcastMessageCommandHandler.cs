using Hydra.Api.Core.Entities;
using Hydra.Api.Core.Interfaces;
using Hydra.Api.Core.Services;
using MediatR;

namespace Hydra.Api.Core.Commands;

public class BroadcastMessageCommandHandler : IRequestHandler<BroadcastMessageCommand, Post>
{
    private readonly IPostRepository _postRepository;
    private readonly BlockchainRelayService _relayService;

    public BroadcastMessageCommandHandler(IPostRepository postRepository, BlockchainRelayService relayService)
    {
        _postRepository = postRepository;
        _relayService = relayService;
    }

    public async Task<Post> Handle(BroadcastMessageCommand request, CancellationToken cancellationToken)
    {
        var post = new Post
        {
            AuthorWallet = request.AuthorWallet,
            Content = request.Content,
            ContentHash = request.ContentHash,
            Signature = request.Signature,
            Timestamp = request.Timestamp,
            Status = "Minting"
        };

        await _postRepository.CreatePostAsync(post);

        // Fire and forget relay
        _ = _relayService.RelayToNodeAsync(request.AuthorWallet, request.ContentHash, request.Signature, request.Timestamp);

        return post;
    }
}
