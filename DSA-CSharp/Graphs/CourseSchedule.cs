/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Course Schedule
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
You have to take a total of numCourses courses, labeled from 0 to numCourses-1.
You are given a list of prerequisites pairs, where prerequisites[i] = [a, b] 
indicates that you must complete course b before course a.

Determine if it is possible to finish all courses.

INPUT:
- numCourses: int - Total number of courses (labeled 0 to numCourses-1)
- prerequisites: int[][] - List of prerequisite pairs [a, b] meaning b → a
- 1 <= numCourses <= 2000
- 0 <= prerequisites.length <= 5000
- prerequisites[i].length == 2
- 0 <= ai, bi < numCourses
- All pairs are unique

OUTPUT:
- bool - true if possible to finish all courses, false otherwise

EXAMPLES:
Example 1:
Input: numCourses = 3, prerequisites = [[1,0], [2,1]]
Output: true
Explanation: 
- Course 0 has no prerequisites
- Course 1 requires course 0
- Course 2 requires course 1
- Valid order: 0 → 1 → 2 ✓

Example 2:
Input: numCourses = 3, prerequisites = [[1,0], [0,1], [1,2]]
Output: false
Explanation:
- Course 1 requires course 0
- Course 0 requires course 1
- Circular dependency! 0 ⇄ 1 ✗
- Impossible to complete

Example 3:
Input: numCourses = 2, prerequisites = [[1,0]]
Output: true
Explanation: Take course 0 first, then course 1

Example 4:
Input: numCourses = 2, prerequisites = []
Output: true
Explanation: No prerequisites, can take any order

Example 5:
Input: numCourses = 4, prerequisites = [[1,0], [2,0], [3,1], [3,2]]
Output: true
Explanation:
- Course 0: no prerequisites
- Courses 1,2: require course 0
- Course 3: requires courses 1 and 2
- Valid order: 0 → 1, 0 → 2, 1 → 3, 2 → 3 ✓

CONSTRAINTS:
- 1 <= numCourses <= 2000
- 0 <= prerequisites.length <= 5000
- prerequisites[i].length == 2
- 0 <= ai, bi < numCourses
- All prerequisite pairs are unique

EDGE CASES:
✓ No prerequisites → always possible
✓ Single course → always possible
✓ Self-loop: [0,0] → impossible (cycle)
✓ Direct cycle: [1,0], [0,1] → impossible
✓ Indirect cycle: [1,0], [2,1], [0,2] → impossible
✓ Multiple components (disconnected courses)
✓ All courses have prerequisites

TRICK CASES:
⚡ This is a CYCLE DETECTION problem in a directed graph!
⚡ If there's a cycle, courses cannot be completed
⚡ Direction matters: [1,0] means 0→1 (0 before 1)
⚡ Not asking for the order, just if it's possible
⚡ Courses can have multiple prerequisites
⚡ Some courses may be independent (no prerequisites)

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
UNDERSTANDING THE PROBLEM - GRAPH REPRESENTATION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

GRAPH INTERPRETATION:
- Each course is a NODE
- Prerequisite [a, b] creates EDGE: b → a
  (meaning: must complete b before a)

Example 1: [[1,0], [2,1]]
Graph:
  0 → 1 → 2

This is a DAG (Directed Acyclic Graph) → Possible! ✓

Example 2: [[1,0], [0,1]]
Graph:
  0 ⟷ 1  (cycle!)

This has a CYCLE → Impossible! ✗

KEY INSIGHT:
Course completion is possible ⟺ Graph has NO CYCLES
(i.e., the graph must be a DAG - Directed Acyclic Graph)

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
SOLUTION 1: DFS WITH CYCLE DETECTION (Most Intuitive)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APPROACH:
Use DFS to detect cycles. Track three states for each node:
- UNVISITED (0): Node not yet explored
- VISITING (1): Node currently being explored (in DFS stack)
- VISITED (2): Node fully explored (all descendants checked)

If we visit a node that's currently VISITING, we found a cycle!

KEY INSIGHTS:
• Use adjacency list to represent graph
• Track visit states: 0 = unvisited, 1 = visiting, 2 = visited
• During DFS, if we reach a "visiting" node → cycle detected!
• If we reach a "visited" node → safe, already checked
• Must check all components (not all nodes may be connected)

ALGORITHM STEPS:
1. Build adjacency list from prerequisites
2. Initialize state array (all nodes UNVISITED)
3. For each unvisited course:
   - Start DFS
   - Mark as VISITING
   - Recursively visit all neighbors
   - If neighbor is VISITING → cycle! return false
   - If neighbor is VISITED → skip (already checked)
   - Mark as VISITED after all neighbors checked
4. If no cycle found in any component → return true

WALKTHROUGH: [[1,0], [2,1], [0,2]]

