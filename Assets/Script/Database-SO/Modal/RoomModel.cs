using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomModel", menuName = "Game/Room Model")]
public class RoomModel : EntityModel
{
    [SerializeField] private List<EnemyModal> enemiesOfRoom = new List<EnemyModal>();
    [SerializeField] private List<EnemyModal> candidateEnemies = new List<EnemyModal>();
    [SerializeField, Range(0, 500)] private int weightBudget;
    [SerializeField, Range(0f, 1f)] private float overflowPercent = 0.1f;
    private EnemySpawnEntry[] _entries;

    public List<EnemySpawnEntry> GetSpawnSet()
    {
        int n = enemiesOfRoom.Count;
        if (n == 0) return null;

        // cấp/mở rộng scratch nếu số loại đổi
        if (_entries == null || _entries.Length != n)
        {
            _entries = new EnemySpawnEntry[n];
        }
        System.Array.Clear(_entries, 0, n);   // reset dedup mỗi call

        var outList = new List<EnemySpawnEntry>();
        var ranDomChosen = new EnemyModal();
        var weightBudget = this.weightBudget;

        // ----- PHA 1: RANDOM — pick tự do trong nhóm fit, cho trùng loại -----
        int usedRandom = 0;
        while (weightBudget > this.weightBudget * 0.1f)
        {

            SetListCandidate(1, weightBudget);
            ranDomChosen = candidateEnemies[Random.Range(0, candidateEnemies.Count)];
            for (int i = 0; i < n; i++)
            {
                if (enemiesOfRoom[i] == ranDomChosen)
                {
                    int idx = i;
                    weightBudget -= enemiesOfRoom[i].weight;
                    Emit(idx, outList);
                    break;
                }
            }
            candidateEnemies.Clear();

        }

        return outList;
    }

    private void SetListCandidate(int retry, int weightBudget)
    {
        if (retry > 4)
        {
            candidateEnemies.AddRange(enemiesOfRoom);
            return;
        }
        foreach (var enemy in enemiesOfRoom)
        {
            if (Random.Range(0f, 100f) <= (int)enemy.rarityTier &&
            enemy.weight <= weightBudget)
            {
                candidateEnemies.Add(enemy);
            }
        }
        if (candidateEnemies.Count == 0)
        {
            SetListCandidate(++retry, weightBudget);
        }
    }

    private void Emit(int idx, List<EnemySpawnEntry> outList)
    {
        var entry = _entries[idx];
        if (entry == null)
        {
            entry = new EnemySpawnEntry(enemiesOfRoom[idx]); // count = 1
            _entries[idx] = entry;
            outList.Add(entry);
        }
        else
        {
            entry.count++;
        }
    }
}

[System.Serializable]
public class EnemySpawnEntry
{
    public EnemyModal enemy;
    public int count;
    
    public EnemySpawnEntry(EnemyModal enemy)
    {
        this.enemy = enemy;
        this.count = 1;
    }
}
[System.Serializable]
public class EnemyModal
{
    public GameObject prefab;
    public int weight;
    public RarityTier rarityTier;
}
[System.Serializable]
public enum RarityTier
{
    Common = 50,
    Rare = 30,
    Epic = 15,
    Legendary = 5
}