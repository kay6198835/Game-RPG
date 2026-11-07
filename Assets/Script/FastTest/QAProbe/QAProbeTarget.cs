using System.Collections.Generic;
using UnityEngine;

// Damage target for the automated player-flow probe. Records every raw hit so a case can assert the exact
// value a weapon or ability sent. Weapons and abilities resolve their receiver with TryGetComponent on the
// hit collider, so the Collider2D sits on this GameObject, on the Enemy layer.
[RequireComponent(typeof(Collider2D))]
public class QAProbeTarget : MonoBehaviour, INegativeReceiver
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

    public void TakeDamage(float amountDamage, Vector2 attackPosition) => _hits.Add(new Hit(Time.time, amountDamage));

    public void Clear() => _hits.Clear();

    public void MoveTo(Vector2 position)
    {
        transform.position = position;
        Physics2D.SyncTransforms();
    }
}
