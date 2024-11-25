using System.Collections.Generic;
using UnityEngine;

public struct FormationData
{
    public Vector2 Position { get; set; }
    public Vector2 Forward { get; set; }
    public List<Vector2> Points { get; set; }
    public float FormationWidth { get; set; }
}

public enum FormationType
{
    Line,
    Circle,
    Wedge,
    Loose
}

