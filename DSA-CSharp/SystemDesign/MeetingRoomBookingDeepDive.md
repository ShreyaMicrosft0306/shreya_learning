# Meeting Room Booking System - Deep Dive Questions & Solutions

## Problem Overview
Build a module that manages meeting room bookings with operations:
- `book(room, start, end, organiser)` - Book a room
- `cancel(booking_id)` - Cancel a booking
- `list_bookings(room)` - List all bookings for a room

---

## Deep Dive #1: Handling Concurrent Booking Requests (Race Conditions)

### Question
**How would you handle the scenario where two users try to book the same room for overlapping time slots simultaneously? What happens if both requests pass the availability check at the same time?**

### The Problem
```
Time    User A                          User B
T0      Check 2-3pm available ✓         
T1                                      Check 2-3pm available ✓
T2      Insert booking 2-3pm            
T3                                      Insert booking 2-3pm
Result: DOUBLE BOOKING! ❌
```

### Solution 1: Pessimistic Locking (Database Level)

**How it works internally:**
1. Start a database transaction with Serializable isolation level
2. Execute `SELECT ... FOR UPDATE` to acquire an exclusive lock on the room row
3. While lock is held, check for booking conflicts
4. If no conflict, insert the booking and commit transaction
5. Lock is automatically released after commit/rollback
6. Other concurrent requests for same room wait until lock is released

**Pros:**
- Guarantees consistency
- Simple to implement
- No double bookings possible

**Cons:**
- Reduced throughput (serialized access per room)
- Can cause deadlocks in complex scenarios
- Slower for read-heavy workloads

### Solution 2: Optimistic Locking with Version Numbers

**How it works internally:**
1. Add a `Version` column to the Bookings table
2. Check for conflicts without acquiring any locks
3. Attempt to insert booking with version = 1
4. If another transaction committed in the meantime, catch the concurrency exception
5. Retry with exponential backoff (50ms, 100ms, 150ms)
6. After max retries (e.g., 3), return failure
7. No locks held, so other transactions proceed in parallel

**Pros:**
- Higher throughput (no locks during reads)
- Better performance for low-contention scenarios
- No deadlocks

**Cons:**
- Requires retry logic
- Can waste work on conflicts
- Not suitable for high-contention scenarios

### Solution 3: Unique Constraint at Database Level

**How it works internally:**
1. Create a database exclusion constraint using range types (PostgreSQL GIST)
2. Constraint: `EXCLUDE USING gist (RoomId WITH =, tsrange(StartTime, EndTime) WITH &&)`
3. Database checks for overlapping time ranges before insert/update
4. If overlap detected, database rejects the transaction with constraint violation error
5. Application catches the specific error code and returns "time slot unavailable"
6. No application-level conflict checking needed - database enforces integrity
7. Works atomically even under high concurrency

**Pros:**
- Database enforces correctness
- Simplest application code
- Best performance

**Cons:**
- Database-specific features
- Less control over error handling
- Complex query patterns for some databases

### Recommendation
**Use Solution 3 (Database Constraint)** for production systems - let the database handle what it's best at. Fall back to pessimistic locking if your database doesn't support exclusion constraints.

---

## Deep Dive #2: Efficiently Finding Available Time Slots

### Question
**How would you implement a feature to find all available time slots for a room on a given day? What if users want to find common availability across multiple rooms?**

### The Problem
Naive approach: Check every possible time slot → O(n) where n = minutes in a day (1440)

### Solution 1: Interval Merging (Invert Booked Slots)

**How it works internally:**
1. Define business hours (e.g., 9 AM - 6 PM)
2. Query all bookings for the room on that day, sorted by start time
3. Initialize current pointer at start of day (9 AM)
4. Iterate through bookings:
   - If gap exists between current pointer and next booking start, that's available time
   - Move current pointer to end of booking
5. After last booking, check if time remains until end of day
6. Return list of available gaps that meet minimum duration requirement

**Time Complexity:** O(n log n) where n = number of bookings
**Space Complexity:** O(n)

### Solution 2: Multi-Room Availability (Intersection of Intervals)

**How it works internally:**
1. Get availability slots for each room independently (using Solution 1)
2. Optimization: Start with the room that has fewest available slots
3. For each time slot in that room, check if all other rooms have overlapping availability
4. Interval intersection: A slot is available in Room B if Room B has any slot that fully contains it
5. Collect rooms available for each time slot
6. Return time slots with their list of available rooms
7. Complexity: O(n × m × k) where n=rooms, m=avg slots per room, k=comparison cost

