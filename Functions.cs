using System;
using System.Collections.Generic;
using Godot;

namespace CityExtras
{
    public static class Functions
    {
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

        public static Vector2 LineIntersection(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
        {
            float x, y, mb, ma, ba, bb;

            if (a1.X - a2.X == 0)
            {
                if (b1.X - b2.X == 0)
                {
                    return Vector2.Inf;
                }

                x = a1.X;
                mb = (b1.Y - b2.Y) / (b1.X - b2.X);
                bb = b1.Y - mb * b1.X;

                y = mb * x + bb;

                return new Vector2(x, y);
            }

            if (b1.X - b2.X == 0)
            {
                x = b1.X;
                ma = (a1.Y - a2.Y) / (a1.X - a2.X);
                ba = a1.Y - ma * a1.X;

                y = ma * x + ba;

                return new Vector2(x, y);
            }

            if (a1.Y - a2.Y == 0)
            {

            }

            ma = (a1.Y - a2.Y) / (a1.X - a2.X);
            mb = (b1.Y - b2.Y) / (b1.X - b2.X);

            ba = a1.Y - ma * a1.X;
            bb = b1.Y - mb * b1.X;

            x = (bb - ba) / (ma - mb);
            y = ma * x + ba;

            return new Vector2(x, y);
        }


        public static float NextRangeFloat(this Random random, float min, float max)
            => (float)(random.NextDouble() * (max - min)) + min;

        public static Curve2D AddCurves(Curve2D a, Curve2D b)
        {
            if (a.GetBakedPoints().Length == 0)
            {
                a.AddPoint(Vector2.Zero);
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