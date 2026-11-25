# Contention and concurrent updates

Contention occurs when multiple requests try to change the same data at once. The important first step is not choosing a lock; it is stating the invariant—the condition that must remain true. Examples include “inventory never falls below zero,” “a username is unique,” or “a payment is applied at most once.” The invariant determines which operations must be coordinated and where that coordination belongs.

## When to use

Consider a concurrency-control mechanism whenever simultaneous updates can violate a business rule or lose changes. For low-conflict records, optimistic concurrency is usually efficient. For short critical sections with high conflict, a database lock can serialize changes. For operations that span multiple services, local database transactions are insufficient; use idempotent workflows, reservations, or a saga rather than pretending there is one global transaction.

## Architecture and flow

Prefer enforcing simple invariants in the database. A unique constraint prevents duplicate identifiers even when application checks race. A check constraint can enforce a valid range. For inventory, an atomic conditional update such as “decrement only where quantity is at least the requested amount” combines the check and change in one statement; inspect affected-row count to learn whether it succeeded.

Pessimistic locking acquires a lock before modifying a record, so competing transactions wait or fail. Keep the transaction short, lock rows in a consistent order, and define a timeout. Optimistic concurrency allows reads without a lock, but updates include a version number or prior value in the condition. If another writer changed the record, the update affects zero rows; the caller can reload, merge, or return a conflict.

Idempotency addresses repeated commands, not simultaneous conflicting commands. A client supplies or receives a unique operation key; the service records that key and its outcome atomically with the effect. A repeated request returns the recorded outcome instead of applying the effect again. Use a scoped key and retain it long enough to cover realistic retries.

## Tradeoffs

Locks provide straightforward serialization but reduce throughput, can cause deadlocks, and can leave callers waiting. Optimistic concurrency avoids waiting when conflicts are rare, but repeated conflicts waste work and require a retry or user-visible conflict policy. Database constraints are robust, but their error cases must be translated into meaningful application responses. Idempotency records improve retry safety but consume storage and need expiration and payload-matching rules.

For scarce resources, a reservation with an expiry can let a user hold capacity temporarily. Reservation creation must itself be atomic; expiration and finalization must be safe to repeat. For money movement or multi-service actions, record durable intent and progress; do not rely on a client retry being exactly once.

## Failure modes and scaling

Deadlocks can occur when transactions acquire locks in different orders; databases often abort one participant, which should be retried only when safe. Long transactions hold locks and amplify queueing. Hot rows become throughput bottlenecks even with correct logic. Reduce unnecessary writes, split independent state, or serialize by a key through a partitioned command queue when per-key ordering is acceptable. A queue does not remove contention; it makes the ordering and backlog explicit.

A timeout does not prove a transaction failed: its commit may have succeeded while the response was lost. Resolve uncertainty by looking up the operation key or durable result, not by blindly repeating a non-idempotent effect. Monitor conflict rate, lock wait time, deadlocks, retry volume, and hot-key concentration.

## Example prompts

- Design seat booking without double-selling a seat.
- Design wallet transfers that preserve balances.
- Design inventory reservations during a flash sale.

## Interview quick checklist

- What exact invariant must always hold?
- Which database constraint or atomic operation enforces it?
- Are conflicts handled by locking, version checks, or serialization?
- What happens on duplicate requests and lost responses?
- Are transactions short and locks acquired in consistent order?
- How are retries bounded and hot records monitored?
