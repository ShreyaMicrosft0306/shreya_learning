/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Clone Graph
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
Given a reference to a node in a connected undirected graph, return a deep
copy of the graph. Each cloned node must contain a new Node object, not an
object from the original graph.

INPUT:
- node: Node - Reference to the first node (value 1) in the graph
- 1 <= number of nodes <= 100
- Node values are unique and range from 1 to 100
- There are no duplicate edges or self-loops

OUTPUT:
- Node - Reference to the cloned copy of the first node

EXAMPLES:
Example 1:
Input: adjList = [[2,4],[1,3],[2,4],[1,3]]
Output: [[2,4],[1,3],[2,4],[1,3]]
Explanation: The returned graph has the same nodes and connections.

Example 2:
Input: adjList = [[]]
Output: [[]]

Example 3:
Input: node = null
Output: null

CONSTRAINTS:
- The graph is undirected
- The graph may contain any number of nodes up to 100
- The graph is connected
- Node values are 1-indexed in the test format

EDGE CASES:
✓ Null graph
✓ One node with no neighbors
✓ Two-node graph
✓ Cycles between neighboring nodes
✓ Dense graph

TRICK CASES:
⚡ Reusing the original node instead of creating a copy is incorrect
⚡ A graph cycle can cause infinite recursion without a visited map
⚡ The same original node may appear through multiple neighbor paths
⚡ Each original node must map to exactly one clone

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OPTIMAL SOLUTION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APPROACH:
Use DFS with a dictionary that maps each original node to its cloned node.

Before recursively cloning neighbors, create a clone for the current node
and immediately add the pair to the dictionary. This also breaks cycles:
if a neighbor has already been cloned, reuse that clone instead of visiting
it again.

KEY INSIGHTS:
• Creating a clone requires copying both its value and neighbor list
• The dictionary acts as both a clone map and a visited set
• Every original node is created exactly once
• Existing clones are reused to preserve graph connections

ALGORITHM STEPS:
1. If node is null, return null
2. If the original node already exists in the dictionary, return its clone
3. Create a new Node with the original value
4. Store original-to-clone mapping before exploring neighbors
5. For every original neighbor, recursively clone it
6. Add the returned clone to the current clone's neighbor list
7. Return the cloned first node

TIME COMPLEXITY: O(V + E)
- V nodes are visited once
- E neighbor relationships are processed once

SPACE COMPLEXITY: O(V)
- Dictionary stores V mappings
- Recursion stack can use up to O(V)

PATTERN: Graph DFS with visited mapping
DIFFICULTY: Medium
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;
using System.Collections.Generic;

namespace DSA_CSharp.Graphs
{
    public class Node
    {
        public int val;
        public IList<Node> neighbors;

        public Node()
        {
            val = 0;
            neighbors = new List<Node>();
        }

        public Node(int val)
        {
            this.val = val;
            neighbors = new List<Node>();
        }

        public Node(int val, IList<Node> neighbors)
        {
            this.val = val;
            this.neighbors = neighbors;
        }
    }

    public class CloneGraph
    {
        private Dictionary<Node, Node> originalToClone;

        /// <summary>
        /// Creates and returns a deep copy of the connected graph.
        /// Time: O(V + E), Space: O(V)
        /// </summary>
        public Node Clone(Node node)
        {
            originalToClone = new Dictionary<Node, Node>();
            return CloneFrom(node);
        }

        private Node CloneFrom(Node original)
        {
            if (original == null)
                return null;

            if (originalToClone.TryGetValue(original, out Node clone))
                return clone;

            clone = new Node(original.val);
            originalToClone[original] = clone;

            foreach (Node neighbor in original.neighbors)
            {
                clone.neighbors.Add(CloneFrom(neighbor));
            }

            return clone;
        }

        public static void Test()
        {
            var solution = new CloneGraph();

            Console.WriteLine("=== Clone Graph Test Cases ===\n");

            Node one = new Node(1);
            Node two = new Node(2);
            Node three = new Node(3);
            Node four = new Node(4);

            one.neighbors.Add(two);
            one.neighbors.Add(four);
            two.neighbors.Add(one);
            two.neighbors.Add(three);
            three.neighbors.Add(two);
            three.neighbors.Add(four);
            four.neighbors.Add(one);
            four.neighbors.Add(three);

            Node clonedOne = solution.Clone(one);

            Console.WriteLine("Four-node graph:");
            Console.WriteLine($"Original node reference different: {!ReferenceEquals(one, clonedOne)}");
            Console.WriteLine($"Cloned value: {clonedOne.val}");
            Console.WriteLine($"Cloned neighbors: {clonedOne.neighbors[0].val}, {clonedOne.neighbors[1].val}");
            Console.WriteLine("Expected: true, 1, 2, 4\n");

            Node isolated = new Node(1);
            Node clonedIsolated = solution.Clone(isolated);

            Console.WriteLine("Single node:");
            Console.WriteLine($"Value: {clonedIsolated.val}");
            Console.WriteLine($"Neighbor count: {clonedIsolated.neighbors.Count}");
            Console.WriteLine("Expected: 1, 0\n");

            Node clonedNull = solution.Clone(null);
            Console.WriteLine("Null graph:");
            Console.WriteLine($"Output: {(clonedNull == null ? "null" : "not null")}");
            Console.WriteLine("Expected: null");
        }
    }
}
