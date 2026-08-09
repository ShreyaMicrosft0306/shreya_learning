/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Decode Ways
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
A message containing letters from A-Z can be encoded using the mapping:
'1' -> 'A', '2' -> 'B', ..., '26' -> 'Z'.

Given a string s containing only digits, return the total number of ways
to decode it.

A '0' cannot be decoded alone. It must be part of "10" or "20".

INPUT:
- s: string - Numeric string (1 <= s.Length <= 100)

OUTPUT:
- int - Number of valid decodings

EXAMPLES:
Example 1:
Input: s = "12"
Output: 2
Explanation: "AB" (1,2) or "L" (12)

Example 2:
Input: s = "226"
Output: 3
Explanation: "BZ" (2,26), "VF" (22,6), "BBF" (2,2,6)

Example 3:
Input: s = "101"
Output: 1
Explanation: Only "JA" (10,1). "01" is invalid.

CONSTRAINTS:
- 1 <= s.Length <= 100
- s contains only digits

EDGE CASES:
✓ Starts with 0 -> 0 ways
✓ Contains invalid pair like "30" -> 0 ways
✓ Contains valid zero pairs like "10", "20" -> valid
✓ Single character (non-zero) -> 1 way
✓ Long string with many combinations

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OPTIMAL SOLUTION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APPROACH:
Use Dynamic Programming.

Let dp[i] be the number of ways to decode the substring s[0..i-1]
(first i characters).

Transitions:
1. Single digit decode:
   If s[i-1] != '0', then dp[i] += dp[i-1]
2. Two digit decode:
   If s[i-2..i-1] forms a number in [10, 26], then dp[i] += dp[i-2]

TIME COMPLEXITY: O(n)
SPACE COMPLEXITY: O(n)

PATTERN: Dynamic Programming - String Decoding
DIFFICULTY: Medium
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;

namespace DSA_CSharp.DynamicProgramming
{
    public class DecodeWays
    {
        /// <summary>
        /// Returns number of valid decodings for a numeric string.
        /// Time: O(n), Space: O(n)
        /// </summary>
        public int NumDecodings(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return 0;
            }

            int n = s.Length;
            int[] dp = new int[n + 1];

            // Empty prefix has one way to decode.
            dp[0] = 1;

            // First character cannot be '0'.
            dp[1] = s[0] == '0' ? 0 : 1;

            for (int i = 2; i <= n; i++)
            {
                char one = s[i - 1];
                char ten = s[i - 2];

                // Single digit decode (1..9)
                if (one != '0')
                {
                    dp[i] += dp[i - 1];
                }

                // Two digit decode (10..26)
                int twoDigit = (ten - '0') * 10 + (one - '0');
                if (twoDigit >= 10 && twoDigit <= 26)
                {
                    dp[i] += dp[i - 2];
                }
            }

            return dp[n];
        }

        // Test method
        public static void Test()
        {
            var solution = new DecodeWays();

            Console.WriteLine("=== Decode Ways Test Cases ===\n");

            string s1 = "12";
            Console.WriteLine($"Input: {s1}");
            Console.WriteLine($"Output: {solution.NumDecodings(s1)}");
            Console.WriteLine("Expected: 2\n");

            string s2 = "226";
            Console.WriteLine($"Input: {s2}");
            Console.WriteLine($"Output: {solution.NumDecodings(s2)}");
            Console.WriteLine("Expected: 3\n");

            string s3 = "101";
            Console.WriteLine($"Input: {s3}");
            Console.WriteLine($"Output: {solution.NumDecodings(s3)}");
            Console.WriteLine("Expected: 1\n");

            string s4 = "06";
            Console.WriteLine($"Input: {s4}");
            Console.WriteLine($"Output: {solution.NumDecodings(s4)}");
            Console.WriteLine("Expected: 0\n");

            string s5 = "2101";
            Console.WriteLine($"Input: {s5}");
            Console.WriteLine($"Output: {solution.NumDecodings(s5)}");
            Console.WriteLine("Expected: 1\n");
        }
    }
}
