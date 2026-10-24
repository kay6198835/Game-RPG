# Story 003: Vitals hold resources only; other stats are read live

> **Epic**: Stat Reduction (Debuff) Flow
> **Status**: Blocked
> **Layer**: Foundation
> **Type**: Logic
> **Estimate**: 0.3 d
> **Source task**: SD-02 (`production/epics/stat-reduction/EPIC.md`)
> **Manifest Version**: N/A — `docs/architecture/control-manifest.md` does not exist
> **Last Updated**: [set by /dev-story when implementation begins]
> **BLOCKED**: ADR-0005 and ADR-0001 are Proposed — accept with `/architecture-decision accept ADR-NNNN` once decided. The code these ADRs describe is already in the project; only the ADR status is pending.

## Context

`VitalStatsBase.Reborn()` snapshots all 17 stats into `currentStats`; buffs and debuffs change only the SO, and combat reads the snapshot, so stat debuffs have no effect (inbox NOTE-20261021-2).

**GDD**: `design/gdd/stat-system.md`
**Requirement**: `TR-stat-???` — ⚠️ no stat-system entry exists in `docs/architecture/tr-registry.yaml`; register one with `/architecture-review`

**ADR Governing Implementation**: ADR-0005: Unified character contract (secondary: ADR-0001)
**ADR Decision Summary**: Player and Entity share component interfaces (`IStatService` = max values, `IVitalComponent` = current values, `INegativeReceiver`, `IMovement`) through generic bases (`StatHandlerBase<TCore>`, `VitalStatsBase<TCore>`, `MovementBase<TCore>`); from outside a character, reach a capability from the `ICharacter` root with `GetComponentInChildren<T>()`.
**ADR Version**: 2026-09-23 (Date; amendments 2026-09-24, 2026-09-25; no Last Verified)

**Engine**: Unity 2022.3.62f3 LTS | **Risk**: LOW
**Engine Notes**: LOW — no post-cutoff APIs. Verification required: `ScriptableObject` cloning with `Object.Instantiate`; `GetComponentInParent<T>()` with an interface `T`.

**Control Manifest Rules (this layer)**: N/A — `docs/architecture/control-manifest.md` does not exist

---

## Acceptance Criteria

*From `production/epics/stat-reduction/EPIC.md` (SD-02), scoped to this story. The GDD has no debuff criteria yet — Story 001 adds them:*

- [ ] `currentStats` holds only resource stats (HP, Mana); `GetCurrentStatValue()` reads every other `StatType` from `IStatService`
- [ ] A timed PhysicalDamage / CritChance debuff changes `WeaponHolderBase.CalculateCurrentDamage()` while active and stops at expiry
- [ ] When max HP / Mana changes (`OnStatChanged`), current HP / Mana is clamped to the new max
- [ ] No `KeyNotFoundException` from `Recovery()` / `Reduction()` on any `StatType`

---

## Implementation Notes

- Keep `IVitalComponent` as the only current-value API (ADR-0005 §2).
- Audit `Reborn()` seeding against the max-value fallback in `GetCurrentStatValue()` (BUG-066 / BUG-070 caveat).

---

## Out of Scope

- Story 004: modifier sources.
- Story 008: health bar refresh.

---

## QA Test Cases

*Back-filled 2026-10-22 by `/qa-plan feature: stat-reduction` (no qa-lead gate at review mode `lean`). Automated cases are a **specification**: they cannot compile until BUG-084 is resolved (no `.asmdef`). Until then run the **Play Mode** checks and record them in the week's playtest sheet. `<Story 001 …>` placeholders must be filled from the GDD `Debuffs` subsection before testing. Use a fresh `ScriptableObject.CreateInstance<BaseStatsSO>()` or a duplicate asset — never a committed stat asset.*

- **AC-1: `currentStats` holds only HP and Mana**
  - Given: a character after `Reborn()`
  - When: inspect `currentStats` keys
  - Then: exactly `{HP, Mana}`; `GetCurrentStatValue(PhysicalDamage)` == `IStatService.GetStatValue(PhysicalDamage)`
  - Edge cases: missing HP key must not read as 0 (would trigger death — BUG-066/070 caveat)
- **AC-2: timed damage/crit debuff reaches combat**
  - Given: PhysicalDamage P
  - When: a timed −20 % PhysicalDamage modifier is applied, then expires
  - Then: `CalculateCurrentDamage()` uses 0.8 P while active and P after expiry
  - Edge cases: debuff applied and expiring in the same frame; CritChance debuff to 0
- **AC-3: current resource clamps to a lowered max**
  - Given: current HP = max HP = M
  - When: max HP is lowered to M−30 by a modifier
  - Then: current HP == M−30; raising max again does not raise current
  - Edge cases: max lowered to exactly current; Mana at 0
- **AC-4: no `KeyNotFoundException`**
  - Given: a character after `Reborn()`
  - When: `Recovery` / `Reduction` on every `StatType` value
  - Then: no exception; resource stats change, other stats are unchanged
  - Edge cases: primary stat types (early return)

**Play Mode**: cast Blessing → damage per hit rises, then returns at expiry; Weaken an enemy → its hits on the player shrink; no `KeyNotFoundException` in the Console.

---

## Test Evidence

**Story Type**: Logic
**Required evidence**:
- Logic: `tests/EditMode/StatSystem/VitalsResourcesOnlyTests.cs` — must exist and pass.
  ⚠️ **Cannot be satisfied today — BUG-084:** no `.asmdef` under `Assets/` and `tests/` sits outside `Assets/`, so Unity Test Runner cannot discover a test. Until BUG-084 is resolved, record a Play Mode run in the week's playtest sheet as interim evidence and say so in `/story-done`.

**Status**: [ ] Not yet created

---

## Dependencies

- Depends on: Story 002
- Unlocks: Story 008, Story 011
