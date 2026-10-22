# Story 009: Player runtime stat clone and leaked-asset clean-up

> **Epic**: Stat Reduction (Debuff) Flow
> **Status**: Blocked
> **Layer**: Foundation
> **Type**: Integration
> **Estimate**: 0.25 d
> **Source task**: SD-08 (`production/epics/stat-reduction/EPIC.md`)
> **Manifest Version**: N/A — `docs/architecture/control-manifest.md` does not exist
> **Last Updated**: [set by /dev-story when implementation begins]
> **BLOCKED**: ADR-0005 and ADR-0001 are Proposed — accept with `/architecture-decision accept ADR-NNNN` once decided. The code these ADRs describe is already in the project; only the ADR status is pending.

## Context

The player writes buffs, debuffs and stat points into the shared committed stat asset; all four player stat assets already carry leaked weapon modifiers (inbox NOTE-20261021-3).

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

*From `production/epics/stat-reduction/EPIC.md` (SD-08), scoped to this story. The GDD has no debuff criteria yet — Story 001 adds them:*

- [ ] `StatHandler.ResolveProfile()` instantiates the profile, assigns it to `statsSO` (Inspector shows "… (Runtime)") and destroys it in `OnDestroy`; it is rebuilt on `SetCharacterData()`
- [ ] `ResignChangeStat` / `ClearResignChangeStat` use `StatsSO`, with a null guard
- [ ] `Assets/SO/Stat/Player/{Paladin,Archer,Mage,Base}Stats.asset` carry no runtime modifiers and correct `equipmentValue` / `finalValue`
- [ ] After Play Mode + Save Project, `git status` shows no change under `Assets/SO/Stat/`; a debuff on the player does not survive death or New Game

---

## Implementation Notes

- Mirror `EntityStatsHandler.ResolveProfile()` (ADR-0005 §3, `Object.Instantiate` of a `ScriptableObject`).
- Asset clean-up is an owner (Editor) step; re-save through the Inspector so `OnValidate` recomputes values.
- Decide BUG-063 (`#if UNITY_EDITOR [SerializeField]` on `Stat.modifiers`) at the same time.

---

## Out of Scope

- Story 004: modifier sources.

---

## QA Test Cases

*Back-filled 2026-10-22 by `/qa-plan feature: stat-reduction` (no qa-lead gate at review mode `lean`). Automated cases are a **specification**: they cannot compile until BUG-084 is resolved (no `.asmdef`). Until then run the **Play Mode** checks and record them in the week's playtest sheet. `<Story 001 …>` placeholders must be filled from the GDD `Debuffs` subsection before testing. Use a fresh `ScriptableObject.CreateInstance<BaseStatsSO>()` or a duplicate asset — never a committed stat asset.*

- **AC-1: runtime clone**
  - Given: the player is spawned
  - When: inspect `StatHandler` → Stats SO
  - Then: it is not the project asset (`!ReferenceEquals`), is named "… (Runtime)", and is destroyed with the player; `SetCharacterData()` rebuilds it from the new class asset
  - Edge cases: `PlayerData.Stats` null; Inspector `statsSO` assigned vs empty
- **AC-2: subscribe helpers use `StatsSO`**
  - Given: profile comes from `PlayerData` (Inspector field empty)
  - When: `ResignChangeStat` then `OnDisable`
  - Then: no `NullReferenceException`; the callback fires on the clone's `OnStatChanged`
  - Edge cases: subscribe twice
- **AC-3: the four player stat assets are clean**
  - Given: `Assets/SO/Stat/Player/{Paladin,Archer,Mage,Base}Stats.asset`
  - When: read the YAML
  - Then: no `modifiers` entries; `equipmentValue` / `finalValue` match a fresh `OnValidate` recompute
  - Edge cases: Archer/Mage carry no Sword-and-Shield bonus
- **AC-4: nothing leaks to disk or across runs**
  - Given: equip a weapon, allocate a point, take a buff
  - When: exit Play Mode → File → Save Project; then die 3 times; then New Game
  - Then: `git status` clean under `Assets/SO/Stat/`; STR stays base + weapon after each death; points reset on New Game
  - Edge cases: domain reload disabled (Enter Play Mode Options)

**Play Mode**: double-click the "(Runtime)" Stats SO field to inspect live values; after the session, `git status` must be clean under `Assets/SO/Stat/`.

---

## Test Evidence

**Story Type**: Integration
**Required evidence**:
- Integration: `tests/PlayMode/StatSystem/PlayerRuntimeStatCloneTests.cs` OR a playtest doc in `production/qa/playtests/` (the PlayMode test path is blocked by BUG-084, so the playtest doc is the practical route).

**Status**: [ ] Not yet created

---

## Dependencies

- Depends on: None
- Unlocks: Story 010 (enemy debuffs on the player), Story 011
