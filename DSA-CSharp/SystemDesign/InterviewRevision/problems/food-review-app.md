# Food Review App

## Scope and assumptions

Users discover restaurants, search and filter them, read reviews, and submit or edit their own reviews. Restaurant onboarding, reservations, and delivery are out of scope. Search results should be responsive; a newly submitted review may take a short time to affect the displayed aggregate rating.

## Requirements and exclusions

- Search restaurants by map area, cuisine, price range, and text.
- Read restaurant details and paginated reviews.
- Create, edit, and report reviews with authenticated authorship.
- Prevent duplicate reviews per user and restaurant unless edits are intended.
- Moderate spam and abuse without blocking all reads during a moderation outage.

## Data model and API

```text
Restaurant(restaurant_id, name, location, cuisines, price_band, status)
Review(review_id, restaurant_id, author_id, rating, body, created_at, status)
ReviewSummary(restaurant_id, rating_sum, rating_count, updated_at)
```

Representative operations:

```text
GET  /restaurants?lat=...&lon=...&radius=...&filters=...
GET  /restaurants/{id}/reviews?cursor=...
POST /restaurants/{id}/reviews  (idempotency key)
PATCH /reviews/{id}
POST /reviews/{id}/reports
```

Keep review records authoritative. Treat search documents and rating summaries as derived data.

## Architecture

An API service validates requests and authorization. A relational store owns restaurant and review records, enforces one review per user/restaurant, and supports transactional edits. A geospatial index narrows candidates by area; a text-search index handles names and cuisine terms. A cache serves popular restaurant pages and summaries. An event/outbox publisher updates search documents, aggregate ratings, and moderation queues.

## End-to-end flow

1. A search request is normalized and routed to the geo/text index.
2. The index returns candidate restaurant IDs; the service applies filters and ranking and hydrates a bounded page of records.
3. On review submission, the service checks identity, restaurant state, content limits, and duplicate policy.
4. It writes the review and an outbox event in one transaction.
5. Consumers update the rating summary and search view; moderation can mark the review pending or hidden.
6. Reads return only reviews allowed by the visibility policy and use a stable cursor for pagination.

## Scaling, consistency, and reliability

The review write and uniqueness constraint must be atomic. Aggregate ratings can be eventually consistent, but label/cache their freshness if necessary. A hot restaurant can overload a single summary row; batch updates or partition counters and periodically reconcile them from review records. Geo queries should limit radius and candidate count. Cache invalidation should follow review changes, with short TTLs as a fallback.

## Deep dives and failure cases

- **Abuse and moderation:** accept reports and moderation state transitions idempotently; keep an audit trail and avoid exposing private reporter data.
- **Search/index lag:** if an index is behind, the authoritative restaurant record still works by ID; show freshness separately from correctness.
- **Rating integrity:** compute average from sum/count rather than repeatedly averaging averages; repair derived summaries from source reviews.

## Quick recall

Geo/text index finds candidates; the relational store owns restaurants and reviews; derived summaries make reads fast. Protect authorship and duplicate rules at write time, and make moderation and search asynchronous but repairable.
