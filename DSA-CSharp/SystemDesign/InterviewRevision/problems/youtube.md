# Video Platform (YouTube-Style)

## Scope and assumptions

Design a platform for uploading, processing, discovering, and watching user-published videos. Viewers expect fast playback across devices and network conditions; uploads may be large and intermittent. The design covers video metadata, asynchronous media processing, playback delivery, and basic engagement. It excludes live broadcast, full recommendation-model design, copyright adjudication, and detailed ad auctions.

## Requirements and exclusions

**Functional:** upload resumably; validate and transcode source media into multiple renditions; publish metadata; retrieve a watch page; stream or download playable segments; record views and basic likes/comments.

**Quality:** durable originals, high playback availability, low startup delay, adaptive quality, and eventual consistency for counters. Metadata edits and privacy changes should take effect promptly. A view counter need not be exact in real time.

## Data model and API

`Video(video_id, owner_id, title, description, visibility, status, created_at, source_object, processing_version)` is the canonical record. `Rendition(video_id, codec, resolution, bitrate, manifest_uri, status)` describes derived assets. Engagement events are append-only `{event_id, video_id, viewer_id?, type, event_time}`; aggregates are separate derived values. Comments and likes have their own records and moderation state.

APIs: `POST /uploads` creates a session; `PUT /uploads/{id}/parts/{n}` uploads a part; `POST /uploads/{id}/complete` finalizes; `POST /videos/{id}/publish` changes visibility; `GET /videos/{id}` returns watch metadata and playback manifest; `POST /videos/{id}/events` reports engagement. Upload endpoints require owner authorization; playback checks privacy/access before returning a short-lived manifest or signed URLs.

## Architecture

An API gateway and metadata service manage users, policies, and video records in a relational or distributed SQL store. The client uploads directly to durable object storage using multipart signed URLs, keeping large bytes away from application servers. Completion emits an event to a durable queue. Workers probe, scan, transcode, generate thumbnails/manifests, and write renditions to object storage. A search index supports title/description queries. A CDN caches manifests and media segments close to viewers. Event ingestion and stream processing aggregate views and likes into read-optimized stores.

## End-to-end flow

1. Creator requests an upload session. The service authorizes ownership, creates a video in `UPLOADING`, and returns bounded, expiring part-upload URLs.
2. Client sends chunks to object storage and retries failed chunks independently. Completion verifies expected parts and checksums, then marks the source durable.
3. A transactional outbox or reliable event publication schedules processing. Workers claim jobs idempotently using video ID plus processing version.
4. Workers validate the media, create several codecs/resolutions and a segmented manifest, store outputs, and report readiness. Failures retry with backoff; poison jobs enter a dead-letter queue.
5. Once required renditions exist, metadata changes to `READY` or `PUBLISHED` according to creator intent. Search indexing and thumbnail generation happen asynchronously.
6. Viewer requests the watch page. The service checks visibility and returns metadata plus an authorized manifest reference.
7. Player fetches manifest and segments from the CDN, adapts rendition to observed bandwidth, and emits playback/engagement events asynchronously.

## Scaling, consistency, and reliability

Object storage is the source of truth for bytes; metadata storage is the source of truth for publication state. Keep them distinct and use explicit states to avoid displaying a video whose assets are incomplete. Large video transfer is bandwidth-heavy but does not scale with metadata QPS on the API fleet. CDN cache keys should include rendition and immutable asset version. Purge or version manifests when metadata or access changes; use short-lived authorization for private content so cached bytes do not bypass visibility checks.

Use adaptive bitrate streaming (for example HLS/DASH-style segmenting) to reduce stalls on variable networks. Transcoding is CPU-intensive and bursty, so queue jobs by priority and capacity, track age, and cap per-upload fan-out. Popular videos benefit from CDN caching; long-tail content may be fetched from origin. Views are at-least-once events: deduplicate by event/session policy or accept approximate aggregates. Strongly consistent records are appropriate for ownership and privacy, while counters and search can lag.

## Deep dives and failure cases

**Resumable upload and processing:** A client may lose its connection after uploading only some parts. Part numbering and checksums allow safe retry; completion must verify the assembled object before processing. Processing workers can crash after writing some renditions, so outputs use deterministic versioned names and the state transition is idempotent.

**Privacy and deletion:** A public CDN cache can outlive a permission change. Keep sensitive authorization at manifest issuance, use short TTLs or signed paths, and support invalidation for urgent takedowns. Deletion should first make metadata inaccessible, then asynchronously remove originals, renditions, indexes, and cached objects with auditable retries.

## Quick recall

Direct-to-object-store resumable upload; durable queue; idempotent transcode workers; object storage plus CDN for playback; adaptive segmented media; metadata gates publication; asynchronous approximate engagement. Treat privacy and deletion propagation as first-class paths.
