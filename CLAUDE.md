  # CLAUDE.md

  This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

  ## Engine Version Reference

  @docs/engine-reference/unity/VERSION.md

  ---

  ## Project Overview

  Unity action roguelike RPG. Combat inspired by **Cult of the Lamb**: top-down, real-time melee with directional attacks, weapon-linked skills, and per-run power progression. Procedurally generated rooms — clear enemies to unlock doors to the next room.

  **Demo target:** Full game life cycle — start menu → dungeon run (movement + melee combat + 2 skills + enemies + room progression) → death/restart. Focus: combat system only.

  ---

  ## Unity Environment

  - **Unity:** 2022.3.62f3 LTS
  - **Key packages:** Input System 1.14.0, TextMeshPro 3.0.7, 2D Feature Pack, Visual Scripting 1.9.4
  - **Open project:** Unity Hub → Open → `d:/Fork/Roughlike/Game-RPG`
  - **Main dev scene:** `Assets/Scenes/Main/Test/RandomMaze.unity`
  - **Play/test:** Enter Play Mode in the Unity Editor — no separate build step for development
  - **Compile check:** Any `.cs` edit triggers auto-recompile; errors appear in the Console window
  - **IDE:** Open `Game-RPG.sln` in Rider or Visual Studio for IntelliSense

  There are no standalone build, lint, or test CLI commands — all development happens through the Unity Editor.

  ---

  ## Repository Layout

  ```
  Assets/
    Script/                                     # ALL active code — single source of truth
      Character/
        Player/
          NewPlayer.cs                          # Player MonoBehaviour (class inside renamed `Player`) — state machine host
          PlayerState.cs                        # Base state: animBool, anim events, startTime
          PlayerStateMachine.cs                 # Initialize/ChangeState
          PlayerData.cs                         # SO: maxHealth, currentHealth, movementVelocities, Reborn()
          Input/
            PlayerInput.cs                      # Auto-generated InputActionAsset
            PlayerInputHandle.cs                # New Input System; 8-direction angle calc; all flags
          Core/
            Core.cs                             # Component hub: List<CoreComponent> + AddCoreComponent()/GetCoreComponent<T>() — old Inspector refs commented out; TakeDamage() removed
            CoreCompoment.cs                    # Base for core components (gets Core from parent)
          CoreComponent/
            PlayerMovement.cs                   # rb.velocity wrapper
            WeaponHolder.cs                     # Equip/UnEquip weapon
            AbilityHolder.cs                    # Skill state machine Start→Cast→Do→Exit per frame
            Interact.cs                         # Base: OverlapCircleNonAlloc + nearest-by-mouse
            Interactor.cs                       # FindInteraction via OverlapCircle
            NegativeReciver.cs                  # ⚠️ CoreComponent implements INegativeReceiver — TakeDamage() throws NotImplementedException (moved from Interface/ 2026-07-01)
          States/                               # All player states (sub + super flattened)
            PlayerBasicState.cs                 # Shared: attack/skill/equip/interact/damage transitions
            PlayerUseWeaponState.cs             # Freezes movement, exits on animFinish
            PlayerDisadvantageState.cs
            PlayerIdleState.cs, PlayerMoveState.cs
            PlayerAttackState.cs                # Calls Weapon.Attack() on AnimationTrigger anim event
            PlayerSkillWeaponState.cs           # Drives AbilityHolder each frame
            PlayerEquidUnequid.cs, PlayerIntertorState.cs, PlayerTakeDamageState.cs
          Projectile/
            Projectile.cs                       # Raycast hit → INegativeReceiver.TakeDamage()
            Spell.cs                            # Extends Projectile; also calls IEffectable.ApplyEffect()
        Entity/                                 # Enemy AI framework
          Entity.cs                             # Enemy MonoBehaviour: state machine host
          EntityData.cs                         # SO: stats, layerMask, FOV, attack range, WeaponSO
          EntityInput.cs                        # Auto-detects player via OverlapCircle each frame
          EntityStateMachine.cs, EntityState.cs, EntityStatsSO.cs
          EntityWeaponMelee.cs                  # COMPLETE Attack() with INegativeReceiver damage
          Core/
            EntityCore.cs                       # Component hub for enemies (implements INegativeReceiver)
            EntityCoreComponent.cs
          CoreComponent/
            EntityMovement.cs, EntityFindTarget.cs
            EntityWeapon.cs, EntityWeaponHolder.cs, EntityEffectStats.cs
          States/                               # All entity states (super + sub flattened)
            EntityBasicState.cs                 # Transitions: direction, take-damage, attack check
            EntityIdleState.cs                  # Idle timer or target detected → MoveState
            EntityMoveState.cs                  # Move toward player; wall avoidance; timer → IdleState
            EntityAttackState.cs                # Triggers weapon.Attack() on anim event
            EntityTakeDamageState.cs
            EntityDeathState.cs                 # ⚠️ wrong base class (MonoBehaviour), fix needed
            EntityUseWeaponState.cs, EntityDisadvantageState.cs
        StatsCharacter.cs                       # SO base: blockDMG, maxMana, maxHealth, AnimatorController

      Enemy/
        EnemySO.cs                              # Data-only SO: speed, FOV, damage, projectile, drops
        NewEnemy.cs                             # Extends Entity — ready to wire up

      Interface/
        IInteractable.cs, IEffectable.cs
        INegativeReciver.cs                     # filename kept old typo; interface name is INegativeReceiver — TakeDamage(int amountDamage, Vector2 attackPosition)

      Map/
        BaseGrid.cs                             # Generic MonoBehaviour grid: AddCell, Setting, GetNext, CaculateIndex
        BaseCell.cs                             # Abstract cell base: AddCell, GetGridPosition, Setting()
        Interface/
          IGrid.cs                              # IGrid / IGrid<T>: Columns, Rows, AddCell, Setting, GetNext
          IGridItem.cs                          # IGridItem: AddCell, GetGridPosition
        Maze/
          MazeGenerator.cs                      # DFS; random Start cell, last-visited End cell; public Start/End
          MazeController.cs                     # Singleton; wires MapGrid + RoomGrid; GetCellStart() / GetCellEnd()
        Cell/
          Cell.cs                               # Data class: Row, Col, Doors dict (string→STATUS_DOOR)
          MapCell.cs                            # Minimap cell MonoBehaviour (extends BaseCell) — wall visibility per door
          MapGridController.cs                  # ✅ Minimap grid (extends BaseGrid<MapCell>) — Move/OnLoadMap implemented, avatar tween via DOTween (2026-07-02)
        Room/
          RoomCell.cs                           # World room cell: doors, StartDoorPosition, IsCleared, ClearRoom(), OpenDoors()
          RoomGridController.cs                 # ✅ Room grid event hub: OnLoadMap(); listens ON_LOAD_MAZE_DONE / ON_PLAYER_ON_DOOR / ON_CLEAR_ENEMY; delegates to RoomGeneraterController
          RoomGeneraterController.cs            # ✅ Tilemap loader (same GameObject, RequireComponent): Setting() random room pick, LoadRoom(), ClearRoom(), DeleteDoorTileMap(), SwapTileMap()
          Door/
            DoorController.cs                   # ✅ Manages door collider; collider enabled only when Status==OPEN; Emit ON_PLAYER_ON_DOOR on player touch
        Legacy/ Door.cs, Room.cs                # Superseded — do not use

      LevelEdit/                                # Editor + runtime room loading
        LevelEditor.cs                          # Stub — Tilemap/Camera refs, Update() empty
        LevelManager.cs                         # Singleton ⚠️; SaveLevel() / ImportRoomJsonFiles() (editor); GetDungeonRoomSO() / GetRandomRooms() (runtime); GetTileSOs() / GetTilemaps() for RoomGridController

      StatSystem/                               # NEW 2026-06-30 → 07-01 — RPG stat framework, chưa được gameplay tiêu thụ (GDD: design/gdd/stat-system.md; số liệu: ToolExcel/stat_system_formula_reference.xlsx)
        StatType.cs                             # Enum: primary STR/DEX/INT/VIT/LUK (0-4); derived MaxHP/MaxMana/PhysicalDamage/MagicDamage/Defense... (100+)
        Stat.cs, StatModifier.cs                # Base value + modifier stack
        DerivedStatFormula.cs                   # Formula mapping primary → derived
        StatsSO.cs                              # SO "Game/Stats Profile": level-driven RecalculateDerived(), OnStatChanged event

      Handler/
        EventHandler/                           # EMPTY folder — WIP placeholder

    SO/
      Dungeon/
        DungeonRoomSO.cs                        # SO: List<RoomFile> (roomName, filePath, roomType)
        TileSO.cs                               # SO: tile id → TileBase mapping
        Maze_Storage.asset                      # Full room pool (all authored rooms)
        Maze_Load_Room.asset                    # Runtime pool (subset selected per run)

    Editor/
      LevelManagerEditor.cs                     # Custom Inspector button for LevelManager

    Data/
      Json/Room/                                # Room tilemap data: NormalRoom_0.json … NormalRoom_12.json (13 rooms)

      Weapons/
        Weapon.cs (abstract base), WeaponStats.cs, WeaponType.cs
        MeleeWeapon/
          WeaponMelee.cs                        # Combo attack done 2026-06-22 (AttackState list, SetAnimation, DurationNextAttack) — ⚠️ Attack() foreach still EMPTY, no damage applied
          SwordAndShield.cs                     # Extends WeaponMelee — empty stub
          AttackSO.cs                           # SO: attackRange, attackDamege, animatorOV
          WeaponMeleeStats.cs
          PlayerCombat.cs                       # Legacy class — all code commented out, do not use
        RangeWeapon/  RangeWeapon.cs, bullet.cs, Shooting.cs, BulletDataSO.cs

      Skill_Ability/
        ActivateSkill.cs                        # Base skill SO: Enter/Activate/Cast/Do/Exit lifecycle
        DashAbility.cs, SlashAbility.cs, BlockAbility.cs
        DualAbility.cs                          # Extends ActivateSkill — all code commented out (WIP)
        EffectSkillSO.cs (abstract), EffectSkillDuringTime.cs, EffectSkillOneTime.cs
        InternalSkillSO.cs                      # Talent tree node SO

      Manager/
        EventManager.cs                         # Static bus: Resgister / UnResgister / Emit; EventID enum: ON_PLAYER_ON_DOOR, ON_LOAD_MAP, ON_LOAD_MAZE_DONE
        AnimationEventManager.cs                # AnimationEventId enum: StartAnimation, MoveAnimation, AttactAnimation, DoSkillAnimation, EndAnimation
        UI/UIManager.cs                         # EMPTY STUB

      Item/, Interact/, Pooling/, MainMenu/
      Utility/
        GameConstants.cs                        # Direction vectors, tile names, room scale constants (GAME_SCALE=3, LENGTH_ROOM=10)
        Utility.cs                              # Static helpers: PickUniqueIndex, ToCardinalDirection, SealUnusedDoors, GetDoorWorldPosition
        DirectionTarget.cs                      # Direction targeting helper

      Enemy/
        EnemySO.cs                              # Data-only SO: speed, FOV, damage, projectile, drops
        NewEnemy.cs                             # Extends Entity — body empty, needs prefab wiring
        NewEnemyState.cs                        # ⚠️ extends MonoBehaviour instead of EntityState — stub
        NewEnemyStateMachine.cs                 # Stub

      Character/Player/
        Animation/
          AnimationName.cs                      # Animation string constants
          AnimationPlayerController.cs          # ⚠️ OnEnable registers StartAnimation twice (EndAnimation callback bug)
        States/
          PlayerUserItemState.cs                # Stub — extends MonoBehaviour (wrong base class)
        CoreComponent/
          TalentManager.cs                      # Prototype: strength/dex/int/cha/skillPoint hardcoded in Awake, not SO-driven

    Prefab/       Player/, Weapon/, Enemy/, UI/, Particle Effect/, Item/, ...
    Sprite/       Knight/, Enemy/, Map/, Weapons/, Items/, VFX/, UI/
    Animation/    Player animation clips
    AnimationController/
    ScriptableObjects/
    Scenes/       StartScene, Level 1, DungeonStart, RandomMaze, Test AI, SampleScene

  ToolExcel/      stat_system.xlsx, stat_system_v1.xlsx, stat_system_formula_reference.xlsx — stat formula emulators: player / 5 creep types / boss (repo root, outside Assets/). `_formula_reference` = source of truth for per-entity base/perLevel/coefficients; described generally in design/gdd/stat-system.md
  ```

  ---

  ## Architecture

  ### Player State Machine

  [NewPlayer.cs](Script/Character/Player/NewPlayer.cs) creates all states in `Awake`, ticks `CurrentState.LogicUpdate()` in `Update`.

  ```
  PlayerState (base)
    PlayerBasicState          — shared transitions: attack / skill / equip / interact / take-damage
      PlayerIdleState / PlayerMoveState
    PlayerUseWeaponState      — freezes movement; exits on animFinish
      PlayerAttackState       — calls Weapon.Attack() on AnimationTrigger anim event
      PlayerSkillWeaponState  — calls AbilityHolder.SetStateAbility() on AnimationTrigger
      PlayerEquidUnequid / PlayerIntertorState
    PlayerDisadvantageState
      PlayerTakeDamageState
  ```

  [Core.cs](Script/Character/Player/Core/Core.cs) is the component hub: components self-register via `AddCoreComponent()`, consumers resolve via `GetCoreComponent<T>(out var comp)`. The old Inspector-wired `Movement`/`WeaponHolder`/`AbilityHolder`/`Interactor` properties are commented out (2026-07-02).

  ### Enemy AI Framework

  [Entity.cs](Script/Character/Entity/Entity.cs) mirrors the player pattern. All config via `EntityData` SO.

  ```
  EntityBasicState   — direction tracking + transition checks (attack range, take-damage)
    EntityIdleState  — idle timer or target detected → MoveState
    EntityMoveState  — moves toward player; wall avoidance via Raycast; timer → IdleState
  EntityUseWeaponState → EntityAttackState  — triggers EntityWeaponMelee.Attack()
  EntityDisadvantageState → EntityTakeDamageState
  ```

  `EntityInput.Update()` auto-detects player via `Physics2D.OverlapCircle` every frame — no manual target assignment.  
  `EntityCore` implements `INegativeReceiver`; taking damage reduces `EntityStatsSO.ModifiersHealth`.  
  `EntityWeaponMelee.Attack()` is **fully implemented** — `OverlapCircle` + `INegativeReceiver.TakeDamage()`.

  **Wiring a new enemy prefab:** `Entity` + `EntityCore` (child) + `EntityInput` (child) + `EntityMovement`, `EntityFindTarget`, `EntityWeaponHolder`, `EntityEffectStats` (grandchildren of EntityCore). Assign `EntityData` SO.

  ### Weapon-Linked Skill System

  `ActivateSkill` SO lifecycle, driven by `AbilityHolder` each frame:
  ```
  Enter(player) → Activate() → Cast() [button held] → Do() [button released] → Exit()
  ```
  `WeaponMeleeStats` carries two SO slots: `AbilityWeapon` (RMB/Block) and `SkillWeapon` (E key).

  ### Damage Chain

  ```
  # Player hits enemy
  PlayerAttackState → AnimTrigger → WeaponMelee.Attack()
    → Physics2D.OverlapCircleAll()  ← circle fires correctly
    → foreach body EMPTY           ← ⚠️ INegativeReceiver.TakeDamage() call missing

  # Enemy hits player
  EntityAttackState → AnimTrigger → EntityWeaponMelee.Attack()
    → Physics2D.OverlapCircle → INegativeReceiver.TakeDamage()
    → NegativeReciver.TakeDamage()  ← ⚠️ throws NotImplementedException — Core.TakeDamage() removed (Bug #6)

  # Projectile hits anything
  Projectile.CheckCollisions() → Raycast → INegativeReceiver.TakeDamage()
  ```

  ### Map / Dungeon Generation

  ```
  MazeController.Awake() [singleton]
    → MazeGenerator.Generator(Rows, Cols)
        DFS từ random start cell → Cell[rows*cols]
        Start = random cell; End = last visited cell
    → SetCellData: for each Cell → MapGrid.AddCell() + RoomGrid.AddCell()
    → MapGrid.Setting() / RoomGrid.Setting()
        RoomGridController.Setting() → RoomGeneraterController.Setting():
          → GetCellStart/End → _startIndex / _endIndex
          → LevelManager.GetDungeonRoomSO() → _fullDungeonRoomSO (full pool)
          → Utility.PickUniqueIndex(total, mazeSize) → randomMazeRoomsIndex
          → gán random rooms; force [_startIndex]=room[0], [_endIndex]=room[last]
    → EventManager.Emit(ON_LOAD_MAZE_DONE)

  IGrid<T> — interface chung cho BaseGrid<T>
    AddCell(cell)           Instantiate prefab cell
    Setting(cols, rows)     lưu Columns/Rows
    GetNext(direction)      flip Y → index = y*Columns+x → trả về _next
    GetValue(int)           truy cập _list trực tiếp
    CaculateIndex(Vector2)  row*Columns + col

  RoomGridController [ON_LOAD_MAZE_DONE] → OnDoneLoadRoomGrid()
    → LoadRoom(_startIndex, _current)
    → fastMovement.position = _current.StartDoorPosition

  RoomGeneraterController.LoadRoom(index, roomCell):
    → đọc JSON từ _dungeonRoomSO.room[index].filePath (hoặc dùng cached data nếu IsCleared)
    → clear tilemaps
    → for each tile:
        if DOOR tile && !IsCleared:
          direction = Utility.ToCardinalDirection(tilePos)
          if direction IN roomCell.ListDirectionDoors → giữ door; lưu vào DoorPoints + CurentDoorLevelData
          else → swap sang ROOM tile (wall); lưu vào SwapLevelData
    → SetTile trên _genmap[layerIdx]
    → roomCell.SetDoorPoints(DoorPoints) → reposition DoorController transforms

  DoorController.OnTriggerEnter2D()    tag=="Player" && Status==OPEN → Emit(ON_PLAYER_ON_DOOR, dir)

  RoomGridController [ON_PLAYER_ON_DOOR] → ClearRoom(direction):
    → cache state: Data, CurentDoorLevelData, DoorPoints → RoomCell.IsCleared = true
    → clear tilemaps; clear SwapLevelData / DoorPoints
    → OnLoadMap(direction)

  RoomGridController.OnLoadMap(direction):
    → _next = GetNext(direction)
    → LoadRoom(index, _next)
    → _next.GetStartDoorPosition(-direction)   [open entry door; calc StartDoorPosition]
    → _current.UpdateStatusDoor(direction)     [open exit door]
    → fastMovement.position = _next.StartDoorPosition
    → _current = _next
    → Emit(ON_LOAD_MAP, index)

  RoomGridController [ON_CLEAR_ENEMY] → DeleteDoorTileMap():
    → xóa CurentDoorLevelData tiles khỏi tilemap (lộ cửa ra)
    → RoomCell.OpenDoors() → tất cả door SetStatus(OPEN)

  MapGridController [ON_PLAYER_ON_DOOR → Move / ON_LOAD_MAZE_DONE → OnLoadMap]
    → ✅ avatar tween theo phòng hiện tại (DOTween); MapCell.VisitRoom() hiện cell đã thăm
  ```

  **STATUS_DOOR semantics:**
  | Value | Tên | Ý nghĩa | Collider |
  |-------|-----|---------|----------|
  | 0 | `DISABLE` | Hướng này không có door trong maze | Off |
  | 1 | `ENEBLE` | Door tồn tại, đang bị khóa | Off |
  | 2 | `BE_OPEN` | Receiver side của passage | Off |
  | 3 | `OPEN` | Passable — player đi qua được | **On** |
  | 4 | `CLOSE` | Tường kín runtime | Off |

  **Setup quan trọng (Inspector):** `RoomGridController` cần wire: `_fastMovement`, `_dungeonRoomSO` (= `Maze_Load_Room.asset`), `_fullDungeonRoomSO` (= `Maze_Storage.asset`), `_listTiles`, `_genmap` (list Tilemap layers). `LevelManager` phải có trong scene với `dungeonRoomSO` = `Maze_Storage.asset`.

  ### Event System

  ```csharp
  EventManager.Resgister(EventID.ON_PLAYER_ON_DOOR, callback);  // note: typo in source — use as-is
  EventManager.Emit(EventID.ON_PLAYER_ON_DOOR, (Vector2)direction);
  ```
  Events currently defined: `ON_PLAYER_ON_DOOR`, `ON_LOAD_MAP`, `ON_LOAD_MAZE_DONE`, `ON_CLEAR_ENEMY` (enemy all dead → open doors), `ON_TEST` (debug only). Events needed but not yet added: `ON_ENEMY_DEATH`, `ON_PLAYER_DEATH`, `ON_PLAYER_TAKE_DAMAGE`, `ON_ROOM_CLEAR`.

  ---

  ## Known Bugs (block demo)

  | # | Severity | Status | Description | Location |
  |---|----------|--------|-------------|----------|
  | 1 | COMPILE | ✅ SUPERSEDED | `RoomMapController` — class đã bị xóa, thay bởi `RoomGridController` (2026-06-04) | — |
  | 2 | COMPILE | ✅ SUPERSEDED | `MainMapController` — class đã bị xóa, logic chuyển vào `RoomGridController` (2026-06-04) | — |
  | 3 | LOGIC | ✅ SUPERSEDED | `MainMapController.Start()` — class không còn tồn tại (2026-06-04) | — |
  | 4 | LOGIC | ⚠️ OPEN | `WeaponMelee.Attack()` — `OverlapCircleAll` runs but `foreach` body is empty, no `INegativeReceiver.TakeDamage()` call | [WeaponMelee.cs:29](Assets/Script/Weapons/MeleeWeapon/WeaponMelee.cs#L29) |
  | 5 | LOGIC | ⚠️ OPEN | `EntityMoveState.LogicUpdate()` dereferences `entity.Input.Target.transform.position` (line 30) before null check (line 34) — NullRef if target lost mid-chase | [EntityMoveState.cs:30](Assets/Script/Character/Entity/States/EntityMoveState.cs#L30) |
  | 6 | LOGIC | ⚠️ OPEN | Player damage chain broken — `Core.TakeDamage()` đã bị xóa; `NegativeReciver.TakeDamage()` hiện `throw NotImplementedException`; `EventID` vẫn thiếu `ON_PLAYER_DEATH` (re-verified 2026-07-02) | [NegativeReciver.cs:8](Assets/Script/Character/Player/CoreComponent/NegativeReciver.cs#L8) |
  | 7 | LOGIC | ⚠️ OPEN | `EntityDeathState` extends `MonoBehaviour` instead of `EntityState` — not wired into state machine | [EntityDeathState.cs](Assets/Script/Character/Entity/States/EntityDeathState.cs) |
  | 8 | LOGIC | ⚠️ OPEN | `EntityBasicState.LogicUpdate()` — `Health <= 0` block is empty (line 21), no transition to `EntityDeathState` | [EntityBasicState.cs:21](Assets/Script/Character/Entity/States/EntityBasicState.cs#L21) |
  | 9 | LOGIC | ⚠️ OPEN | `AnimationPlayerController.OnEnable()` registers `StartAnimation` callback twice on line 21 — `EndAnimation` event never fires; mirror bug in `OnDisable` line 29 | [AnimationPlayerController.cs:21](Assets/Script/Character/Player/Animation/AnimationPlayerController.cs#L21) |
  | 10 | BUILD | ✅ FIXED | `EventManager.cs` removed `using UnityEditor.PackageManager` — no longer breaks Player builds | [EventManager.cs](Assets/Script/Manager/EventManager.cs) |
  | 11 | LOGIC | ✅ FIXED | `MapGridController` — `Move`/`OnLoadMap` đã implement với DOTween; minimap avatar hoạt động (2026-07-02) | [MapGridController.cs](Assets/Script/Map/Cell/MapGridController.cs) |
  | 12 | ARCH | ⚠️ OPEN | `LevelManager` dùng singleton pattern (`public static Instance`) — vi phạm quy tắc "no new singletons"; cần refactor thành Inspector ref | [LevelManager.cs](Assets/Script/LevelEdit/LevelManager.cs) |
  | 13 | LOGIC | ⚠️ OPEN | Player không được teleport vào phòng start — dòng teleport bị comment; `RoomGeneraterController.OnDoneLoadRoomGrid()` (có teleport) không được gọi từ đâu | [RoomGridController.cs:56](Assets/Script/Map/Room/RoomGridController.cs#L56) |
  | 14 | LOGIC | ⚠️ OPEN | `MazeController.Awake()` thiếu `return` sau `Destroy(gameObject)` — instance trùng vẫn ghi đè `Instance` và chạy generator lần nữa | [MazeController.cs:17](Assets/Script/Map/Maze/MazeController.cs#L17) |
  | 15 | BUILD | ⚠️ OPEN | Room JSON load qua `File.ReadAllText(Application.dataPath + filePath)` — chỉ chạy trong Editor; `Assets/Data/Json/` không được đóng gói vào Player build (cùng pattern trong `LevelManager.cs`) | [RoomGeneraterController.cs:57](Assets/Script/Map/Room/RoomGeneraterController.cs#L57) |
  | 16 | LOGIC | ⚠️ OPEN | `RoomType` enum không được đọc ở runtime — start/end room ép theo vị trí list `room[0]`/`room[last]`; vỡ ngầm nếu `Maze_Storage.asset` bị sắp xếp lại | [RoomGeneraterController.cs:43](Assets/Script/Map/Room/RoomGeneraterController.cs#L43) |
  | 17 | ARCH | ⚠️ OPEN | Dead code gây hiểu nhầm: `DoorController.OpenDoor()`/`CheckCanBeOpened()` và `RoomCell.UpdateStatusDoor()` là no-op (door instance không bao giờ DISABLE) — cơ chế đóng/mở thật là `OpenDoors()`/`CloseDoor()`; nên xóa | [DoorController.cs:29](Assets/Script/Map/Room/Door/DoorController.cs#L29) |

  ---

  ## Coding Conventions

  - **No comments** unless the WHY is non-obvious (hidden constraint, workaround, surprising invariant).
  - **ScriptableObject-first**: game data (abilities, enemies, weapons, items) lives in SO assets, not hardcoded values.
  - **No new singletons** — `MazeController` is the only permitted singleton; use `GetComponent` or Inspector refs everywhere else.
  - **State machine for all characters**: new behaviour = new `PlayerState` / `EntityState` subclass, never inline `if/else` in `Update`.
  - **Weapon skills**: subclass `ActivateSkill` and override `Do()` (one-shot) or `Cast()`+`Do()` (hold-release).
  - **Layer masks** must be set in Inspector; never hardcode layer indices.

  ---

  ## Demo Completion Checklist

  1. ~~**Fix map compile errors**~~ ✅ Done — `RoomMapController`/`MainMapController` superseded; `RoomGridController` compile-clean (bugs 1-3 SUPERSEDED).
  2. ~~**Dungeon navigation + random room load**~~ ✅ Done — random DFS start/end; random room pool via `Utility.PickUniqueIndex`; `RoomGridController.LoadRoom()` loads JSON + door tile swap; teleport via `fastMovement`; `ON_LOAD_MAZE_DONE` / `ON_PLAYER_ON_DOOR` / `ON_CLEAR_ENEMY` wired.
  3. ~~**Level editor tool**~~ ✅ Done — `LevelManager` saves/loads room tilemaps as JSON under `Assets/Data/Json/Room/`; `DungeonRoomSO` tracks room file list; `LevelManagerEditor` custom Inspector button.
  4. ~~**Fix EventManager build break**~~ ✅ Done (Bug #10 FIXED) — `using UnityEditor.PackageManager` removed from `EventManager.cs`.
  5. **Fix WeaponMelee.Attack()** ⚠️ (Bug #4) — add inside the `foreach`: `INegativeReceiver dmg = enemy.GetComponentInChildren<INegativeReceiver>(); if (dmg != null) dmg.TakeDamage(currrentSA.attackDamege, transform.position);` (keep typo `attackDamege`).
  6. **Player death** ⚠️ (Bug #6) — implement `NegativeReciver.TakeDamage()` (hiện `throw NotImplementedException`): decrement `PlayerData.currentHealth`, rồi `if (currentHealth <= 0) EventManager.Emit(EventID.ON_PLAYER_DEATH)`. Add `ON_PLAYER_DEATH` to `EventID` enum. New `GameManager` subscribes: calls `PlayerData.Reborn()` + reload `StartScene`.
  7. **Deploy enemy** ⚠️ (Bugs #5, #7, #8) — three sub-tasks:
     - Fix `EntityMoveState` NullRef: move null guard `if (entity.Input.Target == null)` to top of `LogicUpdate()` before line 30.
     - Rewrite `EntityDeathState` to extend `EntityState` not `MonoBehaviour`.
     - Fill `EntityBasicState` empty death block: transition to `EntityDeathState`.
  8. **Room clear condition** ⚠️ — `RoomCell` cần đếm enemy; lock doors khi player vào room; unlock khi count = 0. `ON_CLEAR_ENEMY` đã có trong EventID — cần emit từ EntityDeathState. Thêm `ON_ENEMY_DEATH` + `ON_ROOM_CLEAR` nếu cần granular events.
  9. **HUD** ⚠️ — implement `UIManager`: bind health bar slider via `EventID.ON_PLAYER_TAKE_DAMAGE` subscription (currently empty stub).
  10. **Between-room upgrade** ⚠️ — after room clear: pause, offer 3 stat cards (+damage / +speed / +maxHealth on `PlayerData`), apply chosen.
  11. **Fix AnimationPlayerController** ⚠️ (Bug #9) — `OnEnable` line 21: change second `StartAnimation` registration to `EndAnimation`; mirror fix in `OnDisable` (line 29).
  12. ~~**Combo attack**~~ ✅ Done (2026-06-22) — multi-stage `AttackState` list trên `WeaponMeleeStats`; `WeaponMelee.SetAnimation()` cycle qua các state, `DurationNextAttack()` tính delay; damage vẫn bị chặn bởi Bug #4.
  13. **Fix start-room teleport** ⚠️ (Bug #13) — bật lại dòng teleport trong `RoomGridController.OnDoneLoadRoomGrid()` (line 56) hoặc gọi `RoomGeneraterController.OnDoneLoadRoomGrid()`; phòng start không có cửa vào nên cần tính `StartDoorPosition` riêng.
  14. **Build-safe room JSON loading** ⚠️ (Bug #15) — thay `File.ReadAllText(Application.dataPath...)` bằng `TextAsset` refs trong `DungeonRoomSO` hoặc StreamingAssets.
  15. **Enemy spawn system** ⚠️ — hướng đã chốt (2026-07-02): `Tile_Spawn` marker tiles trong room JSON (constant có sẵn trong `GameConstants.TileName.SPAWN`, chưa dùng) + `EncounterSO` theo `RoomType` + `RoomEnemySpawner` (listen `ON_LOAD_MAP`, track alive count, emit `ON_ENEMY_DEATH` mới + `ON_CLEAR_ENEMY` có sẵn). Phụ thuộc Bug #7/#8 (enemy chưa chết được). Hiện `ON_CLEAR_ENEMY` chỉ được phát từ nút debug trong `LevelManagerEditor`.

  ---

  ## Enemy Definitions

  `EnemySO` (spawning/drop data): name, level, speedMove, fieldOfViewRange, rateAttack, attackRange, damage, projectile, depotItem.

  `EntityData` SO (AI runtime): statsSO, layerMask, animatorOV, rangeCheckFieldOfView, idleDurationTime, moveDurationTime, movementVelocities, rangeCheckAttack, weaponSO.

  Available rigs: **Bat, Crab, Golem (3 phases), Pebble, Rat, Skull, Spiked Slime**.

  ---

  ## Input Bindings

  | Action | Binding |
  |--------|---------|
  | Movement | WASD |
  | Attack | Left Mouse Button |
  | Block / Ability | Right Mouse Button (Hold) |
  | Skill | E (Hold) |
  | Equip/Unequip | F |
  | Interact | G |
  | Dash | Space |

  ---

  ## Scene Map

  | Scene | Purpose |
  |-------|---------|
  | `StartScene` | Main menu |
  | `RandomMaze` | Procedural dungeon — primary dev and play scene |
  | `Level 1` | Primary dungeon level |
  | `Test AI` | Enemy AI sandbox |
  | `SampleScene` | General dev sandbox |
  | `DungeonStart` | Dungeon intro |
