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
        currentBestTime = 1000000;
        return _CalculateTime(start, end, 0, new HashSet<int>());
    }

    private static void Log(string s)
    {
        if (!File.Exists(filePath))
        {
            File.Create(filePath);
        }

        StreamWriter file = new StreamWriter(filePath, true);

        file.WriteLine(s);
        file.Flush();
        file.Close();
    }

    private static RoadPath _CalculateTime(Intersection start, Intersection end, double currentPathTime, HashSet<int> path)
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

        if (path.Contains(start.ID))
        {
            return new RoadPath();
        }

        if (intersectionsUsed.ContainsKey(start.ID))
        {
            return intersectionsUsed[start.ID];
        }

        GD.Print(start.ID.ToString());

        HashSet<int> newPath = new HashSet<int>(path);
        newPath.Add(start.ID);

        List<Road> nextRoads = new List<Road>();

        GD.Print(start.Roads.Count.ToString());

        foreach (Road road in start.Roads)
        {
            nextRoads = nextRoads.RoadInsertionSort(road, start, end);
        }

        RoadPath bestResult = new RoadPath();
        for (int i = 0; i < nextRoads.Count; i++)
        {

            RoadPath nextResult = _CalculateTime(nextRoads[i].Destination, end, currentPathTime + nextRoads[i].TravelTime, newPath);

            nextResult.Roads.Insert(0, nextRoads[i]);

            if (bestResult.Time > nextResult.Time)
            {
                bestResult = new RoadPath(nextResult.Roads);
            }

        }
        intersectionsUsed.Add(start.ID, bestResult);

        return bestResult;
    }
}