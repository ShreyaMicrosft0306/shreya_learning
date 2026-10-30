// ============================================================
// PROBLEM: Container With Most Water
// ============================================================
// Given an array where each element represents the height of a vertical
// line at that index, find two lines that together with the x-axis
// form a container that holds the maximum amount of water.
// You may not slant the container.
//
// INPUT:
//   int[] height — array where height[i] is the height of line at index i
//   Example: height = [1, 8, 6, 2, 5, 4, 8, 3, 7]
//
// OUTPUT:
//   int — the maximum water a container can hold
//   Example: 49   (between index 1 (h=8) and index 8 (h=7): min(8,7) * 7 = 49)
//
// EDGE CASES:
//   - Only two lines [3, 5]    → only one container possible: min(3,5)*1 = 3
//   - All same height [4,4,4]  → widest container wins: 4 * 2 = 8
//   - Decreasing [5,4,3,2,1]  → best is first two: min(5,4)*1 = 4... wait no
//                                best is leftmost pair: min(5,1)*4=4 or min(5,4)*1=4
//                                actually [5,4,3,2,1]: left=0,right=4 → min(5,1)*4=4
//   - One very tall line [1,1,100,1,1] → answer limited by the shorter side
//   - Two elements only [1,2]  → min(1,2) * 1 = 1
//
// ============================================================
// HOW TO SOLVE — Simple Walkthrough
// ============================================================
// Key Insight:
//   Water held = min(left height, right height) × distance between them.
//   Start with the widest possible container (left=0, right=end).
//   Then move the shorter side inward — it's the only move that could
//   ever increase the area. Moving the taller side inward is pointless
//   because the height is still capped by the shorter side AND the width shrinks.
//
// Think of it like this: the shorter line is the bottleneck.
// The only hope of doing better is to replace that bottleneck with something taller.
// So always move the pointer at the shorter line.
//
// Step-by-step with example: height = [1, 8, 6, 2, 5, 4, 8, 3, 7]
//                                       0  1  2  3  4  5  6  7  8
//
//   left=0 (h=1), right=8 (h=7)
//   area = min(1,7) × (8-0) = 1 × 8 = 8     max=8
//   h[left] < h[right] → move left inward
//
//   left=1 (h=8), right=8 (h=7)
//   area = min(8,7) × (8-1) = 7 × 7 = 49    max=49
//   h[right] < h[left] → move right inward
//
//   left=1 (h=8), right=7 (h=3)
//   area = min(8,3) × (7-1) = 3 × 6 = 18    max=49
//   h[right] < h[left] → move right inward
//
//   left=1 (h=8), right=6 (h=8)
//   area = min(8,8) × (6-1) = 8 × 5 = 40    max=49
//   equal → move either (move right)
//
//   left=1 (h=8), right=5 (h=4)
//   area = min(8,4) × (5-1) = 4 × 4 = 16    max=49
//   h[right] < h[left] → move right inward
//
//   left=1 (h=8), right=4 (h=5)
//   area = min(8,5) × (4-1) = 5 × 3 = 15    max=49
//   h[right] < h[left] → move right inward
//
//   left=1 (h=8), right=3 (h=2)
//   area = min(8,2) × (3-1) = 2 × 2 = 4     max=49
//   h[right] < h[left] → move right inward
//
//   left=1 (h=8), right=2 (h=6)
//   area = min(8,6) × (2-1) = 6 × 1 = 6     max=49
//   h[right] < h[left] → move right inward
//
//   left=1, right=1 → left >= right → STOP
//
//   Return: 49
//
// WHY MOVING THE SHORTER SIDE IS ALWAYS CORRECT?
//   Say left is shorter. Current area = h[left] × width.
//   If we move the taller side (right) inward:
//     - Width shrinks by 1
//     - Height is STILL capped at h[left] (the shorter side didn't change)
//     - Area can only decrease or stay the same → pointless move
//   If we move the shorter side (left) inward:
//     - Width shrinks by 1
//     - Height MIGHT increase if the next line is taller
//     - Area might increase → worth trying
//   So moving the taller side can never improve the result.
//   We safely skip it and always move the shorter side.
//
// WHY THIS APPROACH?
//   Brute force: check every pair of lines → O(n²). Too slow.
//   Two pointers (this): start wide, move the weaker side inward.
//   Every move is justified by the argument above, so no valid pair is skipped.
//   Result: O(n) time, O(1) space.
//
// ============================================================
// TIME & SPACE COMPLEXITY
// ============================================================
//   n = number of elements in height
//
//   Time:  O(n)
//     - left and right start at opposite ends and move toward each other
//     - Each step one pointer moves inward by 1
//     - Total steps before left >= right: at most n-1
//     - Total: O(n)
//
//   Space: O(1)
//     - Only four integer variables: left, right, area, maxArea
// ============================================================

public class ContainerWithMostWater
{
    public static int Solve(int[] height)
    {
        int left = 0;
        int right = height.Length - 1;
        int maxArea = 0;

        while (left < right)
        {
            int area = Math.Min(height[left], height[right]) * (right - left);
            maxArea = Math.Max(maxArea, area);

            // Move the shorter side inward — it's the only side worth replacing
            if (height[left] < height[right])
                left++;
            else
                right--;
        }

        return maxArea;
    }

    public static void Main(string[] args)
    {
        int[] height1 = { 1, 8, 6, 2, 5, 4, 8, 3, 7 };
        Console.WriteLine("Example 1: " + Solve(height1));
        // Expected: 49

        int[] height2 = { 1, 1 };
        Console.WriteLine("Example 2: " + Solve(height2));
        // Expected: 1

        int[] height3 = { 4, 4, 4, 4 };
        Console.WriteLine("Example 3: " + Solve(height3));
        // Expected: 12  (min(4,4) × 3)
    }
}
