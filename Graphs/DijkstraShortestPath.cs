/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Dijkstra's Shortest Path Algorithm (Non-Negative Weights)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
You are given a weighted graph represented as an adjacency list, where each edge 
has a non-negative weight. The graph has n nodes numbered from 0 to n-1.

Given a starting node (source), implement Dijkstra's algorithm to find the 
shortest path from the source to all other nodes in the graph.

Return an array where result[i] represents the shortest distance from source to 
node i. If node i is unreachable from source, return -1 for that position.

IMPORTANT: This algorithm ONLY works with NON-NEGATIVE edge weights!
For negative weights, use Bellman-Ford algorithm instead.

INPUT:
- n: int - Number of nodes in graph (0 to n-1)
- edges: int[][] - Array of [from, to, weight] representing directed edges
- source: int - Starting node
- 1 <= n <= 1000
- 0 <= edges.length <= n * (n - 1)
- edges[i].length == 3
- 0 <= from_i, to_i < n
- 0 <= weight_i <= 1000
- All edge weights are non-negative
- Graph may have cycles
- Graph may be disconnected

OUTPUT:
- int[] - Array of shortest distances from source to each node
- result[i] = shortest distance from source to node i
- result[i] = -1 if node i is unreachable

EXAMPLES:
Example 1:
Input: 
  n = 5
  edges = [[0,1,10], [0,4,5], [1,2,1], [1,4,2], [2,3,4], [3,2,6], [3,0,7], [4,1,3], [4,2,9], [4,3,2]]
  source = 0

Graph visualization:
        10
    0 ─────→ 1
    │    ↗ ↙ │ ↘
   5│   3  2  │  1
    ↓ ↙      ↓   ↓
    4 ────→ 2 ← 3
      9  ↘ ↑  6↙
          2  4

Output: [0, 8, 9, 7, 5]
Explanation:
  - Node 0 to 0: 0 (source)
  - Node 0 to 1: 0→4→1 = 5+3 = 8
  - Node 0 to 2: 0→4→1→2 = 5+3+1 = 9
  - Node 0 to 3: 0→4→3 = 5+2 = 7
  - Node 0 to 4: 0→4 = 5

Example 2:
Input:
  n = 3
  edges = [[0,1,5], [1,2,3]]
  source = 0
  
Graph: 0 ─5→ 1 ─3→ 2

Output: [0, 5, 8]
Explanation:
  - 0→0 = 0
  - 0→1 = 5
  - 0→1→2 = 5+3 = 8

Example 3:
Input:
  n = 4
  edges = [[0,1,1], [2,3,1]]
  source = 0
  
Graph: 0→1    2→3 (disconnected)

Output: [0, 1, -1, -1]
Explanation: Nodes 2 and 3 are unreachable from source 0

Example 4:
Input:
  n = 3
  edges = [[0,1,2], [1,2,3], [0,2,10]]
  source = 0
  
Graph: 0 ─2→ 1 ─3→ 2
       └─────10─────┘

Output: [0, 2, 5]
Explanation: 
  - Direct path 0→2 costs 10
  - Better path 0→1→2 costs 2+3 = 5
  - Dijkstra finds the optimal path!

CONSTRAINTS:
- 1 <= n <= 1000
- 0 <= edges.length <= n * (n - 1)
- All edge weights are NON-NEGATIVE (>= 0)
- Graph can be directed or undirected
- Graph may contain cycles
- Self-loops are allowed but typically ignored

EDGE CASES:
✓ Source node has no outgoing edges
✓ Disconnected graph (some nodes unreachable)
✓ Single node (source is the only node)
✓ All nodes connected with uniform weights
✓ Multiple paths to same node (pick shortest)
✓ Cycles in graph (algorithm handles correctly)
✓ Self-loops (typically ignored or weight added)

