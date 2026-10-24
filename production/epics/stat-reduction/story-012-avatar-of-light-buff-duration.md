# Story 012: Avatar of Light timed buff duration is zero

> **Epic**: Stat Reduction (Debuff) Flow
> **Status**: Ready
> **Layer**: Feature
> **Type**: Config/Data
> **Estimate**: 0.02 d
> **Source task**: SD-12 (`production/epics/stat-reduction/EPIC.md`)
> **Manifest Version**: N/A — `docs/architecture/control-manifest.md` does not exist
> **Last Updated**: [set by /dev-story when implementation begins]

## Context

`Assets/SO/Skill/Paladin/Ability/Avatar of Light/Effect/Gain For Time Effect.asset` has `duration: 0`, so its +15 HP buff is removed about one frame after it is applied.

**GDD**: `design/gdd/stat-system.md`
**Requirement**: `TR-stat-???` — ⚠️ no stat-system entry exists in `docs/architecture/tr-registry.yaml`; register one with `/architecture-review`

**ADR Governing Implementation**: N/A — single asset field fix
**ADR Decision Summary**: N/A
**ADR Version**: N/A

**Engine**: Unity 2022.3.62f3 LTS | **Risk**: LOW
**Engine Notes**: none

**Control Manifest Rules (this layer)**: N/A — `docs/architecture/control-manifest.md` does not exist

---

## Acceptance Criteria

*From `production/epics/stat-reduction/EPIC.md` (SD-12), scoped to this story. The GDD has no debuff criteria yet — Story 001 adds them:*

- [ ] `duration` is set to the intended value (owner decision)
- [ ] In Play Mode the +15 HP buff lasts the authored duration, then is removed

---

## Implementation Notes

- Data only; no code change.

---

## Out of Scope

- Stories 002–004: whether the buff actually reaches max HP.

---

## QA Test Cases

*N/A — no qa-lead specs at this tier (review mode `lean`, QL-STORY-READY skipped); implement against the Acceptance Criteria above. Run `/qa-plan feature: stat-reduction` for the test plan.*

---

## Test Evidence

**Story Type**: Config/Data
**Required evidence**:
- Config/Data: smoke check pass (`production/qa/smoke-*.md`).

**Status**: [ ] Not yet created

---

## Dependencies

- Depends on: None
- Unlocks: None
