using Godot;
using System;
using System.Collections.Generic;
using CityExtras;
using System.Reflection.Metadata.Ecma335;

public partial class Main : Node
{
    [Export]
    public Font font;

    public static Random Rand = new Random();

    public static Generator _generator;

    public static PackedScene _cityScene, _pathScene, _roadScene, _personScene, _junctionScene;

    public static Font _mainFont;

    private Camera2D _camera;

    public static List<City> cities;

    public static Main Instance;

    public Theme Theme;

    public Config GeneratorConfiguration;

    public Config GraphicsConfiguration;

    //Implement AStar


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Print("Started...");
        Instance = this;
        _LoadResources();
        _SpawnInitialCities();
        _GenerateRoads(GeneratorConfiguration.GetValueAs<float>("RoadMaxDistance"));
        GenerateTrip(cities[0]);
    }

    private void _LoadResources()
    {
        _LoadGeneratorConfigFile();
        _cityScene = GD.Load("res://City.tscn") as PackedScene;
        _roadScene = GD.Load("res://Road.tscn") as PackedScene;
        _pathScene = GD.Load("res://Path2D.tscn") as PackedScene;
        _personScene = GD.Load("res://Person.tscn") as PackedScene;
        _junctionScene = GD.Load("res://Junction.tscn") as PackedScene;
        _camera = GetChild(0, false) as Camera2D;
        _generator =
            new Generator(
                GeneratorConfiguration.GetValueAs<int>("Seed"),
                GeneratorConfiguration.GetValueAs<float>("ChunkSize")
            );
        _mainFont = GD.Load("res://main_font.ttf") as Font;
        Theme = new Theme()
        {
            DefaultFont = _mainFont,
            DefaultFontSize = 240
        };
    }

    private void _LoadGeneratorConfigFile()
    {
        GeneratorConfiguration = new Config(new List<string>()
            {
                "Seed", "ChunkSize", "CityMinDistance", "CityDensity",
                "RoadMaxDistance", "RoadMinSpeed", "RoadMaxSpeed"
            });

        GeneratorConfiguration.LoadValuesFromFile(ProjectSettings.GlobalizePath("res://generator.config"));
    }

    private void _SpawnInitialCities()
    {
        cities = new List<City>();

        Vector2[] positionList = _generator.GenerateStructures(
            GeneratorConfiguration.GetValueAs<float>("CityMinDistance"), GeneratorConfiguration.GetValueAs<float>("CityDensity"));

        positionList = Functions.OriginSort(positionList);

        for (int i = 0; i < positionList.Length; i++)
        {
            cities.Add(_cityScene.Instantiate<City>());
            cities[i].Position = positionList[i];
            cities[i].Name = i.ToString();
            cities[i].SetID(i);
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

        City end = excludingStart[Rand.Next(0, excludingStart.Count)];

        RoadPath roadPath = Pathfinder.CalculateTime(start, end);
        Path2D path = _pathScene.Instantiate<Path2D>();

        path.Name = $"Path from {start.Name} to {end.Name}";

        AddChild(path);

        Curve2D pathCurve = new Curve2D();

        pathCurve.AddPoint(start.Position);
        path.Position = Vector2.Zero;

        foreach (Road road in roadPath.GetRoads())
        {
            //AddChild(Graphics.DrawCircle(road.Destination.Position, 50, Graphics.Red, 20));

            Vector2 offset = Graphics.CalculateLaneOffset(road.Direction, new Vector2(100, 0));

            pathCurve.AddPoint(road.Destination.Position + offset);
        }

        path.Curve = pathCurve;

        //AddChild(Graphics.DrawLine(path.Curve.GetBakedPoints(), Graphics.Green, 20));

        Person person = _personScene.Instantiate<Person>();

        path.AddChild(person);
        person.Position = Vector2.Zero;

        person.StartJourney(start, end, roadPath.Clone());
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
        road.Initialize(origin, end, Rand.Next(GeneratorConfiguration.GetValueAs<int>("RoadMinSpeed"), GeneratorConfiguration.GetValueAs<int>("RoadMaxSpeed")));

        return road;
    }

    public int GetTotalCityCount() =>
        cities.Count;

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {

    }
}
