/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Subsets
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
Given an integer array nums of unique elements, return all possible subsets 
(the power set).

The solution set must not contain duplicate subsets. Return the solution in 
any order.

A subset is a selection of elements from the array where order doesn't matter 
and elements can be chosen or not chosen. The power set includes the empty set 
and the set itself.

INPUT:
- nums: int[] - Array of unique integers
- 1 <= nums.length <= 10
- -10 <= nums[i] <= 10
- All elements are unique

OUTPUT:
- IList<IList<int>> - All possible subsets

EXAMPLES:
Example 1:
Input: nums = [1,2,3]
Output: [[],[1],[2],[1,2],[3],[1,3],[2,3],[1,2,3]]
Explanation: The power set contains all combinations including empty set

Example 2:
Input: nums = [0]
Output: [[],[0]]
Explanation: Single element has 2 subsets: empty and itself

Example 3:
Input: nums = [1,2]
Output: [[],[1],[2],[1,2]]
Explanation: Two elements yield 4 subsets (2^2)

Example 4:
Input: nums = [5,1,3]
Output: [[],[5],[1],[5,1],[3],[5,3],[1,3],[5,1,3]]
Explanation: Order in output doesn't matter; this is one valid ordering

CONSTRAINTS:
- 1 <= nums.length <= 10
- -10 <= nums[i] <= 10
- All the numbers of nums are unique

EDGE CASES:
✓ Single element array → [[],[element]]
✓ Negative numbers → treated same as positive
✓ Array with zero → zero is just another element
✓ Maximum size (10 elements) → 2^10 = 1024 subsets
✓ Empty subset must always be included

TRICK CASES:
⚡ Must include empty subset [] in result
⚡ Number of subsets = 2^n (exponential growth)
⚡ Order of subsets in output doesn't matter
⚡ Order within each subset doesn't matter ([1,2] same as [2,1])
⚡ Don't confuse with permutations (order matters) or combinations (fixed size)
⚡ Each element has binary choice: include it or don't include it

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OPTIMAL SOLUTION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APPROACH: Backtracking (Decision Tree)
Generate all subsets by making binary decisions at each position: include the 
current element or skip it.

Think of it as building a decision tree:
  "For element at index i, I have 2 choices:"
  "1. Include nums[i] in current subset → explore with it"
  "2. Don't include nums[i] → explore without it"
  "When I've made decisions for all elements → one subset is complete"

KEY INSIGHT:
• Every subset is the result of a sequence of binary decisions (include/exclude)
• This naturally maps to a binary tree where each path represents one subset
• Backtracking explores all paths in the decision tree
• Unlike combinations, we add the current subset at EVERY node, not just leaves

VISUAL EXAMPLE: nums = [1,2,3]

                            []
                    /                \
                  [1]                 []
              /        \           /      \
            [1,2]      [1]       [2]       []
           /    \     /   \     /   \     /   \
       [1,2,3][1,2] [1,3][1]  [2,3][2]  [3]  []

Result: [[],[1],[2],[1,2],[3],[1,3],[2,3],[1,2,3]]

Notice: We collect subsets at EVERY node in the tree!

ALGORITHM STEPS:
1. Create an empty result list to store all subsets
2. Start backtracking with index 0 and empty current subset
3. At each step:
   a. Add current subset to result (add at every level!)
   b. For each remaining element from index to end:
      - Include element in current subset (CHOOSE)
      - Recursively explore with next index (EXPLORE)
      - Remove element from current subset (BACKTRACK)
4. Return result with all subsets

WHY START LOOP FROM index (not 0):
- Prevents duplicates like [1,2] and [2,1] being counted as different
- Ensures we only move forward, maintaining order
- Each element considered once per recursive branch

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
COMPLEXITY ANALYSIS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

TIME COMPLEXITY: O(n × 2^n)
  - n = length of nums array
  - Total subsets = 2^n (each element: in or out)
  - For each subset, we copy it to result → O(n) per subset
  - Total: O(n × 2^n)
  
  Examples:
    nums = [1,2]     → 2^2 = 4 subsets
    nums = [1,2,3]   → 2^3 = 8 subsets  
    nums = [1..10]   → 2^10 = 1024 subsets

SPACE COMPLEXITY: O(n)
  - Recursion stack depth = n (one level per element)
  - Current subset list = O(n) maximum
  - Result list size = O(n × 2^n) but this is output, not auxiliary space
  - Auxiliary space (excluding output) = O(n)

