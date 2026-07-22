[System.Serializable]
public class PathfindingGrid
{
    private Node[,] nodesGrid;
    private int cols, rows;
    private GridBuilder build;
    private Vector3 originPosition = new Vector3();
    public Node GetNode(int x, int y)
    {
        return nodesGrid[x, y];
    }

    public void BuildGrid(LevelData data)
    {
        if (build == null) build = new GridBuilder();
        nodesGrid = null;
        nodesGrid = build.BuildGrid(data, ref cols, ref rows, ref originPosition);
    }

    public List<Node> GetNeighbours(Node node)
    {
        node.Neighbors;
    }

    public SearchNode GetNodeFromWorld(Vector3 positionWorld)
    {
        Vector3 gridPosition = positionWorld - originPosition;
        SearchNode searchNode = new SearchNode(GetNode(gridPosition.x, gridPosition.y));
    }
}