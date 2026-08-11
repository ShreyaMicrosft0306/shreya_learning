# Text Processing Pipeline - Deep Dive Questions & Solutions

## Problem Overview
Build a module that processes text through a configurable pipeline of transformations:
- `create_pipeline(steps)` - Create a pipeline from a list of named transformation steps
- `process(pipeline, text)` - Run text through the pipeline and return the result

**Available transformations:**

| Step | Input | Output | Description |
|------|-------|--------|-------------|
| `lowercase` | string | string | Convert to lowercase |
| `trim` | string | string | Strip leading/trailing whitespace |
| `remove_punctuation` | string | string | Remove all punctuation characters |
| `tokenize` | string | list\<string\> | Split text into words |

**Core rules:**
1. Steps apply in the order given
2. `tokenize` must be the last step (it changes the output type: string → list)

**Baseline complexity:**

| Operation | Time | Space | Notes |
|-----------|------|-------|-------|
| `create_pipeline` | O(s) | O(s) | s = number of steps; validates names + ordering |
| `process` | O(s × t) | O(t) | t = text length; each step touches the full text |

## Examples (Input/Output)

### Example 1: String Output Pipeline

**Input:**
```text
steps = ["trim", "lowercase", "remove_punctuation"]
text  = "  Hello, World!  "
```

**Processing:**
```text
trim               -> "Hello, World!"
lowercase          -> "hello, world!"
remove_punctuation -> "hello world"
```

**Output:**
```text
"hello world"
```

### Example 2: Tokenized Output Pipeline

**Input:**
```text
steps = ["trim", "lowercase", "remove_punctuation", "tokenize"]
text  = "  The Quick, Brown Fox!  "
```

**Processing:**
```text
trim               -> "The Quick, Brown Fox!"
lowercase          -> "the quick, brown fox!"
remove_punctuation -> "the quick brown fox"
tokenize           -> ["the", "quick", "brown", "fox"]
```

**Output:**
```text
["the", "quick", "brown", "fox"]
```

### Example 3: Order Matters

**Input:**
```text
text = "state-of-the-art design"
```

**Pipeline A:**
```text
steps = ["remove_punctuation", "tokenize"]
```

**Output A:**
```text
["stateoftheart", "design"]
```

**Pipeline B:**
```text
steps = ["tokenize"]
```

**Output B:**
```text
["state-of-the-art", "design"]
```

This shows order changes meaning, not just formatting.

### Example 4: Error Cases

**Case A: Unknown step name**
```text
create_pipeline(["trim", "lowercse"])
-> UnknownStepException: unknown step 'lowercse'. Did you mean 'lowercase'?
```

**Case B: tokenize not last**
```text
create_pipeline(["tokenize", "lowercase"])
-> InvalidPipelineOrderException: tokenize must be the last step.
```

**Case C: tokenize repeated**
```text
create_pipeline(["trim", "tokenize", "tokenize"])
-> InvalidPipelineOrderException: tokenize must be the last step.
```

**Case D: null steps list**
```text
create_pipeline(null)
-> ArgumentNullException
```

**Case E: null text input**
```text
var p = create_pipeline(["trim"])
process(p, null)
-> ArgumentNullException
```

### Example 5: Edge Cases

**Case A:**
```text
steps = []
text  = "a b"
output = "a b"
```

**Case B:**
```text
steps = ["tokenize"]
text  = ""
output = []
```

**Case C:**
```text
steps = ["tokenize"]
text  = "   "
output = []
```

---

## Deep Dive #1: Modelling the Type Change (string → list)

### Question
**`tokenize` changes the output type from `string` to `List<string>`. How do you model a pipeline whose steps don't all share the same signature? What would break if `tokenize` were allowed in the middle?**

### The Problem
```
"  Hello, World!  "
   → trim              → "Hello, World!"        (string)
   → remove_punct      → "Hello World"          (string)
   → tokenize          → ["Hello", "World"]     (list)   ← type changed
   → lowercase         → ???                    (BOOM)
```
`lowercase` expects a string. After `tokenize` the value is a list. Either the code crashes,
or it silently does the wrong thing (e.g. `list.ToString()` → `"System.Collections..."`).

### Solution 1: Two-Phase Pipeline (String Stage + Optional Terminal Stage)

