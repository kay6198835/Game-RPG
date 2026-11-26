using System.Collections.Generic;
using UnityEngine;
// Payload (GDD AB-E15): acts on one recipient, has no SubEffects.
public abstract class StatsEffectBase : PayloadEffectBase
{
    [field: SerializeField] protected StatusType statusType;
    [field: SerializeField] protected StatusStatsEffect statusStatsEffect;
    [field: SerializeField] protected StatModifierGroup statModifier;

    // The statusType that statModifiers was last rebuilt for. Lets OnValidate tell
    // "statusType changed" apart from "any other field changed", so editing a
    // modifier's value does not get wiped by the next validate.
    [SerializeField, HideInInspector] private StatusType syncedStatusType;

    public override void OnValidate()
    {
        base.OnValidate();
#if UNITY_EDITOR
        AutoAssignStatusStatsEffect();
        if (statusType != syncedStatusType)
        {
            SyncModifiersFromStatusType();
            syncedStatusType = statusType;
        }
#endif
    }

#if UNITY_EDITOR
    // Called by Unity when the asset is first created or "Reset" from the context menu.
    private void Reset() => AutoAssignStatusStatsEffect();

    // Finds the single StatusStatsEffect config asset anywhere in the project and
    // stores it in the serialized field, so the ref survives into builds without Resources.
    private void AutoAssignStatusStatsEffect()
    {
        if (statusStatsEffect != null) return;

        string[] guids = UnityEditor.AssetDatabase.FindAssets("t:" + nameof(StatusStatsEffect));
        if (guids.Length == 0)
        {
            Debug.LogWarning($"[{name}] No {nameof(StatusStatsEffect)} asset found in project.", this);
            return;
        }
        if (guids.Length > 1)
        {
            Debug.LogWarning($"[{name}] {guids.Length} {nameof(StatusStatsEffect)} assets found; using the first.", this);
        }

        string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
        statusStatsEffect = UnityEditor.AssetDatabase.LoadAssetAtPath<StatusStatsEffect>(path);
        UnityEditor.EditorUtility.SetDirty(this);
    }

    // Rebuilds statModifiers to match the StatType list configured for statusType.
    // A modifier whose stat is still in the new list keeps its authored value/type.
    private void SyncModifiersFromStatusType()
    {
        if (statusType == StatusType.None)
        {
            statModifier = new StatModifierGroup(new List<StatModifier>());
            UnityEditor.EditorUtility.SetDirty(this);
            return;
        }
        if (statusStatsEffect == null || statusStatsEffect.statusStatsEffects == null) return;

        StatusStats entry = statusStatsEffect.statusStatsEffects.Find(s => s.statusType == statusType);
        if (entry == null || entry.statType == null)
        {
            Debug.LogWarning($"[{name}] {statusType} has no entry in {statusStatsEffect.name}.", this);
            statModifier = new StatModifierGroup(new List<StatModifier>());
            UnityEditor.EditorUtility.SetDirty(this);
            return;
        }

        // Read the current authored list so a stat that stays in the new set keeps its value/type.
        IReadOnlyList<StatModifier> current = statModifier?.GetStatsModifiers;
        StatImpactType impact = statusType.IsBuff() ? StatImpactType.Recovery : StatImpactType.Reduction;

        var rebuilt = new List<StatModifier>(entry.statType.Count);
        foreach (StatType stat in entry.statType)
        {
            StatModifier existing = null;
            if (current != null)
            {
                foreach (StatModifier m in current)
                {
                    if (m != null && m.TargetStatType == stat) { existing = m; break; }
                }
            }
            // Keep the authored value/type but re-stamp the impact: buff vs debuff can flip with statusType.
            rebuilt.Add(existing != null
                ? new StatModifier(stat, existing.Value, existing.Type, impact)
                : new StatModifier(stat, 0f, ModifierType.Flat, impact));
        }

        statModifier = new StatModifierGroup(rebuilt);
        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif


    public override void Apply(AbilityContext context)
    {
        // Same path for a player or an enemy: only IVitalComponent is touched.
        if (!context.TryGetRecipientComponent(Recipient, out IEffectReceiver effectReceiver)) return;
        object source = Recipient == EffectRecipient.Target ? (object)context.Target : context.Caster;
        ApplyImpact(effectReceiver, statModifier, context);
    }

    protected abstract void ApplyImpact(IEffectReceiver effectReceiver, StatModifierGroup statModifierGroup, AbilityContext context);
}
