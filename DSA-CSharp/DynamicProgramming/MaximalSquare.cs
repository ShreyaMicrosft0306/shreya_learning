// ============================================================
// PROBLEM: Maximal Square
// ============================================================
// Given a binary matrix containing 0s and 1s, return the area of
// the largest square made completely of 1s.
//
// INPUT:
//   int[,] matrix — an m x n binary matrix
//   Example:
//     [
//       [1, 1, 1],
//       [1, 1, 0],
//       [1, 1, 1]
//     ]
//
// OUTPUT:
//   int — the largest square area
//   Example: 4
//
// EDGE CASES:
//   - All zeroes → 0
//   - All ones → min(rows, cols)²
//   - Single row or column → 1 if any 1 exists, otherwise 0
//   - Single cell 1 → 1
//
// ============================================================
// HOW TO SOLVE — Simple Walkthrough
// ============================================================
// Key Insight (Dynamic Programming):
//   Every cell can be the bottom-right corner of a square. We ask:
//   "What is the largest square side that can end at this cell?"
//
//   If matrix[row, col] is 0:
//     dp[row, col] = 0
//
//   If it is 1:
//     dp[row, col] =
//       1 + min(top, left, top-left)
//
//   The top, left, and top-left cells must already contain enough
//   1-cells to support the square. Taking the minimum ensures all
//   three required sides are available.
//
// Step-by-step example:
//
//   matrix:
//     1 1 1
//     1 1 0
//     1 1 1
//
//   dp table:
//     1 1 1
//     1 2 0
//     1 2 1
//
//   Why dp[1,1] = 2?
//     Matrix cell (1,1) is 1.
//     top = dp[0,1] = 1
//     left = dp[1,0] = 1
//     top-left = dp[0,0] = 1
//     1 + min(1,1,1) = 2
//
//     This means a 2 x 2 square ending at (1,1) is possible:
//       1 1
//       1 1
//
//   Other 1-cells are checked in the same way. The largest dp value
//   is the longest square side. If maxSide = 2:
//
//     area = 2 * 2 = 4
//
// Visualization:
//
//   1 1 1
//   1 [1] 0
//   1 [1] 1
//
//   The square includes the two rows and two columns around dp[1,1]:
//
//     [1 1]
//     [1 1]
//
// Why this approach works:
//   If any of top, left, or top-left cannot support a 2 x 2 square,
//   a 2 x 2 square cannot end at the current cell. For example, if
//   top-left is 0, the current 1 can only start a 1 x 1 square.
//
// ============================================================
// TIME & SPACE COMPLEXITY
// ============================================================
//   m = number of rows
//   n = number of columns
//
//   Time: O(m * n)
//     - Each matrix cell is visited once
//     - Its dp value is calculated in constant time
//     - Total: O(m * n)
//
//   Space: O(m * n)
//     - dp stores one side-length value per matrix cell
//     - A one-row dp array can reduce this to O(n)
//
// ============================================================

using System;

namespace DSA_CSharp.DynamicProgramming
{
    public class MaximalSquare
    {
        /// <summary>
        /// Returns area of the largest square containing only 1s.
        /// Time: O(m * n), Space: O(m * n)
        /// </summary>
        /// <param name="matrix">Binary matrix of 0s and 1s</param>
        /// <returns>Area of largest valid square</returns>
        public int Solution(int[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            int[,] dp = new int[rows + 1, cols + 1];
            int maxSide = 0;

            for (int r = 1; r <= rows; r++)
            {
                for (int c = 1; c <= cols; c++)
                {
                    if (matrix[r - 1, c - 1] == 1)
                    {
                        dp[r, c] = 1 + Math.Min(
                            dp[r - 1, c],
                            Math.Min(dp[r, c - 1], dp[r - 1, c - 1])
                        );

                        if (dp[r, c] > maxSide)
                        {
                            maxSide = dp[r, c];
                        }
                    }
                }
            }

            return maxSide * maxSide;
        }

        /// <summary>
        /// Space-optimized version using one row.
        /// Time: O(m * n), Space: O(n)
        /// </summary>
        public int SolutionOptimized(int[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            int[] dp = new int[cols + 1];
            int maxSide = 0;

            for (int r = 1; r <= rows; r++)
            {
                int prevDiagonal = 0; // Stores dp[c - 1] from previous row.

                for (int c = 1; c <= cols; c++)
                {
                    int temp = dp[c];

                    if (matrix[r - 1, c - 1] == 1)
                    {
                        dp[c] = 1 + Math.Min(dp[c], Math.Min(dp[c - 1], prevDiagonal));

                        if (dp[c] > maxSide)
                        {
                            maxSide = dp[c];
                        }
                    }
                    else
                    {
                        dp[c] = 0;
                    }

                    prevDiagonal = temp;
                }
            }

            return maxSide * maxSide;
        }

        // Test method
        public static void Test()
        {
            var solution = new MaximalSquare();

            Console.WriteLine("=== Maximal Square Test Cases ===\n");

            int[,] matrix1 =
            {
                { 0, 0, 1, 0, 0 },
                { 1, 1, 1, 0, 1 },
                { 0, 1, 1, 0, 0 }
            };
            Console.WriteLine($"Test 1 Output: {solution.Solution(matrix1)}");
            Console.WriteLine("Expected: 4\n");

            int[,] matrix2 =
            {
                { 0, 1 },
                { 1, 0 }
            };
            Console.WriteLine($"Test 2 Output: {solution.Solution(matrix2)}");
            Console.WriteLine("Expected: 1\n");

            int[,] matrix3 =
            {
                { 0 }
            };
            Console.WriteLine($"Test 3 Output: {solution.Solution(matrix3)}");
            Console.WriteLine("Expected: 0\n");

            int[,] matrix4 =
            {
                { 1, 1, 1 },
                { 1, 1, 1 },
                { 1, 1, 1 }
            };
            Console.WriteLine($"Test 4 Output: {solution.Solution(matrix4)}");
            Console.WriteLine("Expected: 9\n");

            Console.WriteLine("Optimized check for Test 1:");
            Console.WriteLine($"Output: {solution.SolutionOptimized(matrix1)}");
            Console.WriteLine("Expected: 4");
        }
    }
}
