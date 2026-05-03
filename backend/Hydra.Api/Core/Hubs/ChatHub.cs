using Hydra.Api.Core.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;

namespace Hydra.Api.Core.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IMediator _mediator;
    private readonly IConnectionMultiplexer _redis;

    public ChatHub(IMediator mediator, IConnectionMultiplexer redis)
    {
        _mediator = mediator;
        _redis = redis;
    }

    public override async Task OnConnectedAsync()
    {
        var walletAddress = Context.UserIdentifier;
        if (!string.IsNullOrEmpty(walletAddress))
        {
            var db = _redis.GetDatabase();
            await db.StringSetAsync($"presence:{walletAddress}", "Online", TimeSpan.FromMinutes(15));
            await Clients.Others.SendAsync("UserPresenceChanged", walletAddress, "Online");
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var walletAddress = Context.UserIdentifier;
        if (!string.IsNullOrEmpty(walletAddress))
        {
            var db = _redis.GetDatabase();
            await db.KeyDeleteAsync($"presence:{walletAddress}");
            await Clients.Others.SendAsync("UserPresenceChanged", walletAddress, "Offline");
        }
        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinConversation(string conversationId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
    }

    public async Task LeaveConversation(string conversationId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId);
    }

    public async Task SendMessage(string conversationId, string encryptedContent, string nonce)
    {
        var walletAddress = Context.UserIdentifier;
        if (string.IsNullOrEmpty(walletAddress)) return;

        await _mediator.Send(new SendMessageCommand(walletAddress, conversationId, encryptedContent, nonce));
    }
}