Graph: 0 → 1 → 2 → 0 (cycle!)

DFS from node 0:
- Mark 0 as VISITING (1)
- Visit neighbor 1
  - Mark 1 as VISITING (1)
  - Visit neighbor 2
    - Mark 2 as VISITING (1)
    - Visit neighbor 0
      - Node 0 is VISITING! → CYCLE DETECTED! ✗

TIME COMPLEXITY: O(V + E)
- V = numCourses (vertices)
- E = prerequisites.length (edges)
- Visit each node once, traverse each edge once

SPACE COMPLEXITY: O(V + E)
- Adjacency list: O(V + E)
- State array: O(V)
- Recursion stack: O(V) in worst case
- Total: O(V + E)

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;
using System.Collections.Generic;

namespace DSA_CSharp.Graphs
{
    public class CourseSchedule
    {
        // Node states for cycle detection
        private const int UNVISITED = 0;
        private const int VISITING = 1;
        private const int VISITED = 2;
        
        /// <summary>
        /// Determines if all courses can be finished using DFS cycle detection.
        /// </summary>
        /// <param name="numCourses">Total number of courses</param>
        /// <param name="prerequisites">List of [course, prerequisite] pairs</param>
        /// <returns>True if possible to finish all courses</returns>
        public bool CanFinish(int numCourses, int[][] prerequisites)
        {
            // ═══════════════════════════════════════════════════════════════
            // STEP 1: BUILD ADJACENCY LIST
            // ═══════════════════════════════════════════════════════════════
            // For prerequisite [a, b]: add edge b → a
            
            List<int>[] graph = new List<int>[numCourses];
            for (int i = 0; i < numCourses; i++)
            {
                graph[i] = new List<int>();
            }
            
            foreach (var prereq in prerequisites)
            {
                int course = prereq[0];
                int prerequisite = prereq[1];
                // Add edge: prerequisite → course
                graph[prerequisite].Add(course);
            }
            
            // ═══════════════════════════════════════════════════════════════
            // STEP 2: INITIALIZE STATE TRACKING
            // ═══════════════════════════════════════════════════════════════
            int[] state = new int[numCourses]; // All start as UNVISITED (0)
            
            // ═══════════════════════════════════════════════════════════════
            // STEP 3: DFS FROM EACH UNVISITED NODE
            // ═══════════════════════════════════════════════════════════════
            // Need to check all components (graph might be disconnected)
            
            for (int course = 0; course < numCourses; course++)
            {
                if (state[course] == UNVISITED)
                {
                    // If DFS detects a cycle, return false
                    if (HasCycleDFS(course, graph, state))
                    {
                        return false; // Cycle detected! Cannot finish courses
                    }
                }
            }
            
            // ═══════════════════════════════════════════════════════════════
            // NO CYCLES FOUND → CAN FINISH ALL COURSES! ✓
            // ═══════════════════════════════════════════════════════════════
            return true;
        }
        
        /// <summary>
        /// DFS helper to detect cycles in directed graph.
        /// </summary>
        /// <param name="course">Current course being explored</param>
        /// <param name="graph">Adjacency list representation</param>
        /// <param name="state">State tracking array</param>
        /// <returns>True if cycle detected, false otherwise</returns>
        private bool HasCycleDFS(int course, List<int>[] graph, int[] state)
        {
            // ───────────────────────────────────────────────────────────────
            // Mark current course as VISITING (currently in DFS path)
            // ───────────────────────────────────────────────────────────────
            state[course] = VISITING;
            
            // ───────────────────────────────────────────────────────────────
            // Visit all neighbors (courses that depend on current course)
            // ───────────────────────────────────────────────────────────────
            foreach (int nextCourse in graph[course])
            {
                if (state[nextCourse] == VISITING)
                {
                    // ═══════════════════════════════════════════════════════
                    // CYCLE DETECTED! ✗
                    // ═══════════════════════════════════════════════════════
                    // We reached a course that's currently being visited
                    // This means there's a path back to a node in our DFS stack
                    return true;
                }
                
                if (state[nextCourse] == UNVISITED)
                {
                    // Recursively check if there's a cycle through this neighbor
                    if (HasCycleDFS(nextCourse, graph, state))
                    {
                        return true; // Cycle found in subtree
                    }
                }
                
                // If state[nextCourse] == VISITED, skip it (already fully explored)
            }
            
            // ───────────────────────────────────────────────────────────────
            // Mark as VISITED (all descendants checked, no cycle found)
            // ───────────────────────────────────────────────────────────────
            state[course] = VISITED;
            return false; // No cycle through this path
        }
        
        // ═══════════════════════════════════════════════════════════════════
        // SOLUTION 2: BFS WITH TOPOLOGICAL SORT (Kahn's Algorithm)
        // ═══════════════════════════════════════════════════════════════════
        // Alternative approach using in-degree counting
        
