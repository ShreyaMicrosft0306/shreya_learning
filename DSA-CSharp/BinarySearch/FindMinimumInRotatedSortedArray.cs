/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Find Minimum in Rotated Sorted Array
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
A sorted ascending array has been rotated at an unknown pivot. The array
contains unique elements. Find and return the minimum element.

INPUT:
- nums: int[] - Sorted array rotated between 1 and n times

OUTPUT:
- int - The minimum element in nums

EXAMPLES:
Example 1:
Input: nums = [3,4,5,1,2]
Output: 1

Example 2:
Input: nums = [4,5,6,7,0,1,2]
Output: 0

Example 3:
Input: nums = [11,13,15,17]
Output: 11

CONSTRAINTS:
- 1 <= nums.Length <= 5000
- -5000 <= nums[i] <= 5000
- All nums[i] values are unique
- nums is sorted and rotated

EDGE CASES:
✓ One element
✓ Array not rotated
✓ Minimum is first element
✓ Minimum is last element
✓ Minimum is at the middle

TRICK CASES:
⚡ Compare nums[mid] with nums[right], not nums[left]
⚡ If nums[mid] > nums[right], the minimum is to the right
⚡ If nums[mid] < nums[right], the minimum is at mid or to the left
⚡ The loop must end with left == right

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OPTIMAL SOLUTION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APPROACH:
Use Binary Search.

The rotation splits the array into two sorted sections. The minimum is the
first element of the second sorted section. If the middle is greater than
the rightmost element, the minimum must be to the right. Otherwise, the
minimum is at the middle or to the left.

KEY INSIGHTS:
• nums[mid] > nums[right] means the array crosses the pivot to the right
• nums[mid] < nums[right] means the current section is still sorted
• Checking the right boundary avoids comparisons with a potentially rotated half
• The minimum remains between left and right

ALGORITHM STEPS:
1. Set left = 0 and right = nums.Length - 1
2. While left < right:
   a. Calculate mid
   b. If nums[mid] > nums[right], set left = mid + 1
   c. Otherwise, set right = mid
3. Return nums[left]

TIME COMPLEXITY: O(log n)
- Each step eliminates half of the remaining search space

SPACE COMPLEXITY: O(1)
- Only left, right, and mid indices are stored

PATTERN: Binary Search
DIFFICULTY: Medium
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;

namespace DSA_CSharp.BinarySearch
{
    public class FindMinimumInRotatedSortedArray
    {
        /// <summary>
        /// Finds the minimum element in a rotated sorted array.
        /// Time: O(log n), Space: O(1)
        /// </summary>
        public int FindMin(int[] nums)
        {
            if (nums == null || nums.Length == 0)
                throw new ArgumentException("nums cannot be null or empty", nameof(nums));

            int left = 0;
            int right = nums.Length - 1;

            while (left < right)
            {
                int mid = left + (right - left) / 2;

                if (nums[mid] > nums[right])
                    left = mid + 1;
                else
                    right = mid;
            }

            return nums[left];
        }

        public static void Test()
        {
            var solution = new FindMinimumInRotatedSortedArray();

            Console.WriteLine("=== Find Minimum in Rotated Sorted Array Test Cases ===\n");

            int[] example1 = { 3, 4, 5, 1, 2 };
            Console.WriteLine($"Example 1: {solution.FindMin(example1)}");
            Console.WriteLine("Expected: 1\n");

            int[] example2 = { 4, 5, 6, 7, 0, 1, 2 };
            Console.WriteLine($"Example 2: {solution.FindMin(example2)}");
            Console.WriteLine("Expected: 0\n");

            int[] example3 = { 11, 13, 15, 17 };
            Console.WriteLine($"Example 3: {solution.FindMin(example3)}");
            Console.WriteLine("Expected: 11\n");

            int[] middleMinimum = { 6, 7, 8, 1, 2, 3, 4, 5 };
            Console.WriteLine($"Middle minimum: {solution.FindMin(middleMinimum)}");
            Console.WriteLine("Expected: 1\n");

            int[] single = { 1 };
            Console.WriteLine($"Single element: {solution.FindMin(single)}");
            Console.WriteLine("Expected: 1");
        }
    }
}
