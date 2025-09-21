using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using OpenCvSharp;
using System;
using Microsoft.Xna.Framework;

public class MeshGenerator
{
    public static Mesh GenerateMesh(Texture2D SDFTexture, float threshold, bool simplify, float tolerance)
    {
        Color[] pixelData = new Color[SDFTexture.Width * SDFTexture.Height];
        SDFTexture.GetData(pixelData);

        Mesh mesh = new Mesh();
        mesh.Vertices = new List<Vertex>();

        // Use marching squares algorithm to generate contour
        var contourPoints = MarchingSquares(pixelData, SDFTexture.Width, SDFTexture.Height, threshold);
        
        // Convert to mesh vertices
        foreach (var point in contourPoints)
        {
            mesh.Vertices.Add(new Vertex 
            { 
                X = point.X / (float)SDFTexture.Width,  // Normalize to 0-1 range
                Y = point.Y / (float)SDFTexture.Height  // Normalize to 0-1 range
            });
        }

        if(simplify && mesh.Vertices.Count > 2)
        {
            mesh = MeshSimplifier.Simplify(mesh, tolerance);
        }

        return mesh;
    }

    private static List<Vector2> MarchingSquares(Color[] pixelData, int width, int height, float threshold)
    {
        var contourPoints = new List<Vector2>();
        
        // Process each 2x2 cell in the grid
        for (int y = 0; y < height - 1; y++)
        {
            for (int x = 0; x < width - 1; x++)
            {
                // Get the four corner values
                float tl = GetSDFValue(pixelData, x, y, width);         // Top-left
                float tr = GetSDFValue(pixelData, x + 1, y, width);     // Top-right
                float bl = GetSDFValue(pixelData, x, y + 1, width);     // Bottom-left
                float br = GetSDFValue(pixelData, x + 1, y + 1, width); // Bottom-right

                // Determine the marching squares case (0-15)
                int marchingCase = 0;
                if (tl >= threshold) marchingCase |= 1;
                if (tr >= threshold) marchingCase |= 2;
                if (br >= threshold) marchingCase |= 4;
                if (bl >= threshold) marchingCase |= 8;

                // Generate line segments based on the marching case
                var segments = GetMarchingSquareSegments(marchingCase, x, y, tl, tr, bl, br, threshold);
                contourPoints.AddRange(segments);
            }
        }

        return contourPoints;
    }

    private static float GetSDFValue(Color[] pixelData, int x, int y, int width)
    {
        int index = y * width + x;
        return pixelData[index].R / 255f;
    }

    private static List<Vector2> GetMarchingSquareSegments(int marchingCase, int x, int y, 
        float tl, float tr, float bl, float br, float threshold)
    {
        var segments = new List<Vector2>();
        
        // Calculate interpolated edge points
        Vector2 topEdge = new Vector2(x + Interpolate(tl, tr, threshold), y);
        Vector2 rightEdge = new Vector2(x + 1, y + Interpolate(tr, br, threshold));
        Vector2 bottomEdge = new Vector2(x + Interpolate(bl, br, threshold), y + 1);
        Vector2 leftEdge = new Vector2(x, y + Interpolate(tl, bl, threshold));

        // Handle each marching squares case
        switch (marchingCase)
        {
            case 0:  // No contour
            case 15: // Completely filled
                break;

            case 1:  // Bottom-left corner
                segments.Add(leftEdge);
                segments.Add(bottomEdge);
                break;

            case 2:  // Bottom-right corner
                segments.Add(bottomEdge);
                segments.Add(rightEdge);
                break;

            case 3:  // Bottom edge
                segments.Add(leftEdge);
                segments.Add(rightEdge);
                break;

            case 4:  // Top-right corner
                segments.Add(rightEdge);
                segments.Add(topEdge);
                break;

            case 5:  // Saddle point (ambiguous case)
                // Connect top-left to bottom-right
                segments.Add(leftEdge);
                segments.Add(topEdge);
                segments.Add(bottomEdge);
                segments.Add(rightEdge);
                break;

            case 6:  // Right edge
                segments.Add(bottomEdge);
                segments.Add(topEdge);
                break;

            case 7:  // Everything except top-left
                segments.Add(leftEdge);
                segments.Add(topEdge);
                break;

            case 8:  // Top-left corner
                segments.Add(topEdge);
                segments.Add(leftEdge);
                break;

            case 9:  // Left edge
                segments.Add(topEdge);
                segments.Add(bottomEdge);
                break;

            case 10: // Saddle point (ambiguous case)
                // Connect top-right to bottom-left
                segments.Add(topEdge);
                segments.Add(rightEdge);
                segments.Add(leftEdge);
                segments.Add(bottomEdge);
                break;

            case 11: // Everything except top-right
                segments.Add(topEdge);
                segments.Add(rightEdge);
                break;

            case 12: // Top edge
                segments.Add(rightEdge);
                segments.Add(leftEdge);
                break;

            case 13: // Everything except bottom-right
                segments.Add(rightEdge);
                segments.Add(bottomEdge);
                break;

            case 14: // Everything except bottom-left
                segments.Add(bottomEdge);
                segments.Add(leftEdge);
                break;
        }

        return segments;
    }

    private static float Interpolate(float value1, float value2, float threshold)
    {
        // Linear interpolation to find where the threshold crosses the edge
        if (System.Math.Abs(value1 - value2) < 0.001f)
            return 0.5f; // Avoid division by zero
        
        return (threshold - value1) / (value2 - value1);
    }
}