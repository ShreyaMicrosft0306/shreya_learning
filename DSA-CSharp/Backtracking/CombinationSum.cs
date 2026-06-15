/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Combination Sum
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
Given an array of distinct integers candidates and a target integer target, 
return a list of all unique combinations of candidates where the chosen numbers 
sum to target. You may return the combinations in any order.

The same number may be chosen from candidates an unlimited number of times. 
Two combinations are unique if the frequency of at least one of the chosen 
numbers is different.

INPUT:
- candidates: int[] - Array of distinct positive integers
- target: int - Target sum to achieve
- 1 <= candidates.length <= 30
- 2 <= candidates[i] <= 40
- All elements of candidates are distinct
- 1 <= target <= 40

OUTPUT:
- IList<IList<int>> - List of all unique combinations that sum to target

EXAMPLES:
Example 1:
Input: candidates = [2,3,6,7], target = 7
Output: [[2,2,3],[7]]
Explanation:
- 2 + 2 + 3 = 7
- 7 = 7
These are the only two combinations.

Example 2:
Input: candidates = [2,3,5], target = 8
Output: [[2,2,2,2],[2,3,3],[3,5]]
Explanation:
- 2 + 2 + 2 + 2 = 8
- 2 + 3 + 3 = 8
- 3 + 5 = 8

Example 3:
Input: candidates = [2], target = 1
Output: []
Explanation: No combination sums to 1.

Example 4:
Input: candidates = [1], target = 1
Output: [[1]]

Example 5:
Input: candidates = [1], target = 2
Output: [[1,1]]

CONSTRAINTS:
- 1 <= candidates.length <= 30
- 2 <= candidates[i] <= 40
- All elements in candidates are distinct
- 1 <= target <= 40

EDGE CASES:
✓ Target = 0 → return empty combination [[]]
✓ No valid combination → return []
✓ Single element array
✓ Can reuse same number multiple times
✓ Multiple valid combinations
✓ Target smaller than smallest candidate

TRICK CASES:
⚡ Same number can be used UNLIMITED times (key difference from Combination Sum II)
⚡ Must avoid duplicate combinations (e.g., [2,3] and [3,2] are same)
⚡ Use backtracking with index tracking to avoid duplicates
⚡ Early termination when sum exceeds target (pruning)
⚡ Don't sort if order doesn't matter, but sorting helps with pruning

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
SIMPLE EXPLANATION (KEY TO UNDERSTANDING!)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

IMAGINE YOU'RE AT A CANDY STORE:
You have coins: [2, 3, 5]
You need to pay exactly 8 cents
You can use each coin type as many times as you want

HOW DO YOU FIND ALL WAYS TO MAKE 8?

APPROACH: Try each coin, then recursively solve the remaining amount

Decision Tree for candidates = [2,3,5], target = 8:

                        Start (need 8)
                       /      |      \
                   Use 2   Use 3   Use 5
                   (need 6) (need 5) (need 3)
                  /   |   \
            Use 2  Use 3  Use 5
         (need 4)(need 3)(need 1)
           / | \
      Use 2...

KEY INSIGHTS:
1. At each step, we decide: "Should I include this number?"
2. If we include it, we can use it AGAIN (unlimited reuse!)
3. To avoid duplicates like [2,3] vs [3,2], we only move FORWARD in array
4. Base cases:
   - If sum = target: Found a solution! ✓
   - If sum > target: This path won't work, backtrack
   - If no more candidates: Stop exploring

AVOIDING DUPLICATES:
Wrong approach: Try all numbers at each step
  → Would generate [2,3] and [3,2] as separate combinations

Correct approach: Only try numbers from current index onwards
  → If we're exploring "number at index i", next we only try i, i+1, i+2...
  → This ensures we build combinations in non-decreasing order
  → [2,3] is built, but [3,2] is never considered

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
DETAILED SOLUTION WALKTHROUGH - HOW THE CODE WORKS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Let's trace through Example 2: candidates = [2,3,5], target = 8

────────────────────────────────────────────────────────────────────────────
INITIAL STATE:
────────────────────────────────────────────────────────────────────────────
candidates = [2, 3, 5]
target = 8
result = []
currentCombination = []

────────────────────────────────────────────────────────────────────────────
BACKTRACKING TREE EXPLORATION:
────────────────────────────────────────────────────────────────────────────

