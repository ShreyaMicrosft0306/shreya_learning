/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Minimum Knight Moves
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
You are given a chessboard of infinite size where the coordinates of each cell 
are defined by integer pairs (x, y). The knight piece moves in an L-shape, 
either two squares horizontally and one square vertically, or two squares 
vertically and one square horizontally.

Write a function to determine the minimum number of moves required for the 
knight to move from the starting position (0, 0) to the target position (x, y). 
Assume that it is always possible to reach the target position.

INPUT:
- x: int - Target x-coordinate (-200 <= x <= 200)
- y: int - Target y-coordinate (-200 <= y <= 200)

OUTPUT:
- int - Minimum number of knight moves from (0, 0) to (x, y)

EXAMPLES:
Example 1:
Input: x = 1, y = 2
Output: 1
Explanation: The knight can move from (0, 0) to (1, 2) in one move.

Example 2:
Input: x = 4, y = 4
Output: 4
Explanation: One possible path: [0,0] → [2,1] → [4,2] → [6,3] → [4,4]

Example 3:
Input: x = 2, y = 1
Output: 1
Explanation: Knight moves from (0, 0) to (2, 1) in one move.

Example 4:
Input: x = 5, y = 5
Output: 4
Explanation: [0,0] → [2,1] → [4,2] → [6,4] → [5,5]

CONSTRAINTS:
- -200 <= x, y <= 200
- Chessboard is infinite in size
- Knight always starts at (0, 0)

EDGE CASES:
✓ Target at origin (0, 0) → 0 moves
✓ Target one knight move away → 1 move
✓ Negative coordinates
✓ Target at (1, 1) → 2 moves (tricky!)
✓ Large coordinates (x=200, y=200)
✓ Asymmetric targets (x ≠ y)
✓ Target on axes (x=5, y=0) or (x=0, y=5)

TRICK CASES:
⚡ Infinite board - need to bound search space to avoid infinite exploration
⚡ Negative coordinates - use symmetry (knight moves are symmetric)
⚡ Can convert to first quadrant: use abs(x) and abs(y)
⚡ Knight sometimes needs to go "away" from target to reach it optimally
⚡ Search space bound: roughly 2 * max(|x|, |y|) + 4 is safe
⚡ (1, 1) requires 2 moves, not 1 (special case to remember)
⚡ BFS guarantees shortest path (use BFS, not DFS)

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OPTIMAL SOLUTION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APPROACH:
Use BFS (Breadth-First Search) to find the shortest path from (0, 0) to (x, y).
BFS naturally explores level by level, guaranteeing the shortest path in an 
unweighted graph.

KEY INSIGHTS:
• Knight has 8 possible moves from any position (L-shaped moves)
• BFS explores all positions at distance d before exploring distance d+1
• Chessboard symmetry: can work in first quadrant only (use abs values)
• Need bounded search space to avoid infinite exploration
• Use visited set to avoid revisiting positions

ALGORITHM STEPS:
1. Handle edge case: if target is (0, 0), return 0
2. Use symmetry: work with abs(x) and abs(y) in first quadrant
3. Initialize BFS:
   - Queue with starting position (0, 0) and moves = 0
   - Visited set to track explored positions
4. Define 8 knight move directions
5. While queue is not empty:
   a. Dequeue current position and move count
   b. Try all 8 knight moves
   c. For each valid move:
      - Check if it reaches target → return moves + 1
      - Check if within bounds and not visited
      - Add to queue and mark visited
   d. Increment move count
6. Return result (guaranteed to find path)

OPTIMIZATION:
- Bound the search space to avoid exploring too far
- Use (max(|x|, |y|) + 4) as reasonable bounds
- The +4 buffer accounts for cases where knight goes beyond target first

TIME COMPLEXITY: O(x × y)
- In worst case, explore area proportional to target coordinates
- Each cell visited at most once
- With bounded search: O(max(|x|, |y|)²)

