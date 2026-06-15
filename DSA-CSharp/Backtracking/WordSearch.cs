/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Word Search
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
Given an m x n grid of characters board and a string word, return true if word 
exists in the grid.

The word can be constructed from letters of sequentially adjacent cells, where 
adjacent cells are horizontally or vertically neighboring. The same letter cell 
may not be used more than once.

INPUT:
- board: char[][] - m x n grid of uppercase English letters
- word: string - Word to search for (uppercase English letters)
- m, n: 1 <= m, n <= 6
- 1 <= word.length <= 15
- board[i][j] and word consist of uppercase English letters

OUTPUT:
- bool - True if word exists in the grid, false otherwise

EXAMPLES:
Example 1:
Input: board = [
    ['B', 'L', 'C', 'H'],
    ['D', 'E', 'L', 'T'],
    ['D', 'A', 'K', 'A']
], word = "BLEAK"
Output: True
Explanation: Path B[0,0] → L[0,1] → E[1,1] → A[2,1] → K[2,2]

Example 2:
Input: board = [
    ['B', 'L', 'C', 'H'],
    ['D', 'E', 'L', 'T'],
    ['D', 'A', 'K', 'A']
], word = "BLEED"
Output: False
Explanation: Cannot form "BLEED" following adjacent cells rules

Example 3:
Input: board = [
    ['A', 'B', 'C', 'E'],
    ['S', 'F', 'C', 'S'],
    ['A', 'D', 'E', 'E']
], word = "ABCCED"
Output: True

Example 4:
Input: board = [
    ['A', 'B'],
    ['C', 'D']
], word = "ABCD"
Output: False
Explanation: Path would need to reuse cells

CONSTRAINTS:
- m == board.length
- n == board[i].length
- 1 <= m, n <= 6
- 1 <= word.length <= 15
- board and word consist of only uppercase English letters

