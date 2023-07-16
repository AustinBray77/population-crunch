using Godot;
using System;
using System.Collections.Generic;
using CityExtras;

public partial class Main : Node
{
    public static Random Rand = new Random();

    public PackedScene _cityScene, _pathScene, _roadScene, _personScene;

    private Camera2D _camera;

    public static List<City> cities;


    //Implement AStar
    public static Tuple<double, List<Road>> CalculateTime(City start, City end, HashSet<City> citiesPassedThrough)
    {
        if (start == end)
        {
            return new Tuple<double, List<Road>>(0, new List<Road>());
        }

        if (citiesPassedThrough.Contains(start))
        {
            return new Tuple<double, List<Road>>(double.MaxValue, new List<Road>());
        }

        citiesPassedThrough.Add(start);

        List<Road> nextRoads = new List<Road>();

        foreach (Road road in start.Roads)
        {
            nextRoads = nextRoads.RoadInsertionSort(road, start, end);
        }

        HashSet<City> citiesUsed = new HashSet<City>(citiesPassedThrough);
        Tuple<double, List<Road>> bestResult = new Tuple<double, List<Road>>(double.MaxValue, new List<Road>());

        for (int i = 0; i < nextRoads.Count; i++)
        {
            if (citiesUsed.Contains(nextRoads[i].Destination))
            {
                continue;
            }

            Tuple<double, List<Road>> nextResult = CalculateTime(nextRoads[i].Destination, end, citiesPassedThrough);

            if (bestResult.Item1 > (nextResult.Item1 + nextRoads[i].TravelTime))
            {
                bestResult = new Tuple<double, List<Road>>(nextResult.Item1 + nextRoads[i].TravelTime, nextResult.Item2);
                bestResult.Item2.Add(nextRoads[i]);
            }

            citiesUsed.Add(nextRoads[i].Destination);
        }

        return bestResult;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Print("Started...");
        _loadResources();
        _spawnInitialCities(3);

        GenerateTwoWayRoadBetweenCities(cities[0], cities[1]);
        GenerateTwoWayRoadBetweenCities(cities[0], cities[2]);
        GenerateRoadBetweenCities(cities[2], cities[1]);
    }

    private void _loadResources()
    {
        _cityScene = GD.Load("res://City.tscn") as PackedScene;
        _roadScene = GD.Load("res://Road.tscn") as PackedScene;
        _pathScene = GD.Load("res://Path2D.tscn") as PackedScene;
        _personScene = GD.Load("res://Person.tscn") as PackedScene;
        _camera = GetChild(0, false) as Camera2D;
    }

    private void _spawnInitialCities(int cityCount)
    {
        cities = new List<City>();

        Vector2[] positionList = new Vector2[] {
            new Vector2 (0, 1500),
            new Vector2 (3000, 0),
            new Vector2 (-3000, 0)
        };

        for (int i = 0; i < cityCount; i++)
        {
            cities.Add(_cityScene.Instantiate<City>());
            cities[i].Position = positionList[i];
            cities[i].MainReference = this;
            cities[i].Name = i.ToString();
            AddChild(cities[i]);
        }
    }

    public void GenerateTrip(City start)
    {
        List<City> excludingStart = new List<City>(cities);

        excludingStart.Remove(start);

        City end = excludingStart[Rand.Next(0, excludingStart.Count)];

        Tuple<double, List<Road>> roads = CalculateTime(start, end, new HashSet<City>());
        Path2D path = _pathScene.Instantiate<Path2D>();

        path.Name = $"Path from {start.Name} to {end.Name}";

        AddChild(path);

        foreach (Road road in roads.Item2)
        {
            path.Curve = CityExtras.Functions.AddCurves(path.Curve, road.Curve);
        }

        Person person = _personScene.Instantiate<Person>();

        path.AddChild(person);

        person.StartJourney(start, end);
    }

    public void GenerateTwoWayRoadBetweenCities(City a, City b)
    {
        GenerateRoadBetweenCities(a, b);
        GenerateRoadBetweenCities(b, a);
    }

    public void GenerateRoadBetweenCities(City origin, City end, List<Vector2> additionalPositions = null)
    {
        Road road = _roadScene.Instantiate<Road>();
        Curve2D curve = new Curve2D();
        curve.AddPoint(origin.Position);

        if (!(additionalPositions == null))
        {
            foreach (Vector2 position in additionalPositions)
            {
                //Refine for curves later
                curve.AddPoint(position);
            }
        }

        curve.AddPoint(end.Position);

        road.Curve = curve;

        AddChild(road);
        road.Initialize(origin, end, 40);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {

    }
}
