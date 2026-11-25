# Pattern Index

Patterns are starting points, not templates. First identify the requirement or invariant, then select and adapt the pattern. Several patterns often appear together in one system.

See the [problem index](../problems/INDEX.md) for a focused sequence that covers all eight patterns and additional walkthroughs for each.

## Quick selector

| Problem signal | Pattern note | Main decision |
|---|---|---|
| Clients need timely server-originated updates | [Realtime updates](realtime-updates.md) | Polling, SSE, WebSockets, or a broker-backed stream |
| Work is too slow for a synchronous request | [Long-running tasks](long-running-tasks.md) | Queue, worker pool, status model, retries, and cancellation |
| Concurrent actions compete for the same state | [Contention and concurrency](contention.md) | Atomic write, lock, version check, or serialization |
| Read volume or read latency is the bottleneck | [Scaling reads](scaling-reads.md) | Index, replica, cache, denormalization, or precomputation |
| Write throughput exceeds a single owner/store | [Scaling writes](scaling-writes.md) | Partitioning, batching, queues, and backpressure |
| Objects are large or expensive to transfer | [Large blobs](large-blobs.md) | Direct object transfer, metadata service, CDN, and lifecycle |
| A business action spans multiple services | [Multi-step processes](multi-step-processes.md) | Durable workflow, idempotent steps, and compensation |
| Results are based on distance or a region | [Proximity services](proximity-services.md) | Geospatial index, candidate selection, and exact ranking |

## Pattern combinations to recognize

- **Feed with media:** fanout or pull strategy + cache + object storage/CDN.
- **Ticket or flash sale:** contention control + admission queue + reservation expiry.
- **Chat:** realtime transport + durable message log + delivery receipts.
- **Video processing:** large-blob upload + long-running task queue + CDN delivery.
- **Payment workflow:** idempotency + multi-step state machine + reconciliation.
- **Nearby search:** geospatial candidate index + read scaling + freshness policy.

## Example walkthroughs

These are places to look for the pattern in context; each problem note is an independently designed example.

| Pattern | Example prompts |
|---|---|
| Realtime updates | [WhatsApp](../problems/whatsapp.md), [live comments](../problems/live-comments.md), [ChatGPT](../problems/chatgpt.md) |
| Long-running tasks | [YouTube](../problems/youtube.md), [web crawler](../problems/web-crawler.md), [job scheduler](../problems/job-scheduler.md), [GitHub Actions](../problems/github-actions.md) |
| Contention and concurrency | [Ticketmaster](../problems/ticketmaster.md), [online auction](../problems/online-auction.md), [flash sale](../problems/flash-sale.md), [payment system](../problems/payment-system.md), [Donations Website](../problems/donations-website.md) |
| Scaling reads | [Facebook news feed](../problems/facebook-news-feed.md), [Instagram](../problems/instagram.md), [Yelp](../problems/yelp.md), [Google News](../problems/google-news.md), [Food Review App](../problems/food-review-app.md) |
| Scaling writes | [ad click aggregation](../problems/ad-click-aggregator.md), [metrics monitoring](../problems/metrics-monitoring.md), [top-K](../problems/top-k.md), [distributed rate limiter](../problems/distributed-rate-limiter.md), [Game Leaderboard](../problems/game-leaderboard.md) |
| Large blobs | [Dropbox](../problems/dropbox.md), [YouTube](../problems/youtube.md), [Instagram](../problems/instagram.md), [Google Docs](../problems/google-docs.md), [GitHub Actions](../problems/github-actions.md) |
| Multi-step processes | [local delivery](../problems/local-delivery.md), [payment system](../problems/payment-system.md), [Robinhood](../problems/robinhood.md), [job scheduler](../problems/job-scheduler.md), [Donations Website](../problems/donations-website.md), [GitHub Actions](../problems/github-actions.md) |
| Proximity services | [Uber](../problems/uber.md), [Yelp](../problems/yelp.md), [local delivery](../problems/local-delivery.md), [Strava](../problems/strava.md), [Food Review App](../problems/food-review-app.md) |

## Quick review

For any chosen pattern, be able to explain: the trigger, the baseline that fails, the mechanism, what becomes the source of truth, its failure modes, and what trade-off the pattern introduces.
