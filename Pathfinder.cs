using System;
using System.Collections.Generic;
using CityExtras;

public static class Pathfinder
{
    private static Dictionary<int, RoadPath> intersectionsUsed = new();
    private static double currentBestTime = 0;

    public static RoadPath CalculateTime(Intersection start, Intersection end)
    {
        intersectionsUsed = new();
        currentBestTime = double.MaxValue;
        return _CalculateTime(start, end);
    }

    private static RoadPath _CalculateTime(Intersection start, Intersection end)
    {
        if (currentBestTime == 0)
        {
            intersectionsUsed = new();
        }

        if (start == end)
        {
            return new RoadPath(new List<Road>());
        }

        if (intersectionsUsed.ContainsKey(start.ID))
        {
            return intersectionsUsed[start.ID].Clone();
        }

        List<Road> nextRoads = new List<Road>();

        foreach (Road road in start.Roads)
        {
            nextRoads = nextRoads.RoadInsertionSort(road, start, end);
        }

        RoadPath bestResult = new RoadPath(new List<Road>());
        for (int i = 0; i < nextRoads.Count; i++)
        {
            RoadPath nextResult = CalculateTime(nextRoads[i].Destination, end);

            nextResult.Roads.Insert(0, nextRoads[i]);

            if (bestResult.Time > nextResult.Time || bestResult.Time == 0)
            {
                bestResult = new RoadPath(nextResult.Roads);
            }

            intersectionsUsed.Add(start.ID, nextResult);
        }

        return bestResult;
    }
}