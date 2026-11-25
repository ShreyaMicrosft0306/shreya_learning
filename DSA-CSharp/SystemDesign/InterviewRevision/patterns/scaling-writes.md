# Scaling writes

Write scaling is about accepting and applying changes without losing correctness or overwhelming storage and downstream services. First identify what limits writes: transaction contention, indexes, disk or network bandwidth, synchronous dependencies, or uneven key distribution. Increasing server count helps only when work can actually be distributed.

## When to use

Batching helps when many small writes can be grouped into fewer database or network operations. A queue is useful when producers arrive in bursts and consumers can process later. Partitioning becomes relevant when one database cannot handle data volume or write throughput and the workload can be divided by key. These mechanisms add latency and operational complexity; use them after measuring a bottleneck and defining acceptable consistency.

## Architecture and flow

Batching groups multiple records into one operation, amortizing round-trip and transaction costs. Bound batches by size and waiting time so low-volume traffic is not delayed indefinitely. Preserve per-record results where partial failure is possible, and make retries safe with unique identifiers or idempotent upserts.

A queue separates request acceptance from processing. The API can durably accept an event and acknowledge it, while consumers apply the change. “Durably” means the system can recover the accepted work after a process failure. A transactional outbox can ensure a database update and the intent to publish are committed together. Consumers should tolerate duplicate events, track progress, and expose backlog age.

Partitioning (also called sharding) divides records across independent storage groups using a partition key. A routing rule maps each key to a partition. Choose a key that distributes writes and aligns with common reads. A monotonically increasing key may concentrate new writes on one partition; a random key can spread writes but make range queries harder. Rebalancing or changing the mapping requires a migration plan.

## Tradeoffs and backpressure

Batching reduces overhead but increases per-item latency and can make retries larger. Queues absorb short bursts, but cannot create infinite capacity: if arrivals persistently exceed processing, backlog grows without bound. Backpressure is the signal or policy that slows producers when downstream capacity is limited. Apply admission limits, quotas, smaller concurrency, and explicit rejection or retry-after responses.

Partitioning increases parallel capacity and can isolate failures, but cross-partition transactions, global ordering, uniqueness, joins, and reporting become harder. Replication is not the same as partitioning: replicas copy data, while partitions split it. A common pattern is a partitioned write store with replicated or derived read views, but the views may lag.

## Failure modes and scaling

Consumers may crash after applying a write but before acknowledging the message, causing redelivery. Use idempotency keys and durable offsets or acknowledgments. Poison messages that always fail should be quarantined for inspection rather than retried forever. A hot partition can remain overloaded even when other partitions are idle; monitor per-key and per-partition rates and consider splitting the hot key or changing write semantics.

During overload, prioritize critical traffic and shed optional work before all requests slow down. Limit batch sizes, queue depth, retries, and downstream concurrency. Retries need exponential backoff with jitter to avoid synchronized retry storms. Track accepted versus completed rates, backlog age, per-partition skew, write latency, and error rates. Define what the user sees when work is accepted but not yet reflected in reads.

## Example prompts

- Design write ingestion for telemetry from millions of devices.
- Scale reactions and counters during a major live event.
- Design bulk updates for a large product catalog.

## Interview quick checklist

- What is the actual write bottleneck and target throughput?
- Which work can be batched or processed asynchronously?
- What durability and consistency is promised at acknowledgment?
- What partition key balances writes and supports important reads?
- How are duplicates, ordering, poison events, and retries handled?
- How does backpressure prevent an unbounded queue or overload?
