/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Subtree of Another Tree
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
Given the roots of two binary trees, root and subRoot, return true if there
exists a subtree of root with the same structure and node values as subRoot.

A subtree of a tree is a node and all of its descendants. A tree can also be
considered a subtree of itself.

EXAMPLE:
Input: root = [3,4,5,1,2], subRoot = [4,1,2]
Output: true

APPROACH:
We check every node in root. For each node, we ask: does this node and all of
its descendants exactly match subRoot?

This is done in two parts:
1. DFS through root looking for a node whose value matches subRoot.root.
2. When the value matches, compare the two subtrees recursively.

If both trees are identical at the node, return true.

WHY THIS WORKS:
Any matching subtree must start at some node in root. So checking every node in
root is sufficient. Once a matching root value is found, the only remaining
question is whether the full structure and values match exactly.

TIME COMPLEXITY: O(n * m) in the worst case
- n = number of nodes in root
- m = number of nodes in subRoot
- We may compare multiple candidate nodes.

With a good pruning strategy, this is usually much faster in practice.

SPACE COMPLEXITY: O(h + k)
- Recursion stack for root and subRoot
- h and k are the tree heights.

PATTERN: Tree Traversal + Recursive Comparison
DIFFICULTY: Easy
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;

namespace DSA_CSharp.Trees
{
    public class TreeNode
    {
        public int val;
        public TreeNode? left;
        public TreeNode? right;

        public TreeNode(int val = 0, TreeNode? left = null, TreeNode? right = null)
        {
            this.val = val;
            this.left = left;
            this.right = right;
        }
    }

    public class SubtreeOfAnotherTree
    {
        /// <summary>
        /// Returns true if subRoot is a subtree of root.
        /// </summary>
        public bool IsSubtree(TreeNode? root, TreeNode? subRoot)
        {
            if (subRoot == null)
            {
                return true;
            }

            if (root == null)
            {
                return false;
            }

            if (IsSameTree(root, subRoot))
            {
                return true;
            }

            return IsSubtree(root.left, subRoot) || IsSubtree(root.right, subRoot);
        }

        private bool IsSameTree(TreeNode? a, TreeNode? b)
        {
            if (a == null && b == null)
            {
                return true;
            }

            if (a == null || b == null)
            {
                return false;
            }

            if (a.val != b.val)
            {
                return false;
            }

            return IsSameTree(a.left, b.left) && IsSameTree(a.right, b.right);
        }

        public static void Test()
        {
            var solution = new SubtreeOfAnotherTree();

            Console.WriteLine("=== Subtree of Another Tree Test Cases ===\n");

            TreeNode root1 = new TreeNode(3)
            {
                left = new TreeNode(4)
                {
                    left = new TreeNode(1),
                    right = new TreeNode(2)
                },
                right = new TreeNode(5)
            };

            TreeNode subRoot1 = new TreeNode(4)
            {
                left = new TreeNode(1),
                right = new TreeNode(2)
            };

            Console.WriteLine($"Test 1 Output: {solution.IsSubtree(root1, subRoot1)}");
            Console.WriteLine("Expected: True\n");

            TreeNode root2 = new TreeNode(3)
            {
                left = new TreeNode(4)
                {
                    left = new TreeNode(1),
                    right = new TreeNode(2)
                },
                right = new TreeNode(5)
            };

            TreeNode subRoot2 = new TreeNode(4)
            {
                left = new TreeNode(1),
                right = new TreeNode(2)
            };

            root2.left.right.right = new TreeNode(0);

            Console.WriteLine($"Test 2 Output: {solution.IsSubtree(root2, subRoot2)}");
            Console.WriteLine("Expected: False");
        }
    }
}
