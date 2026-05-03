# Tech Stack Specifications

## Backend (.NET 8.0+)
- **Architecture:** Clean Architecture with MediatR for Command/Query separation.
- **Communication:** Use gRPC for high-speed internal talk between .NET and the C++ node.
- **Real-time:** SignalR for persistent WebSocket connections (DMs).

## Blockchain (C++ 20)
- **Node Logic:** A custom light-node based on the 2009 Bitcoin source. 
- **Ledger:** Append-only binary file format.
- **Consensus:** "Authority-Gated PoW"—only verified users can submit broadcast blocks.

## Frontend (Angular 17+)
- **State Management:** Angular Signals for real-time DM updates.
- **Web3 Integration:** Ethers.js for wallet signing and transaction tracking.

## Deployment (Free Tier Strategy)
- **Database:** MongoDB Atlas M0 (512MB).
- **Hosting:** Render (Back-end) / Vercel (Front-end).
- **Node Hosting:** Oracle Cloud "Always Free" (ARM Ampere 4 OCPU / 24GB RAM).