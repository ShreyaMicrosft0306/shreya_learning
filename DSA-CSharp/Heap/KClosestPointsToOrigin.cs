// ============================================================
// PROBLEM: K Closest Points to Origin
// ============================================================
// Given a list of 2D points and an integer k, return the k points
// closest to the origin (0, 0). Distance is Euclidean. Order does not matter.
//
// INPUT:
//   int[][] points — list of [x, y] coordinates
//   int k          — number of closest points to return
//   Example: points = [[3,4],[2,2],[1,1],[0,0],[5,5]],  k = 3
//
// OUTPUT:
//   int[][] — any k points closest to origin (order does not matter)
//   Example: [[2,2],[1,1],[0,0]]
//
// EDGE CASES:
//   - k = points.Length   → return all points
//   - k = 1               → return the single closest point
//   - Multiple points equidistant → return any k of them
//   - Origin [0,0] in list → always included if k ≥ 1
//
// ============================================================
// HOW TO SOLVE — Simple Walkthrough
// ============================================================
// Key Insight (Max-Heap of size k):
//   Keep a running collection of the k closest points seen so far.
//   Use a max-heap so the FARTHEST of the k candidates sits at the top.
//   For each new point: if it is closer than the current farthest,
//   evict the farthest and add the new point.
//   At the end, the heap holds exactly the k closest points.
//
// WHY SQUARED DISTANCE?
//   √(x²+y²) < √(a²+b²)  iff  x²+y² < a²+b²
//   Comparing x²+y² avoids floating-point sqrt — same ordering, cheaper.
//
// Step-by-step with example: points = [[3,4],[2,2],[1,1],[0,0],[5,5]], k = 3
//   Squared distances: [3,4]=25, [2,2]=8, [1,1]=2, [0,0]=0, [5,5]=50
//
//   [3,4] dist²=25 → heap size 1 ≤ 3, push   heap: {25}
//   [2,2] dist²=8  → heap size 2 ≤ 3, push   heap: {25,8}
//   [1,1] dist²=2  → heap size 3 ≤ 3, push   heap: {25,8,2}
//   [0,0] dist²=0  → push → size 4 > 3 → pop max(25) → evict [3,4]
//                                                heap: {8,2,0}
//   [5,5] dist²=50 → push → size 4 > 3 → pop max(50) → evict [5,5]
//                                                heap: {8,2,0}
//
//   Return remaining: [2,2],[1,1],[0,0]  ✓
//
// WHY A MAX-HEAP AND NOT MIN-HEAP?
//   We want to KEEP the k smallest. With a max-heap the worst candidate
//   is always on top — easy to compare and evict when a better point arrives.
//   A min-heap would require popping k times at the end (O(k log n) extra work).
//
// WHY THIS APPROACH?
//   Sort all points by distance: O(n log n) — simple but does more work than needed.
//   Max-heap of size k (this): O(n log k) — only maintains k entries at a time.
//   When k << n this is significantly faster.
//
// ============================================================
// TIME & SPACE COMPLEXITY
// ============================================================
//   n = number of points
//
//   Time:  O(n log k)
//     - For each of n points: one push + one conditional pop, each O(log k)
//     - Total: O(n log k)
//
//   Space: O(k)
//     - Heap holds at most k+1 points at any moment
// ============================================================

public class KClosestPointsToOrigin
{
    public static int[][] Solve(int[][] points, int k)
    {
        // Max-heap: negate dist² so PriorityQueue (min by default) acts as max-heap
        var maxHeap = new PriorityQueue<int[], int>();

        foreach (int[] point in points)
        {
            int dist2 = point[0] * point[0] + point[1] * point[1];
            maxHeap.Enqueue(point, -dist2);   // negate → largest dist² dequeued first

            if (maxHeap.Count > k)
                maxHeap.Dequeue();            // evict the farthest point
        }

        return maxHeap.UnorderedItems
                      .Select(item => item.Element)
                      .ToArray();
    }

    public static void Main(string[] args)
    {
        static string Format(int[][] pts) =>
            "[" + string.Join(", ", pts.Select(p => $"[{p[0]},{p[1]}]")) + "]";

        Console.WriteLine("Example 1: " + Format(Solve(new[] {
            new[]{3,4}, new[]{2,2}, new[]{1,1}, new[]{0,0}, new[]{5,5}
        }, 3)));
        // Expected: any 3 of [[2,2],[1,1],[0,0]]

        Console.WriteLine("Example 2: " + Format(Solve(new[] {
            new[]{1,3}, new[]{-2,2}
        }, 1)));
        // Expected: [[-2,2]]  (dist²=8 vs 10)

        Console.WriteLine("Example 3: " + Format(Solve(new[] {
            new[]{0,0}, new[]{1,1}
        }, 2)));
        // Expected: [[0,0],[1,1]]  (k = all points)
    }
}