**How it works internally:**
1. Pipeline stores two things: an ordered list of `Func<string,string>` transforms, and an optional terminal `Func<string, List<string>>`
2. `create_pipeline` walks the step names; any string→string step is appended to phase 1
3. If `tokenize` is seen, it is stored as the terminal step; any further step → validation error
4. `process` runs all phase-1 transforms in order, then applies the terminal if present
5. Return type is modelled as a small result wrapper: `ProcessResult { string? Text; List<string>? Tokens; bool IsTokenized; }`

**Pros:**
- Illegal states are unrepresentable — you cannot construct a pipeline with a step after `tokenize`
- Each phase is strongly typed; no `object` casting anywhere
- Validation is structural, not a special-case `if` in `process`

**Cons:**
- Hard-codes the assumption "exactly one terminal transform kind"
- Adding a second type-changing step (e.g. `sentence_split`) needs the model to be generalised

### Solution 2: Uniform `object`-Based Steps with Declared Types

**How it works internally:**
1. Every step is described by metadata: `{ Name, InputType, OutputType, Func<object,object> Apply }`
2. `create_pipeline` type-checks the chain: `step[i].OutputType` must be assignable to `step[i+1].InputType`
3. Mismatch → throw `InvalidPipelineException` at **creation** time, not execution time
4. `process` walks steps applying each transform to an `object` accumulator
5. The "tokenize must be last" rule emerges naturally: no registered step accepts a list as input

**Pros:**
- Extensible — add `join`, `dedupe`, `stem` and the type-checker keeps working
- The ordering rule is data-driven, not a hard-coded special case

**Cons:**
- Boxing / casting overhead on every step
- Loses compile-time type safety; errors surface only via the runtime type-check
- Harder to read stack traces

### Solution 3: Generic Builder with Fluent Typing

**How it works internally:**
1. `Pipeline<TIn, TOut>` carries the type through the chain
2. `.Then(Func<TOut, TNext>)` returns `Pipeline<TIn, TNext>`
3. `.Tokenize()` returns `Pipeline<string, List<string>>`, which exposes no further string-only methods
4. The compiler enforces "tokenize is last" because `Pipeline<string, List<string>>` has no `.Lowercase()` overload
5. String-name input (`create_pipeline(["trim","tokenize"])`) is parsed into this chain by a factory

**Pros:**
- Compile-time enforcement — zero runtime validation cost for statically written pipelines
- Idiomatic and composable (mirrors LINQ / `IEnumerable` chaining)

**Cons:**
- Doesn't help when steps come from config/JSON at runtime — still need the dynamic factory
- Generic signatures get noisy fast

### Why `tokenize` Must Be Last — The Real Answer
- **Type safety**: subsequent steps are `string → string`; feeding them a list is a contract violation
- **Semantics change**: `trim` on a list is ambiguous — trim each token, or drop empty tokens?
- **Idempotence breaks**: `tokenize` is not composable with itself (`tokenize(tokenize(x))` is meaningless)
- **Information loss**: after tokenizing, the original whitespace/separator structure is gone, so any step that depends on it (`trim`, delimiter-aware logic) can never be recovered

> If you *do* want post-tokenize processing, the correct design is a **map step**: `map(lowercase)` applies a
> string transform to every element. That's a different step type with signature `list → list`, and it keeps
> the type-chain rule intact.

### Recommendation
Use **Solution 1** for the interview answer (simple, provably correct, no casting). Mention **Solution 2**
as the path when the step catalogue grows, and **Solution 3** when pipelines are written in code rather than config.

---

## Deep Dive #2: Cost of Chaining String Transformations

### Question
**What's the actual cost of running the pipeline? Is there a difference between building a new string at each step versus transforming in place?**

### The Problem
```
Text of length t = 1,000,000 chars, pipeline = [trim, lowercase, remove_punctuation, tokenize]

Naive: each step allocates a brand new string
  trim              → allocate 1MB
  lowercase         → allocate 1MB
  remove_punctuation→ allocate 1MB
  tokenize          → allocate ~150k string objects
Total: 3 full copies + GC pressure, before you even get to tokens
```
Time is O(s × t) either way — but the **constant factor and allocation count** differ enormously.

### Solution 1: Naive Immutable Chaining (Baseline)

