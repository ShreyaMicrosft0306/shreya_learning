/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Binary Tree Traversals
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
Return the values of a binary tree in inorder, preorder, and postorder order.

For this tree:

        1
       / \
      2   3
     / \
    4   5

INPUT:
- root: TreeNode - Root of the binary tree

OUTPUT:
- IList<int> - Values in the requested traversal order

EXAMPLES:
Example 1:
Input: root = [1, 2, 3, 4, 5]
Inorder:   [4, 2, 5, 1, 3]
Preorder:  [1, 2, 4, 5, 3]
Postorder: [4, 5, 2, 3, 1]

Example 2:
Input: root = [1, null, 2, null, 3]
    1
     \
      2
       \
        3
Inorder:   [1, 2, 3]
Preorder:  [1, 2, 3]
Postorder: [3, 2, 1]

Example 3:
Input: root = null
    (empty tree)
Output: [] for all three traversals

CONSTRAINTS:
- 0 <= number of nodes <= 10^4
- Node values are integers

EDGE CASES:
✓ Empty tree -> empty list
✓ Single node -> [node value]
✓ Only left children
✓ Only right children
✓ Complete binary tree

TRICK CASES:
⚡ Inorder is left -> root -> right.
⚡ Preorder is root -> left -> right.
⚡ Postorder is left -> right -> root.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OPTIMAL SOLUTION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APPROACH:
Use recursive depth-first traversal. The only difference between the three
methods is when the current node is added to the result:

Inorder:   visit left, add root, visit right
Preorder:  add root, visit left, visit right
Postorder: visit left, visit right, add root

TIME COMPLEXITY: O(n)
- Every node is visited once.

SPACE COMPLEXITY: O(h)
- The recursion stack can contain one path of height h.
- The result list is not counted as auxiliary space.

PATTERN: DFS, Tree Traversal
DIFFICULTY: Easy
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System.Collections.Generic;

namespace DSA_CSharp.Trees
{
    public class BinaryTreeTraversals
    {
        /// <summary>
        /// Visits left subtree, current node, then right subtree.
        /// </summary>
        public IList<int> Inorder(TreeNode root)
        {
            List<int> result = new List<int>();
            BuildInorder(root, result);
            return result;
        }

        /// <summary>
        /// Visits current node, left subtree, then right subtree.
        /// </summary>
        public IList<int> Preorder(TreeNode root)
        {
            List<int> result = new List<int>();
            BuildPreorder(root, result);
            return result;
        }

        /// <summary>
        /// Visits left subtree, right subtree, then current node.
        /// </summary>
        public IList<int> Postorder(TreeNode root)
        {
            List<int> result = new List<int>();
            BuildPostorder(root, result);
            return result;
        }

        private static void BuildInorder(TreeNode node, IList<int> result)
        {
            if (node == null)
                return;

            BuildInorder(node.left, result);
            result.Add(node.val);
            BuildInorder(node.right, result);
        }

        private static void BuildPreorder(TreeNode node, IList<int> result)
        {
            if (node == null)
                return;

            result.Add(node.val);
            BuildPreorder(node.left, result);
            BuildPreorder(node.right, result);
        }

        private static void BuildPostorder(TreeNode node, IList<int> result)
        {
            if (node == null)
                return;

            BuildPostorder(node.left, result);
            BuildPostorder(node.right, result);
            result.Add(node.val);
        }
    }
}