SPACE COMPLEXITY: O(x × y)
- Queue and visited set store positions
- Proportional to explored area
- With bounded search: O(max(|x|, |y|)²)

PATTERN: BFS (Shortest Path), Symmetry Optimization
DIFFICULTY: Medium
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;
using System.Collections.Generic;

namespace DSA_CSharp.Graphs
{
    public class MinimumKnightMoves
    {
        // 8 possible knight moves (L-shaped)
        private readonly int[][] knightMoves = new int[][]
        {
            new int[] {2, 1},   // Right 2, Up 1
            new int[] {2, -1},  // Right 2, Down 1
            new int[] {-2, 1},  // Left 2, Up 1
            new int[] {-2, -1}, // Left 2, Down 1
            new int[] {1, 2},   // Right 1, Up 2
            new int[] {1, -2},  // Right 1, Down 2
            new int[] {-1, 2},  // Left 1, Up 2
            new int[] {-1, -2}  // Left 1, Down 2
        };
        
        /// <summary>
        /// Finds minimum number of knight moves from (0, 0) to (x, y).
        /// Uses BFS for shortest path on infinite chessboard.
        /// </summary>
        /// <param name="x">Target x-coordinate</param>
        /// <param name="y">Target y-coordinate</param>
        /// <returns>Minimum number of moves</returns>
        public int MinKnightMoves(int x, int y)
        {
            // Use symmetry - work in first quadrant only
            x = Math.Abs(x);
            y = Math.Abs(y);
            
            // Edge case: already at target
            if (x == 0 && y == 0) return 0;
            
            // BFS setup
            Queue<(int x, int y, int moves)> queue = new Queue<(int, int, int)>();
            HashSet<(int, int)> visited = new HashSet<(int, int)>();
            
            queue.Enqueue((0, 0, 0));
            visited.Add((0, 0));
            
            // Bound the search space to avoid infinite exploration
            // Knight might need to go slightly beyond target
            int bound = Math.Max(x, y) + 4;
            
            while (queue.Count > 0)
            {
                var (currX, currY, moves) = queue.Dequeue();
                
                // Try all 8 knight moves
                foreach (var move in knightMoves)
                {
                    int nextX = currX + move[0];
                    int nextY = currY + move[1];
                    
                    // Check if we reached the target
                    if (nextX == x && nextY == y)
                        return moves + 1;
                    
                    // Check bounds and visited
                    // Allow small negative values as knight might need to backtrack
                    if (nextX >= -2 && nextX <= bound && 
                        nextY >= -2 && nextY <= bound &&
                        !visited.Contains((nextX, nextY)))
                    {
                        visited.Add((nextX, nextY));
                        queue.Enqueue((nextX, nextY, moves + 1));
                    }
                }
            }
            
            // Should never reach here (always possible to reach target)
            return -1;
        }
        