Level 0: Start
  currentSum = 0, target = 8, startIdx = 0
  currentCombination = []
  
  Try index 0 (value = 2):
  ↓

Level 1: Added 2
  currentSum = 2, remaining = 6, startIdx = 0
  currentCombination = [2]
  
  Try index 0 (value = 2) again (unlimited reuse!):
  ↓

Level 2: Added 2
  currentSum = 4, remaining = 4, startIdx = 0
  currentCombination = [2, 2]
  
  Try index 0 (value = 2) again:
  ↓

Level 3: Added 2
  currentSum = 6, remaining = 2, startIdx = 0
  currentCombination = [2, 2, 2]
  
  Try index 0 (value = 2) again:
  ↓

Level 4: Added 2
  currentSum = 8, remaining = 0 ✓ TARGET REACHED!
  currentCombination = [2, 2, 2, 2]
  → Add [2, 2, 2, 2] to result
  → Backtrack (remove last 2)

Back to Level 3: currentCombination = [2, 2, 2]
  Try index 1 (value = 3):
  currentSum = 6 + 3 = 9 > 8 ✗ EXCEEDS TARGET
  → Backtrack
  
  Try index 2 (value = 5):
  currentSum = 6 + 5 = 11 > 8 ✗ EXCEEDS TARGET
  → Backtrack
  
Back to Level 2: currentCombination = [2, 2]
  Try index 1 (value = 3):
  ↓

Level 3b: Added 3
  currentSum = 7, remaining = 1, startIdx = 1
  currentCombination = [2, 2, 3]
  
  Try index 1 (value = 3):
  currentSum = 7 + 3 = 10 > 8 ✗
  → Backtrack
  
Back to Level 2: currentCombination = [2, 2]
  Try index 2 (value = 5):
  currentSum = 4 + 5 = 9 > 8 ✗
  → Backtrack

Back to Level 1: currentCombination = [2]
  Try index 1 (value = 3):
  ↓

Level 2c: Added 3
  currentSum = 5, remaining = 3, startIdx = 1
  currentCombination = [2, 3]
  
  Try index 1 (value = 3) again:
  ↓

Level 3c: Added 3
  currentSum = 8, remaining = 0 ✓ TARGET REACHED!
  currentCombination = [2, 3, 3]
  → Add [2, 3, 3] to result
  → Backtrack

Back to Level 2c: currentCombination = [2, 3]
  Try index 2 (value = 5):
  currentSum = 5 + 5 = 10 > 8 ✗
  → Backtrack

Back to Level 1: currentCombination = [2]
  Try index 2 (value = 5):
  currentSum = 2 + 5 = 7, remaining = 1
  currentCombination = [2, 5]
  
  Try index 2 (value = 5):
  currentSum = 7 + 5 = 12 > 8 ✗
  → Backtrack

Back to Level 0: currentCombination = []
  Try index 1 (value = 3):
  ↓

Level 1d: Added 3
  currentSum = 3, remaining = 5, startIdx = 1
  currentCombination = [3]
  
  Try index 1 (value = 3):
  currentSum = 6, remaining = 2
  currentCombination = [3, 3]
  
  Try index 1 (value = 3):
  currentSum = 9 > 8 ✗
  → Backtrack
  
  Try index 2 (value = 5):
  currentSum = 6 + 5 = 11 > 8 ✗
  → Backtrack

Back to Level 1d: currentCombination = [3]
  Try index 2 (value = 5):
  ↓

Level 2d: Added 5
  currentSum = 8, remaining = 0 ✓ TARGET REACHED!
  currentCombination = [3, 5]
  → Add [3, 5] to result
  → Backtrack

Back to Level 0: currentCombination = []
  Try index 2 (value = 5):
  currentSum = 5, remaining = 3
  
  Try index 2 (value = 5):
  currentSum = 10 > 8 ✗
  → Backtrack

────────────────────────────────────────────────────────────────────────────
FINAL RESULT:
────────────────────────────────────────────────────────────────────────────
result = [[2,2,2,2], [2,3,3], [3,5]]

