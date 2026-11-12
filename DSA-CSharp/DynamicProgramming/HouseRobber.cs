/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: House Robber
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
You are a professional robber planning to rob houses along a street. Each
house has a certain amount of money stashed.

But there is a major constraint: adjacent houses have connected security systems,
so you cannot rob both adjacent houses in the same night.

Return the maximum amount of money you can rob without alerting the police.

EXAMPLE:
Input: nums = [1,2,3,1]
Output: 4
Explanation: Rob house 2 and 4, total = 2 + 1 = 3. Or rob 1 and 3, total = 4.

APPROACH:
Dynamic Programming.

Let dp[i] = maximum money you can rob up to house i.

Transition:
- Either rob house i and skip i - 1
- Or skip house i and keep the previous best

So:
- dp[i] = max(dp[i - 1], nums[i] + dp[i - 2])

Why this works:
At each house, there are only two choices:
1. Skip it and keep the best from the previous house.
2. Rob it, but then you must skip the previous house.

Take the better of the two.

TIME COMPLEXITY: O(n)
SPACE COMPLEXITY: O(1) extra space if we keep only the previous two values

PATTERN: Dynamic Programming / House Robber
DIFFICULTY: Easy
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;

namespace DSA_CSharp.DynamicProgramming
{
    public class HouseRobber
    {
        /// <summary>
        /// Returns the maximum amount robbable without robbing adjacent houses.
        /// Uses O(1) extra space.
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

            int prev2 = nums[0];
            int prev1 = Math.Max(nums[0], nums[1]);

            for (int i = 2; i < nums.Length; i++)
            {
                int current = Math.Max(prev1, nums[i] + prev2);
                prev2 = prev1;
                prev1 = current;
            }

            return prev1;
        }

        public static void Test()
        {
            var solution = new HouseRobber();

            Console.WriteLine("=== House Robber Test Cases ===\n");

            int[] nums1 = { 1, 2, 3, 1 };
            Console.WriteLine($"Test 1 Output: {solution.Solution(nums1)}");
            Console.WriteLine("Expected: 4\n");

            int[] nums2 = { 2, 7, 9, 3, 1 };
            Console.WriteLine($"Test 2 Output: {solution.Solution(nums2)}");
            Console.WriteLine("Expected: 12\n");

            int[] nums3 = { 1, 1, 1, 1 };
            Console.WriteLine($"Test 3 Output: {solution.Solution(nums3)}");
            Console.WriteLine("Expected: 2");
        }
    }
}
