using Hydra.Api.Core.Entities;
using Hydra.Api.Core.Hubs;
using Hydra.Api.Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace Hydra.Api.Core.Commands;

public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, Message>
{
    private readonly IChatRepository _chatRepository;
    private readonly IHubContext<ChatHub> _hubContext;

    public SendMessageCommandHandler(IChatRepository chatRepository, IHubContext<ChatHub> hubContext)
    {
        _chatRepository = chatRepository;
        _hubContext = hubContext;
    }

    public async Task<Message> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var message = new Message
        {
            SenderWallet = request.SenderWallet,
            ConversationId = request.ConversationId,
            EncryptedBody = request.EncryptedContent,
            Nonce = request.Nonce,
            Timestamp = DateTime.UtcNow
        };

        await _chatRepository.SaveMessageAsync(message);

        // Broadcast to all clients in the conversation group
        await _hubContext.Clients.Group(request.ConversationId).SendAsync("ReceiveMessage", message, cancellationToken: cancellationToken);

        return message;
    }
}
