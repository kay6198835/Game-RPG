using UnityEngine;

public static class Heuristic
{
    public const int STRAIGHT = 10;
    public const int DIAGONAL = 14;
    private const bool USE_TIE_BREAKING = true;

    public static int Octile(Node a, Node b)
    {
        int dx = Mathf.Abs(a.GridPosition.x - b.GridPosition.x);
        int dy = Mathf.Abs(a.GridPosition.y - b.GridPosition.y);
        int min = Mathf.Min(dx, dy);
        int max = Mathf.Max(dx, dy);
        int h = DIAGONAL * min + STRAIGHT * (max - min);
        if (USE_TIE_BREAKING) h = Mathf.RoundToInt(h * 1.001f);
        return h;
    }
}
