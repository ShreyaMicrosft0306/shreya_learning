/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Serialize and Deserialize Binary Tree
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
Convert a binary tree into a string and reconstruct the exact same tree from
that string.

INPUT:
- Serialize: TreeNode root - The binary tree root
- Deserialize: string data - Serialized preorder representation

OUTPUT:
- Serialize: string - Encoded tree
- Deserialize: TreeNode - Reconstructed root

EXAMPLES:
Example 1:
Input: root = [1,2,3,null,null,4,5]
Serialized: "1,2,N,3,4,N,5,N"
Output: [1,2,3,null,null,4,5]

Example 2:
Input: root = []
Serialized: "N"
Output: []

CONSTRAINTS:
- 0 <= number of nodes <= 10^4
- -1000 <= Node.val <= 1000

EDGE CASES:
✓ Empty tree
✓ Single node
✓ Left-skewed tree
✓ Right-skewed tree
✓ Negative values
✓ Root only

TRICK CASES:
⚡ Null children must be stored explicitly
⚡ Node values can contain '-' or ',' if delimiter choice is invalid
⚡ Comma separation is safe because values are parsed numerically
⚡ Preorder uniquely reconstructs a tree when null markers are included

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OPTIMAL SOLUTION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APPROACH:
Serialize using preorder traversal.

Visit each node in this order:
1. Node value
2. Left subtree
3. Right subtree

Write "N" for every null node. This records enough information to reconstruct
the tree shape exactly.

Deserialization reads values in the same preorder order. When it encounters
"N", it creates no node and returns. Otherwise, it creates a node and
recursively reconstructs its left and right subtrees.

KEY INSIGHTS:
• Preorder value followed by left/right subtrees defines tree structure
• Null markers preserve missing children
• A queue or index is used to consume serialized values once
• Values are separated only by commas

ALGORITHM STEPS:
SERIALIZE:
1. Append the node value, or "N" for null
2. Serialize the left subtree
3. Serialize the right subtree

DESERIALIZE:
1. Split the serialized string by commas
2. Use an index to read the next token
3. If token is "N", return null
4. Otherwise create a node
5. Recursively deserialize left and right children

TIME COMPLEXITY: O(n)
- Every node and null marker is processed once

SPACE COMPLEXITY: O(n)
- Serialized string and recursion stack use O(n) space

PATTERN: Preorder DFS / Tree Serialization
DIFFICULTY: Hard
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace DSA_CSharp.Trees
{
    public class SerializeAndDeserializeBinaryTree
    {
        private const string NullMarker = "N";

        /// <summary>
        /// Serializes a binary tree using preorder values and null markers.
        /// Time: O(n), Space: O(n)
        /// </summary>
        public string Serialize(TreeNode root)
        {
            var result = new StringBuilder();
            SerializeNode(root, result);
            return result.Length == 0 ? NullMarker : result.ToString().TrimEnd(',');
        }

        private void SerializeNode(TreeNode node, StringBuilder result)
        {
            if (node == null)
            {
                result.Append(NullMarker);
                result.Append(',');
                return;
            }

            result.Append(node.val);
            result.Append(',');
            SerializeNode(node.left, result);
            SerializeNode(node.right, result);
        }

        /// <summary>
        /// Deserializes a preorder string back into a binary tree.
        /// Time: O(n), Space: O(n)
        /// </summary>
        public TreeNode Deserialize(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
                return null;

            string[] values = data.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            int index = 0;

            return DeserializeNode(values, ref index);
        }

        private TreeNode DeserializeNode(string[] values, ref int index)
        {
            if (index >= values.Length)
                return null;

            string value = values[index++];

            if (value == NullMarker)
                return null;

            if (!int.TryParse(value, out int nodeValue))
                throw new FormatException("Invalid serialized tree value: " + value);

            var node = new TreeNode(nodeValue);
            node.left = DeserializeNode(values, ref index);
            node.right = DeserializeNode(values, ref index);
            return node;
        }

        public static void Test()
        {
            var solution = new SerializeAndDeserializeBinaryTree();

            Console.WriteLine("=== Serialize and Deserialize Binary Tree Test Cases ===\n");

            var root = new TreeNode(
                1,
                new TreeNode(2),
                new TreeNode(3,
                    new TreeNode(4),
                    new TreeNode(5)));

            string serialized = solution.Serialize(root);
            TreeNode restored = solution.Deserialize(serialized);

            Console.WriteLine($"Serialized: {serialized}");
            Console.WriteLine($"Root value: {restored?.val}");
            Console.WriteLine($"Left value: {restored?.left?.val}");
            Console.WriteLine($"Right value: {restored?.right?.val}");
            Console.WriteLine("Expected: 1,2,3\n");

            TreeNode empty = solution.Deserialize(solution.Serialize(null));
            Console.WriteLine($"Empty serialized: {solution.Serialize(null)}");
            Console.WriteLine($"Empty restored is null: {empty == null}");
            Console.WriteLine("Expected: N, true\n");

            var negative = new TreeNode(-10, null, new TreeNode(-5));
            string negativeSerialized = solution.Serialize(negative);
            TreeNode negativeRestored = solution.Deserialize(negativeSerialized);

            Console.WriteLine($"Negative serialized: {negativeSerialized}");
            Console.WriteLine($"Negative root/right: {negativeRestored?.val}, {negativeRestored?.right?.val}");
            Console.WriteLine("Expected: -10, -5");
        }
    }
}
