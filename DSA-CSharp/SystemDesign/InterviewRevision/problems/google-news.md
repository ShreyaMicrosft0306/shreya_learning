# News aggregation and personalized discovery

## Scope and assumptions

Design a news product that collects articles from publishers, normalizes and deduplicates them, then presents topical and personalized feeds. It links to publisher pages rather than hosting full copyrighted articles. Ingestion should notice new stories promptly, while ranking may be updated periodically. Assume publishers provide feeds, APIs, or permitted crawl access. Full web crawling, ad auctions, editorial operations, and article translation are excluded.

## Requirements and exclusions

- Ingest article metadata and permitted snippets, identify duplicates or related coverage, and organize stories by topic.
- Serve a fast home feed and topic pages; support follow/mute topics and source preferences.
- Respect source attribution, robots/access policies, removals, and user privacy.
- Exclude publisher paywall handling, complete fact-checking, and detailed ranking-model training.

## Data model and API

Store `Source(id, domain, fetch_policy, trust_metadata)`, `Article(id, source_id, canonical_url, title, summary, published_at, fetched_at, language, content_hash, topic_ids, status)`, `StoryCluster(id, member_article_ids, representative_id, updated_at)`, and `UserPreference(user_id, followed_topics, muted_sources, locale)`. Keep fetched metadata and permitted excerpts, plus provenance for each field. APIs include `GET /feed?cursor=`, `GET /topics/{id}/stories`, `POST /preferences`, and `GET /articles/{id}` returning attribution and outbound link.

## Architecture

Source adapters poll feeds/APIs or receive publisher updates, respecting per-source limits. Raw responses and fetch metadata are stored for audit and replay. A normalization pipeline canonicalizes URLs, cleans metadata, detects language, extracts entities/topics, and computes duplicate candidates using hashes and similarity features. A clustering service groups coverage into stories while preserving separate publisher articles. Search/graph indexes and materialized topic feeds serve reads. A ranking service combines freshness, relevance, source diversity, and user preferences. A policy service handles removals and source restrictions.

## End-to-end request/data flow

1. **Ingest:** Scheduler selects eligible sources based on their poll interval and recent change rate. Adapter fetches only allowed endpoints with conditional requests where supported, stores raw provenance, and emits changed article records. Backoff and per-domain limits protect publisher infrastructure.
2. **Normalize and cluster:** Workers canonicalize URLs and metadata, calculate content fingerprints, and search for near-duplicate candidates by title, source, time, and text similarity. They attach coverage to an existing cluster or create a new one; clustering records remain reversible because early matches can be wrong.
3. **Index and rank:** Topic extraction updates article and cluster indexes. Topic pages rank fresh clusters while retaining article-level source links. For personalized feeds, candidate generation retrieves followed topics and sources, then ranking applies locale, recency, diversity, and deduplication.
4. **Serve:** Client requests a cursor page. Feed API returns representative headline, source attribution, publication time, and cluster alternatives. Clicking opens the publisher URL. A stable cursor combines score bucket and IDs; ranking changes may affect later pages, so cursors have a bounded lifetime.

## Scaling, consistency, and reliability

Partition ingestion by source/domain to enforce fair, independent scheduling; partition article stores by ID or publication time. Deduplicate using staged, scalable matching: exact normalized URL/hash first, then approximate candidates rather than comparing every article pair. Most read paths use materialized topic/personalized candidates, regenerated asynchronously after article, topic, or preference changes. Canonical article provenance and policy status are authoritative; ranking and clustering can be eventually consistent. Make raw normalized records replayable so taxonomy changes can reprocess history. If a publisher endpoint fails, preserve last known valid stories and mark freshness; do not flood it with retries. If ranking is unavailable, serve a recency-ordered topic feed.

## Deep dives and failure cases

**Deduplication versus story clustering:** Exact duplicate detection is straightforward, but related reporting is not identical content. Over-aggressive clustering hides distinct updates or local reporting; weak clustering shows repetitive headlines. Separate article identity from story grouping, retain source-level alternatives, use time/topic constraints, and support later split/merge correction. Ranking a cluster should not erase coverage from smaller publishers; apply source diversity and transparency.

**Stale or removed content:** Feed entries and caches can outlive a publisher update or policy change. Keep source status/version and recheck policy before serving; removal events invalidate dependent clusters and caches. A stale cache may show a headline temporarily, so short TTLs for mutable policy state and a read-time suppression check are safer than relying only on background cleanup.

## Quick recall

Respectful source-specific ingestion; provenance-preserving normalization; article identity separate from story clusters; materialized feeds with freshness/diversity ranking; policy-aware reads; replayable pipelines and recency fallback.
