# Weekly to-do — week of 2026-11-09

> Owner request 2026-11-09. Planned work for this week, noted before implementation. Not bug IDs (see
> `.claude/rules/bug-inbox.md`); defects found while doing these go to `production/qa/bug-inbox.md`.
> Facts below were read from the repo on 2026-11-09 (branch `origin/feature/polish-game`, STATIC).

## Suggested order

1. Player death → replay loop (blocks every release build)
2. Room creation: one path, then the "finish a tour → expand the level" loop
3. Boss (needs 2)
4. Bonfire stat popup
5. Dash with wall check
6. Animation review against room size and stats (last, or in parallel with 2)

## 1. Player death and replay loop

**Goal.** Player dies → death animation → Game Over → replay (new run) works end to end, repeatably.

**Current state.**
- `PlayerDeathState` emits `ON_PLAYER_DEATH` only on `StatusAnimation.EndRangeTrigger`; no Paladin Die clip sends it (BUG-122, S1). Also fires every frame once reached (BUG-086).
- The only subscriber is `UIFlow.RealPlayerDataProvider` (Game Over, de-duplicated by `_deathReported`). No `GameManager`, no player-side `Reborn()` caller, no `ON_REALOAD_GAME` emitter (BUG-087).
- Death is entered from the sticky `ICharacterInput.IsDeath` flag (`4097be4c`); `ReloadComponent()` clears it.
- Since the stat split, spent points live only in `StatSheet`, so a replay starts with fresh stats by construction. Weapon/ability re-apply goes through `Player.SetCharacterData()` (Champion pick) — check the replay path calls it.

**Done when.** Die → Game Over → Replay → fresh run (full HP/Mana, no leftover buffs, start room, no stale enemies/pool state), three times in a row; `ON_PLAYER_DEATH` fires once.

## 2. Room creation, then level expansion

**2a. Unify room creation.**
- Today several paths build a level: `MazeController` / `MazeGenerator` (DFS grid), `RoomGridController` (events), `RoomGeneraterController` (JSON → tilemaps), `LevelManager` (editor save/import + runtime queries), plus `CombatTest` scene builder.
- Known gaps: room JSON read with `File.ReadAllText(Application.dataPath…)` is Editor-only (Bug #15); start/end rooms chosen by list position, so the start cell gets `BossRoom_ThroneArena` (Bug #16 / BUG-097); rooms without a spawn marker (Start, Boss) stay sealed (BUG-096, half addressed by `AutoOpenRoomTypes`); `MazeController.Awake()` missing `return` after `Destroy` (Bug #14).
- Decide one source of truth (what picks the room type, start, end, boss) and remove the duplicate paths. Needs an ADR or a GDD update (`design/gdd/map-system.md`).

**2b. Expand the level after one tour of rooms.**
- Meaning to confirm with the owner: when the player has cleared/visited all rooms of the current maze, generate the next, larger maze (floor 2…), keep the player and stats, reset room state.
- Depends on 2a. Needs: a "run progress / floor" owner (there is no `GameManager`), a trigger (last room cleared or portal), maze size per floor, and what carries over.

## 3. Boss for the release notes build

- Exists: `Assets/SO/Enemy/Role/Boss/BossEntityData.asset`, `Assets/SO/Stat/Enemy/Boss/BossStats.asset` (HP rebalanced to 1080 on 2026-11-09, Defense 112.5), room `BossRoom_ThroneArena.json`.
- Missing: boss behaviour (multi-phase, per the balance doc ~30 s fight), boss weapon/abilities, arena entry/exit, "boss dead → run complete". An earlier boss attempt was reverted (`ffe1976` → `4421fdc`/`ff67f4d`); read `production/boss-architecture-report.md` first.
- Needs 2a (which room is the boss room) and 1 (win/lose flow).

## 4. Bonfire: stat popup only through an interactable

**Goal.** The stat allocation popup opens only when the player interacts with a bonfire object (Dark Souls / Elden Ring style), not with the C key.

**Current state.** Popup is the UIFlow `StatsPanel` (key C) writing through `StatPointSession`. Interactables derive from `InteractiveObjects` (`PedestalChampionController` is the reference; `UpgradeController` in `System/RoomGameSystem/` is an empty `InteractiveObjects` stub that can become the bonfire). Player interaction is `PlayerIntertorState` → `Interactor.Intertion()` (key G).

**To do.** `BonfireController : InteractiveObjects` that opens the panel (event or `UIEvents`); remove/disable the C shortcut in `KeyBindings` + panel toggle; decide where bonfires are placed (Rest room, start room?); optional: bonfire = checkpoint/heal for the replay loop (item 1).

## 5. Dash

**Current state.** The `Dash` input action exists in `PlayerInput.cs` (Space) but no gameplay code uses it: no dash state, no cooldown, no stat. Movement goes through `IMovement` (`MovementBase`: locks, speed multipliers, knockback).

**To do.** `PlayerDashState` (state-machine rule: new behaviour = new state), distance/duration/cooldown from data (SO or stats), brief invulnerability if wanted.

**Wall safety (required).** The dash must not leave the player stuck in a large wall:
- sweep the path before moving (Rigidbody2D `Cast` / `CircleCast` with the obstacle layer mask set in the Inspector, never a hardcoded layer) and shorten the dash to the last free point, **or**
- if passing through thin obstacles is wanted, only allow it when the end point is free (`OverlapCircle` at the target) — otherwise clamp back.
- Test cases: dash into a thick wall, into a door tile, into a corner, from inside a doorway, with a knockback in progress.

## 6. Animation review (player and enemies)

**Goal.** Animations fit the room size and the character stats (scale, reach, speed).

**To check.**
- Attack reach vs `AttackSO.attackRange` / `MaxRangeAttack` and room tile size; movement clip speed vs `MoveSpeed`; `AttackSpeed` vs clip length (`attackRate` pause).
- Death clips need the animation event `BUG-122` waits for; Die clip direction order (the 8-direction convention, `DirectionResolver`).
- Pending direction fixes from 2026-11-08 (stash "anim dir shift -1": Mage, Assasin, RangedCaster TakeDamage, 45° offset) — re-check before reapplying.
- Tooling: `tools/anim-viewer/` (character viewer build scripts committed; generated HTML left untracked).
- Briefs: `production/animation-brief-detailed.md`, `design/gdd/animation-system.md`.

## Open questions for the owner

1. Item 2b: what exactly triggers the expansion (all rooms cleared, last room, portal) and how does the next floor grow?
2. Item 4: remove the C key completely, or keep it in a debug build only?
3. Item 5: dash may pass through thin obstacles, or never through anything solid?
4. Boss: one boss at the end of floor 1, or at the end of every expansion?
