using CityExtras;
using Godot;
using System;
using System.Collections.Generic;

public partial class Person : PathFollow2D
{
    private City _homeCity;
    private City _destination;

    public void StartJourney(City start, City end)
    {
        _homeCity = start;
        _destination = end;

        GD.Print($"Journey Started -> {_homeCity.Name}, {_destination.Name}");

        Line2D line = Graphics.DrawCircle(Position, 20, _destination.Color, 5);

        AddChild(line);
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Print("Here Buddy Circle");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        //QueueRedraw();
        if (_homeCity == null || _destination == null)
        {
            return;
        }

        int speed = 40;

        Progress += speed; /// Functions.Distance(_homeCity, _destination));
    }
}
