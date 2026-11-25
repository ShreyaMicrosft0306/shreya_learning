# Distributed Rate Limiter

## Scope and assumptions

Design a shared service that decides whether an identified client may perform an operation within a configured quota. Clients can be users, API keys, tenants, or combinations such as tenant plus route. Requests arrive at many application instances, and the decision should normally complete within a few milliseconds. Policies may be changed without redeploying callers. The limiter returns a decision; it does not authenticate callers, enforce business authorization, or meter billing-grade usage.

## Requirements and exclusions

**Required:** enforce limits across application replicas; support per-key policies and useful windows; expose remaining capacity and retry guidance; degrade deliberately when storage is unhealthy; protect the backing store from unbounded key cardinality.

**Not required:** a globally perfect, linearizable count for every request, arbitrary analytics, or exactly-once billing. The design prioritizes predictable enforcement and availability. A product can select strict or approximate behavior per policy.

## Data model and API

A policy is `{policy_id, scope, limit, window, algorithm, mode, version}`. Scope describes the key dimensions and operation; mode selects strict enforcement or bounded approximation. Runtime state is keyed by `(policy_id, subject_key, time_bucket)` and stores counters or algorithm state plus expiry. Subject keys should be derived from authenticated identifiers, normalized, and protected from leaking raw credentials in storage or logs.

An internal API can be `Check(subject, operation, cost=1) -> {allowed, remaining, retry_after_ms, policy_version}`. A batch form accepts several checks to amortize network cost. Policy management uses authenticated `PutPolicy` and `GetPolicy` calls; changes carry versions so stale caches can be recognized.

## Architecture

An application-side SDK performs local validation, caches policy metadata, and sends checks to a regional limiter tier. Limiter instances are stateless and partition keys consistently across a replicated in-memory data store, such as Redis. Atomic scripts or transactions combine read, decision, increment, and expiry so concurrent requests cannot both consume the same last token. A policy control plane validates and distributes versioned configurations. Metrics and sampled decision logs feed operations, not the synchronous decision path.

## End-to-end flow

1. The caller authenticates the request and derives a stable subject and operation key.
2. The SDK reads a cached policy version; if unavailable, it requests policy metadata from the limiter or control-plane cache.
3. It sends a check, including request cost and policy version, to the regional limiter.
4. The limiter hashes the key to its state shard and runs one atomic operation. It evaluates current state, consumes capacity only if allowed, and refreshes bounded expiry.
5. It returns allow/deny, remaining capacity, and an estimated retry delay. The application rejects or continues accordingly.
6. Asynchronously, the policy plane distributes edits and telemetry reports latency, denials, store health, and hot keys.

## Scaling, consistency, and reliability

Use a token bucket for smooth average rate plus a controlled burst: refill at rate `r` up to capacity `B`, then spend request cost. A fixed window is cheaper but permits boundary bursts; a sliding log is accurate but stores many events; sliding counters approximate a moving window with small state. Choose according to policy semantics. Shard by a stable hash and replicate for availability. A hot subject can overload one shard; isolate exceptional tenants, use local prefilters, or split hot-key state only if the resulting consistency tradeoff is acceptable.

Regional enforcement avoids a network round trip to a global quorum. It means a subject active in two regions may receive extra quota. Global strictness requires coordination or quota leasing, trading latency and availability for tighter bounds. For leased quotas, regions receive bounded token allocations and periodically reconcile. During a store outage, policy selects fail-open (availability, possible excess), fail-closed (protection, possible false rejection), or a local emergency budget. Never silently treat a failed check as an ordinary allow.

## Deep dives and failure cases

**Atomicity:** Separate `GET` and `INCR` calls race at the limit. A script/transaction must calculate refill, test capacity, decrement, and set expiry atomically. Include policy version in state or reset/migrate state carefully when limits change; lowering a limit should not accidentally reset usage.

**Clock and retry behavior:** Use store-side time or a monotonic source where possible, clamp negative elapsed time, and avoid depending on synchronized client clocks. `retry_after` is an estimate, not a reservation. Retry storms can worsen overload, so return jitterable guidance and let clients apply backoff.

## Quick recall

Define identity and policy key first. Keep the decision path regional, atomic, and bounded. Token buckets handle bursts; strict global limits require coordination. Make outage behavior explicit, version policy, expire state, and monitor hot keys.
