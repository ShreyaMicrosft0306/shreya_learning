/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: 01 Matrix (Distance to Nearest Zero)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
You are given an m x n binary matrix grid where each cell contains either a 0 
or a 1.

Write a function that returns a matrix of the same dimensions where each cell 
contains the distance to the nearest 0 in the original matrix. The distance 
between two adjacent cells (4-directionally: up, down, left, right) is 1.

INPUT:
- mat: int[][] - m x n binary matrix containing only 0s and 1s
- m, n: 1 <= m, n <= 10^4
- 1 <= m * n <= 10^4
- mat[i][j] is 0 or 1

OUTPUT:
- int[][] - Matrix of same size with distances to nearest 0

EXAMPLES:
Example 1:
Input: mat = [
    [0, 0, 0],
    [0, 1, 0],
    [0, 0, 0]
]
Output: [
    [0, 0, 0],
    [0, 1, 0],
    [0, 0, 0]
]
Explanation: The 1 at [1,1] has distance 1 to any adjacent 0.

Example 2:
Input: mat = [
    [0, 0, 0],
    [0, 1, 0],
    [1, 1, 1]
]
Output: [
    [0, 0, 0],
    [0, 1, 0],
    [1, 2, 1]
]
Explanation: 
- [2,0]: distance 1 to [1,0] which is 0
- [2,1]: distance 2 (path: [2,1] → [1,1] → [1,0] or similar)
- [2,2]: distance 1 to [1,2] which is 0

Example 3:
Input: mat = [
    [1, 0, 1],
    [0, 1, 0],
    [1, 1, 1]
]
Output: [
    [1, 0, 1],
    [0, 1, 0],
    [1, 2, 1]
]

Example 4:
Input: mat = [
    [0, 1, 1, 1],
    [1, 1, 1, 1],
    [1, 1, 1, 1],
    [1, 1, 1, 0]
]
Output: [
    [0, 1, 2, 3],
    [1, 2, 3, 2],
    [2, 3, 2, 1],
    [3, 2, 1, 0]
]

CONSTRAINTS:
- m == mat.length
- n == mat[i].length
- 1 <= m, n <= 10^4
- 1 <= m * n <= 10^4
- mat[i][j] is either 0 or 1
- There is at least one 0 in mat

EDGE CASES:
✓ All cells are 0 → all distances are 0
✓ Single 0 in corner → increasing distances from that corner
✓ Multiple 0s scattered → each 1 finds nearest 0
✓ Single row or single column
✓ Large matrix (10^4 total cells)
✓ 0s only on borders
✓ Alternating pattern of 0s and 1s

TRICK CASES:
⚡ Don't calculate from each 1 to all 0s (too slow: O(m²n²))
⚡ Use REVERSE thinking: Start from all 0s and spread outward
⚡ Multi-source BFS: All 0s start propagating simultaneously
⚡ BFS guarantees shortest distance (level = distance)
⚡ Initialize distance matrix with 0s for zeros, infinity for ones
⚡ Each BFS level increases distance by 1

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OPTIMAL SOLUTION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APPROACH:
Use MULTI-SOURCE BFS starting from all 0 cells simultaneously. Each BFS level 
represents an increase in distance by 1. This ensures we find the shortest 
distance to any 0 for each cell.

Similar to Rotting Oranges - reverse thinking with multi-source BFS!

KEY INSIGHTS:
• Start from all 0s, not from all 1s (reverse thinking)
• Multi-source BFS: All 0s propagate distance simultaneously
• BFS level = distance from nearest 0
• No need to track visited - use distance matrix itself
• Cells already processed have minimum distance (BFS property)

ALGORITHM STEPS:
1. Initialize:
   - Create result matrix same size as input
   - For each cell:
     • If 0: set distance to 0, add to queue
     • If 1: set distance to infinity (or large value)
