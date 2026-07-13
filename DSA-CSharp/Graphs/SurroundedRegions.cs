/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Surrounded Regions
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
Given an m x n matrix grid containing only characters 'X' and 'O', modify grid 
to replace all regions of 'O' that are completely surrounded by 'X' with 'X'.

A region of 'O' is surrounded by 'X' if there is no adjacent path (cells that 
border each other in the N, W, E, S directions) consisting of only 'O' from 
anywhere inside that region to the border of the board.

EXAMPLES:
Example 1: Simple Surrounded Cluster
Input:
[
  ['X','X','X','X','X'],
  ['X','O','O','O','X'],
  ['X','O','X','O','X'],
  ['X','O','O','O','X'],
  ['X','X','X','X','X']
]
Output:
[
  ['X','X','X','X','X'],
  ['X','X','X','X','X'],
  ['X','X','X','X','X'],
  ['X','X','X','X','X'],
  ['X','X','X','X','X']
]
Explanation: All O's are completely surrounded by X's (not touching borders).
Therefore, ALL O's flip to X.

Example 2: Multiple Isolated Clusters
Input:
[
  ['X','X','X','X','X','X','X'],
  ['X','O','O','X','X','X','X'],
  ['X','O','O','X','X','X','X'],
  ['X','X','X','X','O','O','X'],
  ['X','X','X','X','O','O','X'],
  ['X','X','X','X','X','X','X']
]
Output:
[
  ['X','X','X','X','X','X','X'],
  ['X','X','X','X','X','X','X'],
  ['X','X','X','X','X','X','X'],
  ['X','X','X','X','X','X','X'],
  ['X','X','X','X','X','X','X'],
  ['X','X','X','X','X','X','X']
]
Explanation: Two separate clusters of O's, both surrounded by X's.
Both clusters flip to X.

Example 3: Mixed - Surrounded vs Border-Connected
Input:
[
  ['X','X','X','X','X','X'],
  ['X','O','O','X','O','X'],
  ['X','O','X','X','O','X'],
  ['X','X','X','X','O','O']
]
Output:
[
  ['X','X','X','X','X','X'],
  ['X','X','X','X','O','X'],
  ['X','X','X','X','O','X'],
  ['X','X','X','X','O','O']
]
Explanation: 
- LEFT cluster at [1-2, 1-2]: Isolated from border → FLIP to X
- RIGHT cluster at [1-3, 4-5]: Connected to border O at [3,5] → KEEP as O
One border O protects the entire connected chain!

Example 4: Original Problem Example
Input:
[
  ['X','X','X','X','O'],
  ['X','X','O','X','X'],
  ['X','X','X','X','O'],
  ['X','O','X','X','X'],
  ['X','O','X','X','X']
]
Output:
[
  ['X','X','X','X','O'],
  ['X','X','X','X','X'],
  ['X','X','X','X','O'],
  ['X','O','X','X','X'],
  ['X','O','X','X','X']
]
Explanation:
- O's at [0,4] and [2,4]: Touch RIGHT border → KEEP
- O's at [3,1] and [4,1]: Touch LEFT border → KEEP
- O at [1,2]: Isolated, no border connection → FLIP to X

CONSTRAINTS:
- m == grid.length
- n == grid[i].length
- 1 <= m, n <= 200
- grid[i][j] is 'X' or 'O'

EDGE CASES:
✓ Single cell → no change needed
✓ All 'X' → no change
✓ All 'O' → no change (all connected to border)
✓ Border cells with 'O' → never flipped (not surrounded)
✓ 'O' region connected to border → entire region preserved
✓ Multiple disconnected 'O' regions → some flipped, some not
✓ Large grid (200x200)

TRICK CASES:
⚡ Don't try to identify "surrounded" regions directly (complex)
⚡ Use REVERSE thinking: Find regions NOT surrounded, then flip rest
⚡ 'O' on border = automatically NOT surrounded
⚡ Any 'O' connected to border 'O' = also NOT surrounded
⚡ Need DFS/BFS from ALL border 'O' cells
⚡ Use temporary marker (like 'T') to mark safe 'O' cells

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OPTIMAL SOLUTION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APPROACH:
Instead of finding surrounded regions (hard), use REVERSE THINKING:
1. Find all 'O' regions that are NOT surrounded (connected to borders)
2. Mark them as safe
3. Flip all remaining 'O' to 'X' (these are the surrounded ones)
4. Restore safe 'O' cells

This is similar to Pacific Atlantic Water Flow - reverse thinking optimization!

KEY INSIGHTS:
• 'O' on border cannot be surrounded
• Any 'O' connected to border 'O' cannot be surrounded
• Start DFS/BFS from all border 'O' cells
• Mark all reachable 'O' cells as "safe" (temporary marker)
• Flip remaining 'O' to 'X'
• Restore safe markers back to 'O'

