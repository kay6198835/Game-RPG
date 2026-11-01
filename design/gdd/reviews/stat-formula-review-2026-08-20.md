---
status: review
source: ToolExcel/stat_system_formula_reference.xlsx, Assets/Script/StatSystem/
date: 2026-08-20
scope: Primary/derived stat formulas + StatSystem implementation
verdict: CONCERNS — 2 blockers, 5 high, 3 medium
---

# Stat Formula Review — Player Primary / Derived

Companion artifact: **`ToolExcel/stat_system_player_demo.xlsx`** — a live calculator that takes
the sample primary stats and level as input and derives all 16 stats automatically, plus baked
demo tables (Lv1→20, build comparison, four-layer breakdown, TTK check).

---

## 1. What was checked

| Input | Source |
|---|---|
| Formula shape | `DerivedStatFormula.Evaluate()` |
| Value layering | `Stat.cs` (BaseValue / LevelUpValue / EquipmentValue / EquipmentByPrimaryValue) |
| Recalculation | `StatsSO.RecalculateDerived()`, `OnValidate()`, `Level` setter |
| Coefficients | `stat_system_formula_reference.xlsx` — 7 entities × 16 stats |
| Point budget | `StatsSO.CalculateStatUnusedBonus()` → 1 point per level |
| Mitigation model | `design/gdd/character-system.md:116` → `finalDamage = rawDamage - target.entityArmor` |

The uploaded workbook is **numerically identical** to the copy already in the repository. A full
cell-by-cell diff of both files returns exactly one difference: the title text moved from cells
`A1`/`A2` to `B1`/`B2`. All 7 entities × 16 stats, every `base`, `perLevel`, and primary
coefficient are unchanged. Every finding below therefore applies to the current number set.

The formula shape itself is sound. `baseConstant + Level × perLevel + Σ(primary × coefficient)`
is linear, cheap, fully data-driven, and maps 1:1 onto `DerivedStatFormula`. Restricting
contributions to primary stats only (no derived→derived references) correctly rules out
circular dependencies. The problems are in the **numbers** and in the **implementation**, not
in the model.

---

## 2. Blockers

### B1 — Flat defense subtraction breaks both ends of the matchup table

Under the planned `finalDamage = rawDamage - defense`, the chosen coefficients produce
impossible matchups at every level from 1 to 20:

| Matchup | Lv1 | Lv10 | Lv20 |
|---|---:|---:|---:|
| Player → Boss | −52.3 | −44.5 | −36.5 |
| Player → Tank | −12.2 | −4.4 | +3.6 |
| Fast Swarm → Player | −2.8 | −5.9 | −8.9 |
| Tank → Player | −0.6 | −2.8 | −4.8 |

A negative number means zero damage. The player **can never damage the boss**, and Swarm and
Tank enemies **can never damage the player**, at any level in the demo range. This is not a
tuning gap that specialisation fixes — a Lv20 all-STR build (STR 29) reaches 95.8 physical
damage against the boss's 102.5 defense and still deals nothing.

The root cause is structural: subtraction has a hard zero, and defense values grow into the
same magnitude as damage values.

**Recommendation:** switch to a ratio model before tuning anything else, e.g.

```
mitigation  = Defense / (Defense + K)        where K = 100 + 10 × attackerLevel
finalDamage = rawDamage × (1 - mitigation)
```

This is monotonic, never produces zero or negative damage, and keeps the existing
Defense/MagicDefense coefficients usable. Update `character-system.md` §Formulas at the same
time.

### B2 — `RecalculateDerived()` skip condition uses `||` where it needs `&&`

`Assets/Script/StatSystem/StatsSO.cs:272`

```csharp
if (!isDevMode && (
    Mathf.Approximately(target.FinalValue,   newBase.FinalValue)   ||
    Mathf.Approximately(target.LevelUpValue, newBase.LevelUpValue) ||
    Mathf.Approximately(target.EquipmentValue, newBase.EquipmentValue) ||
    Mathf.Approximately(target.EquipmentByPrimaryValue, newBase.EquipmentByPrimaryValue))
) continue;
```

