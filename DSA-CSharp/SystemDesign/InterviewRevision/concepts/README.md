# Concepts: Index and Coverage Checklist

Use this page as a revision map. The goal is to recognize the decision being made, state the invariant it protects, and explain the trade-off rather than recite technology names.

## Course-section coverage map

This is a topic-level cross-check to help you navigate the source material. The notes are independently written, not a reconstruction of its lessons.

| Source section | Independent notes in this pack |
|---|---|
| [Introduction and assessment](https://www.hellointerview.com/learn/system-design/in-a-hurry/introduction) | [Interview workflow](interview-workflow.md), [operations and communication](operations-security-and-communication.md) |
| [How to prepare](https://www.hellointerview.com/learn/system-design/in-a-hurry/how-to-prepare) | [Preparation plan](preparation-plan.md) |
| [Delivery framework](https://www.hellointerview.com/learn/system-design/in-a-hurry/delivery) | [Interview workflow](interview-workflow.md), [data modeling and APIs](data-modeling-and-apis.md) |
| [Core concepts](https://www.hellointerview.com/learn/system-design/in-a-hurry/core-concepts) | [Networking and estimation](networking-and-estimation.md), [data modeling and APIs](data-modeling-and-apis.md), [scaling and data systems](scaling-and-data-systems.md), [reliability and correctness](reliability-and-correctness.md) |
| [Key technologies](https://www.hellointerview.com/learn/system-design/in-a-hurry/key-technologies) | [Technology primer](technology-primer.md), [quick glossary](glossary.md), [pattern index](../patterns/README.md) |
| [Common patterns](https://www.hellointerview.com/learn/system-design/in-a-hurry/patterns) | [Pattern index](../patterns/README.md) and its eight linked pattern notes |
| [Problem breakdowns](https://www.hellointerview.com/learn/system-design/in-a-hurry/problem-breakdowns) | [Problem index](../problems/INDEX.md): 32 official walkthroughs plus four independent More Practice examples |

## Topic index

- [Interview workflow](interview-workflow.md): clarify scope, prioritize requirements, estimate, model, design, deepen, summarize.
- [Preparation plan](preparation-plan.md): build fundamentals, run deliberate practice, review, and calibrate depth.
- [Data modeling and APIs](data-modeling-and-apis.md): entities, access patterns, interfaces, indexes, transactions, idempotency, events.
- [Networking and estimation](networking-and-estimation.md): request paths, transport choices, rough capacity math, and availability intuition.
- [Scaling and data systems](scaling-and-data-systems.md): capacity estimates, load balancing, replication, sharding, consistent hashing, caching, search, object storage.
- [Reliability and correctness](reliability-and-correctness.md): consistency, concurrency, queues, retries, workflow state, failure handling.
- [Operations, security, and communication](operations-security-and-communication.md): observability, SLOs, privacy, security boundaries, rollout, communicating trade-offs.
- [Technology primer](technology-primer.md): what common infrastructure components do and the trade-offs to ask about.
- [Quick glossary](glossary.md): concise definitions of distributed-systems terms used throughout this pack.
- [Patterns](../patterns/README.md): reusable solutions to recurring system pressures.

## Coverage checklist

### Framing and problem solving

- [ ] Clarify users, use cases, scale, latency, availability, durability, and data-retention needs.
- [ ] Separate must-have functional behavior from quality attributes and explicit non-goals.
- [ ] Make assumptions visible; identify the invariant that must never be violated.
- [ ] Prioritize the highest-risk flow before adding optional features.
- [ ] Leave time to explain trade-offs and summarize the design.

### APIs and data

- [ ] Define operations, inputs, outputs, pagination, errors, and authorization boundaries.
- [ ] Model entities and relationships around the dominant read/write paths.
- [ ] Choose relational, document, key-value, wide-column, graph, or search storage based on query and consistency needs.
- [ ] Use indexes to support actual predicates and sort order; account for write amplification.
- [ ] Identify transaction boundaries and concurrency controls.
- [ ] Make retryable mutations idempotent and define deduplication scope.
- [ ] Distinguish source of truth from derived indexes, caches, and analytics views.

### Scale and distributed systems

- [ ] Estimate request rates, data growth, bandwidth, fanout, and hot-key pressure at an order-of-magnitude level.
- [ ] Separate stateless compute from stateful storage; scale each independently.
- [ ] Explain load balancing, replication lag, partition keys, rebalancing, and hot partitions.
- [ ] Choose cache keys, TTL/invalidation behavior, and acceptable staleness.
- [ ] Decide where queues absorb bursts and where synchronous backpressure is preferable.
- [ ] Include CDN/object storage for large or cacheable media when appropriate.
- [ ] Explain search indexing and eventual consistency when search is derived from transactional data.

### Correctness, resilience, and operations

- [ ] State consistency guarantees per operation, not just for the whole system.
- [ ] Protect inventory, balances, uniqueness, ordering, and ownership with explicit invariants.
- [ ] Define timeout, retry, circuit-breaker, dead-letter, and replay behavior.
- [ ] Consider partial failure, duplicate delivery, out-of-order events, and recovery.
- [ ] Cover authentication, authorization, privacy, abuse controls, and sensitive-data handling.
- [ ] Name useful metrics, logs, traces, alerts, and service-level objectives.

## Pattern recognition

When a prompt describes a dominant pressure, use the matching pattern as a starting point:

| Signal in the problem | Start with |
|---|---|
| Clients need updates without repeatedly asking | [Realtime updates](../patterns/realtime-updates.md) |
| Work takes longer than a request/response should | [Long-running tasks](../patterns/long-running-tasks.md) |
| Concurrent actions can violate a shared invariant | [Contention and concurrency](../patterns/contention.md) |
| Reads greatly outnumber writes | [Scaling reads](../patterns/scaling-reads.md) |
| One database cannot sustain write throughput | [Scaling writes](../patterns/scaling-writes.md) |
| Payloads are large or expensive to serve | [Large blobs](../patterns/large-blobs.md) |
| A business action spans multiple services | [Multi-step processes](../patterns/multi-step-processes.md) |
| Results depend on distance or geographic cells | [Proximity services](../patterns/proximity-services.md) |
