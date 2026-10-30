// ============================================================
// PROBLEM: 3Sum (Three Sum)
// ============================================================
// Given an integer array, find all unique triplets of distinct
// indices i, j, k such that nums[i] + nums[j] + nums[k] == 0.
// The result must not contain duplicate triplets.
//
// INPUT:
//   int[] nums — array of integers (can include negatives, zeros, duplicates)
//   Example: nums = [-1, 0, 1, 2, -1, -1]
//
// OUTPUT:
//   IList<IList<int>> — all unique triplets that sum to zero
//   Example: [[-1,-1,2], [-1,0,1]]
//
// EDGE CASES:
//   - Fewer than 3 elements → return []  (can't form a triplet)
//   - All zeros [0,0,0] → return [[0,0,0]]
//   - All positive or all negative → return []  (impossible to sum to 0)
//   - Many duplicates [-1,-1,-1,2,2,2] → return [[-1,-1,2]] only once
//   - No valid triplets [1,2,3] → return []
//
// ============================================================
// HOW TO SOLVE — Simple Walkthrough
// ============================================================
// Key Insight:
//   Sort the array first. Then for each number at index i, the problem
//   reduces to finding TWO numbers in the rest of the array that sum to
//   -nums[i] (so all three add up to 0). Use two pointers (left and right)
//   starting from both ends of the remaining portion to find those pairs in O(n).
//   Sorting also puts duplicates next to each other, making it easy to skip them.
//
// Step-by-step with example: nums = [-1, 0, 1, 2, -1, -1]
//
//   Step 1 — Sort:
//     [-1, -1, -1, 0, 1, 2]
//      i=0   1    2  3  4  5
//
//   Step 2 — Outer loop: fix one number at index i, find a pair in the rest.
//
//   i=0, nums[i]=-1, need two numbers that sum to +1 (0 - (-1) = 1)
//     left=1 (-1), right=5 (2)
//     -1 + 2 = 1 ✓ → triplet found: [-1, -1, 2]
//                    move left++ and right-- to look for more pairs
//     left=2 (-1), right=4 (1)
//     -1 + 1 = 0 ✗ (need 1, got 0) → sum too small → left++
//     left=3 (0), right=4 (1)
//     0 + 1 = 1 ✓ → triplet found: [-1, 0, 1]
//                   move left++ and right--
//     left=4, right=3 → left > right → stop inner loop
//
//   i=1, nums[i]=-1 == nums[0]=-1 → SKIP (duplicate outer value)
//   i=2, nums[i]=-1 == nums[0]=-1 → SKIP (duplicate outer value)
//
//   i=3, nums[i]=0, need two numbers that sum to 0
//     left=4 (1), right=5 (2)
//     1 + 2 = 3 ✗ (need 0, got 3) → sum too big → right--
//     left=4, right=4 → left >= right → stop inner loop
//
//   i=4: not enough elements on the right for two pointers → stop
//
//   Return: [[-1,-1,2], [-1,0,1]]
//
// HOW DUPLICATE SKIPPING WORKS:
//   Outer: if nums[i] == nums[i-1], skip — we already tried this value as anchor.
//   Inner: after finding a triplet, advance left past any repeated value,
//          and retreat right past any repeated value, to avoid duplicate triplets.
//
// TWO POINTER MOVEMENT RULES (inside inner loop):
//   sum < 0 → left++   (need a bigger number on the left)
//   sum > 0 → right--  (need a smaller number on the right)
//   sum = 0 → record triplet, then move both inward and skip duplicates
//
// WHY THIS APPROACH?
//   Brute force: three nested loops checking all triplets → O(n³). Too slow.
//   HashSet approach: for each pair, check if -sum exists → O(n²) but tricky
//   to deduplicate without extra bookkeeping.
//   Sort + Two Pointers (this): O(n²) time, O(1) extra space, and deduplication
//   is handled naturally by skipping equal adjacent values after sorting.
//
// ============================================================
// TIME & SPACE COMPLEXITY
// ============================================================
//   n = number of elements in nums
//
//   Time:  O(n²)
//     - Sorting: O(n log n)
//     - Outer loop runs n times: O(n)
//     - For each i, two-pointer inner loop scans remaining elements: O(n)
//     - Combined: O(n) × O(n) = O(n²), which dominates O(n log n)
//     - Total: O(n²)
//
//   Space: O(1) extra  (excluding the output list)
//     - Sorting is done in-place
//     - Only a few integer variables used (i, left, right, sum)
//     - Output list size depends on the number of valid triplets (not extra space)
// ============================================================

public class ThreeSum
{
    public static IList<IList<int>> Solve(int[] nums)
    {
        Array.Sort(nums);
        var result = new List<IList<int>>();

        for (int i = 0; i < nums.Length - 2; i++)
        {
            // Skip duplicate values for the outer anchor
            if (i > 0 && nums[i] == nums[i - 1])
                continue;

            // Early exit: smallest possible sum is already > 0
            if (nums[i] > 0)
                break;

            int left = i + 1;
            int right = nums.Length - 1;

            while (left < right)
            {
                int sum = nums[i] + nums[left] + nums[right];

                if (sum == 0)
                {
                    result.Add(new List<int> { nums[i], nums[left], nums[right] });

                    // Skip duplicate values for left and right pointers
                    while (left < right && nums[left] == nums[left + 1]) left++;
                    while (left < right && nums[right] == nums[right - 1]) right--;

                    left++;
                    right--;
                }
                else if (sum < 0)
                {
                    left++;   // Need a larger number
                }
                else
                {
                    right--;  // Need a smaller number
                }
            }
        }

        return result;
    }

    public static void Main(string[] args)
    {
        int[] nums1 = { -1, 0, 1, 2, -1, -1 };
        var result1 = Solve(nums1);
        Console.WriteLine("Example 1:");
        foreach (var t in result1)
            Console.WriteLine("  [" + string.Join(", ", t) + "]");
        // Expected: [-1,-1,2] and [-1,0,1]

        int[] nums2 = { 0, 0, 0 };
        var result2 = Solve(nums2);
        Console.WriteLine("Example 2:");
        foreach (var t in result2)
            Console.WriteLine("  [" + string.Join(", ", t) + "]");
        // Expected: [0,0,0]

        int[] nums3 = { 1, 2, 3 };
        var result3 = Solve(nums3);
        Console.WriteLine("Example 3: " + (result3.Count == 0 ? "[]" : "non-empty"));
        // Expected: []
    }
}
