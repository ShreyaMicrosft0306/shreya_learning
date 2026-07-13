/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Climbing Stairs
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
You are climbing a staircase. It takes n steps to reach the top.
Each time you can either climb 1 or 2 steps. In how many distinct ways 
can you climb to the top?

INPUT:
- n: int - Number of steps in the staircase (1 <= n <= 45)

OUTPUT:
- int - Number of distinct ways to climb to the top

EXAMPLES:
Example 1:
Input: n = 2
Output: 2
Explanation: There are two ways to climb to the top:
1. 1 step + 1 step
2. 2 steps

Example 2:
Input: n = 3
Output: 3
Explanation: There are three ways to climb to the top:
1. 1 step + 1 step + 1 step
2. 1 step + 2 steps
3. 2 steps + 1 step

Example 3:
Input: n = 5
Output: 8
Explanation: There are eight ways:
1. 1+1+1+1+1
2. 1+1+1+2
3. 1+1+2+1
4. 1+2+1+1
5. 2+1+1+1
6. 1+2+2
7. 2+1+2
8. 2+2+1

CONSTRAINTS:
- 1 <= n <= 45
- Result fits in 32-bit integer

EDGE CASES:
✓ n = 1 (only one way: single step)
✓ n = 2 (two ways: 1+1 or 2)
✓ Large n (n = 45, near upper constraint)
✓ Result grows exponentially (Fibonacci sequence)

TRICK CASES:
⚡ This is actually the Fibonacci sequence! f(n) = f(n-1) + f(n-2)
⚡ Base cases: f(1) = 1, f(2) = 2 (or f(0) = 1, f(1) = 1)
⚡ Don't use pure recursion - exponential time O(2^n)
⚡ Order matters: (1+2) is different from (2+1)
⚡ For large n, recursive solution will timeout/stack overflow

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OPTIMAL SOLUTION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APPROACH:
This is a DYNAMIC PROGRAMMING problem disguised as a combinatorics question.

KEY INSIGHT:
To reach step n, you must have come from either:
- Step n-1 (then take 1 step)
- Step n-2 (then take 2 steps)

Therefore: ways(n) = ways(n-1) + ways(n-2)

This is the Fibonacci sequence!

RECURRENCE RELATION:
dp[n] = dp[n-1] + dp[n-2]
Base cases: dp[1] = 1, dp[2] = 2

WHY THIS WORKS:
- If you're at step n-1, there's only 1 way to reach n (take 1 step)
- If you're at step n-2, there's only 1 way to reach n (take 2 steps)
- Total ways to reach n = sum of ways to reach n-1 and n-2

ALGORITHM STEPS (Bottom-Up DP):
1. Handle base cases (n = 1 or n = 2)
2. Create array dp of size n+1
3. Initialize dp[1] = 1, dp[2] = 2
4. For i from 3 to n:
   - dp[i] = dp[i-1] + dp[i-2]
5. Return dp[n]

TIME COMPLEXITY: O(n)
- Single pass from 3 to n
- Each step is O(1) computation

SPACE COMPLEXITY: O(1) [Space-Optimized Version]
- Only need to track last two values
- Can optimize from O(n) array to two variables

PATTERN: Dynamic Programming - Fibonacci Pattern
DIFFICULTY: Easy
RELATED: House Robber, Min Cost Climbing Stairs, Decode Ways
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;

namespace DSA_CSharp.DynamicProgramming
{
    public class ClimbingStairs
    {
        /// <summary>
        /// OPTIMAL SOLUTION: Space-Optimized Dynamic Programming
        /// Calculate number of ways to climb n stairs.
        /// Time: O(n), Space: O(1)
        /// </summary>
        /// <param name="n">Number of steps</param>
        /// <returns>Number of distinct ways to climb</returns>
        public int ClimbStairs(int n)
        {
            // Base cases
            if (n <= 2) return n;
            
            // Only need to track last two values (Fibonacci pattern)
            int prev2 = 1; // ways to reach step 1
            int prev1 = 2; // ways to reach step 2
            
            // Build up from step 3 to n
            for (int i = 3; i <= n; i++)
            {
                int current = prev1 + prev2;
                prev2 = prev1;
                prev1 = current;
            }
            
            return prev1;
        }
        
