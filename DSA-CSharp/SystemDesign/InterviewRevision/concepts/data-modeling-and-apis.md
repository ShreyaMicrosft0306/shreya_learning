# Data Modeling and APIs

The data model should make the important queries and correctness rules explicit. Begin with access patterns, then choose entities, keys, indexes, and storage.

## API design checklist

For each operation, describe:

- Method and resource or command name.
- Required identifiers and validated fields.
- Response shape, pagination, and meaningful error cases.
- Authentication and authorization checks.
- Whether a retry can safely repeat the operation.

Use stable resource IDs, bounded pagination, and explicit filters. Avoid returning unbounded collections. For writes that clients may retry, accept an idempotency key and bind it to the caller and operation.

## Modeling workflow

1. List the top reads and writes.
2. Identify entities, ownership, and lifecycle transitions.
3. Decide which entities must change atomically.
4. Choose keys and indexes to match those access patterns.
5. Separate authoritative records from derived views.
6. Revisit the model when sharding or retention changes the query.

Example for a reservation system:

```text
Product(product_id, sale_state, price)
InventoryUnit(unit_id, product_id, state, reservation_id)
Reservation(reservation_id, user_id, product_id, expires_at, state)
Payment(payment_id, reservation_id, provider_reference, state)
```

The inventory unit or a guarded inventory counter is authoritative; a cached “remaining” count is only a display hint unless protected by a transactional claim.

## Storage choices

- **Relational:** transactions, constraints, joins, and flexible querying; watch write contention and horizontal partitioning complexity.
- **Key-value:** direct key lookups and high throughput; requires access patterns known in advance.
- **Document:** aggregate-shaped records and flexible attributes; large aggregates and cross-document invariants can be difficult.
- **Wide-column:** high-volume partitioned reads/writes; partition-key design is critical and ad hoc queries are limited.
- **Search index:** text retrieval, ranking, and filters; usually derived and eventually consistent, not the source of truth.
- **Object storage:** large immutable or versioned bytes; pair with metadata in a database and serve via signed URLs/CDN.

These are tendencies, not rules. A system can use several stores when each has a clear ownership boundary.

## Indexes and transactions

An index trades storage and write cost for faster reads. Build it for a concrete query shape, including filters, sort order, and pagination cursor. Monitor selectivity and avoid creating many overlapping indexes without a reason.

Keep transactions around the smallest state transition that must be atomic. Use constraints for invariants the database can enforce. For high contention, consider conditional writes, row locks, optimistic version checks, or an explicit reservation/queue.

## Events and derived data

An event records a committed state change for other consumers. The transactional outbox pattern writes the business row and an outbox row together; a publisher later delivers the event. Consumers should deduplicate by event ID and tolerate redelivery.

Derived systems such as search indexes, counters, feeds, analytics tables, and caches can lag. Define how to rebuild them from authoritative data or an event log.

## API and data-model pitfalls

- A read-modify-write sequence is not safe merely because each individual database call is atomic.
- A database transaction does not include an external payment provider or message broker.
- A unique key can prevent duplicate records but may not enforce a cross-row invariant.
- Denormalization speeds reads but requires an update, repair, and reconciliation strategy.
- Do not put an unbounded activity history inside a single user or document record.

