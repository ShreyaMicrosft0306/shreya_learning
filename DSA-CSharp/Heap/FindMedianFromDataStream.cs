// ============================================================
// PROBLEM: Find Median from Data Stream
// ============================================================
// Given initial readings nums and a stream of incoming values adds,
// return the running median after each new value is inserted.
// For even total count, median = average of the two middle values.
//
// INPUT:
//   int[] nums — initial readings already in the stream
//   int[] adds — new values arriving one at a time
//   Example: nums = [5, 2, 8],  adds = [3, 10, 4]
//
// OUTPUT:
//   double[] — median after each value in adds is inserted
//   Example: [4, 5, 4.5]
//
// EDGE CASES:
//   - nums is empty, adds = [1, 2]         → [1, 1.5]
//   - Single element after insert           → that element is median
//   - All equal values [3,3,3], add 3      → median = 3
//   - Even count → exact average of middle two (may be fractional)
//
// ============================================================
// HOW TO SOLVE — Simple Walkthrough
// ============================================================
// Key Insight (Two Heaps):
//   Split all values into two halves:
//     maxHeap — lower half  (max-heap, top = largest of the smaller values)
//     minHeap — upper half  (min-heap, top = smallest of the larger values)
//   Keep them balanced: maxHeap.Count == minHeap.Count  (even total)
//                   or  maxHeap.Count == minHeap.Count + 1 (odd total)
//
//   Median:
//     Odd total  → maxHeap.Peek()                        (middle element)
//     Even total → (maxHeap.Peek() + minHeap.Peek()) / 2 (average of middle two)
//
// Insert rule (always preserves the boundary maxHeap.top ≤ minHeap.top):
//   1. Push to maxHeap.
//   2. Pop maxHeap's max → push it to minHeap.       (fixes any boundary violation)
//   3. If minHeap now has more elements, pop its min → push to maxHeap. (rebalance)
//
// Step-by-step with example: nums = [5, 2, 8],  adds = [3, 10, 4]
//
//   After inserting initial nums [5, 2, 8]:
//     Insert 5: maxHeap=[5]         minHeap=[]   → rebalance → maxHeap=[5]  minHeap=[]
//     Insert 2: maxHeap=[5,2]       minHeap=[]   → move 5   → maxHeap=[2]   minHeap=[5]
//     Insert 8: maxHeap=[8,2]       minHeap=[5]  → move 8   → maxHeap=[2]   minHeap=[5,8]
//                                                → rebalance → maxHeap=[5,2] minHeap=[8]
//     State: lower=[5,2]  upper=[8]   sorted view: 2|5|8   median=5
//
//   Add 3:
//     Push 3 → maxHeap=[5,3,2]  move 5 → maxHeap=[3,2]  minHeap=[5,8]
//     counts equal (2==2) → no rebalance
//     Median: (3+5)/2 = 4  ✓
//
//   Add 10:
//     Push 10 → maxHeap=[10,3,2]  move 10 → maxHeap=[3,2]  minHeap=[5,8,10]
//     minHeap bigger (3>2) → move 5 → maxHeap=[5,3,2]  minHeap=[8,10]
//     Median: maxHeap top = 5  ✓
//
//   Add 4:
//     Push 4 → maxHeap=[5,4,3,2]  move 5 → maxHeap=[4,3,2]  minHeap=[5,8,10]
//     counts equal (3==3) → no rebalance
//     Median: (4+5)/2 = 4.5  ✓
//
// WHY ALWAYS PUSH TO maxHeap FIRST, THEN MOVE TO minHeap?
//   Directly deciding which heap to push into requires comparing to both
//   heap tops, and still needs rebalancing. The push-then-move trick
//   handles all cases in two steps: it auto-fixes any boundary violation
//   (a large value pushed to maxHeap is immediately forwarded to minHeap),
//   then the size check handles balance.
//
// WHY THIS APPROACH?
//   Naive: keep a sorted list, insert in O(n), read middle in O(1) → O(n) per insert.
//   Two heaps (this): insert in O(log n), read median in O(1) → optimal.
//
// ============================================================
// TIME & SPACE COMPLEXITY
// ============================================================
//   n = total elements seen so far
//
//   Time per Insert:  O(log n)
//     - Each heap push/pop is O(log n) (heap size ≤ n)
//     - At most 3 heap operations per insert
//
//   Time GetMedian: O(1)
//     - Just peek at one or two heap tops
//
//   Space: O(n)
//     - Both heaps together hold all n elements
// ============================================================

public class FindMedianFromDataStream
{
    private readonly PriorityQueue<int, int> maxHeap = new(); // lower half, using negative priority
    private readonly PriorityQueue<int, int> minHeap = new(); // upper half

    public void Insert(int num)
    {
        if (maxHeap.Count == 0 || num <= maxHeap.Peek())
        {
            maxHeap.Enqueue(num, -num);
        }
        else
        {
            minHeap.Enqueue(num, num);
        }

        // Rebalance so the lower half is never more than 1 larger than the upper half.
        if (maxHeap.Count > minHeap.Count + 1)
        {
            maxHeap.TryDequeue(out int maxVal, out _);
            minHeap.Enqueue(maxVal, maxVal);
        }
        else if (minHeap.Count > maxHeap.Count)
        {
            minHeap.TryDequeue(out int minVal, out _);
            maxHeap.Enqueue(minVal, -minVal);
        }
    }

    public double GetMedian()
    {
        if (maxHeap.Count > minHeap.Count)
        {
            maxHeap.TryPeek(out int top, out _);
            return top;
        }

        maxHeap.TryPeek(out int maxTop, out _);
        minHeap.TryPeek(out int minTop, out _);
        return (maxTop + minTop) / 2.0;
    }

    public static double[] Solve(int[] nums, int[] adds)
    {
        var finder = new FindMedianFromDataStream();
        foreach (int n in nums) finder.Insert(n);

        var result = new double[adds.Length];
        for (int i = 0; i < adds.Length; i++)
        {
            finder.Insert(adds[i]);
            result[i] = finder.GetMedian();
        }
        return result;
    }

    public static void Main(string[] args)
    {
        static string Format(double[] arr) =>
            "[" + string.Join(", ", arr) + "]";

        Console.WriteLine("Example 1: " + Format(Solve(new[] { 5, 2, 8 }, new[] { 3, 10, 4 })));
        // Expected: [4, 5, 4.5]

        Console.WriteLine("Example 2: " + Format(Solve(Array.Empty<int>(), new[] { 1, 2 })));
        // Expected: [1, 1.5]

        Console.WriteLine("Example 3: " + Format(Solve(new[] { 1 }, new[] { 2, 3 })));
        // Expected: [1.5, 2]

        Console.WriteLine("Example 4: " + Format(Solve(new[] { 3, 3, 3 }, new[] { 3 })));
        // Expected: [3]
    }
}
