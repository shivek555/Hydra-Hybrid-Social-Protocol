#include "TcpServer.h"
#include <iostream>
#include <thread>
#include <stdexcept>
#include <unistd.h>
#include <sys/socket.h>
#include <netinet/in.h>

TcpServer::TcpServer(int port) : isRunning(false), listenSocket(-1) {
    listenSocket = socket(AF_INET, SOCK_STREAM, 0);
    if (listenSocket < 0) {
        throw std::runtime_error("Error at socket()");
    }

    int opt = 1;
    if (setsockopt(listenSocket, SOL_SOCKET, SO_REUSEADDR, &opt, sizeof(opt))) {
        close(listenSocket);
        throw std::runtime_error("setsockopt failed");
    }

    sockaddr_in serverAddr;
    serverAddr.sin_family = AF_INET;
    serverAddr.sin_addr.s_addr = INADDR_ANY;
    serverAddr.sin_port = htons(port);

    if (bind(listenSocket, (struct sockaddr*)&serverAddr, sizeof(serverAddr)) < 0) {
        close(listenSocket);
        throw std::runtime_error("bind failed");
    }

    if (listen(listenSocket, SOMAXCONN) < 0) {
        close(listenSocket);
        throw std::runtime_error("listen failed");
    }
}

TcpServer::~TcpServer() {
    Stop();
    if (listenSocket >= 0) {
        close(listenSocket);
    }
}

void TcpServer::Start(std::function<void(std::string)> onPayloadReceived) {
    isRunning = true;
    std::cout << "TCP Server listening on port 8333..." << std::endl;

    while (isRunning) {
        int clientSocket = accept(listenSocket, nullptr, nullptr);
        if (clientSocket < 0) {
            continue;
        }

        std::thread([clientSocket, onPayloadReceived]() {
            char recvbuf[4096];
            int iResult = read(clientSocket, recvbuf, sizeof(recvbuf) - 1);
            if (iResult > 0) {
                recvbuf[iResult] = '\0';
                onPayloadReceived(std::string(recvbuf));
            }
            close(clientSocket);
        }).detach();
    }
}

void TcpServer::Stop() {
    isRunning = false;
}
