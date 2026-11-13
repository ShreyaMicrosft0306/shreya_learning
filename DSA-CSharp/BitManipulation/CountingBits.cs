/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Counting Bits
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
Given a non-negative integer n, return an array ans where ans[i] is the number
of set bits (1s) in the binary representation of i.

EXAMPLE:
Input: n = 5
Output: [0,1,1,2,1,2]

APPROACH:
Dynamic programming with the observation:

For any number i:
- If i is even, then its binary representation is just i / 2 shifted left by 1
  and the last bit is 0.
- If i is odd, then its binary representation is the same as (i - 1) / 2 shifted
  left by 1 with an extra 1 at the end.

This gives the recurrence:
- dp[i] = dp[i >> 1] + (i & 1)

Why this works:
The number of set bits in i is equal to the number of set bits in i >> 1 plus
whether the last bit is 1.

This allows a single pass in O(n) time.

TIME COMPLEXITY: O(n)
SPACE COMPLEXITY: O(n)

PATTERN: Bit Manipulation + Dynamic Programming
DIFFICULTY: Easy
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;

namespace DSA_CSharp.BitManipulation
{
    public class CountingBits
    {
        /// <summary>
        /// Returns the count of set bits for every number from 0 to n.
        /// Time: O(n), Space: O(n)
        /// </summary>
        public int[] Solution(int n)
        {
            int[] dp = new int[n + 1];

            for (int i = 1; i <= n; i++)
            {
                dp[i] = dp[i >> 1] + (i & 1);
            }

            return dp;
        }

        public static void Test()
        {
            var solution = new CountingBits();

            Console.WriteLine("=== Counting Bits Test Cases ===\n");

            int[] result1 = solution.Solution(2);
            Console.WriteLine($"Test 1 Output: [{string.Join(", ", result1)}]");
            Console.WriteLine("Expected: [0,1,1]\n");

            int[] result2 = solution.Solution(5);
            Console.WriteLine($"Test 2 Output: [{string.Join(", ", result2)}]");
            Console.WriteLine("Expected: [0,1,1,2,1,2]");
        }
    }
}
