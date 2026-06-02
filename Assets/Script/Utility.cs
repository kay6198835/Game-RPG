using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class Utility
{

    // Maps any Vector2 to the nearest cardinal direction (TOP / BOTTOM / LEFT / RIGHT).
    // Strategy: whichever axis has the larger magnitude is the dominant one;
    // the sign of that axis then picks between the two opposing directions.
    // Example: dir = (-3, 1) → |x|=3 > |y|=1 → horizontal dominant → x<0 → LEFT
    public static Vector2 ToCardinalDirection(Vector2Int dir)
    {
        return CheckDirection(dir);
    }

    public static Vector2 ToCardinalDirection(Vector3 dir)
    {
        return CheckDirection(dir);
    }

    public static Vector2 ToCardinalDirection(Vector2 dir)
    {
        return CheckDirection(dir);
    }

    private static Vector2 CheckDirection(Vector2 dir)
    {
        bool dominantAxisIsHorizontal = Mathf.Abs(dir.x) >= Mathf.Abs(dir.y);

        if (dominantAxisIsHorizontal)
            return dir.x >= 0 ? GameConstants.Direction.RIGHT : GameConstants.Direction.LEFT;

        return dir.y >= 0 ? GameConstants.Direction.TOP : GameConstants.Direction.BOTTOM;
    }



    #region Tilemap Door Utility

    /// <summary>
    /// Duyệt tất cả layer trong <paramref name="tilemaps"/>, chỉ xét roomTile và doorTile
    /// (bỏ qua floor và các tile khác để bounds phản ánh đúng vùng wall).
    /// Trả về bounds của toàn bộ wall và danh sách vị trí của mọi doorTile.
    /// Trả về false nếu không tìm thấy tile nào (map chưa load).
    /// </summary>
    public static bool CollectWallTileData(
        List<Tilemap> tilemaps, TileBase roomTile, TileBase doorTile,
        out int minX, out int maxX, out int minY, out int maxY,
        out List<(Vector3Int pos, int layer)> doorPositions)
    {
        minX = int.MaxValue; maxX = int.MinValue;
        minY = int.MaxValue; maxY = int.MinValue;
        doorPositions = new List<(Vector3Int, int)>();

        for (int layerIdx = 0; layerIdx < tilemaps.Count; layerIdx++)
        {
            Tilemap tm = tilemaps[layerIdx];
            foreach (Vector3Int pos in tm.cellBounds.allPositionsWithin)
            {
                TileBase tile = tm.GetTile(pos);

                // Chỉ quan tâm wall tile; floor và các tile khác bị bỏ qua.
                if (tile != roomTile && tile != doorTile) continue;

                // Mở rộng bounds để bao phủ toàn bộ vùng wall.
                if (pos.x < minX) minX = pos.x;
                if (pos.x > maxX) maxX = pos.x;
                if (pos.y < minY) minY = pos.y;
                if (pos.y > maxY) maxY = pos.y;

                if (tile == doorTile)
                    doorPositions.Add((pos, layerIdx));
            }
        }

        // minX vẫn là MaxValue → không có tile nào được tìm thấy.
        return minX != int.MaxValue;
    }

    /// <summary>
    /// Xác định hướng của một Tile_Door dựa trên khoảng cách từ nó đến 4 cạnh biên của wall.
    /// Cạnh nào gần nhất → door đó thuộc cạnh đó → trả về hướng tương ứng theo GameConstants.Direction.
    ///
    /// Ví dụ với room bounds x=[-15,14], y=[-15,14]:
    ///   pos(-15, 2) → distLeft=0  → LEFT
    ///   pos(-8, -14) → distDown=1 → BOTTOM
    ///   pos( 1,  14) → distUp=0   → TOP
    /// </summary>
    public static Vector2 GetDoorFacingDirection(Vector3Int pos, int minX, int maxX, int minY, int maxY)
    {
        int distLeft  = pos.x - minX;  // = 0 nếu đang ở cạnh trái
        int distRight = maxX - pos.x;  // = 0 nếu đang ở cạnh phải
        int distDown  = pos.y - minY;  // = 0 nếu đang ở cạnh dưới
        int distUp    = maxY - pos.y;  // = 0 nếu đang ở cạnh trên

        // Cạnh có khoảng cách nhỏ nhất là cạnh mà door thuộc về.
        int minDist = Mathf.Min(distLeft, Mathf.Min(distRight, Mathf.Min(distDown, distUp)));

        if (minDist == distLeft)  return GameConstants.Direction.LEFT;
        if (minDist == distRight) return GameConstants.Direction.RIGHT;
        if (minDist == distDown)  return GameConstants.Direction.BOTTOM;
        return GameConstants.Direction.TOP;
    }

    /// <summary>
    /// Trả về vị trí world (trung tâm) của nhóm doorTile nằm trên cạnh wall
    /// ứng với <paramref name="direction"/> truyền vào.
    /// Tính bằng cách lấy trung bình GetCellCenterWorld của tất cả door tile khớp hướng.
    /// Trả về Vector2.zero nếu không tìm thấy door nào theo hướng đó.
    /// </summary>
    public static Vector2 GetDoorWorldPosition(
        Vector2 direction, List<Tilemap> tilemaps, TileBase roomTile, TileBase doorTile)
    {
        if (!CollectWallTileData(tilemaps, roomTile, doorTile,
                out int minX, out int maxX, out int minY, out int maxY,
                out var doorPositions)) return Vector2.zero;

        // Cộng dồn world position của tất cả door tile thuộc hướng được chỉ định,
        // sau đó lấy trung bình → ra điểm giữa của cụm door đó.
        Vector3 sum = Vector3.zero;
        int count = 0;

        foreach (var (pos, layerIdx) in doorPositions)
        {
            if (GetDoorFacingDirection(pos, minX, maxX, minY, maxY) != direction) continue;

            // GetCellCenterWorld trả về tâm của tile trong world space (có tính transform của Tilemap).
            sum += tilemaps[layerIdx].GetCellCenterWorld(pos);
            count++;
        }

        if (count == 0) return Vector2.zero;
        return (Vector2)(sum / count);
    }

    /// <summary>
    /// Duyệt tất cả doorTile trong map. Với mỗi door, tính hướng của nó (TOP/BOTTOM/LEFT/RIGHT).
    /// Nếu hướng đó KHÔNG có trong mảng <paramref name="directions"/> → seal door:
    ///   - Thay chính tile door thành roomTile.
    ///   - Thay thêm 2 tile tiếp theo đi vào bên trong room (ngược chiều doorDir) thành roomTile.
    /// Mục đích: ẩn cửa không dùng và lấp kín khoảng hở mà door để lại trong wall.
    /// </summary>
    public static void SealUnusedDoors(
        Vector2[] directions, List<Tilemap> tilemaps, TileBase roomTile, TileBase doorTile)
    {
        if (!CollectWallTileData(tilemaps, roomTile, doorTile,
                out int minX, out int maxX, out int minY, out int maxY,
                out var doorPositions)) return;

        // Dùng HashSet để kiểm tra direction O(1) thay vì duyệt mảng mỗi lần.
        var directionSet = new HashSet<Vector2>(directions);

        foreach (var (pos, layerIdx) in doorPositions)
        {
            Vector2 doorDir = GetDoorFacingDirection(pos, minX, maxX, minY, maxY);

            // Nếu direction của door này nằm trong danh sách được phép → giữ nguyên.
            if (directionSet.Contains(doorDir)) continue;

            // inward = chiều ngược lại với doorDir → hướng đi vào bên trong room.
            // Ví dụ: doorDir = LEFT(-1,0) → inward = RIGHT(+1,0).
            Vector3Int inward = new Vector3Int(-(int)doorDir.x, -(int)doorDir.y, 0);

            // Seal: tile door + 2 tile kề theo hướng inward đều trở thành roomTile.
            // Lấp kín cả outer wall lẫn inner wall của đoạn corridor door.
            tilemaps[layerIdx].SetTile(pos,             roomTile);
            tilemaps[layerIdx].SetTile(pos + inward,     roomTile);
            tilemaps[layerIdx].SetTile(pos + inward * 2, roomTile);
        }
    }

    internal static Vector2 ToCardinalDirection(object value)
    {
        throw new NotImplementedException();
    }

    #endregion
}