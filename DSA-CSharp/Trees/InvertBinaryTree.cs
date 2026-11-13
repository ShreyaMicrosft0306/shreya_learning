using System;

namespace DSA_CSharp.Trees
{
    // ============================================================
    // PROBLEM: Invert Binary Tree
    // ============================================================
    // Given the root of a binary tree, invert the tree and return its root.
    //
    // EXAMPLE:
    // Input: root = [4,2,7,1,3,6,9]
    // Output: [4,7,2,9,6,3,1]
    //
    // APPROACH:
    // Recursively swap the left and right children of every node.
    // The inversion of a subtree is independent of the rest of the tree,
    // so we solve the same problem on the left and right children.
    //
    // TIME COMPLEXITY: O(n)
    // SPACE COMPLEXITY: O(h) recursion stack, where h is the tree height
    // ============================================================
    public class InvertBinaryTree
    {
        public TreeNode? InvertTree(TreeNode? root)
        {
            if (root == null)
            {
                return null;
            }

            TreeNode? left = root.left;
            TreeNode? right = root.right;

            root.left = InvertTree(right);
            root.right = InvertTree(left);

            return root;
        }

        public static void Test()
        {
            var solution = new InvertBinaryTree();

            Console.WriteLine("=== Invert Binary Tree Test Cases ===\n");

            TreeNode root1 = new TreeNode(4,
                new TreeNode(2,
                    new TreeNode(1),
                    new TreeNode(3)),
                new TreeNode(7,
                    new TreeNode(6),
                    new TreeNode(9)));

            TreeNode inverted1 = solution.InvertTree(root1);
            Console.WriteLine($"Test 1 Output Root: {inverted1.val}");
            Console.WriteLine("Expected: 4\n");

            TreeNode root2 = new TreeNode(2,
                new TreeNode(1),
                new TreeNode(3));
            TreeNode inverted2 = solution.InvertTree(root2);
            Console.WriteLine($"Test 2 Output Root: {inverted2.val}");
            Console.WriteLine("Expected: 2\n");

            TreeNode root3 = null;
            TreeNode inverted3 = solution.InvertTree(root3);
            Console.WriteLine($"Test 3 Output: {(inverted3 == null ? "null" : inverted3.val.ToString())}");
            Console.WriteLine("Expected: null");
        }
    }
}