PATTERN: Backtracking (Binary Decisions)
DIFFICULTY: Medium
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;
using System.Collections.Generic;

namespace DSA_CSharp.Backtracking
{
    public class Subsets
    {
        /// <summary>
        /// Generates all possible subsets (power set) of the given array.
        /// Uses backtracking to explore include/exclude decisions for each element.
        /// </summary>
        public IList<IList<int>> GenerateSubsets(int[] nums)
        {
            IList<IList<int>> result = new List<IList<int>>();
            List<int> currentSubset = new List<int>();
            
            // Start backtracking from index 0
            Backtrack(nums, 0, currentSubset, result);
            
            return result;
        }
        
        /*
        ────────────────────────────────────────────────────────────────────
        BACKTRACKING FUNCTION
        ────────────────────────────────────────────────────────────────────
        
        Parameters:
        - nums: The input array
        - index: Current position in nums (start of elements to consider)
        - currentSubset: Current subset being built
        - result: List of all subsets
        
        Key Difference from Other Backtracking Problems:
        - We add currentSubset to result at EVERY recursive call, not just base case
        - This is because every state represents a valid subset
        
        Why loop starts at 'index':
        - Ensures we only consider elements ahead (no going back)
        - Prevents duplicates: [1,2] won't be generated twice as [1,2] and [2,1]
        - Maintains lexicographic order if input is sorted
        
        Recursive Pattern:
        1. Add current state to result (valid subset)
        2. For each element from index onwards:
            a. CHOOSE: Add element to currentSubset
            b. EXPLORE: Recurse with next index
            c. BACKTRACK: Remove element from currentSubset
        */
        private void Backtrack(int[] nums, int index, List<int> currentSubset, 
                               IList<IList<int>> result)
        {
            // Add current subset to result
            // Make a copy because currentSubset will be modified
            result.Add(new List<int>(currentSubset));
            
            // Try adding each remaining element
            for (int i = index; i < nums.Length; i++)
            {
                // CHOOSE: Include nums[i] in current subset
                currentSubset.Add(nums[i]);
                
                // EXPLORE: Recursively build subsets with nums[i] included
                // Start from i+1 to avoid duplicates and ensure forward movement
                Backtrack(nums, i + 1, currentSubset, result);
                
                // BACKTRACK: Remove nums[i] to try other combinations
                currentSubset.RemoveAt(currentSubset.Count - 1);
            }
        }
    }
}

/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
DETAILED WALKTHROUGH: nums = [1,2,3]
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Step-by-Step Execution:

┌──────────────────────────────────────────────────────────────────────────┐
│ Initial Call: Backtrack(nums=[1,2,3], index=0, current=[], result=[])   │
└──────────────────────────────────────────────────────────────────────────┘

