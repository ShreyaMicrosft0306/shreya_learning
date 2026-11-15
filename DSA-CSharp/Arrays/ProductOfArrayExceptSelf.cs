/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Product of Array Except Self
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
Given an integer array nums, return an array where answer[i] is the product
of every number in nums except nums[i].

Do not use division.

INPUT:
- nums: int[] - Array of integers

OUTPUT:
- int[] - Product of all values except the value at the same index

EXAMPLES:
Example 1:
Input: nums = [1, 2, 3, 4]
Output: [24, 12, 8, 6]
Explanation:
- answer[0] = 2 * 3 * 4 = 24
- answer[1] = 1 * 3 * 4 = 12
- answer[2] = 1 * 2 * 4 = 8
- answer[3] = 1 * 2 * 3 = 6

Example 2:
Input: nums = [-1, 1, 0, -3, 3]
Output: [0, 0, 9, 0, 0]
Explanation: For index 2, the product of all other values is
(-1) * 1 * (-3) * 3 = 9.

Example 3:
Input: nums = [2, 3]
Output: [3, 2]

CONSTRAINTS:
- 2 <= nums.Length <= 100,000
- -30 <= nums[i] <= 30
- Every prefix and suffix product fits in a 32-bit integer
- Every answer value fits in a 32-bit integer

EDGE CASES:
✓ One zero -> only the zero's position gets a non-zero product
✓ Two or more zeros -> every answer is zero
✓ Negative values
✓ No zero values
✓ Minimum length array

TRICK CASES:
⚡ Do not use division.
⚡ The product before index i and after index i must both be included.
⚡ A zero does not need special handling with the prefix/suffix approach.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OPTIMAL SOLUTION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APPROACH:
For each index, split the product into two parts:

  product before i × product after i

First pass from left to right:
- Store the product of all values before each index in answer.

Second pass from right to left:
- Keep a running product of values after each index.
- Multiply answer[i] by that running product.

Example: nums = [1, 2, 3, 4]

After the left-to-right pass:
  answer = [1, 1, 2, 6]
  Each position contains the product of values to its left.

After the right-to-left pass:
  answer = [24, 12, 8, 6]

The output array stores the left products, so no additional prefix or suffix
arrays are needed.

ALGORITHM STEPS:
1. Create answer with the same length as nums.
2. Set prefix = 1.
3. For each index from left to right:
   a. Store prefix in answer[i].
   b. Multiply prefix by nums[i].
4. Set suffix = 1.
5. For each index from right to left:
   a. Multiply answer[i] by suffix.
   b. Multiply suffix by nums[i].
6. Return answer.

TIME COMPLEXITY: O(n)
- Two passes over the array.

SPACE COMPLEXITY: O(1) EXTRA SPACE
- Only prefix and suffix variables are used.
- The output array does not count as extra space.

PATTERN: Prefix and Suffix Products
DIFFICULTY: Medium
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

namespace DSA_CSharp.Arrays
{
    public class ProductOfArrayExceptSelf
    {
        /// <summary>
        /// Returns the product of every array value except the value at each index.
        /// </summary>
        public int[] ProductExceptSelf(int[] nums)
        {
            int[] answer = new int[nums.Length];
            int prefix = 1;

            for (int i = 0; i < nums.Length; i++)
            {
                answer[i] = prefix;
                prefix *= nums[i];
            }

            int suffix = 1;

            for (int i = nums.Length - 1; i >= 0; i--)
            {
                answer[i] *= suffix;
                suffix *= nums[i];
            }

            return answer;
        }
    }
}
