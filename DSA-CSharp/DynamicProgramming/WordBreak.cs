/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Word Break
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
Given a string s and a dictionary of strings wordDict, determine whether s
can be segmented into a space-separated sequence of dictionary words.
Dictionary words may be reused.

INPUT:
- s: string - String to segment
- wordDict: IList<string> - Available dictionary words

OUTPUT:
- bool - true if s can be segmented completely, otherwise false

EXAMPLES:
Example 1:
Input: s = "leetcode", wordDict = ["leet", "code"]
Output: true
Explanation: "leetcode" = "leet" + "code"

Example 2:
Input: s = "applepenapple", wordDict = ["apple", "pen"]
Output: true
Explanation: "applepenapple" = "apple" + "pen" + "apple"

Example 3:
Input: s = "catsandog", wordDict = ["cats", "dog", "sand", "and", "cat"]
Output: false

CONSTRAINTS:
- 1 <= s.Length <= 300
- 1 <= wordDict.Count <= 1000
- 1 <= wordDict[i].Length <= 20
- s and wordDict[i] contain only lowercase English letters
- All dictionary strings are unique

EDGE CASES:
✓ Empty string → true
✓ One character matching one dictionary word
✓ Dictionary word longer than s
✓ Same dictionary word reused multiple times
✓ Valid prefix but an invalid remainder

TRICK CASES:
⚡ The previous prefix must end at a valid boundary
⚡ A word can start at many positions
⚡ Dictionary lookup must be fast
⚡ Repeated words are allowed

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OPTIMAL SOLUTION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APPROACH:
Use bottom-up Dynamic Programming.

Let dp[i] indicate whether the prefix s[0..i-1] can be segmented.
Base case dp[0] = true because an empty prefix can always be segmented.

For each prefix length i, use all valid earlier prefixes j where dp[j]
is true. If the substring s[j..i-1] exists in the dictionary, mark dp[i]
true.

KEY INSIGHTS:
• Store word membership in a HashSet for O(1) lookup
• Only the remaining substring needs checking
• dp[i] depends on earlier dp[j] values
• Once dp[n] is true, the complete string is segmentable

ALGORITHM STEPS:
1. Add every dictionary word to a HashSet
2. Create dp[n + 1], with dp[0] = true
3. For each end position i from 1 to n
4. For each start position j from 0 to i - 1
5. If dp[j] is true and s.Substring(j, i - j) is in the dictionary
6. Set dp[i] = true and stop checking start positions
7. Return dp[n]

TIME COMPLEXITY: O(n²)
- The solution checks O(n²) start/end pairs
- Dictionary substring membership is O(1) on average

SPACE COMPLEXITY: O(n)
- dp stores one result for each prefix
- The HashSet stores all dictionary words

MEMOIZED BACKTRACKING EXPLANATION:
Memoization saves the answer for each starting index.
- memo[start] = null means the position has not been checked yet
- memo[start] = true means s[start...] can be segmented
- memo[start] = false means s[start...] cannot be segmented

Example: s = "catsandog", wordDict = ["cats", "sand", "and", "dog"]
- Start at index 0. Try "cats"; if present, call index 4
- At index 4, try "and"; if present, call index 7
- At index 7, try "dog"; if present, call index 10
- index 10 equals s.Length, so the complete string is segmentable

When a start index has already been memoized, that method call returns
its saved result immediately instead of trying the same substrings again.

BACKTRACKING TIME: O(n²) with memoization, O(2^n) without it
BACKTRACKING SPACE: O(n)

PATTERN: Dynamic Programming - String Segmentation
DIFFICULTY: Medium
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;
using System.Collections.Generic;

namespace DSA_CSharp.DynamicProgramming
{
    public class WordBreak
    {
        /// <summary>
        /// Returns true if s can be segmented using words from wordDict.
        /// Time: O(n²), Space: O(n)
        /// </summary>
        public bool Solution(string s, IList<string> wordDict)
        {
            if (string.IsNullOrEmpty(s))
                return true;

            if (wordDict == null || wordDict.Count == 0)
                return false;

            var dictionary = new HashSet<string>(wordDict);
            int n = s.Length;
            var dp = new bool[n + 1];
            dp[0] = true;

            for (int end = 1; end <= n; end++)
            {
                for (int start = 0; start < end; start++)
                {
                    if (!dp[start])
                        continue;

                    string word = s.Substring(start, end - start);

                    if (dictionary.Contains(word))
                    {
                        dp[end] = true;
                        break;
                    }
                }
            }

            return dp[n];
        }

        /// <summary>
        /// Uses memoized backtracking to check each possible segmentation.
        /// Time: O(n²), Space: O(n)
        /// </summary>
        public bool SolutionWithMemo(string s, IList<string> wordDict)
        {
            if (string.IsNullOrEmpty(s))
                return true;

            if (wordDict == null || wordDict.Count == 0)
                return false;

            var dictionary = new HashSet<string>(wordDict);
            bool?[] memo = new bool?[s.Length];

            bool CanSegment(int start)
            {
                // The start index reached the end, so the complete string is valid.
                if (start == s.Length)
                    return true;

                // null means this start position has not been checked yet.
                if (memo[start].HasValue)
                    return memo[start].Value;

                for (int end = start + 1; end <= s.Length; end++)
                {
                    string word = s.Substring(start, end - start);

                    if (dictionary.Contains(word) && CanSegment(end))
                    {
                        // Store true because the remaining string is segmentable.
                        memo[start] = true;
                        return true;
                    }
                }

                // Store false so this start position is not explored again.
                memo[start] = false;
                return false;
            }

            return CanSegment(0);
        }

        public static void Test()
        {
            var solution = new WordBreak();

            Console.WriteLine("=== Word Break Test Cases ===\n");

            bool example1BottomUp = solution.Solution("leetcode", new[] { "leet", "code" });
            bool example1Memo = solution.SolutionWithMemo("leetcode", new[] { "leet", "code" });
            Console.WriteLine($"Example 1 - Bottom-up: {example1BottomUp}");
            Console.WriteLine($"Example 1 - Memoized: {example1Memo}");
            Console.WriteLine("Expected: true\n");

            bool example2BottomUp = solution.Solution("applepenapple", new[] { "apple", "pen" });
            bool example2Memo = solution.SolutionWithMemo("applepenapple", new[] { "apple", "pen" });
            Console.WriteLine($"Example 2 - Bottom-up: {example2BottomUp}");
            Console.WriteLine($"Example 2 - Memoized: {example2Memo}");
            Console.WriteLine("Expected: true\n");

            bool example3BottomUp = solution.Solution("catsandog", new[] { "cats", "dog", "sand", "and", "cat" });
            bool example3Memo = solution.SolutionWithMemo("catsandog", new[] { "cats", "dog", "sand", "and", "cat" });
            Console.WriteLine($"Example 3 - Bottom-up: {example3BottomUp}");
            Console.WriteLine($"Example 3 - Memoized: {example3Memo}");
            Console.WriteLine("Expected: false\n");

            bool reuseMemo = solution.SolutionWithMemo("aaaaaaa", new[] { "aaa", "aaaa" });
            Console.WriteLine($"Reuse word - Memoized: {reuseMemo}");
            Console.WriteLine("Expected: true\n");

            bool emptyMemo = solution.SolutionWithMemo("", new[] { "a" });
            Console.WriteLine($"Empty string - Memoized: {emptyMemo}");
            Console.WriteLine("Expected: true");
        }
    }
}
