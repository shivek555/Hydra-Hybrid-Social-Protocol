using Hydra.Api.Core.Entities;
using MediatR;

namespace Hydra.Api.Core.Commands;

public class BroadcastMessageCommand : IRequest<Post>
{
    public string AuthorWallet { get; set; }
    public string Content { get; set; }
    public string ContentHash { get; set; }
    public string Signature { get; set; }
    public long Timestamp { get; set; }

    public BroadcastMessageCommand(string authorWallet, string content, string contentHash, string signature, long timestamp)
    {
        AuthorWallet = authorWallet;
        Content = content;
        ContentHash = contentHash;
        Signature = signature;
        Timestamp = timestamp;
    }
}
