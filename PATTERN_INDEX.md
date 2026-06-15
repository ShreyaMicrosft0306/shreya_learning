# Pattern Index 📚

Quick reference guide for all DSA patterns and their characteristics.

## 🎯 How to Identify Patterns

| Pattern | When to Use | Key Characteristics |
|---------|-------------|---------------------|
| **Two Pointers** | Array/String with sorted or specific order | Two indices moving towards/away from each other |
| **Sliding Window** | Contiguous subarray/substring problems | Fixed or variable window size, optimize over window |
| **Fast & Slow Pointers** | Linked list cycle detection, middle element | Two pointers at different speeds |
| **Merge Intervals** | Overlapping intervals, scheduling | Sort intervals, merge overlapping |
| **BFS** | Level-order traversal, shortest path (unweighted) | Queue-based, explore level by level |
| **DFS** | Explore all paths, backtracking | Stack/Recursion-based, go deep first |
| **Binary Search** | Sorted array search, search space reduction | Divide and conquer, O(log n) |
| **Monotonic Stack** | Next greater/smaller element | Stack maintains increasing/decreasing order |
| **Heap/Priority Queue** | Top K elements, Kth largest/smallest | Extract min/max efficiently |
| **Dynamic Programming** | Optimization, counting ways | Overlapping subproblems, optimal substructure |
| **Backtracking** | Generate all combinations/permutations | Explore + undo, constraint satisfaction |
| **Greedy** | Local optimal leads to global optimal | Make best choice at each step |
| **Union Find** | Connected components, cycle detection | Disjoint sets, merge operations |
| **Trie** | Prefix matching, word search | Tree for string storage |
| **Topological Sort** | DAG ordering, prerequisites | DFS or BFS-based ordering |

## 📊 Pattern Categories

### Linear Data Structures
- **Arrays** - Index manipulation, in-place operations
- **Strings** - Character arrays, pattern matching
- **LinkedLists** - Pointer manipulation, reversal
- **Stacks** - LIFO, monotonic stack, expression parsing
- **Queues** - FIFO, BFS, sliding window

### Non-Linear Data Structures
- **Trees** - Recursion, traversals (inorder, preorder, postorder)
- **Graphs** - DFS, BFS, shortest paths, connectivity
- **Heaps** - Priority operations, top K problems
- **Tries** - Prefix trees, autocomplete

### Algorithmic Techniques
- **Binary Search** - Search space reduction, O(log n)
- **Two Pointers** - Optimize nested loops to O(n)
- **Sliding Window** - Optimize subarray problems
- **Dynamic Programming** - Memoization, tabulation
- **Greedy** - Local optimal choices
- **Backtracking** - Generate all solutions
- **Divide & Conquer** - Break into subproblems

## 🔍 Problem Recognition Guide

### Ask These Questions:

1. **Is the data sorted?** → Binary Search, Two Pointers
2. **Need to find subarrays/substrings?** → Sliding Window
3. **Involves linked list?** → Fast & Slow Pointers
4. **Tree/Graph traversal?** → BFS (level-order), DFS (all paths)
5. **Next greater/smaller element?** → Monotonic Stack
6. **Top K elements?** → Heap/Priority Queue
7. **Overlapping intervals?** → Merge Intervals
8. **Optimization problem?** → Dynamic Programming or Greedy
9. **Generate all combinations?** → Backtracking
10. **Connected components?** → Union Find, DFS

## 💡 Complexity Cheat Sheet

| Pattern | Typical Time | Typical Space |
|---------|-------------|---------------|
| Two Pointers | O(n) | O(1) |
| Sliding Window | O(n) | O(k) or O(1) |
| Binary Search | O(log n) | O(1) |
| BFS | O(V + E) | O(V) |
| DFS | O(V + E) | O(V) |
| Heap Operations | O(log n) | O(n) |
| Dynamic Programming | O(n²) typical | O(n) or O(n²) |
| Backtracking | O(2ⁿ) or O(n!) | O(n) |

## 🎓 Learning Resources

For each pattern, this repository contains:
- ✅ Example problems demonstrating the pattern
- ✅ Detailed explanations of when and how to apply
- ✅ Complexity analysis
- ✅ Common variations and tricks

## 📈 Difficulty Progression

1. **Easy** - Arrays, Two Pointers, HashMaps, Simple Recursion
2. **Medium** - Sliding Window, BFS, DFS, Binary Search, Stacks
3. **Hard** - Dynamic Programming, Backtracking, Advanced Graphs, Tries

---

**Tip:** When encountering a new problem, identify its pattern first before coding!
