using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

// Mirrors QA-PATH-01..03 (QASystemsSuite). Small hand-drawn grids give exact expected paths; the last three
// tests run the Play Mode checks on the committed room CombatRoom_GrandArena.json.
public class AStarTests
{
    private const string WallTile = "Tile_Room";
    private const string PathfindingRoom = "CombatRoom_GrandArena.json";

    // Rows are written top to bottom: the last string is y = 0. '.' = floor, '#' = wall.
    private static PathfindingGrid MakeGrid(params string[] rows)
    {
        var data = new LevelData();
        for (int r = 0; r < rows.Length; r++)
        {
            int y = rows.Length - 1 - r;
            for (int x = 0; x < rows[r].Length; x++)
            {
                data.tiles.Add(rows[r][x] == '.' ? GameConstants.TileName.FLOOR : WallTile);
                data.poses.Add(new Vector3Int(x, y, 0));
                data.layerIndices.Add(0);
            }
        }
        var grid = new PathfindingGrid();
        grid.BuildGrid(data, Vector2.zero);
        return grid;
    }

    private static Vector2 P(int x, int y) => new Vector2(x, y);

    [Test]
    public void FindPath_StraightCorridor_ReturnsEveryTileInOrder()
    {
        PathfindingGrid grid = MakeGrid("...");

        NavPath path = AStar.FindPath(grid, P(0, 0), P(2, 0));

        Assert.IsTrue(path.Success);
        CollectionAssert.AreEqual(new[] { P(0, 0), P(1, 0), P(2, 0) }, path.Waypoints);
    }

    [Test]
    public void FindPath_OpenDiagonal_StepsDiagonally()
    {
        PathfindingGrid grid = MakeGrid(
            "...",
            "...",
            "...");

        NavPath path = AStar.FindPath(grid, P(0, 0), P(2, 2));

        Assert.IsTrue(path.Success);
        CollectionAssert.AreEqual(new[] { P(0, 0), P(1, 1), P(2, 2) }, path.Waypoints);
    }

    [Test]
    public void FindPath_AroundAWall_TakesTheOnlyShortestDetour()
    {
        PathfindingGrid grid = MakeGrid(
            "...",
            ".#.",
            ".#.");

        NavPath path = AStar.FindPath(grid, P(0, 0), P(2, 0));

        Assert.IsTrue(path.Success);
        CollectionAssert.AreEqual(new[] { P(0, 0), P(0, 1), P(1, 2), P(2, 1), P(2, 0) }, path.Waypoints);
    }

    [Test]
    public void FindPath_StartEqualsTarget_ReturnsASingleWaypoint()
    {
        PathfindingGrid grid = MakeGrid("...");

        NavPath path = AStar.FindPath(grid, P(1, 0), P(1, 0));

        Assert.IsTrue(path.Success);
        CollectionAssert.AreEqual(new[] { P(1, 0) }, path.Waypoints);
    }

    // QA-PATH-02
    [Test]
    public void FindPath_ToAWallTile_FailsCleanly()
    {
        PathfindingGrid grid = MakeGrid("..#");

        NavPath path = null;
        Assert.DoesNotThrow(() => path = AStar.FindPath(grid, P(0, 0), P(2, 0)));

        Assert.IsFalse(path.Success);
        Assert.AreEqual(0, path.Waypoints.Count);
    }

    [Test]
    public void FindPath_FromAWallTile_Fails()
    {
        PathfindingGrid grid = MakeGrid("#..");

        Assert.IsFalse(AStar.FindPath(grid, P(0, 0), P(2, 0)).Success);
    }

    [Test]
    public void FindPath_TargetOutsideTheGrid_Fails()
    {
        PathfindingGrid grid = MakeGrid("...");

        Assert.IsFalse(AStar.FindPath(grid, P(0, 0), P(50, 50)).Success);
    }

    [Test]
    public void FindPath_TargetWalledOff_Fails()
    {
        PathfindingGrid grid = MakeGrid(".#.");

        Assert.IsFalse(AStar.FindPath(grid, P(0, 0), P(2, 0)).Success);
    }

    [Test]
    public void FindPath_DiagonalBetweenTwoWalls_IsNotAllowed()
    {
        PathfindingGrid grid = MakeGrid(
            "#.",
            ".#");

        Assert.IsFalse(AStar.FindPath(grid, P(0, 0), P(1, 1)).Success);
    }

    // QA-PATH-03
    [Test]
    public void FindPath_SameQueryAfterAnotherSearch_GivesTheSamePath()
    {
        PathfindingGrid grid = MakeGrid(
            ".....",
            ".##..",
            ".....",
            "..##.",
            ".....");

        NavPath first = AStar.FindPath(grid, P(0, 0), P(4, 4));
        AStar.FindPath(grid, P(4, 0), P(0, 4));
        NavPath again = AStar.FindPath(grid, P(0, 0), P(4, 4));

        Assert.IsTrue(first.Success);
        Assert.IsTrue(again.Success);
        CollectionAssert.AreEqual(first.Waypoints, again.Waypoints);
    }

