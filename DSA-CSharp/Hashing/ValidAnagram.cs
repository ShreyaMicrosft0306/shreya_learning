/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Valid Anagram
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
Given two strings s and t, return true if t is an anagram of s.

An anagram uses exactly the same characters with the same frequencies, but
the characters may be in a different order.

INPUT:
- s: string - First lowercase English string
- t: string - Second lowercase English string

OUTPUT:
- bool - True if t is an anagram of s; otherwise false

EXAMPLES:
Example 1:
Input: s = "anagram", t = "nagaram"
Output: true
Explanation: Both strings contain the same characters with the same counts.

Example 2:
Input: s = "rat", t = "car"
Output: false
Explanation: The strings have different character frequencies.

Example 3:
Input: s = "a", t = "aa"
Output: false
Explanation: The strings have different lengths.

CONSTRAINTS:
- 1 <= s.Length, t.Length <= 50,000
- s and t contain lowercase English letters

EDGE CASES:
✓ Different lengths -> false
✓ Same character repeated
✓ Empty count after matching all characters
✓ Strings with all unique characters

TRICK CASES:
⚡ Character order does not matter.
⚡ Character frequency does matter.
⚡ Check lengths first because anagrams must have equal lengths.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OPTIMAL SOLUTION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APPROACH:
Use an array of 26 counters.

1. Add one for each character in s.
2. Subtract one for each character in t.
3. If every counter is zero, the strings contain exactly the same characters.

For Unicode input, replace the fixed array with a Dictionary<char, int>
or another frequency map that can store characters beyond 'a' through 'z'.

TIME COMPLEXITY: O(n)
- We scan both strings once.

SPACE COMPLEXITY: O(1)
- The frequency array always has 26 entries.

PATTERN: Hashing, Frequency Counting
DIFFICULTY: Easy
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

namespace DSA_CSharp.Hashing
{
    public class ValidAnagram
    {
        /// <summary>
        /// Returns true when s and t contain the same characters with the same counts.
        /// </summary>
        public bool IsAnagram(string s, string t)
        {
            if (s.Length != t.Length)
                return false;

            int[] frequencies = new int[26];

            foreach (char character in s)
                frequencies[character - 'a']++;

            foreach (char character in t)
                frequencies[character - 'a']--;

            foreach (int frequency in frequencies)
            {
                if (frequency != 0)
                    return false;
            }

            return true;
        }
    }
}
