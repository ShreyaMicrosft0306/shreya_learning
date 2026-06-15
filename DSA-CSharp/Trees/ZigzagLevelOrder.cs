/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Binary Tree Zigzag Level Order Traversal
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
Given the root of a binary tree, return the zigzag level order traversal of 
its nodes' values. (i.e., from left to right, then right to left for the next 
level and alternate between).

INPUT:
- root: TreeNode - Root of binary tree
- Number of nodes: 0 <= nodes <= 2000
- Node values: -100 <= Node.val <= 100

OUTPUT:
- List<List<int>> - List of lists, each inner list represents a level
  Odd levels (1, 3, 5...): left to right
  Even levels (2, 4, 6...): right to left

EXAMPLES:
Example 1:
Input: root = [3,9,20,null,null,15,7]
        3
       / \
      9  20
         / \
        15  7
Output: [[3],[20,9],[15,7]]
Explanation:
- Level 1: [3] → left to right
- Level 2: [20,9] → right to left (reversed)
- Level 3: [15,7] → left to right

Example 2:
Input: root = [1,2,3,4,null,null,5]
        1
       / \
      2   3
     /     \
    4       5
Output: [[1],[3,2],[4,5]]
Explanation:
- Level 1: [1] → left to right
- Level 2: [3,2] → right to left
- Level 3: [4,5] → left to right

Example 3:
Input: root = [1]
Output: [[1]]

Example 4:
Input: root = []
Output: []

CONSTRAINTS:
- 0 <= number of nodes <= 2000
- -100 <= Node.val <= 100

EDGE CASES:
✓ Empty tree (null root) → empty list
✓ Single node → [[value]]
✓ Only left children (skewed left)
✓ Only right children (skewed right)
✓ Complete binary tree
✓ Perfect binary tree
✓ Tree with many levels requiring multiple direction switches

TRICK CASES:
⚡ Don't forget to alternate direction for EACH level
⚡ Level 1 is left-to-right, level 2 is right-to-left (1-indexed thinking)
⚡ Can't just reverse final result - need per-level reversal
⚡ Need to track which level you're on (odd/even)
⚡ Empty tree returns empty list, not null

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OPTIMAL SOLUTION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APPROACH:
Use standard BFS (level order traversal) with a direction flag that alternates
each level. For levels that need to be reversed, add elements to the list in 
reverse order.

Alternative: Use BFS and reverse alternate levels after collecting them.

KEY INSIGHTS:
• Standard BFS processes nodes level by level
• Track level number or use boolean flag to determine direction
• For right-to-left levels, insert at beginning of list OR collect and reverse
• Queue still processes left-to-right (standard BFS), only OUTPUT changes

ALGORITHM STEPS:
1. Handle edge case: if root is null, return empty list
2. Initialize result list, queue with root, and leftToRight flag = true
3. While queue is not empty:
   a. Get current level size
   b. Create list for current level
   c. For each node in current level:
      - Dequeue node
      - If leftToRight: add to end of level list
      - Else: insert at beginning of level list (or add and reverse later)
      - Enqueue left and right children
   d. Add level list to result
   e. Toggle leftToRight flag
4. Return result

IMPLEMENTATION NOTE:
Using List.Insert(0, value) for right-to-left is O(n) per insert.
Better: Add normally and reverse the list at end of level if needed.
Or use LinkedList for O(1) insertion at beginning.
For this solution, we'll add normally and reverse when needed.

TIME COMPLEXITY: O(n)
- Visit each node exactly once: O(n)
- Reversing lists: each node is involved in one reverse, so O(n) total
- Total: O(n)

SPACE COMPLEXITY: O(w)
- Queue stores nodes at current level, max width w
- Result list stores all nodes: O(n) but that's output, not auxiliary
- Auxiliary space: O(w) where w is maximum width

PATTERN: BFS (Level Order Traversal), Direction Toggle
DIFFICULTY: Medium
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;
using System.Collections.Generic;
using System.Linq;

namespace DSA_CSharp.Trees
{
    public class ZigzagLevelOrder
    {
        /// <summary>
        /// Returns zigzag level order traversal of a binary tree.
        /// Alternates between left-to-right and right-to-left for each level.
        /// </summary>
        /// <param name="root">Root of the binary tree</param>
        /// <returns>List of lists representing zigzag level order</returns>
        public IList<IList<int>> ZigzagLevelOrderTraversal(TreeNode root)
        {
            IList<IList<int>> result = new List<IList<int>>();
            if (root == null) return result;
            
            Queue<TreeNode> queue = new Queue<TreeNode>();
            queue.Enqueue(root);
            bool leftToRight = true;
            
            while (queue.Count > 0)
            {
                int levelSize = queue.Count;
                List<int> currentLevel = new List<int>();
                
                // Process all nodes at current level
                for (int i = 0; i < levelSize; i++)
                {
                    TreeNode node = queue.Dequeue();
                    currentLevel.Add(node.val);
                    
                    // Add children for next level (always left then right)
                    if (node.left != null)
                        queue.Enqueue(node.left);
                    if (node.right != null)
                        queue.Enqueue(node.right);
                }
                
                // Reverse if right-to-left level
                if (!leftToRight)
                    currentLevel.Reverse();
                
                result.Add(currentLevel);
                leftToRight = !leftToRight; // Toggle direction
            }
            
            return result;
        }
        
