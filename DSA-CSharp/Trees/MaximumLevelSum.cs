/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Maximum Level Sum of a Binary Tree
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
Given the root of a binary tree, return the smallest level x such that the 
sum of all the values of nodes at level x is maximal.

Note: The level of the root is 1, the level of its children is 2, and so on.

INPUT:
- root: TreeNode - Root of binary tree
- Number of nodes: 1 <= nodes <= 10^4
- Node values: -10^5 <= Node.val <= 10^5

OUTPUT:
- int - The level (1-indexed) with the maximum sum

EXAMPLES:
Example 1:
Input: root = [1,7,0,7,-8,null,null]
        1
       / \
      7   0
     / \
    7  -8
Output: 2
Explanation: 
- Level 1 sum = 1
- Level 2 sum = 7 + 0 = 7
- Level 3 sum = 7 + (-8) = -1
Maximum sum is 7 at level 2

Example 2:
Input: root = [989,null,10250,98693,-89388,null,null,null,-32127]
Output: 2
Explanation: Level 2 has the maximum sum

Example 3:
Input: root = [1]
Output: 1
Explanation: Only one level

CONSTRAINTS:
- 1 <= number of nodes <= 10^4
- -10^5 <= Node.val <= 10^5

EDGE CASES:
✓ Single node tree → level 1
✓ All negative values → choose level with least negative sum
✓ Skewed tree (only left or only right children)
✓ Complete binary tree
✓ Multiple levels with same sum → return smallest level number
✓ Very deep tree (up to 10^4 nodes)
✓ Large negative values causing overflow risk

TRICK CASES:
⚡ Negative values mean max sum might be negative
⚡ Return SMALLEST level if there's a tie
⚡ Levels are 1-indexed, not 0-indexed
⚡ Need to track level number along with sum
⚡ Int overflow possible with large sums (use long for safety)

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OPTIMAL SOLUTION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APPROACH:
Use LEVEL ORDER TRAVERSAL (BFS) to process the tree level by level.
Calculate the sum for each level and track which level has the maximum sum.

This is a classic BFS application - perfect for level-by-level processing.

KEY INSIGHTS:
• BFS naturally processes nodes level by level
• Use queue to maintain nodes at current level
• Process all nodes at current level before moving to next
• Track level number (1-indexed) and corresponding sum
• Keep running maximum and the level that produced it

ALGORITHM STEPS:
1. Handle edge case: if root is null, return 0 (or handle as needed)
2. Initialize queue with root node
3. Initialize level counter = 1, maxSum = int.MinValue, maxLevel = 1
4. While queue is not empty:
   a. Get current level size (number of nodes at this level)
   b. Initialize levelSum = 0
   c. For each node in current level:
      - Dequeue node
      - Add node.val to levelSum
      - Enqueue left and right children if they exist
   d. If levelSum > maxSum:
      - Update maxSum = levelSum
      - Update maxLevel = current level
   e. Increment level counter
5. Return maxLevel

TIME COMPLEXITY: O(n)
- Visit each node exactly once
- Queue operations (enqueue/dequeue) are O(1)
- Total: O(n) where n is number of nodes

SPACE COMPLEXITY: O(w)
- Queue stores nodes at current level
- Maximum width w of tree determines queue size
- Worst case: complete binary tree at last level has n/2 nodes
- So O(n) in worst case, but typically O(w) where w << n

PATTERN: BFS (Level Order Traversal), Queue
DIFFICULTY: Medium
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;
using System.Collections.Generic;

namespace DSA_CSharp.Trees
{
    // Definition for a binary tree node
    public class TreeNode
    {
        public int val;
        public TreeNode left;
        public TreeNode right;
        
        public TreeNode(int val = 0, TreeNode left = null, TreeNode right = null)
        {
            this.val = val;
            this.left = left;
            this.right = right;
        }
    }
    
