/*
 * Problem: Longest Substring Without Repeating Characters
 * 
 * Given a string s, find the length of the longest substring without repeating characters.
 * eabcabecbb
 * Example 1:
 * Input: s = "abcabcbb"
 * Output: 3
 * Explanation: The answer is "abc", with the length of 3.
 * 
 * Example 2:
 * Input: s = "bbbbb"
 * Output: 1
 * Explanation: The answer is "b", with the length of 1.
 * 
 * Example 3:
 * Input: s = "pwwkew"
 * Output: 3
 * Explanation: The answer is "wke", with the length of 3.
 * Notice that the answer must be a substring, "pwke" is a subsequence and not a substring.
 * 
 * Approach:
 * Use sliding window technique with a hash map to track character positions.
 * When we find a duplicate, move the left pointer to skip the duplicate.
 * 
 * Time Complexity: O(n) where n is the length of the string
 * Space Complexity: O(min(m, n)) where m is the charset size
 */

using System;
using System.Collections.Generic;

namespace DSA_CSharp.Arrays
{
    public class LongestSubstringWithoutRepeatingCharacters
    {
        // Approach 1: Sliding Window with Dictionary
        // Track the last index where each character was seen
        public int LengthOfLongestSubstring(string s)
        {
            if (string.IsNullOrEmpty(s))
                return 0;
            
            var charIndex = new Dictionary<char, int>();
            int maxLength = 0;
            int left = 0;
            
            for (int right = 0; right < s.Length; right++)
            {
                char currentChar = s[right];
                
                // If character is already in the window, move left pointer
                if (charIndex.ContainsKey(currentChar) && charIndex[currentChar] >= left)
                {
                    left = charIndex[currentChar] + 1;
                }
                
                // Update the character's position
                charIndex[currentChar] = right;
                
                // Update max length
                maxLength = Math.Max(maxLength, right - left + 1);
            }
            
            return maxLength;
        }

        // Approach 2: Optimized Sliding Window with Array (for ASCII characters)
        // Faster for ASCII strings as array access is O(1) vs Dictionary lookup
        public int LengthOfLongestSubstringOptimized(string s)
        {
            if (string.IsNullOrEmpty(s))
                return 0;
            
            // Array to store last index of each ASCII character
            int[] charIndex = new int[128];
            Array.Fill(charIndex, -1);
            
            int maxLength = 0;
            int left = 0;
            
            for (int right = 0; right < s.Length; right++)
            {
                char currentChar = s[right];
                
                // If character was seen in current window, move left pointer
                if (charIndex[currentChar] >= left)
                {
                    left = charIndex[currentChar] + 1;
                }
                
                // Update the character's position
                charIndex[currentChar] = right;
                
                // Update max length
                maxLength = Math.Max(maxLength, right - left + 1);
            }
            
            return maxLength;
        }

        // Approach 3: Brute Force (for comparison)
        // Check all substrings - not recommended for large inputs
        public int LengthOfLongestSubstringBruteForce(string s)
        {
            if (string.IsNullOrEmpty(s))
                return 0;
            
            int maxLength = 0;
            
            for (int i = 0; i < s.Length; i++)
            {
                var seen = new HashSet<char>();
                int currentLength = 0;
                
                for (int j = i; j < s.Length; j++)
                {
                    if (seen.Contains(s[j]))
                        break;
                    
                    seen.Add(s[j]);
                    currentLength++;
                    maxLength = Math.Max(maxLength, currentLength);
                }
            }
            
            return maxLength;
        }

