/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Non-overlapping Intervals
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
Given an array of intervals, remove the minimum number of intervals so that
remaining intervals do not overlap.

An interval [a, b] and [c, d] are considered non-overlapping if they only touch
at a point, meaning b <= c or d <= a. In other words, [1,2] and [2,3] are okay.

EXAMPLE:
Input: intervals = [[1,2],[2,3],[3,4],[1,3]]
Output: 1
Explanation: Remove [1,3] and the rest do not overlap.

APPROACH:
Greedy.

1. Sort intervals by their end time.
2. Keep a variable prevEnd = -infinity.
3. For each interval in sorted order:
   - If current start >= prevEnd, this interval can stay and we update prevEnd.
   - Otherwise, this interval overlaps with the last kept interval, so remove it.
     We also count one removal.
4. To minimize removals, always keep the interval that ends earliest.

Why this works:
If we keep an interval that ends later, it blocks more future intervals. By
choosing the earliest ending interval, we leave the most room for the remaining
intervals and avoid unnecessary conflicts.

TIME COMPLEXITY: O(n log n)
- Sorting dominates the time.

SPACE COMPLEXITY: O(1) extra space
- Aside from the input array and the sorted order.

PATTERN: Greedy + Sorting
DIFFICULTY: Medium
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;

namespace DSA_CSharp.Arrays
{
    public class NonOverlappingIntervals
    {
        /// <summary>
        /// Returns the minimum number of intervals to remove so the remaining
        /// intervals are non-overlapping.
        /// </summary>
        public int Solution(int[][] intervals)
        {
            if (intervals == null || intervals.Length == 0)
            {
                return 0;
            }

            Array.Sort(intervals, (a, b) => a[1].CompareTo(b[1]));

            int removals = 0;
            int prevEnd = int.MinValue;

            foreach (int[] interval in intervals)
            {
                int start = interval[0];
                int end = interval[1];

                if (start >= prevEnd)
                {
                    prevEnd = end;
                }
                else
                {
                    removals++;
                    // We keep the current interval only if it is an earlier finish
                    // than the one we would otherwise keep. Since the array is sorted
                    // by end time, the current interval is the one we prefer to keep.
                    // To minimize removals, we simply skip the current interval.
                }
            }

            return removals;
        }

        // Test method
        public static void Test()
        {
            var solution = new NonOverlappingIntervals();

            Console.WriteLine("=== Non-overlapping Intervals Test Cases ===\n");

            int[][] intervals1 =
            {
                new[] { 1, 2 },
                new[] { 2, 3 },
                new[] { 3, 4 },
                new[] { 1, 3 }
            };
            Console.WriteLine($"Test 1 Output: {solution.Solution(intervals1)}");
            Console.WriteLine("Expected: 1\n");

            int[][] intervals2 =
            {
                new[] { 1, 2 },
                new[] { 1, 2 },
                new[] { 1, 2 }
            };
            Console.WriteLine($"Test 2 Output: {solution.Solution(intervals2)}");
            Console.WriteLine("Expected: 2\n");

            int[][] intervals3 =
            {
                new[] { 1, 2 },
                new[] { 2, 3 }
            };
            Console.WriteLine($"Test 3 Output: {solution.Solution(intervals3)}");
            Console.WriteLine("Expected: 0");
        }
    }
}
