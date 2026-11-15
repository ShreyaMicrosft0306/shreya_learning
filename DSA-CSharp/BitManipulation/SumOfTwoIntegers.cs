/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Sum of Two Integers
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
Given two integers a and b, return their sum without using the + or -
operators.

INPUT:
- a: int - First integer
- b: int - Second integer

OUTPUT:
- int - The sum of a and b

EXAMPLES:
Example 1:
Input: a = 1, b = 2
Output: 3

Example 2:
Input: a = 2, b = 3
Output: 5

Example 3:
Input: a = -4, b = 7
Output: 3

CONSTRAINTS:
- -1000 <= a, b <= 1000
- The + and - operators cannot be used

EDGE CASES:
✓ Adding zero
✓ Two positive numbers
✓ Two negative numbers
✓ One positive and one negative number
✓ Carrying across multiple bits

TRICK CASES:
⚡ XOR adds bits without carrying.
⚡ AND finds the bits that need to be carried.
⚡ Shift the carry left by one position before adding it.
⚡ Repeat until there is no carry left.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OPTIMAL SOLUTION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APPROACH:
Binary addition has two parts:

1. XOR (`a ^ b`) adds the bits but ignores carry.
2. AND followed by a left shift (`(a & b) << 1`) finds the carry.

Repeat these two operations until the carry becomes zero.

Example: 1 + 3

First, find the sum without carry:

  1 = 01
  3 = 11
  XOR = 11  (sum without carry = 3)

Now find the carry:

  1 AND 3 = 01  (both numbers have a 1 in this bit)
  01 << 1 = 10  (move the carry one position left)

So the next step adds:

  10 XOR 10 = 00  (sum without carry)
  10 AND 10 = 10
  10 << 1 = 100   (new carry)

Final step:

  00 XOR 100 = 100
  00 AND 100 = 000  (no more carry)

Continue until the carry becomes zero. The final result is 4.

Why does this work?
- XOR gives the sum when no carry is needed.
- AND finds positions where both bits are 1, which creates a carry.
- Shifting left moves each carry to the next bit position.

ALGORITHM STEPS:
1. Calculate the sum without carry using XOR.
2. Calculate the carry using AND and a left shift.
3. Use the sum as the new a and the carry as the new b.
4. Repeat while b is not zero.
5. Return a.

TIME COMPLEXITY: O(1)
- Integers have a fixed number of bits.
- At most 32 bit operations are needed for a 32-bit integer.

SPACE COMPLEXITY: O(1)
- Only a few integer variables are used.

PATTERN: Bit Manipulation
DIFFICULTY: Medium
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

namespace DSA_CSharp.BitManipulation
{
    public class SumOfTwoIntegers
    {
        /// <summary>
        /// Adds two integers without using + or -.
        /// </summary>
        public int GetSum(int a, int b)
        {
            while (b != 0)
            {
                int sumWithoutCarry = a ^ b;
                int carry = (a & b) << 1;

                a = sumWithoutCarry;
                b = carry;
            }

            return a;
        }
    }
}
