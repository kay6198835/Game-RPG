using System.Collections.Generic;
using UnityEngine;

public static class AStar
{
    public static Path FindPath(PathfindingGrid grid, Vector2 startWorld, Vector2 targetWorld)
    {
        SearchNode startNode  = grid.GetNodeFromWorld(startWorld);
        SearchNode targetNode = grid.GetNodeFromWorld(targetWorld);

        if (startNode == null || targetNode == null
            || !startNode.Walkable || !targetNode.Walkable)
            return Path.Failure;

        grid.ClearSearchState();   // reset G/H/Parent/HeapIndex toàn lưới

        PriorityQueue openSet   = new PriorityQueue(grid.MaxSize);
        HashSet<SearchNode> closedSet = new HashSet<SearchNode>();

        startNode.GCost = 0;
        startNode.HCost = Heuristic.Octile(startNode, targetNode);
        openSet.Enqueue(startNode);

        while (openSet.Count > 0)
        {
            SearchNode current = openSet.Dequeue();
            closedSet.Add(current);

            if (current == targetNode)
                return Retrace(startNode, targetNode);

            foreach (SearchNode neighbour in grid.GetNeighbours(current))
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

    private static Path Retrace(SearchNode start, SearchNode target)
    {
        List<Vector2> waypoints = new List<Vector2>();
        SearchNode current = target;

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