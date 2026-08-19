using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatsProfile", menuName = "Game/Stats Profile")]
public class StatsSO : ScriptableObject
{
    [SerializeField, Min(1)] private int level = 1;
    [SerializeField, Min(1)] private int statUnusedBonus;
    [SerializeField] private List<Stat> stats = new List<Stat>();          // nguồn dữ liệu: sửa trực tiếp ở Inspector
    [SerializeField] DerivedStatFormula[] statFormulas;
    private readonly Dictionary<StatType, Stat> lookup = new Dictionary<StatType, Stat>(); // index runtime, KHÔNG serialize -> Get O(1)
    public readonly Dictionary<StatType, StatsViewDTO> statViewDTOs = new Dictionary<StatType, StatsViewDTO>();
    //[SerializeField] private List<Stat> stats = new List<Stat>(); 
    private bool initialized;

    /// <summary>Bắn ra mỗi khi một StatType đổi giá trị (UI subscribe để cập nhật).</summary>
    public event Action<StatType> OnStatChanged;
    [field: SerializeField] public bool isDevMode { get; private set; } = false;   // bật để debug khi stat dirty / recalc derived
    public int StatUnusedBonus
    {
        get => statUnusedBonus;
        set
        {
            int clamped = Mathf.Max(1, value);
            if (clamped == statUnusedBonus) return;
            statUnusedBonus = clamped;
        }
    }
    public int Level
    {
        get => level;
        set
        {
            int clamped = Mathf.Max(1, value);
            if (clamped == level) return;
            level = clamped;
            RecalculateDerived();
            CalculateStatUnusedBonus();
#if UNITY_EDITOR

#endif

        }
    }
    void OnEnable()
    {
        OnStatChanged += AfterChanged;
        for (int i = 0; i < stats.Count; i++)
        {
            stats[i]?.ClearModifiers();
        }
        initialized = false;
        EnsureInitialized();
    }

    void OnDisable()
    {
        OnStatChanged -= AfterChanged;
    }


    private void OnValidate()
    {
        for (int i = 0; i < stats.Count; i++)
        {
            stats[i]?.MarkDirty();
        }
        RecalculateDerived();
        CalculateStatUnusedBonus();
    }
    public void Reset()
    {
        stats.Clear();
        lookup.Clear();
        statViewDTOs.Clear();
        foreach (StatType t in Enum.GetValues(typeof(StatType)))
        {
            Stat s = new(t, 0f);
            stats.Add(s);
            lookup[t] = s;
        }
        initialized = true;
        RecalculateDerived();
    }
    // ------------------------- API chính -------------------------

    /// <summary>Đọc giá trị cuối cùng của một chỉ số. O(1).</summary>
    public Stat Get(StatType type)
    {
        EnsureInitialized();
        lookup.TryGetValue(type, out Stat stat);
        return stat;
    }

    /// <summary>Gắn một modifier (buff/trang bị) vào chỉ số mà nó nhắm tới.</summary>
    // public void AddModifier(StatModifier modifier)
    // {
    //     if (modifier == null) return;
    //     EnsureInitialized();
    //     GetOrCreate(modifier.TargetStat).AddModifier(modifier);
    //     OnStatChanged?.Invoke(modifier.TargetStat);
    // }

    /// <summary>
    /// Gắn nhiều modifier từ cùng một nguồn trong MỘT lần (trang bị, buff, thẻ nâng cấp).
    /// Đối xứng với RemoveModifiersFromSource — chỉ recalc derived một lần cho cả cụm,
    /// thay vì một lần cho mỗi modifier như khi gọi AddModifier lặp.
    /// Mỗi modifier được nhân bản rồi đóng dấu source, nên list truyền vào (asset dùng chung)
    /// không bị sửa. source phải là reference ổn định: lúc gỡ so khớp bằng ReferenceEquals.
    /// </summary>
    public void AddModifiersFromSource(object source, IReadOnlyList<StatModifier> modifiers)
    {
        if (modifiers == null) return;
        EnsureInitialized();

        bool primaryChanged = false;
        for (int i = 0; i < modifiers.Count; i++)
        {
            StatModifier authored = modifiers[i];
            if (authored == null) continue;

            Stat stat = GetOrCreate(authored.TargetStat);
            stat.AddModifier(authored.WithSource(source));
            OnStatChanged?.Invoke(stat.Type);
            if (stat.Type.IsPrimary()) primaryChanged = true;

        }
        if (primaryChanged) RecalculateDerived();
    }

