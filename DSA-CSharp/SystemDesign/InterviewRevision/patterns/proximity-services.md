# Proximity services

Proximity services answer questions such as “What is nearby?” for restaurants, drivers, stores, or events. The key is to narrow candidates efficiently, then compute exact distance and apply business filters. A latitude/longitude pair describes a point on Earth, but degrees are not equal-distance units everywhere, so naive coordinate subtraction is not a reliable distance measurement.

## When to use

Use geospatial indexing when location is a primary filter and the dataset or query rate makes scanning every point too expensive. For a small dataset, a database query or even an in-memory scan with a bounding box may be enough. Choose the solution based on query shape: nearest points, all points within a radius, geographic containment, or route-aware travel time. Straight-line distance is not driving distance.

## Architecture and flow

Store each entity’s coordinates with its ordinary metadata and an index designed for spatial lookup. A spatial database index can quickly return points inside a radius or nearby region. Another approach maps coordinates to cells, such as geohashes: a geohash encodes a location into a string whose shared prefixes represent nearby areas. Query the cell containing the user and adjacent cells, then calculate exact distance for candidates. Cell boundaries matter, so include all intersecting neighboring cells.

A typical request validates the user location and radius, selects candidates from the geospatial index or cells, filters for availability and permissions, calculates precise great-circle distance (distance along the Earth’s surface), sorts by distance or a ranking score, and returns a bounded page. For large result sets, use a cursor based on stable ordering rather than an unbounded offset. If the product needs travel time, call or precompute a routing service separately; routing is more costly and changes with roads and traffic.

## Tradeoffs

Spatial indexes provide expressive queries but require database-specific capabilities and careful query tuning. Cell indexes are portable and easy to shard by region, but cell size trades candidate count against boundary queries. Fine cells reduce candidates but increase index entries and neighboring-cell lookups; coarse cells do the reverse. Ranking may combine distance, quality, availability, and personalization, so define whether “nearest” means strictly shortest distance or highest overall relevance.

Freshness matters for moving entities. A driver’s old location can be misleading; attach update timestamps and exclude or downgrade stale positions. Location data is sensitive: collect only what is needed, restrict access, apply retention limits, and avoid logging precise coordinates unnecessarily. Consider user consent and location accuracy; a device may provide an approximate point.

## Failure modes and scaling

Popular urban cells can become hot while rural cells remain sparse. Partition by geographic region, but split hot regions or add a secondary partition dimension when one cell overloads. A global query may cross many partitions; require a bounded radius or route it through a broader regional index. Missing neighboring cells can silently omit results at boundaries. Invalid coordinates, stale updates, or location spoofing can corrupt results; validate ranges and apply domain-specific plausibility checks.

Cache stable query results carefully: user location, radius, filters, and data freshness all affect the key. Frequently changing availability limits cache duration. Use coarse geolocation for caching where product requirements allow. Monitor candidate-to-result ratio, query latency by region, stale-location rate, and hot-cell skew.

## Example prompts

- Design “nearby restaurants” search with filters and ranking.
- Design a driver dispatch service for nearby available drivers.
- Design a store locator that supports radius and category filters.

## Interview quick checklist

- Is the query radius-based, nearest-k, containment, or route-time based?
- What index narrows candidates, and are boundary neighbors included?
- How are exact distance, pagination, and ranking defined?
- How fresh must moving locations be, and how are stale points treated?
- How are hot urban regions partitioned and broad queries bounded?
- What privacy, retention, and access controls protect location data?
