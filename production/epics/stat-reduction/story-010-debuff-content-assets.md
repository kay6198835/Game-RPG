# Story 010: Debuff content assets: Armor Break, Weaken, Slow, Poison

> **Epic**: Stat Reduction (Debuff) Flow
> **Status**: Ready
> **Layer**: Feature
> **Type**: Config/Data
> **Estimate**: 0.2 d
> **Source task**: SD-09 (`production/epics/stat-reduction/EPIC.md`)
> **Manifest Version**: N/A — `docs/architecture/control-manifest.md` does not exist
> **Last Updated**: [set by /dev-story when implementation begins]

## Context

No asset in `Assets/SO` targets `recipient: Target` or uses `Reduction`, so the debuff path has never run.

**GDD**: `design/gdd/stat-system.md`
**Requirement**: `TR-stat-???` — ⚠️ no stat-system entry exists in `docs/architecture/tr-registry.yaml`; register one with `/architecture-review`

**ADR Governing Implementation**: N/A — ScriptableObject data authoring only
**ADR Decision Summary**: N/A
**ADR Version**: N/A

**Engine**: Unity 2022.3.62f3 LTS | **Risk**: LOW
**Engine Notes**: none

**Control Manifest Rules (this layer)**: N/A — `docs/architecture/control-manifest.md` does not exist

---

## Acceptance Criteria

*From `production/epics/stat-reduction/EPIC.md` (SD-09), scoped to this story. The GDD has no debuff criteria yet — Story 001 adds them:*

- [ ] Assets exist for Armor Break (Defense −30 % PercentAdd, 3 s), Weaken (PhysicalDamage −20 %, 3 s), Slow (Story 007, ×0.6, 2 s) and Poison (DoT 5 × 1 s)
- [ ] Each is attached as a `SubEffect` with `recipient: Target` on one Paladin ability and one enemy ability
- [ ] Each produces its effect in Play Mode on an enemy; the enemy-ability one on the player

---

## Implementation Notes

- Values are starting points; final numbers come from Story 001 and the balance sheet.

---

## Out of Scope

- Code changes — Stories 002–009.

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

- Depends on: Stories 002–009
- Unlocks: Story 011