        // Alternative: BFS with bidirectional search (more optimal for large distances)
        public int MinKnightMovesBidirectional(int x, int y)
        {
            x = Math.Abs(x);
            y = Math.Abs(y);
            
            if (x == 0 && y == 0) return 0;
            
            // Two BFS searches: from start and from target
            HashSet<(int, int)> visitedStart = new HashSet<(int, int)> { (0, 0) };
            HashSet<(int, int)> visitedTarget = new HashSet<(int, int)> { (x, y) };
            
            Queue<(int x, int y)> queueStart = new Queue<(int, int)>();
            Queue<(int x, int y)> queueTarget = new Queue<(int, int)>();
            
            queueStart.Enqueue((0, 0));
            queueTarget.Enqueue((x, y));
            
            int moves = 0;
            
            while (queueStart.Count > 0 || queueTarget.Count > 0)
            {
                moves++;
                
                // Expand from start
                int startSize = queueStart.Count;
                for (int i = 0; i < startSize; i++)
                {
                    var (currX, currY) = queueStart.Dequeue();
                    
                    foreach (var move in knightMoves)
                    {
                        int nextX = currX + move[0];
                        int nextY = currY + move[1];
                        
                        if (visitedTarget.Contains((nextX, nextY)))
                            return moves;
                        
                        if (nextX >= -2 && nextY >= -2 && 
                            nextX <= x + 4 && nextY <= y + 4 &&
                            !visitedStart.Contains((nextX, nextY)))
                        {
                            visitedStart.Add((nextX, nextY));
                            queueStart.Enqueue((nextX, nextY));
                        }
                    }
                }
                
                // Expand from target
                int targetSize = queueTarget.Count;
                for (int i = 0; i < targetSize; i++)
                {
                    var (currX, currY) = queueTarget.Dequeue();
                    
                    foreach (var move in knightMoves)
                    {
                        int nextX = currX + move[0];
                        int nextY = currY + move[1];
                        
                        if (visitedStart.Contains((nextX, nextY)))
                            return moves;
                        
                        if (nextX >= -2 && nextY >= -2 && 
                            nextX <= x + 4 && nextY <= y + 4 &&
                            !visitedTarget.Contains((nextX, nextY)))
                        {
                            visitedTarget.Add((nextX, nextY));
                            queueTarget.Enqueue((nextX, nextY));
                        }
                    }
                }
            }
            
            return -1;
        }
        
        // Test method
        public static void Test()
        {
            var solution = new MinimumKnightMoves();
            
            // Test case 1: Simple one move
            int result1 = solution.MinKnightMoves(1, 2);
            Console.WriteLine($"Test Case 1: Target (1, 2)");
            Console.WriteLine($"Result: {result1}");
            Console.WriteLine($"Expected: 1\n");
            
            // Test case 2: Square diagonal
            int result2 = solution.MinKnightMoves(4, 4);
            Console.WriteLine($"Test Case 2: Target (4, 4)");
            Console.WriteLine($"Result: {result2}");
            Console.WriteLine($"Expected: 4\n");
            
            // Test case 3: Another one move
            int result3 = solution.MinKnightMoves(2, 1);
            Console.WriteLine($"Test Case 3: Target (2, 1)");
            Console.WriteLine($"Result: {result3}");
            Console.WriteLine($"Expected: 1\n");
            
            // Test case 4: Tricky case (1, 1)
            int result4 = solution.MinKnightMoves(1, 1);
            Console.WriteLine($"Test Case 4: Target (1, 1) - Tricky!");
            Console.WriteLine($"Result: {result4}");
            Console.WriteLine($"Expected: 2 (Knight can't reach (1,1) in one move)\n");
            
            // Test case 5: Origin
            int result5 = solution.MinKnightMoves(0, 0);
            Console.WriteLine($"Test Case 5: Target (0, 0) - Origin");
            Console.WriteLine($"Result: {result5}");
            Console.WriteLine($"Expected: 0\n");
            
            // Test case 6: Negative coordinates
            int result6 = solution.MinKnightMoves(-5, -5);
            Console.WriteLine($"Test Case 6: Target (-5, -5) - Negative");
            Console.WriteLine($"Result: {result6}");
            Console.WriteLine($"Expected: 4 (symmetry applies)\n");
            
            // Test case 7: Large distance
            int result7 = solution.MinKnightMoves(5, 5);
            Console.WriteLine($"Test Case 7: Target (5, 5)");
            Console.WriteLine($"Result: {result7}");
            Console.WriteLine($"Expected: 4\n");
            
            // Test case 8: On axis
            int result8 = solution.MinKnightMoves(6, 0);
            Console.WriteLine($"Test Case 8: Target (6, 0) - On x-axis");
            Console.WriteLine($"Result: {result8}");
            Console.WriteLine($"Expected: 4\n");
            
            // Test bidirectional version
            Console.WriteLine("Testing bidirectional BFS:");
            int resultBi = solution.MinKnightMovesBidirectional(4, 4);
            Console.WriteLine($"Bidirectional result for (4, 4): {resultBi}");
            Console.WriteLine($"Expected: 4\n");
        }
    }
}
