using Godot;
using System;
using System.Collections.Generic;
using CityExtras;

public partial class Main : Node
{
    public static Random Rand = new Random();

    public static PackedScene _cityScene, _pathScene, _roadScene, _personScene;

    private Camera2D _camera;

    public static List<City> cities;


    //Implement AStar
    public static Tuple<double, List<Road>> CalculateTime(Intersection start, Intersection end, HashSet<Intersection> intersectionsPassedThrough)
    {
        if (start == end)
        {
            return new Tuple<double, List<Road>>(0, new List<Road>());
        }

        if (intersectionsPassedThrough.Contains(start))
        {
            return new Tuple<double, List<Road>>(double.MaxValue, new List<Road>());
        }

        intersectionsPassedThrough.Add(start);

        List<Road> nextRoads = new List<Road>();

        foreach (Road road in start.Roads)
        {
            nextRoads = nextRoads.RoadInsertionSort(road, start, end);
        }

        HashSet<Intersection> IntersectionsUsed = new HashSet<Intersection>(intersectionsPassedThrough);
        Tuple<double, List<Road>> bestResult = new Tuple<double, List<Road>>(double.MaxValue, new List<Road>());

        for (int i = 0; i < nextRoads.Count; i++)
        {
            if (IntersectionsUsed.Contains(nextRoads[i].Destination))
            {
                continue;
            }

            Tuple<double, List<Road>> nextResult = CalculateTime(nextRoads[i].Destination, end, intersectionsPassedThrough);

            if (bestResult.Item1 > (nextResult.Item1 + nextRoads[i].TravelTime))
            {
                bestResult = new Tuple<double, List<Road>>(nextResult.Item1 + nextRoads[i].TravelTime, nextResult.Item2);
                bestResult.Item2.Add(nextRoads[i]);
            }

            IntersectionsUsed.Add(nextRoads[i].Destination);
        }

        return bestResult;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Print("Started...");
        _loadResources();
        _spawnInitialCities();

        for (int i = 0; i < cities.Count; i++)
        {
            int roadCount = Rand.Next(1, 4);

            for (int j = 0; j < roadCount; j++)
            {
                GenerateRoadBetweenCities(cities[i], cities[Rand.Next(0, cities.Count)]);
            }
        }
    }

    private void _loadResources()
    {
        _cityScene = GD.Load("res://City.tscn") as PackedScene;
        _roadScene = GD.Load("res://Road.tscn") as PackedScene;
        _pathScene = GD.Load("res://Path2D.tscn") as PackedScene;
        _personScene = GD.Load("res://Person.tscn") as PackedScene;
        _camera = GetChild(0, false) as Camera2D;
    }

    private void _spawnInitialCities()
    {
        cities = new List<City>();

        Vector2[] positionList = Generator.GenerateStructures(7000f, 10287932);

        for (int i = 0; i < positionList.Length; i++)
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

        Tuple<double, List<Road>> roads = CalculateTime(start, end, new HashSet<Intersection>());
        Path2D path = _pathScene.Instantiate<Path2D>();

        path.Name = $"Path from {start.Name} to {end.Name}";

        AddChild(path);

        roads.Item2.Reverse();

        foreach (Road road in roads.Item2)
        {
            path.Curve = CityExtras.Functions.AddCurves(path.Curve, road.Curve);
        }

        path.Position = start.Position;

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
        if (origin == end)
        {
            return;
        }

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
