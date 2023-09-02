using System.Collections.Generic;
using System.Diagnostics;

public class RoadPath
{
    public static double MaxTime = 10000000;

    private List<Road> _roads { get; set; }

    public double Time { get; private set; }

    public bool Complete { get; private set; }

    public RoadPath(List<Road> roads)
    {
        _roads = new List<Road>(roads);
        Complete = true;
        Time = CheckRoadTime();
    }

    public RoadPath()
    {
        _roads = new List<Road>();
        Time = MaxTime;
        Complete = false;
    }

    public RoadPath(List<Road> roads, double time, bool complete)
    {
        _roads = new List<Road>(roads);
        Time = time;
        Complete = complete;
    }

    public void SetRoads(List<Road> roads)
    {
        _roads = new List<Road>(roads);
        Time = CheckRoadTime();
    }

    public List<Road> GetRoads() =>
        _roads;

    public Road GetRoad(int index) =>
        _roads[index];

    public void InsertRoad(int index, Road road)
    {
        _roads.Insert(index, road);
        Time += road.TravelTime;
    }

    public double CheckRoadTime()
    {
        double time = 0;

        foreach (Road road in _roads)
        {
            time += road.TravelTime;
        }

        return time;
    }

    public override string ToString()
    {
        string output = "";

        output += "Path: ";

        foreach (Road road in _roads)
        {
            output += road.Name + "\n";
        }

        output += $"Time: {Time}";

        return output;
    }

    public RoadPath Clone()
    {
        return new RoadPath(_roads, Time, Complete);
    }
}