using System.Collections.Generic;
using UnityEngine;

public class Utility : MonoBehaviour
{
    public static Utility Instance;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }
    // Maps any Vector2 to the nearest cardinal direction (TOP / BOTTOM / LEFT / RIGHT).
    // Strategy: whichever axis has the larger magnitude is the dominant one;
    // the sign of that axis then picks between the two opposing directions.
    // Example: dir = (-3, 1) → |x|=3 > |y|=1 → horizontal dominant → x<0 → LEFT
    private static Vector2 ToCardinalDirection(Vector2 dir)
    {
        bool dominantAxisIsHorizontal = Mathf.Abs(dir.x) >= Mathf.Abs(dir.y);

        if (dominantAxisIsHorizontal)
            return dir.x >= 0 ? GameConstants.Direction.RIGHT : GameConstants.Direction.LEFT;

        return dir.y >= 0 ? GameConstants.Direction.TOP : GameConstants.Direction.BOTTOM;
    }
}