**How it works internally:**
1. Each step receives a string, produces a new string, discards the old one
2. `s` intermediate strings of size ~t are allocated
3. Strings > 85,000 bytes land on the **Large Object Heap** — expensive, not compacted by default
4. Each intermediate becomes garbage immediately, causing frequent Gen0/Gen2 collections

**Cost:** Time O(s × t), Allocations O(s) buffers of O(t) each → **O(s × t) bytes allocated**

**Pros:** Trivial to write, each step independently testable, thread-safe by construction
**Cons:** s full copies of the data; GC pressure dominates runtime for large inputs

### Solution 2: Mutable Buffer (`StringBuilder` / `char[]`) — "Transform In Place"

**How it works internally:**
1. Convert the input to a `char[]` (or `StringBuilder`) **once**
2. Each step mutates the buffer instead of returning a new string
3. `lowercase`: in-place, write index == read index — pure O(t) walk, zero allocation
4. `remove_punctuation`: **two-pointer compaction** — read pointer scans, write pointer only advances on kept chars; buffer shrinks in place
5. `trim`: adjust start/end offsets only — O(1) after the scan, no data movement
6. Materialise the final `string` once at the end

**Cost:** Time O(s × t), Allocations **O(t) total** (one buffer + one final string)

**Pros:**
- One allocation instead of `s`; no LOH churn
- Removal steps shrink the working set for subsequent steps
- Typically 3–10× faster on large inputs purely from reduced GC

**Cons:**
- Steps are no longer pure functions — harder to reason about and test in isolation
- Mutable shared state means the buffer cannot be reused across threads
- A step that throws mid-way leaves the buffer in a corrupt state (need copy-on-entry or transactional semantics)

### Solution 3: Single-Pass Fusion (Loop Fusion / Deforestation)

**How it works internally:**
1. Recognise that `lowercase`, `remove_punctuation` and `trim` are all **character-local** predicates/maps
2. Compose them into one `char → char?` function: `c => IsPunct(c) ? null : ToLower(c)`
3. Walk the input **once**, applying the fused function per character
4. `trim` folds into the same pass via leading/trailing state flags
5. `tokenize` fuses too: emit a token whenever the fused output hits a separator — never materialise the cleaned string at all

**Cost:** Time **O(t)** regardless of `s` (for fusable steps), Allocations O(output size)

**Pros:**
- Removes the `s` factor entirely — one pass over the data
- Skips the intermediate cleaned string when tokenizing (biggest win)
- This is exactly what LINQ's lazy `IEnumerable` chaining and Rust iterator adapters do

**Cons:**
- Only works for **element-local, order-preserving** steps. A step like `reverse`, `sort_words` or `normalize_unicode_NFC` is a barrier and forces a materialisation point
- Optimiser complexity: you need step metadata (`IsCharLocal`) to know what's fusable
- Harder to debug — you can't inspect the intermediate value of each step

### Solution 4: `Span<char>` / Zero-Copy Slicing

**How it works internally:**
1. Represent the working text as `ReadOnlySpan<char>` over the original buffer
2. `trim` → `span.Trim()` returns a **slice**, zero allocation, zero copying
3. `tokenize` → yield `ReadOnlySpan<char>` / `Range` slices instead of new strings; caller materialises only what it needs
4. Only genuinely mutating steps (`lowercase`, `remove_punctuation`) need a writable `Span<char>` destination
5. `string.Create` writes the final result directly with no intermediate

**Pros:**
- Near-zero allocation for slicing-only pipelines
- Stack-allocated for small inputs (`stackalloc` under ~1KB)

**Cons:**
- `Span<T>` is a `ref struct`: cannot be stored in fields, used in `async`, or captured in lambdas — which conflicts with storing steps as delegates
- Lifetime bugs if the underlying buffer is reused/freed

### Performance Comparison

| Approach | Time | Allocations | Best For |
|----------|------|-------------|----------|
| **Immutable chaining** | O(s × t) | O(s × t) bytes | Small inputs, clarity, default choice |
| **Mutable buffer** | O(s × t) | O(t) bytes | Large documents, hot paths |
| **Single-pass fusion** | O(t) | O(output) | Long pipelines of char-local steps |
| **Span slicing** | O(s × t) | ~O(1) for slice-only steps | Trim/split-heavy, ultra-low-latency |

