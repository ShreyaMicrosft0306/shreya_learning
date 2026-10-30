// ============================================================
// PROBLEM: Minimum Harvest Rate (Koko Eating Bananas variant)
// ============================================================
// Bobby harvests apples at a fixed rate k (apples/hour), one tree
// at a time. If a tree has fewer apples than k, the remaining time
// in that hour is wasted. Find the MINIMUM rate k that finishes
// all trees in at most h hours.
//
// INPUT:
//   int[] apples — apples on each tree
//   int h        — total hours available
//   Example: apples = [3, 6, 7],  h = 8
//
// OUTPUT:
//   int — minimum valid harvest rate
//   Example: 3
//
// EDGE CASES:
//   - h = apples.Length → must finish each tree in exactly 1 hour → rate = max(apples)
//   - h is very large   → rate can be 1 (plenty of time)
//   - Single tree [10], h=3 → rate = ceil(10/3) = 4
//   - All trees have 1 apple → rate = 1 always works
//
// ============================================================
// HOW TO SOLVE — Simple Walkthrough
// ============================================================
// Key Insight (Binary Search on the Answer):
//   The answer (minimum rate) lies in the range [1, max(apples)].
//   Instead of trying every rate from 1 upward (linear), binary search
//   this range. For any rate `mid`, check if it finishes in ≤ h hours.
//   If YES → try a slower rate (search left half).
//   If NO  → need a faster rate (search right half).
//
// Hours needed for one tree with `a` apples at rate `k`:
//   hours = ceil(a / k) = (a + k - 1) / k    (integer ceiling division)
//
//   e.g. 7 apples at rate 3: ceil(7/3) = 3 hours
//        6 apples at rate 3: ceil(6/3) = 2 hours
//
// Step-by-step with example: apples = [3, 6, 7],  h = 8
//
//   Search range: lo=1, hi=max(apples)=7
//
//   mid=4: ceil(3/4)+ceil(6/4)+ceil(7/4) = 1+2+2 = 5 ≤ 8 → VALID → hi=4
//   mid=2: ceil(3/2)+ceil(6/2)+ceil(7/2) = 2+3+4 = 9 > 8 → TOO SLOW → lo=3
//   mid=3: ceil(3/3)+ceil(6/3)+ceil(7/3) = 1+2+3 = 6 ≤ 8 → VALID → hi=3
//
//   lo=3, hi=3 → lo==hi → return 3
//
// WHY [1, max(apples)] AS SEARCH RANGE?
//   Rate of 1: always valid eventually but may take too long.
//   Rate of max(apples): finishes the biggest tree in 1 hour — the fastest
//   we'd ever need. No rate higher than this is ever more useful.
//   The answer MUST exist somewhere in this range.
//
// WHY hi = mid (not mid-1) WHEN VALID?
//   `mid` itself is a valid rate — it might be the answer. Setting hi=mid
//   keeps it in the search window. Setting hi=mid-1 would discard the answer.
//
// WHY THIS APPROACH?
//   Brute force: try every rate from 1 to max(apples), first valid one is answer.
//   → O(m * n) where m = max(apples), n = number of trees. Slow for large m.
//   Binary search (this): halves the search space each step.
//   → O(n log m). Much faster.
//
// ============================================================
// TIME & SPACE COMPLEXITY
// ============================================================
//   n = number of trees,  m = max(apples)
//
//   Time:  O(n log m)
//     - Binary search runs O(log m) iterations
//     - Each iteration checks all n trees: O(n)
//     - Total: O(n log m)
//
//   Space: O(1)
//     - Only a fixed number of integer variables: lo, hi, mid, hours
// ============================================================

public class MinHarvestRate
{
    // Returns true if Bobby can finish all trees at this rate within h hours
    private static bool CanFinish(int[] apples, int rate, int h)
    {
        int totalHours = 0;
        foreach (int a in apples)
            totalHours += (a + rate - 1) / rate;  // ceiling division
        return totalHours <= h;
    }

    public static int Solve(int[] apples, int h)
    {
        int lo = 1;
        int hi = apples.Max();  // fastest we'd ever need

        while (lo < hi)
        {
            int mid = lo + (hi - lo) / 2;  // avoid integer overflow

            if (CanFinish(apples, mid, h))
                hi = mid;   // mid works — try slower (search left)
            else
                lo = mid + 1;  // mid too slow — need faster (search right)
        }

        return lo;  // lo == hi, the minimum valid rate
    }

    public static void Main(string[] args)
    {
        Console.WriteLine("Example 1: " + Solve(new[] { 3, 6, 7 }, 8));
        // Expected: 3

        Console.WriteLine("Example 2: " + Solve(new[] { 25, 9, 23, 8, 3 }, 5));
        // Expected: 25

        Console.WriteLine("Example 3: " + Solve(new[] { 1, 1, 1, 1 }, 10));
        // Expected: 1  (plenty of time)

        Console.WriteLine("Example 4: " + Solve(new[] { 10 }, 3));
        // Expected: 4  (ceil(10/3)=4, need 3 hours at rate 4)
    }
}
