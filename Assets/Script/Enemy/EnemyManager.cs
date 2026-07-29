using UnityEngine;
[RequireComponent(typeof(PathRequestManager))]
public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    [SerializeField] private PathRequestManager _pathRequests;
    [SerializeField] public PathfindingGrid Grid{get;private set;}

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (_pathRequests == null)
            _pathRequests = GetComponent<PathRequestManager>();
    }

    public void SetPathfindingGrid(PathfindingGrid grid)
    {
        this.Grid = grid;
        _pathRequests.SetGrid(grid);
    }

    public void RequestPath(PathRequest request)
    {
        _pathRequests.Request(request);
    }

    public Node GetNodeByPositionWorld(Vector2 worldPosition)
    {
        return Grid.GetNode(worldPosition);
    }

    
}