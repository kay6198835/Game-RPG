using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

// The data side of item pickup (QA-ITEM-05..12 judge it in Play Mode): modifier signs and sources, stacking of
// two pickups on one StatSheet, the wallet, and the committed items a drop table can roll.
// Added 2026-11-20 (coverage review).
public class ItemEffectDataTests
{
    private const float Tolerance = 0.0001f;
    private readonly List<Object> _created = new();

    [TearDown]
    public void TearDown() => TestSupport.DestroyAll(_created);

    [Test]
    public void VitalModifier_PositiveValue_IsRecovery_NegativeIsReduction()
    {
        Assert.AreEqual(StatImpactType.Recovery, new VitalModifier(VitalStatType.HP, 10f, ModifierType.Flat).ImpactType);
        Assert.AreEqual(StatImpactType.Recovery, new VitalModifier(VitalStatType.HP, 0f, ModifierType.Flat).ImpactType);
        Assert.AreEqual(StatImpactType.Reduction, new VitalModifier(VitalStatType.HP, -10f, ModifierType.Flat).ImpactType);
    }

    [Test]
    public void StatModifier_WithSource_CopiesValuesAndLeavesTheAuthoredOneUnstamped()
    {
        var authored = new StatModifier(StatType.STR, 3f, ModifierType.Flat);
        var source = new object();

        StatModifier stamped = authored.WithSource(source);

        Assert.AreNotSame(authored, stamped);
        Assert.AreSame(source, stamped.Source);
        Assert.IsNull(authored.Source, "the authored (asset) modifier was stamped");
        Assert.AreEqual(authored.TargetStatType, stamped.TargetStatType);
        Assert.AreEqual(authored.Type, stamped.Type);
        Assert.AreEqual(authored.Value, stamped.Value, Tolerance);
    }

    [Test]
    public void StatModifierGroup_New_HasEmptyNonNullLists()
    {
        var group = new StatModifierGroup();

        Assert.IsNotNull(group.GetStatsModifiers);
        Assert.IsNotNull(group.GetVitalModifiers);
        Assert.AreEqual(0, group.GetStatsModifiers.Count);
        Assert.AreEqual(0, group.GetVitalModifiers.Count);
    }

    [Test]
    public void AddModifiersFromSource_SameItemListFromTwoPickups_StacksAndRemovesIndependently()
    {
        var sheet = new StatSheet(null);
        var authored = new List<StatModifier> { new StatModifier(StatType.STR, 3f, ModifierType.Flat), new StatModifier(StatType.INT, 1f, ModifierType.Flat) };
        var first = new object();
        var second = new object();

        sheet.AddModifiersFromSource(first, authored);
        sheet.AddModifiersFromSource(second, authored);
        Assert.AreEqual(6f, sheet.GetStatValue(StatType.STR), Tolerance);
        Assert.AreEqual(2f, sheet.GetStatValue(StatType.INT), Tolerance);
        Assert.AreEqual(2, authored.Count, "the authored list was changed");

        sheet.RemoveModifiersFromSource(first);
        Assert.AreEqual(3f, sheet.GetStatValue(StatType.STR), Tolerance);
        Assert.AreEqual(1f, sheet.GetStatValue(StatType.INT), Tolerance);
    }

    [Test]
    public void AddModifiersFromSource_NullListOrNullEntry_IsSkipped()
    {
        var sheet = new StatSheet(null);
        var source = new object();

        Assert.DoesNotThrow(() => sheet.AddModifiersFromSource(source, null));
        sheet.AddModifiersFromSource(source, new List<StatModifier> { null, new StatModifier(StatType.DEX, 2f, ModifierType.Flat) });

        Assert.AreEqual(2f, sheet.GetStatValue(StatType.DEX), Tolerance);
    }

    [Test]
    public void RemoveModifiersFromSource_UnknownSource_ChangesNothing()
    {
        var sheet = new StatSheet(null);
        sheet.AddModifiersFromSource(new object(), new List<StatModifier> { new StatModifier(StatType.VIT, 4f, ModifierType.Flat) });

        sheet.RemoveModifiersFromSource(new object());

        Assert.AreEqual(4f, sheet.GetStatValue(StatType.VIT), Tolerance);
    }

