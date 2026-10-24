# Story 002: One definition of a stat's max value (FinalValue)

> **Epic**: Stat Reduction (Debuff) Flow
> **Status**: Blocked
> **Layer**: Foundation
> **Type**: Logic
> **Estimate**: 0.15 d
> **Source task**: SD-01 (`production/epics/stat-reduction/EPIC.md`)
> **Manifest Version**: N/A — `docs/architecture/control-manifest.md` does not exist
> **Last Updated**: [set by /dev-story when implementation begins]
> **BLOCKED**: ADR-0001 is Proposed — accept with `/architecture-decision accept ADR-NNNN` once decided. The code these ADRs describe is already in the project; only the ADR status is pending.

## Context

`BaseStatsSO.GetStatValue()` returns `AdjustedValue` (base + levelUp, no modifiers or equipment) while `FullStatsValue()` returns `FinalValue`. Heals clamp to the un-buffed max and Defense modifiers are ignored (inbox NOTE-20261021-1).

**GDD**: `design/gdd/stat-system.md`
**Requirement**: `TR-stat-???` — ⚠️ no stat-system entry exists in `docs/architecture/tr-registry.yaml`; register one with `/architecture-review`

**ADR Governing Implementation**: ADR-0001: StatSystem dual data structure (List + runtime Dictionary)
**ADR Decision Summary**: `BaseStatsSO` keeps `List<Stat> stats` as the serialized source and a runtime-only lookup dictionary built by `EnsureInitialized()`; every write goes through the `BaseStatsSO` API.
**ADR Version**: 2026-07-06 (Date; no Last Verified)

**Engine**: Unity 2022.3.62f3 LTS | **Risk**: LOW
**Engine Notes**: LOW — pinned version within training data. No post-cutoff APIs.

**Control Manifest Rules (this layer)**: N/A — `docs/architecture/control-manifest.md` does not exist

---

## Acceptance Criteria

*From `production/epics/stat-reduction/EPIC.md` (SD-01), scoped to this story. The GDD has no debuff criteria yet — Story 001 adds them:*

- [ ] `BaseStatsSO.GetStatValue()` returns `Stat.FinalValue`; an un-modified accessor is added only if a caller needs it
- [ ] A Defense modifier on an enemy changes the damage `EntityNegativeReciver.DamageCalculate()` lets through
- [ ] With +HP gear, `VitalStatsBase.Recovery()` never lowers current HP; the HUD max (`RealPlayerDataProvider`) includes gear

---

## Implementation Notes

- Callers to re-check: `VitalStatsBase.Recovery()` (`:54-56`), `EntityNegativeReciver.cs:25,30`, `RealPlayerDataProvider.cs:153-154`, `RegenComponent`.
- Writes still go only through the `BaseStatsSO` API (ADR-0001).

---

## Out of Scope

- Story 003: what `currentStats` holds.

---

## QA Test Cases

*Back-filled 2026-10-22 by `/qa-plan feature: stat-reduction` (no qa-lead gate at review mode `lean`). Automated cases are a **specification**: they cannot compile until BUG-084 is resolved (no `.asmdef`). Until then run the **Play Mode** checks and record them in the week's playtest sheet. `<Story 001 …>` placeholders must be filled from the GDD `Debuffs` subsection before testing. Use a fresh `ScriptableObject.CreateInstance<BaseStatsSO>()` or a duplicate asset — never a committed stat asset.*

- **AC-1: `GetStatValue()` returns `FinalValue`**
  - Given: a fresh `BaseStatsSO` with authored stats, no modifiers
  - When: `AddModifiersFromSource(src, [HP +20 Flat])`
  - Then: `GetStatValue(HP)` == `GetStat(HP).FinalValue` == previous + 20 and `FullStatsValue()[HP]` agrees
  - Edge cases: no modifiers (Final = base + levelUp + equipmentByPrimary); modifier removed → value returns
- **AC-2: Defense modifier changes damage taken**
  - Given: an enemy with Defense D and an incoming hit of damage d > D
  - When: a Defense −50 % PercentAdd modifier is applied, then the same hit lands
  - Then: damage = `max(0, d − D×0.5)` instead of `d − D`
  - Edge cases: D ≥ d → 0 damage; modifier removed → original value
- **AC-3: heal never lowers HP with +HP gear**
  - Given: player max HP M with +20 HP gear (max M+20), current = M+15
  - When: `Recovery(HP, 10)`
  - Then: current == M+20 (clamped to the buffed max), never < M+15; HUD max shows M+20
  - Edge cases: current already at max; heal of 0

**Play Mode**: pick up a +HP item → take 5 damage → heal 10 → HP never ends below its pre-heal value; hit an Armor-Broken enemy → more HP lost per hit.

---

## Test Evidence

**Story Type**: Logic
**Required evidence**:
- Logic: `tests/EditMode/StatSystem/OneMaxDefinitionTests.cs` — must exist and pass.
  ⚠️ **Cannot be satisfied today — BUG-084:** no `.asmdef` under `Assets/` and `tests/` sits outside `Assets/`, so Unity Test Runner cannot discover a test. Until BUG-084 is resolved, record a Play Mode run in the week's playtest sheet as interim evidence and say so in `/story-done`.

**Status**: [ ] Not yet created

---

## Dependencies

- Depends on: None
- Unlocks: Story 003
