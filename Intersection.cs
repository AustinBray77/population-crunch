using System.Collections.Generic;
using Godot;

public partial class Intersection : Area2D
{
    public List<Road> Roads { get; protected set; }

    public void AddRoad(Road road)
    {
        Roads.Add(road);
    }

    public float DistanceTo(Intersection i)
        => Position.DistanceTo(i.Position);
}