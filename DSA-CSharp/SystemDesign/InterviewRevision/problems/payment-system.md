# Payment System

## Scope and assumptions

Design a merchant-facing service that accepts a payment request, authorizes or captures through an external payment processor, records the financial result, and reports status. A processor or bank can be slow, unavailable, or return an ambiguous response. Card data handling and regional regulation require dedicated compliance work; this design minimizes direct exposure by using processor tokens.

## Requirements and exclusions

**Must:** prevent accidental duplicate charges, track asynchronous state transitions, maintain balanced accounting records, support refunds and disputes, protect sensitive data, and reconcile internal state with processor reports.

**Not in scope:** implementing card networks, guaranteeing settlement timing, fraud-policy design, or storing raw payment credentials. A merchant cannot equate an HTTP response with settled funds.

## Data model and API

- `Payment`: ID, merchant, amount in minor currency units, currency, capture mode, state, idempotency key, processor reference, and timestamps.
- `PaymentAttempt`: payment, processor, request reference, outcome, and sanitized error.
- `LedgerEntry`: immutable debit/credit account, amount, currency, payment reference, and posting time.
- `Refund`: original payment, amount, idempotency key, state, and processor reference.
- `WebhookEvent`: provider event ID, verification result, and processing state.

`POST /v1/payments` creates or retrieves a payment by merchant-scoped idempotency key. `GET /v1/payments/{id}` returns current state. `POST /v1/payments/{id}/refunds` creates a bounded refund. Signed processor webhooks provide asynchronous updates.

## Architecture

An API gateway authenticates merchants, enforces quotas, and validates idempotency scope. Payment orchestration persists the intent and emits work through an outbox. A processor adapter normalizes provider-specific requests, errors, and callbacks. A durable workflow/state machine controls authorization, capture, void, and refund transitions. An accounting service posts immutable double-entry ledger records; a reconciliation service imports processor settlement files and flags mismatches. Webhook ingestion verifies signatures, persists raw event metadata safely, deduplicates event IDs, and updates the workflow. Read APIs use projections but derive financial truth from ledger and processor reconciliation.

## End-to-end flow

1. Merchant sends a payment request with amount, currency, tokenized payment method, and an idempotency key.
2. The API validates amount and merchant permissions. In one transaction it creates the payment intent and outbox event, or returns the prior result for a duplicate key.
3. Orchestration creates a payment attempt with a stable processor reference and sends authorize/capture according to the requested mode.
4. A synchronous success or decline is recorded as a transition. A timeout becomes `UNKNOWN` or `PROCESSING`, not a guessed decline; retry first queries the processor by reference where possible.
5. Processor webhooks are signature-checked, stored durably, and applied idempotently. State transitions reject stale or illegal updates.
6. On capture confirmation, the accounting service posts balanced entries (for example, processor receivable against merchant payable, with fees represented separately). Authorization alone is not posted as settled revenue.
7. Refund requests verify remaining refundable amount, create a separate idempotent workflow, and post accounting entries only when the refund reaches the defined accounting point.
8. Settlement reports are matched by processor references and amounts. Mismatches become review cases; they are not silently patched.

## Scaling, consistency, and reliability

Partition by merchant or payment ID; require strong consistency for idempotency, state transitions, and refund limits. Read status can be eventually consistent if state freshness is visible. The outbox closes the database-to-queue gap. Consumers deduplicate provider events and enforce monotonic/valid transitions. Retry only transient errors with backoff and jitter; processor timeouts need lookup/reconciliation rather than blind duplicate charge attempts. Isolate providers with circuit breakers and maintain a safe pause switch. Encrypt tokens, redact logs, restrict operational access, and keep an audit trail. Ledger entries are append-only; corrections use explicit compensating entries.

## Deep dives

**Unknown outcome:** The processor may have charged while the response was lost. Mark uncertainty, query using the stable reference, and reconcile before creating another attempt. Idempotency helps only within the processor's supported scope and retention period.

**Webhook ordering:** A delayed authorization event can arrive after capture. Deduplication alone is insufficient; enforce allowed state transitions and compare provider sequence/time where trustworthy. Settlement files remain an independent check.

## Quick recall

Idempotent intent → outbox → state-machine processor adapter → signed webhook reconciliation → double-entry ledger. Treat timeouts as unknown, distinguish authorization from settlement, and use compensating entries rather than rewriting history.
