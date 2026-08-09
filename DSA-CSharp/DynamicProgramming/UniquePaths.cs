/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Unique Paths
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
A robot starts at the top-left corner of an m x n grid.
The robot can only move either DOWN or RIGHT at any point.
Return the number of unique paths to reach the bottom-right corner.

INPUT:
- m: int - Number of rows (1 <= m <= 100)
- n: int - Number of columns (1 <= n <= 100)

OUTPUT:
- int - Number of unique paths from top-left to bottom-right

EXAMPLES:
Example 1:
Input: m = 2, n = 3
Output: 3
Explanation: Paths are Right->Right->Down, Right->Down->Right, Down->Right->Right

Example 2:
Input: m = 3, n = 7
Output: 28

CONSTRAINTS:
- 1 <= m, n <= 100
- Answer is guaranteed to be <= 2 * 10^9

EDGE CASES:
✓ m = 1 or n = 1 -> only one straight path
✓ Small grid 2x2 -> 2 paths
✓ Larger rectangular grids

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OPTIMAL SOLUTION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APPROACH:
Use Dynamic Programming with a 2D table.

Let dp[r][c] = number of ways to reach cell (r, c).

Transition:
- Robot can come to (r, c) only from:
  1) top:    (r - 1, c)
  2) left:   (r, c - 1)
- So: dp[r][c] = dp[r - 1][c] + dp[r][c - 1]

Base cases:
- First row has only 1 way (all RIGHT moves)
- First column has only 1 way (all DOWN moves)

TIME COMPLEXITY: O(m * n)
SPACE COMPLEXITY: O(m * n)

PATTERN: Dynamic Programming - Grid
DIFFICULTY: Medium
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;

namespace DSA_CSharp.DynamicProgramming
{
    public class UniquePaths
    {
        /// <summary>
        /// Returns number of unique paths from top-left to bottom-right.
        /// Time: O(m * n), Space: O(m * n)
        /// </summary>
        public int Solution(int m, int n)
        {
            int[,] dp = new int[m, n];

            // First row and first column are reachable in exactly one way.
            for (int r = 0; r < m; r++)
            {
                dp[r, 0] = 1;
            }

            for (int c = 0; c < n; c++)
            {
                dp[0, c] = 1;
            }

            // Fill remaining cells from top and left contributions.
            for (int r = 1; r < m; r++)
            {
                for (int c = 1; c < n; c++)
                {
                    dp[r, c] = dp[r - 1, c] + dp[r, c - 1];
                }
            }

            return dp[m - 1, n - 1];
        }

        /// <summary>
        /// Space-optimized variant using one row.
        /// Time: O(m * n), Space: O(n)
        /// </summary>
        public int SolutionOptimized(int m, int n)
        {
            int[] dp = new int[n];

            // First row is all 1s.
            for (int c = 0; c < n; c++)
            {
                dp[c] = 1;
            }

            for (int r = 1; r < m; r++)
            {
                for (int c = 1; c < n; c++)
                {
                    dp[c] = dp[c] + dp[c - 1];
                }
            }

            return dp[n - 1];
        }

        // Test method
        public static void Test()
        {
            var solution = new UniquePaths();

            Console.WriteLine("=== Unique Paths Test Cases ===\n");

            int m1 = 2, n1 = 3;
            Console.WriteLine($"Input: m = {m1}, n = {n1}");
            Console.WriteLine($"Output: {solution.Solution(m1, n1)}");
            Console.WriteLine("Expected: 3\n");

            int m2 = 3, n2 = 7;
            Console.WriteLine($"Input: m = {m2}, n = {n2}");
            Console.WriteLine($"Output: {solution.Solution(m2, n2)}");
            Console.WriteLine("Expected: 28\n");

            int m3 = 1, n3 = 5;
            Console.WriteLine($"Input: m = {m3}, n = {n3}");
            Console.WriteLine($"Output: {solution.Solution(m3, n3)}");
            Console.WriteLine("Expected: 1\n");

            int m4 = 5, n4 = 1;
            Console.WriteLine($"Input: m = {m4}, n = {n4}");
            Console.WriteLine($"Output: {solution.Solution(m4, n4)}");
            Console.WriteLine("Expected: 1\n");

            int m5 = 3, n5 = 3;
            Console.WriteLine($"Input: m = {m5}, n = {n5}");
            Console.WriteLine($"Output: {solution.Solution(m5, n5)}");
            Console.WriteLine("Expected: 6\n");

            Console.WriteLine("Optimized check (m=3, n=7):");
            Console.WriteLine($"Output: {solution.SolutionOptimized(3, 7)}");
            Console.WriteLine("Expected: 28");
        }
    }
}
