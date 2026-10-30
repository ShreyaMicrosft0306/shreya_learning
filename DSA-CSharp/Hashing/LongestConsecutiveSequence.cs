// ============================================================
// PROBLEM: Longest Consecutive Sequence
// ============================================================
// Given an unsorted array of integers, return the length of the
// longest sequence of consecutive integers (e.g. 1,2,3,4) present
// in the array. Must run in O(n) time — no sorting allowed.
//
// INPUT:
//   int[] nums — unsorted array of integers (may have duplicates, negatives)
//   Example: nums = [100, 4, 200, 1, 3, 2]
//
// OUTPUT:
//   int — length of the longest consecutive sequence
//   Example: 4   (sequence is [1, 2, 3, 4])
//
// EDGE CASES:
//   - Empty array [] → return 0
//   - Single element [5] → return 1
//   - All duplicates [1,1,1] → return 1  (duplicates don't extend a sequence)
//   - Negative numbers [-3,-2,-1,0] → return 4  (works the same as positives)
//   - Entire array is consecutive [1,2,3,4,5] → return 5
//
// ============================================================
// HOW TO SOLVE — Simple Walkthrough
// ============================================================
// Key Insight:
//   Put all numbers in a HashSet for O(1) lookups.
//   A consecutive sequence has exactly ONE starting point — the number
//   where (num - 1) is NOT in the set. Only start counting from there.
//   This avoids recounting the same sequence multiple times.
//
// Step-by-step with example: nums = [100, 4, 200, 1, 3, 2]
//
//   Step 1 — Build HashSet:
//     set = {100, 4, 200, 1, 3, 2}
//
//   Step 2 — For each number, check if it is a sequence START:
//     (a number is a start only if num-1 is NOT in the set)
//
//     num=100 → is 99 in set? No  → START → check 101? No  → length = 1
//     num=4   → is 3 in set?  Yes → SKIP  (3 is the real start, not 4)
//     num=200 → is 199 in set? No → START → check 201? No  → length = 1
//     num=1   → is 0 in set?  No  → START → check 2? Yes
//                                          → check 3? Yes
//                                          → check 4? Yes
//                                          → check 5? No   → length = 4
//     num=3   → is 2 in set?  Yes → SKIP
//     num=2   → is 1 in set?  Yes → SKIP
//
//   Step 3 — Track the max length seen: max(1, 1, 4) = 4
//
//   Return: 4
//
// WHY THIS APPROACH?
//   Naive: Sort the array, then scan for streaks → O(n log n). Violates constraint.
//   This approach: HashSet gives O(1) lookup. The "only start from sequence start"
//   rule ensures each number is visited at most TWICE total (once in the outer loop,
//   once in the inner while loop of its sequence's start). So the inner while loop
//   costs O(n) across all iterations combined, not O(n) per iteration.
//
// ============================================================
// TIME & SPACE COMPLEXITY
// ============================================================
//   n = number of elements in nums
//
//   Time:  O(n)
//     - Building the HashSet: O(n)
//     - Outer loop runs n times: O(n)
//     - Inner while loop: each number is followed at most once
//       (only from its sequence's true start), so across ALL outer
//       iterations the inner loop runs at most n times total → O(n)
//     - Total: O(n)
//
//   Space: O(n)
//     - HashSet stores at most n unique elements: O(n)
// ============================================================

public class LongestConsecutiveSequence
{
    public static int Solve(int[] nums)
    {
        // Step 1: Load all numbers into a HashSet for O(1) lookup
        var set = new HashSet<int>(nums);

        int longest = 0;

        foreach (int num in set)
        {
            // Step 2: Only start counting if this is the beginning of a sequence.
            // If num-1 exists, this number is in the middle of a sequence — skip it.
            if (!set.Contains(num - 1))
            {
                int current = num;
                int length = 1;

                // Step 3: Extend the sequence as far as it goes
                while (set.Contains(current + 1))
                {
                    current++;
                    length++;
                }

                // Step 4: Update the global max
                longest = Math.Max(longest, length);
            }
        }

        return longest;
    }

    public static void Main(string[] args)
    {
        int[] nums1 = { 100, 4, 200, 1, 3, 2 };
        Console.WriteLine("Example 1: " + Solve(nums1));
        // Expected: 4  (sequence: 1,2,3,4)

        int[] nums2 = { 0, 3, 7, 2, 5, 8, 4, 6, 0, 1 };
        Console.WriteLine("Example 2: " + Solve(nums2));
        // Expected: 9  (sequence: 0,1,2,3,4,5,6,7,8)

        int[] nums3 = { 1, 0, 1, 2 };
        Console.WriteLine("Example 3: " + Solve(nums3));
        // Expected: 3  (sequence: 0,1,2)
    }
}
