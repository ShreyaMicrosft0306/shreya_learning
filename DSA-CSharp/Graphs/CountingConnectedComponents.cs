// ============================================================
// PROBLEM: Counting Connected Components
// ============================================================
// Given a 2D grid of 0s and 1s, count the number of distinct islands.
// An island is a group of 1s connected horizontally or vertically.
//
// INPUT:
//   int[][] grid — 2D grid where 1 = land, 0 = water
//   Example: grid = [[0,1,0,0,0,0],
//                    [0,1,1,0,1,0],
//                    [0,0,0,1,1,0],
//                    [1,1,0,0,0,1],
//                    [1,1,0,0,0,1]]
//
// OUTPUT:
//   int — number of distinct islands
//   Example: 4
//
// EDGE CASES:
//   - All zeros → 0 islands
//   - All ones  → 1 island (entire grid is connected)
//   - Single cell [1] → 1 island
//   - Single cell [0] → 0 islands
//
// ============================================================
// HOW TO SOLVE — Simple Walkthrough
// ============================================================
// Key Insight (DFS Flood Fill):
//   Scan every cell. When a 1 is found, a new island is discovered.
//   Immediately flood-fill the entire island using DFS — mark every
//   connected 1 as visited (set to 0) so it is never counted again.
//   Count how many times a new flood fill had to be started.
//
// Step-by-step with example (rows 0-4, cols 0-5):
//
//   Scan (0,0)=0 skip.  Scan (0,1)=1 → count=1, start DFS:
//     DFS(0,1): mark 0. Explore neighbours:
//       up=OOB, down=(1,1)=1→DFS(1,1), left=(0,0)=0, right=(0,2)=0
//       DFS(1,1): mark 0. Explore neighbours:
//         up=(0,1)=0, down=(2,1)=0, left=(1,0)=0, right=(1,2)=1→DFS(1,2)
//         DFS(1,2): mark 0. All neighbours 0 or OOB → return
//       return
//     Island 1 fully marked: (0,1)(1,1)(1,2) all set to 0
//
//   Continue scan... (1,4)=1 → count=2, DFS floods (1,4)(2,3)(2,4).
//   Continue scan... (3,0)=1 → count=3, DFS floods (3,0)(3,1)(4,0)(4,1).
//   Continue scan... (3,5)=1 → count=4, DFS floods (3,5)(4,5).
//
//   No more 1s found → return 4  ✓
//
// WHY MARK AS 0 DURING DFS?
//   Avoids a separate visited array. Once a cell is part of a counted
//   island, setting it to 0 guarantees the outer scan skips it.
//   This is safe when modifying the input is acceptable.
//
// WHY THIS APPROACH?
//   Naive: for every 1, scan all cells to find connected ones → O((n*m)²). Too slow.
//   DFS flood fill (this): each cell is visited at most twice
//   (once by the outer scan, once by DFS) → O(n * m). Optimal.
//
// ============================================================
// TIME & SPACE COMPLEXITY
// ============================================================
//   n = number of rows,  m = number of columns
//
//   Time:  O(n * m)
//     - Outer scan visits every cell once: O(n * m)
//     - DFS across all calls visits each cell at most once total: O(n * m)
//     - Total: O(n * m)
//
//   Space: O(n * m)
//     - Recursion stack depth is at most n * m in the worst case
//       (e.g. one giant snake-shaped island spanning the whole grid)
// ============================================================

public class CountingConnectedComponents
{
    private static void Dfs(int[][] grid, int r, int c)
    {
        if (r < 0 || r >= grid.Length || c < 0 || c >= grid[0].Length || grid[r][c] != 1)
            return;

        grid[r][c] = 0;  // mark visited

        Dfs(grid, r + 1, c);
        Dfs(grid, r - 1, c);
        Dfs(grid, r, c + 1);
        Dfs(grid, r, c - 1);f
    }

    public static int Solve(int[][] grid)
    {
        int count = 0;
        for (int r = 0; r < grid.Length; r++)
            for (int c = 0; c < grid[0].Length; c++)
                if (grid[r][c] == 1)
                {
                    count++;
                    Dfs(grid, r, c);
                }
        return count;
    }

    public static void Main(string[] args)
    {
        Console.WriteLine("Example 1: " + Solve(new[]
        {
            new[] { 0, 1, 0, 0, 0, 0 },
            new[] { 0, 1, 1, 0, 1, 0 },
            new[] { 0, 0, 0, 1, 1, 0 },
            new[] { 1, 1, 0, 0, 0, 1 },
            new[] { 1, 1, 0, 0, 0, 1 }
        }));
        // Expected: 4

        Console.WriteLine("Example 2: " + Solve(new[]
        {
            new[] { 1, 1, 0 },
            new[] { 1, 0, 0 },
            new[] { 0, 0, 1 }
        }));
        // Expected: 2

        Console.WriteLine("Example 3: " + Solve(new[]
        {
            new[] { 0, 0, 0 },
            new[] { 0, 0, 0 }
        }));
        // Expected: 0

        Console.WriteLine("Example 4: " + Solve(new[]
        {
            new[] { 1, 1 },
            new[] { 1, 1 }
        }));
        // Expected: 1
    }
}
