# Long-running tasks

Long-running tasks are operations that should continue beyond the lifetime of a single web request. Examples include video processing, report generation, bulk imports, and sending a large campaign. A request should usually acknowledge that work was accepted, while a separate worker performs it and the caller checks or receives its outcome.

## When to use

Use an asynchronous job design when work takes longer than a safe request timeout, has variable duration, needs retries, or can be processed independently of the caller. Keep the operation synchronous if it is reliably quick and the caller needs an immediate result. Moving work to a queue adds operational complexity, so do not do so solely because a queue is available.

## Architecture and flow

The client submits a request. The API validates it, authorizes the caller, records a job with a unique ID and initial status, then places a message on a queue. Return an accepted response containing the job ID and a status URL. A worker consumes the message, loads the job and required inputs, performs the work, and records progress and a terminal result such as succeeded or failed. The client can poll the status endpoint, receive a callback, or subscribe to status events.

Persist job state in a database; treat the queue as delivery infrastructure, not as the only record of truth. A transactional outbox is one way to avoid saving a job but failing to publish its message: write the job and an outbox record in one database transaction, then a publisher forwards the outbox record. Workers should claim or lease jobs so abandoned work can become eligible again after a timeout.

## Retries and correctness

Queues commonly provide at-least-once delivery: after a crash, the same message may be delivered more than once. Give each job a stable ID and make worker effects idempotent—repeating the operation produces the same outcome, or is detected and ignored. Use bounded retries with exponential backoff and jitter for transient failures. Permanent validation or business-rule failures should not be retried. After the retry budget, move the job to a dead-letter or failed state that operators can inspect and safely replay.

Define states and transitions explicitly, for example queued → running → succeeded/failed/cancelled. Record attempt count, timestamps, error category, and progress where useful. Avoid exposing internal stack traces or sensitive payloads through status APIs. Cancellation is a request to stop, not proof the work stopped; workers need cooperative cancellation checks and a clear policy for already-committed side effects.

## Tradeoffs, failures, and scaling

Asynchrony improves request latency isolation and smooths bursts, but results are delayed and the system must explain pending states to users. A queue can be unavailable, messages can be duplicated, workers can crash mid-effect, and status can disagree temporarily with queue contents. Reconciliation can find jobs stuck in running or queued too long and re-enqueue or flag them. Keep job data durable, version message schemas, and bound payload size; store large inputs elsewhere and pass references.

Scale consumers according to queue depth and age, not CPU alone. Partition work by a key when ordering is required, recognizing that one hot key limits parallelism. Apply concurrency caps to protect databases and downstream APIs. Backpressure means slowing acceptance or processing when capacity is exhausted; use quotas, admission control, and visible retry-after guidance rather than allowing an endless backlog.

## Example prompts

- Design asynchronous video transcoding with progress and cancellation.
- Design a bulk CSV import that can be retried safely.
- Design scheduled report generation for many tenants.

## Interview quick checklist

- What response acknowledges acceptance, and how is status retrieved?
- What is durable: job record, message, inputs, and result?
- How are duplicate delivery, retries, and permanent errors handled?
- How do workers claim leases and recover abandoned jobs?
- How are ordering, cancellation, progress, and replay defined?
- What limits protect the queue and downstream dependencies?