TRICK CASES:
⚡ MUST use priority queue (min-heap) for optimal O((V+E)logV) time
⚡ Without priority queue, becomes O(V²) - too slow for large graphs
⚡ Mark nodes as "visited" to avoid reprocessing
⚡ Process nodes in order of shortest distance from source
⚡ Greedy choice: always expand closest unvisited node
⚡ Relaxation: Update distance if shorter path found
⚡ Cannot handle NEGATIVE weights (use Bellman-Ford instead)

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
DIJKSTRA'S ALGORITHM EXPLAINED
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

THE BIG IDEA:
Dijkstra's algorithm is a GREEDY algorithm that finds shortest paths by always
expanding the closest unvisited node. Think of it like a "spreading wave" from
the source, reaching closer nodes before farther ones.

KEY PRINCIPLE:
"If I've found the shortest path to node A, and there's an edge A→B, 
 then I can potentially improve the path to B through A."

This is called RELAXATION - we "relax" the distance to B if going through A 
is shorter.

CORE COMPONENTS:

1. DISTANCE ARRAY: 
   - Tracks shortest known distance from source to each node
   - Initially: source = 0, all others = infinity
   
2. PRIORITY QUEUE (Min-Heap):
   - Stores (node, distance) pairs
   - Always gives us the closest unvisited node
   - This is what makes Dijkstra efficient!
   
3. VISITED SET:
   - Tracks processed nodes
   - Once processed, we've found the optimal path to that node
   
4. RELAXATION:
   - For each edge (u, v) with weight w:
     if (distance[u] + w < distance[v]):
         distance[v] = distance[u] + w
   - "Can I get to v cheaper through u?"

WHY IT WORKS (The Proof):
- We process nodes in order of increasing distance from source
- When we process node v, we've explored all shorter paths
- Any unprocessed node has distance >= distance[v]
- Therefore, no future discovery can improve distance[v]
- This is why it only works with NON-NEGATIVE weights!

WHY NEGATIVE WEIGHTS BREAK IT:
Consider: A ─(-5)→ B ─1→ C
         A ────6────→ C
         
- Process A (distance 0)
- Process B (distance -5 through A)
- Process C (distance 1 through B: -5+1=-4)
- But direct path A→C = 6 is processed earlier!
- Algorithm doesn't reconsider already-processed nodes
- Result: WRONG answer!

For negative weights, use Bellman-Ford algorithm instead.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
STEP-BY-STEP WALKTHROUGH
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Example: n = 5, source = 0
Edges: [[0,1,4], [0,2,1], [2,1,2], [1,3,1], [2,3,5], [3,4,3]]

Graph:
      4
  0 ───→ 1 ───→ 3 ───→ 4
  │    ↗      ↗      3
 1│   2      1
  ↓  ↙      ↙
  2 ───────5

INITIALIZATION:
  distances = [0, ∞, ∞, ∞, ∞]
  pq = [(0, 0)]  // (distance, node)
  visited = {}

────────────────────────────────────────────────────────────────────────────
ITERATION 1: Process node 0 (distance = 0)
────────────────────────────────────────────────────────────────────────────
Pop (0, 0) from pq
Mark node 0 as visited

Explore neighbors of node 0:
  - Edge 0→1 (weight 4):
    distance[1] = min(∞, 0+4) = 4
    Add (4, 1) to pq
    
  - Edge 0→2 (weight 1):
    distance[2] = min(∞, 0+1) = 1
    Add (1, 2) to pq

State after iteration 1:
  distances = [0, 4, 1, ∞, ∞]
  pq = [(1, 2), (4, 1)]  // min-heap ordered by distance
  visited = {0}

────────────────────────────────────────────────────────────────────────────
ITERATION 2: Process node 2 (distance = 1)
────────────────────────────────────────────────────────────────────────────
Pop (1, 2) from pq
Mark node 2 as visited

Explore neighbors of node 2:
  - Edge 2→1 (weight 2):
    distance[1] = min(4, 1+2) = 3  ✓ IMPROVEMENT!
    Add (3, 1) to pq
    
  - Edge 2→3 (weight 5):
    distance[3] = min(∞, 1+5) = 6
    Add (6, 3) to pq

State after iteration 2:
  distances = [0, 3, 1, 6, ∞]
  pq = [(3, 1), (4, 1), (6, 3)]
  visited = {0, 2}

