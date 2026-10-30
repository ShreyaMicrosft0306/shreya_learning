// ============================================================
// PROBLEM: Spiral Matrix
// ============================================================
// Given an m x n matrix, return all elements in spiral order starting
// from the top-left corner and moving clockwise inward.
//
// INPUT:
//   int[][] matrix — rectangular matrix of integers
//   Example: [[0,1,2],[3,4,5],[6,7,8]]
//
// OUTPUT:
//   List<int> — all matrix elements in spiral order
//   Example: [0,1,2,5,8,7,6,3,4]
//
// EDGE CASES:
//   - Empty matrix → []
//   - Single row → return the row in order
//   - Single column → return the column in order
//   - Single element → return that one element
//
// ============================================================
// HOW TO SOLVE — Simple Walkthrough
// ============================================================
// Key Insight:
//   Instead of trying to “jump” around the matrix, keep four boundaries:
//     top, bottom, left, right.
//   Each iteration collects one full outer layer:
//     top row left → right
//     right column top + 1 → bottom
//     bottom row right - 1 → left
//     left column bottom - 1 → top + 1
//   Then shrink the boundaries inward and repeat until all cells are visited.
//
// Step-by-step with example: matrix = [[0,1,2],[3,4,5],[6,7,8]]
//
//   1. top=0, bottom=2, left=0, right=2
//   2. Top row: [0,1,2] → add 0,1,2
//   3. Right column without top row: [5,8] → add 5,8
//   4. Bottom row without last column: [7,6] → add 7,6
//   5. Left column without top and bottom: [3] → add 3
//   6. Shrink boundaries: top=1, bottom=1, left=1, right=1
//   7. Remaining cell is [4] → add 4
//
//   Final order: [0,1,2,5,8,7,6,3,4]
//
// WHY THIS APPROACH?
//   A naive solution would use repeated index math with many edge checks.
//   Boundary-based traversal works because each layer is processed exactly once,
//   and every cell is added exactly once. This keeps the code simple and linear.
//
// ============================================================
// TIME & SPACE COMPLEXITY
// ============================================================
//   m = number of rows, n = number of columns
//
//   Time:  O(m * n)
//     - Each cell is visited once while the boundaries move inward
//     - Total: O(m * n)
//
//   Space: O(1)
//     - Only boundary indexes and the output list are used
//     - Output is excluded from auxiliary space in the usual complexity model
// ============================================================

public class SpiralMatrix
{
    public static List<int> Solve(int[][] matrix)
    {
        List<int> result = new();

        if (matrix == null || matrix.Length == 0 || matrix[0].Length == 0)
            return result;

        int top = 0;
        int bottom = matrix.Length - 1;
        int left = 0;
        int right = matrix[0].Length - 1;

        while (top <= bottom && left <= right)
        {
            // top row: left -> right
            for (int col = left; col <= right; col++)
                result.Add(matrix[top][col]);
            top++;

            // right column: top -> bottom
            for (int row = top; row <= bottom; row++)
                result.Add(matrix[row][right]);
            right--;

            // bottom row: right -> left
            if (top <= bottom)
            {
                for (int col = right; col >= left; col--)
                    result.Add(matrix[bottom][col]);
                bottom--;
            }

            // left column: bottom -> top
            if (left <= right)
            {
                for (int row = bottom; row >= top; row--)
                    result.Add(matrix[row][left]);
                left++;
            }
        }

        return result;
    }

    public static void Main(string[] args)
    {
        var matrix1 = new int[][]
        {
            new[] { 0, 1, 2 },
            new[] { 3, 4, 5 },
            new[] { 6, 7, 8 }
        };

        Console.WriteLine("Example 1: [" + string.Join(", ", Solve(matrix1)) + "]");
        // Expected: [0, 1, 2, 5, 8, 7, 6, 3, 4]

        var matrix2 = new int[][]
        {
            new[] { 1, 2, 3, 4 },
            new[] { 5, 6, 7, 8 },
            new[] { 9, 10, 11, 12 }
        };

        Console.WriteLine("Example 2: [" + string.Join(", ", Solve(matrix2)) + "]");
        // Expected: [1, 2, 3, 4, 8, 12, 11, 10, 9, 5, 6, 7]
    }
}