    /// <summary>Gỡ mọi modifier đến từ một nguồn (tháo trang bị, hết buff).</summary>
    public void RemoveModifiersFromSource(object source)
    {
        EnsureInitialized();

        bool primaryChanged = false;
        for (int i = 0; i < stats.Count; i++)
        {
            Stat stat = stats[i];
            IReadOnlyList<StatModifier> mods = stat.Modifiers;

            bool removed = false;
            for (int j = mods.Count - 1; j >= 0; j--)   // duyệt ngược để RemoveAt không lệch index
            {
                if (!ReferenceEquals(mods[j].Source, source)) continue;
                stat.RemoveModifierAt(j);
                removed = true;
            }
            if (!removed) continue;

            OnStatChanged?.Invoke(stat.Type);
            if (stat.Type.IsPrimary()) primaryChanged = true;
        }
        if (primaryChanged) RecalculateDerived();
    }

    /// <summary>Cộng điểm gốc cho một primary stat (lên cấp / phân bổ điểm).</summary>
    public void AddPrimaryPoint(StatType type, float amount = 1f)
    {
        if (!type.IsPrimary())
        {
            Debug.LogWarning($"[StatsSO] {type} không phải primary stat.");
            return;
        }
        EnsureInitialized();
        GetOrCreate(type).LevelUpValue += amount;
        AfterChanged(type);
    }

    // ------------------------- Nội bộ -------------------------

    private void EnsureInitialized()
    {
        if (initialized) return;
        initialized = true;

        lookup.Clear();
        statViewDTOs.Clear();
        // Bỏ null / trùng key ngay trong list authored -> giữ bất biến list ↔ dict 1:1.
        for (int i = stats.Count - 1; i >= 0; i--)
        {
            Stat s = stats[i];
            if (s == null || lookup.ContainsKey(s.Type))
            {
                stats.RemoveAt(i);
                continue;
            }
            lookup[s.Type] = s;
            StatsViewDTO statViewDTO = new StatsViewDTO(s.Type);
            statViewDTO.Update(s.AdjustedValue, s.Value);
            statViewDTOs[s.Type] = statViewDTO;
        }

        // Bù các StatType còn thiếu trong enum, không thiếu chỉ số nào.
        foreach (StatType t in Enum.GetValues(typeof(StatType)))
            if (!lookup.ContainsKey(t))
            {
                Stat s = new Stat(t, 0f);
                stats.Add(s);
                lookup[t] = s;
            }

        RecalculateDerived();
    }

    private void CalculateStatUnusedBonus()
    {
        statUnusedBonus = 0;
        foreach (var (type, stat) in lookup)
        {
            if (type.IsPrimary())
            {
                statUnusedBonus += (int)stat.LevelUpValue;
            }
        }
        statUnusedBonus = level - statUnusedBonus - 1; // trừ 1 vì level 1 không có điểm bonus
        if (statUnusedBonus < 0) statUnusedBonus = 0;
    }

    private void AfterChanged(StatType type)
    {
        Stat stat = GetOrCreate(type);
        StatsViewDTO statViewDTO = GetOrCreateStatsViewDTO(stat.Type);
        statViewDTO.Update(stat.AdjustedValue, stat.Value);
    }

    public float GetStatValue(StatType type)
    {
        Stat stat = GetOrCreate(type);
        return stat.Value;
    }

    private void RecalculateDerived()
    {
        if (statFormulas == null) return;
        foreach (var formula in statFormulas)
        {
            if (formula == null) continue;

            Stat target = GetOrCreate(formula.targetStat);
            float newBase = formula.Evaluate(GetStatValue, level);
            if (!isDevMode && Mathf.Approximately(target.BaseValue, newBase)) continue;
            target.BaseValue = newBase;
            lookup[target.Type] = target;
            OnStatChanged?.Invoke(target.Type);
            Debug.Log($"[StatsSO] Recalc {target.Type}: {newBase} (level {level})");
        }
    }

    private Stat GetOrCreate(StatType type)
    {
        Stat stat = Get(type);
        if (stat == null)
        {
            stat = new Stat(type, 0f);
            stats.Add(stat);
            lookup[type] = stat;
        }
        return stat;
    }

    private StatsViewDTO GetOrCreateStatsViewDTO(StatType type)
    {
        statViewDTOs.TryGetValue(type, out StatsViewDTO statViewDTO);
        if (statViewDTO == null)
        {
            statViewDTO = new StatsViewDTO(type);
            statViewDTOs[type] = statViewDTO;
        }
        return statViewDTO;
    }


}
[System.Serializable]

public class StatsViewDTO
{
    public StatType StatType;
    public string Name;
    public float BonusValue;
    public float FinalValue;
    public float AdjustedValue;
    public StatsViewDTO(StatType statType)
    {
        StatType = statType;
        Name = GameConstants.StatTypeName[statType];
    }
    public void Update(float adjustedValue, float finalValue)
    {
        this.AdjustedValue = adjustedValue;
        this.FinalValue = finalValue;
        this.BonusValue = FinalValue - AdjustedValue;
    }
}