    [Test]
    public void FindPath_OnALargerGrid_WaypointsAreWalkableAndAdjacent()
    {
        PathfindingGrid grid = MakeGrid(
            ".....",
            ".##..",
            ".....",
            "..##.",
            ".....");

        NavPath path = AStar.FindPath(grid, P(0, 0), P(4, 4));

        Assert.IsTrue(path.Success);
        Assert.AreEqual(P(0, 0), path.Waypoints[0]);
        Assert.AreEqual(P(4, 4), path.Waypoints[path.Waypoints.Count - 1]);
        AssertWalkableAndAdjacent(grid, path);
    }

    private static void AssertWalkableAndAdjacent(PathfindingGrid grid, NavPath path)
    {
        for (int i = 0; i < path.Waypoints.Count; i++)
        {
            Node node = grid.GetNode(path.Waypoints[i]);
            Assert.IsTrue(node != null && node.Walkable, $"waypoint {i} {path.Waypoints[i]} is not walkable");
            if (i > 0)
                Assert.LessOrEqual(Vector2.Distance(path.Waypoints[i - 1], path.Waypoints[i]), 1.5f, $"jump between waypoints {i - 1} and {i}");
        }
    }

    // ----- Committed room -----
    // Same checks as the Play Mode cases, with one difference: the start tile is taken from the LARGEST
    // connected floor region. The Play Mode case starts at the first walkable tile of the scan, which in
    // CombatRoom_GrandArena is a 2-tile pocket beside a door, so its path is only one step long.

    private static PathfindingGrid LoadRoomGrid(out List<Node> region, out Node wall)
    {
        string file = System.IO.Path.Combine(Application.dataPath, "Data", "Json", "Room", PathfindingRoom);
        Assert.IsTrue(File.Exists(file), $"room file missing: {file}");
        LevelData data = JsonUtility.FromJson<LevelData>(File.ReadAllText(file));
        var grid = new PathfindingGrid();
        grid.BuildGrid(data, Vector2.zero);

        var walkable = new List<Node>();
        wall = null;
        var seen = new HashSet<Node>();
        for (int x = -150; x <= 150; x++)
        for (int y = -150; y <= 150; y++)
        {
            Node node = grid.GetNode(new Vector2(x, y));
            if (node == null || !seen.Add(node)) continue;
            if (node.Walkable) walkable.Add(node);
            else if (wall == null) wall = node;
        }

        region = new List<Node>();
        var assigned = new HashSet<Node>();
        foreach (Node seed in walkable)
        {
            if (assigned.Contains(seed)) continue;
            List<Node> current = Reachable(seed);
            foreach (Node node in current) assigned.Add(node);
            if (current.Count > region.Count) region = current;
        }
        Assert.GreaterOrEqual(region.Count, 10, "largest floor region of " + PathfindingRoom + " has fewer than 10 tiles");
        return grid;
    }

    // Breadth-first over Node.Neighbors, nearest first: the last element is a farthest tile.
    private static List<Node> Reachable(Node start)
    {
        var order = new List<Node> { start };
        var visited = new HashSet<Node> { start };
        for (int i = 0; i < order.Count; i++)
        {
            foreach (Node next in order[i].Neighbors)
            {
                if (next.Walkable && visited.Add(next)) order.Add(next);
            }
        }
        return order;
    }

    // QA-PATH-01
    [Test]
    public void CommittedRoom_PathToFarthestReachableTile_IsWalkableAdjacentAndEndsAtTarget()
    {
        PathfindingGrid grid = LoadRoomGrid(out List<Node> region, out _);
        Node start = region[0];
        Node goal = region[region.Count - 1];

        NavPath path = AStar.FindPath(grid, start.WorldPosition, goal.WorldPosition);

        Assert.IsTrue(path.Success, $"no path from {start.WorldPosition} to reachable {goal.WorldPosition}");
        AssertWalkableAndAdjacent(grid, path);
        Assert.Less(Vector2.Distance(path.Waypoints[path.Waypoints.Count - 1], goal.WorldPosition), 0.6f, "path does not end at the target");
    }

    // QA-PATH-02
    [Test]
    public void CommittedRoom_PathToAWallTile_FailsCleanly()
    {
        PathfindingGrid grid = LoadRoomGrid(out List<Node> region, out Node wall);
        Assert.IsNotNull(wall, PathfindingRoom + " has no unwalkable tile");

        NavPath path = null;
        Assert.DoesNotThrow(() => path = AStar.FindPath(grid, region[0].WorldPosition, wall.WorldPosition));

        Assert.IsFalse(path.Success);
    }

    // QA-PATH-03
    [Test]
    public void CommittedRoom_SameQueryTwice_GivesTheSamePath()
    {
        PathfindingGrid grid = LoadRoomGrid(out List<Node> region, out _);
        Node start = region[0];
        Node goal = region[region.Count - 1];

        NavPath a = AStar.FindPath(grid, start.WorldPosition, goal.WorldPosition);
        NavPath b = AStar.FindPath(grid, start.WorldPosition, goal.WorldPosition);

        Assert.AreEqual(a.Success, b.Success);
        CollectionAssert.AreEqual(a.Waypoints, b.Waypoints);
    }
}
