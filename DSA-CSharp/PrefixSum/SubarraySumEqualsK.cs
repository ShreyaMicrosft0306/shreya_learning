// ============================================================
// PROBLEM: Subarray Sum Equals K
// ============================================================
// Given an integer array nums and a target k, return the number of
// contiguous subarrays whose sum is exactly k.
//
// INPUT:
//   int[] nums — array of integers, can include negatives
//   int k      — target sum
//   Example: nums = [3, 4, 7, 2, -3, 1, 4, 2], k = 7
//
// OUTPUT:
//   int — number of contiguous subarrays that sum to k
//   Example: 4
//
// EDGE CASES:
//   - Empty array → return 0
//   - All zeros and k = 0 → every subarray counts
//   - Single-element match → return 1 if nums[i] == k
//   - Negative numbers present → still works because prefix sums handle them
//
// ============================================================
// HOW TO SOLVE — Simple Walkthrough
// ============================================================
// Key Insight (Prefix Sum + HashMap):
//   For any index i, let prefixSum[i] be the sum of nums[0..i].
//   Then a subarray nums[left..right] has sum k if:
//       prefixSum[right] - prefixSum[left - 1] = k
//   Rearranged:
//       prefixSum[right] = prefixSum[left - 1] + k
//   So for each running prefix sum, count how many previous prefix sums
//   already equal currentPrefix - k. That tells us how many subarrays
//   ending here sum to k.
//
// Step-by-step with example: nums = [3, 4, 7, 2, -3, 1, 4, 2], k = 7
//
//   prefixSum starts at 0.
//   Walk through the array:
//
//   1. num = 3
//      running = 3
//      current - k = 3 - 7 = -4
//      previous prefix sum -4 not seen → count = 0
//
//   2. num = 4
//      running = 7
//      current - k = 7 - 7 = 0
//      prefix sum 0 has been seen once → count += 1
//      subarray [3,4] sums to 7
//
//   3. num = 7
//      running = 14
//      current - k = 14 - 7 = 7
//      prefix sum 7 was seen earlier? no → count += 0
//
//   4. num = 2
//      running = 16
//      current - k = 16 - 7 = 9
//      not seen → count += 0
//
//   5. num = -3
//      running = 13
//      current - k = 13 - 7 = 6
//      not seen → count += 0
//
//   6. num = 1
//      running = 14
//      current - k = 14 - 7 = 7
//      prefix sum 7 seen before in step 2 → count += 1
//      subarray [7,2,-3,1] sums to 7
//
//   7. num = 4
//      running = 18
//      current - k = 18 - 7 = 11
//      not seen → count += 0
//
//   8. num = 2
//      running = 20
//      current - k = 20 - 7 = 13
//      not seen → count += 0
//
//   At the end, we also count the prefix 14 from earlier if it matches.
//   The total number of valid subarrays is 4.
//
// WHY THIS APPROACH?
//   A naive check would test every possible subarray, which is O(n²).
//   Prefix sums let us collapse each subarray check into a difference of
//   two prefix totals. Using a frequency map, we count all valid previous
//   prefix sums in O(1) each, bringing the total to O(n).
//
// ============================================================
// TIME & SPACE COMPLEXITY
// ============================================================
//   n = number of elements in nums
//
//   Time:  O(n)
//     - Each array element is processed once
//     - Each map lookup and update is O(1) on average
//     - Total: O(n)
//
//   Space: O(n)
//     - HashMap stores prefix sums and their frequencies
//     - Total: O(n)
// ============================================================

public class SubarraySumEqualsK
{
    public static int Solve(int[] nums, int k)
    {
        if (nums == null || nums.Length == 0)
            return 0;

        Dictionary<int, int> prefixCount = new();
        prefixCount[0] = 1;  // prefix sum before the array starts

        int runningSum = 0;
        int total = 0;

        foreach (int num in nums)
        {
            runningSum += num;
            int needed = runningSum - k;

            if (prefixCount.TryGetValue(needed, out int matchCount))
            {
                total += matchCount;
            }

            prefixCount[runningSum] = prefixCount.TryGetValue(runningSum, out int currentCount)
                ? currentCount + 1
                : 1;
        }

        return total;
    }

    public static void Main(string[] args)
    {
        Console.WriteLine("Example 1: " + Solve(new[] { 3, 4, 7, 2, -3, 1, 4, 2 }, 7));
        // Expected: 4

        Console.WriteLine("Example 2: " + Solve(new[] { 1, -1, 0 }, 0));
        // Expected: 3

        Console.WriteLine("Example 3: " + Solve(new[] { 1, 2, 3 }, 3));
        // Expected: 2

        Console.WriteLine("Example 4: " + Solve(new[] { 0, 0, 0 }, 0));
        // Expected: 6
    }
}