        // Alternative implementation using deque (LinkedList) for O(1) insertion
        public IList<IList<int>> ZigzagLevelOrderDeque(TreeNode root)
        {
            IList<IList<int>> result = new List<IList<int>>();
            if (root == null) return result;
            
            Queue<TreeNode> queue = new Queue<TreeNode>();
            queue.Enqueue(root);
            bool leftToRight = true;
            
            while (queue.Count > 0)
            {
                int levelSize = queue.Count;
                LinkedList<int> currentLevel = new LinkedList<int>();
                
                for (int i = 0; i < levelSize; i++)
                {
                    TreeNode node = queue.Dequeue();
                    
                    // Add to appropriate end based on direction
                    if (leftToRight)
                        currentLevel.AddLast(node.val);
                    else
                        currentLevel.AddFirst(node.val);
                    
                    if (node.left != null)
                        queue.Enqueue(node.left);
                    if (node.right != null)
                        queue.Enqueue(node.right);
                }
                
                result.Add(new List<int>(currentLevel));
                leftToRight = !leftToRight;
            }
            
            return result;
        }
        
        // Test method
        public static void Test()
        {
            var solution = new ZigzagLevelOrder();
            
            // Test case 1: Standard tree
            //        3
            //       / \
            //      9  20
            //         / \
            //        15  7
            TreeNode root1 = new TreeNode(3,
                new TreeNode(9),
                new TreeNode(20,
                    new TreeNode(15),
                    new TreeNode(7)));
            
            var result1 = solution.ZigzagLevelOrderTraversal(root1);
            Console.WriteLine("Test Case 1:");
            Console.WriteLine("Tree structure:");
            Console.WriteLine("      3");
            Console.WriteLine("     / \\");
            Console.WriteLine("    9  20");
            Console.WriteLine("       / \\");
            Console.WriteLine("      15  7");
            Console.WriteLine("Zigzag traversal:");
            foreach (var level in result1)
            {
                Console.WriteLine($"[{string.Join(",", level)}]");
            }
            Console.WriteLine("Expected: [[3],[20,9],[15,7]]\n");
            
            // Test case 2: More complex tree
            //        1
            //       / \
            //      2   3
            //     /     \
            //    4       5
            TreeNode root2 = new TreeNode(1,
                new TreeNode(2,
                    new TreeNode(4),
                    null),
                new TreeNode(3,
                    null,
                    new TreeNode(5)));
            
            var result2 = solution.ZigzagLevelOrderTraversal(root2);
            Console.WriteLine("Test Case 2:");
            Console.WriteLine("Zigzag traversal:");
            foreach (var level in result2)
            {
                Console.WriteLine($"[{string.Join(",", level)}]");
            }
            Console.WriteLine("Expected: [[1],[3,2],[4,5]]\n");
            
            // Test case 3: Single node
            TreeNode root3 = new TreeNode(1);
            var result3 = solution.ZigzagLevelOrderTraversal(root3);
            Console.WriteLine("Test Case 3 (Single node):");
            Console.WriteLine($"Result: [{string.Join(",", result3[0])}]");
            Console.WriteLine("Expected: [[1]]\n");
            
            // Test case 4: Empty tree
            TreeNode root4 = null;
            var result4 = solution.ZigzagLevelOrderTraversal(root4);
            Console.WriteLine("Test Case 4 (Empty tree):");
            Console.WriteLine($"Result: {(result4.Count == 0 ? "[]" : "Error")}");
            Console.WriteLine("Expected: []\n");
            
            // Test case 5: Skewed tree (right only)
            //    1
            //     \
            //      2
            //       \
            //        3
            TreeNode root5 = new TreeNode(1,
                null,
                new TreeNode(2,
                    null,
                    new TreeNode(3)));
            
            var result5 = solution.ZigzagLevelOrderTraversal(root5);
            Console.WriteLine("Test Case 5 (Skewed right):");
            Console.WriteLine("Zigzag traversal:");
            foreach (var level in result5)
            {
                Console.WriteLine($"[{string.Join(",", level)}]");
            }
            Console.WriteLine("Expected: [[1],[2],[3]]\n");
            
            // Test deque version
            Console.WriteLine("Testing deque implementation on Test Case 1:");
            var resultDeque = solution.ZigzagLevelOrderDeque(root1);
            foreach (var level in resultDeque)
            {
                Console.WriteLine($"[{string.Join(",", level)}]");
            }
        }
    }
}
