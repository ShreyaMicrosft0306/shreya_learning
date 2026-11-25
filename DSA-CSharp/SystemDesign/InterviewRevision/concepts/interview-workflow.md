# Interview Workflow

A good design is a sequence of justified decisions. Keep returning to the requirement being served; do not begin with a list of fashionable components.

## What the interview is assessing

Expect the interviewer to observe several dimensions together:

- **Problem navigation:** can you reduce ambiguity, set scope, and keep moving?
- **Solution design:** can you create a coherent system whose components support the stated use cases?
- **Technical judgment:** can you reason about data, scale, consistency, failures, and trade-offs?
- **Communication and collaboration:** can you explain your thinking, invite constraints, and adjust when new information appears?

The diagram is only one part of the signal. Make assumptions and alternatives visible as you work.

## 1. Clarify the task

Ask who the users are, what they need to do, what is explicitly out of scope, and which constraints matter most. For a feed, for example, reading a page may matter more than editing an old post. For payments, preventing duplicate charges matters more than shaving a few milliseconds from a rare admin operation.

Write down assumptions rather than quietly relying on them. If a number is unknown, choose a reasonable illustrative value and say it can be revised.

## 2. Rank requirements

Separate:

- **Functional requirements:** actions and results the system must provide.
- **Non-functional requirements:** latency, availability, durability, correctness, privacy, and cost targets.
- **Non-goals:** useful features deliberately left out of the first design.

Pick a small number of requirements to drive the architecture. A design that claims to optimize every quality attribute at once usually hides its trade-offs.

## 3. Estimate the shape of the workload

Use round numbers. Estimate active users, operations per user, peak-to-average ratio, read/write mix, object sizes, and retention. Then derive rough requests per second, storage growth, and bandwidth.

For example:

```text
daily operations = active users * operations per user per day
average operations/s = daily operations / 86,400
peak operations/s = average operations/s * peak multiplier
storage growth/day = writes/day * average stored bytes
```

The estimate is a tool for choosing between one database and partitions, synchronous work and queues, or database reads and caching. It is not a prediction to several decimal places.

## 4. Model state and interfaces

Name the core entities, their ownership, relationships, and lifecycle. Write a few APIs or events that correspond to the critical flows. Decide which store owns the authoritative state and which data is derived.

State the invariants: examples include “a unit is reserved at most once,” “a payment is not captured twice,” and “only a document member can read its content.”

## 5. Draw the simplest useful architecture

Start with the client, API boundary, core service, and source-of-truth data store. Add a cache, queue, search index, stream, or object store only when a requirement needs it.

Trace one write and one read end-to-end. Call out where authentication, validation, transactions, and state changes happen.

## 6. Deepen the risky parts

Pick the highest-risk bottlenecks from requirements and estimates. Typical examples are hot keys, write contention, fanout, large payloads, global ordering, workflow retries, and geographic queries.

For each deep dive, explain:

1. What fails in the baseline design?
2. What invariant or service objective is threatened?
3. What is the smallest change that helps?
4. What new complexity or failure mode does it introduce?

## 7. Close with trade-offs and failure behavior

Cover partial failure, timeouts, retries, duplicate requests, stale data, and recovery. State which operations may be eventually consistent and which must be strongly protected.

Finish with a concise recap: components, data ownership, critical flow, primary bottleneck, and the most important trade-off.

## Frequent mistakes

- Designing for a guessed scale before clarifying the workload.
- Introducing microservices before identifying independent ownership or scaling needs.
- Treating a cache as the source of truth.
- Saying “use a queue” without explaining ordering, retries, idempotency, or poison messages.
- Claiming exactly-once processing when the practical design is at-least-once delivery plus deduplication.
- Listing technologies without tracing requests or explaining failure behavior.
- Adding a global transaction across services instead of defining a workflow and compensation.
