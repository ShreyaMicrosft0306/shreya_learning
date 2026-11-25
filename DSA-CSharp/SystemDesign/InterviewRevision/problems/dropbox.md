# Dropbox: file synchronization and sharing

## Scope and assumptions

Design cloud file storage that synchronizes files and folders across devices and supports sharing. Files may be large, clients disconnect, and devices can edit concurrently. Keep metadata separate from bytes in object storage. This is not a filesystem protocol or collaborative editor.

## Requirements and exclusions

**Key requirements:** upload, download, list, rename, delete, synchronize changes across devices, share access, and recover from interrupted transfers. Preserve prior versions sufficiently for recovery. Enforce per-user authorization and avoid exposing private object keys.

**Out of scope:** global block deduplication, real-time co-editing, OS file locking, and conflict-free merges of arbitrary binary files.

## Data model and API

Use `Node(node_id, owner_id, parent_id, name, type, current_revision, deleted_at)` for files and folders. `Revision(revision_id, node_id, content_hash, size, object_key, created_at, actor_id, status)` describes immutable content versions. `Share(share_id, node_id, grantee, role, expires_at)` stores explicit access grants. `DeviceCursor(user_id, device_id, change_seq)` tracks each client's last acknowledged change. Names are unique within a parent according to a documented case/normalization policy.

`POST /files/upload-session` creates a resumable upload and returns authorized part URLs. `POST /files/{id}/commit` publishes the completed revision using an expected revision or idempotency token. `GET /files/{id}/download` authorizes and returns a short-lived download URL. `GET /changes?cursor=` returns ordered metadata changes and the next cursor. Metadata endpoints support create, move, rename, trash, restore, and share operations.

## Architecture

An API handles metadata; an upload coordinator creates transfer sessions. A transactional database stores nodes, revision pointers, permissions, and change sequence. Object storage holds immutable content. A change-log/outbox wakes devices, which pull from a durable cursor API. CDN gateways accelerate downloads; optional malware scanning gates new revisions.

## End-to-end flows

1. **Upload start:** Client hashes or sizes a local file, asks for an upload session, and sends parent, name, size, and current revision. The service checks quota, permission, and optimistic concurrency, then creates a pending revision.
2. The coordinator returns bounded, time-limited multipart upload URLs. Client uploads parts directly to object storage and retries failed parts independently.
3. **Commit:** Client tells the service which parts completed. The service verifies the object exists, validates checksum/size, finalizes an immutable object, and atomically points the node at the new revision while appending a change-log entry.
4. **Sync:** A device receives a push hint or periodically calls `/changes` with its cursor. It applies ordered metadata updates and fetches content for changed files, then acknowledges the cursor only after local state is safely recorded.
5. **Download:** Client obtains an authorized short-lived URL for a specific revision and downloads from object storage/CDN. It verifies the content hash before replacing its local copy.
6. **Concurrent edit:** If the expected current revision no longer matches at commit, preserve both revisions and create a conflict copy or expose a conflict state; do not silently discard either user's bytes.

## Scaling, consistency, and reliability

Keep large bytes out of the metadata database. Immutable revisions simplify retries, history, and caching. Conditional metadata writes prevent concurrent commits from claiming the same base revision. Durable cursors are the source of truth; push is only an optimization. Clients reconnect by cursor and can reconcile a snapshot if it expires.

Multipart transfer limits retry cost; expire sessions and clean unfinished objects. Publish only after verifying parts and checksums. Route downloads away from regions lacking a replicated object. Check permissions before issuing a short-lived signed URL.

## Deep dives and failure cases

**Conflict handling:** Last-write-wins can destroy offline edits. Compare the client's base revision at commit and retain both versions with a deterministic conflict copy name. Define behavior for name conflicts, moves, and delete-versus-edit; do not silently overwrite.

**Change delivery:** Push can drop messages during sleep or network transitions. Atomically write metadata and change-log records, then treat notifications as hints. Cursor pagination reconstructs missed updates; consumers deduplicate by change sequence and node/revision ID. Commit idempotency tokens make timed-out retries safe.

## Quick recall

Separate metadata from immutable blob storage. Use resumable uploads, checksum validation, atomic revision publication, and cursor-based change sync. Push is best-effort acceleration; conflict preservation and short-lived authorized download URLs protect correctness and access.
