/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Palindrome Partitioning
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
Given a string s, partition s such that every substring of the partition is a 
palindrome. Return all possible palindrome partitioning of s.

A palindrome string is a string that reads the same backward as forward.

INPUT:
- s: string - The input string to partition
- 1 <= s.length <= 16
- s contains only lowercase English letters

OUTPUT:
- IList<IList<string>> - List of all possible palindrome partitionings

EXAMPLES:
Example 1:
Input: s = "aab"
Output: [["a","a","b"], ["aa","b"]]
Explanation:
- Partition 1: "a" | "a" | "b" → All are palindromes ✓
- Partition 2: "aa" | "b" → Both are palindromes ✓
- "aab" is NOT a palindrome, so no single partition works

Example 2:
Input: s = "a"
Output: [["a"]]
Explanation: Single character is always a palindrome

Example 3:
Input: s = "racecar"
Output: [["r","a","c","e","c","a","r"], ["r","aceca","r"], ["racecar"]]
Explanation:
- Can split into 7 single characters (all palindromes)
- Can split as "r" | "aceca" | "r" (all palindromes)
- Entire string is a palindrome

Example 4:
Input: s = "aabb"
Output: [["a","a","b","b"], ["a","a","bb"], ["aa","b","b"], ["aa","bb"]]

CONSTRAINTS:
- 1 <= s.length <= 16
- s contains only lowercase English letters

EDGE CASES:
✓ Single character → always palindrome
✓ All same characters → many partitioning options
✓ Entire string is palindrome → include as one option
✓ No palindromes longer than 1 char → split into all single chars
✓ Multiple valid partitionings

TRICK CASES:
⚡ Must check EVERY possible partition point
⚡ A substring is palindrome if it reads same forwards/backwards
⚡ Use backtracking to explore all partition possibilities
⚡ At each position, try all substrings starting from that position
⚡ Only recurse if current substring is a palindrome (pruning!)
⚡ Can optimize palindrome check with DP or two-pointer

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
SIMPLE EXPLANATION (KEY TO UNDERSTANDING!)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

IMAGINE CUTTING A STRING WITH SCISSORS:
You have string: "aab"
You need to make cuts so each piece reads the same forwards/backwards

WHERE CAN YOU CUT?

Option 1: Cut after each character
  "a" | "a" | "b"
  Check: "a" palindrome? ✓  "a" palindrome? ✓  "b" palindrome? ✓
  Valid partition!

Option 2: Cut after second character
  "aa" | "b"
  Check: "aa" palindrome? ✓  "b" palindrome? ✓
  Valid partition!

Option 3: No cuts
  "aab"
  Check: "aab" palindrome? ✗ (reads "baa" backwards)
  Invalid!

THE BACKTRACKING APPROACH:
At each position in the string, we try ALL possible substrings from that 
position and check if they're palindromes.

Think of it as a decision tree:

                        "aab" (start)
                       /      |      \
                "a"|"ab"  "aa"|"b"  "aab"
                /   \        |        ✗
         "a"|"a"|"b" ✗      ✓      (not palindrome)
              ✓

KEY INSIGHTS:
1. At each position, try substrings of increasing length
2. Only continue if current substring is a palindrome (pruning!)
3. When we reach end of string, we found a valid partition
4. Backtrack to explore other possibilities

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
DETAILED SOLUTION WALKTHROUGH - HOW THE CODE WORKS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Let's trace through Example 1: s = "aab"

────────────────────────────────────────────────────────────────────────────
INITIAL STATE:
────────────────────────────────────────────────────────────────────────────
s = "aab"
result = []
currentPartition = []

────────────────────────────────────────────────────────────────────────────
BACKTRACKING TREE EXPLORATION:
────────────────────────────────────────────────────────────────────────────

Level 0: Start at index 0
  currentPartition = []
  Need to partition from index 0 to end (index 3)
  
  Try substrings starting at index 0:
  
  Option A: substring "a" (index 0 to 0)
  ↓

Level 1: Added "a"
  Is "a" a palindrome? YES ✓
  currentPartition = ["a"]
  Now need to partition from index 1 to end
  
  Try substrings starting at index 1:
  
  Option A1: substring "a" (index 1 to 1)
  ↓

