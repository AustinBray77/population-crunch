using CityExtras;
using Godot;

public class Junction : Intersection
{
    public override void _Ready()
    {
        AddChild(Graphics.DrawCircle(Vector2.Zero, 100, new Color(0.5f, 0.5f, 0.5f, 1), 20));
    }
}