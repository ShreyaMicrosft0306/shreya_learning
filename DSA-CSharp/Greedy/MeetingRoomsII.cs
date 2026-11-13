using System;
using System.Collections.Generic;
using System.Linq;

namespace DSA_CSharp.Greedy
{
    // ============================================================
    // PROBLEM: Meeting Rooms II
    // ============================================================
    // Given an array of meeting time intervals where each interval is
    // [start, end), find the minimum number of meeting rooms required.
    //
    // EXAMPLE:
    // Input: intervals = [[0,30],[5,10],[15,20]]
    // Output: 2
    //
    // APPROACH:
    // We sort all start times and all end times separately.
    // Then we use a sweep line:
    // - when a meeting starts, increase active meetings
    // - when a meeting ends, decrease active meetings
    // We track the maximum overlap seen at any moment.
    //
    // This is a standard greedy + sorting solution.
    //
    // TIME COMPLEXITY: O(n log n)
    // SPACE COMPLEXITY: O(n)
    // ============================================================
    public class MeetingRoomsII
    {
        public int MinMeetingRooms(int[][] intervals)
        {
            if (intervals == null || intervals.Length == 0)
            {
                return 0;
            }

            int[] starts = new int[intervals.Length];
            int[] ends = new int[intervals.Length];

            for (int i = 0; i < intervals.Length; i++)
            {
                starts[i] = intervals[i][0];
                ends[i] = intervals[i][1];
            }

            Array.Sort(starts);
            Array.Sort(ends);

            int rooms = 0;
            int endIndex = 0;

            for (int startIndex = 0; startIndex < starts.Length; startIndex++)
            {
                if (starts[startIndex] < ends[endIndex])
                {
                    rooms++;
                }
                else
                {
                    endIndex++;
                }
            }

            return rooms;
        }

        public static void Test()
        {
            var solution = new MeetingRoomsII();

            Console.WriteLine("=== Meeting Rooms II Test Cases ===\n");

            int[][] intervals1 = { new[] { 0, 30 }, new[] { 5, 10 }, new[] { 15, 20 } };
            Console.WriteLine($"Test 1 Output: {solution.MinMeetingRooms(intervals1)}");
            Console.WriteLine("Expected: 2\n");

            int[][] intervals2 = { new[] { 7, 10 }, new[] { 2, 4 } };
            Console.WriteLine($"Test 2 Output: {solution.MinMeetingRooms(intervals2)}");
            Console.WriteLine("Expected: 1\n");

            int[][] intervals3 = { new[] { 0, 5 }, new[] { 5, 10 }, new[] { 10, 15 } };
            Console.WriteLine($"Test 3 Output: {solution.MinMeetingRooms(intervals3)}");
            Console.WriteLine("Expected: 1\n");

            int[][] intervals4 = { new[] { 1, 5 }, new[] { 2, 7 }, new[] { 4, 6 }, new[] { 8, 9 } };
            Console.WriteLine($"Test 4 Output: {solution.MinMeetingRooms(intervals4)}");
            Console.WriteLine("Expected: 2");
        }
    }
}
