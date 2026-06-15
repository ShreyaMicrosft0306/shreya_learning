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

[
  ['X','X','X','X','O'],
  ['X','X','O','X','X'],
  ['X','X','X','X','O'],
  ['X','O','X','X','X'],
  ['X','O','X','X','X']
]
Sample Output
[
  ['X','X','X','X','O'],
  ['X','X','X','X','X'],
  ['X','X','X','X','O'],
  ['X','O','X','X','X'],
  ['X','O','X','X','X']
]

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
            
            // Test case 1
            char[][] grid1 = new char[][]
            {
                new char[] {'X','X','X','X'},
                new char[] {'X','O','O','X'},
                new char[] {'X','X','O','X'},
                new char[] {'X','O','X','X'}
            };
            
            Console.WriteLine("Test Case 1:");
            Console.WriteLine("Input:");
            solution.PrintGrid(grid1);
            solution.Solve(grid1);
            Console.WriteLine("\nOutput:");
            solution.PrintGrid(grid1);
            Console.WriteLine("\nExpected: Center O's flipped to X, bottom O kept\n");
            
            // Test case 2: More complex
            char[][] grid2 = new char[][]
            {
                new char[] {'X','X','X','X','O'},
                new char[] {'X','X','O','X','X'},
                new char[] {'X','X','O','X','O'},
                new char[] {'X','O','X','X','X'},
                new char[] {'X','O','X','X','X'}
            };
            
            Console.WriteLine("Test Case 2:");
            Console.WriteLine("Input:");
            solution.PrintGrid(grid2);
            solution.Solve(grid2);
            Console.WriteLine("\nOutput:");
            solution.PrintGrid(grid2);
            Console.WriteLine("\nExpected: Center O's at [1,2] and [2,2] flipped, border O's kept\n");
            
            // Test case 3: All O's connected to border
            char[][] grid3 = new char[][]
            {
                new char[] {'O','O','O'},
                new char[] {'O','O','O'},
                new char[] {'O','O','O'}
            };
            
            Console.WriteLine("Test Case 3 (All O's):");
            Console.WriteLine("Input:");
            solution.PrintGrid(grid3);
            solution.Solve(grid3);
            Console.WriteLine("\nOutput:");
            solution.PrintGrid(grid3);
            Console.WriteLine("\nExpected: No change (all O's touch border)\n");
            
            // Test case 4: Single surrounded O
            char[][] grid4 = new char[][]
            {
                new char[] {'X','X','X'},
                new char[] {'X','O','X'},
                new char[] {'X','X','X'}
            };
            
            Console.WriteLine("Test Case 4 (Surrounded O):");
            Console.WriteLine("Input:");
            solution.PrintGrid(grid4);
            solution.Solve(grid4);
            Console.WriteLine("\nOutput:");
            solution.PrintGrid(grid4);
            Console.WriteLine("\nExpected: Center O flipped to X\n");
            
            // Test case 5: No O's
            char[][] grid5 = new char[][]
            {
                new char[] {'X','X'},
                new char[] {'X','X'}
            };
            
            Console.WriteLine("Test Case 5 (No O's):");
            Console.WriteLine("Input:");
            solution.PrintGrid(grid5);
            solution.Solve(grid5);
            Console.WriteLine("\nOutput:");
            solution.PrintGrid(grid5);
            Console.WriteLine("\nExpected: No change\n");
        }
    }
}
