# Status Debuffs Built from Stat Reductions (Design Note)

> Status: idea note, 2026-11-23. Not a GDD. Section 3 (kit mapping) is a proposal and is not in any GDD.
> Config: `Assets/SO/Config/newStatusStatsEffect.asset` (`StatusStatsEffect`) maps each `StatusType`
> to the `StatType` list it reduces. Values are authored per ability on `StatsEffectBase.statModifiers`.

Stats come from `StatType.cs`:
- Primary: STR, DEX, INT, VIT, LUK
- Derived: HP, Mana, PhysicalDamage, MagicDamage, Defense, AttackSpeed, CritChance, CritDamage,
  MoveSpeed, HPRegen, ManaRegen, Evasion, MaxRangeAttack, MinRangeAttack

## 1. Single-stat debuffs (building blocks)

| Debuff | Reduced stat | Feel |
|---|---|---|
| Slow | MoveSpeed -20..40% | Moves slower |
| Root (approx.) | MoveSpeed -100% | Cannot move, can still attack |
| Weaken | PhysicalDamage -% | Weaker hits |
| Mind Fog (soft silence) | MagicDamage -% | Weaker spells |
| Armor Break / Sunder | Defense -% | Takes more damage |
| Exhaust | AttackSpeed -% | Attacks slower |
| Blind | CritChance -, MaxRangeAttack - | Misses, must close in |
| Clumsy | Evasion -% | Easier to hit |
| Grievous Wound | HPRegen -100% | No healing |
| Mana Drain | ManaRegen -% | Slow mana recovery |

## 2. Multi-stat debuffs

| Debuff | Combined reduction | Idea / fitting class |
|---|---|---|
| Chill | MoveSpeed -40% + AttackSpeed -20% | Follow-up after Frost Nova's Freeze |
| Freeze (stat-faked) | MoveSpeed -100% + AttackSpeed -100% + Evasion -100% | Stands still, no attack, no dodge. Needs an animation-stop flag |
| Stun (stat-faked) | Same as Freeze but short (0.2-1s) | Chain Lightning, Meteor |
| Burn | HPRegen -100% + DOT | Fire stops wounds from closing (Fireball) |
| Poison | Negative HPRegen (HP loss over time) + PhysicalDamage -10% | Extra option for Archer |
| Frostbite | Defense -15% + MoveSpeed -25% | Ice makes armor brittle, pairs with Shatter |
| Shock | Evasion -50% + CritChance -% | Paralysis, poor reflexes |
| Crippled | MoveSpeed -30% + MaxRangeAttack -30% | Ranged enemies must close in |
| Terror / Fear | MinRangeAttack up (value increases) + PhysicalDamage -% | AI backs off and keeps distance. Fits Paladin (light) |
| Exposed | Defense -30% + Evasion -30% | Ultimate setup |
| Curse | LUK - (cascades into crit, evasion...) | Debuff chains through derived formulas |
| Enfeeble | STR - and INT - | Cuts physical and magic damage at once |
| Fatigue | VIT - (cascades into HP, HPRegen) + AttackSpeed - | Long-term, stacking debuff |
| Manaburn | Current Mana -, ManaRegen -50% | Counters casters (RangedCaster) |

## 3. Kit mapping (proposal, not in GDD)

- Archer: Volley uses Slow. Kick-back Trap uses Root (MoveSpeed -100%) plus knockback. Rain Arrows
  uses Slow 20%. Could add Crippled or Exposed to Hawk Eye on a full charge.
- Mage: Fireball uses Burn. Frost Nova uses Freeze, then Chill. Could add Frostbite. Chain Lightning
  uses Shock or Stun 0.2s. Meteor uses Stun.
- Paladin: Blessed Slash already reduces recovery, matching Grievous Wound. Could add Terror or Exposed.

## Notes

- Reducing primary stats (STR/DEX/INT/VIT/LUK) spreads to every derived stat through
  `DerivedStatFormula`. The result is harder to predict, so it suits long-term debuffs such as Curse
  or Fatigue. Short tactical debuffs should reduce derived stats directly.
- Real Stun, Freeze and Silence cannot be built from stats alone. With AttackSpeed = 0 an enemy can
  still cast an ability. These need a status flag that blocks the state machine or AI. Use stats for
  the "soft" part (slow, weaken) and `StatusType` for the "hard" part (cannot act).
- Percent reductions are only safe when the stat system clamps values (MoveSpeed >= 0). Stacking when
  the same debuff is reapplied is still open question OQ-4. Until it is answered, default to
  "reapply refreshes duration, does not stack strength", matching the GDD rule.
- BUG-100 (timed buffs share one modifier source) and BUG-118 (tick effects are NotImplemented) must
  be fixed before timed debuffs or DOTs such as Burn and Poison can work.
- Manaburn's "current Mana -" is a vital change, not a stat modifier: `StatType.Mana` is max Mana.
