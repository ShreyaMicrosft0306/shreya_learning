# Time-bounded online auction

## Scope and assumptions

Design an auction for a single item with a scheduled close time, public current price, and competing bidders. Accepted bids must have a clear order and cannot be accepted after the close decision. A bidder may retry after a timeout. Assume one auction is the unit of serialization and that payment authorization is handled by an external payment system. Multi-item shopping, sealed-bid formats, and complex proxy-bidding policies are excluded; the model can extend to those later.

## Requirements and exclusions

- Create an auction with item description, opening price, reserve, and start/end time.
- Submit bids, display current state/history, and notify participants about meaningful changes.
- Determine a single winner and make closure durable and auditable.
- Prevent duplicate submissions, bid races, and post-close acceptance.
- Exclude recommendation, fraud-model internals, settlement disputes, and checkout UX.

## Data model and API

Store `Auction(id, seller_id, status, start_at, end_at, reserve, current_price, leader_id, version)`, `Bid(id, auction_id, bidder_id, amount, client_request_id, accepted_at, sequence)`, and `AuctionEvent(auction_id, version, type, payload)`. Enforce unique `(auction_id, bidder_id, client_request_id)` for retry idempotency and retain accepted bids append-only. APIs: `POST /auctions`, `POST /auctions/{id}/bids` with amount and idempotency key, `GET /auctions/{id}`, and `GET /auctions/{id}/bids?cursor=`.

## Architecture

The API authenticates bidders and routes each auction to a single logical write authority. A transactional database is canonical for auction state and bid history. A transaction or per-auction actor serializes bid acceptance, verifies status/time/amount rules, appends the bid, updates leader and price, increments version, and writes an outbox event atomically. Read replicas/cache serve public views; a websocket or notification service distributes updates but is not part of correctness. A scheduler triggers closure, while a database-time check in the bid transaction is the final authority.

## End-to-end request/data flow

1. **Create:** Seller submits an auction. Validate item ownership and time bounds, store it as scheduled, and emit a creation event. A scheduler records a close task keyed by auction ID and expected version.
2. **Bid:** API validates request shape and checks that auction is open. The write authority locks or serializes the auction row, uses authoritative server/database time, and verifies the amount against policy and current leader. It inserts the idempotent bid, updates price/leader/version, writes an event, commits, then returns accepted state. A duplicate request returns the original result.
3. **Propagate:** Outbox consumers update caches, publish realtime notifications, and append audit/analytics events. Public reads include the state version so clients can ignore out-of-order websocket messages and refresh when they detect a gap.
4. **Close:** Scheduler invokes close, but closure is idempotent. A transaction changes open to closing/closed only if server time has passed the deadline, selects the final accepted leader, checks reserve and eligibility, and records the outcome. Payment authorization and winner notification run as retryable downstream tasks; they cannot reopen bidding.

## Scaling, consistency, and reliability

Serialize writes per auction while allowing different auctions to operate in parallel. This localizes contention and makes bid ordering explainable. A high-profile auction may become a hot row; an actor partition or sequencer can queue requests, but capacity is still bounded by one ordered stream. Give clients a fair, explicit rejection when overloaded rather than accepting bids with ambiguous time. Auction state, accepted bids, and closure are strongly consistent; notifications, search, and counters are eventually consistent. Replicate reads, but never accept writes independently in two regions for the same auction. Failover must fence the former leader using leases/epochs or database consensus so split-brain writers cannot create conflicting winners.

Close tasks can be delayed or delivered repeatedly; correctness comes from idempotent state transition and bid-time validation, not exact scheduler timing. If a bid commits before the deadline but its response is lost, its idempotency key reveals the accepted outcome. Keep immutable bid records for audit and apply retention rules appropriate to legal requirements.

## Deep dives and failure cases

**Deadline semantics:** Decide whether a bid is eligible by server receipt time, transaction commit time, or a sequenced timestamp, and expose that rule. A row lock may queue a request beyond closing; therefore time must be checked after acquiring the serialization point. If bids at the deadline are not accepted, use strict `now < end_at`. Any anti-sniping extension must be a committed update to `end_at`, not an informal client timer.

**Payment and winner failure:** A winner can fail payment authorization after the auction closes. Keep auction result distinct from settlement status; do not silently select a different bidder unless the published policy specifies fallback. Payment retries use their own idempotency key and reconciliation queue. If the payment provider is unavailable, preserve the winner and retry rather than losing the auction record.

## Quick recall

One ordered writer per auction; append-only bids; server-time check under serialization; idempotent submissions and close; transactional outbox; realtime is advisory; settlement is separate from the durable result.