        /// <summary>
        /// Determines if all courses can be finished using BFS topological sort.
        /// Uses Kahn's algorithm with in-degree counting.
        /// </summary>
        public bool CanFinishBFS(int numCourses, int[][] prerequisites)
        {
            // ═══════════════════════════════════════════════════════════════
            // STEP 1: BUILD GRAPH AND CALCULATE IN-DEGREES
            // ═══════════════════════════════════════════════════════════════
            List<int>[] graph = new List<int>[numCourses];
            int[] inDegree = new int[numCourses]; // Count of incoming edges
            
            for (int i = 0; i < numCourses; i++)
            {
                graph[i] = new List<int>();
            }
            
            foreach (var prereq in prerequisites)
            {
                int course = prereq[0];
                int prerequisite = prereq[1];
                graph[prerequisite].Add(course);
                inDegree[course]++; // Increase in-degree of dependent course
            }
            
            // ═══════════════════════════════════════════════════════════════
            // STEP 2: ADD ALL COURSES WITH NO PREREQUISITES TO QUEUE
            // ═══════════════════════════════════════════════════════════════
            Queue<int> queue = new Queue<int>();
            for (int i = 0; i < numCourses; i++)
            {
                if (inDegree[i] == 0)
                {
                    queue.Enqueue(i); // Can take this course immediately
                }
            }
            
            // ═══════════════════════════════════════════════════════════════
            // STEP 3: PROCESS COURSES LEVEL BY LEVEL
            // ═══════════════════════════════════════════════════════════════
            int coursesCompleted = 0;
            
            while (queue.Count > 0)
            {
                int currentCourse = queue.Dequeue();
                coursesCompleted++;
                
                // Remove edges from current course to its dependents
                foreach (int nextCourse in graph[currentCourse])
                {
                    inDegree[nextCourse]--;
                    
                    // If in-degree becomes 0, all prerequisites met!
                    if (inDegree[nextCourse] == 0)
                    {
                        queue.Enqueue(nextCourse);
                    }
                }
            }
            
            // ═══════════════════════════════════════════════════════════════
            // CHECK: DID WE COMPLETE ALL COURSES?
            // ═══════════════════════════════════════════════════════════════
            // If we completed all courses → no cycle
            // If some courses remain → they're in a cycle
            return coursesCompleted == numCourses;
        }
        
