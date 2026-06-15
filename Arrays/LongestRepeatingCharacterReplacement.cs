/*
 * Problem: Longest Repeating Character Replacement
 * 
 * You are given a string s and an integer k. You can choose any character of the string 
 * and change it to any other uppercase English character. You can perform this operation 
 * at most k times.
 * 
 * Return the length of the longest substring containing the same letter you can get after 
 * performing the above operations.
 * 
 * Example 1:
 * Input: s = "ABAB", k = 2
 * Output: 4
 * Explanation: Replace the two 'A's with two 'B's or vice versa to get "AAAA" or "BBBB".
 * 
 * Example 2:
 * Input: s = "AABABBA", k = 1
 * Output: 4
 * Explanation: Replace the one 'A' in the middle with 'B' and form "AABBBBA".
 * The substring "BBBB" has the longest repeating letters, which is 4.
 * 
 * Example 3:
 * Input: s = "ABBB", k = 2
 * Output: 4
 * Explanation: Replace 'A' with 'B' to get "BBBB".
 * 
 * Approach:
 * Use sliding window technique with character frequency tracking.
 * 
 * Key Insight:
 * For a valid window: window_length - max_frequency <= k
 * This means we can replace the non-majority characters (at most k of them) 
 * to make all characters the same as the most frequent one.
 * 
 * Time Complexity: O(n) where n is the length of the string
 * Space Complexity: O(26) = O(1) for tracking character frequencies
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace DSA_CSharp.Arrays
{
    public class LongestRepeatingCharacterReplacement
    {
        // Approach 1: Sliding Window with Frequency Map
        // Track frequency of each character in current window
        public int CharacterReplacement(string s, int k)
        {
            if (string.IsNullOrEmpty(s))
                return 0;
            
            var charCount = new Dictionary<char, int>();
            int maxLength = 0;
            int maxFreq = 0; // Frequency of most common character in current window
            int left = 0;
            
            for (int right = 0; right < s.Length; right++)
            {
                // Add current character to window AABABBA
                char rightChar = s[right];
                charCount[rightChar] = charCount.GetValueOrDefault(rightChar, 0) + 1;
                
                // Update max frequency in current window
                maxFreq = Math.Max(maxFreq, charCount[rightChar]);
                
                // Calculate number of characters that need to be replaced
                int windowLength = right - left + 1;
                int replacementsNeeded = windowLength - maxFreq;
                
                // If replacements needed > k, shrink window from left
                if (replacementsNeeded > k)
                {
                    char leftChar = s[left];
                    charCount[leftChar]--;
                    left++;
                    // Note: We don't update maxFreq here because we only care about
                    // finding windows where maxFreq is at its highest, and even if
                    // the character with maxFreq is removed, a smaller maxFreq won't
                    // give us a better result
                }
                
                // Update max length
                maxLength = Math.Max(maxLength, right - left + 1);
            }
            
            return maxLength;
        }

        // Approach 2: Optimized Sliding Window
        // Uses array instead of dictionary for O(1) access
        public int CharacterReplacementOptimized(string s, int k)
        {
            if (string.IsNullOrEmpty(s))
                return 0;
            
            int[] charCount = new int[26]; // For uppercase English letters
            int maxLength = 0;
            int maxFreq = 0;
            int left = 0;
            
            for (int right = 0; right < s.Length; right++)
            {
                // Add current character to window
                int rightIdx = s[right] - 'A';
                charCount[rightIdx]++;
                
                // Update max frequency
                maxFreq = Math.Max(maxFreq, charCount[rightIdx]);
                
                // Shrink window if invalid
                int windowLength = right - left + 1;
                if (windowLength - maxFreq > k)
                {
                    int leftIdx = s[left] - 'A';
                    charCount[leftIdx]--;
                    left++;
                }
                
                maxLength = Math.Max(maxLength, right - left + 1);
            }
            
            return maxLength;
        }

        // Approach 3: Alternative with explicit max frequency calculation
        // Recalculates maxFreq each time for clarity (slightly less efficient)
        public int CharacterReplacementExplicit(string s, int k)
        {
            if (string.IsNullOrEmpty(s))
                return 0;
            
            var charCount = new Dictionary<char, int>();
            int maxLength = 0;
            int left = 0;
            
            for (int right = 0; right < s.Length; right++)
            {
                char rightChar = s[right];
                charCount[rightChar] = charCount.GetValueOrDefault(rightChar, 0) + 1;
                
                // Calculate current max frequency
                int maxFreq = charCount.Values.Max();
                int windowLength = right - left + 1;
                
                // Shrink window while invalid
                while (windowLength - maxFreq > k)
                {
                    char leftChar = s[left];
                    charCount[leftChar]--;
                    if (charCount[leftChar] == 0)
                        charCount.Remove(leftChar);
                    left++;
                    windowLength = right - left + 1;
                    if (charCount.Count > 0)
                        maxFreq = charCount.Values.Max();
                }
                
                maxLength = Math.Max(maxLength, windowLength);
            }
            
            return maxLength;
        }

        // Helper method to visualize the solution
        public string GetLongestSubstring(string s, int k)
        {
            if (string.IsNullOrEmpty(s))
                return "";
            
            var charCount = new Dictionary<char, int>();
            int maxLength = 0;
            int maxStart = 0;
            int maxFreq = 0;
            int left = 0;
            
            for (int right = 0; right < s.Length; right++)
            {
                char rightChar = s[right];
                charCount[rightChar] = charCount.GetValueOrDefault(rightChar, 0) + 1;
                maxFreq = Math.Max(maxFreq, charCount[rightChar]);
                
                int windowLength = right - left + 1;
                if (windowLength - maxFreq > k)
                {
                    char leftChar = s[left];
                    charCount[leftChar]--;
                    left++;
                }
                
                int currentLength = right - left + 1;
                if (currentLength > maxLength)
                {
                    maxLength = currentLength;
                    maxStart = left;
                }
            }
            
            return s.Substring(maxStart, maxLength);
        }

        // Test cases
        public static void Test()
        {
            var solver = new LongestRepeatingCharacterReplacement();
            
            // Test case 1
            Console.WriteLine("Test Case 1:");
            string test1 = "ABAB";
            int k1 = 2;
            Console.WriteLine($"Input: s = \"{test1}\", k = {k1}");
            Console.WriteLine($"Output (Dictionary): {solver.CharacterReplacement(test1, k1)}");
            Console.WriteLine($"Output (Optimized): {solver.CharacterReplacementOptimized(test1, k1)}");
            Console.WriteLine($"Substring: \"{solver.GetLongestSubstring(test1, k1)}\"");
            Console.WriteLine("Expected: 4");
            Console.WriteLine();
            
            // Test case 2
            Console.WriteLine("Test Case 2:");
            string test2 = "AABABBA";
            int k2 = 1;
            Console.WriteLine($"Input: s = \"{test2}\", k = {k2}");
            Console.WriteLine($"Output (Dictionary): {solver.CharacterReplacement(test2, k2)}");
            Console.WriteLine($"Output (Optimized): {solver.CharacterReplacementOptimized(test2, k2)}");
            Console.WriteLine($"Substring: \"{solver.GetLongestSubstring(test2, k2)}\"");
            Console.WriteLine("Expected: 4");
            Console.WriteLine();
            
            // Test case 3
            Console.WriteLine("Test Case 3:");
            string test3 = "ABBB";
            int k3 = 2;
            Console.WriteLine($"Input: s = \"{test3}\", k = {k3}");
            Console.WriteLine($"Output: {solver.CharacterReplacement(test3, k3)}");
            Console.WriteLine($"Substring: \"{solver.GetLongestSubstring(test3, k3)}\"");
            Console.WriteLine("Expected: 4");
            Console.WriteLine();
            
            // Test case 4: All same characters
            Console.WriteLine("Test Case 4:");
            string test4 = "AAAA";
            int k4 = 2;
            Console.WriteLine($"Input: s = \"{test4}\", k = {k4}");
            Console.WriteLine($"Output: {solver.CharacterReplacement(test4, k4)}");
            Console.WriteLine("Expected: 4");
            Console.WriteLine();
            
            // Test case 5: k = 0 (no replacements allowed)
            Console.WriteLine("Test Case 5:");
            string test5 = "ABCDE";
            int k5 = 0;
            Console.WriteLine($"Input: s = \"{test5}\", k = {k5}");
            Console.WriteLine($"Output: {solver.CharacterReplacement(test5, k5)}");
            Console.WriteLine("Expected: 1");
            Console.WriteLine();
            
            // Test case 6: Complex case
            Console.WriteLine("Test Case 6:");
            string test6 = "AABABBA";
            int k6 = 2;
            Console.WriteLine($"Input: s = \"{test6}\", k = {k6}");
            Console.WriteLine($"Output: {solver.CharacterReplacement(test6, k6)}");
            Console.WriteLine($"Substring: \"{solver.GetLongestSubstring(test6, k6)}\"");
            Console.WriteLine("Expected: 5");
            Console.WriteLine();
            
            // Test case 7: Large k
            Console.WriteLine("Test Case 7:");
            string test7 = "ABCABC";
            int k7 = 10;
            Console.WriteLine($"Input: s = \"{test7}\", k = {k7}");
            Console.WriteLine($"Output: {solver.CharacterReplacement(test7, k7)}");
            Console.WriteLine("Expected: 6 (can replace all to same character)");
        }
    }
}
