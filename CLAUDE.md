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
  - **Key packages:** Input System 1.14.0, TextMeshPro 3.0.7, 2D Feature Pack, Visual Scripting 1.9.4, DOTween
  - **Open project:** Unity Hub → Open → `d:/Fork/Game-RPG`
  - **Main dev scene:** `Assets/Scenes/Main/Test/LoadRandomMap.unity` (the old `RandomMaze.unity` no longer exists)
  - **Play/test:** Enter Play Mode in the Unity Editor — no separate build step for development
  - **Compile check:** Any `.cs` edit triggers auto-recompile; errors appear in the Console window
  - **IDE:** Open `Game-RPG.sln` in Rider or Visual Studio for IntelliSense

  There are no standalone build, lint, or test CLI commands — all development happens through the Unity Editor.

  ---

  ## Repository Layout

  > Verified against source on 2026-08-20. Filenames below are the real ones on disk — several
  > classes were renamed during the Sprint 8–10 refactor (`NewPlayer.cs`→`Player.cs`,
  > `WeaponMelee.cs`→`MeleeWeapon.cs`, `WeaponMeleeStats.cs`→`MeleeWeaponStats.cs`).

  ```
  Assets/
    Script/                                     # ALL gameplay code in Assets/ — single source of truth again since 2026-08-22 (the second ability framework moved to prototypes/, see below)
      Character/
        Base/                                   # Shared hub + state-machine layer under BOTH Player and Entity
          BaseEntity.cs                         # MonoBehaviour base: Awake/Start/Update/FixedUpdate ticks CurrentState
          CoreBase.cs                           # Abstract component hub: Setup() pulls ICoreComponent via GetComponentsInChildren; GetCoreComponent<T>(out) with type cache
          CoreComponentBase.cs                  # Abstract base: resolves its Core via GetComponentInParent<T>()
          StateMachine.cs                       # Generic StateMachine<TState>: Initialize / ChangeState
          IState.cs                             # Enter / Exit / LogicUpdate / PhysicsUpdate
          StatusAnimation.cs                    # Enum: None, Start, Animaing, StartRangeTrigger, OnActivate, OffActivate, EndRangeTrigger, End
          DirectionResolver.cs                  # Static: Vector2 → angle → 8-direction index
          Interface/
            ICore.cs                            # AddCoreComponent / GetCoreComponent<T> / Setup
            ICoreComponent.cs                   # Marker + ICoreComponent<out T> (both currently memberless)
            ICharacter.cs                       # ⚠️ Empty, zero implementers, zero references — dead API
        Player/
          Player.cs                             # Player MonoBehaviour (extends BaseEntity) — creates states in Awake, exposes Core/Anim/Rigidbody/StatsSO
          PlayerState.cs                        # Base state: animBool, `StatusAnimation Status`, startTime, SetAnimationStatus()
          PlayerStateMachine.cs                 # `: StateMachine<PlayerState>`
          PlayerData.cs                         # SO: maxHealth, currentHealth, movementVelocities, Reborn() (⚠️ Reborn has no caller)
          Input/
            PlayerInput.cs                      # Auto-generated InputActionAsset
          Core/
            Core.cs                             # `: CoreBase` — holds the Player back-reference only
            CoreCompoment.cs                    # `CoreComponent<T> : CoreComponentBase<T> where T : Core` — generic shim
          CoreComponent/
            PlayerInputHandle.cs                # ⚠️ file name vs class: class is `PlayerInputHandler`. New Input System; implements IAimProvider; 8-direction angles; BufferIsAttack
            PlayerMovement.cs                   # rb.velocity wrapper
            WeaponHolder.cs                     # Equip/UnEquip; Attack() / MakeDamage() / EndDamage() / CanAttack() / CanChain()
            AbilityHolder.cs                    # Skill state machine Start→Cast→Do per frame
            Interact.cs                         # Base: OverlapCircleNonAlloc + nearest-by-mouse
            Interactor.cs                       # FindInteraction via OverlapCircle
            NegativeReciver.cs                  # ✅ CoreComponent implementing INegativeReceiver — decrements its own `currentHealth`, emits ON_PLAYER_DEATH. ⚠️ does NOT write PlayerData.currentHealth (story S10-08)
            TalentManager.cs                    # ⚠️ file name vs class: class is `TalentManagger`. Plain MonoBehaviour, stats hardcoded in Awake (TD-018)
          Animation/
            AnimationName.cs                    # ⚠️ an empty ScriptableObject stub — the real constants live in GameConstants.AnimationName
            AnimationPlayerController.cs        # ✅ Bug #9 FIXED — OnEnable/OnDisable register all five AnimationEventIds correctly
          States/                               # All player states (sub + super flattened)
            PlayerBasicState.cs                 # Shared: attack/skill/equip/interact/damage transitions
            PlayerUseWeaponState.cs             # Freezes movement, exits on animFinish
            PlayerDisadvantageState.cs
            PlayerIdleState.cs, PlayerMoveState.cs
            PlayerAttackState.cs                # StatusAnimation-driven: OnActivate → WeaponHolder.MakeDamage(); EndRangeTrigger → chain or exit
            PlayerSkillWeaponState.cs           # Drives AbilityHolder each frame
            PlayerTakeDamageState.cs
            PlayerDeathState.cs                 # ⚠️ `: PlayerDisadvantageState` but LogicUpdate() body is fully commented out AND it is never constructed in Player.Awake() (BUG-044)
            PlayerEquidUnequid.cs, PlayerIntertorState.cs
            PlayerUserItemState.cs              # ⚠️ Stub — extends MonoBehaviour (wrong base class, TD-001)
          Projectile/
            Projectile.cs                       # Raycast hit → INegativeReceiver.TakeDamage()
            Spell.cs                            # Extends Projectile; also calls IEffectable.ApplyEffect()
        Entity/                                 # Enemy AI framework
          Entity.cs                             # Enemy MonoBehaviour (extends BaseEntity): builds Idle/Move/Attack/TakeDamage/Death states
          EntityData.cs                         # SO: statsSO, layerMask, animatorOV, FOV, idle/move duration, attack range, WeaponSO
          EntityStatsSO.cs                      # SO: base/modifier/amount for health, velocities, armor. ⚠️ `ModifiersAmor` getter recurses into itself → StackOverflow (TD-011)
          EntityStateMachine.cs, EntityState.cs
          EntityWeaponMelee.cs                  # Attack() implemented. ⚠️ uses allocating Physics2D.OverlapCircle (TD-005 / BUG-046)
          Core/
            EntityCore.cs                       # `: CoreBase, INegativeReceiver` — ⚠️ TakeDamage() throws NotImplementedException (BUG-042)
            EntityCoreComponent.cs
          CoreComponent/
            EntityInput.cs                      # Implements IAimProvider. ⚠️ `GetTargetInRange()` is COMMENTED OUT in Update() — targetTransform is never assigned, so enemies never detect the player
            EntityAttack.cs                     # ⚠️ second attack implementation alongside EntityWeaponMelee (BUG-043); hardcodes TakeDamage(10, …)
            EntityNegativeReciver.cs            # ⚠️ copy-pasted player logic on an enemy component: resolves PlayerInputHandler off EntityCore and emits ON_PLAYER_DEATH (BUG-053)
            EntityMovement.cs                   # Chase / flee / wander; pulls the grid from EnemyManager.Instance
            EntityFindTarget.cs, EntityWeapon.cs, EntityWeaponHolder.cs, EntityEffectStats.cs
          States/                               # All entity states (super + sub flattened)
            EntityBasicState.cs                 # Transitions: direction, take-damage, death (reads Data.StatsSO.Health), attack check
            EntityIdleState.cs                  # Idle timer or target detected → MoveState
            EntityMoveState.cs                  # ✅ null-guards TargetTransform first; wall avoidance; timeout → IdleState
            EntityAttackState.cs                # Triggers weapon Attack() on anim event
            EntityTakeDamageState.cs
            EntityDeathState.cs                 # ✅ `: EntityBasicState` — emits ON_ENEMY_DEATH on EndRangeTrigger
            EntityUseWeaponState.cs, EntityDisadvantageState.cs
        StatsCharacter.cs                       # SO base: blockDMG, maxMana, maxHealth, RuntimeAnimatorController

      Enemy/                                    # Canonical Assets/Script/Enemy/ — 3 files
        EnemySO.cs                              # Data-only SO: name, level, speedMove, FOV, rateAttack, attackRange, damage, projectile, depotItem
        EnemyManager.cs                         # ✅ NOT a stub any more — it is now the PATHFINDING service: [RequireComponent(PathRequestManager)], SetPathfindingGrid(), RequestPath(), GetNodeByPositionWorld(). Awake() guard has the correct `return`. ADR-0002 amended 2026-08-21 to re-scope the singleton exception onto this role; the spawn lifecycle it originally described lives in RoomCell + EnemySpawner + RoomGridController and has no ADR
        EnemySpawner.cs                         # ✅ Event-driven: ON_GET_SPAWN_POSITIONS → OnGetSpawnPositions(); ON_SPAWN_EXTRA_ENEMY → SpawnExtraEnemy(). Spawns via ObjectPoolManager, emits ON_DONE_SPAWN_ENEMY. ⚠️ BUG-033 at line 62 — `set.Count == 0 || set == null` dereferences before the null test. Dead `Spawn()` method still present

      Pathfinding/                              # A* — NO GDD, NO ADR yet (see BUG-052)
        Algorithm/ AStar.cs, Heuristic.cs, PriorityQueue.cs
        Data/ Node.cs, Path.cs, PathRequest.cs, SearchNode.cs
        Grid/ GridBuilder.cs, PathfindingGrid.cs
        PathRequestManager.cs, Utility/GridUtility.cs

      Poolable/                                 # Generic object pool — supersedes the deleted Pooling/ObjectPooling.cs
        ObjectPoolManager.cs, Pool.cs, PoolMember.cs

      UI/                                       # NO GDD yet
        UIController.cs                         # Runtime UI Toolkit: MainMenu / Settings / Pause screens from .uxml
        StatsUIController.cs                    # Stat panel driven by ON_*_STATS_*_UI events. ⚠️ calls statsSO.AddPrimaryPoint() — UI writing gameplay state (ui-code.md violation)
        StatSlot.cs                             # One stat row, bound to a StatsViewDTO

      FastTest/
        FasTestEnemyDeath.cs                    # Debug harness: emits ON_ENEMY_DEATH on OnDisable

      Interface/
        IInteractable.cs, IEffectable.cs, IAimProvider.cs, IPoolable.cs
        INegativeReciver.cs                     # filename keeps the old typo; interface name is INegativeReceiver — TakeDamage(int amountDamage, Vector2 attackPosition)

      Map/
        BaseGrid.cs                             # Generic MonoBehaviour grid: AddCell, Setting, GetNext, CaculateIndex (⚠️ GetNext has no bounds check, TD-027)
        BaseCell.cs                             # Abstract cell base: AddCell, GetGridPosition, Setting()
        Interface/
          IGrid.cs                              # IGrid / IGrid<T>: Columns, Rows, AddCell, Setting, GetNext
          IGridItem.cs                          # IGridItem: AddCell, GetGridPosition
        Maze/
          MazeGenerator.cs                      # DFS; random Start cell, last-visited End cell; public Start/End
          MazeController.cs                     # Singleton; wires MapGrid + RoomGrid; GetCellStart() / GetCellEnd()
        Cell/
          Cell.cs                               # Data class: Row, Col, Doors dict (string→STATUS_DOOR)
          MapCell.cs                            # Minimap cell MonoBehaviour (extends BaseCell)
          MapGridController.cs                  # ✅ Minimap grid — Move/OnLoadMap implemented, avatar tween via DOTween
        Room/
          RoomCell.cs                           # World room cell: doors, StartDoorPosition, IsCleared, ClearRoom(), OpenDoors()/CloseDoor(), and ✅ the room-clear counter — EnemyCount, OnDoneSpawnEnemy(), OnSpawnExtraEnemy(), OnEnemyDeath() → emits ON_CLEAR_ENEMY at zero
          RoomGridController.cs                 # ✅ Room grid event hub — listens to SIX events: ON_LOAD_MAZE_DONE, ON_CLEAR_ENEMY, ON_PLAYER_ON_DOOR, ON_ENEMY_DEATH, ON_DONE_SPAWN_ENEMY, ON_SPAWN_EXTRA_ENEMY
          RoomGeneraterController.cs            # ✅ Tilemap loader (same GameObject, RequireComponent): Setting(), LoadRoom(), ClearRoom(), DeleteDoorTileMap(), SwapTileMap(). Parses Tile_Spawn_Enemy markers → emits ON_GET_SPAWN_POSITIONS. Builds the PathfindingGrid
          Door/
            DoorController.cs                   # ✅ Manages door collider; collider enabled only when Status==OPEN; emits ON_PLAYER_ON_DOOR on player touch
        Legacy/ Door.cs, Room.cs                # Superseded — do not use

      LevelEdit/                                # Editor + runtime room loading
        LevelManager.cs                         # Singleton ⚠️ (TD-023); SaveLevel() / ImportRoomJsonFiles() / UpdateRoom() (editor); GetDungeonRoomSO() / GetRandomRooms() / GetTileSOs() / GetTilemaps() (runtime); SpawnRoomEnemies() (Editor button — the second spawn driver). Also declares `LevelData`

      StatSystem/                               # RPG stat framework (GDD: design/gdd/stat-system.md; numbers: ToolExcel/stat_system_formula_reference.xlsx)
        StatType.cs                             # Enum: primary STR/DEX/INT/VIT/LUK (0-4); derived HP/Mana/PhysicalDamage/… (100-111)
        Stat.cs                                 # BaseValue/LevelUpValue/EquipmentValue/EquipmentByPrimaryValue/AdjustedValue/FinalValue + modifier list. ✅ `modifiers` is a bare private field (NOT serialized) since 2026-08-21 — runtime buffs no longer leak into .asset files
        StatModifier.cs                         # Authored (targetStat/type/value) + runtime Source ([NonSerialized], stamped by WithSource()). Order derived from Type
        StatModifierGroup.cs                    # NOT a ScriptableObject — a plain [System.Serializable] class embedded in WeaponStats (ratified 2026-08-21, ADR-0001). Field is `authoredModifiers` (serialized — real designer data). ApplyTo() / RemoveFrom() by source
        DerivedStatFormula.cs                   # baseConstant + level×perLevel + Σ(primary × coefficient)
        StatsSO.cs                              # SO "Game/Stats Profile": Level, StatUnusedBonus, Get/GetStat/GetStatValue, AddModifiersFromSource / RemoveModifiersFromSource, RecalculateDerived(), CalculateStatUnusedBonus() (public since sprint-10), OnStatChanged. Also declares StatsViewDTO — Update() takes (baseValue, levelUpValue, equipmentValue) since sprint-10
        StatModifierTester.cs                   # Debug MonoBehaviour driven by Assets/Editor/StatModifierTesterEditor.cs

      Manager/
        EventManager.cs                         # Static bus: Resgister / UnResgister / Emit; EventID enum (20 values — see Event System below)
        AnimationEventManager.cs                # AnimationEventId enum: StartAnimation, MoveAnimation, AttactAnimation, DoSkillAnimation, EndAnimation
        UI/UIManager.cs                         # EMPTY STUB (TD-017)

      Database-SO/Modal/                        # Enemy-spawn data model; note `Modal` typo = "Model"
        EntityModel.cs                          # Base SO: id (int, private, ID getter — OnValidate GUID-gens when 0), nameEnity
        MapModel.cs                             # SO "Game/Map Model": fullRoomList + runtime _pool + GetRandomRoom() shuffle-bag
        RoomModel.cs                            # SO "Game/Room Model": enemiesOfRoom, candidateEnemies, weightBudget[0,500], overflowPercent (⚠️ declared but never read — a literal 0.1f is used instead). GetSpawnSet() = candidate-pool + RarityTier roll + retry-fallback. Also declares EnemySpawnEntry, EnemyModal ([Serializable] class, Prefab + weight[1,100] + rarityTier) and the RarityTier enum (Common=50, Rare=30, Epic=15, Legendary=5)

      Weapons/
        Weapon.cs (abstract base)                # CanAttack() / CanChain() / OnAttackEnter(player) / OnActivate() / OnDeactivate() / Equid() / UnEquid()
        WeaponStats.cs                          # Abstract SO base: LayerMask, AttackStages (List<AttackSO>), AbilityWeapon, SkillWeapon, StatModifiers (StatModifierGroup — renamed from `modifiers` 2026-08-21, FormerlySerializedAs keeps the data)
        WeaponType.cs                           # Enum: RangeWP, MeleeWP
        MeleeWeapon/
          MeleeWeapon.cs                        # ✅ OnActivate() = OverlapCircleNonAlloc + INegativeReceiver.TakeDamage() (Bug #4 FIXED). maxTargetsPerSwing buffer cached in Awake
          MeleeWeaponStats.cs                   # nameWeapon, idWeapon, shieldEra, blockDamage[0,500]
          SwordAndShield.cs                     # Extends MeleeWeapon — empty
          AttackSO.cs                           # SO "WeaponData/AttackStage/Melee": nameState, attackRange, attackDamege, attackRate, directionAttackAnimatorOV, ability
        RangeWeapon/
          RangeWeapon.cs                        # ✅ OnActivate() spawns pooled bullets fanned across SpreadAngle; RecoveryTime cooldown
          RangeWeaponStats.cs                   # nameWeapon, autoFire
          RangeAttackSO.cs                      # Extends AttackSO: BulletPrefab, BulletData, ProjectileCount, SpreadAngle, RecoveryTime
          bullet.cs, BulletDataSO.cs

      Skill_Ability/
        AbstractSkillSO.cs                      # Abstract SO base: skillName, skillDescription, GenerateDescription()
        ActivateSkill.cs                        # Base skill SO: Enter/Activate/Cast/Do/Exit lifecycle
        DashAbility.cs, SlashAbility.cs, BlockAbility.cs
        DualAbility.cs                          # Extends ActivateSkill — all code commented out (WIP)
        EffectSkillSO.cs (abstract), EffectSkillDuringTime.cs, EffectSkillOneTime.cs
        InternalSkillSO.cs                      # Talent tree node SO
        WeaponSO.cs                             # SO "Item SO/Enemy/Weapon" — extends ItemOS, holds an enemy weapon prefab

      Item/, Interact/, MainMenu/
      Utility/
        GameConstants.cs                        # AnimationName, Direction (vectors + names + both lookup dicts), Input, RouteAsset, SettingStats (GAME_SCALE=3, LENGTH_ROOM=10, PADDING_DOOR_TELE_SCALE, PADDING_NODE_VALUE), RoomTypeNames, TileName (ROOM/DOOR/FLOOR/SPAWN="Tile_Spawn_Enemy"), StatTypeName
        Utility.cs                              # PickUniqueIndex, ToCardinalDirection, SealUnusedDoors, GetDoorWorldPosition, GetOverrideClips, DurationNextAttack
        VectorExtensions.cs                     # Position2D() / WithZ()
        DirectionTarget.cs, FastMovement.cs, FollowPlayer.cs (⚠️ GameObject.Find, TD-029), SpawnCharacter.cs

    SO/
      Dungeon/
        DungeonRoomSO.cs                        # SO: List<RoomFile> (roomName, filePath, roomType) — no `roomData` field exists
        TileSO.cs                               # SO: tile id → TileBase mapping
        Maze_Storage.asset                      # Full room pool (all authored rooms)
        Maze_Load_Room.asset                    # Runtime pool (subset selected per run)
      Stat/                                     # PlayerStats.asset, Test.asset, Enemy/{Assasin,TrashMelee,FastSwarm,RangedCaster,Tank}Stats.asset, Enemy/Boss/BossStats.asset

    Editor/
      LevelManagerEditor.cs                     # Custom Inspector buttons for LevelManager
      StatModifierTesterEditor.cs               # Custom Inspector buttons for StatModifierTester

    Data/
      Json/Room/                                # Room tilemap data: NormalRoom_0.json … NormalRoom_12.json (13 rooms).
                                                # ALL 13 now carry Tile_Spawn_Enemy markers (2 in NormalRoom_0, 6 in each of the rest)

    Prefab/       Player/, Weapon/, Enemy/, UI/, Particle Effect/, Item/, ...
    Sprite/       Knight/, Enemy/, Map/, Weapons/, Items/, VFX/, UI/
    Animation/    Player animation clips
    AnimationController/
    ScriptableObjects/
    UI/Screens/   MainMenu.uxml, Settings.uxml, PauseMenu.uxml
    Scenes/       Main/StartScene, Main/SetLevel, Main/Test/LoadRandomMap, Test/Test AI, Test/ObjectPooling, SampleScene, UISample

  prototypes/     Prototype code — OUTSIDE Assets/, so Unity does not compile it (.claude/rules/prototype-code.md)
    skill-enhance-abilities/                    # A second, composition-based ability framework — 17 .cs files.
                                                # Was Assets/Skill Enhance/; moved 2026-08-22 by owner decision.
                                                # Never wired: no SO assets, no prefabs, no scene wiring, shares no
                                                # types with Script/Skill_Ability/, and DamageInFrontEffect.Apply()
                                                # is fully commented out (3D Physics.OverlapSphere + a `Damageable`
                                                # type that does not exist here). See its README.md for the
                                                # hypothesis/result/decision and how to pick it back up;
                                                # docs/diagrams/ability-system-diagrams.md diagrams it

  tests/          EditMode/, PlayMode/, playtest/ — all three contain only .gitkeep (zero tests exist, TD-014)
  ToolExcel/      stat_system.xlsx, stat_system_v1.xlsx, stat_system_formula_reference.xlsx — stat formula emulators: player / 5 creep types / boss (repo root, outside Assets/). `_formula_reference` = source of truth for per-entity base/perLevel/coefficients
  ```

  ---

  ## Architecture

  ### Shared base layer (`Character/Base/`)

  `BaseEntity` owns the Unity lifecycle: `Update()` ticks `CurrentState.LogicUpdate()`, `FixedUpdate()`
  ticks `CurrentState.PhysicsUpdate()`. Both `Player` and `Entity` extend it and supply
  `CurrentState` from their own state machine.

  `CoreBase` is the component hub. Registration is **pull-based**: `Setup()` runs
  `GetComponentsInChildren<ICoreComponent>(true)` in `Awake()` and adds everything it finds; components
  do not self-register. Consumers resolve siblings with `GetCoreComponent<T>(out var comp)`, which is
  backed by a `Dictionary<Type, ICoreComponent<ICore>>` cache.

  > ⚠️ `GetCoreComponent<T>` returns silently with `coreComponent = null` when nothing matches, and has
  > no doc comment. A mis-wired prefab therefore NullRefs a frame later somewhere unrelated.

  This layer is **not covered by any ADR** — see BUG-052.

  ### Player State Machine

  [Player.cs](Assets/Script/Character/Player/Player.cs) creates all states in `Awake`; the tick comes
  from `BaseEntity`.

  ```
  PlayerState (base)
    PlayerBasicState          — shared transitions: attack / skill / equip / interact / take-damage
      PlayerIdleState / PlayerMoveState
    PlayerUseWeaponState      — freezes movement; exits on animFinish
      PlayerAttackState       — StatusAnimation-driven; OnActivate → WeaponHolder.MakeDamage()
      PlayerSkillWeaponState  — calls AbilityHolder.SetStateAbility()
      PlayerEquidUnequid / PlayerIntertorState
    PlayerDisadvantageState
      PlayerTakeDamageState
      PlayerDeathState        — ⚠️ body commented out AND never constructed in Player.Awake()
  ```

  Animation handoff uses the **`StatusAnimation` enum**, not boolean flags. Animation events on the
  Player call `AnimationStart` / `AnimationTrigger` / `AnimationOnAction` / `AnimationOffAction` /
  `AnimtionFinishTrigger` / `AnimationEnd`, each of which calls
  `CurrentState.SetAnimationStatus(StatusAnimation.X)`. States branch on `Status` in `LogicUpdate()`.

  ### Enemy AI Framework

  [Entity.cs](Assets/Script/Character/Entity/Entity.cs) mirrors the player pattern. All config via `EntityData` SO.

  ```
  EntityBasicState   — direction tracking + transitions (take-damage, death, attack check)
    EntityIdleState  — idle timer or target detected → MoveState
    EntityMoveState  — null-guards the target first; chase / flee / wander; timeout → IdleState
    EntityAttackState / EntityTakeDamageState / EntityDeathState
  ```

  `EntityMovement` requests paths from `EnemyManager.Instance` (A*, `Pathfinding/`).
  `EntityDeathState` emits `ON_ENEMY_DEATH` on `EndRangeTrigger`.

  **Wiring a new enemy prefab:** `Entity` + `EntityCore` (child) + the core components
  (`EntityInput`, `EntityMovement`, `EntityFindTarget`, `EntityAttack`, `EntityWeaponHolder`,
  `EntityEffectStats`) as descendants of `EntityCore`. Assign an `EntityData` SO. A scene-level
  `EnemyManager` is **required** — `EntityMovement.Start()` reads `EnemyManager.Instance.Grid`.

  ### Weapon-Linked Skill System

  `ActivateSkill` SO lifecycle, driven by `AbilityHolder` each frame:
  ```
  Enter(player) → Activate() → Cast() [button held] → Do() [button released] → Exit()
  ```
  `WeaponStats` carries two SO slots: `AbilityWeapon` (RMB/Block) and `SkillWeapon` (E key), plus a
  `StatModifierGroup StatModifiers` bundle applied to `Player.Stats` on equip.

  ### Damage Chain

  ```
  # Player hits enemy — ✅ WORKS end to end on the weapon side
  PlayerAttackState [OnActivate] → WeaponHolder.MakeDamage() → Weapon.OnActivate()
    → MeleeWeapon: OverlapCircleNonAlloc → INegativeReceiver.TakeDamage(attackDamege, pos)
    → RangeWeapon: pooled bullets → bullet → INegativeReceiver.TakeDamage()

  # Enemy receives damage — ⚠️ BROKEN
    → EntityNegativeReciver.TakeDamage()  decrements its OWN `currentHealth`,
                                          resolves PlayerInputHandler off EntityCore (null → NRE),
                                          and emits ON_PLAYER_DEATH on an ENEMY death   (BUG-053)
    → EntityCore.TakeDamage()             throws NotImplementedException                (BUG-042)
    → EntityBasicState death check reads  entity.Data.StatsSO.Health — a DIFFERENT number
                                          that nothing ever decrements → enemies cannot die

  # Enemy hits player — ✅ works
  EntityAttackState → EntityWeaponMelee.Attack() (or EntityAttack.Attack(), BUG-043)
    → INegativeReceiver.TakeDamage() → NegativeReciver decrements currentHealth
    → emits ON_PLAYER_DEATH at zero   (⚠️ but PlayerData.currentHealth is never written — S10-08)

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
          → LevelManager.Instance.GetDungeonRoomSO() → full pool (⚠️ singleton reach-through)
          → Utility.PickUniqueIndex(total, mazeSize) → randomMazeRoomsIndex
          → gán random rooms; force [_startIndex]=room[0], [_endIndex]=room[last]
          → new PathfindingGrid() → EnemyManager.Instance.SetPathfindingGrid()
    → EventManager.Emit(ON_LOAD_MAZE_DONE)

  RoomGridController [ON_LOAD_MAZE_DONE] → OnDoneLoadRoomGrid()
    → LoadRoom(_startIndex, _current)
    → ⚠️ the fastMovement teleport line is COMMENTED OUT (Bug #13)

  RoomGeneraterController.LoadRoom(index, roomCell):
    → đọc JSON qua File.ReadAllText(Application.dataPath + filePath)   ⚠️ Editor-only (Bug #15)
    → clear tilemaps
    → for each tile:
        DOOR tile && !IsCleared  → keep if the direction is in roomCell.ListDirectionDoors,
                                   else swap to a ROOM (wall) tile
        SPAWN tile && !IsCleared → append world position to spawnPositions
    → SetTile trên _genmap[layerIdx]
    → roomCell.SetDoorPoints(DoorPoints)
    → if !IsCleared: SwapTileMap() + Emit(ON_GET_SPAWN_POSITIONS, spawnPositions)
                     + pathfindingGrid.BuildGrid(...)
      else:          roomCell.OpenDoors()

  EnemySpawner [ON_GET_SPAWN_POSITIONS] → OnGetSpawnPositions()
    → mapModel.GetRandomRoom() → roomModel
    → roomModel.GetSpawnSet() → List<EnemySpawnEntry>       ⚠️ can return null (BUG-033)
    → ObjectPoolManager.Spawn() per entry at a random marker position
    → Emit(ON_DONE_SPAWN_ENEMY, enemyCount)

  RoomCell tracks the count:
    ON_DONE_SPAWN_ENEMY → EnemyCount = n
    ON_SPAWN_EXTRA_ENEMY → EnemyCount++
    ON_ENEMY_DEATH → EnemyCount--; at zero → Emit(ON_CLEAR_ENEMY)

  DoorController.OnTriggerEnter2D()    tag=="Player" && Status==OPEN → Emit(ON_PLAYER_ON_DOOR, dir)

  RoomGridController [ON_PLAYER_ON_DOOR] → ClearRoom(direction) → OnLoadMap(direction):
    → _next = GetNext(direction); _current = _next
    → _current.UpdateStatusDoor(direction)     [no-op — dead code, Bug #17]
    → roomGeneraterController.LoadRoom(index, _current)
    → _current.GetStartDoorPosition(-direction)
    → fastMovement.transform.position = _next.StartDoorPosition
    → Emit(ON_LOAD_MAP, index)

  RoomGridController [ON_CLEAR_ENEMY] → DeleteDoorTileMap():
    → xóa door tiles khỏi tilemap (lộ cửa ra)
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

  **Setup quan trọng (Inspector):** `RoomGridController` cần wire `_dungeonRoomSO` (= `Maze_Load_Room.asset`);
  `RoomGeneraterController` cần `_fastMovement` and `_genmap`. The full room pool, tile list and tilemap
  list are **pulled from `LevelManager.Instance` at runtime**, so a `LevelManager` with
  `dungeonRoomSO = Maze_Storage.asset` must be present in the scene. An `EnemyManager` and an
  `EnemySpawner` (with `mapModel` assigned) must also be in the scene.

  ### Event System

  ```csharp
  EventManager.Resgister(EventID.ON_PLAYER_ON_DOOR, callback);  // note: typo in source — use as-is
  EventManager.Emit(EventID.ON_PLAYER_ON_DOOR, (Vector2)direction);
  ```

  `EventID` currently has **20 values** (`EventManager.cs`):

  > **Count history — check here before assuming a value was deleted.** The 2026-08-20 audit wrote
  > "19 values" here and in five other documents; that was a miscount, the real figure was **18**
  > and the table below was correct all along. Corrected to 18 on 2026-08-21. Then on **2026-08-22**
  > the StatsScreen UI work genuinely added `ON_REVERT_STATS_BY_UI` and `ON_RESTORE_STATS_BY_UI`,
  > taking it to **20** (5 + 3 + 3 + 8 + 1). So 19→18 was a correction; 18→20 was a real change.
  > Nothing has ever been removed.

  | Group | Values |
  |---|---|
  | Room / map | `ON_PLAYER_ON_DOOR`, `ON_LOAD_MAZE_DONE`, `ON_LOAD_MAP`, `ON_CLEAR_ENEMY`, `ON_ROOM_CLEAR` |
  | Spawn | `ON_GET_SPAWN_POSITIONS`, `ON_DONE_SPAWN_ENEMY`, `ON_SPAWN_EXTRA_ENEMY` |
  | Life cycle | `ON_PLAYER_DEATH`, `ON_ENEMY_DEATH`, `ON_REALOAD_GAME` |
  | Stats UI | `ON_OPEN_STATS_PLAYER_UI`, `ON_CLOSE_STATS_PLAYER_UI`, `ON_INCREASE_STATS_BY_UI`, `ON_DECREASE_STATS_BY_UI`, `ON_CHANGE_STATS_BY_UI_RUN_TIME`, `ON_UPDATE_STATS_BY_UI`, `ON_REVERT_STATS_BY_UI`, `ON_RESTORE_STATS_BY_UI` |
  | Debug | `ON_TEST` |

  Still missing: **`ON_PLAYER_TAKE_DAMAGE`** — `.claude/rules/ui-code.md` tells the health bar to bind
  to it, but the value has never existed. `ON_ROOM_CLEAR` exists in the enum but has no producer yet.

  ---

  ## Known Bugs (block demo)

  > Re-verified against source 2026-08-20. IDs #1–#17 are the historical CLAUDE.md numbering;
  > BUG-0NN IDs come from `production/qa/bugs/` and the sprint files.

  | # | Severity | Status | Description | Location |
  |---|----------|--------|-------------|----------|
  | 1 | COMPILE | ✅ SUPERSEDED | `RoomMapController` — class đã bị xóa, thay bởi `RoomGridController` (2026-06-04) | — |
  | 2 | COMPILE | ✅ SUPERSEDED | `MainMapController` — class đã bị xóa, logic chuyển vào `RoomGridController` (2026-06-04) | — |
  | 3 | LOGIC | ✅ SUPERSEDED | `MainMapController.Start()` — class không còn tồn tại (2026-06-04) | — |
  | 4 | LOGIC | ✅ FIXED | Player melee damage — `MeleeWeapon.OnActivate()` now does `OverlapCircleNonAlloc` + `INegativeReceiver.TakeDamage()` | [MeleeWeapon.cs:26](Assets/Script/Weapons/MeleeWeapon/MeleeWeapon.cs#L26) |
  | 5 | LOGIC | ✅ SUPERSEDED | `EntityMoveState` rewritten — `LogicUpdate()` now null-guards `entityInput.TargetTransform` first | [EntityMoveState.cs:24](Assets/Script/Character/Entity/States/EntityMoveState.cs#L24) |
  | 6 | LOGIC | ⚠️ PARTIAL | `NegativeReciver.TakeDamage()` is implemented and emits `ON_PLAYER_DEATH`, but it writes its own `currentHealth` field — `PlayerData.currentHealth` is never touched, `PlayerData.Reborn()` has no caller, and no `GameManager` exists (story S10-08) | [NegativeReciver.cs:6](Assets/Script/Character/Player/CoreComponent/NegativeReciver.cs#L6) |
  | 7 | LOGIC | ✅ FIXED | `EntityDeathState` now extends `EntityBasicState` and emits `ON_ENEMY_DEATH` | [EntityDeathState.cs:1](Assets/Script/Character/Entity/States/EntityDeathState.cs#L1) |
  | 8 | LOGIC | ✅ FIXED | `EntityBasicState.LogicUpdate()` transitions to `entity.DeathState` when `Data.StatsSO.Health <= 0` | [EntityBasicState.cs:30](Assets/Script/Character/Entity/States/EntityBasicState.cs#L30) |
  | 9 | LOGIC | ✅ FIXED | `AnimationPlayerController` registers all five `AnimationEventId`s correctly, `EndAnimation` included | [AnimationPlayerController.cs:21](Assets/Script/Character/Player/Animation/AnimationPlayerController.cs#L21) |
  | 10 | BUILD | ✅ FIXED | No `using UnityEditor` remains in `Assets/Script/` runtime code | [EventManager.cs](Assets/Script/Manager/EventManager.cs) |
  | 11 | LOGIC | ✅ FIXED | `MapGridController` — minimap avatar tween works (DOTween) | [MapGridController.cs](Assets/Script/Map/Cell/MapGridController.cs) |
  | 12 | ARCH | ⚠️ OPEN | `LevelManager` dùng singleton (`public static Instance`, a bare field) — vi phạm "no new singletons"; `RoomGeneraterController.Setting()` reaches through it | [LevelManager.cs:10](Assets/Script/LevelEdit/LevelManager.cs#L10) |
  | 13 | LOGIC | ⚠️ OPEN | Player không được teleport vào phòng start — dòng teleport bị comment; `RoomGeneraterController.OnDoneLoadRoomGrid()` (có teleport) không được gọi từ đâu | [RoomGridController.cs:82](Assets/Script/Map/Room/RoomGridController.cs#L82) |
  | 14 | LOGIC | ⚠️ OPEN | `MazeController.Awake()` thiếu `return` sau `Destroy(gameObject)` — instance trùng vẫn ghi đè `Instance` và chạy generator lần nữa | [MazeController.cs:17](Assets/Script/Map/Maze/MazeController.cs#L17) |
  | 15 | BUILD | ⚠️ OPEN | Room JSON load qua `File.ReadAllText(Application.dataPath + filePath)` — chỉ chạy trong Editor; `Assets/Data/Json/` không được đóng gói vào Player build (cùng pattern trong `LevelManager.cs`) | [RoomGeneraterController.cs:63](Assets/Script/Map/Room/RoomGeneraterController.cs#L63) |
  | 16 | LOGIC | ⚠️ OPEN | `RoomType` enum không được đọc ở runtime — start/end room ép theo vị trí list `room[0]`/`room[last]`; vỡ ngầm nếu `Maze_Storage.asset` bị sắp xếp lại | [RoomGeneraterController.cs:47](Assets/Script/Map/Room/RoomGeneraterController.cs#L47) |
  | 17 | ARCH | ⚠️ OPEN | Dead code gây hiểu nhầm: `DoorController.OpenDoor()`/`CheckCanBeOpened()` và `RoomCell.UpdateStatusDoor()` là no-op; cơ chế thật là `OpenDoors()`/`CloseDoor()` | [DoorController.cs:29](Assets/Script/Map/Room/Door/DoorController.cs#L29) |
  | BUG-042 | LOGIC | ⚠️ OPEN | `EntityCore.TakeDamage()` throws `NotImplementedException` | [EntityCore.cs:11](Assets/Script/Character/Entity/Core/EntityCore.cs#L11) |
  | BUG-053 | LOGIC | ⚠️ OPEN | `EntityNegativeReciver` runs player-only logic on an enemy: resolves `PlayerInputHandler` off `EntityCore` (→ NRE) and emits `ON_PLAYER_DEATH` when an **enemy** dies | [EntityNegativeReciver.cs:10](Assets/Script/Character/Entity/CoreComponent/EntityNegativeReciver.cs#L10) |
  | BUG-043 | ARCH | ⚠️ OPEN | Two divergent enemy attack paths: `EntityWeaponMelee.Attack()` and `EntityAttack.Attack()` (the latter hardcodes damage `10`) | [EntityAttack.cs:33](Assets/Script/Character/Entity/CoreComponent/EntityAttack.cs#L33) |
  | BUG-044 | LOGIC | ⚠️ OPEN | `PlayerDeathState.LogicUpdate()` body is commented out and the state is never constructed in `Player.Awake()` | [PlayerDeathState.cs:17](Assets/Script/Character/Player/States/PlayerDeathState.cs#L17) |
  | BUG-046 | PERF | ⚠️ OPEN | `EntityWeaponMelee.Attack()` uses allocating `Physics2D.OverlapCircle` in a per-attack path | [EntityWeaponMelee.cs:29](Assets/Script/Character/Entity/EntityWeaponMelee.cs#L29) |
  | BUG-033 | LOGIC | ⚠️ OPEN | `EnemySpawner.SpawnRoomEnemies()` — `set.Count == 0 \|\| set == null` dereferences before the null test; `RoomModel.GetSpawnSet()` can return `null` | [EnemySpawner.cs:62](Assets/Script/Enemy/EnemySpawner.cs#L62) |
  | BUG-052 | DOC | ⚠️ OPEN | `Character/Base/`, `Pathfinding/`, `Poolable/` are live subsystems with no ADR. Layout above now lists them; the ADR decision is still owed | — |
  | NEW-1 | LOGIC | ⚠️ OPEN | `EntityInput.Update()` has `//GetTargetInRange();` commented out — the only writer of `targetTransform`. Enemies never detect the player; `EntityAttackState` would NullRef if reached | [EntityInput.cs:67](Assets/Script/Character/Entity/CoreComponent/EntityInput.cs#L67) |
  | NEW-2 | LOGIC | ⚠️ OPEN | `EntityStatsSO.ModifiersAmor` getter/setter recurse into themselves → `StackOverflowException` (TD-011, open since 2026-05-31) | [EntityStatsSO.cs:47](Assets/Script/Character/Entity/EntityStatsSO.cs#L47) |
  | NEW-3 | LOGIC | ✅ FIXED | `StatsSO.RecalculateDerived()` skip-guard used `\|\|` where it needed `&&`. Fixed on `sprint-10` — the guard now ANDs all four comparisons, and `AddPrimaryPoint()` calls `RecalculateDerived()` directly. (Harmless leftover: `FinalValue` is compared twice) | [StatsSO.cs:274](Assets/Script/StatSystem/StatsSO.cs#L274) |
  | NEW-4 | DATA | ✅ FIXED | `Stat.modifiers` no longer carries `[SerializeField]`, so runtime buffs are not written into `.asset` files any more. A warning comment above the field records the past leak (`STR +1 Flat` reached `PlayerStats.asset` / `Test.asset` and was committed) and the distinction from `StatModifierGroup.authoredModifiers`, which **must stay serialized** — `SnS_Stat.asset` holds real authored data there | [Stat.cs:49-62](Assets/Script/StatSystem/Stat.cs#L49) |

  ---

  ## Coding Conventions

  - **No comments** unless the WHY is non-obvious (hidden constraint, workaround, surprising invariant).
  - **ScriptableObject-first**: game data (abilities, enemies, weapons, items) lives in SO assets, not hardcoded values.
  - **No new singletons** — `MazeController` and `EnemyManager` (ratified exception, ADR-0002) are the only permitted singletons; `LevelManager` is a standing unratified violation (TD-023). Use `GetComponent` or Inspector refs everywhere else.
  - **State machine for all characters**: new behaviour = new `PlayerState` / `EntityState` subclass, never inline `if/else` in `Update`.
  - **Weapon skills**: subclass `ActivateSkill` and override `Do()` (one-shot) or `Cast()`+`Do()` (hold-release).
  - **Layer masks** must be set in Inspector; never hardcode layer indices.
  - **Preserve intentional typos** — `Resgister`, `INegativeReciver.cs`, `attackDamege`, `Modal`, `ENEBLE`, `CaculateIndex`, `currrentSA`, `deplayTime` are real contracts in code and in serialized assets.

  ---

  ## Demo Completion Checklist

  1. ~~**Fix map compile errors**~~ ✅ Done — bugs 1-3 SUPERSEDED.
  2. ~~**Dungeon navigation + random room load**~~ ✅ Done — random DFS start/end; random room pool; JSON load + door tile swap; teleport via `fastMovement`.
  3. ~~**Level editor tool**~~ ✅ Done — `LevelManager` saves/loads room tilemaps as JSON; `LevelManagerEditor` custom Inspector.
  4. ~~**Fix EventManager build break**~~ ✅ Done (Bug #10).
  5. ~~**Fix player melee damage**~~ ✅ Done (Bug #4) — `MeleeWeapon.OnActivate()`.
  6. **Player death** ⚠️ (Bug #6 / S10-08) — `NegativeReciver` emits `ON_PLAYER_DEATH`, but it must write `PlayerData.currentHealth`; `PlayerDeathState` must be constructed in `Player.Awake()` and its body restored (BUG-044); a `GameManager` must subscribe, call `PlayerData.Reborn()` and reload `StartScene`.
  7. ~~**Deploy enemy** (Bugs #5, #7, #8)~~ ✅ Done — all three sub-tasks landed.
  8. ~~**Room clear condition**~~ ✅ Done — `RoomCell.EnemyCount` counts spawns/deaths and emits `ON_CLEAR_ENEMY`; `RoomGridController` opens the doors.
  9. **HUD** ⚠️ — `UIManager` is still an empty stub. A stats panel (`UI/StatsUIController.cs`) and UI Toolkit menus (`UI/UIController.cs`) exist but neither has a GDD, and health/mana are not displayed anywhere.
  10. **Between-room upgrade** ⚠️ — after room clear: pause, offer 3 stat cards, apply through `StatsSO.AddModifiersFromSource()`.
  11. ~~**Fix AnimationPlayerController**~~ ✅ Done (Bug #9).
  12. ~~**Combo attack**~~ ✅ Done — stage list on `WeaponStats.AttackStages`; `Weapon.OnAttackEnter()` advances the index modulo `StageCount`; damage now lands.
  13. **Fix start-room teleport** ⚠️ (Bug #13) — re-enable the teleport in `RoomGridController.OnDoneLoadRoomGrid()` or call `RoomGeneraterController.OnDoneLoadRoomGrid()`.
  14. **Build-safe room JSON loading** ⚠️ (Bug #15) — replace `File.ReadAllText(Application.dataPath…)` with `TextAsset` refs on `DungeonRoomSO` or StreamingAssets.
  15. **Enemy spawn system** ⚠️ — GDD `design/gdd/enemy-spawn-system.md` + ADR-0002/0003. **Built:** `EntityModel`/`MapModel`/`RoomModel` + `GetSpawnSet()` (candidate-pool + `RarityTier` roll), `EnemySpawner` (event-driven, pooled), `Tile_Spawn_Enemy` markers in all 13 room JSONs, `RoomCell` alive-count, `ON_ENEMY_DEATH`/`ON_CLEAR_ENEMY` wired. **Open:** BUG-033 null-guard; BUG-ES-2 two parallel spawn drivers (`EnemySpawner` + `LevelManager.SpawnRoomEnemies()` Editor button); `EnemyManager` does **not** own the spawn lifecycle ADR-0002 assigns it; `overflowPercent` declared but unread; the `retry > 4` fallback in `SetListCandidate()` breaks ADR-0003's budget guarantee. Blocked in practice by BUG-042/053 (enemies cannot die).
  16. **Enemy targeting** ⚠️ (NEW-1) — re-enable `EntityInput.GetTargetInRange()` with a null guard and NonAlloc queries. Nothing in enemy AI works until this lands.
  17. **Enemy damage/death chain** ⚠️ (BUG-042 + BUG-053, story S10-01) — pick one `INegativeReceiver` implementer for the enemy, route health through `EntityStatsSO`, delete the duplicate.

  ---

  ## Enemy Definitions

  `EnemySO` (spawning/drop data): name, level, speedMove, fieldOfViewRange, rateAttack, attackRange, damage, powerShoot, projectile, layerMask, depotItem. ⚠️ Not consumed by `Entity` (TD-030).

  `EntityData` SO (AI runtime): statsSO, layerMask, aima (AnimatorOverrideController), rangeCheckFieldOfView, idleDurationTime, moveDurationTime, movementVelocities, rangeCheckAttack, weaponSO.

  `EnemyModal` (spawn metadata, nested in `RoomModel.cs`): Prefab, weight `[Range(1,100)]`, rarityTier.

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

  | Scene | Path | Purpose |
  |-------|------|---------|
  | `StartScene` | `Assets/Scenes/Main/StartScene.unity` | Main menu |
  | `LoadRandomMap` | `Assets/Scenes/Main/Test/LoadRandomMap.unity` | Procedural dungeon — primary dev and play scene |
  | `SetLevel` | `Assets/Scenes/Main/SetLevel.unity` | Room authoring scene for the level editor |
  | `Test AI` | `Assets/Scenes/Test/Test AI.unity` | Enemy AI sandbox |
  | `ObjectPooling` | `Assets/Scenes/Test/ObjectPooling.unity` | Pool sandbox |
  | `UISample` | `Assets/Scenes/UISample.unity` | UI Toolkit sample screens |
  | `SampleScene` | `Assets/Scenes/SampleScene.unity` | General dev sandbox |
