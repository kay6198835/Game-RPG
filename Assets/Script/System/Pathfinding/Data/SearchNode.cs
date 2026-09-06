using UnityEngine;

public class SearchNode
{
    public Node Node { get; }
    public Vector2Int TileMapPosition => Node.TileMapPosition;

    public int GCost;
    public int HCost;
    public int FCost => GCost + HCost;

    public SearchNode Parent;
    public int HeapIndex;
    public SearchNode(Node node)
    {
        Node = node;
    }
}