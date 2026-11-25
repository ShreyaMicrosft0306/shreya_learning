# Brokerage and Trading Platform

## Scope and assumptions

Design a retail brokerage backend for account balances, order entry, order routing, and execution updates for listed securities. A broker connects to external venues or market makers; it does not itself guarantee execution price or availability. This walkthrough focuses on the order lifecycle and money/securities accounting, not investment advice, tax reporting, or every regulatory workflow.

## Requirements and exclusions

**Must:** authenticate and authorize account actions, validate orders against buying power and trading rules, avoid duplicate submissions, track partial fills and cancellations, maintain an auditable ledger, and recover safely from venue or network failures. Market data should be clearly separated from authoritative execution records.

**Not in scope:** matching engine design, portfolio optimization, custody infrastructure, or jurisdiction-specific compliance rules. Those require specialist systems and policy.

## Data model and API

- `Account`: owner, status, permissions, and trading constraints.
- `CashLedgerEntry`: immutable debit/credit, currency, reason, reference, and effective time; balance is a projection of entries.
- `PositionLedgerEntry`: signed quantity changes and settlement metadata.
- `Order`: client ID, account, instrument, side, type, quantity, limit/stop price, time-in-force, state, version, and venue reference.
- `Execution`: unique venue execution ID, order ID, quantity, price, fees, and timestamp.

`POST /v1/accounts/{id}/orders` accepts a client-generated idempotency key and order parameters. `GET /v1/orders/{id}` returns lifecycle state and fills. `DELETE /v1/orders/{id}` requests cancellation; it does not assert that the order was cancelled until the venue confirms it. Streaming updates are advisory; clients can reconcile through the API.

## Architecture

An API gateway authenticates the user and enforces account-level authorization and request limits. Order service validates syntax and trading policy. A risk/reservation service atomically checks buying power and reserves funds or sellable quantity. A durable order journal and transactional outbox publish accepted orders to a router. The router selects eligible venues under routing policy and communicates via venue adapters. An execution ingestion service deduplicates venue reports and updates order state and immutable ledgers. Reconciliation jobs compare internal orders, fills, and balances with venue and clearing records. Read models provide history and portfolio views without becoming the accounting authority.

## End-to-end flow

1. The client submits an order with a unique idempotency key. Authentication binds it to the account; duplicate keys return the original order result.
2. Validation checks market session, instrument status, quantity/price bounds, order type, account restrictions, and freshness of required reference data.
3. Risk evaluation computes a conservative reservation, then atomically records the reservation and accepted order. If either fails, neither is committed.
4. An outbox event is delivered to the router. The router records each attempt and sends a venue-specific request with a stable client reference.
5. Venue acknowledgements, rejects, fills, and cancel responses arrive asynchronously. Ingestion verifies signatures/transport, deduplicates event IDs, and applies legal state transitions.
6. Each fill writes execution facts and corresponding cash/position ledger entries exactly once by execution ID. Projections update account views and notify clients.
7. A cancel request moves the order to cancel-pending; any racing fill is still booked. Once venue status is authoritative, unused reservation is released.
8. Reconciliation identifies missing or contradictory reports and routes them to controlled repair, never silently rewriting the ledger.

## Scaling, consistency, and reliability

Shard by account or order ID while preserving atomic reservation and order acceptance for one account. The critical write path favors serializable or carefully scoped conditional transactions; portfolio display can be eventually consistent if labeled and refreshed. Persist the append-only facts before publishing events. Use idempotent consumers, bounded retries, and a reconciliation queue for ambiguous outcomes. Venue adapters need independent timeouts, rate limits, circuit breakers, and health-based routing. A fail-closed policy is appropriate when risk checks or reference data are unavailable; accepting unbounded exposure is worse than temporary rejection. Maintain durable audit trails, strict operator controls, encryption, and tested recovery procedures.

## Deep dives

**Cancel/fill race:** A cancellation is a request, not a rollback. A fill may occur before the venue processes the cancel. Represent `CANCEL_PENDING`, accept valid late fills, and release only the remaining reservation after authoritative confirmation.

**Ambiguous submit timeout:** The venue may have accepted an order even when the response was lost. Blind resubmission can create a second live order. Query by stable client reference or reconcile venue state before retrying; keep the order in an explicitly uncertain state meanwhile.

## Quick recall

Authorize → validate → atomically reserve → journal → route → deduplicate executions → append ledgers → reconcile. Venue state is asynchronous, cancellations race fills, and the ledger—not a cache—is the source of truth.
