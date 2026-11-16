using System;
using UnityEngine;

public struct PathRequest
{
    public Vector2 Start;
    public Vector2 Target;
    public Action<NavPath> Callback;   // enemy nhận path qua đây

    public PathRequest(Vector2 start, Vector2 target, Action<NavPath> callback)
    {
        Start = start;
        Target = target;
        Callback = callback;
    }
}