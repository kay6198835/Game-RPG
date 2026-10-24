# Story 004: One modifier source per buff or debuff application

> **Epic**: Stat Reduction (Debuff) Flow
> **Status**: Blocked
> **Layer**: Foundation
> **Type**: Logic
> **Estimate**: 0.2 d
> **Source task**: SD-03 (= sprint-19 S19-09, BUG-100) (`production/epics/stat-reduction/EPIC.md`)
> **Manifest Version**: N/A — `docs/architecture/control-manifest.md` does not exist
> **Last Updated**: [set by /dev-story when implementation begins]
> **BLOCKED**: ADR-0005 is Proposed — accept with `/architecture-decision accept ADR-NNNN` once decided. The code these ADRs describe is already in the project; only the ADR status is pending.

## Context

`VitalStatsBase` passes `this` as the source for permanent item buffs and timed buffs/debuffs, so one expiry removes everything (BUG-100).

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

*From `production/epics/stat-reduction/EPIC.md` (SD-03 (= sprint-19 S19-09, BUG-100)), scoped to this story. The GDD has no debuff criteria yet — Story 001 adds them:*

- [ ] Each timed application uses its own source token; permanent item buffs use the item/effect instance
- [ ] An item buff survives the expiry of a timed debuff
- [ ] Two overlapping timed debuffs each last their full duration

---

## Implementation Notes

- `RemoveModifiersFromSource` matches by `ReferenceEquals` — a token must be a stable object reference, never a value type.
- Adjust `IVitalComponent.ApplyBuffDebuff` / `BuffDebuffForDuration` signatures as needed; update both implementers through `VitalStatsBase` (ADR-0005).

---

## Out of Scope

- Story 005: cancelling on death / disable.

---

## QA Test Cases

*Back-filled 2026-10-22 by `/qa-plan feature: stat-reduction` (no qa-lead gate at review mode `lean`). Automated cases are a **specification**: they cannot compile until BUG-084 is resolved (no `.asmdef`). Until then run the **Play Mode** checks and record them in the week's playtest sheet. `<Story 001 …>` placeholders must be filled from the GDD `Debuffs` subsection before testing. Use a fresh `ScriptableObject.CreateInstance<BaseStatsSO>()` or a duplicate asset — never a committed stat asset.*

- **AC-1: one source token per application**
  - Given: an item buff and a timed debuff on the same character
  - When: both are applied
  - Then: their modifiers carry different `Source` references
  - Edge cases: same `StatModifierGroup` asset applied twice → two tokens
- **AC-2: item buff survives a debuff expiry**
  - Given: item buff STR +3 and a timed debuff active
  - When: the timed debuff expires
  - Then: STR +3 is still applied
  - Edge cases: removing an unknown token is a no-op
- **AC-3: overlapping debuffs each last full duration**
  - Given: debuff X (3 s) applied at t=0 and again at t=1
  - When: t = 3 s
  - Then: the second application is still active until t = 4 s (or per `<Story 001 stacking rule>`)
  - Edge cases: null token rejected

**Play Mode**: pick up a stat item, cast Blessing, wait for expiry → item bonus still on the stats panel (C). Bench: Add from A, Add from B, Remove A → only B remains.

---

## Test Evidence

**Story Type**: Logic
**Required evidence**:
- Logic: `tests/EditMode/StatSystem/ModifierSourcePerApplicationTests.cs` — must exist and pass.
  ⚠️ **Cannot be satisfied today — BUG-084:** no `.asmdef` under `Assets/` and `tests/` sits outside `Assets/`, so Unity Test Runner cannot discover a test. Until BUG-084 is resolved, record a Play Mode run in the week's playtest sheet as interim evidence and say so in `/story-done`.

**Status**: [ ] Not yet created

---

## Dependencies

- Depends on: None
- Unlocks: Story 005, Story 007
