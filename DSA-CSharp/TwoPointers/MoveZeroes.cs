// ============================================================
// PROBLEM: Move Zeroes
// ============================================================
// Given an integer array, move all zeros to the end while keeping
// the relative order of non-zero elements unchanged.
// Must be done in-place — no creating a copy of the array.
//
// INPUT:
//   int[] nums — array of integers that may contain zeros
//   Example: nums = [2, 0, 4, 0, 9]
//
// OUTPUT:
//   void — array is modified in-place
//   Example: [2, 4, 9, 0, 0]
//
// EDGE CASES:
//   - No zeros [1,2,3]       → unchanged [1,2,3]
//   - All zeros [0,0,0]      → unchanged [0,0,0]
//   - Single element [0]     → [0]
//   - Zeros already at end [1,2,0,0] → unchanged [1,2,0,0]
//   - Zero at the front [0,1,2]  → [1,2,0]
//
// ============================================================
// HOW TO SOLVE — Simple Walkthrough
// ============================================================
// Key Insight:
//   Use a single pointer called insertPos that tracks WHERE the next
//   non-zero element should be written. Walk through the array once,
//   and every time you see a non-zero number, write it at insertPos
//   and advance insertPos. After the walk, fill everything from
//   insertPos to the end with zeros.
//
// Think of it like packing a suitcase — you pull out all the real
// items first (non-zeros), pack them tightly at the front, then
// stuff the empty space (zeros) at the back.
//
// Step-by-step with example: nums = [2, 0, 4, 0, 9]
//
//   insertPos = 0
//
//   i=0: nums[0]=2  (non-zero) → write 2 at insertPos=0 → insertPos=1
//        array: [2, 0, 4, 0, 9]
//
//   i=1: nums[1]=0  (zero)     → skip
//        array: [2, 0, 4, 0, 9]
//
//   i=2: nums[2]=4  (non-zero) → write 4 at insertPos=1 → insertPos=2
//        array: [2, 4, 4, 0, 9]
//
//   i=3: nums[3]=0  (zero)     → skip
//        array: [2, 4, 4, 0, 9]
//
//   i=4: nums[4]=9  (non-zero) → write 9 at insertPos=2 → insertPos=3
//        array: [2, 4, 9, 0, 9]
//
//   First pass done. Non-zeros are packed at the front.
//   insertPos=3 marks where zeros should start.
//
//   Fill zeros from insertPos=3 to end:
//     nums[3] = 0
//     nums[4] = 0
//     array: [2, 4, 9, 0, 0]  ✓
//
// WHY THIS APPROACH?
//   Naive: collect non-zeros into a new array, copy back, fill zeros.
//     Works but uses O(n) extra space — violates the in-place requirement.
//   Swap approach: swap each non-zero with the element at insertPos.
//     Also O(n) time and O(1) space, but does more writes when zeros
//     are already at the end (unnecessary swaps).
//   Two-pass write (this solution): O(n) time, O(1) space.
//     Pass 1 writes non-zeros to the front (preserving order).
//     Pass 2 fills the tail with zeros. Minimal writes, cleanest logic.
//
// ============================================================
// TIME & SPACE COMPLEXITY
// ============================================================
//   n = number of elements in nums
//
//   Time:  O(n)
//     - Pass 1 (collect non-zeros): visits every element once → O(n)
//     - Pass 2 (fill zeros): fills at most n positions → O(n)
//     - Total: O(n)
//
//   Space: O(1)
//     - Only one extra integer variable (insertPos)
//     - No new arrays created — all modifications are in-place
// ============================================================

public class MoveZeroes
{
    public static void Solve(int[] nums)
    {
        // Pass 1: overwrite array from the front with non-zero elements only
        int insertPos = 0;
        foreach (int num in nums)
        {
            if (num != 0)
            {
                nums[insertPos] = num;
                insertPos++;
            }
        }

        // Pass 2: fill the remaining positions with zeros
        while (insertPos < nums.Length)
        {
            nums[insertPos] = 0;
            insertPos++;
        }
    }

    public static void Main(string[] args)
    {
        int[] nums1 = { 2, 0, 4, 0, 9 };
        Solve(nums1);
        Console.WriteLine("Example 1: [" + string.Join(", ", nums1) + "]");
        // Expected: [2, 4, 9, 0, 0]

        int[] nums2 = { 0, 0, 0 };
        Solve(nums2);
        Console.WriteLine("Example 2: [" + string.Join(", ", nums2) + "]");
        // Expected: [0, 0, 0]

        int[] nums3 = { 1, 2, 3 };
        Solve(nums3);
        Console.WriteLine("Example 3: [" + string.Join(", ", nums3) + "]");
        // Expected: [1, 2, 3]

        int[] nums4 = { 0, 1, 2 };
        Solve(nums4);
        Console.WriteLine("Example 4: [" + string.Join(", ", nums4) + "]");
        // Expected: [1, 2, 0]
    }
}