EDGE CASES:
✓ Single cell board
✓ Word longer than total cells in board
✓ Word same length as total cells (use entire board)
✓ Multiple starting positions possible
✓ Word with repeated letters (can't reuse same cell)
✓ Word not in board at all
✓ Word is single character

TRICK CASES:
⚡ Same letter can appear multiple times in board (different cells)
⚡ But same CELL cannot be used twice in same path
⚡ Need to mark cells as visited during search, unmark on backtrack
⚡ Must try all possible starting positions
⚡ Early termination if word too long for board
⚡ Backtracking is essential - explore all paths

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
VISUAL WALKTHROUGH - THE temp VARIABLE EXPLAINED
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Let's trace through searching for "ABC" in this board:
    A B
    C D

────────────────────────────────────────────────────────────────────────────
WHY WE NEED THE temp VARIABLE:
────────────────────────────────────────────────────────────────────────────
Problem: Can't use same CELL twice in ONE path
Solution: Mark it visited, then RESTORE it for other paths

────────────────────────────────────────────────────────────────────────────
WHY WE MARK CELLS AS '#' - THE CORE REASON:
────────────────────────────────────────────────────────────────────────────

📌 PROBLEM RULE: "Same letter cell may not be used more than once"

This means:
  ✓ Can't visit same cell TWICE in ONE path
  ✓ But DIFFERENT paths CAN visit same cell

Example WITHOUT marking (WRONG):
  Board: A B    Word: "ABA"
         C D
  
  Path: A[0,0] → B[0,1] → A[0,0]  ❌ Used A twice!
  
  Without marking, we have NO WAY to know we already used A[0,0]!
  The recursive function would just find A again and think it's valid!

Example WITH marking (CORRECT):
  Board: A B    Word: "ABA"
         C D
  
  Start at A[0,0]:
    Mark A → # B    "I'm using A, don't come back!"
             C D
    
    Go to B[0,1]:
      Mark B → # #  "Now using B too!"
               C D
      
      Looking for A:
        Try LEFT → See '#' → Not 'A' → FAIL! ✓
        
        Marking PREVENTS us from reusing A[0,0]!

🔑 THE KEY INSIGHT:
   Marking = Temporary "DO NOT ENTER" sign
   Restoring = Removing the sign when we're done
   
   This lets us:
   1. Prevent cycles in ONE path (no infinite loops!)
   2. Allow OTHER paths to use the same cells

────────────────────────────────────────────────────────────────────────────
STEP-BY-STEP EXECUTION:
────────────────────────────────────────────────────────────────────────────

Starting board:
  A B
  C D

1. Try starting at A[0,0] (first character matches!)
   ├─ temp = 'A'              ← SAVE original value
   ├─ board[0][0] = '#'       ← MARK as visited
   │
   │  Board now:
   │    # B
   │    C D
   │
   ├─ Try direction RIGHT: B[0,1] (looking for 'B')
   │  ├─ temp = 'B'           ← SAVE 'B'
   │  ├─ board[0][1] = '#'    ← MARK as visited
   │  │
   │  │  Board now:
   │  │    # #
   │  │    C D
   │  │
   │  ├─ Try direction DOWN: D[1,1] (looking for 'C')
   │  │  └─ 'D' != 'C' → FAIL
   │  │
   │  ├─ Try direction LEFT: A[0,0] (looking for 'C')
   │  │  └─ '#' != 'C' → FAIL (can't reuse A!)
   │  │
   │  ├─ All directions failed!
   │  ├─ board[0][1] = temp   ← RESTORE 'B' (backtrack!)
   │  │
   │  │  Board now:
   │  │    # B
   │  │    C D
   │  │
   │  └─ Return false
   │
   ├─ Try direction DOWN: C[1,0] (looking for 'B')
   │  └─ 'C' != 'B' → FAIL
   │
   ├─ All directions failed from A!
   ├─ board[0][0] = temp      ← RESTORE 'A' (backtrack!)
   │
   │  Board now:
   │    A B
   │    C D
   │
   └─ Return false

2. Try starting at B[0,1]
   └─ 'B' != 'A' → Skip (first character doesn't match)

3. Try starting at C[1,0]
   └─ 'C' != 'A' → Skip

4. Try starting at D[1,1]
   └─ 'D' != 'A' → Skip

Result: Word "ABC" not found (return false)

────────────────────────────────────────────────────────────────────────────
THE CRITICAL INSIGHT:
────────────────────────────────────────────────────────────────────────────

WITHOUT RESTORE (wrong!):
  After failing at A, board stays: # B
                                   C D
  Can't try other paths starting at A because it's marked!

WITH RESTORE (correct!):
  After failing at A, board restored: A B
                                      C D
  Other starting positions can still work!
  Board is clean for next attempt!

THE temp VARIABLE IS THE "UNDO BUTTON" FOR BACKTRACKING!

────────────────────────────────────────────────────────────────────────────
SIMPLE ANALOGY - WALKING THROUGH A MAZE:
────────────────────────────────────────────────────────────────────────────

Think of it like leaving breadcrumbs in a maze:

1. DROP BREADCRUMB (mark as '#')
   "I've been here, don't come back in THIS path"

2. EXPLORE all exits
   Try each direction to find the treasure

3. PICK UP BREADCRUMB (restore to original)
   "Done exploring from here, remove my mark"
   "So FUTURE paths can use this spot"

Without picking up breadcrumbs (no restore):
  → Future explorers see breadcrumbs everywhere
  → Can't find paths because spots seem "taken"
  → WRONG!

With picking up breadcrumbs (with restore):
  → Each new path starts fresh
  → Can explore all possibilities
  → CORRECT!

temp variable = Your pocket where you keep the original tile letter
                before replacing it with a breadcrumb ('#')

────────────────────────────────────────────────────────────────────────────
DETAILED CODE EXECUTION - WHERE RESTORATION HAPPENS:
────────────────────────────────────────────────────────────────────────────

Let's trace EXACTLY when restore happens with code line numbers:

Board: A B    Word: "CD"
       C D

Call: DFS(board, "CD", 0, 0, 0)  // Try starting at A[0,0]

Line-by-line execution:
  ✓ index == 2? No (index=0)
  ✓ Out of bounds? No
  ✓ board[0][0] == 'C'? No, it's 'A' 
  → Return false immediately, no marking happened

Call: DFS(board, "CD", 0, 1, 0)  // Try starting at B[0,1]

  ✓ index == 2? No
  ✓ Out of bounds? No  
  ✓ board[0][1] == 'C'? No, it's 'B'
  → Return false immediately

Call: DFS(board, "CD", 1, 0, 0)  // Try starting at C[1,0]

  ✓ index == 2? No
  ✓ Out of bounds? No
  ✓ board[1][0] == 'C'? YES! ✓
  
  → char temp = 'C'              // Save original
  → board[1][0] = '#'            // Mark as visited
  
  Board now: A B
             # D
  
  → foreach (4 directions):
      
      Try RIGHT: D[1,1]
        Recursive call: DFS(board, "CD", 1, 1, 1)
          ✓ index == 2? No (index=1)
          ✓ Out of bounds? No
          ✓ board[1][1] == 'D'? YES! ✓
          
          → char temp = 'D'      // Save 'D'
          → board[1][1] = '#'    // Mark
          
          Board now: A B
                     # #
          
          → foreach (4 directions):
              Try UP, DOWN, LEFT, RIGHT...
              All fail (out of bounds or hit '#')
          
          → ALL DIRECTIONS FAILED!
          → board[1][1] = temp   // ← RESTORE 'D' HERE!
          → return false
          
          Board now: A B
                     # D          // D is restored!
      
      → DFS returned false, continue to next direction
      
      Try UP, LEFT, DOWN: All fail
  
  → ALL DIRECTIONS FAILED!
  → board[1][0] = temp             // ← RESTORE 'C' HERE!
  → return false
  
  Board now: A B
             C D                   // C is restored!

Result: "CD" not found (but board is clean!)

────────────────────────────────────────────────────────────────────────────
THE ANSWER TO YOUR QUESTION:
────────────────────────────────────────────────────────────────────────────

Q: "When path returns false, will it restore? Where does it happen?"

A: YES! Restoration happens BEFORE returning false!
   
   Location: RIGHT AFTER the foreach loop, BEFORE return false
   
   Code:
   ```
   foreach (var dir in directions) {
       // Try all 4 directions...
   }
   
   // ← If we reach here, all directions failed
   board[row][col] = temp;  // ← RESTORE HAPPENS HERE!
   return false;             // ← Then return false
   ```

Every DFS call follows this pattern:
  1. Mark cell as '#'
  2. Try all 4 directions
  3. **ALWAYS restore** (success OR failure)
  4. Return result

NO DFS call returns without restoring first!

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OPTIMAL SOLUTION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APPROACH:
Use BACKTRACKING with DFS. Try each cell as starting position, then recursively
explore adjacent cells to build the word. Mark cells as visited during search
and unmark when backtracking.

This is a classic backtracking problem - explore paths, backtrack on failure.

KEY INSIGHTS:
• Try every cell as potential starting position
• Use DFS to explore paths letter by letter
• Mark cells as visited to prevent reuse in same path
• Backtrack: unmark cells when returning from recursion
• Early termination when word character doesn't match

ALGORITHM STEPS:
1. Edge case: if word empty, return true
2. Iterate through all cells in board:
   - Try each cell as starting position
   - If first character matches, start DFS
3. DFS function (recursive):
   - Base case: if we've matched entire word, return true
   - If out of bounds or cell doesn't match current character, return false
   - If cell already visited in this path, return false
   - Mark current cell as visited (use temporary marker)
   - Recursively try all 4 directions (up, down, left, right)
   - If any direction succeeds, return true
   - Unmark cell as visited (backtrack)
   - Return false
4. If any starting position succeeds, return true
5. Otherwise, return false

TIME COMPLEXITY: O(m × n × 4^L)
- m × n: Try each cell as starting point
- 4^L: For each starting point, explore up to 4 directions at each of L steps
- L = word length
- In practice, much faster due to pruning

SPACE COMPLEXITY: O(L)
- Recursion stack depth = word length
- O(L) for recursive calls
- Note: We modify board in-place for visited tracking

PATTERN: Backtracking, DFS, Grid Traversal
DIFFICULTY: Medium
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;

namespace DSA_CSharp.Backtracking
{
    public class WordSearch
    {
        private int rows;
        private int cols;
        private readonly int[][] directions = new int[][]
        {
            new int[] {0, 1},   // Right
            new int[] {1, 0},   // Down
            new int[] {0, -1},  // Left
            new int[] {-1, 0}   // Up
        };
        
        /// <summary>
        /// Determines if word exists in the board following adjacency rules.
        /// Uses backtracking with DFS to explore all possible paths.
        /// </summary>
        /// <param name="board">2D grid of characters</param>
        /// <param name="word">Word to search for</param>
        /// <returns>True if word exists, false otherwise</returns>
        public bool Exist(char[][] board, string word)
        {
            if (board == null || board.Length == 0 || word == null || word.Length == 0)
                return false;
            
            rows = board.Length;
            cols = board[0].Length;
            
            // Early termination: word too long
            if (word.Length > rows * cols)
                return false;
            
            // Try each cell as starting position
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    // If first character matches, start DFS
                    if (board[row][col] == word[0])
                    {
                        if (DFS(board, word, row, col, 0))
                            return true;
                    }
                }
            }
            
            return false;
        }
        
        /// <summary>
        /// DFS with backtracking to find word starting from current position.
        /// </summary>
        /// <param name="board">The grid</param>
        /// <param name="word">Target word</param>
        /// <param name="row">Current row</param>
        /// <param name="col">Current column</param>
        /// <param name="index">Current index in word we're trying to match</param>
        /// <returns>True if word can be formed from this position</returns>
        private bool DFS(char[][] board, string word, int row, int col, int index)
        {
            // ═══════════════════════════════════════════════════════════════
            // BASE CASE: Successfully matched entire word!
            // ═══════════════════════════════════════════════════════════════
            if (index == word.Length)
                return true;
            
            // ═══════════════════════════════════════════════════════════════
            // BOUNDARY CHECKS
            // ═══════════════════════════════════════════════════════════════
            if (row < 0 || row >= rows || col < 0 || col >= cols)
                return false;
            
            // ═══════════════════════════════════════════════════════════════
            // CHARACTER MATCH CHECK
            // ═══════════════════════════════════════════════════════════════
            if (board[row][col] != word[index])
                return false;
            
            // ═══════════════════════════════════════════════════════════════
            // WHY DO WE MARK CELLS AS '#'? THE FUNDAMENTAL PROBLEM
            // ═══════════════════════════════════════════════════════════════
            //
            // PROBLEM STATEMENT RULE: "Same cell may not be used more than once"
            //
            // What does this mean?
            // → In ONE path, you cannot visit the same cell twice
            // → But DIFFERENT paths can visit the same cell
            //
            // ───────────────────────────────────────────────────────────────
            // EXAMPLE: What happens WITHOUT marking?
            // ───────────────────────────────────────────────────────────────
            //
            // Board: A B    Word: "ABA"
            //        C D
            //
            // WITHOUT marking (#):
            //   Start at A[0,0]
            //   → Looking for 'A' ✓ Found at A[0,0]
            //   → Looking for 'B' → Try RIGHT: B[0,1] ✓
            //   → Looking for 'A' → Try LEFT: A[0,0] ✓ FOUND!
            //   → Path: A[0,0] → B[0,1] → A[0,0]
            //   
            //   ❌ WRONG! We used A[0,0] TWICE in the SAME path!
            //   This violates the rule!
            //
            // WITH marking (#):
            //   Start at A[0,0]
            //   → Mark A[0,0] as '#' (visited in THIS path)
            //   → Board: # B
            //            C D
            //   → Looking for 'B' → Try RIGHT: B[0,1] ✓
            //   → Mark B[0,1] as '#'
            //   → Board: # #
            //            C D
            //   → Looking for 'A' → Try LEFT: '#' ✗ Doesn't match!
            //   → Can't reuse A[0,0] because it's marked!
            //   
            //   ✓ CORRECT! Marking prevents reusing the same cell!
            //
            // ───────────────────────────────────────────────────────────────
            // THE MARKING MECHANISM:
            // ───────────────────────────────────────────────────────────────
            //
            // WHY '#'? 
            // → It's a character that will NEVER match a letter in the word
            // → When we check: board[row][col] != word[index]
            //   If cell is marked '#', it will fail the check
            // → This prevents revisiting marked cells!
            //
            // THE COMPLETE CYCLE:
            // 1. MARK as '#': "I'm using this cell RIGHT NOW in THIS path"
            // 2. EXPLORE: Try all 4 directions recursively
            // 3. RESTORE: "I'm done, remove my mark for OTHER paths"
            //
            // ───────────────────────────────────────────────────────────────
            // ANOTHER EXAMPLE: Why restore is needed
            // ───────────────────────────────────────────────────────────────
            //
            // Board: A B    Word: "AB"
            //        C D
            //
            // Attempt 1: Start at A[0,0]
            //   Mark A as '#': # B
            //                  C D
            //   Try RIGHT: B[0,1] ✓ SUCCESS!
            //   Path: A[0,0] → B[0,1]
            //   Restore A: A B  ← Clean up!
            //              C D
            //
            // Attempt 2: If word was "BA" instead
            //   Start at B[0,1]
            //   Mark B as '#': A #
            //                  C D
            //   Try LEFT: A[0,0] ✓ SUCCESS!
            //   Path: B[0,1] → A[0,0]
            //
            // → Same cell A[0,0] used in DIFFERENT paths ✓ Allowed!
            // → But never twice in SAME path ✓ Prevented by marking!
            //
            // ═══════════════════════════════════════════════════════════════
            
            // Step 1: Save original character (we'll need it to restore!)
            char temp = board[row][col];
            
            // Step 2: Mark this cell as visited for THIS path
            // Use '#' because:
            // - It won't match any letter in the word
            // - When DFS tries to visit this cell again, the check
            //   board[row][col] != word[index] will FAIL
            // - This prevents infinite loops and cell reuse!
            board[row][col] = '#';
            
            // Now this cell is "protected" from being revisited in THIS path
            
            // ═══════════════════════════════════════════════════════════════
            // EXPLORE ALL 4 DIRECTIONS (Backtracking core)
            // ═══════════════════════════════════════════════════════════════
            foreach (var dir in directions)
            {
                int newRow = row + dir[0];
                int newCol = col + dir[1];
                
                // Try this direction with next character in word
                if (DFS(board, word, newRow, newCol, index + 1))
                {
                    // ✓ SUCCESS! We found the word!
                    // 
                    // IMPORTANT: Restore before returning
                    // Why? Because the board is passed by reference,
                    // and we want to leave it unchanged for caller
                    board[row][col] = temp;
                    return true;
                }
                
                // ✗ This direction didn't work
                // The recursive call already restored its cells (backtracked)
                // Now we try the next direction
            }
            
            // ═══════════════════════════════════════════════════════════════
            // BACKTRACK: Restore original character BEFORE returning false
            // ═══════════════════════════════════════════════════════════════
            // All 4 directions failed - this path doesn't work
            // 
            // CRITICAL: We MUST restore before returning false!
            // This is THE KEY to backtracking!
            //
            // ┌─────────────────────────────────────────────────────────────┐
            // │ RESTORATION HAPPENS IN TWO PLACES:                          │
            // │                                                              │
            // │ 1. SUCCESS (inside foreach): restore + return true           │
            // │ 2. FAILURE (here): restore + return false ← YOU ARE HERE!   │
            // │                                                              │
            // │ Both cases ALWAYS restore before returning!                 │
            // └─────────────────────────────────────────────────────────────┘
            //
            // Why restore on failure?
            // → So OTHER paths can try using this cell
            // → Clean up after ourselves
            // → Leave board as we found it
            //
            // Example execution when path fails:
            //   Board: A B    Word: "BD"
            //          C D
            //
            // Step 1: Try A[0,0]
            //   temp = 'A'
            //   board[0][0] = '#'     ← Mark
            //   Board: # B
            //          C D
            //
            // Step 2: Try all 4 directions from A
            //   RIGHT: B (doesn't match 'D')
            //   DOWN:  C (doesn't match 'D')
            //   All fail!
            //
            // Step 3: RESTORE (this line below!)
            //   board[0][0] = temp    ← Restore 'A'
            //   Board: A B            ← Clean!
            //          C D
            //
            // Step 4: Return false
            //   Now caller can try other starting positions
            //   Board is clean, A is visible again
            //
            board[row][col] = temp;  // ← RESTORATION HAPPENS HERE on failure!
            
            return false;  // Only return AFTER restoring!
        }
        
        // Helper method to print board
        private void PrintBoard(char[][] board)
        {
            foreach (var row in board)
            {
                Console.WriteLine($"[{string.Join(", ", Array.ConvertAll(row, c => $"'{c}'"))}]");
            }
        }
        
        // Test method
        public static void Test()
        {
            var solution = new WordSearch();
            
            // Test case 1: Word exists
            char[][] board1 = new char[][]
            {
                new char[] {'B', 'L', 'C', 'H'},
                new char[] {'D', 'E', 'L', 'T'},
                new char[] {'D', 'A', 'K', 'A'}
            };
            string word1 = "BLEAK";
            
            Console.WriteLine("Test Case 1:");
            Console.WriteLine("Board:");
            solution.PrintBoard(board1);
            Console.WriteLine($"Word: \"{word1}\"");
            Console.WriteLine($"Path: B[0,0] → L[0,1] → E[1,1] → A[2,1] → K[2,2]");
            bool result1 = solution.Exist(board1, word1);
            Console.WriteLine($"Result: {result1}");
            Console.WriteLine($"Expected: True\n");
            
            // Test case 2: Word doesn't exist
            char[][] board2 = new char[][]
            {
                new char[] {'B', 'L', 'C', 'H'},
                new char[] {'D', 'E', 'L', 'T'},
                new char[] {'D', 'A', 'K', 'A'}
            };
            string word2 = "BLEED";
            
            Console.WriteLine("Test Case 2 (Word doesn't exist):");
            Console.WriteLine("Board:");
            solution.PrintBoard(board2);
            Console.WriteLine($"Word: \"{word2}\"");
            bool result2 = solution.Exist(board2, word2);
            Console.WriteLine($"Result: {result2}");
            Console.WriteLine($"Expected: False\n");
            
            // Test case 3: Complex path
            char[][] board3 = new char[][]
            {
                new char[] {'A', 'B', 'C', 'E'},
                new char[] {'S', 'F', 'C', 'S'},
                new char[] {'A', 'D', 'E', 'E'}
            };
            string word3 = "ABCCED";
            
            Console.WriteLine("Test Case 3 (Complex path):");
            Console.WriteLine("Board:");
            solution.PrintBoard(board3);
            Console.WriteLine($"Word: \"{word3}\"");
            bool result3 = solution.Exist(board3, word3);
            Console.WriteLine($"Result: {result3}");
            Console.WriteLine($"Expected: True\n");
            
            // Test case 4: Would require cell reuse
            char[][] board4 = new char[][]
            {
                new char[] {'A', 'B'},
                new char[] {'C', 'D'}
            };
            string word4 = "ABCD";
            
            Console.WriteLine("Test Case 4 (Would need to reuse cells):");
            Console.WriteLine("Board:");
            solution.PrintBoard(board4);
            Console.WriteLine($"Word: \"{word4}\"");
            bool result4 = solution.Exist(board4, word4);
            Console.WriteLine($"Result: {result4}");
            Console.WriteLine($"Expected: False\n");
            
            // Test case 5: Single character
            char[][] board5 = new char[][]
            {
                new char[] {'A', 'B'},
                new char[] {'C', 'D'}
            };
            string word5 = "A";
            
            Console.WriteLine("Test Case 5 (Single character):");
            Console.WriteLine("Board:");
            solution.PrintBoard(board5);
            Console.WriteLine($"Word: \"{word5}\"");
            bool result5 = solution.Exist(board5, word5);
            Console.WriteLine($"Result: {result5}");
            Console.WriteLine($"Expected: True\n");
            
            // Test case 6: Word uses entire board
            char[][] board6 = new char[][]
            {
                new char[] {'A', 'B'},
                new char[] {'D', 'C'}
            };
            string word6 = "ABCD";
            
            Console.WriteLine("Test Case 6 (Uses entire board):");
            Console.WriteLine("Board:");
            solution.PrintBoard(board6);
            Console.WriteLine($"Word: \"{word6}\"");
            Console.WriteLine("Path: A[0,0] → B[0,1] → C[1,1] → D[1,0]");
            bool result6 = solution.Exist(board6, word6);
            Console.WriteLine($"Result: {result6}");
            Console.WriteLine($"Expected: True\n");
        }
    }
}
