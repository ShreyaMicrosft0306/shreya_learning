# Web Crawler

## Scope and assumptions

Design a crawler that discovers URLs, fetches pages, extracts links and metadata, and stores results for indexing or analysis. It operates on publicly reachable sites under an explicit allowlist/policy and respects robots rules and host load. The crawler is distributed and may revisit pages. Search ranking, browser rendering for every page, and bypassing access controls are out of scope.

## Requirements and exclusions

**Required:** seed and discover URLs; normalize and deduplicate; schedule fairly; fetch with bounded concurrency; obey robots and politeness delays; retry transient failures; store content and crawl metadata; support recrawl priorities.

**Not required:** exactly-once fetch, full fidelity for dynamic applications, or guaranteed completion of every URL. A crawl is eventually consistent and best-effort; compliance and safe resource limits take precedence over throughput.

## Data model and API

`URLRecord(canonical_url, host, discovered_at, source_url, priority, state, next_fetch_at, last_fetch_at, content_hash, failure_count)` captures scheduling state. `FetchResult(url, fetched_at, status_code, headers, body_uri, content_hash, parser_version)` records an attempt/result. `HostPolicy(host, robots_rules, crawl_delay, concurrency_limit, next_allowed_at)` controls politeness. The canonical URL key should normalize scheme/host case and remove known tracking parameters without merging semantically distinct URLs.

APIs: `POST /seeds` submits approved seed URLs; internal `ClaimBatch(worker, limit)` leases due URLs; `Complete(url, lease, result)` records outcome and discovered links; `GET /crawl/{url}` exposes status for authorized operators. A lease prevents two schedulers from needlessly fetching the same URL concurrently.

## Architecture

Seed and link ingestion flows into a URL normalization and deduplication service. A durable URL frontier stores due work, partitioned by host or host hash. Scheduler workers claim URLs while a per-host coordinator enforces crawl delay and concurrency. Fetch workers use restricted egress, DNS/IP validation, timeouts, response-size caps, and content-type checks. A parser extracts links and metadata; content and raw response bodies go to object storage, while URL state and searchable metadata go to databases/indexes. A retry queue and dead-letter stream isolate persistent failures.

## End-to-end flow

1. Operator submits seeds. The system validates scheme, host, and policy, canonicalizes each URL, and inserts unseen URLs with initial priority.
2. Fetch workers ask the scheduler for work. It selects due URLs whose host policy allows a request, creates a lease, and assigns the task.
3. Worker rechecks DNS resolution and destination safety, fetches with redirect limits, timeout and body-size caps, and reports status, headers, and body.
4. Result handler verifies the lease, stores a content-addressed or versioned object, updates URL state and content hash, and schedules retry or recrawl if appropriate.
5. Parser extracts same-policy links and metadata. Normalized unseen URLs are deduplicated and added with a priority based on source quality, depth, and freshness needs.
6. Scheduler continues fairly across hosts. Monitoring surfaces queue age, host-level throughput, error rates, and policy violations.

## Scaling, consistency, and reliability

URL discovery is an at-least-once stream. A unique canonical URL key prevents duplicate frontier entries; transactional insertion or conditional upsert handles races. Fetch itself is not exactly once: a worker can finish after its lease expires. Completion should be idempotent and compare lease/version before overwriting newer results. Content hashes avoid reprocessing unchanged pages, though parser-version changes may require re-extraction.

Partitioning by host makes politeness enforcement straightforward but can create skew for large hosts; partition by host identity while scheduling multiple fair queues within the host. A global priority queue is a bottleneck, so use priority bands and shard-local queues with periodic fairness. Retries use exponential backoff and jitter; permanent 4xx responses and malformed content should not retry indefinitely. Robots policy changes invalidate or pause queued work promptly.

A crawler is also an SSRF risk. Resolve hostnames, reject private, loopback, link-local, and otherwise prohibited destination ranges, and validate every redirect hop to prevent DNS rebinding or redirect escapes. Isolate fetchers from internal networks, limit decompression expansion and parser CPU, and never execute page scripts by default.

## Deep dives and failure cases

**Politeness at scale:** Workers cannot independently enforce one host delay based only on local clocks. A host owner/coordinator grants bounded leases or schedules next-allowed timestamps atomically. If that coordinator fails, pause that host until state recovers rather than letting every worker hammer it. Cross-host parallelism remains available.

**URL explosion:** Calendar traps, session IDs, and generated query combinations can create infinite crawl spaces. Normalize cautiously, cap depth and per-host URL counts, detect repeated templates, assign low priority to near-duplicates, and provide operator kill switches. Limits must be observable and explainable, not silent data loss.

## Quick recall

Normalize and deduplicate a durable frontier; schedule by host; enforce robots and politeness; fetch in isolated bounded workers; parse asynchronously; make retries and completion idempotent. Guard every redirect and response, and control crawl traps.
