/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Paint House
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
Paint n houses in a row using Red, Blue, or Green.
No two adjacent houses can have the same color.
Return the minimum cost to paint all houses.

Costs are given as:
- costs[i][0] = cost of painting house i Red
- costs[i][1] = cost of painting house i Blue
- costs[i][2] = cost of painting house i Green

INPUT:
- costs: int[,] - n x 3 cost matrix

OUTPUT:
- int - Minimum total cost to paint all houses

EXAMPLES:
Example 1:
Input: costs = [[8,4,15],[10,7,3],[6,9,12]]
Output: 13
Explanation: Blue(4) + Green(3) + Red(6) = 13

Example 2:
Input: costs = [[5,8,6],[19,14,13],[7,5,12],[14,5,9]]
Output: 30
Explanation: Red(5) + Green(13) + Red(7) + Blue(5) = 30

CONSTRAINTS:
- 1 <= n <= 100
- costs[i].Length == 3
- 0 <= costs[i][j] <= 1000

EDGE CASES:
✓ Single house -> pick minimum of 3 colors
✓ Two houses -> each house picks cheapest color different from neighbor
✓ All costs equal -> any valid coloring gives same answer

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OPTIMAL SOLUTION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APPROACH:
Dynamic Programming.

Let dp[i][c] = minimum cost to paint houses 0..i where house i is color c.

Transition (no two adjacent houses can share color):
  dp[i][0] = costs[i][0] + min(dp[i-1][1], dp[i-1][2])  // Red: prev was Blue or Green
  dp[i][1] = costs[i][1] + min(dp[i-1][0], dp[i-1][2])  // Blue: prev was Red or Green
  dp[i][2] = costs[i][2] + min(dp[i-1][0], dp[i-1][1])  // Green: prev was Red or Blue

Base case:
  dp[0][c] = costs[0][c]  for c in {0,1,2}

Answer:
  min(dp[n-1][0], dp[n-1][1], dp[n-1][2])

DRY RUN for Example 1: costs = [[8,4,15],[10,7,3],[6,9,12]]

House 0 (base):
  dp[0] = [8, 4, 15]

House 1:
  Red   = 10 + min(4, 15) = 10 + 4  = 14
  Blue  =  7 + min(8, 15) =  7 + 8  = 15
  Green =  3 + min(8,  4) =  3 + 4  =  7
  dp[1] = [14, 15, 7]

House 2:
  Red   =  6 + min(15, 7) =  6 + 7  = 13
  Blue  =  9 + min(14, 7) =  9 + 7  = 16
  Green = 12 + min(14,15) = 12 + 14 = 26
  dp[2] = [13, 16, 26]

Answer = min(13, 16, 26) = 13 ✓

TIME COMPLEXITY: O(n)
- Single pass through all houses, O(1) work per house.

SPACE COMPLEXITY: O(1)
- Only need previous row's three values at any time.

PATTERN: Dynamic Programming - State Machine
DIFFICULTY: Easy/Medium
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;

namespace DSA_CSharp.DynamicProgramming
{
    public class PaintHouse
    {
        /// <summary>
        /// Returns minimum cost to paint all houses with no two adjacent
        /// houses sharing the same color.
        /// Time: O(n), Space: O(1)
        /// </summary>
        public int Solution(int[][] costs)
        {
            // Track minimum cost to reach current house for each color.
            // Initialize with the cost of painting the first house.
            int prevRed   = costs[0][0];
            int prevBlue  = costs[0][1];
            int prevGreen = costs[0][2];

            for (int i = 1; i < costs.Length; i++)
            {
                int curRed   = costs[i][0] + Math.Min(prevBlue,  prevGreen);
                int curBlue  = costs[i][1] + Math.Min(prevRed,   prevGreen);
                int curGreen = costs[i][2] + Math.Min(prevRed,   prevBlue);

                prevRed   = curRed;
                prevBlue  = curBlue;
                prevGreen = curGreen;
            }

            return Math.Min(prevRed, Math.Min(prevBlue, prevGreen));
        }

        // Test method
        public static void Test()
        {
            var solution = new PaintHouse();

            Console.WriteLine("=== Paint House Test Cases ===\n");

            int[][] costs1 = { new[] { 8, 4, 15 }, new[] { 10, 7, 3 }, new[] { 6, 9, 12 } };
            Console.WriteLine($"Test 1 Output: {solution.Solution(costs1)}");
            Console.WriteLine("Expected: 13\n");

            int[][] costs2 = { new[] { 5, 8, 6 }, new[] { 19, 14, 13 }, new[] { 7, 5, 12 }, new[] { 14, 5, 9 } };
            Console.WriteLine($"Test 2 Output: {solution.Solution(costs2)}");
            Console.WriteLine("Expected: 30\n");

            int[][] costs3 = { new[] { 1, 2, 3 } };
            Console.WriteLine($"Test 3 (single house) Output: {solution.Solution(costs3)}");
            Console.WriteLine("Expected: 1\n");

            int[][] costs4 = { new[] { 3, 3, 3 }, new[] { 3, 3, 3 } };
            Console.WriteLine($"Test 4 (equal costs) Output: {solution.Solution(costs4)}");
            Console.WriteLine("Expected: 6");
        }
    }
}