**Recommendation:**
Start with **Solution 1** — correctness and testability first; for typical text (< 100KB) the difference is
noise. Switch to **Solution 2** when profiling shows GC pressure, and reach for **Solution 3** only when the
pipeline is long, hot, and made of char-local steps.

---

## Deep Dive #3: Validation Strategy — Fail Fast vs Fail Late

### Question
**Where should errors surface: at `create_pipeline` or at `process`? What errors are possible, and what should each one return?**

### The Problem
```
create_pipeline(["trim", "lowercse"])      # typo — unknown step
create_pipeline(["tokenize", "lowercase"]) # tokenize not last
create_pipeline(["tokenize", "tokenize"])  # tokenize twice
process(pipeline, None)                    # null input
```
If validation happens in `process`, a bad config deployed at 9am fails at 3pm on the first request.

### Solution 1: Validate Entirely at Creation (Fail Fast) — Recommended

**How it works internally:**
1. `create_pipeline` looks up each name in a `Dictionary<string, StepDefinition>` registry
2. Unknown name → throw `UnknownStepException` listing the valid step names (and a "did you mean?" suggestion via edit distance)
3. Ordering rule → after a terminal step is bound, any further step throws `InvalidPipelineOrderException`
4. Collect **all** errors and report them together rather than failing on the first one
5. The returned pipeline object is **immutable and guaranteed valid** — `process` needs no validation at all

**Pros:**
- Errors surface at config-load / startup, not at request time
- `process` becomes a tight loop with no branches — faster and simpler
- The type of the pipeline object is a proof of validity

**Cons:**
- Cannot express pipelines whose validity depends on runtime data
- Aggregating errors is slightly more code than throw-on-first

### Solution 2: Result Objects Instead of Exceptions

**How it works internally:**
1. `create_pipeline` returns `Result<Pipeline, List<PipelineError>>` rather than throwing
2. Caller pattern-matches on success/failure — errors are part of the signature
3. Each error carries the step index, the offending name, and a machine-readable code
4. Useful when the pipeline definition comes from user input (a web form/API) where errors are expected, not exceptional

**Pros:** No exception cost on the expected-failure path; errors are enumerable for a UI
**Cons:** Callers can ignore the result; more ceremony in C# than in F#/Rust

### Solution 3: Lenient Mode (Skip Unknown Steps + Warn)

**How it works internally:**
1. `create_pipeline(steps, StrictMode.Lenient)` skips unrecognised names and records a warning list
2. Ordering violations are still hard errors — they cannot be safely ignored
3. Useful for forward compatibility: old service versions tolerate pipelines using newer step names

**Pros:** Rolling deployments don't break when a new step name ships
**Cons:** Silent behaviour change — the text isn't transformed as the caller intended. Only acceptable with loud telemetry

### Error Catalogue

| Condition | When Detected | Behaviour |
|-----------|---------------|-----------|
| Unknown step name | create | Throw / `Err` with the valid-name list |
| `tokenize` not last | create | Throw / `Err` with the offending index |
| `tokenize` appears twice | create | Same as "not last" — the second is after the first |
| `steps` is null | create | `ArgumentNullException` |
| `steps` is empty | create | **Valid** — identity pipeline |
| `text` is null | process | `ArgumentNullException` (a null string is not empty text) |
| `text` is empty | process | **Valid** — returns `""` or `[]` |

### Recommendation
Use **Solution 1**. Validation belongs at construction; `process` should be incapable of failing on a
well-formed input. Add **Solution 2** at the API boundary where invalid input is routine rather than exceptional.

---

## Deep Dive #4: Edge Cases and Semantics

### Question
**Walk through the edge cases: empty input, empty pipeline, tokenize-only pipeline, repeated steps. What does each return and why?**

### Edge Case Matrix

| Input | Pipeline | Result | Reasoning |
|-------|----------|--------|-----------|
| `""` | `[]` | `""` | Identity in, identity out |
| `""` | `[trim, lowercase]` | `""` | Every step is total on empty input |
| `""` | `[tokenize]` | `[]` | **Not** `[""]` — zero words, not one empty word |
| `"   "` | `[tokenize]` | `[]` | Whitespace-only yields no tokens |
| `"   "` | `[trim]` | `""` | Trim collapses to empty |
| `"a b"` | `[]` | `"a b"` | Empty pipeline must be the identity function, not an error |
| `"Hi!"` | `[lowercase, lowercase]` | `"hi"` | Repeated idempotent steps are harmless, just wasted work |
| `"a,b"` | `[remove_punctuation, tokenize]` | `["ab"]` | Punctuation removal **joins** the words — order matters! |
| `"a,b"` | `[tokenize]` | `["a,b"]` | One token, punctuation intact |
| `"!!!"` | `[remove_punctuation, tokenize]` | `[]` | Everything removed |

