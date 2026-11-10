/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Maximum Product Subarray
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
Given an integer array nums, find a contiguous subarray that has the largest
product and return that product.

INPUT:
- nums: int[] - Integer array with 1 <= nums.Length <= 2 * 10^4

OUTPUT:
- int - Maximum product among all contiguous subarrays

EXAMPLES:
Example 1:
Input: nums = [2,3,-2,4]
Output: 6
Explanation: [2,3] has the maximum product 6.

Example 2:
Input: nums = [-2,0,-1]
Output: 0
Explanation: The maximum product is 0.

CONSTRAINTS:
- 1 <= nums.Length <= 2 * 10^4
- -10 <= nums[i] <= 10
- Every subarray product fits in a 32-bit integer

EDGE CASES:
✓ One element
✓ Array containing zero
✓ Array containing negative numbers
✓ Even number of negative values
✓ Odd number of negative values

TRICK CASES:
⚡ A negative product can become positive when multiplied by another negative
⚡ A zero resets every running product
⚡ The maximum product may start or end at the middle of the array

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OPTIMAL SOLUTION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APPROACH:
Use prefix and suffix products.

Traverse the array from left to right, tracking the running product of each
prefix. Traverse again from right to left, tracking the running suffix product.
Return the largest product seen from either traversal.

When a running product becomes 0, reset it to 1. A nonzero subarray product
can continue without a zero, but a zero should start a fresh segment.

KEY INSIGHTS:
• The maximum product prefix or suffix is affected by negative values
• A second reverse traversal covers products whose last negative value
  changes the result
• Zeros reset the product to 1

ALGORITHM STEPS:
1. Initialize prefixProduct, suffixProduct, and result as 1
2. Scan the array from left to right
3. If prefixProduct is 0, reset it to 1
4. Multiply prefixProduct by nums[i]
5. Update result with prefixProduct
6. Scan right to left with the same suffix logic
7. Update result with suffixProduct

TIME COMPLEXITY: O(n)
- Two array passes are performed

SPACE COMPLEXITY: O(1)
- Only running products and the result are stored

PATTERN: Prefix and Suffix Product
DIFFICULTY: Medium
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;

namespace DSA_CSharp.DynamicProgramming
{
    public class MaximumProductSubarray
    {
        /// <summary>
        /// Returns the maximum product of any contiguous subarray.
        /// Time: O(n), Space: O(1)
        /// </summary>
        public int MaxProduct(int[] nums)
        {
            if (nums == null || nums.Length == 0)
                return 0;

            int prefixProduct = 1;
            int suffixProduct = 1;
            int maximum = int.MinValue;

            for (int i = 0; i < nums.Length; i++)
            {
                prefixProduct = prefixProduct == 0 ? 1 : prefixProduct;
                prefixProduct *= nums[i];
                maximum = Math.Max(maximum, prefixProduct);

                int suffixIndex = nums.Length - 1 - i;
                suffixProduct = suffixProduct == 0 ? 1 : suffixProduct;
                suffixProduct *= nums[suffixIndex];
                maximum = Math.Max(maximum, suffixProduct);
            }

            return maximum;
        }

        public static void Test()
        {
            var solution = new MaximumProductSubarray();

            Console.WriteLine("=== Maximum Product Subarray Test Cases ===\n");

            int[] example1 = { 2, 3, -2, 4 };
            Console.WriteLine($"Example 1: {solution.MaxProduct(example1)}");
            Console.WriteLine("Expected: 6\n");

            int[] example2 = { -2, 0, -1 };
            Console.WriteLine($"Example 2: {solution.MaxProduct(example2)}");
            Console.WriteLine("Expected: 0\n");

            int[] example3 = { -2, -3, -4 };
            Console.WriteLine($"Example 3: {solution.MaxProduct(example3)}");
            Console.WriteLine("Expected: 12\n");

            int[] example4 = { 0, 2 };
            Console.WriteLine($"Example 4: {solution.MaxProduct(example4)}");
            Console.WriteLine("Expected: 2\n");

            int[] example5 = { -1, -2, -9, -6 };
            Console.WriteLine($"Example 5: {solution.MaxProduct(example5)}");
            Console.WriteLine("Expected: 108");
        }
    }
}
