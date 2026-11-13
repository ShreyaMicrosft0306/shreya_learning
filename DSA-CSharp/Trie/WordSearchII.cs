/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Word Search II
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
Given an m x n board of characters and a list of words, return all words that
exist in the board.

A word can be formed by starting from any cell and moving in 4 directions:
up, down, left, right.

Each cell can only be used once per word.

EXAMPLE:
Input:
board = [
  ["o","a","a","n"],
  ["e","t","a","e"],
  ["i","h","k","r"],
  ["i","f","l","v"]
]
words = ["oath","pea","eat","rain"]
Output: ["eat","oath"]

APPROACH:
Use a trie for the dictionary and DFS over the board.

1. Build a trie from the list of words.
2. For each cell on the board, start DFS from that cell.
3. At each step, follow the trie path for the current character.
4. If a trie node marks the end of a word, add it to the result.
5. Mark the board cell visited and recursively explore neighbors.
6. Backtrack by restoring the board cell.

This is efficient because DFS only explores valid trie prefixes, avoiding useless
searches that do not match any dictionary word.

TIME COMPLEXITY:
- Let K be the total number of characters across all words.
- Board traversal is O(m * n * 4^L) in the worst case, but trie pruning keeps it
  much smaller in practice.
- In typical problem constraints, this is accepted efficiently.

SPACE COMPLEXITY:
- O(K + m * n)
- Trie + visited board.

PATTERN: Trie + DFS + Backtracking
DIFFICULTY: Hard
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;
using System.Collections.Generic;

namespace DSA_CSharp.Trie
{
    public class WordSearchII
    {
        private class TrieNode
        {
            public TrieNode[] Children { get; } = new TrieNode[26];
            public string Word { get; set; } = string.Empty;
        }

        private readonly HashSet<string> found = new HashSet<string>();

        public IList<string> FindWords(char[][] board, string[] words)
        {
            if (board == null || board.Length == 0 || words == null || words.Length == 0)
            {
                return new List<string>();
            }

            TrieNode root = new TrieNode();
            foreach (string word in words)
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

                node.Word = word;
            }

            int rows = board.Length;
            int cols = board[0].Length;
            bool[,] visited = new bool[rows, cols];

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    DFS(board, r, c, root, visited);
                }
            }

            return new List<string>(found);
        }

        private void DFS(char[][] board, int row, int col, TrieNode node, bool[,] visited)
        {
            if (row < 0 || row >= board.Length || col < 0 || col >= board[0].Length || visited[row, col])
            {
                return;
            }

            char ch = board[row][col];
            int index = ch - 'a';
            if (node.Children[index] == null)
            {
                return;
            }

            TrieNode next = node.Children[index];
            visited[row, col] = true;

            if (!string.IsNullOrEmpty(next.Word))
            {
                found.Add(next.Word);
            }

            int[] dr = { -1, 1, 0, 0 };
            int[] dc = { 0, 0, -1, 1 };

            for (int i = 0; i < 4; i++)
            {
                int nr = row + dr[i];
                int nc = col + dc[i];
                if (nr >= 0 && nr < board.Length && nc >= 0 && nc < board[0].Length && !visited[nr, nc])
                {
                    DFS(board, nr, nc, next, visited);
                }
            }

            visited[row, col] = false;
        }

        public static void Test()
        {
            var solution = new WordSearchII();

            char[][] board1 =
            {
                new[] { 'o', 'a', 'a', 'n' },
                new[] { 'e', 't', 'a', 'e' },
                new[] { 'i', 'h', 'k', 'r' },
                new[] { 'i', 'f', 'l', 'v' }
            };

            string[] words1 = { "oath", "pea", "eat", "rain" };
            Console.WriteLine("=== Word Search II Test Cases ===\n");
            Console.WriteLine($"Test 1 Output: [{string.Join(", ", solution.FindWords(board1, words1))}]");
            Console.WriteLine("Expected: [eat, oath]\n");

            char[][] board2 =
            {
                new[] { 'a', 'b' },
                new[] { 'c', 'd' }
            };
            string[] words2 = { "abcb" };
            Console.WriteLine($"Test 2 Output: [{string.Join(", ", solution.FindWords(board2, words2))}]");
            Console.WriteLine("Expected: []");
        }
    }
}
