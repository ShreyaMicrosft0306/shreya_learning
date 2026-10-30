// ============================================================
// PROBLEM: Path Sum II
// ============================================================
// Given the root of a binary tree and an integer target, find all
// root-to-leaf paths where the sum of node values equals target.
//
// INPUT:
//   TreeNode root — root of the binary tree
//   int target    — required path sum
//   Example: root = [1,2,4,4,7,5,1],  target = 10
//              Tree:     1
//                       / \
//                      2   4
//                     / \ / \
//                    4  7 5  1
//
// OUTPUT:
//   List<List<int>> — all root-to-leaf paths that sum to target
//   Example: [[1,2,7],[1,4,5]]
//
// EDGE CASES:
//   - Empty tree (root = null)           → return []
//   - Single node equals target          → return [[node.val]]
//   - Single node not equal to target    → return []
//   - Target is negative, tree has negative values → handled naturally
//
// ============================================================
// HOW TO SOLVE — Simple Walkthrough
// ============================================================
// Key Insight (DFS with Backtracking):
//   Walk every root-to-leaf path using DFS. Carry the current path
//   as a list. When a leaf is reached and remaining == 0, the path
//   is valid — copy it into results. Backtrack by removing the node
//   before returning, so the path list is reused across all branches.
//
// Step-by-step with example: target = 10
//   Tree:     1
//            / \
//           2   4
//          / \ / \
//         4  7 5  1
//
//   DFS(1, rem=10, path=[]):
//     add 1 → path=[1], rem=9
//     → DFS(2, rem=9, path=[1]):
//         add 2 → path=[1,2], rem=7
//         → DFS(4, rem=7, path=[1,2]):
//             add 4 → path=[1,2,4], rem=3
//             leaf, rem=3 ≠ 0 → not valid
//             backtrack → path=[1,2]
//         → DFS(7, rem=7, path=[1,2]):
//             add 7 → path=[1,2,7], rem=0
//             leaf, rem=0 → ADD [1,2,7] ✓
//             backtrack → path=[1,2]
//         backtrack → path=[1]
//     → DFS(4, rem=9, path=[1]):
//         add 4 → path=[1,4], rem=5
//         → DFS(5, rem=5, path=[1,4]):
//             add 5 → path=[1,4,5], rem=0
//             leaf, rem=0 → ADD [1,4,5] ✓
//             backtrack → path=[1,4]
//         → DFS(1, rem=5, path=[1,4]):
//             add 1 → path=[1,4,1], rem=4
//             leaf, rem=4 ≠ 0 → not valid
//             backtrack → path=[1,4]
//         backtrack → path=[1]
//     backtrack → path=[]
//
//   Result: [[1,2,7],[1,4,5]]  ✓
//
// WHY COPY THE PATH WHEN ADDING TO RESULTS?
//   The path list is shared and mutated throughout the DFS. Adding
//   the reference directly would give stale/empty results as backtracking
//   removes elements. new List<int>(path) captures a snapshot.
//
// WHY BACKTRACK?
//   The single path list is reused for every branch. Removing the current
//   node before returning restores the list to its state before this call,
//   so the parent can explore the next branch with the correct path.
//
// WHY THIS APPROACH?
//   Generating all paths as new lists: O(n * h) space per call frame — wasteful.
//   DFS with backtracking (this): one shared list, O(h) space for the path,
//   O(h) recursion stack depth → space-efficient.
//
// ============================================================
// TIME & SPACE COMPLEXITY
// ============================================================
//   n = number of nodes,  h = height of tree
//
//   Time:  O(n * h)
//     - Every node is visited exactly once: O(n)
//     - Copying the path at each valid leaf takes O(h): O(n * h) total
//
//   Space: O(h)
//     - Recursion stack depth: O(h)
//     - Shared path list holds at most h nodes at a time: O(h)
//     - Output list not counted in auxiliary space
// ============================================================

public class TreeNode
{
    public int val;
    public TreeNode left, right;
    public TreeNode(int val) { this.val = val; }
}

public class PathSumII
{
    private static void Dfs(
        TreeNode node,
        int remaining,
        List<int> path,
        List<List<int>> result)
    {
        if (node == null) return;

        path.Add(node.val);
        remaining -= node.val;

        // Leaf node with correct sum — valid path found
        if (node.left == null && node.right == null && remaining == 0)
            result.Add(new List<int>(path));    // snapshot, not reference

        Dfs(node.left,  remaining, path, result);
        Dfs(node.right, remaining, path, result);

        path.RemoveAt(path.Count - 1);          // backtrack
    }

    public static List<List<int>> Solve(TreeNode root, int target)
    {
        var result = new List<List<int>>();
        Dfs(root, target, new List<int>(), result);
        return result;
    }

    public static void Main(string[] args)
    {
        static string Format(List<List<int>> paths) =>
            "[" + string.Join(", ", paths.Select(p => "[" + string.Join(",", p) + "]")) + "]";

        // Tree:     1
        //          / \
        //         2   4
        //        / \ / \
        //       4  7 5  1
        TreeNode root1 = new(1)
        {
            left  = new(2) { left = new(4), right = new(7) },
            right = new(4) { left = new(5), right = new(1) }
        };
        Console.WriteLine("Example 1: " + Format(Solve(root1, 10)));
        // Expected: [[1,2,7],[1,4,5]]

        // Single node equals target
        Console.WriteLine("Example 2: " + Format(Solve(new TreeNode(5), 5)));
        // Expected: [[5]]

        // Single node not equal to target
        Console.WriteLine("Example 3: " + Format(Solve(new TreeNode(5), 3)));
        // Expected: []

        // Empty tree
        Console.WriteLine("Example 4: " + Format(Solve(null, 10)));
        // Expected: []
    }
}
