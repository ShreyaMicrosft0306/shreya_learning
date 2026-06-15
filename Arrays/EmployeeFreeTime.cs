/*
 * Problem: Employee Free Time
 * 
 * We are given a list of employees with their schedules, where each employee has a list 
 * of non-overlapping intervals representing their working hours. We need to return a list 
 * of finite intervals representing common free time for all employees.
 * 
 * Example:
 * Input: schedule = [[[1,3],[6,7]],[[2,4]],[[2,5],[9,12]]]
 * Output: [[5,6],[7,9]]
 * Explanation: Employee 1 works [1,3] and [6,7]
 *              Employee 2 works [2,4]
 *              Employee 3 works [2,5] and [9,12]
 *              Common free time is [5,6] (between 5 and 6) and [7,9] (between 7 and 9)
 * 
 * Approach:
 * 1. Flatten all intervals from all employees into a single list
 * 2. Sort the intervals by start time
 * 3. Merge overlapping intervals
 * 4. Find gaps between merged intervals - these are the free times
 * 
 * Time Complexity: O(N log N) where N is total number of intervals
 * Space Complexity: O(N) for storing all intervals
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace DSA_CSharp.Arrays
{
    public class Interval
    {
        public int start;
        public int end;
        
        public Interval(int start, int end)
        {
            this.start = start;
            this.end = end;
        }
    }

    public class EmployeeFreeTime
    {
        // Approach 1: Flatten, Sort, and Find Gaps
        public IList<Interval> EmployeeFreeTimeFlattening(IList<IList<Interval>> schedule)
        {
            var result = new List<Interval>();
            
            // Step 1: Flatten all intervals into one list
            var allIntervals = new List<Interval>();
            foreach (var employee in schedule)
            {
                foreach (var interval in employee)
                {
                    allIntervals.Add(interval);
                }
            }
            
            // Step 2: Sort by start time
            allIntervals.Sort((a, b) => a.start.CompareTo(b.start));
            
            // Step 3: Merge overlapping intervals and find gaps
            var merged = new List<Interval>();
            var current = allIntervals[0];
            
            for (int i = 1; i < allIntervals.Count; i++)
            {
                if (allIntervals[i].start <= current.end)
                {
                    // Overlapping - extend the current interval
                    current.end = Math.Max(current.end, allIntervals[i].end);
                }
                else
                {
                    // Gap found - add to merged and record free time
                    merged.Add(current);
                    result.Add(new Interval(current.end, allIntervals[i].start));
                    current = allIntervals[i];
                }
            }
            
            return result;
        }

        // Approach 2: Using Priority Queue (Min Heap)
        // More efficient for large number of employees with few intervals each
        public IList<Interval> EmployeeFreeTimePriorityQueue(IList<IList<Interval>> schedule)
        {
            var result = new List<Interval>();
            
            // Priority queue to store (interval, employeeIdx, intervalIdx)
            var pq = new SortedSet<(int start, int end, int empIdx, int intIdx)>(
                Comparer<(int start, int end, int empIdx, int intIdx)>.Create((a, b) =>
                {
                    int cmp = a.start.CompareTo(b.start);
                    if (cmp != 0) return cmp;
                    cmp = a.end.CompareTo(b.end);
                    if (cmp != 0) return cmp;
                    cmp = a.empIdx.CompareTo(b.empIdx);
                    if (cmp != 0) return cmp;
                    return a.intIdx.CompareTo(b.intIdx);
                })
            );
            
            // Add first interval from each employee
            for (int i = 0; i < schedule.Count; i++)
            {
                if (schedule[i].Count > 0)
                {
                    var interval = schedule[i][0];
                    pq.Add((interval.start, interval.end, i, 0));
                }
            }
            
            if (pq.Count == 0) return result;
            
            // Get the first interval
            var first = pq.Min;
            pq.Remove(first);
            int prevEnd = first.end;
            
            // Add next interval from same employee if exists
            if (first.intIdx + 1 < schedule[first.empIdx].Count)
            {
                var next = schedule[first.empIdx][first.intIdx + 1];
                pq.Add((next.start, next.end, first.empIdx, first.intIdx + 1));
            }
            
            while (pq.Count > 0)
            {
                var curr = pq.Min;
                pq.Remove(curr);
                
                if (curr.start > prevEnd)
                {
                    // Found a gap - free time
                    result.Add(new Interval(prevEnd, curr.start));
                }
                
                prevEnd = Math.Max(prevEnd, curr.end);
                
                // Add next interval from same employee if exists
                if (curr.intIdx + 1 < schedule[curr.empIdx].Count)
                {
                    var next = schedule[curr.empIdx][curr.intIdx + 1];
                    pq.Add((next.start, next.end, curr.empIdx, curr.intIdx + 1));
                }
            }
            
            return result;
        }

        // Test cases
        public static void Test()
        {
            var solver = new EmployeeFreeTime();
            
            // Test case 1
            var schedule1 = new List<IList<Interval>>
            {
                new List<Interval> { new Interval(1, 3), new Interval(6, 7) },
                new List<Interval> { new Interval(2, 4) },
                new List<Interval> { new Interval(2, 5), new Interval(9, 12) }
            };
            
            Console.WriteLine("Test Case 1:");
            Console.WriteLine("Schedule:");
            PrintSchedule(schedule1);
            var result1 = solver.EmployeeFreeTimeFlattening(schedule1);
            Console.WriteLine("Free Time (Flattening): " + FormatResult(result1));
            var result1b = solver.EmployeeFreeTimePriorityQueue(schedule1);
            Console.WriteLine("Free Time (Priority Queue): " + FormatResult(result1b));
            Console.WriteLine("Expected: [[5,6],[7,9]]");
            Console.WriteLine();
            
            // Test case 2
            var schedule2 = new List<IList<Interval>>
            {
                new List<Interval> { new Interval(1, 3), new Interval(4, 6) },
                new List<Interval> { new Interval(1, 3) },
                new List<Interval> { new Interval(1, 3) }
            };
            
            Console.WriteLine("Test Case 2:");
            Console.WriteLine("Schedule:");
            PrintSchedule(schedule2);
            var result2 = solver.EmployeeFreeTimeFlattening(schedule2);
            Console.WriteLine("Free Time (Flattening): " + FormatResult(result2));
            var result2b = solver.EmployeeFreeTimePriorityQueue(schedule2);
            Console.WriteLine("Free Time (Priority Queue): " + FormatResult(result2b));
            Console.WriteLine("Expected: [[3,4]]");
            Console.WriteLine();
            
            // Test case 3: No free time
            var schedule3 = new List<IList<Interval>>
            {
                new List<Interval> { new Interval(1, 5) },
                new List<Interval> { new Interval(2, 6) }
            };
            
            Console.WriteLine("Test Case 3 (No free time):");
            Console.WriteLine("Schedule:");
            PrintSchedule(schedule3);
            var result3 = solver.EmployeeFreeTimeFlattening(schedule3);
            Console.WriteLine("Free Time: " + FormatResult(result3));
            Console.WriteLine("Expected: []");
        }
        
        private static void PrintSchedule(List<IList<Interval>> schedule)
        {
            for (int i = 0; i < schedule.Count; i++)
            {
                Console.Write($"  Employee {i + 1}: ");
                Console.WriteLine(string.Join(", ", schedule[i].Select(interval => $"[{interval.start},{interval.end}]")));
            }
        }
        
        private static string FormatResult(IList<Interval> intervals)
        {
            if (intervals.Count == 0) return "[]";
            return "[" + string.Join(",", intervals.Select(i => $"[{i.start},{i.end}]")) + "]";
        }
    }
}
