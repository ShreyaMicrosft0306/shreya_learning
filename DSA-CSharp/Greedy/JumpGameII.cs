// ============================================================
// PROBLEM: Jump Game II
// ============================================================
// Given an array nums where nums[i] is the maximum jump length from
// index i, return the fewest jumps needed to reach the final index.
// You may assume the final index is always reachable.
//
// INPUT:
//   int[] nums - maximum jump distance available from each index
//   Example: nums = [3, 4, 2, 1, 2, 1]
//
// OUTPUT:
//   int - minimum number of jumps needed to reach nums.Length - 1
//   Example: 2
//
// EDGE CASES:
//   - Single stone [0] -> return 0 because we are already at the end
//   - First jump reaches the end [5,1,1] -> return 1
//   - Only one step possible each time [1,1,1,1] -> return 3
//   - Large jumps later [1,4,1,1,1] -> take 0->1, then 1->4 -> return 2
//
// ============================================================
// HOW TO SOLVE - Simple Walkthrough
// ============================================================
// Building Intuition:
//   A jump does not need to decide the exact landing stone immediately.
//   Instead, look at all stones reachable using the current number of
//   jumps, then pick the next range that reaches farthest.
//
// Think in Levels:
//   Treat each jump count like a BFS level:
//     level 0: index 0
//     level 1: every index reachable from index 0
//     level 2: every index reachable from those level 1 indexes
//   Once we finish scanning the current level, we must spend one more
//   jump to move into the farthest next level we discovered.
//
// The Greedy Strategy:
//   Keep two boundaries while scanning left to right:
//     currentJumpEnd = farthest index reachable with the jumps already used
//     farthestReach  = farthest index reachable by adding one more jump
//   Every time the scan reaches currentJumpEnd, we have finished the
//   current level, so increment jumps and move currentJumpEnd to
//   farthestReach.
//
// Walkthrough:
//   Step-by-step with example: nums = [3, 4, 2, 1, 2, 1]
//
//   1. Start at index 0. jumps = 0, currentJumpEnd = 0, farthestReach = 0.
//   2. index 0: farthestReach = max(0, 0 + 3) = 3.
//      index 0 equals currentJumpEnd, so use jump 1 and set currentJumpEnd = 3.
//   3. index 1: farthestReach = max(3, 1 + 4) = 5.
//   4. index 2: farthestReach = max(5, 2 + 2) = 5.
//   5. index 3: farthestReach = max(5, 3 + 1) = 5.
//      index 3 equals currentJumpEnd, so use jump 2 and set currentJumpEnd = 5.
//   6. currentJumpEnd now reaches the last index, so return 2.
//
// Solution:
//   Scan only through the second-to-last index. We never need to jump
//   from the final index; reaching it is enough.
//
// Comparison with Jump Game I:
//   Jump Game I asks whether the end can be reached, so tracking a single
//   farthest reachable index is enough. Jump Game II asks for the minimum
//   jump count, so we also track where the current jump range ends and
//   count a jump each time that range is fully scanned.
//
// WHY THIS APPROACH?
//   A naive BFS could enqueue every reachable next index from every stone,
//   which may check O(n^2) edges. This greedy scan keeps the same level
//   idea but compresses each level into two numbers: the current boundary
//   and the farthest next boundary. That gives the minimum jump count in
//   one pass.
//
// ============================================================
// TIME & SPACE COMPLEXITY
// ============================================================
//   n = number of stones in nums
//
//   Time:  O(n)
//     - The loop visits each index from 0 to n - 2 at most once
//     - Each visit does constant-time max and boundary checks
//     - Total: O(n)
//
//   Space: O(1)
//     - Only jumps, currentJumpEnd, and farthestReach are stored
//     - No queue, recursion stack, or extra array is used
//     - Total: O(1)
// ============================================================

public class JumpGameII
{
    public static int Solve(int[] nums)
    {
        if (nums.Length <= 1)
            return 0;

        int jumps = 0;
        int currentJumpEnd = 0;
        int farthestReach = 0;

        for (int index = 0; index < nums.Length - 1; index++)
        {
            farthestReach = Math.Max(farthestReach, index + nums[index]);

            if (index == currentJumpEnd)
            {
                jumps++;
                currentJumpEnd = farthestReach;

                if (currentJumpEnd >= nums.Length - 1)
                    break;
            }
        }

        return jumps;
    }

    public static void Main(string[] args)
    {
        Console.WriteLine("Example 1: " + Solve(new[] { 3, 4, 2, 1, 2, 1 }));
        // Expected: 2

        Console.WriteLine("Example 2: " + Solve(new[] { 1, 2, 1, 1, 1 }));
        // Expected: 3

        Console.WriteLine("Example 3: " + Solve(new[] { 0 }));
        // Expected: 0

        Console.WriteLine("Example 4: " + Solve(new[] { 1, 4, 1, 1, 1 }));
        // Expected: 2
    }
}