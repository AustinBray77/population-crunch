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
        GD.Print(start.ID);
        if (currentPathTime > currentBestTime)
        {
            GD.Print("Removed - Too Long");
            return new RoadPath();
        }

        if (start == end)
        {
            GD.Print("Reached Destination");
            currentBestTime = currentPathTime;
            return new RoadPath(new List<Road>());
        }

        if (path.Contains(start.ID))
        {
            GD.Print("Removed - Already Used In Path");
            return new RoadPath();
        }

        if (intersectionsUsed.ContainsKey(start.ID))
        {
            GD.Print("Removed - Already Used Overall");
            GD.Print(intersectionsUsed[start.ID]);
            return intersectionsUsed[start.ID];
        }

        HashSet<int> newPath = new HashSet<int>();

        foreach (int i in path)
        {
            newPath.Add(i);
        }

        newPath.Add(start.ID);

        List<Road> nextRoads = new List<Road>();

        foreach (Road road in start.Roads)
        {
            nextRoads = nextRoads.RoadInsertionSort(road, start, end);
        }

        RoadPath bestResult = new RoadPath();
        for (int i = 0; i < nextRoads.Count; i++)
        {
            GD.Print(nextRoads[i].Name);

            RoadPath nextResult = _CalculateTime(nextRoads[i].Destination, end, currentPathTime + nextRoads[i].TravelTime, newPath);

            nextResult.InsertRoad(0, nextRoads[i]);

            if (bestResult.Time > nextResult.Time)
            {
                GD.Print("*--Best Result--*");

                GD.Print(nextResult.ToString());

                bestResult = nextResult.Clone();
            }

        }

        intersectionsUsed.Add(start.ID, bestResult.Clone());
        GD.Print($"Adding Path for {start.ID}\n {bestResult}");

        return bestResult.Clone();
    }
}