1. Add [] to result → result = [[]]
   Loop: i=0,1,2

   ┌─ i=0: Try including 1 ─────────────────────────────────────┐
   │ current = [1]                                               │
   │ Backtrack(nums, index=1, current=[1], result)              │
   │                                                             │
   │   2. Add [1] to result → result = [[],[1]]                 │
   │      Loop: i=1,2                                            │
   │                                                             │
   │      ┌─ i=1: Try including 2 ──────────────────────┐       │
   │      │ current = [1,2]                              │       │
   │      │ Backtrack(nums, index=2, current=[1,2])     │       │
   │      │                                              │       │
   │      │   3. Add [1,2] to result                    │       │
   │      │      → result = [[],[1],[1,2]]              │       │
   │      │      Loop: i=2                               │       │
   │      │                                              │       │
   │      │      ┌─ i=2: Try including 3 ─────┐         │       │
   │      │      │ current = [1,2,3]           │         │       │
   │      │      │ Backtrack(nums, index=3)    │         │       │
   │      │      │                              │         │       │
   │      │      │   4. Add [1,2,3] to result  │         │       │
   │      │      │      → result = [[],[1],    │         │       │
   │      │      │         [1,2],[1,2,3]]      │         │       │
   │      │      │      Loop: i=3 (end)        │         │       │
   │      │      │      Return                 │         │       │
   │      │      └─────────────────────────────┘         │       │
   │      │                                              │       │
   │      │   Backtrack: Remove 3                       │       │
   │      │   current = [1,2]                           │       │
   │      │   Loop ends, Return                          │       │
   │      └──────────────────────────────────────────────┘       │
   │                                                             │
   │   Backtrack: Remove 2                                      │
   │   current = [1]                                            │
   │                                                             │
   │      ┌─ i=2: Try including 3 ──────────────────────┐       │
   │      │ current = [1,3]                              │       │
   │      │ Backtrack(nums, index=3, current=[1,3])     │       │
   │      │                                              │       │
   │      │   5. Add [1,3] to result                    │       │
   │      │      → result = [[],[1],[1,2],[1,2,3],     │       │
   │      │         [1,3]]                               │       │
   │      │      Loop: i=3 (end)                         │       │
   │      │      Return                                  │       │
   │      └──────────────────────────────────────────────┘       │
   │                                                             │
   │   Backtrack: Remove 3                                      │
   │   current = [1]                                            │
   │   Loop ends, Return                                         │
   └─────────────────────────────────────────────────────────────┘
   
   Backtrack: Remove 1
   current = []
   
   ┌─ i=1: Try including 2 ─────────────────────────────────────┐
   │ current = [2]                                               │
   │ Backtrack(nums, index=2, current=[2], result)              │
   │                                                             │
   │   6. Add [2] to result                                     │
   │      → result = [[],[1],[1,2],[1,2,3],[1,3],[2]]          │
   │      Loop: i=2                                              │
   │                                                             │
   │      ┌─ i=2: Try including 3 ──────────────────────┐       │
   │      │ current = [2,3]                              │       │
   │      │ Backtrack(nums, index=3, current=[2,3])     │       │
   │      │                                              │       │
   │      │   7. Add [2,3] to result                    │       │
   │      │      → result = [[],[1],[1,2],[1,2,3],     │       │
   │      │         [1,3],[2],[2,3]]                    │       │
   │      │      Loop: i=3 (end)                         │       │
   │      │      Return                                  │       │
   │      └──────────────────────────────────────────────┘       │
   │                                                             │
   │   Backtrack: Remove 3                                      │
   │   current = [2]                                            │
   │   Loop ends, Return                                         │
   └─────────────────────────────────────────────────────────────┘
   
   Backtrack: Remove 2
   current = []
   
   ┌─ i=2: Try including 3 ─────────────────────────────────────┐
   │ current = [3]                                               │
   │ Backtrack(nums, index=3, current=[3], result)              │
   │                                                             │
   │   8. Add [3] to result                                     │
   │      → result = [[],[1],[1,2],[1,2,3],[1,3],[2],          │
   │         [2,3],[3]]                                          │
   │      Loop: i=3 (end)                                        │
   │      Return                                                 │
   └─────────────────────────────────────────────────────────────┘
   
   Backtrack: Remove 3
   current = []
   Loop ends, Return

FINAL RESULT: [[],[1],[1,2],[1,2,3],[1,3],[2],[2,3],[3]]

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
FOCUSED EXAMPLE: HOW [2,3] GETS ADDED
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Let's trace the specific path that creates the subset [2,3]:

Step 1: Initial Call
────────────────────────────────────────────────────────────────
Backtrack(nums=[1,2,3], index=0, current=[], result=[])
- Add [] to result → result = [[]]
- Loop starts: i = 0, 1, 2

Step 2: Explore with element 1 (i=0)
────────────────────────────────────────────────────────────────
- Add 1 to current → current = [1]
- Call Backtrack(nums, index=1, current=[1])
  - Adds [1], [1,2], [1,2,3], [1,3] to result
  - Returns after exploring all subsets containing 1
- Backtrack: Remove 1 → current = []
- Now result = [[],[1],[1,2],[1,2,3],[1,3]]

