/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Kth Smallest Element in a BST
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
Given the root of a Binary Search Tree (BST) and an integer k, return the
k-th smallest value in the tree. k is 1-indexed.

INPUT:
- root: TreeNode - Root of the BST
- k: int - Position of the value to return

OUTPUT:
- int - The k-th smallest value

EXAMPLES:
Example 1:
Input: root = [3, 1, 4, null, 2], k = 1
        3
       / \
      1   4
       \
        2
Output: 1
Explanation: The values in sorted order are [1, 2, 3, 4].

Example 2:
Input: root = [5, 3, 6, 2, 4, null, null, 1], k = 3
          5
         / \
        3   6
       / \
      2   4
     /
    1
Output: 3
Explanation: The values in sorted order are [1, 2, 3, 4, 5, 6].

Example 3:
Input: root = [1], k = 1
    1
Output: 1

CONSTRAINTS:
- 1 <= number of nodes <= 10^4
- 1 <= k <= number of nodes
- Node values are valid integers

EDGE CASES:
✓ k = 1 -> return the smallest value
✓ k = number of nodes -> return the largest value
✓ A tree with only one node
✓ A completely left-skewed tree
✓ A completely right-skewed tree

TRICK CASES:
⚡ Do not use level order; BST order is discovered through inorder traversal.
⚡ k is 1-indexed, so the first visited node has count 1.
⚡ Stop as soon as the k-th node is visited.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OPTIMAL SOLUTION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APPROACH:
In a BST, inorder traversal visits values in ascending order:

  left subtree -> node -> right subtree

Therefore, the k-th node visited during inorder traversal is the k-th
smallest value.

There are two ways to perform the inorder traversal:

1. Iterative: use a stack to remember nodes while walking left.
2. Recursive: let the call stack remember those nodes.

Both visit nodes in sorted order and stop when the k-th node is found.

ALGORITHM STEPS:
1. Start at the root and create an empty stack.
2. Push nodes while moving left.
3. Pop one node; this is the next smallest value.
4. Increase the visited count.
5. If the count equals k, return the node value.
6. Move to the node's right child and repeat.

ITERATIVE TIME COMPLEXITY: O(h + k)
- h = height of the tree
- We walk down the first left path and visit nodes until the k-th value.
- Worst case: O(n)

ITERATIVE SPACE COMPLEXITY: O(h)
- The explicit stack stores at most one path from the root to a leaf.

RECURSIVE TIME COMPLEXITY: O(h + k)
- The recursive traversal visits nodes until the k-th value.
- Worst case: O(n).

RECURSIVE SPACE COMPLEXITY: O(h)
- The function call stack stores the current path.

PATTERN: Inorder Traversal, Stack, Binary Search Tree
DIFFICULTY: Medium
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System.Collections.Generic;

namespace DSA_CSharp.Trees
{
    public class KthSmallestElementInBST
    {
        /// <summary>
        /// Returns the k-th smallest value in a binary search tree.
        /// </summary>
        public int KthSmallest(TreeNode root, int k)
        {
            Stack<TreeNode> stack = new Stack<TreeNode>();
            TreeNode current = root;
            int visitedCount = 0;

            while (current != null || stack.Count > 0)
            {
                while (current != null)
                {
                    stack.Push(current);
                    current = current.left;
                }

                current = stack.Pop();
                visitedCount++;

                if (visitedCount == k)
                    return current.val;

                current = current.right;
            }

            return -1;
        }

        /// <summary>
        /// Returns the k-th smallest value using recursive inorder traversal.
        /// </summary>
        public int KthSmallestRecursive(TreeNode root, int k)
        {
            int remaining = k;
            int? result = FindKthInorder(root, ref remaining);
            return result ?? -1;
        }

        private static int? FindKthInorder(TreeNode node, ref int remaining)
        {
            if (node == null)
                return null;

            int? leftResult = FindKthInorder(node.left, ref remaining);
            if (leftResult.HasValue)
                return leftResult;

            remaining--;
            if (remaining == 0)
                return node.val;

            return FindKthInorder(node.right, ref remaining);
        }
    }
}
