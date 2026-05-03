  # CLAUDE.md

  This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

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
          NewPlayer.cs                          # Player MonoBehaviour — state machine host
          PlayerState.cs                        # Base state: animBool, anim events, startTime
          PlayerStateMachine.cs                 # Initialize/ChangeState
          PlayerData.cs                         # SO: maxHealth, currentHealth, movementVelocities, Reborn()
          Input/
            PlayerInput.cs                      # Auto-generated InputActionAsset
            PlayerInputHandle.cs                # New Input System; 8-direction angle calc; all flags
          Core/
            Core.cs                             # Component hub: Movement, WeaponHolder, AbilityHolder, Interactor
            CoreCompoment.cs                    # Base for core components (gets Core from parent)
          CoreComponent/
            PlayerMovement.cs                   # rb.velocity wrapper
            WeaponHolder.cs                     # Equip/UnEquip weapon
            AbilityHolder.cs                    # Skill state machine Start→Cast→Do→Exit per frame
            Interact.cs                         # Base: OverlapCircleNonAlloc + nearest-by-mouse
            Interactor.cs                       # FindInteraction via OverlapCircle
          States/                               # All player states (sub + super flattened)
            PlayerBasicState.cs                 # Shared: attack/skill/equip/interact/damage transitions
            PlayerUseWeaponState.cs             # Freezes movement, exits on animFinish
            PlayerDisadvantageState.cs
            PlayerIdleState.cs, PlayerMoveState.cs
            PlayerAttackState.cs                # Calls Weapon.Attack() on AnimationTrigger anim event
            PlayerSkillWeaponState.cs           # Drives AbilityHolder each frame
            PlayerEquidUnequid.cs, PlayerIntertorState.cs, PlayerTakeDamageState.cs
          Projectile/
            Projectile.cs                       # Raycast hit → INegativeReciver.TakeDamage()
            Spell.cs                            # Extends Projectile; also calls IEffectable.ApplyEffect()
        Entity/                                 # Enemy AI framework
          Entity.cs                             # Enemy MonoBehaviour: state machine host
          EntityData.cs                         # SO: stats, layerMask, FOV, attack range, WeaponSO
          EntityInput.cs                        # Auto-detects player via OverlapCircle each frame
          EntityStateMachine.cs, EntityState.cs, EntityStatsSO.cs
          EntityWeaponMelee.cs                  # COMPLETE Attack() with INegativeReciver damage
          Core/
            EntityCore.cs                       # Component hub for enemies (implements INegativeReciver)
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
        INegativeReciver.cs                     # TakeDamage(int amount, Vector2 attackPosition)

      Map/
        Maze/   MazeGenerator.cs (DFS), MazeController.cs (singleton)
        Cell/   Cell.cs, CellControll.cs, CellMapController.cs
        Room/   RoomController.cs, RoomMapController.cs (⚠️ compile bug), Door/DoorController.cs
        Controllers/
          MainMapController.cs                  # ⚠️ two bugs — see Known Bugs table
          MiniMapController.cs
        Legacy/ Door.cs, Room.cs                # Superseded — do not use

      Weapons/
        Weapon.cs (abstract base), WeaponStats.cs, WeaponType.cs
        MeleeWeapon/
          WeaponMelee.cs                        # ⚠️ Attack() EMPTY — no damage applied
          AttackSO.cs                           # SO: attackRange, attackDamege, animatorOV
          WeaponMeleeStats.cs
        RangeWeapon/  RangeWeapon.cs, bullet.cs, Shooting.cs, BulletDataSO.cs

      Skill_Ability/
        ActivateSkill.cs                        # Base skill SO: Enter/Activate/Cast/Do/Exit lifecycle
        DashAbility.cs, SlashAbility.cs, BlockAbility.cs
        EffectSkillSO.cs (abstract), EffectSkillDuringTime.cs, EffectSkillOneTime.cs
        InternalSkillSO.cs                      # Talent tree node SO

      Manager/
        EventManager.cs                         # Static bus: Resgister / UnResgister / Emit
        AnimationEventManager.cs
        UI/UIManager.cs                         # EMPTY STUB

      Item/, Interact/, Pooling/, MainMenu/, Utility/
      GameConstants.cs                          # Direction vectors + Input axis name constants

    Prefab/       Player/, Weapon/, Enemy/, UI/, Particle Effect/, Item/, ...
    Sprite/       Knight/, Enemy/, Map/, Weapons/, Items/, VFX/, UI/
    Animation/    Player animation clips
    AnimationController/
    ScriptableObjects/
    Scenes/       StartScene, Level 1, DungeonStart, RandomMaze, Test AI, SampleScene
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

  [Core.cs](Script/Character/Player/Core/Core.cs) is the component hub wired in Inspector: `Movement`, `WeaponHolder`, `AbilityHolder`, `Interactor`.

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
  `EntityCore` implements `INegativeReciver`; taking damage reduces `EntityStatsSO.ModifiersHealth`.  
  `EntityWeaponMelee.Attack()` is **fully implemented** — `OverlapCircle` + `INegativeReciver.TakeDamage()`.

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
  PlayerAttackState → AnimTrigger → WeaponMelee.Attack()  ← ⚠️ EMPTY, fix needed

  # Enemy hits player
  EntityAttackState → AnimTrigger → EntityWeaponMelee.Attack()
    → Physics2D.OverlapCircle → INegativeReciver.TakeDamage()
    → Core.TakeDamage() → PlayerData.currentHealth -= dmg → PlayerTakeDamageState

  # Projectile hits anything
  Projectile.CheckCollisions() → Raycast → INegativeReciver.TakeDamage()
  ```

  ### Map / Dungeon Generation

  ```
  MazeGenerator.Generator(rows, cols)   DFS → Cell[] with door flags (OPEN/BE_OPEN/CLOSE)
  MazeController (singleton)            distributes Cell data to CellMapController + RoomMapController
  RoomController.AddCell(cell)          positions room prefab, enables matching DoorControllers
  DoorController.OnTriggerEnter2D()     Player tag + isOpen → EventManager.Emit(ON_PLAYER_ON_DOOR, dir)
  MainMapController.Move()              GetNextRoom(dir) → teleports player
  ```

  ### Event System

  ```csharp
  EventManager.Resgister(EventID.ON_PLAYER_ON_DOOR, callback);  // note: typo in source — use as-is
  EventManager.Emit(EventID.ON_PLAYER_ON_DOOR, (Vector2)direction);
  ```
  Events: `ON_PLAYER_ON_DOOR`, `ON_LOAD_MAP`.

  ---

  ## Known Bugs (block demo)

  | # | Severity | Description | Location |
  |---|----------|-------------|----------|
  | 1 | COMPILE | `RoomMapController.GetNextRoom()` refs `this.Columns` + `this._roomMapController` — both undefined | [RoomMapController.cs:64](Script/Map/Room/RoomMapController.cs#L64) |
  | 2 | COMPILE | `MainMapController.LoadRoom()` typo `cuonefsakdjfhnasdklfhjasdrrent` | [MainMapController.cs:32](Script/Map/Controllers/MainMapController.cs#L32) |
  | 3 | LOGIC | `MainMapController.Start()` calls `MazeController.Instance.GetStartRoom()` — method missing; use `.RoomMapController.GetStartRoom()` | [MainMapController.cs:12](Script/Map/Controllers/MainMapController.cs#L12) |
  | 4 | LOGIC | `WeaponMelee.Attack()` body empty — player deals no damage | [WeaponMelee.cs:26](Script/Weapons/MeleeWeapon/WeaponMelee.cs#L26) |
  | 5 | LOGIC | `EntityMoveState` NullRef when target lost mid-chase (distance check not guarded) | [EntityMoveState.cs:30](Script/Character/Entity/States/EntityMoveState.cs#L30) |
  | 6 | LOGIC | No player death check — `currentHealth ≤ 0` triggers nothing | [Core.cs:20](Script/Character/Player/Core/Core.cs#L20) |
  | 7 | LOGIC | `EntityDeathState` extends `MonoBehaviour` instead of `EntityState` | [EntityDeathState.cs](Script/Character/Entity/States/EntityDeathState.cs) |

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

  1. **Fix WeaponMelee.Attack()** — mirror `EntityWeaponMelee.Attack()`: `OverlapCircle` → `INegativeReciver.TakeDamage(currrentSA.attackDamege, transform.position)`.
  2. **Fix map compile errors** — `RoomMapController.GetNextRoom()`: add `Columns` property, fix `_roomMapController` self-ref; fix `MainMapController` typo + `GetStartRoom()` call.
  3. **Player death** — in `Core.TakeDamage()`: `currentHealth ≤ 0` → emit `ON_PLAYER_DEATH` → `GameManager` calls `PlayerData.Reborn()` + reload `StartScene`.
  4. **Deploy enemy** — wire `NewEnemy` prefab; fix `EntityMoveState` NullRef guard; fix `EntityDeathState` base class.
  5. **Room clear condition** — `RoomController` counts enemies; locks doors on spawn, calls `DoorController.OpenDoor()` when count reaches 0.
  6. **HUD** — bind health bar slider to `PlayerData.currentHealth / maxHealth` inside `UIManager`.
  7. **Between-room upgrade** — after clear: pause, offer 3 stat cards (+damage / +speed / +maxHealth on `PlayerData`), apply chosen.

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
