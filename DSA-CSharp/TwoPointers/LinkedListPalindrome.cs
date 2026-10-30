// ============================================================
// PROBLEM: Linked List Palindrome
// ============================================================
// Given the head of a singly linked list, return true if the values
// read the same forwards and backwards. An empty list is a palindrome.
//
// INPUT:
//   ListNode head — head of a singly linked list
//   Example: 5 → 4 → 3 → 4 → 5
//
// OUTPUT:
//   bool — true if palindrome, false otherwise
//   Example: true   (same forwards and backwards)
//
// EDGE CASES:
//   - Empty list null           → true
//   - Single node [1]           → true  (trivially palindrome)
//   - Two same nodes [1,1]      → true
//   - Two different nodes [1,2] → false
//   - Even length [1,2,2,1]     → true
//   - Odd length [1,2,3,2,1]    → true  (middle node is always equal to itself)
//
// ============================================================
// HOW TO SOLVE — Simple Walkthrough
// ============================================================
// Key Insight:
//   A palindrome reads the same from both ends. For a linked list we
//   can only go forward — so instead of reading backward, REVERSE the
//   second half and compare it with the first half node by node.
//
// Three steps:
//   1. Find the MIDDLE using slow/fast pointers
//   2. REVERSE the second half in-place
//   3. COMPARE first half and reversed second half
//
// Step-by-step with example: 5 → 4 → 3 → 4 → 5
//   Nodes: n0(5) → n1(4) → n2(3) → n3(4) → n4(5) → null
//
//   STEP 1 — Find middle (slow stops at middle node):
//     Start: slow=n0, fast=n0
//     Iter1: slow=n1(4), fast=n2(3)
//     Iter2: slow=n2(3), fast=n4(5)  → fast.next=null → STOP
//     Middle = n2(3)
//
//   STEP 2 — Reverse second half (from middle.next):
//     Before: n3(4) → n4(5) → null
//     After:  n4(5) → n3(4) → null
//
//   STEP 3 — Compare:
//     left=n0(5),  right=n4(5)  → 5==5 ✓ → advance
//     left=n1(4),  right=n3(4)  → 4==4 ✓ → advance
//     right=null → stop → all matched → return true
//
//     (Middle node n2(3) is never compared — it has no mirror)
//
// Example 2: 5 → 4 → 3 (not a palindrome)
//   Middle = n1(4),  second half reversed = [3]
//   left=n0(5),  right=n2(3)  → 5 ≠ 3 → return false
//
// HOW SLOW/FAST FINDS THE MIDDLE:
//   Fast moves twice as fast. When fast reaches the end, slow is at the middle.
//   Condition: fast.next != null && fast.next.next != null
//     Odd  length [1,2,3,2,1]: slow stops at node 3 (true middle)
//     Even length [1,2,2,1]:   slow stops at first 2 (left-middle)
//   Either way, slow.next is where the second half begins.
//
// HOW REVERSING WORKS:
//   Walk the list keeping track of prev, curr, next:
//     null ← n3(4)   n4(5) → null    (before)
//     null ← n4(5) ← n3(4)           (after, prev is new head)
//
// WHY THIS APPROACH?
//   Simple approach: copy all values into an array, check if array is palindrome.
//   → O(n) time, O(n) space.
//   Slow/fast + reverse (this): no extra array needed.
//   → O(n) time, O(1) space.
//
// ============================================================
// TIME & SPACE COMPLEXITY
// ============================================================
//   n = number of nodes in the list
//
//   Time:  O(n)
//     - Finding middle with slow/fast: O(n/2)
//     - Reversing second half: O(n/2)
//     - Comparing both halves: O(n/2)
//     - Total: O(n)
//
//   Space: O(1)
//     - Only pointer variables (slow, fast, prev, curr, left, right)
//     - Reversal is done in-place — no extra list or array created
// ============================================================

// Note: ListNode is defined in LinkedListCycle.cs — shared across linked list problems.
// In a real project, define it once in a shared file or namespace.

public class LinkedListPalindrome
{
    private static ListNode Reverse(ListNode head)
    {
        ListNode prev = null;
        ListNode curr = head;

        while (curr != null)
        {
            ListNode next = curr.next;  // save next before overwriting
            curr.next = prev;           // reverse the link
            prev = curr;                // move prev forward
            curr = next;                // move curr forward
        }

        return prev;  // prev is now the new head of the reversed list
    }

    public static bool Solve(ListNode head)
    {
        if (head == null || head.next == null) return true;

        // Step 1: Find middle — stop when fast can't move two steps
        ListNode slow = head;
        ListNode fast = head;
        while (fast.next != null && fast.next.next != null)
        {
            slow = slow.next;
            fast = fast.next.next;
        }

        // Step 2: Reverse the second half (from middle.next onward)
        ListNode secondHalf = Reverse(slow.next);

        // Step 3: Compare first half and reversed second half
        ListNode left = head;
        ListNode right = secondHalf;
        while (right != null)
        {
            if (left.val != right.val) return false;
            left = left.next;
            right = right.next;
        }

        return true;
    }

    public static void Main(string[] args)
    {
        // Example 1: 5→4→3→4→5  → true
        var n = new ListNode[] {
            new ListNode(5), new ListNode(4), new ListNode(3),
            new ListNode(4), new ListNode(5)
        };
        for (int i = 0; i < n.Length - 1; i++) n[i].next = n[i + 1];
        Console.WriteLine("Example 1 (5→4→3→4→5): " + Solve(n[0]));
        // Expected: true

        // Example 2: 5→4→3  → false
        var m = new ListNode[] {
            new ListNode(5), new ListNode(4), new ListNode(3)
        };
        for (int i = 0; i < m.Length - 1; i++) m[i].next = m[i + 1];
        Console.WriteLine("Example 2 (5→4→3):     " + Solve(m[0]));
        // Expected: false

        // Example 3: 1→2→2→1  → true
        var p = new ListNode[] {
            new ListNode(1), new ListNode(2),
            new ListNode(2), new ListNode(1)
        };
        for (int i = 0; i < p.Length - 1; i++) p[i].next = p[i + 1];
        Console.WriteLine("Example 3 (1→2→2→1):   " + Solve(p[0]));
        // Expected: true
    }
}
