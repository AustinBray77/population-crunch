using System.Collections.Generic;
using System.Diagnostics;

public class RoadPath
{
    public List<Road> Roads { get; private set; }

    public double Time { get; private set; }

    public RoadPath(List<Road> roads)
    {
        Roads = roads;
        Time = CheckRoadTime();
    }

    public RoadPath()
    {
        Roads = new List<Road>();
        Time = double.MaxValue;
    }

    public void SetRoads(List<Road> roads)
    {
        Roads = roads;
        Time = CheckRoadTime();
    }

    public double CheckRoadTime()
    {
        double time = 0;

        foreach (Road road in Roads)
        {
            time += road.TravelTime;
        }

        return time;
    }

    public RoadPath Clone()
    {
        return new RoadPath(Roads);
    }
}