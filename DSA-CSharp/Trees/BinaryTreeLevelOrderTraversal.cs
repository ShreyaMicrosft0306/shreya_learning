/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Binary Tree Level Order Traversal
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
Given the root of a binary tree, return its values level by level from
left to right.

INPUT:
- root: TreeNode - Root of the binary tree

OUTPUT:
- IList<IList<int>> - Values grouped by tree level

EXAMPLES:
Example 1:
Input:
        3
       / \
      9  20
         / \
        15  7
Output: [[3], [9, 20], [15, 7]]
Explanation: Read the tree one level at a time.

Example 2:
Input:
        1
       / \
      2   3
     /
    4
Output: [[1], [2, 3], [4]]

Example 3:
Input: root = null
Output: []
Explanation: An empty tree has no levels.

CONSTRAINTS:
- 0 <= number of nodes <= 2000
- -1000 <= node value <= 1000

EDGE CASES:
✓ Empty tree -> empty result
✓ Single node -> one level containing that node
✓ Only left children
✓ Only right children
✓ Complete binary tree

TRICK CASES:
⚡ Process all nodes in the current level before moving to the next level.
⚡ Save the queue size before processing a level.
⚡ Add children to the queue for the next level.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OPTIMAL SOLUTION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APPROACH:
Use Breadth-First Search (BFS) with a queue.

The queue stores nodes waiting to be processed. At the start of each level,
queue.Count tells us exactly how many nodes belong to that level. Process
only those nodes, save their values, and add their children to the queue.

ALGORITHM STEPS:
1. If root is null, return an empty result.
2. Put root into a queue.
3. While the queue is not empty:
   a. Save the current queue size as the level size.
   b. Remove exactly level size nodes and add their values to one list.
   c. Add each removed node's children to the queue.
   d. Add the level list to the result.
4. Return the result.

TIME COMPLEXITY: O(n)
- Every node is added to and removed from the queue once.

SPACE COMPLEXITY: O(n)
- The queue and result store tree values. The queue's maximum size is the
  maximum width of the tree.

PATTERN: BFS (Level Order Traversal), Queue
DIFFICULTY: Medium
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System.Collections.Generic;

namespace DSA_CSharp.Trees
{
    public class BinaryTreeLevelOrderTraversal
    {
        /// <summary>
        /// Returns the values of a binary tree grouped by level.
        /// </summary>
        public IList<IList<int>> LevelOrder(TreeNode root)
        {
            IList<IList<int>> result = new List<IList<int>>();

            if (root == null)
                return result;

            Queue<TreeNode> queue = new Queue<TreeNode>();
            queue.Enqueue(root);

            while (queue.Count > 0)
            {
                int levelSize = queue.Count;
                IList<int> currentLevel = new List<int>();

                for (int i = 0; i < levelSize; i++)
                {
                    TreeNode node = queue.Dequeue();
                    currentLevel.Add(node.val);

                    if (node.left != null)
                        queue.Enqueue(node.left);

                    if (node.right != null)
                        queue.Enqueue(node.right);
                }

                result.Add(currentLevel);
            }

            return result;
        }
    }
}
