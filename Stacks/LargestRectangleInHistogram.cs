/*
 * Problem: Largest Rectangle in Histogram
 * 
 * Given an array of integers heights representing the histogram's bar height where the width 
 * of each bar is 1, return the area of the largest rectangle in the histogram.
 * 
 * Example 1:
 * Input: heights = [2,1,5,6,2,3]
 * Output: 10
 * Explanation: The largest rectangle is formed by bars at index 2,3 with height 5,6.
 * The rectangle has width 2 and minimum height 5, so area = 2 * 5 = 10.
 * 
 * Example 2:
 * Input: heights = [2,4]
 * Output: 4
 * Explanation: The largest rectangle has width 1 and height 4, so area = 4.
 * 
 * Example 3:
 * Input: heights = [2,1,2]
 * Output: 2
 * Explanation: Multiple rectangles with area 2.
 * 
 * Approach:
 * Use a stack to keep track of indices of bars in increasing order of height.
 * When we find a bar shorter than the stack's top, we calculate the area with 
 * the top bar as the smallest height.
 * 
 * Key Insight:
 * For each bar, we want to know:
 * - How far left can we extend with this height?
 * - How far right can we extend with this height?
 * The area = height * width (where width is the span we can extend)
 * 
 * Time Complexity: O(n) - each bar is pushed and popped once
 * Space Complexity: O(n) - for the stack
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace DSA_CSharp.Stacks
{
    public class LargestRectangleInHistogram
    {
        // Approach 1: Optimal Stack Solution
        // Use stack to track indices of bars in increasing height order
        public int LargestRectangleArea(int[] heights)
        {
            if (heights == null || heights.Length == 0)
                return 0;
            
            var stack = new Stack<int>();
            int maxArea = 0;
            int n = heights.Length;
            
            for (int i = 0; i < n; i++)
            {
                // While current bar is shorter than stack top, calculate area
                while (stack.Count > 0 && heights[i] < heights[stack.Peek()])
                {
                    int heightIndex = stack.Pop();
                    int height = heights[heightIndex];
                    
                    // Width: from element after previous stack top to current position
                    int width = stack.Count == 0 ? i : i - stack.Peek() - 1;
                    
                    maxArea = Math.Max(maxArea, height * width);
                }
                
                stack.Push(i);
            }
            
            // Process remaining bars in stack
            while (stack.Count > 0)
            {
                int heightIndex = stack.Pop();
                int height = heights[heightIndex];
                int width = stack.Count == 0 ? n : n - stack.Peek() - 1;
                
                maxArea = Math.Max(maxArea, height * width);
            }
            
            return maxArea;
        }

        // Approach 2: Stack with Sentinel (cleaner code)
        // Add a sentinel value at the end to avoid separate processing
        public int LargestRectangleAreaWithSentinel(int[] heights)
        {
            if (heights == null || heights.Length == 0)
                return 0;
            
            var stack = new Stack<int>();
            int maxArea = 0;
            
            // Process all heights plus a sentinel 0 at the end
            for (int i = 0; i <= heights.Length; i++)
            {
                // Use 0 as sentinel for the last iteration
                int currentHeight = (i == heights.Length) ? 0 : heights[i];
                
                while (stack.Count > 0 && currentHeight < heights[stack.Peek()])
                {
                    int heightIndex = stack.Pop();
                    int height = heights[heightIndex];
                    int width = stack.Count == 0 ? i : i - stack.Peek() - 1;
                    
                    maxArea = Math.Max(maxArea, height * width);
                }
                
                stack.Push(i);
            }
            
            return maxArea;
        }

        // Approach 3: Divide and Conquer (Less efficient but good to understand)
        // Find minimum height, calculate area with it, then recurse on left and right
        public int LargestRectangleAreaDivideConquer(int[] heights)
        {
            return CalculateArea(heights, 0, heights.Length - 1);
        }
        
        private int CalculateArea(int[] heights, int start, int end)
        {
            if (start > end)
                return 0;
            
            // Find index of minimum height in current range
            int minIndex = start;
            for (int i = start; i <= end; i++)
            {
                if (heights[i] < heights[minIndex])
                    minIndex = i;
            }
            
            // Calculate area with minimum height spanning the entire range
            int areaWithMin = heights[minIndex] * (end - start + 1);
            
            // Calculate max area in left and right subarrays
            int leftArea = CalculateArea(heights, start, minIndex - 1);
            int rightArea = CalculateArea(heights, minIndex + 1, end);
            
            return Math.Max(areaWithMin, Math.Max(leftArea, rightArea));
        }

        // Approach 4: Brute Force (for understanding - O(n²))
        // For each bar, expand left and right to find maximum width
        public int LargestRectangleAreaBruteForce(int[] heights)
        {
            if (heights == null || heights.Length == 0)
                return 0;
            
            int maxArea = 0;
            
            for (int i = 0; i < heights.Length; i++)
            {
                int minHeight = heights[i];
                
                // Try all possible rectangles starting at i
                for (int j = i; j < heights.Length; j++)
                {
                    minHeight = Math.Min(minHeight, heights[j]);
                    int width = j - i + 1;
                    maxArea = Math.Max(maxArea, minHeight * width);
                }
            }
            
            return maxArea;
        }

        // Approach 5: Using Left and Right Boundaries
        // Pre-calculate how far each bar can extend left and right
        public int LargestRectangleAreaWithBoundaries(int[] heights)
        {
            if (heights == null || heights.Length == 0)
                return 0;
            
            int n = heights.Length;
            int[] left = new int[n];   // Left boundary (exclusive)
            int[] right = new int[n];  // Right boundary (exclusive)
            
            // Calculate left boundaries
            left[0] = -1;
            for (int i = 1; i < n; i++)
            {
                int p = i - 1;
                while (p >= 0 && heights[p] >= heights[i])
                {
                    p = left[p];
                }
                left[i] = p;
            }
            
            // Calculate right boundaries
            right[n - 1] = n;
            for (int i = n - 2; i >= 0; i--)
            {
                int p = i + 1;
                while (p < n && heights[p] >= heights[i])
                {
                    p = right[p];
                }
                right[i] = p;
            }
            
            // Calculate max area
            int maxArea = 0;
            for (int i = 0; i < n; i++)
            {
                int width = right[i] - left[i] - 1;
                maxArea = Math.Max(maxArea, heights[i] * width);
            }
            
            return maxArea;
        }

        // Helper method to visualize histogram
        public static void PrintHistogram(int[] heights)
        {
            if (heights == null || heights.Length == 0)
                return;
            
            int maxHeight = heights.Max();
            
            for (int level = maxHeight; level > 0; level--)
            {
                for (int i = 0; i < heights.Length; i++)
                {
                    Console.Write(heights[i] >= level ? "█ " : "  ");
                }
                Console.WriteLine();
            }
            
            for (int i = 0; i < heights.Length; i++)
            {
                Console.Write(heights[i] + " ");
            }
            Console.WriteLine();
        }

        // Test cases
        public static void Test()
        {
            var solver = new LargestRectangleInHistogram();
            
            // Test case 1
            Console.WriteLine("Test Case 1:");
            int[] heights1 = { 2, 1, 5, 6, 2, 3 };
            Console.WriteLine("Heights: [" + string.Join(",", heights1) + "]");
            PrintHistogram(heights1);
            Console.WriteLine($"Largest Area (Stack): {solver.LargestRectangleArea(heights1)}");
            Console.WriteLine($"Largest Area (With Sentinel): {solver.LargestRectangleAreaWithSentinel(heights1)}");
            Console.WriteLine($"Largest Area (Boundaries): {solver.LargestRectangleAreaWithBoundaries(heights1)}");
            Console.WriteLine("Expected: 10");
            Console.WriteLine();
            
            // Test case 2
            Console.WriteLine("Test Case 2:");
            int[] heights2 = { 2, 4 };
            Console.WriteLine("Heights: [" + string.Join(",", heights2) + "]");
            PrintHistogram(heights2);
            Console.WriteLine($"Largest Area: {solver.LargestRectangleArea(heights2)}");
            Console.WriteLine("Expected: 4");
            Console.WriteLine();
            
            // Test case 3
            Console.WriteLine("Test Case 3:");
            int[] heights3 = { 2, 1, 2 };
            Console.WriteLine("Heights: [" + string.Join(",", heights3) + "]");
            PrintHistogram(heights3);
            Console.WriteLine($"Largest Area: {solver.LargestRectangleArea(heights3)}");
            Console.WriteLine("Expected: 2");
            Console.WriteLine();
            
            // Test case 4: Increasing heights
            Console.WriteLine("Test Case 4 (Increasing):");
            int[] heights4 = { 1, 2, 3, 4, 5 };
            Console.WriteLine("Heights: [" + string.Join(",", heights4) + "]");
            PrintHistogram(heights4);
            Console.WriteLine($"Largest Area: {solver.LargestRectangleArea(heights4)}");
            Console.WriteLine("Expected: 9");
            Console.WriteLine();
            
            // Test case 5: Decreasing heights
            Console.WriteLine("Test Case 5 (Decreasing):");
            int[] heights5 = { 5, 4, 3, 2, 1 };
            Console.WriteLine("Heights: [" + string.Join(",", heights5) + "]");
            PrintHistogram(heights5);
            Console.WriteLine($"Largest Area: {solver.LargestRectangleArea(heights5)}");
            Console.WriteLine("Expected: 9");
            Console.WriteLine();
            
            // Test case 6: All same height
            Console.WriteLine("Test Case 6 (Same Height):");
            int[] heights6 = { 3, 3, 3, 3 };
            Console.WriteLine("Heights: [" + string.Join(",", heights6) + "]");
            PrintHistogram(heights6);
            Console.WriteLine($"Largest Area: {solver.LargestRectangleArea(heights6)}");
            Console.WriteLine("Expected: 12");
            Console.WriteLine();
            
            // Test case 7: Single bar
            Console.WriteLine("Test Case 7 (Single Bar):");
            int[] heights7 = { 5 };
            Console.WriteLine("Heights: [" + string.Join(",", heights7) + "]");
            Console.WriteLine($"Largest Area: {solver.LargestRectangleArea(heights7)}");
            Console.WriteLine("Expected: 5");
        }
    }
}
