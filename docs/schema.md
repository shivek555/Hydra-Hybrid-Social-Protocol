# Data & Protocol Schema

## MongoDB (Centralized)
- **Collection: `Users`**
  - `_id: ObjectId`
  - `wallet_address: String (Index: Unique)`
  - `settings: { allow_stranger_dm: Boolean, block_list: Array<UUID> }`
- **Collection: `DMs`**
  - `participants: Array<UUID>`
  - `encrypted_body: Base64`
  - `nonce: String`

## C++ Blockchain Protocol (Protobuf)
```protobuf
message BroadcastPayload {
  string author_wallet = 1;
  string content_hash = 2; // IPFS/Arweave Hash
  bytes signature = 3;     // ECDSA signature
  uint64 timestamp = 4;
}

message Block {
  uint32 version = 1;
  bytes prev_block_hash = 2;
  repeated BroadcastPayload transactions = 3;
}