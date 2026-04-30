# CLAUDE.md — Game-RPG (Action Roguelike)

## Project Overview

Unity action roguelike RPG. Combat phase inspired by **Cult of the Lamb**: top-down, real-time melee with directional attacks, ability slots, and per-run power progression. Each dungeon run consists of procedurally generated rooms; the player clears enemies to unlock doors to the next room.

**Demo target:** Functional dungeon run from start to boss — movement, melee combat, 2–3 active abilities, room-to-room progression, basic enemy AI, stat upgrades between rooms.

---

## Engine & Stack

- **Unity** (target: Unity 6 / 2022 LTS+)
- **C#** — all gameplay code
- **New Input System** — `PlayerInputHandler`
- **TextMesh Pro** — UI text
- **ScriptableObjects** — data-driven design for abilities, items, enemies, weapons, skills

---

## Repository Layout

```
Assets/
  Script/               # ALL active gameplay code
    Character/
      Player/           # Player state machine, input, sub/superstates
      Entity/           # Entity/Enemy state machine framework
      Core/             # Core component base classes (PlayerCore, EntityCore)
    Abilities/          # Ability system v2 — Core, Effects, Conditions, Runtime
    Combat/             # Health, Damageable, SimpleDamageReceiver
    Stats/              # CharacterStats, RuntimeStat, StatModifier
    Map/                # Maze generation, room/door controllers, cell logic
    Weapons/            # Weapon base + melee implementation
    Skill_Ability/      # Legacy ability system (ActivateSkill) — do not extend
    Enemy/              # EnemySO data + EnemyProjectile
    Item/               # Item ScriptableObjects, drop rate
    Interact/           # Interactive objects, weapon pickups
    Manager/            # EventManager, UIManager
    Handler/            # Event handler utilities
    Interface/          # IEffectable, IInteractable
    Stats/              # CharacterStats with runtime modifier system
    Pooling/            # Object pooling
  Prefab/               # All prefabs
    Map/                # Room, Cell, Direction prefabs
    Player/
    Enemy/
    Weapon/
    Item/
    UI/
    VFX/
  Sprite/               # All sprite assets
    Knight/             # Player knight sprites (SnS, Basic, etc.)
    Enemy/
    Map/
    Weapons/
    Items/
    VFX/
    UI/
  Animation/            # Animation clips
    Player/             # ← active (see DUPLICATE_NOTE.md)
    Player 1/           # ← possible duplicate, check DUPLICATE_NOTE.md
    Enemy/
  AnimationController/  # Animator Controllers
  ScriptableObjects/    # (SO/) — game data assets
  Fonts/                # Font assets (TTF)
  Scenes/               # .unity scene files only — no scripts or prefabs here
  Backup/               # Pending cleanup — see items below
Packages/               # Unity package manifest
```

### ⚠️ Backup — Còn lại cần quyết định

Sau khi tái cơ cấu, `Assets/Backup/` chỉ còn:

| File | Trạng thái | Cần làm |
|------|-----------|---------|
| `Backup/Enemy/NewEnemy.cs` + 2 stubs | Empty stubs, không có logic | **Hỏi để xóa** |
| `Backup/Dungeon/Door.cs` | Superseded bởi `DoorController.cs` | **Hỏi để xóa** |

### ⚠️ Cần quyết định thêm

| Folder | Vấn đề | Cần làm |
|--------|--------|---------|
| `Animation/Player` vs `Animation/Player 1` | 155 file trùng nhau | Xem `Animation/DUPLICATE_NOTE.md` |
| `Sprite/Player/ExMovement/` | Sprites platformer không dùng trong top-down | Hỏi để xóa |
| `TextMesh Pro/Examples & Extras/` | Không cần trong production | Hỏi để xóa |
| `Skill Enhance/` | Đã empty sau khi merge | Xóa folder |

---

## Architecture

### Player State Machine

Entry point: `NewPlayer.cs` — hosts `PlayerStateMachine`, fires state transitions.

```
PlayerState (base)
  PlayerBasicState          — idle / move
    PlayerIdleState
    PlayerMoveState
  PlayerUseWeaponState      — weapon actions
    PlayerAttackState
    PlayerSkillWeaponState
  PlayerEquidUnequid
  PlayerIntertorState
  PlayerTakeDamageState
```

States communicate via `NewPlayer` ref; input comes from `PlayerInputHandler`.

### Ability System (v2 — use this for new ability work)

Located in `Assets/Script/Abilities/`.

```
AbilitySystem          — MonoBehaviour on Player; 4 slots (Primary/Secondary/Utility/Ultimate)
AbilityDefinition      — ScriptableObject: activation type, cooldown, mana cost, conditions[], effects[]
AbilityInstance        — Runtime wrapper: tracks cooldown, hold time, validates conditions
AbilityConditionDefinition  — SO: pluggable condition (e.g. HasEnoughMana, NotDead)
AbilityEffectDefinition     — SO: pluggable effect (e.g. DamageInFront, LungeForward)
```

Add new abilities by creating `AbilityDefinition` assets and wiring `AbilityEffectDefinition` implementations. Do **not** extend the legacy `ActivateSkill` system for new features.

### Combat Flow

