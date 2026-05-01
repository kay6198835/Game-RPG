# Technical Preferences

## Engine & Language

- **Engine**: Unity 2022.3.62f3 LTS
- **Language**: C# (.NET Standard 2.1)
- **Rendering**: Universal Render Pipeline (URP) — 2D Renderer
- **Physics**: Physics2D (Box2D)

## Input & Platform

- **Target Platforms**: PC (Windows)
- **Input Methods**: Keyboard + Mouse
- **Primary Input**: Keyboard (WASD movement) + Mouse (attack direction)
- **Gamepad Support**: None (demo target)
- **Touch Support**: None

## Naming Conventions

- **Classes**: `PascalCase` — `PlayerAttackState`, `EntityWeaponMelee`
- **Private fields**: `_camelCase` (or `camelCase` for serialized legacy fields — match existing)
- **Public properties**: `PascalCase`
- **Constants**: `UPPER_SNAKE_CASE` in `GameConstants.cs`
- **ScriptableObject assets**: `PascalCase` — `BatEntityData`, `SwordAttackSO`
- **Prefab assets**: `PascalCase` — `EnemyBat`, `RoomBasic`
- **Scene files**: `PascalCase` — `RandomMaze`, `StartScene`

## Performance Budgets

- **Target Framerate**: 60 FPS
- **Frame Budget**: 16.7ms total
- **Enemy update budget**: max 0.1ms per enemy per frame
- **Physics queries**: NonAlloc variants only in hot paths
- **Draw Calls**: max 100 per frame (2D game)
- **Memory Ceiling**: 256MB

## Testing

- **Framework**: Unity Test Framework (EditMode + PlayMode)
- **Test location**: `tests/EditMode/`, `tests/PlayMode/`
- **Minimum Coverage**: Core damage chain + state transitions (blocking gate)
- **Required Tests**: Damage formula, player death, room clear trigger

## Forbidden Patterns

- `GameObject.Find()`, `FindObjectOfType()`, `SendMessage()` in production code
- `new` allocations in `Update()`, `LogicUpdate()`, `PhysicsUpdate()`
- `Resources.Load()` — use direct references or Addressables for future growth
- Hardcoded layer indices — always use LayerMask set in Inspector
- New singletons beyond `MazeController`
- `public` fields on MonoBehaviours — use `[SerializeField] private`

## Allowed Libraries / Addons

- Input System 1.14.0 (already in project)
- TextMeshPro 3.0.7 (already in project)
- 2D Feature Pack (already in project)
- Visual Scripting 1.9.4 (already in project)

## Architecture Decisions Log

- State machine pattern for all characters (Player + Enemy) — enforced
- ScriptableObject-first data model — enforced
- EventManager static bus for cross-system events — enforced
- INegativeReciver interface for damage — enforced
- No new singletons beyond MazeController — enforced

## Engine Specialists

- **Primary**: unity-specialist
- **Shader/VFX Specialist**: unity-shader-specialist
- **UI Specialist**: unity-ui-specialist
- **DOTS Specialist**: unity-dots-specialist (consult only — no ECS in current build)
- **Addressables**: unity-addressables-specialist (future growth)

### File Extension Routing

| File Extension / Type | Specialist to Spawn |
|-----------------------|---------------------|
| `*.cs` game code | unity-specialist → gameplay-programmer |
| `*.cs` AI/enemy code | unity-specialist → ai-programmer |
| `*.cs` UI code | unity-ui-specialist |
| `*.cs` map/dungeon code | engine-programmer |
| `*.shader` / Shader Graph | unity-shader-specialist |
| `.unity` scene files | level-designer |
| `.asset` ScriptableObjects | systems-designer |
| Architecture review | technical-director → unity-specialist |