────────────────────────────────────────────────────────────────────────────
ITERATION 3: Process node 1 (distance = 3)
────────────────────────────────────────────────────────────────────────────
Pop (3, 1) from pq
Mark node 1 as visited

Explore neighbors of node 1:
  - Edge 1→3 (weight 1):
    distance[3] = min(6, 3+1) = 4  ✓ IMPROVEMENT!
    Add (4, 3) to pq

State after iteration 3:
  distances = [0, 3, 1, 4, ∞]
  pq = [(4, 1), (4, 3), (6, 3)]
  visited = {0, 2, 1}

────────────────────────────────────────────────────────────────────────────
ITERATION 4: Process (4, 1) - SKIP! Already visited
────────────────────────────────────────────────────────────────────────────
Pop (4, 1) from pq
Node 1 already in visited set - skip it!
(This is an old entry with worse distance)

────────────────────────────────────────────────────────────────────────────
ITERATION 5: Process node 3 (distance = 4)
────────────────────────────────────────────────────────────────────────────
Pop (4, 3) from pq
Mark node 3 as visited

Explore neighbors of node 3:
  - Edge 3→4 (weight 3):
    distance[4] = min(∞, 4+3) = 7
    Add (7, 4) to pq

State after iteration 5:
  distances = [0, 3, 1, 4, 7]
  pq = [(6, 3), (7, 4)]
  visited = {0, 2, 1, 3}

────────────────────────────────────────────────────────────────────────────
ITERATION 6: Process (6, 3) - SKIP! Already visited
────────────────────────────────────────────────────────────────────────────

────────────────────────────────────────────────────────────────────────────
ITERATION 7: Process node 4 (distance = 7)
────────────────────────────────────────────────────────────────────────────
Pop (7, 4) from pq
Mark node 4 as visited
No outgoing edges

────────────────────────────────────────────────────────────────────────────
DONE! Priority queue is empty
────────────────────────────────────────────────────────────────────────────

FINAL ANSWER: [0, 3, 1, 4, 7]
  - 0→0: 0
  - 0→2→1: 1+2 = 3
  - 0→2: 1
  - 0→2→1→3: 1+2+1 = 4
  - 0→2→1→3→4: 1+2+1+3 = 7

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OPTIMAL SOLUTION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APPROACH:
Use Dijkstra's algorithm with a priority queue (min-heap). Greedily expand the
closest unvisited node, relaxing distances to its neighbors. The priority queue
ensures we always process the closest node next.

KEY INSIGHTS:
• Greedy approach: always process closest unvisited node
• Priority queue is essential for efficiency
• Relaxation: update distance if shorter path found
• Mark visited to avoid reprocessing
• Works ONLY with non-negative weights

ALGORITHM STEPS:
1. Initialize:
   - distances[source] = 0, all others = infinity
   - Add (0, source) to priority queue
   - visited = empty set
   
2. While priority queue is not empty:
   a. Pop node with smallest distance
   b. If already visited, skip (old entry)
   c. Mark as visited
   d. For each neighbor:
      - Calculate new distance through current node
      - If new distance < old distance:
        * Update distance array
        * Add (new distance, neighbor) to pq
        
3. Return distances array (replace infinity with -1)

TIME COMPLEXITY: O((V + E) log V)
- V = number of vertices (nodes)
- E = number of edges
- Each node added to pq at most once: O(V log V)
- Each edge processed at most once: O(E log V)
- Total: O((V + E) log V)
- With Fibonacci heap: O(E + V log V) - theoretical optimum

SPACE COMPLEXITY: O(V + E)
- Adjacency list: O(E)
- Distance array: O(V)
- Priority queue: O(V)
- Visited set: O(V)
- Total: O(V + E)

PATTERN: Greedy, Priority Queue, Graph, Shortest Path, Relaxation
DIFFICULTY: Medium
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;
using System.Collections.Generic;