### Solution 3: Caching with Redis (For High Traffic)

**How it works internally:**
1. Generate cache key: `availability:{roomId}:{date}`
2. On query: Check Redis cache first
3. Cache hit: Return cached availability immediately (sub-millisecond response)
4. Cache miss: Calculate availability from database, store in cache with 5-min TTL
5. On booking creation/cancellation: Invalidate cache for affected room+date
6. Trade-off: Slight staleness (up to 5 min) for massive performance gain
7. Reduces database load by 90%+ for frequently queried rooms

### Alternative: Pre-computed Availability Matrix

**How it works internally:**
1. Use fixed time slots (e.g., 30-minute intervals from 9 AM - 6 PM = 18 slots)
2. Represent availability as a bit array: 1 = available, 0 = booked
3. Pre-compute the bit array for each room-date combination
4. Store in memory: `{"ROOM-101:2026-07-11" => [1,1,0,0,1,1,1,...]}`
5. On availability check: Convert requested time to slot indices, check if all bits are 1
6. On booking: Flip bits to 0 for booked slots
7. Extremely fast lookups (O(1) after indexing), minimal memory (18 bits per room-day)

**Recommendation:** Use **Solution 1** (Interval Merging) for most cases, add **Solution 3** (Caching) for high-traffic systems.

---

## Deep Dive #3: Recurring Meetings and Series Management

### Question
**How would you extend the system to support recurring meetings (daily, weekly, monthly)? How would you handle operations like "cancel this occurrence" vs "cancel all future occurrences"?**

### The Problem
- Creating 52 individual booking records for a weekly meeting is inefficient
- Updating a series should be fast
- Need to handle exceptions (skip one occurrence, modify another)

### Edge case for conflict checking
Series A:
Every first Monday, 2 PM - 3 PM

Series B:
Every second Monday, 2:30 PM - 3:30 PM


### Solution 1: Series-Based Model with Exceptions

**Data Model:**
- `BookingSeries` table: Stores series metadata (pattern, start/end dates, recurrence type)
- `BookingExceptions` table: Stores deviations (cancelled, modified, moved occurrences)

**How it works internally:**
1. **Create Series**: Store one record with pattern (e.g., "Weekly on Monday 2-3 PM")
2. **Generate Occurrences**: Algorithm expands pattern into actual dates on-demand
3. **Cancel Single Occurrence**: Insert exception record with Type=Cancelled for that date
4. **Cancel All Future**: Update series end date to today
5. **Modify Occurrence**: Insert exception with Type=Modified, store new time/room
6. **Query Occurrences**:
   - Generate dates from pattern
   - Load all exceptions for the series
   - Apply exceptions: skip cancelled, use modified values
   - Return merged result
7. **Conflict Checking**: Expand series to individual dates, check each against existing bookings

### Solution 2: Materialized Occurrences (Hybrid Approach)

**How it works internally:**
1. Store series metadata in `BookingSeries` table
2. Pre-generate individual occurrence records for next N days (e.g., 90 days)
3. Store in `MaterializedOccurrences` table with foreign key to series
4. Background job runs nightly:
   - Delete occurrences in the past
   - Generate new occurrences to maintain 90-day window
5. On query: Simple SELECT from materialized table (no pattern expansion)
6. On cancellation: Mark occurrence as `IsCancelled = true`
7. On series modification: Regenerate all future occurrences
8. Trade-off: More storage for much faster queries and simpler conflict checking

**Pros of Series Model:**
- Efficient storage (1 record vs N records)
- Easy to modify all future occurrences
- Clear separation of pattern and exceptions

**Cons:**
- Complex query logic
- Need to expand series for conflict checking
- Performance impact for long-running series

**Recommendation:** Use **hybrid approach** - store series metadata but materialize occurrences for the next 90 days for query efficiency.

---

## Deep Dive #4: Handling Time Zones and Global Teams

### Question
**How would you handle meeting bookings across different time zones? What happens when a user in New York books a room in London for 3 PM their time?**

### The Problem
```
User in New York (EST): Books London room for 3 PM EST
Room in London: Should be booked for 8 PM GMT
Daylight saving time changes: Meeting time might shift!
```

### Solution 1: Store Everything in UTC, Display in Local Time

**Data Model:**
- `Rooms` table: Include `TimeZoneId` field (IANA format: "Europe/London")
- `Bookings` table: Store `StartTimeUtc` and `EndTimeUtc`, plus `OrganiserTimeZone`

