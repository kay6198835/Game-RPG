// ---------------- RoomModel.cs ----------------
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomModel", menuName = "Game/Room Model")]
public class RoomModel : EntityModel
{
    [SerializeField] private List<EnemyModal> enemiesOfRoom = new List<EnemyModal>();
    [SerializeField, Range(0, 500)] private int weightBudget;
    [SerializeField, Range(0f, 1f)] private float randomRatio = 0.33f;
    [SerializeField, Range(0f, 1f)] private float overflowPercent = 0.1f;

    // scratch — reuse giữa các call. Index đi song song enemiesOfRoom
    // (ổn định vì list clean + không trùng).
    private EnemySpawnEntry[] _entries;
    private int[] _fitBuf;   // buffer chứa index nhóm fit mỗi vòng pick

    /// <summary>
    /// Kết quả gửi spawn system: mỗi loại quái + số lượng.
    /// Tổng Σ(count × weight) ≤ (1 + overflowPercent) × weightBudget.
    /// </summary>
    public List<EnemySpawnEntry> GetSpawnSet()
    {
        int n = enemiesOfRoom.Count;
        if (n == 0) return null;

        // cấp/mở rộng scratch nếu số loại đổi
        if (_entries == null || _entries.Length != n)
        {
            _entries = new EnemySpawnEntry[n];
            _fitBuf  = new int[n];
        }
        System.Array.Clear(_entries, 0, n);   // reset dedup mỗi call

        var outList = new List<EnemySpawnEntry>();
        int randomBudget = Mathf.RoundToInt(weightBudget * randomRatio);
        int maxOverflow  = Mathf.RoundToInt(weightBudget * overflowPercent);
        int remaining    = weightBudget;

        // ----- PHA 1: RANDOM — pick tự do trong nhóm fit, cho trùng loại -----
        int usedRandom = 0;
        while (true)
        {
            int threshold = randomBudget - usedRandom;
            int fitCount = 0;
            for (int i = 0; i < n; i++)
                if (enemiesOfRoom[i].weight <= threshold)
                    _fitBuf[fitCount++] = i;
            if (fitCount == 0) break;

            int idx = _fitBuf[Random.Range(0, fitCount)];
            Emit(idx, outList);
            usedRandom += enemiesOfRoom[idx].weight;
            remaining  -= enemiesOfRoom[idx].weight;
        }

        // ----- PHA 2: LẤP ĐẦY — random trong nhóm khớp, overflow nhẹ -----
        while (remaining > 0)
        {
            int threshold = remaining + maxOverflow;
            int fitCount = 0;
            for (int i = 0; i < n; i++)
                if (enemiesOfRoom[i].weight <= threshold)
                    _fitBuf[fitCount++] = i;
            if (fitCount == 0) break;

            int idx = _fitBuf[Random.Range(0, fitCount)];
            Emit(idx, outList);
            remaining -= enemiesOfRoom[idx].weight;
        }

        return outList;
    }

    // convert ngay tại bước pick: chưa có thì tạo + add, có rồi thì tăng count
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

#if UNITY_EDITOR
    // Lưới an toàn author-time: "clean" giờ là hợp đồng ngoài code.
    // Cảnh báo ngay khi designer nhập sai trong Inspector.
    protected override void OnValidate()
    {
        base.OnValidate();   // giữ gen id ở base
        for (int i = 0; i < enemiesOfRoom.Count; i++)
        {
            var e = enemiesOfRoom[i];
            if (e == null)
                Debug.LogWarning($"[RoomModel] '{name}': enemiesOfRoom[{i}] null.", this);
            else if (e.weight <= 0)
                Debug.LogWarning($"[RoomModel] '{name}': '{e.name}' weight <= 0 → Pha 2 loop vô hạn.", this);
        }
    }
#endif
}[System.Serializable]
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