using UnityEngine;

/// <summary>
/// Movement of ONE character — player or enemy — as seen by systems that impact it (knockback,
/// slow/haste, stun/root). Get it with GetComponentInParent&lt;ICharacter&gt;() then
/// GetComponentInChildren&lt;IMovement&gt;(). Sources are the owning object (buff, ability, weapon
/// instance) and are removed by reference, like StatModifier sources.
/// </summary>
public interface IMovement
{
    Vector2 Velocity { get; }
    /// <summary>False while locked (stun/root) or while a knockback runs.</summary>
    bool CanMove { get; }
    /// <summary>Product of every active speed multiplier (1 = unchanged).</summary>
    float SpeedMultiplier { get; }
    void SetVelocity(Vector2 velocity);
    void Stop();
    void AddSpeedMultiplier(object source, float multiplier);
    void RemoveSpeedMultiplier(object source);
    void Lock(object source);
    void Unlock(object source);
    void ApplyKnockback(Vector2 velocity, float duration);
    /// <summary>Drops every multiplier, lock and knockback (e.g. a pooled enemy re-spawning).</summary>
    void ClearImpacts();
}
