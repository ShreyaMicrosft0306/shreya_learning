/*
============================================================
PROBLEM: Find the City With the Fewest Reachable Cities
============================================================

DESCRIPTION:
Given n cities connected by bidirectional roads, return the city that can
reach the fewest other cities within distanceThreshold. If multiple cities
have the same count, return the city with the greatest number.

Multiple roads may connect the same pair of cities. Reachability is based
on the shortest path between the cities, not just a direct road.

EXAMPLES:
1. n = 4, edges = [[0,1,3],[1,2,1],[1,3,4],[2,3,1]],
   distanceThreshold = 4 -> 3
2. n = 5, edges = [[0,1,2],[0,4,8],[1,2,3],[1,4,2],[2,3,1],[3,4,1]],
   distanceThreshold = 2 -> 0

APPROACH:
Use Floyd-Warshall to compute the shortest distance between every pair of
cities. Initialize each pair with its shortest direct road, then consider
each city as an intermediate stop and relax all routes through it.

Afterward, count each city's other cities whose shortest distance is within
the threshold. Updating the answer on equal counts ensures the larger city
number wins ties.

TIME COMPLEXITY: O(n^3 + m)
SPACE COMPLEXITY: O(n^2)

PATTERN: Graph, All-Pairs Shortest Paths, Floyd-Warshall
DIFFICULTY: Medium
============================================================
*/

namespace DSA_CSharp.Graphs
{
    public class FindCityWithFewestReachableCities
    {
        /// <summary>
        /// Returns the city that can reach the fewest other cities within
        /// the threshold, preferring the highest city number on a tie.
        /// </summary>
        /// <param name="n">Number of cities labeled from 0 to n - 1.</param>
        /// <param name="edges">Bidirectional roads [cityA, cityB, distance].</param>
        /// <param name="distanceThreshold">Maximum allowed shortest-path distance.</param>
        public int FindTheCity(int n, int[][] edges, int distanceThreshold)
        {
            long[,] distances = new long[n, n];

            for (int city = 0; city < n; city++)
            {
                for (int otherCity = 0; otherCity < n; otherCity++)
                {
                    distances[city, otherCity] = long.MaxValue;
                }

                distances[city, city] = 0;
            }

            foreach (int[] edge in edges)
            {
                int firstCity = edge[0];
                int secondCity = edge[1];
                int roadDistance = edge[2];

                if (roadDistance < distances[firstCity, secondCity])
                {
                    distances[firstCity, secondCity] = roadDistance;
                    distances[secondCity, firstCity] = roadDistance;
                }
            }

            for (int intermediate = 0; intermediate < n; intermediate++)
            {
                for (int from = 0; from < n; from++)
                {
                    if (distances[from, intermediate] == long.MaxValue)
                        continue;

                    for (int to = 0; to < n; to++)
                    {
                        if (distances[intermediate, to] == long.MaxValue)
                            continue;

                        long routeDistance =
                            distances[from, intermediate] +
                            distances[intermediate, to];

                        if (routeDistance < distances[from, to])
                            distances[from, to] = routeDistance;
                    }
                }
            }

            int cityWithFewestReachable = -1;
            int fewestReachable = int.MaxValue;

            for (int city = 0; city < n; city++)
            {
                int reachable = 0;

                for (int otherCity = 0; otherCity < n; otherCity++)
                {
                    if (city != otherCity &&
                        distances[city, otherCity] <= distanceThreshold)
                    {
                        reachable++;
                    }
                }

                if (reachable <= fewestReachable)
                {
                    fewestReachable = reachable;
                    cityWithFewestReachable = city;
                }
            }

            return cityWithFewestReachable;
        }
    }
}
