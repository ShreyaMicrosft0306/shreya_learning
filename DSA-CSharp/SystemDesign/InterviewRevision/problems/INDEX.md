# Problem Walkthrough Index

Use the **Core Pattern-Coverage Path** to encounter every reusable pattern with the fewest walkthroughs. The rest of the catalog gives you additional variants and practice. These are independently designed examples; requirements and implementation choices can change the best design.

## Core Pattern-Coverage Path

Complete these seven walkthroughs in order. The table shows where each of the eight pattern notes gets a concrete example.

| Step | Walkthrough | Patterns covered | Why it belongs in the core path |
|---:|---|---|---|
| 1 | [Facebook News Feed](facebook-news-feed.md) | [Scaling reads](../patterns/scaling-reads.md) | Contrasts fanout-on-write and fanout-on-read; connects precomputation, caching, and feed freshness. |
| 2 | [YouTube](youtube.md) | [Large blobs](../patterns/large-blobs.md), [Long-running tasks](../patterns/long-running-tasks.md) | Separates media transfer/storage from metadata and asynchronous transcoding. |
| 3 | [WhatsApp](whatsapp.md) | [Realtime updates](../patterns/realtime-updates.md) | Exercises persistent connections, message durability, delivery state, and reconnect behavior. |
| 4 | [Flash Sale](flash-sale.md) | [Contention](../patterns/contention.md), [Multi-step processes](../patterns/multi-step-processes.md) | Protects scarce inventory while reservations and payment proceed safely. |
| 5 | [Ad Click Aggregator](ad-click-aggregator.md) | [Scaling writes](../patterns/scaling-writes.md) | Uses partitioned ingestion and aggregation to absorb high-volume events. |
| 6 | [Uber](uber.md) | [Proximity services](../patterns/proximity-services.md) | Finds nearby candidates, then handles matching and changing driver/rider state. |
| 7 | [Payment System](payment-system.md) | Reinforces [Multi-step processes](../patterns/multi-step-processes.md) and [Contention](../patterns/contention.md) | Deepens idempotency, ledger correctness, external-provider failure, and reconciliation. |

### Coverage check

- [x] Realtime updates
- [x] Long-running tasks
- [x] Contention and concurrency
- [x] Scaling reads
- [x] Scaling writes
- [x] Large blobs
- [x] Multi-step processes
- [x] Proximity services

The checkmarks describe the intended learning path, not a claim that one design exhausts a pattern. Read the corresponding pattern note and its additional examples for alternatives and limits.

## Additional practice by pattern

After the core path, use these prompts to see the same pattern under different constraints:

| Pattern | Additional walkthroughs |
|---|---|
| Realtime updates | [Live comments](live-comments.md), [Google Docs](google-docs.md), [ChatGPT](chatgpt.md), [Online Chess](online-chess.md) |
| Long-running tasks | [Job Scheduler](job-scheduler.md), [Web Crawler](web-crawler.md), [LeetCode](leetcode.md), [Price Tracking](price-tracking.md), [GitHub Actions](github-actions.md) |
| Contention and concurrency | [Ticketmaster](ticketmaster.md), [Online Auction](online-auction.md), [Robinhood](robinhood.md), [Distributed Cache](distributed-cache.md), [Donations Website](donations-website.md) |
| Scaling reads | [Bitly](bitly.md), [Instagram](instagram.md), [Yelp](yelp.md), [Google News](google-news.md), [Facebook Post Search](facebook-post-search.md), [Food Review App](food-review-app.md), [Game Leaderboard](game-leaderboard.md) |
| Scaling writes | [Distributed Rate Limiter](distributed-rate-limiter.md), [Top-K](top-k.md), [Metrics Monitoring](metrics-monitoring.md), [Strava](strava.md), [Game Leaderboard](game-leaderboard.md) |
| Large blobs | [Dropbox](dropbox.md), [Instagram](instagram.md), [Google Docs](google-docs.md), [GitHub Actions](github-actions.md) |
| Multi-step processes | [Local Delivery](local-delivery.md), [Ticketmaster](ticketmaster.md), [Robinhood](robinhood.md), [Job Scheduler](job-scheduler.md), [Donations Website](donations-website.md), [GitHub Actions](github-actions.md) |
| Proximity services | [Yelp](yelp.md), [Local Delivery](local-delivery.md), [Strava](strava.md), [Tinder](tinder.md), [Food Review App](food-review-app.md) |

## Full catalog by design area

### Storage, content, and processing

- [Bitly](bitly.md)
- [Dropbox](dropbox.md)
- [YouTube](youtube.md)
- [Web Crawler](web-crawler.md)
- [Price Tracking](price-tracking.md)
- [LeetCode](leetcode.md)
- [Job Scheduler](job-scheduler.md)

### Social, messaging, and discovery

- [Facebook News Feed](facebook-news-feed.md)
- [Facebook Post Search](facebook-post-search.md)
- [Instagram](instagram.md)
- [WhatsApp](whatsapp.md)
- [Live Comments](live-comments.md)
- [Tinder](tinder.md)
- [Yelp](yelp.md)
- [Strava](strava.md)
- [Google News](google-news.md)
- [Notification System](notification-system.md)

### Commerce, money, and marketplaces

- [Local Delivery](local-delivery.md)
- [Ticketmaster](ticketmaster.md)
- [Uber](uber.md)
- [Online Auction](online-auction.md)
- [Flash Sale](flash-sale.md)
- [Payment System](payment-system.md)
- [Robinhood](robinhood.md)

### Infrastructure and data platforms

- [Distributed Rate Limiter](distributed-rate-limiter.md)
- [Top-K](top-k.md)
- [Distributed Cache](distributed-cache.md)
- [Ad Click Aggregator](ad-click-aggregator.md)
- [Metrics Monitoring](metrics-monitoring.md)

### Collaboration and interactive systems

- [Google Docs](google-docs.md)
- [Online Chess](online-chess.md)
- [ChatGPT](chatgpt.md)

## Additional More Practice prompts

These four entries appear in the course's separate guided-practice section, not among the 32 official written breakdowns. Their notes below are original walkthroughs based on the prompt titles.

| Practice prompt | Difficulty shown | Useful pattern connections |
|---|---|---|
| [Food Review App](food-review-app.md) | Medium | Proximity search, read scaling, user-generated content |
| [Game Leaderboard](game-leaderboard.md) | Medium | Top-K ranking, high-volume writes, read scaling |
| [Donations Website](donations-website.md) | Hard | Payment workflow, idempotency, ledger correctness |
| [GitHub Actions](github-actions.md) | Hard | Long-running tasks, workflow orchestration, artifact storage |

## Scope note

The source lists 32 official problem-breakdown pages plus these four separate community “More Practice” prompts. All 36 now have independent walkthrough files; the four additional notes are not presented as official written breakdowns.
