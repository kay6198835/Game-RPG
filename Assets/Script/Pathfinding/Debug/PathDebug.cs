using System.Collections.Generic;
using UnityEngine;

public class PathDebug : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private bool drawGrid = true;
    [SerializeField] private bool drawPath = true;

    private PathfindingGrid grid;
    private List<Vector3> waypoints;

    private void OnEnable()  => EventManager.Resgister(EventID.ON_GRID_BUILT, OnGridBuilt);
    private void OnDisable() => EventManager.UnResgister(EventID.ON_GRID_BUILT, OnGridBuilt);

    private void OnGridBuilt(object obj) => SetGrid((PathfindingGrid)obj);

    public void SetGrid(PathfindingGrid g) => grid = g;
    public void SetPath(Path path) => waypoints = path != null && path.Success ? path.Waypoints : null;

    private void OnDrawGizmos()
    {
        if (grid != null && drawGrid)
        {
            for (int x = 0; x < grid.Cols; x++)
                for (int y = 0; y < grid.Rows; y++)
                {
                    Node node = grid.GetNode(x, y);
                    if (node == null) continue;
                    Gizmos.color = node.Walkable
                        ? new Color(0.2f, 0.8f, 0.3f, 0.25f)
                        : new Color(0.9f, 0.2f, 0.2f, 0.35f);
                    Gizmos.DrawCube(node.WorldPosition, Vector3.one * 0.9f);
                }
        }

        if (waypoints != null && drawPath)
        {
            Gizmos.color = Color.cyan;
            for (int i = 0; i < waypoints.Count; i++)
            {
                Gizmos.DrawSphere(waypoints[i], 0.15f);
                if (i > 0) Gizmos.DrawLine(waypoints[i - 1], waypoints[i]);
            }
            if (waypoints.Count > 0)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(waypoints[0], 0.22f);
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(waypoints[waypoints.Count - 1], 0.22f);
            }
        }
    }
#endif
}
