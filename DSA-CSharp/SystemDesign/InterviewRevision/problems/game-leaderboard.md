# Game Leaderboard

## Scope and assumptions

Players submit scores for a game or match. The system shows a player's best score, a global or seasonal top list, and optionally an approximate rank. Scores arrive at high volume, and leaderboards may be divided by game, region, season, or friend group. Anti-cheat adjudication is separate from ranking but can invalidate a result later.

## Requirements and exclusions

- Accept each completed result once, despite retries.
- Rank players by a defined score rule and deterministic tie-breaker.
- Serve top-K quickly and provide a player’s own score/rank.
- Support season reset and delayed score correction.
- Do not let a client directly overwrite an authoritative score.

## Data model and API

```text
MatchResult(result_id, player_id, game_id, season_id, score, submitted_at, status)
PlayerBest(player_id, board_id, score, result_id, version)
LeaderboardEntry(board_id, player_id, score, tie_break_key, updated_at)
```

```text
POST /results
GET  /leaderboards/{board_id}?limit=...
GET  /leaderboards/{board_id}/players/{player_id}
```

The server validates a result or receives it from a trusted game backend; use `result_id` as an idempotency key.

## Architecture

A result API authenticates the source, checks bounds and duplicate IDs, then appends accepted events to a durable log. Consumers update per-player best scores and leaderboard indexes. A sorted-set/index store can serve moderate boards directly. Very large boards may be partitioned by score range, geography, or a coarse bucket, with a top-K merge service and a cached global top list. The event log and accepted result records support replay and audit.

## End-to-end flow

1. A trusted game server submits a result with a unique result ID.
2. The ingestion service validates it and durably records the accepted event.
3. A consumer compares the new score with the player’s best for that board.
4. If it improves the best, update the ranking index using a deterministic tie-break key.
5. Top-K reads query the precomputed ranking structure; a player-rank read looks up their entry and requests rank from the index.
6. Anti-cheat review can revoke a result and trigger a compensating ranking update.

## Scaling, consistency, and reliability

Partition by board identity, not only player ID, so common leaderboard reads stay local. Keep writes partitioned by player or board, then merge results for global ranking. Exact global rank across shards may be expensive; choose between exact computation, score-bucket estimates, or a delayed rank cache based on product needs. Cache top-K lists briefly and invalidate them when leading scores change. Consumer updates must be idempotent and replayable.

## Deep dives and failure cases

- **Hot boards:** shard or batch updates, but retain an authoritative score event for repair.
- **Ties and corrections:** define stable tie-breaks and ensure a revoked score removes the correct entry.
- **Season resets:** use a new board/season key rather than deleting a huge active index synchronously.

## Quick recall

Trust the score source, deduplicate results, keep an append-only record, and maintain derived ranking indexes. Top-K is cheap when precomputed; exact rank and hot-board writes are the main trade-offs.