The intent is "skip when nothing changed", which requires **all four** layers to match. With
`||`, a single matching layer short-circuits the whole condition and the derived stat is never
written. With no equipment attached — the default state — `EquipmentValue` is `0` on both sides,
`Mathf.Approximately(0, 0)` is `true`, and **every derived stat is skipped on every recalculation**.

Consequence: with `isDevMode = false` (the shipped default), levelling up and spending stat
points do not change any derived stat. This is almost certainly why `isDevMode` had to be added
as an escape hatch.

**Fix:** replace the three `||` with `&&`.

---

## 3. High

### B3 — `FinalValue` silently drops equipment when a stat has any modifier

`Assets/Script/StatSystem/Stat.cs:198`

The no-modifier branch returns `finalValue + equipmentValue` (line 203), but the modifier branch
initialises `finalValue = baseValue + levelUpValue` and never adds `equipmentByPrimaryValue`
back. So a derived stat that carries equipment *and* one buff loses its entire equipment
contribution. The class doc-comment (line 15) specifies the opposite:

```
FinalValue = (AdjustedValue + EquipmentValue + ΣFlat) × (1 + ΣPercentAdd) × Π(1 + PercentMult)
```

The subsequent `equipmentValue = finalValue - (baseValue + levelUpValue) + equipmentByPrimaryValue`
(line 235) then reports an equipment figure that the returned `finalValue` does not actually
contain — the two fields disagree.

**Fix:** initialise `finalValue = baseValue + levelUpValue + equipmentByPrimaryValue` in both
branches, so percent modifiers scale the equipped total as documented.

### B4 — `modifiers` is serialized against its own stated invariant

`Assets/Script/StatSystem/Stat.cs:52` is `[SerializeField]`, directly under a comment reading
*"BẮT BUỘC [NonSerialized]"* and contradicting `CLAUDE.md`, which documents the field as
`[NonSerialized]`. Runtime buffs will be written into the `.asset` file and survive across Play
Mode sessions; the `ClearModifiers()` loop in `OnEnable` masks the symptom at load time but the
asset still churns on disk.

**Fix:** restore `[NonSerialized]`.

### B5 — `StatType` covers only 12 of the 16 stats the formula table defines

Missing: `MagicDefense`, `LifeSteal`, `CrowdControlResist`, `AttackRange`. `MagicDefense` is the
magic-defense pillar for all 7 entities and is authored for every one of them in the
spreadsheet, so a quarter of the balance table is currently unimplementable.

**Fix:** add the four values in the 100+ range, and add matching entries to
`GameConstants.StatTypeName` — `StatsViewDTO`'s constructor indexes that dictionary directly and
will throw `KeyNotFoundException` on a missing key.

### B6 — Clamps in the table have no implementation

The spreadsheet specifies `clamp(AttackSpeed, 0.3, 3)`, `clamp(CritChance, 0, 80)` and
`clamp(Evasion, 0, 75)`, but `DerivedStatFormula` has no min/max fields, so these bounds exist
only as Excel comment text. They do not bind today (see B7 — the values are far from their
ceilings), but they will as soon as percent modifiers stack.

**Fix:** add `minValue` / `maxValue` fields defaulting to ±infinity and clamp inside
`Evaluate()`.

### B7 — The percent/clamp stat group is effectively frozen across the whole level range

Growth from Lv1 to Lv20, balanced build:

| Stat | Lv1 | Lv20 | Change |
|---|---:|---:|---:|
| MaxHP | 198.00 | 406.00 | +105% |
| PhysicalDamage | 31.20 | 66.00 | +112% |
| MagicDamage | 29.20 | 62.90 | +115% |
| Defense | 20.60 | 40.00 | +94% |
| **CritChance** | 9.00 | 10.25 | **+14%** |
| **Evasion** | 3.50 | 4.80 | **+37%** |
| **AttackSpeed** | 0.95 | 1.01 | **+6%** |
| **MoveSpeed** | 3.20 | 3.28 | **+3%** |
| **CritDamage** | 158.00 | 160.40 | **+2%** |

Nineteen levels of investment move attack speed by six percent and move speed by three. A player
who dumps every point into DEX gains 0.29 attacks per second over the whole run. These stats are
invisible to the player, which makes DEX and LUK feel like wasted points.

The cause is a mismatch between the coefficients (0.015–0.05 per point, sized for tens of points)
and the point budget (1 point per level → 19 points total).

