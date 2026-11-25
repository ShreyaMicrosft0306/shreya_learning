# Distributed job scheduler

## Scope and assumptions

Design a service that accepts one-time or recurring jobs, dispatches them near a requested time, retries failures, and exposes job status. “Near” means bounded scheduling delay under healthy capacity, not hard real-time. A worker may crash after executing a side effect but before acknowledging completion, so exactly-once execution cannot be promised generally. Workflows/DAGs, arbitrary user code sandboxing, and strict millisecond deadlines are excluded.

## Requirements and exclusions

- Create, cancel, and inspect jobs with execution time, payload, retry policy, and optional recurrence.
- Dispatch eligible work to compatible workers; lease work so crashes are recoverable.
- Avoid duplicate scheduling of recurring occurrences and support idempotent producer retries.
- Provide backpressure, fairness, and visibility into queue delay and failures.

## Data model and API

Use `Job(id, tenant_id, type, payload_ref, run_at, state, attempt, lease_until, version, idempotency_key)`, `Schedule(id, recurrence, timezone, next_run_at, version)`, and `JobAttempt(job_id, attempt, started_at, outcome, error_code)`. Store large payloads in object storage with a validated reference. APIs: `POST /jobs`, `DELETE /jobs/{id}`, `GET /jobs/{id}`, and `POST /jobs/{id}/retry` for authorized manual retry.

## Architecture

The scheduler API validates and persists job definitions in a transactional database. A due-time index or time-bucketed queue helps dispatchers efficiently find jobs whose `run_at` has arrived. Dispatchers atomically claim due jobs and publish them to partitioned broker queues; workers poll/receive, execute, and report completion. A lease/reaper process returns abandoned work to ready state. Recurrence expansion creates durable occurrence jobs with a unique `(schedule_id, occurrence_time)` key. A control plane manages worker capabilities, concurrency limits, and tenant quotas.

## End-to-end request/data flow

1. **Submit:** Client sends job data and an idempotency key. API validates tenant permissions and payload, then inserts job and outbox record in one transaction. Repeating the request returns the same job.
2. **Find due work:** Dispatcher scans indexed time buckets and claims a bounded batch using a transactional lock or compare-and-set state/version. It publishes job IDs to a broker with a dispatch token. Acknowledgement updates queued state; a crash between publish and update can duplicate delivery, which workers must tolerate.
3. **Execute:** Worker receives a job, verifies the lease/token and job version, records attempt start, then calls the handler. It renews a lease for long work. Completion atomically records success and releases capacity. Failure records error and schedules retry with capped exponential backoff and jitter, or moves the job to a dead-letter state.
4. **Recover and recur:** Reaper detects expired leases and makes jobs eligible again. Recurrence planner advances `next_run_at` and inserts the next occurrence transactionally or through an idempotent materialization key. Status APIs read canonical state and expose attempt history.

## Scaling, consistency, and reliability

Partition by tenant or job ID and spread due-time buckets to avoid one “top of minute” hotspot. Partition broker topics by job type/capability to avoid sending incompatible jobs to every worker. Apply tenant quotas and weighted fair dispatch so one customer cannot consume all worker slots. Scheduling and job-state transitions need transactional or conditional updates; broker delivery is at-least-once. Side-effecting handlers should receive the stable job ID as an idempotency token or use an external deduplication record. Leases need generous bounds and heartbeats; very short leases create duplicate work during pauses, while long leases delay recovery. Monitor oldest due job, queue delay, retry rate, lease expiry, and per-tenant usage. If workers are unavailable, retain durable queued jobs and reject or defer new submissions when storage/backlog limits are reached.

## Deep dives and failure cases

**Exactly-once myth:** A worker can complete an external charge and crash before reporting success. On redelivery, the scheduler cannot know whether the charge happened. Use idempotent downstream operations, transactional inbox/outbox where both sides are controlled, or reconciliation for ambiguous outcomes. State “at least once” honestly; an idempotency key reduces duplicate effects but does not make arbitrary handlers exactly-once.

**Time and recurrence:** Store instants in UTC while retaining the schedule's timezone and recurrence rule. Daylight-saving transitions can produce missing or repeated local times; define whether to skip, shift, or execute both. Use a unique occurrence identity and versioned schedule updates so a delayed planner cannot recreate cancelled or superseded jobs. Recalculate due status using a trusted clock, not a worker's local clock.

## Quick recall

Durable job state plus due-time index; dispatch through broker; lease and reaper recover crashes; at-least-once delivery with idempotent effects; fair quotas, jittered retries, and explicit recurrence/DST policy.
