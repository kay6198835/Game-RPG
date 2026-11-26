# CLAUDE.md

Instructions for Claude Code in this repository. **Short summaries + paths only.** Detail lives in the files
linked below; read them when the task needs them.

> **Last updated:** 2026-11-22 (HEAD `1e5afb3c`, branch `origin/sprint-23`).
>
> **Rule for every writer (skills, routines, sessions, owner):** keep this file ≤ 200 lines (the commit hook
> blocks more). Never copy data in here — bugs, events, checklists, history, per-file layout. Write them to
> their owner file (table below) and, at most, add one summary line + path here. No dated "Previous entry"
> blocks. Old full text: [docs/archive/CLAUDE-history-2026-11-19.md](docs/archive/CLAUDE-history-2026-11-19.md).

## Project

Unity 2D action roguelike RPG. Top-down real-time melee with directional attacks, weapon-linked abilities,
per-run progression, procedurally generated rooms (clear enemies → doors open).
**Demo target:** menu → dungeon run → death / restart, combat focus. Open items:
[production/demo-checklist.md](production/demo-checklist.md).

## Where things live

| Need | Owner file (source of truth) |
|---|---|
| Engine version, packages, API notes | `docs/engine-reference/unity/VERSION.md` — **read before using an engine API** |
| How a system works (code shape, flow, gotchas) | `docs/systems/<system>/README.md`; index `docs/systems/README.md` |
| Open bugs (generated) | `production/qa/open-bugs.md` — regenerate: `bash .claude/scripts/bug-id.sh open > production/qa/open-bugs.md` |
| One bug's status / evidence | `production/qa/bugs/BUG-NNN.md` (`**Status**` line always wins) |
| Suspected defect (no ID yet) | `production/qa/bug-inbox.md` — IDs only from `/weekly-wrapup` (`.claude/rules/bug-inbox.md`) |
| Demo checklist | `production/demo-checklist.md` |
| Design | `design/gdd/*.md`; decisions `docs/architecture/adr-*.md` |
| UI flow | `docs/ui/ui-ux-flow.md` |
| EventID list | `docs/systems/event-system/README.md` |
| Doc change history | `docs/CHANGELOG-DOCS.md`, `docs/systems/<system>/CHANGELOG.md` |
| Sprint / routine state | `production/sprints/`, `production/session-state/`, `.claude/docs/pm-routine-protocol.md` |
| Code rules per area (auto-loaded by path) | `.claude/rules/*.md` |

Systems in `docs/systems/`: abilities, character, dependency-injection, enemy, event-system, items, map,
object-pooling, pathfinding, stats, ui, weapons.

## Environment

- **Unity 2022.3.62f3 LTS**, URP 2D. VContainer 1.19.0 (git package), Input System 1.14, TextMeshPro 3.0.7,
  DOTween vendored in `Assets/Plugins/`. Detail: `docs/engine-reference/unity/VERSION.md`.
- No CLI build / lint / test. Play Mode in the Editor; compile errors in the Console. IDE: `Game-RPG.sln`.
- Main dev scene `Assets/Scenes/Main/LoadRandomMap.unity`; entry scene (index 0) `MainGamePlay.unity`;
  in-game UI `GameplayUI.unity` (additive); room authoring `SetLevel.unity`.
- Gameplay code: `Assets/Script/GameRPG.asmdef`. Tests: `Assets/Tests/EditMode/` (no PlayMode assembly yet).
- ⚠️ Every playable scene needs a `GameLifetimeScope`; `RegisterComponentInHierarchy<T>()` finds, never spawns.

## Code map (folders → docs)

