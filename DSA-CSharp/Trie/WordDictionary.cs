/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Design Add and Search Words Data Structure
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
Design a data structure that supports:
- addWord(word)
- search(word)

The search operation supports '.' as a wildcard for any letter.

EXAMPLE:
Input:
["WordDictionary","addWord","addWord","addWord","search","search","search","search"]
[[],["bad"],["dad"],["mad"],["pad"],["bad"],[".ad"],["b.."]]
Output:
[null,null,null,null,false,true,true,true]

APPROACH:
Trie (Prefix Tree).

Each node contains:
- children[26]
- isEndOfWord

For addWord, traverse the trie and create nodes as needed.
For search, use recursion with backtracking.
If the current character is '.': try every possible child.
If it is a normal letter: follow only that child.

Why this works:
A trie naturally stores all dictionary words by prefix. Wildcards are handled by
trying all possible next letters at a node during recursion. Once a branch fails,
we backtrack and continue with other letters.

TIME COMPLEXITY:
- addWord: O(word.Length)
- search: worst-case O(26^k) in the presence of many wildcards, but with the
  constraint of at most 2 dots, this is very manageable in practice.
SPACE COMPLEXITY: O(total characters stored)

PATTERN: Trie + Backtracking
DIFFICULTY: Medium
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;

namespace DSA_CSharp.Trie
{
    public class WordDictionary
    {
        private class TrieNode
        {
            public TrieNode[] Children { get; } = new TrieNode[26];
            public bool IsWord { get; set; }
        }

        private readonly TrieNode root = new TrieNode();

        public void AddWord(string word)
        {
            TrieNode node = root;

            foreach (char ch in word)
            {
                int index = ch - 'a';
                if (node.Children[index] == null)
                {
                    node.Children[index] = new TrieNode();
                }

                node = node.Children[index];
            }

            node.IsWord = true;
        }

        public bool Search(string word)
        {
            return SearchInNode(root, word, 0);
        }

        private bool SearchInNode(TrieNode node, string word, int index)
        {
            if (node == null)
            {
                return false;
            }

            if (index == word.Length)
            {
                return node.IsWord;
            }

            char ch = word[index];

            if (ch == '.')
            {
                foreach (TrieNode child in node.Children)
                {
                    if (child != null && SearchInNode(child, word, index + 1))
                    {
                        return true;
                    }
                }

                return false;
            }

            int childIndex = ch - 'a';
            return node.Children[childIndex] != null && SearchInNode(node.Children[childIndex], word, index + 1);
        }

        public static void Test()
        {
            var data = new WordDictionary();

            data.AddWord("bad");
            data.AddWord("dad");
            data.AddWord("mad");

            Console.WriteLine("=== Word Dictionary Test Cases ===\n");
            Console.WriteLine($"Search pad: {data.Search("pad")}");
            Console.WriteLine("Expected: False\n");

            Console.WriteLine($"Search bad: {data.Search("bad")}");
            Console.WriteLine("Expected: True\n");

            Console.WriteLine($"Search .ad: {data.Search(".ad")}");
            Console.WriteLine("Expected: True\n");

            Console.WriteLine($"Search b..: {data.Search("b..")}");
            Console.WriteLine("Expected: True");
        }
    }
}