ALGORITHM STEPS:
1. Handle edge cases: if grid is empty, return
2. Iterate through all border cells:
   - If cell is 'O', start DFS to mark connected region as safe
   - Use temporary marker 'T' (or any non-X, non-O character)
3. Iterate through entire grid:
   - If cell is 'O': flip to 'X' (it's surrounded)
   - If cell is 'T': restore to 'O' (it's safe)
   - If cell is 'X': keep as 'X'
4. Grid is now modified in-place

TIME COMPLEXITY: O(m × n)
- Border traversal: O(m + n)
- DFS from border 'O' cells: visits each cell at most once = O(m × n)
- Final grid traversal: O(m × n)
- Total: O(m × n)

SPACE COMPLEXITY: O(m × n)
- DFS recursion stack: O(m × n) in worst case
- No extra data structures needed (modify in-place)
- Total: O(m × n) for recursion

PATTERN: Graph DFS/BFS, Reverse Thinking, In-Place Modification
DIFFICULTY: Medium
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;

namespace DSA_CSharp.Graphs
{
    public class SurroundedRegions
    {
        private int rows;
        private int cols;
        private readonly int[][] directions = new int[][] 
        { 
            new int[] {0, 1},   // Right
            new int[] {1, 0},   // Down
            new int[] {0, -1},  // Left
            new int[] {-1, 0}   // Up
        };
        
        /// <summary>
        /// Captures all regions surrounded by 'X' by flipping 'O' to 'X'.
        /// Modifies the grid in-place using reverse thinking approach.
        /// </summary>
        /// <param name="grid">Grid containing 'X' and 'O'</param>
        public void Solve(char[][] grid)
        {
            if (grid == null || grid.Length == 0 || grid[0].Length == 0)
                return;
            
            rows = grid.Length;
            cols = grid[0].Length;
            
            // Step 1: Mark all 'O' cells connected to borders as 'T' (temporary safe)
            
            // Check top and bottom borders
            for (int col = 0; col < cols; col++)
            {
                if (grid[0][col] == 'O')
                    DFS(grid, 0, col);           // Top border
                if (grid[rows - 1][col] == 'O')
                    DFS(grid, rows - 1, col);    // Bottom border
            }
            
            // Check left and right borders
            for (int row = 0; row < rows; row++)
            {
                if (grid[row][0] == 'O')
                    DFS(grid, row, 0);           // Left border
                if (grid[row][cols - 1] == 'O')
                    DFS(grid, row, cols - 1);    // Right border
            }
            
            // Step 2: Flip all remaining 'O' to 'X' and restore 'T' to 'O'
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    if (grid[row][col] == 'O')
                        grid[row][col] = 'X';    // Surrounded 'O' -> flip to 'X'
                    else if (grid[row][col] == 'T')
                        grid[row][col] = 'O';    // Safe 'O' -> restore from 'T'
                }
            }
        }
        
        /// <summary>
        /// DFS to mark all 'O' cells connected to current cell as safe ('T').
        /// </summary>
        private void DFS(char[][] grid, int row, int col)
        {
            // Out of bounds or not 'O'
            if (row < 0 || row >= rows || col < 0 || col >= cols || grid[row][col] != 'O')
                return;
            
            // Mark as safe (temporary marker)
            grid[row][col] = 'T';
            
            // Explore all 4 directions
            foreach (var dir in directions)
            {
                int newRow = row + dir[0];
                int newCol = col + dir[1];
                DFS(grid, newRow, newCol);
            }
        }
        
        // Helper method to print grid for visualization
        private void PrintGrid(char[][] grid)
        {
            foreach (var row in grid)
            {
                Console.WriteLine($"[{string.Join(",", Array.ConvertAll(row, c => $"\"{c}\""))}]");
            }
        }
        
        // Test method
        public static void Test()
        {
            var solution = new SurroundedRegions();
            
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.WriteLine("SURROUNDED REGIONS - VISUAL EXAMPLES");
            Console.WriteLine("═══════════════════════════════════════════════════════\n");
            
            // Test case 1: Simple surrounded region
            char[][] grid1 = new char[][]
            {
                new char[] {'X','X','X','X','X'},
                new char[] {'X','O','O','O','X'},
                new char[] {'X','O','X','O','X'},
                new char[] {'X','O','O','O','X'},
                new char[] {'X','X','X','X','X'}
            };
            
            Console.WriteLine("Test Case 1: SIMPLE SURROUNDED CLUSTER");
            Console.WriteLine("─────────────────────────────────────────────────────");
            Console.WriteLine("Input:");
            solution.PrintGrid(grid1);
            Console.WriteLine("\nAnalysis:");
            Console.WriteLine("• All O's are INSIDE the border (not touching edges)");
            Console.WriteLine("• Completely surrounded by X's");
            Console.WriteLine("• Result: ALL O's should flip to X ✗");
            solution.Solve(grid1);
            Console.WriteLine("\nOutput:");
            solution.PrintGrid(grid1);
            Console.WriteLine("Expected: All O's → X (completely surrounded)\n");
            
            // Test case 2: Multiple isolated clusters
            char[][] grid2 = new char[][]
            {
                new char[] {'X','X','X','X','X','X','X'},
                new char[] {'X','O','O','X','X','X','X'},
                new char[] {'X','O','O','X','X','X','X'},
                new char[] {'X','X','X','X','O','O','X'},
                new char[] {'X','X','X','X','O','O','X'},
                new char[] {'X','X','X','X','X','X','X'}
            };
            
            Console.WriteLine("\nTest Case 2: MULTIPLE ISOLATED CLUSTERS");
            Console.WriteLine("─────────────────────────────────────────────────────");
            Console.WriteLine("Input:");
            solution.PrintGrid(grid2);
            Console.WriteLine("\nAnalysis:");
            Console.WriteLine("• Cluster 1: O's at [1-2, 1-2] - surrounded");
            Console.WriteLine("• Cluster 2: O's at [3-4, 4-5] - surrounded");
            Console.WriteLine("• Both clusters isolated from borders");
            Console.WriteLine("• Result: BOTH clusters flip to X ✗");
            solution.Solve(grid2);
            Console.WriteLine("\nOutput:");
            solution.PrintGrid(grid2);
            Console.WriteLine("Expected: All O's → X (both clusters surrounded)\n");
            
            // Test case 3: One surrounded, one connected to border
            char[][] grid3 = new char[][]
            {
                new char[] {'X','X','X','X','X','X'},
                new char[] {'X','O','O','X','O','X'},
                new char[] {'X','O','X','X','O','X'},
                new char[] {'X','X','X','X','O','O'},  // Right cluster touches border
                new char[] {'X','X','X','X','X','X'}
            };
            
            Console.WriteLine("\nTest Case 3: MIXED - SURROUNDED vs BORDER-CONNECTED");
            Console.WriteLine("─────────────────────────────────────────────────────");
            Console.WriteLine("Input:");
            solution.PrintGrid(grid3);
            Console.WriteLine("\nAnalysis:");
            Console.WriteLine("• LEFT cluster [1-2, 1]: Isolated → FLIP ✗");
            Console.WriteLine("• RIGHT cluster [1-3, 4-5]: Touches border at [3,5] → KEEP ✓");
            Console.WriteLine("• Border O protects entire connected region!");
            solution.Solve(grid3);
            Console.WriteLine("\nOutput:");
            solution.PrintGrid(grid3);
            Console.WriteLine("Expected: Left O's → X, Right O's → Keep as O\n");
            
            // Test case 4: Complex pattern - donut shape
            char[][] grid4 = new char[][]
            {
                new char[] {'X','X','X','X','X','X','X'},
                new char[] {'X','O','O','O','O','O','X'},
                new char[] {'X','O','X','X','X','O','X'},
                new char[] {'X','O','X','O','X','O','X'},  // Inner O surrounded
                new char[] {'X','O','X','X','X','O','X'},
                new char[] {'X','O','O','O','O','O','X'},
                new char[] {'X','X','X','X','X','X','X'}
            };
            
            Console.WriteLine("\nTest Case 4: DONUT PATTERN (Nested Regions)");
            Console.WriteLine("─────────────────────────────────────────────────────");
            Console.WriteLine("Input:");
            solution.PrintGrid(grid4);
            Console.WriteLine("\nAnalysis:");
            Console.WriteLine("• OUTER ring of O's: Isolated from border → FLIP ✗");
            Console.WriteLine("• INNER O at [3,3]: Surrounded by X's → FLIP ✗");
            Console.WriteLine("• Even complex patterns follow same rule!");
            solution.Solve(grid4);
            Console.WriteLine("\nOutput:");
            solution.PrintGrid(grid4);
            Console.WriteLine("Expected: ALL O's → X (outer ring and inner O both surrounded)\n");
            
            // Test case 5: Diagonal pattern (not connected diagonally!)
            char[][] grid5 = new char[][]
            {
                new char[] {'X','X','X','X','X'},
                new char[] {'X','O','X','X','X'},
                new char[] {'X','X','O','X','X'},
                new char[] {'X','X','X','O','X'},
                new char[] {'X','X','X','X','X'}
            };
            
            Console.WriteLine("\nTest Case 5: DIAGONAL PATTERN (Not Connected!)");
            Console.WriteLine("─────────────────────────────────────────────────────");
            Console.WriteLine("Input:");
            solution.PrintGrid(grid5);
            Console.WriteLine("\nAnalysis:");
            Console.WriteLine("• O's appear on diagonal: [1,1], [2,2], [3,3]");
            Console.WriteLine("• IMPORTANT: Only 4-directional connections count!");
            Console.WriteLine("• Diagonal cells are NOT connected");
            Console.WriteLine("• Each O is isolated → ALL flip ✗");
            solution.Solve(grid5);
            Console.WriteLine("\nOutput:");
            solution.PrintGrid(grid5);
            Console.WriteLine("Expected: All O's → X (diagonal ≠ connected)\n");
            
            // Test case 6: Border connection saves entire region
            char[][] grid6 = new char[][]
            {
                new char[] {'O','X','X','X','X'},
                new char[] {'O','O','O','O','X'},
                new char[] {'X','O','X','O','X'},
                new char[] {'X','O','O','O','X'},
                new char[] {'X','X','X','X','X'}
            };
            
            Console.WriteLine("\nTest Case 6: BORDER CONNECTION SAVES ALL");
            Console.WriteLine("─────────────────────────────────────────────────────");
            Console.WriteLine("Input:");
            solution.PrintGrid(grid6);
            Console.WriteLine("\nAnalysis:");
            Console.WriteLine("• O at [0,0] touches TOP border → Safe! ✓");
            Console.WriteLine("• Entire chain of connected O's is safe:");
            Console.WriteLine("  [0,0]→[1,0]→[1,1]→[1,2]→[1,3]→[2,1]→[2,3]→[3,1]→[3,2]→[3,3]");
            Console.WriteLine("• ONE border connection protects ENTIRE region!");
            solution.Solve(grid6);
            Console.WriteLine("\nOutput:");
            solution.PrintGrid(grid6);
            Console.WriteLine("Expected: ALL O's kept (connected to border)\n");
            
            // Test case 7: Example from problem description
            char[][] grid7 = new char[][]
            {
                new char[] {'X','X','X','X','O'},
                new char[] {'X','X','O','X','X'},
                new char[] {'X','X','X','X','O'},
                new char[] {'X','O','X','X','X'},
                new char[] {'X','O','X','X','X'}
            };
            
            Console.WriteLine("\nTest Case 7: PROBLEM EXAMPLE");
            Console.WriteLine("─────────────────────────────────────────────────────");
            Console.WriteLine("Input:");
            solution.PrintGrid(grid7);
            Console.WriteLine("\nAnalysis:");
            Console.WriteLine("• RIGHT border O's at [0,4] and [2,4]: Touch RIGHT edge → Safe ✓");
            Console.WriteLine("• LEFT border O's at [3,1] and [4,1]: Touch LEFT edge → Safe ✓");
            Console.WriteLine("• CENTER O at [1,2]: Isolated, no border connection → FLIP ✗");
            Console.WriteLine("• Border O's protect themselves, but NOT the center O!");
            solution.Solve(grid7);
            Console.WriteLine("\nOutput:");
            solution.PrintGrid(grid7);
            Console.WriteLine("Expected: Center O → X, Border O's stay\n");
            
            // Test case 8: Large surrounded cluster
            char[][] grid8 = new char[][]
            {
                new char[] {'X','X','X','X','X','X','X','X'},
                new char[] {'X','O','O','O','O','O','O','X'},
                new char[] {'X','O','O','O','O','O','O','X'},
                new char[] {'X','O','O','O','O','O','O','X'},
                new char[] {'X','X','X','X','X','X','X','X'}
            };
            
            Console.WriteLine("\nTest Case 8: LARGE SURROUNDED CLUSTER");
            Console.WriteLine("─────────────────────────────────────────────────────");
            Console.WriteLine("Input:");
            solution.PrintGrid(grid8);
            Console.WriteLine("\nAnalysis:");
            Console.WriteLine("• 18 O's forming a large rectangle");
            Console.WriteLine("• ALL are inside the border (rows 1-3, cols 1-6)");
            Console.WriteLine("• Size doesn't matter - still surrounded!");
            Console.WriteLine("• Result: ALL O's flip to X ✗");
            solution.Solve(grid8);
            Console.WriteLine("\nOutput:");
            solution.PrintGrid(grid8);
            Console.WriteLine("Expected: Entire O cluster → X\n");
            
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.WriteLine("KEY TAKEAWAYS:");
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.WriteLine("✓ BORDER O = Safe (never flipped)");
            Console.WriteLine("✓ O connected to border O = Safe (entire chain protected)");
            Console.WriteLine("✗ O NOT connected to any border = Flipped to X");
            Console.WriteLine("✓ Connection = 4-directional only (N, S, E, W)");
            Console.WriteLine("✗ Diagonal cells are NOT connected");
            Console.WriteLine("✓ Use REVERSE thinking: mark safe O's first, flip rest");
        }
    }
}
