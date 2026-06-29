/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Trapping Rain Water
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
Given n non-negative integers representing an elevation map where the width of 
each bar is 1, compute how much water it can trap after raining.

The elevation map is represented by an array of heights, where each element 
represents the height of a bar. Water can be trapped between bars if there are 
taller bars on both sides.

INPUT:
- height: int[] - Array of non-negative integers representing bar heights
  - 1 <= height.length <= 2 * 10^4
  - 0 <= height[i] <= 10^5

OUTPUT:
- int - Total amount of water that can be trapped

EXAMPLES:
Example 1:
Input: height = [0,1,0,2,1,0,1,3,2,1,2,1]
Output: 6
Explanation: The elevation map looks like this:
    3|           ▓
    2|       ▓ W ▓ W ▓
    1|   ▓ W ▓ ▓ W ▓ ▓ W ▓
    0| ▓ ▓ ▓ ▓ ▓ ▓ ▓ ▓ ▓ ▓ ▓ ▓
Water (W) is trapped in the spaces: 1+1+2+1+1 = 6 units

Example 2:
Input: height = [4,2,0,3,2,5]
Output: 9
Explanation:
    5|               ▓
    4| ▓             ▓
    3|     W W ▓ W W ▓
    2|   ▓ W W ▓ ▓ W ▓
    1|   ▓ W W ▓ ▓ W ▓
    0| ▓ ▓ ▓ ▓ ▓ ▓
Water trapped: 2+2+1+2+2 = 9 units

Example 3:
Input: height = [4,2,3]
Output: 1
Explanation:
    4| ▓
    3|   W ▓
    2|   ▓ ▓
Water trapped between positions 1 and 2: 1 unit

CONSTRAINTS:
- 1 <= height.length <= 20,000
- 0 <= height[i] <= 100,000
- Array contains non-negative integers only
- Width of each bar is exactly 1 unit

EDGE CASES:
✓ Empty array or single element - Cannot trap water
✓ Two elements - Cannot trap water between them
✓ All heights same - No water trapped
✓ Strictly increasing heights - No water trapped
✓ Strictly decreasing heights - No water trapped
✓ All zeros - No water trapped
✓ Heights with zeros in middle - Can trap water over zero-height bars
✓ Very large arrays (20,000 elements) - Must be O(n)
✓ Very tall bars (100,000 height) - Must handle large numbers

TRICK CASES:
⚡ Zero-height bars can still have water above them if surrounded by taller bars
⚡ Water level at position i is determined by MIN(max_left, max_right) - height[i]
⚡ Need to track maximum heights on BOTH left and right sides
⚡ Naive O(n²) approach times out - must optimize to O(n)
⚡ Can't just find two tallest bars - water forms multiple pockets
⚡ The "container" metaphor is misleading - this is about elevation, not walls
⚡ Peak in the middle creates two separate trapping zones

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OPTIMAL SOLUTION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APPROACH:
The key insight is that water trapped at any position depends on the minimum of:
1. Maximum height to its LEFT
2. Maximum height to its RIGHT

The amount of water at position i = min(leftMax, rightMax) - height[i]

We use TWO POINTERS starting from both ends, moving inward. We maintain:
- leftMax: highest bar seen from the left so far
- rightMax: highest bar seen from the right so far

We move the pointer from the side with the SMALLER maximum. Why? Because water 
level is determined by the minimum of the two maximums. If leftMax < rightMax, 
we know the right side has a sufficient barrier, so we can calculate water at 
the left pointer position based on leftMax alone.

KEY INSIGHTS:
• Water at position i = min(max_left, max_right) - height[i], if positive
• Two pointers allow us to process array in O(n) time, O(1) space
• Move pointer from the side with smaller max - the smaller max is the bottleneck
• No need to store all left/right maximums - track them on the fly
• Greedy approach: always process the side with lower maximum height

ALGORITHM STEPS:
1. Initialize two pointers: left = 0, right = n-1
2. Initialize leftMax = 0, rightMax = 0, water = 0
3. While left < right:
   a. If height[left] < height[right]:
      - If height[left] >= leftMax: update leftMax = height[left]
      - Else: water += (leftMax - height[left])
      - Move left pointer: left++
   b. Else (height[right] <= height[left]):
      - If height[right] >= rightMax: update rightMax = height[right]
      - Else: water += (rightMax - height[right])
      - Move right pointer: right--
4. Return total water

TIME COMPLEXITY: O(n)
- Single pass through the array with two pointers
- Each element is visited exactly once
- Constant work per element

