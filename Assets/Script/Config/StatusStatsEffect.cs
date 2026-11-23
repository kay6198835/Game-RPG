using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "newStatusStatsEffect", menuName = "Data/Config/Status Stats Effect")]
public class StatusStatsEffect : ScriptableObject
{
    public List<StatusStats> statusStatsEffects;
}
[Serializable]
public class StatusStats{
    public StatusType statusType;
    public List<StatType> statType;
}

// Serialized as int: never renumber a member once assets use it.
// Even value = buff, odd value = debuff (None = 0 is neither). See StatusTypeExtensions.IsBuff.
// Stat lists per status live in Assets/SO/Config/newStatusStatsEffect.asset.
// Kind: Duration = AdjustMaxForDuration, Tick = AdjustMaxPerTimeForDuration,
//       Instant = AdjustCurrentStats, Movement = IMovement (no stat list).
public enum StatusType
{
    None = 0,

    [Header("Buff (even)")]
    // ----- Buff: giá trị chẵn -----
    Haste = 2,          // Duration  MoveSpeed +, AttackSpeed +
    Might = 4,          // Duration  PhysicalDamage +
    ArcaneSurge = 6,    // Duration  MagicDamage +, ManaRegen +
    IronSkin = 8,       // Duration  Defense +
    EagleEye = 10,      // Duration  CritChance +, CritDamage +, MaxRangeAttack +
    Blur = 12,          // Duration  Evasion +, MoveSpeed +
    Regeneration = 14,  // Tick      HP + per tick
    ManaSpring = 16,    // Tick      Mana + per tick
    Heal = 18,          // Instant   current HP +
    ManaSurge = 20,     // Instant   current Mana +

    [Header("Debuff (odd)")]
    // ----- Debuff: giá trị lẻ -----
    Slow = 1,           // Duration  MoveSpeed -
    Weaken = 3,         // Duration  PhysicalDamage -
    ArmorBreak = 5,     // Duration  Defense -
    Exhaust = 7,        // Duration  AttackSpeed -
    GrievousWound = 9,  // Duration  HPRegen -100%
    Burn = 11,          // Tick      HP - per tick, HPRegen -
    Poison = 13,        // Tick      HP - per tick, PhysicalDamage -
    ManaBurn = 15,      // Instant   current Mana -
    Knockback = 17,     // Instant   IMovement.ApplyKnockback
    Stun = 19,          // Duration  IMovement.Lock (needs AI/state-machine block)
}

public static class StatusTypeExtensions
{
    public static bool IsBuff(this StatusType type) => type != StatusType.None && (int)type % 2 == 0;
}