# Collaborative Document Editing

## Scope and assumptions

Design a browser-based editor where multiple authorized people edit one text-rich document concurrently and see changes quickly. Users may go offline briefly, reconnect, and continue. The design covers document text, cursors/presence, and durable history; complex page layout, embedded media, and full offline conflict UX are simplified. A named product is inspiration for the problem category, not a claim about any particular company's implementation.

## Requirements and exclusions

**Must:** preserve acknowledged edits, converge concurrent edits, support reconnect and access control, and keep typing responsive. Show collaborator presence on a best-effort basis.

**Not in scope:** pixel-perfect rendering, arbitrary binary-object collaboration, end-to-end encryption key management, or guaranteeing instantaneous global synchronization. Presence can be lost without losing document content.

## Data model and API

- `Document`: ID, owner, current version, schema, and metadata.
- `Operation`: document ID, actor/session ID, base version, operation ID, operation payload, and server sequence.
- `Snapshot`: document ID, version, compact document state, and checksum.
- `Acl`: document, principal, role, and revision.
- `Presence`: ephemeral session, cursor/selection, and expiry.

`POST /documents/{id}/ops` submits a batch with operation IDs and a base version. `GET /documents/{id}?since=version` returns a snapshot or missing operations. A WebSocket carries operations, acknowledgements, and presence updates. Every write is checked against current ACL.

## Architecture

The client maintains a local document model and unacknowledged operation queue for immediate editing. A collaboration gateway manages authenticated WebSocket sessions and routes each document to a sequencer/replica group. The chosen merge strategy is either operational transformation (transform operations against intervening operations) or a CRDT (operations carry stable identities and merge deterministically); use one coherent model per document format. The document service persists ordered operations and periodic snapshots. A pub/sub layer fans changes to active sessions. Presence is kept in expiring memory or a short-lived store, separate from durable content. ACL and identity services authorize joining and every mutation.

## End-to-end flow

1. The client fetches the latest snapshot and operation tail, validates access, then opens a WebSocket with its session identity and last acknowledged version.
2. Each keystroke changes the local view immediately and creates an operation with a stable unique ID. It is queued until acknowledged.
3. The gateway authenticates the session and forwards the operation to the document's sequencing or merge service.
4. The service checks ACL, deduplicates operation ID, and reconciles the submitted base version. OT transforms against unseen operations; a CRDT merges by its defined ordering rules.
5. The accepted operation receives a durable sequence/version and is persisted before the server acknowledges it. Rejection or an ACL change is explicit so the client can stop or reconcile.
6. Pub/sub distributes the committed operation to connected collaborators. Each client applies it to its local state and transforms/rebases its pending edits as required.
7. Cursor and selection updates travel on a low-priority presence channel with throttling and expiry. They are never replayed as document edits.
8. Snapshot compaction periodically stores a checksummed state plus version. Reconnecting clients fetch the snapshot and tail, then resume pending operations using their IDs.

## Scaling, consistency, and reliability

Partition by document ID to keep coordination local to collaborators on one document. Hot documents may need a dedicated sequencer or larger replica group; splitting a single document across unrelated writers complicates ordering. Durable operation append precedes acknowledgement; snapshots are derived and can be rebuilt from log history. WebSocket nodes are stateless except for live connections, so reconnect is normal. Use bounded operation batches, backpressure, rate limits, and snapshot catch-up instead of unlimited buffering. Versioned ACL checks matter: revocation must stop future writes and close or downgrade sessions. Presence may be eventually consistent; content convergence and acknowledged durability are stronger requirements.

## Deep dives

**OT versus CRDT:** OT can be compact and natural for centrally sequenced text, but transforms must be correct for every operation pair and path. CRDTs simplify merge independence, especially offline, but identifiers and tombstones can grow and semantics for structured rich text are subtle. Select based on offline duration, document structure, and proven library behavior, then test convergence under adversarial operation order.

**Lost acknowledgement:** A client reconnects after its operation committed but before its acknowledgement arrived. Stable operation IDs let the server recognize the retry and return the original result, avoiding a duplicate insertion.

## Quick recall

Local optimistic edits → authenticated per-document merge/order → persist before ack → broadcast → rebase/converge. Keep ephemeral presence separate, use snapshots for catch-up, and make retries idempotent.
