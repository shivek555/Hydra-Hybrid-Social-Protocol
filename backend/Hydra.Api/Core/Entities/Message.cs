using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Hydra.Api.Core.Entities;

public class Message
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("conversation_id")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string ConversationId { get; set; } = string.Empty;

    [BsonElement("sender_wallet")]
    public string SenderWallet { get; set; } = string.Empty;

    [BsonElement("encrypted_body")]
    public string EncryptedBody { get; set; } = string.Empty;

    [BsonElement("nonce")]
    public string Nonce { get; set; } = string.Empty;

    [BsonElement("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
