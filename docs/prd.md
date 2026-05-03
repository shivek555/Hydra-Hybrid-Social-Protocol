# Project Hydra: Hybrid Social Protocol (PRD)

## 1. Product Goals
Build a "Sufficiently Decentralized" app where:
- **Centralized Layer:** High-speed DMs, Stories, and Profiles (.NET/Mongo).
- **Decentralized Layer:** "Global Broadcasts" that are cryptographically immutable (C++ Node).

## 2. Core User Stories
- **As a User**, I want to send an instant DM that is never stored on a public ledger.
- **As a Broadcaster**, I want to post an "On-Chain Message" that no central authority can delete.
- **As a Consumer**, I want a single feed that seamlessly blends private stories and public broadcasts.

## 3. High-Signal Requirements
- **Hybrid Auth:** Must support "Sign-In with Ethereum" (EIP-4361) alongside standard .NET Identity.
- **Zero-Trust Broadcast:** Every broadcast must be hashed (SHA-256) and signed (ECDSA) before being sent to the C++ node.
- **Consistency Gate:** If a user is "Blocked" in the Centralized DB, the UI must also hide their public "On-Chain" broadcasts.

## 4. Edge Cases
- **Node Downtime:** If the C++ blockchain node is offline, broadcasts should be queued in MongoDB and marked as "Pending Mint."
- **Wallet Mismatch:** If a user changes their wallet address, the system must verify ownership of the old address via a signature before re-linking.