Level 2: Added "a"
  Is "a" a palindrome? YES ✓
  currentPartition = ["a", "a"]
  Now need to partition from index 2 to end
  
  Try substrings starting at index 2:
  
  Option A1a: substring "b" (index 2 to 2)
  ↓

Level 3: Added "b"
  Is "b" a palindrome? YES ✓
  currentPartition = ["a", "a", "b"]
  Reached end of string! ✓ FOUND VALID PARTITION
  
  → Add ["a", "a", "b"] to result
  → Backtrack (remove "b")

Back to Level 2: currentPartition = ["a", "a"]
  No more substrings to try from index 2
  → Backtrack (remove second "a")

Back to Level 1: currentPartition = ["a"]
  Try next substring starting at index 1:
  
  Option A2: substring "ab" (index 1 to 2)
  ↓

Level 2b: Check "ab"
  Is "ab" a palindrome? NO ✗
  (reads "ba" backwards)
  → Don't add to partition, backtrack immediately

Back to Level 1: currentPartition = ["a"]
  No more substrings to try from index 1
  → Backtrack (remove first "a")

Back to Level 0: currentPartition = []
  Try next substring starting at index 0:
  
  Option B: substring "aa" (index 0 to 1)
  ↓

Level 1c: Added "aa"
  Is "aa" a palindrome? YES ✓
  currentPartition = ["aa"]
  Now need to partition from index 2 to end
  
  Try substrings starting at index 2:
  
  Option B1: substring "b" (index 2 to 2)
  ↓

Level 2c: Added "b"
  Is "b" a palindrome? YES ✓
  currentPartition = ["aa", "b"]
  Reached end of string! ✓ FOUND VALID PARTITION
  
  → Add ["aa", "b"] to result
  → Backtrack (remove "b")

Back to Level 1c: currentPartition = ["aa"]
  No more substrings to try from index 2
  → Backtrack (remove "aa")

Back to Level 0: currentPartition = []
  Try next substring starting at index 0:
  
  Option C: substring "aab" (index 0 to 2)
  ↓

Level 1d: Check "aab"
  Is "aab" a palindrome? NO ✗
  (reads "baa" backwards)
  → Don't add to partition, backtrack immediately

Back to Level 0: No more substrings to try
  DONE!

────────────────────────────────────────────────────────────────────────────
FINAL RESULT:
────────────────────────────────────────────────────────────────────────────
result = [["a","a","b"], ["aa","b"]]

────────────────────────────────────────────────────────────────────────────
KEY OBSERVATIONS:
────────────────────────────────────────────────────────────────────────────
1. We try ALL possible substrings at each position
2. We ONLY recurse if substring is a palindrome (pruning!)
3. When startIdx reaches end of string, we found a complete partition
4. Backtracking explores all valid combinations

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PALINDROME CHECK VISUALIZATION:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

How do we check if a string is a palindrome?
Use two pointers: one from start, one from end

Example: "racecar"
  r a c e c a r
  ↑           ↑  Compare: r == r ✓
  
  r a c e c a r
    ↑       ↑    Compare: a == a ✓
  
  r a c e c a r
      ↑   ↑      Compare: c == c ✓
  
  r a c e c a r
        ↑        Middle reached, it's a palindrome!

Example: "aab"
  a a b
  ↑   ↑  Compare: a != b ✗  Not a palindrome!

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OPTIMAL SOLUTION - ALGORITHM SUMMARY
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APPROACH:
Use backtracking to explore all possible ways to partition the string.
At each position, try all substrings and only recurse if the substring
is a palindrome.

KEY INSIGHTS:
• Backtracking explores all partition combinations systematically
• At each index, try substrings of increasing length
• Only continue if current substring is palindrome (pruning)
• Base case: when we've processed entire string, add partition to result
• Use two-pointer technique to check palindromes efficiently

ALGORITHM STEPS:
1. Initialize result list and current partition list
2. Call backtracking helper starting at index 0
3. In backtracking function:
   a. Base case: if start index == string length
      → Reached end, add copy of current partition to result
      → Return
   b. For each possible end index from start to string end:
      → Extract substring from start to end
      → Check if substring is palindrome
      → If YES:
        - Add substring to current partition
        - Recurse with next start index
        - Remove substring (backtrack)
      → If NO: skip this substring
4. Return result list

