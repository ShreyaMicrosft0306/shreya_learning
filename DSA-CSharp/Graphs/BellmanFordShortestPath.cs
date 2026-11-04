// ============================================================
// PROBLEM: Bellman-Ford Shortest Path
// ============================================================
// This is the negative-weight counterpart to single-source shortest-path
// problems such as Network Delay Time. Given a directed graph with
// possible negative edge weights, find the shortest distance from a
// source node to every other node.
//
// INPUT:
//   int n        — number of vertices, labeled 0 through n - 1
//   int[][] edges — directed edges [from, to, weight]
//   int source   — starting vertex
//
// OUTPUT:
//   long[] — minimum distance to each vertex, or int.MaxValue for unreachable
//   Negative cycle: return an indication that shortest paths are undefined
//
// EDGE CASES:
//   - Source equals every node reached with distance 0
//   - Disconnected vertices → int.MaxValue
//   - Negative edges → Bellman-Ford must update them over multiple passes
//   - Negative cycle reachable from source → shortest paths are undefined
//
// ============================================================
// HOW TO SOLVE — Simple Walkthrough
// ============================================================
// Key Insight:
//   Dijkstra is fast only when edge weights are non-negative. Bellman-Ford
//   instead relaxes every edge repeatedly. A simple path has at most n-1
//   edges, so n-1 passes are enough to find every shortest path.
//
// Relax an edge:
//   If distance[from] + weight < distance[to],
//   replace distance[to] with the smaller value.
//
// Step-by-step with example:
//
//   n = 4
//   source = 0
//   edges:
//     [0,1,5]
//     [0,2,4]
//     [1,3,-2]
//     [2,1,-3]
//
//   Initial distances: [0, INF, INF, INF]
//
//   Pass 1:
//     0→1: [0,5,INF,INF]
//     0→2: [0,5,4,INF]
//     2→1: [0,1,4,INF]  // 4 + (-3) improves 5
//     1→3: [0,1,4,-1]
//
//   Pass 2:
//     2→1 remains 1
//     1→3 remains -1
//
//   After at most n-1 passes:
//     Final distances should be [0,1,4,-1].
//
// Negative-cycle check:
//   After n-1 passes, scan every edge once more. If a distance can still
//   improve, at least one reachable negative cycle exists.
//
// Related LeetCode-style problem:
//   Bellman-Ford is most useful for negative-edge networks and is the
//   correct pattern for a LeetCode-style "find shortest paths with
//   negative weights" problem. Bellman-Ford is not usually the intended
//   solution for Network Delay Time or Cheapest Flights Within K Stops,
//   because those LeetCode problems provide non-negative edge weights.
//
// ============================================================
// TIME & SPACE COMPLEXITY
// ============================================================
//   n = number of vertices
//   E = number of directed edges
//
//   Time: O(n * E)
//     - Up to n - 1 passes over all E edges
//     - One additional pass detects negative cycles
//
//   Space: O(n)
//     - One distance value per vertex
// ============================================================

using System;

namespace DSA_CSharp.Graphs
{
    public class BellmanFordShortestPath
    {
        public const long Infinity = long.MaxValue;

        /// <summary>
        /// Finds shortest paths from source and detects a reachable negative cycle.
        /// </summary>
        /// <returns>
        /// A distance array and true when a reachable negative cycle exists.
        /// </returns>
        public (long[] Distances, bool HasNegativeCycle) Solve(
            int n,
            int[][] edges,
            int source)
        {
            if (n <= 0)
                return (Array.Empty<long>(), false);

            if (source < 0 || source >= n)
                throw new ArgumentOutOfRangeException(nameof(source));

            long[] distances = new long[n];
            Array.Fill(distances, Infinity);
            distances[source] = 0;

            for (int pass = 1; pass < n; pass++)
            {
                bool changed = false;

                foreach (int[] edge in edges)
                {
                    int from = edge[0];
                    int to = edge[1];
                    int weight = edge[2];

                    if (distances[from] == Infinity)
                        continue;

                    long candidate = distances[from] + weight;
                    if (candidate < distances[to])
                    {
                        distances[to] = candidate;
                        changed = true;
                    }
                }

                if (!changed)
                    break;
            }

            foreach (int[] edge in edges)
            {
                int from = edge[0];
                int to = edge[1];
                int weight = edge[2];

                if (distances[from] != Infinity &&
                    distances[from] + weight < distances[to])
                {
                    return (distances, true);
                }
            }

            return (distances, false);
        }

        public static void Main(string[] args)
        {
            var solution = new BellmanFordShortestPath();

            int[][] edges =
            {
                new[] { 0, 1, 5 },
                new[] { 0, 2, 4 },
                new[] { 1, 3, -2 },
                new[] { 2, 1, -3 }
            };

            var result = solution.Solve(4, edges, 0);
            Console.WriteLine(
                "[" + string.Join(", ", result.Distances) + "]");
            // Expected: [0, 1, 4, -1]

            int[][] negativeCycle =
            {
                new[] { 0, 1, 1 },
                new[] { 1, 2, -2 },
                new[] { 2, 1, 1 }
            };

            var cycleResult = solution.Solve(3, negativeCycle, 0);
            Console.WriteLine("Negative cycle: " + cycleResult.HasNegativeCycle);
            // Expected: true
        }
    }
}
