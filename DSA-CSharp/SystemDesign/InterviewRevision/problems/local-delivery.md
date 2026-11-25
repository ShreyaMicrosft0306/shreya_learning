# Local delivery: on-demand convenience goods

## Scope and assumptions

Design local delivery for nearby goods fulfilled by stores or micro-fulfillment sites. A customer browses a location-specific catalog, orders, and receives goods by courier. Inventory can change between browsing and checkout; focus on fulfillment and dispatch rather than long-haul logistics.

## Requirements and exclusions

**Key requirements:** show availability and estimated delivery time; reserve stock during checkout; accept payment; coordinate picking and courier assignment; update order status; handle substitutions, cancellation, refunds, and delivery proof.

**Out of scope:** warehouse automation, large-fleet route optimization, and exact real-time browse inventory. Checkout must validate stock; catalog views may be stale.

## Data model and API

`Store(store_id, location, service_polygon, open_hours, status)` identifies a fulfillment point. `SKU(sku_id, description, restrictions)` defines sellable products. `Inventory(store_id, sku_id, on_hand, reserved, version)` holds sellable quantity. `Order(order_id, customer_id, store_id, status, total, payment_ref, created_at, version)` and `OrderLine(order_id, sku_id, quantity, unit_price, substitution_policy)` record the purchase snapshot. `Reservation(order_id, sku_id, quantity, expires_at, state)` tracks temporary stock claims. `Delivery(order_id, courier_id, eta, status, proof_ref)` covers handoff.

`GET /catalog?lat=&lon=&items=` returns nearby availability estimates and fees. `POST /orders/quote` validates a cart and gives a short-lived quote. `POST /orders` accepts the quote plus idempotency key and payment authorization token. `GET /orders/{id}` returns status and line outcomes. Courier-facing APIs accept offers and status transitions.

## Architecture

The API fronts catalog, pricing, and order services. A geospatial index chooses nearby stores; catalog search uses replicated products and cached stock snapshots. Inventory owns reservations in a transactional store. A durable order state machine coordinates payment, picking, and dispatch through events. Dispatch matches couriers by location and capacity; notifications report state.

## End-to-end flows

1. **Browse:** Client sends location and cart candidates. The location service filters active stores by distance, service area, hours, and restrictions. Catalog returns prices, estimated availability, and a delivery estimate labeled as provisional.
2. **Quote:** Pricing computes item, tax, fees, and promotions for a specific store. The quote has an expiry and version so checkout can detect price or inventory changes.
3. **Place order:** Client sends quote and idempotency key. Orchestrator creates a pending order, requests an atomic inventory reservation, and authorizes payment. Failures trigger compensation: release reserved items and void authorization.
4. **Confirm:** Once reservation and payment authorization succeed, order moves to accepted and enters the store's picking queue. The picker scans items and records actual quantities.
5. **Resolve exceptions:** If an item is unavailable, follow the customer's substitution rule or remove it and adjust totals. Reauthorize or partially capture according to the payment provider's supported flow; notify the customer for approval when required.
6. **Dispatch and deliver:** Dispatch offers a pickup to an eligible courier. Accepted courier gets store instructions and route details. Status transitions—arrived, picked up, delivered—are recorded, with proof or customer confirmation.
7. **Close:** Capture final amount, release unused reservations, and issue refund/credit if fulfillment differs from the authorized cart. Emit events for receipts and analytics.

## Scaling, consistency, and reliability

Inventory reservations are the core consistency boundary. Use a conditional update such as `on_hand - reserved >= requested` and increment `reserved` in one transaction per store/SKU, with a unique order key to make retries safe. Multi-item carts may span many SKU rows; reserve them atomically where practical, otherwise use a short-lived reservation workflow that rolls back acquired rows if any item fails. Keep reservation expiration and cleanup idempotent.

Catalog reads may be stale because checkout validates authoritative inventory. Partition by store. Version-check order transitions so duplicate callbacks cannot advance state twice. An outbox ensures committed orders reach picking and dispatch. Payment and inventory require compensating actions, not a distributed transaction.

## Deep dives and failure cases

**Inventory versus checkout speed:** Holding stock during browsing reduces availability and invites abandoned-cart hoarding. Quote without holding; reserve at checkout for a short TTL. Handle racing orders with an explicit out-of-stock response. During picking, release quantities not fulfilled.

**Order orchestration:** Payment may succeed despite a timeout, or reservation may commit before a crash. Idempotency keys and provider references allow recovery by querying participants. A reconciler resumes or compensates stuck orders. Keep picking, pickup, and delivery separate; courier acceptance is not fulfillment.

## Quick recall

Browse from location-aware, eventually consistent catalog data; validate and reserve authoritative stock at checkout. Orchestrate payment, picking, and dispatch through a durable state machine with idempotent steps and compensations. Picking exceptions and courier delays are first-class order states, not edge cases.
