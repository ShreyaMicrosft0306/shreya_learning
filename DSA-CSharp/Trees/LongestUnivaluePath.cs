// ============================================================
// PROBLEM: Longest Univalue Path
// ============================================================
// Given the root of a binary tree, return the number of edges in
// the longest path where every node has the same value. The path
// does not need to pass through the root.
//
// INPUT:
//   TreeNode root — root of the binary tree
//   Example: root = [1,4,5,4,4,5]
//              Tree:     1
//                       / \
//                      4   5
//                     / \   \
//                    4   4   5
//
// OUTPUT:
//   int — number of edges on the longest univalue path
//   Example: 2  (path 4→4→4 has 2 edges)
//
// EDGE CASES:
//   - Single node           → 0 edges (no path possible)
//   - All nodes same value  → longest path spans as many nodes as possible
//   - Root is not part of longest path → handled by tracking global max
//   - Left-only or right-only path → one arm contributes, other is 0
//
// ============================================================
// HOW TO SOLVE — Simple Walkthrough
// ============================================================
// Key Insight:
//   At each node, the univalue path can either:
//     (a) extend DOWN into one child only  → returned to the parent
//     (b) form a "V" through both children → updates global max, NOT returned up
//
//   DFS returns the longest arm going DOWN from a node (one direction only).
//   The path THROUGH a node (both arms) is computed locally and updates maxLen.
//
//   For each node:
//     leftLen  = DFS(left)  + 1  if left child has same value, else 0
//     rightLen = DFS(right) + 1  if right child has same value, else 0
//     maxLen   = max(maxLen, leftLen + rightLen)   ← "V" path through this node
//     return     max(leftLen, rightLen)             ← best single arm to parent
//
// Step-by-step with example: target tree above
//
//   DFS(4, bottom-left leaf):  no children → return 0
//   DFS(4, bottom-right leaf): no children → return 0
//
//   DFS(4, level-1 left):
//     left  child val=4 == 4 → leftLen  = 0+1 = 1
//     right child val=4 == 4 → rightLen = 0+1 = 1
//     maxLen = max(0, 1+1) = 2
//     return max(1,1) = 1   (best single arm to parent)
//
//   DFS(5, bottom-right leaf): no children → return 0
//
//   DFS(5, level-1 right):
//     left  child = null      → leftLen  = 0
//     right child val=5 == 5  → rightLen = 0+1 = 1
//     maxLen = max(2, 0+1) = 2
//     return max(0,1) = 1
//
//   DFS(1, root):
//     left  child val=4 ≠ 1  → leftLen  = 0
//     right child val=5 ≠ 1  → rightLen = 0
//     maxLen = max(2, 0+0) = 2
//     return 0
//
//   Result: 2  ✓
//
// WHY RETURN ONLY ONE ARM TO THE PARENT?
//   The parent can only extend the path in ONE direction through this node.
//   If we returned both arms combined, the parent would "double count" the path
//   and break the rule that a path cannot branch at two points.
//
// WHY THIS APPROACH?
//   Naive: enumerate all paths and check if values match → exponential.
//   DFS with global max (this): each node is visited once, O(n) time.
//   The global max captures the "V" paths that don't propagate upward.
//
// ============================================================
// TIME & SPACE COMPLEXITY
// ============================================================
//   n = number of nodes,  h = height of tree
//
//   Time:  O(n)
//     - Each node is visited exactly once by DFS
//
//   Space: O(h)
//     - Recursion stack depth equals the height of the tree
//     - O(log n) for balanced trees, O(n) worst case (skewed tree)
// ============================================================

public class LongestUnivaluePath
{
    private static int Dfs(TreeNode node, ref int maxLen)
    {
        if (node == null) return 0;

        int left  = Dfs(node.left,  ref maxLen);
        int right = Dfs(node.right, ref maxLen);

        // Extend arm only if child has the same value
        int leftLen  = (node.left  != null && node.left.val  == node.val) ? left  + 1 : 0;
        int rightLen = (node.right != null && node.right.val == node.val) ? right + 1 : 0;

        // "V" path through this node — update global max
        maxLen = Math.Max(maxLen, leftLen + rightLen);

        // Return best single arm upward
        return Math.Max(leftLen, rightLen);
    }

    public static int Solve(TreeNode root)
    {
        int maxLen = 0;
        Dfs(root, ref maxLen);
        return maxLen;
    }

    public static void Main(string[] args)
    {
        // Tree:     1
        //          / \
        //         4   5
        //        / \   \
        //       4   4   5
        TreeNode root1 = new(1)
        {
            left  = new(4) { left = new(4), right = new(4) },
            right = new(5) { right = new(5) }
        };
        Console.WriteLine("Example 1: " + Solve(root1));
        // Expected: 2

        // Tree:     1
        //          / \
        //         1   1
        //        / \ / \
        //       1  1 1  1
        TreeNode root2 = new(1)
        {
            left  = new(1) { left = new(1), right = new(1) },
            right = new(1) { left = new(1), right = new(1) }
        };
        Console.WriteLine("Example 2: " + Solve(root2));
        // Expected: 4

        // Single node
        Console.WriteLine("Example 3: " + Solve(new TreeNode(5)));
        // Expected: 0

        // All different values
        TreeNode root4 = new(1) { left = new(2), right = new(3) };
        Console.WriteLine("Example 4: " + Solve(root4));
        // Expected: 0
    }
}
