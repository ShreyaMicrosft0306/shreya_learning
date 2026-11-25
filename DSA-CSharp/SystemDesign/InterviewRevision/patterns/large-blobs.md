# Large blobs and files

A blob is a large, opaque object such as an image, video, archive, or document. Relational databases are good at metadata and relationships, but storing very large file bytes in them can inflate backups, replication, and query costs. A common design stores bytes in object storage and keeps ownership, permissions, size, content type, and object key in a database.

## When to use

Use object storage when files are large, numerous, or need durable low-cost storage. Keep small records and searchable metadata in the application database. A local filesystem may be sufficient for a single-instance tool, but shared, durable object storage is usually safer for a distributed service. Decide limits, allowed formats, privacy, retention, and deletion rules before exposing uploads.

## Architecture and upload flow

For a small upload, the client can send the file to the application, which validates and forwards it. This is simple but consumes application bandwidth and worker capacity. For larger files, use a direct-to-object-storage flow: the client asks the API for permission to upload; the API authenticates and authorizes, creates a pending metadata record, then issues a short-lived signed URL or upload token scoped to one object key and allowed operation. The client uploads bytes directly to storage. The API or a storage event then verifies completion and marks metadata ready.

For very large or unreliable transfers, multipart upload splits a file into numbered parts. Parts can be retried independently and finalized into one object. Store upload state and expire abandoned multipart sessions. Validate actual size, checksum, and content type; a declared type alone is untrusted. Malware scanning or media processing can run asynchronously before the object becomes available.

## Download flow and CDN

A download request first checks authorization and metadata, then returns a short-lived signed download URL or streams through the application when policy requires inspection or transformation. A Content Delivery Network (CDN) caches objects at edge locations near users, reducing latency and origin traffic. Use immutable, versioned object keys where possible; cache headers then remain safe when content changes. Private content needs signed access, short expiry, or an authenticated edge policy.

## Tradeoffs, failures, and scaling

Direct uploads scale bandwidth independently from application servers, but shift validation and lifecycle complexity to the service. Proxying gives tighter control and can simplify auditing, but app servers become a bandwidth bottleneck. Object storage is durable, not automatically a full application-level transaction: the database record may exist before bytes arrive, or bytes may be uploaded while finalization fails. Use explicit pending/ready/deleted states, idempotent completion, and a reconciliation job to find orphaned objects or stuck records.

Network interruptions can leave partial uploads; support resumable parts and cleanup. Duplicate callbacks may arrive, so completion handlers must be repeatable. A leaked signed URL grants access until expiry, so scope it narrowly, make it short-lived, and never put long-lived secrets in clients. Deletion may need to remove versions, thumbnails, and CDN copies; specify a retention and cache-purge policy. Encrypt at rest and in transit, restrict bucket permissions, and avoid predictable public keys.

Scale with multipart concurrency limits, quotas, storage lifecycle tiers, CDN caching, and asynchronous derivatives. Watch upload completion rate, failed parts, storage growth, egress cost, cache hit ratio, and orphan count.

## Example prompts

- Design photo uploads and private sharing for a social app.
- Design video upload, processing, and playback.
- Store user documents with retention and download controls.

## Interview quick checklist

- Where do bytes and searchable metadata live?
- Is upload proxied or direct, and how is access scoped?
- How are large/resumable uploads verified and finalized?
- How are authorization, scanning, retention, and deletion enforced?
- How do CDN caching and object versioning avoid stale content?
- How are partial uploads and orphaned objects reconciled?
