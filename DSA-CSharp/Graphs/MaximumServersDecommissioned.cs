/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Maximum Servers Decommissioned
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
A server room is represented by rack-slot coordinates. Server i is located
at servers[i] = [ri, ci], and no two servers occupy the same position.

A server may be decommissioned only when another active server exists in
the same rack or at the same slot number. Return the maximum number of
servers that can be decommissioned.

INPUT:
- servers: int[][] - Distinct server positions [rack, slot]

OUTPUT:
- int - Maximum number of servers that can be decommissioned

EXAMPLES:
Example 1:
Input: servers = [[0,0],[0,1],[1,0],[1,2],[2,1],[2,2]]
Output: 5
Explanation: All six servers are connected through shared racks or slots,
so five can be removed while one remains.

Example 2:
Input: servers = [[0,0],[0,2],[1,1],[2,0],[2,2]]
Output: 3
Explanation: There are two connected groups. One server must remain in each,
so 5 - 2 = 3 servers can be removed.

Example 3:
Input: servers = [[0,0]]
Output: 0
Explanation: A lone server does not share its rack or slot with another server.

CONSTRAINTS:
- 1 <= servers.length <= 1000
- servers[i].length == 2
- 0 <= ri, ci <= 10^4
- All server positions are distinct

EDGE CASES:
✓ A single server
✓ No servers share a rack or slot
✓ Every server is in the same rack
✓ Every server is at the same slot number
✓ Connections formed transitively through alternating racks and slots
✓ Multiple disconnected groups
✓ Maximum input size

TRICK CASES:
⚡ Servers need not directly share a rack or slot to belong to the same group
⚡ One server must remain in every connected component
⚡ Counting only duplicate racks or slots misses transitive connections
⚡ Pairwise comparison works but takes O(n²) time
⚡ Rack and slot numbers belong to separate dimensions

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OPTIMAL SOLUTION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APPROACH:
Treat each server as a graph node. Servers sharing a rack or slot are in the
same connected component. In a component containing k servers, exactly k - 1
servers can be decommissioned: repeatedly remove a server while preserving
one final server. Therefore, the total answer is:

    number of servers - number of connected components

Use Disjoint Set Union (Union-Find) to build the components efficiently.
For each rack and slot, remember the first server seen there. Union every
later server with that representative.

KEY INSIGHTS:
• Shared racks and slots define an undirected connectivity relation
• Connectivity is transitive even when two servers do not directly align
• Every connected component must leave exactly one active server
• Union-Find tracks component merges in near-constant amortized time

ALGORITHM STEPS:
1. Create one disjoint-set node for every server.
2. Maintain maps from each rack and slot to a representative server index.
3. For each server:
   a. Union it with the representative already seen in its rack, if any.
   b. Union it with the representative already seen at its slot, if any.
   c. Otherwise record it as the representative for that rack or slot.
4. Track the number of components, decreasing it after every successful union.
5. Return the server count minus the final component count.

TIME COMPLEXITY: O(n α(n))
- Each server performs at most two Union-Find operations
- α(n), the inverse Ackermann function, is effectively constant

SPACE COMPLEXITY: O(n)
- Union-Find arrays and rack/slot dictionaries store at most O(n) entries

PATTERN: Graph Connectivity, Union-Find (Disjoint Set Union)
DIFFICULTY: Medium
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;
using System.Collections.Generic;

namespace DSA_CSharp.Graphs
{
    public class MaximumServersDecommissioned
    {
        /// <summary>
        /// Returns the maximum number of servers removable while leaving one
        /// server in every rack/slot-connected component.
        /// </summary>
        public int MaxDecommissioned(int[][] servers)
        {
            if (servers == null || servers.Length <= 1)
                return 0;

            var disjointSet = new DisjointSet(servers.Length);
            var rackRepresentative = new Dictionary<int, int>();
            var slotRepresentative = new Dictionary<int, int>();

            for (int serverIndex = 0; serverIndex < servers.Length; serverIndex++)
            {
                int rack = servers[serverIndex][0];
                int slot = servers[serverIndex][1];

                if (rackRepresentative.TryGetValue(rack, out int sameRackServer))
                {
                    disjointSet.Union(serverIndex, sameRackServer);
                }
                else
                {
                    rackRepresentative[rack] = serverIndex;
                }

                if (slotRepresentative.TryGetValue(slot, out int sameSlotServer))
                {
                    disjointSet.Union(serverIndex, sameSlotServer);
                }
                else
                {
                    slotRepresentative[slot] = serverIndex;
                }
            }

            return servers.Length - disjointSet.ComponentCount;
        }

        private class DisjointSet
        {
            private readonly int[] parent;
            private readonly int[] size;

            public int ComponentCount { get; private set; }

            public DisjointSet(int nodeCount)
            {
                parent = new int[nodeCount];
                size = new int[nodeCount];
                ComponentCount = nodeCount;

                for (int node = 0; node < nodeCount; node++)
                {
                    parent[node] = node;
                    size[node] = 1;
                }
            }

            public int Find(int node)
            {
                if (parent[node] != node)
                {
                    parent[node] = Find(parent[node]);
                }

                return parent[node];
            }

            public void Union(int firstNode, int secondNode)
            {
                int firstRoot = Find(firstNode);
                int secondRoot = Find(secondNode);

                if (firstRoot == secondRoot)
                    return;

                // Attach the smaller tree below the larger tree.
                if (size[firstRoot] < size[secondRoot])
                {
                    int temporaryRoot = firstRoot;
                    firstRoot = secondRoot;
                    secondRoot = temporaryRoot;
                }

                parent[secondRoot] = firstRoot;
                size[firstRoot] += size[secondRoot];
                ComponentCount--;
            }
        }

        public static void Test()
        {
            var solver = new MaximumServersDecommissioned();

            int[][] servers1 =
            {
                new[] { 0, 0 }, new[] { 0, 1 }, new[] { 1, 0 },
                new[] { 1, 2 }, new[] { 2, 1 }, new[] { 2, 2 }
            };
            Console.WriteLine($"Example 1: {solver.MaxDecommissioned(servers1)} (expected 5)");

            int[][] servers2 =
            {
                new[] { 0, 0 }, new[] { 0, 2 }, new[] { 1, 1 },
                new[] { 2, 0 }, new[] { 2, 2 }
            };
            Console.WriteLine($"Example 2: {solver.MaxDecommissioned(servers2)} (expected 3)");

            int[][] servers3 = { new[] { 0, 0 } };
            Console.WriteLine($"Example 3: {solver.MaxDecommissioned(servers3)} (expected 0)");
        }
    }
}
