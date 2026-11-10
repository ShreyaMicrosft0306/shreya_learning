/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Missing Number
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
Given an array nums containing n distinct numbers in the range [0, n],
return the only number in that range that does not appear in nums.

INPUT:
- nums: int[] - Array of n distinct integers

OUTPUT:
- int - The missing number in [0, n]

EXAMPLES:
Example 1:
Input: nums = [3,0,1]
Output: 2
Explanation: n = 3, and 2 is missing.

Example 2:
Input: nums = [0,1]
Output: 2

Example 3:
Input: nums = [9,6,4,2,3,5,7,0,1]
Output: 8

CONSTRAINTS:
- nums.Length == n
- 1 <= n <= 10^4
- 0 <= nums[i] <= n
- All nums[i] are distinct

EDGE CASES:
✓ Missing number is 0
✓ Missing number is n
✓ One element [0] → missing is 1
✓ One element [1] → missing is 0

TRICK CASES:
⚡ Do not use a separate visited array
⚡ Every number from 0 to n should be present exactly once except the missing value
⚡ XOR of a number with itself is 0

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OPTIMAL SOLUTION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APPROACH:
Use XOR.

XOR every number from 0 to n and every number in nums.
Every repeated value appears twice and cancels to 0. The missing number
appears only once, so it remains in the final XOR result.

KEY INSIGHTS:
• A XOR A = 0
• A XOR 0 = A
• XOR order does not matter
• The missing number is the only unpaired value

ALGORITHM STEPS:
1. Initialize result = 0
2. XOR result with every value i from 0 through nums.Length
3. XOR result with every number in nums
4. Return result

TIME COMPLEXITY: O(n)
- One pass XORs the complete range
- One pass XORs all array values

SPACE COMPLEXITY: O(1)
- Only one integer result is used

PATTERN: XOR / Missing Number
DIFFICULTY: Easy
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;

namespace DSA_CSharp.Arrays
{
    public class MissingNumber
    {
        /// <summary>
        /// Returns the missing number from the range [0, n].
        /// Time: O(n), Space: O(1)
        /// </summary>
        public int FindMissingNumber(int[] nums)
        {
            if (nums == null)
                throw new ArgumentNullException(nameof(nums));

            int n = nums.Length;
            int result = n;

            for (int i = 0; i < n; i++)
            {
                result ^= i;
                result ^= nums[i];
            }

            return result;
        }

        public static void Test()
        {
            var solution = new MissingNumber();

            Console.WriteLine("=== Missing Number Test Cases ===\n");

            int[] example1 = { 3, 0, 1 };
            Console.WriteLine($"Example 1: {solution.FindMissingNumber(example1)}");
            Console.WriteLine("Expected: 2\n");

            int[] example2 = { 0, 1 };
            Console.WriteLine($"Example 2: {solution.FindMissingNumber(example2)}");
            Console.WriteLine("Expected: 2\n");

            int[] example3 = { 9, 6, 4, 2, 3, 5, 7, 0, 1 };
            Console.WriteLine($"Example 3: {solution.FindMissingNumber(example3)}");
            Console.WriteLine("Expected: 8\n");

            int[] missingZero = { 1 };
            Console.WriteLine($"Missing zero: {solution.FindMissingNumber(missingZero)}");
            Console.WriteLine("Expected: 0\n");

            int[] missingN = { 0 };
            Console.WriteLine($"Missing n: {solution.FindMissingNumber(missingN)}");
            Console.WriteLine("Expected: 1");
        }
    }
}
