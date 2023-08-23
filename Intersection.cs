using System.Collections.Generic;
using Godot;

public partial class Intersection : Area2D
{
    public List<Road> Roads { get; protected set; }
    public int ID { get; protected set; }

    public virtual void _IReady()
    {
        GD.Print("No override for method _IReady() is available...");
    }

    /*public static Intersection GetIntersectionByID(int id)
        => _intersections[id];*/

    public void SetID(int id) =>
        ID = id;

    public void AddRoad(Road road)
    {
        Roads.Add(road);
    }

    public float DistanceTo(Intersection i)
        => Position.DistanceTo(i.Position);
}