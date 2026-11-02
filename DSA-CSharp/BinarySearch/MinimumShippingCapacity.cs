// ============================================================
// PROBLEM: Minimum Shipping Capacity
// ============================================================
// You are given the quantity of each product and the weight of each
// product item. Every shipping box has the same capacity and the same
// maximum weight limit. A box can hold at most capacity items OR
// maxWeightPerBox kg, whichever comes first. You must pack each product
// type separately, and you want the minimum capacity that allows all
// products to ship within the allowed number of boxes.
//
// INPUT:
//   int[] quantities — number of items for each product type
//   int[] weights    — weight of one item for each product type
//   int maxBoxes     — maximum number of boxes allowed overall
//   int maxWeightPerBox — maximum kilograms allowed per box
//   Example: quantities = [8, 12, 5], weights = [2, 3, 1],
//            maxBoxes = 6, maxWeightPerBox = 20
//
// OUTPUT:
//   int — minimum box capacity that allows all products to ship
//   Example: 5
//
// EDGE CASES:
//   - One product only → capacity is just enough to fit that product
//   - A product item is heavier than maxWeightPerBox → impossible unless capacity is 0
//   - Some products are small and fit easily; others need more boxes
//   - maxBoxes is exactly the number of boxes needed at the minimum valid capacity
//   - single item product → capacity must at least 1
//
// ============================================================
// HOW TO SOLVE — Simple Walkthrough
// ============================================================
// Key Insight:
//   For a chosen capacity C, each product can fill a box with at most
//   min(C, floor(maxWeightPerBox / weight[i])) items. If the total number
//   of boxes needed is <= maxBoxes, then C is feasible. Feasibility is
//   monotonic: once a capacity works, every larger capacity also works.
//   This makes binary search the right tool.
//
// Step-by-step with example: quantities = [8, 12, 5],
// weights = [2, 3, 1], maxBoxes = 6, maxWeightPerBox = 20
//
//   Try capacity = 5.
//   Product 0 (8 items, 2kg each):
//     maxItemsPerBox = min(5, floor(20 / 2) = 10) = 5
//     boxesNeeded = ceil(8 / 5) = 2
//
//   Product 1 (12 items, 3kg each):
//     maxItemsPerBox = min(5, floor(20 / 3) = 6) = 5
//     boxesNeeded = ceil(12 / 5) = 3
//
//   Product 2 (5 items, 1kg each):
//     maxItemsPerBox = min(5, floor(20 / 1) = 20) = 5
//     boxesNeeded = ceil(5 / 5) = 1
//
//   Total boxes = 2 + 3 + 1 = 6 <= 6, so capacity 5 is feasible.
//   Search for smaller feasible capacity by binary search.
//
// WHY THIS APPROACH?
//   A brute-force search over every possible capacity would be too slow.
//   The feasibility check is monotonic, so binary search reduces the
//   search space from O(U) to O(log U), where U is the answer range.
//
// ============================================================
// TIME & SPACE COMPLEXITY
// ============================================================
//   n = number of product types
//   U = maximum possible capacity value
//
//   Time:  O(n log U)
//     - Each feasibility check scans all products once: O(n)
//     - Binary search runs O(log U) times
//     - Total: O(n log U)
//
//   Space: O(1)
//     - Only loop variables and counters are used
// ============================================================

public class MinimumShippingCapacity
{
    public static int Solve(int[] quantities, int[] weights, int maxBoxes, int maxWeightPerBox)
    {
        int low = 1;
        int high = 1;

        foreach (int quantity in quantities)
        {
            high = Math.Max(high, quantity);
        }

        while (low < high)
        {
            int mid = low + (high - low) / 2;

            if (CanShip(quantities, weights, maxBoxes, maxWeightPerBox, mid))
            {
                high = mid;
            }
            else
            {
                low = mid + 1;
            }
        }

        return low;
    }

    private static bool CanShip(int[] quantities, int[] weights, int maxBoxes, int maxWeightPerBox, int capacity)
    {
        int totalBoxesNeeded = 0;

        for (int i = 0; i < quantities.Length; i++)
        {
            int quantity = quantities[i];
            int weight = weights[i];

            if (weight <= 0)
                throw new ArgumentException("Weight per item must be positive.");

            int maxItemsPerBox = Math.Min(capacity, maxWeightPerBox / weight);
            if (maxItemsPerBox <= 0)
                return false;

            totalBoxesNeeded += (quantity + maxItemsPerBox - 1) / maxItemsPerBox;

            if (totalBoxesNeeded > maxBoxes)
                return false;
        }

        return true;
    }

    public static void Main(string[] args)
    {
        int[] quantities1 = { 8, 12, 5 };
        int[] weights1 = { 2, 3, 1 };
        Console.WriteLine(Solve(quantities1, weights1, 6, 20));
        // Expected: 5

        int[] quantities2 = { 10, 15, 8 };
        int[] weights2 = { 5, 2, 3 };
        Console.WriteLine(Solve(quantities2, weights2, 10, 15));
        // Expected: 4
    }
}
