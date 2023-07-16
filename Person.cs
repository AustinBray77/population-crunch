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
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        AddChild(Graphics.DrawCircle(Position, 20, Graphics.Red, 5));
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (_homeCity == null || _destination == null)
        {
            return;
        }

        int speed = 40;

        Progress += (speed / Functions.Distance(_homeCity, _destination));
    }
}