PALINDROME CHECK:
Use two pointers (left and right):
- Start left at beginning, right at end
- Compare characters at left and right
- If different, not palindrome
- Move pointers toward center
- If pointers meet/cross, it's a palindrome

TIME COMPLEXITY: O(N × 2^N)
- N = length of string
- In worst case (all chars same), we have 2^N partitions
- For each partition, we do O(N) work (checking palindromes, copying)
- Total: O(N × 2^N)

SPACE COMPLEXITY: O(N)
- Recursion depth: O(N) in worst case
- Current partition: O(N) strings
- Result space not counted (output space)
- Total: O(N)

OPTIMIZATION:
Can pre-compute palindrome status using DP in O(N²) time and space,
then lookup becomes O(1). Trade-off: extra space for faster checks.

PATTERN: Backtracking, String Partitioning, Palindrome Detection
DIFFICULTY: Medium
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;
using System.Collections.Generic;

namespace DSA_CSharp.Backtracking
{
    public class PalindromePartitioning
    {
        /// <summary>
        /// Finds all possible palindrome partitionings of a string.
        /// Uses backtracking to explore all partition combinations.
        /// </summary>
        /// <param name="s">Input string to partition</param>
        /// <returns>List of all valid palindrome partitionings</returns>
        public IList<IList<string>> Partition(string s)
        {
            // ═══════════════════════════════════════════════════════════════
            // INITIALIZE RESULT AND START BACKTRACKING
            // ═══════════════════════════════════════════════════════════════
            IList<IList<string>> result = new List<IList<string>>();
            List<string> currentPartition = new List<string>();
            
            // Start backtracking from index 0
            Backtrack(s, 0, currentPartition, result);
            
            return result;
        }
        
        /// <summary>
        /// Backtracking helper to explore all partition possibilities.
        /// </summary>
        /// <param name="s">Input string</param>
        /// <param name="startIdx">Current starting index to partition from</param>
        /// <param name="currentPartition">Current partition being built</param>
        /// <param name="result">List to store all valid partitions</param>
        private void Backtrack(
            string s,
            int startIdx,
            List<string> currentPartition,
            IList<IList<string>> result)
        {
            // ═══════════════════════════════════════════════════════════════
            // BASE CASE: Reached end of string - found valid partition! ✓
            // ═══════════════════════════════════════════════════════════════
            if (startIdx == s.Length)
            {
                // We've successfully partitioned the entire string
                // All substrings in currentPartition are palindromes
                // IMPORTANT: Add a COPY (not reference) to result
                result.Add(new List<string>(currentPartition));
                return;
            }
            
            // ═══════════════════════════════════════════════════════════════
            // RECURSIVE CASE: Try all possible substrings from current index
            // ═══════════════════════════════════════════════════════════════
            // We'll try substrings of length 1, 2, 3, ... up to end of string
            
            for (int endIdx = startIdx; endIdx < s.Length; endIdx++)
            {
                // ───────────────────────────────────────────────────────────
                // Extract substring from startIdx to endIdx (inclusive)
                // ───────────────────────────────────────────────────────────
                // Length = endIdx - startIdx + 1
                int length = endIdx - startIdx + 1;
                string substring = s.Substring(startIdx, length);
                
                // ───────────────────────────────────────────────────────────
                // CHECK: Is this substring a palindrome?
                // ───────────────────────────────────────────────────────────
                // This is the KEY check - only proceed if palindrome!
                
                if (IsPalindrome(substring))
                {
                    // ───────────────────────────────────────────────────────
                    // STEP 1: CHOOSE - Include this palindrome substring
                    // ───────────────────────────────────────────────────────
                    currentPartition.Add(substring);
                    
                    // ───────────────────────────────────────────────────────
                    // STEP 2: EXPLORE - Recurse to partition remaining string
                    // ───────────────────────────────────────────────────────
                    // Next partition starts at endIdx + 1
                    Backtrack(s, endIdx + 1, currentPartition, result);
                    
                    // ───────────────────────────────────────────────────────
                    // STEP 3: UNCHOOSE - Remove substring (backtrack)
                    // ───────────────────────────────────────────────────────
                    currentPartition.RemoveAt(currentPartition.Count - 1);
                }
                // If not palindrome, we simply skip this substring and
                // try the next longer one in the next iteration
            }
        }
        
