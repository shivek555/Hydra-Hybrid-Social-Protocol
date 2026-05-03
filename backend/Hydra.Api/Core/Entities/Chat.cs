using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Hydra.Api.Core.Entities;

public class Chat
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("participants")]
    public List<string> Participants { get; set; } = new();

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
