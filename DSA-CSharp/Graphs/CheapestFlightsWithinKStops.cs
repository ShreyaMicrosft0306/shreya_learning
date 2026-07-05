/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Cheapest Flights Within K Stops
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
There are n cities connected by m flights. Each flight starts from city u and 
arrives at city v with a price w.

You are given all the cities, flights, a starting city src, a destination city dst,
and a number k. Your task is to find the CHEAPEST price from src to dst with at 
most k stops (intermediate cities). If there is no such route, return -1.

Note: The number of stops is the number of intermediate cities, NOT including 
the source and destination. A direct flight from src to dst has 0 stops.

INPUT:
- n: int - Number of cities (labeled 0 to n-1)
- flights: int[][] - Array of [from, to, price] directed edges (0-indexed)
- src: int - Source city
- dst: int - Destination city
- k: int - Maximum number of stops allowed (intermediate cities)

OUTPUT:
- int - Minimum cost from src to dst with at most k stops, or -1 if impossible.

EXAMPLES:
Example 1:
Input: n = 4, flights = [[0,1,100],[1,2,100],[2,0,100],[1,3,600],[2,3,200]], 
       src = 0, dst = 3, k = 1
Output: 700
Explanation: 
The optimal path is 0 → 1 → 3 with price 100 + 600 = 700.
The path 0 → 1 → 2 → 3 would be cheaper (100 + 100 + 200 = 400) but has 2 stops,
exceeding the limit of k = 1.

Example 2:
Input: n = 3, flights = [[0,1,100],[1,2,100],[0,2,500]], src = 0, dst = 2, k = 1
Output: 200
Explanation: 
The path 0 → 1 → 2 costs 200 with 1 stop.
The direct path 0 → 2 costs 500 with 0 stops.
The cheaper option is 200.

Example 3:
Input: n = 3, flights = [[0,1,100],[1,2,100],[0,2,500]], src = 0, dst = 2, k = 0
Output: 500
Explanation: 
With k = 0 stops, we can only take direct flights.
The only direct flight from 0 to 2 costs 500.

CONSTRAINTS:
- 1 <= n <= 100
- 0 <= flights.length <= (n * (n - 1) / 2)
- flights[i].length == 3
- 0 <= from_i, to_i < n
- from_i != to_i (no self-loops)
- 1 <= price_i <= 10^4
- There may be multiple flights between the same pair of cities
- 0 <= src, dst, k < n
- src != dst

EDGE CASES:
✓ No path exists from src to dst → return -1
✓ Direct flight exists and k = 0 → must use direct flight
✓ k = 0 but no direct flight → return -1
✓ Multiple flights between same cities → consider all options
✓ k is very large (k >= n-2) → effectively unconstrained shortest path
✓ src == dst → should not happen per constraints, but would be 0
✓ Graph with cycles → handled naturally by stop limit

TRICK CASES:
⚡ Cities are 0-INDEXED (0 to n-1), not 1-indexed.
⚡ k represents STOPS (intermediate cities), not total edges or cities visited.
⚡ A path with HIGHER cost but FEWER stops might lead to cheaper final solution.
⚡ Standard Dijkstra alone fails because we can't always prune by cost alone.
⚡ Use modified Dijkstra/BFS that tracks (city, cost, stops) tuples.
⚡ Keep track of best cost to reach each city with each number of stops.
⚡ Multiple flights between same cities → all should be in adjacency list.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OPTIMAL SOLUTION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APPROACH:
This is a constrained shortest-path problem where we need to find the minimum 
cost path from src to dst with at most k intermediate stops.

We use a MODIFIED DIJKSTRA approach with a priority queue where each state is
(cost, city, stops). The key difference from standard Dijkstra:
• We CANNOT discard a state just because we've seen that city with lower cost.
• A more expensive path with fewer stops might enable cheaper extensions later.
• We track the minimum cost to reach each city for each number of stops used.

KEY INSIGHTS:
• Priority queue ordered by cost (greedy: explore cheaper paths first).
• State = (cost, city, stops), where stops is the number used so far.
• Track best[city][stops] to avoid redundant exploration.
• When we pop dst from the queue, we have the answer.
• If dst is never popped or popped cost exceeds limit, return -1.

ALGORITHM STEPS:
1. Build directed adjacency list from flights.
2. Initialize a min-heap with (0, src, 0) = (cost, city, stops).
3. Track best[city][stops] = minimum cost to reach city using exactly stops.
4. While heap is not empty:
   a. Pop (cost, city, stops).
   b. If city == dst, return cost immediately (greedy guarantee).
   c. If stops > k, skip (too many stops to continue).
   d. If this state is stale (better cost already known), skip.
   e. For each neighbor, compute newCost = cost + price.
   f. If newCost improves best[neighbor][stops+1], push to heap.
5. If dst never reached, return -1.

