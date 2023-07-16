using Godot;
using System;
using System.Collections.Generic;
using CityExtras;

public partial class Road : Path2D
{
    public City Origin { get; private set; }
    public City Destination { get; private set; }

    public List<Person> People { get; private set; }

    [Export]
    public int Speed { get; private set; }

    [Export]
    public float Length { get; private set; }

    public float TravelTime { get => Length / Speed; }

    // Called after _Ready by the instantiating function
    public void Initialize(City origin, City destination, int speed)
    {
        origin.Roads.Add(this);
        Origin = origin;
        Destination = destination;
        People = new List<Person>();
        Speed = speed;
        Length = Functions.Distance(origin, destination);

        _DrawRoad();
    }

    private void _DrawRoad()
    {
        Line2D roadLine = Graphics.DrawLine(Curve.GetBakedPoints(), Graphics.TarGray, 20, Position);

        Vector2 roadDirectionVector = Destination.Position - Origin.Position;

        Line2D[] arrow = Graphics.DrawArrow(roadDirectionVector, 1000, Graphics.Green, 20, Position + Origin.Position);

        AddChild(roadLine);

        foreach (Line2D line in arrow)
        {
            AddChild(line);
        }
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
