/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: House Robber II
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
There are n houses arranged in a circle. You cannot rob two adjacent houses.
Return the maximum amount of money you can rob without alerting the police.

Unlike the standard House Robber, the first and last house are adjacent because
of the circular arrangement.

EXAMPLE:
Input: nums = [2,3,2]
Output: 3
Explanation: Rob house 2 only.

APPROACH:
Break the circle into two linear cases:

1. Rob the first house and skip the last one.
2. Skip the first house and rob the last one.

Then run the standard House Robber logic on each case and take the maximum.

Why this works:
In a circle, the first and last houses are adjacent, so they cannot both be
robbed. Therefore, any valid solution must choose one of these two patterns:
- either include house 0 and exclude house n - 1
- or exclude house 0 and include house n - 1

We compute the best solution for both linear versions and pick the better one.

TIME COMPLEXITY: O(n)
SPACE COMPLEXITY: O(1)

PATTERN: Dynamic Programming / House Robber
DIFFICULTY: Medium
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;

namespace DSA_CSharp.DynamicProgramming
{
    public class HouseRobberII
    {
        /// <summary>
        /// Returns the maximum amount robbable in a circular arrangement.
        /// </summary>
        public int Solution(int[] nums)
        {
            if (nums == null || nums.Length == 0)
            {
                return 0;
            }

            if (nums.Length == 1)
            {
                return nums[0];
            }

            int robFirst = RobLinear(nums, 0, nums.Length - 2);
            int robLast = RobLinear(nums, 1, nums.Length - 1);

            return Math.Max(robFirst, robLast);
        }

        private int RobLinear(int[] nums, int start, int end)
        {
            if (start > end)
            {
                return 0;
            }

            int prev2 = nums[start];
            int prev1 = Math.Max(nums[start], start + 1 <= end ? nums[start + 1] : nums[start]);

            for (int i = start + 2; i <= end; i++)
            {
                int current = Math.Max(prev1, nums[i] + prev2);
                prev2 = prev1;
                prev1 = current;
            }

            return prev1;
        }

        public static void Test()
        {
            var solution = new HouseRobberII();

            Console.WriteLine("=== House Robber II Test Cases ===\n");

            int[] nums1 = { 2, 3, 2 };
            Console.WriteLine($"Test 1 Output: {solution.Solution(nums1)}");
            Console.WriteLine("Expected: 3\n");

            int[] nums2 = { 1, 2, 3, 1 };
            Console.WriteLine($"Test 2 Output: {solution.Solution(nums2)}");
            Console.WriteLine("Expected: 4\n");

            int[] nums3 = { 1, 2, 3, 4, 5, 1, 2, 3, 4, 5 };
            Console.WriteLine($"Test 3 Output: {solution.Solution(nums3)}");
            Console.WriteLine("Expected: 16");
        }
    }
}
