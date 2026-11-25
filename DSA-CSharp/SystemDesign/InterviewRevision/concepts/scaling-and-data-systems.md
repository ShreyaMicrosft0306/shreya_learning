# Scaling and Data Systems

Scale the resource that is actually constrained. Estimate workload first, then choose the least complex technique that meets the requirement.

## Capacity sketch

Estimate:

- average and peak requests per second by operation;
- read/write ratio and burstiness;
- bytes per request and response;
- retained records and growth per day;
- fanout per write or read;
- hot-key concentration and geographic distribution.

Use powers of ten and label assumptions. Capacity math should identify pressure points, not give false precision.

## Compute and traffic

Keep request handlers stateless where practical so instances can be added behind a load balancer. Store sessions or workflow state in a shared durable store when requests may land on different instances. Apply admission control before an overloaded dependency becomes unavailable.

Scale vertically for a simple early system; scale horizontally when a single node becomes a throughput or availability limit. Horizontal scale introduces coordination, partitioning, retries, and operational work.

## Replication

Replicas improve read capacity and availability, but replication lag means a read may not immediately reflect a successful write. Choose a consistency contract per operation:

- Read-your-writes can route a user's immediate follow-up to the primary.
- Eventual consistency may be acceptable for feeds, analytics, and search.
- Stronger coordination may be required for balances, reservations, and uniqueness.

Failover also needs a policy for stale replicas, split-brain prevention, and recovery.

## Partitioning and consistent hashing

Sharding distributes records by a partition key. A good key spreads load while keeping common queries local. A bad key creates hot partitions or forces scatter-gather queries.

Hash partitioning is simple but may move many keys when the shard count changes. Consistent hashing maps keys and nodes to a ring, limiting movement during membership changes; virtual nodes can smooth uneven capacity. It does not solve hot keys, data migration, or cross-shard transactions by itself.

Plan for rebalancing, shard discovery, repair, and partition-key evolution.

## Caching

Use caches for data that can tolerate the cache's consistency model. Define:

- key composition and tenant/user isolation;
- TTL and invalidation/update strategy;
- behavior on cache miss or cache outage;
- protection against stampedes and hot keys;
- what data must never be served stale.

Common patterns include cache-aside for reads, write-through for coordinated updates, and write-behind for deferred persistence (with careful durability implications). A cache should not silently become the only copy of critical data.

## Search, media, and data pipelines

Search engines serve ranked text queries, not transactional truth. Send changes through an outbox or event stream and accept/measure indexing delay. Large media belongs in object storage; upload directly with short-lived signed credentials, store metadata separately, and distribute downloads through a CDN.

For stream processing, define partitioning, event-time windows, late data, deduplication, checkpointing, and replay. For batch jobs, define input snapshots, retries, output idempotency, and progress tracking.