namespace DSA_CSharp.Graphs
{
    public class DijkstraShortestPath
    {
        /// <summary>
        /// Finds shortest paths from source to all nodes using Dijkstra's algorithm.
        /// Only works with NON-NEGATIVE edge weights!
        /// </summary>
        /// <param name="n">Number of nodes (0 to n-1)</param>
        /// <param name="edges">Array of [from, to, weight] representing edges</param>
        /// <param name="source">Starting node</param>
        /// <returns>Array of shortest distances, or -1 if unreachable</returns>
        public int[] DijkstraShortestPaths(int n, int[][] edges, int source)
        {
            // ═══════════════════════════════════════════════════════════════
            // STEP 1: BUILD ADJACENCY LIST
            // ═══════════════════════════════════════════════════════════════
            // adjacency[u] = list of (neighbor, weight) pairs
            
            List<(int neighbor, int weight)>[] adjacency = 
                new List<(int, int)>[n];
            
            for (int i = 0; i < n; i++)
            {
                adjacency[i] = new List<(int, int)>();
            }
            
            foreach (var edge in edges)
            {
                int from = edge[0];
                int to = edge[1];
                int weight = edge[2];
                adjacency[from].Add((to, weight));
            }
            
            // ═══════════════════════════════════════════════════════════════
            // STEP 2: INITIALIZE DATA STRUCTURES
            // ═══════════════════════════════════════════════════════════════
            
            // Distance array: shortest known distance to each node
            int[] distances = new int[n];
            Array.Fill(distances, int.MaxValue);  // Initialize to infinity
            distances[source] = 0;  // Distance to source is 0
            
            // Priority Queue: (distance, node)
            // C# doesn't have built-in min-heap, so we use SortedSet
            // Or implement our own PriorityQueue (see helper class below)
            var pq = new PriorityQueue<int, int>();
            pq.Enqueue(source, 0);  // (node, distance)
            
            // Visited set: tracks processed nodes
            HashSet<int> visited = new HashSet<int>();
            
            // ═══════════════════════════════════════════════════════════════
            // STEP 3: DIJKSTRA'S MAIN LOOP
            // ═══════════════════════════════════════════════════════════════
            
            while (pq.Count > 0)
            {
                // Get node with smallest distance
                int currentNode = pq.Dequeue();
                int currentDistance = distances[currentNode];
                
                // Skip if already processed (old entry with worse distance)
                if (visited.Contains(currentNode))
                    continue;
                
                // Mark as visited - we've found optimal path to this node!
                visited.Add(currentNode);
                
                // ───────────────────────────────────────────────────────────
                // RELAXATION: Try to improve distances to neighbors
                // ───────────────────────────────────────────────────────────
                
                foreach (var (neighbor, weight) in adjacency[currentNode])
                {
                    // Skip if neighbor already processed
                    if (visited.Contains(neighbor))
                        continue;
                    
                    // Calculate distance through current node
                    int newDistance = currentDistance + weight;
                    
                    // If this path is shorter, update!
                    if (newDistance < distances[neighbor])
                    {
                        distances[neighbor] = newDistance;
                        pq.Enqueue(neighbor, newDistance);
                    }
                }
            }
            
            // ═══════════════════════════════════════════════════════════════
            // STEP 4: CONVERT UNREACHABLE NODES TO -1
            // ═══════════════════════════════════════════════════════════════
            
            for (int i = 0; i < n; i++)
            {
                if (distances[i] == int.MaxValue)
                {
                    distances[i] = -1;  // Unreachable
                }
            }
            
            return distances;
        }
        
