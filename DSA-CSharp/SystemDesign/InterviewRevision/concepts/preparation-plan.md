# Preparation and Deliberate Practice

System design improves through repeated practice, explicit feedback, and revisiting weak areas. Use this plan as a flexible routine rather than a fixed syllabus.

## 1. Build a foundation

Before attempting full designs, be comfortable explaining:

- request/response networking, load balancing, and service boundaries;
- relational and non-relational storage, indexes, transactions, and replication;
- caches, queues, event streams, object storage, and search indexes;
- consistency, concurrency, retries, and failure recovery;
- basic capacity estimates and common performance bottlenecks.

Use the [concept index](README.md) to identify a gap, then return to a problem walkthrough where that concept appears.

## 2. Practice in small drills

Do short exercises before full interviews:

- turn an open-ended prompt into a requirements list;
- identify entities, APIs, and one critical invariant;
- estimate request rate, storage, and bandwidth;
- draw a baseline architecture in a few minutes;
- explain one bottleneck and compare two solutions.

These drills isolate a skill so that a full practice session is not spent discovering several gaps at once.

## 3. Run timed end-to-end sessions

Choose a prompt from the [problem index](../problems/INDEX.md). Speak while drawing. Set a timer, keep assumptions visible, and reserve time for a deep dive and final summary.

After each session, write down:

1. What requirement did I misunderstand or leave vague?
2. Which assumption changed the architecture?
3. Could I trace a write and a read through the system?
4. Which invariant or failure mode did I miss?
5. Did I explain why a component was needed and what it cost?
6. What one concept should I revisit before the next attempt?

## 4. Review and repeat

Compare your solution with a trusted reference only after completing your own design. Record differences as decisions, not as a script to memorize: what requirement makes one approach preferable, and what would make you switch?

Use spaced repetition for the [pattern notes](../patterns/README.md): review the pattern trigger, baseline failure, mechanism, trade-off, and failure modes after increasing intervals.

## 5. Calibrate depth

- **Early-career interviews:** prioritize clear requirements, a coherent baseline, and correct basic data flow.
- **Mid-level interviews:** add scale estimates, concrete storage and API choices, and safe handling of common failures.
- **Senior interviews:** reason about consistency boundaries, bottlenecks, operational recovery, and alternatives.
- **Staff-level interviews:** make cross-service and multi-region trade-offs explicit, including migration, organizational ownership, cost, and long-term evolution.

These are practice priorities, not rigid title rules; follow the interviewer’s signals and the stated scope.

