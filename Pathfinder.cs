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
        GD.Print(start.ID);
        if (currentPathTime > currentBestTime)
        {
            GD.Print("RTL");
            return new RoadPath();
        }

        if (start == end)
        {
            GD.Print("RD");
            currentBestTime = currentPathTime;
            return new RoadPath(new List<Road>());
        }

        if (path[start.ID] >= 2)
        {
            GD.Print("RAU-IP");
            return new RoadPath();
        }

        if (intersectionsUsed.ContainsKey(start.ID))
        {
            GD.Print("RAU-O");
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

        GD.Print("**--Start Roads--**");

        foreach (Road road in start.Roads)
        {
            GD.Print(road.Name);
        }

        foreach (Road road in start.Roads)
        {
            nextRoads = nextRoads.RoadInsertionSort(road, start, end);
        }

        GD.Print("**--Sorted Roads--**");

        foreach (Road road in nextRoads)
        {
            GD.Print(road.Name);
        }

        RoadPath bestResult = new RoadPath();
        for (int i = 0; i < nextRoads.Count; i++)
        {

            GD.Print(nextRoads[i].Name);

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
                GD.Print($"Adding Path for {start.ID}\n {bestResult}");
            }
        }
        else
        {
            intersectionsUsed.Add(start.ID, bestResult.Clone());
            GD.Print($"Adding Path for {start.ID}\n {bestResult}");
        }

        return bestResult.Clone();
    }
}