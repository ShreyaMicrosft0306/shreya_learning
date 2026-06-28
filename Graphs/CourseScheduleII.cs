/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Course Schedule II
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
You have to take a total of numCourses courses, which are labeled from 0 to 
numCourses - 1. You are given a list of prerequisites pairs, where 
prerequisites[i] = [a, b] indicates that you must complete course b before 
course a.

Given the total number of courses and a list of prerequisite pairs, write a 
function to return the ordering of courses you should take to finish all courses.

If there are multiple valid orderings, return any valid ordering. If it is 
impossible to finish all courses, return an empty array.

INPUT:
- numCourses: int - Total number of courses (labeled 0 to numCourses - 1)
- prerequisites: int[][] - Array of pairs [a, b] meaning b must be taken before a
- 1 <= numCourses <= 2000
- 0 <= prerequisites.length <= 5000
- prerequisites[i].length == 2
- 0 <= ai, bi < numCourses
- All pairs [ai, bi] are distinct

OUTPUT:
- int[] - Valid ordering of courses, or empty array if impossible

EXAMPLES:
Example 1:
Input: numCourses = 4, prerequisites = [[1,0], [2,0], [3,1], [3,2]]
Output: [0, 1, 2, 3] or [0, 2, 1, 3]
Explanation:
- Course 0 has no prerequisites
- Course 1 requires course 0
- Course 2 requires course 0
- Course 3 requires courses 1 and 2
Valid orderings: [0,1,2,3] or [0,2,1,3]

Graph visualization:
    0 ──→ 1 ──→ 3
    │           ↑
    └──→ 2 ─────┘

Example 2:
Input: numCourses = 2, prerequisites = [[1, 0], [0, 1]]
Output: []
Explanation:
- Course 0 requires course 1
- Course 1 requires course 0
This creates a cycle, making it impossible to complete all courses.

Graph visualization:
    0 ⇄ 1  (cycle)

Example 3:
Input: numCourses = 1, prerequisites = []
Output: [0]
Explanation: Only one course with no prerequisites.

Example 4:
Input: numCourses = 3, prerequisites = [[1,0], [1,2], [0,1]]
Output: []
Explanation: Cycle between courses 0 and 1.

CONSTRAINTS:
- 1 <= numCourses <= 2000
- 0 <= prerequisites.length <= 5000
- prerequisites[i].length == 2
- 0 <= ai, bi < numCourses
- All pairs [ai, bi] are distinct

EDGE CASES:
✓ No prerequisites → return [0, 1, 2, ..., numCourses-1]
✓ Single course → return [0]
✓ Simple cycle → return []
✓ Complex cycle in middle of graph → return []
✓ Multiple valid orderings → return any valid one
✓ All courses depend on one course → valid (start with that course)
✓ Linear chain of dependencies → valid

