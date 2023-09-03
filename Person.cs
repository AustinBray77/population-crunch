using CityExtras;
using Godot;
using System;
using System.Collections.Generic;
using System.IO;

public partial class Person : PathFollow2D
{
    private City _homeCity;
    private City _destination;
    private RoadPath _path;
    private Road _curRoad;
    private int _curRoadIndex;
    private float _distanceAlongRoad;

    public void StartJourney(City start, City end, RoadPath path)
    {
        if (path.GetRoads().Count == 0 || path.Time >= RoadPath.MaxTime)
        {
            Dispose();
            return;
        }

        _homeCity = start;
        _destination = end;
        _path = path;

        _curRoad = path.GetRoad(0);
        _curRoadIndex = 0;
        _distanceAlongRoad = 0f;


        GD.Print($"Journey Started -> {_homeCity.Name}, {_destination.Name}");

        Line2D line = Graphics.DrawCircle(Position, Graphics.s_Configuration.GetValueAs<int>("PersonRadius"), _destination.Color, 5);

        AddChild(line);
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        //GD.Print("Here Buddy Circle");
    }

    private bool _IncrementRoad()
    {
        if (_curRoadIndex == _path.GetRoads().Count - 1)
        {
            Dispose();
            return false;
        }

        _curRoad = _path.GetRoads()[++_curRoadIndex];
        _distanceAlongRoad = 0f;
        return true;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        //QueueRedraw();
        if (_homeCity == null || _destination == null)
        {
            return;
        }

        if (_distanceAlongRoad > _curRoad.Length)
        {
            bool wasSuccessful = _IncrementRoad();

            if (!wasSuccessful)
            {
                return;
            }
            //GD.Print($"Switching Roads, Current Speed is now: {_curRoad.Speed}");
        }


        int speed = _curRoad.Speed;

        Progress += speed;
        _distanceAlongRoad += speed;
    }
}
