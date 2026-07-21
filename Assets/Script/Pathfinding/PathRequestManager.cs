using System.Collections.Generic;
using UnityEngine;

public class PathRequestManager : MonoBehaviour
{
    [SerializeField] private int maxRequestsPerFrame = 2;

    private readonly Queue<PathRequest> _queue = new();
    private PathfindingGrid _grid;

    public void SetGrid(PathfindingGrid grid) => _grid = grid;

    public void Request(PathRequest request) => _queue.Enqueue(request);

    private void Update()
    {
        if (_grid == null) return;

        int processed = 0;
        while (_queue.Count > 0 && processed < maxRequestsPerFrame)
        {
            PathRequest req = _queue.Dequeue();
            Path path = AStar.FindPath(_grid, req.Start, req.Target);
            req.Callback?.Invoke(path);
            processed++;
        }
    }
}