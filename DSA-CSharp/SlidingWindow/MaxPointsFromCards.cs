// ============================================================
// PROBLEM: Max Points You Can Obtain From Cards
// ============================================================
// Given an array of card values, pick exactly k cards from either
// end (front or back) to maximise your total score. You may split
// your picks — some from the front, the rest from the back — but
// you cannot skip cards or pick from the middle.
//
// INPUT:
//   int[] cards — card values
//   int k       — exact number of cards to pick
//   Example: cards = [2, 11, 4, 5, 3, 9, 2], k = 3
//
// OUTPUT:
//   int — maximum achievable score
//   Example: 17   (take first 3: 2 + 11 + 4 = 17)
//
// EDGE CASES:
//   - k = cards.Length    → must take all cards, return total sum
//   - k = 1               → return max of first card vs last card
//   - All equal [3,3,3,3] → every combination has the same sum
//   - Single card [5], k=1 → return 5
//
// ============================================================
// HOW TO SOLVE — Simple Walkthrough
// ============================================================
// Key Insight (Sliding Window across the boundary):
//   Every valid pick is some number of cards from the left (0..k)
//   and the rest from the right. That gives exactly k+1 combinations:
//
//     left=3, right=0:  [2, 11, 4]            = 17
//     left=2, right=1:  [2, 11]      + [2]    = 15
//     left=1, right=2:  [2]          + [9, 2] = 13
//     left=0, right=3:  []           + [3,9,2] = 14
//
//   Instead of computing each combination from scratch, start with
//   all k cards from the left, then slide one card at a time:
//   drop the rightmost left-card, pick up the next card from the right end.
//
// Step-by-step with example: cards = [2, 11, 4, 5, 3, 9, 2], k = 3
//
//   n = 7
//
//   Initial window — all k from the left:
//     sum = cards[0]+cards[1]+cards[2] = 2+11+4 = 17   max=17
//
//   Slide 1 (i=1): drop cards[k-i]=cards[2]=4, add cards[n-i]=cards[6]=2
//     sum = 17 - 4 + 2 = 15   max=17
//     represents: [2, 11] + [2]
//
//   Slide 2 (i=2): drop cards[k-i]=cards[1]=11, add cards[n-i]=cards[5]=9
//     sum = 15 - 11 + 9 = 13   max=17
//     represents: [2] + [9, 2]
//
//   Slide 3 (i=3): drop cards[k-i]=cards[0]=2, add cards[n-i]=cards[4]=3
//     sum = 13 - 2 + 3 = 14   max=17
//     represents: [] + [3, 9, 2]
//
//   Return: 17
//
// HOW THE INDICES WORK:
//   cards[k - i] = the rightmost card of the shrinking left portion
//   cards[n - i] = the next card to pick up from the right end
//
//   cards = [2, 11, 4,  5,  3,  9,  2]
//             0   1  2  3  4   5   6
//                     ↑               ↑
//             left end drops here   right end adds from here
//
// WHY THIS APPROACH?
//   Brute force: enumerate all k+1 combinations and sum each → O(k²).
//   Sliding window (this): reuse previous sum, one subtract + one add per step.
//   Total: O(k) which is at most O(n). Clean and minimal.
//
//   Alternative — minimum middle window:
//   Total sum - min sum of middle subarray of size (n-k) = answer.
//   Same O(n) complexity but requires computing total sum first.
//   The sliding window approach is more direct.
//
// ============================================================
// TIME & SPACE COMPLEXITY
// ============================================================
//   n = number of cards,  k = cards to pick
//
//   Time:  O(k)
//     - Building initial window of size k: O(k)
//     - Sliding k times, one operation per step: O(k)
//     - k ≤ n, so this is at most O(n)
//     - Total: O(k)
//
//   Space: O(1)
//     - Only three integer variables: windowSum, maxSum, loop index i
// ============================================================

public class MaxPointsFromCards
{
    public static int Solve(int[] cards, int k)
    {
        int n = cards.Length;

        // Step 1: Start with all k cards from the left
        int windowSum = 0;
        for (int i = 0; i < k; i++)
            windowSum += cards[i];

        int maxSum = windowSum;

        // Step 2: Slide — drop one from the left end, pick one from the right end
        for (int i = 1; i <= k; i++)
        {
            windowSum -= cards[k - i];   // remove rightmost card of left portion
            windowSum += cards[n - i];   // add next card from right end
            maxSum = Math.Max(maxSum, windowSum);
        }

        return maxSum;
    }

    public static void Main(string[] args)
    {
        int[] cards1 = { 2, 11, 4, 5, 3, 9, 2 };
        Console.WriteLine("Example 1: " + Solve(cards1, 3));
        // Expected: 17  (first 3: 2+11+4)

        int[] cards2 = { 1, 100, 10, 0, 4, 5, 6 };
        Console.WriteLine("Example 2: " + Solve(cards2, 3));
        // Expected: 111  (first 3: 1+100+10)

        int[] cards3 = { 9, 7, 7, 9, 7, 7, 9 };
        Console.WriteLine("Example 3: " + Solve(cards3, 7));
        // Expected: 55  (take all cards)

        int[] cards4 = { 1, 79, 80, 1, 1, 1, 200, 1 };
        Console.WriteLine("Example 4: " + Solve(cards4, 3));
        // Expected: 202  (last 2 + first 1: 200+1+1)
    }
}
