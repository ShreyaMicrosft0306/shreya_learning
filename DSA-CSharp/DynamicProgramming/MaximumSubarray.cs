/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Maximum Subarray
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
Given an integer array nums, find the subarray with the largest sum and return
its sum.

A subarray is a contiguous segment of the array.

EXAMPLE:
Input: nums = [-2,1,-3,4,-1,2,1,-5,4]
Output: 6
Explanation: [4,-1,2,1] has the largest sum 6.

APPROACH: Kadane's Algorithm

We scan the array once and keep two running values:
- currentSum: the best sum of a subarray that ends at the current index
- maxSum: the best sum seen anywhere in the array so far

At each step:
- currentSum = max(nums[i], currentSum + nums[i])
- maxSum = max(maxSum, currentSum)

Why this works:
If the running sum so far is already negative, it is better to start fresh at
current index. Otherwise, extending the current subarray is useful.

TIME COMPLEXITY: O(n)
- One pass through the array.

SPACE COMPLEXITY: O(1)
- Only a few variables are stored.

PATTERN: Dynamic Programming / Greedy
DIFFICULTY: Medium
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;

namespace DSA_CSharp.DynamicProgramming
{
    public class MaximumSubarray
    {
        /// <summary>
        /// Returns the maximum sum of any contiguous subarray.
        /// Uses Kadane's algorithm: O(n), O(1)
        /// </summary>
        public int Solution(int[] nums)
        {
            if (nums == null || nums.Length == 0)
            {
                return 0;
            }

            int currentSum = nums[0];
            int maxSum = nums[0];

            for (int i = 1; i < nums.Length; i++)
            {
                currentSum = Math.Max(nums[i], currentSum + nums[i]);
                maxSum = Math.Max(maxSum, currentSum);
            }

            return maxSum;
        }

        public static void Test()
        {
            var solution = new MaximumSubarray();

            Console.WriteLine("=== Maximum Subarray Test Cases ===\n");

            int[] nums1 = { -2, 1, -3, 4, -1, 2, 1, -5, 4 };
            Console.WriteLine($"Test 1 Output: {solution.Solution(nums1)}");
            Console.WriteLine("Expected: 6\n");

            int[] nums2 = { 1 };
            Console.WriteLine($"Test 2 Output: {solution.Solution(nums2)}");
            Console.WriteLine("Expected: 1\n");

            int[] nums3 = { 5, 4, -1, 7, 8 };
            Console.WriteLine($"Test 3 Output: {solution.Solution(nums3)}");
            Console.WriteLine("Expected: 23\n");

            int[] nums4 = { -1, -2, -3 };
            Console.WriteLine($"Test 4 Output: {solution.Solution(nums4)}");
            Console.WriteLine("Expected: -1");
        }
    }
}
