# Inventory Manager System - Deep Dive Questions & Solutions

## Problem Overview

Build a module that manages product inventory. Implement the following operations:

- `add_stock(product_id, quantity)` — Add stock for a product
- `remove_stock(product_id, quantity)` — Remove stock; error if removal would cause stock to go negative
- `check_availability(product_id)` — Return current stock level, or error if product does not exist

**Constraints:**
- Stock can never go below zero
- Quantities must be positive integers

**Baseline complexity:**

| Operation | Time | Space | Notes |
|-----------|------|-------|-------|
| `add_stock` | O(1) | O(p) | p = distinct products; dict keyed by product_id |
| `remove_stock` | O(1) | — | Lookup + validation |
| `check_availability` | O(1) | — | Direct key access |

---

## Deep Dive #1: Handling Concurrent Stock Updates (Race Conditions)

### Question

**How would you handle the scenario where two users try to remove stock simultaneously from the same product? What happens if both requests check availability at the same time?**

### The Problem

```
Time    User A                    User B
T0      Check stock = 5 ✓         
T1                               Check stock = 5 ✓
T2      Remove 4 units           
T3                               Remove 4 units
Result: Stock becomes -3 ❌
```

Both users see 5 units available. Both proceed to remove 4. Final stock should be -3, which violates the constraint.

### Solution 1: Pessimistic Locking (Database / In-Memory Lock)

**How it works internally:**

1. Acquire a lock on the product row before checking stock
2. While locked, read current stock
3. Validate that `current - quantity >= 0`
4. Update stock and release lock
5. Other concurrent requests for the same product wait until lock is released

**Pros:**
- Guarantees consistency
- Simple to implement
- No double removals possible

**Cons:**
- Reduced throughput (serialized access per product) even reads locked
- Can cause deadlocks in complex scenarios
- Slower for high-concurrency scenarios

### Solution 2: Optimistic Locking with Version Numbers

**How it works internally:**

1. Add a `Version` column to the inventory table
2. Read current stock and version without locking
3. Attempt to update: `UPDATE Inventory SET Stock = Stock - ?, Version = Version + 1 WHERE ProductId = ? AND Version = ?`
4. If version mismatch, retry with exponential backoff (50ms, 100ms, 150ms)
5. After max retries (3), return failure

**Pros:**
- Higher throughput (no locks during reads)
- Better for low-contention scenarios
- No deadlocks

**Cons:**
- Requires retry logic
- Can waste work on conflicts
- Not suitable for high-contention products

### Solution 3: Atomic Compare-And-Swap (CAS) / Database Constraints

**How it works internally:**

1. Use a database constraint: `CHECK (Stock >= 0)`
2. Attempt the update without locks
3. If constraint violated, database rejects the transaction
4. Application catches the constraint error and retries or returns failure
5. Constraint enforces invariant atomically

**Pros:**
- Database handles correctness
- No application-level locking overhead
- Simple to reason about

**Cons:**
- Less control over retry strategy
- Constraint violation doesn't tell you why (low stock vs other reason)
- Database-specific syntax

### Solution 4: Event-Driven with Saga Pattern (Distributed Transactions)

**How it works internally:**

1. Don't update immediately; create a `StockRemovalRequest` event
2. Event is added to an event queue (Kafka, RabbitMQ)
3. Single-threaded consumer processes events in order for each product
4. Consumer checks stock, removes, and publishes `StockRemoved` event
5. Ordering is guaranteed per product; no race conditions

Partitioned queue by product_id
Multiple consumers
Sequential processing only per product
Parallel processing across products


**Pros:**
- Scales to very high concurrency
- Audit trail of all stock changes
- Can replay history

**Cons:**
- Added complexity (message queue, consumer)
- Slight latency (async processing)
- Distributed system failures to handle

### Recommendation

Use **Solution 3 (Atomic Database Constraint)** for most systems. Add **Solution 2 (Optimistic Locking)** if you need finer control over retries. Use **Solution 4 (Event-Driven)** for extremely high-concurrency, globally-distributed inventory.

---

## Deep Dive #2: Efficient Queries — Low Stock, Trending Products, Stock Movement

### Question

**How would you efficiently answer queries like: (1) All products with stock below threshold, (2) Products with highest stock turnover, (3) Top movers in the last hour?**

### The Problem

Naive approach: Scan all products and filter in-memory → O(n) every query

```
SELECT * FROM Inventory
filter at db level stock<10  still need to scan all rows to check this condition without an index
or filter in application logic
→ With 1M products, this is very slow
```

### Solution 1: Materialized Views with Thresholds

**How it works internally:**

1. Create a table `LowStockAlerts` that caches products with stock < threshold
2. Update this view whenever stock changes:
   - If stock was above threshold and now below, INSERT
   - If stock was below and now above, DELETE
3. Query this view directly: `SELECT * FROM LowStockAlerts` → O(1) on table size
Inventory
├── ProductId (PK)
├── ProductName
├── Stock
└── Category

