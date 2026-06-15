/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Letter Combinations of a Phone Number
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
Given a string containing digits from 2-9 inclusive, return all possible letter 
combinations that the number could represent. Return the answer in any order.

A mapping of digits to letters (just like on the telephone buttons) is given below. 
Note that 1 does not map to any letters.

Phone Keypad Mapping:
  2 → "abc"
  3 → "def"
  4 → "ghi"
  5 → "jkl"
  6 → "mno"
  7 → "pqrs"
  8 → "tuv"
  9 → "wxyz"

INPUT:
- digits: string of digits from 2-9
- Length: 0 to 4

OUTPUT:
- List<string>: All possible letter combinations

EXAMPLES:
Example 1:
Input: digits = "23"
Output: ["ad","ae","af","bd","be","bf","cd","ce","cf"]
Explanation:
  2 → "abc"
  3 → "def"
  All combinations: a+d, a+e, a+f, b+d, b+e, b+f, c+d, c+e, c+f

Example 2:
Input: digits = ""
Output: []
Explanation: Empty input returns empty result

Example 3:
Input: digits = "2"
Output: ["a","b","c"]
Explanation: Single digit returns all its letters

Example 4:
Input: digits = "234"
Output: ["adg","adh","adi","aeg","aeh","aei","afg","afh","afi",
         "bdg","bdh","bdi","beg","beh","bei","bfg","bfh","bfi",
         "cdg","cdh","cdi","ceg","ceh","cei","cfg","cfh","cfi"]

CONSTRAINTS:
- 0 <= digits.length <= 4
- digits[i] is a digit in the range ['2', '9']

EDGE CASES:
✓ Empty string → return empty list
✓ Single digit → return all letters for that digit
✓ Digits with different letter counts (e.g., "79" where 7 has 4 letters, 9 has 4 letters)
✓ Maximum length (4 digits)

TRICK CASES:
⚡ Empty input must return empty list, not list with empty string
⚡ Digit '7' and '9' have 4 letters (not 3 like others)
⚡ Result size grows exponentially: 3^n or 4^n combinations
⚡ Order of results doesn't matter (any order is acceptable)

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OPTIMAL SOLUTION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APPROACH: Backtracking
Build combinations by exploring all possible letter choices for each digit.

Think of it as making a decision at each step:
  "For digit 2, I can choose 'a', 'b', or 'c'"
  "After choosing, move to next digit and make next choice"
  "When all digits are processed, we have one complete combination"
  "Backtrack and try other choices"

Step-by-step:
1. Create a mapping: digit → letters
2. Start with empty current combination
3. For each digit in the input:
   - Try each letter that digit maps to
   - Add letter to current combination
   - Recursively process next digit
   - Remove letter (backtrack) and try next letter
4. When we've processed all digits, add combination to result

KEY INSIGHTS:
• Classic Backtracking Pattern: Make choice → Explore → Backtrack
• Build combination incrementally (one character at a time)
• Each digit position is independent - any letter from digit[i] can combine with any letter from digit[i+1]
• Recursion depth = number of digits
• At each level, branch out to try all possible letters for that digit

VISUAL EXAMPLE: digits = "23"

                          ""
                    /     |     \
                  "a"    "b"    "c"     (digit 2: try a, b, c)
                / | \   / | \   / | \
              ad ae af bd be bf cd ce cf  (digit 3: try d, e, f)

Each path from root to leaf = one combination!

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
COMPLEXITY ANALYSIS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

TIME COMPLEXITY: O(4^n × n)
  - n = length of digits string
  - Each digit maps to 3 or 4 letters
  - Worst case: all digits are 7 or 9 (4 letters each) → 4^n combinations
  - For each combination, we build a string of length n → × n
  - Total: O(4^n × n)
  
  Examples:
    digits = "23"  → 3 × 3 = 9 combinations
    digits = "234" → 3 × 3 × 3 = 27 combinations
    digits = "79"  → 4 × 4 = 16 combinations

