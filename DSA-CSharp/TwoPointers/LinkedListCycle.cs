// ============================================================
// PROBLEM: Linked List Cycle Detection
// ============================================================
// Given the head of a linked list, return true if the list contains
// a cycle (a node whose next pointer points back to a previous node),
// and false otherwise. Do not modify the list.
//
// INPUT:
//   ListNode head — reference to the head of a linked list
//   Example: 1 → 2 → 3 → 4 → 5 → (back to node 2)  [cycle]
//
// OUTPUT:
//   bool — true if cycle exists, false otherwise
//   Example: true
//
// EDGE CASES:
//   - Empty list (head=null)              → false
//   - Single node, no self-loop [1]→null → false
//   - Single node pointing to itself [1]→[1] → true
//   - Two nodes, no cycle [1]→[2]→null   → false
//   - Cycle at the very end (tail→head)  → true
//
// ============================================================
// HOW TO SOLVE — Simple Walkthrough
// ============================================================
// Key Insight (Floyd's Cycle Detection — Slow & Fast Pointers):
//   Use two pointers: slow moves 1 step at a time, fast moves 2 steps.
//   If there is NO cycle, fast reaches null and we return false.
//   If there IS a cycle, fast laps slow inside the loop — they will
//   eventually land on the SAME node.
//
// Think of it like a circular running track:
//   Two runners start together. The faster one will eventually lap
//   the slower one and they'll be at the same point again.
//   On a straight road (no cycle), the fast runner just runs off the end.
//
// Step-by-step with example: 1→2→3→4→5→(back to node 2)
//   Nodes: n0(1) → n1(2) → n2(3) → n3(4) → n4(5) → n1(2)...
//
//   Start: slow=n0, fast=n0
//
//   Step 1: slow=n1(2),  fast=n2(3)
//   Step 2: slow=n2(3),  fast=n4(5)   [fast: n2→n3→n4]
//   Step 3: slow=n3(4),  fast=n2(3)   [fast: n4→n1→n2]
//   Step 4: slow=n4(5),  fast=n4(5)   [fast: n2→n3→n4]
//           slow == fast → CYCLE DETECTED → return true
//
// WHY DO THEY ALWAYS MEET (and not skip past each other)?
//   Inside the cycle, fast gains exactly 1 step on slow per iteration.
//   So the gap between them decreases by 1 each step.
//   Eventually the gap reaches 0 — they are at the same node.
//   Fast can never "jump over" slow by exactly 1 step without meeting.
//
// WHY THIS APPROACH?
//   HashSet approach: store every visited node, check if current was seen before.
//   → O(n) time but O(n) space — stores all nodes in memory.
//   Floyd's slow/fast (this): no extra storage needed.
//   → O(n) time, O(1) space — only two pointer variables.
//
// ============================================================
// TIME & SPACE COMPLEXITY
// ============================================================
//   n = number of nodes in the linked list
//
//   Time:  O(n)
//     - No cycle: fast reaches null in n/2 steps → O(n)
//     - With cycle: fast laps slow within one full cycle traversal → O(n)
//     - Total: O(n)
//
//   Space: O(1)
//     - Only two pointer variables (slow, fast) regardless of list size
// ============================================================

public class ListNode
{
    public int val;
    public ListNode next;
    public ListNode(int val = 0, ListNode next = null)
    {
        this.val = val;
        this.next = next;
    }
}

public class LinkedListCycle
{
    public static bool Solve(ListNode head)
    {
        ListNode slow = head;
        ListNode fast = head;

        while (fast != null && fast.next != null)
        {
            slow = slow.next;        // move 1 step
            fast = fast.next.next;   // move 2 steps

            if (slow == fast)
                return true;         // pointers met — cycle exists
        }

        return false;                // fast hit null — no cycle
    }

    public static void Main(string[] args)
    {
        // Example 1: 1→2→3→4→5→(back to node at index 1)  → true
        ListNode n1 = new ListNode(1);
        ListNode n2 = new ListNode(2);
        ListNode n3 = new ListNode(3);
        ListNode n4 = new ListNode(4);
        ListNode n5 = new ListNode(5);
        n1.next = n2; n2.next = n3; n3.next = n4; n4.next = n5; n5.next = n2;
        Console.WriteLine("Example 1 (cycle):    " + Solve(n1));
        // Expected: true

        // Example 2: 1→2→3→null  → false
        ListNode a = new ListNode(1);
        ListNode b = new ListNode(2);
        ListNode c = new ListNode(3);
        a.next = b; b.next = c;
        Console.WriteLine("Example 2 (no cycle): " + Solve(a));
        // Expected: false

        // Example 3: single node pointing to itself  → true
        ListNode lone = new ListNode(1);
        lone.next = lone;
        Console.WriteLine("Example 3 (self-loop): " + Solve(lone));
        // Expected: true
    }
}
