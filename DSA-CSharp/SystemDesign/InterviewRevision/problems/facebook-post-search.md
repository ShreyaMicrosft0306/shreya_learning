# Social-Network Post Search

## Scope and assumptions

Design keyword search over posts in a large social network. Users expect relevant, recent results, filtered to content they are currently allowed to see. Posts are created and edited continuously; privacy settings, blocks, and deletions can change after indexing. The focus is indexing and query serving, not news-feed ranking, people search, or an entire social graph design.

## Requirements and exclusions

**Required:** index text and basic metadata; retrieve matching posts by query, recency, and relevance; filter by author/time and supported language; enforce visibility, blocks, and deletion; support edits and index lag.

**Quality:** low-latency common queries, horizontal scaling, graceful degradation, and privacy-safe results. Search may be eventually consistent for discovery, but stale index entries must not leak content after access revocation.

## Data model and API

Canonical `Post(post_id, author_id, text, created_at, visibility, language, version, deleted_at)` lives in a transactional store. Visibility may be public, friends-only, or private, with policy references resolved against current authorization data. Index documents contain tokenized text, post/author IDs, timestamp, language, and version—not a permanent grant of access. An outbox event `{post_id, version, operation}` records create, edit, visibility change, or delete.

API: `GET /search/posts?q=...&cursor=...&time_range=...&author=...` returns post IDs and safe summaries; query is authenticated and bounded. `GET /posts/{id}` performs canonical authorization before returning full content. Cursor pagination includes a query/index snapshot or stable sort key to avoid duplicates where feasible.

## Architecture

Write service commits canonical post changes and an outbox event in one transaction. An indexer consumes events, tokenizes text, and updates a distributed inverted index partitioned by document ID or term, with replicas for availability. A query service parses and normalizes terms, fans out to relevant index shards, merges ranked candidates, and applies filters. An authorization service or compact access-control projection evaluates visibility, friend relationships, blocks, and viewer state. A post hydration service fetches canonical summaries in batches and verifies current version/access before response. Deletion and privacy-change streams propagate tombstones quickly.

## End-to-end flow

1. Search client sends query and authenticated viewer context. Query service validates syntax, applies limits, and normalizes supported language/analyzer settings.
2. Planner selects index shards from query terms, filters, and time range. Shards return bounded candidate IDs, scores, versions, and sort values.
3. Merge stage combines candidates, removes duplicates, and applies ranking signals such as textual relevance, freshness, and quality. It should not treat an index match as authorization.
4. Authorization filtering checks current visibility, blocks, and viewer relationship for candidates. Bulk checks reduce round trips; stale permission projections require conservative handling.
5. Hydration fetches canonical post records in batches, drops deleted or version-mismatched items, and constructs safe summaries.
6. Service returns results plus an opaque cursor. Subsequent pages repeat authorization/hydration checks; clients do not receive internal index fields.
7. In parallel, outbox events update the index. Tombstones and visibility changes prioritize fast removal; retry queues and reconciliation compare indexed versions with canonical records.

## Scaling, consistency, and reliability

The inverted index reduces query work from scanning all posts to traversing posting lists. Common terms create large lists, so shards use time partitions and top-N retrieval. Bound fan-out and prioritize selective terms. Ranking can evolve independently of canonical storage. Index replicas tolerate failures; return partial results only if clearly marked or retry within the latency budget.

Index writes are asynchronous and at-least-once. Versioned documents make duplicate and out-of-order events safe: apply only versions newer than the stored version, and retain tombstones long enough to defeat delayed upserts. Search discovery can lag slightly, but privacy revocation is more sensitive. Use fast invalidation plus authorization at read time and canonical hydration. If authorization dependency is unavailable, fail closed for restricted content; public-only results can remain available. Cache only with viewer-aware keys and short lifetimes, or cache candidate IDs rather than rendered private content.

Pagination is unstable as new posts arrive. Use a stable sort key such as `(score bucket, created_at, post_id)` and an opaque cursor; snapshot pagination costs more and is only needed if the product requires it.

## Deep dives and failure cases

**Privacy and stale indexes:** A post changing from public to friends-only may remain in an index replica or cache. Index tombstones are necessary but not sufficient. Every result must pass a current access check or a safely versioned authorization projection; hydration rechecks the canonical version. Purge viewer-facing caches on revocation where practical.

**Edits and deletion races:** An old edit event can arrive after a newer delete. Compare monotonically increasing post versions and keep a tombstone version so stale writes cannot resurrect content. Reconciliation scans identify missing or stale index documents after outages.

## Quick recall

Canonical posts plus transactional outbox; asynchronous inverted index; shard fan-out and ranking; then live authorization and canonical hydration. Version all updates, preserve tombstones, bound query work, and never equate searchability with permission to view.
