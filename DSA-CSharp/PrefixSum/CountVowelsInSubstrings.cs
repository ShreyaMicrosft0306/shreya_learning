// ============================================================
// PROBLEM: Count Vowels in Substrings
// ============================================================
// Given a lowercase string word and a list of [left, right] queries,
// count how many vowels appear in each substring word[left..right].
// The function must handle many queries efficiently.
//
// INPUT:
//   string word    — lowercase English letters
//   int[][] queries — each query is [left, right], inclusive
//   Example: word = "prefixsum", queries = [[0,2],[1,4],[3,5]]
//
// OUTPUT:
//   int[] — number of vowels in each queried substring
//   Example: [1, 2, 1]
//
// EDGE CASES:
//   - Single-character word, query [0,0] → count 1 or 0 depending on vowel
//   - Query of entire string → count all vowels in the string
//   - Repeated queries → same answer each time
//   - No vowels in substring → return 0
//
// ============================================================
// HOW TO SOLVE — Simple Walkthrough
// ============================================================
// Key Insight (Prefix Sum):
//   Instead of re-scanning the substring for every query, build an array
//   where prefix[i] stores how many vowels appear in word[0..i-1].
//   Then each query answer becomes a simple subtraction:
//       vowels(left..right) = prefix[right + 1] - prefix[left]
//
// Step-by-step with example: word = "prefixsum", queries = [[0,2],[1,4],[3,5]]
//
//   1. Build prefix counts for vowels a, e, i, o, u.
//      word letters with positions:
//      p(0), r(1), e(2), f(2), i(3), x(4), s(5), u(6), m(7)
//
//   2. prefix array:
//      [0, 0, 1, 1, 2, 2, 2, 3, 3]
//
//   3. Query [0,2] -> "pre"
//      prefix[3] - prefix[0] = 1 - 0 = 1
//
//   4. Query [1,4] -> "refi"
//      prefix[5] - prefix[1] = 2 - 0 = 2
//
//   5. Query [3,5] -> "ixs"
//      prefix[6] - prefix[3] = 2 - 1 = 1
//
//   6. Return [1,2,1]
//
// WHY THIS APPROACH?
//   A naive solution would scan each substring separately, costing O(n * q)
//   for q queries. Prefix sums reduce each query to O(1) after a single
//   O(n) preprocessing pass, which is dramatically faster for large inputs.
//
// ============================================================
// TIME & SPACE COMPLEXITY
// ============================================================
//   n = length of word, q = number of queries
//
//   Time:  O(n + q)
//     - Build prefix array: O(n)
//     - Answer each query with O(1) subtraction: O(q)
//     - Total: O(n + q)
//
//   Space: O(n)
//     - Prefix array stores one count per character position: O(n)
// ============================================================

public class CountVowelsInSubstrings
{
    private static bool IsVowel(char c)
    {
        return c == 'a' || c == 'e' || c == 'i' || c == 'o' || c == 'u';
    }

    public static int[] Solve(string word, int[][] queries)
    {
        int n = word.Length;
        int[] prefix = new int[n + 1];

        for (int i = 0; i < n; i++)
        {
            prefix[i + 1] = prefix[i] + (IsVowel(word[i]) ? 1 : 0);
        }

        int[] result = new int[queries.Length];
        for (int i = 0; i < queries.Length; i++)
        {
            int left = queries[i][0];
            int right = queries[i][1];
            result[i] = prefix[right + 1] - prefix[left];
        }

        return result;
    }

    public static void Main(string[] args)
    {
        var word1 = "prefixsum";
        var queries1 = new int[][]
        {
            new[] { 0, 2 },
            new[] { 1, 4 },
            new[] { 3, 5 }
        };

        Console.WriteLine("Example 1: " + string.Join(", ", Solve(word1, queries1)));
        // Expected: 1, 2, 1

        var word2 = "leetcode";
        var queries2 = new int[][]
        {
            new[] { 0, 0 },
            new[] { 1, 4 },
            new[] { 5, 6 }
        };

        Console.WriteLine("Example 2: " + string.Join(", ", Solve(word2, queries2)));
        // Expected: 1, 2, 0
    }
}
