#include "Blockchain.h"
#include "HashUtil.h"
#include <sstream>
#include <iostream>
#include <fstream>
#include <chrono>

std::string BroadcastPayload::Serialize() const {
    return author_wallet + "|" + content_hash + "|" + signature + "|" + std::to_string(timestamp);
}

Block::Block(std::string prevHash) : version(1), prev_block_hash(prevHash), difficulty_target(2), nonce(0) {
    timestamp = static_cast<uint32_t>(std::chrono::system_clock::now().time_since_epoch() / std::chrono::seconds(1));
    merkle_root = "0000000000000000000000000000000000000000000000000000000000000000"; // Simplified for MVP
}

std::string Block::CalculateHash() const {
    std::stringstream ss;
    ss << version << prev_block_hash << merkle_root << timestamp << difficulty_target << nonce;
    for (const auto& tx : transactions) {
        ss << tx->Serialize();
    }
    return HashUtil::ComputeSHA256(ss.str());
}

void Block::MineBlock(uint32_t difficultyLeadingZeros) {
    std::string target(difficultyLeadingZeros, '0');
    std::string hash = CalculateHash();
    
    std::cout << "Mining block..." << std::endl;
    while (hash.substr(0, difficultyLeadingZeros) != target) {
        nonce++;
        hash = CalculateHash();
    }
    std::cout << "Block mined! Hash: " << hash << ", Nonce: " << nonce << std::endl;
}

std::string Block::Serialize() const {
    std::stringstream ss;
    ss << version << "\n" << prev_block_hash << "\n" << merkle_root << "\n" 
       << timestamp << "\n" << difficulty_target << "\n" << nonce << "\n";
    ss << transactions.size() << "\n";
    for (const auto& tx : transactions) {
        ss << tx->Serialize() << "\n";
    }
    return ss.str();
}

Ledger::Ledger(const std::string& path) : filepath(path) {
    lastBlockHash = std::string(64, '0'); // Genesis prev hash
    
    std::ifstream file(filepath, std::ios::binary);
    if (file.good()) {
        std::cout << "Ledger loaded from " << filepath << std::endl;
        // Basic implementation, a full node would parse blocks to find last hash
    } else {
        std::cout << "Creating new ledger at " << filepath << std::endl;
    }
}

void Ledger::AppendBlock(std::unique_ptr<Block> block) {
    std::ofstream file(filepath, std::ios::binary | std::ios::app);
    if (!file.is_open()) {
        throw std::runtime_error("Cannot open ledger file for appending");
    }

    std::string serialized = block->Serialize();
    file.write(serialized.c_str(), serialized.length());
    file.flush();
    
    lastBlockHash = block->CalculateHash();
    std::cout << "Block appended to ledger successfully. New tip: " << lastBlockHash << std::endl;
}
