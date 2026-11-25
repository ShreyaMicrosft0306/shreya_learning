# Donations Website

## Scope and assumptions

Donors can browse campaigns, make one-time or recurring contributions, receive confirmation, and see campaign totals. The payment provider handles card collection and settlement. Campaign administrators can set goals and close a campaign. The design prioritizes correct financial records and clear payment status over instantly updated public totals.

## Requirements and exclusions

- Create and read campaign pages and progress.
- Accept one-time donations and schedule recurring attempts.
- Prevent duplicate charges when clients or providers retry.
- Deliver receipts and allow reconciliation/refund workflows.
- Protect donor data and separate administrative permissions.

## Data model and API

```text
Campaign(campaign_id, owner_id, goal_amount, currency, state)
Donation(donation_id, campaign_id, donor_id, amount, currency, state, provider_ref)
PaymentAttempt(attempt_id, donation_id, idempotency_key, state, provider_ref)
LedgerEntry(entry_id, donation_id, account, amount, currency, created_at)
```

```text
GET  /campaigns/{id}
POST /campaigns/{id}/donations
POST /donations/{id}/cancel-recurring
POST /webhooks/payment-provider
```

Amounts use integer minor currency units or a precise decimal type; never use floating-point arithmetic for money.

## Architecture

The campaign service owns campaign metadata and permissions. A donation service creates durable intents and payment attempts. A provider adapter tokenizes and submits payments without storing raw card data. A ledger records financial movements; an outbox emits events for receipts and aggregate totals. A scheduler triggers recurring attempts. Webhook processing updates payment state idempotently.

## End-to-end flow

1. The donor submits a campaign, amount, currency, and idempotency key.
2. The service validates campaign state, currency, limits, and donor authorization, then creates a pending donation and payment attempt.
3. It calls the provider with a stable provider idempotency key.
4. The provider response or webhook transitions the attempt to succeeded, failed, or unknown; an unknown outcome is reconciled rather than blindly charged again.
5. A successful transition writes the ledger entry once and emits an outbox event.
6. Consumers update campaign totals and send a receipt.

## Scaling, consistency, and reliability

Keep the donation state machine explicit. Provider calls are outside the database transaction, so use durable state plus idempotent callbacks and reconciliation. Public campaign totals can be cached or asynchronously aggregated; the ledger remains authoritative. If campaigns have hard caps, reserve capacity atomically or define how late concurrent donations are refunded. Partition ledgers by account/campaign while preserving auditable ordering.

## Deep dives and failure cases

- **Recurring donations:** store consent and schedule metadata, retry transient failures with limits, and notify donors when action is needed.
- **Webhook duplication or reordering:** dedupe by provider event ID and validate state transitions against the attempt.
- **Refunds and chargebacks:** append compensating ledger entries instead of deleting historical financial records.

## Quick recall

Create a durable donation intent first, use idempotency at both client and provider boundaries, and let the ledger—not a cached campaign counter—define financial truth.
