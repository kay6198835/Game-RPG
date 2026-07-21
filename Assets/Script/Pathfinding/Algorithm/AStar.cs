using System.Collections.Generic;
using UnityEngine;

public static class AStar
{
    public static Path FindPath(PathfindingGrid grid, Vector3 startWorld, Vector3 targetWorld)
    {
        Node startNode  = grid.GetNodeFromWorld(startWorld);
        Node targetNode = grid.GetNodeFromWorld(targetWorld);

        if (startNode == null || targetNode == null
            || !startNode.Walkable || !targetNode.Walkable)
            return Path.Failure;

        grid.ClearSearchState();   // reset G/H/Parent/HeapIndex toàn lưới

        PriorityQueue openSet   = new PriorityQueue(grid.MaxSize);
        HashSet<Node> closedSet = new HashSet<Node>();

        startNode.GCost = 0;
        startNode.HCost = Heuristic.Octile(startNode, targetNode);
        openSet.Enqueue(startNode);

        while (openSet.Count > 0)
        {
            Node current = openSet.Dequeue();
            closedSet.Add(current);

            if (current == targetNode)
                return Retrace(startNode, targetNode);

            foreach (Node neighbour in grid.GetNeighbours(current))
            {
                if (!neighbour.Walkable || closedSet.Contains(neighbour))
                    continue;

                bool diagonal = neighbour.GridPosition.x != current.GridPosition.x
                             && neighbour.GridPosition.y != current.GridPosition.y;
                int stepCost = diagonal ? Heuristic.DIAGONAL : Heuristic.STRAIGHT;
                int newG = current.GCost + stepCost;

                bool inOpen = openSet.Contains(neighbour);
                if (!inOpen || newG < neighbour.GCost)
                {
                    neighbour.GCost  = newG;
                    neighbour.HCost  = Heuristic.Octile(neighbour, targetNode);
                    neighbour.Parent = current;

                    if (!inOpen) openSet.Enqueue(neighbour);
                    else         openSet.UpdatePriority(neighbour);
                }
            }
        }

        return Path.Failure;   // không tới được target
    }

    private static Path Retrace(Node start, Node target)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Node current = target;

        while (current != start)
        {
            waypoints.Add(current.WorldPosition);
            current = current.Parent;
        }
        waypoints.Add(start.WorldPosition);
        waypoints.Reverse();   // đang là target→start, đảo lại thành start→target

        return new Path(waypoints, true);
    }
}