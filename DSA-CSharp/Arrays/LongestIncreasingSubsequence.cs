/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Longest Increasing Subsequence (LIS)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
Given an integer array nums, return the length of the longest strictly
increasing subsequence.

A subsequence is a sequence that can be derived by deleting some or no elements
without changing the order of the remaining elements. The subsequence must be
strictly increasing: nums[i1] < nums[i2] < ...

EXAMPLE:
Input: nums = [10, 9, 2, 5, 3, 7, 101, 18]
Output: 4
Explanation: [2, 3, 7, 101] or [2, 3, 7, 18] is a valid LIS.

APPROACH: Patience Sorting / Binary Search

We maintain a tails array where tails[len] stores the smallest possible tail
value of an increasing subsequence of length len + 1.

For each number x in nums:
- If x is greater than all tails, append it.
- Otherwise, replace the first tail >= x with x.

This keeps tails as small as possible, which maximizes the future ability to
build longer subsequences.

This is the key optimization: instead of checking all previous values, we find
where x should go using binary search in O(log n) time.

WHY THIS WORKS:
For a given subsequence length, the smallest possible tail value is always the
best choice to keep future subsequences flexible. If a value is too large for
the current ending, it cannot help extend a shorter subsequence; replacing the
first valid tail keeps the sequence as "small" as possible for that length.

TIME COMPLEXITY: O(n log n)
- Each element does one binary search and at most one update.

SPACE COMPLEXITY: O(n)
- The tails array stores the best ending values for different LIS lengths.

PATTERN: Dynamic Programming + Binary Search
DIFFICULTY: Medium
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;
using System.Collections.Generic;

namespace DSA_CSharp.Arrays
{
    public class LongestIncreasingSubsequence
    {
        /// <summary>
        /// Returns the length of the longest strictly increasing subsequence.
        /// Optimized approach: O(n log n) using patience sorting.
        /// </summary>
        public int Solution(int[] nums)
        {
            if (nums == null || nums.Length == 0)
            {
                return 0;
            }

            List<int> tails = new List<int>();

            foreach (int num in nums)
            {
                int left = 0;
                int right = tails.Count;

                while (left < right)
                {
                    int mid = left + (right - left) / 2;

                    if (tails[mid] < num)
                    {
                        left = mid + 1;
                    }
                    else
                    {
                        right = mid;
                    }
                }

                if (left == tails.Count)
                {
                    tails.Add(num);
                }
                else
                {
                    tails[left] = num;
                }
            }

            return tails.Count;
        }

        // Test method
        public static void Test()
        {
            var solution = new LongestIncreasingSubsequence();

            Console.WriteLine("=== Longest Increasing Subsequence Test Cases ===\n");

            int[] nums1 = { 10, 9, 2, 5, 3, 7, 101, 18 };
            Console.WriteLine($"Test 1 Output: {solution.Solution(nums1)}");
            Console.WriteLine("Expected: 4\n");

            int[] nums2 = { 0, 1, 0, 3, 2, 3 };
            Console.WriteLine($"Test 2 Output: {solution.Solution(nums2)}");
            Console.WriteLine("Expected: 4\n");

            int[] nums3 = { 7, 7, 7, 7 };
            Console.WriteLine($"Test 3 Output: {solution.Solution(nums3)}");
            Console.WriteLine("Expected: 1\n");

            int[] nums4 = { 1, 3, 6, 7, 9, 4, 10, 5, 6 };
            Console.WriteLine($"Test 4 Output: {solution.Solution(nums4)}");
            Console.WriteLine("Expected: 6");
        }
    }
}