SPACE COMPLEXITY: O(1)
- Only use a few variables: left, right, leftMax, rightMax, water
- No additional data structures needed
- In-place computation

PATTERN: Two Pointers
DIFFICULTY: Hard
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

namespace DSA_CSharp.Arrays
{
    public class TrappingRainWater
    {
        /// <summary>
        /// Calculates the total amount of rainwater that can be trapped given an elevation map.
        /// Uses two pointers approach for optimal O(n) time and O(1) space complexity.
        /// </summary>
        /// <param name="height">Array of non-negative integers representing bar heights</param>
        /// <returns>Total units of water that can be trapped</returns>
        public int Trap(int[] height)
        {
            // Edge case: need at least 3 bars to trap water
            if (height == null || height.Length < 3)
            {
                return 0;
            }

            int left = 0;
            int right = height.Length - 1;
            int leftMax = 0;
            int rightMax = 0;
            int water = 0;

            // Two pointers approach - move from both ends towards center
            while (left < right)
            {
                // Process the side with smaller height (bottleneck)
                if (height[left] < height[right])
                {
                    // Update leftMax or add water
                    if (height[left] >= leftMax)
                    {
                        leftMax = height[left];
                    }
                    else
                    {
                        // Water trapped = leftMax - current height
                        water += leftMax - height[left];
                    }
                    left++;
                }
                else
                {
                    // Process right side
                    if (height[right] >= rightMax)
                    {
                        rightMax = height[right];
                    }
                    else
                    {
                        // Water trapped = rightMax - current height
                        water += rightMax - height[right];
                    }
                    right--;
                }
            }

            return water;
        }

        /// <summary>
        /// Alternative solution using dynamic programming approach.
        /// Pre-computes left and right max arrays.
        /// Time: O(n), Space: O(n)
        /// </summary>
        public int TrapDP(int[] height)
        {
            if (height == null || height.Length < 3)
            {
                return 0;
            }

            int n = height.Length;
            int[] leftMax = new int[n];
            int[] rightMax = new int[n];

            // Build leftMax array
            leftMax[0] = height[0];
            for (int i = 1; i < n; i++)
            {
                leftMax[i] = Math.Max(leftMax[i - 1], height[i]);
            }

            // Build rightMax array
            rightMax[n - 1] = height[n - 1];
            for (int i = n - 2; i >= 0; i--)
            {
                rightMax[i] = Math.Max(rightMax[i + 1], height[i]);
            }

            // Calculate water at each position
            int water = 0;
            for (int i = 0; i < n; i++)
            {
                int waterLevel = Math.Min(leftMax[i], rightMax[i]);
                water += waterLevel - height[i];
            }

            return water;
        }

        // Test method
        public static void TestTrappingRainWater()
        {
            var solution = new TrappingRainWater();

            // Test case 1: Example from problem
            int[] test1 = { 0, 1, 0, 2, 1, 0, 1, 3, 2, 1, 2, 1 };
            Console.WriteLine($"Test 1: {solution.Trap(test1)}"); // Expected: 6

            // Test case 2: Another example
            int[] test2 = { 4, 2, 0, 3, 2, 5 };
            Console.WriteLine($"Test 2: {solution.Trap(test2)}"); // Expected: 9

            // Test case 3: Small example
            int[] test3 = { 4, 2, 3 };
            Console.WriteLine($"Test 3: {solution.Trap(test3)}"); // Expected: 1

            // Test case 4: No water trapped (increasing)
            int[] test4 = { 1, 2, 3, 4, 5 };
            Console.WriteLine($"Test 4: {solution.Trap(test4)}"); // Expected: 0

            // Test case 5: No water trapped (decreasing)
            int[] test5 = { 5, 4, 3, 2, 1 };
            Console.WriteLine($"Test 5: {solution.Trap(test5)}"); // Expected: 0

            // Test case 6: Valley in middle
            int[] test6 = { 3, 0, 2, 0, 4 };
            Console.WriteLine($"Test 6: {solution.Trap(test6)}"); // Expected: 7

            // Test case 7: Edge case - too few elements
            int[] test7 = { 1, 2 };
            Console.WriteLine($"Test 7: {solution.Trap(test7)}"); // Expected: 0

            // Test case 8: All same height
            int[] test8 = { 3, 3, 3, 3 };
            Console.WriteLine($"Test 8: {solution.Trap(test8)}"); // Expected: 0
        }
    }
}
