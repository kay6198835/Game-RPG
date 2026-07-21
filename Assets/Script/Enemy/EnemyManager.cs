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

    // Cổng cho grid (nghe ON_LOAD_MAP ở đâu đó → gọi vào đây)
    public void SetPathfindingGrid(PathfindingGrid grid)
        => _pathRequests.SetGrid(grid);

    // Cổng cho enemy xin đường — đây là "global access" mà b muốn
    public void RequestPath(PathRequest request)
        => _pathRequests.Request(request);
}