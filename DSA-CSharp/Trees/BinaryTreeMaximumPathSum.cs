/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Binary Tree Maximum Path Sum
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
Given a binary tree, find the maximum sum of any non-empty path.

A path can start and end at any nodes, but it must follow connected edges
and cannot visit a node more than once. The path does not need to pass
through the root.

INPUT:
- root: TreeNode - Root of the binary tree

OUTPUT:
- int - Maximum path sum

EXAMPLES:
Example 1:
Input: root = [1, 2, 3]
        1
       / \
      2   3
Output: 6
Explanation: The best path is 2 -> 1 -> 3, with sum 6.

Example 2:
Input: root = [-10, 9, 20, null, null, 15, 7]
        -10
         / \
        9  20
           / \
          15  7
Output: 42
Explanation: The best path is 15 -> 20 -> 7, with sum 42.

Example 3:
Input: root = [-3]
    -3
Output: -3
Explanation: The path must be non-empty, so the node itself is used.

CONSTRAINTS:
- 1 <= number of nodes <= 30,000
- -1000 <= node value <= 1000

EDGE CASES:
✓ One node
✓ All values are negative
✓ Best path passes through the root
✓ Best path does not pass through the root
✓ A left-only or right-only tree
✓ A path uses both children of a node

TRICK CASES:
⚡ The answer is not always a root-to-leaf path.
⚡ A negative branch should not be included in a larger path.
⚡ The global answer can use both left and right branches.
⚡ The helper must return only one branch to its parent, not both.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OPTIMAL SOLUTION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APPROACH:
For each node, calculate the maximum path gain that can continue upward
to its parent:

  gain(node) = node.val + max(leftGain, rightGain)

Negative gains are ignored because taking them would make the path smaller:

  leftGain = max(0, gain(left))
  rightGain = max(0, gain(right))

At the current node, a complete path may use both branches:

  leftGain + node.val + rightGain

Update the global answer with this value. Return only the larger branch to
the parent because a path going upward cannot split into two branches.

ALGORITHM STEPS:
1. Set the global answer to the smallest possible integer.
2. Recursively calculate the best gain from the left subtree.
3. Recursively calculate the best gain from the right subtree.
4. Ignore negative gains by replacing them with zero.
5. Update the global answer using both gains and the current node.
6. Return the current value plus the larger gain to the parent.
7. Return the global answer.

TIME COMPLEXITY: O(n)
- Every node is visited once.

SPACE COMPLEXITY: O(h)
- The recursion stack stores one path.
- h is the tree height; worst case O(n).

PATTERN: DFS, Recursion, Tree Dynamic Programming
DIFFICULTY: Hard
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;

namespace DSA_CSharp.Trees
{
    public class BinaryTreeMaximumPathSum
    {
        private int maximumPathSum;

        /// <summary>
        /// Returns the maximum sum of any non-empty path in the tree.
        /// </summary>
        public int MaxPathSum(TreeNode root)
        {
            maximumPathSum = int.MinValue;
            MaxGain(root);
            return maximumPathSum;
        }

        private int MaxGain(TreeNode node)
        {
            if (node == null)
                return 0;

            int leftGain = Math.Max(0, MaxGain(node.left));
            int rightGain = Math.Max(0, MaxGain(node.right));

            int pathThroughNode = leftGain + node.val + rightGain;
            maximumPathSum = Math.Max(maximumPathSum, pathThroughNode);

            return node.val + Math.Max(leftGain, rightGain);
        }
    }
}
