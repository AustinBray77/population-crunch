using System.Collections.Generic;
using CityExtras;
using Godot;

public partial class Junction : Intersection
{
    public override void _Ready()
    {
        Name = "Junction";
        Roads = new List<Road>();
        AddChild(Graphics.DrawCircle(Vector2.Zero, 50, new Color(0.5f, 0.5f, 0.5f, 1), 15));
    }
}