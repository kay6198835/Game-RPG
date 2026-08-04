using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Vector2Int GridPosition { get; }
    public bool Walkable { get; }
    public Vector3 WorldPosition { get; set; }

    public List<Node> Neighbors { get; } = new List<Node>();

    public int GCost;
    public int HCost;
    public int FCost => GCost + HCost;
    public Node Parent;
    public int HeapIndex;

    public Node(Vector2Int gridPosition, bool walkable)
    {
        GridPosition = gridPosition;
        Walkable = walkable;
    }

    public void AddNeighbor(Node neighbor)
    {
        if (neighbor != null) Neighbors.Add(neighbor);
    }

    public void ClearSearchState()
    {
        GCost = 0;
        HCost = 0;
        Parent = null;
        HeapIndex = 0;
    }
}
