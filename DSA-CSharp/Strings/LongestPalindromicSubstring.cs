/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Longest Palindromic Substring
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
Given a string s, return the longest palindromic substring in s.
A palindrome reads the same forward and backward.

INPUT:
- s: string - A string containing 1 to 1000 characters

OUTPUT:
- string - A longest palindromic substring

EXAMPLES:
Example 1:
Input: s = "babad"
Output: "bab"
Explanation: "aba" is also a valid answer.

Example 2:
Input: s = "cbbd"
Output: "bb"

CONSTRAINTS:
- 1 <= s.Length <= 1000
- s consists only of printable ASCII characters

EDGE CASES:
✓ Empty string -> ""
✓ One character -> the character itself
✓ All characters are identical -> the entire string
✓ No palindrome longer than one character -> any one-character substring

TRICK CASES:
⚡ Do not confuse palindrome substrings with subsequences
⚡ Check both odd-length and even-length palindromes
⚡ Keep the longest indices while expanding, rather than copying substrings repeatedly

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OPTIMAL SOLUTION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APPROACH:
Expand Around Centers.

For every character position, treat it as a possible center:
- A center for an odd-length palindrome: one character
- A center for an even-length palindrome: the gap between two characters

Expand outward while the characters match, and update the longest
[start, end] range whenever a longer palindrome is found.

KEY INSIGHTS:
• Every palindrome has a center
• Odd-length palindromes have 2n - 1 possible centers
• Even-length palindromes have n - 1 possible centers
• Checking both types covers all possible palindromes

ALGORITHM STEPS:
1. If the string is empty, return ""
2. Track the start and length of the longest palindrome
3. For each index, expand around a single-character center
4. For each index, expand around a two-character center
5. Update the longest range whenever the expanded length exceeds it
6. Return s.Substring(start, longestLength)

TIME COMPLEXITY: O(n²)
- There are O(n) centers
- Expansion for a center can take O(n) in the worst case

SPACE COMPLEXITY: O(1)
- The algorithm only tracks indices and lengths

PATTERN: Expand Around Centers
DIFFICULTY: Medium
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;

namespace DSA_CSharp.Strings
{
    public class LongestPalindromicSubstring
    {
        /// <summary>
        /// Returns the longest palindromic substring in s.
        /// Dynamic programming approach: O(n²) time, O(n²) space.
        /// </summary>
        public string LongestPalindrome(string s)
        {
            if (string.IsNullOrEmpty(s))
                return "";

            int n = s.Length;
            bool[,] dp = new bool[n, n];

            int longestStart = 0;
            int longestLength = 1;

            // Single-character palindromes
            for (int i = 0; i < n; i++)
            {
                dp[i, i] = true;
            }

            // Two-character palindromes
            for (int i = 0; i < n - 1; i++)
            {
                if (s[i] == s[i + 1])
                {
                    dp[i, i + 1] = true;
                    longestStart = i;
                    longestLength = 2;
                }
            }

            // Length >= 3
            for (int length = 3; length <= n; length++)
            {
                for (int left = 0; left + length - 1 < n; left++)
                {
                    int right = left + length - 1;

                    if (s[left] == s[right] && dp[left + 1, right - 1])
                    {
                        dp[left, right] = true;

                        if (length > longestLength)
                        {
                            longestStart = left;
                            longestLength = length;
                        }
                    }
                }
            }

            return s.Substring(longestStart, longestLength);
        }

        public static void Test()
        {
            var solution = new LongestPalindromicSubstring();

            Console.WriteLine("=== Longest Palindromic Substring Test Cases ===\n");

            string s1 = "babad";
            Console.WriteLine($"Input: {s1}");
            Console.WriteLine($"Output: {solution.LongestPalindrome(s1)}");
            Console.WriteLine("Expected: \"bab\" or \"aba\"\n");

            string s2 = "cbbd";
            Console.WriteLine($"Input: {s2}");
            Console.WriteLine($"Output: {solution.LongestPalindrome(s2)}");
            Console.WriteLine("Expected: \"bb\"\n");

            string s3 = "a";
            Console.WriteLine($"Input: {s3}");
            Console.WriteLine($"Output: {solution.LongestPalindrome(s3)}");
            Console.WriteLine("Expected: \"a\"\n");

            string s4 = "aaaa";
            Console.WriteLine($"Input: {s4}");
            Console.WriteLine($"Output: {solution.LongestPalindrome(s4)}");
            Console.WriteLine("Expected: \"aaaa\"\n");

            string s5 = "abacdfgdcaba";
            Console.WriteLine($"Input: {s5}");
            Console.WriteLine($"Output: {solution.LongestPalindrome(s5)}");
            Console.WriteLine("Expected: \"aba\" or \"aca\"\n");

            string s6 = "";
            Console.WriteLine("Input: \"\"");
            Console.WriteLine($"Output: \"{solution.LongestPalindrome(s6)}\"");
            Console.WriteLine("Expected: \"\"\n");
        }
    }
}
