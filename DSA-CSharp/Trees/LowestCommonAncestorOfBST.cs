/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Lowest Common Ancestor of a Binary Search Tree
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
Given a Binary Search Tree (BST) and two nodes p and q, find their lowest
common ancestor.

A node can be an ancestor of itself.

INPUT:
- root: TreeNode - Root of the BST
- p: TreeNode - First target node
- q: TreeNode - Second target node

OUTPUT:
- TreeNode - The lowest common ancestor of p and q

EXAMPLES:
Example 1:
Input: root = [6,2,8,0,4,7,9,null,null,3,5], p = 2, q = 8
          6
         / \
        2   8
       / \ / \
      0  4 7  9
        / \
       3   5
Output: 6
Explanation: 2 is on the left of 6 and 8 is on the right, so 6 is the LCA.

Example 2:
Input: root = [6,2,8,0,4,7,9,null,null,3,5], p = 2, q = 4
          6
         / \
        2   8
       / \ / \
      0  4 7  9
        / \
       3   5
Output: 2
Explanation: 2 is an ancestor of itself and is also an ancestor of 4.

Example 3:
Input: root = [2,1], p = 2, q = 1
      2
     /
    1
Output: 2

CONSTRAINTS:
- The tree has at least two nodes
- All node values are unique
- p and q are different nodes in the BST
- p and q exist in the tree

EDGE CASES:
✓ One target is the root
✓ p and q are in different subtrees
✓ p is an ancestor of q
✓ q is an ancestor of p
✓ Both targets are deep in the same subtree

TRICK CASES:
⚡ Use BST ordering instead of searching both subtrees.
⚡ A node can be the LCA when its value equals p or q.
⚡ If both values are smaller, move left; if both are larger, move right.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OPTIMAL SOLUTION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APPROACH:
At each node, compare its value with p and q:

- If both p and q are smaller, their LCA must be in the left subtree.
- If both p and q are larger, their LCA must be in the right subtree.
- Otherwise, the current node is where the paths split, so it is the LCA.

The "otherwise" case also includes when the current node equals p or q.

ALGORITHM STEPS:
1. Start at the root.
2. If both target values are smaller, move to the left child.
3. If both target values are larger, move to the right child.
4. Otherwise, return the current node.

TIME COMPLEXITY: O(h)
- h is the height of the BST.
- We move down at most one path.
- Worst case: O(n) for a skewed tree.

SPACE COMPLEXITY: O(1)
- The iterative solution uses only a few variables.

PATTERN: Binary Search Tree, Tree Traversal
DIFFICULTY: Medium
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

namespace DSA_CSharp.Trees
{
    public class LowestCommonAncestorOfBST
    {
        /// <summary>
        /// Returns the lowest common ancestor of p and q in a BST.
        /// </summary>
        public TreeNode LowestCommonAncestor(TreeNode root, TreeNode p, TreeNode q)
        {
            TreeNode current = root;

            while (current != null)
            {
                if (p.val < current.val && q.val < current.val)
                {
                    current = current.left;
                }
                else if (p.val > current.val && q.val > current.val)
                {
                    current = current.right;
                }
                else
                {
                    return current;
                }
            }

            return null;
        }
    }
}