        /// <summary>
        /// Checks if a string is a palindrome using two-pointer technique.
        /// </summary>
        /// <param name="s">String to check</param>
        /// <returns>True if palindrome, false otherwise</returns>
        private bool IsPalindrome(string s)
        {
            // ═══════════════════════════════════════════════════════════════
            // TWO-POINTER TECHNIQUE FOR PALINDROME CHECK
            // ═══════════════════════════════════════════════════════════════
            int left = 0;
            int right = s.Length - 1;
            
            while (left < right)
            {
                // Compare characters from both ends
                if (s[left] != s[right])
                {
                    return false; // Mismatch found, not a palindrome
                }
                
                // Move pointers toward center
                left++;
                right--;
            }
            
            // Pointers met/crossed, all characters matched!
            return true;
        }
        
        // ═══════════════════════════════════════════════════════════════════
        // OPTIMIZED SOLUTION: Pre-compute Palindromes with Dynamic Programming
        // ═══════════════════════════════════════════════════════════════════
        // This optimization pre-computes which substrings are palindromes
        // Trade-off: O(N²) extra space for O(1) palindrome lookups
        
        public IList<IList<string>> PartitionOptimized(string s)
        {
            int n = s.Length;
            
            // ───────────────────────────────────────────────────────────────
            // Pre-compute palindrome status using DP
            // ───────────────────────────────────────────────────────────────
            // isPalindrome[i][j] = true if s[i...j] is a palindrome
            
            bool[,] isPalindrome = new bool[n, n];
            
            // Every single character is a palindrome
            for (int i = 0; i < n; i++)
            {
                isPalindrome[i, i] = true;
            }
            
            // Check substrings of length 2
            for (int i = 0; i < n - 1; i++)
            {
                if (s[i] == s[i + 1])
                {
                    isPalindrome[i, i + 1] = true;
                }
            }
            
            // Check substrings of length 3 and above
            // A substring s[i...j] is palindrome if:
            // 1. s[i] == s[j] AND
            // 2. s[i+1...j-1] is palindrome
            for (int length = 3; length <= n; length++)
            {
                for (int i = 0; i <= n - length; i++)
                {
                    int j = i + length - 1;
                    if (s[i] == s[j] && isPalindrome[i + 1, j - 1])
                    {
                        isPalindrome[i, j] = true;
                    }
                }
            }
            
            // Now use backtracking with O(1) palindrome lookups
            IList<IList<string>> result = new List<IList<string>>();
            List<string> currentPartition = new List<string>();
            BacktrackOptimized(s, 0, currentPartition, result, isPalindrome);
            
            return result;
        }
        
        private void BacktrackOptimized(
            string s,
            int startIdx,
            List<string> currentPartition,
            IList<IList<string>> result,
            bool[,] isPalindrome)
        {
            if (startIdx == s.Length)
            {
                result.Add(new List<string>(currentPartition));
                return;
            }
            
            for (int endIdx = startIdx; endIdx < s.Length; endIdx++)
            {
                // O(1) palindrome check using pre-computed table!
                if (isPalindrome[startIdx, endIdx])
                {
                    int length = endIdx - startIdx + 1;
                    string substring = s.Substring(startIdx, length);
                    
                    currentPartition.Add(substring);
                    BacktrackOptimized(s, endIdx + 1, currentPartition, result, isPalindrome);
                    currentPartition.RemoveAt(currentPartition.Count - 1);
                }
            }
        }
        