SPACE COMPLEXITY: O(n)
  - Recursion stack depth = n (number of digits)
  - Current combination string = O(n)
  - Result list size = O(4^n × n) but this is output, not auxiliary space
  - Auxiliary space (excluding output) = O(n)

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace DSA_CSharp.Backtracking
{
    public class LetterCombinationsPhoneNumber
    {
        public IList<string> LetterCombinations(string digits)
        {
            List<string> result = new List<string>();
            
            // Edge case: empty input
            if (string.IsNullOrEmpty(digits))
            {
                return result;
            }
            
            // Phone keypad mapping
            // Index represents the digit (0-9), value is the letters
            string[] digitToLetters = new string[]
            {
                "",      // 0 (not used)
                "",      // 1 (not used)
                "abc",   // 2
                "def",   // 3
                "ghi",   // 4
                "jkl",   // 5
                "mno",   // 6
                "pqrs",  // 7
                "tuv",   // 8
                "wxyz"   // 9
            };
            
            // Start backtracking with empty current combination
            Backtrack(digits, 0, new StringBuilder(), result, digitToLetters);
            
            return result;
        }
        
        /*
        ────────────────────────────────────────────────────────────────────
        BACKTRACKING FUNCTION
        ────────────────────────────────────────────────────────────────────
        
        Parameters:
        - digits: The input digit string
        - index: Current position in digits string (which digit we're processing)
        - current: Current combination being built (StringBuilder for efficiency)
        - result: Final list of all combinations
        - digitToLetters: Mapping array from digit to letters
        
        Base Case:
        - When index == digits.Length, we've processed all digits
        - Current combination is complete → add to result
        
        Recursive Case:
        - Get the current digit
        - Get all possible letters for this digit
        - For each letter:
            1. Add letter to current combination (CHOOSE)
            2. Recursively process next digit (EXPLORE)
            3. Remove letter from current combination (BACKTRACK)
        
        Why StringBuilder?
        - Strings are immutable in C#
        - StringBuilder allows efficient add/remove operations
        - Append() to add, Remove() to backtrack
        */
        private void Backtrack(string digits, int index, StringBuilder current, 
                               List<string> result, string[] digitToLetters)
        {
            // BASE CASE: Processed all digits
            if (index == digits.Length)
            {
                // Current combination is complete
                result.Add(current.ToString());
                return;
            }
            
            // Get current digit and its possible letters
            char digit = digits[index];
            string letters = digitToLetters[digit - '0']; // Convert char '2' to int 2
            
            // Try each possible letter for this digit
            foreach (char letter in letters)
            {
                // CHOOSE: Add this letter to current combination
                current.Append(letter);
                
                // EXPLORE: Move to next digit
                Backtrack(digits, index + 1, current, result, digitToLetters);
                
                // BACKTRACK: Remove the letter to try other options
                current.Remove(current.Length - 1, 1);
            }
        }
    }
}

/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
DETAILED WALKTHROUGH: digits = "23"
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Initial State:
  digits = "23"
  digitToLetters[2] = "abc"
  digitToLetters[3] = "def"
  
Call Stack Visualization:

Backtrack(digits="23", index=0, current="", result=[])
├─ digit = '2', letters = "abc"
│
├─ Try letter 'a':
│  ├─ current.Append('a') → current = "a"
│  ├─ Backtrack(digits="23", index=1, current="a", result=[])
│  │  ├─ digit = '3', letters = "def"
│  │  │
│  │  ├─ Try letter 'd':
│  │  │  ├─ current.Append('d') → current = "ad"
│  │  │  ├─ Backtrack(digits="23", index=2, current="ad", result=[])
│  │  │  │  └─ index == 2 (length) → BASE CASE
│  │  │  │  └─ result.Add("ad") → result = ["ad"]
│  │  │  │  └─ return
│  │  │  └─ current.Remove() → current = "a"
│  │  │
│  │  ├─ Try letter 'e':
│  │  │  ├─ current.Append('e') → current = "ae"
│  │  │  ├─ Backtrack(digits="23", index=2, current="ae", result=["ad"])
│  │  │  │  └─ index == 2 → BASE CASE
│  │  │  │  └─ result.Add("ae") → result = ["ad", "ae"]
│  │  │  │  └─ return
│  │  │  └─ current.Remove() → current = "a"
│  │  │
│  │  └─ Try letter 'f':
│  │     ├─ current.Append('f') → current = "af"
│  │     ├─ Backtrack(digits="23", index=2, current="af", result=["ad","ae"])
│  │     │  └─ index == 2 → BASE CASE
│  │     │  └─ result.Add("af") → result = ["ad","ae","af"]
│  │     │  └─ return
│  │     └─ current.Remove() → current = "a"
│  │
│  └─ current.Remove() → current = ""
│
├─ Try letter 'b':
│  ├─ current.Append('b') → current = "b"
│  ├─ Backtrack(digits="23", index=1, current="b", result=["ad","ae","af"])
│  │  ├─ digit = '3', letters = "def"
│  │  │
│  │  ├─ Try 'd': current = "bd" → result.Add("bd")
│  │  ├─ Try 'e': current = "be" → result.Add("be")
│  │  └─ Try 'f': current = "bf" → result.Add("bf")
│  │
│  └─ current.Remove() → current = ""
│
└─ Try letter 'c':
   ├─ current.Append('c') → current = "c"
   ├─ Backtrack(digits="23", index=1, current="c", result=[...6 items...])
   │  ├─ Try 'd': current = "cd" → result.Add("cd")
   │  ├─ Try 'e': current = "ce" → result.Add("ce")
   │  └─ Try 'f': current = "cf" → result.Add("cf")
   │
   └─ current.Remove() → current = ""

Final result = ["ad","ae","af","bd","be","bf","cd","ce","cf"]

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
KEY OBSERVATIONS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