        /// <summary>
        /// APPROACH 2: Tabulation (Bottom-Up DP with array)
        /// Time: O(n), Space: O(n)
        /// More intuitive but uses extra space
        /// </summary>
        public int ClimbStairsTabulation(int n)
        {
            if (n <= 2) return n;
            
            // dp[i] = number of ways to reach step i
            int[] dp = new int[n + 1];
            dp[1] = 1;
            dp[2] = 2;
            
            for (int i = 3; i <= n; i++)
            {
                dp[i] = dp[i - 1] + dp[i - 2];
            }
            
            return dp[n];
        }
        
        /// <summary>
        /// APPROACH 3: Memoization (Top-Down DP with recursion)
        /// Time: O(n), Space: O(n) for call stack + memo
        /// Good for understanding the recursive structure
        /// </summary>
        public int ClimbStairsMemo(int n)
        {
            int[] memo = new int[n + 1];
            return ClimbStairsMemoHelper(n, memo);
        }
        
        private int ClimbStairsMemoHelper(int n, int[] memo)
        {
            // Base cases
            if (n <= 2) return n;
            
            // Check if already computed
            if (memo[n] != 0) return memo[n];
            
            // Compute and store
            memo[n] = ClimbStairsMemoHelper(n - 1, memo) + 
                      ClimbStairsMemoHelper(n - 2, memo);
            
            return memo[n];
        }
        
        /// <summary>
        /// APPROACH 4: Naive Recursion (DON'T USE - Educational Only)
        /// Time: O(2^n), Space: O(n) for call stack
        /// Exponential time - too slow for n > 30
        /// </summary>
        public int ClimbStairsRecursive(int n)
        {
            if (n <= 2) return n;
            return ClimbStairsRecursive(n - 1) + ClimbStairsRecursive(n - 2);
        }
        
        // Test method
        public static void Test()
        {
            var solution = new ClimbingStairs();
            
            Console.WriteLine("=== Climbing Stairs Test Cases ===\n");
            
            // Test case 1
            int n1 = 2;
            Console.WriteLine($"Test 1: n = {n1}");
            Console.WriteLine($"Output: {solution.ClimbStairs(n1)}");
            Console.WriteLine($"Expected: 2");
            Console.WriteLine($"Explanation: Two ways (1+1) or (2)\n");
            
            // Test case 2
            int n2 = 3;
            Console.WriteLine($"Test 2: n = {n2}");
            Console.WriteLine($"Output: {solution.ClimbStairs(n2)}");
            Console.WriteLine($"Expected: 3");
            Console.WriteLine($"Explanation: Three ways (1+1+1), (1+2), (2+1)\n");
            
            // Test case 3
            int n3 = 5;
            Console.WriteLine($"Test 3: n = {n3}");
            Console.WriteLine($"Output: {solution.ClimbStairs(n3)}");
            Console.WriteLine($"Expected: 8\n");
            
            // Test case 4 - Edge case
            int n4 = 1;
            Console.WriteLine($"Test 4 (Edge): n = {n4}");
            Console.WriteLine($"Output: {solution.ClimbStairs(n4)}");
            Console.WriteLine($"Expected: 1\n");
            
            // Test case 5 - Large value
            int n5 = 10;
            Console.WriteLine($"Test 5: n = {n5}");
            Console.WriteLine($"Output: {solution.ClimbStairs(n5)}");
            Console.WriteLine($"Expected: 89\n");
            
            // Test case 6 - Compare all approaches
            int n6 = 8;
            Console.WriteLine($"Test 6 - Comparing all approaches for n = {n6}:");
            Console.WriteLine($"Space-Optimized DP: {solution.ClimbStairs(n6)}");
            Console.WriteLine($"Tabulation DP:      {solution.ClimbStairsTabulation(n6)}");
            Console.WriteLine($"Memoization:        {solution.ClimbStairsMemo(n6)}");
            Console.WriteLine($"Recursive:          {solution.ClimbStairsRecursive(n6)}");
            Console.WriteLine($"All should be: 34\n");
            
            // Fibonacci pattern demonstration
            Console.WriteLine("=== Fibonacci Pattern ===");
            Console.WriteLine("n  | Ways | Fibonacci");
            Console.WriteLine("---|------|----------");
            for (int i = 1; i <= 10; i++)
            {
                Console.WriteLine($"{i,2} | {solution.ClimbStairs(i),4} | F({i})");
            }
            Console.WriteLine("\nNotice: ClimbStairs(n) = Fibonacci(n+1)!");
        }
    }
}
