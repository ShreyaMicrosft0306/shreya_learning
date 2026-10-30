// ============================================================
// PROBLEM: Sort Colors (Dutch National Flag)
// ============================================================
// Given an array containing only 0s, 1s, and 2s, sort it in-place
// so all 0s come first, then 1s, then 2s. No built-in sort allowed.
//
// INPUT:
//   int[] nums — array of integers, each value is 0, 1, or 2
//   Example: nums = [2, 1, 2, 0, 1, 0, 1, 0, 1]
//
// OUTPUT:
//   void — array is sorted in-place
//   Example: [0, 0, 0, 1, 1, 1, 1, 2, 2]
//
// EDGE CASES:
//   - Already sorted [0,1,2]     → unchanged
//   - All same value [1,1,1]     → unchanged
//   - Only two colors [2,0,2,0]  → [0,0,2,2]
//   - Single element [2]         → [2]
//   - Reverse sorted [2,2,1,0]   → [0,1,2,2]
//
// ============================================================
// HOW TO SOLVE — Simple Walkthrough
// ============================================================
// Key Insight (Dutch National Flag algorithm by Dijkstra):
//   Divide the array into four regions using three pointers:
//
//   [  0s  |  1s  |  unknown  |  2s  ]
//          ↑       ↑           ↑
//         low     mid         high
//
//   low  = next position where a 0 should go
//   mid  = current element being looked at
//   high = next position where a 2 should go
//
//   Loop while mid <= high, and apply these three rules:
//     nums[mid] == 0 → belongs in 0s region → swap with low, low++, mid++
//     nums[mid] == 1 → already in 1s region → mid++ (no swap needed)
//     nums[mid] == 2 → belongs in 2s region → swap with high, high--
//                      (do NOT mid++ — swapped element is unknown, check it next)
//
// Step-by-step with example: nums = [2, 1, 2, 0, 1, 0, 1, 0, 1]
//                                     0  1  2  3  4  5  6  7  8
//   low=0, mid=0, high=8
//
//   mid=0: nums[0]=2 → swap nums[0]↔nums[8] → [1,1,2,0,1,0,1,0,2], high=7
//   mid=0: nums[0]=1 → mid=1
//   mid=1: nums[1]=1 → mid=2
//   mid=2: nums[2]=2 → swap nums[2]↔nums[7] → [1,1,0,0,1,0,1,2,2], high=6
//   mid=2: nums[2]=0 → swap nums[2]↔nums[0] → [0,1,1,0,1,0,1,2,2], low=1, mid=3
//   mid=3: nums[3]=0 → swap nums[3]↔nums[1] → [0,0,1,1,1,0,1,2,2], low=2, mid=4
//   mid=4: nums[4]=1 → mid=5
//   mid=5: nums[5]=0 → swap nums[5]↔nums[2] → [0,0,0,1,1,1,1,2,2], low=3, mid=6
//   mid=6: nums[6]=1 → mid=7
//   mid=7 > high=6 → STOP
//
//   Result: [0, 0, 0, 1, 1, 1, 1, 2, 2]  ✓
//
// WHY DO WE NOT INCREMENT mid AFTER SWAPPING A 2?
//   When we swap nums[mid] with nums[high], the element that lands
//   at mid came from the "unknown" region — we haven't looked at it yet.
//   So we must examine it again before moving mid forward.
//   When we swap nums[mid] with nums[low], the element at low is always
//   a 1 (because low only advances past confirmed 1s), so it's safe to
//   move mid forward immediately.
//
// WHY THIS APPROACH?
//   Counting sort: count 0s, 1s, 2s, then overwrite array → O(n) time
//   but needs TWO passes over the array.
//   Dutch National Flag (this): O(n) time, single pass, O(1) space.
//   One pass is better — it handles the sort in the minimum possible work.
//
// ============================================================
// TIME & SPACE COMPLEXITY
// ============================================================
//   n = number of elements in nums
//
//   Time:  O(n)
//     - Single pass: mid starts at 0 and only moves forward
//     - high only moves backward, low only moves forward
//     - Each element is looked at at most twice (once by mid, once after a swap)
//     - Total: O(n)
//
//   Space: O(1)
//     - Only three integer pointers: low, mid, high
//     - All swaps done in-place — no extra array needed
// ============================================================

public class SortColors
{
    public static void Solve(int[] nums)
    {
        int left = 0;
        int i = 0;
        int right = nums.Length - 1;

        while (i <= right)
        {
            if (nums[i] == 0)
            {
                // 0 belongs at the front — swap with left boundary
                int temp = nums[i];
                nums[i] = nums[left];
                nums[left] = temp;
                left++;
                i++;
            }
            else if (nums[i] == 2)
            {
                // 2 belongs at the back — swap with right boundary
                // Do not advance i — the swapped element is unexamined
                int temp = nums[i];
                nums[i] = nums[right];
                nums[right] = temp;
                right--;
            }
            else
            {
                // 1 is already in the correct middle region
                i++;
            }
        }
    }

    public static void Main(string[] args)
    {
        int[] nums1 = { 2, 1, 2, 0, 1, 0, 1, 0, 1 };
        Solve(nums1);
        Console.WriteLine("Example 1: [" + string.Join(", ", nums1) + "]");
        // Expected: [0, 0, 0, 1, 1, 1, 1, 2, 2]

        int[] nums2 = { 2, 0, 2, 1, 1, 0 };
        Solve(nums2);
        Console.WriteLine("Example 2: [" + string.Join(", ", nums2) + "]");
        // Expected: [0, 0, 1, 1, 2, 2]

        int[] nums3 = { 1, 1, 1 };
        Solve(nums3);
        Console.WriteLine("Example 3: [" + string.Join(", ", nums3) + "]");
        // Expected: [1, 1, 1]
    }
}