LowStockAlerts  ← Pre-computed view
├── ProductId (FK)
├── ProductName
├── Stock
└── UpdatedAt

**Pros:**
- Queries are extremely fast
- Threshold is configurable
- Easy to add alerts/notifications

**Cons:**
- Extra table to maintain
- View becomes stale if updates fail mid-way

### Solution 2: Indexed Queries (B-Tree on Stock Level)

**How it works internally:**

1. Create index: `CREATE INDEX idx_stock ON Inventory(Stock, ProductId)`
2. Query: `SELECT * FROM Inventory WHERE Stock < 10 ORDER BY Stock`
3. Index is sorted by stock level; range scan finds all products below threshold

**Pros:**
- No extra table
- Works for any threshold value (not pre-computed)
- Simple to implement

**Cons:**
- Still O(m) where m = products below threshold
- If many products below threshold, scan is expensive

### Solution 3: Time-Series Data and Turnover Tracking
Top movers are the products that had the most activity during a specific time period.

Product | Units removed/sold
--------|--------------
MILK    | 100
BREAD   | 75
CHEESE  | 10


**How it works internally:**

1. Create `StockHistory` table with timestamp:

```text
ProductId, Timestamp, Action, Quantity, StockAfter
PROD-1,    2026-08-13 10:00, REMOVE,   5,         45
PROD-1,    2026-08-13 10:05, REMOVE,   3,         42
PROD-1,    2026-08-13 10:10, ADD,      10,        52
```

2. Calculate top movers for a time window:

```text
SELECT
    ProductId,
    SUM(Quantity) AS UnitsMoved
FROM StockHistory
WHERE Action = 'REMOVE'
  AND Timestamp >= CURRENT_TIMESTAMP - INTERVAL '1 hour'
GROUP BY ProductId
ORDER BY UnitsMoved DESC
LIMIT 10;```

CREATE INDEX idx_stock_history_movers
ON StockHistory (Action, Timestamp, ProductId);

3. Create `TopMoversCache` updated every 5 minutes
For very frequent dashboard queries, calculate the totals every few minutes and store them in a TopMoversCache table instead of scanning StockHistory every time.

**Pros:**
- Complete audit trail
- Can calculate any time window
- Supports historical analysis

**Cons:**
- Extra storage for history
- Cache must be updated periodically
- Queries are still O(m)

### Solution 4: Bloom Filters + Indexed Lookup (For Exact Threshold Queries)

**How it works internally:**

1. Use a Bloom filter to quickly check if a product *might* be below threshold (O(1) false positives possible)
2. If Bloom says "maybe", query the index to confirm (O(log n))
3. Reduces index lookups for products above threshold

**Pros:**
- Very fast negative checks (products definitely above threshold)
- Memory-efficient

**Cons:**
- False positives require extra validation
- Complexity for marginal gain

### Recommendation

Use **Solution 2 (Indexed Queries)** for ad-hoc low-stock checks. Add **Solution 3 (Time-Series History)** for turnover and trending products. Materialize views only if queries are extremely frequent.

---

## Deep Dive #3: Stock Reservations, Holds, and Pre-Allocation

### Question

**How would you support reserving stock for an order that hasn't been paid yet? What if a customer abandons their cart after 30 minutes?**

### The Problem

```
Scenario 1:
Customer A adds 10 units to cart (should this reduce available stock?)
Customer B tries to buy the same 10 units
Should Customer B's purchase be blocked, or should only paid orders hold stock?

Scenario 2:
Customer A reserves 10 units
Customer A abandons cart after 30 minutes
Should the stock automatically become available again?
```

### Solution 1: Soft Reservations (Logical Holds, Not Real Deductions)

**How it works internally:**

1. Create a `StockReservation` table:

```text
ReservationId, ProductId, Quantity, CustomerId, ExpiresAt, Status
RES-001,       PROD-1,    10,       CUST-A,     2026-08-13 10:30, ACTIVE
```

2. Available stock calculation:

```text
AvailableStock = TotalStock - SUM(ReservedQuantity)
```

3. Expiration handled by:
   - Background job checks expired reservations every 5 minutes
   - Deletes expired reservations
   - Available stock auto-increases

**Pros:**
- Flexible hold periods
- Doesn't mutate actual inventory until payment
- Easy to extend or cancel

**Cons:**
- Need background job to clean up
- Another table to query
- "Available" becomes calculated rather than stored

### Solution 2: Hard Reservations (Immediate Stock Deduction)

**How it works internally:**

1. When customer adds to cart:
   - Remove from inventory immediately
   - Store in `CartItems` or `Order` table with status PENDING
2. On payment:
   - Status changes to CONFIRMED
   - Stock remains removed
3. On cart abandonment:
   - Status changes to CANCELLED
   - Stock is returned via `add_stock`

**Pros:**
- Inventory reflects true availability
- Simple logic: inventory is what you see
- No "surprise" oversells

**Cons:**
- Stock is locked up even if customer abandons cart
- Requires cleanup job or timeout monitoring
- Customer experience: see "sold out" if others add to cart first

