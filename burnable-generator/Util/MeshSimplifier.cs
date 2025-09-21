using System;
using System.Collections.Generic;

public class MeshSimplifier
{
    public static Mesh Simplify(Mesh mesh, float tolerance)
    {
        if (mesh?.Vertices == null || mesh.Vertices.Count < 3)
            return mesh;

        var sortedVertices = Sort(mesh.Vertices);

        var simplifiedVertices = RamerDouglasPeucker(sortedVertices, tolerance);

        return new Mesh
        {
            Vertices = simplifiedVertices
        };
    }

    private static List<Vertex> Sort(List<Vertex> vertices)
    {
        // Simple sorting based on angle from centroid
        float centerX = 0;
        float centerY = 0;
        foreach (var v in vertices)
        {
            centerX += v.X;
            centerY += v.Y;
        }
        centerX /= vertices.Count;
        centerY /= vertices.Count;

        vertices.Sort((a, b) =>
        {
            float angleA = MathF.Atan2(a.Y - centerY, a.X - centerX);
            float angleB = MathF.Atan2(b.Y - centerY, b.X - centerX);
            return angleA.CompareTo(angleB);
        });

        return vertices;
    }

    private static List<Vertex> RamerDouglasPeucker(List<Vertex> points, float tolerance)
    {
        if (points == null || points.Count < 3)
            return points;

        // Find the start and end points
        var start = points[0];
        var end = points[points.Count - 1];

        // Find the point with the maximum distance from the line
        float maxDistance = 0;
        int index = 0;
        for (int i = 1; i < points.Count - 1; i++)
        {
            float distance = PerpendicularDistance(points[i], start, end);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                index = i;
            }
        }

        // If the maximum distance is greater than the tolerance, recursively simplify
        if (maxDistance > tolerance)
        {
            var left = RamerDouglasPeucker(points.GetRange(0, index + 1), tolerance);
            var right = RamerDouglasPeucker(points.GetRange(index, points.Count - index), tolerance);
            left.AddRange(right);
            return left;
        }
        else
        {
            // If not, return the start and end points
            return new List<Vertex> { start, end };
        }
    }

    private static float PerpendicularDistance(Vertex point, Vertex lineStart, Vertex lineEnd)
    {
        float dx = lineEnd.X - lineStart.X;
        float dy = lineEnd.Y - lineStart.Y;

        if (MathF.Abs(dx) < float.Epsilon && MathF.Abs(dy) < float.Epsilon)
        {
            return Distance(point, lineStart);
        }

        float numerator = MathF.Abs(dy * point.X - dx * point.Y + lineEnd.X * lineStart.Y - lineEnd.Y * lineStart.X);
        float denominator = MathF.Sqrt(dy * dy + dx * dx);

        return numerator / denominator;
    }

    private static float Distance(Vertex v1, Vertex v2)
    {
        float dx = v1.X - v2.X;
        float dy = v1.Y - v2.Y;
        return MathF.Sqrt(dx * dx + dy * dy);
    }
}