    [Test]
    public void ClearModifiers_RemovesEverySource()
    {
        var sheet = new StatSheet(null);
        sheet.AddModifiersFromSource(new object(), new List<StatModifier> { new StatModifier(StatType.LUK, 4f, ModifierType.Flat) });
        sheet.AddModifiersFromSource(new object(), new List<StatModifier> { new StatModifier(StatType.LUK, 1f, ModifierType.Flat) });

        sheet.ClearModifiers();

        Assert.AreEqual(0f, sheet.GetStatValue(StatType.LUK), Tolerance);
        Assert.AreEqual(0, sheet.GetStat(StatType.LUK).Modifiers.Count);
    }

    [Test]
    public void NewItemSO_HasAnEmptyEffectList()
    {
        ItemSO item = ScriptableObject.CreateInstance<ItemSO>();
        _created.Add(item);

        Assert.IsNotNull(item.Effects);
        Assert.AreEqual(0, item.Effects.Count);
    }

    // WalletComponent's methods do not touch its Core, so it is tested on a bare GameObject (Awake does not run in
    // Edit Mode).
    private WalletComponent NewWallet()
    {
        var go = new GameObject("QAWallet");
        _created.Add(go);
        return go.AddComponent<WalletComponent>();
    }

    [Test]
    public void Wallet_AddCoins_AddsExactly()
    {
        WalletComponent wallet = NewWallet();

        wallet.AddCoins(10);
        wallet.AddCoins(5);

        Assert.AreEqual(15, wallet.GetCurrentCoin());
    }

    [Test]
    public void Wallet_TryPayCoin_MoreThanOwned_IsRefusedAndKeepsTheBalance()
    {
        WalletComponent wallet = NewWallet();
        wallet.AddCoins(10);

        Assert.IsFalse(wallet.TryPayCoin(11));
        Assert.AreEqual(10, wallet.GetCurrentCoin());
    }

    [Test]
    public void Wallet_TryPayCoin_ExactBalance_PaysToZero()
    {
        WalletComponent wallet = NewWallet();
        wallet.AddCoins(10);

        Assert.IsTrue(wallet.TryPayCoin(10));
        Assert.AreEqual(0, wallet.GetCurrentCoin());
    }

    [Test]
    public void Wallet_ReloadComponent_ResetsToZero()
    {
        WalletComponent wallet = NewWallet();
        wallet.AddCoins(10);

        wallet.ReloadComponent();

        Assert.AreEqual(0, wallet.GetCurrentCoin());
    }

    [Test]
    public void CommittedDropTables_EveryItemHasANameASpriteAndLoadableEffects()
    {
        List<DepotItem> depots = AssetDatabase.FindAssets("t:DepotItem", new[] { "Assets/SO" })
            .Select(AssetDatabase.GUIDToAssetPath).Select(AssetDatabase.LoadAssetAtPath<DepotItem>).Where(d => d != null).ToList();
        Assert.IsNotEmpty(depots, "no DepotItem under Assets/SO");
        List<string> problems = new();
        foreach (ItemSO item in depots.SelectMany(d => d.listItems).Where(s => s != null && s.DepotItem != null).Select(s => s.DepotItem).Distinct())
        {
            string path = AssetDatabase.GetAssetPath(item);
            if (string.IsNullOrWhiteSpace(item.ItemName)) problems.Add($"{path}: empty ItemName");
            if (item.ItemSprite == null) problems.Add($"{path}: no ItemSprite");
            if (item.Effects == null || item.Effects.Count == 0) problems.Add($"{path}: no effect");
            else if (item.Effects.Any(e => e == null)) problems.Add($"{path}: an effect slot is empty or bound to a missing script");
        }
        Assert.IsEmpty(problems, string.Join("\n", problems));
    }
}
