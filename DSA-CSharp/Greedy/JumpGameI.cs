// ============================================================
// PROBLEM: Jump Game I
// ============================================================
// Given an array nums where nums[i] is the maximum number of steps
// you can jump forward from index i, determine whether you can reach
// the final index.
//
// INPUT:
//   int[] nums — maximum jump length available at each index
//   Example: nums = [2, 3, 1, 1, 4]
//
// OUTPUT:
//   bool — true if the final index is reachable, otherwise false
//   Example: true
//
// EDGE CASES:
//   - Single element [0] → already at the end → true
//   - First element 0 with more elements [0,1] → false
//   - Zero in a position but later jump is unnecessary [2,0,0,4] → false
//   - Zero surrounded by enough reach [2,0,2,0,4] → true
//
// ============================================================
// HOW TO SOLVE — Simple Walkthrough
// ============================================================
// Key Insight:
//   We do not need to know every exact position we can reach. We only
//   need the farthest index reached so far. If that farthest position
//   is at least the current index, we can keep moving.
//
// Algorithm:
//   Track farthestReach.
//   Scan each index once.
//   If index is greater than farthestReach, a gap has been reached.
//   Otherwise, update farthestReach = max(farthestReach, index + nums[i]).
//   Return true if farthestReach reaches the final index.
//
// Step-by-step with example: nums = [2,3,1,1,4]
//
//   index 0: farthestReach = max(0, 0+2) = 2
//   index 1: farthestReach = max(2, 1+3) = 4
//   farthestReach is now 4, the final index
//   return true
//
//   No exact jump sequence is needed. Reaching index 1 is enough because
//   it can jump over the remaining positions to the final index.
//
// Example with a blocked jump: nums = [3,2,1,0,4]
//
//   index 0: farthestReach = 3
//   index 1: farthestReach = 3
//   index 2: farthestReach = 3
//   index 3: farthestReach remains 3
//   index 4 is not reachable because farthestReach never reaches 4
//   return false
//
// WHY THIS APPROACH?
//   Trying every possible jump from every index can produce exponential
//   or heavily overlapping searches. Tracking the farthest reachable
//   index checks every position in one pass and proves whether the end
//   can be reached.
//
// ============================================================
// TIME & SPACE COMPLEXITY
// ============================================================
//   n = number of elements in nums
//
//   Time: O(n)
//     - The array is scanned once
//     - Updating the farthest reachable index costs constant time
//
//   Space: O(1)
//     - Only farthestReach and the loop index are stored
// ============================================================

public class JumpGameI
{
    public static bool CanJump(int[] nums)
    {
        if (nums == null || nums.Length == 0)
            return false;

        int farthestReach = 0;
        int lastIndex = nums.Length - 1;

        for (int i = 0; i < nums.Length; i++)
        {
            if (i > farthestReach)
                return false;

            farthestReach = Math.Max(farthestReach, i + nums[i]);

            if (farthestReach >= lastIndex)
                return true;
        }

        return false;
    }

    public static void Main(string[] args)
    {
        Console.WriteLine("Example 1: " + CanJump(new[] { 2, 3, 1, 1, 4 }));
        // Expected: true

        Console.WriteLine("Example 2: " + CanJump(new[] { 3, 2, 1, 0, 4 }));
        // Expected: false

        Console.WriteLine("Example 3: " + CanJump(new[] { 0 }));
        // Expected: true

        Console.WriteLine("Example 4: " + CanJump(new[] { 2, 0, 0, 4 }));
        // Expected: false
    }
}
