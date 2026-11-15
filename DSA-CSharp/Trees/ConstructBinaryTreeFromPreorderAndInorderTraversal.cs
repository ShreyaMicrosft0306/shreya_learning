/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Construct Binary Tree from Preorder and Inorder Traversal
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
Given the preorder and inorder traversals of the same binary tree, construct
and return the original tree.

Preorder order:
  root -> left -> right

Inorder order:
  left -> root -> right

INPUT:
- int[] preorder - Preorder traversal of the tree
- int[] inorder  - Inorder traversal of the tree

OUTPUT:
- TreeNode - Root of the reconstructed binary tree

EXAMPLES:
Example 1:
Input:
  preorder = [3, 9, 20, 15, 7]
  inorder  = [9, 3, 15, 20, 7]

        3
       / \
      9  20
         / \
        15  7

Output: [3, 9, 20, null, null, 15, 7]

Example 2:
Input:
  preorder = [-1]
  inorder  = [-1]

    -1

Output: [-1]

CONSTRAINTS:
- preorder and inorder have the same length
- Values are unique
- Every inorder value appears in preorder
- 1 <= number of nodes <= 3000

EDGE CASES:
✓ One-node tree
✓ Completely left-skewed tree
✓ Completely right-skewed tree
✓ A balanced tree

TRICK CASES:
⚡ The first preorder value is always the root of the current subtree.
⚡ The root's position in inorder separates left and right subtrees.
⚡ Use a map so finding the root in inorder takes O(1) time.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OPTIMAL SOLUTION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APPROACH:
Use preorder to find the root and inorder to divide the tree.

For the example:
preorder = [3, 9, 20, 15, 7]
  inorder  = [9, 3, 15, 20, 7]
1. Preorder starts with 3, so 3 is the root.
2. In inorder, 3 divides the values:
   - Left subtree: [9]
   - Right subtree: [15, 20, 7]
3. The next preorder value builds the left subtree first, then the right.
4. Repeat the same process recursively.

Store every inorder value's index in a dictionary. This avoids scanning
inorder repeatedly.

ALGORITHM STEPS:
1. Map each value to its index in inorder.
2. Keep a pointer at the next unused preorder value.
3. Take that preorder value as the current root.
4. Use its inorder index to create the left subtree range and right subtree
   range.
5. Recursively build the left and right subtrees.
6. Return the root.

TIME COMPLEXITY: O(n)
- Every node is created once.
- Each inorder index lookup takes O(1).

SPACE COMPLEXITY: O(n)
- The dictionary stores n values.
- Recursion uses O(h) stack space.

PATTERN: DFS, Recursion, Hash Map
DIFFICULTY: Medium
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System.Collections.Generic;

namespace DSA_CSharp.Trees
{
    public class ConstructBinaryTreeFromPreorderAndInorderTraversal
    {
        private readonly Dictionary<int, int> inorderIndex = new Dictionary<int, int>();
        private int preorderIndex;

        /// <summary>
        /// Reconstructs a binary tree from preorder and inorder traversals.
        /// </summary>
        public TreeNode BuildTree(int[] preorder, int[] inorder)
        {
            inorderIndex.Clear();
            preorderIndex = 0;

            for (int i = 0; i < inorder.Length; i++)
                inorderIndex[inorder[i]] = i;

            return BuildSubtree(preorder, 0, inorder.Length - 1);
        }
  preorder = [3, 9, 20, 15, 7]
  inorder  = [9, 3, 15, 20, 7]

        3
       / \
      9  20
         / \
        15  7

        inorderindex={ 9: 0, 3: 1, 15: 2, 20: 3, 7: 4 }

rootval=preorder[preorderindex]

Output: [3, 9, 20, null, null, 15, 7]
        private TreeNode BuildSubtree(int[] preorder, int left, int right)
        {
            if (left > right)
                return null;

            int rootValue = preorder[preorderIndex++];
            TreeNode root = new TreeNode(rootValue);
            int rootIndex = inorderIndex[rootValue];

            root.left = BuildSubtree(preorder, left, rootIndex - 1);
            root.right = BuildSubtree(preorder, rootIndex + 1, right);

            return root;
        }
    }
}
