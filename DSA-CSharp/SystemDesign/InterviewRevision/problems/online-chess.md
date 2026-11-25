# Online Chess

## Scope and assumptions

Design a real-time online chess service for matchmaking, turn-based games, clocks, and reconnects. A move is a discrete state transition; the server is authoritative and clients render the board. The service may support casual and rated play, but rating calculations and anti-cheat analysis are separate systems. Network delays and disconnected clients are expected.

## Requirements and exclusions

**Must:** pair eligible players, enforce legal moves and turn order, maintain a single authoritative game state, run clocks consistently, recover after reconnect, and deliver moves to both players promptly.

**Not in scope:** computer opponents, tournament administration, deep engine analysis, or guaranteeing that a disconnected player's local timer exactly matches server time.

## Data model and API

- `Game`: ID, players, variant, status, current position/FEN, side to move, move number, version, and clock state.
- `Move`: game ID, sequence, player, UCI/SAN representation, resulting position hash, server timestamp, and client request ID.
- `QueueEntry`: player, rating band, region preference, and enqueue time.
- `Clock`: remaining time per side, increment, and server-monotonic anchor.

`POST /v1/matchmaking` joins a queue. `POST /v1/games/{id}/moves` submits a move with expected version and request ID. WebSocket events carry committed moves, clock updates, and game completion. `GET /v1/games/{id}` returns a snapshot plus move history.

## Architecture

An API gateway handles identity, rate limits, and session security. Matchmaking workers partition queue entries by mode/rating/region and widen search windows as wait time grows. A game service assigns each active game to one logical owner or serial executor, validates moves using a trusted chess rules library, and atomically advances state. A durable event log stores moves and game transitions; snapshots speed recovery. WebSocket gateways deliver events but do not own game truth. A clock scheduler evaluates deadlines using server time. Results feed rating and history consumers asynchronously.

## End-to-end flow

1. A player requests a match; the service validates eligibility and enqueues a unique queue entry.
2. Matchmaking pairs compatible entries, creates a game record and initial clock state, then notifies both players.
3. Clients load the authoritative snapshot and establish a live connection with their last seen version.
4. On a move, the server verifies player identity, game status, expected version, side to move, legality, and that the clock has not expired.
5. In a per-game serialized transaction, it records the move, resulting position/hash, increments version, and updates the active clock. Only after durable commit does it acknowledge and broadcast the move.
6. Clients apply the sequenced event. If they see a version gap, they fetch missing moves or a fresh snapshot rather than guessing.
7. A reconnecting player resumes from the latest version. Duplicate submissions with the same request ID return the original result; stale versions are rejected with current state.
8. The clock service finalizes timeouts, resignations, draws, and checkmate via the same serialized state transition. A result event drives history and rating updates.

## Scaling, consistency, and reliability

Partition games by game ID, keeping one active writer per game. This makes move order and clock decisions straightforward; matchmaking and read traffic scale independently. Moves require strong per-game ordering and durable acknowledgement. Presence and spectator fan-out may be eventually consistent. Persist moves before broadcast, and reconstruct from log plus snapshot after owner failure. Owner leases or fencing tokens prevent a stale game worker from committing after reassignment. WebSocket reconnection is expected; clients need bounded replay and snapshot fallback. Clock deadlines should use server-side monotonic timing, while persisted timestamps support recovery. Apply rate limits and authorization to prevent move spam or cross-game access.

## Deep dives

**Clock fairness:** Client clocks are manipulable and network arrival is delayed. The server decides based on its authoritative receipt and clock anchor, with clear latency policy. Persist enough timing state to resume safely after a worker crash; do not extend a clock merely because the connection dropped.

**Split-brain game ownership:** Two workers accepting moves can produce incompatible board states. Use a lease with fencing version and conditional writes on game version; fail closed if ownership is uncertain.

## Quick recall

Queue → pair → single-writer authoritative game → legal/versioned move commit → broadcast and replay → server-clock result. Durability precedes acknowledgement, and clients recover by sequence rather than trusting local state.
