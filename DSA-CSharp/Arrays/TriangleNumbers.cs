/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Triangle Numbers
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
Given an integer array nums, count the index triplets whose values can form
the three sides of a valid triangle. Triplets with the same values but
different indices are counted separately.

For positive side lengths sorted as a <= b <= c, it is enough to check:

    a + b > c

The other two triangle inequalities are automatically true.

INPUT:
- nums: int[] - Array of non-negative possible side lengths

OUTPUT:
- int - Number of index triplets that form valid triangles

EXAMPLES:
Example 1:
Input: nums = [11,4,9,6,15,18]
Output: 10
Explanation: Ten choices of three indices satisfy the triangle inequality.

Example 2:
Input: nums = [2,2,3,4]
Output: 3
Explanation: The valid index triplets have values [2,2,3], [2,3,4], and
[2,3,4]. The repeated values come from different indices and both count.

CONSTRAINTS:
- 0 <= nums.Length
- 0 <= nums[i]
- The result fits in an int

EDGE CASES:
✓ Null or fewer than three values -> 0
✓ Zero-length sides -> never form a valid triangle
✓ All equal positive sides
✓ Duplicate values at different indices
✓ No valid triangles

TRICK CASES:
⚡ Triplets do not need unique values; different index choices all count
⚡ Equality is invalid: a + b must be strictly greater than c
⚡ Sorting changes the input array
⚡ Checking every triplet takes O(n³) time

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OPTIMAL SOLUTION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APPROACH:
Sort the array and fix the largest side at each index. Use two pointers over
the smaller values. If nums[left] + nums[middle] is greater than the largest
side, then every value from left through middle - 1 also works with nums[middle]
and the largest side. Count all middle - left pairs at once and move middle
left. Otherwise, increase left to make the sum larger.

KEY INSIGHTS:
• After sorting a <= b <= c, only a + b > c needs to be tested
• A valid pair at left and middle means every larger possible left value is
  also valid, allowing many triplets to be counted together
• Sorting makes pointer movement predictable

ALGORITHM STEPS:
1. Sort nums in ascending order.
2. Fix each possible largest side, starting at index 2.
3. Set left to 0 and middle to the index before the largest side.
4. If nums[left] + nums[middle] > nums[largest], add middle - left and move
   middle left.
5. Otherwise, move left right to increase the sum.
6. Repeat for every possible largest side.

TIME COMPLEXITY: O(n²)
- Sorting takes O(n log n), followed by an O(n) scan for each largest side.

SPACE COMPLEXITY: O(1), excluding sorting
- Only pointer and counter variables are used.

PATTERN: Sorting + Two Pointers
DIFFICULTY: Medium
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;

namespace DSA_CSharp.Arrays
{
    public class TriangleNumbers
    {
        /// <summary>
        /// Counts index triplets whose values can form a valid triangle.
        /// Time: O(n²), Space: O(1), excluding sorting.
        /// </summary>
        public int CountValidTriangles(int[] nums)
        {
            if (nums == null || nums.Length < 3)
            {
                return 0;
            }

            Array.Sort(nums);
            int triangleCount = 0;

            for (int largest = nums.Length - 1; largest >= 2; largest--)
            {
                int left = 0;
                int middle = largest - 1;

                while (left < middle)
                {
                    long smallerSidesSum = (long)nums[left] + nums[middle];

                    if (smallerSidesSum > nums[largest])
                    {
                        triangleCount += middle - left;
                        middle--;
                    }
                    else
                    {
                        left++;
                    }
                }
            }

            return triangleCount;
        }

        public static void Test()
        {
            var solution = new TriangleNumbers();

            int[] nums1 = { 11, 4, 9, 6, 15, 18 };
            Console.WriteLine(
                $"Example 1: {solution.CountValidTriangles(nums1)} (expected 10)");

            int[] nums2 = { 2, 2, 3, 4 };
            Console.WriteLine(
                $"Duplicates: {solution.CountValidTriangles(nums2)} (expected 3)");

            int[] nums3 = { 0, 1, 1 };
            Console.WriteLine(
                $"Invalid sides: {solution.CountValidTriangles(nums3)} (expected 0)");

            int[] nums4 = { 3, 3, 3, 3 };
            Console.WriteLine(
                $"Equal sides: {solution.CountValidTriangles(nums4)} (expected 4)");
        }
    }
}