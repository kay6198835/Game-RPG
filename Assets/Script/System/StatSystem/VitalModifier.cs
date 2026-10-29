using System;
using UnityEngine;

public class VitalModifier
{
    [SerializeField] private VitalStatType targetStatType;
    [SerializeField] private ModifierType type;
    [SerializeField] private float value;

    [NonSerialized] private object source;

    /// <summary>Chỉ số mà modifier này gắn vào.</summary>
    public VitalStatType TargetStatType => targetStatType;
    public ModifierType Type => type;
    public StatImpactType ImpactType { get => value >= 0 ? StatImpactType.Recovery : StatImpactType.Reduction; }
    public float Value => value;

    /// <summary>
    /// Khóa sắp xếp trong danh sách modifier của Stat. Luôn suy ra từ Type, không author được:
    /// Stat.CalculateFinalValue gom các PercentAdd LIÊN TIẾP dựa trên list đã sort theo Order,
    /// nên một Order tự đặt sẽ làm sai kết quả mà không báo lỗi.
    /// </summary>
    public int Order => (int)type;

    /// <summary>Nguồn tạo ra modifier (item, buff, passive...); StatsSO gỡ theo nguồn bằng ReferenceEquals.</summary>
    public object Source => source;

    public VitalModifier() { }   // Unity deserialization

    public VitalModifier(VitalStatType targetStat, float value, ModifierType type)
    {
        this.targetStatType = targetStat;
        this.value = value;
        this.type = type;
    }
}

public enum VitalStatType
{
    HP = StatType.HP,
    Mana = StatType.Mana,
    Coin = 1000
}