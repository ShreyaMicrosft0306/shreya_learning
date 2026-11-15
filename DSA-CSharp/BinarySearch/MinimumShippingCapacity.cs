/*
============================================================
PROBLEM: Minimum Shipping Capacity
============================================================

DESCRIPTION:
You have several product types to ship. Each product type has a quantity
and a weight per item. Every box has:
- A capacity limit: maximum number of items
- A weight limit: maximum total weight

Each product type must be packed separately. Find the smallest item capacity
that allows all products to be shipped using at most maxBoxes boxes.

For one product, a box can hold:
  min(capacity, maxWeightPerBox / weightPerItem)
items, using integer division.

INPUT:
- int[] quantities      - number of items for each product type
- int[] weights         - weight of one item for each product type
- int maxBoxes          - maximum number of boxes available
- int maxWeightPerBox   - maximum weight allowed in one box

OUTPUT:
- int - minimum item capacity per box that ships all products in time

EXAMPLES:
Example 1:
Input:
  quantities = [8, 12, 5]
  weights = [2, 3, 1]
  maxBoxes = 6
  maxWeightPerBox = 20
Output: 5
Explanation:
  Capacity 5 needs 2 + 3 + 1 = 6 boxes.
  Capacity 4 needs 2 + 3 + 2 = 7 boxes, so 5 is the minimum.

Example 2:
Input:
  quantities = [10, 15, 8]
  weights = [5, 2, 3]
  maxBoxes = 10
  maxWeightPerBox = 15
Output: 4
Explanation:
  Capacity 4 needs 4 + 4 + 2 = 10 boxes.

CONSTRAINTS:
- quantities and weights have the same length
- Every quantity is positive
- Every item weight is positive
- maxBoxes and maxWeightPerBox are positive
- Every item must fit in a box by weight:
  weight[i] <= maxWeightPerBox

EDGE CASES:
✓ One product type
✓ A product that fits exactly in one box
✓ maxBoxes equals the number of products
✓ Capacity is limited by weight rather than item count
✓ Large quantities
✓ Different products have different weight limits per box

TRICK CASES:
⚡ Use ceiling division: boxes = (quantity + itemsPerBox - 1) / itemsPerBox.
⚡ maxWeightPerBox / weight is integer division; fractional items are not allowed.
⚡ A larger capacity does not bypass the weight limit.
⚡ Capacity is an item-count limit, not a kilogram limit.

============================================================
OPTIMAL SOLUTION
============================================================

APPROACH:
Binary search the answer instead of trying every capacity.

The smallest useful capacity is 1. The largest useful capacity is the
largest product quantity, because a larger capacity cannot reduce a product
below one box.

For a candidate capacity:
1. Find the maximum number of items of each product that fit in one box.
2. Calculate how many boxes that product needs.
3. Add the boxes for all products.
4. If the total is at most maxBoxes, the candidate works.

The check is monotonic:
- If a capacity works, every larger capacity also works.
- If a capacity fails, every smaller capacity also fails.

That monotonic behavior makes binary search possible.

ALGORITHM STEPS:
1. Set low = 1 and high = the largest quantity.
2. While low < high:
   a. Try the middle capacity.
   b. Count the required boxes for every product.
   c. If the count is within maxBoxes, search the smaller half.
   d. Otherwise, search the larger half.
3. Return low, the smallest valid capacity.

TIME COMPLEXITY: O(n log q)
- n = number of product types
- q = largest product quantity
- Each feasibility check examines all n products.
- Binary search performs O(log q) checks.

SPACE COMPLEXITY: O(1)
- Only a fixed number of variables are used.

PATTERN: Binary Search on the Answer
DIFFICULTY: Medium
============================================================
*/

using System;

public class MinimumShippingCapacity
{
    /// <summary>
    /// Returns the minimum item capacity needed to stay within maxBoxes.
    /// </summary>
    public static int Solve(
        int[] quantities,
        int[] weights,
        int maxBoxes,
        int maxWeightPerBox)
    {
        int low = 1;
        int high = 0;

        foreach (int quantity in quantities)
            high = Math.Max(high, quantity);

        while (low < high)
        {
            int capacity = low + (high - low) / 2;

            if (CanShipWithinLimit(
                    quantities,
                    weights,
                    maxBoxes,
                    maxWeightPerBox,
                    capacity))
            {
                high = capacity;
            }
            else
            {
                low = capacity + 1;
            }
        }

        return low;
    }

    private static bool CanShipWithinLimit(
        int[] quantities,
        int[] weights,
        int maxBoxes,
        int maxWeightPerBox,
        int capacity)
    {
        long boxesNeeded = 0;

        for (int i = 0; i < quantities.Length; i++)
        {
            int weightBasedLimit = maxWeightPerBox / weights[i];
            int itemsPerBox = Math.Min(capacity, weightBasedLimit);

            // The input guarantees that every item can fit by weight.
            boxesNeeded += (quantities[i] + (long)itemsPerBox - 1) / itemsPerBox;

            if (boxesNeeded > maxBoxes)
                return false;
        }

        return true;
    }

    public static void Main(string[] args)
    {
        Console.WriteLine(
            "Example 1: " +
            Solve(
                new[] { 8, 12, 5 },
                new[] { 2, 3, 1 },
                6,
                20));
        // Expected: 5

        Console.WriteLine(
            "Example 2: " +
            Solve(
                new[] { 10, 15, 8 },
                new[] { 5, 2, 3 },
                10,
                15));
        // Expected: 4

        Console.WriteLine(
            "Example 3: " +
            Solve(
                new[] { 6 },
                new[] { 2 },
                2,
                10));
        // Expected: 3
    }
}
