using UnityEngine;

public static class DirectionResolver
{
    public static void Calculate(Vector2 directionVector, ref float angle, ref int direction)
    {
        angle = Mathf.Atan2(directionVector.x, directionVector.y) * Mathf.Rad2Deg;
        angle += 180;
        direction = ToDirectionIndex(angle);
    }

    public static int ToDirectionIndex(float angle)
    {
        if (angle > 22 && angle <= 67) return 0;
        if (angle > 67 && angle <= 112) return 1;
        if (angle > 112 && angle <= 157) return 2;
        if (angle > 157 && angle <= 202) return 3;
        if (angle > 202 && angle <= 247) return 4;
        if (angle > 247 && angle <= 292) return 5;
        if (angle > 292 && angle <= 337) return 6;
        return 7;
    }
}
