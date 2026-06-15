/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Generate Parentheses
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
Given n pairs of parentheses, write a function to generate all combinations of 
well-formed parentheses.

A well-formed (valid) parentheses expression means:
- Every opening parenthesis '(' has a matching closing parenthesis ')'
- Closing parentheses must come after their corresponding opening parentheses
- All parentheses are properly nested and balanced

INPUT:
- n: int - Number of pairs of parentheses
- 1 <= n <= 8

OUTPUT:
- IList<string> - All valid parentheses combinations

EXAMPLES:
Example 1:
Input: n = 1
Output: ["()"]
Explanation: Only one way to arrange 1 pair

Example 2:
Input: n = 2
Output: ["(())", "()()"]
Explanation: Two valid arrangements for 2 pairs

Example 3:
Input: n = 3
Output: ["((()))", "(()())", "(())()", "()(())", "()()()"]
Explanation: Five valid arrangements for 3 pairs

Example 4:
Input: n = 4
Output: ["(((())))", "((()()))", "((())())", "((()))()", "(()(()))", 
         "(()()())", "(()())()", "(())(())", "(())()()", "()((()))", 
         "()(()())", "()(())()", "()()(())", "()()()()"]
Explanation: 14 valid arrangements (Catalan number C(4))

CONSTRAINTS:
- 1 <= n <= 8
- Each output string has exactly 2n characters (n opening, n closing)

EDGE CASES:
✓ n = 1 → Only "()"
✓ n = 8 → Maximum input (1430 combinations)
✓ Must generate all valid combinations (no duplicates)
✓ Order of output doesn't matter

TRICK CASES:
⚡ Cannot start with ')' - invalid
⚡ Cannot have more closing than opening at any point - invalid
⚡ Must use exactly n opening and n closing parentheses
⚡ Number of valid combinations = Catalan Number C(n) = (2n)! / ((n+1)! × n!)
⚡ Cannot just generate all permutations and filter - too inefficient
⚡ Must build only valid combinations using backtracking with constraints

Invalid Examples:
- ")(" - starts with closing
- "(((" - not enough closing
- "())" - too many closing for openings used
- "(())" when n=1 - uses 2 pairs but n=1

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OPTIMAL SOLUTION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APPROACH: Backtracking with Constraints
Build valid parentheses strings incrementally by making smart choices at each 
step. Only add parentheses when they maintain validity.

Think of it as making decisions with rules:
  "At each position, I can add '(' or ')'"
  "BUT I can only add '(' if I haven't used all n opening parens"
  "AND I can only add ')' if it won't make the string invalid"
  "When I've placed all 2n characters → one valid combination is complete"

KEY INSIGHTS:
• Track how many opening '(' and closing ')' parentheses we've used
• Constraint 1: Can add '(' only if openCount < n
• Constraint 2: Can add ')' only if closeCount < openCount
• Constraint 2 ensures we never have more ')' than '(' at any point
• This prevents generating invalid strings entirely (no filtering needed!)
• Generate ONLY valid combinations → optimal efficiency

WHY closeCount < openCount?
- If we have 3 '(' and 2 ')', we can add another ')'
- If we have 2 '(' and 2 ')', we CANNOT add ')' (would be invalid)
- We need unmatched '(' before we can add matching ')'
- Example: "(()" → openCount=2, closeCount=1 → can add ')'
- Example: "(())" → openCount=2, closeCount=2 → cannot add ')'

