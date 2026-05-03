using Microsoft.AspNetCore.SignalR;

namespace Hydra.Api.Core.Hubs;

public class CustomUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        return connection.User?.FindFirst("WalletAddress")?.Value;
    }
}
