# Reliability and Correctness

Reliability is not only uptime. It also means preserving the system's invariants when requests race, dependencies fail, or messages arrive more than once.

## Consistency and invariants

Write down invariants before choosing storage. Examples:

- an item cannot be sold twice;
- a balance change is recorded once;
- a user cannot access another tenant's object;
- a document operation has a stable ordering or conflict-resolution rule.

Use strong coordination only where it is needed. A feed counter can be approximate; an order's paid state cannot be guessed from a cache.

CAP is a useful reminder that during a network partition, a distributed data system must choose whether to reject some operations or return potentially stale/conflicting data. It is not a simple label that classifies every database as one of two types.

## Concurrency controls

- **Atomic conditional write:** update only if a predicate still holds.
- **Optimistic concurrency:** include a version; reject and retry on conflict.
- **Pessimistic lock:** serialize access to a contested record or range.
- **Uniqueness/constraint:** let the database enforce a key or range invariant.
- **Queue/actor ownership:** route operations for one key through a single ordered owner.

Choose by contention, required latency, scope of invariant, and database capabilities. Locks need timeout and deadlock behavior; retries need jitter and a bounded limit.

## Queues and delivery semantics

Queues smooth bursts and decouple work, but add latency and state. Define message identity, ordering scope, visibility/acknowledgment, retry policy, maximum attempts, dead-letter handling, and replay.

At-least-once delivery is common. Make handlers idempotent by recording processed IDs or using naturally idempotent state transitions. Exactly-once effects across independent systems generally require transactions or deduplication at the effect boundary.

## Timeouts, retries, and overload

- Set timeouts at every network boundary.
- Retry transient errors only when safe; use exponential backoff with jitter and a cap.
- Avoid retry multiplication across multiple layers.
- Use circuit breakers or load shedding to stop an unhealthy dependency from consuming all capacity.
- Apply backpressure when producers can outpace consumers.
- Reserve capacity for health checks and recovery work.

## Multi-step workflows

When one business action spans services, use a durable state machine or saga. Persist each step, make commands idempotent, record external references, and define compensating actions. A compensation is a new business action, not time travel; some steps may be irreversible and require manual review.

Reconciliation jobs compare internal records with external providers and repair or flag mismatches. Track stuck workflows and expose an operational recovery path.

## Failure checklist

For every critical dependency, ask what happens on timeout, partial response, duplicate request, stale response, overload, and restart. Include graceful degradation for non-critical features and a clear fail-closed policy for security-sensitive decisions.

