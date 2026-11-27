using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
public class EffectReceiverBase<TCore> : CoreComponentBase<TCore>, IEffectReceiver where TCore : CoreBase
{
    protected IVitalComponent<TCore> vital;
    protected IStatService statHandler;
    protected ICharacterInput input;
    [SerializeField] protected SourceEffect sourceEffect;
    protected IObjectPoolService _objectPoolManager;
    [Inject]
    public void Construct(IObjectPoolService objectPoolManager)
    {
        _objectPoolManager = objectPoolManager;
    }
    public override void Setup()
    {
        base.Setup();
        Core.TryGetCapability(out vital);
        Core.TryGetCapability(out statHandler);
        Core.TryGetCapability(out input);
    }

    GameObject SetEffectSource(AbilityContext context, float lifetime, Action callback)
    {
        Vector2 position = context != null ? context.Caster.Transform.position : Vector2.zero;
        GameObject instance = _objectPoolManager.Spawn(sourceEffect.gameObject, position, Quaternion.identity);
        instance.GetComponent<SourceEffect>().SetSourceEffect(new SourceEffectConfig
        {
            callback = callback,
            lifetime = lifetime
        });
        return instance;
    }
    public override void ReloadComponent()
    {
        base.ReloadComponent();
        ClearAllStatuses();
    }

    // NEED CHECK: 2026-11-28 (B6 / AB-E20) — a pooled character is disabled on despawn; Update stops there,
    // so its statuses would never expire and their modifiers would leak into the next spawn.
    protected virtual void OnDisable()
    {
        ClearAllStatuses();
    }

    // NEED CHECK: new 2026-11-25 — ability hit damage enters here (DamageEffect) and is forwarded to the
    // hurtbox's damage pipeline, so mitigation, the dead check and the TakeDamage reaction stay in one place
    // (NegativeReceiverBase.TakeDamage). Resolved lazily: Awake order between core components is not fixed.
    protected INegativeReceiver negativeReceiver;
    public void ApplyDamage(float amount, Vector2 attackPosition, AbilityContext context = null)
    {
        if (negativeReceiver == null && !Core.TryGetCapability(out negativeReceiver))
        {
            Debug.LogWarning($"[{GetType().Name}] {name}: no INegativeReceiver on this core, damage ignored.", this);
            return;
        }
        negativeReceiver.TakeDamage(amount, attackPosition);
    }
    // NEED CHECK: 2026-11-26 — a dead character takes no new stat / vital effect (GDD AB-E19). Between HP 0 and
    // the despawn (death animation frame) the hurtbox is still a trigger, so late hits still reach this receiver.
    // Hit damage is already refused by NegativeReceiverBase's own dead check.
    protected bool IsDead => input != null && input.IsDeath;

    public void AdjustCurrentStats(StatModifierGroup statModifierGroup, AbilityContext context, Action callback = null)
    {
        if (IsDead) return;
        foreach (var modifier in statModifierGroup.GetStatsModifiers)
        {
            var vitalStatType = (VitalStatType)modifier.TargetStatType;
            if (!Enum.IsDefined(typeof(VitalStatType), vitalStatType)) continue;
            var adjustValue = Utility.ModifierStatsCalculate(modifier, vital.GetCurrentStatValue(vitalStatType));
            if (modifier.ImpactType == StatImpactType.Reduction)
            {
                vital.Reduction(vitalStatType, Math.Abs(adjustValue));
            }
            else if (modifier.ImpactType == StatImpactType.Recovery)
            {
                vital.Recovery(vitalStatType, Math.Abs(adjustValue));
            }
        }
    }

    public void AdjustMaxStats(StatModifierGroup statModifierGroup, AbilityContext context = null, Action callback = null)
    {
        if (IsDead) return;
        statHandler.AddModifiersFromSource(SetEffectSource(context, 0f, callback), statModifierGroup.GetStatsModifiers);
    }
    #region Reduction/Recovery per timer for duration
    // BUG-071: the iterators below are built but never started. Moved here unchanged.
    public void ReductionPerTimeForDuration(StatModifierGroup statModifierGroup, float perTime, float timeCount, AbilityContext context, Action action = null)
    {
        StartCoroutine(ReductionPerTime(statModifierGroup, perTime, timeCount, context));
    }