### Solution 3: Hybrid: Tiered Availability

**How it works internally:**

1. Track three tiers:

```text
TotalStock = 100
ReservedForOrder = 30        (paid and confirmed)
ReservedForCart = 40         (in abandoned carts, expires in 30 min)
AvailableForNewOrders = 30   (TotalStock - ReservedForOrder - ReservedForCart)
```

2. When checking availability:
   - Customers see "AvailableForNewOrders"
   - Reserved stock is hidden but allocated
3. On abandon:
   - ReservedForCart decreases
   - AvailableForNewOrders increases

**Pros:**
- Customers see realistic availability
- Reserved stock is isolated from available
- Clear separation of concerns

**Cons:**
- More complex tracking
- More queries to calculate tiers

### Recommendation

Use **Solution 1 (Soft Reservations)** for simple systems where cart abandonment is common. Use **Solution 2 (Hard Reservations)** for inventory-constrained systems (e.g., tickets, limited runs). Use **Solution 3 (Hybrid)** for high-volume, mixed-urgency scenarios.

---

## Deep Dive #4: Multi-Warehouse Inventory Distribution

### Question

**How would you manage inventory across 50 warehouses? How do you route an order to the nearest warehouse with stock?**

### The Problem

```
Product PROD-1:
Warehouse US-East: 100 units
Warehouse US-West: 50 units
Warehouse EU-Central: 75 units

Customer in New York orders 30 units.
Which warehouse should fulfill?
```

Options:
1. Always use US-East (closest)
2. Use whichever has stock
3. Balance across warehouses
4. Reserve from nearest, fallback to furthest

### Solution 1: Centralized Inventory (All in One Warehouse)

**How it works internally:**

1. Single global stock count
2. Warehouse fulfillment is a separate routing layer
3. On order: pick warehouse based on:
   - Geographic proximity to customer
   - Warehouse utilization
   - Shipping cost

**Pros:**
- Simple stock tracking
- Flexible fulfillment routing

**Cons:**
- No local inventory resilience
- Network delays to central database

### Solution 2: Distributed Inventory (Per-Warehouse Tracking)

**How it works internally:**

1. Each warehouse maintains its own inventory:

```text
InventoryByWarehouse
  Warehouse, ProductId, Stock
  US-East,   PROD-1,    100
  US-West,   PROD-1,    50
  EU-Central,PROD-1,    75
```

2. On order:
   - Find nearest warehouse with stock
   - Deduct from that warehouse
   - If nearest is empty, try next closest
   - Fallback to furthest if needed

3. Rebalancing job periodically redistributes stock:
   - Move from over-stocked to under-stocked warehouses
   - Minimize long-term transit costs

