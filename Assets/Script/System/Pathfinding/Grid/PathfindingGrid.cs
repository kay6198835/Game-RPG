using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[System.Serializable]
public class PathfindingGrid
{
    private Node[,] nodesGrid;
    private SearchNode[,] searchGrid;
    private int cols, rows;
    private GridBuilder build;
    private Vector2 originPosition;
    private readonly List<SearchNode> neighbourBuffer = new List<SearchNode>(8);

    public int MaxSize => cols * rows;

    public void BuildGrid(LevelData data, Vector2 originPosition)
    {
        this.originPosition = originPosition;
        if (build == null) build = new GridBuilder();
        nodesGrid = build.BuildGrid(data, ref cols, ref rows, ref this.originPosition);
        searchGrid = new SearchNode[cols, rows];
        for (int x = 0; x < cols; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                searchGrid[x, y] = new SearchNode(nodesGrid[x, y]);
            }
        }
    }

    private Vector2Int GetGridPosition(Vector2 positionWorld)
    {
        // originPosition is the corner of cell (0, 0) and a cell spans [n, n + 1), so floor, not round:
        // RoundToInt (banker's rounding) put a point near a wall into the wall cell.
        Vector2 gridPosition = positionWorld - originPosition;
        int x = Mathf.FloorToInt(gridPosition.x);
        int y = Mathf.FloorToInt(gridPosition.y);
        if (x < 0 || x >= cols || y < 0 || y >= rows) return -Vector2Int.one;
        return new Vector2Int(x, y);
    }

    public Node GetNode(Vector2 positionWorld)
    {
        if (nodesGrid == null) return null;
        Vector2Int gridPosition = GetGridPosition(positionWorld);
        if (gridPosition == -Vector2Int.one) return null;
        return nodesGrid[gridPosition.x, gridPosition.y];
    }

    public SearchNode GetNodeFromWorld(Vector2 positionWorld)
    {
        if (searchGrid == null) return null;
        Vector2Int gridPosition = GetGridPosition(positionWorld);
        if (gridPosition == -Vector2Int.one) return null;
        return searchGrid[gridPosition.x, gridPosition.y];
    }

    public IReadOnlyList<SearchNode> GetNeighbours(SearchNode searchNode)
    {
        neighbourBuffer.Clear();
        foreach (Node neighbor in searchNode.Node.Neighbors)
        {
            neighbourBuffer.Add(searchGrid[neighbor.TileMapPosition.x, neighbor.TileMapPosition.y]);
        }
        return neighbourBuffer;
    }

    private const float LineSampleStep = 0.1f;

    /// <summary>
    /// True when every cell a straight segment crosses is walkable, with the same corner rule as A*
    /// (a diagonal cell change needs both side cells walkable). False outside the grid.
    /// </summary>
    public bool IsLineWalkable(Vector2 from, Vector2 to)
    {
        if (nodesGrid == null) return false;
        Vector2Int previous = GetGridPosition(from);
        if (!IsWalkableCell(previous)) return false;

        int steps = Mathf.Max(1, Mathf.CeilToInt(Vector2.Distance(from, to) / LineSampleStep));
        for (int i = 1; i <= steps; i++)
        {
            Vector2Int cell = GetGridPosition(Vector2.Lerp(from, to, (float)i / steps));
            if (cell == previous) continue;
            if (!IsWalkableCell(cell)) return false;
            if (cell.x != previous.x && cell.y != previous.y
                && (!IsWalkableCell(new Vector2Int(cell.x, previous.y))
                    || !IsWalkableCell(new Vector2Int(previous.x, cell.y))))
                return false;
            previous = cell;
        }
        return true;
    }

    private bool IsWalkableCell(Vector2Int cell)
    {
        return cell != -Vector2Int.one && nodesGrid[cell.x, cell.y].Walkable;
    }

    public void ClearSearchState()
    {
        if (searchGrid == null) return;
        for (int x = 0; x < cols; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                SearchNode sn = searchGrid[x, y];
                sn.GCost = 0;
                sn.HCost = 0;
                sn.Parent = null;
                sn.HeapIndex = 0;
            }
        }
    }
}
