# LLM Chat Serving

## Scope and assumptions

Design a conversational service that accepts user turns, constructs model context, streams generated output, and stores conversation history. The model may be hosted internally or through a provider. Generation is expensive, can take seconds, and may fail mid-stream. This is a serving architecture, not a proposal for model training or a claim about any particular product's internals.

## Requirements and exclusions

**Must:** authenticate users, preserve conversation ordering, stream low-latency output, enforce model/token budgets, handle cancellation and retries safely, and protect user content. Support model routing and operational observability.

**Not in scope:** training/fine-tuning, guaranteeing factual correctness, or treating model text as trusted executable instructions. Tool execution needs a separate permissioned boundary.

## Data model and API

- `Conversation`: ID, owner, creation time, title/metadata, and version.
- `Message`: ID, conversation, role, content reference, sequence, status, model/version, and timestamps.
- `Generation`: assistant message ID, request ID, selected model, token budget, state, and usage.
- `Policy`: user/tenant limits, allowed models, retention, and safety configuration.

`POST /v1/conversations/{id}/messages` accepts a user message and idempotency key; a streaming response emits events such as `started`, `delta`, `completed`, or `error`, each with a message ID. `GET /v1/conversations/{id}` paginates history. Cancellation targets a generation ID.

## Architecture

An edge/API tier authenticates, rate-limits, and checks tenant policy. Conversation service persists the user turn and reserves a generation slot. A context builder retrieves recent messages and approved knowledge, applies truncation/summarization under a token budget, and constructs the model request. A model router chooses an available deployment based on capability, load, cost, and policy. Inference workers run generation; a streaming gateway relays deltas over SSE or WebSocket. A safety/policy layer applies input/output checks and handles tool requests without granting the model direct credentials. Durable object storage may hold larger content, while a database indexes message metadata. Usage and latency events feed billing and operations.

## End-to-end flow

1. The client submits a turn with conversation ID, idempotency key, and streaming preference. The API authenticates ownership and enforces request/token quotas.
2. In a transaction, the service deduplicates the request, appends the user message, and creates a generation record. Concurrent turns are serialized or rejected using the conversation version.
3. The context builder loads a bounded history, applies retention and policy, estimates tokens, and truncates or summarizes older context when needed.
4. The router selects a permitted healthy model/deployment and reserves capacity. If capacity is unavailable, the request is queued within a deadline or rejected with retry guidance.
5. The inference worker generates tokens and emits deltas. The streaming gateway forwards them with sequence numbers and heartbeats; disconnects can be resumed if output is retained, otherwise the generation reports interruption.
6. Safety checks and tool routing follow explicit policy. Tool calls execute in isolated services with scoped authorization; results are treated as new context, not as privileged commands.
7. On completion, the service persists the assistant message, token usage, model metadata, and final status, then emits `completed`. Cancellation stops generation where possible and records a partial/interrupted state accurately.
8. Usage events update quotas and analytics asynchronously. Logs use redaction or content-free identifiers and do not expose prompts by default.

## Scaling, consistency, and reliability

Partition conversation storage by conversation ID; preserve per-conversation message order while scaling inference independently. User-message acceptance and idempotency need strong writes; token streaming is transient and may be resumable only within a bounded retention window. GPU pools are expensive, so use admission control, batching where latency permits, per-tenant fairness, and separate queues for model classes. Backpressure should stop new generations before memory or GPU saturation. Retry only before generation is known to have started or when the provider supports idempotent request recovery; otherwise a retry can double cost or produce a different answer. Track queue wait, time-to-first-token, tokens/second, cancellations, and error rates. Apply encryption, access controls, retention limits, and explicit data-use policies.

## Deep dives

**Streaming disconnect:** The client may lose its connection while generation continues. Store bounded deltas or the final answer keyed by generation ID, allow authenticated resume, and support cancellation. Avoid generating indefinitely for abandoned sessions.

**Context and cost:** Longer history improves continuity but increases latency, cost, and distraction. Use an explicit token budget, preserve recent turns and durable user-approved facts, and disclose when context is summarized or omitted.

## Quick recall

Persist turn → build bounded context → route with capacity reservation → stream generation → finalize message and usage. Serialize turns, isolate tools, make cancellation real, and treat model output as untrusted.
