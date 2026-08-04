using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    [SerializeField] private PathRequestManager pathRequestManager;
    public PathfindingGrid Grid { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void OnEnable()  => EventManager.Resgister(EventID.ON_GRID_BUILT, OnGridBuilt);
    private void OnDisable() => EventManager.UnResgister(EventID.ON_GRID_BUILT, OnGridBuilt);

    private void OnGridBuilt(object obj)
    {
        Grid = (PathfindingGrid)obj;
        if (pathRequestManager != null) pathRequestManager.SetGrid(Grid);
    }

    public void RequestPath(PathRequest request)
    {
        if (pathRequestManager != null) pathRequestManager.RequestPath(request);
    }
}