| Folder (`Assets/Script/`) | What | Doc |
|---|---|---|
| `Character/Base/` | Shared hub, state machine, stats/vitals/effects/receiver bases | `docs/systems/character/` |
| `Character/Player/`, `Character/Entity/` | Player (10 states) and enemy AI | `character/`, `enemy/` |
| `System/LifetimeScope/` | VContainer composition root | `dependency-injection/` |
| `System/Abilities/` | Abilities v2 — the only ability framework | `abilities/` |
| `System/StatSystem/` | `BaseStatsSO` (data) + `StatSheet` (calculator) | `stats/` |
| `System/Enemy/`, `System/Pathfinding/` | Spawning, A* | `enemy/`, `pathfinding/` |
| `System/Item/`, `Weapons/` | Items, melee / ranged weapons | `items/`, `weapons/` |
| `System/PoolableService/` | Object pool | `object-pooling/` |
| `Map/`, `LevelEdit/`, `Database-SO/Modal/` | Maze, rooms, doors, room data | `map/` |
| `UIFlow/`, `UI/` | Menu → loading → HUD (UGUI); legacy UI Toolkit | `ui/`, `docs/ui/ui-ux-flow.md` |
| `Manager/` | `EventManager` static bus | `event-system/` |
| `FastTest/` | Combat test scene, automated QA probe | — |

Data: `Assets/SO/` (stats, abilities, weapons, items, dungeon), `Assets/Data/Json/Room/` (20 rooms).

## Architecture — essentials only

- **DI (ADR-0004):** cross-system services via VContainer `[Inject] Construct(...)`. The player is spawned by
  `PlayerManager` — never place it in a scene. Pooled / runtime objects are injected at spawn.
  Siblings inside a character use `Core.GetCoreComponent<T>(out …)` (returns null silently if mis-wired).
- **Characters:** `ICharacter.SetCharacterData()` → `CharacterBase.ApplyCharacterData()`. All behaviour is a
  state subclass; animation events drive states through the `StatusAnimation` enum. Death = sticky
  `ICharacterInput.IsDeath`.
- **Damage:** `INegativeReceiver.TakeDamage(float, Vector2)`; single implementer `NegativeReceiverBase`.
- **Abilities:** `AbilityDefinition` SO composed of effects + conditions; no GDD yet.
- **Map:** `MazeController` → `RoomGeneraterController` (holds `EnemyManager` / `LevelManager` by reference)
  → room JSON → spawn → `RoomCell` counts deaths → `ON_OPEN_DOOR`.
- **Events:** `EventManager.Resgister` / `Emit` with `EventID` (typo is real). UIFlow has its own `UIEvents`.

## Coding conventions

- **No comments** unless the WHY is non-obvious.
- **ScriptableObject-first** for game data.
- **No new singletons** — only `MazeController` and `EnemyManager` (ADR-0002).
- **State machine for all characters** — new behaviour = new state subclass, never `if/else` in `Update`.
- **Layer masks** in the Inspector; never hardcode indices.
- **Renaming a serialized field** needs `[FormerlySerializedAs]` or migrated assets (lost data three times).
- **Preserve intentional typos:** `Resgister`, `INegativeReciver.cs`, `Modal`, `ENEBLE`, `CaculateIndex`,
  `currrentSA`, `deplayTime`, `IObjecPoolService.cs` (file name only), `amoutDamage`.
- **File / class mismatches — do not rename:** `PlayerInputHandle.cs` → `PlayerInputHandler`,
  `TalentManager.cs` → `TalentManagger`, `VitalComponent.cs` → `VitalStatsComponent`,
  `CoreCompoment.cs` → `CoreComponent<T>`, `StatEffectBase.cs` → `StatsEffectBase`.
- **Bugs:** never allocate a `BUG-NNN` ID; write a note to `production/qa/bug-inbox.md`.

## Current state — one line each

- **Bugs:** 6 open S1 (BUG-135 no hit damages anyone since `8005d27e` and player death still not reaching Game Over are the top two) — see `production/qa/open-bugs.md`.
- **Demo:** death/restart loop and Abilities v2 smoke are the main gaps — see `production/demo-checklist.md`.
- **Builds:** no Player build verified yet (BUG-126 blocks it).

## Input

WASD move · LMB attack · `1`–`4` abilities (hold) · F equip · G interact · Q pick up · Space dash ·
C stats panel · RMB block (handler commented out).
