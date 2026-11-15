/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Meeting Rooms
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
Given an array of meeting time intervals, determine whether a person can
attend every meeting without attending two meetings at the same time.

Each interval is [start, end), so a meeting ending at time 5 does not overlap
with a meeting starting at time 5.

INPUT:
- intervals: int[][] - Meeting intervals [start, end]

OUTPUT:
- bool - True if no meetings overlap; otherwise false

EXAMPLES:
Example 1:
Input: intervals = [[0, 30], [5, 10], [15, 20]]
Output: false
Explanation: [0, 30] overlaps with both other meetings.

Example 2:
Input: intervals = [[7, 10], [2, 4]]
Output: true
Explanation: The meetings do not overlap.

Example 3:
Input: intervals = [[0, 5], [5, 10], [10, 15]]
Output: true
Explanation: A meeting ending at 5 does not overlap with one starting at 5.

CONSTRAINTS:
- 0 <= intervals.Length <= 10^4
- intervals[i].Length == 2
- intervals[i][0] < intervals[i][1]

EDGE CASES:
✓ Empty list -> true
✓ One meeting -> true
✓ Meetings already sorted
✓ Meetings provided in an unsorted order
✓ Meetings that touch at endpoints -> no overlap
✓ Duplicate or overlapping meetings

TRICK CASES:
⚡ Sort by start time before checking overlaps.
⚡ Compare the current start with the previous end.
⚡ Use current start < previous end for a real overlap.
⚡ current start == previous end is allowed.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OPTIMAL SOLUTION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APPROACH:
Sort all meetings by their start time. After sorting, an overlap can only
occur between a meeting and the meeting immediately before it.

For each meeting:
- If its start is earlier than the previous meeting's end, they overlap.
- Otherwise, continue checking.

ALGORITHM STEPS:
1. Sort intervals by start time.
2. Compare each meeting with the previous meeting.
3. If current start < previous end, return false.
4. If no overlap is found, return true.

TIME COMPLEXITY: O(n log n)
- Sorting takes O(n log n).
- The scan takes O(n).

SPACE COMPLEXITY: O(1) EXTRA SPACE
- Only a few variables are used besides the sorting implementation.

PATTERN: Sorting, Intervals
DIFFICULTY: Easy
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;

namespace DSA_CSharp.Intervals
{
    public class MeetingRooms
    {
        /// <summary>
        /// Returns true when all meetings can be attended without overlap.
        /// </summary>
        public bool CanAttendMeetings(int[][] intervals)
        {
            if (intervals == null || intervals.Length <= 1)
                return true;

            Array.Sort(intervals, (first, second) =>
                first[0].CompareTo(second[0]));

            for (int i = 1; i < intervals.Length; i++)
            {
                if (intervals[i][0] < intervals[i - 1][1])
                    return false;
            }

            return true;
        }
    }
}
