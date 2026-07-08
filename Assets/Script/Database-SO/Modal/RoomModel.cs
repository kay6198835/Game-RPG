using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomModel", menuName = "Game/Room Model")]
public class RoomModel : EntityModel
{
    [SerializeField] private List<EnemyModal> enemiesOfRoom = new List<EnemyModal>();
    [SerializeField, Range(0, 500)] private int weightBudget;
    [SerializeField, Range(0f, 1f)] private float randomRatio = 0.33f;
    [SerializeField, Range(0f, 1f)] private float overflowPercent = 0.1f;

    /// <summary>
    /// Kết quả gửi spawn system: mỗi loại quái + số lượng.
    /// Chạy hybrid select trên enemiesOfRoom, gom trùng theo loại ra count.
    /// </summary>
    public List<EnemySpawnEntry> GetSpawnSet()
    {
        List<EnemyModal> chosen = GetHybridEnemySet();

        Dictionary<EnemyModal, EnemySpawnEntry> map = new Dictionary<EnemyModal, EnemySpawnEntry>();
        foreach (var e in chosen)
        {
            if (map.TryGetValue(e, out var entry))
                entry.count++;
            else
                map[e] = new EnemySpawnEntry(e);
        }
        return new List<EnemySpawnEntry>(map.Values);
    }

    private List<EnemyModal> GetHybridEnemySet()
    {
        // ứng viên hợp lệ: bỏ null + weight > 0 (chống Pha 2 loop vô hạn)
        List<EnemyModal> candidates = new List<EnemyModal>();
        foreach (var e in enemiesOfRoom)
            if (e != null && e.weight > 0) candidates.Add(e);

        if (candidates.Count == 0)
            return new List<EnemyModal>();

        List<EnemyModal> result = new List<EnemyModal>();
        int randomBudget = Mathf.RoundToInt(weightBudget * randomRatio);
        int maxOverflow = Mathf.RoundToInt(weightBudget * overflowPercent);
        int remaining = weightBudget;

        // ----- PHA 1: RANDOM — pick tự do trong nhóm fit, cho trùng loại -----
        List<EnemyModal> fit = new List<EnemyModal>();
        int usedRandom = 0;
        while (true)
        {
            fit.Clear();
            foreach (var e in candidates)
                if (usedRandom + e.weight <= randomBudget && e.weight <= remaining)
                    fit.Add(e);
            if (fit.Count == 0) break;

            EnemyModal pick = fit[Random.Range(0, fit.Count)];
            result.Add(pick);
            usedRandom += pick.weight;
            remaining -= pick.weight;
        }

        // ----- PHA 2: LẤP ĐẦY — random trong nhóm khớp, overflow nhẹ -----
        while (remaining > 0)
        {
            fit.Clear();
            foreach (var e in candidates)
                if (e.weight - remaining <= maxOverflow)
                    fit.Add(e);
            if (fit.Count == 0) break;

            EnemyModal pick = fit[Random.Range(0, fit.Count)];
            result.Add(pick);
            remaining -= pick.weight;
        }

        return result;
    }
}
[System.Serializable]
public class EnemySpawnEntry
{
    public EnemyModal enemy;   // ref SO — id/weight/prefab lấy từ đây
    public int count;          // số con cùng loại spawn

    public EnemySpawnEntry(EnemyModal enemy)
    {
        this.enemy = enemy;
        this.count = 1;
    }
}