        // Helper method to return the actual substring (not just length)
        public string GetLongestSubstring(string s)
        {
            if (string.IsNullOrEmpty(s))
                return "";
            
            var charIndex = new Dictionary<char, int>();
            int maxLength = 0;
            int maxStart = 0;
            int left = 0;
            
            for (int right = 0; right < s.Length; right++)
            {
                char currentChar = s[right];
                
                if (charIndex.ContainsKey(currentChar) && charIndex[currentChar] >= left)
                {
                    left = charIndex[currentChar] + 1;
                }
                
                charIndex[currentChar] = right;
                
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
            var solver = new LongestSubstringWithoutRepeatingCharacters();
            
            // Test case 1
            string test1 = "abcabcbb";
            Console.WriteLine($"Test Case 1: \"{test1}\"");
            Console.WriteLine($"Length (Dictionary): {solver.LengthOfLongestSubstring(test1)}");
            Console.WriteLine($"Length (Optimized): {solver.LengthOfLongestSubstringOptimized(test1)}");
            Console.WriteLine($"Substring: \"{solver.GetLongestSubstring(test1)}\"");
            Console.WriteLine("Expected: 3 (\"abc\")");
            Console.WriteLine();
            
            // Test case 2
            string test2 = "bbbbb";
            Console.WriteLine($"Test Case 2: \"{test2}\"");
            Console.WriteLine($"Length (Dictionary): {solver.LengthOfLongestSubstring(test2)}");
            Console.WriteLine($"Length (Optimized): {solver.LengthOfLongestSubstringOptimized(test2)}");
            Console.WriteLine($"Substring: \"{solver.GetLongestSubstring(test2)}\"");
            Console.WriteLine("Expected: 1 (\"b\")");
            Console.WriteLine();
            
            // Test case 3
            string test3 = "pwwkew";
            Console.WriteLine($"Test Case 3: \"{test3}\"");
            Console.WriteLine($"Length (Dictionary): {solver.LengthOfLongestSubstring(test3)}");
            Console.WriteLine($"Length (Optimized): {solver.LengthOfLongestSubstringOptimized(test3)}");
            Console.WriteLine($"Substring: \"{solver.GetLongestSubstring(test3)}\"");
            Console.WriteLine("Expected: 3 (\"wke\" or \"kew\")");
            Console.WriteLine();
            
            // Test case 4: Empty string
            string test4 = "";
            Console.WriteLine($"Test Case 4: \"\" (empty)");
            Console.WriteLine($"Length: {solver.LengthOfLongestSubstring(test4)}");
            Console.WriteLine("Expected: 0");
            Console.WriteLine();
            
            // Test case 5: Single character
            string test5 = "a";
            Console.WriteLine($"Test Case 5: \"{test5}\"");
            Console.WriteLine($"Length: {solver.LengthOfLongestSubstring(test5)}");
            Console.WriteLine("Expected: 1");
            Console.WriteLine();
            
            // Test case 6: All unique characters
            string test6 = "abcdef";
            Console.WriteLine($"Test Case 6: \"{test6}\"");
            Console.WriteLine($"Length: {solver.LengthOfLongestSubstring(test6)}");
            Console.WriteLine($"Substring: \"{solver.GetLongestSubstring(test6)}\"");
            Console.WriteLine("Expected: 6 (\"abcdef\")");
            Console.WriteLine();
            
            // Test case 7: Complex case
            string test7 = "dvdf";
            Console.WriteLine($"Test Case 7: \"{test7}\"");
            Console.WriteLine($"Length: {solver.LengthOfLongestSubstring(test7)}");
            Console.WriteLine($"Substring: \"{solver.GetLongestSubstring(test7)}\"");
            Console.WriteLine("Expected: 3 (\"vdf\")");
            Console.WriteLine();
            
            // Test case 8: With spaces and special characters
            string test8 = "a b c a b c d";
            Console.WriteLine($"Test Case 8: \"{test8}\"");
            Console.WriteLine($"Length: {solver.LengthOfLongestSubstring(test8)}");
            Console.WriteLine($"Substring: \"{solver.GetLongestSubstring(test8)}\"");
            Console.WriteLine("Expected: 5 (\" abcd\" or \"bcd \")");
        }
    }
}
