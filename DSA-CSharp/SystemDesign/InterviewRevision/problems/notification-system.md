# Notification System

## Scope and assumptions

Design a service that accepts notification intents from product services and delivers them to a user's eligible endpoints. Support push, email, and SMS as separate channels; users can choose channel preferences and quiet hours. A notification may be urgent or routine. The system should expose delivery progress, but cannot promise that a provider or recipient device ultimately receives or displays a message. Provider integrations and user devices are outside our control.

## Requirements and exclusions

**Must:** accept requests quickly, validate recipients and templates, honor consent and preferences, retry transient provider failures, prevent duplicate sends where feasible, and provide auditable status. Prioritize urgent messages without starving normal work.

**Not in scope:** authoring campaign content, rendering the client inbox, guaranteeing exactly-once delivery, or choosing legal policy for consent. Those are product and compliance responsibilities; the delivery service enforces supplied policy.

## Data model and API

- `Notification`: `id`, `user_id`, `type`, `template_id`, safe template variables, priority, scheduled time, expiry, idempotency key, and creation time.
- `Delivery`: `notification_id`, channel, destination reference, state, attempt count, provider message ID, next attempt, and last error. Keep destination data protected and minimize retention.
- `Preference`: user, channel consent, locale, quiet-hour window, and optional per-type rules.
- `OutboxEvent`: durable event written with the product transaction that creates an intent.

`POST /v1/notifications` accepts a notification intent plus idempotency key and returns `202` with an ID. `GET /v1/notifications/{id}` returns aggregate and per-channel state. Provider callbacks update delivery state through authenticated, deduplicated webhook handling.

## Architecture

Product services write intents and an outbox record in their own database transaction. An outbox publisher or change-data-capture worker transfers events to a durable broker. Intake workers validate and persist canonical notifications, resolve policy and preferences, then create channel-specific delivery tasks. Separate queues and worker pools isolate push, email, and SMS, while priority lanes and per-provider rate limiters control load. Workers render vetted templates, call providers, and store attempt outcomes. A callback processor reconciles asynchronous delivery receipts. A status API reads from the notification store; metrics and a dead-letter workflow support operations.

## End-to-end flow

1. A product transaction writes the business change and an outbox entry, using a stable idempotency key for retried requests.
2. The publisher emits the entry to the broker; intake deduplicates it, validates its schema, and persists the intent.
3. A planner resolves the user's current consent, channel preferences, locale, quiet hours, expiry, and urgency. It records why each channel is included or suppressed.
4. The planner schedules eligible deliveries. Quiet-hour messages are delayed only when their expiry and type policy permit; urgent categories follow explicitly configured exceptions.
5. A channel worker obtains a bounded lease, renders the template, checks provider limits, and sends with a stable provider idempotency token when supported.
6. The worker stores accepted, retryable, or terminal outcome. Provider callbacks later refine accepted into delivered, bounced, or failed; repeated callbacks are harmless.
7. Clients poll status or receive a product-level event. Operators inspect lag, error rates, and dead letters without logging message contents.

## Scaling, consistency, and reliability

Partition broker traffic by user or notification ID to distribute load; avoid a single global ordering promise. Preference changes need a clear cutoff: re-check immediately before sending, while acknowledging a race with a concurrent opt-out and providing an emergency suppression mechanism. Queue durability and the outbox prevent losing accepted intents; bounded retries with exponential backoff and jitter avoid amplifying outages. Expired tasks are dropped, permanent errors go to a dead-letter queue, and replay requires deduplication. Use per-provider bulkheads, circuit breakers, quotas, and backpressure. Encrypt destinations and variables, restrict access, and set retention by data class.

## Deep dives

**Delivery versus duplicates:** A timeout after a provider accepted a request leaves the result ambiguous. Retrying may duplicate; not retrying may lose delivery. Provider idempotency and callback reconciliation narrow this window, but exactly-once external delivery is generally unavailable. Model state as at-least-once attempts and make duplicate risk visible.

**Fan-out and overload:** A popular event can generate millions of tasks. Apply admission limits, batch planning, and independent channel queues. If backlogs exceed expiry budgets, reject or suppress low-priority notifications rather than delaying urgent ones indefinitely.

## Quick recall

Durable intent (outbox) → policy-aware planner → isolated channel queues → idempotent-ish provider attempts → callback reconciliation. Preserve consent, expiry, auditability, bounded retries, and honest delivery semantics.
