/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Path With Minimum Effort
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
A hiking app needs to find the easiest route across terrain. Given a 2D grid 
where each cell represents the elevation at that point, find the path from the 
top-left corner (0, 0) to the bottom-right corner (rows-1, cols-1) that minimizes 
the "effort".

The EFFORT of moving between two adjacent cells is the absolute difference in 
their elevations. The EFFORT OF A PATH is its single hardest step, meaning the 
MAXIMUM effort among all the moves you make along the way.

You can move up, down, left, or right from any cell (4-directional). Return the 
minimum possible effort for any valid path.

INPUT:
- heights: int[][] - 2D grid where heights[i][j] is the elevation at cell (i, j)

OUTPUT:
- int - Minimum effort required to reach the bottom-right corner from top-left

EXAMPLES:
Example 1:
Input: heights = [[1,10,2],[2,3,3],[3,2,1]]
Output: 1
Explanation: 
Grid visualization:
  1  10   2
  2   3   3
  3   2   1

The cliff (10) has a huge elevation difference.
Path with minimum effort: 1→2→3→2→1
- 1→2: effort = |1-2| = 1
- 2→3: effort = |2-3| = 1  
- 3→2: effort = |3-2| = 1
- 2→1: effort = |2-1| = 1
Maximum effort along this path = 1

Example 2:
Input: heights = [[4,3,2],[2,6,3],[3,2,1]]
Output: 2
Explanation:
Grid visualization:
  4   3   2
  2   6   3
  3   2   1

The center peak (6) would require effort 3 or 4 to cross.
Going around the top: 4→3→2→3→1
- 4→3: effort = 1
- 3→2: effort = 1
- 2→3: effort = 1
- 3→1: effort = 2
Maximum effort = 2

Example 3:
Input: heights = [[1,2,2],[3,8,2],[5,3,5]]
Output: 2
Explanation: The path 1→2→2→2→5 has maximum effort of 3 (from 2 to 5).
But the path 1→3→5→3→5 has maximum effort of 2.

CONSTRAINTS:
- rows == heights.length
- cols == heights[i].length
- 1 <= rows, cols <= 100
- 1 <= heights[i][j] <= 10^6
- Start is always at (0, 0), destination is always at (rows-1, cols-1)

EDGE CASES:
✓ Single cell (1x1 grid) → effort is 0 (already at destination)
✓ All cells have same elevation → effort is 0
✓ Large elevation differences → need to find alternative paths
✓ Multiple paths with same minimum effort → any is valid
✓ Grid with bottlenecks → must pass through high-effort cells

TRICK CASES:
⚡ Effort is the MAXIMUM step along a path, not the sum or average.
⚡ Similar to finding the "narrowest" bottleneck in a path.
⚡ Cannot use standard Dijkstra summing weights; need modified version.
⚡ Priority queue should order by the maximum effort encountered so far.
⚡ Once we reach destination via priority queue, that's the optimal answer.
⚡ Can also solve with binary search + BFS on effort threshold.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OPTIMAL SOLUTION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APPROACH:
This is a shortest-path problem with a twist: instead of minimizing the SUM of 
edge weights, we minimize the MAXIMUM edge weight along a path. This is known 
as the "minimax path" or "bottleneck shortest path" problem.

We use a MODIFIED DIJKSTRA approach where:
• State: (row, col, maxEffortSoFar)
• Priority queue ordered by effort (explore paths with lower max effort first)
• Track the minimum maximum-effort to reach each cell
• When we pop the destination cell, that effort is the answer

KEY INSIGHTS:
• Effort metric is the max edge weight, not the sum.
• Standard Dijkstra structure works with modified relaxation logic.
• When relaxing an edge, the new effort = max(currentEffort, edgeWeight).
• First time we reach destination via priority queue guarantees optimality.
• No need to track visited set; rely on effort comparison for pruning.

ALGORITHM STEPS:
1. Handle edge case: if grid is 1x1, return 0 (already at destination).
2. Initialize priority queue with (0, 0, 0) = (row, col, effort).
3. Track minEffort[row][col] = minimum effort to reach each cell.
4. While queue is not empty:
   a. Pop cell with smallest effort.
   b. If we reached destination, return effort immediately.
   c. If effort is stale (worse than recorded), skip.
   d. Explore 4 neighbors (up, down, left, right):
      - Calculate effort to move = abs(currentHeight - neighborHeight).
      - New total effort = max(currentEffort, moveEffort).
      - If this improves minEffort[neighbor], update and push to queue.
5. Return minEffort[destination] (should never reach if connected).

WHY THIS WORKS:
• Priority queue ensures we explore paths with lower max effort first.
• Greedy selection: first path to reach destination is optimal.
• Tracking minEffort prevents redundant exploration of worse paths.
• The max() operation correctly tracks the bottleneck along the path.

