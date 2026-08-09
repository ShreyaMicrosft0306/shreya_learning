/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Maximal Square
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
Given an m x n matrix of 0s and 1s, return the area of the largest square
containing only 1s.

INPUT:
- matrix: int[,] - Binary matrix where each cell is 0 or 1

OUTPUT:
- int - Area of the largest square containing only 1s

EXAMPLES:
Example 1:
Input:
[
 [0, 0, 1, 0, 0],
 [1, 1, 1, 0, 1],
 [0, 1, 1, 0, 0]
]
Output: 4
Explanation: Largest square side = 2, so area = 2 * 2 = 4

Example 2:
Input:
[
 [0, 1],
 [1, 0]
]
Output: 1

Example 3:
Input:
[
 [0]
]
Output: 0

CONSTRAINTS:
- 1 <= m, n <= 300
- matrix[i][j] is 0 or 1

EDGE CASES:
✓ All zeros -> answer 0
✓ All ones -> answer is min(m, n)^2
✓ Single row/column -> answer is 1 if any 1 exists, else 0
✓ Mixed matrix with multiple candidate squares

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OPTIMAL SOLUTION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APPROACH:
Use Dynamic Programming.

Let dp[r, c] be the side length of the largest square whose bottom-right
corner is at cell (r - 1, c - 1) in the original matrix.

If matrix[r - 1, c - 1] == 1, then:
  dp[r, c] = 1 + min(dp[r - 1, c], dp[r, c - 1], dp[r - 1, c - 1])
Else:
  dp[r, c] = 0

Why minimum of top, left, and top-left?
A square of side k can end at current cell only if all three neighboring
positions can support at least side k - 1.

Track the maximum side while filling dp, then return maxSide * maxSide.

TIME COMPLEXITY: O(m * n)
SPACE COMPLEXITY: O(m * n)

PATTERN: Dynamic Programming - 2D Grid
DIFFICULTY: Medium
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

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
