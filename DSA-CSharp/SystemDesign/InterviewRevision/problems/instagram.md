# Photo-sharing social network

## Scope and assumptions

Design a service for following accounts, publishing photo/video posts, viewing a home feed, and liking or commenting. Users expect uploads to survive retries and feeds to load quickly on mobile. Media delivery is global and read-heavy; feed freshness is near-real-time, not transactionally synchronized with every follow. Stories, direct messages, live video, advertising, and advanced content moderation are excluded, though the design leaves room for them.

## Requirements and exclusions

- Upload media and metadata, then view a user profile and a ranked or chronological feed.
- Follow/unfollow accounts; like and comment on posts; paginate safely.
- Enforce account privacy and block rules before returning content.
- Exclude video transcoding internals, recommendation-model training, and exact ranking algorithms.

## Data model and API

Use `User(id, privacy, profile_version)`, `Follow(follower_id, followee_id, created_at)`, `Post(id, author_id, caption, media_keys[], visibility, created_at)`, `Like(user_id, post_id, created_at)`, and `Comment(id, post_id, author_id, text, created_at)`. Unique constraints on follow pairs and likes make retries idempotent. Store media bytes in object storage; metadata is the source of truth. Feed entries are derived references `(viewer_id, post_id, score_or_time, source_version)`.

APIs include `POST /media/uploads` (returns a signed upload URL), `POST /posts`, `GET /feed?cursor=`, `GET /users/{id}/posts`, `PUT /users/{id}/follow`, and post like/comment endpoints.

## Architecture

An API layer performs authentication and privacy checks. A post service commits metadata and an outbox event to a sharded database. A media pipeline validates uploaded objects, generates thumbnails and multiple playback sizes, and records ready variants in object storage/CDN. Feed services maintain per-user candidate lists, while a ranking service merges candidates with fresh posts. Follow edges, likes, and comments use stores chosen for their access patterns; counters are asynchronous projections. Push notifications are best-effort event consumers.

## End-to-end request/data flow

1. **Upload and publish:** Client asks for an upload URL, sends bytes directly to object storage, then calls the post API with the object key. The API verifies upload ownership and readiness, writes post metadata plus outbox event atomically, and returns a post ID. The media pipeline can finish derivatives after publication; clients may use the original until variants are ready.
2. **Distribute post:** An event consumer finds followers and adds a post reference to their feed inboxes for ordinary accounts. For accounts with very large audiences, it records the post in an author outbox rather than writing to every follower.
3. **Read feed:** Feed API fetches recent inbox candidates, adds outboxes for followed high-follower authors, removes duplicates, checks visibility/block state, ranks, and returns a page with a cursor. Media URLs reference CDN objects; feed items never embed image bytes.
4. **Engage:** Like/comment writes go to their canonical stores. Counters and notification events update asynchronously; a client can optimistically show its own action while the server response remains authoritative.

## Scaling, consistency, and reliability

Partition social edges and inboxes by user; partition posts by author plus time or post ID. CDN caching avoids repeatedly transferring large media. Feed inboxes are bounded windows, and older history can be rebuilt from followed-author timelines. Fanout-on-write is fast for reads but expensive for celebrity posts; fanout-on-read saves writes but increases feed latency, so use a hybrid threshold based on follower count and write load. Follow/privacy changes require prompt enforcement at read time even if inbox cleanup is delayed. Post metadata and upload authorization are durable; feed order, counters, and notifications are eventually consistent. Use cursor pagination with a stable tuple so new posts do not shift existing pages. Queue consumers must be idempotent, retry with backoff, and expose lag metrics.

## Deep dives and failure cases

**Media upload versus post commit:** A client may upload bytes and lose connectivity before publishing. Keep uploads in a temporary state with expiry and cleanup; publication is a separate idempotent metadata operation. If derivative generation is delayed, serve an available rendition and mark processing state rather than failing the post read. Never treat a client-supplied object key alone as authorization.

**Celebrity feed overload:** Writing each post to millions of inboxes creates a queue spike and delays normal users. Route high-fanout authors through an outbox, then merge a bounded number of their recent posts during feed reads. Cache author timelines and cap candidates per source. If ranking is unavailable, return a deterministic recency-ordered feed; do not block the entire social graph on personalization.

## Quick recall

Object storage/CDN for media; metadata and outbox for durable publishing; hybrid push/pull feed fanout; privacy checked on reads; derived counters and notifications; bounded candidate merge with stable cursors.
