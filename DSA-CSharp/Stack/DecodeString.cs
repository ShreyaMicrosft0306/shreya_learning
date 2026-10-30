// ============================================================
// PROBLEM: Decode String
// ============================================================
// Given an encoded string in the format k[encoded_string], decode it
// by repeating encoded_string exactly k times. Brackets can be nested.
// k is always a positive integer. Letters outside brackets appear as-is.
//
// INPUT:
//   string s — encoded string with digits, letters, '[' and ']'
//   Example: s = "3[a2[c]]"
//
// OUTPUT:
//   string — fully decoded string
//   Example: "accaccacc"
//
// EDGE CASES:
//   - No brackets "abc"       → "abc"  (plain letters returned unchanged)
//   - Multi-digit k "10[a]"  → "aaaaaaaaaa"
//   - Multiple groups "2[a]3[b]" → "aabbb"
//   - Nested "3[a2[c]]"      → "accaccacc"
//   - Letters outside "ab2[c]d" → "abccd"
//
// ============================================================
// HOW TO SOLVE — Simple Walkthrough
// ============================================================
// Key Insight:
//   When you hit a '[', you need to "remember" what you had before it
//   so you can come back to it after the matching ']'. That's exactly
//   what a stack does — it stores work in progress, lets you go deeper,
//   then restores the previous state when you come back up.
//
//   Use TWO stacks:
//     countStack — stores the repeat count k when entering '['
//     stringStack — stores the string built so far when entering '['
//
// Think of it like a recipe card:
//   When you see '3[', write down your current work on a sticky note,
//   put it aside, and start fresh. When you see ']', pick the sticky
//   note back up and attach what you built (repeated 3 times) to it.
//
// Step-by-step with example: s = "3[a2[c]]"
//
//   char='3': num = 3
//   char='[': push(count=3, current="") onto stacks → reset num=0, current=""
//             countStack=[3], stringStack=[""]
//
//   char='a': current = "a"
//
//   char='2': num = 2
//   char='[': push(count=2, current="a") onto stacks → reset num=0, current=""
//             countStack=[3,2], stringStack=["","a"]
//
//   char='c': current = "c"
//
//   char=']': pop → count=2, prev="a"
//             current = prev + current×count = "a" + "c"×2 = "acc"
//             countStack=[3], stringStack=[""]
//
//   char=']': pop → count=3, prev=""
//             current = prev + current×count = "" + "acc"×3 = "accaccacc"
//             countStack=[], stringStack=[]
//
//   Return: "accaccacc"
//
// WHY TWO STACKS AND NOT ONE?
//   Each '[' has TWO pieces of state to remember: the count AND the string
//   built so far. Storing them in separate stacks (always pushed/popped
//   together) keeps the code simple and easy to read.
//   Alternatively a stack of tuples (count, string) works the same way.
//
// HOW MULTI-DIGIT NUMBERS WORK:
//   We build the number digit by digit before pushing:
//   '1','2','[' → num = 1*10 + 2 = 12 → push 12
//   This handles any integer k, not just single digits.
//
// WHY THIS APPROACH?
//   Recursion also works (each '[' is a recursive call), but uses the
//   call stack implicitly. An explicit stack is iterative, avoids stack
//   overflow for deeply nested strings, and makes the state visible.
//
// ============================================================
// TIME & SPACE COMPLEXITY
// ============================================================
//   n = length of input string s
//   m = length of the decoded output string
//
//   Time:  O(m)
//     - Each character of the OUTPUT is written exactly once
//       (string repetition at each ']' builds toward the final result)
//     - m can be much larger than n (e.g. "100[a]" → n=7, m=100)
//     - Total: O(m)
//
//   Space: O(n)
//     - The two stacks hold at most one entry per '[' in the input: O(n)
//     - The strings stored in stringStack sum to at most O(m) in total
// ============================================================

public class DecodeString
{
    public static string Solve(string s)
    {
        var countStack = new Stack<int>();
        var stringStack = new Stack<string>();
        string current = "";
        int num = 0;

        foreach (char c in s)
        {
            if (char.IsDigit(c))
            {
                // Build the number digit by digit (handles multi-digit like 12, 100)
                num = num * 10 + (c - '0');
            }
            else if (c == '[')
            {
                // Save current state and start fresh inside the bracket
                countStack.Push(num);
                stringStack.Push(current);
                num = 0;
                current = "";
            }
            else if (c == ']')
            {
                // Restore previous state and append repeated current string
                int count = countStack.Pop();
                string prev = stringStack.Pop();
                current = prev + string.Concat(Enumerable.Repeat(current, count));
            }
            else
            {
                // Plain letter — just append to current string
                current += c;
            }
        }

        return current;
    }

    public static void Main(string[] args)
    {
        Console.WriteLine("Example 1: " + Solve("3[a2[c]]"));
        // Expected: accaccacc

        Console.WriteLine("Example 2: " + Solve("2[abc]3[cd]ef"));
        // Expected: abcabccdcdcdef

        Console.WriteLine("Example 3: " + Solve("10[a]"));
        // Expected: aaaaaaaaaa

        Console.WriteLine("Example 4: " + Solve("abc"));
        // Expected: abc
    }
}
