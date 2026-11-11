/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Coin Change
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
You are given coins of different denominations and an amount. Return the fewest
number of coins needed to make that amount.

If the amount cannot be made, return -1.

You can use an unlimited number of each coin.

EXAMPLE:
Input: coins = [1, 2, 5], amount = 11
Output: 3
Explanation: 11 = 5 + 5 + 1

APPROACH:
Dynamic Programming.

Let dp[x] = minimum coins needed to make amount x.

Initialize dp[0] = 0, and all other dp values to infinity.
For each coin value c:
  For each amount from c to target:
    dp[amount] = min(dp[amount], dp[amount - c] + 1)

This is the unbounded coin change pattern because each coin can be used again
and again.

Why this works:
We build the answer from smaller amounts up to the target. Every time we try a
coin, we ask: “If I already know the best way to make amount - coin, how many
more coins do I need to reach this amount?”

TIME COMPLEXITY: O(amount * coins.Length)
SPACE COMPLEXITY: O(amount)

PATTERN: Dynamic Programming / Unbounded Knapsack
DIFFICULTY: Medium
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;

namespace DSA_CSharp.DynamicProgramming
{
    public class CoinChange
    {
        /// <summary>
        /// Returns the fewest coins needed to form the target amount.
        /// Uses unbounded knapsack DP: O(amount * coins.Length), O(amount)
        /// </summary>
        public int Solution(int[] coins, int amount)
        {
            if (amount == 0)
            {
                return 0;
            }

            int[] dp = new int[amount + 1];
            Array.Fill(dp, int.MaxValue);
            dp[0] = 0;

            for (int i = 0; i < coins.Length; i++)
            {
                int coin = coins[i];

                for (int currentAmount = coin; currentAmount <= amount; currentAmount++)
                {
                    if (dp[currentAmount - coin] != int.MaxValue)
                    {
                        dp[currentAmount] = Math.Min(dp[currentAmount], dp[currentAmount - coin] + 1);
                    }
                }
            }

            return dp[amount] == int.MaxValue ? -1 : dp[amount];
        }

        public static void Test()
        {
            var solution = new CoinChange();

            Console.WriteLine("=== Coin Change Test Cases ===\n");

            int[] coins1 = { 1, 2, 5 };
            Console.WriteLine($"Test 1 Output: {solution.Solution(coins1, 11)}");
            Console.WriteLine("Expected: 3\n");

            int[] coins2 = { 2 };
            Console.WriteLine($"Test 2 Output: {solution.Solution(coins2, 3)}");
            Console.WriteLine("Expected: -1\n");

            int[] coins3 = { 1 };
            Console.WriteLine($"Test 3 Output: {solution.Solution(coins3, 0)}");
            Console.WriteLine("Expected: 0\n");

            int[] coins4 = { 2, 3 };
            Console.WriteLine($"Test 4 Output: {solution.Solution(coins4, 7)}");
            Console.WriteLine("Expected: 3");
        }
    }
}
