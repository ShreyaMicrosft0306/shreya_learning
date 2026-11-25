# Top-K Trending Items

## Scope and assumptions

Design a service that returns the most popular `K` items over a defined recent interval, such as articles, products, or search terms. Events arrive continuously and may be duplicated or late. Clients need fast top-K reads, while the result can be approximate and refreshed periodically. This design addresses counting and ranking; it does not define the product's relevance formula or recommendation policy.

## Requirements and exclusions

**Required:** ingest popularity events at high volume; compute rankings for a selected time window and partition; serve low-latency top-K queries; handle skew and event replay; expose freshness and approximation behavior.

**Excluded:** exact ordering at every instant, arbitrary ad hoc historical analytics, and personalized ranking. If rankings must be exact or financially consequential, a different strongly reconciled computation is needed.

## Data model and API

An event is `{event_id, item_id, event_type, event_time, partition_key}`. The aggregation key is `(window_id, item_id)` with a weighted count; event type can map to a configured weight. A result snapshot is `{scope, window_start, generated_at, items: [(item_id, score, rank)], watermark, algorithm_version}`.

APIs: producers publish to an ingestion topic; `GET /top-k?scope=...&window=...&k=...` returns a snapshot and freshness metadata; `GET /items/{id}` resolves display data separately. Restrict allowed `K`, windows, and scopes so callers cannot trigger unbounded computation.

## Architecture

An event gateway authenticates producers and validates schemas, then appends events to a partitioned durable log. Stream processors deduplicate within a bounded horizon, assign events to time buckets, and maintain per-item counters. Each worker maintains a local candidate set using a min-heap of size `K` or a larger candidate threshold, then periodically emits partial top candidates. A second aggregation stage merges candidates and writes versioned snapshots to a cache or key-value store. A metadata service provides item labels/images independently so hot ranking reads do not join a large catalog on every request.

## End-to-end flow

1. A producer emits an event with stable event ID, item, event time, and type. The gateway rejects malformed or unauthorized events and writes accepted records to the log.
2. Stream consumers read partitions, deduplicate event IDs for the configured retention horizon, and assign events to event-time buckets.
3. Workers update bucket counts. For a sliding window, they combine the active buckets and expire buckets outside the window; late events update retained buckets and may trigger corrected results.
4. Each worker publishes a candidate list and watermark at a bounded interval. A coordinator merges candidates by item and score, selects the global top K, and publishes an immutable snapshot version.
5. API reads fetch the latest snapshot from cache/store, optionally falling back to the prior version if refresh is incomplete. Response includes generated time and watermark.
6. An asynchronous repair job can recompute selected intervals from retained log data when duplicates, processor bugs, or late events require correction.

## Scaling, consistency, and reliability

Partition by item hash for even counter updates, but partitioning only by item means all events for a very popular item go to one worker. Salting the item key distributes hot counters; a later merge sums salts. For top-K correctness, local candidate pruning has a subtle risk: an item that is not locally top-K in any one shard can still be globally high after summing many shards. Keep more than K local candidates, merge hierarchical candidates, or use a two-pass threshold algorithm when accuracy matters. The candidate bound trades memory/network for recall.

Use event time and watermarks to balance freshness against late-arrival correctness. A short allowed-lateness interval yields fresh but revisable rankings; a longer one improves completeness but increases delay and state. Deduplication is bounded because indefinite event-ID state is expensive. At-least-once log delivery plus idempotent updates keyed by event ID avoids common retry inflation. Snapshots are atomically replaced by version so readers never see partial results.

For enormous cardinalities, approximate sketches reduce memory but can overestimate due to collisions. Pair them with exact candidate verification. Keep count semantics explicit: unique-user counts require additional state and privacy controls.

## Deep dives and failure cases

**Sliding window:** Exact per-event queues cost memory proportional to event rate. Bucketed counters bound state and make expiration cheap, at the cost of boundary error up to bucket width. Choose bucket size from product freshness and accuracy needs; do not imply exact rolling-window counts if buckets are coarse.

**Late data and replay:** A consumer can restart and replay records. Stable IDs plus idempotent state updates prevent double counting, and snapshots carry a watermark/version. If state is lost, rebuild from the durable log or a checkpoint; serve the last known snapshot with stale metadata during recovery rather than an empty ranking.

## Quick recall

Durable event log → deduplicating stream aggregation → bounded per-shard candidates → global merge → versioned cached snapshot. State the window, score, freshness, and approximation. Salt hot keys, use watermarks, and keep a rebuild path.
