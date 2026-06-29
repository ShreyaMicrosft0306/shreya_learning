/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Longest Valid Parentheses
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
Given a string containing just the characters '(' and ')', return the length 
of the longest valid (well-formed) parentheses substring.

A valid parentheses substring is one where every opening bracket has a 
corresponding closing bracket in the correct order.

INPUT:
- s: string - Contains only '(' and ')' characters
- 0 <= s.length <= 3 × 10^4

OUTPUT:
- int - Length of the longest valid parentheses substring

EXAMPLES:
Example 1:
Input: s = "(()"
Output: 2
Explanation: The longest valid parentheses substring is "()".

Example 2:
Input: s = ")()())"
Output: 4
Explanation: The longest valid parentheses substring is "()()".

Example 3:
Input: s = ""
Output: 0
Explanation: Empty string has no valid parentheses.

Example 4:
Input: s = "()(()"
Output: 2
Explanation: The longest valid parentheses substring is "()".

Example 5:
Input: s = "()(())"
Output: 6
Explanation: The entire string is valid.

Example 6:
Input: s = "(()()"
Output: 4
Explanation: The valid substring is "()()" or "(())".

CONSTRAINTS:
- 0 <= s.length <= 3 × 10^4
- s[i] is '(' or ')'

EDGE CASES:
✓ Empty string → 0
✓ Single character → 0 (can't be valid)
✓ All opening brackets "(((" → 0
✓ All closing brackets ")))" → 0
✓ Entire string is valid → s.length
✓ Multiple valid substrings (need longest)
✓ Nested parentheses "(())"
✓ Mixed valid/invalid parts

TRICK CASES:
⚡ Valid substrings can be non-contiguous when considering indices
⚡ Don't confuse with "Valid Parentheses" (different problem)
⚡ Need to handle both standalone pairs "()" and nested "((()))"
⚡ A valid substring doesn't have to start at index 0
⚡ Multiple approaches: Stack, DP, Two-Pass

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OPTIMAL SOLUTION - STACK APPROACH
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APPROACH:
Use a stack to track indices of unmatched parentheses. The stack helps us
identify valid substrings by marking the boundaries of invalid sections.

KEY INSIGHTS:
• Stack stores indices, not characters
• Initialize stack with -1 as a base (start marker)
• For '(': push index to stack
• For ')': 
  - Pop from stack
  - If stack becomes empty: push current index (new base)
  - Else: calculate length from current index to top of stack
• Maximum length is tracked throughout

ALGORITHM STEPS:
1. Initialize stack with -1 (base index)
2. For each character at index i:
   a. If '(': push i to stack
   b. If ')':
      - Pop from stack
      - If stack is empty:
        * Push i (this ')' is unmatched, becomes new base)
      - Else:
        * Calculate length: i - stack.peek()
        * Update maxLength
3. Return maxLength

EXAMPLE WALKTHROUGH: s = ")()())"
Index:  0 1 2 3 4 5
Char:   ) ( ) ( ) )

i=0, ')': pop(-1), stack empty → push(0), stack=[0]
i=1, '(': push(1), stack=[0,1]
i=2, ')': pop(1), length=2-0=2, max=2, stack=[0]
i=3, '(': push(3), stack=[0,3]
i=4, ')': pop(3), length=4-0=4, max=4, stack=[0]
i=5, ')': pop(0), stack empty → push(5), stack=[5]

Result: 4

TIME COMPLEXITY: O(n)
- Single pass through the string
- Each index pushed/popped at most once

SPACE COMPLEXITY: O(n)
- Stack can hold all indices in worst case

PATTERN: Stack, String Processing
DIFFICULTY: Hard
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;
using System.Collections.Generic;

namespace DSA_CSharp.Stacks
{
    public class LongestValidParentheses
    {
        /// <summary>
        /// Finds the length of longest valid parentheses substring using stack.
        /// </summary>
        /// <param name="s">String containing '(' and ')' characters</param>
        /// <returns>Length of longest valid parentheses substring</returns>
        public int LongestValidParenthesesStack(string s)
        {
            if (string.IsNullOrEmpty(s)) return 0;
            
            int maxLength = 0;
            Stack<int> stack = new Stack<int>();
            stack.Push(-1); // Base index
            
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '(')
                {
                    // Push index of opening bracket
                    stack.Push(i);
                }
                else // s[i] == ')'
                {
                    // Pop the matched opening bracket or previous base
                    stack.Pop();
                    
                    if (stack.Count == 0)
                    {
                        // No matching opening bracket, this becomes new base
                        stack.Push(i);
                    }
                    else
                    {
                        // Calculate length of current valid substring
                        int currentLength = i - stack.Peek();
                        maxLength = Math.Max(maxLength, currentLength);
                    }
                }
            }
            
