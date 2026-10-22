# Story 005: Debuff lifecycle: cancel on death, disable and Reborn; apply the stacking rule

> **Epic**: Stat Reduction (Debuff) Flow
> **Status**: Blocked
> **Layer**: Foundation
> **Type**: Logic
> **Estimate**: 0.2 d
> **Source task**: SD-04 (= sprint-19 S19-10, BUG-071 residual) (`production/epics/stat-reduction/EPIC.md`)
> **Manifest Version**: N/A — `docs/architecture/control-manifest.md` does not exist
> **Last Updated**: [set by /dev-story when implementation begins]
> **BLOCKED**: ADR-0005 is Proposed — accept with `/architecture-decision accept ADR-NNNN` once decided. The code these ADRs describe is already in the project; only the ADR status is pending.

## Context

HoT/DoT coroutines recurse through a public wrapper with no handle, so nothing can stop them (BUG-071); timed debuffs have no stacking rule.

**GDD**: `design/gdd/stat-system.md`
**Requirement**: `TR-stat-???` — ⚠️ no stat-system entry exists in `docs/architecture/tr-registry.yaml`; register one with `/architecture-review`

**ADR Governing Implementation**: ADR-0005: Unified character contract
**ADR Decision Summary**: Player and Entity share component interfaces (`IStatService` = max values, `IVitalComponent` = current values, `INegativeReceiver`, `IMovement`) through generic bases (`StatHandlerBase<TCore>`, `VitalStatsBase<TCore>`, `MovementBase<TCore>`); from outside a character, reach a capability from the `ICharacter` root with `GetComponentInChildren<T>()`.
**ADR Version**: 2026-09-23 (Date; amendments 2026-09-24, 2026-09-25; no Last Verified)

**Engine**: Unity 2022.3.62f3 LTS | **Risk**: LOW
**Engine Notes**: LOW — no post-cutoff APIs. Verification required: `ScriptableObject` cloning with `Object.Instantiate`; `GetComponentInParent<T>()` with an interface `T`.

**Control Manifest Rules (this layer)**: N/A — `docs/architecture/control-manifest.md` does not exist

---

## Acceptance Criteria

*From `production/epics/stat-reduction/EPIC.md` (SD-04 (= sprint-19 S19-10, BUG-071 residual)), scoped to this story. The GDD has no debuff criteria yet — Story 001 adds them:*

- [ ] Coroutine handles are kept per source token; DoTs and timed modifiers are cancelled on death, disable and `Reborn()`
- [ ] A re-spawned pooled enemy starts with no debuff and no DoT
- [ ] Repeated applications follow the stacking rule from Story 001

---

## Implementation Notes

- Enemy side already clears modifiers and movement impacts in `EntityVitalStats.Reborn()` — keep that path.

---

## Out of Scope

- Story 006: floors and caps.

---

## QA Test Cases

*Back-filled 2026-10-22 by `/qa-plan feature: stat-reduction` (no qa-lead gate at review mode `lean`). Automated cases are a **specification**: they cannot compile until BUG-084 is resolved (no `.asmdef`). Until then run the **Play Mode** checks and record them in the week's playtest sheet. `<Story 001 …>` placeholders must be filled from the GDD `Debuffs` subsection before testing. Use a fresh `ScriptableObject.CreateInstance<BaseStatsSO>()` or a duplicate asset — never a committed stat asset.*

- **AC-1: DoTs and timed modifiers cancel on death / disable / `Reborn()`**
  - Given: an enemy with Poison (5 × 1 s) and a timed debuff
  - When: it dies at tick 2
  - Then: no further HP change and no modifier left after death
  - Edge cases: debuff applied in the same frame as death
- **AC-2: pooled re-spawn starts clean**
  - Given: the enemy above is released to the pool
  - When: it is spawned again
  - Then: full HP, no modifier, no running DoT
  - Edge cases: re-spawn within the DoT interval
- **AC-3: stacking follows the Story 001 rule**
  - Given: debuff X active
  - When: X is applied again before it ends
  - Then: result matches `<Story 001 stacking rule>`
  - Edge cases: re-application at the cap N; `timeCount = 1`

**Play Mode**: poison an enemy and kill it before the DoT ends → no activity after death; its next spawn is full HP with no debuff.

---

## Test Evidence

**Story Type**: Logic
**Required evidence**:
- Logic: `tests/EditMode/StatSystem/DebuffLifecycleTests.cs` — must exist and pass.
  ⚠️ **Cannot be satisfied today — BUG-084:** no `.asmdef` under `Assets/` and `tests/` sits outside `Assets/`, so Unity Test Runner cannot discover a test. Until BUG-084 is resolved, record a Play Mode run in the week's playtest sheet as interim evidence and say so in `/story-done`.

**Status**: [ ] Not yet created

---

## Dependencies

- Depends on: Story 001, Story 004
- Unlocks: Story 011
