using CityExtras;
using Godot;
using System;
using System.Collections.Generic;

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

        _SetOffset();


        GD.Print($"Journey Started -> {_homeCity.Name}, {_destination.Name}");

        Line2D line = Graphics.DrawCircle(Position, 20, _destination.Color, 5);

        AddChild(line);
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        //GD.Print("Here Buddy Circle");
    }

    private void _IncrementRoad()
    {
        _curRoad = _path.GetRoads()[++_curRoadIndex];
        _distanceAlongRoad = 0f;
        //_SetOffset();
    }

    private void _SetOffset()
    {
        Position = _curRoad.Position + new Vector2(20, 0);
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
            _IncrementRoad();
            //GD.Print($"Switching Roads, Current Speed is now: {_curRoad.Speed}");
        }


        int speed = _curRoad.Speed;

        float preProgress = ProgressRatio;

        Progress += speed;
        _distanceAlongRoad += speed;

        if (preProgress > ProgressRatio)
        {
            Dispose();
        }
    }
}
