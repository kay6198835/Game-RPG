# Story 006: Stat floors and caps

> **Epic**: Stat Reduction (Debuff) Flow
> **Status**: Blocked
> **Layer**: Foundation
> **Type**: Logic
> **Estimate**: 0.15 d
> **Source task**: SD-05 (`production/epics/stat-reduction/EPIC.md`)
> **Manifest Version**: N/A — `docs/architecture/control-manifest.md` does not exist
> **Last Updated**: [set by /dev-story when implementation begins]
> **BLOCKED**: ADR-0001 is Proposed — accept with `/architecture-decision accept ADR-NNNN` once decided. The code these ADRs describe is already in the project; only the ADR status is pending.

## Context

Nothing bounds a stat today; the GDD Formulas section requires a clamp for percentage stats, and debuffs can push Defense or MoveSpeed negative.

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

*From `production/epics/stat-reduction/EPIC.md` (SD-05), scoped to this story. The GDD has no debuff criteria yet — Story 001 adds them:*

- [ ] A −200 % Defense debuff yields the Story 001 floor, not a negative value
- [ ] CritChance (and every percentage stat listed in Story 001) never exceeds its cap
- [ ] Floors/caps are data (per `StatType`), not magic numbers in code

---

## Implementation Notes

- Apply the clamp where `FinalValue` is produced (`Stat.CalculateFinalValue()`) or in a per-`StatType` table on `BaseStatsSO`, so every reader sees the clamped value (ADR-0001: writes go through the SO).

---

## Out of Scope

- Story 002: which value is the max.

---

## QA Test Cases

*Back-filled 2026-10-22 by `/qa-plan feature: stat-reduction` (no qa-lead gate at review mode `lean`). Automated cases are a **specification**: they cannot compile until BUG-084 is resolved (no `.asmdef`). Until then run the **Play Mode** checks and record them in the week's playtest sheet. `<Story 001 …>` placeholders must be filled from the GDD `Debuffs` subsection before testing. Use a fresh `ScriptableObject.CreateInstance<BaseStatsSO>()` or a duplicate asset — never a committed stat asset.*

- **AC-1: Defense floor**
  - Given: Defense D > 0
  - When: a −200 % PercentAdd Defense modifier is applied
  - Then: `FinalValue` == `<Story 001 Defense floor>` (expected 0), not negative
  - Edge cases: value exactly at the floor; PercentMult below −100 %
- **AC-2: percentage-stat caps**
  - Given: CritChance C
  - When: +500 Flat CritChance
  - Then: `FinalValue` == `<Story 001 cap>` (expected 100)
  - Edge cases: value exactly at the cap; every capped stat listed in Story 001
- **AC-3: floors/caps are data**
  - Given: the floor/cap table
  - When: a value is changed in data only
  - Then: the clamp follows without a code change
  - Edge cases: removing the modifier restores the un-clamped value

**Bench**: `StatModifierTester` on a duplicate asset — add Defense PercentAdd −2.0 → floor; add CritChance +500 → cap; remove both → originals return.

---

## Test Evidence

**Story Type**: Logic
**Required evidence**:
- Logic: `tests/EditMode/StatSystem/StatFloorsCapsTests.cs` — must exist and pass.
  ⚠️ **Cannot be satisfied today — BUG-084:** no `.asmdef` under `Assets/` and `tests/` sits outside `Assets/`, so Unity Test Runner cannot discover a test. Until BUG-084 is resolved, record a Play Mode run in the week's playtest sheet as interim evidence and say so in `/story-done`.

**Status**: [ ] Not yet created

---

## Dependencies

- Depends on: Story 001
- Unlocks: Story 011
