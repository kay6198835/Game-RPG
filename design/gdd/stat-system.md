---
status: authored
source: Assets/Script/StatSystem/, ToolExcel/stat_system_formula_reference.xlsx
date: 2026-07-07
revised: 2026-08-20 (documentation audit — recorded the StatModifierGroupSO -> StatModifierGroup
drift as an open owner decision, and two implementation defects that falsify the Modifiers
section and AC-1. Design intent unchanged.)
verified-by: Kiet
---

# Stat System Design

**Status**: In Design

> **Detailed numbers live in the spreadsheet, not here.** The concrete per-entity
> coefficients, base constants, and `perLevel` values for every stat are maintained in
> **`ToolExcel/stat_system_formula_reference.xlsx`** (Player + 5 enemy archetypes + Boss).
> This GDD describes the system in general terms and defers all per-entity detail to that
> file so there is a single source of truth for the numbers.

---

## Overview

The stat system turns a small set of **primary stats** (STR, DEX, INT, VIT, LUK) into the
full set of **derived stats** every character uses in combat (health, damage, defense,
attack speed, crit, regen, etc.). Every character — the player and all enemies — shares the
same data-driven formula shape; only the coefficients differ per entity. Values are authored
in ScriptableObjects and read at runtime through `StatsSO`.

The system is **level-aware**: each derived stat may grow with the character's level through a
dedicated `perLevel` term, on top of its primary-stat contribution.

---

## Player Fantasy

Progression feels tangible: leveling up and investing primary points visibly raises the
numbers that matter, while build choices (dumping STR vs VIT vs LUK) create distinct play
styles. Enemies of different archetypes feel mechanically different — a tank soaks hits, a
swarm dies fast but overwhelms — because the same formula produces different shapes from
different coefficients.

---

## Detailed Rules

- **Primary stats**: STR, DEX, INT, VIT, LUK. Authored per entity; the player also gains
  points on level-up to allocate.
- **Derived stats**: computed from primary stats via a per-entity formula. They are never
  authored directly — always recalculated when a primary stat or level changes.
- **Stat groups** (governs whether `perLevel` is used):
  - *Flat / resource* (MaxHP, MaxMana, PhysicalDamage, MagicDamage, Defense, MagicDefense,
    HPRegen, ManaRegen) — **may** use `perLevel` for guaranteed vertical growth.
  - *Percentage / soft-capped* (AttackSpeed, CritChance, CritDamage, MoveSpeed, Evasion,
    LifeSteal) — **`perLevel = 0`**; flat per-level growth here compounds degenerately.
  - *Fixed* (CrowdControlResist, AttackRange) — constant, not driven by the formula.
- **Enemy variety** is layered on top of the archetype formula by a **rank** tier
  (creep / elite / champion) and a separate **boss** definition. Rank and boss scaling
  parameters are maintained in the spreadsheet and the combat balance doc.
- **Modifiers** `[IMPLEMENTED]`: buffs, equipment, and effects attach as `StatModifier`s on top of
  the computed base value; they never mutate the base. A bundle of modifiers (one piece of
  equipment, one buff, one upgrade card) is authored as a `StatModifierGroupSO` asset and attached
  or detached as a unit, keyed by **source**:
  - `StatsSO.AddModifiersFromSource(source, modifiers)` attaches the whole bundle;
    `StatsSO.RemoveModifiersFromSource(source)` detaches everything from that source.
  - `source` is matched by reference identity, so it must be the **owning instance** (the equipped
    item instance, MonoBehaviour, or ability instance) — never a value type and never the shared
    group asset, or two copies of the same asset cannot be detached independently.
  - Bulk operations recalculate derived stats once for the whole bundle, not once per modifier.
  - `Stat` itself only ever adds or removes a single modifier; all iteration lives in `StatsSO`.

