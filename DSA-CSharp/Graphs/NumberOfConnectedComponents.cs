/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Number of Connected Components in an Undirected Graph
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
You are given n nodes labeled from 0 to n - 1 and an undirected graph with a
list of edges. Return the number of connected components in the graph.

EXAMPLE:
Input: n = 5, edges = [[0,1],[1,2],[3,4]]
Output: 2
Explanation: {0,1,2} and {3,4} are the connected components.

APPROACH:
Use Disjoint Set Union (Union-Find).

Each node starts in its own set.
For every edge (u, v):
- union(u, v)

At the end, count how many distinct parent roots exist.

Why this works:
If two nodes are connected directly or indirectly, they will end up in the same
set after unions are processed. Every connected component becomes exactly one
root set.

TIME COMPLEXITY: O((n + edges) * α(n))
- α(n) is the inverse Ackermann function, effectively constant.

SPACE COMPLEXITY: O(n)
- One parent array and one rank/size array.

PATTERN: Union-Find / Graph Connectivity
DIFFICULTY: Medium
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;

namespace DSA_CSharp.Graphs
{
    public class NumberOfConnectedComponents
    {
        /// <summary>
        /// Returns the number of connected components in an undirected graph.
        /// Uses Union-Find (Disjoint Set Union) for nearly constant-time operations.
        /// </summary>
        public int CountComponents(int n, int[][] edges)
        {
            if (n <= 0)
            {
                return 0;
            }

            int[] parent = new int[n];
            int[] rank = new int[n];

            for (int i = 0; i < n; i++)
            {
                parent[i] = i;
            }

            for (int i = 0; i < edges.Length; i++)
            {
                Union(parent, rank, edges[i][0], edges[i][1]);
            }

            int components = 0;
            for (int i = 0; i < n; i++)
            {
                if (parent[i] == i)
                {
                    components++;
                }
            }

            return components;
        }

        private void Union(int[] parent, int[] rank, int x, int y)
        {
            int rootX = Find(parent, x);
            int rootY = Find(parent, y);

            if (rootX == rootY)
            {
                return;
            }

            if (rank[rootX] < rank[rootY])
            {
                parent[rootX] = rootY;
            }
            else if (rank[rootX] > rank[rootY])
            {
                parent[rootY] = rootX;
            }
            else
            {
                parent[rootY] = rootX;
                rank[rootX]++;
            }
        }

        private int Find(int[] parent, int x)
        {
            if (parent[x] != x)
            {
                parent[x] = Find(parent, parent[x]);
            }

            return parent[x];
        }

        /// <summary>
        /// Alternate DFS-based solution.
        /// Builds adjacency list and counts connected components by exploring each
        /// unvisited node once.
        /// Time: O(n + e), Space: O(n + e)
        /// </summary>
        public int CountComponentsDFS(int n, int[][] edges)
        {
            var adjacency = new List<int>[n];
            for (int i = 0; i < n; i++)
            {
                adjacency[i] = new List<int>();
            }

            foreach (int[] edge in edges)
            {
                int u = edge[0];
                int v = edge[1];
                adjacency[u].Add(v);
                adjacency[v].Add(u);
            }

            bool[] visited = new bool[n];
            int components = 0;

            for (int node = 0; node < n; node++)
            {
                if (!visited[node])
                {
                    components++;
                    DFS(node, adjacency, visited);
                }
            }

            return components;
        }

        private void DFS(int node, List<int>[] adjacency, bool[] visited)
        {
            visited[node] = true;

            foreach (int neighbor in adjacency[node])
            {
                if (!visited[neighbor])
                {
                    DFS(neighbor, adjacency, visited);
                }
            }
        }

        public static void Test()
        {
            var solution = new NumberOfConnectedComponents();

            Console.WriteLine("=== Number of Connected Components Test Cases ===\n");

            int n1 = 5;
            int[][] edges1 =
            {
                new[] { 0, 1 },
                new[] { 1, 2 },
                new[] { 3, 4 }
            };
            Console.WriteLine($"Test 1 Union-Find: {solution.CountComponents(n1, edges1)}");
            Console.WriteLine($"Test 1 DFS: {solution.CountComponentsDFS(n1, edges1)}");
            Console.WriteLine("Expected: 2\n");

            int n2 = 5;
            int[][] edges2 =
            {
                new[] { 0, 1 },
                new[] { 1, 2 },
                new[] { 2, 3 },
                new[] { 3, 4 }
            };
            Console.WriteLine($"Test 2 Union-Find: {solution.CountComponents(n2, edges2)}");
            Console.WriteLine($"Test 2 DFS: {solution.CountComponentsDFS(n2, edges2)}");
            Console.WriteLine("Expected: 1\n");

            int n3 = 3;
            int[][] edges3 =
            {
                new[] { 0, 1 }
            };
            Console.WriteLine($"Test 3 Union-Find: {solution.CountComponents(n3, edges3)}");
            Console.WriteLine($"Test 3 DFS: {solution.CountComponentsDFS(n3, edges3)}");
            Console.WriteLine("Expected: 2");
        }
    }
}