        /// <summary>
        /// Alternative implementation returning the actual paths (not just distances).
        /// Tracks parent nodes to reconstruct paths.
        /// </summary>
        public Dictionary<int, List<int>> DijkstraWithPaths(int n, int[][] edges, int source)
        {
            // Build adjacency list
            List<(int neighbor, int weight)>[] adjacency = 
                new List<(int, int)>[n];
            
            for (int i = 0; i < n; i++)
            {
                adjacency[i] = new List<(int, int)>();
            }
            
            foreach (var edge in edges)
            {
                adjacency[edge[0]].Add((edge[1], edge[2]));
            }
            
            // Initialize
            int[] distances = new int[n];
            int[] parent = new int[n];  // Track parent for path reconstruction
            Array.Fill(distances, int.MaxValue);
            Array.Fill(parent, -1);
            distances[source] = 0;
            
            var pq = new PriorityQueue<int, int>();
            pq.Enqueue(source, 0);
            
            HashSet<int> visited = new HashSet<int>();
            
            // Dijkstra's algorithm
            while (pq.Count > 0)
            {
                int currentNode = pq.Dequeue();
                
                if (visited.Contains(currentNode))
                    continue;
                
                visited.Add(currentNode);
                
                foreach (var (neighbor, weight) in adjacency[currentNode])
                {
                    if (visited.Contains(neighbor))
                        continue;
                    
                    int newDistance = distances[currentNode] + weight;
                    
                    if (newDistance < distances[neighbor])
                    {
                        distances[neighbor] = newDistance;
                        parent[neighbor] = currentNode;  // Track parent
                        pq.Enqueue(neighbor, newDistance);
                    }
                }
            }
            
            // Reconstruct paths
            var paths = new Dictionary<int, List<int>>();
            
            for (int i = 0; i < n; i++)
            {
                if (distances[i] == int.MaxValue)
                    continue;  // Unreachable
                
                // Build path from source to i
                List<int> path = new List<int>();
                int current = i;
                
                while (current != -1)
                {
                    path.Add(current);
                    current = parent[current];
                }
                
                path.Reverse();  // Reverse to get source → target order
                paths[i] = path;
            }
            
            return paths;
        }
        
        /// <summary>
        /// Single source to single target variant.
        /// Stops early when target is reached (optimization).
        /// </summary>
        public int DijkstraSingleTarget(int n, int[][] edges, int source, int target)
        {
            if (source == target) return 0;
            
            // Build adjacency list
            List<(int neighbor, int weight)>[] adjacency = 
                new List<(int, int)>[n];
            
            for (int i = 0; i < n; i++)
            {
                adjacency[i] = new List<(int, int)>();
            }
            
            foreach (var edge in edges)
            {
                adjacency[edge[0]].Add((edge[1], edge[2]));
            }
            
            // Initialize
            int[] distances = new int[n];
            Array.Fill(distances, int.MaxValue);
            distances[source] = 0;
            
            var pq = new PriorityQueue<int, int>();
            pq.Enqueue(source, 0);
            
            HashSet<int> visited = new HashSet<int>();
            
            // Dijkstra's algorithm with early termination
            while (pq.Count > 0)
            {
                int currentNode = pq.Dequeue();
                
                // Early termination: found target!
                if (currentNode == target)
                    return distances[target];
                
                if (visited.Contains(currentNode))
                    continue;
                
                visited.Add(currentNode);
                
                foreach (var (neighbor, weight) in adjacency[currentNode])
                {
                    if (visited.Contains(neighbor))
                        continue;
                    
                    int newDistance = distances[currentNode] + weight;
                    
                    if (newDistance < distances[neighbor])
                    {
                        distances[neighbor] = newDistance;
                        pq.Enqueue(neighbor, newDistance);
                    }
                }
            }
            
            // Target not reachable
            return -1;
        }
        
        /// <summary>
        /// Helper method to visualize the graph.
        /// </summary>
        private void PrintGraph(int n, int[][] edges)
        {
            Console.WriteLine("Graph Edges:");
            foreach (var edge in edges)
            {
                Console.WriteLine($"  {edge[0]} ─({edge[2]})→ {edge[1]}");
            }
            Console.WriteLine();
        }
        
        /// <summary>
        /// Helper method to print shortest paths.
        /// </summary>
        private void PrintPaths(int source, Dictionary<int, List<int>> paths)
        {
            Console.WriteLine($"Shortest paths from node {source}:");
            foreach (var kvp in paths)
            {
                int target = kvp.Key;
                var path = kvp.Value;
                Console.WriteLine($"  To {target}: {string.Join(" → ", path)}");
            }
        }
    }
}

