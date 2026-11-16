using System.Collections.Generic;
using UnityEngine;

public class NavPath
{
    public readonly List<Vector2> Waypoints;   // chuỗi world position, start → target
    public readonly bool Success;

    public NavPath(List<Vector2> waypoints, bool success)
    {
        Waypoints = waypoints;
        Success = success;
    }

    public static readonly NavPath Failure = new NavPath(new List<Vector2>(), false);
}