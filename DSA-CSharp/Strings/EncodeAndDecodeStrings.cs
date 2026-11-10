/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Encode and Decode Strings
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
Design an algorithm to encode a list of strings into one string, then decode
that string back into the original list.

The encoding must preserve:
- Empty strings
- Spaces
- Special characters
- Comma and bracket characters
- The exact number and order of every string

INPUT:
- Encode: IList<string> strs - List of strings
- Decode: string encoded - String produced by Encode

OUTPUT:
- Encode: string - Encoded representation
- Decode: IList<string> - Original list of strings

EXAMPLES:
Example 1:
Input: strs = ["hello", "world"]
Encoded: "5#hello5#world"
Output: ["hello", "world"]

Example 2:
Input: strs = ["", "a", "42", ""]
Encoded: "0#1#a2#420#"
Output: ["", "a", "42", ""]

Example 3:
Input: strs = ["a:b", "#", "[]"]
Encoded: "3#a:b1#3#[]"
Output: ["a:b", "#", "[]"]

CONSTRAINTS:
- 1 <= strs.Count <= 200
- 0 <= strs[i].Length <= 200
- strs[i] may contain any printable ASCII character

EDGE CASES:
✓ Empty list
✓ Empty strings
✓ Strings containing numbers
✓ Strings containing '#' or ':'
✓ Strings containing commas and brackets
✓ Strings with trailing spaces

TRICK CASES:
⚡ Never encode by commas; string contents may contain commas
⚡ Never decode by searching for '#' only
⚡ Length must describe the exact number of characters
⚡ Store the length before the delimiter

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OPTIMAL SOLUTION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APPROACH:
Use a length-prefix encoding.

For every string:
1. Count its exact length
2. Append that length
3. Append '#'
4. Append the string itself

Example:
"hello" → "5#hello"
"world" → "5#world"
Combined: "5#hello5#world"

KEY INSIGHTS:
• '#' is only a delimiter immediately after a numeric length
• The numeric length tells Decode exactly where the string ends
• String contents are never parsed for special separators
• Empty strings become "0#" and remain distinguishable

ALGORITHM STEPS:
ENCODE:
1. Create a StringBuilder
2. For each string, append its length, '#', and its characters
3. Return the combined string

DECODE:
1. Read from index zero
2. Find the next '#' to locate the end of the length section
3. Parse the substring before '#' as the string length
4. Read exactly that many characters after '#'
5. Add the substring to the result
6. Continue until all characters are consumed

TIME COMPLEXITY: O(n)
- Encoding visits every character once
- Decoding reads every character once
- n is the total number of characters plus separators

SPACE COMPLEXITY: O(n)
- The output encoding uses O(n) space
- The decoded list stores the original strings

PATTERN: String Encoding / Length Prefix
DIFFICULTY: Medium
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace DSA_CSharp.Strings
{
    public class EncodeAndDecodeStrings
    {
        /// <summary>
        /// Encodes a list of strings using length-prefixed entries.
        /// Time: O(n), Space: O(n)
        /// </summary>
        public string Encode(IList<string> strs)
        {
            if (strs == null)
                return "";

            var encoded = new StringBuilder();

            foreach (string value in strs)
            {
                string safeValue = value ?? "";
                encoded.Append(safeValue.Length);
                encoded.Append('#');
                encoded.Append(safeValue);
            }

            return encoded.ToString();
        }

        /// <summary>
        /// Decodes a length-prefixed string back into a list.
        /// Time: O(n), Space: O(n)
        /// </summary>
        public IList<string> Decode(string encoded)
        {
            var decoded = new List<string>();

            if (encoded == null)
                return decoded;

            int position = 0;

            while (position < encoded.Length)
            {
                int delimiter = encoded.IndexOf('#', position);

                if (delimiter < 0)
                    throw new FormatException("Invalid encoded string: missing length delimiter.");

                string lengthText = encoded.Substring(position, delimiter - position);

                if (!int.TryParse(lengthText, out int length) || length < 0)
                    throw new FormatException("Invalid encoded string: invalid length.");

                int stringStart = delimiter + 1;

                if (stringStart + length > encoded.Length)
                    throw new FormatException("Invalid encoded string: incomplete string.");

                decoded.Add(encoded.Substring(stringStart, length));
                position = stringStart + length;
            }

            return decoded;
        }

        public static void Test()
        {
            var solution = new EncodeAndDecodeStrings();

            Console.WriteLine("=== Encode and Decode Strings Test Cases ===\n");

            IList<string>[] testCases =
            {
                new[] { "hello", "world" },
                new[] { "", "a", "42", "" },
                new[] { "a:b", "#", "[]" },
                new[] { "hello world", "with,comma", "" }
            };

            foreach (IList<string> original in testCases)
            {
                string encoded = solution.Encode(original);
                IList<string> decoded = solution.Decode(encoded);

                Console.WriteLine($"Input: [{string.Join(", ", original)}]");
                Console.WriteLine($"Encoded: {encoded}");
                Console.WriteLine($"Decoded: [{string.Join(", ", decoded)}]");
                Console.WriteLine();
            }

            IList<string> emptyList = Array.Empty<string>();
            Console.WriteLine($"Empty list encoded: \"{solution.Encode(emptyList)}\"");
            Console.WriteLine($"Empty list decoded count: {solution.Decode(solution.Encode(emptyList)).Count}");
        }
    }
}
