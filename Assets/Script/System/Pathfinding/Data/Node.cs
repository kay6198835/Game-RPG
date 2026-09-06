using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Node
{
    [SerializeField] public Vector2Int TileMapPosition { get; private set; }
    [SerializeField] public Vector2 WorldPosition { get; private set; }
    [SerializeField] public bool Walkable { get; private set; } = false;
    public IReadOnlyList<Node> Neighbors => neighbors;
    [SerializeField] private List<Node> neighbors = new List<Node>();
    public Node(Vector2Int tileMapPosition, bool walkable, Vector2 worldPosition)
    {
        TileMapPosition = tileMapPosition;
        Walkable = walkable;
        WorldPosition = worldPosition;
    }
    public Node(Vector2Int tileMapPosition, bool walkable)
    {
        TileMapPosition = tileMapPosition;
        Walkable = walkable;
    }
    public Node(){}
    public void AddNeighbor(Node neighborNode)
    {
        neighbors.Add(neighborNode);
    }

    public void SetWalkable(bool walkable)
    {
        this.Walkable = walkable;
    }

    // public void SetPostionWold(Vector2 gridOrigin)
    // {
    //     WorldPosition = gridOrigin + (Vector2)TileMapPosition;
    // }

    public void SetGridPosition(Vector2Int worldPosition)
    {
        this.TileMapPosition = worldPosition;
    }
}