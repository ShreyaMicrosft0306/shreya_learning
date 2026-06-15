/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Pacific Atlantic Water Flow
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
There is an m x n rectangular island that borders both the Pacific Ocean and 
Atlantic Ocean. The Pacific Ocean touches the island's left and top edges, 
and the Atlantic Ocean touches the island's right and bottom edges.

The island is partitioned into a grid of square cells. You are given an m x n 
integer matrix heights where heights[r][c] represents the height above sea 
level of the cell at coordinate (r, c).

Water can flow from a cell to a neighboring cell (up, down, left, right) if 
the neighboring cell's height is less than or equal to the current cell's height.
Water can flow from any cell adjacent to an ocean into that ocean.

Return a 2D list of grid coordinates where water can flow to both the Pacific 
and Atlantic oceans.

INPUT:
- heights: int[][] - m x n matrix representing cell heights
- m, n: 1 <= m, n <= 200
- heights[i][j]: 0 <= heights[i][j] <= 10^5

OUTPUT:
- List<List<int>> - List of coordinates [row, col] that can reach both oceans

EXAMPLES:
Example 1:
Input: heights = [[1,2,2,3,5],
                  [1,2,3,4,4],
                  [2,4,5,3,1],
                  [6,7,1,4,5],
                  [5,1,1,2,4]]
Output: [[0,4],[1,3],[1,4],[2,2],[3,0],[3,1],[4,0]]
Explanation: Water from these cells can flow to both oceans.

Example 2:
Input: heights = [[1]]
Output: [[0,0]]
Explanation: Water can flow from the only cell to both oceans.

CONSTRAINTS:
- m == heights.length
- n == heights[r].length
- 1 <= m, n <= 200
- 0 <= heights[r][c] <= 10^5

EDGE CASES:
✓ Single cell (1x1) → always can reach both oceans
✓ Single row or single column
✓ All cells same height
✓ Strictly increasing heights from corners
✓ Mountains in the middle (no cells can reach both)
✓ Flat terrain (all cells can reach both)
✓ Maximum size (200x200)

TRICK CASES:
⚡ Don't start from each cell and try to reach oceans (too slow: O(m²n²))
⚡ Water flows from HIGH to LOW or EQUAL - reverse thinking required
⚡ Start from ocean borders and flow UPWARD (reverse flow)
⚡ Find intersection of cells reachable from both oceans
⚡ Need two separate DFS/BFS passes (one per ocean)

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OPTIMAL SOLUTION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APPROACH:
Instead of starting from each cell and trying to reach oceans (expensive),
we REVERSE the problem: start from ocean borders and flow UPWARD (to higher 
or equal cells). Then find cells reachable from BOTH oceans.

This is a brilliant example of "reverse thinking" optimization!

KEY INSIGHTS:
• Reverse the flow: Start from oceans, flow upward to higher cells
• If water flows from A to B, then we can reach A from B (reverse)
• Use DFS/BFS from all border cells of each ocean
• Mark cells reachable from Pacific and Atlantic separately
• Intersection = cells that can reach both oceans

ALGORITHM STEPS:
1. Create two boolean matrices: pacificReachable, atlanticReachable
2. Start DFS from all Pacific border cells (top row + left column)
   - Mark all cells reachable by flowing upward
3. Start DFS from all Atlantic border cells (bottom row + right column)
   - Mark all cells reachable by flowing upward
4. Find intersection: cells marked true in both matrices
5. Return coordinates of intersection cells

TIME COMPLEXITY: O(m × n)
- Each cell is visited at most twice (once per ocean DFS)
- DFS visits each cell once: O(m × n)
- Total: O(m × n) for Pacific + O(m × n) for Atlantic = O(m × n)

SPACE COMPLEXITY: O(m × n)
- Two boolean matrices: O(m × n) each
- DFS recursion stack: O(m × n) in worst case
- Total: O(m × n)

PATTERN: Graph DFS/BFS, Reverse Thinking, Multi-Source Search
DIFFICULTY: Medium
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;
using System.Collections.Generic;

namespace DSA_CSharp.Graphs
{
    public class PacificAtlanticWaterFlow
    {
        private int[][] heights;
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
        /// Finds all cells from which water can flow to both Pacific and Atlantic oceans.
        /// Uses reverse DFS from ocean borders.
        /// </summary>
        /// <param name="heights">Matrix of cell heights</param>
        /// <returns>List of coordinates that can reach both oceans</returns>
        public IList<IList<int>> PacificAtlantic(int[][] heights)
        {
            if (heights == null || heights.Length == 0) 
                return new List<IList<int>>();
            
            this.heights = heights;
            rows = heights.Length;
            cols = heights[0].Length;
            
            // Track which cells can reach each ocean
            bool[,] pacificReachable = new bool[rows, cols];
            bool[,] atlanticReachable = new bool[rows, cols];
            
            // DFS from Pacific borders (top row and left column)
            for (int col = 0; col < cols; col++)
            {
                DFS(0, col, pacificReachable);  // Top row
            }
            for (int row = 0; row < rows; row++)
            {
                DFS(row, 0, pacificReachable);  // Left column
            }
            
            // DFS from Atlantic borders (bottom row and right column)
            for (int col = 0; col < cols; col++)
            {
                DFS(rows - 1, col, atlanticReachable);  // Bottom row
            }
            for (int row = 0; row < rows; row++)
            {
                DFS(row, cols - 1, atlanticReachable);  // Right column
            }
            
            // Find intersection: cells reachable from both oceans
            List<IList<int>> result = new List<IList<int>>();
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    if (pacificReachable[row, col] && atlanticReachable[row, col])
                    {
                        result.Add(new List<int> { row, col });
                    }
                }
            }
            
            return result;
        }
        
        /// <summary>
        /// DFS to mark all cells reachable from current cell by flowing upward.
        /// </summary>
        private void DFS(int row, int col, bool[,] reachable)
        {
            // Already visited
            if (reachable[row, col]) return;
            
            // Mark as reachable
            reachable[row, col] = true;
            
            // Explore all 4 directions
            foreach (var dir in directions)
            {
                int newRow = row + dir[0];
                int newCol = col + dir[1];
                
                // Check bounds
                if (newRow < 0 || newRow >= rows || newCol < 0 || newCol >= cols)
                    continue;
                
                // Can only flow to cells with HIGHER or EQUAL height (reverse flow)
                if (heights[newRow][newCol] >= heights[row][col])
                {
                    DFS(newRow, newCol, reachable);
                }
            }
        }
        
        // Test method
        public static void Test()
        {
            var solution = new PacificAtlanticWaterFlow();
            
            // Test case 1
            int[][] heights1 = new int[][]
            {
                new int[] {1, 2, 2, 3, 5},
                new int[] {3, 2, 3, 4, 4},
                new int[] {2, 4, 5, 3, 1},
                new int[] {6, 7, 1, 4, 5},
                new int[] {5, 1, 1, 2, 4}
            };
            var result1 = solution.PacificAtlantic(heights1);
            Console.WriteLine("Test Case 1:");
            Console.WriteLine("Cells that can reach both oceans:");
            foreach (var cell in result1)
            {
                Console.WriteLine($"[{cell[0]},{cell[1]}]");
            }
            Console.WriteLine();
            
            // Test case 2 - Edge case: single cell
            int[][] heights2 = new int[][] { new int[] {1} };
            var result2 = solution.PacificAtlantic(heights2);
            Console.WriteLine("Test Case 2 (Single cell):");
            Console.WriteLine($"Result: [{result2[0][0]},{result2[0][1]}]");
        }
    }
}
