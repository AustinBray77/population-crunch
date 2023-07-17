using Godot;
using System;
using System.Collections.Generic;

public class Generator
{
    public static Vector2[] GenerateStructures(float chunkSize, int seed, float minimumDistance = 300f, float density = 0.021f)
    {
        List<Vector2> positions = new List<Vector2>();

        Random random = new Random(seed);

        for (float i = -chunkSize / 2; i < chunkSize / 2; i += minimumDistance)
        {
            for (float j = -chunkSize / 2; j < chunkSize / 2; j += minimumDistance)
            {
                if (random.NextDouble() < density)
                {
                    positions.Add(new Vector2(i, j));
                }
            }
        }

        return positions.ToArray();
    }

    public static Curve2D[] GenerateInitialRoads(float chunkSize, int seed, float density)
    {
        List<Curve2D> roads = new List<Curve2D>();



        return roads.ToArray();
    }
}