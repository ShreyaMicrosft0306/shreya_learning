# WhatsApp-style messaging

## Scope and assumptions

Design private one-to-one messaging with multi-device delivery, history, offline support, and optional media. Group messaging extends delivery fan-out. Order messages per conversation; end-to-end encryption means servers route ciphertext.

## Requirements and exclusions

**Key requirements:** send and receive messages reliably; support offline recipients, acknowledgments, delivery/read state, reconnect synchronization, multi-device sessions, and media transfer. Preserve conversation ordering and avoid duplicate display after retries.

**Out of scope:** calls, public channels, key-verification UX, server-side content search, and delivery after permanent loss of every device/backup.

## Data model and API

`Conversation(conversation_id, type, member_ids, created_at, version)` defines participants. `Message(conversation_id, sequence, message_id, sender_device, ciphertext_ref, created_at, client_id)` is immutable and ordered per conversation. `Device(user_id, device_id, push_token, key_bundle, last_ack_seq, state)` represents endpoints. `Receipt(message_id, device_id, state, updated_at)` tracks sent/delivered/read state. `MediaObject(object_id, size, content_hash, encrypted_object_key)` stores encrypted blobs.

`POST /messages` sends an encrypted payload with client-generated message ID and conversation ID; server returns assigned sequence and acceptance status. `GET /sync?cursor=` returns encrypted messages and state changes after a cursor. `POST /receipts` acknowledges sequence ranges. `POST /media/upload-session` returns a short-lived upload URL. Push payloads contain a wake-up hint, not plaintext content.

## Architecture

Clients connect to regional gateways. Authentication validates devices and key registration. A conversation router assigns a home partition for ordered writes. Message storage persists ciphertext and sequence; delivery service handles online devices and offline queues. Push wakes sleeping devices. Encrypted media uses object storage; sync replays from durable cursors. Servers cannot decrypt bodies.

## End-to-end flows

1. **Device setup:** Client registers an authenticated device and public key material. Other clients can retrieve the key bundle needed to encrypt a session; identity/key changes are surfaced to users according to security policy.
2. **Media send:** Sender encrypts media locally, uploads ciphertext through a scoped multipart session, and receives an opaque object reference. The encrypted message contains that reference and decryption material for the recipient.
3. **Send message:** Client encrypts message for recipient device(s), assigns a stable client message ID, and sends over its connection or HTTPS fallback. Gateway authenticates, checks conversation membership and size limits, then routes to the conversation's home partition.
4. The partition deduplicates client ID, assigns the next conversation sequence, durably stores ciphertext, and appends a delivery event before acknowledging acceptance.
5. **Deliver:** Online recipient devices receive the message; offline devices get a push hint. Delivery workers track acknowledgments and retry with bounded backoff. Recipient sync uses the last acknowledged sequence to fetch missed messages.
6. **Receipts:** Recipient acknowledges received sequence ranges; service records device receipts and can aggregate a user-visible delivered state. Read receipts are a separate policy-controlled event.
7. **Reconnect:** Device presents cursor and device identity; sync returns ordered messages and conversation changes. Client deduplicates by message ID/sequence and advances its cursor only after persisting local state.

## Scaling, consistency, and reliability

Partition by conversation ID for ordered writes without global coordination. Large groups create fan-out pressure; store one message and deliver asynchronously in batches. Acceptance means ciphertext is durable, not that every device received it.

Connections reduce latency but are not storage. Clients sync from durable cursors; push may drop. Use at-least-once delivery, stable IDs, and client deduplication. Receipts are per-device and can be aggregated. Define retention and deletion separately; server-side indexing conflicts with end-to-end confidentiality.

## Deep dives and failure cases

**Ordering and retries:** If acknowledgment is lost, retry with the same client ID and deduplicate by sender/device and conversation. The partition assigns sequence numbers. On failover, recover committed sequence state before writes to avoid reuse or reordering. Clients show pending sends until sequence assignment.

**Multi-device encryption and delivery:** Devices may need separate encrypted envelopes. Device membership changes can require key rotation; servers route but do not decrypt. Bound offline queues; sync retrieves retained history. Push failure is harmless if messages remain durable. Reauthorize media URL access after membership checks without prematurely deleting the ciphertext.

## Quick recall

Use a conversation partition for ordered durable writes, then asynchronous per-device delivery. Client IDs and sync cursors make retries and reconnects safe; push is only a hint. In end-to-end encryption, servers store and route ciphertext, and multi-device/key lifecycle decisions shape the protocol.
