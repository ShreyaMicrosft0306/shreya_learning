// ============================================================
// PROBLEM: Max Sum of Distinct Subarrays Length K
// ============================================================
// Given an integer array and an integer k, find the maximum sum
// of any subarray of length exactly k where all elements are unique.
// Return 0 if no such subarray exists.
//
// INPUT:
//   int[] nums — array of integers (can include negatives, duplicates)
//   int k      — required subarray length
//   Example: nums = [3, 2, 2, 3, 4, 6, 7, 7, -1], k = 4
//
// OUTPUT:
//   long — maximum sum of a valid (all-distinct, length-k) subarray, or 0
//   Example: 20   (subarray [3, 4, 6, 7])
//
// EDGE CASES:
//   - All duplicates [5,5,5,5], k=3    → no valid window → return 0
//   - k = 1                            → every single element is valid → return max element
//   - Negative elements in valid window → still counted, return actual sum
//   - k = nums.Length with all distinct → only one window, return total sum
//
// ============================================================
// HOW TO SOLVE — Simple Walkthrough
// ============================================================
// Key Insight (Sliding Window + Frequency Map):
//   Slide a window of size k across the array. Track how many elements
//   are currently DISTINCT using a frequency map. When the window is
//   full and distinctCount == k, all elements are unique — record the sum.
//
//   Frequency map tells us two things:
//     Adding:   if freq[num] was 0, a new distinct element entered → distinctCount++
//     Removing: if freq[num] becomes 0, a distinct element left   → distinctCount--
//
// Step-by-step with example: nums = [3, 2, 2, 3, 4, 6, 7, 7, -1], k = 4
//
//   end=0: add 3 → freq={3:1}, distinct=1, sum=3,  size=1  (not k)
//   end=1: add 2 → freq={3:1,2:1}, distinct=2, sum=5,  size=2
//   end=2: add 2 → freq={3:1,2:2}, distinct=2, sum=7,  size=3  (2 already existed)
//   end=3: add 3 → freq={3:2,2:2}, distinct=2, sum=10, size=4=k
//                  distinct(2) ≠ k(4) → invalid
//
//   end=4: add 4 → freq={3:2,2:2,4:1}, distinct=3, sum=14, size=5 > k
//          remove nums[start=0]=3: freq={3:1,2:2,4:1}, distinct=3, sum=11, start=1
//          size=4=k, distinct(3) ≠ k(4) → invalid
//
//   end=5: add 6 → freq={3:1,2:2,4:1,6:1}, distinct=4, sum=17, size=5 > k
//          remove nums[start=1]=2: freq={3:1,2:1,4:1,6:1}, distinct=4, sum=15, start=2
//          size=4=k, distinct(4) == k(4) → VALID   max=15
//                    window = [2, 3, 4, 6]
//
//   end=6: add 7 → freq={3:1,2:1,4:1,6:1,7:1}, distinct=5, sum=22, size=5 > k
//          remove nums[start=2]=2: freq={3:1,4:1,6:1,7:1}, distinct=4, sum=20, start=3
//          size=4=k, distinct(4) == k(4) → VALID   max=20
//                    window = [3, 4, 6, 7]
//
//   end=7: add 7 → freq={3:1,4:1,6:1,7:2}, distinct=4, sum=27, size=5 > k
//          remove nums[start=3]=3: freq={4:1,6:1,7:2}, distinct=3, sum=24, start=4
//          size=4=k, distinct(3) ≠ k(4) → invalid  [4, 6, 7, 7]
//
//   end=8: add -1 → freq={4:1,6:1,7:2,-1:1}, distinct=4, sum=23, size=5 > k
//          remove nums[start=4]=4: freq={6:1,7:2,-1:1}, distinct=3, sum=19, start=5
//          size=4=k, distinct(3) ≠ k(4) → invalid  [6, 7, 7, -1]
//
//   Return: 20
//
// WHY TRACK distinctCount SEPARATELY INSTEAD OF CHECKING THE MAP EACH TIME?
//   Checking if all values in the map equal 1 every window would cost O(k) per step.
//   Maintaining a running distinctCount means each add/remove is O(1).
//   Total stays O(n) instead of O(n*k).
//
// WHY THIS APPROACH?
//   Brute force: check every subarray of size k with a HashSet to verify uniqueness
//   → O(n * k). For large k and n this is slow.
//   Sliding window + frequency map (this): each element is added once and removed once,
//   and distinctCount is updated in O(1) → Total: O(n).
//
// ============================================================
// TIME & SPACE COMPLEXITY
// ============================================================
//   n = number of elements in nums
//   k = required subarray length
//
//   Time:  O(n)
//     - Each element is added to the window exactly once: O(n)
//     - Each element is removed from the window at most once: O(n)
//     - All map operations (get, set) are O(1) amortized
//     - Total: O(n)
//
//   Space: O(k)
//     - Frequency map holds at most k+1 elements at any point
//       (we remove the leftmost before checking, so window stays size k)
//     - Total: O(k)
// ============================================================

public class MaxSumDistinctSubarrayK
{
    public static long Solve(int[] nums, int k)
    {
        var freq = new Dictionary<int, int>();
        long windowSum = 0;
        long maxSum = 0;
        bool foundValid = false;

        for (int i = 0; i < nums.Length; i++)
        {
            // Add right element
            freq[nums[i]] = freq.GetValueOrDefault(nums[i]) + 1;
            windowSum += nums[i];

            // Remove left element once window exceeds k
            // nums[i - k] is always the element that just fell off the left
            if (i >= k)
            {
                int left = nums[i - k];
                if (--freq[left] == 0) freq.Remove(left);  // remove key entirely when count hits 0
                windowSum -= left;
            }

            // freq.Count == number of distinct elements currently in the window
            // valid when window is exactly size k and every element is unique
            if (i >= k - 1 && freq.Count == k)
            {
                foundValid = true;
                maxSum = Math.Max(maxSum, windowSum);
            }
        }

        return foundValid ? maxSum : 0;
    }

    public static void Main(string[] args)
    {
        int[] nums1 = { 3, 2, 2, 3, 4, 6, 7, 7, -1 };
        Console.WriteLine("Example 1: " + Solve(nums1, 4));
        // Expected: 20  ([3, 4, 6, 7])

        int[] nums2 = { 5, 5, 5, 5, 5 };
        Console.WriteLine("Example 2: " + Solve(nums2, 3));
        // Expected: 0  (no valid window)

        int[] nums3 = { 1, 2, 3, 4 };
        Console.WriteLine("Example 3: " + Solve(nums3, 4));
        // Expected: 10  (entire array, all distinct)

        int[] nums4 = { 4, 4, 4, 1, 2, 3 };
        Console.WriteLine("Example 4: " + Solve(nums4, 3));
        // Expected: 6  ([1, 2, 3])
    }
}
