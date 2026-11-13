using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

// Mirrors QA-ITEM-01 (QASystemsSuite): DepotItem rolls only items of its table, by absolute tier rate.
public class DepotItemTests
{
    private const string DepotItemPath = "Assets/SO/Item/Depot Item 1.asset";

    private readonly List<Object> _created = new List<Object>();
    private Random.State _randomState;

    [SetUp]
    public void SeedRandom()
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

    private ItemSO NewItem(string name)
    {
        ItemSO item = ScriptableObject.CreateInstance<ItemSO>();
        item.name = name;
        _created.Add(item);
        return item;
    }

    private static DepotItemSlot Slot(ItemSO item, RarityTierItem tier)
    {
        var slot = new DepotItemSlot();
        TestSupport.SetProperty(slot, "DepotItem", item);
        TestSupport.SetProperty(slot, "RarityTier", tier);
        return slot;
    }

    private static DropRateItemSlot Rate(RarityTierItem tier, float rate)
    {
        var slot = new DropRateItemSlot(tier);
        TestSupport.SetProperty(slot, "TierRate", rate);
        return slot;
    }

    private DepotItem NewDepot(List<DepotItemSlot> items, List<DropRateItemSlot> rates)
    {
        DepotItem depot = ScriptableObject.CreateInstance<DepotItem>();
        _created.Add(depot);
        depot.listItems = items;
        TestSupport.SetField(depot, "listTierValidItems", rates);
        depot.InvalidateCache();
        return depot;
    }

    [Test]
    public void TryRollItem_EmptyDepot_NeverDrops()
    {
        DepotItem depot = NewDepot(new List<DepotItemSlot>(), new List<DropRateItemSlot>());

        Assert.IsFalse(depot.TryRollItem(out ItemSO item));
        Assert.IsNull(item);
        Assert.AreEqual(0f, depot.TotalDropRate);
    }

    [Test]
    public void TryRollItem_TierAtHundredPercent_AlwaysDropsAnItemOfTheTable()
    {
        ItemSO ore = NewItem("Ore");
        DepotItem depot = NewDepot(
            new List<DepotItemSlot> { Slot(ore, RarityTierItem.Common) },
            new List<DropRateItemSlot> { Rate(RarityTierItem.Common, 100f) });

        for (int i = 0; i < 200; i++)
        {
            Assert.IsTrue(depot.TryRollItem(out ItemSO item), $"roll {i} dropped nothing");
            Assert.AreSame(ore, item);
        }
    }

    [Test]
    public void TryRollItem_TierWithZeroRate_NeverDrops()
    {
        DepotItem depot = NewDepot(
            new List<DepotItemSlot> { Slot(NewItem("Ore"), RarityTierItem.Common) },
            new List<DropRateItemSlot> { Rate(RarityTierItem.Common, 0f) });

        for (int i = 0; i < 200; i++) Assert.IsFalse(depot.TryRollItem(out _), $"roll {i} dropped an item");
    }

    [Test]
    public void TierWithoutAnyItem_IsExcludedFromTheDropRate()
    {
        DepotItem depot = NewDepot(
            new List<DepotItemSlot> { Slot(NewItem("Ore"), RarityTierItem.Common) },
            new List<DropRateItemSlot> { Rate(RarityTierItem.Common, 40f), Rate(RarityTierItem.Epic, 60f) });

        Assert.AreEqual(40f, depot.TotalDropRate, 0.0001f);
        Assert.AreEqual(1, depot.RollableTiers.Count);
        Assert.IsTrue(depot.HasItemOfTier(RarityTierItem.Common));
        Assert.IsFalse(depot.HasItemOfTier(RarityTierItem.Epic));
    }

    [Test]
    public void TryRollItem_ReportsTheTierOfTheItemItReturns()
    {
        ItemSO common = NewItem("Common");
        ItemSO rare = NewItem("Rare");
        DepotItem depot = NewDepot(
            new List<DepotItemSlot> { Slot(common, RarityTierItem.Common), Slot(rare, RarityTierItem.Rare) },
            new List<DropRateItemSlot> { Rate(RarityTierItem.Common, 50f), Rate(RarityTierItem.Rare, 50f) });
        int commons = 0, rares = 0;

        for (int i = 0; i < 200; i++)
        {
            if (!depot.TryRollItem(out ItemSO item, out RarityTierItem tier)) continue;
            Assert.AreSame(tier == RarityTierItem.Common ? common : rare, item, $"roll {i}: tier {tier} returned {item.name}");
            if (tier == RarityTierItem.Common) commons++; else rares++;
        }

        Assert.Greater(commons, 0, "the Common tier never rolled in 200 tries");
        Assert.Greater(rares, 0, "the Rare tier never rolled in 200 tries");
    }

    // TierRate is an absolute percentage per roll: 30 means about 30% of rolls drop.
    [Test]
    public void TryRollItem_ThirtyPercentTier_DropsAboutThirtyPercentOfRolls()
    {
        DepotItem depot = NewDepot(
            new List<DepotItemSlot> { Slot(NewItem("Ore"), RarityTierItem.Common) },
            new List<DropRateItemSlot> { Rate(RarityTierItem.Common, 30f) });
        int drops = 0;

        for (int i = 0; i < 2000; i++)
            if (depot.TryRollItem(out _)) drops++;

        Assert.That(drops, Is.InRange(400, 800), "expected about 600 drops of 2000");
    }

    [Test]
    public void TryRollItem_SameSeed_GivesTheSameItem()
    {
        DepotItem depot = NewDepot(
            new List<DepotItemSlot>
            {
                Slot(NewItem("A"), RarityTierItem.Common),
                Slot(NewItem("B"), RarityTierItem.Common),
                Slot(NewItem("C"), RarityTierItem.Common),
            },
            new List<DropRateItemSlot> { Rate(RarityTierItem.Common, 100f) });

        Random.InitState(42);
        depot.TryRollItem(out ItemSO first);
        Random.InitState(42);
        depot.TryRollItem(out ItemSO second);

        Assert.IsNotNull(first);
        Assert.AreSame(first, second);
    }

    // QA-ITEM-01 on the committed drop table, through an Instantiate copy.
    [Test]
    public void CommittedDepot_RollsOnlyItemsOfItsTableAndIsDeterministicPerSeed()
    {
        DepotItem depot = Object.Instantiate(TestSupport.LoadAsset<DepotItem>(DepotItemPath));
        _created.Add(depot);
        var table = new HashSet<ItemSO>(depot.listItems.Where(s => s != null && s.DepotItem != null).Select(s => s.DepotItem));
        int foreign = 0;

        for (int i = 0; i < 200; i++)
        {
            if (depot.TryRollItem(out ItemSO item) && !table.Contains(item)) foreign++;
        }
        Assert.AreEqual(0, foreign, "rolled items that are not in the table");

        Random.InitState(42);
        depot.TryRollItem(out ItemSO first);
        Random.InitState(42);
        depot.TryRollItem(out ItemSO second);
        Assert.AreSame(first, second, "the same seed gave different items");
    }
}
