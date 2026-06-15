/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Two Sum
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
Given an array of integers nums and an integer target, return indices of 
the two numbers in the array such that they add up to target.

You may assume that each input would have exactly one solution, and you 
may not use the same element twice. You can return the answer in any order.

INPUT:
- nums: int[] - Array of integers (2 <= nums.length <= 10^4)
- target: int - Target sum value

OUTPUT:
- int[] - Array containing two indices [i, j] where nums[i] + nums[j] == target

EXAMPLES:
Example 1:
Input: nums = [2,7,11,15], target = 9
Output: [0,1]
Explanation: nums[0] + nums[1] = 2 + 7 = 9

Example 2:
Input: nums = [3,2,4], target = 6
Output: [1,2]
Explanation: nums[1] + nums[2] = 2 + 4 = 6

Example 3:
Input: nums = [3,3], target = 6
Output: [0,1]
Explanation: Both elements are the same value

CONSTRAINTS:
- 2 <= nums.length <= 10^4
- -10^9 <= nums[i] <= 10^9
- -10^9 <= target <= 10^9
- Only one valid answer exists

EDGE CASES:
✓ Minimum size array (2 elements)
✓ Duplicate values in array
✓ Negative numbers
✓ Zero values
✓ Large array (10^4 elements)
✓ Target is zero (sum of positive and negative)
✓ Solution at beginning vs end of array

TRICK CASES:
⚡ Cannot use same element twice (e.g., [3], target=6 is invalid)
⚡ Don't use nested loops O(n²) - too slow for large arrays
⚡ Need to track both value AND index
⚡ Watch for duplicate values - they can be part of solution
⚡ Complement calculation: complement = target - current

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OPTIMAL SOLUTION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APPROACH:
Use a HASH MAP (Dictionary) to store seen numbers and their indices.
For each number, check if its complement (target - current) exists in the map.

This is a classic "trade space for time" optimization - we use O(n) extra 
space to reduce time complexity from O(n²) to O(n).

KEY INSIGHTS:
• If nums[i] + nums[j] = target, then nums[j] = target - nums[i]
• Instead of searching for nums[j] linearly, use hash map for O(1) lookup
• Store numbers as we iterate - don't need to pre-populate entire map
• Check for complement BEFORE adding current number (avoids using same index)

ALGORITHM STEPS:
1. Create empty hash map: Dictionary<value, index>
2. For each number in array:
   a. Calculate complement = target - current number
   b. If complement exists in map:
      - Return [map[complement], current index]
   c. Add current number and index to map
3. Return result (guaranteed to exist per problem statement)

TIME COMPLEXITY: O(n)
- Single pass through array
- Hash map operations (get/put) are O(1) average case
- Total: O(n) × O(1) = O(n)

SPACE COMPLEXITY: O(n)
- Hash map stores up to n-1 elements in worst case
- (When solution is last two elements, we store all others)

PATTERN: Hash Map for O(1) Lookup
DIFFICULTY: Easy
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;
using System.Collections.Generic;

namespace DSA_CSharp.Arrays
{
    public class TwoSum
    {
        /// <summary>
        /// Finds two indices in the array whose values sum to target.
        /// Uses hash map for O(n) time complexity.
        /// </summary>
        /// <param name="nums">Array of integers</param>
        /// <param name="target">Target sum value</param>
        /// <returns>Array of two indices [i, j]</returns>
        public int[] Solution(int[] nums, int target)
        {
            // Map: value -> index
            Dictionary<int, int> seen = new Dictionary<int, int>();
            
            for (int i = 0; i < nums.Length; i++)
            {
                int complement = target - nums[i];
                
                // Check if complement exists in map
                if (seen.ContainsKey(complement))
                {
                    return new int[] { seen[complement], i };
                }
                
                // Add current number to map
                // Note: This handles duplicates correctly - we only add if not found yet
                seen[nums[i]] = i;
            }
            
            // Should never reach here per problem statement
            throw new ArgumentException("No solution found");
        }
        
        // Test method
        public static void Test()
        {
            var solution = new TwoSum();
            
            // Test case 1
            int[] nums1 = { 2, 7, 11, 15 };
            int target1 = 9;
            int[] result1 = solution.Solution(nums1, target1);
            Console.WriteLine($"Input: nums = [{string.Join(",", nums1)}], target = {target1}");
            Console.WriteLine($"Output: [{result1[0]},{result1[1]}]");
            Console.WriteLine($"Verification: {nums1[result1[0]]} + {nums1[result1[1]]} = {nums1[result1[0]] + nums1[result1[1]]}\n");
            
            // Test case 2
            int[] nums2 = { 3, 2, 4 };
            int target2 = 6;
            int[] result2 = solution.Solution(nums2, target2);
            Console.WriteLine($"Input: nums = [{string.Join(",", nums2)}], target = {target2}");
            Console.WriteLine($"Output: [{result2[0]},{result2[1]}]");
            Console.WriteLine($"Verification: {nums2[result2[0]]} + {nums2[result2[1]]} = {nums2[result2[0]] + nums2[result2[1]]}\n");
            
            // Test case 3 - Duplicates
            int[] nums3 = { 3, 3 };
            int target3 = 6;
            int[] result3 = solution.Solution(nums3, target3);
            Console.WriteLine($"Input: nums = [{string.Join(",", nums3)}], target = {target3}");
            Console.WriteLine($"Output: [{result3[0]},{result3[1]}]");
            Console.WriteLine($"Verification: {nums3[result3[0]]} + {nums3[result3[1]]} = {nums3[result3[0]] + nums3[result3[1]]}\n");
            
            // Test case 4 - Negative numbers
            int[] nums4 = { -1, -2, -3, -4, -5 };
            int target4 = -8;
            int[] result4 = solution.Solution(nums4, target4);
            Console.WriteLine($"Input: nums = [{string.Join(",", nums4)}], target = {target4}");
            Console.WriteLine($"Output: [{result4[0]},{result4[1]}]");
            Console.WriteLine($"Verification: {nums4[result4[0]]} + {nums4[result4[1]]} = {nums4[result4[0]] + nums4[result4[1]]}\n");
        }
    }
}
