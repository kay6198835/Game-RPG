using System;
using UnityEngine;

public struct PathRequest
{
    public Vector3 Start;
    public Vector3 Target;
    public Action<Path> Callback;   // enemy nhận path qua đây

    public PathRequest(Vector3 start, Vector3 target, Action<Path> callback)
    {
        Start = start;
        Target = target;
        Callback = callback;
    }
}