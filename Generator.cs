using Godot;
using System;
using System.Collections.Generic;

public class Generator : Configureable
{
    private int _seed { get; set; }
    private float _chunkSize { get; set; }

    public Generator(string configPath)
    {
        Configure(configPath, new()
            {
                "Seed", "ChunkSize", "CityMinDistance", "CityDensity",
                "RoadMaxDistance", "RoadMinSpeed", "RoadMaxSpeed"
            });
        _seed = Configuration.GetValueAs<int>("Seed");
        _chunkSize = Configuration.GetValueAs<float>("ChunkSize");
    }

    public Vector2[] GenerateStructures(float minimumDistance, float density)
    {
        List<Vector2> positions = new List<Vector2>();

        Random random = new Random(_seed);

        for (float i = -_chunkSize / 2; i < _chunkSize / 2; i += minimumDistance)
        {
            for (float j = -_chunkSize / 2; j < _chunkSize / 2; j += minimumDistance)
            {
                if (random.NextDouble() < density)
                {
                    positions.Add(new Vector2(i, j));
                }
            }
        }

        return positions.ToArray();
    }

    public Tuple<Junction[], int> GenerateInitialRoads(float density, float minimumDistance = 100f)
    {
        List<Junction> roads = new List<Junction>();

        Random random = new Random(_seed);

        //NS Roads
        for (float i = -_chunkSize / 2; i < _chunkSize / 2; i += minimumDistance)
        {
            if (random.NextDouble() < density)
            {
                Vector2 northPosition = new Vector2(i, _chunkSize / 2);
                Vector2 southPosition = new Vector2(i, -_chunkSize / 2);

                Junction northJunction = Main._junctionScene.Instantiate() as Junction;
                Junction southJunction = Main._junctionScene.Instantiate() as Junction;

                northJunction.Position = northPosition;
                southJunction.Position = southPosition;

                roads.Add(northJunction);
                roads.Add(southJunction);
            }
        }

        int EWStart = roads.Count;

        //EW Roads
        for (float i = -_chunkSize / 2; i < _chunkSize / 2; i += minimumDistance)
        {
            if (random.NextDouble() < density)
            {
                Vector2 eastPosition = new Vector2(_chunkSize / 2, i);
                Vector2 westPosition = new Vector2(-_chunkSize / 2, i);

                Junction eastJunction = Main._junctionScene.Instantiate() as Junction;
                Junction westJunction = Main._junctionScene.Instantiate() as Junction;

                eastJunction.Position = eastPosition;
                westJunction.Position = westPosition;

                roads.Add(eastJunction);
                roads.Add(westJunction);
            }
        }

        return new(roads.ToArray(), EWStart);
    }
}