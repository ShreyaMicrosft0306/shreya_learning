// ============================================================
// PROBLEM: Maximum Sum of Subarrays of Size K
// ============================================================
// Given an array of integers and an integer k, find the maximum
// sum of any contiguous subarray that is exactly k elements long.
//
// INPUT:
//   int[] nums — array of integers
//   int k      — fixed size of the subarray window
//   Example: nums = [2, 1, 5, 1, 3, 2], k = 3
//
// OUTPUT:
//   int — maximum sum among all subarrays of size k
//   Example: 9   (subarray [5, 1, 3])
//
// EDGE CASES:
//   - k equals array length [2,3,1], k=3  → only one window: sum of all = 6
//   - k = 1                               → return the maximum single element
//   - All negative numbers [-3,-1,-2], k=2 → return least negative: -1+(-2) = -3
//   - All same elements [4,4,4,4], k=2    → every window has same sum: 8
//
// ============================================================
// HOW TO SOLVE — Simple Walkthrough
// ============================================================
// Key Insight (Sliding Window):
//   Instead of recomputing the sum of k elements from scratch for each
//   window (which is wasteful), SLIDE the window one step at a time:
//     - Add the new element entering the window on the right
//     - Remove the old element leaving the window on the left
//   One addition and one subtraction per step instead of k additions.
//
// Visualised:
//   nums = [2, 1, 5, 1, 3, 2],  k = 3
//
//   Window 1: [2, 1, 5] . . .       sum = 8
//   Window 2:  . [1, 5, 1] . .      sum = 8 - 2 + 1 = 7
//   Window 3:  . . [5, 1, 3] .      sum = 7 - 1 + 3 = 9  ← max
//   Window 4:  . . . [1, 3, 2]      sum = 9 - 5 + 2 = 6
//
// Step-by-step with example: nums = [2, 1, 5, 1, 3, 2], k = 3
//
//   Step 1 — Build first window (sum first k elements):
//     windowSum = 2 + 1 + 5 = 8,  maxSum = 8
//
//   Step 2 — Slide the window from index k to end:
//     i=3: add nums[3]=1, remove nums[3-k]=nums[0]=2
//          windowSum = 8 - 2 + 1 = 7,  maxSum = max(8,7) = 8
//
//     i=4: add nums[4]=3, remove nums[4-k]=nums[1]=1
//          windowSum = 7 - 1 + 3 = 9,  maxSum = max(8,9) = 9
//
//     i=5: add nums[5]=2, remove nums[5-k]=nums[2]=5
//          windowSum = 9 - 5 + 2 = 6,  maxSum = max(9,6) = 9
//
//   Return: 9
//
// WHY THIS APPROACH?
//   Brute force: for each of the (n-k+1) windows, sum k elements → O(n*k).
//   For large k (e.g. k=1000, n=100000) this becomes very slow.
//   Sliding window (this): each element is added once and removed once.
//   Total work = n additions + n subtractions = O(n), regardless of k.
//
// ============================================================
// TIME & SPACE COMPLEXITY
// ============================================================
//   n = number of elements in nums
//   k = window size
//
//   Time:  O(n)
//     - Building the first window: O(k)
//     - Sliding across remaining (n-k) positions: O(n-k)
//     - Total: O(k) + O(n-k) = O(n)
//
//   Space: O(1)
//     - Only three integer variables: windowSum, maxSum, loop index i
//     - No extra arrays or data structures used
// ============================================================

public class MaxSumSubarraySizeK
{
    public static int Solve(int[] nums, int k)
    {
        // Step 1: Build the first window
        int windowSum = 0;
        for (int i = 0; i < k; i++)
            windowSum += nums[i];

        int maxSum = windowSum;

        // Step 2: Slide the window — add next element, drop first element of previous window
        for (int i = k; i < nums.Length; i++)
        {
            windowSum += nums[i] - nums[i - k];
            maxSum = Math.Max(maxSum, windowSum);
        }

        return maxSum;
    }

    public static void Main(string[] args)
    {
        int[] nums1 = { 2, 1, 5, 1, 3, 2 };
        Console.WriteLine("Example 1: " + Solve(nums1, 3));
        // Expected: 9  ([5, 1, 3])

        int[] nums2 = { 1, 4, 2, 10, 23, 3, 1, 0, 20 };
        Console.WriteLine("Example 2: " + Solve(nums2, 4));
        // Expected: 39  ([4, 2, 10, 23])

        int[] nums3 = { -3, -1, -2 };
        Console.WriteLine("Example 3: " + Solve(nums3, 2));
        // Expected: -3  ([-1, -2])
    }
}
