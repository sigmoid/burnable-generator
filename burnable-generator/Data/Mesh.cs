using System.Collections.Generic;

[System.Serializable]
public class Mesh
{
    public float Amount { get; set; }
    public List<Vertex> Vertices { get; set; }
}

[System.Serializable]
public class Vertex
{
    public float X { get; set; }
    public float Y { get; set; }
}