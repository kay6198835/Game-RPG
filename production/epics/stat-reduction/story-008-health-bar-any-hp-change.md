# Story 008: Enemy health bar refreshes on any HP change

> **Epic**: Stat Reduction (Debuff) Flow
> **Status**: Blocked
> **Layer**: Presentation
> **Type**: UI
> **Estimate**: 0.1 d
> **Source task**: SD-07 (`production/epics/stat-reduction/EPIC.md`)
> **Manifest Version**: N/A — `docs/architecture/control-manifest.md` does not exist
> **Last Updated**: [set by /dev-story when implementation begins]
> **BLOCKED**: ADR-0005 is Proposed — accept with `/architecture-decision accept ADR-NNNN` once decided. The code these ADRs describe is already in the project; only the ADR status is pending.

## Context

`EntityUIController.UpdateUIHealth()` is driven only from `EntityNegativeReciver.OnDamaged`, so instant reductions and DoT ticks lower HP without moving the bar.

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

*From `production/epics/stat-reduction/EPIC.md` (SD-07), scoped to this story. The GDD has no debuff criteria yet — Story 001 adds them:*

- [ ] `EntityUIController` subscribes to `IVitalComponent.CurrentStatsChanged` (and unsubscribes on disable)
- [ ] The enemy bar drops on an instant HP reduction and on every DoT tick
- [ ] The bar ratio uses the max from Story 002 and never exceeds 100 %

---

## Implementation Notes

- UI reads state only; it never writes vitals (`.claude/rules/ui-code.md`).

---

## Out of Scope

- Player HUD — already bound through `RealPlayerDataProvider`.

---

## QA Test Cases

*Back-filled 2026-10-22 by `/qa-plan feature: stat-reduction` (no qa-lead gate at review mode `lean`). Automated cases are a **specification**: they cannot compile until BUG-084 is resolved (no `.asmdef`). Until then run the **Play Mode** checks and record them in the week's playtest sheet. `<Story 001 …>` placeholders must be filled from the GDD `Debuffs` subsection before testing. Use a fresh `ScriptableObject.CreateInstance<BaseStatsSO>()` or a duplicate asset — never a committed stat asset.*

- **AC-1: subscribes to `CurrentStatsChanged`**
  - Setup: enemy in a combat room
  - Verify: the bar updates on events that do not go through `TakeDamage`
  - Pass condition: bar width follows current/max after every change; no leak after the enemy is pooled
- **AC-2: bar drops on instant reduction and every DoT tick**
  - Setup: apply an instant HP reduction, then Poison
  - Verify: screenshots before, after the reduction, and mid-DoT
  - Pass condition: each screenshot shows a lower bar; saved to `production/qa/evidence/story-008-*.png`
- **AC-3: bar never exceeds 100 %**
  - Setup: give the enemy a +HP buff
  - Verify: bar width at full HP
  - Pass condition: bar is exactly full, never wider

---

## Test Evidence

**Story Type**: UI
**Required evidence**:
- UI: a retained screenshot of each screen touched, in `production/qa/evidence/`.

**Status**: [ ] Not yet created

---

## Dependencies

- Depends on: Story 003
- Unlocks: Story 011
