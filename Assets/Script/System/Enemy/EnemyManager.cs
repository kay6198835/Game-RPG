using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(PathRequestManager))]
public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    [SerializeField] private PathRequestManager _pathRequests;
    // Not [RequireComponent]: the CombatTest and QA scene builders add an EnemyManager on its own, and an
    // auto-added, un-injected EnemySpawner there would release their enemies on death.
    [SerializeField] private EnemySpawner _spawner;
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
        if (_spawner == null)
            _spawner = GetComponent<EnemySpawner>();
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

    /// <summary>True when a straight walk between the two points stays on walkable cells (A* corner rule).</summary>
    public bool IsLineWalkable(Vector2 from, Vector2 to)
    {
        return Grid != null && Grid.IsLineWalkable(from, to);
    }

    public void SpawnRoom(List<Vector2Int> spawnPositions)
    {
        if (!HasSpawner()) return;
        _spawner.SpawnRoom(spawnPositions);
    }

    public void SpawnBoss(List<Vector2Int> spawnPositions)
    {
        if (!HasSpawner()) return;
        _spawner.SpawnBoss(spawnPositions);
    }

    private bool HasSpawner()
    {
        if (_spawner != null) return true;
        Debug.LogError($"[{nameof(EnemyManager)}] No EnemySpawner on {name}; nothing can be spawned.");
        return false;
    }
}
