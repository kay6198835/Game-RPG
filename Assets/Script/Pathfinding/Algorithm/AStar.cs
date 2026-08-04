using System.Collections.Generic;
using UnityEngine;

public static class AStar
{
    public static Path FindPath(PathfindingGrid grid, Vector3 startWorld, Vector3 targetWorld)
    {
        Node start  = grid.GetNodeFromWorld(startWorld);
        Node target = grid.GetNodeFromWorld(targetWorld);
        if (start == null || target == null || !start.Walkable || !target.Walkable)
            return Path.Failure;

        grid.ClearSearchState();
        PriorityQueue openSet = new PriorityQueue(grid.MaxSize);
        HashSet<Node> closedSet = new HashSet<Node>();

        openSet.Enqueue(start);
        while (openSet.Count > 0)
        {
            Node current = openSet.Dequeue();
            if (current == target) return Retrace(grid, start, target);
            closedSet.Add(current);

            foreach (Node neighbour in grid.GetNeighbours(current))
            {
                if (!neighbour.Walkable || closedSet.Contains(neighbour)) continue;

                bool diagonal = neighbour.GridPosition.x != current.GridPosition.x
                             && neighbour.GridPosition.y != current.GridPosition.y;
                int stepCost = diagonal ? Heuristic.DIAGONAL : Heuristic.STRAIGHT;
                int newG = current.GCost + stepCost;

                bool inOpen = openSet.Contains(neighbour);
                if (newG < neighbour.GCost || !inOpen)
                {
                    neighbour.GCost = newG;
                    neighbour.HCost = Heuristic.Octile(neighbour, target);
                    neighbour.Parent = current;
                    if (!inOpen) openSet.Enqueue(neighbour);
                    else openSet.UpdatePriority(neighbour);
                }
            }
        }
        return Path.Failure;
    }

    private static Path Retrace(PathfindingGrid grid, Node start, Node target)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Node current = target;
        while (current != start)
        {
            waypoints.Add(grid.GetWorldFromNode(current));
            current = current.Parent;
        }
        waypoints.Add(grid.GetWorldFromNode(start));
        waypoints.Reverse();
        return new Path(waypoints, true);
    }
}
