/*
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PROBLEM: Best Time to Buy and Sell Stock
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DESCRIPTION:
Given an array prices where prices[i] is the stock price on day i, choose
one day to buy and a later day to sell to maximize profit.

Only one transaction is allowed.

INPUT:
- prices: int[] - Stock price for each day

OUTPUT:
- int - Maximum profit, or 0 if no profit is possible

EXAMPLES:
Example 1:
Input: prices = [7, 1, 5, 3, 6, 4]
Output: 5
Explanation: Buy at 1 and sell later at 6. Profit = 6 - 1 = 5.

Example 2:
Input: prices = [7, 6, 4, 3, 1]
Output: 0
Explanation: Prices keep decreasing, so no profitable transaction exists.

Example 3:
Input: prices = [2, 4]
Output: 2
Explanation: Buy at 2 and sell at 4.

CONSTRAINTS:
- 1 <= prices.Length <= 100,000
- 0 <= prices[i] <= 100,000

EDGE CASES:
✓ One day -> no transaction, return 0
✓ Prices always decrease -> return 0
✓ Prices always increase -> buy on the first day and sell on the last
✓ Repeated prices
✓ Best buying day is not the global minimum at the end

TRICK CASES:
⚡ The selling day must come after the buying day.
⚡ Update the best profit after considering the current selling price.
⚡ A negative profit is never useful, so keep the answer at 0.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OPTIMAL SOLUTION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

APPROACH:
Scan the prices from left to right.

For each day:
1. Treat today's price as a possible selling price.
2. Use the cheapest price seen before today as the buying price.
3. Calculate today's profit.
4. Keep the largest profit found.
5. Update the cheapest price for future days.

Because we only use prices from earlier days as buying prices, the buy day
always comes before the sell day.

ALGORITHM STEPS:
1. Set lowestPrice to the first price.
2. Set maxProfit to 0.
3. For each later price:
   a. Update maxProfit with price - lowestPrice.
   b. Update lowestPrice if the current price is cheaper.
4. Return maxProfit.

TIME COMPLEXITY: O(n)
- The prices array is scanned once.

SPACE COMPLEXITY: O(1)
- Only two variables are used.

PATTERN: Greedy, One-Pass Scan
DIFFICULTY: Easy
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
*/

namespace DSA_CSharp.Greedy
{
    public class BestTimeToBuyAndSellStock
    {
        /// <summary>
        /// Returns the maximum profit from one buy and one later sell.
        /// </summary>
        public int MaxProfit(int[] prices)
        {
            if (prices == null || prices.Length < 2)
                return 0;

            int lowestPrice = prices[0];
            int maxProfit = 0;

            for (int i = 1; i < prices.Length; i++)
            {
                int currentProfit = prices[i] - lowestPrice;
                if (currentProfit > maxProfit)
                    maxProfit = currentProfit;

                if (prices[i] < lowestPrice)
                    lowestPrice = prices[i];
            }

            return maxProfit;
        }
    }
}
