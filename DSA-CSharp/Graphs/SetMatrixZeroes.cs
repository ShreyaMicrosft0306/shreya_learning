// ============================================================
// PROBLEM: Set Matrix Zeroes
// ============================================================
// Given an m x n matrix, if an element is 0, set its entire row and
// column to 0. This must be done in-place.
//
// INPUT:
//   int[][] matrix — m x n integer matrix
//   Example: [[0,2,3],[4,5,6],[7,8,9]]
//
// OUTPUT:
//   void — matrix is modified in-place
//   Example: [[0,0,0],[0,5,6],[0,8,9]]
//
// EDGE CASES:
//   - Empty matrix → no change
//   - Single cell zero → matrix becomes [[0]]
//   - Single row or single column → all entries become 0 if any zero exists
//   - Multiple zeros → all affected rows and columns become 0
//
// ============================================================
// HOW TO SOLVE — Simple Walkthrough
// ============================================================
// Key Insight (Use first row and first column as markers):
//   We need to know which rows and columns must become zero, but we cannot
//   allocate an extra matrix. Instead, we reuse the first row and the first
//   column as memory to mark whether a row/column should be zeroed.
//
//   Step 1: Scan the matrix and mark rows/cols needing zeroing by setting
//           matrix[i][0] = 0 and matrix[0][j] = 0 whenever a zero is found.
//   Step 2: Traverse every cell except the first row and first column.
//           If matrix[i][0] == 0 or matrix[0][j] == 0, set matrix[i][j] = 0.
//   Step 3: Fix the first row and first column separately based on whether
//           they were marked in step 1.
//
// Step-by-step with example:
//   matrix = [[0,2,3],[4,5,6],[7,8,9]]
//
//   Scan for zeros:
//     (0,0) is 0 → mark row0 and col0
//   Matrix now conceptually looks like:
//     [0,2,3]
//     [4,5,6]
//     [7,8,9]
//   with row0 and col0 marked.
//
//   Second pass:
//     (1,1) has row0/col0 flags? row0 is marked, so set (1,1)=0? no,
//     because we only consider cells after the first row/col. Actually the
//     first row and column are used as markers only: row0 and col0 will be
//     handled afterward.
//
//   Final fix:
//     first row all zeros
//     first column all zeros
//     middle cells unchanged unless they were in a marked row/col.
//
//   Result:
//   [[0,0,0],[0,5,6],[0,8,9]]
//
// WHY THIS APPROACH?
//   A naive solution would create another boolean grid to track zeros, which
//   is extra space. Using the first row and column as markers keeps the same
//   matrix and still records exactly which rows and columns need to be cleared.
//
// ============================================================
// TIME & SPACE COMPLEXITY
// ============================================================
//   m = rows, n = columns
//
//   Time:  O(m * n)
//     - First pass marks rows/cols: O(m * n)
//     - Second pass zeroes cells: O(m * n)
//     - Final row/col fix: O(m + n)
//     - Total: O(m * n)
//
//   Space: O(1)
//     - Only flags stored in the matrix itself, no extra grid
// ============================================================

public class SetMatrixZeroes
{
    public static void Solve(int[][] matrix)
    {
        if (matrix == null || matrix.Length == 0 || matrix[0].Length == 0)
            return;

        int rows = matrix.Length;
        int cols = matrix[0].Length;
        bool firstRowHasZero = false;
        bool firstColHasZero = false;

        // Step 1: mark rows and columns that need zeroing using first row/col
        for (int c = 0; c < cols; c++)
            if (matrix[0][c] == 0)
                firstRowHasZero = true;

        for (int r = 0; r < rows; r++)
            if (matrix[r][0] == 0)
                firstColHasZero = true;

        for (int r = 1; r < rows; r++)
        {
            for (int c = 1; c < cols; c++)
            {
                if (matrix[r][c] == 0)
                {
                    matrix[r][0] = 0;
                    matrix[0][c] = 0;
                }
            }
        }

        // Step 2: zero cells based on row/col markers
        for (int r = 1; r < rows; r++)
        {
            for (int c = 1; c < cols; c++)
            {
                if (matrix[r][0] == 0 || matrix[0][c] == 0)
                    matrix[r][c] = 0;
            }
        }

        // Step 3: fix first row and first column
        if (firstRowHasZero)
        {
            for (int c = 0; c < cols; c++)
                matrix[0][c] = 0;
        }

        if (firstColHasZero)
        {
            for (int r = 0; r < rows; r++)
                matrix[r][0] = 0;
        }
    }

    public static void Main(string[] args)
    {
        var matrix1 = new int[][]
        {
            new[] { 0, 2, 3 },
            new[] { 4, 5, 6 },
            new[] { 7, 8, 9 }
        };

        Solve(matrix1);
        Console.WriteLine("Example 1:");
        foreach (var row in matrix1)
            Console.WriteLine("[" + string.Join(", ", row) + "]");
        // Expected:
        // [0,0,0]
        // [0,5,6]
        // [0,8,9]
    }
}
