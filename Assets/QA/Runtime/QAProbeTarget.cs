#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;

namespace QA.Automation
{
    // Damage target for the QA suites (copied from Assets/Script/FastTest/QAProbe/QAProbeTarget.cs).
    // Records every raw hit so a case can assert the exact value a weapon or ability sent. Weapons and
    // abilities resolve their receiver with TryGetComponent on the hit collider, so the Collider2D sits
    // on this GameObject, on the Enemy layer.
    // Since b001d78e / 85899a52 (2026-11-25/26) ability hit damage is a DamageEffect that calls
    // IEffectReceiver.ApplyDamage on the hit collider (the spawned object still selects the collider by
    // INegativeReceiver), so the probe implements both, like the game's hurtboxes. Stat effects are ignored.
    [RequireComponent(typeof(Collider2D))]
    public class QAProbeTarget : MonoBehaviour, INegativeReceiver, IEffectReceiver
    {
        public readonly struct Hit
        {
            public readonly float Time;
            public readonly float Damage;

            public Hit(float time, float damage)
            {
                Time = time;
                Damage = damage;
            }
        }

        private readonly List<Hit> _hits = new();

        public IReadOnlyList<Hit> Hits => _hits;

        public void TakeDamage(float amountDamage, Vector2 attackPosition) => _hits.Add(new Hit(UnityEngine.Time.time, amountDamage));

        public void ApplyDamage(float amount, Vector2 attackPosition, AbilityContext context = null) => TakeDamage(amount, attackPosition);

        public void AdjustMaxStats(StatModifierGroup modifierGroup, AbilityContext context = null, Action callback = null) { }

        public void AdjustCurrentStats(StatModifierGroup modifierGroup, AbilityContext context = null, Action callback = null) { }

        public void ApplyStatus(StatusRequest request) { }

        public void AdjustMaxForDuration(StatModifierGroup modifierGroup, float duration, AbilityContext context = null, Action callback = null) { }

        public void AdjustMaxPerTimeForDuration(StatModifierGroup modifierGroup, float perTime, float duration, AbilityContext context = null, Action callback = null) { }

        public void Clear() => _hits.Clear();

        public void MoveTo(Vector2 position)
        {
            transform.position = position;
            Physics2D.SyncTransforms();
        }
    }
}
#endif
