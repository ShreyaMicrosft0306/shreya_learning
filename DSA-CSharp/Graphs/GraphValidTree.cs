/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Graph Valid Tree
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
Given n nodes labeled from 0 to n - 1 and a list of undirected edges,
return true if these edges form a valid tree.

A valid tree must satisfy two conditions:
- The graph is connected.
- The graph contains no cycles.

INPUT:
- n: int - Number of nodes
- edges: int[][] - Undirected pairs [nodeA, nodeB]

OUTPUT:
- bool - true if edges form one connected graph with no cycles

EXAMPLES:
Example 1:
Input: n = 5, edges = [[0,1],[0,2],[0,3],[1,4]]
Output: true
Explanation: All nodes are connected and there are no cycles.

Example 2:
Input: n = 5, edges = [[0,1],[1,2],[2,3],[1,3],[1,4]]
Output: false
Explanation: Nodes 1, 2, and 3 form a cycle.

CONSTRAINTS:
- 1 <= n <= 2000
- 0 <= edges.Length <= 5000
- edges[i].Length == 2
- 0 <= edges[i][0], edges[i][1] < n
- There are no repeated edges

EDGE CASES:
✓ One node and no edges → valid tree
✓ One node with a self-loop → invalid
✓ Fewer than n - 1 edges → disconnected or invalid
✓ Exactly n - 1 edges but disconnected → invalid
✓ More than n - 1 edges and a cycle → invalid

TRICK CASES:
⚡ An acyclic graph with n - 1 edges is connected
⚡ Checking only edge count is not enough
⚡ If Find(a) equals Find(b), adding this edge creates a cycle
⚡ In Union-Find, every successful union combines two components

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OPTIMAL SOLUTION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APPROACH:
Use Union-Find to track connected components.

For each edge:
1. Find the representative of both endpoints.
2. If both representatives are already equal, an edge joins nodes in the
   same component, so it creates a cycle.
3. Otherwise, union the two components.

After processing all edges, a valid tree must have exactly n - 1 edges and
exactly one remaining component.

KEY INSIGHTS:
• n nodes require exactly n - 1 edges for a tree
• A cycle is formed only when an edge joins two already connected nodes
• Union-Find is faster than repeatedly traversing the graph
• One final component check verifies connectivity

ALGORITHM STEPS:
1. If n is 0 or edges.Length != n - 1, return false
2. Create a Union-Find structure with n nodes
3. Process every edge
4. If the endpoints are already in the same component, return false
5. Union the endpoints
6. If all unions finish, return true

TIME COMPLEXITY: O(n + E α(n))
- E edges are processed
- α(n) is the inverse Ackermann function for Union-Find operations

SPACE COMPLEXITY: O(n)
- Parent and rank arrays store n nodes

PATTERN: Union-Find
DIFFICULTY: Medium
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;

