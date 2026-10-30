// ============================================================
// PROBLEM: Search in Rotated Sorted Array
// ============================================================
// A sorted array was rotated at an unknown pivot. Given the rotated
// array and a target, return the index of the target or -1 if absent.
// Must run in O(log n) — no linear scan allowed.
//
// A rotation of [0,1,2,4,5,6,7] at pivot index 4 gives [4,5,6,7,0,1,2].
//
// INPUT:
//   int[] nums — rotated sorted array with no duplicates
//   int target — value to search for
//   Example: nums = [4,5,6,7,0,1,2],  target = 0
//
// OUTPUT:
//   int — index of target, or -1 if not found
//   Example: 4
//
// EDGE CASES:
//   - No rotation [1,2,3,4,5], target=3 → standard binary search, return 2
//   - Target is at pivot point          → found normally during search
//   - Target not in array               → return -1
//   - Single element [5], target=5      → return 0
//   - Single element [5], target=1      → return -1
//
// ============================================================
// HOW TO SOLVE — Simple Walkthrough
// ============================================================
// Key Insight:
//   Even in a rotated array, ONE HALF is always fully sorted.
//   Check which half is sorted by comparing nums[lo] and nums[mid].
//   Then check if the target falls in that sorted half — if yes,
//   search it; if no, search the other half.
//   This eliminates half the array every step → O(log n).
//
// Which half is sorted?
//   nums[lo] <= nums[mid] → LEFT  half [lo..mid] is sorted
//   nums[lo] >  nums[mid] → RIGHT half [mid..hi] is sorted
//
// Step-by-step with example: nums = [4,5,6,7,0,1,2],  target = 0
//                                     0 1 2 3 4 5 6
//
//   lo=0, hi=6
//   mid=3, nums[mid]=7
//   nums[0]=4 <= nums[3]=7 → LEFT [4,5,6,7] is sorted
//   Is target(0) in [4..7]? NO → search RIGHT → lo=4
//
//   lo=4, hi=6
//   mid=5, nums[mid]=1
//   nums[4]=0 <= nums[5]=1 → LEFT [0,1] is sorted
//   Is target(0) in [0..1]? YES → search LEFT → hi=5
//
//   lo=4, hi=5
//   mid=4, nums[mid]=0
//   nums[mid] == target → return 4  ✓
//
// Example 2: nums = [4,5,6,7,0,1,2],  target = 3
//
//   lo=0, hi=6, mid=3, nums[mid]=7
//   Left [4..7] sorted. Is 3 in [4..7]? NO → lo=4
//
//   lo=4, hi=6, mid=5, nums[mid]=1
//   Left [0..1] sorted. Is 3 in [0..1]? NO → lo=6
//
//   lo=6, hi=6, mid=6, nums[mid]=2
//   Left [2..2] sorted. Is 3 in [2..2]? NO → lo=7
//
//   lo=7 > hi=6 → return -1  ✓
//
// HOW TO CHECK IF TARGET IS IN THE SORTED HALF:
//   Left half sorted:  target >= nums[lo] && target <= nums[mid]  → search left
//   Right half sorted: target >= nums[mid] && target <= nums[hi]  → search right
//   Otherwise: search the other (unsorted) half
//
// WHY THIS APPROACH?
//   Linear scan: O(n) — too slow, doesn't use sorted structure.
//   Find pivot first, then binary search: O(log n) but two passes.
//   Single-pass binary search (this): O(log n) in one pass — optimal.
//   The key insight that makes it work: rotation always leaves one half intact.
//
// ============================================================
// TIME & SPACE COMPLEXITY
// ============================================================
//   n = number of elements in nums
//
//   Time:  O(log n)
//     - Each iteration eliminates half the remaining elements
//     - At most log₂(n) iterations before lo > hi or target found
//
//   Space: O(1)
//     - Only three integer variables: lo, hi, mid
// ============================================================

public class SearchRotatedSortedArray
{
    public static int Solve(int[] nums, int target)
    {
        int lo = 0;
        int hi = nums.Length - 1;

        while (lo <= hi)
        {
            int mid = lo + (hi - lo) / 2;

            if (nums[mid] == target) return mid;

            if (nums[lo] <= nums[mid])
            {
                // Left half [lo..mid] is sorted
                if (target >= nums[lo] && target < nums[mid])
                    hi = mid - 1;   // target in left sorted half
                else
                    lo = mid + 1;   // target in right half
            }
            else
            {
                // Right half [mid..hi] is sorted
                if (target > nums[mid] && target <= nums[hi])
                    lo = mid + 1;   // target in right sorted half
                else
                    hi = mid - 1;   // target in left half
            }
        }

        return -1;
    }

    public static void Main(string[] args)
    {
        Console.WriteLine("Example 1: " + Solve(new[] { 4, 5, 6, 7, 0, 1, 2 }, 0));
        // Expected: 4

        Console.WriteLine("Example 2: " + Solve(new[] { 4, 5, 6, 7, 0, 1, 2 }, 3));
        // Expected: -1

        Console.WriteLine("Example 3: " + Solve(new[] { 1 }, 1));
        // Expected: 0

        Console.WriteLine("Example 4: " + Solve(new[] { 1, 3 }, 3));
        // Expected: 1

        Console.WriteLine("Example 5: " + Solve(new[] { 3, 1 }, 1));
        // Expected: 1  (rotated)
    }
}