Step 3: Move to element 2 (i=1) ⭐ THIS IS WHERE [2,3] STARTS!
────────────────────────────────────────────────────────────────
- Add 2 to current → current = [2]
- Call Backtrack(nums=[1,2,3], index=2, current=[2])
  
  Inside this recursive call:
  ├─ Add [2] to result → result = [[],[1],[1,2],[1,2,3],[1,3],[2]]
  │
  └─ Loop starts at index=2: i = 2
     
     Step 4: Add element 3 (i=2) ⭐ CREATES [2,3]!
     ─────────────────────────────────────────────────
     - Add 3 to current → current = [2,3]
     - Call Backtrack(nums=[1,2,3], index=3, current=[2,3])
       
       Inside this recursive call:
       ├─ Add [2,3] to result ✅
       │  → result = [[],[1],[1,2],[1,2,3],[1,3],[2],[2,3]]
       │
       └─ Loop starts at index=3 (out of bounds, loop doesn't run)
       └─ Return
     
     - Backtrack: Remove 3 → current = [2]
     - Loop ends (no more elements)
     - Return
  
- Backtrack: Remove 2 → current = []

Step 5: Continue with element 3 (i=2)
────────────────────────────────────────────────────────────────
- Add 3 to current → current = [3]
- Call Backtrack(nums, index=3, current=[3])
  - Adds [3] to result
  - Returns
- Done!

KEY INSIGHT FOR [2,3]:
─────────────────────────────────────────────────────────────────
The loop starts at 'index', NOT at 0. This means:
- When we add element 2 (at i=1), we call Backtrack with index=2
- This SKIPS element 1 (it's behind us now)
- From that point, we can only add elements 2 and 3 onwards
- This creates [2] first, then [2,3] by adding 3

Why we don't revisit element 1:
- Loop in Backtrack(index=2) starts at i=2, not i=0
- This prevents going backwards and creating [2,1]
- Ensures no duplicate subsets like [1,2] and [2,1]

Visual Path for [2,3]:
                []
                 |
            (skip 1)
                 |
             Add 2 → [2] ← Added to result first!
                 |
             Add 3 → [2,3] ← Then this is added!

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
KEY OBSERVATIONS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

✓ We add to result IMMEDIATELY at each recursive call (not just at leaves)
✓ Loop starts at 'index' parameter, not 0 → prevents revisiting earlier elements
✓ This guarantees no duplicates: [1,2] won't appear as [2,1]
✓ Each path in recursion tree represents valid subset
✓ Total recursive calls = 2^n (one for each subset)
✓ Empty set [] is added first (before entering any loop)

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
ALTERNATIVE APPROACH: Iterative (Bit Manipulation)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

For nums = [1,2,3], there are 2^3 = 8 subsets.
Each subset maps to a 3-bit number (0 to 7):

Bits  Binary  Subset      Explanation
─────────────────────────────────────
000   0       []          No elements included
001   1       [3]         Only bit 0 set (rightmost)
010   2       [2]         Only bit 1 set
011   3       [2,3]       Bits 0 and 1 set
100   4       [1]         Only bit 2 set
101   5       [1,3]       Bits 0 and 2 set
110   6       [1,2]       Bits 1 and 2 set
111   7       [1,2,3]     All bits set

Algorithm:
1. Iterate from 0 to 2^n - 1
2. For each number, check which bits are set
3. If bit i is set, include nums[i] in subset

public IList<IList<int>> GenerateSubsetsIterative(int[] nums)
{
    IList<IList<int>> result = new List<IList<int>>();
    int n = nums.Length;
    int totalSubsets = 1 << n;  // 2^n
    
    for (int mask = 0; mask < totalSubsets; mask++)
    {
        List<int> subset = new List<int>();
        
        for (int i = 0; i < n; i++)
        {
            // Check if i-th bit is set in mask
            if ((mask & (1 << i)) != 0)
            {
                subset.Add(nums[i]);
            }
        }
        
        result.Add(subset);
    }
    
    return result;
}

Time Complexity: O(n × 2^n) - same as backtracking
Space Complexity: O(1) auxiliary (no recursion stack)

Both approaches are equally valid!
Backtracking is more intuitive, iterative is more elegant.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
COMPARISON WITH SIMILAR PROBLEMS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Subsets vs Combinations vs Permutations:

Problem        Output                   Size  Order Matters  Pattern
─────────────────────────────────────────────────────────────────────
Subsets        All combinations         2^n   No             Include/exclude
               of any size                                   each element

Combinations   All groups of            C(n,k) No            Choose k from n
               fixed size k                                   elements

Permutations   All arrangements         n!    Yes            All orderings
               of all elements                               matter

Examples with [1,2,3]:

Subsets:       [[],[1],[2],[3],[1,2],[1,3],[2,3],[1,2,3]]
Combinations   [[1,2],[1,3],[2,3]]  (if k=2)
(k=2):         
Permutations:  [[1,2,3],[1,3,2],[2,1,3],[2,3,1],[3,1,2],[3,2,1]]

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/