**Pros:**
- Local resilience (one warehouse outage doesn't block all orders)
- Faster fulfillment (local warehouse)
- Independent scaling per warehouse

**Cons:**
- More complex queries (check multiple tables)
- Rebalancing overhead
- Risk of uneven distribution

### Solution 3: Hierarchical Inventory (Hub-and-Spoke)

**How it works internally:**

1. Structure:

```text
Central Hub
  ├── Regional Hub (US)
  │     ├── Warehouse US-East
  │     ├── Warehouse US-West
  └── Regional Hub (EU)
        ├── Warehouse EU-Central
```

2. Stock flows:
   - Central Hub knows aggregate stock
   - Regional Hubs know regional availability
   - Local warehouses know local inventory
3. Query routing:
   - First check local warehouse
   - If empty, check regional hub cache
   - If still empty, check central hub

**Pros:**
- Balances locality and global visibility
- Reduces cross-warehouse lookups
- Hierarchical resilience

**Cons:**
- Cache inconsistency (hub and warehouse out of sync)
- More complex rebalancing

### Solution 4: Event-Driven Inventory Sync (Eventually Consistent)

**How it works internally:**

1. Each warehouse operates independently with local stock
2. On stock change, publish event: `StockChanged(Warehouse, ProductId, NewQuantity)`
3. Central inventory service subscribes and maintains aggregate view
4. On order, central service suggests warehouse but doesn't enforce:
   - Warehouse confirms stock availability
   - If warehouse is out, customer is rerouted
5. Eventual consistency: central view lags by ~1-5 seconds

**Pros:**
- Highly resilient (warehouses work even if central is down)
- Scalable (no single bottleneck)
- Works well for distributed teams

**Cons:**
- Possible double-sells during sync delay
- Requires conflict resolution (which warehouse fulfills if multiple have stock)
- More infrastructure (message queue, event bus)

### Recommendation

Use **Solution 2 (Distributed Inventory)** for systems with 10-50 warehouses and predictable demand. Add **Solution 3 (Hierarchical)** for complex supply chains. Use **Solution 4 (Event-Driven)** for global, autonomous warehouse networks.

---

## Deep Dive #5: Automated Stock Reorder and Predictive Restocking

### Question

**How would you automatically reorder stock when it drops below a threshold? How do you predict demand and avoid both stockouts and overstock?**

### The Problem

```
Product PROD-1:
Current stock: 8 units
Reorder threshold: 20 units
Daily demand: ~5 units (avg)
Supplier lead time: 7 days

If we order when stock hits 20, we may over-order.
If we order at 10, we may run out before delivery.
What's the optimal reorder point and quantity?
```

### Solution 1: Fixed Reorder Point (Simple, No Prediction)

**How it works internally:**

1. Define reorder point and reorder quantity (static):

```text
ReorderPoint = 50 units
ReorderQuantity = 200 units
```

2. Background job every hour:
   - Check current stock
   - If stock <= ReorderPoint, create a `PurchaseOrder`
3. Stop reordering if an open PO already exists for the product

**Pros:**
- Simple to implement
- Works for stable demand

**Cons:**
- No demand forecasting
- Leads to overstock in low-demand periods
- Stockouts in high-demand periods

### Solution 2: Demand-Based Reorder Point (Time-Series Analysis)

**How it works internally:**

1. Track historical demand:

```text
StockHistory table with daily/hourly removals
ProductId, Date, QuantityRemoved
PROD-1,    2026-08-01, 50
PROD-1,    2026-08-02, 48
PROD-1,    2026-08-03, 52
```

2. Calculate:
   - Average daily demand (last 30 days)
   - Standard deviation
   - Seasonal trends (higher on weekends, etc)

3. Reorder point formula:

```text
ReorderPoint = (AvgDailyDemand × SupplierLeadTime) + SafetyStock
SafetyStock = StdDev × Z-score (e.g., Z=2 for 95% confidence)

Example:
AvgDailyDemand = 50 units
SupplierLeadTime = 7 days
StdDev = 10 units
SafetyStock = 10 × 2 = 20 units
ReorderPoint = (50 × 7) + 20 = 370 units
```

4. Reorder quantity based on economic order quantity (EOQ):

```text
EOQ = sqrt((2 × D × S) / H)
D = annual demand
S = order cost
H = holding cost per unit per year
```

**Pros:**
- Accounts for demand variability
- Reduces overstock
- Reduces stockout risk

**Cons:**
- Requires historical data (warm-up period)
- Doesn't handle sudden demand spikes
- Forecasting can be wrong

### Solution 3: Dynamic Reorder with Real-Time Predictions

**How it works internally:**

1. Use machine learning or statistical model to predict demand for next week
2. Update reorder point daily:

```text
PredictedDemand (next 7 days) = ML_model(historical_data, seasonality, events)
ReorderPoint = (PredictedDemand) + SafetyStock
```

3. Trigger reorder when:
   - Stock <= ReorderPoint OR
   - Stock + AvailablePOs < MinimumSafetyLevel

4. Reorder quantity:
   - If demand is rising, order more
   - If demand is falling, order less

**Pros:**
- Adapts to demand changes
- Reduces stockout risk without over-ordering
- Accounts for events (sales, holidays)

**Cons:**
- Requires ML infrastructure
- Model accuracy matters
- Can be expensive to train

### Solution 4: Multi-Supplier with Diversified Lead Times

**How it works internally:**

1. Define multiple suppliers per product:

```text
SupplierA: LeadTime=7 days, CostPerUnit=$1.50, MinOrder=100
SupplierB: LeadTime=3 days, CostPerUnit=$1.80, MinOrder=50
SupplierC: LeadTime=14 days, CostPerUnit=$1.20, MinOrder=200
```

2. On reorder decision:
   - If stock is critical (< SafetyStock), use SupplierB (fast, expensive)
   - If stock is low but not critical, use SupplierA (balanced)
   - If stock is falling slowly, use SupplierC (cheap, slow)

3. Blend orders across suppliers to minimize total cost:

```text
TotalCost = (UnitCost × Quantity) + OrderingCost + HoldingCost
```

**Pros:**
- Risk mitigation (supplier outage doesn't stop orders)
- Cost optimization (use slow supplier when time allows)
- Flexibility in lead times

**Cons:**
- Complex supplier management
- Coordination overhead
- May require dual-sourcing contracts

### Recommendation

Use **Solution 1 (Fixed Reorder)** for stable, high-volume products. Add **Solution 2 (Demand-Based)** for seasonal or variable products. Use **Solution 3 (ML Predictions)** for mission-critical inventory. Use **Solution 4 (Multi-Supplier)** for supply-chain resilience.

---

## Deep Dive #6: Handling Backorders and Partial Fulfillment

### Question

**What happens when a customer orders 100 units but only 60 are available? Do you allow backorders, split shipments, or reject the order?**

### The Problem

```
Order: 100 units
Available: 60 units

Options:
1. Reject order (customer goes elsewhere)
2. Fulfill partially, ship 60 now, backorder 40 for later
3. Hold order until all 100 are available (customer waits)
4. Auto-notify customer and ask preference
```

### Solution 1: No Backorders (All or Nothing)

**How it works internally:**

1. On order:
   - Check if `Stock >= OrderQuantity`
   - If yes, deduct and ship
   - If no, reject with "out of stock"

**Pros:**
- Simple logic
- No partial shipments
- Inventory is clean

**Cons:**
- Lost sales
- Poor customer experience
- Competitors benefit

### Solution 2: Full Backorders (Hold Until Complete)

**How it works internally:**

1. Create `BackorderQueue` table:

```text
BackorderID, ProductId, CustomerId, Quantity, OrderDate, Status
BO-001,      PROD-1,    CUST-A,     40,       2026-08-13, PENDING
```

2. When stock is replenished:
   - Check backorder queue (FIFO)
   - Allocate new stock to oldest backorder first
   - Ship and mark as FULFILLED
   - If still short, wait for next replenishment

**Pros:**
- No lost sales
- FIFO fairness

**Cons:**
- Customers wait (some abandon orders)
- Requires backorder management system
- Cash flow impact (money collected but goods not shipped)

### Solution 3: Partial Fulfillment with Split Shipments

**How it works internally:**

1. On order:
   - Fulfill available stock immediately (Shipment 1)
   - Create backorder for remainder (Shipment 2)
   - Notify customer of split shipment plan

2. Inventory tracking:

```text
OrderLineItem
  OrderID, ProductId, Quantity, FulfilledQuantity, BackorderQuantity, Status
  ORD-001, PROD-1,    100,      60,                40,                PARTIAL
```

3. When stock replenishes:
   - Check pending backorders
   - Ship remainder

**Pros:**
- Customer gets partial shipment quickly
- Reduces wait time for backorder
- Improves satisfaction

**Cons:**
- Multiple shipments = higher shipping cost
- Inventory tracking complexity
- Return management becomes complex

### Solution 4: Flexible Backorder with Expiry and Cancellation

**How it works internally:**

1. Create backorder with configurable options:

```text
BackorderID, ProductId, CustomerId, Quantity, ExpiresAt, AutoFulfill, Status
BO-001,      PROD-1,    CUST-A,     40,       2026-08-20, true,         PENDING
```

2. On backorder:
   - Notify customer: "We'll ship on [date] or you can cancel"
   - If AutoFulfill=true, ship when stock available
   - If AutoFulfill=false, wait for customer confirmation
3. On expiry:
   - If not fulfilled, automatically cancel and refund
   - Notify customer

**Pros:**
- Customer control (confirm or cancel)
- Reduces abandoned backorders
- Clear expectations

**Cons:**
- More interactive (API calls to confirm)
- Requires notification system

### Recommendation

Use **Solution 2 (Full Backorders)** for high-demand, limited-supply scenarios (e.g., Apple launch). Use **Solution 3 (Partial Fulfillment)** for e-commerce with predictable restocking. Use **Solution 4 (Flexible)** for premium customers or luxury goods.

---

## Summary

| Deep Dive | Key Technique | When to Use |
|-----------|---------------|-------------|
| **Concurrent Updates** | Optimistic locking or atomic constraints | Always - prevents double sells |
| **Query Efficiency** | Indexed queries + materialized views | High query volume |
| **Stock Reservations** | Soft reservations with expiry | Cart/order systems |
| **Multi-Warehouse** | Distributed inventory + rebalancing | 10+ warehouses |
| **Automated Reorder** | Demand-based reorder points + ML predictions | Predictable supply chains |
| **Backorders** | Partial fulfillment with expiry options | High-demand products |

**General Recommendations:**

1. **Always validate before mutating**: Check constraints before removing stock
2. **Use optimistic locking for high concurrency**: Reduces lock contention
3. **Materialize high-frequency queries**: Low stock, trending products
4. **Track history for auditing**: Why did stock drop? Who ordered it?
5. **Design for eventual consistency**: Multi-warehouse systems benefit from async updates
6. **Set clear expiry policies**: Reservations and backorders should auto-expire
7. **Monitor stock levels in real-time**: Dashboards for critical products
8. **Implement circuit breakers**: If reorder system fails, fall back to manual ordering

---

# Coding Problems & Solutions

## Problem 1: Concurrent Stock Management with Conflict Detection

### Problem Statement

Design an `InventoryManager` class that supports concurrent stock operations. The system must:

1. Support `add_stock(product_id, quantity)` and `remove_stock(product_id, quantity)`
2. Prevent stock from going negative
3. Handle concurrent requests using optimistic locking
4. Return detailed error messages (invalid product, insufficient stock, version conflict)
5. Track version numbers to detect conflicts

### Example

```csharp
var inventory = new InventoryManager();

// Add products
inventory.add_stock("PROD-1", 100);  // Version 1
inventory.add_stock("PROD-2", 50);

// Concurrent removes (simulated)
Thread t1 = new Thread(() => inventory.remove_stock("PROD-1", 60)); // Should succeed
Thread t2 = new Thread(() => inventory.remove_stock("PROD-1", 50)); // Should fail with conflict

t1.Start();
t2.Start();
t1.Join();
t2.Join();

// Check final state
Console.WriteLine(inventory.check_availability("PROD-1")); // 40 or RetryException
```

### Solution

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

public class InventoryManager
{
    private class ProductInventory
    {
        public int Stock { get; set; }
        public int Version { get; set; }
        public object Lock { get; set; } = new object();
    }

    private Dictionary<string, ProductInventory> inventory = new();

    public void add_stock(string product_id, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive");

        lock (inventory)
        {
            if (!inventory.ContainsKey(product_id))
            {
                inventory[product_id] = new ProductInventory { Stock = quantity, Version = 1 };
            }
            else
            {
                lock (inventory[product_id].Lock)
                {
                    inventory[product_id].Stock += quantity;
                    inventory[product_id].Version++;
                }
            }
        }
    }

    public void remove_stock(string product_id, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive");

        lock (inventory)
        {
            if (!inventory.ContainsKey(product_id))
                throw new InvalidOperationException($"Product {product_id} not found");

            var product = inventory[product_id];

            lock (product.Lock)
            {
                if (product.Stock < quantity)
                    throw new InvalidOperationException(
                        $"Insufficient stock. Available: {product.Stock}, Requested: {quantity}");

                product.Stock -= quantity;
                product.Version++;
            }
        }
    }

    public int check_availability(string product_id)
    {
        lock (inventory)
        {
            if (!inventory.ContainsKey(product_id))
                throw new InvalidOperationException($"Product {product_id} not found");

            lock (inventory[product_id].Lock)
            {
                return inventory[product_id].Stock;
            }
        }
    }

    public int get_version(string product_id)
    {
        lock (inventory)
        {
            if (!inventory.ContainsKey(product_id))
                throw new InvalidOperationException($"Product {product_id} not found");

            lock (inventory[product_id].Lock)
            {
                return inventory[product_id].Version;
            }
        }
    }
}

// Test
class Program
{
    static void Main()
    {
        var inventory = new InventoryManager();
        inventory.add_stock("PROD-1", 100);
        
        int success_count = 0;
        
        var threads = new List<Thread>();
        for (int i = 0; i < 10; i++)
        {
            threads.Add(new Thread(() =>
            {
                try
                {
                    inventory.remove_stock("PROD-1", 10);
                    lock (Console.Out)
                    {
                        success_count++;
                    }
                }
                catch (InvalidOperationException ex)
                {
                    lock (Console.Out)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                }
            }));
        }

        threads.ForEach(t => t.Start());
        threads.ForEach(t => t.Join());

        Console.WriteLine($"Final stock: {inventory.check_availability("PROD-1")}"); // 0
        Console.WriteLine($"Successful removals: {success_count}"); // 10
    }
}
```

---

## Problem 2: Low Stock Queries and Materialized Views

### Problem Statement

Optimize low-stock queries. Implement:

1. `check_low_stock(threshold)` — Return all products with stock below threshold
2. `get_stock_distribution()` — Return products grouped by stock level
3. Support for very large inventories (1M+ products) with fast queries
4. Background job to keep materialized view updated

### Example

```csharp
var inventory = new OptimizedInventoryManager();

inventory.add_stock("PROD-1", 5);
inventory.add_stock("PROD-2", 50);
inventory.add_stock("PROD-3", 100);

// Fast query: all products below 30 units
var low_stock = inventory.check_low_stock(30);
// Returns: [("PROD-1", 5), ("PROD-2", 50)]  -- PROD-2 is above threshold but still included? No, only below.

var distribution = inventory.get_stock_distribution();
// Returns: {
//   "CRITICAL" (0-10): [("PROD-1", 5)],
//   "LOW" (11-50): [("PROD-2", 50)],
//   "HEALTHY" (51+): [("PROD-3", 100)]
// }
```

### Solution

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

public class OptimizedInventoryManager
{
    private Dictionary<string, int> inventory = new();
    private SortedDictionary<int, HashSet<string>> stock_index = new(); // stock_level -> products

    public void add_stock(string product_id, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive");

        lock (inventory)
        {
            if (inventory.ContainsKey(product_id))
            {
                int old_stock = inventory[product_id];
                stock_index[old_stock].Remove(product_id);
                if (stock_index[old_stock].Count == 0)
                    stock_index.Remove(old_stock);
            }

            int new_stock = inventory.GetValueOrDefault(product_id, 0) + quantity;
            inventory[product_id] = new_stock;

            if (!stock_index.ContainsKey(new_stock))
                stock_index[new_stock] = new HashSet<string>();
            stock_index[new_stock].Add(product_id);
        }
    }

    public void remove_stock(string product_id, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive");

        lock (inventory)
        {
            if (!inventory.ContainsKey(product_id))
                throw new InvalidOperationException($"Product {product_id} not found");

            int current_stock = inventory[product_id];
            if (current_stock < quantity)
                throw new InvalidOperationException(
                    $"Insufficient stock. Available: {current_stock}, Requested: {quantity}");

            stock_index[current_stock].Remove(product_id);
            if (stock_index[current_stock].Count == 0)
                stock_index.Remove(current_stock);

            int new_stock = current_stock - quantity;
            inventory[product_id] = new_stock;

            if (!stock_index.ContainsKey(new_stock))
                stock_index[new_stock] = new HashSet<string>();
            stock_index[new_stock].Add(product_id);
        }
    }

    public List<(string, int)> check_low_stock(int threshold)
    {
        var result = new List<(string, int)>();

        lock (inventory)
        {
            foreach (var level in stock_index.Keys.Where(k => k < threshold))
            {
                foreach (var product_id in stock_index[level])
                {
                    result.Add((product_id, level));
                }
            }
        }

        return result;
    }

    public Dictionary<string, List<(string, int)>> get_stock_distribution()
    {
        var result = new Dictionary<string, List<(string, int)>>
        {
            { "CRITICAL", new() },  // 0-10
            { "LOW", new() },       // 11-50
            { "HEALTHY", new() }    // 51+
        };

        lock (inventory)
        {
            foreach (var (product_id, stock) in inventory)
            {
                string category = stock <= 10 ? "CRITICAL" : stock <= 50 ? "LOW" : "HEALTHY";
                result[category].Add((product_id, stock));
            }
        }

        return result;
    }

    public int check_availability(string product_id)
    {
        lock (inventory)
        {
            return inventory.GetValueOrDefault(product_id, -1);
        }
    }
}

// Test
class Program
{
    static void Main()
    {
        var inventory = new OptimizedInventoryManager();
        
        inventory.add_stock("PROD-1", 5);
        inventory.add_stock("PROD-2", 25);
        inventory.add_stock("PROD-3", 100);
        inventory.add_stock("PROD-4", 8);

        var low_stock = inventory.check_low_stock(30);
        Console.WriteLine("Low stock products (< 30):");
        foreach (var (product_id, stock) in low_stock)
        {
            Console.WriteLine($"  {product_id}: {stock}");
        }

        var distribution = inventory.get_stock_distribution();
        Console.WriteLine("\nStock distribution:");
        foreach (var (category, products) in distribution)
        {
            Console.WriteLine($"  {category}: {string.Join(", ", products.Select(p => $"{p.Item1}({p.Item2})"))}");
        }
    }
}
```

---

## Problem 3: Multi-Warehouse Inventory with Automatic Rebalancing

### Problem Statement

Implement a multi-warehouse inventory system where:

1. Each warehouse maintains separate stock
2. `find_nearest_warehouse(product_id, customer_location)` returns the warehouse with stock
3. `rebalance_inventory()` redistributes stock to minimize shortages
4. Rebalancing uses a distance matrix to minimize transport cost
5. Prioritize closer warehouses for fulfillment

### Example

```csharp
var warehouse_mgr = new MultiWarehouseInventory();

warehouse_mgr.add_warehouse("US-East", new(40.7128, -74.0060));    // NYC
warehouse_mgr.add_warehouse("US-West", new(34.0522, -118.2437));   // LA
warehouse_mgr.add_warehouse("EU-Central", new(52.5200, 13.4050));  // Berlin

warehouse_mgr.add_stock("US-East", "PROD-1", 100);
warehouse_mgr.add_stock("US-West", "PROD-1", 30);
warehouse_mgr.add_stock("EU-Central", "PROD-1", 50);

// Customer in New York
var best_warehouse = warehouse_mgr.find_nearest_warehouse("PROD-1", new(40.7128, -74.0060));
Console.WriteLine(best_warehouse); // "US-East"

// Remove stock from best warehouse
warehouse_mgr.remove_stock(best_warehouse, "PROD-1", 50);

// Rebalance when US-East gets low
warehouse_mgr.rebalance_inventory();
// Moves 20 units from US-West to US-East
// Moves 15 units from EU-Central to US-East
```

### Solution

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

public class MultiWarehouseInventory
{
    private struct Location
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public Location(double lat, double lon)
        {
            Latitude = lat;
            Longitude = lon;
        }

        public double distance_to(Location other)
        {
            // Haversine formula
            const double R = 6371; // Earth radius in km
            double lat_diff = (other.Latitude - Latitude) * Math.PI / 180;
            double lon_diff = (other.Longitude - Longitude) * Math.PI / 180;
            double a = Math.Sin(lat_diff / 2) * Math.Sin(lat_diff / 2) +
                       Math.Cos(Latitude * Math.PI / 180) * Math.Cos(other.Latitude * Math.PI / 180) *
                       Math.Sin(lon_diff / 2) * Math.Sin(lon_diff / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }
    }

    private Dictionary<string, Location> warehouses = new();
    private Dictionary<string, Dictionary<string, int>> warehouse_inventory = new();
    private readonly object lock_obj = new object();

    public void add_warehouse(string warehouse_id, Location location)
    {
        lock (lock_obj)
        {
            warehouses[warehouse_id] = location;
            warehouse_inventory[warehouse_id] = new();
        }
    }

    public void add_stock(string warehouse_id, string product_id, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive");

        lock (lock_obj)
        {
            if (!warehouses.ContainsKey(warehouse_id))
                throw new InvalidOperationException($"Warehouse {warehouse_id} not found");

            if (!warehouse_inventory[warehouse_id].ContainsKey(product_id))
                warehouse_inventory[warehouse_id][product_id] = 0;

            warehouse_inventory[warehouse_id][product_id] += quantity;
        }
    }

    public void remove_stock(string warehouse_id, string product_id, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive");

        lock (lock_obj)
        {
            if (!warehouse_inventory[warehouse_id].ContainsKey(product_id) ||
                warehouse_inventory[warehouse_id][product_id] < quantity)
                throw new InvalidOperationException($"Insufficient stock in {warehouse_id}");

            warehouse_inventory[warehouse_id][product_id] -= quantity;
        }
    }

    public string find_nearest_warehouse(string product_id, Location customer_location)
    {
        lock (lock_obj)
        {
            var available = warehouses.Keys
                .Where(w => warehouse_inventory[w].ContainsKey(product_id) &&
                            warehouse_inventory[w][product_id] > 0)
                .OrderBy(w => warehouses[w].distance_to(customer_location))
                .FirstOrDefault();

            return available ?? throw new InvalidOperationException($"Product {product_id} not available anywhere");
        }
    }

    public void rebalance_inventory()
    {
        lock (lock_obj)
        {
            // For each product, redistribute stock to minimize shortage
            var all_products = warehouse_inventory.Values
                .SelectMany(inv => inv.Keys)
                .Distinct()
                .ToList();

            foreach (var product_id in all_products)
            {
                int total_stock = warehouse_inventory.Values
                    .Sum(inv => inv.GetValueOrDefault(product_id, 0));

                if (total_stock == 0) continue;

                // Target: evenly distribute stock
                int target_per_warehouse = total_stock / warehouses.Count;

                // Collect excess, redistribute to shortage
                var excess = new Dictionary<string, int>();
                var shortage = new Dictionary<string, int>();

                foreach (var w_id in warehouses.Keys)
                {
                    int current = warehouse_inventory[w_id].GetValueOrDefault(product_id, 0);
                    if (current > target_per_warehouse)
                        excess[w_id] = current - target_per_warehouse;
                    else if (current < target_per_warehouse)
                        shortage[w_id] = target_per_warehouse - current;
                }

                // Rebalance: move excess to shortage
                foreach (var (shortage_warehouse, needed) in shortage)
                {
                    int remaining = needed;
                    foreach (var (excess_warehouse, available) in excess.OrderByDescending(e => e.Value))
                    {
                        if (remaining == 0) break;
                        int transfer = Math.Min(remaining, available);
                        warehouse_inventory[excess_warehouse][product_id] -= transfer;
                        if (!warehouse_inventory[shortage_warehouse].ContainsKey(product_id))
                            warehouse_inventory[shortage_warehouse][product_id] = 0;
                        warehouse_inventory[shortage_warehouse][product_id] += transfer;
                        excess[excess_warehouse] -= transfer;
                        remaining -= transfer;
                    }
                }
            }
        }
    }

    public Dictionary<string, int> get_stock(string product_id)
    {
        lock (lock_obj)
        {
            var result = new Dictionary<string, int>();
            foreach (var w_id in warehouses.Keys)
            {
                result[w_id] = warehouse_inventory[w_id].GetValueOrDefault(product_id, 0);
            }
            return result;
        }
    }
}

// Test
class Program
{
    static void Main()
    {
        var mgr = new MultiWarehouseInventory();
        
        mgr.add_warehouse("US-East", new(40.7128, -74.0060));
        mgr.add_warehouse("US-West", new(34.0522, -118.2437));

        mgr.add_stock("US-East", "PROD-1", 100);
        mgr.add_stock("US-West", "PROD-1", 20);

        var nearest = mgr.find_nearest_warehouse("PROD-1", new(40.7128, -74.0060));
        Console.WriteLine($"Nearest warehouse for NY customer: {nearest}"); // US-East

        mgr.remove_stock("US-East", "PROD-1", 50);

        var distribution = mgr.get_stock("PROD-1");
        Console.WriteLine("Distribution before rebalancing:");
        foreach (var (warehouse, stock) in distribution)
        {
            Console.WriteLine($"  {warehouse}: {stock}");
        }

        mgr.rebalance_inventory();

        distribution = mgr.get_stock("PROD-1");
        Console.WriteLine("Distribution after rebalancing:");
        foreach (var (warehouse, stock) in distribution)
        {
            Console.WriteLine($"  {warehouse}: {stock}");
        }
    }
}
```

---

## Summary of Coding Problems

| Problem | Key Concepts | Difficulty |
|---------|--------------|-----------|
| **Concurrent Stock Management** | Thread safety, locking, version tracking, error handling | Medium |
| **Low Stock Queries** | Indexed data structures, sorted collections, efficient filtering | Medium |
| **Multi-Warehouse Inventory** | Graph algorithms (shortest path), load balancing, distributed systems | Hard |

These problems test understanding of:
- Concurrency and race conditions
- Data structure design for efficient queries
- Distributed systems and geographic optimization
