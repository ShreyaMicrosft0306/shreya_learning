# Tinder-style matching

## Scope and assumptions

Design location-aware discovery and mutual matching. Users browse nearby profiles and swipe; mutual positive decisions create a match that can unlock messaging. Use coarse location and support blocking/reporting.

## Requirements and exclusions

**Key requirements:** profile management; candidate discovery under age/distance/preferences; record likes and passes; create exactly one match for a mutually liked pair; prevent duplicate results; support blocks, unmatches, and abuse reporting.

**Out of scope:** chat transport, billing, identity verification, and perfect ranking. Safety and deterministic match creation matter more than ranking freshness.

## Data model and API

`Profile(user_id, attributes, visibility, updated_at)` stores display data. `DiscoveryState(user_id, coarse_geocell, preferences, last_active_at)` supports candidate search. `Swipe(actor_id, target_id, decision, created_at)` is unique per pair and captures the actor's latest decision according to product rules. `Match(match_id, low_user_id, high_user_id, created_at, state)` uses sorted IDs for a unique pair constraint. `Block(blocker_id, blocked_id, created_at)` and `Report` support safety.

`PUT /profile` updates profile and discovery settings. `GET /discover?cursor=` returns eligible profile cards and an opaque cursor. `POST /swipes` submits target, decision, and idempotency key; response includes match status if created. `POST /blocks/{user}` and `POST /reports` update safety state. Avoid exposing precise coordinates or internal ranking scores.

## Architecture

Profile service stores data and media references; CDN serves approved photos. Geospatial discovery partitions active profiles into coarse cells. Ranking uses distance, activity, and feedback. Swipe service records decisions; Match service resolves mutual likes and unique pairs. Safety state is checked by discovery and match APIs. Notifications follow durable match creation.

## End-to-end flows

1. **Profile setup:** User submits profile and preferences. Service validates required fields, stores profile, and publishes approved profile/media versions to discovery indexes. Exact location is transformed into a coarse cell and access to raw location is limited.
2. **Discover:** Client requests a stack. Discovery selects nearby cells, filters by age/preferences, account visibility, prior swipes, and safety exclusions, then ranks candidates. It records an impression or page token so the client can avoid repeating cards.
3. **Swipe:** Client submits a decision with an idempotency key. Swipe service validates actor and target, checks blocks and eligibility, then upserts the actor-target decision and emits a durable swipe event.
4. On a positive swipe, match resolver checks whether the reverse positive swipe exists. It canonicalizes the pair `(min_id, max_id)` and inserts a match under a unique constraint; concurrent likes therefore converge on one match.
5. Match event triggers notifications and access grants for the messaging product. The API returns match state even if push delivery is delayed.
6. **Block/unmatch:** Safety or user action changes authoritative relationship state, immediately removes eligibility and disables messaging access. Asynchronous workers purge stale discovery cards and notify dependent services.

## Scaling, consistency, and reliability

Discovery is approximate and geographically partitioned; swipe persistence and pair uniqueness need stronger guarantees. Index both swipe directions. A canonical pair constraint prevents duplicate matches; route each pair to a deterministic shard in multi-region deployments.

Candidate feeds are derived. Exclude seen users with cursors and durable swipe history. Location/preference reindexing may lag, but validate eligibility and blocks when serving. Rate-limit swipes, handle retries idempotently, and keep precise location out of APIs and logs.

## Deep dives and failure cases

**Concurrent mutual likes:** Simultaneous likes can evade a non-atomic read-then-write check. Persist swipes first, then retry resolution against durable records and insert the canonical pair under a unique constraint. Duplicate events are harmless; the UI can briefly show pending.

**Safety consistency:** Apply blocks on card retrieval, swipe, match, and messaging paths—not only in async cleanup. If safety state is unavailable, deny sensitive actions. Revalidate cached cards. Reports create durable cases without revealing reporter identity.

## Quick recall

Discovery is location-partitioned and approximate; swipe history and match creation must be durable and idempotent. Persist each swipe, resolve mutual likes asynchronously, and enforce pair uniqueness. Treat blocks and location privacy as real-time safety constraints.
