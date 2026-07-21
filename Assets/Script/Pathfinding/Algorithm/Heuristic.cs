using UnityEngine;

public static class Heuristic
{
    public const int STRAIGHT = 10;   // đi thẳng 1 ô
    public const int DIAGONAL = 14;   // đi chéo 1 ô ≈ 10 * √2

    // Bật để A* "quyết đoán" hơn, mở ít node hơn, đường thẳng hơn.
    // Đổi lại đường có thể dài hơn tối ưu ~0.1% (phá admissible rất nhẹ).
    private const bool USE_TIE_BREAKING = true;
    private const float TIE_BREAK_FACTOR = 1.001f;

    // Octile — CHUẨN cho lưới 8 hướng (cho đi chéo).
    public static int Octile(Node a, Node b)
    {
        int dx = Mathf.Abs(a.GridPosition.x - b.GridPosition.x);
        int dy = Mathf.Abs(a.GridPosition.y - b.GridPosition.y);

        int min = Mathf.Min(dx, dy);
        int max = Mathf.Max(dx, dy);

        // đi chéo min bước (mỗi bước 14), phần dư đi thẳng (max - min) bước (mỗi bước 10)
        int h = DIAGONAL * min + STRAIGHT * (max - min);

        return USE_TIE_BREAKING ? Mathf.RoundToInt(h * TIE_BREAK_FACTOR) : h;
    }

    // Manhattan — chỉ dùng nếu chuyển sang lưới 4 hướng (KHÔNG cho đi chéo).
    public static int Manhattan(Node a, Node b)
    {
        int dx = Mathf.Abs(a.GridPosition.x - b.GridPosition.x);
        int dy = Mathf.Abs(a.GridPosition.y - b.GridPosition.y);

        int h = STRAIGHT * (dx + dy);

        return USE_TIE_BREAKING ? Mathf.RoundToInt(h * TIE_BREAK_FACTOR) : h;
    }
}