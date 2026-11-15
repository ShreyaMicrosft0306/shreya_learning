/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Maximum Depth of a Binary Tree
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
Given the root of a binary tree, return its maximum depth.

Maximum depth means the number of nodes on the longest path from the root
down to a leaf.

INPUT:
- root: TreeNode - Root of the binary tree

OUTPUT:
- int - Maximum number of nodes from root to the deepest leaf

EXAMPLES:
Example 1:
Input: root = [3,9,20,null,null,15,7]
        3
       / \
      9  20
         / \
        15  7
Output: 3
Explanation: The longest paths are 3 -> 20 -> 15 and 3 -> 20 -> 7.

Example 2:
Input: root = [1,null,2]
    1
     \
      2
Output: 2
Explanation: The longest path contains 1 and 2.

CONSTRAINTS:
- 0 <= number of nodes <= 10^4
- -100 <= node value <= 100

EDGE CASES:
✓ Empty tree -> depth 0
✓ Single node -> depth 1
✓ Only left children
✓ Only right children
✓ Complete binary tree

TRICK CASES:
⚡ Depth counts nodes, not edges.
⚡ The answer is the larger depth of the left and right subtrees.
⚡ A null child contributes depth 0.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OPTIMAL SOLUTION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APPROACH:
Use recursive DFS. For each node:
1. Find the maximum depth of its left subtree.
2. Find the maximum depth of its right subtree.
3. Add the current node to the larger depth.

The formula is:

  depth(node) = 1 + max(depth(left), depth(right))

ALGORITHM STEPS:
1. If the node is null, return 0.
2. Recursively find the left subtree depth.
3. Recursively find the right subtree depth.
4. Return 1 plus the larger depth.

TIME COMPLEXITY: O(n)
- Every node is visited once.

SPACE COMPLEXITY: O(h)
- The recursion stack stores one path from root to leaf.
- h is the height of the tree; worst case O(n).

PATTERN: DFS, Recursion
DIFFICULTY: Easy
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;

namespace DSA_CSharp.Trees
{
    public class MaximumDepthOfBinaryTree
    {
        /// <summary>
        /// Returns the maximum depth of a binary tree.
        /// </summary>
        public int MaxDepth(TreeNode root)
        {
            if (root == null)
                return 0;

            int leftDepth = MaxDepth(root.left);
            int rightDepth = MaxDepth(root.right);

            return 1 + Math.Max(leftDepth, rightDepth);
        }
    }
}
