using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

// Mirrors QA-MAP-04 (QASystemsSuite): RoomModel.GetSpawnSet spends a weight budget (ADR-0003).
public class RoomModelTests
{
    private const string MapModelPath = "Assets/SO/Database/Map/MapModel.asset";

    private readonly List<Object> _created = new List<Object>();
    private Random.State _randomState;

    [SetUp]
    public void SaveRandom()
    {
        _randomState = Random.state;
        Random.InitState(20261112);
    }

    [TearDown]
    public void Cleanup()
    {
        Random.state = _randomState;
        TestSupport.DestroyAll(_created);
    }

    private static EnemyModal Enemy(int weight, RarityTierEnemy tier)
    {
        return new EnemyModal { weight = weight, rarityTier = tier };
    }

    private RoomModel NewRoom(int weightBudget, List<EnemyModal> enemies)
    {
        RoomModel room = ScriptableObject.CreateInstance<RoomModel>();
        _created.Add(room);
        TestSupport.SetField(room, "enemiesOfRoom", enemies);
        TestSupport.SetField(room, "weightBudget", weightBudget);
        return room;
    }

    private static int TotalWeight(List<EnemySpawnEntry> set)
    {
        return set.Sum(e => e.enemy.weight * e.count);
    }

    [Test]
    public void GetSpawnSet_WithNoEnemyType_ReturnsNull()
    {
        RoomModel room = NewRoom(100, new List<EnemyModal>());

        Assert.IsNull(room.GetSpawnSet());
    }

    [Test]
    public void GetSpawnSet_SingleEnemyType_SpendsBudgetDownToTenPercent()
    {
        EnemyModal grunt = Enemy(10, RarityTierEnemy.Common);
        RoomModel room = NewRoom(100, new List<EnemyModal> { grunt });

        List<EnemySpawnEntry> set = room.GetSpawnSet();

        // Picks continue while the remaining budget is above 10% of the budget: 100 -> 10 in steps of 10.
        Assert.AreEqual(1, set.Count);
        Assert.AreSame(grunt, set[0].enemy);
        Assert.AreEqual(9, set[0].count);
    }

    [Test]
    public void GetSpawnSet_GroupsRepeatedPicksIntoOneEntryPerEnemyType()
    {
        var enemies = new List<EnemyModal>
        {
            Enemy(5, RarityTierEnemy.Common),
            Enemy(8, RarityTierEnemy.Rare),
            Enemy(10, RarityTierEnemy.Epic),
        };
        RoomModel room = NewRoom(100, enemies);

        for (int seed = 0; seed < 50; seed++)
        {
            Random.InitState(seed);
            List<EnemySpawnEntry> set = room.GetSpawnSet();

            Assert.AreEqual(set.Count, set.Select(e => e.enemy).Distinct().Count(), $"seed {seed}: an enemy type has two entries");
            Assert.IsTrue(set.All(e => enemies.Contains(e.enemy)), $"seed {seed}: an entry is not an enemy of the room");
            Assert.IsTrue(set.All(e => e.count >= 1), $"seed {seed}: an entry has count < 1");
        }
    }

    // Every weight is at most 10% of the budget, so a pick can never overshoot it.
    [Test]
    public void GetSpawnSet_WithSmallWeights_SpendsBetweenNinetyAndHundredPercentOfBudget()
    {
        RoomModel room = NewRoom(100, new List<EnemyModal>
        {
            Enemy(5, RarityTierEnemy.Common),
            Enemy(8, RarityTierEnemy.Rare),
            Enemy(10, RarityTierEnemy.Legendary),
        });

        for (int seed = 0; seed < 50; seed++)
        {
            Random.InitState(seed);
            int spent = TotalWeight(room.GetSpawnSet());

            Assert.That(spent, Is.InRange(90, 100), $"seed {seed}");
        }
    }

    [Test]
    public void GetSpawnSet_SameSeed_GivesTheSameSet()
    {
        var enemies = new List<EnemyModal>
        {
            Enemy(5, RarityTierEnemy.Common),
            Enemy(8, RarityTierEnemy.Rare),
            Enemy(10, RarityTierEnemy.Epic),
        };
        RoomModel room = NewRoom(100, enemies);

        Random.InitState(7);
        string first = Describe(room.GetSpawnSet(), enemies);
        Random.InitState(7);
        string second = Describe(room.GetSpawnSet(), enemies);

        Assert.AreEqual(first, second);
    }

    private static string Describe(List<EnemySpawnEntry> set, List<EnemyModal> enemies)
    {
        return string.Join(",", set.Select(e => enemies.IndexOf(e.enemy) + "x" + e.count));
    }

    // QA-MAP-04 on the committed rooms: same seeds (1000..1019) and same limit as the Play Mode case.
    [Test]
    public void CommittedRooms_SpawnSetStaysWithinBudgetPlusOverflow()
    {
        MapModel map = TestSupport.LoadAsset<MapModel>(MapModelPath);
        List<RoomModel> rooms = map.fullRoomList.Where(r => r != null).Distinct().ToList();
        Assert.Greater(rooms.Count, 0, "MapModel.asset has no room");
        var problems = new List<string>();

        foreach (RoomModel asset in rooms)
        {
            int budget = TestSupport.GetField<int>(asset, "weightBudget");
            float overflow = TestSupport.GetField<float>(asset, "overflowPercent");
            int limit = Mathf.FloorToInt(budget * (1f + overflow));
            int worst = 0, empty = 0;

            for (int seed = 0; seed < 20; seed++)
            {
                Random.InitState(1000 + seed);
                RoomModel room = Object.Instantiate(asset);
                _created.Add(room);
                List<EnemySpawnEntry> set = room.GetSpawnSet();
                if (set == null || set.Count == 0) { empty++; continue; }
                worst = Mathf.Max(worst, TotalWeight(set));
            }

            if (empty > 0) problems.Add($"{asset.name}: {empty}/20 seeds returned an empty spawn set");
            if (worst > limit) problems.Add($"{asset.name}: total weight {worst} exceeds budget {budget} +{overflow:P0} = {limit}");
        }

        Assert.IsEmpty(problems, string.Join("\n", problems));
    }
}
