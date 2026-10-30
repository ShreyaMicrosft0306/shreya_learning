// ============================================================
// PROBLEM: Top K Frequent Elements
// ============================================================
// Given an integer array and an integer k, return the k elements
// that appear most frequently in the array. Answer can be in any order.
//
// INPUT:
//   int[] nums — array of integers (can include negatives)
//   int k      — how many top-frequent elements to return
//   Example: nums = [1,1,1,2,2,3], k = 2
//
// OUTPUT:
//   int[] — the k most frequent elements (any order)
//   Example: [1, 2]
//
// EDGE CASES:
//   - k equals total unique elements → return all unique elements
//       e.g. nums=[1,2,3], k=3 → [1,2,3]
//   - Single element array → always return that element
//       e.g. nums=[1], k=1 → [1]
//   - All elements have the same frequency → any k of them are valid
//       e.g. nums=[1,2,3], k=2 → any two of [1,2,3]
//   - Negative numbers in input → handled the same as positives
//       e.g. nums=[-1,-1,2], k=1 → [-1]
//   - k=1 → return only the single most frequent element
//       e.g. nums=[1,2,2,3,3,3], k=1 → [3]
//
// ============================================================
// HOW TO SOLVE — Simple Walkthrough
// ============================================================
// Key Insight:
//   Instead of sorting (which costs O(n log n)), use BUCKET SORT.
//   The frequency of any number can be at most n (if all elements
//   are the same). So create n+1 buckets where bucket[i] holds
//   all numbers that appear exactly i times. Then read the buckets
//   from the end (highest frequency first) until you have k elements.
//
// Step-by-step with example: nums = [1,1,1,2,2,3], k = 2
//
//   Step 1 — Count frequencies using a dictionary:
//     {1 → 3,  2 → 2,  3 → 1}
//
//   Step 2 — Create buckets of size n+1 = 7 (indices 0..6):
//     bucket[frequency] = list of numbers with that frequency
//     bucket[0] = []
//     bucket[1] = [3]     ← number 3 appears 1 time
//     bucket[2] = [2]     ← number 2 appears 2 times
//     bucket[3] = [1]     ← number 1 appears 3 times
//     bucket[4] = []
//     bucket[5] = []
//     bucket[6] = []
//
//   Step 3 — Scan buckets from right (highest freq) and collect elements:
//     i=6: empty → skip
//     i=5: empty → skip
//     i=4: empty → skip
//     i=3: [1]   → add 1 to result → result = [1],  collected = 1
//     i=2: [2]   → add 2 to result → result = [1,2], collected = 2
//     collected == k (2), so stop.
//
//   Return: [1, 2]
//
// WHY THIS APPROACH?
//   Naive approach: count frequencies → sort by frequency → take top k.
//     Cost: O(n log n) for the sort step.
//   Heap approach: count frequencies → maintain a min-heap of size k.
//     Cost: O(n log k) — better, but still not linear.
//   Bucket sort approach (this solution): no sorting at all.
//     Cost: O(n) — optimal for this problem.
//   The bucket sort works because the frequency values are bounded
//   between 1 and n, so we can use them directly as array indices.
//
// ============================================================
// TIME & SPACE COMPLEXITY
// ============================================================
//   n = number of elements in nums
//   k = number of top frequent elements to return
//
//   Time:  O(n)
//     - Building frequency dictionary: O(n) — one pass over nums
//     - Filling buckets: O(n) — one pass over dictionary entries
//     - Scanning buckets from end: O(n) — at most n+1 buckets scanned
//     - Total: O(n)
//
//   Space: O(n)
//     - Frequency dictionary: O(n) — at most n unique elements
//     - Bucket array of size n+1: O(n)
//     - Result array of size k: O(k) ≤ O(n)
//     - Total: O(n)
// ============================================================

public class TopKFrequentElements
{
    public static int[] Solve(int[] nums, int k)
    {
        // Step 1: Count how many times each number appears
        var freq = new Dictionary<int, int>();
        foreach (int num in nums)
        {
            if (!freq.ContainsKey(num))
                freq[num] = 0;
            freq[num]++;
        }

        // Step 2: Create buckets — bucket[i] holds all numbers with frequency i
        // Max possible frequency is nums.Length, so we need n+1 buckets
        var buckets = new List<int>[nums.Length + 1];
        foreach (var pair in freq)
        {
            int frequency = pair.Value;
            if (buckets[frequency] == null)
                buckets[frequency] = new List<int>();
            buckets[frequency].Add(pair.Key);
        }

        // Step 3: Scan from highest frequency down and collect k elements
        var result = new List<int>();
        for (int i = buckets.Length - 1; i >= 1 && result.Count < k; i--)
        {
            if (buckets[i] != null)
                result.AddRange(buckets[i]);
        }

        return result.ToArray();
    }

    public static void Main(string[] args)
    {
        int[] nums1 = { 1, 1, 1, 2, 2, 3 };
        Console.WriteLine("Example 1: [" + string.Join(", ", Solve(nums1, 2)) + "]");
        // Expected: [1, 2]

        int[] nums2 = { 1 };
        Console.WriteLine("Example 2: [" + string.Join(", ", Solve(nums2, 1)) + "]");
        // Expected: [1]

        int[] nums3 = { 1, 2, 1, 2, 1, 2, 3, 1, 3, 2 };
        Console.WriteLine("Example 3: [" + string.Join(", ", Solve(nums3, 2)) + "]");
        // Expected: [1, 2]
    }
}
