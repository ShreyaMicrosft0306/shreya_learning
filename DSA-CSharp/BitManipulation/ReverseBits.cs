/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Reverse Bits
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
Reverse the bits of a 32-bit unsigned integer and return the result.

EXAMPLE:
Input: n = 43261596
Output: 964176192

APPROACH:
We process the input bit by bit. For each of the 32 bits:
- Take the last bit of n using n & 1
- Shift the result left by 1 to make room for the next bit
- Shift n right by 1 to move to the next bit

This effectively builds the reversed number from the least significant bit to
most significant bit.

Why this works:
If the current least significant bit of n is 1, then in the reversed number it
must become the most significant bit of the final result. By repeatedly moving
one bit at a time from the right side of n to the left side of result, we build
exactly the reversed bit pattern.

TIME COMPLEXITY: O(32)
- We do exactly 32 iterations.

SPACE COMPLEXITY: O(1)
- Only a few integer variables are used.

PATTERN: Bit Manipulation
DIFFICULTY: Easy
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;

namespace DSA_CSharp.BitManipulation
{
    public class ReverseBits
    {
        /// <summary>
        /// Reverses the bits of a 32-bit unsigned integer.
        /// Time: O(32), Space: O(1)
        /// </summary>
        public uint ReverseBitsOfUInt(uint n)
        {
            uint result = 0;

            for (int i = 0; i < 32; i++)
            {
                result = (result << 1) | (n & 1);
                n >>= 1;
            }

            return result;
        }

        // Follow-up optimization for repeated calls:
        // If this function is invoked many times, we can precompute a lookup table
        // for 8-bit chunks and reverse 32 bits in four chunks.
        // This reduces repeated bit-by-bit work and is faster in hot loops.
        public uint ReverseBitsOptimized(uint n)
        {
            uint[] table = new uint[256];
            for (int i = 0; i < 256; i++)
            {
                uint value = (uint)i;
                uint reversed = 0;

                for (int bit = 0; bit < 8; bit++)
                {
                    reversed = (reversed << 1) | (value & 1);
                    value >>= 1;
                }

                table[i] = reversed;
            }

            return (table[n & 0xFF] << 24)
                 | (table[(n >> 8) & 0xFF] << 16)
                 | (table[(n >> 16) & 0xFF] << 8)
                 | table[(n >> 24) & 0xFF];
        }

        public static void Test()
        {
            var solution = new ReverseBits();

            Console.WriteLine("=== Reverse Bits Test Cases ===\n");

            uint n1 = 43261596;
            Console.WriteLine($"Test 1 Output: {solution.ReverseBitsOfUInt(n1)}");
            Console.WriteLine("Expected: 964176192\n");

            uint n2 = 2147483644;
            Console.WriteLine($"Test 2 Output: {solution.ReverseBitsOfUInt(n2)}");
            Console.WriteLine("Expected: 1073741822");
        }
    }
}
