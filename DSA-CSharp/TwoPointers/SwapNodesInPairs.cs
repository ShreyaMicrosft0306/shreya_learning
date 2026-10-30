// ============================================================
// PROBLEM: Swap Nodes in Pairs
// ============================================================
// Given the head of a singly linked list, swap every two adjacent
// nodes and return the new head. Only node pointers may change —
// values inside nodes must not be modified.
//
// INPUT:
//   ListNode head — head of the linked list
//   Example: 5→4→3→2→1
//
// OUTPUT:
//   ListNode — head of the reordered list
//   Example: 4→5→2→3→1
//
// EDGE CASES:
//   - Empty list null       → return null
//   - Single node [1]       → return [1] unchanged
//   - Two nodes [1,2]       → return [2,1]
//   - Odd length [1,2,3]    → last node stays: [2,1,3]
//   - Even length [1,2,3,4] → all swapped: [2,1,4,3]
//
// ============================================================
// HOW TO SOLVE — Simple Walkthrough
// ============================================================
// Key Insight:
//   Use a dummy node so every pair has a node BEFORE it to attach to.
//   Keep a `curr` pointer at the node BEFORE each pair.
//   For each pair (first=curr.next, second=curr.next.next):
//     1. curr → second         (connect into start of pair)
//     2. first → second.next   (first jumps over second)
//     3. second → first        (second points back to first)
//   Then advance curr to first (now the tail of the swapped pair).
//
// Step-by-step with example: 5→4→3→2→1
//   dummy→5→4→3→2→1
//
//   Pair 1 (curr=dummy, first=5, second=4):
//     curr.next  = second(4)       → dummy→4   [5 still linked to 3]
//     first.next = second.next(3)  → 5→3
//     second.next = first(5)       → 4→5
//     State: dummy→4→5→3→2→1
//     curr = first = 5
//
//   Pair 2 (curr=5, first=3, second=2):
//     curr.next  = second(2)       → 5→2        [3 still linked to 1]
//     first.next = second.next(1)  → 3→1
//     second.next = first(3)       → 2→3
//     State: dummy→4→5→2→3→1
//     curr = first = 3
//
//   Pair 3: curr.next=1, curr.next.next=null → STOP (no full pair left)
//
//   Return dummy.next → 4→5→2→3→1  ✓
//
// WHY THE ORDER OF THE THREE POINTER CHANGES MATTERS:
//   The three assignments must happen in this exact order:
//     curr.next  = second           ← must be first (saves second safely)
//     first.next = second.next      ← must be second (before second.next is overwritten)
//     second.next = first           ← must be last (now safe to overwrite)
//   Doing them in a different order would lose a reference before it's saved.
//
// WHY A DUMMY NODE?
//   The head always changes (first pair swaps it). Instead of handling
//   the head as a special case, dummy.next points to the true head.
//   Every pair always has a `curr` node before it — uniform handling.
//
// WHY THIS APPROACH?
//   Value-swap approach: just swap node values, not pointers. Simple but
//   violates the problem constraint (values must not be modified).
//   Pointer-swap with dummy (this): correct, clean, O(n) time, O(1) space.
//
// ============================================================
// TIME & SPACE COMPLEXITY
// ============================================================
//   n = number of nodes
//
//   Time:  O(n)
//     - Each node is visited exactly once as part of a pair: O(n)
//
//   Space: O(1)
//     - Only a fixed number of pointer variables (dummy, curr, first, second)
// ============================================================

public class SwapNodesInPairs
{
    public static ListNode Solve(ListNode head)
    {
        ListNode dummy = new ListNode(0);
        dummy.next = head;
        ListNode curr = dummy;

        while (curr.next != null && curr.next.next != null)
        {
            ListNode first  = curr.next;
            ListNode second = curr.next.next;

            // Wire the three pointer changes in order
            curr.next   = second;        // 1. skip over first, attach to second
            first.next  = second.next;   // 2. first jumps past second
            second.next = first;         // 3. second points back to first

            // Advance curr to the tail of the swapped pair
            curr = first;
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

        Console.WriteLine("Example 1: " + Print(Solve(Build(new[] { 5, 4, 3, 2, 1 }))));
        // Expected: 4→5→2→3→1

        Console.WriteLine("Example 2: " + Print(Solve(Build(new[] { 1, 2, 3, 4 }))));
        // Expected: 2→1→4→3

        Console.WriteLine("Example 3: " + Print(Solve(Build(new[] { 1 }))));
        // Expected: 1

        Console.WriteLine("Example 4: " + Print(Solve(Build(new[] { 1, 2 }))));
        // Expected: 2→1
    }
}