1. `PlayerInputHandler` detects attack input → `PlayerAttackState` entered
2. `WeaponMelee` fires: calculates attack position from mouse direction, runs `Physics2D.OverlapCircleAll` with layer mask
3. Damage applied via `Damageable` interface → `Health.TakeDamage()`
4. Combo counter increments; resets after `comboResetTime`
5. `PlayerTakeDamageState` handles knockback when player receives damage

### Map / Dungeon Generation

```
MazeGenerator      — DFS maze algorithm → Cell grid
MazeController     — Singleton orchestrator (Awake initialises maze)
CellMapController  — Logical grid (Cell.STATUS_DOOR per side)
RoomMapController  — Spawns RoomController prefabs per cell
RoomController     — Positions room, configures DoorController × 4
DoorController     — Trigger → EventManager.Emit(ON_PLAYER_ON_DOOR)
```

Room transitions are event-driven. `MainMapController` listens to `ON_PLAYER_ON_DOOR` and handles scene/room switching.

### Event System

Static dispatcher at `EventManager`. Two events currently:
- `EventID.ON_PLAYER_ON_DOOR` — player steps into door trigger
- `EventID.ON_LOAD_MAP` — map generation complete

Register: `EventManager.Register(EventID, callback)`  
Emit: `EventManager.Emit(EventID, data)`

### Stats & Health

`CharacterStats` holds `RuntimeStat` fields (Attack, MoveSpeed, MaxMana, etc.).  
`RuntimeStat` supports flat + percent modifiers from any source; call `AddModifier` / `RemoveModifier`.  
`Health` is a simple component: `TakeDamage(float)`, `IsDead` property.

---

## Coding Conventions

- **No comments** unless the WHY is non-obvious (hidden constraint, workaround, tricky invariant).
- **ScriptableObject-first**: game data (abilities, enemies, items, weapons) lives in SO assets, not hardcoded.
- **No singletons except MazeController** — use dependency injection or component `GetComponent` lookups.
- **State machine for player**: new player behaviours = new state, not inline `if/else` in `Update`.
- **Ability effects = new `AbilityEffectDefinition` subclass** — keep effect logic isolated and composable.
- **Layer masks** must be configured in Inspector; never hardcode layer indices.
- **Backup/** is read-only reference — never add code there, never call its classes from new code.

---

## Systems Status

### Implemented & Active
- [x] Player state machine (idle / move / attack / skill / damage)
- [x] New Input System integration
- [x] Directional melee combat with combo
- [x] Ability system v2 (conditions + effects, 4 slots)
- [x] DashAbility, SlashAbility (projectile), BlockAbility
- [x] DamageInFrontEffect, LungeForwardEffect
- [x] CharacterStats with runtime modifiers
- [x] Health / Damageable interface
- [x] Procedural maze (DFS) + room spawning
- [x] Door trigger → room transition events
- [x] Item ScriptableObjects + drop rate system
- [x] Talent tree nodes (InternalSkillSO)
- [x] Object pooling (basic)
- [x] Projectile system (slash, enemy ranged)

### Incomplete / Missing (Demo Priority)
- [ ] Enemy AI — pathfinding + attack behaviours (HIGH)
- [ ] UI — health bar, mana bar, ability cooldown icons (HIGH)
- [ ] Between-room upgrade / power selection screen (HIGH)
- [ ] Audio — SFX + music (MEDIUM)
- [ ] Boss room definition (MEDIUM)
- [ ] Save / checkpoint (LOW for demo)
- [ ] Inventory UI (LOW for demo)

---

## Enemy Definitions

Enemy data lives in `EnemySO` assets. Fields: name, level, speed, FOV range, attackRate, attackRange, damage, projectile, drop item pool.

Available animation rigs: **Bat, Crab, Golem (3 phases), Pebble, Rat, Skull, Spiked Slime**.

---

## Scene Map

| Scene | Purpose |
|-------|---------|
| `StartScene` | Main menu entry point |
| `Level 1` | Primary dungeon level |
| `DungeonStart` | Dungeon intro area |
| `RandomMaze` | Procedural maze runtime test |
| `SampleScene` | Dev sandbox |
| `Test AI` | Enemy AI experimentation |

---

## Demo Completion Checklist

To ship a playable demo:

1. **Enemy AI** — implement patrol → chase → attack loop for ≥2 enemy types using the existing `EnemySO` data.
2. **Room clear condition** — lock doors when enemies spawn; unlock on all-dead.
3. **Between-room screen** — offer player 3 random stat upgrades (`RuntimeStat` modifiers) after clearing a room.
4. **HUD** — health bar, mana bar, ability slot icons with cooldown overlay.
5. **Boss room** — one larger room with a Golem enemy that has multi-phase behaviour.
6. **Death / restart flow** — game-over screen → return to StartScene.
7. **Audio pass** — footstep, attack, hit, and ambient SFX (can use free assets).

---

## Do Not Touch / Pending Cleanup

- `Script/Skill_Ability/` — legacy ability system (ActivateSkill, DashAbility, etc.), do not extend
- `Backup/Enemy/` — 3 empty stub files, pending deletion decision
- `Backup/Dungeon/Door.cs` — superseded by `Script/Map/Room/Door/DoorController.cs`, pending deletion
- `Skill Enhance/` — empty folder after merge, safe to delete via Unity Editor
