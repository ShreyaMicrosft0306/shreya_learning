/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Rotting Oranges
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
You are given an m x n grid representing a box of oranges. Each cell in the 
grid can have one of three values:

- "E" representing an empty cell
- "F" representing a fresh orange
- "R" representing a rotten orange

Every minute, any fresh orange that is adjacent (4-directionally: up, down, 
left, right) to a rotten orange becomes rotten.

Write a function that returns the minimum number of minutes that must elapse 
until no cell has a fresh orange. If it is impossible to rot every fresh 
orange, return -1.

INPUT:
- grid: string[][] - m x n grid with "E", "F", or "R"
- m, n: 1 <= m, n <= 10
- grid[i][j] is "E", "F", or "R"

OUTPUT:
- int - Minimum minutes to rot all oranges, or -1 if impossible

EXAMPLES:
Example 1:
Input: grid = [
    ["R", "F"],
    ["F", "F"]
]
Output: 2
Explanation:
- Minute 0: R F    (1 rotten, 3 fresh)
            F F
- Minute 1: R R    (3 rotten, 1 fresh)
            R F
- Minute 2: R R    (4 rotten, 0 fresh)
            R R

Example 2:
Input: grid = [
    ["R", "E"],
    ["E", "F"]
]
Output: -1
Explanation: The fresh orange at [1,1] is isolated by empty cells.
Cannot be reached from rotten orange at [0,0].

Example 3:
Input: grid = [
    ["R", "F", "F", "F"],
    ["F", "F", "F", "R"],
    ["E", "E", "F", "F"]
]
Output: 2

Example 4:
Input: grid = [
    ["F", "F", "F"],
    ["F", "F", "F"],
    ["F", "F", "F"]
]
Output: -1
Explanation: No rotten oranges to start the process.

CONSTRAINTS:
- 1 <= m, n <= 10
- grid[i][j] is "E", "F", or "R"

EDGE CASES:
✓ No fresh oranges → return 0 (nothing to rot)
✓ No rotten oranges but has fresh → return -1 (impossible)
✓ Fresh oranges isolated by empty cells → return -1
✓ All empty cells → return 0
✓ Single cell with rotten → return 0
✓ Single cell with fresh and no rotten → return -1
✓ All oranges are already rotten → return 0

TRICK CASES:
⚡ Multi-source BFS: Start from ALL rotten oranges simultaneously
⚡ Count fresh oranges at start, decrement as they rot
⚡ If fresh count > 0 at end, return -1
⚡ Don't count the initial rotten oranges in minutes
⚡ Empty cells block propagation (not just grid boundaries)
⚡ Process entire level before incrementing time

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OPTIMAL SOLUTION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APPROACH:
Use MULTI-SOURCE BFS starting from all initially rotten oranges simultaneously.
Each BFS level represents one minute passing. Track fresh oranges count to 
determine if all can be rotted.

This is a classic multi-source BFS problem - all sources spread at the same rate.

KEY INSIGHTS:
• All rotten oranges spread simultaneously (multi-source BFS)
• BFS level = time elapsed
• Track fresh orange count to detect impossible cases
• Process entire level before incrementing time
• Queue initially contains ALL rotten oranges

ALGORITHM STEPS:
1. Initialize:
   - Count fresh oranges
   - Add all initially rotten oranges to queue
   - If no fresh oranges, return 0
2. Perform BFS:
   - For each level (minute):
     a. Process all oranges in current level
     b. For each rotten orange, check 4 neighbors
     c. If neighbor is fresh:
        - Mark as rotten
        - Add to queue
        - Decrement fresh count
     d. Increment minutes after processing level
3. Check result:
   - If fresh count > 0: return -1 (impossible)
   - Else: return minutes elapsed

TIME COMPLEXITY: O(m × n)
- Visit each cell at most once
- Each cell processed once when it becomes rotten
- Total: O(m × n)

SPACE COMPLEXITY: O(m × n)
- Queue can hold all cells in worst case
- When all cells are rotten oranges initially
- Total: O(m × n)

PATTERN: Multi-Source BFS, Grid Traversal, Level Order Processing
DIFFICULTY: Medium
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;
using System.Collections.Generic;

namespace DSA_CSharp.Graphs
{
    public class RottingOranges
    {
        private readonly int[][] directions = new int[][]
        {
            new int[] {0, 1},   // Right
            new int[] {1, 0},   // Down
            new int[] {0, -1},  // Left
            new int[] {-1, 0}   // Up
        };
        
        /// <summary>
        /// Calculates minimum minutes to rot all fresh oranges.
        /// Uses multi-source BFS from all initially rotten oranges.
        /// </summary>
        /// <param name="grid">Grid with "E" (empty), "F" (fresh), "R" (rotten)</param>
        /// <returns>Minimum minutes, or -1 if impossible</returns>
        public int OrangesRotting(string[][] grid)
        {
            if (grid == null || grid.Length == 0) return 0;
            
            int rows = grid.Length;
            int cols = grid[0].Length;
            
            Queue<(int row, int col)> queue = new Queue<(int, int)>();
            int freshCount = 0;
            
            // Step 1: Count fresh oranges and find initial rotten oranges
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    if (grid[row][col] == "F")
                    {
                        freshCount++;
                    }
                    else if (grid[row][col] == "R")
                    {
                        queue.Enqueue((row, col));
                    }
                }
            }
            
            // Edge case: No fresh oranges
            if (freshCount == 0) return 0;
            
            // Edge case: No rotten oranges but has fresh
            if (queue.Count == 0) return -1;
            