WHY THIS WORKS:
• Priority queue ensures we explore cheaper paths first.
• Tracking best[city][stops] prevents redundant exploration without losing optimality.
• Unlike pure Dijkstra, we allow revisiting cities if stops are different.
• The constraint k+1 <= n ensures bounded exploration.

TIME COMPLEXITY: O(E * k * log(E * k))
- E = number of flights, k = max stops.
- Each flight can be explored up to k+1 times (one per stop count).
- Each heap operation is O(log(heap size)) ≈ O(log(E * k)).

SPACE COMPLEXITY: O(n * k + E)
- best[][] array: O(n * k)
- Adjacency list: O(E)
- Priority queue: O(E * k) worst case

ALTERNATIVE APPROACHES:
1. Bellman-Ford with k+1 iterations: O(k * E), simpler but possibly slower.
2. BFS with stop-based levels: O(k * E), good when costs are similar.
3. Dynamic Programming: dp[stops][city] = min cost, O(k * E).

PATTERN: Graph, Modified Dijkstra, Priority Queue, Constrained Shortest Path
DIFFICULTY: Medium
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;
using System.Collections.Generic;

namespace DSA_CSharp.Graphs
{
    public class CheapestFlightsWithinKStops
    {
        /// <summary>
        /// Finds the cheapest price from src to dst with at most k stops.
        /// Uses modified Dijkstra's algorithm with stop constraints.
        /// </summary>
        /// <param name="n">Number of cities (0 to n-1).</param>
        /// <param name="flights">Directed edges [from, to, price], 0-indexed.</param>
        /// <param name="src">Source city.</param>
        /// <param name="dst">Destination city.</param>
        /// <param name="k">Maximum number of intermediate stops allowed.</param>
        /// <returns>Minimum cost, or -1 if no valid route exists.</returns>
        public int FindCheapestPrice(int n, int[][] flights, int src, int dst, int k)
        {
            // ═══════════════════════════════════════════════════════════════
            // STEP 1: BUILD DIRECTED ADJACENCY LIST (0-indexed)
            // ═══════════════════════════════════════════════════════════════
            List<(int to, int price)>[] adjacency = new List<(int, int)>[n];
            for (int i = 0; i < n; i++)
            {
                adjacency[i] = new List<(int, int)>();
            }

            foreach (var flight in flights)
            {
                int from = flight[0];
                int to = flight[1];
                int price = flight[2];
                adjacency[from].Add((to, price));
            }

            // ═══════════════════════════════════════════════════════════════
            // STEP 2: INITIALIZE MIN-HEAP AND TRACKING ARRAY
            // ═══════════════════════════════════════════════════════════════
            // PriorityQueue<element, priority>: ((city, stops), cost)
            var pq = new PriorityQueue<(int city, int stops), int>();
            pq.Enqueue((src, 0), 0);

            // best[city][stops] = minimum cost to reach city using exactly stops.
            // We track up to k+1 stops (0, 1, ..., k stops used to reach intermediate cities).
            int[][] best = new int[n][];
            for (int i = 0; i < n; i++)
            {
                best[i] = new int[k + 2];
                Array.Fill(best[i], int.MaxValue);
            }
            best[src][0] = 0;

            // ═══════════════════════════════════════════════════════════════
            // STEP 3: MODIFIED DIJKSTRA WITH STOP CONSTRAINT
            // ═══════════════════════════════════════════════════════════════
            while (pq.Count > 0)
            {
                pq.TryDequeue(out var state, out int cost);
                int city = state.city;
                int stops = state.stops;

                // If we reached the destination, return the cost immediately.
                // Priority queue guarantees this is the minimum cost path.
                if (city == dst)
                    return cost;

                // If we've used k stops to get here, we can still go one more edge
                // to reach dst, but we cannot explore further intermediate cities.
                if (stops > k)
                    continue;

                // Skip stale states: if we've already found a better way to reach
                // this city with this many stops, ignore this entry.
                if (cost > best[city][stops])
                    continue;

                // ═══════════════════════════════════════════════════════════════
                // RELAXATION: EXPLORE ALL NEIGHBORING CITIES
                // ═══════════════════════════════════════════════════════════════
                foreach (var (neighbor, price) in adjacency[city])
                {
                    int newCost = cost + price;
                    int newStops = stops + 1;

                    // Only process if newStops is within bounds (0 to k+1).
                    // newStops represents the number of stops used to reach neighbor.
                    if (newStops <= k + 1 && newCost < best[neighbor][newStops])
                    {
                        best[neighbor][newStops] = newCost;
                        pq.Enqueue((neighbor, newStops), newCost);
                    }
                }
            }

            // ═══════════════════════════════════════════════════════════════
            // STEP 4: NO VALID ROUTE FOUND
            // ═══════════════════════════════════════════════════════════════
            return -1;
        }
    }
}
