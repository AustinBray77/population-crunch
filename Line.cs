using System;
using Godot;

namespace CityExtras
{
    public class Line
    {
        public const float Infinity = Mathf.Inf;

        public float Slope { get; private set; }
        public float Intercept { get; private set; }

        public Line(float slope, float intercept)
        {
            Slope = slope;
            Intercept = intercept;
        }

        public Line(Vector2 a, Vector2 b)
        {
            if (a == b)
            {
                return;
                //throw new Exception("Incorrect Declaration of a Line, Points must not be the same");
            }

            if (a.X == b.X)
            {
                Slope = Infinity;
            }
            else
            {
                Slope = (a.Y - b.Y) / (a.X - b.X);
            }

            if (Slope == Infinity)
            {
                Intercept = a.X;
            }
            else if (Slope == 0)
            {
                Intercept = a.Y - Slope * a.X;
            }
        }

        public void SetIntercept(float b)
        {
            Intercept = b;
        }


        public float ValueAt(float x)
            => Slope * x + Intercept;

        public bool PointExists(Vector2 point)
            => ValueAt(point.X) == point.Y;

        public float DistanceTo(Vector2 point, out Vector2 pointOnLine)
        {
            Line shortestLine = ShortestLine(point);
            pointOnLine = Intersection(this, shortestLine);
            return point.DistanceTo(pointOnLine);
        }

        public float DistanceTo(Vector2 point)
        {
            Line shortestLine = ShortestLine(point);
            Vector2 pointOnLine = Intersection(this, shortestLine);
            return point.DistanceTo(pointOnLine);
        }

        public Line ShortestLine(Vector2 point)
        {
            float newSlope;

            if (Slope == 0)
            {
                newSlope = Infinity;
            }
            else if (Slope == Infinity)
            {
                newSlope = 0;
            }
            else
            {
                newSlope = -1 / Slope;
            }

            Line shortestLine = new Line(newSlope, 0);

            float b;

            if (newSlope != Infinity)
            {
                b = point.Y - newSlope * point.X;
            }
            else
            {
                b = point.X;
            }

            shortestLine.SetIntercept(b);

            return shortestLine;
        }

        public static Vector2 Intersection(Line a, Line b)
        {
            if (a.Slope == b.Slope)
            {
                return Vector2.Inf;
            }

            float x, y;

            if (a.Slope == Infinity)
            {
                x = a.Intercept;
                y = b.ValueAt(x);
            }
            else if (b.Slope == Infinity)
            {
                x = b.Intercept;
                y = a.ValueAt(x);
            }
            else
            {
                x = (b.Intercept - a.Intercept) / (a.Slope - b.Slope);
                y = a.ValueAt(x);
            }

            return new Vector2(x, y);
        }
    }
}
