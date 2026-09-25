using UnityEngine;

/// <summary>
/// Enemy weapon holder: the shared lifecycle and default-weapon equip of WeaponHolderBase, plus the AI's
/// attack cadence. An enemy without a weapon never attacks — there is no fallback attack path.
/// </summary>
public class EntityWeaponHolder : WeaponHolderBase<EntityCore>
{
    private float nextAttackTime;

    /// <summary>True when a weapon is equipped, can attack, and the last attack's recovery has elapsed.</summary>
    public bool CallAttack() => CanAttack() && Time.time >= nextAttackTime;

    /// <summary>Starts the recovery after an attack; its length is the played stage's AttackSO.attackRate.</summary>
    public void SetRecovery()
    {
        float recovery = weapon != null && weapon.CurrentStage != null ? weapon.CurrentStage.attackRate : 0f;
        nextAttackTime = Time.time + recovery;
    }
}
