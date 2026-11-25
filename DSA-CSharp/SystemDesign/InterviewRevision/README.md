# System Design Interview Revision Pack

An independently written study pack for quickly revising system-design concepts, recognizing reusable patterns, and walking through common interview prompts. The notes use general engineering knowledge and original examples; they are not a transcription or replacement for any course.

## Start here

1. Read the [interview workflow](concepts/interview-workflow.md) once to learn a repeatable way to structure a design.
2. Use the [concept index](concepts/README.md) to find a topic when revising.
3. Use the [pattern index](patterns/README.md) to choose common approaches and compare their trade-offs.
4. Open a problem walkthrough for a worked example. Each starts with scope and assumptions, then follows the request or data through the system.

## Concept notes

- [Interview workflow](concepts/interview-workflow.md)
- [Data modeling and APIs](concepts/data-modeling-and-apis.md)
- [Networking and capacity estimation](concepts/networking-and-estimation.md)
- [Preparation and deliberate practice](concepts/preparation-plan.md)
- [Scaling and data systems](concepts/scaling-and-data-systems.md)
- [Reliability and correctness](concepts/reliability-and-correctness.md)
- [Operations, security, and communication](concepts/operations-security-and-communication.md)
- [Technology primer](concepts/technology-primer.md)
- [Quick glossary](concepts/glossary.md)
- [Concept index and coverage checklist](concepts/README.md)

## Reusable patterns

See the [pattern index](patterns/README.md) for all pattern notes and a quick selection guide.

## Problem walkthroughs

See the [problem index](problems/INDEX.md) for a pattern-complete core path, additional examples per pattern, and the full catalog.

### Storage, files, and content

- [URL shortener](problems/bitly.md)
- [Cloud file storage and sync](problems/dropbox.md)
- [Video hosting and streaming](problems/youtube.md)
- [Web crawler](problems/web-crawler.md)
- [Product price tracking](problems/price-tracking.md)

### Social, messaging, and discovery

- [Social news feed](problems/facebook-news-feed.md)
- [Photo sharing](problems/instagram.md)
- [Chat and messaging](problems/whatsapp.md)
- [Live event comments](problems/live-comments.md)
- [Nearby business search](problems/yelp.md)
- [Dating and matching](problems/tinder.md)
- [Social activity tracking](problems/strava.md)
- [News aggregation](problems/google-news.md)
- [Search posts](problems/facebook-post-search.md)

### Commerce, money, and reservations

- [Local delivery marketplace](problems/local-delivery.md)
- [Concert ticketing](problems/ticketmaster.md)
- [Ride hailing](problems/uber.md)
- [Online auction](problems/online-auction.md)
- [Payment processing](problems/payment-system.md)
- [Brokerage and stock trading](problems/robinhood.md)
- [Flash sale](problems/flash-sale.md)

### Infrastructure and data platforms

- [Distributed rate limiter](problems/distributed-rate-limiter.md)
- [Top-K aggregation](problems/top-k.md)
- [Distributed cache](problems/distributed-cache.md)
- [Background job scheduler](problems/job-scheduler.md)
- [Ad click aggregation](problems/ad-click-aggregator.md)
- [Notification delivery](problems/notification-system.md)
- [Metrics monitoring](problems/metrics-monitoring.md)

### Collaboration, games, and AI

- [Online coding platform](problems/leetcode.md)
- [Collaborative documents](problems/google-docs.md)
- [Online chess](problems/online-chess.md)
- [LLM chat serving](problems/chatgpt.md)

### Additional More Practice prompts

The four prompts below are separate guided-practice examples, not part of the 32 official written breakdowns:

- [Food Review App](problems/food-review-app.md)
- [Game Leaderboard](problems/game-leaderboard.md)
- [Donations Website](problems/donations-website.md)
- [GitHub Actions](problems/github-actions.md)

## How to use a walkthrough

For a quick pass, read the scope, architecture, and quick-recall section. For a deeper pass, trace the numbered flow, then answer the trade-off questions without looking. Treat capacity values as illustrative unless a problem explicitly provides them; state your own assumptions in an interview.

## Coverage and limitations

This pack covers all 32 official problem breakdowns, the four separate More Practice prompts, and the concepts/patterns linked from the indexes. A system design has many valid solutions: these are teaching designs, not the only correct architecture. Service boundaries, storage choices, and consistency guarantees should change when requirements change.

## Further reading

For the original course and its current presentation, use its own pages:

- [Introduction](https://www.hellointerview.com/learn/system-design/in-a-hurry/introduction)
- [How to prepare](https://www.hellointerview.com/learn/system-design/in-a-hurry/how-to-prepare)
- [Delivery framework](https://www.hellointerview.com/learn/system-design/in-a-hurry/delivery)
- [Core concepts](https://www.hellointerview.com/learn/system-design/in-a-hurry/core-concepts)
- [Key technologies](https://www.hellointerview.com/learn/system-design/in-a-hurry/key-technologies)
- [Common patterns](https://www.hellointerview.com/learn/system-design/in-a-hurry/patterns)
- [Problem breakdown index](https://www.hellointerview.com/learn/system-design/in-a-hurry/problem-breakdowns)