namespace DSA_CSharp.Graphs
{
    public class GraphValidTree
    {
        /// <summary>
        /// Uses BFS to check connectivity and detect cycles.
        /// Time: O(n + E), Space: O(n)
        /// </summary>
        public bool ValidTreeBfs(int n, int[][] edges)
        {
            if (n <= 0)
                return false;

            // A tree with n nodes must contain exactly n - 1 edges.
            if (edges == null || edges.Length != n - 1)
                return false;

            var adjacency = new List<int>[n];
            for (int i = 0; i < n; i++)
                adjacency[i] = new List<int>();

            foreach (int[] edge in edges)
            {
                int first = edge[0];
                int second = edge[1];

                adjacency[first].Add(second);
                adjacency[second].Add(first);
            }

            var queue = new Queue<int>();
            var visited = new bool[n];
            var parent = new int[n];

            Array.Fill(parent, -1);
            queue.Enqueue(0);
            visited[0] = true;

            while (queue.Count > 0)
            {
                int node = queue.Dequeue();

                foreach (int neighbor in adjacency[node])
                {
                    if (!visited[neighbor])
                    {
                        visited[neighbor] = true;
                        parent[neighbor] = node;
                        queue.Enqueue(neighbor);
                    }
                    else if (neighbor != parent[node])
                    {
                        // This edge points to a visited node that is not the
                        // node we came from, so the graph contains a cycle.
                        return false;
                    }
                }
            }

            // Every node must be reachable from node 0.
            for (int i = 0; i < n; i++)
            {
                if (!visited[i])
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Returns whether n nodes and edges form a valid tree.
        /// Time: O(n + E α(n)), Space: O(n)
        /// </summary>
        public bool ValidTree(int n, int[][] edges)
        {
            if (n <= 0)
                return false;

            // A tree with n nodes must contain exactly n - 1 edges.
            if (edges == null || edges.Length != n - 1)
                return false;

            var unionFind = new UnionFind(n);

            foreach (int[] edge in edges)
            {
                int first = edge[0];
                int second = edge[1];

                // Both nodes already belong to the same component: cycle found.
                if (!unionFind.Union(first, second))
                    return false;
            }

            // n - 1 successful unions leave exactly one component.
            return unionFind.ComponentCount == 1;
        }

        private sealed class UnionFind
        {
            private readonly int[] parent;
            private readonly int[] rank;

            public int ComponentCount { get; private set; }

            public UnionFind(int size)
            {
                parent = new int[size];
                rank = new int[size];
                ComponentCount = size;

                for (int i = 0; i < size; i++)
                    parent[i] = i;
            }

            public int Find(int node)
            {
                if (parent[node] != node)
                    parent[node] = Find(parent[node]);

                return parent[node];
            }

            public bool Union(int first, int second)
            {
                int firstRoot = Find(first);
                int secondRoot = Find(second);

                if (firstRoot == secondRoot)
                    return false;

                if (rank[firstRoot] < rank[secondRoot])
                {
                    parent[firstRoot] = secondRoot;
                }
                else if (rank[firstRoot] > rank[secondRoot])
                {
                    parent[secondRoot] = firstRoot;
                }
                else
                {
                    parent[secondRoot] = firstRoot;
                    rank[firstRoot]++;
                }

                ComponentCount--;
                return true;
            }
        }

        public static void Test()
        {
            var solution = new GraphValidTree();

            Console.WriteLine("=== Graph Valid Tree Test Cases ===\n");

            int n1 = 5;
            int[][] edges1 =
            {
                new[] { 0, 1 },
                new[] { 0, 2 },
                new[] { 0, 3 },
                new[] { 1, 4 }
            };
            Console.WriteLine($"Example 1 - BFS: {solution.ValidTreeBfs(n1, edges1)}");
            Console.WriteLine($"Example 1 - Union-Find: {solution.ValidTree(n1, edges1)}");
            Console.WriteLine("Expected: true\n");

            int n2 = 5;
            int[][] edges2 =
            {
                new[] { 0, 1 },
                new[] { 1, 2 },
                new[] { 2, 3 },
                new[] { 1, 3 },
                new[] { 1, 4 }
            };
            Console.WriteLine($"Example 2 - BFS: {solution.ValidTreeBfs(n2, edges2)}");
            Console.WriteLine($"Example 2 - Union-Find: {solution.ValidTree(n2, edges2)}");
            Console.WriteLine("Expected: false\n");

            Console.WriteLine($"Single node: {solution.ValidTree(1, Array.Empty<int[]>())}");
            Console.WriteLine("Expected: true\n");

            int[][] disconnectedEdges =
            {
                new[] { 0, 1 },
                new[] { 2, 3 }
            };
            Console.WriteLine($"Disconnected graph: {solution.ValidTree(4, disconnectedEdges)}");
            Console.WriteLine("Expected: false\n");

            int[][] selfLoop =
            {
                new[] { 0, 0 }
            };
            Console.WriteLine($"Self-loop: {solution.ValidTree(1, selfLoop)}");
            Console.WriteLine("Expected: false");
        }
    }
}
