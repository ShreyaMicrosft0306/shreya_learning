# Ticketmaster: high-demand ticketing

## Scope and assumptions

Design online sale of assigned seats, from event discovery through checkout and ticket delivery. Demand may far exceed supply. The primary invariant is that one seat cannot be sold twice; general admission can use quantity counters.

## Requirements and exclusions

**Key requirements:** browse events and seat maps; hold seats temporarily; purchase a hold; issue verifiable tickets; support cancellations/refunds; resist bots and traffic spikes; preserve an auditable ownership history.

**Out of scope:** venue scanners, dynamic pricing, resale economics, and zero-latency queues. Fair access and inventory correctness matter more than serving everyone immediately.

## Data model and API

`Event(event_id, venue_id, starts_at, sale_state)` and `Seat(seat_id, venue_id, section, row, number)` define inventory. `Offer(event_id, seat_id, price, currency, state, hold_id, hold_expires_at, version)` represents sale state. `Hold(hold_id, buyer_id, event_id, expires_at, status)` groups selected seats. `Order(order_id, buyer_id, hold_id, amount, payment_state, status)` records purchase. `Ticket(ticket_id, order_id, seat_id, signature_ref, status)` is issued only after payment succeeds.

`GET /events/{id}/seats` returns availability snapshots, not a promise. `POST /holds` requests seat IDs with an idempotency key and returns a short expiry. `POST /orders` purchases a valid hold with a payment token. `GET /orders/{id}` reports status and ticket delivery. APIs enforce account, device, and event-specific purchase limits.

## Architecture

Search replicas and cached maps serve discovery. A virtual waiting room meters access. Inventory owns seat state in a strongly consistent store, with persisted expiring holds. A checkout orchestrator handles payment; ticket service creates credentials after success. Rate limits and an audit log protect and explain the sale.

## End-to-end flows

1. **Sale opening:** Event configuration sets sale time and purchase limits. The waiting room admits users at a controlled rate using a signed queue token, limiting load on the seat service.
2. **Seat discovery:** Client fetches a current seat map; the interface labels availability as changing. It may show approximate open seats from a read model but does not reserve them.
3. **Hold:** Client submits desired seats. Inventory service validates queue token and atomically changes each seat from available to held, recording buyer, hold ID, expiry, and version. If any requested seat cannot be held, release any partial allocation or return an explicit partial result according to product policy.
4. The service responds with the hold deadline and price breakdown. A countdown uses server time; client clock is not authoritative.
5. **Checkout:** Client submits payment credentials against the hold. Orchestrator verifies hold ownership and expiry, creates an idempotent order attempt, and requests payment authorization/capture.
6. On success, the inventory transaction converts held seats to sold and records the order. The ticket service creates a unique signed ticket per seat and delivers it through the account/email channel.
7. On payment decline or timeout, query the provider using the idempotency reference. If payment definitively failed, release the hold; if outcome is uncertain, retain it briefly while reconciliation avoids charging twice. Expiry workers release abandoned holds.

## Scaling, consistency, and reliability

Seat writes need strong consistency. Conditional transitions prevent duplicate holds. Partition by event; a popular event can still be a hot shard. Admission control smooths bursts; cached seat maps are advisory.

Hold duration balances checkout time against locked inventory. Enforce expiry in the transaction converting held to sold, not only in cleanup. Idempotency keys protect retries. Durable logs support repair; ticket delivery is asynchronous, so account confirmation need not wait for email.

## Deep dives and failure cases

**Payment and inventory are not one transaction:** Capturing before finalizing risks charging for a lost seat; selling first risks unpaid reservations. Hold, authorize, finalize under the lease, then capture if supported. Otherwise use a durable saga with retry and void/refund compensation. Reconcile provider and order records.

**Queue fairness and abuse:** Use signed short-lived tokens, account/device limits, and bot scoring; shared IPs do not imply one person. If queue service fails, stop new admission rather than bypass inventory protection. Existing holds can use a protected checkout path.

## Quick recall

Seat reads can be stale; seat claims cannot. A metered waiting room protects the event shard, a short durable hold gives users time to pay, and conditional transitions enforce one owner. Make payment, expiry, ticket issuance, and reconciliation explicit states.
