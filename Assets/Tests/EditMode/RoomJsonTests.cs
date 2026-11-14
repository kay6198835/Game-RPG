using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

// Mirrors QA-MAP-05 (QASystemsSuite): every room file under Assets/Data/Json/Room parses into a LevelData
// whose three lists line up.
public class RoomJsonTests
{
    private static string RoomFolder => System.IO.Path.Combine(Application.dataPath, "Data", "Json", "Room");

    [Test]
    public void RoomFolder_ContainsRoomFiles()
    {
        Assert.IsTrue(Directory.Exists(RoomFolder), $"missing folder {RoomFolder}");
        Assert.Greater(Directory.GetFiles(RoomFolder, "*.json").Length, 0, "no *.json under Assets/Data/Json/Room");
    }

    [Test]
    public void EveryRoomJson_ParsesWithMatchingTilesPosesAndLayers()
    {
        var problems = new List<string>();

        foreach (string file in Directory.GetFiles(RoomFolder, "*.json").OrderBy(f => f))
        {
            string name = System.IO.Path.GetFileNameWithoutExtension(file);
            LevelData data = JsonUtility.FromJson<LevelData>(File.ReadAllText(file));

            if (data == null || data.tiles == null || data.poses == null || data.layerIndices == null)
            {
                problems.Add($"{name}: did not parse into a LevelData");
                continue;
            }
            bool ok = data.tiles.Count > 0 && data.tiles.Count == data.poses.Count && data.poses.Count == data.layerIndices.Count;
            if (!ok) problems.Add($"{name}: tiles {data.tiles.Count} / poses {data.poses.Count} / layers {data.layerIndices.Count}");
        }

        Assert.IsEmpty(problems, string.Join("\n", problems));
    }

    [Test]
    public void EveryRoomJson_HasAtLeastOneFloorTile()
    {
        var problems = new List<string>();

        foreach (string file in Directory.GetFiles(RoomFolder, "*.json").OrderBy(f => f))
        {
            LevelData data = JsonUtility.FromJson<LevelData>(File.ReadAllText(file));
            if (data == null || data.tiles == null || !data.tiles.Contains(GameConstants.TileName.FLOOR))
                problems.Add(System.IO.Path.GetFileNameWithoutExtension(file));
        }

        Assert.IsEmpty(problems, "rooms with no " + GameConstants.TileName.FLOOR + " tile: " + string.Join(", ", problems));
    }
}
