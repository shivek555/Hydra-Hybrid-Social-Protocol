# Project Hydra: The God-Tier Protocol

Hydra is a cloud-native, hybrid Web3 social protocol. It bridges the lightning-fast reactivity of centralized architecture with the immutable zero-trust properties of a decentralized ledger.

## Engineering Flexes
This system isn't just an app; it's a statement.

- **Multimodal Data Ingestion:** Hydra intelligently routes data based on trust requirements. Ephemeral DMs are piped through highly-available WebSockets (SignalR + Redis), while Global Broadcasts are routed to an immutable C++ 20 binary ledger.
- **Asynchronous Fan-out:** Utilizing MediatR in .NET, the system decouples API ingress from background processing. The C++ ledger is tailed via a non-blocking BackgroundWorker that pushes "Verified" states back to Angular clients using SignalR Fan-out.
- **Hybrid Consensus (Authority-Gated PoW):** The C++ Node operates on a streamlined Proof-of-Work (PoW) model. However, to prevent Sybil attacks, blocks can only be minted if the payload contains a valid cryptographic signature verified by the .NET Orchestrator's System Key.
- **Cloud-Native Alpine Footprint:** The C++ Ledger Node has been aggressively refactored to utilize raw POSIX sockets and a header-only SHA-256 implementation, allowing it to run in a sub-50MB Alpine Linux Docker container.

## The Architecture
1. **Frontend:** Angular 17+ with Signals (Sub-10ms reactivity, Optimistic UI, Strategy Pattern).
2. **Orchestrator:** .NET 8 API (MediatR, Clean Architecture, JWT/SIWE Auth).
3. **Ledger:** Custom C++ 20 TCP Server (Append-only binary persistence).
4. **Data:** MongoDB (Document Store) & Redis (Presence/PubSub).

## How to Run (Cloud-Native Deployment)
We have optimized the architecture for direct cloud pulls. You do not need to build this locally. The `docker-compose.yml` is configured to pull the compiled, hyper-optimized Alpine Linux images directly from Docker Hub.

```bash
# Pull the pre-built images and spin up the matrix
docker-compose up -d
```

### For CI/CD Pipelines (Pushing to Hub)
If you are deploying updates to the images:
```bash
docker build -t shiveksm/hydrahybridsocialprotocol:cpp-node ./node
docker build -t shiveksm/hydrahybridsocialprotocol:dotnet-api ./backend/Hydra.Api
docker push shiveksm/hydrahybridsocialprotocol:cpp-node
docker push shiveksm/hydrahybridsocialprotocol:dotnet-api
```