1. TREE STRUCTURE:
   - Each level = one digit
   - Each branch = one letter choice
   - Each path from root to leaf = one combination
   
2. BACKTRACKING PATTERN:
   - Append letter (make choice)
   - Recurse (explore with that choice)
   - Remove letter (undo choice, try next)
   
3. EFFICIENCY:
   - StringBuilder vs String:
     * String: "a" + "d" creates new string object
     * StringBuilder: Append/Remove modifies same object
   - For 4 digits, could have 256 recursive calls
   - StringBuilder saves memory allocations
   
4. ORDER OF RESULTS:
   - Determined by order of iteration
   - First digit's first letter tried first
   - Results in alphabetical order naturally
   - But problem says "any order" is acceptable

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
COMPARISON WITH OTHER APPROACHES
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APPROACH 1: Iterative (BFS-style)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
Process each digit one at a time, expanding all existing combinations:

  Start: result = [""]
  
  Process digit '2' (letters "abc"):
    For each existing combination: ""
      Add 'a': "a"
      Add 'b': "b"
      Add 'c': "c"
    result = ["a", "b", "c"]
  
  Process digit '3' (letters "def"):
    For each existing combination: "a", "b", "c"
      "a" → "ad", "ae", "af"
      "b" → "bd", "be", "bf"
      "c" → "cd", "ce", "cf"
    result = ["ad","ae","af","bd","be","bf","cd","ce","cf"]

Code:
  public IList<string> LetterCombinationsIterative(string digits)
  {
      if (string.IsNullOrEmpty(digits)) return new List<string>();
      
      string[] mapping = {"", "", "abc", "def", "ghi", "jkl", "mno", "pqrs", "tuv", "wxyz"};
      List<string> result = new List<string> { "" }; // Start with empty string
      
      foreach (char digit in digits)
      {
          string letters = mapping[digit - '0'];
          List<string> temp = new List<string>();
          
          // Expand each existing combination
          foreach (string combo in result)
          {
              foreach (char letter in letters)
              {
                  temp.Add(combo + letter);
              }
          }
          
          result = temp;
      }
      
      return result;
  }

Pros:
  ✓ Easier to understand (no recursion)
  ✓ Iterative approach is intuitive
  ✓ No stack overflow risk

Cons:
  ✗ Creates many intermediate lists
  ✗ More memory usage (stores all combinations at each step)

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
APPROACH 2: Backtracking (Current Solution) ✓ OPTIMAL
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Pros:
  ✓ Memory efficient (StringBuilder reused)
  ✓ Clean recursive structure
  ✓ Matches problem's exploration nature
  ✓ Only stores final results

Cons:
  ✗ Recursion overhead (but depth is small: max 4)
  ✗ Harder to debug for beginners

Why it's optimal:
  - Both approaches are O(4^n × n) time
  - Backtracking uses O(n) space vs O(4^n) for iterative
  - StringBuilder optimization reduces allocations
  - Natural fit for "explore all possibilities" problems

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
COMMON MISTAKES TO AVOID
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

❌ MISTAKE 1: Returning [""] for empty input
  Wrong:
    if (string.IsNullOrEmpty(digits))
        return new List<string> { "" };  // ❌ Should be empty list!
  
  Correct:
    if (string.IsNullOrEmpty(digits))
        return new List<string>();  // ✓ Empty list

❌ MISTAKE 2: Using string concatenation instead of StringBuilder
  Wrong:
    Backtrack(digits, index, current + letter, result, mapping);  
    // ❌ Creates new string each time! Inefficient!
  
  Correct:
    current.Append(letter);
    Backtrack(digits, index + 1, current, result, mapping);
    current.Remove(current.Length - 1, 1);
    // ✓ Reuses StringBuilder

❌ MISTAKE 3: Forgetting to backtrack (remove letter)
  Wrong:
    foreach (char letter in letters)
    {
        current.Append(letter);
        Backtrack(digits, index + 1, current, result, mapping);
        // ❌ Missing current.Remove()!
    }
    // This would keep accumulating letters!
  
  Correct:
    foreach (char letter in letters)
    {
        current.Append(letter);
        Backtrack(digits, index + 1, current, result, mapping);
        current.Remove(current.Length - 1, 1);  // ✓ Backtrack!
    }

❌ MISTAKE 4: Wrong array indexing for digit mapping
  Wrong:
    string letters = mapping[digit];  
    // ❌ digit is char '2', not int 2!
  
  Correct:
    string letters = mapping[digit - '0'];  
    // ✓ Convert char '2' to int 2

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
RELATED PROBLEMS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Same Pattern (Backtracking - Combinations):
  • Generate Parentheses
  • Combination Sum
  • Permutations
  • Subsets

Similar Technique:
  • Word Search (backtracking in 2D grid)
  • N-Queens (backtracking with constraints)
  • Sudoku Solver (backtracking with validation)

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/
