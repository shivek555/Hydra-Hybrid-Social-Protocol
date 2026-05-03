# Implementation Roadmap

## Milestone 1: The Identity Bridge
- [ ] Angular: Build Wallet Connection (MetaMask/Phantom).
- [ ] .NET: Implement JWT + Wallet Signature verification middleware.
- [ ] Mongo: Setup User schema with linked Wallet addresses.

## Milestone 2: The WebSocket Pipe
- [ ] .NET: SignalR Hub for private DMs.
- [ ] Angular: Real-time chat UI using Signals.
- [ ] Mongo: Chat history persistence.

## Milestone 3: The C++ Ledger Node
- [ ] C++: Scaffold a basic TCP/gRPC server to receive payloads.
- [ ] C++: Implement the "Genesis Block" and file-based persistence.

## Milestone 4: The Fusion
- [ ] .NET: "Broadcast" endpoint that forwards validated data to C++ node.
- [ ] Angular: The "Universal Feed" component.