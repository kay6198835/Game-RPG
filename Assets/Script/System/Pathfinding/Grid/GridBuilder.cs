using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GridBuilder
{
    private Node[,] nodesGrid;
    private int cols, rows;
    private Vector2 originPosition;
    // Build base on tile map of room
    public Node[,] BuildGrid(LevelData data, ref int cols, ref int rows,ref Vector2 originPosition)
    {
        Vector2Int min = new Vector2Int(int.MaxValue, int.MaxValue);
        Vector2Int max = new Vector2Int(int.MinValue, int.MinValue);
        Dictionary<Vector2Int, bool> walkableAt = new();

        for (int i = 0; i < data.poses.Count; i++)
        {
            Vector3Int p = data.poses[i];
            if (p.x < min.x) min.x = p.x;
            if (p.y < min.y) min.y = p.y;
            if (p.x > max.x) max.x = p.x;
            if (p.y > max.y) max.y = p.y;

            Vector2Int pos = new Vector2Int(p.x, p.y);
            bool isFloor = data.tiles[i] == GameConstants.TileName.FLOOR
                        || data.tiles[i] == GameConstants.TileName.SPAWN;

            walkableAt[pos] = walkableAt.TryGetValue(pos, out bool cur)
                ? (cur && isFloor)
                : isFloor;
        }

        cols = max.x - min.x + 1;
        rows = max.y - min.y + 1;
        Node[,] nodesGrid = new Node[cols, rows];

        for (int x = 0; x < cols; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                Vector2Int rawPos = new Vector2Int(min.x + x, min.y + y);
                bool walkable = walkableAt.TryGetValue(rawPos, out bool isFloor) && isFloor;
                Vector2Int gridPosition = new Vector2Int(x, y);
                nodesGrid[x, y] = new Node(gridPosition, walkable, originPosition + (Vector2)rawPos);
            }
        }
        this.cols = cols;
        this.rows = rows;
        this.nodesGrid = nodesGrid;
        this.originPosition = originPosition + min;
        originPosition = this.originPosition;
        SetNodeNeighbors();
        return nodesGrid;
    }



    private void SetNodeNeighbors()
    {
        for (int col = 0; col < cols; col++)
        {
            for (int row = 0; row < rows; row++)
            {
                foreach (Vector2 dir in GameConstants.Direction.Vector.ALL)
                {
                    // Diagonal entries in ALL are normalized (~0.707); truncating with (int)
                    // collapses them to 0, so round to keep the ±1 diagonal step.
                    int dirX = Mathf.RoundToInt(dir.x);
                    int dirY = Mathf.RoundToInt(dir.y);
                    int nx = col + dirX;
                    int ny = row + dirY;

                    if (!IsInside(nx, ny))
                        continue;
                    Node neighbor = nodesGrid[nx, ny];
                    // Corner Cutting
                    if (dirX != 0 && dirY != 0)
                    {
                        Node horizontal = nodesGrid[col + dirX, row];
                        Node vertical = nodesGrid[col, row + dirY];

                        if (!horizontal.Walkable && !vertical.Walkable)
                            continue;
                    }

                    nodesGrid[col, row].AddNeighbor(neighbor);
                }
            }
        }
    }

    private bool IsInside(int col, int row)
    {
        return col >= 0 &&
               col < cols &&
               row >= 0 &&
               row < rows;
    }
}