> ⚠️ **Implementation defect (audit 2026-08-20, re-verified 2026-08-21 — TD-038).**
> "They never mutate the base" holds, but **runtime modifiers are being persisted**:
> `Stat.modifiers` is `[SerializeField]` although its own doc-comment and ADR-0001 both
> require `[NonSerialized]`. Two leaked `STR +1 Flat` modifiers were committed into
> `Assets/SO/Stat/PlayerStats.asset` and `Test.asset`; they were **cleaned on `sprint-10`**,
> but the attribute is unchanged so the leak will recur. `StatsSO.OnEnable()` calls
> `ClearModifiers()` on every stat at load, which hides the churn but also means any
> modifier legitimately authored in the Inspector is silently wiped — the two behaviours
> are mutually exclusive and the code currently chooses "wipe everything".
>
> Note the distinction: `StatModifierGroup.modifiers` (the bundle on `WeaponStats`) is
> **correctly** serialized and must stay that way — `SnS_Stat.asset` authors real data there.
> Only `Stat.modifiers`, the per-stat runtime list, should be `[NonSerialized]`.
>
> ✅ **Resolved on `sprint-10`:** `StatsSO.RecalculateDerived()` previously skipped its update
> when *any one* of four values matched (`||` where `&&` was needed), so derived stats stopped
> updating unless `isDevMode` was on. The guard now ANDs all comparisons, and
> `AddPrimaryPoint()` calls `RecalculateDerived()` directly.

---

## Formulas

The general formula shape (identical for every entity; coefficients differ per entity):

```
derivedStat = baseConstant + level × perLevel + Σ(primaryStat × coefficient)
```

- `baseConstant` — the stat's floor at level 0 with no primary contribution.
- `perLevel` — flat growth per level (0 for percentage/fixed stats).
- `coefficient` — how much each contributing primary stat adds.
- Percentage stats additionally apply a `clamp(value, min, max)`.

> **The per-entity `baseConstant`, `perLevel`, and coefficients for all 16 derived stats,
> for the player and every enemy archetype + boss, are stored in
> `ToolExcel/stat_system_formula_reference.xlsx`.** The TTK-balanced level 1→20 curve and the
> creep/elite/champion + boss multipliers are in `design/balance/combat-balance-2026-07-07.md`
> and the live emulator `design/balance/stat_system_leveled_v2.xlsx`.

This formula is implemented once in code by
[`DerivedStatFormula.Evaluate()`](../../Assets/Script/StatSystem/DerivedStatFormula.cs); the
GDD/Excel numbers map 1:1 onto its `baseConstant`, `perLevel`, and `contributions` fields.

---

## Edge Cases

| Scenario | Expected behaviour |
|----------|--------------------|
| Percentage stat given a `perLevel > 0` | Disallowed by design — keep `perLevel = 0` (see Detailed Rules) |
| Derived formula references another derived stat | Not allowed — contributions reference **primary** stats only, to avoid circular dependency |
| Level set below 1 | Clamped to 1 (`StatsSO.Level` setter) |
| Missing `StatType` in the authored list | Backfilled to 0 on load (`EnsureInitialized`) |
| Duplicate / null stat entries | Dropped on load, keeping the List ↔ Dictionary 1:1 invariant |

---

## Dependencies

| System | Relationship |
|--------|-------------|
| Damage & Health | Consumes MaxHP / Defense / damage stats; damage application should apply Defense (currently `finalDamage = rawDamage` — see combat balance doc) |
| Character / Enemy AI | Each entity owns a `StatsSO`; enemy `Level` is intended to track dungeon floor |
| Per-Run Upgrades | Upgrade cards add primary points / modifiers through the `StatsSO` API |
| HUD | Subscribes to `StatsSO.OnStatChanged` to display health/mana/etc. |

Storage architecture (List + runtime Dictionary) is recorded in
[ADR-0001](../../docs/architecture/adr-0001-statsystem-dual-data-structure.md).

---

## Tuning Knobs

All concrete values are **edited in `ToolExcel/stat_system_formula_reference.xlsx`** and then
mirrored into the ScriptableObject assets. The knobs are:

| Knob | Meaning |
|------|---------|
| `baseConstant` (per stat, per entity) | stat floor |
| `perLevel` (per flat stat, per entity) | vertical growth per level |
| primary `coefficient` (per stat, per entity) | build/identity weighting |
| primary stat allocation | player build direction |
| rank multipliers (creep/elite/champion) | enemy power tier — see balance doc |
| boss formula | boss stat curve — see balance doc |

---

## Acceptance Criteria

- [x] Every derived stat recalculates when a primary stat or the level changes — fixed on
      `sprint-10`; `AddPrimaryPoint()` now also calls `RecalculateDerived()` directly.
- [ ] Percentage/fixed stats have `perLevel = 0`.
- [ ] Derived formulas reference only primary stats (no derived-on-derived).
- [ ] The coefficients in the SO assets match `ToolExcel/stat_system_formula_reference.xlsx`.
- [ ] `StatsSO.Get(StatType)` returns the authored value for every stat at O(1).
