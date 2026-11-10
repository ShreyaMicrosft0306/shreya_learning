/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Alien Dictionary
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
A new language uses the same letters as English but in an unknown alphabet
order. Given a list of words sorted according to that language's dictionary
rules, derive the order of all letters if a unique or valid order exists.

INPUT:
- words: string[] - Words in sorted order according to the alien language

OUTPUT:
- string - A valid topological alphabet order, or "" if the order is invalid

EXAMPLES:
Example 1:
Input: words = ["wrt", "wrf", "er", "ett", "rftt"]
Output: "wertf"

Example 2:
Input: words = ["z", "x"]
Output: "zx"

Example 3:
Input: words = ["z", "x", "z"]
Output: ""

CONSTRAINTS:
- 1 <= words.Length <= 100
- 1 <= words[i].Length <= 100
- All words contain only lowercase English letters
- Words are unique

EDGE CASES:
✓ One word: all letters appear, but only partial ordering is known
✓ Prefix case: "abc" before "ab" is invalid
✓ Cycle: "z" -> "x" -> "z" returns ""
✓ Duplicate character relationships
✓ An edge appears across multiple word pairs

TRICK CASES:
⚡ Compare only the first differing character between adjacent words
⚡ Prefix words invalidate the order when a longer word comes first
⚡ The result can contain more than one valid answer
⚡ A cycle means no valid alien alphabet exists

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OPTIMAL SOLUTION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APPROACH:
Build a directed graph and apply topological sorting.

Each letter is a node. If letter A appears before letter B at the first
different position of two adjacent words, add edge A -> B. The in-degree
of B increases because A must come before B.

KEY INSIGHTS:
• Adjacent words provide all the ordering constraints needed
• Comparing every pair is unnecessary
• Prefixes must follow the longer-word-first rule
• Topological sort validates cycles and supplies a valid order

ALGORITHM STEPS:
1. Add every character in every word to the graph
2. For each pair of adjacent words:
   a. Compare characters until they differ
   b. Add an edge from firstDifferent to secondDifferent
   c. Increase the second character's in-degree once
   d. If no difference but first is a prefix of second, return ""
3. Add every character with in-degree zero to a queue
4. Remove nodes and decrement neighbors' in-degrees
5. Append each removed character to the result
6. Return "" if the result contains fewer characters than the graph

TIME COMPLEXITY: O(C + N)
- C = total number of characters across all words
- N = number of unique adjacency relationships
- In practice, at most C character comparisons are needed

SPACE COMPLEXITY: O(V + E)
- V = number of unique letters
- E = number of first-difference character relationships

PATTERN: Topological Sort / Directed Graph
DIFFICULTY: Hard
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace DSA_CSharp.Graphs
{
    public class AlienDictionary
    {
        /// <summary>
        /// Derives a valid alien alphabet from sorted words.
        /// Time: O(C + E), Space: O(V + E)
        /// </summary>
        public string AlienOrder(string[] words)
        {
            if (words == null || words.Length == 0)
                return "";

            var adjacency = new Dictionary<char, HashSet<char>>();
            var inDegree = new Dictionary<char, int>();

            foreach (string word in words)
            {
                foreach (char letter in word)
                {
                    if (!adjacency.ContainsKey(letter))
                    {
                        adjacency[letter] = new HashSet<char>();
                        inDegree[letter] = 0;
                    }
                }
            }

            for (int i = 0; i < words.Length - 1; i++)
            {
                string first = words[i];
                string second = words[i + 1];
                int comparisonLength = Math.Min(first.Length, second.Length);

                int position = 0;
                while (position < comparisonLength &&
                       first[position] == second[position])
                {
                    position++;
                }

                if (position == comparisonLength)
                {
                    // A longer word cannot come before its shorter prefix.
                    if (first.Length > second.Length)
                        return "";

                    continue;
                }

                char before = first[position];
                char after = second[position];

                if (adjacency[before].Add(after))
                    inDegree[after]++;
            }

            var zeroInDegree = new Queue<char>();
            foreach (char letter in inDegree.Keys)
            {
                if (inDegree[letter] == 0)
                    zeroInDegree.Enqueue(letter);
            }

            var result = new StringBuilder();

            while (zeroInDegree.Count > 0)
            {
                char letter = zeroInDegree.Dequeue();
                result.Append(letter);

                foreach (char neighbor in adjacency[letter])
                {
                    inDegree[neighbor]--;

                    if (inDegree[neighbor] == 0)
                        zeroInDegree.Enqueue(neighbor);
                }
            }

            if (result.Length != adjacency.Count)
                return "";

            return result.ToString();
        }

        public static void Test()
        {
            var solution = new AlienDictionary();

            Console.WriteLine("=== Alien Dictionary Test Cases ===\n");

            string[] example1 = { "wrt", "wrf", "er", "ett", "rftt" };
            Console.WriteLine($"Example 1: {solution.AlienOrder(example1)}");
            Console.WriteLine("Expected: one valid order such as wertf\n");

            string[] example2 = { "z", "x" };
            Console.WriteLine($"Example 2: {solution.AlienOrder(example2)}");
            Console.WriteLine("Expected: zx\n");

            string[] example3 = { "z", "x", "z" };
            Console.WriteLine($"Example 3: {solution.AlienOrder(example3)}");
            Console.WriteLine("Expected: \"\"\n");

            string[] prefixCase = { "abc", "ab" };
            Console.WriteLine($"Prefix case: {solution.AlienOrder(prefixCase)}");
            Console.WriteLine("Expected: \"\"\n");

            string[] oneWord = { "cab" };
            Console.WriteLine($"One word: {solution.AlienOrder(oneWord)}");
            Console.WriteLine("Expected: any order containing c, a, b");
        }
    }
}
