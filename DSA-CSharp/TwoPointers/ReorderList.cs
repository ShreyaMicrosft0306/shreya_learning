// ============================================================
// PROBLEM: Reorder List
// ============================================================
// Given the head of a singly linked list, reorder it in-place into
// the pattern: first→last→second→second-to-last→third→...
// You may not create a new list — modify the existing nodes.
//
// INPUT:
//   ListNode head — head of the linked list
//   Example: 5→4→3→2→1
//
// OUTPUT:
//   void — list is reordered in-place
//   Example: 5→1→4→2→3
//
// EDGE CASES:
//   - Single node [1]       → unchanged
//   - Two nodes [1,2]       → unchanged (already correct: 1→2)
//   - Even length [1,2,3,4] → [1,4,2,3]
//   - Odd length [1,2,3,4,5]→ [1,5,2,4,3]  (middle node stays in place)
//
// ============================================================
// HOW TO SOLVE — Simple Walkthrough
// ============================================================
// Key Insight:
//   The reordered list interleaves the first half with the REVERSED second half.
//
//   Original:  5 → 4 → 3 → 2 → 1
//   First half:    5 → 4 → 3
//   Second half reversed: 1 → 2
//   Interleaved:  5 → 1 → 4 → 2 → 3
//
//   Three steps: FIND MIDDLE → REVERSE second half → MERGE alternately.
//
// Step-by-step with example: 5→4→3→2→1
//   Nodes: n0(5)→n1(4)→n2(3)→n3(2)→n4(1)
//
//   STEP 1 — Find middle using slow/fast pointers:
//     slow=n0, fast=n0
//     Iter1: slow=n1(4), fast=n2(3)
//     Iter2: slow=n2(3), fast=n4(1)  → fast.next=null → STOP
//     Middle = n2(3)
//     Disconnect: n2.next = null
//     First half:  5→4→3→null
//     Second half: 2→1→null  (starting at old middle.next)
//
//   STEP 2 — Reverse second half:
//     Before: 2→1→null
//     After:  1→2→null
//
//   STEP 3 — Merge alternately:
//     first=5, second=1
//
//     Iter1: firstNext=4, secondNext=2
//            5.next=1,  1.next=4
//            first=4,   second=2
//            List: 5→1→4→3 (3 still attached to 4)
//
//     Iter2: firstNext=3, secondNext=null
//            4.next=2,  2.next=3
//            first=3,   second=null
//            List: 5→1→4→2→3
//
//     second=null → stop
//
//   Result: 5→1→4→2→3  ✓
//
// Example 2: 0→1→2
//   Middle = n1(1). Disconnect. First: 0→1, Second reversed: [2]
//   Merge: 0.next=2, 2.next=1 → 0→2→1  ✓
//
// WHY REVERSE THE SECOND HALF?
//   The pattern pulls from both ends simultaneously — front of first half
//   and front of reversed second half (which is the back of the original).
//   Reversing gives us O(1) access to each "back" node in order.
//
// WHY THIS APPROACH?
//   Naive: use an array or deque to access nodes by index → O(n) space.
//   Three-step in-place (this): find middle + reverse + merge → O(n) time, O(1) space.
//
// ============================================================
// TIME & SPACE COMPLEXITY
// ============================================================
//   n = number of nodes
//
//   Time:  O(n)
//     - Finding middle: O(n/2)
//     - Reversing second half: O(n/2)
//     - Merging two halves: O(n)
//     - Total: O(n)
//
//   Space: O(1)
//     - All operations done by rewiring existing node pointers
//     - Only a fixed number of pointer variables used
// ============================================================

public class ReorderList
{
    private static ListNode FindMiddle(ListNode head)
    {
        ListNode slow = head;
        ListNode fast = head;
        while (fast.next != null && fast.next.next != null)
        {
            slow = slow.next;
            fast = fast.next.next;
        }
        return slow;  // slow is the middle node
    }

    private static ListNode Reverse(ListNode head)
    {
        ListNode prev = null;
        ListNode curr = head;
        while (curr != null)
        {
            ListNode next = curr.next;
            curr.next = prev;
            prev = curr;
            curr = next;
        }
        return prev;
    }

    private static void Merge(ListNode first, ListNode second)
    {
        while (second != null)
        {
            ListNode firstNext  = first.next;
            ListNode secondNext = second.next;

            first.next  = second;           // insert second after first
            if (firstNext != null)
                second.next = firstNext;    // attach remaining first half

            first  = firstNext;
            second = secondNext;
        }
    }

    public static void Solve(ListNode head)
    {
        if (head == null || head.next == null) return;

        // Step 1: Find middle and split into two halves
        ListNode middle     = FindMiddle(head);
        ListNode secondHead = middle.next;
        middle.next = null;                  // disconnect the two halves

        // Step 2: Reverse the second half
        secondHead = Reverse(secondHead);

        // Step 3: Merge the two halves alternately
        Merge(head, secondHead);
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

        var list1 = Build(new[] { 5, 4, 3, 2, 1 });
        Solve(list1);
        Console.WriteLine("Example 1: " + Print(list1));
        // Expected: 5→1→4→2→3

        var list2 = Build(new[] { 0, 1, 2 });
        Solve(list2);
        Console.WriteLine("Example 2: " + Print(list2));
        // Expected: 0→2→1

        var list3 = Build(new[] { 1, 2, 3, 4 });
        Solve(list3);
        Console.WriteLine("Example 3: " + Print(list3));
        // Expected: 1→4→2→3
    }
}