### The Subtle One: `remove_punctuation` Before `tokenize`
```
"state-of-the-art design"
  remove_punct → "stateoftheart design"  → tokenize → ["stateoftheart", "design"]
  tokenize     → ["state-of-the-art", "design"] → (map remove_punct) → ["stateoftheart", "design"]
```
Deleting punctuation vs **replacing it with a space** are different products, not different implementations.
Decide explicitly and document it — this is the question an interviewer is actually probing.

### Solution 1: Deduplicate / Normalise Steps at Creation

**How it works internally:**
1. Steps are tagged as idempotent (`lowercase`, `trim`, `remove_punctuation` all are)
2. `create_pipeline` collapses adjacent duplicates of an idempotent step: `[lower, lower] → [lower]`
3. Non-adjacent duplicates can only be collapsed if the intervening steps commute — `[lower, trim, lower]` is safe because `trim` never introduces uppercase
4. Requires a commutativity matrix in step metadata; skip it unless the win is measured

**Pros:** Free performance for machine-generated pipelines
**Cons:** The executed pipeline differs from the declared one — confusing to debug; commutativity is easy to get subtly wrong

### Solution 2: Keep Steps Verbatim, Warn on Redundancy

**How it works internally:**
1. Execute exactly what was declared — no rewriting
2. Emit a warning/log when an idempotent step repeats adjacently
3. Principle of least surprise: what you configure is what runs

**Recommendation:** **Solution 2.** The cost of a redundant `lowercase` is one O(t) pass; the cost of a wrong
optimisation is a correctness bug. Optimise only after profiling proves it matters.

---

## Deep Dive #5: Extensibility — Registry and Parameterised Steps

### Question
**How do you add a new transformation without modifying `process`? What about steps that need arguments, like `truncate(100)` or `tokenize(delimiter=",")`?**

### Solution 1: Step Registry (Open/Closed Principle)

**How it works internally:**
1. `IStepRegistry` maps `string name → StepDefinition { Name, InputKind, OutputKind, Factory }`
2. Built-in steps register at startup; plugins register via DI at composition root
3. `create_pipeline` resolves names purely through the registry — it has zero knowledge of any concrete step
4. Adding `stem` or `normalize_unicode` means registering a new definition; `process` never changes
5. Registry is immutable after startup, so lookups are lock-free and thread-safe

**Pros:** New steps require no changes to pipeline code; testable with a fake registry
**Cons:** Indirection makes "where is this step implemented?" harder to answer; name collisions need a policy

### Solution 2: Parameterised Steps via Structured Definitions

**How it works internally:**
1. A step is `{ "name": "truncate", "args": { "max_length": 100 } }` instead of a bare string
2. Backward compatible: a plain string parses to a step with empty args
3. Each `StepDefinition` declares its parameter schema; `create_pipeline` validates args at creation
4. The factory closes over the validated args and returns the concrete `Func`
5. Invalid args (negative length, unknown delimiter) fail at creation, consistent with Deep Dive #3

**Pros:** Massively more expressive; validation stays at creation time
**Cons:** Needs a schema/validation layer; the config format becomes non-trivial

### Solution 3: Pipeline Composition (Pipelines as Steps)

**How it works internally:**
1. A `Pipeline` itself satisfies the step interface (`string → string`) as long as it has no terminal step
2. Named sub-pipelines: `"clean" = [trim, lowercase, remove_punctuation]`
3. `create_pipeline(["clean", "tokenize"])` resolves `clean` recursively
4. Guard against cycles with a visited-set during resolution — a self-referential pipeline must be a creation-time error
5. Flatten at creation so `process` still sees a single linear list

**Pros:** Reuse of common step groups; config stays DRY
**Cons:** Cycle detection required; flattening makes stack traces less obvious

**Recommendation:** **Solution 1 + 2**. Registry plus structured step definitions covers essentially every
realistic extension. Add **Solution 3** only when config duplication becomes painful.

