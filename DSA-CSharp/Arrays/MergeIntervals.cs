/*
 * Problem: Merge Intervals
 * 
 * Given an array of intervals where intervals[i] = [start_i, end_i], merge all overlapping 
 * intervals, and return an array of the non-overlapping intervals that cover all the intervals 
 * in the input.
 * 
 * Example 1:
 * Input: intervals = [[1,3],[2,6],[8,10],[15,18]]
 * Output: [[1,6],[8,10],[15,18]]
 * Explanation: Since intervals [1,3] and [2,6] overlap, merge them into [1,6].
 * 
 * Example 2:
 * Input: intervals = [[1,4],[4,5]]
 * Output: [[1,5]]
 * Explanation: Intervals [1,4] and [4,5] are considered overlapping.
 * 
 * Example 3:
 * Input: intervals = [[1,4],[0,4]]
 * Output: [[0,4]]
 * 
 * Approach:
 * 1. Sort intervals by start time
 * 2. Iterate through sorted intervals
 * 3. If current interval overlaps with the last merged interval, extend it
 * 4. Otherwise, add it as a new interval
 * 
 * Two intervals [a,b] and [c,d] overlap if: a <= d and c <= b
 * Or simply: c <= b (when sorted by start time)
 * 
 * Time Complexity: O(n log n) due to sorting
 * Space Complexity: O(n) for the result array
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace DSA_CSharp.Arrays
{
    public class MergeIntervals
    {
        // Approach 1: Sort and Merge
        public int[][] Merge(int[][] intervals)
        {
            if (intervals == null || intervals.Length == 0)
                return new int[0][];
            
            // Sort by start time
            Array.Sort(intervals, (a, b) => a[0].CompareTo(b[0]));
            
            var merged = new List<int[]>();
            int[] currentInterval = intervals[0];
            
            for (int i = 1; i < intervals.Length; i++)
            {
                int[] nextInterval = intervals[i];
                
                // Check if intervals overlap
                if (nextInterval[0] <= currentInterval[1])
                {
                    // Merge: extend the end time to the maximum
                    currentInterval[1] = Math.Max(currentInterval[1], nextInterval[1]);
                }
                else
                {
                    // No overlap: add current interval and move to next
                    merged.Add(currentInterval);
                    currentInterval = nextInterval;
                }
            }
            
            // Don't forget to add the last interval
            merged.Add(currentInterval);
            
            return merged.ToArray();
        }

        // Approach 2: Using LinkedList (alternative collection)
        public int[][] MergeWithLinkedList(int[][] intervals)
        {
            if (intervals == null || intervals.Length == 0)
                return new int[0][];
            
            Array.Sort(intervals, (a, b) => a[0].CompareTo(b[0]));
            
            var merged = new LinkedList<int[]>();
            merged.AddLast(intervals[0]);
            
            for (int i = 1; i < intervals.Length; i++)
            {
                int[] current = intervals[i];
                int[] last = merged.Last.Value;
                
                if (current[0] <= last[1])
                {
                    // Overlapping - update the last interval's end
                    last[1] = Math.Max(last[1], current[1]);
                }
                else
                {
                    // Non-overlapping - add new interval
                    merged.AddLast(current);
                }
            }
            
            return merged.ToArray();
        }

        // Approach 3: In-place modification (modifies input array)
        public int[][] MergeInPlace(int[][] intervals)
        {
            if (intervals == null || intervals.Length == 0)
                return new int[0][];
            
            Array.Sort(intervals, (a, b) => a[0].CompareTo(b[0]));
            
            int writeIndex = 0;
            
            for (int i = 1; i < intervals.Length; i++)
            {
                // Check if current interval overlaps with the last merged one
                if (intervals[i][0] <= intervals[writeIndex][1])
                {
                    // Merge by extending end time
                    intervals[writeIndex][1] = Math.Max(intervals[writeIndex][1], intervals[i][1]);
                }
                else
                {
                    // No overlap - move to next position
                    writeIndex++;
                    intervals[writeIndex] = intervals[i];
                }
            }
            
            // Create result array with only the merged intervals
            int[][] result = new int[writeIndex + 1][];
            Array.Copy(intervals, result, writeIndex + 1);
            
            return result;
        }

        // Helper method: Check if two intervals overlap
        private bool IsOverlapping(int[] a, int[] b)
        {
            return a[0] <= b[1] && b[0] <= a[1];
        }

        // Helper method: Insert a new interval into existing intervals and merge
        public int[][] Insert(int[][] intervals, int[] newInterval)
        {
            var result = new List<int[]>();
            int i = 0;
            
            // Add all intervals that come before newInterval
            while (i < intervals.Length && intervals[i][1] < newInterval[0])
            {
                result.Add(intervals[i]);
                i++;
            }
            
            // Merge overlapping intervals with newInterval
            while (i < intervals.Length && intervals[i][0] <= newInterval[1])
            {
                newInterval[0] = Math.Min(newInterval[0], intervals[i][0]);
                newInterval[1] = Math.Max(newInterval[1], intervals[i][1]);
                i++;
            }
            result.Add(newInterval);
            
            // Add remaining intervals
            while (i < intervals.Length)
            {
                result.Add(intervals[i]);
                i++;
            }
            
            return result.ToArray();
        }

        // Test cases
        public static void Test()
        {
            var solver = new MergeIntervals();
            
            // Test case 1
            Console.WriteLine("Test Case 1:");
            int[][] intervals1 = new int[][]
            {
                new int[] { 1, 3 },
                new int[] { 2, 6 },
                new int[] { 8, 10 },
                new int[] { 15, 18 }
            };
            Console.WriteLine("Input: " + FormatIntervals(intervals1));
            var result1 = solver.Merge(intervals1);
            Console.WriteLine("Output: " + FormatIntervals(result1));
            Console.WriteLine("Expected: [[1,6],[8,10],[15,18]]");
            Console.WriteLine();
            
            // Test case 2
            Console.WriteLine("Test Case 2:");
            int[][] intervals2 = new int[][]
            {
                new int[] { 1, 4 },
                new int[] { 4, 5 }
            };
            Console.WriteLine("Input: " + FormatIntervals(intervals2));
            var result2 = solver.Merge(intervals2);
            Console.WriteLine("Output: " + FormatIntervals(result2));
            Console.WriteLine("Expected: [[1,5]]");
            Console.WriteLine();
            
            // Test case 3: Unsorted input
            Console.WriteLine("Test Case 3 (Unsorted):");
            int[][] intervals3 = new int[][]
            {
                new int[] { 1, 4 },
                new int[] { 0, 4 }
            };
            Console.WriteLine("Input: " + FormatIntervals(intervals3));
            var result3 = solver.Merge(intervals3);
            Console.WriteLine("Output: " + FormatIntervals(result3));
            Console.WriteLine("Expected: [[0,4]]");
            Console.WriteLine();
            
            // Test case 4: No overlaps
            Console.WriteLine("Test Case 4 (No Overlaps):");
            int[][] intervals4 = new int[][]
            {
                new int[] { 1, 2 },
                new int[] { 3, 4 },
                new int[] { 5, 6 }
            };
            Console.WriteLine("Input: " + FormatIntervals(intervals4));
            var result4 = solver.Merge(intervals4);
            Console.WriteLine("Output: " + FormatIntervals(result4));
            Console.WriteLine("Expected: [[1,2],[3,4],[5,6]]");
            Console.WriteLine();
            
            // Test case 5: All merge into one
            Console.WriteLine("Test Case 5 (All Merge):");
            int[][] intervals5 = new int[][]
            {
                new int[] { 1, 4 },
                new int[] { 2, 5 },
                new int[] { 3, 6 }
            };
            Console.WriteLine("Input: " + FormatIntervals(intervals5));
            var result5 = solver.Merge(intervals5);
            Console.WriteLine("Output: " + FormatIntervals(result5));
            Console.WriteLine("Expected: [[1,6]]");
            Console.WriteLine();
            
            // Test case 6: Single interval
            Console.WriteLine("Test Case 6 (Single Interval):");
            int[][] intervals6 = new int[][]
            {
                new int[] { 1, 5 }
            };
            Console.WriteLine("Input: " + FormatIntervals(intervals6));
            var result6 = solver.Merge(intervals6);
            Console.WriteLine("Output: " + FormatIntervals(result6));
            Console.WriteLine("Expected: [[1,5]]");
            Console.WriteLine();
            
            // Test case 7: Insert interval
            Console.WriteLine("Test Case 7 (Insert Interval):");
            int[][] intervals7 = new int[][]
            {
                new int[] { 1, 3 },
                new int[] { 6, 9 }
            };
            int[] newInterval = new int[] { 2, 5 };
            Console.WriteLine("Intervals: " + FormatIntervals(intervals7));
            Console.WriteLine("New Interval: [" + newInterval[0] + "," + newInterval[1] + "]");
            var result7 = solver.Insert(intervals7, newInterval);
            Console.WriteLine("Output: " + FormatIntervals(result7));
            Console.WriteLine("Expected: [[1,5],[6,9]]");
        }
        
        private static string FormatIntervals(int[][] intervals)
        {
            if (intervals.Length == 0)
                return "[]";
            
            return "[" + string.Join(",", intervals.Select(i => $"[{i[0]},{i[1]}]")) + "]";
        }
    }
}
