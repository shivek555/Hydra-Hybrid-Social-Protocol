using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Hydra.Api.Core.Entities;

public class Post
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("author_wallet")]
    public string AuthorWallet { get; set; } = string.Empty;

    [BsonElement("content_hash")]
    public string ContentHash { get; set; } = string.Empty;

    [BsonElement("content")] // Original text
    public string Content { get; set; } = string.Empty;

    [BsonElement("signature")]
    public string Signature { get; set; } = string.Empty;

    [BsonElement("timestamp")]
    public long Timestamp { get; set; }

    [BsonElement("status")]
    public string Status { get; set; } = "Minting"; // "Minting" or "Verified"
}
