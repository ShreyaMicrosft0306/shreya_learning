// ============================================================
// PROBLEM: Kth Smallest Element in a Sorted Matrix
// ============================================================
// Given an n x n matrix whose rows and columns are sorted in
// ascending order, return the k-th smallest element, where k is
// 1-indexed.
//
// INPUT:
//   int[][] matrix — n x n matrix with sorted rows and columns
//   int k          — desired position, from 1 through n*n
//   Example: matrix = [[1,5,9],[10,11,13],[12,13,15]], k = 8
//
// OUTPUT:
//   int — the k-th smallest element
//   Example: 13
//
// EDGE CASES:
//   - k = 1               → return matrix[0][0], the smallest value
//   - k = n*n             → return matrix[n-1][n-1], the largest value
//   - Single-element matrix [[-5]], k=1 → return -5
//   - Duplicate values [[1,1],[1,2]], k=3 → count duplicates correctly
//
// ============================================================
// HOW TO SOLVE — Simple Walkthrough
// ============================================================
// Key Insight (Binary Search on the Value):
//   The answer is a value between matrix[0][0] and matrix[n-1][n-1].
//   Binary search this value range. For a candidate mid, count how
//   many matrix elements are less than or equal to mid. If the count
//   is at least k, mid may be the answer or an answer later; search
//   left. Otherwise, search right.
//
// Counting elements <= mid:
//   Start at the bottom-left corner. If the current value <= mid,
//   every value in that row from this column to the left is <= mid,
//   so add rowIndex+1 and move right. Otherwise, move up.
//   This visits at most 2n cells.
//
// Step-by-step with example: matrix = [[1,5,9],[10,11,13],[12,13,15]], k=8
//
//   lo=1, hi=15
//   mid=8: count <=8 = 3 (1,5,9? Counting 9 > 8 gives 2) -> count 2
//           2 < 8 -> lo=9
//   mid=12: count <=12 = 6 (1,5,9,10,11,12) -> 6 < 8 -> lo=13
//   mid=14: count <=14 = 8 (all values except 15) -> hi=14
//   mid=13: count <=13 = 8 -> hi=13
//   lo=13, hi=13 -> return 13
//
// WHY BINARY SEARCH THE VALUES?
//   Sorting every element costs O(n² log n). Counting the elements
//   <= mid is faster, and the search range contains only values, not
//   matrix positions. Binary search plus the count check finds the
//   smallest value whose rank is at least k.
//
// Counting from bottom-left:
//   Rows and columns are sorted, so moving right increases values and
//   moving up decreases values. Each visited row contributes all
//   elements in a known prefix, making the count both correct and O(n).
//
// ============================================================
// TIME & SPACE COMPLEXITY
// ============================================================
//   n = number of rows and columns
//
//   Time: O(n log(V))
//     - V = hi - lo, bounded by the matrix value range
//     - Each binary-search step counts in O(n)
//     - Total: O(n log V)
//
//   Space: O(1)
//     - Binary search and counting use only fixed-size variables
//
// ============================================================
// ALTERNATE SOLUTION: MIN-HEAP
// ====================================================
// Push the first element of each row into a min-heap. Pop the
// minimum k times. When a value is popped, push the next value in
// the same row. This uses O((n+k) log n) time and O(n) heap space.

public class KthSmallestElementSortedMatrix
{
    public static int Solve(int[][] matrix, int k)
    {
        if (matrix == null || matrix.Length == 0 ||
            matrix[0] == null || matrix[0].Length == 0)
        {
            throw new ArgumentException("Matrix must not be empty.");
        }

        int n = matrix.Length;
        if (k < 1 || k > n * n)
            throw new ArgumentOutOfRangeException(nameof(k));

        int lo = matrix[0][0];
        int hi = matrix[n - 1][n - 1];

        while (lo < hi)
        {
            int mid = lo + (hi - lo) / 2;
            int count = CountLessThanOrEqual(matrix, mid);

            if (count >= k)
                hi = mid;//shift to left part
            else
                lo = mid + 1; //
        }

        return lo;
    }

    private static int CountLessThanOrEqual(int[][] matrix, int target)
    {
        int n = matrix.Length;
        int row = n - 1;
        int col = 0;
        int count = 0;

        while (row >= 0 && col < n)
        {
            if (matrix[row][col] <= target)
            {
                count += row + 1;
                col++;
            }
            else
            {
                row--;
            }
        }

        return count;
    }

    public static int SolveWithMinHeap(int[][] matrix, int k)
    {
        if (matrix == null || matrix.Length == 0 ||
            matrix[0] == null || matrix[0].Length == 0)
        {
            throw new ArgumentException("Matrix must not be empty.");
        }

        int n = matrix.Length;
        if (k < 1 || k > n * n)
            throw new ArgumentOutOfRangeException(nameof(k));

        var heap = new PriorityQueue<(int Row, int Col), int>();

        for (int row = 0; row < n; row++)
            heap.Enqueue((row, 0), matrix[row][0]);

        int answer = 0;
        for (int i = 0; i < k; i++)
        {
            var position = heap.Dequeue();
            answer = matrix[position.Row][position.Col];

            if (position.Col + 1 < n)
            {
                int nextCol = position.Col + 1;
                heap.Enqueue((position.Row, nextCol),
                             matrix[position.Row][nextCol]);
            }
        }

        return answer;
    }

    public static void Main(string[] args)
    {
        int[][] example1 =
        {
            new[] { 1, 5, 9 },
            new[] { 10, 11, 13 },
            new[] { 12, 13, 15 }
        };

        Console.WriteLine("Example 1: " + Solve(example1, 8));
        // Expected: 13

        Console.WriteLine("Example 2: " + Solve(new[] { new[] { -5 } }, 1));
        // Expected: -5

        Console.WriteLine("Example 3: " + Solve(new[]
        {
            new[] { 1, 1 },
            new[] { 1, 2 }
        }, 3));
        // Expected: 1

        Console.WriteLine("Heap alternate: " +
                          SolveWithMinHeap(example1, 8));
        // Expected: 13
    }
}
