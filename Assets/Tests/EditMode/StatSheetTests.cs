using System.Collections.Generic;
using NUnit.Framework;

public class StatSheetTests
{
    private const float Tolerance = 0.0001f;

    [Test]
    public void NewSheet_WithoutProfile_StartsAtZero()
    {
        var sheet = new StatSheet(null);

        Assert.AreEqual(0f, sheet.GetStatValue(StatType.STR), Tolerance);
        Assert.AreEqual(0f, sheet.GetStatValue(StatType.HP), Tolerance);
    }

    [Test]
    public void Modifiers_ApplyFlatThenPercentAddThenPercentMult()
    {
        var sheet = new StatSheet(null);
        var source = new object();

        sheet.AddModifiersFromSource(source, new List<StatModifier>
        {
            new StatModifier(StatType.STR, 1f, ModifierType.PercentMult),
            new StatModifier(StatType.STR, 0.5f, ModifierType.PercentAdd),
            new StatModifier(StatType.STR, 10f, ModifierType.Flat),
        });

        // (0 + 10) x (1 + 0.5) x (1 + 1) = 30
        Assert.AreEqual(30f, sheet.GetStatValue(StatType.STR), Tolerance);
    }

    [Test]
    public void RemoveModifiersFromSource_KeepsOtherSources()
    {
        var sheet = new StatSheet(null);
        var item = new object();
        var buff = new object();
        sheet.AddModifiersFromSource(item, new List<StatModifier> { new StatModifier(StatType.STR, 10f, ModifierType.Flat) });
        sheet.AddModifiersFromSource(buff, new List<StatModifier> { new StatModifier(StatType.STR, 5f, ModifierType.Flat) });

        sheet.RemoveModifiersFromSource(buff);

        Assert.AreEqual(10f, sheet.GetStatValue(StatType.STR), Tolerance);
    }

    [Test]
    public void AddPrimaryPoint_RaisesChangedOnceForThatStat()
    {
        var sheet = new StatSheet(null);
        var changed = new List<StatType>();
        sheet.Changed += changed.Add;

        sheet.AddPrimaryPoint(StatType.STR, 2f);

        Assert.AreEqual(2f, sheet.GetStatValue(StatType.STR), Tolerance);
        CollectionAssert.AreEqual(new[] { StatType.STR }, changed);
    }

    // ---------------------------------------------------------------------------------------------------
    // Mirrors of QA-STAT-01..06 (QASystemsSuite). 01..05 run on a synthetic profile so the expected numbers
    // are exact; 06 reads the committed stat assets.
    //
    // Profile: level 3, STR 10, VIT 8, Defense = 5 + level x 2 + VIT x 0.5.
    //   Defense base = 5 + 8 x 0.5 = 9, level-up = 3 x 2 = 6, so FinalValue = 15 with no modifier.
    // ---------------------------------------------------------------------------------------------------

    private const StatType Probe = StatType.Defense;
    private const float ProbeStart = 15f;

    private readonly List<UnityEngine.Object> _created = new List<UnityEngine.Object>();

    [TearDown]
    public void DestroyCreatedObjects()
    {
        TestSupport.DestroyAll(_created);
    }

    private BaseStatsSO NewProfile()
    {
        BaseStatsSO profile = UnityEngine.ScriptableObject.CreateInstance<BaseStatsSO>();
        _created.Add(profile);
        TestSupport.SetField(profile, "level", 3);
        TestSupport.SetField(profile, "primaryStats", new List<PrimaryStatEntry>
        {
            new PrimaryStatEntry(StatType.STR, 10f),
            new PrimaryStatEntry(StatType.VIT, 8f),
        });
        TestSupport.SetField(profile, "derivedStats", new[]
        {
            new DerivedStatFormula
            {
                targetStat = StatType.Defense,
                baseConstant = 5f,
                perLevel = 2f,
                contributions = new[]
                {
                    new DerivedStatFormula.StatContribution { sourceStat = StatType.VIT, coefficient = 0.5f },
                },
            },
        });
        return profile;
    }

