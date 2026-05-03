#include <iostream>
#include <memory>
#include "Blockchain.h"
#include "TcpServer.h"
#include <mutex>

int main() {
    std::cout << "Hydra C++ Ledger Node Starting..." << std::endl;

    auto ledger = std::make_unique<Ledger>("data/ledger.dat");
    std::mutex ledgerMutex;
    
    // Start TCP Server
    auto server = std::make_unique<TcpServer>(8333);

    auto onPayloadReceived = [&ledger, &ledgerMutex](std::string payloadData) {
        std::cout << "Received payload: " << payloadData << std::endl;
        
        // Simple parsing (system_sig|author_wallet|content_hash|signature|timestamp)
        size_t pos0 = payloadData.find('|'); // after system_sig
        size_t pos1 = payloadData.find('|', pos0 + 1); // after author
        size_t pos2 = payloadData.find('|', pos1 + 1); // after hash
        size_t pos3 = payloadData.find('|', pos2 + 1); // after signature

        if (pos0 != std::string::npos && pos1 != std::string::npos && pos2 != std::string::npos && pos3 != std::string::npos) {
            auto payload = std::make_unique<BroadcastPayload>();
            payload->author_wallet = payloadData.substr(pos0 + 1, pos1 - pos0 - 1);
            payload->content_hash = payloadData.substr(pos1 + 1, pos2 - pos1 - 1);
            payload->signature = payloadData.substr(pos2 + 1, pos3 - pos2 - 1);
            payload->timestamp = std::stoull(payloadData.substr(pos3 + 1));

            std::lock_guard<std::mutex> lock(ledgerMutex);
            
            auto newBlock = std::make_unique<Block>(ledger->GetLastBlockHash());
            newBlock->transactions.push_back(std::move(payload));
            newBlock->MineBlock(2);
            ledger->AppendBlock(std::move(newBlock));
        } else {
            std::cerr << "Invalid payload format received." << std::endl;
        }
    };

    server->Start(onPayloadReceived);

    return 0;
}
