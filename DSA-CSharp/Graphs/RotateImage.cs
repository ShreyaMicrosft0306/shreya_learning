// ============================================================
// PROBLEM: Rotate Image
// ============================================================
// Given an n x n 2D matrix representing an image, rotate it by 90
// degrees clockwise in-place without using an extra matrix.
//
// INPUT:
//   int[][] matrix — square matrix of integers
//   Example: [[1,4,7],[2,5,8],[3,6,9]]
//
// OUTPUT:
//   void — matrix is rotated in-place
//   Example: [[3,2,1],[6,5,4],[9,8,7]]
//
// EDGE CASES:
//   - Empty matrix → no change
//   - 1x1 matrix → no change
//   - Already rotated matrix → still valid, result is a rotation of the same shape
//   - Non-square input is not expected for this problem
//
// ============================================================
// HOW TO SOLVE — Simple Walkthrough
// ============================================================
// Key Insight:
//   A clockwise 90° rotation can be done in two steps:
//     1. Transpose the matrix (swap matrix[i][j] with matrix[j][i])
//     2. Reverse each row
//   Doing this in-place is possible because the transpose uses mirrored
//   positions inside the same matrix, and the row reversal changes the order
//   without needing another matrix.
//
// Step-by-step with example: matrix = [[1,4,7],[2,5,8],[3,6,9]]
//
//   Original:
//   [1,4,7]
//   [2,5,8]
//   [3,6,9]
//
//   Step 1: transpose
//   [1,2,3]
//   [4,5,6]
//   [7,8,9]
//
//   Step 2: reverse each row
//   [3,2,1]
//   [6,5,4]
//   [9,8,7]
//
//   Final rotated matrix:
//   [3,2,1]
//   [6,5,4]
//   [9,8,7]
//
// WHY THIS APPROACH?
//   A naive method would copy all values into a new matrix and place them
//   based on their new positions. This works, but it uses extra space.
//   The transpose + reverse trick keeps the same matrix and still rotates
//   the image correctly, which is the required in-place behavior.
//
// ============================================================
// TIME & SPACE COMPLEXITY
// ============================================================
//   n = side length of the matrix
//
//   Time:  O(n²)
//     - Each cell is touched a constant number of times
//     - Total: O(n²)
//
//   Space: O(1)
//     - Only a few index variables are used; no extra matrix is created
// ============================================================

public class RotateImage
{
    public static void Solve(int[][] matrix)
    {
        int n = matrix.Length;

        for (int row = 0; row < n; row++)
        {
            for (int col = row + 1; col < n; col++)
            {
                int temp = matrix[row][col];
                matrix[row][col] = matrix[col][row];
                matrix[col][row] = temp;
            }
        }

        for (int row = 0; row < n; row++)
        {
            int left = 0;
            int right = n - 1;

            while (left < right)
            {
                int temp = matrix[row][left];
                matrix[row][left] = matrix[row][right];
                matrix[row][right] = temp;
                left++;
                right--;
            }
        }
    }

    public static void Main(string[] args)
    {
        var matrix1 = new int[][]
        {
            new[] { 1, 4, 7 },
            new[] { 2, 5, 8 },
            new[] { 3, 6, 9 }
        };

        Solve(matrix1);
        Console.WriteLine("Example 1:");
        foreach (var row in matrix1)
            Console.WriteLine("[" + string.Join(", ", row) + "]");
        // Expected:
        // [3, 2, 1]
        // [6, 5, 4]
        // [9, 8, 7]
    }
}
