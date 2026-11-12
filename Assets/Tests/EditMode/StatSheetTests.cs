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
}
