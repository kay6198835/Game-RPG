# Story 001: Debuff rules design (stacking, slow mechanism, stat floors and caps, percent scope)

> **Epic**: Stat Reduction (Debuff) Flow
> **Status**: Ready
> **Layer**: Foundation
> **Type**: Config/Data
> **Estimate**: 0.2 d
> **Source task**: SD-00 (`production/epics/stat-reduction/EPIC.md`)
> **Manifest Version**: N/A — `docs/architecture/control-manifest.md` does not exist
> **Last Updated**: [set by /dev-story when implementation begins]

## Context

Several debuff tasks cannot start until the rules are written down: how repeated applications combine, how a slow is implemented, how low a stat may go, and whether percent modifiers scale the primary-derived part of a derived stat. Owner decision (`Needs owner`).

**GDD**: `design/gdd/stat-system.md`
**Requirement**: `TR-stat-???` — ⚠️ no stat-system entry exists in `docs/architecture/tr-registry.yaml`; register one with `/architecture-review`

**ADR Governing Implementation**: N/A — design documentation only; no architectural pattern is implemented
**ADR Decision Summary**: N/A
**ADR Version**: N/A

**Engine**: Unity 2022.3.62f3 LTS | **Risk**: LOW
**Engine Notes**: none

**Control Manifest Rules (this layer)**: N/A — `docs/architecture/control-manifest.md` does not exist

---

## Acceptance Criteria

*From `production/epics/stat-reduction/EPIC.md` (SD-00), scoped to this story. The GDD has no debuff criteria yet — Story 001 adds them:*

- [ ] `design/gdd/stat-system.md` gains a `Debuffs` subsection (Detailed Rules + Formulas + Edge Cases), marked `[PLANNED]`
- [ ] Stacking rule per source is defined: refresh duration, stack, or stack up to a cap N
- [ ] Slow mechanism is chosen: `IMovement.AddSpeedMultiplier` (recommended — already cleared on pool re-spawn by `MovementBase.ClearImpacts()`) or `MovementBase` reading `StatType.MoveSpeed`
- [ ] Floor/cap per stat is listed (e.g. Defense ≥ 0, MoveSpeed ≥ 10 % of base, CritChance 0–100 %) and whether percent debuffs scale `equipmentByPrimaryValue`

---

## Implementation Notes

- Run `/design-review design/gdd/stat-system.md` after editing.
- Formulas follow `.claude/rules/design-docs.md`: variable names match SO field names.

---

## Out of Scope

- Story 005 / 006 / 007 implement the rules decided here.

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
- Unlocks: Story 005, Story 006, Story 007
