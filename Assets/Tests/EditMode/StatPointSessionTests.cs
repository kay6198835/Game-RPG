using System.Collections.Generic;
using NUnit.Framework;

// StatPointSession (stats panel, key C): spend, undo within the session, revert, accept, full respec.
// Added 2026-11-20 (coverage review). Runs on a profile-less StatSheet behind a fake IPlayerStatService whose
// level is set by the test, so points exist without a committed asset.
public class StatPointSessionTests
{
    private sealed class FakePlayerStats : IPlayerStatService
    {
        public readonly StatSheet Sheet = new StatSheet(null);
        public int Level = 1;

        public int GetLevelUpStatsBonus()
        {
            float spent = 0f;
            foreach (Stat stat in Sheet.All)
                if (stat.Type.IsPrimary()) spent += stat.LevelUpValue;
            return System.Math.Max(0, Level - 1 - (int)spent);
        }

        public void AddPrimaryPoint(StatType statType, int amount) => Sheet.AddPrimaryPoint(statType, amount);
        public bool TryLevelup() => false;
        public int GetLevel() => Level;
        public Dictionary<StatType, StatsViewDTO> GetFullViewStats() => Sheet.GetFullViewStats();
        public StatsViewDTO GetViewStat(StatType statType) => Sheet.GetViewStat(statType);
        public Stat GetStat(StatType statType) => Sheet.GetStat(statType);
        public Dictionary<StatType, float> GetFullStat() => Sheet.GetFullStat();
        public float GetStatValue(StatType statType) => Sheet.GetStatValue(statType);
        public void RemoveModifiersFromSource(object source) => Sheet.RemoveModifiersFromSource(source);
        public void AddModifiersFromSource(object source, IReadOnlyList<StatModifier> modifiers) => Sheet.AddModifiersFromSource(source, modifiers);
    }

    private static (FakePlayerStats stats, StatPointSession session) Open(int level)
    {
        var stats = new FakePlayerStats { Level = level };
        var session = new StatPointSession(stats);
        session.Begin();
        return (stats, session);
    }

    [Test]
    public void Constructor_RejectsNullService()
    {
        Assert.Throws<System.ArgumentNullException>(() => new StatPointSession(null));
    }

    [Test]
    public void Begin_OpensWithTheUnspentPoints()
    {
        var (_, session) = Open(level: 4);

        Assert.AreEqual(3, session.PointsAtOpen);
        Assert.AreEqual(3, session.PointsRemaining);
        Assert.IsFalse(session.HasChanges);
    }

    [Test]
    public void TryIncrease_SpendsOnePointOnThePrimaryStat()
    {
        var (stats, session) = Open(level: 3);

        Assert.IsTrue(session.TryIncrease(StatType.STR));

        Assert.AreEqual(1, session.PointsRemaining);
        Assert.AreEqual(1, session.SpentThisSession(StatType.STR));
        Assert.AreEqual(1f, stats.GetStat(StatType.STR).LevelUpValue, 0.0001f);
        Assert.IsTrue(session.HasChanges);
    }

    [Test]
    public void TryIncrease_RefusesADerivedStat()
    {
        var (stats, session) = Open(level: 3);

        Assert.IsFalse(session.TryIncrease(StatType.HP));

        Assert.AreEqual(2, session.PointsRemaining);
        Assert.AreEqual(0f, stats.GetStat(StatType.HP).LevelUpValue, 0.0001f);
    }

    [Test]
    public void TryIncrease_RefusesWhenNoPointIsLeft()
    {
        var (stats, session) = Open(level: 2);

        Assert.IsTrue(session.TryIncrease(StatType.DEX));
        Assert.IsFalse(session.TryIncrease(StatType.DEX));

        Assert.AreEqual(0, session.PointsRemaining);
        Assert.AreEqual(1f, stats.GetStat(StatType.DEX).LevelUpValue, 0.0001f);
    }

    [Test]
    public void TryDecrease_OnlyTakesBackPointsSpentThisSession()
    {
        var (stats, session) = Open(level: 3);
        session.TryIncrease(StatType.INT);

        Assert.IsTrue(session.TryDecrease(StatType.INT));
        Assert.IsFalse(session.TryDecrease(StatType.INT), "a point not spent in this session was taken back");
        Assert.IsFalse(session.TryDecrease(StatType.VIT), "a stat never raised was lowered");

        Assert.AreEqual(2, session.PointsRemaining);
        Assert.AreEqual(0f, stats.GetStat(StatType.INT).LevelUpValue, 0.0001f);
    }

    [Test]
    public void Revert_UndoesEverySpendOfTheSession()
    {
        var (stats, session) = Open(level: 4);
        session.TryIncrease(StatType.STR);
        session.TryIncrease(StatType.STR);
        session.TryIncrease(StatType.LUK);

        session.Revert();

        Assert.AreEqual(3, session.PointsRemaining);
        Assert.AreEqual(0f, stats.GetStat(StatType.STR).LevelUpValue, 0.0001f);
        Assert.AreEqual(0f, stats.GetStat(StatType.LUK).LevelUpValue, 0.0001f);
        Assert.IsFalse(session.HasChanges);
    }

    [Test]
    public void Accept_CommitsThePointsSoTheyCannotBeTakenBack()
    {
        var (stats, session) = Open(level: 3);
        session.TryIncrease(StatType.VIT);

        session.Accept();

        Assert.AreEqual(1, session.PointsAtOpen);
        Assert.IsFalse(session.TryDecrease(StatType.VIT));
        Assert.AreEqual(1f, stats.GetStat(StatType.VIT).LevelUpValue, 0.0001f);
        session.Revert();
        Assert.AreEqual(1f, stats.GetStat(StatType.VIT).LevelUpValue, 0.0001f, "Revert after Accept removed a committed point");
    }

    [Test]
    public void Restore_RefundsEveryCommittedPoint()
    {
        var (stats, session) = Open(level: 4);
        session.TryIncrease(StatType.STR);
        session.TryIncrease(StatType.DEX);
        session.Accept();
        Assert.IsTrue(session.CanRestore);

        session.Restore();

        Assert.AreEqual(3, session.PointsRemaining);
        Assert.AreEqual(0f, stats.GetStat(StatType.STR).LevelUpValue, 0.0001f);
        Assert.AreEqual(0f, stats.GetStat(StatType.DEX).LevelUpValue, 0.0001f);
        Assert.IsFalse(session.CanRestore);
    }

    [Test]
    public void TryIncrease_OnePoint_RaisesFinalValueOfThePrimaryByOne()
    {
        var (stats, session) = Open(level: 2);
        Dictionary<StatType, float> before = stats.GetFullStat();

        session.TryIncrease(StatType.STR);

        Assert.AreEqual(before[StatType.STR] + 1f, stats.GetStatValue(StatType.STR), 0.0001f);
    }
}