TRICK CASES:
⚡ Detect cycles: Use in-degree counting (Kahn's algorithm)
⚡ Cycle exists if processed count < numCourses
⚡ Must track in-degree for each node
⚡ Process only nodes with 0 in-degree
⚡ Multiple valid answers: any topological sort is valid
⚡ [a,b] means b→a (b before a), not a→b

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OPTIMAL SOLUTION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APPROACH:
Use KAHN'S ALGORITHM (BFS-based topological sort) with in-degree tracking.
Build a directed graph where edge b→a represents "b must be taken before a".
Process courses in topological order by starting with courses that have no 
prerequisites (in-degree = 0).

This is a classic topological sorting problem with cycle detection.

KEY INSIGHTS:
• Topological sort only exists if graph is a DAG (Directed Acyclic Graph)
• In-degree = number of prerequisites for a course
• Courses with in-degree 0 can be taken immediately
• After taking a course, reduce in-degree of all dependent courses
• If we can't process all courses, there's a cycle

ALGORITHM STEPS:
1. Build graph representation:
   - adjacency[i] = list of courses that depend on course i
   - inDegree[i] = number of prerequisites for course i
   - For each [a, b]: add edge b→a (b before a)
   
2. Initialize queue with all courses having in-degree 0:
   - These are courses with no prerequisites
   - Can be taken immediately
   
3. Process courses using BFS:
   - For each course in queue:
     a. Add to result (this is the order)
     b. For each dependent course:
        - Reduce its in-degree by 1
        - If in-degree becomes 0, add to queue
        
4. Check for cycles:
   - If result.length < numCourses: cycle exists, return []
   - Otherwise: return result (valid ordering)

DETAILED WALKTHROUGH (Example 1):
Input: numCourses = 4, prerequisites = [[1,0], [2,0], [3,1], [3,2]]

Graph: 0→1→3, 0→2→3
In-degrees: [0, 1, 1, 2]
Adjacency: 
  0: [1, 2]
  1: [3]
  2: [3]
  3: []

Process:
1. Start: queue = [0] (in-degree 0)
2. Process 0: result = [0], reduce in-degree of 1,2 → queue = [1, 2]
3. Process 1: result = [0,1], reduce in-degree of 3 → queue = [2]
4. Process 2: result = [0,1,2], reduce in-degree of 3 → queue = [3]
5. Process 3: result = [0,1,2,3], done
6. result.length = 4 = numCourses ✓

TIME COMPLEXITY: O(V + E)
- V = numCourses (vertices)
- E = prerequisites.length (edges)
- Build graph: O(E)
- Process each vertex once: O(V)
- Process each edge once: O(E)
- Total: O(V + E)

SPACE COMPLEXITY: O(V + E)
- Adjacency list: O(E)
- In-degree array: O(V)
- Queue: O(V) in worst case
- Result array: O(V)
- Total: O(V + E)

PATTERN: Topological Sort, Kahn's Algorithm, Graph, BFS, Cycle Detection
DIFFICULTY: Medium
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;
using System.Collections.Generic;

namespace DSA_CSharp.Graphs
{
    public class CourseScheduleII
    {
        /// <summary>
        /// Returns the ordering of courses to take to finish all courses.
        /// Uses Kahn's algorithm (topological sort with in-degree tracking).
        /// </summary>
        /// <param name="numCourses">Total number of courses</param>
        /// <param name="prerequisites">Array of [a, b] pairs where b must be taken before a</param>
        /// <returns>Valid course ordering, or empty array if impossible</returns>
        public int[] FindOrder(int numCourses, int[][] prerequisites)
        {
            // Step 1: Build graph and calculate in-degrees
            List<int>[] adjacency = new List<int>[numCourses];
            int[] inDegree = new int[numCourses];
            
            // Initialize adjacency list
            for (int i = 0; i < numCourses; i++)
            {
                adjacency[i] = new List<int>();
            }
            
            // Build graph: [a, b] means b → a (b before a)
            foreach (var prereq in prerequisites)
            {
                int course = prereq[0];
                int prerequisite = prereq[1];
                
                adjacency[prerequisite].Add(course);  // prerequisite → course
                inDegree[course]++;  // course depends on prerequisite
            }
            
            // Step 2: Initialize queue with all courses having in-degree 0
            Queue<int> queue = new Queue<int>();
            for (int i = 0; i < numCourses; i++)
            {
                if (inDegree[i] == 0)
                {
                    queue.Enqueue(i);
                }
            }
            
            // Step 3: Process courses in topological order
            List<int> result = new List<int>();
            
            while (queue.Count > 0)
            {
                int currentCourse = queue.Dequeue();
                result.Add(currentCourse);  // Add to result (this is the order)
                
                // Reduce in-degree of all dependent courses
                foreach (int dependentCourse in adjacency[currentCourse])
                {
                    inDegree[dependentCourse]--;
                    
                    // If in-degree becomes 0, all prerequisites are met
                    if (inDegree[dependentCourse] == 0)
                    {
                        queue.Enqueue(dependentCourse);
                    }
                }
            }
            
            // Step 4: Check for cycles
            // If we processed all courses, return result
            // Otherwise, there's a cycle (impossible to complete)
            return result.Count == numCourses ? result.ToArray() : new int[0];
        }
        
        /// <summary>
        /// Alternative solution using DFS-based topological sort.
        /// Detects cycles using three states: unvisited, visiting, visited.
        /// </summary>
        public int[] FindOrderDFS(int numCourses, int[][] prerequisites)
        {
            // Build adjacency list
            List<int>[] adjacency = new List<int>[numCourses];
            for (int i = 0; i < numCourses; i++)
            {
                adjacency[i] = new List<int>();
            }
            
            foreach (var prereq in prerequisites)
            {
                int course = prereq[0];
                int prerequisite = prereq[1];
                adjacency[prerequisite].Add(course);
            }
            
            // States: 0 = unvisited, 1 = visiting, 2 = visited
            int[] state = new int[numCourses];
            List<int> result = new List<int>();
            
            // Try to visit each course
            for (int i = 0; i < numCourses; i++)
            {
                if (state[i] == 0)
                {
                    if (HasCycleDFS(i, adjacency, state, result))
                    {
                        return new int[0];  // Cycle detected
                    }
                }
            }
            
            // DFS adds courses in reverse order, so reverse the result
            result.Reverse();
            return result.ToArray();
        }
        
        /// <summary>
        /// DFS helper to detect cycles and build topological order.
        /// Returns true if cycle is detected.
        /// </summary>
        private bool HasCycleDFS(int course, List<int>[] adjacency, int[] state, List<int> result)
        {
            if (state[course] == 1)  // Currently visiting (cycle detected)
                return true;
            
            if (state[course] == 2)  // Already visited
                return false;
            
            // Mark as visiting
            state[course] = 1;
            
            // Visit all neighbors
            foreach (int neighbor in adjacency[course])
            {
                if (HasCycleDFS(neighbor, adjacency, state, result))
                {
                    return true;  // Cycle detected in neighbor
                }
            }
            
            // Mark as visited and add to result
            state[course] = 2;
            result.Add(course);  // Add in post-order (after all dependencies)
            
            return false;  // No cycle
        }
        
        /// <summary>
        /// Helper method to visualize the course graph.
        /// </summary>
        private void PrintGraph(int numCourses, int[][] prerequisites)
        {
            Console.WriteLine("Course Dependencies:");
            
            // Group by course
            var courseDeps = new Dictionary<int, List<int>>();
            for (int i = 0; i < numCourses; i++)
            {
                courseDeps[i] = new List<int>();
            }
            
            foreach (var prereq in prerequisites)
            {
                courseDeps[prereq[0]].Add(prereq[1]);
            }
            
            // Print
            foreach (var kvp in courseDeps)
            {
                if (kvp.Value.Count > 0)
                {
                    Console.WriteLine($"Course {kvp.Key} requires: {string.Join(", ", kvp.Value)}");
                }
                else
                {
                    Console.WriteLine($"Course {kvp.Key} has no prerequisites");
                }
            }
        }
        
        /// <summary>
        /// Helper method to validate the ordering.
        /// Checks if all prerequisites come before their dependent courses.
        /// </summary>
        private bool IsValidOrder(int[] order, int[][] prerequisites)
        {
            if (order.Length == 0) return false;
            
            // Create position map: course → position in order
            var position = new Dictionary<int, int>();
            for (int i = 0; i < order.Length; i++)
            {
                position[order[i]] = i;
            }
            
            // Check each prerequisite
            foreach (var prereq in prerequisites)
            {
                int course = prereq[0];
                int prerequisite = prereq[1];
                
                // Prerequisite must come before course
                if (position[prerequisite] >= position[course])
                {
                    return false;
                }
            }
            
            return true;
        }
    }
}

/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
TESTING GUIDE
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Test Cases:

1. Example 1 - Multiple valid orderings:
   Input: numCourses = 4, prerequisites = [[1,0], [2,0], [3,1], [3,2]]
   Expected: [0,1,2,3] or [0,2,1,3]
   
2. Example 2 - Simple cycle:
   Input: numCourses = 2, prerequisites = [[1,0], [0,1]]
   Expected: []
   
3. Single course:
   Input: numCourses = 1, prerequisites = []
   Expected: [0]
   
4. No prerequisites:
   Input: numCourses = 3, prerequisites = []
   Expected: [0,1,2] or any permutation
   
5. Linear chain:
   Input: numCourses = 4, prerequisites = [[1,0], [2,1], [3,2]]
   Expected: [0,1,2,3]
   
6. Complex cycle:
   Input: numCourses = 4, prerequisites = [[1,0], [2,1], [3,2], [0,3]]
   Expected: [] (cycle: 0→1→2→3→0)
   
7. Multiple components:
   Input: numCourses = 4, prerequisites = [[1,0], [3,2]]
   Expected: [0,2,1,3] or [2,0,3,1] etc.

Usage Example:

var solution = new CourseScheduleII();

// Test 1: Multiple valid orderings
int[][] prereqs1 = new int[][] { 
    new int[] {1,0}, 
    new int[] {2,0}, 
    new int[] {3,1}, 
    new int[] {3,2} 
};
int[] result1 = solution.FindOrder(4, prereqs1);
Console.WriteLine($"Order: [{string.Join(", ", result1)}]");

// Test 2: Cycle detection
int[][] prereqs2 = new int[][] { 
    new int[] {1,0}, 
    new int[] {0,1} 
};
int[] result2 = solution.FindOrder(2, prereqs2);
Console.WriteLine($"Order: [{string.Join(", ", result2)}]");  // []

// Test DFS approach
int[] result3 = solution.FindOrderDFS(4, prereqs1);
Console.WriteLine($"DFS Order: [{string.Join(", ", result3)}]");

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
KEY TAKEAWAYS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

✓ Kahn's Algorithm: BFS-based topological sort using in-degree
✓ In-degree: Number of incoming edges (prerequisites)
✓ Cycle Detection: If processed count < numCourses, cycle exists
✓ Start with in-degree 0: Courses with no prerequisites
✓ Edge direction: [a,b] means b→a (prerequisite→course)
✓ Multiple solutions: Any valid topological order is acceptable
✓ DFS Alternative: Use three states to detect cycles during traversal
✓ Post-order: DFS adds nodes after visiting all dependencies (reverse result)

Related Problems:
- Course Schedule (detection only, no ordering)
- Alien Dictionary
- Minimum Height Trees
- Parallel Courses
- Sort Items by Groups Respecting Dependencies

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/
