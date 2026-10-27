# Story 007: Slow effect through IMovement.AddSpeedMultiplier

> **Epic**: Stat Reduction (Debuff) Flow
> **Status**: Blocked
> **Layer**: Feature
> **Type**: Integration
> **Estimate**: 0.2 d
> **Source task**: SD-06 (`production/epics/stat-reduction/EPIC.md`)
> **Manifest Version**: N/A — `docs/architecture/control-manifest.md` does not exist
> **Last Updated**: [set by /dev-story when implementation begins]
> **BLOCKED**: ADR-0005 is Proposed — accept with `/architecture-decision accept ADR-NNNN` once decided. The code these ADRs describe is already in the project; only the ADR status is pending.

## Context

Enemy speed does not read `StatType.MoveSpeed`, and no effect calls `MovementBase.AddSpeedMultiplier()`, so a slow cannot be authored today.

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

*From `production/epics/stat-reduction/EPIC.md` (SD-06), scoped to this story. The GDD has no debuff criteria yet — Story 001 adds them:*

- [ ] A new `AbilityEffectDefinition` (e.g. `SpeedModifierForDuration`) resolves `IMovement` with `TryGetRecipientComponent<IMovement>(Target)` and applies `AddSpeedMultiplier(token, x)`
- [ ] The slowed enemy moves at `x` × speed for the duration, then returns to normal
- [ ] The slow is cleared on pool re-spawn (`MovementBase.ClearImpacts()`)

---

## Implementation Notes

- Deliver it as a `SubEffect` with `recipient: Target` under a spawn effect — the top-level `Effects` list has no target.
- If Story 001 chooses the `MoveSpeed` stat route instead, rewrite this story's AC before starting.

---

## Out of Scope

- Story 010: authoring the Slow asset.

---

## QA Test Cases

*Back-filled 2026-10-22 by `/qa-plan feature: stat-reduction` (no qa-lead gate at review mode `lean`). Automated cases are a **specification**: they cannot compile until BUG-084 is resolved (no `.asmdef`). Until then run the **Play Mode** checks and record them in the week's playtest sheet. `<Story 001 …>` placeholders must be filled from the GDD `Debuffs` subsection before testing. Use a fresh `ScriptableObject.CreateInstance<BaseStatsSO>()` or a duplicate asset — never a committed stat asset.*

- **AC-1: slow resolves `IMovement` from the target**
  - Given: a spawn effect with a Slow SubEffect (`recipient: Target`)
  - When: its projectile hits an enemy
  - Then: `AddSpeedMultiplier(token, x)` is called on that enemy's `IMovement`
  - Edge cases: hit collider without an `ICharacter` root → no-op
- **AC-2: slow lasts its duration**
  - Given: x = 0.6, duration = 2 s
  - When: the enemy is slowed
  - Then: speed = 0.6 × normal for 2 s, then normal
  - Edge cases: two slows from different tokens multiply; target dies mid-slow
- **AC-3: cleared on pool re-spawn**
  - Given: a slowed enemy
  - When: it dies and is re-spawned
  - Then: speed multiplier = 1
  - Edge cases: x = 0 (root)

**Play Mode**: hit an enemy with the Slow ability → visibly slower for 2 s, then normal; kill it → next spawn moves at normal speed.

---

## Test Evidence

**Story Type**: Integration
**Required evidence**:
- Integration: `tests/PlayMode/StatSystem/SlowEffectTests.cs` OR a playtest doc in `production/qa/playtests/` (the PlayMode test path is blocked by BUG-084, so the playtest doc is the practical route).

**Status**: [ ] Not yet created

---

## Dependencies

- Depends on: Story 001, Story 004
- Unlocks: Story 010
