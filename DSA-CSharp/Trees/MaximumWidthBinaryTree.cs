// ============================================================
// PROBLEM: Maximum Width of Binary Tree
// ============================================================
// Given the root of a binary tree, return the maximum width across
// all levels. The width of a level is the number of nodes between
// the leftmost and rightmost nodes at that level, counting null
// nodes in between.
//
// INPUT:
//   TreeNode root — root of the binary tree
//   Example: root = [4, 2, 7, 1, null, null, 9]
//              Tree:     4
//                       / \
//                      2   7
//                     /     \
//                    1       9
//
// OUTPUT:
//   int — maximum width of any level
//   Example: 4
//
// EDGE CASES:
//   - Single node          → width = 1
//   - Full binary tree     → widest level is the last
//   - Left-skewed tree     → every level width = 1
//   - Nodes on opposite ends with nulls in between → nulls counted
//
// ============================================================
// HOW TO SOLVE — Simple Walkthrough
// ============================================================
// Key Insight (BFS with column index):
//   Assign each node a column index using heap-style numbering:
//     root → 0
//     left child of index i  → 2*i
//     right child of index i → 2*i + 1
//   Width of a level = last_index - first_index + 1
//   This counts all positions including null gaps between real nodes.
//
// IMPORTANT — Normalize indices each level:
//   Deep trees produce exponentially large indices (2^depth).
//   At the start of each level, subtract the first node's index from
//   all indices on that level. This resets the leftmost node to 0
//   every level, keeping numbers small and preventing overflow.
//
// Step-by-step with example: [4, 2, 7, 1, null, null, 9]
//   Tree:      4(0)
//             /     \
//           2(0)   7(1)          ← normalized each level
//           /         \
//         1(0)        9(3)       ← normalized
//
//   Level 0: queue=[(4,idx=0)]
//     firstIdx=0. Process 4: normIdx=0-0=0, push (2,0),(7,1)
//     width = 0+1 = 1. maxWidth=1.
//
//   Level 1: queue=[(2,0),(7,1)]
//     firstIdx=0. Process 2: normIdx=0, push (1,0). right=null skip.
//                 Process 7: normIdx=1. left=null skip. push (9,3)
//     width = 1+1 = 2. maxWidth=2.
//
//   Level 2: queue=[(1,0),(9,3)]
//     firstIdx=0. Process 1: normIdx=0. No children.
//                 Process 9: normIdx=3. No children. lastIdx=3.
//     width = 3+1 = 4. maxWidth=4.  ✓
//
// WHY NORMALIZE?
//   Without normalization, a right-skewed tree 100 levels deep produces
//   index 2^100 — overflows even a long. Normalizing resets to 0 each
//   level: the relative gap between leftmost and rightmost is preserved,
//   but the absolute values stay small.
//
// WHY THIS APPROACH?
//   Naive: store all nodes per level including nulls in a list → exponential
//   space for deep sparse trees. BFS with index arithmetic (this): O(n) space
//   since only real nodes are in the queue, and indices track gaps virtually.
//
// ============================================================
// TIME & SPACE COMPLEXITY
// ============================================================
//   n = number of nodes in the tree
//
//   Time:  O(n)
//     - Each node is enqueued and dequeued exactly once
//
//   Space: O(n)
//     - Queue holds at most one full level at a time
//     - Widest level has at most n/2 nodes (last level of full tree)
// ============================================================

public class MaximumWidthBinaryTree
{
    public static int Solve(TreeNode root)
    {
        if (root == null) return 0;

        var queue = new Queue<(TreeNode node, long idx)>();
        queue.Enqueue((root, 0));
        int maxWidth = 0;

        while (queue.Count > 0)
        {
            int levelSize = queue.Count;
            long firstIdx = queue.Peek().idx;
            long lastIdx  = firstIdx;

            for (int i = 0; i < levelSize; i++)
            {
                var (node, idx) = queue.Dequeue();
                long normIdx = idx - firstIdx;   // normalize to prevent overflow
                lastIdx = normIdx;

                if (node.left  != null) queue.Enqueue((node.left,  normIdx * 2));
                if (node.right != null) queue.Enqueue((node.right, normIdx * 2 + 1));
            }

            maxWidth = Math.Max(maxWidth, (int)(lastIdx + 1));
        }

        return maxWidth;
    }

    public static void Main(string[] args)
    {
        // Example 1:     4
        //               / \
        //              2   7
        //             /     \
        //            1       9
        TreeNode root1 = new(4)
        {
            left  = new(2) { left = new(1) },
            right = new(7) { right = new(9) }
        };
        Console.WriteLine("Example 1: " + Solve(root1));
        // Expected: 4

        // Example 2:    4
        //              / \
        //             2   7
        //            /
        //           1
        TreeNode root2 = new(4)
        {
            left  = new(2) { left = new(1) },
            right = new(7)
        };
        Console.WriteLine("Example 2: " + Solve(root2));
        // Expected: 2

        // Single node
        Console.WriteLine("Example 3: " + Solve(new TreeNode(1)));
        // Expected: 1

        // Example 3 from problem: width 7
        TreeNode root4 = new(4)
        {
            left = new(2)
            {
                left  = new(1) { left = new(7) },
                right = null
            },
            right = new(7)
            {
                left  = new(6) { left = new(1), right = new(1) },
                right = new(9) { left = null }
            }
        };
        Console.WriteLine("Example 4: " + Solve(root4));
        // Expected: 7
    }
}
