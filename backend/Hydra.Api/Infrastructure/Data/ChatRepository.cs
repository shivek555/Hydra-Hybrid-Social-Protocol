using Hydra.Api.Core.Entities;
using Hydra.Api.Core.Interfaces;
using MongoDB.Driver;

namespace Hydra.Api.Infrastructure.Data;

public class ChatRepository : IChatRepository
{
    private readonly MongoDbContext _context;

    public ChatRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<Chat> CreateChatAsync(Chat chat)
    {
        await _context.Chats.InsertOneAsync(chat);
        return chat;
    }

    public async Task<Message> SaveMessageAsync(Message message)
    {
        await _context.Messages.InsertOneAsync(message);
        return message;
    }

    public async Task<List<Message>> GetLast50MessagesAsync(string conversationId)
    {
        var messages = await _context.Messages
            .Find(m => m.ConversationId == conversationId)
            .SortByDescending(m => m.Timestamp)
            .Limit(50)
            .ToListAsync();
            
        messages.Reverse(); // Return in chronological order
        return messages;
    }
}
