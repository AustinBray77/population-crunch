using System;
using Godot;

namespace CityExtras
{
    public static class Graphics
    {
        public static readonly Color White = new Color(1, 1, 1, 1);
        public static readonly Color TarGray = new Color(0.1f, 0.1f, 0.1f, 1);
        public static readonly Color Green = new Color(0, 1, 0, 1);
        public static readonly Color Yellow = new Color(1, 1, 0, 1);
        public static readonly Color Red = new Color(1, 0, 0, 1);
        public static readonly Color Orange = new Color(1, 0.5f, 0, 1);

        public static Line2D DrawLine(Vector2[] points, Color color, int width, Vector2 offset = new Vector2())
        {
            Line2D line = new Line2D();

            line.DefaultColor = color;
            line.Width = 20;

            foreach (Vector2 point in points)
            {
                line.AddPoint(point + offset);
            }

            return line;
        }

        public static Line2D[] DrawArrow(Vector2 direction, int length, Color color, int width, Vector2 offset = new Vector2())
        {
            Line2D[] lines = new Line2D[2];

            if (direction.Magnitude() != 1)
            {
                direction /= direction.Magnitude();
            }

            //lines[0] = DrawLine(new Vector2[] { Vector2.Zero, direction * length }, color, width, offset);

            //float thetaOne = (tip - origin).Angle() + (float)Math.PI / 6;
            //float thetaTwo = (tip - origin).Angle() - (float)Math.PI / 6;

            Vector2 scaledBranch = direction * length / 4;

            Vector2 branchOne = scaledBranch.Rotated(5 * (float)Math.PI / 6);
            Vector2 branchTwo = scaledBranch.Rotated(-5 * (float)Math.PI / 6);

            lines[0] = DrawLine(new Vector2[] { Vector2.Zero, branchOne }, color, width, offset + direction * length);
            lines[1] = DrawLine(new Vector2[] { Vector2.Zero, branchTwo }, color, width, offset + direction * length);

            return lines;
        }

        public static Line2D DrawCircle(Vector2 center, float radius, Color color, int width)
        {
            int pointAccuracy = 100;
            Vector2[] points = new Vector2[pointAccuracy + 1];

            for (int i = 0; i < pointAccuracy; i++)
            {
                float angle = (2 * (float)Math.PI / pointAccuracy) * i;

                float x = radius * (float)Math.Cos(angle);
                float y = radius * (float)Math.Sin(angle);

                points[i] = new Vector2(x, y);
            }

            points[pointAccuracy - 1] = points[0];

            return DrawLine(points, color, width, center);
        }
    }
}