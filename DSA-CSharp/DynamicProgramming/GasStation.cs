/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Gas Station
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
There are n gas stations arranged in a circle. At station i:
- gas[i] is the amount of gas received.
- cost[i] is the gas required to travel to station i + 1.

Start with an empty tank and return the unique starting station index that
allows a complete clockwise circuit. Return -1 if no solution exists.

EXAMPLE:
Input:  gas = [5, 2, 0, 3, 3]
        cost = [1, 5, 5, 1, 1]
Output: 3

APPROACH:
Greedy.

1. If total gas is less than total cost, completing the circuit is impossible.
2. Track the current tank while scanning from left to right.
3. If the tank becomes negative at station i, no station from the current
   starting point through i can be a valid start. Set the next station as the
   new candidate start and reset the tank to zero.

WHY THE GREEDY RESET WORKS:
We keep a running balance from the current candidate start. If this balance
becomes negative at station i, then the route from the current start through i
has a net deficit. That means every station in that range is part of a bad
prefix and cannot lead to a valid full circuit, because the tank is already
below zero before reaching the next station.

So we discard that candidate start and try the next station: start = i + 1.
This is safe because a negative prefix can never be fixed by continuing later;
any complete loop that includes that prefix would still be invalid.

Example:
If currentBalance is -3 at station i, then from the current start to station i,
the total gas collected is 3 units less than the total cost paid. No later
station can magically recover that lost gas. Therefore, the only useful new
candidate is a later start index.

We still keep totalBalance for the final validation. If totalBalance >= 0 after
scanning the full circle, the final start is valid; otherwise no station can
complete the circuit.

TIME COMPLEXITY: O(n)
- One pass to calculate total gas and the candidate start.

SPACE COMPLEXITY: O(1)
- Only a few running totals are stored.

PATTERN: Greedy
DIFFICULTY: Medium
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;

namespace DSA_CSharp.DynamicProgramming
{
    public class GasStation
    {
        /// <summary>
        /// Returns the starting station index for a complete circuit,
        /// or -1 when the circuit cannot be completed.
        /// Time: O(n), Space: O(1)
        /// </summary>
        public int Solution(int[] gas, int[] cost)
        {
            if (gas == null || cost == null || gas.Length != cost.Length || gas.Length == 0)
            {
                return -1;
            }

            int totalBalance = 0; // total surplus/deficit across the entire circle
            int currentBalance = 0; // running surplus from the current candidate start
            int start = 0;

            for (int station = 0; station < gas.Length; station++)
            {
                int balance = gas[station] - cost[station];
                totalBalance += balance;
                currentBalance += balance;

                // If the current candidate start cannot reach the next station,
                // then every station before this point is a bad prefix. Reset to
                // the next station and test a later start.
                if (currentBalance < 0)
                {
                    start = station + 1;
                    currentBalance = 0;
                }
            }

            return totalBalance >= 0 ? start : -1;
        }

        // Test method
        public static void Test()
        {
            var solution = new GasStation();

            Console.WriteLine("=== Gas Station Test Cases ===\n");

            int[] gas1 = { 5, 2, 0, 3, 3 };
            int[] cost1 = { 1, 5, 5, 1, 1 };
            Console.WriteLine($"Test 1 Output: {solution.Solution(gas1, cost1)}");
            Console.WriteLine("Expected: 3\n");

            int[] gas2 = { 2, 3, 4 };
            int[] cost2 = { 3, 4, 3 };
            Console.WriteLine($"Test 2 Output: {solution.Solution(gas2, cost2)}");
            Console.WriteLine("Expected: -1\n");

            int[] gas3 = { 1, 2, 3, 4, 5 };
            int[] cost3 = { 3, 4, 5, 1, 2 };
            Console.WriteLine($"Test 3 Output: {solution.Solution(gas3, cost3)}");
            Console.WriteLine("Expected: 3\n");

            int[] gas4 = { 5 };
            int[] cost4 = { 5 };
            Console.WriteLine($"Test 4 (single station) Output: {solution.Solution(gas4, cost4)}");
            Console.WriteLine("Expected: 0");
        }
    }
}
