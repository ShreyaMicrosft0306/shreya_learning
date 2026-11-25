# Bitly: URL shortening

## Scope and assumptions

Design a service that accepts a long URL, returns a compact link, and redirects visitors to the destination. Assume public links, globally distributed reads, and optional expiry or deactivation. A link is not a security boundary: anyone with its code can use it. Custom aliases and basic click counts are optional.

## Requirements and exclusions

**Key requirements:** create a short link; redirect quickly and reliably; prevent accidental code collisions; support expiry and disablement; optionally report approximate click totals. Preserve the submitted destination exactly enough for a valid redirect while rejecting malformed or disallowed schemes.

**Out of scope:** previews, spam classification, billing, and real-time analytics. Redirect availability and correct lookup take priority over analytics freshness.

## Data model and API

`Link(code, destination, owner_id, created_at, expires_at, state, version)` is the authoritative record. `code` is a unique, case-sensitive random identifier; state can be active, disabled, or expired. Store destination as an opaque validated URL rather than repeatedly reconstructing it. A separate `ClickEvent(code, occurred_at, region, referrer_class)` stream supports aggregate analytics without putting every visit on the redirect's synchronous path.

`POST /v1/links` accepts `{url, expiresAt?, alias?}` and returns `{code, shortUrl}`. `GET /{code}` returns an HTTP redirect (normally 302 when destination may change, or 301 only when permanence is explicitly promised). `DELETE /v1/links/{code}` disables an owned link. `GET /v1/links/{code}/stats?range=` returns delayed or approximate aggregates. Creation should accept an idempotency key so a retry after a timeout does not create multiple links.

## Architecture

An API gateway applies quotas and validation. A link service writes to a key-value store indexed by code. Regional redirect services use a distributed cache before querying storage. A durable event stream receives clicks asynchronously; consumers build time-bucketed analytics. An invalidation channel propagates disablement and expiry changes.

## End-to-end flows

1. **Create:** Client submits URL, expiry, and optional alias. The gateway validates size, scheme, and account limits; the service checks ownership rules and normalizes only what is safe to normalize.
2. The service generates a high-entropy code, or reserves the requested alias with a uniqueness check. It writes the record durably before returning the short URL. If the write outcome is uncertain, the idempotency key resolves the retry.
3. **Redirect:** Browser requests `/{code}`. The edge routes to a nearby region; the redirect service checks local cache, then reads the authoritative key-value record on a miss.
4. It rejects missing, disabled, or expired records; otherwise it emits a click event without waiting for analytics and returns `Location: destination`. Cache misses and event-sink failures must not turn a valid redirect into an error.
5. **Disable:** An authorized owner updates state in the durable store; an invalidation event evicts regional cache entries. Short TTLs bound the period in which a stale cache can still redirect.
6. **Stats:** Consumers aggregate click events by code and time bucket. The stats API reads these aggregates and clearly reports their freshness.

## Scaling, consistency, and reliability

Random codes avoid centralized sequential allocation; use sufficient entropy and enforce uniqueness with retry on collision. A short code is not authorization. Partition storage by code. Cache popular mappings, aligning TTLs with expiry. Geographic replicas reduce latency; define an authoritative write path and acceptable read lag.

Redirects should fail closed for unknown codes but remain available during analytics outages. If the primary store is unavailable, serve unexpired cached records and return a controlled error on misses rather than inventing a destination. Protect the service from abusive creation and hot-key concentration through quotas, cache replication, and edge throttling. Use 302 as the default because permanent browser and intermediary caching can make later disabling ineffective.

## Deep dives and failure cases

**Code allocation:** Sequential IDs encoded in base-N are compact but expose creation order and concentrate allocation. Random codes avoid a global counter but need collision retries and a uniqueness index. Never overwrite on collision. Custom aliases require transactional reservation and defined case-sensitivity rules.

**Cache invalidation:** Disabling cannot instantly erase every regional entry. Use invalidations and bounded TTLs; strict revocation requires checking state at the edge, adding latency. A stale redirect is the consistency tradeoff. Buffer/retry analytics events and allow lag; synchronous counting would couple redirects to analytics availability.

## Quick recall

The critical path is code lookup → active/expiry check → redirect. Durable mapping storage is authoritative; cache protects latency and hot links; click analytics are asynchronous. Choose code generation, cache TTL, and redirect status with collision risk, revocation guarantees, and browser caching in mind.