**Fix — pick one:** multiply the percent-group coefficients by roughly 4–5×, or raise the budget
to 3–5 points per level. The second is the smaller change and also fixes B8.

---

## 4. Medium

### B8 — The game gets easier as the player levels

Trash Melee damage against the player: **6.2/hit at Lv1 → 5.8/hit at Lv20**, while player HP goes
198 → 406. Hits-to-kill the player rises from 32 to 70. The player's defense growth (+19.4)
exactly cancels the enemy's `perLevel` damage growth (+19.0), and the HP gain is pure surplus.

Enemy primary stats are constant across levels — only their `perLevel` term scales — so enemies
gain roughly 1.7× while the player gains roughly 2.1× plus allocated points.

**Fix:** scale enemy primaries with level (a `primaryPerLevel` term), or cut the VIT→Defense
coefficient on the player.

### B9 — Damage spread between archetypes is far too wide

Damage per hit taken by a Lv20 player: Trash 5.8, Assassin 19.6, Caster 53.4 (magic), Boss 136.0.
That is a 23× spread. The boss kills a full-health Lv20 player in 3.5 seconds while a trash mob
needs 77 seconds. Boss `PhysicalDamage = 10 + Level×2.6 + STR×4 + DEX×1` with STR 25 is the
outlier.

**Fix:** cut boss physical/magic damage to about 60% of current and raise the floor on Trash and
Swarm.

### B10 — Trash TTK is far too long for the genre

The player needs **7.5s at Lv1** and **5.2s at Lv20** to kill one Trash Melee (crit-averaged, no
weapon multiplier). The stated reference, Cult of the Lamb, kills a basic enemy in 1–3 hits — on
the order of one second. `MaxHP = 30 + Level×6 + STR×1.5 + VIT×13` with VIT 8 is the driver: 104
of a trash mob's 152 starting HP comes from the VIT term alone.

**Fix:** drop the trash VIT coefficient from 13 to roughly 6, targeting 2–3 hits per basic enemy.

---

## 5. Low / cleanup

| # | Location | Issue |
|---|---|---|
| B11 | `Stat.cs:145` | `adjustedValue = AdjustedValue;` in `Recaulate()` is a self-assignment. `AdjustedValue` is now a serialized field, not the computed property described in the doc-comment. `CalculateFinalValue()` already assigns it correctly — delete the line. |
| B12 | `Stat.cs:46-47` | `cachedValue` and `isDirty` are dead after lazy caching was removed; `SetDirty()` now recalculates eagerly and nothing reads `isDirty`. The class doc-comment still describes the removed `Value` / `BonusValue` API. |
| B13 | `StatsSO.cs:9` | `[Min(1)]` on `statUnusedBonus` and `Mathf.Max(1, value)` in the setter conflict with `CalculateStatUnusedBonus()`, which floors at 0 — "all points spent" can never display. Use `[Min(0)]` and `Mathf.Max(0, …)`. |
| B14 | `Stat.cs:236-237` | Two unguarded `Debug.Log` calls in `CalculateFinalValue()`'s modifier branch. They fire on every write to any stat that carries a modifier, including inside `OnValidate` loops over the whole profile. Remove them, or gate them behind `#if UNITY_EDITOR` (`isDevMode` lives on `StatsSO` and is not reachable from `Stat`). |

---

## 6. Recommended order

1. **B2** — one operator; without it nothing else is observable in the editor.
2. **B3, B4, B13, B11, B12, B14** — implementation correctness and cleanup, all local.
3. **B5, B6** — close the enum and clamp gaps so the table can be fully authored.
4. **B1** — decide the mitigation model; this gates all number tuning.
5. **B7, B8, B9, B10** — retune against the chosen mitigation model using the demo workbook.

---

## 7. Acceptance criteria for the retune

- No matchup in the 7×7 entity table produces zero or negative damage at any level 1–20.
- Player kills a Trash Melee in 1.5–3.0s; a Boss fight lasts 45–120s.
- Every primary stat moves at least one derived stat by ≥25% over a full 19-point investment.
- Enemy damage per hit against the player grows at least as fast as player effective HP.
- An EditMode test asserts `RecalculateDerived()` updates every derived stat after a level
  change with `isDevMode = false` (regression guard for B2).
