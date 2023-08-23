using Godot;
using System;
using System.Collections.Generic;
using CityExtras;

public partial class Road : Path2D
{
    public Intersection Origin;

    public Intersection Destination;

    public List<Person> People { get; private set; }

    public Line RoadLine { get; private set; }

    [Export]
    public int Speed { get; private set; }

    [Export]
    public float Length { get; private set; }

    public float TravelTime { get => Length / Speed; }

    public void Update()
    {
        RoadLine = new Line(Origin.Position, Destination.Position);
        Name = $"Road {Origin.Name} -> {Destination.Name}";

        Curve2D newCurve = new Curve2D();

        newCurve.AddPoint(Origin.Position);
        newCurve.AddPoint(Destination.Position);

        Curve = newCurve;
    }

    // Called after _Ready by the instantiating function
    public void Initialize(Intersection origin, Intersection destination, int speed)
    {
        Origin = origin;
        Destination = destination;

        Origin.AddRoad(this);

        People = new List<Person>();
        Speed = speed;
        Length = origin.Position.DistanceTo(destination.Position);

        Update();
        _DrawRoad();
    }

    private void _DrawRoad()
    {
        while (GetChildCount() > 0)
        {
            this.DeleteChild(0);
        }

        Line2D roadLine = Graphics.DrawLine(Curve.GetBakedPoints(), Graphics.TarGray, 20, Position);

        //Vector2 roadDirectionVector = Destination.Position - Origin.Position;

        //Line2D[] arrow = Graphics.DrawArrow(roadDirectionVector, 1000, Graphics.Green, 20, Position + Origin.Position);

        AddChild(roadLine);

        //foreach (Line2D line in arrow)
        {
            //AddChild(line);
        }
    }

    /*public void InsertIntersection(Intersection newIntersection)
    {
        int i;

        for (i = 0; i < Intersections.Count; i++)
        {
            float dist1 = newIntersection.DistanceTo(Intersections[i]);
            float dist2 = newIntersection.DistanceTo(Intersections[i + 1]);
            float dist3 = Intersections[i].DistanceTo(Intersections[i + 1]);

            //Inside Case, break;
            if (dist1 + dist2 <= dist3)
            {
                break;
            }
        }

        if (i < Intersections.Count - 1)
        {
            Intersections.Insert(i, newIntersection);
        }
        else
        {
            Intersections.Add(newIntersection);
        }

        newIntersection.AddRoad(this);

        Update();
        _DrawRoad();

    }*/

    public static Vector2 Intersection(Road r, Road s)
        => Line.Intersection(r.RoadLine, s.RoadLine);


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
