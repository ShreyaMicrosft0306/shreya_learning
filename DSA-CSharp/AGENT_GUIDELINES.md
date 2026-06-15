# Agent Guidelines for DSA Problem Creation

## 🎯 Purpose
This file MUST be read before creating any new DSA problem file. It defines the standard structure and requirements for all coding problems in this repository.

## 📁 Folder Structure by Pattern

Organize problems into folders based on their primary pattern.
**Folders are created as needed when problems are added.**

Pattern categories to use:

- **Arrays/** - Array manipulation, sliding window, two pointers
- **Strings/** - String manipulation, pattern matching
- **LinkedLists/** - Linked list operations
- **Trees/** - Binary trees, BST, tree traversals
- **Graphs/** - Graph traversal, BFS, DFS, topological sort
- **DynamicProgramming/** - DP problems, memoization, tabulation
- **Stacks/** - Stack-based problems, monotonic stack
- **Queues/** - Queue operations, priority queues
- **Heaps/** - Heap operations, top K problems
- **HashMaps/** - Hash table problems, frequency counting
- **Sorting/** - Sorting algorithms and problems
- **Searching/** - Binary search, search algorithms
- **Backtracking/** - Recursive backtracking problems
- **Greedy/** - Greedy algorithm problems
- **Math/** - Mathematical problems, number theory
- **BitManipulation/** - Bit operations
- **Design/** - System design, data structure design
- **Intervals/** - Interval problems, merging intervals
- **Matrix/** - 2D array, matrix traversal
- **Recursion/** - Recursive problems

**Note**: Create the folder only when adding the first problem of that pattern.

## 📝 File Naming Convention

- Use PascalCase: `ProblemName.cs`
- Be descriptive: `PacificAtlanticWaterFlow.cs`, `DailyTemperatures.cs`
- Avoid abbreviations unless commonly known

## 🔖 Required File Structure

Every problem file MUST follow this exact structure:

```csharp
/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: [Problem Name]
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
[Clear problem statement with context]

INPUT:
[Describe input parameters with types and constraints]

OUTPUT:
[Describe expected output]

EXAMPLES:
Example 1:
Input: [example input]
Output: [example output]
Explanation: [why this output]

Example 2:
Input: [example input]
Output: [example output]

CONSTRAINTS:
- [List all constraints]
- [Include ranges, limits, special conditions]

EDGE CASES:
✓ Empty input / null values
✓ Single element
✓ All same elements
✓ Maximum size input
✓ Minimum values
✓ [Problem-specific edge cases]

TRICK CASES:
⚡ [Cases that might break naive solutions]
⚡ [Common mistakes to avoid]
⚡ [Corner cases that need special handling]

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OPTIMAL SOLUTION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APPROACH:
[Simple, clear explanation of the optimal approach]
[Break down the algorithm step-by-step]
[Explain WHY this approach is optimal]

KEY INSIGHTS:
• [Main insight that makes this solution work]
• [Any patterns or techniques used]

ALGORITHM STEPS:
1. [Step 1]
2. [Step 2]
3. [Continue...]

TIME COMPLEXITY: O(?)
- [Explain why this is the time complexity]

SPACE COMPLEXITY: O(?)
- [Explain auxiliary space used]

PATTERN: [Primary pattern name - e.g., "Monotonic Stack", "DFS", "Sliding Window"]
DIFFICULTY: [Easy/Medium/Hard]
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

namespace DSA_CSharp.[PatternFolder]
{
    public class [ProblemName]
    {
        /// <summary>
        /// [Brief description of what this method does]
        /// </summary>
        public [ReturnType] SolutionMethod([Parameters])
        {
            // Implementation here
        }
    }
}
```

## ✅ Quality Checklist

Before creating a problem file, ensure:

- [ ] Problem statement is clear and complete
- [ ] All examples have explanations
- [ ] Edge cases are comprehensive
- [ ] Trick cases highlight common pitfalls
- [ ] Solution explanation is simple and intuitive
- [ ] Algorithm is the MOST optimal known solution
- [ ] Time complexity analysis is accurate
- [ ] Space complexity includes auxiliary space
- [ ] Pattern is correctly identified
- [ ] File is in the correct folder
- [ ] Code compiles and runs
- [ ] Namespace matches folder structure

## 🎨 Code Style

- Use clear, descriptive variable names
- Add inline comments for complex logic
- Follow C# naming conventions
- Keep methods focused and readable
- Use LINQ when it improves readability

## 📚 Common Patterns Reference

| Pattern | Example Problems |
|---------|-----------------|
| Two Pointers | Two Sum II, Container With Most Water |
| Sliding Window | Longest Substring Without Repeating Characters |
| Fast & Slow Pointers | Linked List Cycle, Find Duplicate Number |
| Merge Intervals | Merge Intervals, Meeting Rooms |
| Cyclic Sort | Find Missing Number |
| In-place Reversal | Reverse Linked List |
| BFS | Binary Tree Level Order, Word Ladder |
| DFS | Number of Islands, Clone Graph |
| Binary Search | Search in Rotated Array |
| Top K Elements | Top K Frequent Elements (Heap) |
| K-way Merge | Merge K Sorted Lists |
| Monotonic Stack | Daily Temperatures, Next Greater Element |
| Backtracking | N-Queens, Word Search |
| Dynamic Programming | Longest Common Subsequence, Coin Change |

## 🚀 When Creating a New Problem

1. **Identify the pattern** - Which category does it belong to?
2. **Create in correct folder** - Use the pattern folders above
3. **Follow the template** - Include ALL required sections
4. **Optimize first** - Only include the most optimal solution
5. **Explain clearly** - Simple language, no jargon
6. **Test edge cases** - Think through all scenarios

---
**Last Updated:** May 23, 2026
