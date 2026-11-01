/*
============================================================
PROBLEM: Kth Smallest Element in a Sorted Matrix
============================================================

DESCRIPTION:
Given an n x n matrix where every row and every column is sorted in
ascending order, return the k-th smallest element.

The value of k is 1-indexed:
- k = 1 returns the smallest element
- k = n * n returns the largest element

INPUT:
- int[][] matrix - square matrix sorted by rows and columns
- int k          - 1-indexed position to find

OUTPUT:
- int - the k-th smallest value

EXAMPLES:
Example 1:
Input:
  matrix = [[1, 5, 9], [10, 11, 13], [12, 13, 15]], k = 8
Output: 13
Explanation: The sorted values are [1, 5, 9, 10, 11, 12, 13, 13, 15].

Example 2:
Input:
  matrix = [[-5]], k = 1
Output: -5

CONSTRAINTS:
- matrix is non-empty and square
- Every row is sorted from left to right
- Every column is sorted from top to bottom
- 1 <= k <= n * n

EDGE CASES:
✓ A 1 x 1 matrix
✓ k = 1
✓ k = n * n
✓ Duplicate values
✓ Negative values
✓ All values are the same

TRICK CASES:
⚡ Do not confuse the matrix's sorted rows and columns with one fully sorted array.
⚡ Count values less than or equal to mid, not values strictly less than mid.
⚡ k is 1-indexed, so the first value has rank 1.

============================================================
OPTIMAL SOLUTION
============================================================

APPROACH:
Do not flatten and sort the matrix. That would cost O(n^2 log n).

Instead, binary search for the answer's VALUE:
1. The answer must be between matrix[0][0] and matrix[n - 1][n - 1].
2. Pick the middle value mid.
3. Count how many matrix values are <= mid.
4. If that count is at least k, mid might be the answer, so search smaller
   values. Otherwise, search larger values.

HOW TO COUNT IN O(n):
Start at the bottom-left cell.
- If matrix[row][column] <= mid, every value above it in that column is also
  <= mid. Add row + 1 and move right.
- Otherwise, the current value is too large. Move up.

Each move goes either up or right, so at most 2n moves are made.

WHY THIS WORKS:
The count is monotonic:
- As mid increases, the number of values <= mid never decreases.
- Therefore, binary search can find the smallest value whose count is >= k.

ALGORITHM STEPS:
1. Set low to the smallest matrix value and high to the largest.
2. While low < high:
   a. Calculate mid without risking integer overflow.
   b. Count values <= mid using the bottom-left staircase walk.
   c. If count >= k, move high to mid.
   d. Otherwise, move low to mid + 1.
3. Return low.

TIME COMPLEXITY: O(n log(maxValue - minValue))
- Each count takes O(n) time.
- Binary search checks O(log(maxValue - minValue)) possible values.

SPACE COMPLEXITY: O(1)
- Only a fixed number of variables are used.

PATTERN: Binary Search on the Answer
DIFFICULTY: Medium
============================================================
*/

public class KthSmallestElementInSortedMatrix
{
    /// <summary>
    /// Returns the k-th smallest value in a row- and column-sorted square matrix.
    /// </summary>
    public static int Solve(int[][] matrix, int k)
    {
        int n = matrix.Length;
        long low = matrix[0][0];
        long high = matrix[n - 1][n - 1];

        while (low < high)
        {
            long mid = low + (high - low) / 2;
            int count = CountLessThanOrEqualTo(matrix, mid);

            if (count >= k)
                high = mid;
            else
                low = mid + 1;
        }

        return (int)low;
    }

    private static int CountLessThanOrEqualTo(int[][] matrix, long value)
    {
        int n = matrix.Length;
        int row = n - 1;
        int column = 0;
        int count = 0;

        while (row >= 0 && column < n)
        {
            if (matrix[row][column] <= value)
            {
                // Everything above this cell in the column is also <= value.
                count += row + 1;
                column++;
            }
            else
            {
                // This value and everything to its right are too large.
                row--;
            }
        }

        return count;
    }

    public static void Main(string[] args)
    {
        Console.WriteLine(
            "Example 1: " +
            Solve(
                new[]
                {
                    new[] { 1, 5, 9 },
                    new[] { 10, 11, 13 },
                    new[] { 12, 13, 15 }
                },
                8));
        // Expected: 13

        Console.WriteLine(
            "Example 2: " +
            Solve(
                new[]
                {
                    new[] { -5 }
                },
                1));
        // Expected: -5

        Console.WriteLine(
            "Example 3: " +
            Solve(
                new[]
                {
                    new[] { 1, 2 },
                    new[] { 1, 3 }
                },
                3));
        // Expected: 2
    }
}
