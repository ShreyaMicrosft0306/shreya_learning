# Product price tracking and alerts

## Scope and assumptions

Design a consumer service that tracks publicly available product offers, shows historical prices, and alerts users when watched products meet a threshold. A product can have multiple sellers, variants, and shipping conditions. Data is sampled periodically and may be delayed or incomplete; displayed prices must communicate observation time and currency. Assume collection uses permitted retailer feeds/APIs or compliant public pages. Checkout, purchasing automation, and bypassing access controls are excluded.

## Requirements and exclusions

- Users save a retailer product or variant and configure a target price.
- Collect price/availability observations over time and render a history chart.
- Send deduplicated email/push alerts when an observed offer crosses a target.
- Support retailer errors and changing product identifiers without inventing values.
- Exclude automatic purchase, private retailer APIs, and currency conversion guarantees.

## Data model and API

Use `Retailer(id, collection_policy, currency)`, `Product(id, retailer_id, external_id, canonical_url, variant_key, title)`, `Offer(id, product_id, seller, price, shipping, availability, observed_at, source_version)`, `Watch(user_id, product_id, target_amount, currency, channel, enabled, last_alerted_observation)`, and `Alert(id, watch_id, observation_id, state)`. Preserve raw source provenance separately from normalized observations. APIs: `POST /watches`, `PATCH /watches/{id}`, `GET /products/{id}/history?range=`, and `GET /watches`.

## Architecture

A collection scheduler selects products by retailer-specific cadence, demand, and failure history. Compliant adapters retrieve feeds/APIs/pages with rate limits, conditional requests, and parser versioning. Normalizers map observations to canonical product/variant, seller, amount, currency, shipping, and stock state. A time-series store retains observations; a relational store owns users, watches, product identity, and alert state. A change detector evaluates watch conditions and writes durable alert tasks to an outbox. Notification workers deliver email/push and record provider outcomes. A chart API aggregates history into daily/weekly buckets while preserving the original observation timeline.

## End-to-end request/data flow

1. **Create watch:** User submits a retailer URL or selected product. Resolver validates the retailer and maps its external ID/variant to a canonical product, then stores watch and target. If identity is ambiguous, ask for a variant rather than silently tracking the wrong item.
2. **Collect:** Scheduler chooses due products and enqueues jobs partitioned by retailer. Adapter fetches within its allowed rate and records fetch outcome. Parser emits a normalized observation with explicit currency, availability, shipping treatment, and observed timestamp; failures are stored as errors, not as zero prices.
3. **Evaluate:** Observation writer persists the sample and emits a change event. Alert evaluator finds enabled watches for that product, applies target and currency rules, checks observation freshness and availability policy, and atomically inserts an alert keyed by watch plus observation. This makes replay safe.
4. **Notify and display:** Notification worker sends the message with observed price/time and a link to the retailer, retrying transient failures. Product history API queries time buckets and returns sampling gaps visibly. Watch reads show last checked time so users can distinguish old data from a current offer.

## Scaling, consistency, and reliability

Collection load is driven by watched products and retailer limits, not by every visitor. Deduplicate scheduled work per product, adapt polling cadence to watch count, price volatility, and retailer policy, and add jitter to avoid synchronized bursts. Partition observations by product and time; keep a compact rollup for long-range charts while retaining raw observations for audit or recomputation. User watch updates and alert dedupe need strong transactional constraints; price histories and charts are eventually consistent. Isolate retailer adapters so a site redesign does not take down unrelated sources. Circuit-break repeatedly failing domains, respect backoff, and expose stale status. If queues accumulate, prioritize products with active watches but remain within per-retailer limits.

## Deep dives and failure cases

**Price correctness and product identity:** A page may show a coupon, membership price, shipping fee, or a different size/color variant. Store the observation's basis and variant, and avoid comparing unlike currencies or tax/shipping semantics. Validate parser output with ranges and structural checks; suspicious jumps should be flagged for confirmation instead of creating false alerts. Keep parser version and raw permitted evidence so changed extraction can be audited.

**Alert exactly-once behavior:** Queue retries can deliver the same notification multiple times. Create a durable alert record with a unique `(watch_id, observation_id, condition_version)` identity before dispatch. Notification provider calls may still have ambiguous outcomes; use provider idempotency where available and track sent/unknown states. Do not mark an alert sent before it is accepted, and do not erase the alert if a later price rises.

## Quick recall

Policy-compliant scheduled collection; canonical product/variant identity; timestamped, provenance-rich observations; watch-specific target evaluation; unique alert event before delivery; show freshness and sampling gaps honestly.
