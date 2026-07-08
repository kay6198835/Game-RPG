using NUnit.Framework;

// NOTE: test infrastructure (asmdef + Unity Test Framework wiring) is not set up yet
// (tech-debt TD-014). This file follows .claude/rules/test-standards.md naming and will
// compile once /test-setup scaffolds tests/EditMode into the Unity project.
public class RoomEncounterTrackerTests
{
    [Test]
    public void OnEnemyDeath_LastEnemyDies_ReturnsTrueExactlyOnce()
    {
        var tracker = new RoomEncounterTracker();
        tracker.StartEncounter(3);

        Assert.IsFalse(tracker.OnEnemyDeath());
        Assert.IsFalse(tracker.OnEnemyDeath());
        Assert.IsTrue(tracker.OnEnemyDeath());
        Assert.IsFalse(tracker.OnEnemyDeath());
    }

    [Test]
    public void OnEnemyDeath_NoEncounterStarted_ReturnsFalse()
    {
        var tracker = new RoomEncounterTracker();

        Assert.IsFalse(tracker.OnEnemyDeath());
        Assert.AreEqual(0, tracker.AliveCount);
    }

    [Test]
    public void StartEncounter_NegativeCount_ClampsToZero()
    {
        var tracker = new RoomEncounterTracker();
        tracker.StartEncounter(-5);

        Assert.AreEqual(0, tracker.AliveCount);
        Assert.IsFalse(tracker.IsEncounterActive);
    }

    [Test]
    public void Reset_DuringActiveEncounter_StopsTracking()
    {
        var tracker = new RoomEncounterTracker();
        tracker.StartEncounter(2);
        tracker.Reset();

        Assert.IsFalse(tracker.IsEncounterActive);
        Assert.IsFalse(tracker.OnEnemyDeath());
    }
}