**How it works internally:**
1. **On Booking Creation**:
   - Receive user's local time (e.g., "2 PM EST")
   - Convert to UTC using user's timezone
   - Convert to room's local time to validate business hours
   - Store only UTC times in database
   - Save user's timezone for context

2. **On Conflict Checking**:
   - Always compare UTC times
   - Simple range overlap: `StartA < EndB AND EndA > StartB`
   - No timezone complexity in conflict logic

3. **On Display**:
   - Retrieve UTC times from database
   - Convert to user's current timezone for display
   - Show both user's local time and room's local time
   - Example: "2:00 PM EST (7:00 PM GMT in London)"

4. **Benefits**: Single source of truth, DST-safe, consistent conflict checking

### Solution 2: Handle Daylight Saving Time Edge Cases

**How it works internally:**
1. **DST Transition Problem**:
   - Spring forward: 2 AM becomes 3 AM (hour disappears)
   - Fall back: 2 AM happens twice
   - Recurring meetings at 2:30 AM could be ambiguous or invalid

2. **Detection Strategy**:
   - Query timezone adjustment rules from system
   - Identify DST transition dates for each year
   - Check if any occurrence falls on transition date

3. **Handling Approaches**:
   - **Skip**: Don't create meeting on transition day
   - **Adjust**: Shift meeting by 1 hour automatically
   - **Flag**: Notify organizer for manual decision
   - **Use UTC**: Store as fixed UTC time (meeting shifts in local time)

4. **Best Practice**: For recurring meetings, use "leniently" conversion - if time doesn't exist, use nearest valid time

### Solution 3: NodaTime Library (Better Timezone Handling)

**Why NodaTime is better than DateTime:**
- **Explicit types**: `Instant` (UTC), `ZonedDateTime` (with timezone), `LocalDateTime` (no timezone)
- **Prevents bugs**: Can't accidentally compare times in different timezones
- **Better DST handling**: `InZoneLeniently()` handles non-existent times gracefully
- **IANA timezones**: Uses "America/New_York" instead of Windows "Eastern Standard Time"

**How it works internally:**
1. Accept input as `LocalDateTime` (date+time without timezone)
2. Convert to `ZonedDateTime` using user's timezone
3. Convert to `Instant` (point in time, UTC)
4. Perform business logic on `Instant` (conflict checking)
5. Convert back to `ZonedDateTime` for different timezones as needed
6. Store `Instant` as UTC DateTime in database

**Key Principles:**
1. **Always store in UTC** in the database
2. **Convert to local time only for display**
3. **Store the timezone** used when creating the booking
4. **Use IANA timezone IDs** (not Windows timezone IDs)
5. **Consider NodaTime** for complex timezone scenarios

**Recommendation:** Use **Solution 1** with NodaTime library for production systems handling global teams.

---

## Deep Dive #5: Scalability and Performance Optimization

### Question
**Your system now handles 10,000 rooms across 100 office locations with millions of bookings per day.How would you check the conflicts and make sure latency is less. How would you architect the system for scalability? What are the bottlenecks?**

### The Problem
- Single database can't handle the write load
- Conflict checking becomes slow with millions of bookings
- Users expect real-time availability updates

### Solution 0: Optimize Conflict Checking (100k Bookings Per Room)

**The Core Problem:**
```
Naive approach for conflict checking:
SELECT * FROM Bookings WHERE RoomId = 'ROOM-101'
Then iterate through all results to check overlap → O(n) where n = 100,000

With 100k bookings: Every booking request scans 100k records = VERY SLOW
```

**How to make it O(log n):**

**1. Proper Database Indexing (B-Tree)**
- Create composite index: `CREATE INDEX idx_room_time ON Bookings(RoomId, StartTime, EndTime)`
- **How B-Tree index works internally:**
  - Index creates a balanced tree structure sorted by (RoomId, StartTime, EndTime)
  - Database can binary search to find relevant bookings in O(log n) time
  - For conflict check query: `WHERE RoomId = ? AND StartTime < ? AND EndTime > ?`
  - Index scan: O(log n) to find first matching record + O(k) to scan overlapping records
  - k = number of actual overlaps (typically 0-5, not 100k)
  
