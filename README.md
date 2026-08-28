# Game-RPG

A Unity 2D action roguelike RPG. Top-down real-time melee combat inspired by
**Cult of the Lamb**: directional attacks, weapon-linked skills, and per-run power
progression through procedurally generated rooms — clear the enemies to unlock the doors
to the next room.

**Demo target:** one complete game life cycle — start menu → dungeon run (movement, melee
combat, two skills, enemies, room progression) → death and restart. Scope is deliberately
limited to the combat system.

---

## Getting started

| | |
|---|---|
| **Engine** | Unity 2022.3.62f3 LTS (URP, 2D Renderer) |
| **Open** | Unity Hub → Open → this folder |
| **Play scene** | `Assets/Scenes/Main/Test/LoadRandomMap.unity` |
| **IDE** | `Game-RPG.sln` in Rider or Visual Studio |

Key packages: Input System 1.14.0, TextMeshPro 3.0.7, 2D Feature Pack,
Visual Scripting 1.9.4, DOTween.

There are **no build, lint, or test CLI commands** — all development happens through the
Unity Editor. Any `.cs` edit triggers auto-recompile; errors appear in the Console.

### Scene wiring the dungeon needs

`LoadRandomMap` will not run without all of these present in the scene:

- a `MazeController` (wires `MapGridController` + `RoomGridController`)
- a `LevelManager` with `dungeonRoomSO = Maze_Storage.asset`
- an `EnemyManager` (pathfinding service — `EntityMovement` reads its grid on `Start`)
- an `EnemySpawner` with `mapModel` assigned
- `RoomGridController._dungeonRoomSO = Maze_Load_Room.asset`
- `RoomGeneraterController._fastMovement` and `_genmap` assigned

---

## Controls

| Action | Binding |
|--------|---------|
| Move | WASD |
| Attack | Left Mouse |
| Block / Ability | Right Mouse (hold) |
| Skill | E (hold) |
| Equip / Unequip | F |
| Interact | G |
| Dash | Space |

---

## Repository map

```
Assets/Script/       All active game code — the single source of truth
  Character/Base/      Shared hub + state machine under both Player and Entity
  Character/Player/    Player MonoBehaviour, states, core components
  Character/Entity/    Enemy AI framework (mirrors the player pattern)
  Weapons/             Weapon base, melee, ranged, AttackSO stages
  Skill_Ability/       ActivateSkill SO lifecycle and concrete abilities
  StatSystem/          Primary/derived stat framework, modifiers
  Map/                 Maze generation, room grid, doors, minimap
  Enemy/               EnemySO, EnemyManager (pathfinding), EnemySpawner
  Pathfinding/         A*, grid, request manager
  Poolable/            Generic object pool
  Database-SO/Modal/   Enemy-spawn data model ("Modal" is a preserved typo)
  LevelEdit/           Room authoring + runtime room loading
  Manager/             EventManager static bus, AnimationEventManager
  UI/                  UI Toolkit menus + stats panel
  Utility/             GameConstants, helpers, extensions

Assets/Data/Json/Room/   13 authored room tilemaps as JSON
Assets/SO/               ScriptableObject assets (dungeon, stats, weapons, skills)
Assets/Scenes/           See the Scene Map in CLAUDE.md

design/          Game design documents (GDDs) and balance data
docs/            Architecture decision records, tech debt register, engine reference
production/      Sprints, epics, QA (bugs, triage, playtests), retrospectives
memory/          project_state.md — current code-state snapshot
tests/           EditMode / PlayMode / playtest — currently empty (see TD-014)
ToolExcel/       Stat formula spreadsheets (source of truth for balance numbers)
.claude/         Agent definitions, skills, and the coding rules in .claude/rules/
```

---

## Where to look for what

| Question | File |
|----------|------|
| How is the code organised, and what is broken right now? | [`CLAUDE.md`](CLAUDE.md) |
| What is actually implemented today? | [`memory/project_state.md`](memory/project_state.md) |
| How is a system supposed to work? | [`design/gdd/`](design/gdd/) — start with [`systems-index.md`](design/gdd/systems-index.md) |
| Why was an architectural choice made? | [`docs/architecture/`](docs/architecture/) |
| What shortcuts are we carrying? | [`docs/tech-debt-register.md`](docs/tech-debt-register.md) |
| What is being worked on this week? | [`production/sprint-status.yaml`](production/sprint-status.yaml) |
| What are the coding rules? | [`.claude/rules/`](.claude/rules/) |

---

## Conventions that will bite you

- **Preserve the intentional typos.** `EventManager.Resgister`, `INegativeReciver.cs`,
  `attackDamege`, `Modal` (= Model), `ENEBLE`, `CaculateIndex`, `currrentSA`, `deplayTime`
  are real contracts in code and in serialized `.asset` files. Renaming them breaks
  deserialization.
- **ScriptableObject first.** Gameplay numbers live in SO assets, never hardcoded.
- **State machines only.** New character behaviour is a new `PlayerState` / `EntityState`
  subclass — never an `if/else` branch in `Update()`.
- **No new singletons.** `MazeController` and `EnemyManager` are the only sanctioned ones
  (`LevelManager` is a known, unratified violation — TD-023).
- **All damage flows through `INegativeReceiver.TakeDamage(int, Vector2)`.** No script may
  mutate another entity's health directly.

---

## Project status

Combat, dungeon generation, room progression, and room-clear detection are implemented.
The enemy damage/death chain and the player death/restart chain are **not yet working** —
see the Known Bugs table in [`CLAUDE.md`](CLAUDE.md) for the current, source-verified list.
