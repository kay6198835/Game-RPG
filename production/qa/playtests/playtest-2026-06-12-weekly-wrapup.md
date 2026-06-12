# Playtest Report

## Session Info
- **Date**: 2026-06-12
- **Build**: 220c7cf
- **Duration**: N/A (code-based verification pass — no live Play Mode session logged this week)
- **Tester**: Automated weekly wrap-up (Claude Code)
- **Platform**: PC — Unity Editor
- **Input Method**: N/A
- **Session Type**: Weekly wrap-up — code verification pass

## Test Focus
End-of-week verification of changes merged this week: minimap refactor (`MapGridController`, `MapCell`), DOTween integration + "reload old map" logic fix, and a new `GameConstants` block. Combat/death/AI blockers from Sprint 1 were checked for progress.

## First Impressions (First 5 minutes)
- **Understood the goal?** N/A — no live session
- **Understood the controls?** N/A
- **Emotional response**: Concerned — Sprint 1's Must Have critical path (S1-01 to S1-05) shows zero code progress this week
- **Notes**: All 5 commits this week (`220c7cf`, `b2fb184`, `1a21050`, `be6c4cd`, `527a00d`) touch Map/Minimap systems, DOTween plugin import, and docs. None touch `WeaponMelee.cs`, `Core.cs`, `EntityDeathState.cs`, `EntityBasicState.cs`, or `AnimationPlayerController.cs`.

## Gameplay Flow

### What worked well
- Minimap refactor (`MapCell.cs`, `MapGridController.cs`) re-enables previously commented-out minimap logic (Bug #11 in CLAUDE.md) — partial progress toward minimap functionality.
- DOTween (Demigiant) added as a plugin — enables tween-based UI/VFX work going forward.
- `b2fb184` fixes a logic bug in reloading a previously-visited map/room.
- New `GameConstants.cs` additions reduce magic-string/number usage per coding conventions.

### Pain points
- **[Critical] Sprint 1 Must Have tasks (S1-01 through S1-05) show NO code changes this week.** Verified directly in source:
  - `WeaponMelee.cs:29-33` — `Attack()` foreach body is still empty. No `INegativeReceiver.TakeDamage()` call. (Bug #4 / S1-01 — still OPEN)
  - `Core.cs:21-25` — `TakeDamage()` still has no death check, no `ON_PLAYER_DEATH` emit. (Bug #6 / S1-03 — still OPEN)
  - `EntityDeathState.cs` — still extends `MonoBehaviour`, not `EntityState`; not wired into state machine. (Bug #7 / S1-04 — still OPEN)
  - `EntityBasicState.cs:21-24` — `Health <= 0` block is still empty, no transition to death state. (Bug #8 / S1-04 — still OPEN)
  - `AnimationPlayerController.cs:21` — still registers `StartAnimation` twice in `OnEnable` (and `OnDisable` mirrors the bug); `EndAnimation` callback never fires. (Bug #9 / S1-02 — still OPEN)
  - `EventManager.cs` `EventID` enum still has no `ON_PLAYER_DEATH` or `ON_PLAYER_TAKE_DAMAGE`.
- This is the **third consecutive sprint/week** where effort went to Map/Minimap work instead of the flagged combat/death/AI demo blockers — the exact risk Sprint 1's own risk register called out ("Pattern risk: 3 straight weeks of effort went to Map refactors instead of these exact blockers").
- Combat remains entirely untestable in Play Mode — no damage exchange possible.

### Confusion points
- None new this week beyond the carryover `RoomCell.GetDoor()` returning `new DoorController()` instead of `null` (Bug #5 from last playtest — not addressed).

### Moments of delight
- N/A — no live session this week.

## Bugs Encountered
| # | Description | Severity | Reproducible |
|---|-------------|----------|-------------|
| 1 | `WeaponMelee.Attack()` foreach body empty — no damage (Bug #4 / S1-01) | High | Always |
| 2 | `Core.TakeDamage()` no death check — player cannot die (Bug #6 / S1-03) | High | Always |
| 3 | `EntityDeathState` wrong base class + `EntityBasicState` death block empty (Bugs #7/#8 / S1-04) | High | Always |
| 4 | `AnimationPlayerController.OnEnable()` double-registers `StartAnimation` — `EndAnimation` never fires (Bug #9 / S1-02) | Critical | Always |
| 5 | `RoomCell.GetDoor()` returns `new DoorController()` instead of `null` for missing direction | Medium | Conditional |
| 6 | `_dungeonRoomSO.room` mutated on SO asset at runtime — dirty asset risk on crash | Low | On crash |

## Feature-Specific Feedback

### Minimap Refactor
- **Understood purpose?** Yes — re-enables minimap cell wall-visibility logic
- **Found engaging?** N/A — internal system, not yet verified in Play Mode this session
- **Suggestions**: Needs a Play Mode smoke check next session to confirm `MapGridController` no longer throws and minimap reflects room state correctly.

### DOTween Integration
- **Understood purpose?** Yes — adds tweening capability for future UI/VFX (HUD, transitions)
- **Found engaging?** N/A
- **Suggestions**: No gameplay code yet depends on it; verify it doesn't increase build size/load time materially.

### Combat
- **Understood purpose?** Yes
- **Found engaging?** N/A — still cannot deal damage, unchanged from last week
- **Suggestions**: Unchanged — Bug #1 (S1-01) must be fixed before any combat evaluation is possible.

## Quantitative Data
- **Room transitions completed**: N/A (no live session)
- **Deaths**: 0 — player death still not implemented
- **Combat encounters**: 0 — still no damage exchange possible
- **Features discovered vs missed**: Navigation ✅ | Minimap (refactored, unverified) ⚠️ | Combat ❌ | Death/restart ❌ | Room clear ❌

## Overall Assessment
- **Would play again?** N/A — no live session
- **Difficulty**: N/A — cannot die
- **Pacing**: N/A — game loop still incomplete
- **Session length preference**: N/A

## Top 3 Priorities from this session
1. **Sprint 1 Must Have tasks (S1-01 to S1-05) were not started this week despite being the committed sprint goal.** Re-plan immediately: either re-commit to these 5 tasks for next sprint with explicit time-boxing, or formally descope and document why Map work took priority again.
2. **Fix `WeaponMelee.Attack()` empty foreach** (Bug #4 / S1-01) — combat must deal damage before any game loop testing is possible. Still the single highest-leverage fix in the codebase.
3. **Fix `AnimationPlayerController` double-registration** (Bug #9 / S1-02, rated Critical — "all combat states permanently stuck") — blocks `PlayerUseWeaponState` from ever exiting via `animFinish`, which blocks attack/skill states regardless of S1-01.
