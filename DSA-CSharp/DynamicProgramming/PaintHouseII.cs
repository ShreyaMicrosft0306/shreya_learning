/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Paint House II
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
Paint n houses in a row using k colors.
No two adjacent houses can have the same color.
Return the minimum cost to paint all houses.

Costs are given as:
- costs[i][j] = cost of painting house i with color j

INPUT:
- costs: int[][] - n x k cost matrix

OUTPUT:
- int - Minimum total cost to paint all houses

EXAMPLES:
Example 1:
Input: costs = [[4,2,8],[7,1,5],[3,9,6]]
Output: 8
Explanation: Color 0 (4) + Color 1 (1) + Color 0 (3) = 8

Example 2:
Input: costs = [[8,3,12,5],[15,9,4,7]]
Output: 7
Explanation: Color 1 (3) + Color 2 (4) = 7

CONSTRAINTS:
- 1 <= n <= 100
- 1 <= k <= 20
- costs[i].Length == k
- 1 <= costs[i][j] <= 1000

EDGE CASES:
✓ Single house -> pick minimum color cost
✓ One color and one house -> answer is that color cost
✓ One color and multiple houses -> impossible under adjacency rule
✓ All costs equal -> any valid alternating colors give same answer

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OPTIMAL SOLUTION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APPROACH:
Dynamic Programming optimized with the smallest and second-smallest
cost from the previous house.

Naive idea:
For every house i and color c:
  dp[i][c] = costs[i][c] + min(dp[i-1][all colors except c])

That checks up to k previous colors for every color, so it costs O(n * k^2).

Optimization:
For the previous house, keep:
- prevMin1: smallest total cost
- prevMin1Color: color that produced prevMin1
- prevMin2: second-smallest total cost

Then for current color c:
- If c is NOT prevMin1Color, we can use prevMin1.
- If c IS prevMin1Color, we must use prevMin2 to avoid same adjacent color.

Transition:
  currentCost = costs[i][c] + (c == prevMin1Color ? prevMin2 : prevMin1)

TIME COMPLEXITY: O(n * k)
- For each house, scan all k colors once.

SPACE COMPLEXITY: O(1)
- Only store smallest and second-smallest previous totals.

PATTERN: Dynamic Programming - State Optimization
DIFFICULTY: Hard
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;

namespace DSA_CSharp.DynamicProgramming
{
    public class PaintHouseII
    {
        /// <summary>
        /// Returns minimum cost to paint all houses with k colors where no
        /// two adjacent houses share the same color.
        /// Time: O(n * k), Space: O(1)
        /// </summary>
        public int Solution(int[][] costs)
        {
            int n = costs.Length;
            int k = costs[0].Length;

            if (n == 1)
            {
                int answer = costs[0][0];

                for (int color = 1; color < k; color++)
                {
                    answer = Math.Min(answer, costs[0][color]);
                }

                return answer;
            }

            if (k == 1)
            {
                return -1;
            }

            int prevMin1 = 0;
            int prevMin2 = 0;
            int prevMin1Color = -1;

            for (int house = 0; house < n; house++)
            {
                int curMin1 = int.MaxValue;
                int curMin2 = int.MaxValue;
                int curMin1Color = -1;

                for (int color = 0; color < k; color++)
                {
                    int previousBest = color == prevMin1Color ? prevMin2 : prevMin1;
                    int totalCost = costs[house][color] + previousBest;

                    if (totalCost < curMin1)
                    {
                        curMin2 = curMin1;
                        curMin1 = totalCost;
                        curMin1Color = color;
                    }
                    else if (totalCost < curMin2)
                    {
                        curMin2 = totalCost;
                    }
                }

                prevMin1 = curMin1;
                prevMin2 = curMin2;
                prevMin1Color = curMin1Color;
            }

            return prevMin1;
        }

        // Test method
        public static void Test()
        {
            var solution = new PaintHouseII();

            Console.WriteLine("=== Paint House II Test Cases ===\n");

            int[][] costs1 = { new[] { 4, 2, 8 }, new[] { 7, 1, 5 }, new[] { 3, 9, 6 } };
            Console.WriteLine($"Test 1 Output: {solution.Solution(costs1)}");
            Console.WriteLine("Expected: 8\n");

            int[][] costs2 = { new[] { 8, 3, 12, 5 }, new[] { 15, 9, 4, 7 } };
            Console.WriteLine($"Test 2 Output: {solution.Solution(costs2)}");
            Console.WriteLine("Expected: 7\n");

            int[][] costs3 = { new[] { 5, 8, 6 } };
            Console.WriteLine($"Test 3 (single house) Output: {solution.Solution(costs3)}");
            Console.WriteLine("Expected: 5\n");

            int[][] costs4 = { new[] { 1 }, new[] { 2 } };
            Console.WriteLine($"Test 4 (impossible) Output: {solution.Solution(costs4)}");
            Console.WriteLine("Expected: -1");
        }
    }
}