────────────────────────────────────────────────────────────────────────────
KEY OBSERVATIONS:
────────────────────────────────────────────────────────────────────────────
1. We NEVER go backwards in the array (no [3,2] after [2,3])
2. We CAN reuse same index (that's why we pass 'i' not 'i+1' in recursion)
3. Early termination when currentSum > target (pruning)
4. Backtracking removes last element to try different paths

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OPTIMAL SOLUTION - ALGORITHM SUMMARY
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APPROACH:
Use backtracking to explore all possible combinations. At each step, decide
whether to include the current candidate (can be reused unlimited times) or
move to the next candidate. Track the current sum and prune paths that exceed
the target.

KEY INSIGHTS:
• Backtracking explores all possible combinations systematically
• Can reuse same number unlimited times (pass same index in recursion)
• To avoid duplicates, only explore candidates from current index onwards
• Prune early when sum exceeds target (optimization)
• Base case: when sum equals target, add current combination to result

ALGORITHM STEPS:
1. Initialize result list and current combination list
2. Call backtracking helper function with:
   - candidates array
   - target sum
   - current combination (empty initially)
   - current sum (0 initially)
   - start index (0 initially)
   - result list
3. In backtracking function:
   a. Base case: if currentSum == target
      → Add copy of current combination to result
      → Return
   b. Pruning: if currentSum > target
      → Return (this path won't work)
   c. For each candidate from startIdx to end:
      → Add candidate to current combination
      → Recurse with same startIdx (allow reuse!)
      → Remove candidate (backtrack)
4. Return result list

TIME COMPLEXITY: O(N^(T/M))
- N = number of candidates
- T = target value
- M = minimum value in candidates
- In worst case, we might use minimum value T/M times
- At each position, we have N choices
- Height of recursion tree: T/M
- Branching factor: N
- Note: Actual complexity is better due to pruning

SPACE COMPLEXITY: O(T/M)
- Recursion depth: O(T/M) in worst case
- Current combination size: O(T/M)
- Result space not counted (output space)
- Total: O(T/M)

PATTERN: Backtracking, Recursive Exploration, Combinatorial Search
DIFFICULTY: Medium
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;
using System.Collections.Generic;

namespace DSA_CSharp.Backtracking
{
    public class CombinationSum
    {
        /// <summary>
        /// Finds all unique combinations of candidates that sum to target.
        /// Same number can be used unlimited times.
        /// </summary>
        /// <param name="candidates">Array of distinct positive integers</param>
        /// <param name="target">Target sum to achieve</param>
        /// <returns>List of all unique combinations that sum to target</returns>
        public IList<IList<int>> CombinationSumSolution(int[] candidates, int target)
        {
            // ═══════════════════════════════════════════════════════════════
            // INITIALIZE RESULT AND START BACKTRACKING
            // ═══════════════════════════════════════════════════════════════
            IList<IList<int>> result = new List<IList<int>>();
            List<int> currentCombination = new List<int>();
            
            // Start backtracking from index 0 with sum 0
            Backtrack(candidates, target, currentCombination, 0, 0, result);
            
            return result;
        }
        
        /// <summary>
        /// Backtracking helper function to explore all combinations.
        /// </summary>
        /// <param name="candidates">Array of candidate numbers</param>
        /// <param name="target">Target sum</param>
        /// <param name="currentCombination">Current combination being built</param>
        /// <param name="currentSum">Current sum of numbers in combination</param>
        /// <param name="startIdx">Index to start exploring from (avoids duplicates)</param>
        /// <param name="result">List to store all valid combinations</param>
        private void Backtrack(
            int[] candidates,
            int target,
            List<int> currentCombination,
            int currentSum,
            int startIdx,
            IList<IList<int>> result)
        {
            // ═══════════════════════════════════════════════════════════════
            // BASE CASE 1: Found a valid combination! ✓
            // ═══════════════════════════════════════════════════════════════
            if (currentSum == target)
            {
                // IMPORTANT: Add a COPY of current combination
                // If we add the reference, it will be modified later!
                result.Add(new List<int>(currentCombination));
                return;
            }
            
            // ═══════════════════════════════════════════════════════════════
            // BASE CASE 2: Sum exceeded target - prune this branch ✗
            // ═══════════════════════════════════════════════════════════════
            // This is an OPTIMIZATION - stops exploring paths that can't work
            if (currentSum > target)
            {
                return; // No point continuing, sum is already too large
            }
            
            // ═══════════════════════════════════════════════════════════════
            // RECURSIVE CASE: Try each candidate from startIdx onwards
            // ═══════════════════════════════════════════════════════════════
            // Why start from startIdx? To avoid duplicate combinations!
            // Example: We want [2,3] but not [3,2] (they're the same combination)
            
            for (int i = startIdx; i < candidates.Length; i++)
            {
                int candidate = candidates[i];
                
                // ───────────────────────────────────────────────────────────
                // OPTIMIZATION: Early pruning
                // ───────────────────────────────────────────────────────────
                // If adding this candidate exceeds target, skip it
                // (This check is technically redundant because the recursive
                // call will catch it, but it saves a function call)
                if (currentSum + candidate > target)
                {
                    continue; // Try next candidate
                }
                
                // ───────────────────────────────────────────────────────────
                // STEP 1: CHOOSE - Include this candidate
                // ───────────────────────────────────────────────────────────
                currentCombination.Add(candidate);
                
                // ───────────────────────────────────────────────────────────
                // STEP 2: EXPLORE - Recurse with this candidate included
                // ───────────────────────────────────────────────────────────
                // KEY INSIGHT: We pass 'i' (not 'i+1') as startIdx!
                // This allows REUSING the same candidate unlimited times
                // 
                // If we passed 'i+1', each candidate could only be used once
                // (that would be Combination Sum II problem!)
                
                Backtrack(
                    candidates,
                    target,
                    currentCombination,
                    currentSum + candidate, // Update sum
                    i,                       // Same index (allow reuse!)
                    result
                );
                
                // ───────────────────────────────────────────────────────────
                // STEP 3: UNCHOOSE - Remove this candidate (backtrack)
                // ───────────────────────────────────────────────────────────
                // This is the BACKTRACKING step!
                // We remove the candidate we just added so we can try
                // different combinations
                
                currentCombination.RemoveAt(currentCombination.Count - 1);
                
                // Now we'll try the next candidate in the next iteration
            }
        }
        
        // ═══════════════════════════════════════════════════════════════════
        // ALTERNATIVE SOLUTION: With Sorting (Better Pruning)
        // ═══════════════════════════════════════════════════════════════════
        // Sorting the candidates allows for better pruning because once we
        // encounter a candidate that makes sum exceed target, all subsequent
        // candidates will also exceed (since array is sorted)
        
        public IList<IList<int>> CombinationSumOptimized(int[] candidates, int target)
        {
            // Sort candidates for better pruning
            Array.Sort(candidates);
            
            IList<IList<int>> result = new List<IList<int>>();
            List<int> currentCombination = new List<int>();
            
            BacktrackOptimized(candidates, target, currentCombination, 0, 0, result);
            
            return result;
        }
        
        private void BacktrackOptimized(
            int[] candidates,
            int target,
            List<int> currentCombination,
            int currentSum,
            int startIdx,
            IList<IList<int>> result)
        {
            if (currentSum == target)
            {
                result.Add(new List<int>(currentCombination));
                return;
            }
            
            if (currentSum > target)
            {
                return;
            }
            
            for (int i = startIdx; i < candidates.Length; i++)
            {
                int candidate = candidates[i];
                
                // ───────────────────────────────────────────────────────────
                // ENHANCED PRUNING: Because array is sorted, if this candidate
                // makes sum exceed target, all future candidates will too!
                // So we can BREAK (not just continue)
                // ───────────────────────────────────────────────────────────
                if (currentSum + candidate > target)
                {
                    break; // No point checking remaining candidates
                }
                
                currentCombination.Add(candidate);
                BacktrackOptimized(candidates, target, currentCombination, 
                                   currentSum + candidate, i, result);
                currentCombination.RemoveAt(currentCombination.Count - 1);
            }
        }
        
        // Test method with detailed explanations
        public static void Test()
        {
            var solution = new CombinationSum();
            
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.WriteLine("COMBINATION SUM PROBLEM - DETAILED WALKTHROUGH");
            Console.WriteLine("═══════════════════════════════════════════════════════\n");
            
            // Test case 1
            int[] candidates1 = {2, 3, 6, 7};
            int target1 = 7;
            
            Console.WriteLine("Test Case 1:");
            Console.WriteLine($"Candidates: [{string.Join(", ", candidates1)}]");
            Console.WriteLine($"Target: {target1}\n");
            
            Console.WriteLine("Possible combinations:");
            Console.WriteLine("- 2 + 2 + 3 = 7 ✓");
            Console.WriteLine("- 7 = 7 ✓\n");
            
            var result1 = solution.CombinationSumSolution(candidates1, target1);
            Console.WriteLine("Result:");
            foreach (var combo in result1)
            {
                Console.WriteLine($"  [{string.Join(", ", combo)}]");
            }
            Console.WriteLine("Expected: [[2,2,3], [7]]\n");
            Console.WriteLine("───────────────────────────────────────────────────────\n");
            
            // Test case 2
            int[] candidates2 = {2, 3, 5};
            int target2 = 8;
            
            Console.WriteLine("Test Case 2:");
            Console.WriteLine($"Candidates: [{string.Join(", ", candidates2)}]");
            Console.WriteLine($"Target: {target2}\n");
            
            Console.WriteLine("Possible combinations:");
            Console.WriteLine("- 2 + 2 + 2 + 2 = 8 ✓");
            Console.WriteLine("- 2 + 3 + 3 = 8 ✓");
            Console.WriteLine("- 3 + 5 = 8 ✓\n");
            
            var result2 = solution.CombinationSumSolution(candidates2, target2);
            Console.WriteLine("Result:");
            foreach (var combo in result2)
            {
                Console.WriteLine($"  [{string.Join(", ", combo)}]");
            }
            Console.WriteLine("Expected: [[2,2,2,2], [2,3,3], [3,5]]\n");
            Console.WriteLine("───────────────────────────────────────────────────────\n");
            
            // Test case 3: No solution
            int[] candidates3 = {2};
            int target3 = 1;
            
            Console.WriteLine("Test Case 3 (No solution):");
            Console.WriteLine($"Candidates: [{string.Join(", ", candidates3)}]");
            Console.WriteLine($"Target: {target3}\n");
            Console.WriteLine("Analysis: Smallest candidate is 2, target is 1");
            Console.WriteLine("Impossible to reach target!\n");
            
            var result3 = solution.CombinationSumSolution(candidates3, target3);
            Console.WriteLine($"Result: {(result3.Count == 0 ? "[]" : "")}");
            Console.WriteLine("Expected: []\n");
            Console.WriteLine("───────────────────────────────────────────────────────\n");
            
            // Test case 4: Single element
            int[] candidates4 = {1};
            int target4 = 1;
            
            Console.WriteLine("Test Case 4 (Single element):");
            Console.WriteLine($"Candidates: [{string.Join(", ", candidates4)}]");
            Console.WriteLine($"Target: {target4}\n");
            
            var result4 = solution.CombinationSumSolution(candidates4, target4);
            Console.WriteLine("Result:");
            foreach (var combo in result4)
            {
                Console.WriteLine($"  [{string.Join(", ", combo)}]");
            }
            Console.WriteLine("Expected: [[1]]\n");
            Console.WriteLine("───────────────────────────────────────────────────────\n");
            
            // Test case 5: Reusing numbers
            int[] candidates5 = {1};
            int target5 = 2;
            
            Console.WriteLine("Test Case 5 (Demonstrating unlimited reuse):");
            Console.WriteLine($"Candidates: [{string.Join(", ", candidates5)}]");
            Console.WriteLine($"Target: {target5}\n");
            Console.WriteLine("We can use 1 twice: 1 + 1 = 2\n");
            
            var result5 = solution.CombinationSumSolution(candidates5, target5);
            Console.WriteLine("Result:");
            foreach (var combo in result5)
            {
                Console.WriteLine($"  [{string.Join(", ", combo)}]");
            }
            Console.WriteLine("Expected: [[1,1]]\n");
            Console.WriteLine("───────────────────────────────────────────────────────\n");
            
            // Compare optimized version
            Console.WriteLine("Testing Optimized Version (with sorting):");
            int[] candidates6 = {2, 3, 5};
            int target6 = 8;
            var result6 = solution.CombinationSumOptimized(candidates6, target6);
            Console.WriteLine($"Candidates: [{string.Join(", ", candidates6)}]");
            Console.WriteLine($"Target: {target6}");
            Console.WriteLine("Result:");
            foreach (var combo in result6)
            {
                Console.WriteLine($"  [{string.Join(", ", combo)}]");
            }
            Console.WriteLine("\nNote: Same result but potentially faster due to early break!");
        }
    }
}
