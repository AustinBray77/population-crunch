using Godot;
using System;
using System.Collections.Generic;
using CityExtras;

public partial class Main : Node
{
    public static Random Rand = new Random();

    public static Generator _generator;

    public static PackedScene _cityScene, _pathScene, _roadScene, _personScene, _junctionScene;

    private Camera2D _camera;

    public static List<City> cities;


    //Implement AStar
    public static Tuple<double, List<Road>> CalculateTime(Intersection start, Intersection end, HashSet<Intersection> intersectionsPassedThrough, double currentBestTime = Double.MaxValue)
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
        _LoadResources();
        _SpawnInitialCities();
        _GenerateRoads(3200f);
        GenerateTrip(cities[0]);
    }

    private void _LoadResources()
    {
        _cityScene = GD.Load("res://City.tscn") as PackedScene;
        _roadScene = GD.Load("res://Road.tscn") as PackedScene;
        _pathScene = GD.Load("res://Path2D.tscn") as PackedScene;
        _personScene = GD.Load("res://Person.tscn") as PackedScene;
        _junctionScene = GD.Load("res://Junction.tscn") as PackedScene;
        _camera = GetChild(0, false) as Camera2D;
        _generator = new Generator(879699, 15000);
    }

    private void _SpawnInitialCities()
    {
        cities = new List<City>();

        Vector2[] positionList = _generator.GenerateStructures(500, 0.05f);

        positionList = Functions.OriginSort(positionList);

        for (int i = 0; i < positionList.Length; i++)
        {
            cities.Add(_cityScene.Instantiate<City>());
            cities[i].Position = positionList[i];
            cities[i].MainReference = this;
            cities[i].Name = i.ToString();
            AddChild(cities[i]);
        }
    }

    private void _GenerateRoads(float distance)
    {
        for (int i = 0; i < cities.Count; i++)
        {
            for (int j = i + 1; j < cities.Count; j++)
            {

                if (Mathf.Abs(cities[j].Position.Magnitude() - cities[i].Position.Magnitude()) > distance)
                {
                    break;
                }

                if (cities[j].DistanceTo(cities[i]) > distance)
                {
                    continue;
                }

                GenerateTwoWayRoadBetweenIntersections(cities[i], cities[j]);
            }
        }
    }

    /*private void _GenerateRoads()
    {
        Tuple<Junction[], int> initialRoadGroup = _generator.GenerateInitialRoads(0.07f, 750);

        Junction[] initialRoads = initialRoadGroup.Item1;

        int directionSplit = initialRoadGroup.Item2;

        GD.Print($"Initial Road Count: {initialRoads.Length / 2}");

        foreach (Junction junction in initialRoads)
        {
            AddChild(junction);
        }

        Road[] roads = new Road[initialRoads.Length];

        for (int i = 0; i < initialRoads.Length; i += 2)
        {
            Tuple<Road, Road> result = GenerateTwoWayRoadBetweenIntersections(initialRoads[i], initialRoads[i + 1]);

            roads[i] = result.Item1;
            roads[i + 1] = result.Item2;
        }

        for (int i = 0; i < directionSplit; i += 2)
        {
            for (int j = directionSplit; j < initialRoads.Length; j += 2)
            {
                Vector2 newJunctPosition = Road.Intersection(roads[i], roads[j]);

                GD.Print($"Intersecting Roads -> {i} and {j}");

                if (newJunctPosition == Vector2.Inf)
                {
                    continue;
                }


                Junction newJunction = _junctionScene.Instantiate() as Junction;
                newJunction.Position = newJunctPosition;

                AddChild(newJunction);

                AddChild(roads[i].InsertIntersection(newJunction));
                AddChild(roads[i + 1].InsertIntersection(newJunction));

                AddChild(roads[j].InsertIntersection(newJunction));
                AddChild(roads[j + 1].InsertIntersection(newJunction));
            }
        }

        for (int i = 0; i < cities.Count; i++)
        {
            int closestRoad = 0;
            Vector2 connectionPoint = Vector2.Inf;
            float minDistance = Mathf.Inf;

            float x = cities[i].Position.X;
            float y = cities[i].Position.Y;

            for (int j = 0; j < roads.Length; j += 2)
            {
                if (roads[j].RoadLine.ValueAt(x) == y)
                {
                    closestRoad = j;
                    minDistance = 0;
                    break;
                }

                Vector2 curPoint;
                float curDistance = roads[j].RoadLine.DistanceTo(cities[i].Position, out curPoint);

                if (curDistance < minDistance)
                {
                    closestRoad = j;
                    connectionPoint = curPoint;
                    minDistance = curDistance;
                }
            }

            if (minDistance == 0)
            {
                AddChild(roads[closestRoad].InsertIntersection(cities[i]));
                AddChild(roads[closestRoad + 1].InsertIntersection(cities[i]));

                continue;
            }

            Junction newJunction = _junctionScene.Instantiate() as Junction;

            newJunction.Position = connectionPoint;
            AddChild(newJunction);

            AddChild(roads[closestRoad].InsertIntersection(newJunction));
            AddChild(roads[closestRoad + 1].InsertIntersection(newJunction));

            GenerateTwoWayRoadBetweenIntersections(cities[i], newJunction);
        }

    }*/

    public void GenerateTrip(City start)
    {
        List<City> excludingStart = new List<City>(cities);

        excludingStart.Remove(start);

        City end = //excludingStart[Rand.Next(0, excludingStart.Count)];
            cities[25];

        Tuple<double, List<Road>> roads = CalculateTime(start, end, new HashSet<Intersection>());
        Path2D path = _pathScene.Instantiate<Path2D>();

        path.Name = $"Path from {start.Name} to {end.Name}";

        AddChild(path);

        roads.Item2.Reverse();

        Curve2D pathCurve = new Curve2D();

        pathCurve.AddPoint(start.Position);

        foreach (Road road in roads.Item2)
        {
            GD.Print(road.Name);
            AddChild(Graphics.DrawCircle(road.Destination.Position, 50, Graphics.Red, 20));
            pathCurve.AddPoint(road.Destination.Position);
        }

        path.Curve = pathCurve;

        AddChild(Graphics.DrawLine(path.Curve.GetBakedPoints(), Graphics.Green, 20));

        Person person = _personScene.Instantiate<Person>();
        person.Position = path.Position;

        path.AddChild(person);

        person.StartJourney(start, end);
    }

    public Tuple<Road, Road> GenerateTwoWayRoadBetweenIntersections(Intersection a, Intersection b)
    {
        return new(GenerateRoadBetweenIntersections(a, b), GenerateRoadBetweenIntersections(b, a));
    }

    public Road GenerateRoadBetweenIntersections(Intersection origin, Intersection end, List<Vector2> additionalPositions = null)
    {
        if (origin == end)
        {
            GD.Print("Early return on road creation");
            return null;
        }

        Road road = _roadScene.Instantiate<Road>();
        Curve2D curve = new Curve2D();
        curve.AddPoint(origin.Position);

        if (additionalPositions != null)
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

        return road;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {

    }
}
