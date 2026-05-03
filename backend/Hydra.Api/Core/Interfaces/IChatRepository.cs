using Hydra.Api.Core.Entities;

namespace Hydra.Api.Core.Interfaces;

public interface IChatRepository
{
    Task<Chat> CreateChatAsync(Chat chat);
    Task<Message> SaveMessageAsync(Message message);
    Task<List<Message>> GetLast50MessagesAsync(string conversationId);
}