    public class MaximumLevelSum
    {
        /// <summary>
        /// Finds the level with the maximum sum in a binary tree.
        /// Uses BFS (level order traversal) to process level by level.
        /// </summary>
        /// <param name="root">Root of the binary tree</param>
        /// <returns>Level number (1-indexed) with maximum sum</returns>
        public int MaxLevelSum(TreeNode root)
        {
            if (root == null) return 0;
            
            Queue<TreeNode> queue = new Queue<TreeNode>();
            queue.Enqueue(root);
            
            int maxSum = int.MinValue;
            int maxLevel = 1;
            int currentLevel = 1;
            
            while (queue.Count > 0)
            {
                int levelSize = queue.Count;
                long levelSum = 0; // Use long to avoid overflow
                
                // Process all nodes at current level
                for (int i = 0; i < levelSize; i++)
                {
                    TreeNode node = queue.Dequeue();
                    levelSum += node.val;
                    
                    // Add children for next level
                    if (node.left != null)
                        queue.Enqueue(node.left);
                    if (node.right != null)
                        queue.Enqueue(node.right);
                }
                
                // Update max if current level sum is greater
                if (levelSum > maxSum)
                {
                    maxSum = (int)levelSum;
                    maxLevel = currentLevel;
                }
                
                currentLevel++;
            }
            
            return maxLevel;
        }
        
        // Helper method: Get all level sums for visualization
        public List<long> GetAllLevelSums(TreeNode root)
        {
            List<long> levelSums = new List<long>();
            if (root == null) return levelSums;
            
            Queue<TreeNode> queue = new Queue<TreeNode>();
            queue.Enqueue(root);
            
            while (queue.Count > 0)
            {
                int levelSize = queue.Count;
                long levelSum = 0;
                
                for (int i = 0; i < levelSize; i++)
                {
                    TreeNode node = queue.Dequeue();
                    levelSum += node.val;
                    
                    if (node.left != null)
                        queue.Enqueue(node.left);
                    if (node.right != null)
                        queue.Enqueue(node.right);
                }
                
                levelSums.Add(levelSum);
            }
            
            return levelSums;
        }
        
        // Test method
        public static void Test()
        {
            var solution = new MaximumLevelSum();
            
            // Test case 1: Example from problem
            //        1
            //       / \
            //      7   0
            //     / \
            //    7  -8
            TreeNode root1 = new TreeNode(1,
                new TreeNode(7,
                    new TreeNode(7),
                    new TreeNode(-8)),
                new TreeNode(0));
            
            int result1 = solution.MaxLevelSum(root1);
            var sums1 = solution.GetAllLevelSums(root1);
            Console.WriteLine("Test Case 1:");
            Console.WriteLine("Tree structure:");
            Console.WriteLine("      1");
            Console.WriteLine("     / \\");
            Console.WriteLine("    7   0");
            Console.WriteLine("   / \\");
            Console.WriteLine("  7  -8");
            Console.WriteLine($"Level sums: [{string.Join(", ", sums1)}]");
            Console.WriteLine($"Maximum level: {result1}");
            Console.WriteLine($"Expected: 2\n");
            
            // Test case 2: Single node
            TreeNode root2 = new TreeNode(5);
            int result2 = solution.MaxLevelSum(root2);
            Console.WriteLine("Test Case 2 (Single node):");
            Console.WriteLine($"Result: {result2}");
            Console.WriteLine($"Expected: 1\n");
            
            // Test case 3: All negative values
            //      -1
            //     /  \
            //   -2   -3
            TreeNode root3 = new TreeNode(-1,
                new TreeNode(-2),
                new TreeNode(-3));
            
            int result3 = solution.MaxLevelSum(root3);
            var sums3 = solution.GetAllLevelSums(root3);
            Console.WriteLine("Test Case 3 (All negative):");
            Console.WriteLine($"Level sums: [{string.Join(", ", sums3)}]");
            Console.WriteLine($"Maximum level: {result3}");
            Console.WriteLine($"Expected: 1 (least negative)\n");
            
            // Test case 4: Tied sums - should return smallest level
            //       5
            //      / \
            //     3   2
            //    /
            //   5
            TreeNode root4 = new TreeNode(5,
                new TreeNode(3,
                    new TreeNode(5),
                    null),
                new TreeNode(2));
            
            int result4 = solution.MaxLevelSum(root4);
            var sums4 = solution.GetAllLevelSums(root4);
            Console.WriteLine("Test Case 4 (Tied sums):");
            Console.WriteLine($"Level sums: [{string.Join(", ", sums4)}]");
            Console.WriteLine($"Maximum level: {result4}");
            Console.WriteLine($"Expected: 1 (smallest level with max sum)\n");
        }
    }
}
