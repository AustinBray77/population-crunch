using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace CityExtras
{
    public static class Functions
    {
        public static float Infinity = Mathf.Inf;

        public static void DeleteChild(this Node node, int index)
        {
            Node child = node.GetChild(index);
            node.RemoveChild(child);
            child.Dispose();
        }

        public static Vector2[] OriginSort(Vector2[] vectors)
        {
            if (vectors.Length == 1)
            {
                return vectors;
            }

            int bLen = vectors.Length % 2 == 0 ? vectors.Length / 2 : vectors.Length / 2 + 1;


            Vector2[] partitionA = OriginSort(vectors.Skip(0).Take(vectors.Length / 2).ToArray());
            Vector2[] partitionB = OriginSort(vectors.Skip(0).Take(bLen).ToArray());

            for (int i = 0, aIndex = 0, bIndex = 0; i < vectors.Length; i++)
            {
                if (aIndex >= partitionA.Length)
                {
                    vectors[i] = partitionB[bIndex++];
                    continue;
                }

                if (bIndex >= partitionB.Length)
                {
                    vectors[i] = partitionA[aIndex++];
                    continue;
                }

                if (partitionA[aIndex].DirectionTo(Vector2.Zero) < partitionB[bIndex].DirectionTo(Vector2.Zero))
                {
                    vectors[i] = partitionA[aIndex++];
                }
                else
                {
                    vectors[i] = partitionB[bIndex++];
                }
            }

            return vectors;
        }

        public static float Distance(City a, City b) =>
            a.Position.DistanceTo(b.Position);

        public static float MagnitudeSquared(this Vector2 v) =>
            (float)(Math.Pow(v.X, 2) + Math.Pow(v.Y, 2));

        public static float Magnitude(this Vector2 v) =>
            (float)Math.Sqrt(v.MagnitudeSquared());

        public static float DotProduct2D(Vector2 u, Vector2 v) =>
            (u.X * v.X + u.Y * v.Y);

        public static float AngleBetweenVectors(Vector2 u, Vector2 v) =>
            (float)Math.Acos(DotProduct2D(u, v) / (u.Magnitude() * v.Magnitude()));

        public static float NextRangeFloat(this Random random, float min, float max)
            => (float)(random.NextDouble() * (max - min)) + min;

        public static Curve2D AddCurves(Curve2D a, Curve2D b)
        {
            if (a.GetBakedPoints().Length == 0)
            {
                a.AddPoint(Vector2.Zero);
            }

            if (b.GetBakedPoints().Length == 0)
            {
                return a;
            }

            Vector2 translation = a.GetBakedPoints()[a.GetBakedPoints().Length - 1] - b.GetBakedPoints()[0];

            Curve2D result = new Curve2D();

            foreach (var point in a.GetBakedPoints())
            {
                result.AddPoint(point);
            }

            foreach (var point in b.GetBakedPoints())
            {
                result.AddPoint(point + translation);
            }

            return result;
        }

        public static float Projection2D(Vector2 u, Vector2 v)
        {
            Vector2 Projection = (DotProduct2D(u, v) / v.MagnitudeSquared()) * v;
            return Projection.Magnitude();
        }

        public static List<Road> RoadInsertionSort(this List<Road> list, Road item, Intersection origin, Intersection destination)
        {
            Vector2 originToDestinationVector = new Vector2(destination.Position.X - origin.Position.X, destination.Position.Y - origin.Position.Y);
            Vector2 originToInsertedDestination = new Vector2(item.Destination.Position.X - origin.Position.X, item.Destination.Position.Y - origin.Position.Y);

            double relativeSpeedOfInsertedRoad = Projection2D(originToInsertedDestination, originToDestinationVector) / item.TravelTime;

            int i;

            for (i = 0; i < list.Count; i++)
            {
                Vector2 originToIndexDestination = new Vector2(list[i].Destination.Position.X - origin.Position.X, list[i].Destination.Position.Y - origin.Position.Y);
                double relativeSpeedOfRoadAtIndex = Projection2D(originToIndexDestination, originToDestinationVector) / item.TravelTime;

                if (relativeSpeedOfRoadAtIndex < relativeSpeedOfInsertedRoad)
                {
                    break;
                }
            }

            if (i < list.Count)
            {
                list.Insert(i, item);
            }
            else
            {
                list.Add(item);
            }

            return list;
        }
    }
}