**2. Query Optimization**
```sql
-- Bad: O(n) - fetches all bookings for room
SELECT * FROM Bookings WHERE RoomId = 'ROOM-101'

-- Good: O(log n) - uses index range scan
SELECT COUNT(*) FROM Bookings 
WHERE RoomId = 'ROOM-101' 
  AND Status = 'Active'
  AND StartTime < @EndTime 
  AND EndTime > @StartTime
LIMIT 1

-- Even better: Use COUNT(*) which can return immediately on first match
-- Some databases optimize LIMIT 1 or EXISTS even better
```

**3. Data Partitioning (Reduce Search Space)**
- **By Date**: Split into active (future) and historical (past) tables
  - Active bookings table: Only future bookings (much smaller)
  - Archive table: Past bookings (never queried for conflicts)
  - With 100k total, active might only be 5k → 20x faster
  
- **By Time Range**: Partition table by date ranges
  ```sql
  CREATE TABLE Bookings_2026_Q3 PARTITION OF Bookings
  FOR VALUES FROM ('2026-07-01') TO ('2026-10-01');
  ```
  - Query only touches relevant partition
  - 100k bookings across years → only search current month's partition (8k records)

**4. Additional Optimization - Covering Index**
```sql
CREATE INDEX idx_room_time_covering ON Bookings(RoomId, StartTime, EndTime)
INCLUDE (Status, BookingId);
```
- Database can answer query entirely from index without touching table
- "Index-only scan" in query plan
- Faster because index is smaller and more cache-friendly

**5. Alternative: Interval Tree (In-Memory)**
- **When to use**: Hot rooms with many bookings, frequent queries
- **Structure**: Augmented balanced BST where each node stores max end time in subtree
- **Performance**: 
  - Build: O(n log n) one-time cost
  - Query: O(log n + k) where k = overlapping intervals
  - Memory: Store in Redis/cache for frequently accessed rooms
- **How it works**:
  - Each node contains a booking interval [start, end]
  - Store max(end time) of all child nodes
  - Search: Can prune entire subtrees if query doesn't overlap max range
  
**6. Time-Based Bitmap (Fixed Slots)**
- **When to use**: Systems with fixed time slots (e.g., 30-min intervals)
- **Structure**: Bit array representing each time slot
  - Example: 9 AM - 6 PM in 30-min slots = 18 bits per day
  - `[1,1,0,0,1,1,1,...]` where 1=available, 0=booked
- **Performance**: O(1) to check if specific slot is available
- **Trade-off**: Only works for fixed granularity, but extremely fast

**Performance Comparison:**

| Approach | Conflict Check | Memory | Best For |
|----------|----------------|---------|----------|
| **No Index (Naive)** | O(n) = 100k comparisons | Low | Small datasets only |
| **B-Tree Index** | O(log n) ≈ 17 comparisons | Low | Default choice |
| **Partitioning + Index** | O(log m) where m << n | Low | Time-based partitions |
| **Interval Tree (Cache)** | O(log n + k) | High | Hot rooms, read-heavy |
| **Bitmap (Fixed Slots)** | O(1) | Medium | Fixed time granularity |

**Recommendation:** 
- **Minimum**: Use proper B-Tree indexing (RoomId, StartTime, EndTime) → reduces 100k to ~17 comparisons
- **Better**: Add partitioning to split active/historical → search only 5k active bookings
- **Best**: Combine indexing + partitioning + cache hot rooms in interval tree

**Real-World Impact:**
```
Without index: 100,000 rows scanned → 500ms query time
With index: 17 comparisons → 5ms query time
With index + partitioning: 5,000 active → 3ms query time
With cached interval tree: In-memory → 0.5ms query time
```

### Solution 1: Database Sharding by Location

**How it works internally:**
1. **Shard Key**: Use office location (US-East, US-West, EU, APAC)
2. **Shard Mapping**: Maintain lookup table: Room → Location → Database
3. **Write Path**:
   - Extract room ID from request
   - Look up which shard owns this room
   - Route write to that specific database
   - Execute transaction on single shard (fast)
4. **Read Path (Single Room)**:
   - Route to correct shard based on room
   - Query only one database
5. **Read Path (Multi-Room)**:
   - Parallel queries to all shards
   - Merge results in application layer
6. **Benefits**: Distributes load, improves locality (EU users hit EU DB)

### Solution 2: CQRS with Read Replicas

**How it works internally:**
1. **Separate Paths**:
   - Writes go to master database (BookingWriteService)
   - Reads go to read replicas (BookingReadService)
