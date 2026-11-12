/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Minimum Window Substring
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
Given two strings s and t, return the minimum window substring of s such that
every character in t (including duplicates) is included in the window.

If no such window exists, return an empty string.

EXAMPLE:
Input: s = "ADOBECODEBANC", t = "ABC"
Output: "BANC"

APPROACH:
Sliding Window.

We maintain a window in s that we try to expand and shrink.
- need[c] = how many times character c appears in t
- window[c] = how many times c appears in the current window
- required = number of character types that must appear in the window
- formed = how many character types are currently satisfied

Move the right pointer to expand the window.
Whenever a character count reaches the needed count, we mark that character as
satisfied. Once all required characters are satisfied, we try to shrink the
window from the left to find the minimum valid length.

TIME COMPLEXITY: O(m + n)
- Each pointer moves at most the length of s.

SPACE COMPLEXITY: O(1)
- We use fixed-size frequency arrays for characters.

PATTERN: Sliding Window
DIFFICULTY: Hard
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;

namespace DSA_CSharp.Strings
{
    public class MinimumWindowSubstring
    {
        /// <summary>
        /// Returns the minimum window substring of s that contains all characters
        /// in t, including duplicates.
        /// </summary>
        public string Solution(string s, string t)
        {
            if (string.IsNullOrEmpty(s) || string.IsNullOrEmpty(t) || t.Length > s.Length)
            {
                return string.Empty;
            }

            int[] need = new int[128];
            int[] window = new int[128];

            int required = 0;
            foreach (char c in t)
            {
                if (need[c] == 0)
                {
                    required++;
                }

                need[c]++;
            }

            int formed = 0;
            int left = 0;
            int minLen = int.MaxValue;
            int minStart = 0;

            for (int right = 0; right < s.Length; right++)
            {
                char c = s[right];
                window[c]++;

                if (window[c] == need[c])
                {
                    formed++;
                }

                while (formed == required)
                {
                    int currentLen = right - left + 1;
                    if (currentLen < minLen)
                    {
                        minLen = currentLen;
                        minStart = left;
                    }

                    char leftChar = s[left];
                    window[leftChar]--;

                    if (window[leftChar] < need[leftChar])
                    {
                        formed--;
                    }

                    left++;
                }
            }

            return minLen == int.MaxValue ? string.Empty : s.Substring(minStart, minLen);
        }

        public static void Test()
        {
            var solution = new MinimumWindowSubstring();

            Console.WriteLine("=== Minimum Window Substring Test Cases ===\n");

            string s1 = "ADOBECODEBANC";
            string t1 = "ABC";
            Console.WriteLine($"Test 1 Output: {solution.Solution(s1, t1)}");
            Console.WriteLine("Expected: BANC\n");

            string s2 = "a";
            string t2 = "a";
            Console.WriteLine($"Test 2 Output: {solution.Solution(s2, t2)}");
            Console.WriteLine("Expected: a\n");

            string s3 = "a";
            string t3 = "aa";
            Console.WriteLine($"Test 3 Output: {solution.Solution(s3, t3)}");
            Console.WriteLine("Expected: \"\"\n");

            string s4 = "ab";
            string t4 = "b";
            Console.WriteLine($"Test 4 Output: {solution.Solution(s4, t4)}");
            Console.WriteLine("Expected: b");
        }
    }
}
