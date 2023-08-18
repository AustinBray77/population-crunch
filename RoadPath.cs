using System.Collections.Generic;
using System.Diagnostics;

public class RoadPath
{
    public List<Road> Roads
    {
        get => Roads;
        set
        {
            Roads = value;
            Time = CheckRoadTime();
        }
    }

    public double Time { get; private set; }

    public RoadPath(List<Road> roads)
    {
        Roads = roads;
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