        // Test method with detailed explanations
        public static void Test()
        {
            var solution = new CourseSchedule();
            
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.WriteLine("COURSE SCHEDULE - CYCLE DETECTION IN DIRECTED GRAPH");
            Console.WriteLine("═══════════════════════════════════════════════════════\n");
            
            // Test case 1: Valid course order
            int numCourses1 = 3;
            int[][] prereq1 = new int[][] { new int[] {1, 0}, new int[] {2, 1} };
            
            Console.WriteLine("Test Case 1: Linear dependency");
            Console.WriteLine($"Courses: {numCourses1}");
            Console.WriteLine("Prerequisites: [[1,0], [2,1]]");
            Console.WriteLine("\nGraph: 0 → 1 → 2");
            Console.WriteLine("This is a valid DAG (no cycles)\n");
            
            bool result1DFS = solution.CanFinish(numCourses1, prereq1);
            bool result1BFS = solution.CanFinishBFS(numCourses1, prereq1);
            
            Console.WriteLine($"DFS Result: {result1DFS}");
            Console.WriteLine($"BFS Result: {result1BFS}");
            Console.WriteLine($"Expected: true\n");
            Console.WriteLine("Valid order: Course 0 → Course 1 → Course 2 ✓\n");
            Console.WriteLine("───────────────────────────────────────────────────────\n");
            
            // Test case 2: Cycle detected
            int numCourses2 = 3;
            int[][] prereq2 = new int[][] 
            { 
                new int[] {1, 0}, 
                new int[] {0, 1},
                new int[] {1, 2}
            };
            
            Console.WriteLine("Test Case 2: Circular dependency");
            Console.WriteLine($"Courses: {numCourses2}");
            Console.WriteLine("Prerequisites: [[1,0], [0,1], [1,2]]");
            Console.WriteLine("\nGraph:");
            Console.WriteLine("  0 ⟷ 1 → 2");
            Console.WriteLine("  Cycle between 0 and 1!\n");
            
            bool result2DFS = solution.CanFinish(numCourses2, prereq2);
            bool result2BFS = solution.CanFinishBFS(numCourses2, prereq2);
            
            Console.WriteLine($"DFS Result: {result2DFS}");
            Console.WriteLine($"BFS Result: {result2BFS}");
            Console.WriteLine($"Expected: false\n");
            Console.WriteLine("Impossible! Must take 1 before 0 AND 0 before 1 ✗\n");
            Console.WriteLine("───────────────────────────────────────────────────────\n");
            
            // Test case 3: Simple valid case
            int numCourses3 = 2;
            int[][] prereq3 = new int[][] { new int[] {1, 0} };
            
            Console.WriteLine("Test Case 3: Simple prerequisite");
            Console.WriteLine($"Courses: {numCourses3}");
            Console.WriteLine("Prerequisites: [[1,0]]");
            Console.WriteLine("\nGraph: 0 → 1");
            Console.WriteLine("Valid: Take course 0, then course 1\n");
            
            bool result3DFS = solution.CanFinish(numCourses3, prereq3);
            bool result3BFS = solution.CanFinishBFS(numCourses3, prereq3);
            
            Console.WriteLine($"DFS Result: {result3DFS}");
            Console.WriteLine($"BFS Result: {result3BFS}");
            Console.WriteLine($"Expected: true ✓\n");
            Console.WriteLine("───────────────────────────────────────────────────────\n");
            
            // Test case 4: No prerequisites
            int numCourses4 = 2;
            int[][] prereq4 = new int[0][];
            
            Console.WriteLine("Test Case 4: No prerequisites");
            Console.WriteLine($"Courses: {numCourses4}");
            Console.WriteLine("Prerequisites: []");
            Console.WriteLine("\nGraph: 0   1 (no edges)");
            Console.WriteLine("Can take courses in any order\n");
            
            bool result4DFS = solution.CanFinish(numCourses4, prereq4);
            bool result4BFS = solution.CanFinishBFS(numCourses4, prereq4);
            
            Console.WriteLine($"DFS Result: {result4DFS}");
            Console.WriteLine($"BFS Result: {result4BFS}");
            Console.WriteLine($"Expected: true ✓\n");
            Console.WriteLine("───────────────────────────────────────────────────────\n");
            
            // Test case 5: Complex valid graph
            int numCourses5 = 4;
            int[][] prereq5 = new int[][] 
            { 
                new int[] {1, 0}, 
                new int[] {2, 0},
                new int[] {3, 1},
                new int[] {3, 2}
            };
            
            Console.WriteLine("Test Case 5: Multiple paths, no cycle");
            Console.WriteLine($"Courses: {numCourses5}");
            Console.WriteLine("Prerequisites: [[1,0], [2,0], [3,1], [3,2]]");
            Console.WriteLine("\nGraph:");
            Console.WriteLine("      0");
            Console.WriteLine("     / \\");
            Console.WriteLine("    1   2");
            Console.WriteLine("     \\ /");
            Console.WriteLine("      3");
            Console.WriteLine("\nThis is a DAG - valid!\n");
            
            bool result5DFS = solution.CanFinish(numCourses5, prereq5);
            bool result5BFS = solution.CanFinishBFS(numCourses5, prereq5);
            
            Console.WriteLine($"DFS Result: {result5DFS}");
            Console.WriteLine($"BFS Result: {result5BFS}");
            Console.WriteLine($"Expected: true ✓");
            Console.WriteLine("\nValid order: 0 → 1, 0 → 2, then 3");
            Console.WriteLine("(Course 3 needs both 1 and 2 completed)\n");
            Console.WriteLine("───────────────────────────────────────────────────────\n");
            
            // Test case 6: Indirect cycle
            int numCourses6 = 3;
            int[][] prereq6 = new int[][] 
            { 
                new int[] {1, 0}, 
                new int[] {2, 1},
                new int[] {0, 2}
            };
            
            Console.WriteLine("Test Case 6: Indirect cycle");
            Console.WriteLine($"Courses: {numCourses6}");
            Console.WriteLine("Prerequisites: [[1,0], [2,1], [0,2]]");
            Console.WriteLine("\nGraph: 0 → 1 → 2 → 0 (cycle!)");
            Console.WriteLine("Circular dependency: 0 needs 2, 2 needs 1, 1 needs 0\n");
            
            bool result6DFS = solution.CanFinish(numCourses6, prereq6);
            bool result6BFS = solution.CanFinishBFS(numCourses6, prereq6);
            
            Console.WriteLine($"DFS Result: {result6DFS}");
            Console.WriteLine($"BFS Result: {result6BFS}");
            Console.WriteLine($"Expected: false ✗\n");
            Console.WriteLine("Impossible due to cycle!\n");
            
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.WriteLine("KEY TAKEAWAYS:");
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.WriteLine("✓ Course schedule = Cycle detection in directed graph");
            Console.WriteLine("✓ Possible to finish ⟺ No cycles (DAG)");
            Console.WriteLine("✓ DFS: Track VISITING state to detect back edges");
            Console.WriteLine("✓ BFS: Count in-degrees, process nodes with in-degree 0");
            Console.WriteLine("✓ Both approaches: O(V + E) time, O(V + E) space");
        }
    }
}