    IEnumerator ReductionPerTime(StatModifierGroup statModifierGroup, float perTime, float timeCount, AbilityContext context)
    {
        if (input == null || input.IsDeath)
        {
            yield break;
        }
        AdjustCurrentStats(statModifierGroup, context);
        timeCount--;
        if (timeCount > 0)
        {
            yield return new WaitForSeconds(perTime);
            ReductionPerTimeForDuration(statModifierGroup, perTime, timeCount, context);
        }
    }
    #endregion

    #region Status registry (B6, GDD OQ-4)
    // NEED CHECK: new 2026-11-28 (test branch) — replaces the coroutine + pooled SourceEffect per call.
    // One list, one Update. The ActiveStatus itself is the modifier source, so removing it never touches
    // another source (weapon, item, another status). SourceEffect stays for AdjustMaxStats only (Q5).
    private readonly List<ActiveStatus> _statuses = new();

    public void ApplyStatus(StatusRequest request)
    {
        // 1. Refuse
        if (IsDead || request.Duration <= 0f) return;
        if (request.MaxModifiers == null && request.TickModifiers == null) return;

        // 2. Key: (AbilityInstance, asset). No instance → a new object, which never matches.
        object source = (object)request.Context?.AbilityInstance ?? new object();
        float now = Time.time;

        // 3. Same key → refresh the time only
        for (int i = 0; i < _statuses.Count; i++)
        {
            ActiveStatus s = _statuses[i];
            if (s.Source == source && s.Effect == request.Effect)
            {
                s.EndTime = now + s.Duration;
                return;
            }
        }

        // 4. New status
        var status = new ActiveStatus
        {
            Source = source,
            Effect = request.Effect,
            Type = request.StatusType,
            MaxModifiers = request.MaxModifiers,
            TickModifiers = request.TickModifiers,
            Duration = request.Duration,
            TickInterval = request.TickInterval,
            Context = request.Context,
            EndTime = now + request.Duration,
            NextTick = now,                     // first tick on the next Update
        };
        if (status.MaxModifiers != null) statHandler?.AddModifiersFromSource(status, status.MaxModifiers.GetStatsModifiers);
        _statuses.Add(status);
    }

    protected virtual void Update()
    {
        if (_statuses.Count == 0) return;
        float now = Time.time;
        for (int i = _statuses.Count - 1; i >= 0; i--)
        {
            ActiveStatus s = _statuses[i];

            // Tick (at most once a frame; a tick at or after EndTime is not run)
            if (s.HasTick && now >= s.NextTick && s.NextTick < s.EndTime)
            {
                AdjustCurrentStats(s.TickModifiers, s.Context);   // refuses a dead receiver itself
                s.NextTick += s.TickInterval;
                // The tick may have killed and despawned this receiver: OnDisable cleared the list.
                if (i >= _statuses.Count || _statuses[i] != s) continue;
            }

            // Expire
            if (now >= s.EndTime) RemoveStatusAt(i);
        }
    }

    private void RemoveStatusAt(int index)
    {
        ActiveStatus s = _statuses[index];
        if (s.MaxModifiers != null) statHandler?.RemoveModifiersFromSource(s);
        _statuses.RemoveAt(index);
    }

    /// <summary>Ends every status now, each removed by its own source (never ClearModifiers, which would
    /// also drop the weapon's). Called on despawn (OnDisable) and respawn (ReloadComponent).</summary>
    public void ClearAllStatuses()
    {
        for (int i = _statuses.Count - 1; i >= 0; i--) RemoveStatusAt(i);
    }

    // Legacy entry points, kept so existing callers (QA cases) still compile and behave as before:
    // no effect asset and no key → an independent status every call. Duration is in seconds.
    public void AdjustMaxForDuration(StatModifierGroup modifierGroup, float duration, AbilityContext context, Action callback = null)
    {
        ApplyStatus(new StatusRequest { MaxModifiers = modifierGroup, Duration = duration });
    }

    public void AdjustMaxPerTimeForDuration(StatModifierGroup modifierGroup, float perTime, float duration, AbilityContext context, Action callback = null)
    {
        ApplyStatus(new StatusRequest { TickModifiers = modifierGroup, TickInterval = perTime, Duration = duration });
    }
    #endregion
}