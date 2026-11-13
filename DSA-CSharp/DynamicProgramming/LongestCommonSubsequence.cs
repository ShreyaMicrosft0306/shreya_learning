using System;

namespace DSA_CSharp.DynamicProgramming
{
    // ============================================================
    // PROBLEM: Longest Common Subsequence (LCS)
    // ============================================================
    // Given two strings text1 and text2, return the length of their
    // longest common subsequence.
    //
    // A subsequence keeps the relative order, but does not require
    // consecutive characters.
    //
    // EXAMPLE:
    // text1 = "abcde", text2 = "ace"
    // Output: 3
    //
    // APPROACH:
    // Dynamic programming over prefixes of the two strings.
    // dp[i][j] = length of the LCS of text1[0..i-1] and text2[0..j-1].
    //
    // If the current characters match, we extend the LCS by 1.
    // Otherwise, we either skip the character from text1 or text2.
    //
    // TIME COMPLEXITY: O(m * n)
    // SPACE COMPLEXITY: O(m * n)
    // ============================================================
    public class LongestCommonSubsequence
    {
        public int Solution(string text1, string text2)
        {
            if (string.IsNullOrEmpty(text1) || string.IsNullOrEmpty(text2))
            {
                return 0;
            }

            int rows = text1.Length + 1;
            int cols = text2.Length + 1;
            int[,] dp = new int[rows, cols];

            for (int i = 1; i < rows; i++)
            {
                for (int j = 1; j < cols; j++)
                {
                    if (text1[i - 1] == text2[j - 1])
                    {
                        dp[i, j] = dp[i - 1, j - 1] + 1;
                    }
                    else
                    {
                        dp[i, j] = Math.Max(dp[i - 1, j], dp[i, j - 1]);
                    }
                }
            }

            return dp[rows - 1, cols - 1];
        }

        public static void Test()
        {
            var solution = new LongestCommonSubsequence();

            Console.WriteLine("=== Longest Common Subsequence Test Cases ===\n");

            Console.WriteLine($"Test 1 Output: {solution.Solution("abcde", "ace")}");
            Console.WriteLine("Expected: 3\n");

            Console.WriteLine($"Test 2 Output: {solution.Solution("abc", "abc")}");
            Console.WriteLine("Expected: 3\n");

            Console.WriteLine($"Test 3 Output: {solution.Solution("abc", "def")}");
            Console.WriteLine("Expected: 0\n");

            Console.WriteLine($"Test 4 Output: {solution.Solution("bl", "yby")}");
            Console.WriteLine("Expected: 1");
        }
    }
}
