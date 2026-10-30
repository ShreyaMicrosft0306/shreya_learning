// ============================================================
// PROBLEM: Trie
// ============================================================
// A trie (prefix tree) stores strings by shared prefixes so that lookup,
// insertion, and prefix checks are efficient.
//
// INPUT:
//   N/A
//
// OUTPUT:
//   N/A
//
// EDGE CASES:
//   - Empty input → no-op
//   - Single-character word → stored correctly
//   - Duplicate insertion → should not create duplicate paths
//
// ============================================================
// HOW TO SOLVE — Simple Walkthrough
// ============================================================
// Key Insight:
//   Store characters in a tree where each node represents a prefix.
//   Going down one level means adding one character to the current word.
//
// WHY THIS APPROACH?
//   It makes prefix-based operations fast and avoids repeated scanning.
//
// ============================================================
// TIME & SPACE COMPLEXITY
// ============================================================
//   n = number of characters inserted or searched
//
//   Time:  O(n)
//   Space: O(n)
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
