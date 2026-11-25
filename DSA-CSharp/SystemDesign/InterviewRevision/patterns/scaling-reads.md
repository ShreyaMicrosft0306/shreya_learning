# Scaling reads

Read scaling increases how many queries a system can serve and how quickly they return. Begin by understanding the read pattern: point lookup, filtered list, aggregation, full-text search, or location query. A design that speeds one pattern may make writes slower or another query more expensive. Measure latency and load before adding layers.

## When to use

Improve the primary database first when the workload is still manageable: correct queries, suitable indexes, and bounded result sizes often solve the problem. Add caching or read replicas when repeated reads or read volume are a demonstrated bottleneck. Use a search index or analytical store when the query type is not a good fit for transactional storage, accepting that derived data may lag behind the source.

## Architecture and flow

An index is an auxiliary structure that helps a database find rows without scanning the entire table. Choose indexes that match common filters and sort order; composite indexes have an order, so the leftmost columns matter. Check query plans and avoid returning unused columns or unbounded result sets. Cursor pagination uses a stable ordered key to fetch the next page, avoiding the cost and shifting results of very large offsets.

A read replica receives changes from a primary database and serves read queries. Replication is often asynchronous, so a newly written value may not appear immediately on a replica. Route consistency-sensitive reads to the primary or use a session token/version to wait until the replica has caught up. Replicas distribute read load, but do not inherently improve the cost of a single expensive query.

A cache stores computed or fetched values for reuse. A cache-aside flow checks the cache, loads on miss, and stores the result with a time-to-live (TTL), meaning an expiry duration. The database remains authoritative. Cache keys must include relevant identity, filters, and authorization scope. Cache only data whose staleness and eviction behavior are understood.

## Tradeoffs and invalidation

Indexes speed selected reads at the cost of storage and write work. Replicas improve aggregate read capacity but add replication lag and operational overhead. Caches can make hot reads fast and reduce database traffic, but stale values, invalidation complexity, and cold-start load are common costs. TTL limits how long stale data can persist but does not guarantee immediate freshness.

For write-through or explicit invalidation, update the source and cache in a carefully defined sequence; a crash between steps can still leave inconsistency. Versioned keys, short TTLs, or change events can reduce risk. Stampede protection coalesces many simultaneous cache misses for the same key into one load. Negative caching can remember a “not found” result briefly, but be cautious when new records may appear.

## Failure modes and scaling

A cache outage can shift the full read load to the database; use timeouts, bounded concurrency, and graceful degradation. A hot key may overload one cache node; replicate or shard hot values, or vary TTL slightly to spread refreshes. Replicas can fall behind or be promoted incorrectly; expose lag metrics and define failover behavior. Schema changes can break caches and search indexes, so version keys and rebuild derived stores from a durable source.

Scale by query shape: partition large datasets only when growth justifies the routing and cross-partition costs. Keep response sizes bounded, use compression where appropriate, and set budgets for fan-out queries. Track p50/p95/p99 latency, cache hit ratio, query cost, replica lag, and database connection saturation; a high hit ratio alone does not prove correctness.

## Example prompts

- Scale a product catalog serving frequent filtered searches.
- Design a read-heavy news feed with acceptable freshness.
- Improve a dashboard with expensive repeated aggregates.

## Interview quick checklist

- What are the dominant query shapes, cardinalities, and freshness needs?
- Which indexes and pagination strategy match those queries?
- Are replicas safe for the required read-after-write behavior?
- What is the cache key, TTL, invalidation, and authorization scope?
- How does the system behave during cache miss storms or replica lag?
- Which latency, hit-rate, and query-cost metrics guide scaling?
