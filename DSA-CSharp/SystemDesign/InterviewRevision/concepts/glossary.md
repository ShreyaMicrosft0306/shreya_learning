# Quick Glossary

Short definitions for terms that recur in the concept notes and problem walkthroughs.

| Term | Meaning | Why it matters |
|---|---|---|
| At-least-once delivery | A message may be delivered again until acknowledged. | Consumers must handle duplicates safely. |
| Backpressure | Slowing or rejecting producers when consumers cannot keep up. | Prevents an overloaded queue or dependency from growing without bound. |
| Bloom filter | Space-efficient probabilistic structure that can say “definitely absent” or “possibly present.” | Can avoid some expensive lookups; false positives are possible. |
| Cache stampede | Many requests recompute or reload the same expired cache entry at once. | Use request coalescing, jittered TTLs, or stale-while-revalidate. |
| Change data capture (CDC) | Publishing database changes from a transaction log or change stream. | Feeds derived indexes and downstream services with less dual-write risk. |
| Circuit breaker | Temporarily stops calls to a failing dependency. | Gives the dependency time to recover and protects caller capacity. |
| CQRS | Separating command/write models from query/read models. | Useful when read and write needs differ; adds synchronization complexity. |
| Consistent hashing | A key-to-node mapping that limits remapping when nodes change. | Helps distribute cache or partition ownership; does not eliminate hot keys. |
| Dead-letter queue | A holding area for messages that repeatedly fail processing. | Preserves problematic work for inspection instead of retrying forever. |
| Eventual consistency | Replicas or derived views converge after some delay. | Acceptable for many feeds and indexes, but not every invariant. |
| Fanout-on-read | Assemble a result by fetching from sources when the user requests it. | Reduces write amplification; can increase read latency and work. |
| Fanout-on-write | Precompute or push updates to recipient-specific views when content changes. | Speeds reads; can make writes expensive for high-follower or high-volume accounts. |
| Fencing token | A monotonically increasing token attached to ownership of a lease/lock. | Prevents an expired lock holder from writing after a newer owner takes over. |
| Hot key | A key or partition receiving a disproportionate share of requests. | Can bottleneck an otherwise well-sharded system. |
| Idempotency key | A client-supplied identifier used to recognize retries of the same operation. | Prevents duplicate effects such as double orders or charges. |
| Lease | Time-bounded ownership that expires unless renewed. | Useful for distributed coordination; expiry alone does not stop a stale owner without fencing. |
| Materialized view | A stored, precomputed representation of a query. | Speeds reads but requires refresh and repair logic. |
| Quorum | A minimum number of replicas that must participate in a read or write. | Tunes availability and consistency; behavior depends on replication and conflict rules. |
| Read repair | Updating stale replicas during or after a read. | Helps replicas converge but does not replace a full repair strategy. |
| Saga | A multi-step business workflow with durable progress and compensating actions. | Coordinates services without a global database transaction. |
| SLI / SLO | A measured service indicator / target for that indicator. | Connects operational alerts and release decisions to user experience. |
| Write amplification | Extra writes caused by indexes, replicas, logs, or derived views. | Can make a write-heavy design hit storage limits sooner than raw request counts suggest. |
| Watermark | A statement of how far event-time processing believes input has arrived. | Helps streaming systems handle late and out-of-order events. |

