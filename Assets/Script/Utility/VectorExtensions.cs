using UnityEngine;

public static class VectorExtensions
{
    public static Vector2 Position2D(this Transform t) => t.position;

    public static Vector3 WithZ(this Vector2 v, float z) => new Vector3(v.x, v.y, z);
}