/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
TESTING GUIDE
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Test Cases:

1. Standard example with multiple paths:
   Input: n = 5, source = 0
          edges = [[0,1,10], [0,4,5], [1,2,1], [1,4,2], [2,3,4], 
                   [3,2,6], [3,0,7], [4,1,3], [4,2,9], [4,3,2]]
   Expected: [0, 8, 9, 7, 5]
   
2. Simple linear path:
   Input: n = 3, source = 0
          edges = [[0,1,5], [1,2,3]]
   Expected: [0, 5, 8]
   
3. Disconnected graph:
   Input: n = 4, source = 0
          edges = [[0,1,1], [2,3,1]]
   Expected: [0, 1, -1, -1]
   
4. Triangle with better indirect path:
   Input: n = 3, source = 0
          edges = [[0,1,2], [1,2,3], [0,2,10]]
   Expected: [0, 2, 5] (0→1→2 is better than 0→2)
   
5. Single node:
   Input: n = 1, source = 0, edges = []
   Expected: [0]
   
6. Graph with cycle:
   Input: n = 3, source = 0
          edges = [[0,1,1], [1,2,1], [2,0,1]]
   Expected: [0, 1, 2]

Usage Example:

var solution = new DijkstraShortestPath();

// Test 1: Standard case
int n1 = 5;
int[][] edges1 = new int[][]
{
    new int[] {0, 1, 10},
    new int[] {0, 4, 5},
    new int[] {1, 2, 1},
    new int[] {1, 4, 2},
    new int[] {2, 3, 4},
    new int[] {3, 2, 6},
    new int[] {3, 0, 7},
    new int[] {4, 1, 3},
    new int[] {4, 2, 9},
    new int[] {4, 3, 2}
};
int[] result1 = solution.DijkstraShortestPaths(n1, edges1, 0);
Console.WriteLine($"Distances: [{string.Join(", ", result1)}]");
// Output: [0, 8, 9, 7, 5]

// Test 2: Get actual paths
var paths = solution.DijkstraWithPaths(n1, edges1, 0);
foreach (var kvp in paths)
{
    Console.WriteLine($"Path to {kvp.Key}: {string.Join(" → ", kvp.Value)}");
}

// Test 3: Single target
int distance = solution.DijkstraSingleTarget(n1, edges1, 0, 3);
Console.WriteLine($"Distance from 0 to 3: {distance}");

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
KEY TAKEAWAYS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

✓ Greedy Algorithm: Always expand closest unvisited node
✓ Priority Queue: Essential for O((V+E)logV) efficiency
✓ Relaxation: Update distance if shorter path found
✓ Non-Negative Weights Only: Negative weights break the algorithm!
✓ Visited Set: Prevents reprocessing optimal nodes
✓ Works on directed and undirected graphs
✓ Handles cycles correctly (unlike DFS/BFS)
✓ Can track parent nodes to reconstruct actual paths
✓ Early termination: Stop when target reached (single target variant)

WHEN TO USE:
✓ Finding shortest path in weighted graph
✓ All edge weights are non-negative
✓ Single-source shortest paths
✓ Network routing, GPS navigation
✓ Flight route optimization

WHEN NOT TO USE:
✗ Negative edge weights → Use Bellman-Ford
✗ All-pairs shortest paths → Use Floyd-Warshall
✗ Unweighted graphs → Use BFS (simpler and faster)
✗ Need to detect negative cycles → Use Bellman-Ford

Related Problems:
- Network Delay Time (Leetcode 743)
- Path with Minimum Effort (Leetcode 1631)
- Cheapest Flights Within K Stops (Leetcode 787)
- Swim in Rising Water (Leetcode 778)
- Path With Maximum Minimum Value (Leetcode 1102)

Comparisons:
- Dijkstra vs BFS: Dijkstra handles weights, BFS doesn't
- Dijkstra vs Bellman-Ford: Dijkstra faster but no negative weights
- Dijkstra vs Floyd-Warshall: Dijkstra single-source, FW all-pairs

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/
