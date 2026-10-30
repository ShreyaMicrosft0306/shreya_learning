// ============================================================
// PROBLEM: Split Array Largest Sum
// ============================================================
// Given an array nums and integer k, split nums into k contiguous
// subarrays to minimise the maximum subarray sum. Return that minimum
// possible maximum sum.
//
// INPUT:
//   int[] nums — task weights (contiguous order must be preserved)
//   int k      — number of workers (splits)
//   Example: nums = [4, 8, 15, 7, 3],  k = 3
//
// OUTPUT:
//   int — minimum possible value of the maximum workload
//   Example: 15
//
// EDGE CASES:
//   - k = nums.Length → each element is its own group → answer = max(nums)
//   - k = 1           → one worker takes everything → answer = sum(nums)
//   - Single element [10], k=1 → answer = 10
//   - All equal [5,5,5,5], k=2 → split evenly → answer = 10
//
// ============================================================
// HOW TO SOLVE — Simple Walkthrough
// ============================================================
// Key Insight (Binary Search on the Answer):
//   The answer lies in [max(nums), sum(nums)].
//     - Lower bound max(nums): even with unlimited workers, one worker
//       must carry the heaviest single task.
//     - Upper bound sum(nums): one worker carries everything.
//   Binary search this range. For a given capacity `mid`, greedily
//   check if we can split into at most k groups where each group's
//   sum ≤ mid. If YES → try smaller; if NO → need larger.
//
// CanSplit(capacity):
//   Walk left to right. Keep adding tasks to the current worker.
//   When the next task would push over capacity, assign a new worker.
//   Return true if total workers used ≤ k.
//
// Step-by-step with example: nums = [4, 8, 15, 7, 3],  k = 3
//
//   Search range: lo = max(nums) = 15,  hi = sum(nums) = 37
//
//   mid=26: worker1: 4→12→(+15=27>26, new) worker2: 15→22→25 → 2 ≤ 3 → VALID → hi=26
//   mid=20: worker1: 4→12→(27>20, new) worker2: 15→(22>20, new) worker3: 7→10 → 3 ≤ 3 → VALID → hi=20
//   mid=17: same split as above → 3 workers → VALID → hi=17
//   mid=16: same split → 3 workers → VALID → hi=16
//   mid=15: worker1: 4→12→(27>15, new) worker2: 15→(22>15, new) worker3: 7→10 → 3 ≤ 3 → VALID → hi=15
//
//   lo=15, hi=15 → lo == hi → return 15  ✓
//
// WHY lo = max(nums) AND hi = sum(nums)?
//   Any capacity below max(nums) is impossible — the heaviest task
//   alone exceeds it. Any capacity above sum(nums) is wasteful — one
//   worker can already do everything. The answer must live in this range.
//
// WHY GREEDY IN CanSplit?
//   To minimise workers used, always fill the current worker as much
//   as possible before opening a new one. This greedy gives the fewest
//   workers for any fixed capacity, making the feasibility check optimal.
//
// WHY hi = mid (not mid-1) WHEN VALID?
//   mid itself is a valid answer — we must keep it in the window.
//   Setting hi = mid-1 would discard the answer.
//
// WHY THIS APPROACH?
//   DP: dp[i][j] = min largest sum using j splits on first i elements.
//   → O(n²k) time, O(nk) space. Slow for large inputs.
//   Binary search on answer (this): O(n log S) where S = sum(nums).
//   Much faster — log S ≈ 40 iterations even for large sums.
//
// ============================================================
// TIME & SPACE COMPLEXITY
// ============================================================
//   n = number of elements,  S = sum(nums),  M = max(nums)
//
//   Time:  O(n log(S - M))
//     - Binary search range is [M, S]: O(log(S - M)) iterations
//     - Each iteration runs CanSplit which scans all n elements: O(n)
//     - Total: O(n log(S - M))
//
//   Space: O(1)
//     - Only a fixed number of integer variables: lo, hi, mid, workers, load
// ============================================================

public class SplitArrayLargestSum
{
    // Returns true if nums can be split into at most k groups each summing to <= capacity
    private static bool CanSplit(int[] nums, int k, int capacity)
    {
        int workers = 1;
        int currentLoad = 0;

        foreach (int weight in nums)
        {
            if (currentLoad + weight > capacity)
            {
                workers++;          // open a new worker
                currentLoad = weight;
                if (workers > k) return false;
            }
            else
            {
                currentLoad += weight;
            }
        }

        return true;
    }

    public static int Solve(int[] nums, int k)
    {
        int lo = nums.Max();         // minimum possible answer
        int hi = nums.Sum();         // maximum possible answer

        while (lo < hi)
        {
            int mid = lo + (hi - lo) / 2;

            if (CanSplit(nums, k, mid))
                hi = mid;            // mid works — try smaller
            else
                lo = mid + 1;        // mid too small — need larger
        }

        return lo;  // lo == hi, the minimum valid maximum workload
    }

    public static void Main(string[] args)
    {
        Console.WriteLine("Example 1: " + Solve(new[] { 4, 8, 15, 7, 3 }, 3));
        // Expected: 15

        Console.WriteLine("Example 2: " + Solve(new[] { 6, 3, 9, 2, 1, 8 }, 2));
        // Expected: 18

        Console.WriteLine("Example 3: " + Solve(new[] { 1, 2, 3, 4, 5 }, 5));
        // Expected: 5  (k = length, each element alone)

        Console.WriteLine("Example 4: " + Solve(new[] { 10 }, 1));
        // Expected: 10

        Console.WriteLine("Example 5: " + Solve(new[] { 5, 5, 5, 5 }, 2));
        // Expected: 10
    }
}
