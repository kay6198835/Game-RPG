using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridBuilder
{
    private static readonly Vector2Int[] Directions8 =
    {
        new Vector2Int( 0,  1), new Vector2Int( 1,  0),
        new Vector2Int( 0, -1), new Vector2Int(-1,  0),
        new Vector2Int( 1,  1), new Vector2Int( 1, -1),
        new Vector2Int(-1, -1), new Vector2Int(-1,  1),
    };

    public PathfindingGrid BuildGrid(LevelData data, Vector3Int cellOffset, Tilemap tilemap)
    {
        Vector2Int min = new Vector2Int(int.MaxValue, int.MaxValue);
        Vector2Int max = new Vector2Int(int.MinValue, int.MinValue);
        Dictionary<Vector2Int, bool> walkableAt = new Dictionary<Vector2Int, bool>();

        for (int i = 0; i < data.poses.Count; i++)
        {
            if (data.tiles[i] == null) continue;
            Vector3Int cell = data.poses[i] + cellOffset;
            Vector2Int pos = new Vector2Int(cell.x, cell.y);

            if (pos.x < min.x) min.x = pos.x;
            if (pos.y < min.y) min.y = pos.y;
            if (pos.x > max.x) max.x = pos.x;
            if (pos.y > max.y) max.y = pos.y;

            bool blocked = data.tiles[i] == GameConstants.TileName.ROOM
                        || data.tiles[i] == GameConstants.TileName.DOOR;
            bool walkableHere = !blocked;
            walkableAt[pos] = walkableAt.TryGetValue(pos, out bool cur) ? (cur && walkableHere) : walkableHere;
        }

        int cols = max.x - min.x + 1;
        int rows = max.y - min.y + 1;
        Node[,] nodes = new Node[cols, rows];

        for (int x = 0; x < cols; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                Vector3Int cell = new Vector3Int(min.x + x, min.y + y, 0);
                bool walkable = walkableAt.TryGetValue(new Vector2Int(cell.x, cell.y), out bool w) && w;
                Node node = new Node(new Vector2Int(x, y), walkable);
                node.WorldPosition = tilemap.GetCellCenterWorld(cell);
                nodes[x, y] = node;
            }
        }

        SetNodeNeighbors(nodes);
        return new PathfindingGrid(nodes, min, tilemap);
    }

    private void SetNodeNeighbors(Node[,] nodes)
    {
        int cols = nodes.GetLength(0);
        int rows = nodes.GetLength(1);

        for (int col = 0; col < cols; col++)
        {
            for (int row = 0; row < rows; row++)
            {
                foreach (Vector2Int dir in Directions8)
                {
                    int nx = col + dir.x;
                    int ny = row + dir.y;
                    if (!IsInside(nx, ny, cols, rows)) continue;

                    if (dir.x != 0 && dir.y != 0)
                    {
                        Node horizontal = nodes[col + dir.x, row];
                        Node vertical   = nodes[col, row + dir.y];
                        if (!horizontal.Walkable && !vertical.Walkable) continue;
                    }

                    nodes[col, row].AddNeighbor(nodes[nx, ny]);
                }
            }
        }
    }

    private bool IsInside(int col, int row, int cols, int rows)
        => col >= 0 && col < cols && row >= 0 && row < rows;
}
