# Local business discovery and reviews

## Scope and assumptions

Design a service for discovering nearby businesses, viewing details and reviews, and submitting a review. A business can have multiple categories and a geographic location. Search should reflect edits and new reviews within seconds to a few minutes; a successful review submission must be durable immediately. Assume a mobile/web client and a large, read-heavy, multi-region product. Reservations, payments, messaging, and business-owner advertising are outside scope.

## Requirements and exclusions

- Search by text, category, location, open-now status, and optional rating range; sort by relevance or distance.
- Show business details and paginated, recent or useful reviews.
- Authenticated users can create, edit, or remove their own review; prevent duplicate reviews per user/business.
- Exclude full-text review moderation design, recommendation training, and exact business-hours semantics for holidays.

## Data model and API

Persist `Business(id, name, location, address, categories, attributes, hours, version)`, `Review(id, business_id, user_id, rating, text, created_at, updated_at, status)`, and `ReviewSummary(business_id, count, rating_sum, histogram, version)`. Enforce a unique `(business_id, user_id)` constraint in the review store. Keep review text in a relational or document store; the search index contains searchable business fields and derived review statistics, not the source of truth.

Representative APIs:

- `GET /businesses/search?q=&lat=&lon=&category=&cursor=`
- `GET /businesses/{id}` and `GET /businesses/{id}/reviews?sort=&cursor=`
- `PUT /businesses/{id}/review` with rating and text; `DELETE /businesses/{id}/review`

Use opaque cursors to continue a stable sort, and idempotency keys for retryable writes.

## Architecture

An API gateway authenticates and rate-limits requests. A business service owns canonical business and review data, with a relational database for transactional constraints and durable writes. A transactional outbox emits business/review change events after commit. Consumers update a geospatial/text search index and review-summary projections. Search is served by geographically partitioned search nodes; a cache handles hot business details and popular queries. A moderation pipeline may mark review text hidden without deleting its audit record.

## End-to-end flows

1. **Search:** Client sends text and coordinates. The search service filters candidate businesses using a geo-cell/geospatial index and category/open filters, scores text relevance with distance and quality signals, then returns a page plus cursor. It hydrates compact details from the index or cache; the canonical store is consulted for fields that must be current.
2. **Business page:** The API reads business details and review pages separately. Review pages are ordered by a deterministic key such as `(created_at, id)` and fetched with keyset pagination. Cache stable business data, but do not cache user-specific review permissions as shared content.
3. **Review write:** Validate identity, rating bounds, and ownership. Upsert within a transaction under the unique business/user key, update the durable aggregate or append an outbox event, and return the committed review. Event consumers refresh the search document and summary; the write does not wait on index availability.

## Scaling, consistency, and reliability

Partition search geographically, while allowing boundary queries to fan out to adjacent cells and merge ranked results. A single business can become a hot key: cache its page, shard review reads by business plus time bucket, and use asynchronous summary updates rather than serializing every read. Database constraints make duplicate review creation safe across retries. Search and counts are eventually consistent; review ownership and accepted writes are strongly consistent in the primary database. Replicate read indexes regionally, but route writes for a business to one home region to avoid conflicting review edits. Outbox replay, consumer offsets, and periodic reconciliation repair lost or duplicated projection work. If search is degraded, offer category/nearby fallback or a clear retry response rather than pretending stale results are current.

## Deep dives and failure cases

**Geo search versus text relevance:** A pure nearest-neighbor query misses useful matches slightly farther away; a global text query returns irrelevant distant businesses. First find candidates in a bounded geographic area, then rank them with text, distance, rating confidence, and freshness. Expand the radius only when too few candidates qualify. Avoid letting a small number of reviews produce an unjustifiably high rank by applying a confidence-aware rating signal.

**Review consistency:** Updating a review and its aggregate in separate systems risks a committed review with a stale rating. Treat the review row and outbox record as one database transaction; derive aggregates asynchronously and make them rebuildable from reviews. On retries, the unique key and idempotency key return the existing outcome. If index updates lag, the review remains visible on its direct business page even if search ranking has not caught up.

## Quick recall

Canonical transactional business/review store; outbox-driven search and summary projections; geo-filter then rank; unique per-user review; strong write ownership, eventual search; cache hot pages and reconcile projections.
