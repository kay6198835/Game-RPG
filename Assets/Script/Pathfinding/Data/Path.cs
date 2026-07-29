using System.Collections.Generic;
using UnityEngine;

public class Path
{
    public readonly List<Vector2> Waypoints;   // chuỗi world position, start → target
    public readonly bool Success;

    public Path(List<Vector2> waypoints, bool success)
    {
        Waypoints = waypoints;
        Success = success;
    }

    public void SetPath(List<Vector2> waypoints, bool success)
    {
        Waypoints.Clear();
        Waypoints.AddRange(waypoints);
        Success = success;
    }

    public static readonly Path Failure = new Path(new List<Vector2>(), false);
}