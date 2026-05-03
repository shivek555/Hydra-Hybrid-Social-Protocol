#pragma once
#include <string>
#include <vector>
#include <memory>
#include <cstdint>

struct BroadcastPayload {
    std::string author_wallet;
    std::string content_hash;
    std::string signature;
    uint64_t timestamp;

    std::string Serialize() const;
};

class Block {
public:
    uint32_t version;
    std::string prev_block_hash;
    std::string merkle_root;
    uint32_t timestamp;
    uint32_t difficulty_target;
    uint32_t nonce;
    std::vector<std::unique_ptr<BroadcastPayload>> transactions;

    Block(std::string prevHash);
    std::string CalculateHash() const;
    void MineBlock(uint32_t difficultyLeadingZeros);
    std::string Serialize() const;
};

class Ledger {
private:
    std::string filepath;
    std::string lastBlockHash;

public:
    Ledger(const std::string& path);
    void AppendBlock(std::unique_ptr<Block> block);
    std::string GetLastBlockHash() const { return lastBlockHash; }
};