    [Test]
    public void NewSheet_FromProfile_ComputesPrimaryBaseAndDerivedFormula()
    {
        var sheet = new StatSheet(NewProfile());

        Assert.AreEqual(3, sheet.GetLevel());
        Assert.AreEqual(10f, sheet.GetStatValue(StatType.STR), Tolerance);
        Assert.AreEqual(8f, sheet.GetStatValue(StatType.VIT), Tolerance);
        Assert.AreEqual(ProbeStart, sheet.GetStatValue(Probe), Tolerance);
        Assert.AreEqual(0f, sheet.GetStatValue(StatType.HP), Tolerance, "a derived stat with no formula reads 0");
    }

    // QA-STAT-01
    [Test]
    public void FlatModifier_AddsItsValueToFinalValue()
    {
        var sheet = new StatSheet(NewProfile());

        sheet.AddModifiersFromSource(new object(), new[] { new StatModifier(Probe, 5f, ModifierType.Flat) });

        Assert.AreEqual(20f, sheet.GetStatValue(Probe), Tolerance);
    }

    // QA-STAT-02
    [Test]
    public void PercentAddModifiers_SumBeforeMultiplyingOnce()
    {
        var sheet = new StatSheet(NewProfile());

        sheet.AddModifiersFromSource(new object(), new[]
        {
            new StatModifier(Probe, 0.1f, ModifierType.PercentAdd),
            new StatModifier(Probe, 0.2f, ModifierType.PercentAdd),
        });

        // 15 x (1 + 0.1 + 0.2) = 19.5, not 15 x 1.1 x 1.2 = 19.8
        Assert.AreEqual(19.5f, sheet.GetStatValue(Probe), Tolerance);
    }

    // QA-STAT-03
    [Test]
    public void PercentMultModifier_MultipliesAfterFlat()
    {
        var sheet = new StatSheet(NewProfile());

        sheet.AddModifiersFromSource(new object(), new[]
        {
            new StatModifier(Probe, 4f, ModifierType.Flat),
            new StatModifier(Probe, 0.5f, ModifierType.PercentMult),
        });

        // (15 + 4) x 1.5 = 28.5
        Assert.AreEqual(28.5f, sheet.GetStatValue(Probe), Tolerance);
    }

    // QA-STAT-04
    [Test]
    public void RemoveModifiersFromSource_RestoresValueAndLeavesNoModifier()
    {
        var sheet = new StatSheet(NewProfile());
        var source = new object();
        sheet.AddModifiersFromSource(source, new[]
        {
            new StatModifier(Probe, 7f, ModifierType.Flat),
            new StatModifier(Probe, 0.25f, ModifierType.PercentAdd),
        });

        sheet.RemoveModifiersFromSource(source);

        Assert.AreEqual(ProbeStart, sheet.GetStatValue(Probe), Tolerance);
        Assert.AreEqual(0, sheet.GetStat(Probe).Modifiers.Count);
    }

    // QA-STAT-05
    [Test]
    public void RemoveModifiersFromSource_OnProfileSheet_KeepsOtherSourceModifier()
    {
        var sheet = new StatSheet(NewProfile());
        var item = new object();
        var buff = new object();
        sheet.AddModifiersFromSource(item, new[] { new StatModifier(Probe, 3f, ModifierType.Flat) });
        sheet.AddModifiersFromSource(buff, new[] { new StatModifier(Probe, 8f, ModifierType.Flat) });

        sheet.RemoveModifiersFromSource(buff);

        Assert.AreEqual(18f, sheet.GetStatValue(Probe), Tolerance);
        Assert.AreEqual(1, sheet.GetStat(Probe).Modifiers.Count);
    }

