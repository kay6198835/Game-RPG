using UnityEngine;

/// <summary>
/// Công cụ test (debug tool) cho StatSystem: gắn lên một GameObject, kéo một BaseStatsSO vào,
/// rồi dùng các nút trong Inspector (xem StatModifierTesterEditor) để thử tăng/giảm stat.
/// Mọi thao tác chạy trên một StatSheet riêng của tester — asset không bao giờ bị sửa.
/// - LevelUp: +pointsPerLevel điểm vào 1 primary được chọn.
/// - AddModifierFromSource: tạo một StatModifier mới theo nguồn (source) chọn.
/// - AddGroupFromSource: gắn cả một cụm (StatModifierGroup) theo nguồn đang chọn.
/// - RemoveCurrentSource / RemoveAllTesterSources: gỡ modifier theo nguồn.
/// - ResetProfile: bỏ sheet, dựng lại từ asset.
/// </summary>
// NEED CHECK: 2026-11-09 (stat split) — used to call these methods on the BaseStatsSO asset itself.
public class StatModifierTester : MonoBehaviour
{
    // Source phải là reference ổn định giữa lúc Add và Remove (StatSheet so khớp bằng ReferenceEquals).
    // String literal được intern -> cùng literal luôn cùng reference trong process.
    private const string SOURCE_A = "StatTester_SourceA";
    private const string SOURCE_B = "StatTester_SourceB";
    private const string SOURCE_C = "StatTester_SourceC";

    public enum SourceId { A, B, C }

    [SerializeField] private BaseStatsSO statsSO;

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

    private StatSheet sheet;
    private BaseStatsSO sheetProfile;

    /// <summary>The tester's own sheet, rebuilt when the assigned asset changes; null without an asset.</summary>
    public StatSheet Sheet
    {
        get
        {
            if (statsSO == null) return null;
            if (sheet == null || sheetProfile != statsSO)
            {
                sheet = new StatSheet(statsSO);
                sheetProfile = statsSO;
            }
            return sheet;
        }
    }

    public StatType PrimaryToAllocate { get => primaryToAllocate; set => primaryToAllocate = value; }

    private object CurrentSource => source switch
    {
        SourceId.A => SOURCE_A,
        SourceId.B => SOURCE_B,
        SourceId.C => SOURCE_C,
        _ => SOURCE_A,
    };

    /// <summary>+pointsPerLevel điểm vào primary được chọn.</summary>
    public void LevelUp()
    {
        if (!Guard()) return;
        Sheet.AddPrimaryPoint(primaryToAllocate, pointsPerLevel);
    }

    /// <summary>Tạo một StatModifier mới từ nguồn hiện tại và gắn vào sheet.</summary>
    public void AddModifierFromSource()
    {
        if (!Guard()) return;
        Sheet.AddModifiersFromSource(CurrentSource, new[] { new StatModifier(modifierTarget, modifierValue, modifierType) });
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
        Sheet.AddModifiersFromSource(CurrentSource, modifierGroup.GetStatsModifiers);
    }

    /// <summary>Gỡ mọi modifier đến từ nguồn đang chọn.</summary>
    public void RemoveCurrentSource()
    {
        if (!Guard()) return;
        Sheet.RemoveModifiersFromSource(CurrentSource);
    }

    /// <summary>Gỡ mọi modifier đến từ script test này (cả 3 nguồn A/B/C).</summary>
    public void RemoveAllTesterSources()
    {
        if (!Guard()) return;
        Sheet.RemoveModifiersFromSource(SOURCE_A);
        Sheet.RemoveModifiersFromSource(SOURCE_B);
        Sheet.RemoveModifiersFromSource(SOURCE_C);
    }

    /// <summary>Bỏ sheet hiện tại, dựng lại từ asset (mất mọi điểm và modifier đã test).</summary>
    public void ResetProfile()
    {
        if (!Guard()) return;
        sheet = null;
    }

    private bool Guard()
    {
        if (statsSO != null) return true;
        Debug.LogWarning("[StatModifierTester] statsSO chưa được gán.", this);
        return false;
    }
}