VISUAL EXAMPLE: n = 3 (Decision Tree)

                                ""
                                |
                            Add '('
                                |
                              "("
                        /             \
                    Add '('          Add ')'
                       |                 |
                     "(("              "()"
                   /      \           /     \
               '('        ')'      '('      ')'
                |          |        |         |
              "((("      "(())"   "()("     "())"
              ...        ...      ...        INVALID
                                            (can't continue)

Every path that reaches 2n characters with balanced parens = valid combination!

ALGORITHM STEPS:
1. Create result list to store valid combinations
2. Start backtracking with:
   - Empty string (or StringBuilder for efficiency)
   - openCount = 0 (no opening parens used yet)
   - closeCount = 0 (no closing parens used yet)
3. At each step:
   a. BASE CASE: If current.Length == 2n, add to result
   b. If openCount < n → can add '(' → recurse with openCount+1
   c. If closeCount < openCount → can add ')' → recurse with closeCount+1
4. Return result with all valid combinations

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
COMPLEXITY ANALYSIS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

TIME COMPLEXITY: O(4^n / √n) or approximately O(C(n))
  - Number of valid combinations = Catalan Number C(n)
  - C(n) = (2n)! / ((n+1)! × n!) ≈ 4^n / (n√n)
  - For each combination, we build string of length 2n
  - Actual formula: O((4^n / √n) × n) but simplified to O(4^n / √n)
  
  Examples:
    n = 1 → C(1) = 1 combination
    n = 2 → C(2) = 2 combinations
    n = 3 → C(3) = 5 combinations
    n = 4 → C(4) = 14 combinations
    n = 8 → C(8) = 1,430 combinations
  
  Why not 2^(2n)?
  - That would be all possible strings of '(' and ')'
  - Most of those are invalid!
  - We only generate valid ones → much smaller set

SPACE COMPLEXITY: O(n)
  - Recursion stack depth = 2n (one call per character)
  - Current string builder = O(2n) = O(n)
  - Result list size = O(4^n / √n) but this is output, not auxiliary space
  - Auxiliary space (excluding output) = O(n)

PATTERN: Backtracking with Pruning (Constraint-based)
DIFFICULTY: Medium
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace DSA_CSharp.Backtracking
{
    public class GenerateParentheses
    {
        /// <summary>
        /// Generates all valid combinations of n pairs of parentheses.
        /// Uses backtracking with constraints to build only valid strings.
        /// </summary>
        public IList<string> Generate(int n)
        {
            IList<string> result = new List<string>();
            StringBuilder current = new StringBuilder();
            
            // Start backtracking with 0 opening and 0 closing parens used
            Backtrack(n, 0, 0, current, result);
            
            return result;
        }
        
        /*
        ────────────────────────────────────────────────────────────────────
        BACKTRACKING FUNCTION
        ────────────────────────────────────────────────────────────────────
        
        Parameters:
        - n: Total pairs of parentheses needed
        - openCount: Number of '(' used so far
        - closeCount: Number of ')' used so far
        - current: Current parentheses string being built
        - result: List of all valid combinations
        
        BASE CASE:
        - When current.Length == 2n, we've placed all parentheses
        - If we reached here, the string is valid (due to our constraints)
        - Add to result
        
        RECURSIVE CASES (with constraints):
        1. Add '(' if openCount < n
           - We haven't used all opening parentheses yet
           - Always safe to add opening paren
           
        2. Add ')' if closeCount < openCount
           - We have unmatched opening parentheses
           - Adding ')' will match one of them
           - Prevents creating invalid strings like ")(" or "())"
        
        WHY THESE CONSTRAINTS ENSURE VALIDITY:
        - Constraint 1: Ensures we use exactly n opening parens
        - Constraint 2: Ensures we never have more ')' than '(' at any point
        - Combined: Every ')' has a matching '(' before it
        - Result: Only valid, balanced strings are generated
        */
        private void Backtrack(int n, int openCount, int closeCount, 
                               StringBuilder current, IList<string> result)
        {
            // BASE CASE: String is complete (2n characters)
            if (current.Length == 2 * n)
            {
                // String is valid (ensured by our constraints)
                result.Add(current.ToString());
                return;
            }
            
            // CHOICE 1: Add opening parenthesis '('
            // Constraint: Only if we haven't used all n opening parens
            if (openCount < n)
            {
                // CHOOSE: Add '('
                current.Append('(');
                
                // EXPLORE: Recurse with openCount + 1
                Backtrack(n, openCount + 1, closeCount, current, result);
                
                // BACKTRACK: Remove '(' to try other options
                current.Remove(current.Length - 1, 1);
            }
            
            // CHOICE 2: Add closing parenthesis ')'
            // Constraint: Only if we have unmatched opening parens
            if (closeCount < openCount)
            {
                // CHOOSE: Add ')'
                current.Append(')');
                
                // EXPLORE: Recurse with closeCount + 1
                Backtrack(n, openCount, closeCount + 1, current, result);
                
                // BACKTRACK: Remove ')' to try other options
                current.Remove(current.Length - 1, 1);
            }
        }
    }
}

/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
DETAILED WALKTHROUGH: n = 2
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Goal: Generate all valid combinations of 2 pairs of parentheses
Expected Output: ["(())", "()()"]

Step-by-Step Execution:

┌──────────────────────────────────────────────────────────────────────────┐
│ Initial Call: Backtrack(n=2, open=0, close=0, current="", result=[])    │
└──────────────────────────────────────────────────────────────────────────┘

Length = 0, need 4 characters
Check: open < n? → 0 < 2 ✓ Can add '('

   ┌─ Add '(' (open=1, close=0) ──────────────────────────────────┐
   │ current = "("                                                  │
   │ Backtrack(n=2, open=1, close=0, current="(")                 │
   │                                                                │
   │ Length = 1, need 4                                            │
   │ Check: open < n? → 1 < 2 ✓ Can add '('                       │
   │                                                                │
   │    ┌─ Add '(' (open=2, close=0) ─────────────────┐           │
   │    │ current = "(("                                │           │
   │    │ Backtrack(n=2, open=2, close=0)              │           │
   │    │                                               │           │
   │    │ Length = 2, need 4                           │           │
   │    │ Check: open < n? → 2 < 2 ✗ Cannot add '('   │           │
   │    │ Check: close < open? → 0 < 2 ✓ Can add ')'  │           │
   │    │                                               │           │
   │    │    ┌─ Add ')' (open=2, close=1) ──────┐     │           │
   │    │    │ current = "(()"                   │     │           │
   │    │    │ Backtrack(n=2, open=2, close=1)  │     │           │
   │    │    │                                   │     │           │
   │    │    │ Length = 3, need 4               │     │           │
   │    │    │ Check: open < n? → 2 < 2 ✗       │     │           │
   │    │    │ Check: close < open? → 1 < 2 ✓   │     │           │
   │    │    │                                   │     │           │
   │    │    │   ┌─ Add ')' (open=2, close=2)   │     │           │
   │    │    │   │ current = "(())"              │     │           │
   │    │    │   │ Backtrack(n=2, open=2, close=2)    │           │
   │    │    │   │                                │     │           │
   │    │    │   │ Length = 4 ✓ Complete!       │     │           │
   │    │    │   │ Add "(())" to result          │     │           │
   │    │    │   │ result = ["(())"]             │     │           │
   │    │    │   │ Return                         │     │           │
   │    │    │   └───────────────────────────────┘     │           │
   │    │    │                                   │     │           │
   │    │    │ Backtrack: Remove ')'            │     │           │
   │    │    │ current = "(()"                   │     │           │
   │    │    │ Check: close < open? → 1 < 2     │     │           │
   │    │    │ (Already tried, no more)          │     │           │
   │    │    │ Return                            │     │           │
   │    │    └───────────────────────────────────┘     │           │
   │    │                                               │           │
   │    │ Backtrack: Remove ')'                        │           │
   │    │ current = "(("                                │           │
   │    │ Check: close < open? → 0 < 2                 │           │
   │    │ (Already tried, no more options)             │           │
   │    │ Return                                        │           │
   │    └───────────────────────────────────────────────┘           │
   │                                                                │
   │ Backtrack: Remove '('                                         │
   │ current = "("                                                  │
   │                                                                │
   │ Check: close < open? → 0 < 1 ✓ Can add ')'                   │
   │                                                                │
   │    ┌─ Add ')' (open=1, close=1) ─────────────────┐           │
   │    │ current = "()"                                │           │
   │    │ Backtrack(n=2, open=1, close=1)              │           │
   │    │                                               │           │
   │    │ Length = 2, need 4                           │           │
   │    │ Check: open < n? → 1 < 2 ✓ Can add '('      │           │
   │    │                                               │           │
   │    │    ┌─ Add '(' (open=2, close=1) ──────┐     │           │
   │    │    │ current = "()("                   │     │           │
   │    │    │ Backtrack(n=2, open=2, close=1)  │     │           │
   │    │    │                                   │     │           │
   │    │    │ Length = 3, need 4               │     │           │
   │    │    │ Check: open < n? → 2 < 2 ✗       │     │           │
   │    │    │ Check: close < open? → 1 < 2 ✓   │     │           │
   │    │    │                                   │     │           │
   │    │    │   ┌─ Add ')' (open=2, close=2)   │     │           │
   │    │    │   │ current = "()()"              │     │           │
   │    │    │   │ Backtrack(n=2, open=2, close=2)    │           │
   │    │    │   │                                │     │           │
   │    │    │   │ Length = 4 ✓ Complete!       │     │           │
   │    │    │   │ Add "()()" to result          │     │           │
   │    │    │   │ result = ["(())", "()()"]     │     │           │
   │    │    │   │ Return                         │     │           │
   │    │    │   └───────────────────────────────┘     │           │
   │    │    │                                   │     │           │
   │    │    │ Backtrack: Remove ')'            │     │           │
   │    │    │ current = "()("                   │     │           │
   │    │    │ Return                            │     │           │
   │    │    └───────────────────────────────────┘     │           │
   │    │                                               │           │
   │    │ Backtrack: Remove '('                        │           │
   │    │ current = "()"                                │           │
   │    │ Check: close < open? → 1 < 1 ✗               │           │
   │    │ (Cannot add more ')', already balanced)      │           │
   │    │ Return                                        │           │
   │    └───────────────────────────────────────────────┘           │
   │                                                                │
   │ Backtrack: Remove ')'                                         │
   │ current = "("                                                  │
   │ Return                                                         │
   └────────────────────────────────────────────────────────────────┘

Backtrack: Remove '('
current = ""
Return

FINAL RESULT: ["(())", "()()"]

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
KEY OBSERVATIONS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

✓ We ALWAYS try to add '(' first when possible (openCount < n)
✓ We only add ')' when we have unmatched '(' (closeCount < openCount)
✓ Constraints prevent generating invalid strings entirely
✓ No need to validate or filter results - all generated strings are valid
✓ Path from root to leaf in recursion tree = one valid combination
✓ Total leaves in tree = Catalan Number C(n)

Visual Decision Tree for n=2:

                              ""
                              |
                            Add '('
                              |
                             "("
                         /         \
                     Add '('      Add ')'
                        |            |
                      "(("          "()"
                       |             |
                   Add ')'       Add '('
                       |             |
                     "(()"         "()("
                       |             |
                   Add ')'       Add ')'
                       |             |
                    "(())"         "()()"
                    RESULT         RESULT

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
WHY THIS IS BETTER THAN BRUTE FORCE
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Brute Force Approach (BAD):
1. Generate all possible strings of length 2n with '(' and ')'
2. Check each string for validity
3. Keep only valid ones

Problems with brute force:
- Total strings = 2^(2n) - exponential explosion!
- For n=8: 2^16 = 65,536 strings to generate
- But only 1,430 are valid (Catalan number C(8))
- Wasting time generating 64,106 invalid strings!
- Then spending more time validating each one

Our Backtracking Approach (GOOD):
1. Generate ONLY valid strings using constraints
2. No validation needed - all generated strings are valid
3. For n=8: Generate exactly 1,430 strings (no waste!)

Comparison for n=8:
- Brute Force: Generate 65,536 + Validate all → Very slow
- Backtracking: Generate 1,430 → Much faster!

This is the power of constraint-based backtracking with pruning!

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
CATALAN NUMBERS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

The number of valid parentheses combinations for n pairs = C(n) (Catalan Number)

Formula: C(n) = (2n)! / ((n+1)! × n!)

First few Catalan numbers:
n    C(n)    Example
─────────────────────────────────────
0    1       "" (empty)
1    1       "()"
2    2       "(())", "()()"
3    5       "((()))", "(()())", "(())()", "()(())", "()()()"
4    14      [14 combinations]
5    42      [42 combinations]
6    132     [132 combinations]
7    429     [429 combinations]
8    1430    [1430 combinations]

Catalan numbers appear in many problems:
- Valid parentheses combinations
- Binary search trees with n nodes
- Ways to triangulate a polygon
- Paths in a grid that don't cross diagonal
- Ways to associate matrix multiplications

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/
