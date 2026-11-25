# Live Event Comments

## Scope and assumptions

Design a real-time comment stream attached to a live event, such as a match, concert, or broadcast. Viewers post short text messages and receive a near-live feed. A room may range from a small audience to a very large event with uneven popularity. The design focuses on fan-out, ordering, moderation, and reconnect behavior. It excludes video delivery, private messaging, and elaborate threaded discussions.

## Requirements and exclusions

**Required:** authenticate posters; publish comments to participants in the same event; enforce size/rate limits; support moderation and user blocking; reconnect after transient network loss; avoid a single hot event bottleneck.

**Not required:** global total order, guaranteed delivery to every disconnected viewer, or exact synchronization with the video stream. Comments are ephemeral social data and can tolerate small delays and bounded loss, but abuse controls and removal need reliable handling.

## Data model and API

`Comment(comment_id, event_id, author_id, body, created_at, sequence?, status, version)` stores canonical accepted content. `status` may be `VISIBLE`, `HELD`, `REMOVED`, or `DELETED`. A separate moderation action record captures actor, reason, and time. A room cursor is an opaque offset, not a user-facing global sequence.

APIs: `POST /events/{event}/comments` with an idempotency key and text; WebSocket `SUBSCRIBE(event_id, cursor)` and server `COMMENT`, `REMOVE`, `SLOW_MODE`, `HEARTBEAT` messages; `GET /events/{event}/comments?after=cursor` for gap recovery; `DELETE /comments/{id}` for authorized moderation. The service returns a comment ID and acceptance status, not a promise that every viewer received it.

## Architecture

An API gateway authenticates and applies coarse throttles. A comment service validates event membership and text, then appends accepted events to a partitioned durable log keyed by event or event shard. A fan-out layer consumes the log and routes to WebSocket gateways, which maintain connections and per-room subscriptions. A compact recent-comment store supports catch-up and moderation views. A moderation pipeline checks rules and reports abusive content; high-risk rooms can hold messages pending review or enable slow mode. The event control plane manages room state and moderator permissions.

## End-to-end flow

1. Viewer obtains an access token and opens a WebSocket to a nearby gateway, subscribing to an authorized event and presenting the last cursor seen.
2. Gateway validates the token and event entitlement, joins the subscription, and requests a bounded catch-up range from the recent store.
3. On submission, the gateway forwards the message with authenticated author identity and idempotency key; clients cannot choose a trusted timestamp or author.
4. Comment service validates length, encoding, event status, mute/ban state, and rate policy. It either rejects, holds, or appends an accepted event to the log.
5. Log consumers update the recent store and publish the comment to gateways subscribed to the event. Gateways batch or pace deliveries to clients.
6. Clients deduplicate by comment ID, render in room order, and advance their cursor. On reconnect they request missing recent events; old gaps may be intentionally unrecoverable.
7. A moderation action emits a tombstone/update event, which removes or marks the comment in active clients and the catch-up store.

## Scaling, consistency, and reliability

WebSockets are stateful, so gateways track connection-to-room mappings in memory and use a subscription registry for routing. Heartbeats detect dead connections; reconnect with backoff spreads load after outages. The log provides durable ordering within a partition, but not across partitions. A very large event can overwhelm one partition and fan-out tree. Split the event into delivery shards, then broadcast each accepted message to those shards; the service preserves a room-level append order at the producer if needed, while acknowledging the central serialization cost.

Global ordering across regions is expensive and usually unnecessary. Assign a home region per event for writes, or accept regional sequence domains and sort by server timestamp plus ID for display. Store cursors with retention bounds. If the durable log is unavailable, reject posts rather than claim they were accepted; optionally display an explicit degraded status. Fan-out can lag while ingestion remains healthy; track queue age, cap outbound buffers, and disconnect slow consumers rather than allowing memory growth.

## Deep dives and failure cases

**Hot-room fan-out:** Broadcasting one message to many gateways is a multicast workload. A hierarchical pub/sub tree limits duplicated work and isolates failures. Do not have the posting service synchronously contact every socket. Batch messages and allow low-priority comments to be sampled or paced only if product policy explicitly permits it.

**Moderation consistency:** Removal should be a durable tombstone and propagate through the same stream so connected clients converge. A cache or offline client may still retain text; short retention, access-controlled history, and deletion propagation limit exposure. Idempotent IDs prevent client retries from producing duplicate comments.

## Quick recall

Authenticate at the edge, append accepted comments to a partitioned log, fan out asynchronously through WebSocket gateways, and recover from cursors. Order per room where affordable, not globally. Bound slow clients, isolate hot rooms, and make moderation updates durable.
