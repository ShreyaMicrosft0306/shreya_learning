// ============================================================
// PROBLEM: Trie
// ============================================================
// A trie (prefix tree) stores strings by shared prefixes so that lookup,
// insertion, and prefix checks are efficient.
//
// INPUT:
//   words and prefixes passed to Insert, Search, and StartsWith
//   Example: Insert("apple"), Search("apple"), StartsWith("app")
//
// OUTPUT:
//   bool or void for the supported operations
//   Example: Search("apple") = true, Search("app") = false
//            StartsWith("app") = true
//
// EDGE CASES:
//   - Empty input → no-op
//   - Single-character word → stored correctly
//   - Duplicate insertion → should not create duplicate paths
//   - Search for a prefix alone → false unless that prefix is a stored word
//
// ============================================================
// HOW TO SOLVE — Simple Walkthrough
// ============================================================
// Key Insight:
//   Store characters in a tree where each node represents a prefix.
//   Going down one level means adding one character to the current word.
//
// Step-by-step with example: Insert("app"), Insert("apple")
//
//   1. Start at the root.
//   2. Insert 'a':
//      root --a--> node
//   3. Insert 'p':
//      node --p--> node
//   4. Insert 'p':
//      node --p--> node marked IsEndOfWord=true
//      "app" is now a complete word.
//   5. Insert "apple":
//      reuse the "app" path, then add 'l' and 'e'.
//      Mark the 'e' node IsEndOfWord=true.
//
//   Search("apple"):
//     Follow a -> p -> p -> l -> e.
//     The final 'e' node is marked as the end of a word, so return true.
//
//   Search("app"):
//     Follow a -> p -> p.
//     That node is marked as the end of a word, so return true.
//
//   StartsWith("apl"):
//     Follow a -> p -> l.
//     'l' has not been added under "ap", so return false.
//
// WHY THIS APPROACH?
//   It makes prefix-based operations fast and avoids repeated scanning.
//
// ============================================================
// TIME & SPACE COMPLEXITY
// ============================================================
//   n = number of characters inserted or searched
//
//   Time: O(n)
//     - Insert follows one node per character: O(n)
//     - Search and StartsWith follow at most n characters: O(n)
//     - Match follows n prefix characters plus its matching words: O(n + k)
//
//   Space: O(n)
//     - One node may be needed for each unique character path
//     - Shared prefixes use fewer nodes
// ============================================================

public class Trie
{
    private class TrieNode
    {
        public Dictionary<char, TrieNode> Children { get; } = new();
        public bool IsEndOfWord { get; set; }
    }

    private readonly TrieNode root;

    public Trie()
    {
        root = new TrieNode();
    }

    public void Insert(string word)
    {
        if (string.IsNullOrEmpty(word))
            return;

        TrieNode current = root;
        foreach (char ch in word)
        {
            if (!current.Children.ContainsKey(ch))
            {
                current.Children[ch] = new TrieNode();
            }
            current = current.Children[ch];
        }

        current.IsEndOfWord = true;
    }

    public bool Search(string word)
    {
        if (string.IsNullOrEmpty(word))
            return false;

        TrieNode current = root;

        foreach (char ch in word)
        {
            if (!current.Children.ContainsKey(ch))
            {
                return false;
            }

            current = current.Children[ch];
        }

        return current.IsEndOfWord;
    }

    public bool StartsWith(string prefix)
    {
        if (string.IsNullOrEmpty(prefix))
            return true;

        TrieNode current = root;

        foreach (char ch in prefix)
        {
            if (!current.Children.ContainsKey(ch))
            {
                return false;
            }

            current = current.Children[ch];
        }

        return true;
    }

    public List<string> Match(string prefix)
    {
        if (prefix == null)
        {
            return new List<string>();
        }

        TrieNode current = root;
        foreach (char ch in prefix)
        {
            if (!current.Children.TryGetValue(ch, out current))
            {
                return new List<string>();
            }
        }

        var matches = new List<string>();
        var path = new System.Text.StringBuilder(prefix);
        CollectWords(current, path, matches);
        return matches;
    }

    private void CollectWords(TrieNode node, System.Text.StringBuilder path, List<string> result)
    {
        if (node.IsEndOfWord)
        {
            result.Add(path.ToString());
        }

        foreach (var child in node.Children)
        {
            path.Append(child.Key);
            CollectWords(child.Value, path, result);
            path.Length--;
        }
    }
}
