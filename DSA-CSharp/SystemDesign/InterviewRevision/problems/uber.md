# Ride-Hailing Marketplace

## Scope and assumptions

Design the core flow that matches a rider requesting a trip with a nearby available driver, manages trip state, and delivers location updates. A city or region is the operational boundary. Drivers and riders use mobile clients with intermittent connectivity and noisy GPS. Pricing, payments, identity verification, and navigation are integrations rather than the primary design target.

## Requirements and exclusions

**Required:** create/cancel requests; find eligible nearby drivers; prevent one driver accepting two trips; update trip state; share driver location with rider; tolerate GPS delay and client retries.

**Quality:** low dispatch latency, high regional availability, auditable state transitions, and useful behavior during network partitions. ETA is approximate. This does not promise globally optimal matching or continuous sub-second location precision.

## Data model and API

`Driver(driver_id, status, location, location_time, vehicle, service_types, version)` holds current eligibility and a volatile latest location. `Trip(trip_id, rider_id, driver_id?, pickup, destination, status, version, timestamps)` is the canonical lifecycle record. A transition log records actor, old/new state, and request ID. Statuses may include `REQUESTED`, `SEARCHING`, `OFFERED`, `ASSIGNED`, `PICKED_UP`, `COMPLETED`, `CANCELLED`.

APIs: `POST /trips` with pickup/destination and idempotency key; `POST /trips/{id}/cancel`; driver `POST /location`; `POST /offers/{id}/accept`; `GET /trips/{id}`; WebSocket or push channel for status/location events. Coordinates need validation and access control.

## Architecture

Regional API gateways route users to a dispatch service. A geospatial index, such as geohash cells or an R-tree, stores recent eligible driver locations and enables nearby candidate lookup. A location ingestion service validates, timestamps, and coalesces updates. Dispatch scores candidates using distance, estimated arrival, vehicle/service eligibility, driver state, and fairness constraints. A trip service owns lifecycle transitions in a transactional database. A notification service delivers offers and updates, while an event log supports audit and asynchronous projections.

## End-to-end flow

1. Rider submits a trip with an idempotency key. The trip service creates one `REQUESTED` record and emits a dispatch request.
2. Dispatch searches nearby geospatial cells, expanding radius if supply is sparse. It filters stale or ineligible drivers and ranks a bounded candidate set.
3. The system creates expiring offers and notifies one or a small batch of drivers. An offer is not an assignment; the driver can decline or time out.
4. On acceptance, the trip service conditionally assigns the trip only if still searching and the driver is still available. A database transaction or compare-and-set on versions claims the driver and trip atomically.
5. Competing accepts lose the conditional update and receive a clear rejection. If offers expire, dispatch tries the next candidates.
6. Driver location updates refresh the spatial index and are sent to the assigned rider through a low-latency channel. Trip state transitions are authorized and persisted before notifications.
7. Completion or cancellation records a terminal state and asynchronously releases driver availability and updates downstream analytics.

## Scaling, consistency, and reliability

Partition trip state and location indexes by city/region to keep latency bounded and reduce cross-region coordination. Location updates are high-volume and rapidly obsolete; coalesce updates, retain only the latest for dispatch, and use event streams for audit if needed. Do not let delayed GPS overwrite a newer point: compare source time and sequence/version. Geospatial search should return candidates, not determine assignment correctness.

Assignment requires stronger consistency than location. A conditional transition prevents duplicate assignments even when dispatch workers race or clients retry. Use idempotency keys for trip creation and acceptance. Notifications are at-least-once; clients deduplicate by trip version and fetch canonical state after reconnect. If the trip database is unavailable, avoid telling both parties that a trip is assigned. If the location index is stale, widen search or wait briefly rather than treating stale drivers as certainly available.

Matching quality is not just nearest distance: road-network ETA, pickup congestion, driver preference, cancellation likelihood, and balancing supply across neighborhoods matter. Keep scoring pluggable and versioned. Geospatial cells simplify partitioning but boundary search must inspect adjacent cells; cell size should reflect density and search radius.

## Deep dives and failure cases

**Atomic assignment:** A naive flow reads driver available, sends offer, then writes assigned. Two dispatchers can race. Reserve/claim with a conditional write on both trip state and driver availability, or use a single transaction in the authoritative region. A short lease with expiry handles abandoned offers; lease expiration must not release a driver already assigned by a later state.

**Location freshness:** Mobile clients may buffer points offline and replay them. Reject out-of-order sequences, apply plausibility checks, and expire locations after a product-defined staleness threshold. Stale data should lower ranking or exclude the driver, not create false precision in ETA.

## Quick recall

Separate volatile location search from authoritative trip state. Use spatial indexing to find candidates and a conditional transaction to assign exactly one driver. Regionalize, make client retries idempotent, treat notifications as hints, and handle stale GPS explicitly.
