/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Maximum Profit in Job Scheduling
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
Given three arrays starts, ends, and profits where each job i has:
- start time = starts[i]
- end time = ends[i]
- profit = profits[i]

Schedule non-overlapping jobs to maximize total profit.
You can work on only one job at a time.

INPUT:
- starts: int[]
- ends: int[]
- profits: int[]

OUTPUT:
- int - Maximum total profit from a valid non-overlapping schedule

EXAMPLE:
Input:
starts  = [1, 3, 6, 10]
ends    = [4, 5, 10, 12]
profits = [20, 20, 100, 70]

Output: 190
Explanation:
Choose jobs (1,4,20), (6,10,100), (10,12,70) => total = 190

CONSTRAINTS:
- 1 <= starts.Length == ends.Length == profits.Length
- Time values are positive integers

EDGE CASES:
✓ Single job -> answer is its profit
✓ Fully overlapping jobs -> choose highest profit combination possible
✓ Non-overlapping chain -> take all jobs
✓ Jobs that touch at boundaries (end == next start) are allowed

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OPTIMAL SOLUTION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APPROACH:
Weighted Interval Scheduling using:
1) Sorting
2) Binary Search
3) Dynamic Programming

IDEA:
At each job i, we have 2 choices:
- Skip job i -> profit = dp[i + 1]
- Take job i  -> profit = jobs[i].Profit + dp[nextIndex]
  where nextIndex is the first job whose start >= jobs[i].End

Recurrence:
dp[i] = max(dp[i + 1], jobs[i].Profit + dp[nextIndex])

We compute dp from right to left.

TIME COMPLEXITY: O(n log n)
- Sorting: O(n log n)
- For each job, one binary search: O(log n)

SPACE COMPLEXITY: O(n)
- Jobs array + dp array

PATTERN: Dynamic Programming + Binary Search (Weighted Interval Scheduling)
DIFFICULTY: Medium
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;

namespace DSA_CSharp.DynamicProgramming
{
    public class MaximumProfitInJobScheduling
    {
        private struct Job
        {
            public int Start;
            public int End;
            public int Profit;

            public Job(int start, int end, int profit)
            {
                Start = start;
                End = end;
                Profit = profit;
            }
        }

        /// <summary>
        /// Returns maximum total profit from non-overlapping jobs.
        /// Time: O(n log n), Space: O(n)
        /// </summary>
        public int Solution(int[] starts, int[] ends, int[] profits)
        {
            int n = starts.Length;
            Job[] jobs = new Job[n];

            for (int i = 0; i < n; i++)
            {
                jobs[i] = new Job(starts[i], ends[i], profits[i]);
            }

            // Sort jobs by start time for binary searching next compatible job.
            Array.Sort(jobs, (a, b) => a.Start.CompareTo(b.Start));

            // dp[i] = best profit considering jobs from index i to n-1.
            // dp[n] = 0 base case (no jobs left).
            int[] dp = new int[n + 1];

            for (int i = n - 1; i >= 0; i--)
            {
                int nextIndex = LowerBound(jobs, jobs[i].End);
                int take = jobs[i].Profit + dp[nextIndex];
                int skip = dp[i + 1];
                dp[i] = Math.Max(take, skip);
            }

            return dp[0];
        }

        /// <summary>
        /// Returns first index idx where jobs[idx].Start >= target.
        /// If no such index exists, returns jobs.Length.
        /// </summary>
        private int LowerBound(Job[] jobs, int target)
        {
            int left = 0;
            int right = jobs.Length;

            while (left < right)
            {
                int mid = left + (right - left) / 2;

                if (jobs[mid].Start < target)
                {
                    left = mid + 1;
                }
                else
                {
                    right = mid;
                }
            }

            return left;
        }

        // Test method
        public static void Test()
        {
            var solution = new MaximumProfitInJobScheduling();

            Console.WriteLine("=== Maximum Profit in Job Scheduling Test Cases ===\n");

            int[] starts1 = { 1, 3, 6, 10 };
            int[] ends1 = { 4, 5, 10, 12 };
            int[] profits1 = { 20, 20, 100, 70 };
            Console.WriteLine($"Test 1 Output: {solution.Solution(starts1, ends1, profits1)}");
            Console.WriteLine("Expected: 190\n");

            int[] starts2 = { 1, 2, 3, 3 };
            int[] ends2 = { 3, 4, 5, 6 };
            int[] profits2 = { 50, 10, 40, 70 };
            Console.WriteLine($"Test 2 Output: {solution.Solution(starts2, ends2, profits2)}");
            Console.WriteLine("Expected: 120\n");

            int[] starts3 = { 1, 1, 1 };
            int[] ends3 = { 2, 3, 4 };
            int[] profits3 = { 5, 6, 4 };
            Console.WriteLine($"Test 3 Output: {solution.Solution(starts3, ends3, profits3)}");
            Console.WriteLine("Expected: 6\n");

            int[] starts4 = { 1 };
            int[] ends4 = { 2 };
            int[] profits4 = { 99 };
            Console.WriteLine($"Test 4 Output: {solution.Solution(starts4, ends4, profits4)}");
            Console.WriteLine("Expected: 99");
        }
    }
}