    [Test]
    public void PrimaryPoint_RaisesDerivedStatThroughItsCoefficient()
    {
        var sheet = new StatSheet(NewProfile());
        var changed = new List<StatType>();
        sheet.Changed += changed.Add;

        sheet.AddPrimaryPoint(StatType.VIT, 2f);

        // VIT 8 -> 10; Defense level-up 6 + 2 x 0.5 = 7 -> 16
        Assert.AreEqual(10f, sheet.GetStatValue(StatType.VIT), Tolerance);
        Assert.AreEqual(16f, sheet.GetStatValue(Probe), Tolerance);
        CollectionAssert.AreEquivalent(new[] { StatType.VIT, StatType.Defense }, changed);
    }

    [Test]
    public void ModifierOnPrimaryStat_ReachesDerivedStatAsPrimaryBonus()
    {
        var sheet = new StatSheet(NewProfile());

        sheet.AddModifiersFromSource(new object(), new[] { new StatModifier(StatType.VIT, 4f, ModifierType.Flat) });

        // VIT 8 -> 12; Defense 15 + 4 x 0.5 = 17
        Assert.AreEqual(12f, sheet.GetStatValue(StatType.VIT), Tolerance);
        Assert.AreEqual(17f, sheet.GetStatValue(Probe), Tolerance);
    }

    [Test]
    public void AddPrimaryPoint_OnDerivedStat_IsRefused()
    {
        var sheet = new StatSheet(NewProfile());

        UnityEngine.TestTools.LogAssert.Expect(UnityEngine.LogType.Warning, new System.Text.RegularExpressions.Regex("not a primary stat"));
        sheet.AddPrimaryPoint(Probe, 5f);

        Assert.AreEqual(ProbeStart, sheet.GetStatValue(Probe), Tolerance);
    }

    [Test]
    public void TwoSheetsFromOneProfile_DoNotShareModifiers()
    {
        BaseStatsSO profile = NewProfile();
        var buffed = new StatSheet(profile);
        var other = new StatSheet(profile);

        buffed.AddModifiersFromSource(new object(), new[] { new StatModifier(Probe, 50f, ModifierType.Flat) });
        buffed.AddPrimaryPoint(StatType.STR, 3f);

        Assert.AreEqual(ProbeStart, other.GetStatValue(Probe), Tolerance);
        Assert.AreEqual(10f, other.GetStatValue(StatType.STR), Tolerance);
    }

    // QA-STAT-06, on every committed BaseStatsSO asset (the Play Mode case reads PaladinStats only).
    [Test]
    public void CommittedStatAssets_AreUnchangedAfterASheetIsBuffed()
    {
        string[] guids = UnityEditor.AssetDatabase.FindAssets("t:BaseStatsSO", new[] { "Assets/SO" });
        Assert.Greater(guids.Length, 0, "no BaseStatsSO asset found under Assets/SO");
        var problems = new List<string>();

        foreach (string guid in guids)
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            BaseStatsSO asset = UnityEditor.AssetDatabase.LoadAssetAtPath<BaseStatsSO>(path);
            if (asset == null) continue;

            string serializedBefore = UnityEditor.EditorJsonUtility.ToJson(asset);
            Dictionary<StatType, float> before = new StatSheet(asset).GetFullStat();

            var sheet = new StatSheet(asset);
            sheet.AddModifiersFromSource(new object(), new[] { new StatModifier(Probe, 50f, ModifierType.Flat) });
            sheet.AddPrimaryPoint(StatType.STR, 3f);

            Dictionary<StatType, float> after = new StatSheet(asset).GetFullStat();
            foreach (KeyValuePair<StatType, float> pair in before)
            {
                if (!after.TryGetValue(pair.Key, out float value) || !UnityEngine.Mathf.Approximately(value, pair.Value))
                    problems.Add($"{path}: {pair.Key} {pair.Value} -> {value}");
            }
            if (UnityEditor.EditorJsonUtility.ToJson(asset) != serializedBefore)
                problems.Add($"{path}: serialized data changed");
        }

        Assert.IsEmpty(problems, string.Join("\n", problems));
    }
}
