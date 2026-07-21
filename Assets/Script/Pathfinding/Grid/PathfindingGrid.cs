[System.Serializable]
public class PathfindingGrid
{
    private Node[,] nodesGrid;
    private int cols, rows;
    public void GetNode(int x, int y)
    {
        nodesGrid[x, y];
    }

    public void BuildGrid(LevelData data)
    {
        GridBuilder build = new GridBuilder();
        nodesGrid = build.BuildGrid(data, ref cols, ref rows);
    }

    public List<Node> GetNeighbours(Node node)
    {   
        node.Neighbors;
    }
}