---

## Deep Dive #6: Scale — Large Documents, Streaming and Concurrency

### Question
**Now you're processing 10GB of documents per hour, some files larger than memory. How does the design change? Is the pipeline safe to share across threads?**

### The Problem
- A 2GB file cannot be loaded into a single `string` (.NET strings cap at ~1B chars, and LOH pressure kills you long before that)
- `process` is O(s × t) with `t` unbounded — one huge document blocks a worker
- Sharing a pipeline object across threads is only safe if it holds no mutable state

### Solution 1: Streaming / Chunked Processing

**How it works internally:**
1. Read the source as a stream of buffers (e.g. 64KB `char[]` from a `TextReader`)
2. Char-local steps (`lowercase`, `remove_punctuation`) apply per chunk with **no cross-chunk state**
3. `trim` is **not** chunk-local: leading trim only applies to the first chunk, trailing trim to the last — track position state
4. `tokenize` needs **boundary carry-over**: a word split across a chunk boundary must be buffered and joined with the next chunk's prefix
5. Emit tokens lazily via `IAsyncEnumerable<string>` so the consumer processes while I/O continues
6. Memory becomes O(chunk size + longest token) instead of O(t)

**Pros:** Constant memory regardless of document size; overlaps I/O with CPU
**Cons:** Steps must declare whether they're chunkable; boundary handling is the #1 source of bugs (always unit-test the split-mid-word case)

### Solution 2: Immutable Pipeline + Parallel Documents

**How it works internally:**
1. Make the pipeline object immutable: readonly step list, no per-run fields, no cached buffers
2. All mutable state (the working buffer, token accumulator) lives in a per-call context object
3. One pipeline instance is then safely shared by N threads: `documents.AsParallel().Select(d => process(pipeline, d))`
4. Parallelise across **documents**, not within one — avoids all boundary/ordering complexity
5. Use a bounded channel to cap in-flight work and prevent memory blow-up

**Pros:** Linear speed-up with cores, trivially correct, no synchronisation at all
**Cons:** One pathologically huge document still bottlenecks a single worker; needs backpressure

### Solution 3: Buffer Pooling

**How it works internally:**
1. Rent working buffers from `ArrayPool<char>.Shared` instead of allocating per call
2. Return them in a `finally` block; clear them if the text is sensitive
3. Eliminates the per-document allocation identified in Deep Dive #2 entirely
4. Combines with Solution 2 — the pool is thread-safe and per-core partitioned

**Pros:** Near-zero steady-state allocation at high throughput
**Cons:** Leaked (unreturned) buffers are hard to diagnose; must not hold `Span`s past the return

### Solution 4: Compiled Pipelines (Amortise the Dispatch Cost)

**How it works internally:**
1. Interpreting a list of delegates costs one virtual call per step per document
2. For hot pipelines, emit the fused loop once via expression trees / source generators, cache by pipeline hash
3. The JIT can then inline the per-character work and vectorise it (SIMD `ToLower` over 16 chars at a time)
4. Cache key = ordered step names + args; build once, reuse forever

**Pros:** Removes dispatch overhead; unlocks SIMD
**Cons:** Significant complexity and startup cost — only worth it at very high volume

**Recommendation:** **Solution 2 + 3** covers almost every real workload (immutable pipeline, parallel across
documents, pooled buffers). Add **Solution 1** only when individual documents exceed memory.

---

## Deep Dive #7: Correctness of the Transformations Themselves

### Question
**What exactly is "punctuation"? What is a "word"? These look trivial and are the most common source of production bugs.**

### The Problem
```
"Do not use ToLower() without a culture"
  "I".ToLower()  → "ı" in Turkish locale (dotless i) — the classic Turkish-I bug
  "naïve"        → is "ï" punctuation? No — but a naive ASCII check may mangle it
  "don't"        → 1 word or 2? Is the apostrophe punctuation?
  "东京 とうきょう" → whitespace splitting fails; CJK has no spaces
  "e\u0301"      → "é" as two code points; naive char ops split the grapheme
```

### Solution 1: Be Explicit About Culture

**How it works internally:**
1. Use `ToLowerInvariant()` for machine-readable text (identifiers, keys, search index terms)
2. Use `ToLower(CultureInfo)` **only** when the output is shown to a user in that culture
3. Never use bare `ToLower()` — it silently picks up the ambient thread culture and behaves differently per machine
4. Document the choice in the step definition so it's a decision, not an accident

