/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Partition Labels
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
Given a string of lowercase letters, split it into as many segments as
possible so that every character appears in exactly one segment. Joining all
segments must produce the original string. Return the length of each segment.

INPUT:
- s: string - A string containing lowercase English letters

OUTPUT:
- IList<int> - The lengths of the segments in their original order

EXAMPLES:
Example 1:
Input: s = "abacbcdd"
Output: [6, 2]
Explanation: The first segment is "abacbc" and the second is "dd".
Every occurrence of a letter stays inside one segment.

Example 2:
Input: s = "eccbbbbdec"
Output: [10]
Explanation: The letters overlap across the string, so it cannot be split.

CONSTRAINTS:
- 0 <= s.Length
- s contains only lowercase English letters

EDGE CASES:
✓ Null or empty string -> empty result
✓ One character -> one segment of length 1
✓ Every character is unique -> every segment has length 1
✓ Every character is the same -> one segment containing the whole string

TRICK CASES:
⚡ A segment may grow when a character inside it appears farther to the right
⚡ Splitting after a character's last occurrence may still be too early
⚡ The goal is the maximum number of valid segments, not equal-sized segments

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OPTIMAL SOLUTION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

OBSERVATION:
If a segment contains a character, it must continue through that character's
last occurrence. While scanning the segment, another character may have an
even later last occurrence, so the segment boundary must expand to include it.
When the current index reaches the farthest required boundary, the segment can
end safely. Ending there creates the earliest valid cut and therefore the
largest possible number of segments.

PSEUDOCODE:
lastOccurrence = array of size 26
for each index in s:
    lastOccurrence[s[index]] = index

segmentStart = 0
segmentEnd = 0
for each index in s:
    segmentEnd = max(segmentEnd, lastOccurrence[s[index]])
    if index == segmentEnd:
        add segmentEnd - segmentStart + 1 to result
        segmentStart = index + 1

WALKTHROUGH:
For s = "abacbcdd", the last indices are a=2, b=4, c=5, and d=7.
Starting at index 0, 'a' requires reaching index 2. Seeing 'b' expands the
boundary to 4, and seeing 'c' expands it to 5. At index 5, the first segment
closes with length 6. The two 'd' characters form a second segment of length 2.

APPROACH:
First record the last index of every letter. Then scan the string while keeping
the farthest last occurrence required by the current segment. Whenever the
scan reaches that boundary, record the segment length and start a new segment.

KEY INSIGHTS:
• A segment cannot end before the last occurrence of any character inside it
• The maximum last occurrence seen so far is the earliest valid boundary
• Taking every earliest valid cut maximizes the number of segments

ALGORITHM STEPS:
1. Record the last index at which each character appears.
2. Set the current segment start and end to index 0.
3. Scan each character and extend the end to its last occurrence if necessary.
4. When the scan reaches the end, save the segment length.
5. Start the next segment immediately after that boundary.

TIME COMPLEXITY: O(n)
- The string is scanned twice.

SPACE COMPLEXITY: O(1)
- The last-occurrence array always contains exactly 26 entries.
- The returned list is output space.

PATTERN: Greedy / Last Occurrence
DIFFICULTY: Medium

WHEN TO USE THIS PATTERN:
Use this pattern when items must be grouped into non-overlapping contiguous
ranges and each item reveals the minimum right boundary its range must include.
It is especially useful when choosing the earliest valid boundary maximizes
the number of groups.
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;
using System.Collections.Generic;

namespace DSA_CSharp.Greedy
{
    public class PartitionLabels
    {
        /// <summary>
        /// Returns the lengths of the maximum number of valid segments.
        /// Time: O(n), Space: O(1), excluding the result.
        /// </summary>
        public IList<int> Solution(string s)
        {
            var segmentLengths = new List<int>();

            if (string.IsNullOrEmpty(s))
            {
                return segmentLengths;
            }

            var lastOccurrence = new int[26];

            for (int index = 0; index < s.Length; index++)
            {
                lastOccurrence[s[index] - 'a'] = index;
            }

            int segmentStart = 0;
            int segmentEnd = 0;

            for (int index = 0; index < s.Length; index++)
            {
                segmentEnd = Math.Max(segmentEnd, lastOccurrence[s[index] - 'a']);

                if (index == segmentEnd)
                {
                    segmentLengths.Add(segmentEnd - segmentStart + 1);
                    segmentStart = index + 1;
                }
            }

            return segmentLengths;
        }

        public static void Test()
        {
            var solution = new PartitionLabels();

            Console.WriteLine("=== Partition Labels Test Cases ===\n");
            Console.WriteLine($"Test 1 Output: [{string.Join(", ", solution.Solution("abacbcdd"))}]");
            Console.WriteLine("Expected: [6, 2]\n");

            Console.WriteLine($"Test 2 Output: [{string.Join(", ", solution.Solution("eccbbbbdec"))}]");
            Console.WriteLine("Expected: [10]\n");

            Console.WriteLine($"Test 3 Output: [{string.Join(", ", solution.Solution("abc"))}]");
            Console.WriteLine("Expected: [1, 1, 1]\n");

            Console.WriteLine($"Test 4 Output: [{string.Join(", ", solution.Solution("aaaa"))}]");
            Console.WriteLine("Expected: [4]");
        }
    }
}