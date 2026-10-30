// ============================================================
// PROBLEM: Merge K Sorted Lists
// ============================================================
// Given k sorted linked lists, merge them all into one sorted linked
// list and return its head.
//
// INPUT:
//   ListNode[] lists — array of k sorted linked list heads
//   Example: lists = [3→4→6, 2→3→5, -1→6]
//
// OUTPUT:
//   ListNode — head of the merged sorted linked list
//   Example: -1→2→3→3→4→5→6→6
//
// EDGE CASES:
//   - Empty input []              → return null
//   - Some lists are null         → skip them
//   - Single list [1→2→3]         → return it unchanged
//   - All lists are single nodes  → return them merged and sorted
//
// ============================================================
// HOW TO SOLVE — Simple Walkthrough
// ============================================================
// Key Insight (Min-Heap):
//   The next node to attach is always the smallest current head
//   across all k lists. Use a min-heap to always retrieve that
//   minimum in O(log k) instead of scanning all k heads each time.
//
//   Push each list's head into the heap.
//   Repeatedly pop the smallest node, attach it to the result,
//   then push that node's next (if any) back into the heap.
//
// Step-by-step with example: lists = [3→4→6, 2→3→5, -1→6]
//
//   Initial heap (by value): {-1, 2, 3}
//
//   Pop -1  → attach,  push  6  (next of -1)   result: -1→
//                                               heap: {2, 3, 6}
//
//   Pop  2  → attach,  push  3  (next of  2)   result: -1→2→
//                                               heap: {3, 3, 6}
//
//   Pop  3  → attach,  push  4  (next of  3)   result: -1→2→3→
//                                               heap: {3, 4, 6}
//
//   Pop  3  → attach,  push  5  (next of  3)   result: -1→2→3→3→
//                                               heap: {4, 5, 6}
//
//   Pop  4  → attach,  push  6  (next of  4)   result: -1→2→3→3→4→
//                                               heap: {5, 6, 6}
//
//   Pop  5  → attach,  no next                 result: -1→2→3→3→4→5→
//                                               heap: {6, 6}
//
//   Pop  6  → attach,  no next                 result: -1→2→3→3→4→5→6→
//                                               heap: {6}
//
//   Pop  6  → attach,  no next                 result: -1→2→3→3→4→5→6→6
//                                               heap: {}
//
//   Heap empty → return dummy.next  ✓
//
// WHY THIS APPROACH?
//   Naive (merge two at a time, left to right): each of the k-1 merges
//   touches all accumulated nodes → O(n * k) in the worst case.
//   Min-heap (this): every node is pushed and popped exactly once,
//   heap size stays ≤ k → O(n log k). Much faster when k is large.
//
// ============================================================
// TIME & SPACE COMPLEXITY
// ============================================================
//   n = total nodes across all k lists
//
//   Time:  O(n log k)
//     - Each of n nodes is pushed into and popped from the heap once
//     - Each heap operation costs O(log k) since heap holds at most k nodes
//     - Total: O(n log k)
//
//   Space: O(k)
//     - Heap holds at most one node per list (at most k nodes at a time)
// ============================================================

public class MergeKSortedLists
{
    public static ListNode Solve(ListNode[] lists)
    {
        var minHeap = new PriorityQueue<ListNode, int>();

        foreach (ListNode head in lists)
            if (head != null)
                minHeap.Enqueue(head, head.val);

        ListNode dummy = new ListNode(0);
        ListNode curr = dummy;

        while (minHeap.Count > 0)
        {
            ListNode node = minHeap.Dequeue();
            curr.next = node;
            curr = node;

            if (node.next != null)
                minHeap.Enqueue(node.next, node.next.val);
        }

        return dummy.next;
    }

    public static void Main(string[] args)
    {
        static ListNode Build(int[] vals)
        {
            ListNode dummy = new ListNode(0);
            ListNode curr = dummy;
            foreach (int v in vals) { curr.next = new ListNode(v); curr = curr.next; }
            return dummy.next;
        }

        static string Print(ListNode h)
        {
            var parts = new System.Collections.Generic.List<string>();
            while (h != null) { parts.Add(h.val.ToString()); h = h.next; }
            return string.Join("→", parts);
        }

        Console.WriteLine("Example 1: " + Print(Solve(new[] {
            Build(new[] { 3, 4, 6 }),
            Build(new[] { 2, 3, 5 }),
            Build(new[] { -1, 6 })
        })));
        // Expected: -1→2→3→3→4→5→6→6

        Console.WriteLine("Example 2: " + Print(Solve(new[] {
            Build(new[] { 1, 4, 7 }),
            Build(new[] { 2, 5, 8 }),
            Build(new[] { 3, 6, 9 })
        })));
        // Expected: 1→2→3→4→5→6→7→8→9

        Console.WriteLine("Example 3: " + Print(Solve(new[] {
            Build(new[] { 1 })
        })));
        // Expected: 1

        Console.WriteLine("Example 4: " + Print(Solve(Array.Empty<ListNode>())));
        // Expected: (empty)
    }
}
