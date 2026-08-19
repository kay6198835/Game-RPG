using UnityEngine;

/// <summary>
/// Công cụ test (debug tool) cho StatSystem: gắn lên một GameObject, kéo một StatsSO vào,
/// rồi dùng các nút trong Inspector (xem StatModifierTesterEditor) để thử tăng/giảm stat.
/// - LevelUp: mô phỏng lên cấp — +1 Level và +pointsPerLevel điểm vào 1 primary được chọn.
/// - AddModifierFromSource: tạo một StatModifier mới gắn vào StatsSO theo nguồn (source) chọn.
/// - AddGroupFromSource: gắn cả một cụm (StatModifierGroupSO) theo nguồn đang chọn.
/// - RemoveCurrentSource / RemoveAllTesterSources: gỡ modifier theo nguồn.
/// </summary>
public class StatModifierTester : MonoBehaviour
{
    // Source phải là reference ổn định giữa lúc Add và Remove (StatsSO so khớp bằng ReferenceEquals).
    // String literal được intern -> cùng literal luôn cùng reference trong process.
    private const string SOURCE_A = "StatTester_SourceA";
    private const string SOURCE_B = "StatTester_SourceB";
    private const string SOURCE_C = "StatTester_SourceC";

    public enum SourceId { A, B, C }

    [SerializeField] private StatsSO statsSO;

    [Header("Level Up (button 1)")]
    [SerializeField] private StatType primaryToAllocate = StatType.STR;
    [SerializeField] private float pointsPerLevel = 5f;

    [Header("Modifier (button 2 - Add)")]
    [SerializeField] private StatType modifierTarget = StatType.PhysicalDamage;
    [SerializeField] private ModifierType modifierType = ModifierType.Flat;
    [SerializeField] private float modifierValue = 10f;
    [SerializeField] private SourceId source = SourceId.A;

    [Header("Group (button 3 - Add bundle)")]
    [SerializeField] private StatModifierGroup modifierGroup;

    public StatsSO Stats => statsSO;
    public StatType PrimaryToAllocate { get => primaryToAllocate; set => primaryToAllocate = value; }

    private object CurrentSource => source switch
    {
        SourceId.A => SOURCE_A,
        SourceId.B => SOURCE_B,
        SourceId.C => SOURCE_C,
        _ => SOURCE_A,
    };

    /// <summary>Mô phỏng lên cấp: +1 Level (recalc derived) và +pointsPerLevel vào primary được chọn.</summary>
    public void LevelUp()
    {
        if (!Guard()) return;
        statsSO.Level += 1;
        statsSO.AddPrimaryPoint(primaryToAllocate, pointsPerLevel);
    }

    /// <summary>Tạo một StatModifier mới từ nguồn hiện tại và gắn vào StatsSO.</summary>
    public void AddModifierFromSource()
    {
        if (!Guard()) return;
        statsSO.AddModifier(new StatModifier(modifierTarget, modifierValue, modifierType, CurrentSource));
    }

    /// <summary>Gắn cả cụm modifier từ nguồn hiện tại — gỡ lại bằng nút Remove theo nguồn.</summary>
    public void AddGroupFromSource()
    {
        if (!Guard()) return;
        if (modifierGroup == null)
        {
            Debug.LogWarning("[StatModifierTester] modifierGroup chưa được gán.", this);
            return;
        }
        modifierGroup.ApplyTo(statsSO, CurrentSource);
    }

    /// <summary>Gỡ mọi modifier đến từ nguồn đang chọn.</summary>
    public void RemoveCurrentSource()
    {
        if (!Guard()) return;
        statsSO.RemoveModifiersFromSource(CurrentSource);
    }

    /// <summary>Gỡ mọi modifier đến từ script test này (cả 3 nguồn A/B/C).</summary>
    public void RemoveAllTesterSources()
    {
        if (!Guard()) return;
        statsSO.RemoveModifiersFromSource(SOURCE_A);
        statsSO.RemoveModifiersFromSource(SOURCE_B);
        statsSO.RemoveModifiersFromSource(SOURCE_C);
    }

    /// <summary>Đưa profile về base 0 (seed lại mọi StatType) để test lại từ đầu.</summary>
    public void ResetProfile()
    {
        if (!Guard()) return;
        statsSO.SeedAllStats();
    }

    private bool Guard()
    {
        if (statsSO != null) return true;
        Debug.LogWarning("[StatModifierTester] statsSO chưa được gán.", this);
        return false;
    }
}
