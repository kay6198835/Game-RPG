using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    [SerializeField] private PathRequestManager _pathRequests;   // wire Inspector, cùng GameObject

    private void Awake()
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
        => _pathRequests.SetGrid(grid);

    public void RequestPath(PathRequest request)
        => _pathRequests.Request(request);
}