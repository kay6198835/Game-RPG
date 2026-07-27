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
        Vector2 gridPosition = positionWorld - originPosition; ;
        int x = Mathf.RoundToInt(gridPosition.x);
        int y = Mathf.RoundToInt(gridPosition.y);
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
