// ============================================================
// PROBLEM: Group Anagrams
// ============================================================
// Given an array of strings, group the strings that are anagrams
// of each other together and return all groups.
//
// Two strings are anagrams if one can be rearranged to form the other.
// e.g. "eat" and "tea" are anagrams; "tan" and "nat" are anagrams.
//
// INPUT:
//   string[] strs — array of lowercase strings
//   Example: ["eat", "tea", "tan", "ate", "nat", "bat"]
//
// OUTPUT:
//   IList<IList<string>> — groups of anagrams (order within groups doesn't matter)
//   Example: [["eat","tea","ate"], ["tan","nat"], ["bat"]]
//
// EDGE CASES:
//   - Empty input array [] → return []
//   - Single string ["abc"] → return [["abc"]]
//   - All strings are anagrams → return one group
//   - No strings share anagrams → each string is its own group
//   - Empty string "" in input → treated as its own group (all counts are 0)
//
// ============================================================
// HOW TO SOLVE — Simple Walkthrough
// ============================================================
// Key Insight:
//   Two strings are anagrams if and only if they have the exact
//   same character frequencies. So we use those frequencies as
//   a "fingerprint" (key) to group strings.
//
// Step-by-step with example: ["eat", "tea", "tan", "ate", "nat", "bat"]
//
//   1. "eat"  → count: a=1,e=1,t=1 → key: "0#0#0#0#1#0#0#0#1#0#0#0#0#0#0#0#0#0#0#1#0#0#0#0#0#0"
//              → map: { key1: ["eat"] }
//
//   2. "tea"  → count: a=1,e=1,t=1 → same key as "eat"
//              → map: { key1: ["eat","tea"] }
//
//   3. "tan"  → count: a=1,n=1,t=1 → different key (key2)
//              → map: { key1: ["eat","tea"], key2: ["tan"] }
//
//   4. "ate"  → count: a=1,e=1,t=1 → same key as "eat"
//              → map: { key1: ["eat","tea","ate"], key2: ["tan"] }
//
//   5. "nat"  → count: a=1,n=1,t=1 → same key as "tan"
//              → map: { key1: ["eat","tea","ate"], key2: ["tan","nat"] }
//
//   6. "bat"  → count: a=1,b=1,t=1 → new key (key3)
//              → map: { key1: [...], key2: [...], key3: ["bat"] }
//
//   7. Return all map values:
//      [["eat","tea","ate"], ["tan","nat"], ["bat"]]
//
// WHY NOT SORT?
//   Sorting each string also works (e.g. "eat"→"aet") but costs O(k log k) per string.
//   The frequency-count approach costs only O(k) per string — faster for long strings.
//
// ============================================================
// TIME & SPACE COMPLEXITY
// ============================================================
//   n = number of strings
//   k = length of the longest string
//
//   Time:  O(n * k)
//     - For each of the n strings, we iterate over its characters: O(k)
//     - Building the key with string.Join over 26 integers: O(26) = O(1)
//     - Dictionary lookup and insert: O(1) amortized
//     - Total: O(n * k)
//
//   Space: O(n * k)
//     - Dictionary stores all n strings across all groups
//     - Each key is a fixed 26-element joined string: O(1) per key
//     - Total space for all stored strings: O(n * k)
// ============================================================

public class GroupAnagrams
{
    public static IList<IList<string>> Solve(string[] strs)
    {
        var map = new Dictionary<string, List<string>>();

        foreach (string s in strs)
        {
            // Build a frequency count for each of the 26 letters
            int[] count = new int[26];
            foreach (char c in s)
            {
                count[c - 'a']++;
            }

            // Turn the count array into a unique string key
            // e.g. a=1,e=1,t=1 → "0#0#0#0#1#0#0#0#1#0#..."
            string key = string.Join("#", count);

            if (!map.ContainsKey(key))
            {
                map[key] = new List<string>();
            }

            map[key].Add(s);
        }

        return new List<IList<string>>(map.Values);
    }

    public static void Main(string[] args)
    {
        string[] input = { "eat", "tea", "tan", "ate", "nat", "bat" };
        var result = Solve(input);

        Console.WriteLine("Grouped Anagrams:");
        foreach (var group in result)
            Console.WriteLine("[" + string.Join(", ", group) + "]");
    }
}
