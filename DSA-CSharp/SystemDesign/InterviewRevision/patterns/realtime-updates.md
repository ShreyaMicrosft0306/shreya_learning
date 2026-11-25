# Real-time updates

Real-time delivery keeps a client informed shortly after server-side state changes. “Real time” usually means a bounded delay, not literally zero delay. Choose a mechanism based on how fresh the information must be, how many clients receive it, and whether communication is one-way or two-way.

## When to use

Use periodic polling when a delay of several seconds is acceptable, clients are few, or the system is simple. Use Server-Sent Events (SSE) for a long-lived, one-way stream from server to browser, such as a progress feed or live dashboard. Use WebSockets when both sides need to send frequent messages over one persistent connection, as in collaborative editing, chat, or a multiplayer game. A conventional request/response API remains preferable when updates are infrequent or only needed while a page is open.

## Architecture and flow

With polling, the client requests the current state on a timer, optionally using a cursor such as “changes after version 42.” The server returns changes or a full snapshot. Long polling holds a request open until data changes or a timeout, then the client reconnects; it reduces empty responses but still uses HTTP requests.

With SSE, the client opens an HTTP stream and the server emits text events with optional event IDs. Browsers can reconnect and send the last event ID, allowing the service to resume if it retains events. With WebSockets, an HTTP upgrade establishes a bidirectional connection. The client and server exchange framed messages until either side closes the connection.

In all cases, a typical design persists important state or events separately from the live connection. A notification service publishes changes, connection gateways route them to subscribed clients, and clients fetch a snapshot or missing updates after reconnect. A broker can decouple producers from gateways, but the system must map subscriptions to the right recipients.

## Delivery semantics and tradeoffs

“At-most-once” means an update may be lost but is not deliberately repeated. “At-least-once” retries delivery, so duplicates are possible. “Exactly-once” end-to-end is generally not a transport guarantee: it is approximated by unique event IDs, deduplication, and idempotent handling. Acknowledgment means a receiver confirmed a defined step; clarify whether that means received, processed, or durably stored.

Polling is easy to operate but creates repeated work and can make freshness uneven. SSE is simpler than WebSockets for server-to-client events and works with normal HTTP infrastructure, but it does not provide a client-to-server event channel. WebSockets reduce per-message overhead and support two-way interaction, but require connection lifecycle management and compatible proxies.

## Failure modes and scaling

Connections can drop, events can arrive late or out of order, and reconnects can overwhelm a service. Include heartbeats or timeouts, exponential backoff with jitter, event sequence numbers, bounded replay windows, and a snapshot fallback when a cursor is too old. Keep gateway memory bounded; clients that cannot keep up should be disconnected or told to resynchronize rather than accumulate unbounded buffers.

Scale gateways horizontally and route messages by user, room, or topic through a broker or partitioned event log. Sticky routing can simplify local connection state but does not replace shared subscription or delivery coordination. Protect the service with per-client limits, authorization on subscriptions, backpressure, and metrics for connection count, lag, reconnects, and dropped updates.

## Example prompts

- Design live order tracking for customers and couriers.
- Design a notification feed with unread counts and reconnect support.
- Design live collaboration presence for a shared document.

## Interview quick checklist

- What freshness target and client platforms are required?
- Is communication one-way or bidirectional?
- What delivery guarantee matters, and how are duplicates handled?
- How do clients resume, detect gaps, and recover from stale cursors?
- How are subscriptions authorized and routed across gateways?
- What are the fan-out, connection, bandwidth, and slow-client limits?
