# Facebook-style news feed

## Scope and assumptions

Design a personalized feed of posts from followed accounts, pages, or groups. Users can react and comment. New posts should appear promptly, but exact global ordering is unnecessary.

## Requirements and exclusions

**Key requirements:** create posts; retrieve a paginated personalized feed; enforce visibility and blocks; rank relevant recent items; support reactions and comments; tolerate high fan-out and celebrity accounts.

**Out of scope:** messaging, ad auctions, moderation workflow, and video transcoding. Privacy and deletion remain correctness requirements.

## Data model and API

`Post(post_id, author_id, body_ref, media_refs, audience, created_at, state)` stores canonical content. `Follow(follower_id, followed_id, state)` defines graph edges. `FeedEntry(viewer_id, post_id, source_id, published_at, score_features?)` is an optional candidate index, not the source of truth. `Reaction(user_id, post_id, type, updated_at)` and `Comment(comment_id, post_id, author_id, body, created_at, state)` support interactions. `Visibility` rules include audience, group membership, blocks, and account state.

`POST /posts` creates a post with an idempotency key. `GET /feed?cursor=` returns ranked post IDs and content. `POST /posts/{id}/reactions` upserts the viewer's reaction. `GET /posts/{id}/comments?cursor=` paginates comments. Use opaque cursors tied to a ranking snapshot or timestamp boundary, rather than page numbers that shift as new posts arrive.

## Architecture

Post service stores canonical posts and emits durable events. Media resides in object storage behind a CDN. Social graph serves follow and membership queries. Feed generation uses fan-out-on-write for ordinary authors and read-time retrieval for high-follower authors. Ranking scores candidates; Feed API checks visibility, hydrates posts, and paginates. Interaction services update counts asynchronously.

## End-to-end flows

1. **Publish:** Client uploads media directly to object storage using a scoped upload URL, then submits post text and media references. Post service validates audience and references, writes canonical post, and appends an outbox event.
2. A distribution worker consumes the event and retrieves followers or eligible group members. For ordinary accounts, it inserts lightweight feed candidates into follower timelines. For very large accounts, it records the post once and leaves it for read-time merge, avoiding enormous synchronous fan-out.
3. **Read feed:** Client requests a feed with cursor. Candidate service combines precomputed timeline entries with recent posts from high-follower sources and, if useful, other recommendation candidates.
4. Ranking service scores candidates using freshness, relationship, prior engagement, and safety signals. The feed API checks current visibility, removes duplicates and deleted posts, hydrates content, and returns a stable continuation cursor.
5. **Interact:** Reaction endpoint authenticates and upserts one user's reaction. An asynchronous consumer updates approximate counts and ranking features. Comments are separately paginated and moderated.
6. **Delete/privacy change:** Canonical post state or audience is updated first; invalidation events remove cached and precomputed entries over time. Reads still validate access so a stale feed index cannot reveal content.

## Scaling, consistency, and reliability

Candidate generation trades write amplification for read speed. Write fan-out is fast to read but costly for celebrity posts; read fan-out avoids the write burst but increases read work. A hybrid threshold balances cost. Store IDs and timestamps rather than duplicate posts.

The post store is authoritative; indexes are repairable. An outbox prevents lost distribution events, and consumers deduplicate `(viewer_id, post_id)`. Ranking and counters may lag. Check current access or strictly invalidate versioned privacy data. Cursors should tolerate new entries without duplicates or skips.

## Deep dives and failure cases

**Hybrid fan-out:** No universal follower cutoff exists; use cost and read-latency signals. During backlog, prioritize fresh posts and merge missing candidates at read time. If a shard fails, return a smaller feed, never bypass visibility checks.

**Privacy and deletion:** Precomputed items can outlive permission changes; IDs are not permission. Validate audience on reads and use canonical tombstones to prevent deleted content resurfacing. If policy data is unavailable, fail closed. Counts may lag, while reaction upserts must be idempotent.

## Quick recall

Persist canonical posts, derive feed candidates, and rank at read time. Use write fan-out for most authors and read-time merge for very large ones. Treat indexes as disposable, privacy as authoritative, and cursor pagination plus idempotent workers as core correctness tools.
