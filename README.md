# Game-RPG

A Unity 2D top-down action roguelike with real-time directional melee combat, weapon-linked
skills, and procedurally generated dungeon rooms. Clear the enemies to unlock the doors to
the next room.

<!-- TODO: replace with a real 10-20s gameplay GIF. This is the single highest-impact
     thing on this page — a game repo without a moving image gets skipped. -->
![Gameplay](docs/images/gameplay.gif)

**[▶ Play in browser](#)** · *(TODO: itch.io WebGL build link)*

---

## My role

Solo project — I designed and implemented every system in `Assets/Script/`: the combat and
state-machine framework, the A\* pathfinding, the stat system, procedural dungeon generation,
the enemy AI, and the dependency-injection wiring.

Third-party assets used: DOTween (tweening), VContainer (DI), and the sprite/animation art
listed in [`CREDITS.md`](CREDITS.md).

**Development note:** AI-assisted tooling (Claude Code) was used for documentation, code
review, and refactoring passes. All architecture and gameplay systems are my own design and
implementation.

---

## Technical highlights

The parts of this project I would most want to talk through in an interview:

### Custom A\* pathfinding with a frame-budgeted request queue
`Assets/Script/Pathfinding/`

Rather than using Unity's NavMesh, enemy navigation runs on a hand-written A\* over a tilemap
grid: a binary-heap [`PriorityQueue`](Assets/Script/Pathfinding/Algorithm/PriorityQueue.cs),
an octile [`Heuristic`](Assets/Script/Pathfinding/Algorithm/Heuristic.cs) for 8-way movement,
and a [`PathRequestManager`](Assets/Script/Pathfinding/PathRequestManager.cs) that caps
searches at `maxRequestsPerFrame` so a room full of enemies cannot spike the frame time.

### Data-driven derived stat system
`Assets/Script/StatSystem/`

Primary stats (STR/DEX/INT/VIT/LUK) feed derived stats (HP, damage, crit…) through
[`DerivedStatFormula`](Assets/Script/StatSystem/DerivedStatFormula.cs):

```
BaseValue = baseConstant + level × perLevel + Σ(sourceStat × coefficient)
```

Every coefficient is authored in the Inspector, so balancing never touches code. Buffs and
equipment apply as source-tagged [`StatModifier`](Assets/Script/StatSystem/StatModifier.cs)
bundles — equipping a weapon stamps its modifiers with the weapon as the source, and
unequipping removes exactly that source by reference. The balance numbers are modelled in
[`ToolExcel/`](ToolExcel/) and mirrored into the ScriptableObject assets.

### Generic state machine shared by player and enemies
`Assets/Script/Character/`

One generic [`StateMachine<TState>`](Assets/Script/Character/Base/StateMachine.cs) drives both
the player and every enemy. Animation hand-off runs through a `StatusAnimation` enum written
by Unity Animation Events, so states react to animation phases (`OnActivate` = the hit frame)
instead of guessing with timers. Combat behaviour is added by writing a new state subclass,
never an `if/else` branch in `Update()`.

### Procedural dungeon generation
`Assets/Script/Map/`

A DFS maze generator lays out the room graph, rooms are authored in a custom in-editor level
tool and loaded from JSON, and door tiles are swapped at load time to match the maze topology.
Room-clear state is tracked by a live enemy count that unlocks the doors when it hits zero.

### Zero-allocation combat and dependency injection

Melee hit detection uses `Physics2D.OverlapCircleNonAlloc` into a buffer pre-allocated in
`Awake()` ([`MeleeWeapon.cs`](Assets/Script/Weapons/MeleeWeapon/MeleeWeapon.cs)), enemies and
projectiles are pooled, and cross-system services are injected via VContainer behind
interfaces (`IObjecPoolService`, `IPlayerStatService`) instead of singletons.

---

## Tech stack

| | |
|---|---|
| **Engine** | Unity 2022.3.62f3 LTS — URP, 2D Renderer |
| **Language** | C# — ~10k lines across 174 files |
| **Input** | Unity Input System 1.14 |
| **DI** | VContainer 1.19 |
| **Tweening** | DOTween |
| **UI** | UI Toolkit (menus) + UGUI/TextMeshPro (HUD) |

---

## Running it

```
Unity Hub → Open → this folder      (Unity 2022.3.62f3 LTS)
Open scene: Assets/Scenes/Main/Test/LoadRandomMap.unity
Press Play
```

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

## Project structure

```
Assets/Script/
  Character/Base/      Shared component hub + generic state machine
  Character/Player/    Player controller, states, core components
  Character/Entity/    Enemy AI framework (mirrors the player pattern)
  Weapons/             Weapon base, melee, ranged, combo stages as SOs
  Skill_Ability/       Hold-release ability lifecycle (ScriptableObjects)
  StatSystem/          Primary/derived stats, modifiers, formulas
  Pathfinding/         A*, grid builder, request manager
  Map/                 Maze generation, room grid, doors, minimap
  Enemy/               Spawning, weighted rarity selection
  LifetimeScope/       VContainer DI setup + pooled object service
  LevelEdit/           Custom in-editor room authoring tool
  Manager/             Static event bus
```

Deeper technical notes, conventions, and the current bug list live in
[`CLAUDE.md`](CLAUDE.md); design documents are in [`design/gdd/`](design/gdd/) and
architecture decisions in [`docs/architecture/`](docs/architecture/).

---

## Roadmap

Combat, dungeon generation, room progression, enemy spawning, and room-clear detection are
implemented and playable. Currently in progress:

- [ ] Death → game-over → restart flow
- [ ] Player health/mana HUD
- [ ] Build-safe room loading (move room JSON off `Application.dataPath`)
- [ ] Between-room upgrade cards
- [ ] EditMode unit tests for the stat formulas
