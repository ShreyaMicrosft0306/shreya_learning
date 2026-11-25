# Networking and Capacity Estimation

This note combines two skills that quickly expose whether an architecture is plausible: tracing a request across the network and estimating the order of magnitude of its load.

## Trace the request path

A typical public request passes through:

1. DNS resolution to find a service or edge location.
2. A transport connection and TLS when using HTTPS.
3. A CDN or edge cache for eligible content.
4. A load balancer and, if present, an API gateway.
5. An application service, which validates identity, authorization, and input.
6. A database, cache, queue, or downstream service.
7. A response to the client, or an acknowledgment plus an asynchronous job ID.

Each network hop can add latency or fail independently. Use timeouts, propagate request IDs, bound payload sizes, and avoid holding resources while waiting on slow dependencies.

## Transport and protocol choices

- HTTP request/response is a good default for bounded synchronous work.
- Polling is easy to understand but repeats requests and has a delay/traffic trade-off.
- Server-Sent Events support one-way server-to-client updates over HTTP.
- WebSockets support bidirectional communication but require connection lifecycle, heartbeats, reconnect behavior, and routing to the instance holding the connection.
- RPC such as gRPC can provide typed internal APIs and streaming; it still needs deadlines, retries, and compatibility rules.

Prefer the simplest protocol that meets latency and interaction requirements. A persistent connection does not by itself provide durable delivery; pair it with stored state or sequence/cursor semantics where needed.

## Back-of-the-envelope request math

Use round figures and state assumptions:

```text
average requests/s = daily requests / 86,400
peak requests/s = average requests/s * peak-to-average factor
daily storage growth = writes/day * average bytes/write
network bandwidth = requests/s * average bytes/request
```

Helpful sanity checks:

- 1 million operations per day is about 12 operations per second on average.
- 1 billion operations per day is about 11.6 thousand per second on average.
- A 1 KB response at 10,000 responses/s is roughly 10 MB/s before protocol overhead.
- Replication, indexes, backups, and derived data multiply raw storage needs.

Peak load, retries, fanout, and uneven key popularity can matter more than the average.

## Latency and availability intuition

Treat latency estimates as rough, hardware- and region-dependent orders of magnitude, not service-level guarantees:

- CPU and memory operations are much faster than storage or network round trips.
- Local SSD access is typically far faster than a cross-region request.
- A cache hit can avoid a database round trip; it cannot remove client-to-service network latency.
- Each sequential dependency adds latency; parallel independent reads are bounded by the slowest response plus coordination.

Availability targets also compound across serial dependencies. If every dependency must succeed for a request to succeed, the end-to-end availability can be lower than any one component’s availability. Redundancy helps only when failover works and shared failure modes are addressed.

Approximate annual downtime at a constant availability target:

| Availability | Downtime per year |
|---|---:|
| 99% | about 3.65 days |
| 99.9% | about 8.8 hours |
| 99.99% | about 53 minutes |
| 99.999% | about 5 minutes |

## Estimation pitfalls

- Do not confuse average throughput with peak or burst throughput.
- Include metadata, indexes, replication, retries, and retention in storage estimates.
- For fanout systems, count downstream work per event, not only incoming requests.
- Separate payload bytes from metadata bytes and network protocol overhead.
- Use the estimate to motivate a design decision; do not overfit the design to uncertain arithmetic.

