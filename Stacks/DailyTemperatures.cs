/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Daily Temperatures
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
Given an array of integers temperatures representing daily temperatures, 
return an array answer such that answer[i] is the number of days you have 
to wait after the ith day to get a warmer temperature. If there is no future 
day for which this is possible, keep answer[i] == 0 instead.

INPUT:
- temperatures: int[] - Array of daily temperatures (1 <= temperatures.Length <= 10^5)

OUTPUT:
- int[] - Array where each element represents days to wait for warmer temperature

EXAMPLES:
Example 1:
Input: temperatures = [73,74,75,71,69,72,76,73]
Output: [1,1,4,2,1,1,0,0]
Explanation: 
- Day 0 (73°): Next warmer is day 1 (74°) → wait 1 day
- Day 1 (74°): Next warmer is day 2 (75°) → wait 1 day
- Day 2 (75°): Next warmer is day 6 (76°) → wait 4 days
- Day 3 (71°): Next warmer is day 5 (72°) → wait 2 days
- Day 4 (69°): Next warmer is day 5 (72°) → wait 1 day
- Day 5 (72°): Next warmer is day 6 (76°) → wait 1 day
- Day 6 (76°): No warmer day → 0
- Day 7 (73°): No warmer day → 0

Example 2:
Input: temperatures = [30,40,50,60]
Output: [1,1,1,0]

Example 3:
Input: temperatures = [30,60,90]
Output: [1,1,0]

CONSTRAINTS:
- 1 <= temperatures.length <= 10^5
- 30 <= temperatures[i] <= 100

EDGE CASES:
✓ Single element array → [0]
✓ All temperatures decreasing → all zeros
✓ All temperatures increasing → [1,1,1,...,0]
✓ All same temperature → all zeros
✓ Maximum size array (10^5 elements)
✓ Large gaps between warmer days

TRICK CASES:
⚡ Don't use nested loops (O(n²)) - will timeout on large inputs
⚡ Multiple days might be waiting for the same warmer day
⚡ Stack should store indices, not values (need to calculate distance)
⚡ Process temperatures right-to-left OR use monotonic decreasing stack

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OPTIMAL SOLUTION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APPROACH:
Use a MONOTONIC STACK to maintain indices of temperatures in decreasing order.
This allows us to efficiently find the next warmer temperature for each day.

The key insight is: when we encounter a warmer temperature, it's the answer 
for all previous cooler temperatures still waiting in the stack.

KEY INSIGHTS:
• Stack stores indices (not values) to calculate day differences
• Maintain decreasing temperature order in stack
• When we find a warmer day, pop all cooler days from stack
• Single pass through array = O(n) time

ALGORITHM STEPS:
1. Initialize result array with zeros and empty stack
2. Iterate through temperatures left to right
3. While current temp > temperature at stack top:
   - Pop index from stack
   - Calculate days difference: current index - popped index
   - Store difference in result
4. Push current index to stack
5. Return result (remaining stack indices stay 0)

TIME COMPLEXITY: O(n)
- Each element is pushed and popped from stack at most once
- Total operations: 2n in worst case

SPACE COMPLEXITY: O(n)
- Stack can hold all n elements in worst case (decreasing temperatures)
- Result array requires O(n) space (required output, not auxiliary)

PATTERN: Monotonic Stack (Decreasing)
DIFFICULTY: Medium
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;
using System.Collections.Generic;

namespace DSA_CSharp.Stacks
{
    public class DailyTemperatures
    {
        /// <summary>
        /// Calculates the number of days to wait for a warmer temperature for each day.
        /// Uses monotonic stack for O(n) time complexity.
        /// </summary>
        /// <param name="temperatures">Array of daily temperatures</param>
        /// <returns>Array of days to wait for warmer temperature</returns>
        public int[] Solution(int[] temperatures)
        {
            int n = temperatures.Length;
            int[] result = new int[n];
            Stack<int> stack = new Stack<int>(); // Store indices
            
            for (int i = 0; i < n; i++)
            {
                // While current temp is warmer than temperature at stack top
                while (stack.Count > 0 && temperatures[i] > temperatures[stack.Peek()])
                {
                    int prevIndex = stack.Pop();
                    result[prevIndex] = i - prevIndex; // Days to wait
                }
                
                // Push current index to stack
                stack.Push(i);
            }
            
            // Remaining indices in stack have no warmer day (already initialized to 0)
            return result;
        }
        
        // Test method
        public static void Test()
        {
            var solution = new DailyTemperatures();
            
            // Test case 1
            int[] temps1 = { 73, 74, 75, 71, 69, 72, 76, 73 };
            int[] result1 = solution.Solution(temps1);
            Console.WriteLine($"Input: [{string.Join(",", temps1)}]");
            Console.WriteLine($"Output: [{string.Join(",", result1)}]");
            Console.WriteLine($"Expected: [1,1,4,2,1,1,0,0]\n");
            
            // Test case 2
            int[] temps2 = { 30, 40, 50, 60 };
            int[] result2 = solution.Solution(temps2);
            Console.WriteLine($"Input: [{string.Join(",", temps2)}]");
            Console.WriteLine($"Output: [{string.Join(",", result2)}]");
            Console.WriteLine($"Expected: [1,1,1,0]\n");
            
            // Test case 3 - Edge case: decreasing temperatures
            int[] temps3 = { 90, 80, 70, 60 };
            int[] result3 = solution.Solution(temps3);
            Console.WriteLine($"Input: [{string.Join(",", temps3)}]");
            Console.WriteLine($"Output: [{string.Join(",", result3)}]");
            Console.WriteLine($"Expected: [0,0,0,0]\n");
        }
    }
}
