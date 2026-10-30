// ============================================================
// PROBLEM: Remove Nth Node From End of List
// ============================================================
// Given the head of a singly linked list and an integer n, remove
// the nth node from the END of the list and return the head.
// n is guaranteed to be valid (1 ≤ n ≤ list length).
//
// INPUT:
//   ListNode head — head of the linked list
//   int n         — position from the end to remove (1-indexed)
//   Example: head = 5→4→3→2→1,  n = 2
//
// OUTPUT:
//   ListNode — head of the modified list
//   Example: 5→4→3→1   (node with value 2 removed)
//
// EDGE CASES:
//   - n = list length    → remove the head → return head.next
//   - n = 1              → remove the tail → second-to-last.next = null
//   - Single node, n=1   → return null
//   - Two nodes, n=2     → remove head → return remaining node
//
// ============================================================
// HOW TO SOLVE — Simple Walkthrough
// ============================================================
// Key Insight (Gap technique with two pointers):
//   If we move fast n steps ahead of slow, then when fast reaches
//   the LAST node, slow is exactly at the node BEFORE the one to delete.
//   This lets us find the target in a single pass without knowing the list length.
//
//   Use a DUMMY node before head to handle the edge case of removing
//   the head — slow needs a node to sit on "before" the target.
//
// Step-by-step with example: 5→4→3→2→1,  n=2
//   dummy→5→4→3→2→1
//
//   Step 1 — Move fast n=2 steps ahead of slow:
//     Start: slow=dummy, fast=dummy
//     Step1: fast=5
//     Step2: fast=4
//     Gap between slow and fast = 2 nodes
//
//   Step 2 — Move both together until fast.next = null:
//     fast.next=3 (not null): slow=5,  fast=3
//     fast.next=2 (not null): slow=4,  fast=2
//     fast.next=1 (not null): slow=3,  fast=1
//     fast.next=null → STOP
//
//   Step 3 — slow is just BEFORE the node to delete:
//     slow=3,  slow.next=2  ← this is the 2nd from end
//     slow.next = slow.next.next = 1
//
//   Result: dummy→5→4→3→1  → return dummy.next = 5→4→3→1  ✓
//
// EXAMPLE 2 — removing the head: n=5
//   dummy→5→4→3→2→1
//
//   Step 1 — Move fast 5 steps:
//     fast lands on node 1 (last node)
//     slow stays at dummy
//
//   Step 2 — fast.next=null → STOP immediately (no movement needed)
//
//   Step 3 — slow=dummy, slow.next=5 (the head)
//     slow.next = slow.next.next = 4
//     Result: dummy→4→3→2→1  → return 4→3→2→1  ✓
//     (dummy node makes removing the head seamless — no special case needed)
//
// WHY A DUMMY NODE?
//   Without it, removing the head requires a special case:
//     if (fast reaches null during step 1) return head.next;
//   The dummy node gives slow a place to sit before the head,
//   so every deletion is handled identically as slow.next = slow.next.next.
//
// WHY THIS APPROACH?
//   Naive: find list length L, then walk to node (L-n) → two passes → O(L).
//   Two-pointer gap (this): single pass, same O(L) time but cleaner and
//   no need to count the list length first.
//
// ============================================================
// TIME & SPACE COMPLEXITY
// ============================================================
//   L = number of nodes in the list
//
//   Time:  O(L)
//     - fast moves at most L steps (n steps + remaining) → O(L)
//     - slow moves at most L-n steps → O(L)
//     - Total: O(L)
//
//   Space: O(1)
//     - Only three pointer variables: dummy, slow, fast
// ============================================================

public class RemoveNthNodeFromEnd
{
    public static ListNode Solve(ListNode head, int n)
    {
        // Dummy node before head — handles removing the head cleanly
        ListNode dummy = new ListNode(0);
        dummy.next = head;

        ListNode slow = dummy;
        ListNode fast = dummy;

        // Step 1: Move fast n steps ahead
        for (int i = 0; i < n; i++)
            fast = fast.next;

        // Step 2: Move both until fast reaches the last node
        while (fast.next != null)
        {
            slow = slow.next;
            fast = fast.next;
        }

        // Step 3: slow is just before the target — skip it
        slow.next = slow.next.next;

        return dummy.next;
    }

    public static void Main(string[] args)
    {
        // Helper to build list from array
        static ListNode Build(int[] vals)
        {
            ListNode dummy = new ListNode(0);
            ListNode curr = dummy;
            foreach (int v in vals) { curr.next = new ListNode(v); curr = curr.next; }
            return dummy.next;
        }

        // Helper to print list
        static string Print(ListNode h)
        {
            var parts = new System.Collections.Generic.List<string>();
            while (h != null) { parts.Add(h.val.ToString()); h = h.next; }
            return string.Join("→", parts);
        }

        Console.WriteLine("Example 1 (n=2): " + Print(Solve(Build(new[] {5,4,3,2,1}), 2)));
        // Expected: 5→4→3→1

        Console.WriteLine("Example 2 (n=5): " + Print(Solve(Build(new[] {5,4,3,2,1}), 5)));
        // Expected: 4→3→2→1

        Console.WriteLine("Example 3 (n=1): " + Print(Solve(Build(new[] {5,4,3,2,1}), 1)));
        // Expected: 5→4→3→2

        Console.WriteLine("Example 4 (n=1, single node): " + Print(Solve(Build(new[] {1}), 1)));
        // Expected: (empty)
    }
}
