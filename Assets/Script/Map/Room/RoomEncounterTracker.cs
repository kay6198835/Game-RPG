public class RoomEncounterTracker
{
    public int AliveCount { get; private set; }
    public bool IsEncounterActive { get => AliveCount > 0; }

    public void StartEncounter(int enemyCount)
    {
        AliveCount = enemyCount > 0 ? enemyCount : 0;
    }

    // Returns true exactly once: on the death that empties the room.
    public bool OnEnemyDeath()
    {
        if (AliveCount <= 0) return false;
        AliveCount--;
        return AliveCount == 0;
    }

    public void Reset()
    {
        AliveCount = 0;
    }
}