            // Step 2: BFS to spread rotting
            int minutes = 0;
            
            while (queue.Count > 0 && freshCount > 0)
            {
                int levelSize = queue.Count;
                
                // Process all oranges at current level (current minute)
                for (int i = 0; i < levelSize; i++)
                {
                    var (currRow, currCol) = queue.Dequeue();
                    
                    // Check all 4 directions
                    foreach (var dir in directions)
                    {
                        int newRow = currRow + dir[0];
                        int newCol = currCol + dir[1];
                        
                        // Check bounds
                        if (newRow < 0 || newRow >= rows || 
                            newCol < 0 || newCol >= cols)
                            continue;
                        
                        // If fresh orange, rot it
                        if (grid[newRow][newCol] == "F")
                        {
                            grid[newRow][newCol] = "R"; // Mark as rotten
                            freshCount--;
                            queue.Enqueue((newRow, newCol));
                        }
                    }
                }
                
                // One minute has passed after processing this level
                minutes++;
            }
            
            // Step 3: Check if all fresh oranges were rotted
            return freshCount == 0 ? minutes : -1;
        }
        
        // Helper method to print grid for visualization
        private void PrintGrid(string[][] grid)
        {
            foreach (var row in grid)
            {
                Console.WriteLine($"[{string.Join(", ", Array.ConvertAll(row, s => $"\"{s}\""))}]");
            }
        }
        
        // Helper method to create a deep copy of grid
        private string[][] CopyGrid(string[][] grid)
        {
            int rows = grid.Length;
            string[][] copy = new string[rows][];
            for (int i = 0; i < rows; i++)
            {
                copy[i] = new string[grid[i].Length];
                Array.Copy(grid[i], copy[i], grid[i].Length);
            }
            return copy;
        }
        
        // Test method
        public static void Test()
        {
            var solution = new RottingOranges();
            
            // Test case 1: Standard case
            string[][] grid1 = new string[][]
            {
                new string[] {"R", "F"},
                new string[] {"F", "F"}
            };
            
            Console.WriteLine("Test Case 1:");
            Console.WriteLine("Input:");
            solution.PrintGrid(grid1);
            var gridCopy1 = solution.CopyGrid(grid1);
            int result1 = solution.OrangesRotting(gridCopy1);
            Console.WriteLine($"Output: {result1}");
            Console.WriteLine($"Expected: 2\n");
            
            // Test case 2: Impossible case
            string[][] grid2 = new string[][]
            {
                new string[] {"R", "E"},
                new string[] {"E", "F"}
            };
            
            Console.WriteLine("Test Case 2 (Impossible):");
            Console.WriteLine("Input:");
            solution.PrintGrid(grid2);
            var gridCopy2 = solution.CopyGrid(grid2);
            int result2 = solution.OrangesRotting(gridCopy2);
            Console.WriteLine($"Output: {result2}");
            Console.WriteLine($"Expected: -1\n");
            
            // Test case 3: Larger grid
            string[][] grid3 = new string[][]
            {
                new string[] {"R", "F", "F", "F"},
                new string[] {"F", "F", "F", "R"},
                new string[] {"E", "E", "F", "F"}
            };
            
            Console.WriteLine("Test Case 3:");
            Console.WriteLine("Input:");
            solution.PrintGrid(grid3);
            var gridCopy3 = solution.CopyGrid(grid3);
            int result3 = solution.OrangesRotting(gridCopy3);
            Console.WriteLine($"Output: {result3}");
            Console.WriteLine($"Expected: 2\n");
            
            // Test case 4: No rotten oranges
            string[][] grid4 = new string[][]
            {
                new string[] {"F", "F"},
                new string[] {"F", "F"}
            };
            
            Console.WriteLine("Test Case 4 (No rotten):");
            Console.WriteLine("Input:");
            solution.PrintGrid(grid4);
            var gridCopy4 = solution.CopyGrid(grid4);
            int result4 = solution.OrangesRotting(gridCopy4);
            Console.WriteLine($"Output: {result4}");
            Console.WriteLine($"Expected: -1\n");
            
            // Test case 5: No fresh oranges
            string[][] grid5 = new string[][]
            {
                new string[] {"R", "R"},
                new string[] {"E", "R"}
            };
            
            Console.WriteLine("Test Case 5 (No fresh):");
            Console.WriteLine("Input:");
            solution.PrintGrid(grid5);
            var gridCopy5 = solution.CopyGrid(grid5);
            int result5 = solution.OrangesRotting(gridCopy5);
            Console.WriteLine($"Output: {result5}");
            Console.WriteLine($"Expected: 0\n");
            
            // Test case 6: Single cell fresh
            string[][] grid6 = new string[][]
            {
                new string[] {"F"}
            };
            
            Console.WriteLine("Test Case 6 (Single fresh):");
            Console.WriteLine("Input:");
            solution.PrintGrid(grid6);
            var gridCopy6 = solution.CopyGrid(grid6);
            int result6 = solution.OrangesRotting(gridCopy6);
            Console.WriteLine($"Output: {result6}");
            Console.WriteLine($"Expected: -1\n");
            
            // Test case 7: Single cell rotten
            string[][] grid7 = new string[][]
            {
                new string[] {"R"}
            };
            
            Console.WriteLine("Test Case 7 (Single rotten):");
            Console.WriteLine("Input:");
            solution.PrintGrid(grid7);
            var gridCopy7 = solution.CopyGrid(grid7);
            int result7 = solution.OrangesRotting(gridCopy7);
            Console.WriteLine($"Output: {result7}");
            Console.WriteLine($"Expected: 0\n");
        }
    }
}
