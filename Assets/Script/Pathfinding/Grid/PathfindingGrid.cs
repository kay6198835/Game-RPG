using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathfindingGrid
{
    private readonly Node[,] nodes;
    private readonly Tilemap tilemap;

    public int Cols { get; }
    public int Rows { get; }
    public int MaxSize => Cols * Rows;
    public Vector2Int Origin { get; }

    public PathfindingGrid(Node[,] nodes, Vector2Int origin, Tilemap tilemap)
    {
        this.nodes = nodes;
        this.tilemap = tilemap;
        Cols = nodes.GetLength(0);
        Rows = nodes.GetLength(1);
        Origin = origin;
    }

    public bool IsInside(int x, int y) => x >= 0 && x < Cols && y >= 0 && y < Rows;

    public Node GetNode(int x, int y) => IsInside(x, y) ? nodes[x, y] : null;

    public Node GetNodeFromWorld(Vector3 worldPos)
    {
        Vector3Int cell = tilemap.WorldToCell(worldPos);
        return GetNode(cell.x - Origin.x, cell.y - Origin.y);
    }

    public Vector3 GetWorldFromNode(Node node) => node.WorldPosition;

    public List<Node> GetNeighbours(Node node) => node.Neighbors;

    public void ClearSearchState()
    {
        for (int x = 0; x < Cols; x++)
            for (int y = 0; y < Rows; y++)
                nodes[x, y].ClearSearchState();
    }
}
