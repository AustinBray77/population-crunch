using System;
using System.IO;
using System.Collections.Generic;
using CityExtras;
using Godot;

public static class Pathfinder
{
    private static Dictionary<int, RoadPath> intersectionsUsed = new();
    private static double currentBestTime = 0;
    private static string filePath = @"C:\temp\population-crunch.logs.txt";

    public static RoadPath CalculateTime(Intersection start, Intersection end)
    {
        intersectionsUsed = new Dictionary<int, RoadPath>();
        currentBestTime = 10000000;
        return _CalculateTime(start, end, 0, new int[Main.Instance.GetTotalCityCount()]);
    }

    private static RoadPath _CalculateTime(Intersection start, Intersection end, double currentPathTime, int[] path)
    {
        if (currentPathTime > currentBestTime)
        {
            return new RoadPath();
        }

        if (start == end)
        {
            currentBestTime = currentPathTime;
            return new RoadPath(new List<Road>());
        }

        if (path[start.ID] >= 2)
        {
            return new RoadPath();
        }

        if (intersectionsUsed.ContainsKey(start.ID))
        {
            //GD.Print(intersectionsUsed[start.ID]);
            return intersectionsUsed[start.ID];
        }

        int[] newPath = new int[path.Length];

        for (int i = 0; i < path.Length; i++)
        {
            newPath[i] = path[i];
        }

        newPath[start.ID]++;

        List<Road> nextRoads = new List<Road>();

        foreach (Road road in start.Roads)
        {
            nextRoads = nextRoads.RoadInsertionSort(road, start, end);
        }

        RoadPath bestResult = new RoadPath();
        for (int i = 0; i < nextRoads.Count; i++)
        {

            RoadPath nextResult = _CalculateTime(nextRoads[i].Destination, end, currentPathTime + nextRoads[i].TravelTime, newPath);

            nextResult.InsertRoad(0, nextRoads[i]);

            if (bestResult.Time > nextResult.Time)
            {
                //GD.Print("*--Best Result--*");

                //GD.Print(nextResult.ToString());

                bestResult = nextResult.Clone();
            }

        }


        if (intersectionsUsed.ContainsKey(start.ID))
        {
            if (intersectionsUsed[start.ID].Time > bestResult.Time)
            {
                intersectionsUsed[start.ID] = bestResult.Clone();
            }
        }
        else
        {
            intersectionsUsed.Add(start.ID, bestResult.Clone());
        }

        return bestResult.Clone();
    }
}