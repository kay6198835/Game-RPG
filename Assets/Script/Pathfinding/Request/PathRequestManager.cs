using System.Collections.Generic;
using UnityEngine;

public class PathRequestManager : MonoBehaviour
{
    [SerializeField] private int maxRequestsPerFrame = 5;
    private readonly Queue<PathRequest> queue = new Queue<PathRequest>();
    private PathfindingGrid grid;

    public void SetGrid(PathfindingGrid g) => grid = g;

    public void RequestPath(PathRequest request) => queue.Enqueue(request);

    private void Update()
    {
        if (grid == null) return;
        int processed = 0;
        while (queue.Count > 0 && processed < maxRequestsPerFrame)
        {
            PathRequest req = queue.Dequeue();
            req.Callback?.Invoke(AStar.FindPath(grid, req.Start, req.Target));
            processed++;
        }
    }
}
