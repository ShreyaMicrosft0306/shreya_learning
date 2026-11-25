# Metrics and Monitoring Platform

## Scope and assumptions

Design a service that ingests time-series measurements and events from applications and infrastructure, supports dashboards and alert evaluation, and retains data at useful resolutions. Producers may be disconnected, send late samples, or emit malformed/high-cardinality labels. The system should help operators detect and investigate problems; it does not itself guarantee service recovery.

## Requirements and exclusions

**Must:** accept high-volume writes, query recent and historical series, evaluate alerts with bounded delay, retain data under a storage budget, and expose its own health.

**Not in scope:** distributed tracing or log search in full, though both may link to metrics. Avoid unlimited label cardinality and arbitrary unbounded query work.

## Data model and API

- `MetricSeries`: metric name, normalized label set, unit, and schema/version.
- `Sample`: series key, timestamp, numeric value or histogram bucket, and optional exemplar.
- `Rule`: expression, evaluation interval, threshold, pending duration, labels, and notification route.
- `AlertState`: rule, series, state, transition time, and deduplication key.

Agents batch `POST /v1/write` samples with timestamps and resource identity. `POST /v1/query` accepts a constrained range query and time bounds. Rule APIs create and inspect alerts. Use a standard line protocol or protobuf internally, with schema validation and explicit units.

## Architecture

Local agents scrape or receive measurements, buffer short outages to disk within a quota, and batch compressed writes. An ingestion gateway authenticates, applies schema and cardinality controls, and hashes series identity into partitioned durable logs. Stream processors validate, deduplicate where possible, and write hot data to a time-series store optimized for recent ranges. Compaction workers create downsampled aggregates and move older blocks to cheaper object storage. Query frontends enforce limits and merge results from hot and cold tiers. Rule evaluators consume recent partitions on schedule and publish state transitions to an alert manager, which groups, silences, and routes notifications. Metadata and configuration use a strongly consistent control store.

## End-to-end flow

1. An agent samples a target, attaches bounded labels and timestamps, and appends points to a local buffer.
2. A batch is compressed and sent with tenant identity. Gateway authentication, quota checks, label validation, and size limits reject malformed or abusive traffic early.
3. Accepted points are partitioned by series/time and appended to a durable log before acknowledging. Retries may repeat samples, so ingestion defines duplicate semantics rather than assuming exactly once.
4. Consumers write samples into time-partitioned hot blocks and update series indexes. Late points are accepted within a configured window; older points may be rejected or routed to backfill.
5. Compaction merges blocks, builds rollups, and expires data by retention tier. It preserves sufficient metadata to explain resolution changes.
6. A query planner checks tenant limits, chooses raw versus downsampled data, fans out to relevant partitions, and merges results under a deadline.
7. Rule evaluators query bounded windows, apply pending duration and missing-data policy, and emit only alert-state transitions. The alert manager deduplicates and groups notifications.
8. Operators inspect platform lag, dropped samples, cardinality, query latency, and alert-evaluation delay through a separate protected health path.

## Scaling, consistency, and reliability

Shard writes by a stable hash of series plus time bucket; rebalance partitions and separate tenants with noisy-neighbor controls. Storage is append-heavy, so use immutable time blocks and background compaction instead of frequent random updates. Queries can be eventually consistent by a bounded ingestion delay, but dashboards should expose freshness. The durable log absorbs bursts; backpressure and per-tenant quotas prevent one producer from exhausting capacity. Agent disk buffers are bounded, and dropped-data counters must be visible. Query timeouts, point limits, and concurrency budgets protect the cluster. Multi-region replication adds cost and duplicate/ordering complexity; choose based on recovery objectives and explicitly define failover ownership.

## Deep dives

**Cardinality explosion:** Labels like unique user IDs create a new series per value and can exhaust indexes and memory. Restrict label keys, estimate cardinality at ingestion, reject or quarantine unsafe dimensions, and use logs/traces for high-entropy identifiers.

**Downsampling tradeoff:** Rollups reduce storage and query cost but lose spikes and distribution detail. Retain raw data for a shorter period, preserve histogram distributions or percentiles carefully, and make query resolution explicit.

## Quick recall

Bounded agents → validated durable ingestion → partitioned hot blocks → compaction/retention → cost-aware queries → stateful alert transitions. Cardinality and query budgets are first-class reliability controls.
