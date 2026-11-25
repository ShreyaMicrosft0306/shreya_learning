# Multi-step processes

Multi-step processes span several operations or services and may take seconds, hours, or days. Examples include booking travel, onboarding a merchant, or placing an order that reserves inventory, charges payment, and arranges shipment. A database transaction can make changes atomic within one database, but usually cannot atomically cover independent services.

## When to use

Use a workflow when steps have dependencies, take a long time, need retries, or require human action. A saga is a sequence of local transactions, each committed by its owning service, with a compensating action when a later step cannot complete. A workflow engine or explicit state machine is valuable when flows have many branches, timers, or operational visibility needs. For a short process contained in one database, a normal transaction is simpler and safer.

## Architecture and flow

Model the process as named states and allowed transitions, such as order-created → inventory-reserved → payment-authorized → confirmed. Each transition should record durable progress and emit a command or event for the next participant. A coordinator-driven saga has a central orchestrator that issues commands and records outcomes. In a choreography, each service reacts to events and emits the next event; this reduces central control but makes the overall path harder to see.

A step should be idempotent: repeating its command should not repeat an external effect. Give each process and step a stable identifier, persist the result, and deduplicate callbacks. Use an outbox when a service must commit a local change and publish an event reliably. Include timeouts and explicit handling for delayed or missing responses. Store enough context to resume after a process restart, but avoid copying unnecessary sensitive data into workflow history.

## Compensation and tradeoffs

A compensating action is a new business operation that counteracts an earlier one; it is not always a literal rollback. If a hotel reservation was made and a later flight booking fails, compensation may cancel the hotel, but a fee could remain. Some steps cannot be undone—an email cannot be unsent, for example—so delay irreversible actions until likely failure points have passed, or define a manual recovery path.

Orchestration makes state and retries visible in one place but creates a coordinator dependency and requires careful versioning. Choreography avoids a central controller but can create hidden cycles, duplicated subscriptions, and difficult debugging. Sagas improve availability compared with a distributed transaction, but users may observe intermediate states and eventual consistency. Explain what the user sees while the process is pending or compensating.

## Failure modes and scaling

Messages can be duplicated, delayed, or reordered. Validate the expected current state before applying each transition and make repeated commands harmless. A crash may happen after a side effect but before progress is saved; reconciliation should query the affected service by process ID or idempotency key before retrying. Timeouts indicate uncertainty, not definite failure. Compensation itself can fail and therefore needs retries, monitoring, and an escalation queue.

Scale workers horizontally while preserving per-process ordering, for example by partitioning work on process ID. Bound concurrency to protect participating services. Keep workflow histories compact, archive completed processes, and expose stuck-step age, retry count, compensation state, and end-to-end completion time. Support safe workflow-version changes for processes already in flight.

## Example prompts

- Design checkout across inventory, payment, and shipping services.
- Design a travel booking flow with partial failures.
- Design user onboarding that includes external verification.

## Interview quick checklist

- Can this be one local transaction, or does it span services?
- What are the states, transitions, timeouts, and user-visible statuses?
- Is orchestration or choreography clearer for this flow?
- What is the compensation for every completed reversible step?
- How are duplicate, delayed, and uncertain outcomes resolved?
- How are stuck workflows, failed compensation, and version changes operated?
