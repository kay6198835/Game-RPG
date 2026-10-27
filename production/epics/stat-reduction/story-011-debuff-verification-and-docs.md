# Story 011: Debuff verification pass and documentation update

> **Epic**: Stat Reduction (Debuff) Flow
> **Status**: Blocked
> **Layer**: Feature
> **Type**: Integration
> **Estimate**: 0.25 d
> **Source task**: SD-10 + SD-11 (`production/epics/stat-reduction/EPIC.md`)
> **Manifest Version**: N/A — `docs/architecture/control-manifest.md` does not exist
> **Last Updated**: [set by /dev-story when implementation begins]
> **BLOCKED**: ADR-0005 is Proposed — accept with `/architecture-decision accept ADR-NNNN` once decided. The code these ADRs describe is already in the project; only the ADR status is pending.

## Context

The whole flow needs one end-to-end run and the docs must describe the shipped behaviour.

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

*From `production/epics/stat-reduction/EPIC.md` (SD-10 + SD-11), scoped to this story. The GDD has no debuff criteria yet — Story 001 adds them:*

- [ ] The Play Mode checklist from the QA plan for this epic runs with every row PASS, or each FAIL is recorded as a bug-inbox note
- [ ] `design/gdd/stat-system.md` `Debuffs` subsection is marked `[IMPLEMENTED]`
- [ ] `docs/systems/stats/README.md` + `CHANGELOG.md` and `docs/systems/abilities/README.md` describe on-hit `SubEffects` with `recipient: Target`; ADR-0001 names the real dictionary type

---

## Implementation Notes

- Docs follow `docs/CLAUDE.md` and `.claude/rules/language-reporting.md` (English only).

---

## Out of Scope

- Fixing any FAIL — goes back to the owning story or a new inbox note.

---

## QA Test Cases

*Back-filled 2026-10-22 by `/qa-plan feature: stat-reduction` (no qa-lead gate at review mode `lean`). Automated cases are a **specification**: they cannot compile until BUG-084 is resolved (no `.asmdef`). Until then run the **Play Mode** checks and record them in the week's playtest sheet. `<Story 001 …>` placeholders must be filled from the GDD `Debuffs` subsection before testing. Use a fresh `ScriptableObject.CreateInstance<BaseStatsSO>()` or a duplicate asset — never a committed stat asset.*

- **AC-1: checklist run**
  - Given: Stories 002–010 implemented
  - When: run every Play Mode check in Stories 002–010
  - Then: all PASS, or each FAIL recorded as a `production/qa/bug-inbox.md` note
  - Edge cases: a check blocked by a missing asset → recorded as NOT ASSESSED, not PASS
- **AC-2/3: docs describe shipped behaviour**
  - Given: the checklist passed
  - When: review the GDD `Debuffs` subsection, `docs/systems/stats`, `docs/systems/abilities`, ADR-0001
  - Then: marked `[IMPLEMENTED]`; on-hit `SubEffects` with `recipient: Target` documented; ADR-0001 names `Dictionary<StatType, Stat>`
  - Edge cases: none

---

## Test Evidence

**Story Type**: Integration
**Required evidence**:
- Integration: `tests/PlayMode/StatSystem/DebuffVerificationAndDocsTests.cs` OR a playtest doc in `production/qa/playtests/` (the PlayMode test path is blocked by BUG-084, so the playtest doc is the practical route).

**Status**: [ ] Not yet created

---

## Dependencies

- Depends on: Story 010
- Unlocks: None
