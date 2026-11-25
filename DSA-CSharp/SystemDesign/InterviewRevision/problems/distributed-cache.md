# Distributed key-value cache

## Scope and assumptions

Design a low-latency, in-memory cache service for application data. Clients read and write keys, set expiration, and may invalidate entries. The cache is an optimization, not the durable source of truth: eviction or node loss may cause misses but must not corrupt authoritative data. Assume a large key space, uneven popularity, and multiple application instances. Cross-region global transactions and arbitrary SQL queries are excluded.

## Requirements and exclusions

- `GET`, `SET` with TTL, `DELETE`, and optional compare-and-set or version checks.
- Scale capacity and request throughput horizontally; tolerate node failures.
- Bound memory, handle hot keys, and make expiry behavior predictable.
- Exclude persistence guarantees, complex transactions, and exactly-once cache invalidation.

## Data model and API

Each entry is `(key, value, expires_at, version, size, last_access/recency)`. A key's namespace may include tenant or application ID to avoid collisions. Example API: `GET(key)`, `SET(key, value, ttl, if_version?)`, `DELETE(key)`, and `MGET(keys[])`. Responses distinguish not-found/expired from transport failure. Values have maximum size and optional serialization version. Mutations may return an opaque version token; clients should not rely on cache persistence.

## Architecture

A client library routes keys to shards using a stable hash ring or a fixed virtual-slot map. Each cache node holds a bounded in-memory hash table plus an eviction structure and expiry metadata. A control plane tracks node membership and publishes a versioned routing map. Nodes can replicate entries to one or more peers or rely on misses being refilled from the system of record, depending on cost and availability goals. Metrics track hit rate, memory, evictions, latency, replication lag, and per-key skew. Application-side request coalescing can prevent a burst of concurrent misses from overwhelming the backing database.

## End-to-end request/data flow

1. **Read:** Client hashes the namespaced key to an owner using the current routing epoch and sends a request. Node checks expiry and returns a value/version or miss. On miss, application fetches the source of truth and repopulates cache with a bounded TTL; single-flight coalescing reduces duplicate fills.
2. **Write:** Client routes `SET` to the owner. The node validates size and TTL, replaces the entry atomically on that node, optionally replicates asynchronously, then acknowledges according to the configured durability mode. The application usually writes the database first and invalidates or refreshes cache afterward.
3. **Membership change:** Control plane adds or removes nodes and issues a new map epoch. Clients refresh routing information. Keys remap gradually or through migration; during movement, the service may forward requests or accept misses to avoid serving an old owner indefinitely.
4. **Expiry/eviction:** Expired entries are removed lazily on access and by a background sweeper. When memory is full, an eviction policy such as segmented LRU or frequency-aware admission sacrifices less valuable entries.

## Scaling, consistency, and reliability

Hashing balances keys but not load: a hot key can overload one owner despite balanced capacity. Use replicas for read-heavy hot keys, request coalescing, and per-key rate controls. Consistent hashing or virtual slots limits remapping during node changes, while an explicit epoch prevents clients using incompatible maps forever. Expiration should use server clocks and bounded TTLs; clock skew and delayed invalidation still make cache data stale. For mutable database-backed data, write-through or cache-aside policies must define ordering. Database-first followed by invalidation avoids a cache write surviving a failed database write, but a reader can refill stale data in the gap; version checks, short TTLs, or transactional change events reduce this risk.

Node failure loses volatile entries. Replication improves hit continuity but costs memory and complicates writes; many systems instead accept misses and protect the backing store with rate limits and jittered refill. The service should fail open only where stale/missing data is safe; authorization decisions and financial invariants must be revalidated against authoritative state. If control-plane membership is unreachable, nodes can continue serving their last map briefly, then reject or redirect requests rather than silently misroute writes.

## Deep dives and failure cases

**Consistency versus availability:** A cache can offer per-key primary ordering, but synchronous replication increases latency and reduces write availability during partitions. Since the cache is reconstructible, asynchronous replicas or no replication may be preferable. Document that a successful write might disappear after owner failure unless stronger acknowledgement is enabled. Never treat a cache CAS as a substitute for a database transaction.

**Thundering herd and stale refill:** When a popular item expires, many clients may stampede the database. Use per-key single-flight, TTL jitter, stale-while-revalidate for safe content, and circuit breakers. A stale response is not acceptable for every domain, so expose freshness policy explicitly rather than applying stale serving universally.

## Quick recall

Hash keys to bounded in-memory shards; version routing maps; cache-aside with explicit freshness; TTL plus eviction; control hot keys and miss storms; decide whether replication is worth its consistency and memory cost.
