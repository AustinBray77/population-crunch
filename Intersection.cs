using System.Collections.Generic;
using Godot;

public partial class Intersection : Area2D
{
    private static Dictionary<int, Intersection> _intersections = new Dictionary<int, Intersection>();

    public List<Road> Roads { get; protected set; }
    public int ID { get; protected set; }

    public virtual void _IReady()
    {
        GD.Print("No override for method _IReady() is available...");
    }

    public override void _Ready()
    {
        ID = _intersections.Count;
        _intersections.Add(ID, this);
        _IReady();
    }

    public static Intersection GetIntersectionByID(int id)
        => _intersections[id];

    public void AddRoad(Road road)
    {
        Roads.Add(road);
    }

    public float DistanceTo(Intersection i)
        => Position.DistanceTo(i.Position);
}