# Limited-Inventory Flash Sale

## Scope and assumptions

Design a purchase flow for a short, high-demand sale where inventory is finite. Many buyers may compete for the same items, and product pages can receive far more traffic than checkout. The system should avoid overselling while keeping the purchase path responsive. Payment processing is external and may take an uncertain amount of time.

## Requirements and exclusions

**Must:** show useful availability, allocate units fairly, prevent oversell, expire abandoned reservations, handle duplicate checkout attempts, and reconcile payment with inventory. Protect against bots and load spikes.

**Not in scope:** warehouse picking and shipping optimization, defining business fairness rules, or guaranteeing that every interested buyer gets a unit. The system should state whether allocation is first-come, lottery, or another policy.

## Data model and API

- `Inventory`: SKU, sellable quantity, reserved quantity, version, and allocation policy.
- `Reservation`: ID, buyer, SKU, quantity, expiry, state, idempotency key, and payment reference.
- `Order`: reservation, final amount, state, and created time.
- `InventoryLedger`: immutable reserve, release, and sale movements.

`GET /v1/items/{sku}` returns catalog details and coarse availability. `POST /v1/reservations` requests quantity with a client idempotency key. `POST /v1/orders/{id}/confirm` confirms checkout or payment. APIs enforce per-account limits and return explicit sold-out/queue states.

## Architecture

A CDN and cache absorb catalog reads; their availability numbers are approximate and never authorize purchase. An admission/queue service protects the origin and can issue signed queue tokens. The allocation service owns authoritative inventory and performs atomic conditional reservation, partitioned by SKU or inventory bucket. A durable order workflow coordinates reservation expiry, payment authorization/capture, and inventory commitment. A payment adapter handles external outcomes. A scheduler releases expired reservations, while reconciliation compares inventory ledger, orders, and payment reports. Bot/risk controls and rate limits sit before expensive state changes.

## End-to-end flow

1. Before sale opening, the catalog is cached and sale configuration, SKU limits, and inventory partitions are validated. A controlled admission policy prevents all visitors from reaching checkout at once.
2. A buyer obtains a queue token and submits a reservation request with SKU, quantity, and idempotency key.
3. The allocation service validates token, buyer limit, sale window, and request uniqueness. It atomically decrements available quantity and records a reservation with a short expiry; failure returns sold out or retry-later without creating an order.
4. The buyer begins checkout. The order workflow requests payment authorization using a stable payment idempotency reference. It does not release inventory merely because a browser is slow.
5. If authorization succeeds before expiry, the workflow conditionally changes the reservation to committed and creates an order. Capture occurs according to the business's risk policy; each transition is durable and idempotent.
6. If payment declines, the buyer cancels, or the reservation expires, the workflow releases inventory exactly once using a ledger movement. A late successful authorization is voided or routed to recovery rather than silently consuming a second unit.
7. Catalog availability is refreshed asynchronously for display. It may lag, but all purchase decisions pass through the authoritative allocator.
8. Reconciliation detects reservation/order/payment mismatches and pauses affected SKUs or routes cases for repair. Metrics monitor queue wait, reservation success, expiry, payment uncertainty, and oversell indicators.

## Scaling, consistency, and reliability

Use strong atomicity per SKU or allocation bucket; eventual consistency is acceptable for read-only availability. A single hot SKU can create a write hotspot, so pre-partition inventory into escrowed buckets or use a serialized allocator. Escrow allocation scales writes while preserving a global upper bound, but unused units in one bucket can appear unavailable elsewhere until rebalanced. Apply queueing and admission control before database contention, plus per-user quantity limits and bot signals. Keep reservations short and reclaim through idempotent expiry workers. Outbox-driven workflows survive process crashes. Payments and inventory cannot share a distributed transaction, so model explicit intermediate states and reconcile ambiguity instead of pretending they commit atomically.

## Deep dives

**Strict no-oversell versus throughput:** A single counter with atomic conditional decrement is simple and safe but may bottleneck on the most popular SKU. Escrowed sub-inventories spread load while requiring careful conservation of total units and controlled rebalance.

**Payment/inventory race:** A payment can succeed after a reservation expires. Use a consistent expiry policy, idempotent void/refund handling, and a recovery queue. Never grant a second reservation based only on a stale client view.

## Quick recall

Cache for reads, queue admission, atomically reserve authoritative stock, coordinate payment through a durable state machine, expire/release once, and reconcile. Inventory and payments are separate systems; explicit uncertainty states prevent oversell and double charge.
