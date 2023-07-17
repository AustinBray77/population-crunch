using Godot;
using System;
using System.Collections.Generic;
using CityExtras;

public partial class Road : Path2D
{
    public Intersection Origin { get; private set; }
    public Intersection Destination { get; private set; }

    public List<Person> People { get; private set; }

    [Export]
    public int Speed { get; private set; }

    [Export]
    public float Length { get; private set; }

    public float TravelTime { get => Length / Speed; }

    // Called after _Ready by the instantiating function
    public void Initialize(Intersection origin, Intersection destination, int speed)
    {
        origin.Roads.Add(this);
        Origin = origin;
        Destination = destination;
        People = new List<Person>();
        Speed = speed;
        Length = origin.Position.DistanceTo(destination.Position);

        _DrawRoad();
    }

    private void _DrawRoad()
    {
        var children = GetChildren();

        while (children.Count > 0)
        {
            RemoveChild(children[0]);
        }

        Line2D roadLine = Graphics.DrawLine(Curve.GetBakedPoints(), Graphics.TarGray, 20, Position);

        Vector2 roadDirectionVector = Destination.Position - Origin.Position;

        Line2D[] arrow = Graphics.DrawArrow(roadDirectionVector, 1000, Graphics.Green, 20, Position + Origin.Position);

        AddChild(roadLine);

        foreach (Line2D line in arrow)
        {
            AddChild(line);
        }
    }

    public void SplitAtIntersection(Intersection newIntersection)
    {
        Intersection oldDestination = Destination;

        Destination = newIntersection;

        Road continuation = Main._roadScene.Instantiate<Road>();

        _DrawRoad();

        newIntersection.Roads.Add(continuation);
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