### Solution 2: Unicode-Aware Classification

**How it works internally:**
1. `remove_punctuation` uses `char.IsPunctuation(c) || char.IsSymbol(c)` rather than an ASCII range check
2. Decide explicitly whether symbols (`$`, `+`, `©`) count — they are a separate Unicode category from punctuation
3. Normalise to NFC before processing so combining marks are composed and per-`char` operations are grapheme-safe
4. For true correctness on emoji/CJK, iterate `StringInfo`/text elements rather than `char` (UTF-16 code units)

### Solution 3: Pluggable Tokenizer

**How it works internally:**
1. Whitespace splitting is a *default*, not the definition of tokenization
2. Expose the strategy: whitespace, Unicode word-boundary (UAX #29), regex, or an ICU/dictionary-based segmenter for CJK
3. Use `StringSplitOptions.RemoveEmptyEntries` so `"a  b"` yields 2 tokens, not 3 — this is why `""` tokenizes to `[]`
4. Language-specific tokenizers register through the same registry as any other step (Deep Dive #5)

**Recommendation:** Default to invariant casing + Unicode-category punctuation + whitespace tokenization,
but make each one a documented, swappable decision rather than an implicit one.

---

## Summary

| Deep Dive | Key Technique | When It Matters |
|-----------|---------------|-----------------|
| **Type change (string → list)** | Two-phase pipeline; terminal step is structurally last | Always — it's the core design constraint |
| **Chaining cost** | Mutable buffer → loop fusion → span slicing | Large inputs or long pipelines |
| **Validation** | Fail fast at `create_pipeline`; `process` cannot fail | Always — config errors must not reach production |
| **Edge cases** | Empty pipeline = identity; `""` tokenizes to `[]` | Always — these are the test cases |
| **Extensibility** | Step registry + parameterised definitions | When the step catalogue grows past ~5 |
| **Scale** | Immutable pipeline + parallel docs + pooled buffers | High throughput / files larger than memory |
| **Unicode correctness** | Invariant casing, Unicode categories, pluggable tokenizer | Any non-ASCII input — i.e. always |

### Answers to the Three Follow-Up Questions

**1. What's the cost of running the pipeline?**
O(s × t) time, where s = steps and t = text length — every step touches every character. The interesting
cost is **space**: naive immutable chaining allocates O(s × t) bytes, and GC of those intermediates usually
dominates the actual CPU time for large inputs.

**2. New strings each time vs transforming in place?**
Same asymptotic time, very different constants. Immutable chaining allocates `s` full copies and hammers the
GC (LOH above ~85KB). A mutable buffer allocates once — typically 3–10× faster on large inputs. Loop fusion
goes further, collapsing the whole chain into a single O(t) pass and eliminating intermediates entirely for
character-local steps. The trade-off is purity and testability: immutable steps are trivially composable and
thread-safe; mutable ones are not, and a mid-pipeline exception leaves a corrupted buffer.

**3. Why must `tokenize` be last?**
It's the only step that changes the output **type** (string → list). Every other step's contract is
`string → string`, so anything after `tokenize` receives an argument it cannot handle — either a crash or,
worse, a silent wrong answer. Beyond types, it's semantically lossy and irreversible: separator/whitespace
structure is destroyed, so `trim` or delimiter-dependent logic can never be meaningfully applied afterwards,
and `tokenize` cannot compose with itself. The correct way to allow post-tokenize work is a distinct
`map(step)` operation with a `list → list` signature, which preserves the type-chain invariant instead of
breaking it.

### General Recommendations
1. Validate at creation, execute without branching — an invalid pipeline should be unconstructable
2. Keep steps pure and immutable until profiling proves you need a mutable buffer
3. Model the type change structurally rather than with a runtime `if (step == "tokenize" && !isLast)` check
4. Empty pipeline is the identity function, not an error; empty text tokenizes to `[]`, not `[""]`
5. Use a registry so new steps never require touching `process`
6. Be explicit about culture and Unicode — never call bare `ToLower()`
7. Share one immutable pipeline across threads; parallelise across documents, not within one
8. Stream only when documents exceed memory, and always test the word-split-across-chunk-boundary case