2. Perform multi-source BFS:
   - While queue not empty:
     a. Dequeue current cell and its distance
     b. Check all 4 neighbors
     c. If neighbor's distance > current distance + 1:
        - Update neighbor's distance
        - Add neighbor to queue
3. Return result matrix

TIME COMPLEXITY: O(m × n)
- Visit each cell at most once in BFS
- Each cell processed when first reached (shortest path)
- Total: O(m × n)

SPACE COMPLEXITY: O(m × n)
- Queue can hold all cells in worst case
- Result matrix: O(m × n)
- Total: O(m × n)

PATTERN: Multi-Source BFS, Grid Traversal, Shortest Path
DIFFICULTY: Medium
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;
using System.Collections.Generic;

namespace DSA_CSharp.Graphs
{
    public class ZeroOneMatrix
    {
        private readonly int[][] directions = new int[][]
        {
            new int[] {0, 1},   // Right
            new int[] {1, 0},   // Down
            new int[] {0, -1},  // Left
            new int[] {-1, 0}   // Up
        };
        
        /// <summary>
        /// Calculates distance to nearest 0 for each cell in binary matrix.
        /// Uses multi-source BFS from all 0 cells simultaneously.
        /// </summary>
        /// <param name="mat">Binary matrix containing 0s and 1s</param>
        /// <returns>Matrix of distances to nearest 0</returns>
        public int[][] UpdateMatrix(int[][] mat)
        {
            if (mat == null || mat.Length == 0) return mat;
            
            int rows = mat.Length;
            int cols = mat[0].Length;
            
            // Initialize result matrix and queue
            int[][] result = new int[rows][];
            Queue<(int row, int col)> queue = new Queue<(int, int)>();
            
            // Step 1: Initialize distances and add all 0s to queue
            for (int row = 0; row < rows; row++)
            {
                result[row] = new int[cols];
                for (int col = 0; col < cols; col++)
                {
                    if (mat[row][col] == 0)
                    {
                        result[row][col] = 0;
                        queue.Enqueue((row, col));
                    }
                    else
                    {
                        result[row][col] = int.MaxValue; // Infinity
                    }
                }
            }
            
            // Step 2: Multi-source BFS to propagate distances
            while (queue.Count > 0)
            {
                var (currRow, currCol) = queue.Dequeue();
                int currentDist = result[currRow][currCol];
                
                // Check all 4 directions
                foreach (var dir in directions)
                {
                    int newRow = currRow + dir[0];
                    int newCol = currCol + dir[1];
                    
                    // Check bounds
                    if (newRow < 0 || newRow >= rows || 
                        newCol < 0 || newCol >= cols)
                        continue;
                    
                    // If we found a shorter path to this cell
                    if (result[newRow][newCol] > currentDist + 1)
                    {
                        result[newRow][newCol] = currentDist + 1;
                        queue.Enqueue((newRow, newCol));
                    }
                }
            }
            
            return result;
        }
        
        // Alternative: DP approach (two-pass)
        public int[][] UpdateMatrixDP(int[][] mat)
        {
            if (mat == null || mat.Length == 0) return mat;
            
            int rows = mat.Length;
            int cols = mat[0].Length;
            int[][] result = new int[rows][];
            
            // Initialize
            for (int row = 0; row < rows; row++)
            {
                result[row] = new int[cols];
                for (int col = 0; col < cols; col++)
                {
                    result[row][col] = mat[row][col] == 0 ? 0 : int.MaxValue - 1;
                }
            }
            
            // Pass 1: Top-left to bottom-right
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    if (result[row][col] != 0)
                    {
                        if (row > 0)
                            result[row][col] = Math.Min(result[row][col], result[row - 1][col] + 1);
                        if (col > 0)
                            result[row][col] = Math.Min(result[row][col], result[row][col - 1] + 1);
                    }
                }
            }
            