2. **Write Flow**:
   - Insert booking into master DB
   - Publish BookingCreatedEvent to message bus
   - Event handler invalidates relevant cache entries
   - Database replicates to read replicas (async, 1-5 sec lag)
3. **Read Flow**:
   - Check Redis cache first
   - Cache miss: Query read replica
   - Store result in cache with TTL
   - Return data
4. **Event-Driven Updates**:
   - On booking created/cancelled: Publish event
   - Event handler clears cache for affected room/date
   - Next read gets fresh data
5. **Benefits**: Scales reads infinitely, protects master from query load

### Solution 3: Event Sourcing for Audit and Consistency

**How it works internally:**
1. **Don't Store State, Store Events**:
   - Instead of UPDATE, append immutable events
   - Event stream: `[BookingCreated, BookingModified, BookingCancelled, ...]`
2. **Event Store**:
   - Append-only log per room (or aggregate)
   - Each event has: ID, timestamp, type, user, payload
3. **Rebuild State**:
   - Load all events for a room
   - Replay events to reconstruct current bookings
   - Check conflicts against reconstructed state
4. **Write Operation**:
   - Validate by replaying events
   - Append new event to stream
   - Publish event to message bus
5. **Optimization - Snapshots**:
   - Every N events, save snapshot of current state
   - Replay from snapshot + recent events (faster)
6. **Benefits**: Complete audit trail, time-travel debugging, replay capability

### Solution 4: Real-Time Updates with SignalR

**How it works internally:**
1. **WebSocket Connection**:
   - Client establishes persistent WebSocket to server
   - Fallback to long-polling if WebSockets unavailable
2. **Subscription Groups**:
   - Group pattern: `room-{roomId}-{date}`
   - Client subscribes to relevant room/date combinations
   - Server maintains connection → group mappings in memory
3. **Event Broadcasting**:
   - On booking created: Server publishes to all clients in group
   - Message: `{type: "BookingCreated", data: {...}}`
   - Clients receive instantly (no polling)
4. **Client Updates**:
   - Listener receives event
   - Updates UI immediately
   - No refresh needed
5. **Scalability**: Use Redis backplane to coordinate across multiple servers
6. **Use Case**: Real-time calendar updates, conflict notifications

### Performance Optimization Checklist

**Database Level:**
- ✅ Index on (RoomId, StartTime, EndTime, Status)
- ✅ Partition tables by date range
- ✅ Archive old bookings to separate tables
- ✅ Use read replicas for queries
- ✅ Implement connection pooling

**Application Level:**
- ✅ Cache frequently accessed data (Redis)
- ✅ Implement rate limiting per user
- ✅ Use async/await for I/O operations
- ✅ Batch database writes when possible
- ✅ Implement circuit breakers for external services

**Architecture Level:**
- ✅ Shard by location or room range
- ✅ Separate read/write databases (CQRS)
- ✅ Use message queue for async processing
- ✅ Implement CDN for static assets
- ✅ Use load balancers with health checks

**Monitoring:**
- ✅ Track booking creation latency
- ✅ Monitor conflict rate
- ✅ Alert on database slow queries
- ✅ Dashboard for system health
- ✅ Track cache hit/miss ratio

**Recommendation:** Start with **Solution 2** (CQRS with caching), add **Solution 1** (sharding) when a single database becomes a bottleneck, and use **Solution 4** (SignalR) for real-time features.

---

## Summary

| Deep Dive | Key Technique | When to Use |
|-----------|---------------|-------------|
| **Concurrent Bookings** | Database exclusion constraints + optimistic locking | Always - prevents double bookings |
| **Available Slots** | Interval merging + caching | High query volume for availability |
| **Recurring Meetings** | Series model + exception tracking | Corporate environments |
| **Time Zones** | Store UTC + NodaTime library | Global teams across timezones |
| **Scalability (100k bookings)** | B-Tree indexing + partitioning → O(log n) | Always - critical for performance |
| **Scalability (distributed)** | CQRS + Sharding + Event Sourcing | 10K+ rooms, millions of bookings |

**General Recommendations:**
1. **Optimize query performance first**: Proper indexing reduces O(n) to O(log n) - do this before any other optimization
2. Start simple, optimize based on actual usage patterns
3. Use database constraints to enforce business rules
4. Cache aggressively for read-heavy workloads
5. Always store times in UTC
6. Partition to separate active/historical bookings
7. Plan for horizontal scaling from day one
8. Implement comprehensive monitoring early
9. Design for eventual consistency in distributed scenarios
