# LeetCode-style online judge

## Scope and assumptions

Design a coding platform where users submit code, receive compile/runtime/test results, browse problems, and track submissions. The judge runs untrusted programs with multiple languages, bursty contest traffic, and public and hidden tests.

## Requirements and exclusions

**Key requirements:** problem and test management; submission API; asynchronous compilation and execution; bounded CPU, memory, time, and output; reliable result delivery; submission history and contest scoring.

**Out of scope:** collaborative editing, plagiarism detection, IDE features, and identical performance across hardware. Hidden tests and judge configuration must not leak.

## Data model and API

`Problem(problem_id, version, statement_ref, constraints, visibility)` references content and a pinned judge version. `TestSuite(problem_id, version, public_ref, hidden_ref, runner_config)` stores test metadata and protected assets. `Submission(submission_id, user_id, problem_id, language, source_ref, problem_version, status, created_at)` is immutable input. `ExecutionResult(submission_id, test_group, outcome, cpu_ms, memory_bytes, diagnostic_ref)` stores bounded outcomes. `ContestEntry` and `ScoreEvent` capture contest state.

`GET /problems/{id}` returns authorized statement and sample tests. `POST /submissions` accepts problem/version, language, and source, returning an ID. `GET /submissions/{id}` returns current status/result. Contest submissions include an idempotency key and contest context. Never return hidden test inputs or expected answers.

## Architecture

Web/API services handle authentication, catalog, quotas, and persistence. Submission service stores source privately and publishes a queue job. A scheduler assigns language-specific workers, which launch short-lived isolated sandboxes with CPU, memory, process, and time limits. Result service stores outcomes and notifies clients; contest consumers update leaderboards. Publishing tools version problems, compilers, and tests.

## End-to-end flows

1. **Problem read:** User requests a problem. API checks visibility/contest access and returns the versioned statement and public samples; hidden tests remain on isolated judge storage.
2. **Submit:** Client sends source and language. API validates size, language, problem version, and rate limits; it writes source privately, inserts a queued submission, and publishes an outbox event. The client receives a submission ID without waiting for execution.
3. **Schedule:** Queue assigns work according to language, priority, contest policy, and pool capacity. Scheduler leases the job with a deadline so a crashed worker can be replaced.
4. **Compile:** Worker loads a pinned compiler image and compiles under resource limits. Compiler diagnostics are sanitized and bounded before storing.
5. **Run:** For each test group, worker starts an isolated process with input, captures output, enforces wall/CPU time and memory, and compares output according to the problem's comparator. Kill the process tree on timeout or output abuse; do not let the sandbox access network, host credentials, or other submissions.
6. **Finalize:** Worker posts an idempotent result keyed by submission and attempt. Result service transitions submission to terminal status, persists resource usage, and notifies the client. Contest score consumer updates standings from accepted results.

## Scaling, consistency, and reliability

Execution is expensive and bursty. Queue jobs, scale by language/resource class, and apply quotas and fair scheduling. Status may lag, but terminal results must be durable for a given problem/test version.

Pin compiler images and problem versions. Restrict source/test access and give workers only one job's assets. Attempt IDs enable safe retries; lease expiry recovers abandoned tasks. Validate result suite and completion state server-side.

## Deep dives and failure cases

**Safe execution:** Containers alone may not be a sufficient boundary. Use microVMs or hardened sandboxes, disable network/privilege, apply syscall and resource policies, and use ephemeral filesystems. Never interpolate source into shell commands. Bound output, files, processes, and time; discard partial state after crashes.

**Queue and result correctness:** Idempotency keys make API retries return the same submission. At-least-once queue delivery requires idempotent result persistence. Distinguish infrastructure failures from code failures; defer if hidden tests are unavailable. Leaderboards can lag but derive from durable score events and reconciliation.

## Quick recall

Treat every submission as hostile. Persist first, queue asynchronously, execute in a strongly isolated and resource-bounded sandbox, and pin the full judge version. Idempotent workers, hidden-test isolation, and recoverable queue leases protect both correctness and platform safety.
