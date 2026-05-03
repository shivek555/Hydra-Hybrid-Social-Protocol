using Hydra.Api.Core.Entities;
using MediatR;

namespace Hydra.Api.Core.Commands;

public class SendMessageCommand : IRequest<Message>
{
    public string SenderWallet { get; set; }
    public string ConversationId { get; set; }
    public string EncryptedContent { get; set; }
    public string Nonce { get; set; }

    public SendMessageCommand(string senderWallet, string conversationId, string encryptedContent, string nonce)
    {
        SenderWallet = senderWallet;
        ConversationId = conversationId;
        EncryptedContent = encryptedContent;
        Nonce = nonce;
    }
}
