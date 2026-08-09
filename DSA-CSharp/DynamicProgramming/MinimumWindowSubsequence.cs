/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Minimum Window Subsequence
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
Given a source string s1 and a pattern string s2, find the shortest
contiguous substring of s1 that contains s2 as a subsequence.

Return the shortest such substring. If multiple windows have the same
length, return the leftmost one.
If no valid window exists, return an empty string.

INPUT:
- s1: string
- s2: string

OUTPUT:
- string - shortest window in s1 containing s2 as a subsequence

EXAMPLES:
Example 1:
Input:
s1 = "hellointerview"
s2 = "her"
Output:
"hellointer"

Example 2:
Input:
s1 = "codingisfun"
s2 = "xyz"
Output:
""

CONSTRAINTS:
- 1 <= s1.Length
- 1 <= s2.Length

EDGE CASES:
✓ s2 longer than s1 -> impossible, return ""
✓ s2 characters not present in s1 -> return ""
✓ Multiple equal-size answers -> pick leftmost

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OPTIMAL PRACTICAL SOLUTION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APPROACH:
Two-pass greedy window tightening.

1) Forward scan to find a window end where s2 appears as a subsequence.
2) Backward scan from that end to shrink the start to the minimum
   possible index for that same end.
3) Track best window and continue searching from just after that start.

TIME COMPLEXITY: O(n * m)
- n = s1.Length, m = s2.Length

SPACE COMPLEXITY: O(1)

PATTERN: Two Pointers + Greedy Backtracking
DIFFICULTY: Hard
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;

namespace DSA_CSharp.DynamicProgramming
{
    public class MinimumWindowSubsequence
    {
        /// <summary>
        /// Returns the shortest substring in s1 that contains s2 as a subsequence.
        /// If no such substring exists, returns an empty string.
        /// Time: O(n * m), Space: O(1)
        /// </summary>
        public string Solution(string s1, string s2)
        {
            if (string.IsNullOrEmpty(s1) || string.IsNullOrEmpty(s2) || s2.Length > s1.Length)
            {
                return string.Empty;
            }

            int n = s1.Length;
            int m = s2.Length;
            int bestStart = -1;
            int bestLength = int.MaxValue;

            int i = 0;

            while (i < n)
            {
                if (s1[i] != s2[0])
                {
                    i++;
                    continue;
                }

                // Forward pass: find an end index where s2 is matched as subsequence.
                int s1Index = i;
                int s2Index = 0;

                while (s1Index < n && s2Index < m)
                {
                    if (s1[s1Index] == s2[s2Index])
                    {
                        s2Index++;
                    }

                    s1Index++;
                }

                if (s2Index < m)
                {
                    break;
                }

                int end = s1Index - 1;

                // Backward pass: tighten start for this fixed end.
                s2Index = m - 1;
                int start = end;

                while (s2Index >= 0)
                {
                    if (s1[start] == s2[s2Index])
                    {
                        s2Index--;
                    }

                    start--;
                }

                start++;

                int currentLength = end - start + 1;

                if (currentLength < bestLength)
                {
                    bestLength = currentLength;
                    bestStart = start;
                }

                // Continue from next index after this start to keep leftmost tie behavior.
                i = start + 1;
            }

            return bestStart == -1 ? string.Empty : s1.Substring(bestStart, bestLength);
        }

        // Test method
        public static void Test()
        {
            var solution = new MinimumWindowSubsequence();

            Console.WriteLine("=== Minimum Window Subsequence Test Cases ===\n");

            string s1a = "hellointerview";
            string s2a = "her";
            Console.WriteLine($"Test 1 Output: \"{solution.Solution(s1a, s2a)}\"");
            Console.WriteLine("Expected: \"hellointer\"\n");

            string s1b = "codingisfun";
            string s2b = "xyz";
            Console.WriteLine($"Test 2 Output: \"{solution.Solution(s1b, s2b)}\"");
            Console.WriteLine("Expected: \"\"\n");

            string s1c = "abcdebdde";
            string s2c = "bde";
            Console.WriteLine($"Test 3 Output: \"{solution.Solution(s1c, s2c)}\"");
            Console.WriteLine("Expected: \"bcde\"\n");

            string s1d = "abc";
            string s2d = "abcd";
            Console.WriteLine($"Test 4 Output: \"{solution.Solution(s1d, s2d)}\"");
            Console.WriteLine("Expected: \"\"");
        }
    }
}