            // Pass 2: Bottom-right to top-left
            for (int row = rows - 1; row >= 0; row--)
            {
                for (int col = cols - 1; col >= 0; col--)
                {
                    if (result[row][col] != 0)
                    {
                        if (row < rows - 1)
                            result[row][col] = Math.Min(result[row][col], result[row + 1][col] + 1);
                        if (col < cols - 1)
                            result[row][col] = Math.Min(result[row][col], result[row][col + 1] + 1);
                    }
                }
            }
            
            return result;
        }
        
        // Helper method to print matrix
        private void PrintMatrix(int[][] matrix)
        {
            foreach (var row in matrix)
            {
                Console.WriteLine($"[{string.Join(", ", row)}]");
            }
        }
        
        // Test method
        public static void Test()
        {
            var solution = new ZeroOneMatrix();
            
            // Test case 1: Simple case
            int[][] mat1 = new int[][]
            {
                new int[] {0, 0, 0},
                new int[] {0, 1, 0},
                new int[] {0, 0, 0}
            };
            
            Console.WriteLine("Test Case 1:");
            Console.WriteLine("Input:");
            solution.PrintMatrix(mat1);
            var result1 = solution.UpdateMatrix(mat1);
            Console.WriteLine("Output:");
            solution.PrintMatrix(result1);
            Console.WriteLine("Expected: All 0s except [1,1]=1\n");
            
            // Test case 2: More complex
            int[][] mat2 = new int[][]
            {
                new int[] {0, 0, 0},
                new int[] {0, 1, 0},
                new int[] {1, 1, 1}
            };
            
            Console.WriteLine("Test Case 2:");
            Console.WriteLine("Input:");
            solution.PrintMatrix(mat2);
            var result2 = solution.UpdateMatrix(mat2);
            Console.WriteLine("Output:");
            solution.PrintMatrix(result2);
            Console.WriteLine("Expected: [[0,0,0],[0,1,0],[1,2,1]]\n");
            
            // Test case 3: From problem description
            int[][] mat3 = new int[][]
            {
                new int[] {1, 0, 1},
                new int[] {0, 1, 0},
                new int[] {1, 1, 1}
            };
            
            Console.WriteLine("Test Case 3:");
            Console.WriteLine("Input:");
            solution.PrintMatrix(mat3);
            var result3 = solution.UpdateMatrix(mat3);
            Console.WriteLine("Output:");
            solution.PrintMatrix(result3);
            Console.WriteLine("Expected: [[1,0,1],[0,1,0],[1,2,1]]\n");
            
            // Test case 4: Corners
            int[][] mat4 = new int[][]
            {
                new int[] {0, 1, 1, 1},
                new int[] {1, 1, 1, 1},
                new int[] {1, 1, 1, 1},
                new int[] {1, 1, 1, 0}
            };
            
            Console.WriteLine("Test Case 4 (Corners):");
            Console.WriteLine("Input:");
            solution.PrintMatrix(mat4);
            var result4 = solution.UpdateMatrix(mat4);
            Console.WriteLine("Output:");
            solution.PrintMatrix(result4);
            Console.WriteLine("Expected: [[0,1,2,3],[1,2,3,2],[2,3,2,1],[3,2,1,0]]\n");
            
            // Test case 5: All zeros
            int[][] mat5 = new int[][]
            {
                new int[] {0, 0},
                new int[] {0, 0}
            };
            
            Console.WriteLine("Test Case 5 (All zeros):");
            Console.WriteLine("Input:");
            solution.PrintMatrix(mat5);
            var result5 = solution.UpdateMatrix(mat5);
            Console.WriteLine("Output:");
            solution.PrintMatrix(result5);
            Console.WriteLine("Expected: [[0,0],[0,0]]\n");
            
            // Test DP approach
            Console.WriteLine("Testing DP approach on Test Case 2:");
            var resultDP = solution.UpdateMatrixDP(mat2);
            solution.PrintMatrix(resultDP);
            Console.WriteLine("Expected: [[0,0,0],[0,1,0],[1,2,1]]\n");
        }
    }
}
