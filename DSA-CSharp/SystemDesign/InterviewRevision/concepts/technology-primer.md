# Technology Primer

Use this as a quick “what problem does this component solve?” reference. A component belongs in the design only when a requirement or measured bottleneck justifies its cost.

| Component | Useful for | Important trade-off / question |
|---|---|---|
| DNS | Mapping a service name to an address and directing users to regions. | How fast do changes propagate, and what is the failover behavior? |
| Load balancer | Distributing requests across healthy instances; L4 routes connections, L7 can route by HTTP properties. | How are health, session affinity, and overload handled? |
| API gateway | A common external boundary for routing, auth hooks, quotas, and request shaping. | Avoid placing core business logic in a gateway that becomes a bottleneck. |
| Relational database | Transactions, constraints, and relational queries. | What are the hot rows, indexes, and future partition strategy? |
| Key-value/document store | Predictable key-based access or aggregate-shaped records at scale. | Which queries are unsupported, and how are multi-record invariants protected? |
| Cache | Lower latency and backend load for repeatable reads or short-lived coordination. | What staleness is acceptable, and what happens on miss or outage? |
| Message queue | Buffering work and decoupling producers from consumers. | Define visibility, ordering, retries, deduplication, and dead-letter handling. |
| Event log/stream | Retained ordered events for multiple consumers, replay, and stream processing. | Partition order is usually scoped to a key, not global. |
| Search engine | Full-text retrieval, ranking, and filters. | It is normally a derived view; how far behind the source of truth may it lag? |
| Object storage | Durable, scalable storage for large blobs such as images, documents, and video. | Store metadata separately; use signed access and lifecycle policies. |
| CDN | Caching content near users and reducing origin bandwidth/latency. | How are cache keys, private content, invalidation, and stale responses controlled? |
| Worker pool | Executing asynchronous jobs with independent concurrency and scaling. | How are leases, retries, poison jobs, shutdown, and duplicate execution handled? |
| Distributed lock/lease | Coordinating temporary ownership of a shared resource across processes. | What protects against expiry, pauses, stale owners, and lock-service failure? Consider fencing tokens or database-level atomic writes. |
| WebSockets/SSE | Server-originated updates over a long-lived connection. | How are reconnects, heartbeats, fanout, and cross-instance connection routing handled? |
| Metrics/time-series store | Aggregating measurements over time and powering dashboards/alerts. | Control cardinality, retention, downsampling, and late data. |
| Geospatial index | Finding candidate records near a point or inside a region. | Retrieve candidates efficiently, then apply exact distance/business ranking. |
| Payment provider | Tokenized payment methods, authorization/capture, and external settlement. | Treat callbacks as duplicated/out of order; reconcile provider and internal state. |

## Transport choices

- **HTTP request/response:** simple, stateless interactions; use timeouts and bounded payloads.
- **Polling:** clients ask periodically; simple to operate, but adds repeated traffic and update delay.
- **Server-Sent Events:** one-way server-to-client stream over HTTP; simpler than a bidirectional channel when clients only receive updates.
- **WebSockets:** bidirectional persistent connection; useful for chat or interactive collaboration, with more connection lifecycle and routing work.
- **gRPC:** typed service-to-service RPC and streaming; useful inside a controlled environment, but not a substitute for defining ownership and failure behavior.

## Queue or synchronous call?

Keep a call synchronous when the user needs an immediate result and downstream work is predictably short. Use a queue when work is long, bursty, retryable, or can complete after the request returns. In either case, define timeouts, idempotency, and what the client sees while the outcome is unknown.
