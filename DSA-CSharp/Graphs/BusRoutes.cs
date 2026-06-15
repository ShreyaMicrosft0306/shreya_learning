/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Bus Routes
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
You are given a 2D-integer array routes representing bus routes where routes[i] 
is a list of stops that the i-th bus makes. For example, if routes[0] = [3, 8, 9], 
it means the first bus (bus #0) goes through stops 3, 8, 9, 3, 8, 9, continuously 
in a circular route.

You are also given two integers source and target, representing the starting 
bus stop and the destination bus stop, respectively.

Write a function that returns the minimum number of buses you need to take to 
travel from source to target. Return -1 if it is not possible.

INPUT:
- routes: int[][] - List of bus routes, each route is list of stops
- source: int - Starting stop number
- target: int - Destination stop number
- 1 <= routes.length <= 500
- 1 <= routes[i].length <= 10^5
- All stops in routes[i] are unique
- 0 <= routes[i][j] < 10^6
- 0 <= source, target < 10^6

OUTPUT:
- int - Minimum number of buses to take, or -1 if impossible

EXAMPLES:
Example 1:
Input: routes = [[3,8,9], [5,6,8], [1,7,10]], source = 3, target = 6
Output: 2
Explanation:
- Start at stop 3
- Take bus #0 from stop 3 to stop 8 (1 bus)
- Take bus #1 from stop 8 to stop 6 (2 buses total)

Example 2:
Input: routes = [[1,2,3], [4,5,6], [7,8,9], [10,11,12]], source = 1, target = 12
Output: -1
Explanation: No bus connects these stops (they're on completely separate routes)

Example 3:
Input: routes = [[1,2,7], [3,6,7]], source = 1, target = 6
Output: 2
Explanation:
- Take bus #0 from stop 1 to stop 7 (1 bus)
- Take bus #1 from stop 7 to stop 6 (2 buses)

Example 4:
Input: routes = [[7,12], [4,5,15], [6], [15,19], [9,12,13]], source = 15, target = 12
Output: -1

CONSTRAINTS:
- 1 <= routes.length <= 500
- 1 <= routes[i].length <= 10^5
- All values of routes[i] are unique
- sum(routes[i].length) <= 10^5
- 0 <= routes[i][j] < 10^6
- 0 <= source, target < 10^6

EDGE CASES:
✓ Source equals target → return 0
✓ Source and target on same bus → return 1
✓ No bus at source stop → return -1
✓ No bus at target stop → return -1
✓ Multiple paths → return shortest
✓ Circular routes (stops repeat)
✓ Large number of stops

TRICK CASES:
⚡ Don't BFS through individual stops (too many stops, too slow!)
⚡ BFS through BUSES, not stops (key insight!)
⚡ Think: "Which buses can I take?" not "Which stops can I reach?"
⚡ Build graph of buses connected by common stops
⚡ Need mapping: stop → buses that serve that stop
⚡ Track visited buses to avoid cycles

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
SIMPLE EXPLANATION (KEY TO UNDERSTANDING!)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

WRONG APPROACH (too slow):
❌ BFS through stops: visit each stop, find connected stops
❌ Problem: Too many stops (up to 10^6), too slow

CORRECT APPROACH (the trick!):
✅ BFS through BUSES: visit each bus, find connected buses

THINK OF IT THIS WAY:
1. You're at a stop
2. You can board ANY bus that passes through this stop
3. Once on a bus, you can get off at ANY stop on that bus route
4. Then you can board any OTHER bus at that new stop

THE KEY INSIGHT:
- Instead of thinking "stop by stop"
- Think "bus by bus"
- Count how many buses you board (not how many stops you pass)

VISUAL EXAMPLE:
routes = [[3,8,9], [5,6,8], [1,7,10]]
source = 3, target = 6

Step 1: Build a map - "Which buses serve each stop?"
Stop 3 → Bus #0
Stop 8 → Bus #0, Bus #1
Stop 9 → Bus #0
Stop 5 → Bus #1
Stop 6 → Bus #1
Stop 1 → Bus #2
Stop 7 → Bus #2
Stop 10 → Bus #2

Step 2: Start at source (stop 3)
- Which buses can I take? → Bus #0
- Board Bus #0 (buses taken = 1)

Step 3: On Bus #0, I can reach stops: 3, 8, 9
- From stop 8: I can transfer to Bus #1
- Board Bus #1 (buses taken = 2)

Step 4: On Bus #1, I can reach stops: 5, 6, 8
- Stop 6 is my target! Done!
- Answer: 2 buses

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
DETAILED SOLUTION WALKTHROUGH - HOW THE CODE WORKS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Let's walk through Example 1 step by step:
routes = [[3,8,9], [5,6,8]], source = 3, target = 6

────────────────────────────────────────────────────────────────────────────
STEP 1: BUILD THE MAP (stop → buses)
────────────────────────────────────────────────────────────────────────────
We create a dictionary: "Which buses serve each stop?"

Loop through each bus:
  Bus #0 has stops [3, 8, 9]
    → Add Bus #0 to stop 3's list
    → Add Bus #0 to stop 8's list
    → Add Bus #0 to stop 9's list
  
  Bus #1 has stops [5, 6, 8]
    → Add Bus #1 to stop 5's list
    → Add Bus #1 to stop 6's list
    → Add Bus #1 to stop 8's list

Result - stopToBuses map:
  Stop 3 → [Bus #0]
  Stop 8 → [Bus #0, Bus #1]  ← SHARED by both buses!
  Stop 9 → [Bus #0]
  Stop 5 → [Bus #1]
  Stop 6 → [Bus #1]

────────────────────────────────────────────────────────────────────────────
STEP 2: INITIALIZE BFS (Start from source)
────────────────────────────────────────────────────────────────────────────
Source = stop 3

Look up stop 3 in our map:
  stopToBuses[3] = [Bus #0]

So we can board Bus #0 from source!

Initial state:
  queue = [Bus #0]
  visitedBuses = {Bus #0}
  busesCount = 1  ← We've boarded our first bus!

────────────────────────────────────────────────────────────────────────────
STEP 3: BFS LOOP - LEVEL 1 (Processing buses we've boarded)
────────────────────────────────────────────────────────────────────────────
Process Bus #0 from queue:
  
  Look at all stops on Bus #0: [3, 8, 9]
  
  For stop 3:
    - Is this the target (6)? No
    - Which buses serve stop 3? [Bus #0]
    - Already visited Bus #0? Yes, skip
  
  For stop 8:
    - Is this the target (6)? No
    - Which buses serve stop 8? [Bus #0, Bus #1]
    - Already visited Bus #0? Yes, skip
    - Already visited Bus #1? No! Add to queue
      → We can transfer to Bus #1 at stop 8!
  
  For stop 9:
    - Is this the target (6)? No
    - Which buses serve stop 9? [Bus #0]
    - Already visited Bus #0? Yes, skip

After Level 1:
  queue = [Bus #1]
  visitedBuses = {Bus #0, Bus #1}
  busesCount = 2  ← We increment after processing level

────────────────────────────────────────────────────────────────────────────
STEP 4: BFS LOOP - LEVEL 2 (Processing next set of buses)
────────────────────────────────────────────────────────────────────────────
Process Bus #1 from queue:
  
  Look at all stops on Bus #1: [5, 6, 8]
  
  For stop 5:
    - Is this the target (6)? No
    - Check for connected buses...
  
  For stop 6:
    - Is this the target (6)? YES! ✓
    - RETURN busesCount = 2

DONE! Answer: 2 buses

────────────────────────────────────────────────────────────────────────────
WHY THIS WORKS - THE CORE IDEA
────────────────────────────────────────────────────────────────────────────

Think of it like this:

1. BOARDING A BUS: When you board a bus, you get access to ALL stops 
   on that route instantly. You don't count individual stops.

2. BFS LEVELS = BUS COUNT:
   - Level 1: All buses you can reach with 1 bus
   - Level 2: All buses you can reach with 2 buses (transfer once)
   - Level 3: All buses you can reach with 3 buses (transfer twice)
   etc.

3. SHARED STOPS = TRANSFER POINTS:
   - When two buses share a stop, you can transfer
   - Example: Stop 8 has [Bus #0, Bus #1]
   - If you're on Bus #0 and reach stop 8, you can board Bus #1

4. VISITED SET: Prevents boarding the same bus multiple times
   - We mark buses as visited, not stops
   - Once you've been on a bus, no need to take it again

────────────────────────────────────────────────────────────────────────────
THE QUEUE REPRESENTS: "Buses I can currently board"
THE BUS COUNT REPRESENTS: "How many buses I've taken so far"
────────────────────────────────────────────────────────────────────────────

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OPTIMAL SOLUTION - ALGORITHM SUMMARY
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APPROACH:
Use BFS through buses (not stops!). Build a map of which buses serve each stop.
Start with buses at source, explore connected buses via shared stops.

KEY INSIGHTS:
• BFS through buses minimizes number of buses taken
• Map: stop → list of buses serving that stop
• Two buses are "connected" if they share at least one stop
• Use visited set for buses (not stops) to avoid cycles

ALGORITHM STEPS:
1. Edge case: if source == target, return 0
2. Build stop-to-buses map:
   - For each bus route, add that bus to each stop's list
3. Initialize BFS:
   - Queue: all buses that serve the source stop
   - Mark these buses as visited
   - Buses count = 1
4. BFS loop:
   - For each bus in current level:
     a. Check all stops on this bus route
     b. If target stop found, return current buses count
     c. For each stop, find all connected buses
     d. Add unvisited buses to queue for next level
   - Increment buses count
5. If queue empty and target not found, return -1

TIME COMPLEXITY: O(N × S)
- N = number of buses (routes.length)
- S = average stops per bus
- Building map: O(N × S)
- BFS: Each bus visited once, each stop checked once per bus
- Total: O(N × S)

SPACE COMPLEXITY: O(N × S)
- Stop-to-buses map: O(N × S) 
- Queue: O(N) buses
- Visited set: O(N) buses
- Total: O(N × S)

PATTERN: BFS (Graph of Buses), Mapping, Level-Order Traversal
DIFFICULTY: Hard
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

using System;
using System.Collections.Generic;

namespace DSA_CSharp.Graphs
{
    public class BusRoutes
    {
        /// <summary>
        /// Finds minimum number of buses to take from source to target.
        /// Uses BFS through buses (not stops) for optimal solution.
        /// </summary>
        /// <param name="routes">Array of bus routes (each route is list of stops)</param>
        /// <param name="source">Starting stop</param>
        /// <param name="target">Destination stop</param>
        /// <returns>Minimum buses needed, or -1 if impossible</returns>
        public int NumBusesToDestination(int[][] routes, int source, int target)
        {
            // ═══════════════════════════════════════════════════════════════
            // EDGE CASE: Already at destination - no buses needed
            // ═══════════════════════════════════════════════════════════════
            if (source == target) return 0;
            
            // ═══════════════════════════════════════════════════════════════
            // STEP 1: BUILD THE MAP - "Which buses serve each stop?"
            // ═══════════════════════════════════════════════════════════════
            // This is the KEY data structure!
            // Example: stopToBuses[8] = [Bus #0, Bus #1] means stop 8 is
            // served by both Bus #0 and Bus #1 (transfer point!)
            
            Dictionary<int, List<int>> stopToBuses = new Dictionary<int, List<int>>();
            
            for (int busIdx = 0; busIdx < routes.Length; busIdx++)
            {
                // For each stop on this bus route
                foreach (int stop in routes[busIdx])
                {
                    // Add this bus to the list of buses serving this stop
                    if (!stopToBuses.ContainsKey(stop))
                        stopToBuses[stop] = new List<int>();
                    
                    stopToBuses[stop].Add(busIdx);
                }
            }
            
            // ═══════════════════════════════════════════════════════════════
            // STEP 2: VALIDATE - Can we even start/reach?
            // ═══════════════════════════════════════════════════════════════
            if (!stopToBuses.ContainsKey(source) || !stopToBuses.ContainsKey(target))
                return -1; // No bus at source or target
            
            // ═══════════════════════════════════════════════════════════════
            // STEP 3: INITIALIZE BFS
            // ═══════════════════════════════════════════════════════════════
            // Queue holds: buses we can currently board
            // Visited tracks: buses we've already taken (avoid repeats)
            
            Queue<int> queue = new Queue<int>();
            HashSet<int> visitedBuses = new HashSet<int>();
            
            // IMPORTANT: Start by boarding ALL buses available at source
            // Why? Because we can choose any bus at our starting stop
            foreach (int busIdx in stopToBuses[source])
            {
                queue.Enqueue(busIdx);        // Add to queue to explore
                visitedBuses.Add(busIdx);      // Mark as boarded
            }
            
            int busesCount = 1; // We've boarded our first bus!
            
            // ═══════════════════════════════════════════════════════════════
            // STEP 4: BFS LOOP - Explore buses level by level
            // ═══════════════════════════════════════════════════════════════
            // Each level = one more bus transfer
            // Level 1: buses reachable with 1 bus
            // Level 2: buses reachable with 2 buses (1 transfer)
            // etc.
            
            while (queue.Count > 0)
            {
                int levelSize = queue.Count;
                
                // Process ALL buses at current level before moving to next
                // This ensures we count buses correctly!
                for (int i = 0; i < levelSize; i++)
                {
                    int currentBus = queue.Dequeue();
                    
                    // ───────────────────────────────────────────────────────
                    // Check ALL stops this bus visits
                    // ───────────────────────────────────────────────────────
                    // Once you're ON a bus, you can get off at ANY of its stops
                    
                    foreach (int stop in routes[currentBus])
                    {
                        // ✓ CHECK: Is this stop our target?
                        if (stop == target)
                            return busesCount; // Success!
                        
                        // ───────────────────────────────────────────────────
                        // TRANSFER LOGIC: At this stop, which OTHER buses
                        // can we board?
                        // ───────────────────────────────────────────────────
                        
                        if (stopToBuses.ContainsKey(stop))
                        {
                            // Look at all buses serving this stop
                            foreach (int nextBus in stopToBuses[stop])
                            {
                                // Have we already taken this bus?
                                if (!visitedBuses.Contains(nextBus))
                                {
                                    // No! This is a NEW bus we can transfer to
                                    visitedBuses.Add(nextBus);
                                    queue.Enqueue(nextBus);
                                    
                                    // This bus will be explored in NEXT level
                                    // (next iteration of while loop)
                                }
                            }
                        }
                    }
                }
                
                // ───────────────────────────────────────────────────────────
                // Finished processing this level
                // Increment bus count (we're taking one more bus)
                // ───────────────────────────────────────────────────────────
                busesCount++;
            }
            
            // ═══════════════════════════════════════════════════════════════
            // Queue is empty and we never found target
            // ═══════════════════════════════════════════════════════════════
            return -1; // Target not reachable
        }
        
        // Test method with detailed explanation
        public static void Test()
        {
            var solution = new BusRoutes();
            
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.WriteLine("BUS ROUTES PROBLEM - DETAILED WALKTHROUGH");
            Console.WriteLine("═══════════════════════════════════════════════════════\n");
            
            // Test case 1: Standard case
            int[][] routes1 = new int[][]
            {
                new int[] {3, 8, 9},   // Bus #0
                new int[] {5, 6, 8},   // Bus #1
                new int[] {1, 7, 10}   // Bus #2
            };
            int source1 = 3, target1 = 6;
            
            Console.WriteLine("Test Case 1:");
            Console.WriteLine("Routes:");
            Console.WriteLine("  Bus #0: [3, 8, 9]");
            Console.WriteLine("  Bus #1: [5, 6, 8]");
            Console.WriteLine("  Bus #2: [1, 7, 10]");
            Console.WriteLine($"Source: {source1}, Target: {target1}\n");
            
            Console.WriteLine("Step-by-step:");
            Console.WriteLine("1. At stop 3 → Can board Bus #0");
            Console.WriteLine("2. On Bus #0 → Can reach stops 3, 8, 9");
            Console.WriteLine("3. At stop 8 → Can transfer to Bus #1");
            Console.WriteLine("4. On Bus #1 → Can reach stops 5, 6, 8");
            Console.WriteLine("5. At stop 6 → TARGET REACHED!");
            Console.WriteLine("Buses taken: Bus #0 + Bus #1 = 2 buses\n");
            
            int result1 = solution.NumBusesToDestination(routes1, source1, target1);
            Console.WriteLine($"Result: {result1}");
            Console.WriteLine($"Expected: 2\n");
            Console.WriteLine("───────────────────────────────────────────────────────\n");
            
            // Test case 2: Impossible
            int[][] routes2 = new int[][]
            {
                new int[] {1, 2, 3},
                new int[] {4, 5, 6},
                new int[] {7, 8, 9},
                new int[] {10, 11, 12}
            };
            int source2 = 1, target2 = 12;
            
            Console.WriteLine("Test Case 2 (Impossible):");
            Console.WriteLine("Routes:");
            Console.WriteLine("  Bus #0: [1, 2, 3]");
            Console.WriteLine("  Bus #1: [4, 5, 6]");
            Console.WriteLine("  Bus #2: [7, 8, 9]");
            Console.WriteLine("  Bus #3: [10, 11, 12]");
            Console.WriteLine($"Source: {source2}, Target: {target2}\n");
            
            Console.WriteLine("Analysis:");
            Console.WriteLine("- Start at stop 1 (on Bus #0)");
            Console.WriteLine("- Bus #0 goes to: 1, 2, 3");
            Console.WriteLine("- No other bus shares stops with Bus #0");
            Console.WriteLine("- Cannot reach stop 12 (on Bus #3)");
            Console.WriteLine("- Result: IMPOSSIBLE\n");
            
            int result2 = solution.NumBusesToDestination(routes2, source2, target2);
            Console.WriteLine($"Result: {result2}");
            Console.WriteLine($"Expected: -1\n");
            Console.WriteLine("───────────────────────────────────────────────────────\n");
            
            // Test case 3: Same stop
            int[][] routes3 = new int[][]
            {
                new int[] {1, 2, 7},
                new int[] {3, 6, 7}
            };
            int source3 = 1, target3 = 6;
            
            Console.WriteLine("Test Case 3:");
            Console.WriteLine("Routes:");
            Console.WriteLine("  Bus #0: [1, 2, 7]");
            Console.WriteLine("  Bus #1: [3, 6, 7]");
            Console.WriteLine($"Source: {source3}, Target: {target3}\n");
            
            Console.WriteLine("Step-by-step:");
            Console.WriteLine("1. At stop 1 → Board Bus #0");
            Console.WriteLine("2. On Bus #0 → Reach stop 7");
            Console.WriteLine("3. At stop 7 → Transfer to Bus #1 (both buses stop here!)");
            Console.WriteLine("4. On Bus #1 → Reach stop 6 (TARGET!)");
            Console.WriteLine("Buses taken: 2\n");
            
            int result3 = solution.NumBusesToDestination(routes3, source3, target3);
            Console.WriteLine($"Result: {result3}");
            Console.WriteLine($"Expected: 2\n");
            Console.WriteLine("───────────────────────────────────────────────────────\n");
            
            // Test case 4: Already at target
            int[][] routes4 = new int[][]
            {
                new int[] {1, 2, 3}
            };
            int source4 = 2, target4 = 2;
            
            Console.WriteLine("Test Case 4 (Already at target):");
            Console.WriteLine($"Source: {source4}, Target: {target4}");
            int result4 = solution.NumBusesToDestination(routes4, source4, target4);
            Console.WriteLine($"Result: {result4}");
            Console.WriteLine($"Expected: 0\n");
            Console.WriteLine("───────────────────────────────────────────────────────\n");
            
            // Test case 5: Single bus
            int[][] routes5 = new int[][]
            {
                new int[] {1, 2, 3, 4, 5}
            };
            int source5 = 1, target5 = 5;
            
            Console.WriteLine("Test Case 5 (Single bus):");
            Console.WriteLine("Routes:");
            Console.WriteLine("  Bus #0: [1, 2, 3, 4, 5]");
            Console.WriteLine($"Source: {source5}, Target: {target5}");
            Console.WriteLine("Both stops on same bus → Only 1 bus needed\n");
            
            int result5 = solution.NumBusesToDestination(routes5, source5, target5);
            Console.WriteLine($"Result: {result5}");
            Console.WriteLine($"Expected: 1\n");
        }
    }
}
