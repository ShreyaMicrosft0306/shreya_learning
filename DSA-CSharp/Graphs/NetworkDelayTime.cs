/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Network Delay Time
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
A distributed system has n servers labeled 1 to n. The servers communicate
through directed connections, where each connection [from, to, latency] means
server `from` can send a message to server `to` with the given latency
(in milliseconds).

When server k broadcasts an alert, it propagates through the network along ALL
available paths simultaneously. Each server receives the alert at the earliest
possible time (the shortest-latency path from k).

Return the MINIMUM time required for EVERY server to receive the alert. Since the
signal travels along all paths at once, the answer is the time the LAST server
receives it (the maximum of all shortest-path latencies). If some server is
unreachable from k, return -1.

INPUT:
- connections: int[][] - Array of [from, to, latency] directed edges (1-indexed)
- n: int - Number of servers (labeled 1 to n)
- k: int - The server that broadcasts the alert

OUTPUT:
- int - Minimum time for all servers to receive the signal, or -1 if any
         server is unreachable from k.

EXAMPLES:
Example 1:
Input: connections = [[1,2,3], [1,3,5], [2,3,1]], n = 3, k = 1
Output: 4
Explanation: There are two paths to server 3: the direct path 1→3 takes 5ms,
             but the path 1→2→3 takes only 3+1 = 4ms. Server 2 is reached in
             3ms, server 3 in 4ms. The last server (3) determines the answer: 4.

Example 2:
Input: connections = [[1,2,2], [3,2,1]], n = 3, k = 1
Output: -1
Explanation: The edge [3,2,1] points FROM server 3 TO server 2. Server 1 can
             reach server 2, but there is no path from server 1 to server 3,
             so server 3 never receives the alert.

CONSTRAINTS:
- 1 <= k <= n <= 100
- 1 <= connections.length <= 6000
- connections[i].length == 3
- 1 <= from_i, to_i <= n
- from_i != to_i (no self-loops)
- 0 <= latency_i <= 100
- All latencies are NON-NEGATIVE
- Edges are directed
- There may be multiple edges between the same pair of servers

EDGE CASES:
✓ Single server (n = 1): source is the only node, answer = 0
✓ Source has no outgoing edges but n > 1: unreachable servers → -1
✓ Disconnected graph: some servers never reached → -1
✓ Multiple edges between same pair: pick the shorter latency
✓ Cycles in the graph: handled naturally by Dijkstra
✓ Latency of 0 on some edges: still valid (non-negative)

TRICK CASES:
⚡ Servers are 1-INDEXED (1 to n), not 0-indexed. Size arrays as n+1 or offset.
⚡ Edges are DIRECTED — do NOT add the reverse edge.
⚡ The answer is the MAX of shortest distances, not the sum.
⚡ Must confirm EVERY server was reached; a single unreachable server → -1.
⚡ Use a priority queue for optimal O(E log V); plain scanning is slower.
⚡ Skip stale priority-queue entries (lazy deletion) to stay efficient.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OPTIMAL SOLUTION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APPROACH:
This is a single-source shortest-path problem on a directed graph with
non-negative weights — a textbook fit for Dijkstra's algorithm.

The signal from server k reaches every other server via the shortest-latency
path. Run Dijkstra from k to compute the shortest distance to every server.
The moment ALL servers have received the signal is the LATEST arrival time,
i.e. the maximum over all shortest distances. If any server remains at infinity
(unreachable), return -1.

KEY INSIGHTS:
• "Time for all to receive" = max of the shortest-path distances from k.
• Non-negative weights → Dijkstra is correct and efficient.
• Greedily expand the closest unvisited server, relaxing its neighbors.
• 1-indexed labels: offset indices or size arrays to n+1.

ALGORITHM STEPS:
1. Build a directed adjacency list from the connections.
2. Initialize dist[] to infinity, dist[k] = 0, push (0, k) to a min-heap.
3. While the heap is not empty:
   a. Pop the server with the smallest tentative distance.
   b. If it was already finalized, skip (stale entry).
   c. Finalize it; relax each outgoing edge and push improved neighbors.
4. Scan all servers: if any is still infinity, return -1.
5. Otherwise return the maximum finalized distance.

TIME COMPLEXITY: O(E log V)
- V = n servers, E = number of connections.
- Each edge triggers at most one heap push/pop: O(E log V).

SPACE COMPLEXITY: O(V + E)
- Adjacency list O(E), distance array O(V), heap O(E), visited O(V).

PATTERN: Graph, Dijkstra's Algorithm, Priority Queue, Shortest Path, Greedy
DIFFICULTY: Medium
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;
using System.Collections.Generic;

namespace DSA_CSharp.Graphs
{
    public class NetworkDelayTime
    {
        /// <summary>
        /// Returns the minimum time for all n servers to receive a signal
        /// broadcast from server k, or -1 if any server is unreachable.
        /// Uses Dijkstra's algorithm (non-negative latencies).
        /// </summary>
        /// <param name="connections">Directed edges [from, to, latency], 1-indexed.</param>
        /// <param name="n">Number of servers (labeled 1 to n).</param>
        /// <param name="k">Source server that broadcasts the alert.</param>
        /// <returns>Time for the last server to receive the signal, or -1.</returns>
        public int NetworkDelay(int[][] connections, int n, int k)
        {
            // ═══════════════════════════════════════════════════════════════
            // STEP 1: BUILD DIRECTED ADJACENCY LIST (1-indexed → size n + 1)
            // ═══════════════════════════════════════════════════════════════
            List<(int to, int latency)>[] adjacency = new List<(int, int)>[n + 1];
            for (int i = 1; i <= n; i++)
            {
                adjacency[i] = new List<(int, int)>();
            }

            foreach (var conn in connections)
            {
                int from = conn[0];
                int to = conn[1];
                int latency = conn[2];
                adjacency[from].Add((to, latency));
            }

            // ═══════════════════════════════════════════════════════════════
            // STEP 2: INITIALIZE DISTANCES, MIN-HEAP, AND VISITED SET
            // ═══════════════════════════════════════════════════════════════
            int[] dist = new int[n + 1];
            Array.Fill(dist, int.MaxValue);
            dist[k] = 0;

            // PriorityQueue<element, priority>: (server, currentDistance)
            var pq = new PriorityQueue<int, int>();
            pq.Enqueue(k, 0);

            bool[] visited = new bool[n + 1];

            // ═══════════════════════════════════════════════════════════════
            // STEP 3: DIJKSTRA'S MAIN LOOP
            // ═══════════════════════════════════════════════════════════════
            while (pq.Count > 0)
            {
                int server = pq.Dequeue();

                // Skip stale entries already finalized with a better distance.
                if (visited[server])
                    continue;

                visited[server] = true;

                // Relaxation: try to improve distance to each neighbor.
                foreach (var (to, latency) in adjacency[server])
                {
                    if (visited[to])
                        continue;

                    int newDistance = dist[server] + latency;
                    if (newDistance < dist[to])
                    {
                        dist[to] = newDistance;
                        pq.Enqueue(to, newDistance);
                    }
                }
            }

            // ═══════════════════════════════════════════════════════════════
            // STEP 4: ANSWER = MAX SHORTEST DISTANCE, OR -1 IF ANY UNREACHABLE
            // ═══════════════════════════════════════════════════════════════
            int maxTime = 0;
            for (int i = 1; i <= n; i++)
            {
                if (dist[i] == int.MaxValue)
                    return -1;   // Server i never received the signal.

                maxTime = Math.Max(maxTime, dist[i]);
            }

            return maxTime;
        }
    }
}
