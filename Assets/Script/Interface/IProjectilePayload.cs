using UnityEngine;

public interface IProjectilePayload
{
    /// <summary>
    /// Called once per valid hit, after the projectile has filtered by targetMask.
    /// power is the value snapshotted at launch; the implementer decides what it means.
    /// </summary>
    void OnHit(Collider2D target, Vector2 hitPos);
}