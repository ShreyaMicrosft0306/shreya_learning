# Hosted CI Workflow Runner

## Scope and assumptions

Repository events start user-defined workflows made of dependent jobs and steps. Jobs run on temporary workers, produce logs and artifacts, and report status. This is a generic continuous-integration platform design; it does not describe any particular vendor's internal implementation.

## Requirements and exclusions

- Accept workflow triggers and validate a directed acyclic job graph.
- Schedule jobs after dependencies succeed and allocate compatible runners.
- Isolate untrusted repository code and secrets.
- Stream or retrieve logs and store artifacts durably.
- Support cancellation, timeouts, retries, quotas, and audit history.

## Data model and API

```text
WorkflowRun(run_id, repo_id, commit_id, state, created_at)
Job(job_id, run_id, dependencies, runner_class, state, attempt)
Step(step_id, job_id, command_ref, state, exit_code)
Artifact(artifact_id, run_id, object_key, size, checksum, expires_at)
```

```text
POST /workflow-runs
GET  /workflow-runs/{run_id}
POST /workflow-runs/{run_id}/cancel
GET  /jobs/{job_id}/logs?cursor=...
```

## Architecture

A webhook/API ingress authenticates repository events and writes run metadata. A workflow controller expands the validated DAG into jobs. A durable queue carries runnable jobs; schedulers match them to runner pools by platform, resource class, and isolation requirements. Ephemeral workers pull short-lived job leases, execute steps in sandboxes, and publish status/log events. Object storage holds artifacts and large logs; metadata remains in a database. A secrets broker issues narrowly scoped credentials just before execution.

## End-to-end flow

1. An event starts a run; the controller validates the workflow and stores the DAG.
2. Jobs with no unmet dependencies enter the runnable queue.
3. A scheduler selects a compatible worker and grants a lease with a fencing token.
4. The worker fetches an isolated checkout, obtains scoped secrets, and executes each step under resource/time limits.
5. Logs and artifacts are streamed or uploaded; job state is committed with an attempt ID.
6. On completion, dependent jobs become runnable; the run closes when all jobs reach terminal states.

## Scaling, consistency, and reliability

Scale the control plane separately from runner capacity. Queue depth and oldest-job age reveal saturation. Lease expiry recovers abandoned workers, while fencing prevents a stale worker from committing after reassignment. Keep job transitions idempotent and preserve attempt history. Apply per-repository quotas and fair scheduling so one tenant cannot consume all workers. Artifact lifecycle policies cap storage growth.

## Deep dives and failure cases

- **Untrusted code:** use ephemeral isolation, minimal credentials, network restrictions, and cleanup after every job.
- **Worker loss:** retry only when the step is safe or its side effects are idempotent; mark ambiguous external effects for review.
- **Large artifacts:** upload directly to object storage with checksums and resumable chunks rather than proxying bytes through the controller.

## Quick recall

Persist workflow state in a control plane, dispatch DAG-ready jobs through a durable queue, run them on isolated ephemeral workers, and store logs/artifacts separately. Scheduling fairness and sandbox security are as important as raw throughput.
