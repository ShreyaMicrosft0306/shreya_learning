using System;

namespace DSA_CSharp.Trees
{
    // ============================================================
    // PROBLEM: Validate Binary Search Tree (BST)
    // ============================================================
    // Given the root of a binary tree, determine whether it is a valid BST.
    //
    // A valid BST must satisfy:
    // - left subtree values < root value
    // - right subtree values > root value
    // - both subtrees must also be valid BSTs
    //
    // EXAMPLE:
    // Input: root = [2,1,3]
    // Output: true
    //
    // APPROACH:
    // Recursively check each node with allowed min/max bounds.
    // Each node must stay within a valid range:
    // - left subtree: values must be < node.Value
    // - right subtree: values must be > node.Value
    //
    // We pass boundaries down the recursion so that invalid nodes are rejected
    // as soon as they fall outside the allowed range.
    //
    // TIME COMPLEXITY: O(n)
    // SPACE COMPLEXITY: O(h) recursion stack, where h is tree height
    // ============================================================
    public class TreeNode
    {
        public int val;
        public TreeNode left;
        public TreeNode right;

        public TreeNode(int val = 0, TreeNode? left = null, TreeNode? right = null)
        {
            this.val = val;
            this.left = left;
            this.right = right;
        }
    }

    public class ValidateBinarySearchTree
    {
        public bool IsValidBST(TreeNode root)
        {
            return IsValidBST(root, null, null);
        }

        private bool IsValidBST(TreeNode node, long? min, long? max)
        {
            if (node == null)
            {
                return true;
            }

            if (min.HasValue && node.val <= min.Value)
            {
                return false;
            }

            if (max.HasValue && node.val >= max.Value)
            {
                return false;
            }

            return IsValidBST(node.left, min, node.val)
                && IsValidBST(node.right, node.val, max);
        }

        public static void Test()
        {
            var solution = new ValidateBinarySearchTree();

            Console.WriteLine("=== Validate Binary Search Tree Test Cases ===\n");

            TreeNode root1 = new TreeNode(2,
                new TreeNode(1),
                new TreeNode(3));
            Console.WriteLine($"Test 1 Output: {solution.IsValidBST(root1)}");
            Console.WriteLine("Expected: True\n");

            TreeNode root2 = new TreeNode(5,
                new TreeNode(1),
                new TreeNode(4,
                    new TreeNode(3),
                    new TreeNode(6)));
            Console.WriteLine($"Test 2 Output: {solution.IsValidBST(root2)}");
            Console.WriteLine("Expected: False\n");

            TreeNode root3 = new TreeNode(1,
                new TreeNode(1));
            Console.WriteLine($"Test 3 Output: {solution.IsValidBST(root3)}");
            Console.WriteLine("Expected: False\n");

            TreeNode root4 = new TreeNode(2,
                new TreeNode(1),
                new TreeNode(3));
            Console.WriteLine($"Test 4 Output: {solution.IsValidBST(root4)}");
            Console.WriteLine("Expected: True");
        }
    }
}
