using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Hydra.Api.Core.Entities;

public class User
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("wallet_address")]
    public string WalletAddress { get; set; } = string.Empty;

    [BsonElement("settings")]
    public UserSettings Settings { get; set; } = new();
}

public class UserSettings
{
    [BsonElement("allow_stranger_dm")]
    public bool AllowStrangerDm { get; set; } = false;

    [BsonElement("block_list")]
    public List<Guid> BlockList { get; set; } = new();
}
