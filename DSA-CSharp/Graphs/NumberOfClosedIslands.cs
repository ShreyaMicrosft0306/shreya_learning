// ============================================================
// PROBLEM: Number of Closed Islands
// ============================================================
// Given a 2D grid of 0s and 1s, count islands that do NOT touch
// the border of the grid. An island touching any edge is "open"
// and must be excluded from the count.
//
// INPUT:
//   int[][] grid — 2D grid where 1 = land, 0 = water
//   Example: grid = [[1, 0, 0, 1],
//                    [1, 0, 1, 0],
//                    [0, 0, 1, 0],
//                    [0, 1, 0, 0]]
//
// OUTPUT:
//   int — number of islands fully enclosed (not touching any border)
//   Example: 1
//
// EDGE CASES:
//   - All border → 0 closed islands
//   - Single interior cell surrounded by 0s → 1 closed island
//   - Island touching only a corner → still touches border → excluded
//   - All zeros → 0 islands
//
// ============================================================
// HOW TO SOLVE — Simple Walkthrough
// ============================================================
// Key Insight (Boundary DFS):
//   Instead of asking "can this cell reach the border?" for every cell,
//   flip the question: start FROM the border and flood-fill everything
//   reachable. Whatever 1s remain after that flood-fill are interior islands.
//
//   Step 1: Run DFS from every 1 on the four edges — mark them all 0.
//           This wipes out every island that touches the border.
//   Step 2: Count remaining islands normally (same as CountingConnectedComponents).
//
// Step-by-step with example:
//   grid = [[1, 0, 0, 1],      Border 1s: (0,0),(0,3),(1,0),(3,1)
//            [1, 0, 1, 0],
//            [0, 0, 1, 0],
//            [0, 1, 0, 0]]
//
//   STEP 1 — Flood fill from border 1s:
//     DFS(0,0): marks (0,0) → down to (1,0) → marks (1,0) → done
//     DFS(0,3): marks (0,3) → no connected 1s → done
//     DFS(3,1): marks (3,1) → no connected 1s → done
//
//   Grid after step 1:
//     [[0, 0, 0, 0],
//      [0, 0, 1, 0],
//      [0, 0, 1, 0],
//      [0, 0, 0, 0]]
//
//   STEP 2 — Count remaining islands:
//     Scan finds (1,2)=1 → count=1, DFS marks (1,2) and (2,2)
//     No more 1s → return 1  ✓
//
// WHY FLIP THE QUESTION?
//   Checking "can cell X reach the border?" requires a DFS per cell → O((n*m)²).
//   Starting from the border and reaching inward: each cell visited at most once
//   across all DFS calls → O(n * m). Much faster.
//
// WHY THIS APPROACH?
//   Naive: for each 1, BFS/DFS to check if it touches the border → O((n*m)²).
//   Boundary DFS (this): two passes, each O(n * m) → O(n * m) total. Optimal.
//
// ============================================================
// TIME & SPACE COMPLEXITY
// ============================================================
//   n = number of rows,  m = number of columns
//
//   Time:  O(n * m)
//     - Border scan: O(n + m) to iterate the four edges
//     - DFS across all border flood fills visits each cell at most once: O(n * m)
//     - Interior scan + DFS visits each remaining cell at most once: O(n * m)
//     - Total: O(n * m)
//
//   Space: O(n * m)
//     - Recursion stack depth is at most n * m in the worst case
// ============================================================

public class NumberOfClosedIslands
{
    private static void Dfs(int[][] grid, int r, int c)
    {
        if (r < 0 || r >= grid.Length || c < 0 || c >= grid[0].Length || grid[r][c] != 1)
            return;

        grid[r][c] = 0;

        Dfs(grid, r + 1, c);
        Dfs(grid, r - 1, c);
        Dfs(grid, r, c + 1);
        Dfs(grid, r, c - 1);
    }

    public static int Solve(int[][] grid)
    {
        int rows = grid.Length;
        int cols = grid[0].Length;

        // Step 1: flood-fill all islands connected to the border
        for (int r = 0; r < rows; r++)
        {
            Dfs(grid, r, 0);          // left column
            Dfs(grid, r, cols - 1);   // right column
        }
        for (int c = 0; c < cols; c++)
        {
            Dfs(grid,pnm 0, c);          // top row
            Dfs(grid, rows - 1, c);   // bottom row
        }


        // Step 2: count remaining interior islands
        int count = 0;
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
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
            new[] { 1, 0, 0, 1 },
            new[] { 1, 0, 1, 0 },
            new[] { 0, 0, 1, 0 },
            new[] { 0, 1, 0, 0 }
        }));
        // Expected: 1

        Console.WriteLine("Example 2: " + Solve(new[]
        {
            new[] { 0, 0, 0, 0 },
            new[] { 0, 1, 1, 0 },
            new[] { 0, 1, 1, 0 },
            new[] { 0, 0, 0, 0 }
        }));
        // Expected: 1  (2x2 island fully enclosed)

        Console.WriteLine("Example 3: " + Solve(new[]
        {
            new[] { 0, 1, 0 },
            new[] { 1, 0, 1 },
            new[] { 0, 1, 0 }
        }));
        // Expected: 0  (all 1s touch the border)

        Console.WriteLine("Example 4: " + Solve(new[]
        {
            new[] { 0, 0, 0 },
            new[] { 0, 1, 0 },
            new[] { 0, 0, 0 }
        }));
        // Expected: 1  (single interior cell)
    }
}
