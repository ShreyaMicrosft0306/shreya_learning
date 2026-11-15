/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Valid Palindrome
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
Given a string, determine whether it is a palindrome after:
1. Converting uppercase letters to lowercase.
2. Removing all non-alphanumeric characters.

INPUT:
- s: string - Printable ASCII characters

OUTPUT:
- bool - True if the cleaned string reads the same forward and backward

EXAMPLES:
Example 1:
Input: s = "A man, a plan, a canal: Panama"
Output: true
Explanation: The cleaned string is "amanaplanacanalpanama".

Example 2:
Input: s = "race a car"
Output: false
Explanation: The cleaned string is "raceacar", which is not a palindrome.

Example 3:
Input: s = " "
Output: true
Explanation: Removing the space leaves an empty string, which is a palindrome.

CONSTRAINTS:
- 1 <= s.Length <= 200,000
- s contains printable ASCII characters

EDGE CASES:
✓ Empty cleaned string
✓ One alphanumeric character
✓ Only punctuation and spaces
✓ Mixed uppercase and lowercase letters
✓ Numbers in the string

TRICK CASES:
⚡ Ignore punctuation and spaces while comparing.
⚡ Compare letters without case sensitivity.
⚡ Digits are alphanumeric and must not be skipped.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OPTIMAL SOLUTION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APPROACH:
Use two pointers:
- left starts at the beginning.
- right starts at the end.

Move each pointer past non-alphanumeric characters. Then compare the two
characters after converting them to lowercase.

If they differ, the string is not a palindrome. If they match, move both
pointers inward and continue.

No cleaned copy of the string is created.

ALGORITHM STEPS:
1. Set left = 0 and right = s.Length - 1.
2. Skip non-alphanumeric characters from both ends.
3. Compare the lowercase characters.
4. If they differ, return false.
5. Move both pointers inward.
6. Return true when the pointers meet.

TIME COMPLEXITY: O(n)
- Each pointer moves through the string at most once.

SPACE COMPLEXITY: O(1)
- Only two pointers are used.

PATTERN: Two Pointers
DIFFICULTY: Easy
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;

namespace DSA_CSharp.TwoPointers
{
    public class ValidPalindrome
    {
        /// <summary>
        /// Returns true if s is a valid palindrome after cleaning it.
        /// </summary>
        public bool IsPalindrome(string s)
        {
            int left = 0;
            int right = s.Length - 1;

            while (left < right)
            {
                while (left < right && !char.IsLetterOrDigit(s[left]))
                    left++;

                while (left < right && !char.IsLetterOrDigit(s[right]))
                    right--;

                if (char.ToLowerInvariant(s[left]) != char.ToLowerInvariant(s[right]))
                    return false;

                left++;
                right--;
            }

            return true;
        }
    }
}
