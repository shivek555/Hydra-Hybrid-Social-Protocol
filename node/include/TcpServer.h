#pragma once
#include <string>
#include <memory>
#include <functional>

class TcpServer {
private:
    int listenSocket;
    bool isRunning;

public:
    TcpServer(int port);
    ~TcpServer();
    void Start(std::function<void(std::string)> onPayloadReceived);
    void Stop();
};