        // Test method with detailed explanations
        public static void Test()
        {
            var solution = new PalindromePartitioning();
            
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.WriteLine("PALINDROME PARTITIONING - DETAILED WALKTHROUGH");
            Console.WriteLine("═══════════════════════════════════════════════════════\n");
            
            // Test case 1
            string s1 = "aab";
            Console.WriteLine("Test Case 1:");
            Console.WriteLine($"Input: s = \"{s1}\"\n");
            
            Console.WriteLine("Possible partitions:");
            Console.WriteLine("1. \"a\" | \"a\" | \"b\"");
            Console.WriteLine("   - \"a\" is palindrome? YES ✓");
            Console.WriteLine("   - \"a\" is palindrome? YES ✓");
            Console.WriteLine("   - \"b\" is palindrome? YES ✓");
            Console.WriteLine("   Valid partition!\n");
            
            Console.WriteLine("2. \"aa\" | \"b\"");
            Console.WriteLine("   - \"aa\" is palindrome? YES ✓");
            Console.WriteLine("   - \"b\" is palindrome? YES ✓");
            Console.WriteLine("   Valid partition!\n");
            
            Console.WriteLine("3. \"aab\" (no partition)");
            Console.WriteLine("   - \"aab\" is palindrome? NO ✗");
            Console.WriteLine("   - Reads \"baa\" backwards");
            Console.WriteLine("   Invalid!\n");
            
            var result1 = solution.Partition(s1);
            Console.WriteLine("Result:");
            foreach (var partition in result1)
            {
                Console.WriteLine($"  [{string.Join("\", \"", partition).Insert(0, "\"")}]");
            }
            Console.WriteLine("Expected: [[\"a\",\"a\",\"b\"], [\"aa\",\"b\"]]\n");
            Console.WriteLine("───────────────────────────────────────────────────────\n");
            
            // Test case 2
            string s2 = "a";
            Console.WriteLine("Test Case 2 (Single character):");
            Console.WriteLine($"Input: s = \"{s2}\"");
            Console.WriteLine("Single character is always a palindrome\n");
            
            var result2 = solution.Partition(s2);
            Console.WriteLine("Result:");
            foreach (var partition in result2)
            {
                Console.WriteLine($"  [{string.Join("\", \"", partition).Insert(0, "\"")}]");
            }
            Console.WriteLine("Expected: [[\"a\"]]\n");
            Console.WriteLine("───────────────────────────────────────────────────────\n");
            
            // Test case 3
            string s3 = "aba";
            Console.WriteLine("Test Case 3 (Entire string is palindrome):");
            Console.WriteLine($"Input: s = \"{s3}\"\n");
            
            Console.WriteLine("Possible partitions:");
            Console.WriteLine("1. \"a\" | \"b\" | \"a\" (split all)");
            Console.WriteLine("2. \"aba\" (entire string - it's a palindrome!)\n");
            
            var result3 = solution.Partition(s3);
            Console.WriteLine("Result:");
            foreach (var partition in result3)
            {
                Console.WriteLine($"  [{string.Join("\", \"", partition).Insert(0, "\"")}]");
            }
            Console.WriteLine("Expected: [[\"a\",\"b\",\"a\"], [\"aba\"]]\n");
            Console.WriteLine("───────────────────────────────────────────────────────\n");
            
            // Test case 4
            string s4 = "aabb";
            Console.WriteLine("Test Case 4 (Multiple partitions):");
            Console.WriteLine($"Input: s = \"{s4}\"\n");
            
            var result4 = solution.Partition(s4);
            Console.WriteLine("Result:");
            foreach (var partition in result4)
            {
                Console.WriteLine($"  [{string.Join("\", \"", partition).Insert(0, "\"")}]");
            }
            Console.WriteLine("\nNote: Multiple valid ways to partition!\n");
            Console.WriteLine("───────────────────────────────────────────────────────\n");
            
            // Test case 5 - Compare optimized version
            string s5 = "aab";
            Console.WriteLine("Test Case 5 (Optimized with DP):");
            Console.WriteLine($"Input: s = \"{s5}\"");
            
            var result5 = solution.PartitionOptimized(s5);
            Console.WriteLine("Result:");
            foreach (var partition in result5)
            {
                Console.WriteLine($"  [{string.Join("\", \"", partition).Insert(0, "\"")}]");
            }
            Console.WriteLine("\nNote: Same result but with O(1) palindrome checks!");
            Console.WriteLine("DP pre-computation trades space for speed.\n");
            
            // Demonstrate palindrome checking
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.WriteLine("PALINDROME CHECK DEMONSTRATION");
            Console.WriteLine("═══════════════════════════════════════════════════════\n");
            
            string[] testStrings = { "a", "aa", "aba", "aab", "racecar", "hello" };
            foreach (var testStr in testStrings)
            {
                bool isPalin = solution.IsPalindrome(testStr);
                Console.WriteLine($"\"{testStr}\" is palindrome? {(isPalin ? "YES ✓" : "NO ✗")}");
            }
        }
    }
}
