# Activity tracking and athlete social network

## Scope and assumptions

Design a mobile service that records GPS-based activities such as runs and rides, displays maps and statistics, lets athletes follow one another, and provides activity feeds and segment leaderboards. A phone may be offline during an activity and upload later. GPS accuracy varies, and activity data is personally sensitive. Route planning, live safety tracking, coaching plans, hardware integrations, and payment are out of scope.

## Requirements and exclusions

- Capture and upload an activity with time-ordered location samples.
- Show the owner a route, summary metrics, and activity history; show permitted activities to followers.
- Identify traversed segments and rank eligible efforts.
- Allow privacy controls for private activities and hidden start/end areas.
- Exclude detailed sensor analytics, anti-doping adjudication, and turn-by-turn navigation.

## Data model and API

Represent `Activity(id, athlete_id, start_time, sport, visibility, status, distance, duration, elevation, version)`, `TrackPoint(activity_id, sequence, timestamp, lat, lon, altitude, sensors)`, `Segment(id, geometry, sport_constraints)`, and `Effort(activity_id, segment_id, elapsed_time, quality_status)`. Store raw samples as an immutable object or time-series/blob payload, with activity metadata in a transactional database. A spatial index stores simplified route geometry and segment geometry. APIs include `POST /activities` (create upload session), `PUT /activities/{id}/samples` (chunked, sequenced upload), `POST /activities/{id}/complete`, `GET /activities/{id}`, and `GET /segments/{id}/leaderboard`.

## Architecture

The mobile app buffers samples locally and uploads resumable chunks. An ingestion API validates ownership, sequence numbers, time ranges, and payload size. Raw tracks go to durable object storage; metadata and processing state live in a database. An asynchronous processing pipeline computes distance, pace, elevation smoothing, map polyline, and segment matches, then emits a ready event. A privacy service applies visibility and map redaction rules. Feed, map-tile, and leaderboard read models are separate projections backed by caches and geospatial indexes.

## End-to-end request/data flow

1. **Record and upload:** App captures timestamped coordinates and keeps them on-device through network loss. It creates an activity upload session and sends bounded chunks with sequence IDs and checksums. The server acknowledges persisted chunks so reconnects resume safely.
2. **Finalize:** Client calls completion with the final chunk count. The service marks the activity pending processing, verifies contiguous or explicitly missing chunks, and emits a processing job through an outbox. Duplicate completion requests return the same activity state.
3. **Process:** Workers read raw samples, remove impossible jumps, smooth elevation, calculate summary metrics and a simplified route, and match candidate segments spatially before computing elapsed times. Derived outputs are versioned so the pipeline can be rerun with improved algorithms.
4. **Serve:** The owner can inspect processing state and later retrieve metrics and map data. For feed or leaderboard queries, the API filters activities by visibility and eligibility, then serves projections and cached pages. Hidden-zone transformation is applied consistently to both map imagery and segment matching.

## Scaling, consistency, and reliability

Partition raw track objects by activity ID and time; do not keep every GPS point in a hot relational row. Chunk uploads limit memory and retry costs. Processing is horizontally scalable and idempotent by `(activity_id, processor_version)`. Activity ownership, visibility, and upload acceptance are strongly checked against canonical metadata; metrics, feeds, and leaderboards are eventually consistent. Leaderboards may be partitioned by segment and time window, with a top-N cache and cursor-based athlete pages. Avoid recalculating all efforts when one activity changes; update only affected segment partitions. Replicate raw data and retain processing status so failed jobs can resume. If analytics workers are down, keep the activity visible as “processing” rather than losing a completed upload.

## Deep dives and failure cases

**Offline and partial uploads:** A lost acknowledgement can cause duplicate chunks. Require sequence number plus checksum and make storing a chunk an idempotent operation. Finalization must distinguish an incomplete upload from a valid short activity. Preserve raw samples for recovery; derived metrics should be reproducible rather than the only retained record.

**Privacy and leaderboards:** A private activity must not leak through a segment effort, feed preview, cached map, or count. Enforce visibility at projection generation and again at reads; invalidate cached pages on privacy changes. Hide-start/end zones before map simplification and segment matching so redaction does not merely conceal the rendered polyline while the underlying effort exposes the route. If a visibility update races with an older projection, the read path's canonical policy check prevents disclosure.

## Quick recall

Resumable chunked GPS ingestion; immutable raw track plus versioned derived metrics; asynchronous segment matching; strong ownership/privacy checks; rebuildable feeds and leaderboards; redact routes before indexing or matching.