            return maxLength;
        }
        
        /// <summary>
        /// Alternative solution using Dynamic Programming.
        /// dp[i] = length of longest valid parentheses ending at index i
        /// </summary>
        public int LongestValidParenthesesDP(string s)
        {
            if (string.IsNullOrEmpty(s)) return 0;
            
            int maxLength = 0;
            int[] dp = new int[s.Length];
            
            for (int i = 1; i < s.Length; i++)
            {
                if (s[i] == ')')
                {
                    if (s[i - 1] == '(')
                    {
                        // Case: ...()
                        dp[i] = (i >= 2 ? dp[i - 2] : 0) + 2;
                    }
                    else if (i - dp[i - 1] > 0 && s[i - dp[i - 1] - 1] == '(')
                    {
                        // Case: ...))
                        // Check if there's a matching '(' for current ')'
                        dp[i] = dp[i - 1] + 2 + 
                                (i - dp[i - 1] >= 2 ? dp[i - dp[i - 1] - 2] : 0);
                    }
                    
                    maxLength = Math.Max(maxLength, dp[i]);
                }
            }
            
            return maxLength;
        }
        
        /// <summary>
        /// Alternative solution using Two-Pass approach (left-to-right, right-to-left).
        /// Most space efficient: O(1) space.
        /// </summary>
        public int LongestValidParenthesesTwoPass(string s)
        {
            if (string.IsNullOrEmpty(s)) return 0;
            
            int maxLength = 0;
            
            // Left to right pass
            int left = 0, right = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '(')
                    left++;
                else
                    right++;
                
                if (left == right)
                    maxLength = Math.Max(maxLength, 2 * right);
                else if (right > left)
                    left = right = 0; // Reset when invalid
            }
            
            // Right to left pass (to handle cases like "(()")
            left = right = 0;
            for (int i = s.Length - 1; i >= 0; i--)
            {
                if (s[i] == '(')
                    left++;
                else
                    right++;
                
                if (left == right)
                    maxLength = Math.Max(maxLength, 2 * left);
                else if (left > right)
                    left = right = 0; // Reset when invalid
            }
            
            return maxLength;
        }
        
        // Test method
        public static void Test()
        {
            var solution = new LongestValidParentheses();
            
            // Test case 1
            string s1 = "(()";
            Console.WriteLine($"Test 1: s = \"{s1}\"");
            Console.WriteLine($"Stack:    {solution.LongestValidParenthesesStack(s1)}");
            Console.WriteLine($"DP:       {solution.LongestValidParenthesesDP(s1)}");
            Console.WriteLine($"TwoPass:  {solution.LongestValidParenthesesTwoPass(s1)}");
            Console.WriteLine($"Expected: 2\n");
            
            // Test case 2
            string s2 = ")()())";
            Console.WriteLine($"Test 2: s = \"{s2}\"");
            Console.WriteLine($"Stack:    {solution.LongestValidParenthesesStack(s2)}");
            Console.WriteLine($"DP:       {solution.LongestValidParenthesesDP(s2)}");
            Console.WriteLine($"TwoPass:  {solution.LongestValidParenthesesTwoPass(s2)}");
            Console.WriteLine($"Expected: 4\n");
            
            // Test case 3
            string s3 = "";
            Console.WriteLine($"Test 3: s = \"{s3}\"");
            Console.WriteLine($"Stack:    {solution.LongestValidParenthesesStack(s3)}");
            Console.WriteLine($"DP:       {solution.LongestValidParenthesesDP(s3)}");
            Console.WriteLine($"TwoPass:  {solution.LongestValidParenthesesTwoPass(s3)}");
            Console.WriteLine($"Expected: 0\n");
            
            // Test case 4
            string s4 = "()(())";
            Console.WriteLine($"Test 4: s = \"{s4}\"");
            Console.WriteLine($"Stack:    {solution.LongestValidParenthesesStack(s4)}");
            Console.WriteLine($"DP:       {solution.LongestValidParenthesesDP(s4)}");
            Console.WriteLine($"TwoPass:  {solution.LongestValidParenthesesTwoPass(s4)}");
            Console.WriteLine($"Expected: 6\n");
            
            // Test case 5
            string s5 = "(()(()";
            Console.WriteLine($"Test 5: s = \"{s5}\"");
            Console.WriteLine($"Stack:    {solution.LongestValidParenthesesStack(s5)}");
            Console.WriteLine($"DP:       {solution.LongestValidParenthesesDP(s5)}");
            Console.WriteLine($"TwoPass:  {solution.LongestValidParenthesesTwoPass(s5)}");
            Console.WriteLine($"Expected: 2\n");
            
            // Test case 6 - Edge case: all opening
            string s6 = "((((";
            Console.WriteLine($"Test 6: s = \"{s6}\"");
            Console.WriteLine($"Stack:    {solution.LongestValidParenthesesStack(s6)}");
            Console.WriteLine($"DP:       {solution.LongestValidParenthesesDP(s6)}");
            Console.WriteLine($"TwoPass:  {solution.LongestValidParenthesesTwoPass(s6)}");
            Console.WriteLine($"Expected: 0\n");
            
            // Test case 7 - Edge case: all closing
            string s7 = "))))";
            Console.WriteLine($"Test 7: s = \"{s7}\"");
            Console.WriteLine($"Stack:    {solution.LongestValidParenthesesStack(s7)}");
            Console.WriteLine($"DP:       {solution.LongestValidParenthesesDP(s7)}");
            Console.WriteLine($"TwoPass:  {solution.LongestValidParenthesesTwoPass(s7)}");
            Console.WriteLine($"Expected: 0\n");
            
            // Test case 8 - Complex case
            string s8 = "(()())";
            Console.WriteLine($"Test 8: s = \"{s8}\"");
            Console.WriteLine($"Stack:    {solution.LongestValidParenthesesStack(s8)}");
            Console.WriteLine($"DP:       {solution.LongestValidParenthesesDP(s8)}");
            Console.WriteLine($"TwoPass:  {solution.LongestValidParenthesesTwoPass(s8)}");
            Console.WriteLine($"Expected: 6\n");
        }
    }
}
