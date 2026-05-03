using Hydra.Api.Core.Commands;
using Hydra.Api.Core.Interfaces;
using Hydra.Api.Core.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hydra.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BroadcastController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly WalletAuthService _walletAuthService;
    private readonly IPostRepository _postRepository;

    public BroadcastController(IMediator mediator, WalletAuthService walletAuthService, IPostRepository postRepository)
    {
        _mediator = mediator;
        _walletAuthService = walletAuthService;
        _postRepository = postRepository;
    }

    [HttpPost]
    public async Task<IActionResult> Broadcast([FromBody] BroadcastRequest request)
    {
        var authorWallet = User.FindFirst("WalletAddress")?.Value;
        if (authorWallet == null || !authorWallet.Equals(request.AuthorWallet, StringComparison.OrdinalIgnoreCase))
            return Unauthorized();

        // Verify Signature
        var isValid = _walletAuthService.VerifySignature(request.ContentHash, request.Signature, authorWallet);
        if (!isValid)
            return BadRequest("Invalid signature for broadcast content.");

        var post = await _mediator.Send(new BroadcastMessageCommand(
            authorWallet,
            request.Content,
            request.ContentHash,
            request.Signature,
            request.Timestamp
        ));

        return Ok(post);
    }

    [HttpGet]
    public async Task<IActionResult> GetFeed()
    {
        var posts = await _postRepository.GetLatestPostsAsync();
        return Ok(posts);
    }
}

public class BroadcastRequest
{
    public string AuthorWallet { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string ContentHash { get; set; } = string.Empty;
    public string Signature { get; set; } = string.Empty;
    public long Timestamp { get; set; }
}
