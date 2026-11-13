using System;
using System.Collections.Generic;

namespace DSA_CSharp.Hashing
{
    // ============================================================
    // PROBLEM: Contains Duplicate
    // ============================================================
    // Given an integer array nums, return true if any value appears at
    // least twice in the array, and false if every element is distinct.
    //
    // EXAMPLE:
    // Input: nums = [1, 2, 3, 1]
    // Output: true
    //
    // APPROACH:
    // Use a HashSet to remember numbers we have seen so far.
    // As we iterate the array:
    // - if a number is already in the set, then it is duplicated
    // - otherwise, add it to the set and continue
    //
    // This gives us O(n) average time because set lookup and insert are O(1).
    //
    // TIME COMPLEXITY: O(n)
    // SPACE COMPLEXITY: O(n)
    // ============================================================
    public class ContainsDuplicate
    {
        public bool Solution(int[] nums)
        {
            if (nums == null)
            {
                return false;
            }

            HashSet<int> seen = new HashSet<int>();

            foreach (int num in nums)
            {
                if (!seen.Add(num))
                {
                    return true;
                }
            }

            return false;
        }

        public static void Test()
        {
            var solution = new ContainsDuplicate();

            Console.WriteLine("=== Contains Duplicate Test Cases ===\n");

            int[] nums1 = { 1, 2, 3, 1 };
            Console.WriteLine($"Test 1 Output: {solution.Solution(nums1)}");
            Console.WriteLine("Expected: True\n");

            int[] nums2 = { 1, 2, 3, 4 };
            Console.WriteLine($"Test 2 Output: {solution.Solution(nums2)}");
            Console.WriteLine("Expected: False\n");

            int[] nums3 = { 1, 1, 1, 3, 3, 4, 3, 2, 4, 2 };
            Console.WriteLine($"Test 3 Output: {solution.Solution(nums3)}");
            Console.WriteLine("Expected: True\n");

            int[] nums4 = { -1, -2, -3, -4 };
            Console.WriteLine($"Test 4 Output: {solution.Solution(nums4)}");
            Console.WriteLine("Expected: False");
        }
    }
}