TIME COMPLEXITY: O(rows × cols × log(rows × cols))
- Each cell can be processed at most once effectively.
- Priority queue operations: O(log(size)) per operation.
- Total cells: rows × cols.

SPACE COMPLEXITY: O(rows × cols)
- minEffort array: O(rows × cols)
- Priority queue: O(rows × cols) worst case

ALTERNATIVE APPROACHES:
1. Binary Search + BFS: O(rows × cols × log(max_height))
   - Binary search on the answer (effort threshold).
   - For each threshold, BFS to check if path exists.
2. Union Find + Sorting: O(rows × cols × log(rows × cols))
   - Sort all edges by weight, union cells until start/end connected.
3. Bellman-Ford variant: O(rows × cols × (rows × cols))
   - Less efficient but conceptually similar.

PATTERN: Graph, Modified Dijkstra, Priority Queue, Minimax Path, Greedy
DIFFICULTY: Medium
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;
using System.Collections.Generic;

namespace DSA_CSharp.Graphs
{
    public class PathWithMinimumEffort
    {
        /// <summary>
        /// Finds the minimum effort required to travel from top-left to bottom-right,
        /// where effort is the maximum elevation difference along the path.
        /// Uses modified Dijkstra's algorithm for minimax path.
        /// </summary>
        /// <param name="heights">2D grid of elevations.</param>
        /// <returns>Minimum effort required for any valid path.</returns>
        public int MinimumEffortPath(int[][] heights)
        {
            int rows = heights.Length;
            int cols = heights[0].Length;

            // ═══════════════════════════════════════════════════════════════
            // EDGE CASE: Already at destination
            // ═══════════════════════════════════════════════════════════════
            if (rows == 1 && cols == 1)
                return 0;

            // ═══════════════════════════════════════════════════════════════
            // STEP 1: INITIALIZE DATA STRUCTURES
            // ═══════════════════════════════════════════════════════════════
            // Track minimum effort to reach each cell.
            int[][] minEffort = new int[rows][];
            for (int i = 0; i < rows; i++)
            {
                minEffort[i] = new int[cols];
                Array.Fill(minEffort[i], int.MaxValue);
            }
            minEffort[0][0] = 0;

            // Priority queue: (effort, row, col) ordered by effort.
            var pq = new PriorityQueue<(int row, int col), int>();
            pq.Enqueue((0, 0), 0);

            // 4 directions: up, down, left, right.
            int[] deltaRow = { -1, 1, 0, 0 };
            int[] deltaCol = { 0, 0, -1, 1 };

            // ═══════════════════════════════════════════════════════════════
            // STEP 2: MODIFIED DIJKSTRA - EXPLORE PATHS BY MINIMUM MAX EFFORT
            // ═══════════════════════════════════════════════════════════════
            while (pq.Count > 0)
            {
                pq.TryDequeue(out var cell, out int currentEffort);
                int row = cell.row;
                int col = cell.col;

                // If we reached the destination, this is the optimal effort.
                // Priority queue guarantees this is the minimum.
                if (row == rows - 1 && col == cols - 1)
                    return currentEffort;

                // Skip stale entries: if we've already found a better path to
                // this cell, ignore this outdated entry.
                if (currentEffort > minEffort[row][col])
                    continue;

                // ═══════════════════════════════════════════════════════════════
                // RELAXATION: EXPLORE ALL 4 NEIGHBORS
                // ═══════════════════════════════════════════════════════════════
                for (int d = 0; d < 4; d++)
                {
                    int newRow = row + deltaRow[d];
                    int newCol = col + deltaCol[d];

                    // Check bounds.
                    if (newRow < 0 || newRow >= rows || newCol < 0 || newCol >= cols)
                        continue;

                    // Calculate effort to move to this neighbor.
                    int moveEffort = Math.Abs(heights[row][col] - heights[newRow][newCol]);

                    // The effort of the path through this cell is the MAXIMUM
                    // of the current path's effort and this move's effort.
                    int newEffort = Math.Max(currentEffort, moveEffort);

                    // If this path to the neighbor is better, update and enqueue.
                    if (newEffort < minEffort[newRow][newCol])
                    {
                        minEffort[newRow][newCol] = newEffort;
                        pq.Enqueue((newRow, newCol), newEffort);
                    }
                }
            }

            // ═══════════════════════════════════════════════════════════════
            // STEP 3: RETURN RESULT (should have returned earlier)
            // ═══════════════════════════════════════════════════════════════
            // If we exit the loop without reaching destination, return the
            // recorded minimum effort (though this shouldn't happen in valid input).
            return minEffort[rows - 1][cols - 1];
        }
    }
}
