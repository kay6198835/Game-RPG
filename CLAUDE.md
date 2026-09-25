# CLAUDE.md

  This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

  > **Last updated:** 2026-09-25 (HEAD `c0067f4`, branch `origin/feature/fix-player-control`) —
  > **Post-pull re-verification.** Eleven commits landed since the pass recorded below. The biggest is
  > **ADR-0005 Amendments 1-3**, which rebuilt the character layer around shared base classes and, as a
  > side effect, changed the Abilities v2 damage contract. Every tracked bug was re-read against source
  > at this HEAD. No `.cs` file was changed by the documentation pass.
  >
  > 🔴 **THE PROJECT DOES NOT COMPILE AT HEAD — BUG-092.** `c0067f4` deleted the field
  > `public float perTime;` from `RecoveryReductionPerTimeForDuration.cs` and left both uses of it
  > (`:14`, `:17`) — `error CS0103`, twice. `Assembly-CSharp` fails, so there is no Play Mode, no Test
  > Runner, and **no bug in this register can be verified in the Editor until it is fixed.** Fix it
  > first. ⚠️ A second defect in that file survives the compile fix: `timeCount` (`:8`) is `private`
  > with no `[SerializeField]`, so Unity never serializes it and every HoT/DoT asset runs **exactly one
  > instant tick**, silently.
  >
  > ⚠️ **This is the second compile break committed in four days** (BUG-088 was the first, on
  > 2026-09-22). Nothing in this project compiles the code except a human opening the Editor. That is
  > **TD-048** (a pre-push compile check) and it is now the highest-value open process item; the
  > recurrence is filed as **TD-051**.
  >
  > ✅ **Closed by this pass:** **BUG-043** (`EntityAttack.cs` deleted, `f7d98b1`), **BUG-074**
  > (`Utility.ModifierStatsCalculate` returns the delta, `b0b1337`), **BUG-080** (`ResourceReceiver`
  > gutted to `: Interact`), **BUG-081** (one `INegativeReceiver` implementer project-wide),
  > **BUG-082** (last residual — all three `AbilityEffectDefinition` lists carry `= new()`),
  > **BUG-088** (`Random.Range(30, 75)/100f`), **BUG-091** (committed). **BUG-068** keeps only one
  > site (`AbilityHolderBase.cs:28`); **BUG-071** is PARTIAL (S2 → S3).
  >
  > 🔀 **BUG-066 and BUG-070 are now ONE site**, not two instances of one defect: the unguarded
  > dictionary moved into the shared `VitalStatsBase.cs:28,39,41,45,52,54,58`. One guard closes both.
  >
  > ⚠️ **Architectural change that invalidated several sections below, now corrected in place:**
  > `IAbilityServices` was reduced from five members to **`Pool` only**. `NegativeReceiver`, `Vital`,
  > `Stats` and `ResourceReceiver` were removed from it. The hit target now travels as
  > `AbilityContext.Target` (a `Collider2D`), effects resolve `INegativeReceiver` from it themselves,
  > and character data is read through `IAbilityOwner.GetCurrentStatValue()` — so one
  > `AbilityDefinition` runs for a player or an enemy. **The set-then-invoke contract is unchanged in
  > shape**; only the field it assigns changed. `SpawnProjectileBase.cs:32-33` is still the reference
  > implementation. `ICharacter` is also no longer a dead API — it is the character-root identity
  > marker (ADR-0005 Amendment 1).
  >
  > **Still open and unchanged:** BUG-072 (`layerMask` on `Lightning.prefab` still ships as `0` =
  > *Nothing* — code complete since 2026-09-22, **one Inspector field**), BUG-052, BUG-063 (accepted,
  > deferred), BUG-064 sub-item 7 (`RangeWeapon.poolManager` has no `[Inject]`, so a ranged weapon
  > silently never fires), BUG-065, BUG-073, BUG-079, BUG-083, BUG-084, BUG-086, BUG-087, BUG-090.
  >
  > **Counts:** 32 bug files — 15 closed/fixed, 1 accepted (deferred), 2 partial, 14 open.
  > ⚠️ The `Open bugs: NN` line in the session banner is **not** a count of open bugs:
  > `.claude/hooks/session-start.sh:29-34` sums `production/qa/bugs` and `production` recursively, so it
  > counts every file twice and counts closed bugs as open. It reported 62 for 31 files. Ignore it.
  >
  > Full trail: `production/qa/open-issues-2026-09-25.md` and `docs/CHANGELOG-DOCS.md`.
  >
  > **Previous entry — 2026-09-22 (HEAD `2a83469`, branch `origin/feature/fix-player-control`)** —
  > **Post-fetch re-verification.** Two commits landed after the documentation pass recorded below
  > (`723fab1` "coding", `2a83469` "coding") and were re-read against source. They are a net
  > improvement to Abilities v2 and a build break everywhere else.
  >
  > 🔴 **THE PROJECT DOES NOT COMPILE AT HEAD — BUG-088.** `EntityMovement.SetPositionToCheck()`
  > (`EntityMovement.cs:161-165`, added in `723fab1`) calls `Random.range` (no such member; it is
  > `Random.Range`) and `trasnform.postion` (two typos, neither identifier exists). `Assembly-CSharp`
  > fails with CS0117 and CS0103, so there is no Play Mode, no Test Runner, and **no bug in this
  > register can be verified in the Editor until it is fixed**. Fix it first, before anything else.
  > ⚠️ **Owner review, later the same day — working tree now carries four uncommitted `.cs` edits,**
  > **none of them committed and none of them in the fetched history (remote is still `2a83469`):**
  > `EntityMovement.cs` (BUG-088 — builds again, integer division remains), `AbilityDefinition.cs`
  > (BUG-091 — null-element guards restored), `LightningController.cs` (BUG-072 — overlap query
  > implemented, but `_buffer` is a length-0 serialized array so it finds nothing), and
  > `SpawnProjectileBase.cs` (BUG-075 — **fixed at owner request during this pass**).
  > **BUG-063 is now ACCEPTED (deferred)** by owner decision: the `[SerializeField]` stays through
  > development and is removed at demo prep. **BUG-089 re-framed and downgraded to S3** — `Casting()`
  > is `void` by design, the `Hold`-path claim is withdrawn, and what is left is dormant.
  >
  > *Earlier in this pass:* `Random.range` →
  > `Random.Range`. `trasnform.postion` is still there, so the file still does not build; and the
  > integer division (`Random.Range(0, 100)/100` is always `0`) is untouched, so fixing only the
  > identifiers leaves a method that silently returns the entity’s own position.
  >
  > ✅ **Fixed by `2a83469`:** **BUG-077** (`HasEnoughManaCondition.cs` deleted; all four Paladin
  > assets cleaned to `Conditions: []`), **BUG-085** (`NotDeadCondition.cs` deleted —
  > `Abilities/Conditions/` is now empty), and **BUG-076** in full: the ability-scope affordability
  > gate now exists and is generic over `StatType` (`AbilityDefinition.TryStart()`, `:50-64`), so
  > Avatar of Light’s 50 HP cost is validated; and the three condition walks collapsed to one
  > (`ValidateConditions()`, `TryPayCost()` and the `AbilityHolder` walk are all deleted).
  > `Casting()` was renamed `TryCast()` as the owner said it would be.
  >
  > ⚠️ **Introduced by the same push — BUG-088…BUG-091:** the compile break above; **BUG-089** a
  > refused `TryCast()` calls `CancelHold()` but `CastInstant()` still advances to `Do`, so
  > `Execute()` applies the refused effect anyway; **BUG-090** the orphaned
  > `Has Enough Mana Condition.asset` (script deleted, asset and two references kept);
  > **BUG-091** `TryStart()` lost its null-element guard and `AbilityDefinition.CheckPayCostValid()`
  > is dead code carrying the BUG-082 defect.
  >
  > Unchanged and still open: BUG-072 (`LightningController` overlap query — `723fab1` touched the
  > file, whitespace only), BUG-075, BUG-063, BUG-071, BUG-073, BUG-074, BUG-079, BUG-082, BUG-083,
  > BUG-084, BUG-086, BUG-087.
  >
  > **Previous entry — 2026-09-22 (HEAD `d17fcc5`, branch `main` — merge of `e2cb75e`)** —
  > **Bug-documentation re-verification pass.** Every tracked bug was re-read against source; no
  > `.cs` file was changed. Two ability commits (`73ab8e7` "coding update flow ability, update logic
  > cost", `e2cb75e` "done") had landed after the last doc-sync and were never reviewed, and the
  > 2026-09-21 pass itself introduced three claims that source does not support. Outcome:
  > **BUG-069 and BUG-067 are FIXED** (full evidence chains in their files); **BUG-078 is half
  > fixed** (inverted return closed, double spawn open); **BUG-076 is re-scoped** (original
  > double-charge fixed by a design change; three new cost defects replace it); **BUG-079's
  > consequence was wrong and its severity is lowered** (abilities *do* re-cast —
  > `AbilityHolder.StartHold()` forces the state reset); **BUG-063's `#if UNITY_EDITOR` guard is
  > not a mitigation** (Play Mode *is* `UNITY_EDITOR`); **BUG-074 has no work-in-progress fix**
  > (working tree is clean). **BUG-072 is now the widest-impact bug in the project**: the only
  > writer of `Services.NegativeReceiver` sits inside the dead 3D trigger callback of BUG-075, so
  > the field is always null and **no Abilities v2 effect deals any damage** — and v2 is the
  > player's only ability path. Eight previously untracked defects filed as **BUG-080…BUG-087**,
  > including `ON_PLAYER_DEATH` having zero subscribers (death is a permanent hard lock) and the
  > absence of any `.asmdef` (TD-014's precondition — the first EditMode test cannot be written).
  > See `docs/CHANGELOG-DOCS.md` for the per-document trail.
  >
  > **Previous entry — 2026-09-21 (HEAD `15242e6`, branch `origin/feature/fix-player-control`)** —
  > **Abilities v2 re-synchronisation.** The Abilities effect and runtime layers were replaced
  > wholesale during sprints 13-14 (`8295539` … `7cceda2`) with no doc update. Recorded here now:
  > (a) the `SkillState` enum is **`AbilityState`** and has no `None` member — six documents carried
  > the old name; (b) `DamageInFrontEffect` / `LungeForwardEffect` / `ShootObjectEffect` /
  > `PlayDebugLogEffect` / `AbilityRuntimeHelpers` / `SpiritOrbProjectile` / `SpiritDoTBehaviour` are
  > **deleted**, replaced by the `SpawnEffectBase` + `StatsEffectBase` hierarchies and the
  > `Runtime/SpawnMono/` controller layer; (c) `IAbilityServices` / `AbilityServices` is new and
  > undocumented anywhere before today; (d) v2's live content is now the **Paladin ability set**;
  > (e) the **Paladin sprite direction indices were renumbered** to the project's
  > `DirectionResolver` convention (dir 0 = down-left, clockwise). Five new bugs filed:
  > BUG-075…BUG-079. `docs/diagrams/ability-system-diagrams.md` §1–§3 rewritten from source.
  >
  > **Previous entry — 2026-09-11 (HEAD `6d6a8e4`)** — Full documentation/code re-synchronisation.
  > Four structural changes that had landed with no doc update are now recorded here:
  > (1) the whole-tree move of seven directories under `Assets/Script/System/` (`1c0742e`),
  > (2) the adoption of **VContainer 1.19.0** dependency injection (`aa4e620`),
  > (3) the deletion of `StatsSO` in favour of `BaseStatsSO` / `EnemyStatSO` (`b0512f4`, `1c0742e`),
  > (4) the promotion of the `prototypes/skill-enhance-abilities` framework into
  > `Assets/Script/System/Abilities/` (`9b8d40f`, `5c7afba`).
  > **Bug status corrected:** BUG-053 is FIXED; BUG-044's "stops PlayerMovement" claim was false
  > (now BUG-065); NEW-4 has REGRESSED (now BUG-063). See `docs/CHANGELOG-DOCS.md` for the full
  > per-document trail.

  ## Engine Version Reference

  @docs/engine-reference/unity/VERSION.md

  ---

  ## Project Overview

  Unity action roguelike RPG. Combat inspired by **Cult of the Lamb**: top-down, real-time melee with directional attacks, weapon-linked skills, and per-run power progression. Procedurally generated rooms — clear enemies to unlock doors to the next room.

  **Demo target:** Full game life cycle — start menu → dungeon run (movement + melee combat + 2 skills + enemies + room progression) → death/restart. Focus: combat system only.

  ---

  ## Unity Environment

  - **Unity:** 2022.3.62f3 LTS
  - **Key packages:** Input System 1.14.0, TextMeshPro 3.0.7, 2D Feature Pack 2.0.1, Visual Scripting 1.9.4, Timeline 1.7.7, **VContainer 1.19.0** (DI, git package), DOTween (Asset Store import, not in `manifest.json`)
  - **Open project:** Unity Hub → Open → `d:/Fork/Game-RPG`
  - **Main dev scene:** `Assets/Scenes/Main/Test/LoadRandomMap.unity` (the old `RandomMaze.unity` no longer exists)
  - **Play/test:** Enter Play Mode in the Unity Editor — no separate build step for development
  - **Compile check:** Any `.cs` edit triggers auto-recompile; errors appear in the Console window
  - **IDE:** Open `Game-RPG.sln` in Rider or Visual Studio for IntelliSense

  > ⚠️ **A `GameLifetimeScope` must be present in any playable scene.** Since `aa4e620` the project
  > uses VContainer: `Player`, `PlayerManager`, `StatHandler`, `EnemySpawner`, `ItemSpawner`,
  > `ObjectPoolManager`, `RoomGeneraterController`, `StatsUIController` and `AbilityHolder` are
  > resolved through the container. `RegisterComponentInHierarchy<T>()` only *finds* — it never
  > spawns — so a missing component in the scene hierarchy throws at `Awake()`, not at first use.

  There are no standalone build, lint, or test CLI commands — all development happens through the Unity Editor.

  ---

  ## Repository Layout

  > **Re-verified against source on 2026-09-11 (HEAD `6d6a8e4`).** The tree below reflects the
  > 77-rename reorganisation in `1c0742e` ("update folder", 2026-09-03), which moved seven
  > top-level directories under `Assets/Script/System/`. Every path in every document that
  > predates that commit is wrong; see `docs/CHANGELOG-DOCS.md`.
  >
  > | Was (docs still say this) | Is now |
  > |---|---|
  > | `Assets/Script/Enemy/` | `Assets/Script/System/Enemy/` |
  > | `Assets/Script/Pathfinding/` | `Assets/Script/System/Pathfinding/` |
  > | `Assets/Script/Poolable/` | `Assets/Script/System/PoolableService/` (renamed too) |
  > | `Assets/Script/StatSystem/` | `Assets/Script/System/StatSystem/` |
  > | `Assets/Script/Skill_Ability/` | `Assets/Script/System/Skill_Ability/` |
  > | `Assets/Script/Item/` | `Assets/Script/System/Item/` |
  > | `Assets/Script/LifetimeScope/` | `Assets/Script/System/LifetimeScope/` |

  ```
  Assets/
    Script/                                     # ALL gameplay code — 194 .cs files, ~11k lines
      Character/
        Base/                                   # Shared hub + state-machine layer under BOTH Player and Entity
          BaseEntity.cs                         # MonoBehaviour base: Awake/Start/Update/FixedUpdate ticks CurrentState
          CoreBase.cs                           # Abstract component hub: Setup() pulls ICoreComponent via GetComponentsInChildren; GetCoreComponent<T>(out) with type cache
          CoreComponentBase.cs                  # Abstract base: resolves its Core via GetComponentInParent<T>()
          StateMachine.cs                       # Generic StateMachine<TState>: Initialize / ChangeState
          IState.cs                             # Enter / Exit / LogicUpdate / PhysicsUpdate
          StatusAnimation.cs                    # Enum: None, Start, Animaing, StartRangeTrigger, OnActivate, OffActivate, EndRangeTrigger, End
          StatusBase.cs                         # ✅ NEW (2026-09-08, `9b8d40f`) — shared status base
          DirectionResolver.cs                  # Static: Vector2 → angle → 8-direction index
          Interface/
            ICore.cs                            # AddCoreComponent / GetCoreComponent<T> / Setup
            ICoreComponent.cs                   # Marker + ICoreComponent<out T> (both currently memberless)
            ICharacter.cs                       # ⚠️ **CORRECTED 2026-09-25 — NOT a dead API any more.** ADR-0005 Amendment 1
                                                # (2026-09-24) made it the character-root identity marker, carrying `Transform` only.
                                                # Player and Entity implement it; `CoreBase` exposes `Character` and `TryGetCapability<T>`.
                                                # Never add component interfaces (Stats / Vital / DamageReceiver) to it — reach those from
                                                # the root with `GetComponentInChildren<T>()`. *Previous entry: "Empty, zero implementers,
                                                # zero references — dead API".*
        Boss/                                   # ⚠️ ORPHAN — only three `.meta` files, no `.cs`. Left behind when the
                                                # boss feature (`ffe1976`) was reverted (`4421fdc`, `ff67f4d`). Safe to delete
        Player/
          Player.cs                             # Player MonoBehaviour (extends BaseEntity) — creates 9 states in Awake; exposes Core/Anim/Rigidbody/Data/**Stats (BaseStatsSO)**
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
            AbilityHolder.cs                    # ⚠️ REWRITTEN — now `: CoreComponent<Core>, IAbilityOwner`, drives the **Abilities v2**
                                                # framework (`AbilityInstance` / `AbilityState`), NOT `ActivateSkill`. Takes `IObjecPoolService` via `[Inject]`;
                                                # builds `AbilityServices` in Setup(); `abilityBindings` is OVERWRITTEN in Start() from `Player.Data.AbilityBindings`
            StatHandler.cs                      # ✅ NEW — `: CoreComponent<Core>, IPlayerStatService`; read/write façade over `BaseStatsSO`
            VitalComponent.cs                   # ⚠️ **REWRITTEN 2026-09-25 (ADR-0005).** Now a two-line shim: `VitalStatsComponent :
                                                # `VitalStatsBase<Core>`, empty body. File name kept because PlayerTest.prefab references it by GUID.
                                                # Everything moved to `Character/Base/VitalStatsBase.cs`, shared with `EntityVitalStats`.
                                                # ⚠️ **BUG-066 and BUG-070 are now the SAME seven lines** there (`:28,39,41,45,52,54,58`) —
                                                # one guard closes both. ✅ The player now inherits `Reborn()` (`VitalStatsBase.cs:20`).
                                                # *Previous entry:* ✅ NEW — ⚠️ file name vs class: class is `VitalStatsComponent`. `: CoreComponent<Core>, IVitalComponent`.
                                                # Owns CURRENT stat values in a `Dictionary<StatType, float>`; max values come from StatHandler
            ResourceReceiver.cs                 # ⚠️ **GUTTED 2026-09-25 (ADR-0005).** Now `: Interact` with an EMPTY body — it no longer
                                                # implements `INegativeReceiver` or `IResourceReceiver`. Items apply themselves to the owning
                                                # `ICharacter` via `ItemController`. ✅ This closed **BUG-080** and half of **BUG-081**.
                                                # *Previous entry:* ✅ NEW — `: Interact, INegativeReceiver, IResourceReceiver`; item pickup / buff / recovery entry point
            NegativeReciver.cs                  # ⚠️ **REWRITTEN AGAIN 2026-09-25 (ADR-0005).** Now `NegativeReciver : NegativeReceiverBase<Core>`.
                                                # `NegativeReceiverBase<TCore>` (`Character/Base/`) is the **only** `INegativeReceiver` implementer
                                                # in the project — that is what closed **BUG-081**. *Previous entry:* ⚠️ REWRITTEN — now routes
                                                # damage into `VitalStatsComponent.ReceiveReduction(StatType.HP, …)`.
                                                # No longer decrements a private field, and no longer emits ON_PLAYER_DEATH (that moved to PlayerDeathState)
            Interact.cs                         # Base: OverlapCircleNonAlloc + nearest-by-mouse
            Interactor.cs                       # FindInteraction via OverlapCircle
            TalentManager.cs                    # ⚠️ file name vs class: class is `TalentManagger`. Plain MonoBehaviour, stats hardcoded in Awake (TD-018)
          Animation/
            AnimationName.cs                    # ⚠️ an empty ScriptableObject stub — the real constants live in GameConstants.AnimationName
            AnimationPlayerController.cs        # ✅ Bug #9 FIXED — OnEnable/OnDisable register all five AnimationEventIds correctly
          States/                               # All player states (sub + super flattened) — 9 constructed in Player.Awake()
            PlayerBasicState.cs                 # Shared: attack/skill/equip/interact/damage transitions
            PlayerUseWeaponState.cs             # Freezes movement, exits on animFinish
            PlayerDisadvantageState.cs
            PlayerIdleState.cs, PlayerMoveState.cs
            PlayerAttackState.cs                # StatusAnimation-driven: OnActivate → WeaponHolder.MakeDamage(); EndRangeTrigger → chain or exit
            PlayerSkillWeaponState.cs           # Drives AbilityHolder each frame
            PlayerTakeDamageState.cs
            PlayerDeathState.cs                 # `: PlayerDisadvantageState`, constructed in Player.Awake():58, emits ON_PLAYER_DEATH on EndRangeTrigger.
                                                # ⚠️ Enter() no longer stops PlayerMovement — **BUG-065 OPEN**
                                                # ⚠️ LogicUpdate() never consumes Status → ON_PLAYER_DEATH emitted EVERY FRAME — **BUG-086 OPEN**
                                                # ⚠️ that event has ZERO subscribers and no GameManager exists — **BUG-087 OPEN**
            PlayerResourceReceiverState.cs      # ✅ NEW (2026-09-07, `9f1258c`) — item pickup state
            PlayerEquidUnequid.cs, PlayerIntertorState.cs
            PlayerUserItemState.cs              # ⚠️ Stub — extends MonoBehaviour (wrong base class, TD-001)
          Projectile/
            Projectile.cs                       # Raycast hit → INegativeReceiver.TakeDamage()
            Spell.cs                            # Extends Projectile; also calls IEffectable.ApplyEffect()
        Entity/                                 # Enemy AI framework
          Entity.cs                             # Enemy MonoBehaviour (extends BaseEntity): builds Idle/Move/Attack/TakeDamage/Death states; exposes `Data` (EntityData)
          EntityData.cs                         # SO: statsSO, layerMask, animatorOV, FOV, idle/move duration, attack range, WeaponSO
          EntityStateMachine.cs, EntityState.cs
          Core/
            EntityCore.cs                       # `: CoreBase` — ✅ no longer implements INegativeReceiver (BUG-042 FIXED)
            EntityCoreComponent.cs
          CoreComponent/
            EntityInput.cs                      # Implements IAimProvider; OnTakeDamage(attackPosition) hook
            EntityFindTarget.cs                 # ⚠️ STILL PRESENT (an earlier CLAUDE.md header wrongly claimed it was deleted).
                                                # FOV + range + obstacle mask target search; consumed by EntityAttack
            # ⚠️ **EntityAttack.cs was DELETED 2026-09-25** (`f7d98b1`, "no weapon, no attack").
                                                # **BUG-043 is FIXED** — the hardcoded `TakeDamage(10, …)` is gone and an enemy attacks only
                                                # through `EntityWeaponHolder` + a `Weapon` prefab named by `EntityData.WeaponSO`.
            EntityNegativeReciver.cs            # ✅ **BUG-053 FIXED** (`f3f5f08`, verified 2026-09-06) — implements INegativeReceiver,
                                                # applies Defense via DamageCalculate(), writes EntityVitalStats, refreshes EntityUIController.
                                                # No PlayerInputHandler, no ON_PLAYER_DEATH
            EntityVitalStats.cs                 # ⚠️ **REWRITTEN 2026-09-25 (ADR-0005)** — now `: VitalStatsBase<EntityCore>`, holding only
                                                # its `Reborn()` override (`OnEnable` + reset modifiers + `IMovement.ClearImpacts()`).
                                                # The unguarded dictionary moved to the shared base — **BUG-066 = BUG-070**, one site.
                                                # *Previous entry:* ✅ NEW — current HP/stat store. ⚠️ unguarded `currentStats[statType]` → **BUG-066 OPEN**
                                                # (same defect as BUG-070 on the player side). ✅ Has Reborn() called from Start() AND OnEnable() —
                                                # the pattern VitalStatsComponent is missing (BUG-087)
            EntityStatsHandler.cs               # ✅ NEW — max-stat façade over the entity's stat SO
            EntityUIController.cs               # ✅ NEW — per-enemy health bar driver
            EntityMovement.cs                   # Chase / flee / wander; pulls the grid from EnemyManager.Instance
            EntityWeapon.cs, EntityWeaponHolder.cs, EntityEffectStats.cs
          States/                               # All entity states (super + sub flattened)
            EntityBasicState.cs                 # Transitions: direction, take-damage, death, attack check
            EntityIdleState.cs                  # Idle timer or target detected → MoveState
            EntityMoveState.cs                  # ✅ null-guards TargetTransform first; wall avoidance; timeout → IdleState
            EntityAttackState.cs                # Triggers weapon Attack() on anim event
            EntityTakeDamageState.cs
            EntityDeathState.cs                 # ✅ `: EntityBasicState` — emits ON_ENEMY_DEATH on EndRangeTrigger
            EntityUseWeaponState.cs, EntityDisadvantageState.cs
        StatsCharacter.cs                       # ⚠️ Legacy SO base (blockDMG, maxMana, maxHealth). Superseded by BaseStatsSO — Player/Entity no longer use it

      System/                                   # ✅ NEW top-level grouping (`1c0742e`, 2026-09-03)
        LifetimeScope/                          # ✅ NEW — VContainer composition root (ADR-0004)
          GameLifetimeScope.cs                  # `: LifetimeScope`; RegisterComponentInHierarchy for 9 scene components
          Interface/
            IObjecPoolService.cs                # (typo intentional in source) pooling façade
            IPlayerService.cs                   # player transform / position
            IPlayerStatService.cs               # stat read/write façade (implemented by StatHandler)
          Service/
        Abilities/                              # ✅ **Abilities v2** — promoted from prototypes/ on 2026-09-09 (`9b8d40f`, `5c7afba`).
                                                # ⚠️ **Effects/ and Runtime/ were replaced wholesale during sprints 13-14** (`8295539`…`7cceda2`).
                                                # Re-listed 2026-09-21 against HEAD `15242e6`
          Core/
            AbilityDefinition.cs                # SO "Game/Abilities/Ability Definition": Id, DisplayName, Icon, ActivationType,
                                                # Cooldown, List<StatCost> Costs, MaxHoldTime, Conditions[], Effects[], AnimatorOverride.
                                                # Also declares `AbilityActivationType`, `StatCost`, `GetCostValues()` and the
                                                # **`AbilityState`** enum (Start/Cast/Do/Exit) — ⚠️ renamed from `SkillState`, `None` removed
            AbilityInstance.cs                  # Per-owner runtime state: cooldown, hold time, ActivateInstant/CastInstant/DoInstant/Exit.
                                                # ⚠️ **RESHAPED in `2a83469` (2026-09-22):** the three Try* methods lost their `Try` prefix and
                                                # their bool returns; ValidateConditions(), TryPayCost() and SetupContext() are DELETED. Gating
                                                # moved up to AbilityDefinition.TryStart(), called from CanStart() (:89-96). BUG-076 is FIXED.
                                                # ⚠️ Exit() body commented out (BUG-079 — but StartHold() resets state, so abilities DO re-cast).
                                                # ⚠️ Casting() (:68-82) calls CancelHold() when an effect refuses, but CastInstant() still
                                                # falls through to ChangeState(Do) → the refused effect is applied anyway — **BUG-089**
            AbilityContext.cs                   # Caster/Origin/Forward/TargetPoint/HoldTime/HoldRatio/Services.
                                                # ⚠️ **CHANGED 2026-09-25 (ADR-0005): `IAbilityServices` now declares `Pool` ONLY.**
                                                # `Stats` / `ResourceReceiver` / `Vital` / `NegativeReceiver` were all removed from it. The hit
                                                # target travels as `AbilityContext.Target` (a `Collider2D`, `:24`) and effects resolve
                                                # `INegativeReceiver` from it themselves; character data is read through
                                                # `IAbilityOwner.GetCurrentStatValue()`, so one AbilityDefinition runs for player or enemy.
                                                # Also new: `TryGetRecipientComponent<T>(EffectRecipient, out T)` (`:31`) and the
                                                # `EffectRecipient { Caster, Target }` enum. The set-then-invoke contract is unchanged in shape.
                                                # ⚠️ ctor (:37-47) never assigns NegativeReceiver; it is set by whoever detects a hit, immediately
                                                # before the callback is invoked — deliberate. Neither hook runs today: BUG-075 / BUG-072
                                                # ⚠️ HoldTime/HoldRatio are plain fields snapshotted before StartHold() zeroes the timer → always 0f — **BUG-083**
            AbilitySlot.cs                      # Enum: Primary, Secondary, Utility, Ultimate
            AbilityEffectDefinition.cs          # Abstract SO: AbilityName, SubConditions, SubEffects, Apply(), virtual **TryCast()**
                                                # (renamed from Casting() in `2a83469`). :20 calls CheckPayCostValid() — the gate is real.
                                                # Also declares `StatImpactType` (Recovery/Reduction)
            AbilityConditionDefinition.cs, IAbilityOwner.cs
          Effects/                              # ⚠️ ALL FOUR previous effects deleted (DamageInFront/LungeForward/ShootObject/PlayDebugLog)
            SpawnEffectBase.cs                  # Abstract: Prefab (SpawnMono), SpawnOffset, Lifetime; pools + Launch()es the object
            SpawnProjectileEffect.cs            # `: SpawnEffectBase` — baseDamage; angle from context.Forward
            SpawnSummonEffect.cs                # `: SpawnEffectBase` — summonPrefab; spawns at TargetPoint.
                                                # ⚠️ double spawn from TWO prefab fields — Casting() spawns `Prefab`, Apply() spawns `summonPrefab` (BUG-078, still open).
                                                # ✅ inverted return FIXED in 73ab8e7. ⚠️ Apply() never sets _context but SpawnPos() reads it — do not split them
            StatEffectBase.cs                   # ⚠️ file/class mismatch: class is `StatsEffectBase`. Groups a StatModifierGroup by StatType
            RecoveryReductionStatsEffect.cs     # `: StatsEffectBase` — instant Vital.Recovery / .Reduction
            RecoveryReductionPerTimeForDuration.cs  # `: StatsEffectBase` — HoT/DoT. ⚠️ coroutine never started, not even first tick (BUG-071)
            BuffDebuffStatsForDuration.cs       # `: AbilityEffectDefinition` — timed StatModifierGroup via IVitalComponent
          Conditions/
            # ⚠️ **HasEnoughManaCondition.cs and NotDeadCondition.cs were DELETED in `2a83469`** (2026-09-22).
                                                # ⚠️ one shared asset across all 4 Paladin abilities; checks Mana ONLY, so HP costs go unvalidated
            # BUG-077 and BUG-085 are FIXED. This directory is now EMPTY. The affordability check they
          Runtime/                              # ⚠️ AbilityRuntimeHelpers / SpiritOrbProjectile / SpiritDoTBehaviour all DELETED
            BaseController/SpawnMono.cs         # `: MonoBehaviour, ISpawn` — Launch(lifetime, ctx, callback) + despawn coroutine
            SpawnMono/Interface/ISpawn.cs
            SpawnMono/SpawnProjectileBase.cs    # `: SpawnMono` — Rigidbody2D + CircleCollider2D, speed.
                                                # ⚠️ uses the 3D `OnTriggerEnter(Collider)` — never fires, COMPILES CLEAN WITH NO WARNING (BUG-075).
                                                # :32 is the ONLY writer of Services.NegativeReceiver repo-wide → BUG-072
            SpawnMono/SpawnSummonBase.cs        # `: SpawnMono` — Animator; Execute() invoked by a Unity Animation Event
            SpawnMono/SlashProjectile.cs        # `: SpawnProjectileBase` (Paladin Blessed Slash)
            SpawnMono/LightningController.cs    # `: SpawnSummonBase` — random Animator "Index" 0-9. ⚠️ Execute() target query is still 3 comment lines (BUG-072)
            SpawnMono/RuneCircleController.cs   # `: SpawnSummonBase` — empty body
        Skill_Ability/                          # ⚠️ **Abilities v1 (legacy)** — still compiled and still referenced by
                                                # WeaponStats.AbilityWeapon/SkillWeapon, AttackSO.ability, Weapon.currentAbilitySO
                                                # and EntityWeapon. The PLAYER no longer runs this path
          AbstractSkillSO.cs, ActivateSkill.cs
          DashAbility.cs, SlashAbility.cs, BlockAbility.cs
          DualAbility.cs                        # Extends ActivateSkill — all code commented out (WIP)
          EffectSkillSO.cs (abstract), EffectSkillDuringTime.cs, EffectSkillOneTime.cs
          InternalSkillSO.cs                    # Talent tree node SO
          WeaponSO.cs                           # SO "Item SO/Enemy/Weapon" — extends ItemSO, holds an enemy weapon prefab
        StatSystem/                             # RPG stat framework (GDD: design/gdd/stat-system.md; numbers: ToolExcel/stat_system_formula_reference.xlsx)
          StatType.cs                           # Enum: primary STR/DEX/INT/VIT/LUK (0-4); derived HP/Mana/PhysicalDamage/MagicDamage/
                                                # Defense/AttackSpeed/CritChance/CritDamage/MoveSpeed/HPRegen/ManaRegen/Evasion (100-111)
          Stat.cs                               # BaseValue/LevelUpValue/EquipmentValue/EquipmentByPrimaryValue/AdjustedValue/FinalValue + modifier list.
                                                # ⚠️ **BUG-063 OPEN** — `modifiers` is re-serialized behind `#if UNITY_EDITOR [SerializeField]` (Stat.cs:63-66),
                                                # undoing the 2026-08-21 fix recorded as NEW-4. ⚠️ the guard is NOT a mitigation:
                                                # Play Mode in the Editor IS UNITY_EDITOR — see the capitalised warning at Stat.cs:49-62
          StatModifier.cs                       # Authored (targetStat/type/value) + runtime Source ([NonSerialized], stamped by WithSource())
          StatModifierGroup.cs                  # Plain [System.Serializable] class embedded in WeaponStats (ADR-0001). Field `authoredModifiers` stays serialized
          DerivedStatFormula.cs                 # baseConstant + level×perLevel + Σ(primary × coefficient)
          BaseStatsSO.cs                        # ✅ **Replaces the deleted `StatsSO`** (`b0512f4` deleted it, `1c0742e` added this).
                                                # Same API surface: Level, Get/GetStat/GetStatValue, AddModifiersFromSource /
                                                # RemoveModifiersFromSource, AddPrimaryPoint, GetStatUnusedBonus, OnStatChanged,
                                                # SeedAllStats, FullStatsValue, FullStatView. Also declares StatsViewDTO
          EnemyStatSO.cs                        # `: BaseStatsSO` — enemy stat profile
          StatPointAllocator.cs                 # ✅ NEW — stat-point allocation session (accept / revert / restore), drives ON_RESET_STATS_UI_SESSION
          StatModifierTester.cs                 # Debug MonoBehaviour driven by Assets/Editor/StatModifierTesterEditor.cs
        Enemy/
          EnemySO.cs                            # Data-only SO: name, level, speedMove, FOV, rateAttack, attackRange, damage, projectile, depotItem
          EnemyManager.cs                       # PATHFINDING service: [RequireComponent(PathRequestManager)], SetPathfindingGrid(), RequestPath(), GetNodeByPositionWorld() (ADR-0002)
          EnemySpawner.cs                       # Event-driven: ON_GET_SPAWN_POSITIONS / ON_SPAWN_EXTRA_ENEMY. Spawns via ObjectPoolManager,
                                                # emits ON_DONE_SPAWN_ENEMY. ✅ BUG-033 FIXED. Now resolved through VContainer
        Item/                                   # ✅ NEW system (2026-09-04 `853fe39`, 2026-09-07 `9f1258c`) — NO GDD, NO ADR
          ItemSO.cs                             # SO "Item SO/Item": ItemSprite, ItemName, StyleItem, List<ItemEffectDefinition>, Description. (Renamed from ItemOS.cs)
          DepotItem.cs                          # SO "Item SO/Item Depot": weighted drop table by RarityTierItem; TryRollItem(), GetRandomItemByRarity().
                                                # Also declares DepotItemSlot, DropRateItemSlot, RarityTierItem
          ItemController.cs, ItemSpawner.cs     # ItemSpawner takes IObjecPoolService via [Inject]
          PrefabRandomItem.cs
          ItemEffectDefinition/
            ItemEffectDefinitionSO.cs, RecoveryEffectDefinition.cs, StatModifierEffectDefinition.cs, CurrencyEffectDefinition.cs
        PlayerSystem/
          PlayerManager.cs                      # ✅ NEW — `: MonoBehaviour, IPlayerService`; GetPlayerTransform() / SetPlayerPosition()
        Pathfinding/                            # A* — NO GDD, NO ADR yet (BUG-052)
          Algorithm/ AStar.cs, Heuristic.cs, PriorityQueue.cs
          Data/ Node.cs, Path.cs, PathRequest.cs, SearchNode.cs
          Grid/ GridBuilder.cs, PathfindingGrid.cs
          PathRequestManager.cs, Utility/GridUtility.cs
        PoolableService/                        # Generic object pool (was `Poolable/`)
          ObjectPoolManager.cs                  # Implements IObjecPoolService; injects into pooled instances on Spawn
          Pool.cs, PoolMember.cs

      Handler/                                  # ⚠️ ORPHAN — only `EventHandler.meta`, no `.cs`

      UI/                                       # NO GDD yet
        UIController.cs                         # Runtime UI Toolkit: MainMenu / Settings / Pause screens from .uxml
        StatsUIController.cs                    # Stat panel driven by ON_*_STATS_*_UI events; resolved through VContainer
        StatsScreenUIController.cs              # Full stats screen
        StatSlot.cs                             # One stat row, bound to a StatsViewDTO

      FastTest/
        FasTestEnemyDeath.cs                    # Debug harness: emits ON_ENEMY_DEATH on OnDisable

      Interface/
        IInteractable.cs, IEffectable.cs, IAimProvider.cs, IPoolable.cs
        INegativeReciver.cs                     # filename keeps the old typo; interface name is INegativeReceiver —
                                                # ⚠️ signature is `TakeDamage(**float** amountDamage, Vector2 attackPosition)` (was `int`)
        IResourceReceiver.cs                    # ✅ NEW — ReceverModifierGroup / ReceverRecovery / BuffDebuffForDuration
        IVitalComponent.cs                      # ✅ NEW — GetCurrentStatValue / ApplyBuffDebuff / ReceiverRecovery / ReceiveReduction / BuffDebuffForDuration

      Map/
        BaseGrid.cs                             # Generic MonoBehaviour grid (⚠️ GetNext has no bounds check, TD-027)
        BaseCell.cs                             # Abstract cell base
        Interface/ IGrid.cs, IGridItem.cs
        Maze/
          MazeGenerator.cs                      # DFS; random Start cell, last-visited End cell
          MazeController.cs                     # Singleton; ⚠️ Awake() still missing `return` after Destroy (Bug #14)
        Cell/
          Cell.cs, MapCell.cs
          MapGridController.cs                  # ✅ Minimap grid — avatar tween via DOTween
        Room/
          RoomCell.cs                           # World room cell + the room-clear counter (EnemyCount → ON_CLEAR_ENEMY at zero)
          RoomGridController.cs                 # Room grid event hub — listens to SIX events
          RoomGeneraterController.cs            # Tilemap loader; parses Tile_Spawn_Enemy markers; builds the PathfindingGrid.
                                                # ⚠️ still loads JSON via File.ReadAllText(Application.dataPath…) (Bug #15). Resolved through VContainer
          Door/ DoorController.cs
        Legacy/ Door.cs, Room.cs                # Superseded — do not use

      LevelEdit/
        LevelManager.cs                         # Singleton ⚠️ (TD-023); editor save/import + runtime room queries; SpawnRoomEnemies() Editor button

      Database-SO/Modal/                        # note `Modal` typo = "Model"
        EntityModel.cs                          # Base SO: id, nameEnity
        MapModel.cs                             # SO "Game/Map Model": fullRoomList + runtime _pool + GetRandomRoom() shuffle-bag
        RoomModel.cs                            # SO "Game/Room Model": GetSpawnSet() = candidate-pool + RarityTier roll + retry-fallback (ADR-0003).
                                                # Also declares EnemySpawnEntry, EnemyModal and the RarityTier enum

      Weapons/
        Weapon.cs (abstract base)                # CanAttack() / CanChain() / OnAttackEnter(player) / OnActivate() / OnDeactivate() / Equid() / UnEquid()
        WeaponStats.cs                          # Abstract SO base: LayerMask, AttackStages, AbilityWeapon + SkillWeapon (both `ActivateSkill` = Abilities v1), StatModifiers
        WeaponType.cs                           # Enum: RangeWP, MeleeWP
        MeleeWeapon/
          MeleeWeapon.cs                        # ✅ OnActivate() = OverlapCircleNonAlloc + INegativeReceiver.TakeDamage() — reference implementation
          MeleeWeaponStats.cs, SwordAndShield.cs
          AttackSO.cs                           # SO "WeaponData/AttackStage/Melee": nameState, attackRange, attackDamege, attackRate, directionAttackAnimatorOV, ability
        RangeWeapon/
          RangeWeapon.cs                        # ✅ pooled bullets fanned across SpreadAngle; RecoveryTime cooldown. ⚠️ DI wiring is BUG-064 sub-item 7 (open)
          RangeWeaponStats.cs, RangeAttackSO.cs, bullet.cs, BulletDataSO.cs

      Interact/, MainMenu/
      Manager/
        EventManager.cs                         # Static bus: Resgister / UnResgister / Emit; EventID enum (**23 values** — see Event System below)
        AnimationEventManager.cs                # ⚠️ dead — Emit() has zero callers
        UI/UIManager.cs                         # EMPTY STUB (TD-017)
      Utility/
        GameConstants.cs                        # AnimationName, Direction, Input, RouteAsset, SettingStats, RoomTypeNames, TileName, StatTypeName
        Utility.cs, VectorExtensions.cs, DirectionTarget.cs, FastMovement.cs
        FollowPlayer.cs (⚠️ GameObject.Find, TD-029), SpawnCharacter.cs

    SO/
      Dungeon/        DungeonRoomSO.cs, TileSO.cs, Maze_Storage.asset, Maze_Load_Room.asset
      Stat/           PlayerStats.asset, Test.asset, Enemy/{Assasin,TrashMelee,FastSwarm,RangedCaster,Tank}Stats.asset, Enemy/Boss/BossStats.asset
      Skill/          Block/Dash/Slash/Dual Ability.asset (v1) · Enemy - Knight.asset ·
                      **Paladin/Ability/{Consecrate,Blessed Slash,Blessing,Avatar of Light}/ + their Effect/ subfolders
                      (v2 — the current live content, added sprints 13-14)** ·
                      ShootSpirit/{ShootSpirit,SpiritBomd}.asset (v2, ⚠️ missing-script refs — BUG-073) ·
                      Conditions/Has Enough Mana Condition.asset
      Item/           PlatiumOre 0-5.asset, Depot Item 1.asset, Comsuable/, Effect/
      Database/       Enemy/, Map/, Room/
      Weapons/, Room/, Player/, Spawners/

    Editor/
      LevelManagerEditor.cs, StatModifierTesterEditor.cs
      DepotItemEditor.cs                        # ✅ NEW — custom Inspector for the drop table

    Data/Json/Room/                             # NormalRoom_0.json … NormalRoom_12.json (13 rooms), all carrying Tile_Spawn_Enemy markers

    Prefab/, Sprite/, Animation/, AnimationController/, Plugins/, UI/Screens/, Scenes/

  prototypes/
    skill-enhance-abilities/                    # ⚠️ **EMPTY of code since 2026-09-09.** `Scripts/` holds only two orphan `.meta`
                                                # files; all 17 `.cs` were promoted to `Assets/Script/System/Abilities/`.
                                                # Only `README.md` remains, as the record of the experiment

  tests/          EditMode/, PlayMode/, playtest/ — all three contain only .gitkeep (zero tests exist, TD-014)
  ToolExcel/      stat_system.xlsx, stat_system_v1.xlsx, stat_system_formula_reference.xlsx
  ```

  ---

  ## Architecture

  ### Dependency Injection — VContainer (`System/LifetimeScope/`)

  > ✅ **Added 2026-08-22 (`aa4e620`), documented here 2026-09-11.** This is the largest
  > architectural change in the project and went 3 sprints with no doc entry. See **ADR-0004**.

  `GameLifetimeScope : LifetimeScope` is the composition root. `Configure()` registers nine
  scene components, three of them behind interfaces:

  ```csharp
  builder.RegisterComponentInHierarchy<ObjectPoolManager>().As<IObjecPoolService>();
  builder.RegisterComponentInHierarchy<Player>();
  builder.RegisterComponentInHierarchy<PlayerManager>().As<IPlayerService>();
  builder.RegisterComponentInHierarchy<StatHandler>().As<IPlayerStatService>();
  builder.RegisterComponentInHierarchy<EnemySpawner>();
  builder.RegisterComponentInHierarchy<StatsUIController>();
  builder.RegisterComponentInHierarchy<ItemSpawner>();
  builder.RegisterComponentInHierarchy<RoomGeneraterController>();
  builder.RegisterComponentInHierarchy<AbilityHolder>();
  ```

  Rules that follow from this, and that supersede the older "always `GetComponent` or Inspector
  refs" wording in `.claude/rules/engine-code.md`:

  - `RegisterComponentInHierarchy<T>()` **finds, never spawns.** A component absent from the scene
    at `Awake()` throws immediately. The Player prefab must be placed in the scene, not instantiated.
  - **Runtime-spawned objects cannot be registered.** `EntityInput` lives on enemy prefabs spawned
    through `ObjectPoolManager`, so it is deliberately *not* in `Configure()`; `Pool.Spawn()` injects
    into each new instance instead. Two registrations are commented out in source for this reason
    (`StatsScreenUIController`, `StatPointAllocator`).
  - Injection points are `[Inject] public void Construct(...)` methods — see `AbilityHolder`,
    `ItemSpawner`, `ObjectPoolManager`.
  - **DI does not replace the Core hub.** Sibling core components are still resolved with
    `Core.GetCoreComponent<T>(out …)`. DI supplies *cross-system services* only.

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

  [Player.cs](Assets/Script/Character/Player/Player.cs) creates **nine** states in `Awake`; the tick
  comes from `BaseEntity`.

  ```
  PlayerState (base)
    PlayerBasicState          — shared transitions: attack / skill / equip / interact / take-damage
      PlayerIdleState / PlayerMoveState
    PlayerUseWeaponState      — freezes movement; exits on animFinish
      PlayerAttackState       — StatusAnimation-driven; OnActivate → WeaponHolder.MakeDamage()
      PlayerSkillWeaponState  — drives AbilityHolder (Abilities v2)
      PlayerEquidUnequid / PlayerIntertorState / PlayerResourceReceiverState
    PlayerDisadvantageState
      PlayerTakeDamageState
      PlayerDeathState        — emits ON_PLAYER_DEATH on EndRangeTrigger.
                                ⚠️ Enter() does NOT stop PlayerMovement — BUG-065
                                ⚠️ LogicUpdate() re-emits ON_PLAYER_DEATH every frame — BUG-086
                                ⚠️ nothing subscribes to that event; no GameManager; no player-side Reborn() — BUG-087
  ```

  Animation handoff uses the **`StatusAnimation` enum**, not boolean flags. Animation events on the
  Player call `AnimationStart` / `AnimationTrigger` / `AnimationOnAction` / `AnimationOffAction` /
  `AnimationFinishTrigger` / `AnimationEnd`, each of which calls
  `CurrentState.SetAnimationStatus(StatusAnimation.X)`. States branch on `Status` in `LogicUpdate()`.

  ### Player stat / vitals split

  > ✅ New since 2026-09-08 (`9b8d40f`). Replaces the old "`NegativeReciver` owns a private
  > `currentHealth` field" design that Bug #6 / story S10-08 described.

  Three components now divide the work, all resolved off `Core`:

  | Component | Interface | Owns |
  |---|---|---|
  | `StatHandler` | `IPlayerStatService` | **Max** values — a read/write façade over `BaseStatsSO` |
  | `VitalStatsComponent` (in `VitalComponent.cs`) | `IVitalComponent` | **Current** values — `Dictionary<StatType, float>`, seeded from `StatHandler.GetFullStat()` in `Start()` |
  | `ResourceReceiver` | `INegativeReceiver`, `IResourceReceiver` | Pickup / buff / recovery entry point |

  `NegativeReciver.TakeDamage()` now delegates: `VitalStatsComponent.ReceiveReduction(StatType.HP, amount)`
  plus `PlayerInputHandler.OnTakeDamage(attackPosition)`.

  > ⚠️ `PlayerData.currentHealth` is still never written and `PlayerData.Reborn()` still has no
  > caller — Bug #6 is narrowed, not closed. There is still no `GameManager`.

  ### Enemy AI Framework

  [Entity.cs](Assets/Script/Character/Entity/Entity.cs) mirrors the player pattern. All config via `EntityData` SO.

  ```
  EntityBasicState   — direction tracking + transitions (take-damage, death, attack check)
    EntityIdleState  — idle timer or target detected → MoveState
    EntityMoveState  — null-guards the target first; chase / flee / wander; timeout → IdleState
    EntityAttackState / EntityTakeDamageState / EntityDeathState
  ```

  The enemy mirrors the player's stat/vitals split: `EntityStatsHandler` (max) + `EntityVitalStats`
  (current) + `EntityUIController` (health bar), with `EntityNegativeReciver` as the single
  `INegativeReceiver` implementer.

  `EntityMovement` requests paths from `EnemyManager.Instance` (A*, `System/Pathfinding/`).
  `EntityDeathState` emits `ON_ENEMY_DEATH` on `EndRangeTrigger`.

  **Wiring a new enemy prefab:** `Entity` + `EntityCore` (child) + the core components
  (`EntityInput`, `EntityFindTarget`, `EntityMovement`, `EntityWeaponHolder`,
  `EntityNegativeReciver`, `EntityStatsHandler`, `EntityVitalStats`, `EntityUIController`,
  `EntityEffectStats`) as descendants of `EntityCore`. Assign an `EntityData` SO. A scene-level
  `EnemyManager` is **required** — `EntityMovement.Start()` reads `EnemyManager.Instance.Grid`.

  > ⚠️ **Updated 2026-09-25 (ADR-0005 Amendments 2-3).** `EntityAttack` no longer exists — an enemy
  > attacks **only** through `EntityWeaponHolder` plus a `Weapon` prefab named by
  > `EntityData.WeaponSO.Weapon`. No weapon, no attack. Its `AttackSO`s need animator overrides built
  > on the **enemy** controller and a `LayerMask` hitting the player hurtbox.
  > Optional extra: add `EntityAbilityHolder` to cast Abilities v2, author `EntityData.AbilityBindings`,
  > and give the controller an `Ability` bool. `EntityAbilityState` drives it (cast range = attack range).

  ### Skill / Ability — TWO frameworks now coexist

  > ⚠️ **This is the single most confusing area of the codebase.** Both compile, both ship SO
  > assets, and they share no types. Know which one you are touching.

  **Abilities v2 — `System/Abilities/` — the PLAYER path.** Composition-based, promoted out of
  `prototypes/` on 2026-09-09. Driven by `AbilityHolder : IAbilityOwner`:

  ```
  AbilityDefinition (SO: ActivationType, Cooldown, Costs, MaxHoldTime, Conditions[], Effects[], AnimatorOverride)
    → AbilityInstance (per-owner: cooldown + hold timers + AbilityContext)
      → AbilityState: Start → Cast → Do → Exit          (enum lives in AbilityDefinition.cs)
    → AbilityContext (Caster, Origin, Forward, TargetPoint, HoldTime, HoldRatio, Services)
      → IAbilityServices: Pool          ⚠️ Pool ONLY since ADR-0005 (2026-09-25)
      → AbilityContext.Target (Collider2D)   ← the hit target; effects resolve INegativeReceiver from it
      → IAbilityOwner.GetCurrentStatValue() / .PayCost()   ← character data, works for player or enemy
  ```

  > ⚠️ **Renamed 2026-09-21 in docs:** the enum is `AbilityState`, **not** `SkillState`, and there is
  > no `None` member. Six documents carried the old name for two sprints.

  Bound per `AbilitySlot` (Primary / Secondary / Utility / Ultimate). ⚠️ The serialized
  `abilityBindings` list on `AbilityHolder` is **overwritten in `Start()`** from
  `Player.Data.AbilityBindings` — author bindings on the `PlayerData` SO.

  **Effect hierarchy (rebuilt in sprints 13-14):**

  | Base | Concrete | Applies through |
  |---|---|---|
  | `SpawnEffectBase` | `SpawnProjectileEffect`, `SpawnSummonEffect` | `Services.Pool.Spawn()` → `SpawnMono.Launch()` |
  | `StatsEffectBase` (in `StatEffectBase.cs`) | `RecoveryReductionStatsEffect`, `RecoveryReductionPerTimeForDuration` | `IVitalComponent` |
  | `AbilityEffectDefinition` direct | `BuffDebuffStatsForDuration` | `IVitalComponent.BuffDebuffForDuration()` |

  Spawned runtime objects: `SpawnMono` → `SpawnProjectileBase` (`SlashProjectile`) and
  `SpawnSummonBase` (`LightningController`, `RuneCircleController`). A summon fires its payload from
  a Unity Animation Event calling `Execute()`; a projectile fires from its trigger callback.

  Live SO assets: **`Assets/SO/Skill/Paladin/Ability/{Consecrate,Blessed Slash,Blessing,Avatar of Light}/`**
  (the current content), plus the older `Assets/SO/Skill/ShootSpirit/{ShootSpirit,SpiritBomd}.asset`
  and `Assets/SO/Skill/Conditions/Has Enough Mana Condition.asset` — ⚠️ the ShootSpirit assets now
  reference deleted scripts (BUG-073).

  > 🔴 **Nothing runs at HEAD `c0067f4` — the project does not compile (BUG-092).**
  > `RecoveryReductionPerTimeForDuration.cs:14,17` read `perTime`, a field `c0067f4` deleted —
  > `error CS0103`, twice. Fix that before reading anything below as a Play Mode statement;
  > everything below is static analysis.
  >
  > ⚠️ **The contract below changed on 2026-09-25 (ADR-0005).** `IAbilityServices` is now `Pool` only.
  > The hit target travels as `AbilityContext.Target` (a `Collider2D`); effects resolve
  > `INegativeReceiver` from it themselves; character data comes from
  > `IAbilityOwner.GetCurrentStatValue()`. Read every `Services.NegativeReceiver` mention below as
  > `context.Target`. The set-then-invoke shape itself is unchanged.
  >
  > *(Superseded banner, kept for the record: "Nothing runs at HEAD `2a83469` — the project does not
  > compile (BUG-088). `EntityMovement.cs:163-164` names two identifiers that do not exist
  > (`Random.range`, `trasnform.postion`)." BUG-088 is now FIXED.)*
  >
  > ✅ **Fixed by `2a83469` (2026-09-22):** BUG-076 in full, BUG-077, BUG-085. `Casting()` is now
  > `TryCast()`; `AbilityDefinition.TryStart()` is the single condition+cost gate, generic over
  > `StatType`; `Assets/Script/System/Abilities/Conditions/` is empty.
  >
  > 🐞 **v2 still deals no damage.** Re-verified against source 2026-09-22 (HEAD `2a83469`).
  >
  > **No v2 ability deals damage at HEAD — from two independent, unrelated implementation gaps.**
  > Not, as an earlier draft of this block claimed, from one architectural flaw; that framing was
  > withdrawn on 2026-09-22 after owner review.
  >
  > | Path | Why it deals no damage | Fix |
  > |---|---|---|
  > | Projectile | `SpawnProjectileBase.cs:29` declares the **3D** `OnTriggerEnter(Collider)`; Unity never dispatches it, so the receiver is never assigned and the callback never fires | **BUG-075** — one word, `OnTriggerEnter2D(Collider2D)` |
  > | Summon | `LightningController.Execute()` (`:12-19`) has its overlap query as three comment lines, so it calls `base.Execute()` without ever assigning a receiver | **BUG-072** — implement the query |
  >
  > The two are independent: fixing BUG-075 restores projectile damage on its own. The
  > **set-then-invoke pattern itself is intentional and correct** — `SpawnProjectileBase.cs:31-33`
  > assigns `Services.NegativeReceiver` and invokes the callback in adjacent synchronous statements,
  > and the summon side has the same wiring in place (`SpawnSummonBase.cs:12-15` fires the callback
  > from a Unity Animation Event; `LightningController.cs:18` calls `base.Execute()`). The
  > `if (negativeReceiver != null)` guard in both effects is deliberate: act if a receiver was
  > supplied, skip if not.
  >
  > Also open: **BUG-089** (a refused `TryCast()` calls `CancelHold()`, but `CastInstant()` still
  > advances to `Do` and `Execute()` applies the refused effect anyway — the gate stops nothing),
  > **BUG-090** (orphaned `Has Enough Mana Condition.asset`, script deleted, two references kept),
  > **BUG-091** (`TryStart()` lost its null-element guard; `AbilityDefinition.CheckPayCostValid()` is
  > dead code carrying the BUG-082 defect), BUG-071 (HoT/DoT coroutine never started), BUG-073
  > (`ShootSpirit.asset` references a script GUID that resolves to no file), BUG-074 (stat effect
  > applies the modifier total, not the delta), BUG-076 (✅ now FIXED — was: 
  > `TryPayCost()` has no affordability gate and `HasEnoughManaCondition` covers Mana only, so
  > Avatar of Light's 50 HP cost is unvalidated; plus conditions walked 3× per activation),
  > BUG-077 (✅ now FIXED 2026-09-22 — `HasEnoughManaCondition.cs` deleted, all four Paladin
  > assets cleaned), BUG-082 (unreachable null guard, now at two sites — see BUG-091),
  > BUG-083 (`HoldTime`/`HoldRatio` always `0f` — charge scaling is dead), BUG-085 (✅ now FIXED).
  >
  > Dormant / downgraded: BUG-068 (S3 — `GetAbility()` has zero callers), BUG-079 (S3 — layering,
  > not the "castable once per scene load" lock previously claimed).
  >
  > ✅ Closed since: BUG-069 (cooldown now enforced end to end); BUG-078 (half fixed in `73ab8e7`,
  > half retracted — the two-object spawn is the intended telegraph-then-strike shape, verified
  > against `Paladin Spawn Consecrat Effect.asset`: `Prefab` = RuneCircle, `summonPrefab` =
  > Lightning).
  >
  > ✅ Confirmed by design, not defects: the channelled per-effect cost on Hold abilities, and the
  > `if (negativeReceiver != null)` guard pattern in the spawn effects.
  >
  > ⚠️ **The real blocker is still that there is no GDD for v2.** `Casting()`'s contract, the cost
  > model, the cooldown model and the spawn-callback contract are all undefined, so BUG-076,
  > BUG-078 and BUG-072 cannot be fixed without a design decision first. See demo-checklist item 21.

  **Abilities v1 — `System/Skill_Ability/` — the WEAPON and ENEMY path.** Inheritance-based
  `ActivateSkill` SO with lifecycle `Enter(player) → Activate() → Cast() → Do() → Exit()`. Still
  referenced by `WeaponStats.AbilityWeapon` / `.SkillWeapon`, `AttackSO.ability`,
  `Weapon.currentAbilitySO` and `EntityWeapon.currentAbilitySO`. Live SO assets:
  `Assets/SO/Skill/{Dash,Slash,Block,Dual} Ability.asset`.

  `WeaponStats` also carries a `StatModifierGroup StatModifiers` bundle applied to the player's
  stat profile on equip.

  ### Damage Chain

  > Signature note: `INegativeReceiver.TakeDamage(**float** amountDamage, Vector2 attackPosition)`.
  > It was `int` until the Sprint 12 stat refactor. `.claude/rules/gameplay-code.md` and
  > `weapon-skill-code.md` were corrected on 2026-09-11.

  ```
  # Player hits enemy — ✅ WORKS end to end
  PlayerAttackState [OnActivate] → WeaponHolder.MakeDamage() → Weapon.OnActivate()
    → MeleeWeapon: OverlapCircleNonAlloc → INegativeReceiver.TakeDamage(attackDamege, pos)
    → RangeWeapon: pooled bullets → bullet → INegativeReceiver.TakeDamage()

  # Enemy receives damage — ✅ UNBLOCKED (BUG-053 FIXED, f3f5f08)
    → EntityNegativeReciver.TakeDamage(float, Vector2)
        → DamageCalculate(): amount -= EntityStatsHandler.GetStatValue(StatType.Defense), clamped at 0
        → EntityVitalStats.ReceiveReduction(StatType.HP, finalDamage)
        → EntityInput.OnTakeDamage(attackPosition)
        → EntityUIController.UpdateUIHealth(current / max)
    → EntityBasicState death check → EntityDeathState → ON_ENEMY_DEATH
    ⚠️ BUG-066 OPEN — EntityVitalStats indexes currentStats[statType] with no key guard (BUG-070 is the same defect on the player side)

  # Enemy hits player — ⚠️ damage lands, but the death branch is a dead end
  EntityAttackState → EntityWeaponHolder.MakeDamage() → Weapon.OnActivate()   ✅ EntityAttack DELETED (f7d98b1)
    → INegativeReceiver.TakeDamage()
    ✅ ONE implementer project-wide since ADR-0005: NegativeReceiverBase<TCore> — BUG-081 FIXED
    (superseded) ⚠️ TWO implementers on PlayerTest.prefab with byte-identical bodies — NegativeReciver and
       ResourceReceiver. Which one runs is decided by collider layout, not by design — BUG-081
    → VitalStatsComponent.Reduction(StatType.HP, …)     ⚠️ unguarded dictionary indexer — BUG-070
    → PlayerBasicState.cs:74 reads GetCurrentStatValue(HP) → PlayerDeathState at zero
    → PlayerDeathState emits ON_PLAYER_DEATH … EVERY FRAME, unbounded — BUG-086
    ⚠️ and NOTHING subscribes to it. No GameManager exists. VitalStatsComponent has no Reborn().
       ON_REALOAD_GAME has 0 emitters and 0 subscribers. Death is a PERMANENT HARD LOCK — BUG-087
    ⚠️ PlayerData.currentHealth still never written (Bug #6 / S10-08)

  # Player ability hits anything — ❌ deals no damage today (verified 2026-09-22)
  PlayerSkillWeaponState → AbilityHolder.HandleInput() → AbilityInstance → effect.Apply()
    → SpawnEffectBase.Apply() → Services.Pool.Spawn() → SpawnMono.Launch(lifetime, ctx, callback)

  The intended contract (correct, and correctly implemented on the projectile side):
      whoever detects a hit assigns ctx.Services.NegativeReceiver, THEN invokes the callback.
      SpawnProjectileBase.cs:31-33 is the reference implementation of that pattern.
      The `if (negativeReceiver != null)` guard in both effects is deliberate: act if supplied, skip if not.

  Two independent gaps break it — neither is a design flaw:
    → projectile: SpawnProjectileBase.OnTriggerEnter(Collider)   ← 3D signature, NEVER DISPATCHED (BUG-075)
                  so the assign+invoke pair at :31-33 never runs at all
    → summon:     LightningController.Execute() :12-19           ← overlap query is 3 comment lines (BUG-072)
                  base.Execute() :18 DOES invoke the callback — with no receiver assigned
    ⇒ both effects skip their guard and no-op silently. Fix BUG-075 → projectiles work.
      Fix BUG-072 → summons work. Independent of each other.

  # Projectile hits anything
  Projectile.CheckCollisions() → Raycast → INegativeReceiver.TakeDamage()
  ```

  ### Map / Dungeon Generation

  ```
  MazeController.Awake() [singleton]
    → MazeGenerator.Generator(Rows, Cols)
        DFS from a random start cell → Cell[rows*cols]
        Start = random cell; End = last visited cell
    → SetCellData: for each Cell → MapGrid.AddCell() + RoomGrid.AddCell()
    → MapGrid.Setting() / RoomGrid.Setting()
        RoomGridController.Setting() → RoomGeneraterController.Setting():
          → GetCellStart/End → _startIndex / _endIndex
          → LevelManager.Instance.GetDungeonRoomSO() → full pool (⚠️ singleton reach-through, TD-023)
          → Utility.PickUniqueIndex(total, mazeSize) → randomMazeRoomsIndex
          → assign random rooms; force [_startIndex]=room[0], [_endIndex]=room[last] (⚠️ Bug #16)
          → new PathfindingGrid() → EnemyManager.Instance.SetPathfindingGrid()
    → EventManager.Emit(ON_LOAD_MAZE_DONE)

  RoomGridController [ON_LOAD_MAZE_DONE] → OnDoneLoadRoomGrid()
    → LoadRoom(_startIndex, _current)
    → ⚠️ the fastMovement teleport line is COMMENTED OUT (Bug #13, RoomGridController.cs:82)

  RoomGeneraterController.LoadRoom(index, roomCell):
    → read JSON via File.ReadAllText(Application.dataPath + filePath)   ⚠️ Editor-only (Bug #15)
    → clear tilemaps
    → for each tile:
        DOOR tile && !IsCleared  → keep if the direction is in roomCell.ListDirectionDoors,
                                   else swap to a ROOM (wall) tile
        SPAWN tile && !IsCleared → append world position to spawnPositions
    → SetTile on _genmap[layerIdx]
    → roomCell.SetDoorPoints(DoorPoints)
    → if !IsCleared: SwapTileMap() + Emit(ON_GET_SPAWN_POSITIONS, spawnPositions)
                     + pathfindingGrid.BuildGrid(...)
      else:          roomCell.OpenDoors()

  EnemySpawner [ON_GET_SPAWN_POSITIONS] → OnGetSpawnPositions()
    → mapModel.GetRandomRoom() → roomModel
    → roomModel.GetSpawnSet() → List<EnemySpawnEntry>
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
    → fastMovement.transform.position = _next.StartDoorPosition
    → Emit(ON_LOAD_MAP, index)

  RoomGridController [ON_CLEAR_ENEMY] → DeleteDoorTileMap() → RoomCell.OpenDoors()

  MapGridController [ON_PLAYER_ON_DOOR → Move / ON_LOAD_MAZE_DONE → OnLoadMap]
    → ✅ avatar tween to the current room (DOTween); MapCell.VisitRoom() reveals visited cells
  ```

  **STATUS_DOOR semantics:**
  | Value | Name | Meaning | Collider |
  |-------|------|---------|----------|
  | 0 | `DISABLE` | No door in this direction in the maze | Off |
  | 1 | `ENEBLE` | Door exists but is locked | Off |
  | 2 | `BE_OPEN` | Receiver side of a passage | Off |
  | 3 | `OPEN` | Passable — the player can walk through | **On** |
  | 4 | `CLOSE` | Runtime solid wall | Off |

  **Inspector setup that matters:** `RoomGridController` needs `_dungeonRoomSO`
  (= `Maze_Load_Room.asset`); `RoomGeneraterController` needs `_fastMovement` and `_genmap`. The
  full room pool, tile list and tilemap list are **pulled from `LevelManager.Instance` at runtime**,
  so a `LevelManager` with `dungeonRoomSO = Maze_Storage.asset` must be present. An `EnemyManager`,
  an `EnemySpawner` (with `mapModel` assigned) and a **`GameLifetimeScope`** must also be in the scene.

  ### Event System

  ```csharp
  EventManager.Resgister(EventID.ON_PLAYER_ON_DOOR, callback);  // note: typo in source — use as-is
  EventManager.Emit(EventID.ON_PLAYER_ON_DOOR, (Vector2)direction);
  ```

  `EventID` currently has **23 values** (`EventManager.cs`):

  > **Count history — check here before assuming a value was deleted.** The 2026-08-20 audit wrote
  > "19 values" in six documents; that was a miscount, the real figure was **18**. Corrected to 18 on
  > 2026-08-21. On **2026-08-22** the StatsScreen UI work added `ON_REVERT_STATS_BY_UI` and
  > `ON_RESTORE_STATS_BY_UI` → **20**. On **2026-08-22…2026-09-07** three more were added:
  > `ON_RESET_STATS_UI_SESSION` (StatPointAllocator session reset) and `ON_DROP_ITEM` /
  > `ON_COLLECT_ITEM` (the new Item system) → **23**, recorded here on 2026-09-11.
  > Nothing has ever been removed.

  | Group | Values |
  |---|---|
  | Room / map | `ON_PLAYER_ON_DOOR`, `ON_LOAD_MAZE_DONE`, `ON_LOAD_MAP`, `ON_CLEAR_ENEMY`, `ON_ROOM_CLEAR` |
  | Spawn | `ON_GET_SPAWN_POSITIONS`, `ON_DONE_SPAWN_ENEMY`, `ON_SPAWN_EXTRA_ENEMY` |
  | Life cycle | `ON_PLAYER_DEATH`, `ON_ENEMY_DEATH`, `ON_REALOAD_GAME` |
  | Stats UI | `ON_OPEN_STATS_PLAYER_UI`, `ON_CLOSE_STATS_PLAYER_UI`, `ON_INCREASE_STATS_BY_UI`, `ON_DECREASE_STATS_BY_UI`, `ON_CHANGE_STATS_BY_UI_RUN_TIME`, `ON_UPDATE_STATS_BY_UI`, `ON_REVERT_STATS_BY_UI`, `ON_RESTORE_STATS_BY_UI`, **`ON_RESET_STATS_UI_SESSION`** |
  | Item | **`ON_DROP_ITEM`**, **`ON_COLLECT_ITEM`** |
  | Debug | `ON_TEST` |

  Still missing: **`ON_PLAYER_TAKE_DAMAGE`** — `.claude/rules/ui-code.md` tells the health bar to
  bind to it, but the value has never existed. `ON_ROOM_CLEAR` exists in the enum but has no producer yet.

  ---

  ## Known Bugs (block demo)

  > **Re-verified against source 2026-09-22 (HEAD `d17fcc5`, merge of `e2cb75e`).** IDs #1–#17 are the
  > historical CLAUDE.md numbering; BUG-0NN IDs come from `production/qa/bugs/` and the sprint files.
  > Every BUG-0NN row below was re-read from source in this pass; each bug file carries the evidence
  > under a dated `## Re-verification — 2026-09-22` heading.
  >
  > **Status changes this pass:** BUG-069 → **FIXED**; BUG-067 → **FIXED** (with BUG-080 and BUG-081
  > split out of it, *not* closed with it); BUG-078 → **PARTIAL**; BUG-076 → **RE-SCOPED**;
  > BUG-079 → **S3** (downgraded); BUG-072 → **CONFIRMED** and widened to "no v2 effect deals damage".
  >
  > **Three claims corrected this pass** — all three came from the 2026-09-21 pass and none survive
  > a read of source:
  > - **BUG-063** was described as partly mitigated by its `#if UNITY_EDITOR` guard. It is not
  >   mitigated at all — Play Mode in the Editor *is* `UNITY_EDITOR`, the exact leak path the
  >   comment at `Stat.cs:49-62` warns about in capitals.
  > - **BUG-079** claimed an `Active` ability is "castable once per scene load". False —
  >   `AbilityHolder.StartHold()` (`:143-148`) forces `ChangeState(AbilityState.Start)` on every fresh
  >   press. The bug is a layering defect, not a functional lock.
  > - **BUG-074** was recorded as having a work-in-progress fix in `Utility.cs`. There is none —
  >   `git status` is clean and the file contains no such edit.
  >
  > **Eight defects filed for the first time:** BUG-080…BUG-087. Two of them (BUG-084, BUG-087) are
  > preconditions that invalidate existing sprint estimates, not ordinary bugs.
  >
  > **Previous pass — three corrections on 2026-09-11** — the table before that was wrong on all three:
  > - **BUG-053** was listed OPEN. It is **FIXED** (`f3f5f08`, confirmed in `production/qa/bugs/BUG-053.md`
  >   on 2026-09-06). This table simply never followed the bug file.
  > - **BUG-044** claimed the fix "properly stops `PlayerMovement`". It does not —
  >   `PlayerDeathState.Enter()` only calls `base.Enter()`. Split out as **BUG-065**.
  > - **NEW-4** claimed `Stat.modifiers` no longer carries `[SerializeField]`. It does again,
  >   behind `#if UNITY_EDITOR` (`Stat.cs:63-65`). Regression tracked as **BUG-063**.

  | # | Severity | Status | Description | Location |
  |---|----------|--------|-------------|----------|
  | 1–3 | COMPILE | ✅ SUPERSEDED | `RoomMapController` / `MainMapController` deleted 2026-06-04, replaced by `RoomGridController` | — |
  | 4 | LOGIC | ✅ FIXED | Player melee damage — `MeleeWeapon.OnActivate()` does `OverlapCircleNonAlloc` + `INegativeReceiver.TakeDamage()` | [MeleeWeapon.cs](Assets/Script/Weapons/MeleeWeapon/MeleeWeapon.cs) |
  | 5 | LOGIC | ✅ SUPERSEDED | `EntityMoveState.LogicUpdate()` now null-guards `entityInput.TargetTransform` first | [EntityMoveState.cs](Assets/Script/Character/Entity/States/EntityMoveState.cs) |
  | 6 | LOGIC | ⚠️ PARTIAL (narrowed) | Player health now routes correctly through `VitalStatsComponent`, so the "two disconnected stores" half is closed. Still open: `PlayerData.currentHealth` is never written, `PlayerData.Reborn()` has no caller, and no `GameManager` exists to reload `StartScene` (story S10-08) | [NegativeReciver.cs](Assets/Script/Character/Player/CoreComponent/NegativeReciver.cs) |
  | 7 | LOGIC | ✅ FIXED | `EntityDeathState : EntityBasicState`, emits `ON_ENEMY_DEATH` | [EntityDeathState.cs](Assets/Script/Character/Entity/States/EntityDeathState.cs) |
  | 8 | LOGIC | ✅ FIXED | `EntityBasicState.LogicUpdate()` transitions to `DeathState` at zero health | [EntityBasicState.cs](Assets/Script/Character/Entity/States/EntityBasicState.cs) |
  | 9 | LOGIC | ✅ FIXED | `AnimationPlayerController` registers all five `AnimationEventId`s | [AnimationPlayerController.cs](Assets/Script/Character/Player/Animation/AnimationPlayerController.cs) |
  | 10 | BUILD | ✅ FIXED | No unguarded `using UnityEditor` in `Assets/Script/` runtime code | [EventManager.cs](Assets/Script/Manager/EventManager.cs) |
  | 11 | LOGIC | ✅ FIXED | Minimap avatar tween works (DOTween) | [MapGridController.cs](Assets/Script/Map/Cell/MapGridController.cs) |
  | 12 | ARCH | ⚠️ OPEN | `LevelManager` uses a bare `public static Instance` field — violates "no new singletons"; `RoomGeneraterController.Setting()` reaches through it (TD-023) | [LevelManager.cs:10](Assets/Script/LevelEdit/LevelManager.cs#L10) |
  | 13 | LOGIC | ⚠️ OPEN | Player is not teleported into the start room — the teleport line is commented out; `RoomGeneraterController.OnDoneLoadRoomGrid()` has no caller | [RoomGridController.cs:82](Assets/Script/Map/Room/RoomGridController.cs#L82) |
  | 14 | LOGIC | ⚠️ OPEN | `MazeController.Awake()` missing `return` after `Destroy(gameObject)` — a duplicate still overwrites `Instance` and re-runs the generator. `EnemyManager.Awake()` has the correct shape to copy | [MazeController.cs:17](Assets/Script/Map/Maze/MazeController.cs#L17) |
  | 15 | BUILD | ⚠️ OPEN | Room JSON via `File.ReadAllText(Application.dataPath + filePath)` — Editor-only; `Assets/Data/Json/` is not packaged into a Player build | [RoomGeneraterController.cs:69](Assets/Script/Map/Room/RoomGeneraterController.cs#L69) |
  | 16 | LOGIC | ⚠️ OPEN | `RoomType` never read at runtime — start/end rooms forced by list position `room[0]`/`room[last]`; reordering `Maze_Storage.asset` breaks selection silently | [RoomGeneraterController.cs:47](Assets/Script/Map/Room/RoomGeneraterController.cs#L47) |
  | 17 | ARCH | ⚠️ OPEN | Dead code that reads as live gating: `DoorController.OpenDoor()` / `CheckCanBeOpened()` and `RoomCell.UpdateStatusDoor()` are no-ops. The real mechanism is `OpenDoors()` / `CloseDoor()` | [DoorController.cs:29](Assets/Script/Map/Room/Door/DoorController.cs#L29) |
  | BUG-042 | LOGIC | ✅ FIXED | `EntityCore.TakeDamage()` threw `NotImplementedException` — method removed; entities route health through the stat/vitals split | [EntityCore.cs](Assets/Script/Character/Entity/Core/EntityCore.cs) |
  | BUG-043 | ARCH | ✅ **FIXED 2026-09-25** — `EntityAttack.cs` deleted outright in `f7d98b1` ("no weapon, no attack"). The hardcoded `TakeDamage(10, …)` is gone; an enemy attacks only through `EntityWeaponHolder` + a `Weapon` prefab named by `EntityData.WeaponSO`. Closes TD-042 *(was: ⚠️ PARTIAL)* | `EntityWeaponMelee.cs` deleted, so the worst duplicate is gone. But `EntityAttack.Attack()` still exists alongside `EntityWeapon` and still hardcodes `TakeDamage(10, …)` | [EntityAttack.cs:33](Assets/Script/Character/Entity/CoreComponent/EntityAttack.cs#L33) |
  | BUG-044 | LOGIC | ✅ FIXED (scope corrected) | `PlayerDeathState` **is** constructed in `Player.Awake()`. The earlier "and stops PlayerMovement" claim was false → see BUG-065 | [Player.cs:58](Assets/Script/Character/Player/Player.cs#L58) |
  | BUG-046 | PERF | ✅ FIXED | Allocating `Physics2D.OverlapCircle` in `EntityWeaponMelee.Attack()` — class deleted; melee goes through `MeleeWeapon` | — |
  | BUG-033 | LOGIC | ✅ FIXED | `EnemySpawner` null-check order — now tests `set == null` before `.Count` | [EnemySpawner.cs](Assets/Script/System/Enemy/EnemySpawner.cs) |
  | BUG-052 | DOC | ⚠️ OPEN (widened) | Live subsystems with no ADR. Originally `Character/Base/`, `Pathfinding/`, `Poolable/`. **Now also**: the Item system, Abilities v2, and the UI layer. VContainer was in this set until ADR-0004 landed on 2026-09-11 | — |
  | BUG-053 | LOGIC | ✅ **FIXED** | `EntityNegativeReciver` ran player-only logic on an enemy. Rewritten in the Sprint 12 entity/stat refactor: Defense-aware `DamageCalculate()` → `EntityVitalStats` → `EntityUIController`. No `PlayerInputHandler`, no `ON_PLAYER_DEATH` | [EntityNegativeReciver.cs](Assets/Script/Character/Entity/CoreComponent/EntityNegativeReciver.cs) |
  | BUG-063 | DATA | ⏸️ **ACCEPTED (deferred) 2026-09-22 — owner decision** | The `#if UNITY_EDITOR [SerializeField]` stays while the stat system is in development and is removed at demo prep, because the Inspector view of live modifiers is useful during dev. This supersedes the "one-line fix, no blocker, 29+ cycles" framing — it is a deliberate trade, not neglect. ⚠️ The leak is still real and silent while it stands (Play Mode *is* `UNITY_EDITOR`; two `STR +1 Flat` modifiers were committed to git once already). Mitigation: check `git status` for a dirty `Assets/SO/Stat/*.asset` after any Play Mode session. *Original entry:* | `Stat.modifiers` re-serialized via `#if UNITY_EDITOR` + `[SerializeField]` (`Stat.cs:63-66`). ⚠️ **Corrected 2026-09-22: the `#if UNITY_EDITOR` guard is NOT a mitigation** — Play Mode in the Editor *is* `UNITY_EDITOR`, which is exactly the leak path the comment block at `Stat.cs:49-62` warns about in capitals. The guard only protects the player build, where `.asset` files are read-only anyway. One-line fix, carried 29+ cycles | [Stat.cs:63-66](Assets/Script/System/StatSystem/Stat.cs#L63) |
  | BUG-064 | BUILD | ⚠️ **PARTIAL — sub-item 7 re-scoped 2026-09-25.** `b0b1337` removed the inert `[SerializeField]` from `RangeWeapon.poolManager` (Unity cannot serialize an interface), but **nothing assigns the field** — no `[Inject]`, no Inspector path. `CanAttack():16` tests `poolManager != null`, so a ranged weapon **silently never fires**: no exception, no log. Fix = method injection in the shape of `AbilityHolderBase.Construct():51-55`, injected at spawn time because a weapon is pooled *(was: ⚠️ PARTIAL)* | Entity refactor deleted types without sweeping callers. Sub-items 1–6 fixed; **sub-item 7 (`RangeWeapon` DI wiring) still open** | [RangeWeapon.cs](Assets/Script/Weapons/RangeWeapon/RangeWeapon.cs) |
  | BUG-065 | LOGIC | ⚠️ OPEN | `PlayerDeathState.Enter()` only calls `base.Enter()` — the player keeps sliding during the death animation | [PlayerDeathState.cs:10](Assets/Script/Character/Player/States/PlayerDeathState.cs#L10) |
  | BUG-066 | LOGIC | ⚠️ **OPEN — MERGED with BUG-070 on 2026-09-25.** ADR-0005 moved the unguarded dictionary into the shared `VitalStatsBase.cs:28,39,41,45,52,54,58`, which both `EntityVitalStats` and `VitalStatsComponent` derive from. The two bugs are now literally the same seven lines — **one guard closes both** *(was: ⚠️ OPEN)* | `EntityVitalStats` indexes `currentStats[statType]` with no key guard (`:39,49,55,61,67`) → `KeyNotFoundException`. **Same defect as BUG-070, two instances** — fix and close together | [EntityVitalStats.cs:39](Assets/Script/Character/Entity/CoreComponent/EntityVitalStats.cs#L39) |
  | BUG-067 | LOGIC | ✅ **FIXED** (verified 2026-09-22) | `ResourceReceiver` heal/damage polarity is correct (`:17-24`); consumer `RecoveryEffectDefinition.cs:13` confirms. ⚠️ Two *unrelated* defects found in the same file were split out, **not** closed with it: **BUG-080** and **BUG-081** | [ResourceReceiver.cs:17](Assets/Script/Character/Player/CoreComponent/ResourceReceiver.cs#L17) |
  | BUG-068 | LOGIC | ⚠️ **OPEN — scope reduced 2026-09-25.** ✅ The `GetAbility()` defect is FIXED: `AbilityHolderBase.cs:121` null-guards before the dereference, and the method is no longer dead code. ❌ `CurrentActivationType` (`:28`) still dereferences `currentAbility` unguarded while `:27`/`:29` use `?.` — and enemies now run it too, through `EntityAbilityHolder`, on pooled objects whose `currentAbility` is null until the first `TryDoAbility()` *(was: ⚠️ OPEN (S3, dormant))* | `AbilityHolder.GetAbility()` discards the `TryGetValue` bool then dereferences `currentAbility.Definition` (`:97`). **Zero callers project-wide** — dead code carrying a live defect. Also `CurrentActivationType` (`:22`) dereferences unguarded where `:21` beside it uses `?.`; its one caller is safe only by accident of call ordering | [AbilityHolder.cs:93](Assets/Script/Character/Player/CoreComponent/AbilityHolder.cs#L93) |
  | BUG-069 | LOGIC | ✅ **FIXED** (verified 2026-09-22) | Cooldown now enforced end to end: `CanStart()` called at `AbilityHolder.cs:107`, set by `StartCooldown()` (`AbilityInstance.cs:192`), ticked by `Tick()` (`:23-30`) via `Processing()` (`AbilityHolder.cs:51`) from `PlayerBasicState.cs:32` / `PlayerSkillWeaponState.cs:68`. Residual: `Processing()` only runs in those two states, so cooldowns stall elsewhere — fails **tighter**, not looser | [AbilityInstance.cs:95](Assets/Script/System/Abilities/Core/AbilityInstance.cs#L95) |
  | BUG-070 | LOGIC | ⚠️ **OPEN — MERGED with BUG-066 on 2026-09-25.** `VitalComponent.cs` is now a two-line shim; the unguarded dictionary lives in `VitalStatsBase.cs:28,39,41,45,52,54,58`. `:28` is still on the live death path. ⚠️ A naive `TryGetValue ? v : 0f` would make a missing HP key **trigger death** — audit the seeding in `Reborn()` at the same time *(was: ⚠️ OPEN)* | `VitalStatsComponent` indexes `currentStats[statType]` unguarded (`:28,39,41,46,52,54,58`). `:28` is on the **live death path** (`PlayerBasicState.cs:74`). **Same defect as BUG-066** | [VitalComponent.cs:28](Assets/Script/Character/Player/CoreComponent/VitalComponent.cs#L28) |
  | BUG-071 | LOGIC | ⚠️ **PARTIAL (S2 → S3) 2026-09-25.** ✅ `b0b1337` added the missing `StartCoroutine`; ✅ `c0067f4` deleted the dead `count` and renamed the parameter `duration` → `timeCount`. ❌ `VitalStatsBase.cs:81,92` still recurse through the **public wrapper**, so no handle is kept: nothing can `StopCoroutine` it (a DoT survives death, room transitions and a pooled respawn) and `StartCoroutine` on a component disabled between ticks throws. *Correction: this is NOT a leak — exactly one coroutine is alive at a time.* Also `IVitalComponent.cs:13-14` still names the parameter `duration` and now disagrees with every implementation; the comment at `VitalStatsBase.cs:63` is stale. **Not observable until BUG-092 is fixed** *(was: ⚠️ OPEN)* | `RecoveryPerTimeForDuration` / `ReductionPerTimeForDuration` (`VitalComponent.cs:63-75`) build an iterator and never `StartCoroutine` it — **not even the first tick runs**. Sibling `BuffDebuffForDuration` (`:102-105`) does it right. Two more defects hide behind it: `count` is computed then discarded (`duration` is passed where a tick count is expected), and the recursion re-enters the wrapper (`:84`, `:95`), leaking a coroutine per tick | [VitalComponent.cs:63](Assets/Script/Character/Player/CoreComponent/VitalComponent.cs#L63) |
  | BUG-072 | LOGIC | ⚠️ **OPEN — code complete 2026-09-22, one Inspector step left** | ✅ `LightningController.Execute()` does `OverlapCircleNonAlloc` → assign `Services.NegativeReceiver` → `base.Execute()` **once per target, inside the loop** — the correct set-then-invoke shape (owner). ✅ `_buffer` moved out of `[SerializeField]` and allocated in an `Awake()` override sized by `maxTargets = 16`, with `base.Awake()` preserved so the Animator stays cached; `radius` defaults to `2f` (applied with owner approval). A serialized array field is given **length 0** by Unity, and `OverlapCircleNonAlloc` writes at most `results.Length` hits — that was returning 0 on every call, silently. ❌ **`layerMask` still ships as `0` = *Nothing*** and has no safe code default (hardcoding a layer index is forbidden by `engine-code.md`), so it must be set on `Lightning.prefab` before this works. Confirm with HP loss, not with a clean Console. *Original entry:* | `LightningController.Execute()` (`:12-19`) still has its target query as three comment lines, so it invokes the callback without ever assigning `Services.NegativeReceiver` — summon abilities deal no damage. ⚠️ **The earlier "architectural root cause" framing is WITHDRAWN.** The set-then-invoke pattern is intentional and correctly implemented at `SpawnProjectileBase.cs:31-33`; the summon invoke wiring exists too (`SpawnSummonBase.cs:12-15` fires from a Unity Animation Event, `LightningController.cs:18` calls `base.Execute()`). Only the overlap query is missing. Fix = `OverlapCircleNonAlloc` + assign receiver + invoke, per target | [LightningController.cs:12](Assets/Script/System/Abilities/Runtime/SpawnMono/LightningController.cs#L12) |
  | BUG-073 | BUILD | ⚠️ OPEN — **CONFIRMED by GUID** | `ShootSpirit.asset:12` references script guid `ac9ac7c011812d042ac992007bc0cf48`; resolving it against every `.meta` under `Assets/` returns **0 files**. Still reachable: `PlayerTest.prefab` → `SpiritBomd.asset` → this asset. Also `SpawnEffectBase.cs:14,30` still log the deleted class names `[ShootSpiritOrbEffect]` / `SpiritOrbProjectile` | [ShootSpirit.asset](Assets/SO/Skill/ShootSpirit/MainEffect/ShootSpirit.asset) |
  | BUG-074 | LOGIC | ✅ **FIXED 2026-09-25 (`b0b1337`)** — `Utility.ModifierStatsCalculate` now accumulates into a separate `addedValue` and returns `addedValue - baseValue`, i.e. the **delta**. HP 100 + a `PercentAdd 0.10` modifier now heals **10**. Verified there is exactly one caller (`StatEffectBase.cs:31`), so the contract change was safe *(was: ⚠️ OPEN)* | `StatsEffectBase.Apply` passes the modifier **total** as the recovery/reduction amount instead of the **delta** (`StatEffectBase.cs:26-29` → `Utility.cs:274-304`). HP 100 + a `PercentAdd 0.10` modifier heals **110**, not 10. `ModifierStatsCalculate` has exactly **one** caller, so changing the contract is safe — and the fix belongs at the call site (`delta = total - current`), not in the helper. ⚠️ **Corrected 2026-09-22: there is no work-in-progress fix** — `git status` is clean and `Utility.cs` contains no `finalValue` variable | [StatEffectBase.cs:27](Assets/Script/System/Abilities/Effects/StatEffectBase.cs#L27) |
  | BUG-075 | LOGIC | ✅ **FIXED 2026-09-22** (working tree, uncommitted) — `:29` is now `OnTriggerEnter2D(Collider2D)`. The body was already correct and is the reference implementation of the set-then-invoke contract. Two follow-ups this exposed, not filed: the projectile does not despawn on hit, and it has no layer mask. *Original entry:* | `SpawnProjectileBase.cs:29` declares the **3D** `OnTriggerEnter(Collider)` on a `Rigidbody2D` + `CircleCollider2D` class. ⚠️ **It compiles clean, with no warning and no Console output** — `UnityEngine.Collider` exists in every Unity project, and Unity simply never dispatches to it. A prior review dismissed this on the grounds that it would have errored; it does not. Confirm with a `Debug.Log`, not the Console. Consequence: projectile abilities deal zero damage. One-word fix, independent of BUG-072 | [SpawnProjectileBase.cs:29](Assets/Script/System/Abilities/Runtime/SpawnMono/SpawnProjectileBase.cs#L29) |
  | BUG-076 | LOGIC | ✅ **FIXED 2026-09-22 by `2a83469`** — (a) by design, (b) withdrawn, **(b′) and (c) now fixed**: `AbilityDefinition.TryStart()` (`:50-64`) gates every ability-scope cost generically over `StatType` (so Avatar of Light’s 50 HP is validated), and the three condition walks collapsed to one — `ValidateConditions()`, `TryPayCost()` and the `AbilityHolder` walk are all deleted. New defects from the same commit are **BUG-089** and **BUG-091**, not this bug. *Original entry:* | Original double-charge **fixed** by a design change in `73ab8e7` (two disjoint cost lists now exist). Owner review 2026-09-22 then resolved two of three replacement defects: **(a)** Hold re-charging per effect is **by design** (channelled cast) *and dormant* — no effect asset carries a per-effect `Costs` list; **(b)** effect-scope payment **is** gated by `CheckPayCostValid()` (`AbilityEffectDefinition.cs:20`) before `TryPayEffectCost()` — finding withdrawn, `Casting()` to be renamed `TryCasting()`. **Still open: (b′)** `TryPayCost()` (`AbilityInstance.cs:164-178`) has no affordability gate at all and `HasEnoughManaCondition` covers Mana only, so Avatar of Light's 50 HP cost is unvalidated and `Reduction` clamps at 0 → cast at low HP drains to zero without dying; **(c)** `Definition.Conditions` is walked **3×** per activation (`AbilityHolder.cs:109-112`, `AbilityInstance.cs:44`, `:166-171` — walks 2 and 3 five lines apart in one call), and each walk writes to a committed asset via BUG-077 | [AbilityInstance.cs:164](Assets/Script/System/Abilities/Core/AbilityInstance.cs#L164) |
  | BUG-077 | DATA | ✅ **FIXED 2026-09-22 by `2a83469`** — `HasEnoughManaCondition.cs` and its `.meta` deleted; all four Paladin ability assets cleaned to `Conditions: []`; the affordability check moved up into `AbilityDefinition.TryStart()` and is now generic over `StatType`. Residual, filed separately: **BUG-090** (the orphaned `.asset`). *Original entry:* | `HasEnoughManaCondition.currentMana` / `.costMana` (`:6-7`) are public `ScriptableObject` fields written on every `IsMet()` (`:11-12`) — and **never read**: `:13-14` recompute both values for the comparison. Same class as BUG-063. GUID `02cbacd1d0342774abb852a2ed63f9b4` is shared by **all four Paladin abilities** (plus both ShootSpirit assets), which overwrite each other in one file, 3× per cast (BUG-076c). ⚠️ It checks **Mana only** — Avatar of Light costs 40 Mana **+ 50 HP** and the HP half is validated by nothing; `Reduction` clamps at 0, so casting at low HP silently drains to 0 without dying | [HasEnoughManaCondition.cs:6](Assets/Script/System/Abilities/Conditions/HasEnoughManaCondition.cs#L6) |
  | BUG-078 | LOGIC | ✅ **CLOSED 2026-09-22 — half fixed, half by design** | Inverted `Casting()` return **fixed** in `73ab8e7` (`SpawnSummonEffect.cs:20-25`). The "double spawn" half was a **misreading and is retracted**: the effect deliberately drives two objects. Verified in `Paladin Spawn Consecrat Effect.asset` — `Prefab` = `RuneCircle.prefab` (Cast-phase telegraph, `Execute()` = do nothing, correct for something that deals no damage), `summonPrefab` = `Lightning.prefab` (Do-phase payload, `SummonExecute`). Telegraph-then-strike. No code change required | [SpawnSummonEffect.cs:14](Assets/Script/System/Abilities/Effects/SpawnSummonEffect.cs#L14) |
  | BUG-079 | ARCH | ⚠️ OPEN (S3, downgraded) | `AbilityInstance.Exit()` body commented out (`:73-76`; the comment still names the pre-rename `SkillState`). ⚠️ **Corrected 2026-09-22: the "castable once per scene load" consequence is wrong** — `AbilityHolder.StartHold()` (`:143-148`) forces `ChangeState(AbilityState.Start)` on every fresh press via `PlayerInputHandle.cs:254-256`, so abilities do re-cast. What remains is a layering defect: the instance cannot reset itself, and any future activation path that skips `StartHold()` gets a stuck instance | [AbilityInstance.cs:73](Assets/Script/System/Abilities/Core/AbilityInstance.cs#L73) |
  | BUG-080 | LOGIC | ✅ **FIXED 2026-09-25 (ADR-0005)** — `ResourceReceiver` is now `: Interact` with an empty body. The `vitalStatsComponent` field, `Recovery()`, `Reduction()` and `BuffDebuffForDuration()` are all gone, so there is nothing left to dereference unresolved. Items apply themselves to the owning `ICharacter` via `ItemController` *(was: ⚠️ **OPEN (new 2026-09-22)**)* | `ResourceReceiver.vitalStatsComponent` is assigned only inside `ReceverModifierGroup()`; `Recovery()` / `Reduction()` / `BuffDebuffForDuration()` dereference it unresolved → NRE on the live item-pickup path. Split out of BUG-067 | [ResourceReceiver.cs:5](Assets/Script/Character/Player/CoreComponent/ResourceReceiver.cs#L5) |
  | BUG-081 | ARCH | ✅ **FIXED 2026-09-25 (ADR-0005)** — exactly one `INegativeReceiver` implementer project-wide: `NegativeReceiverBase<TCore>` (`Character/Base/`), which `NegativeReciver` and `EntityNegativeReciver` derive from. `ResourceReceiver` dropped the interface. `PlayerTest.prefab` still carries both components and that is now correct (hurtbox receiver vs pickup interactor). Both `EnemyPrefab.prefab` files carry **0** references to the player receiver. Closes TD-047 *(was: ⚠️ **OPEN (new 2026-09-22)**)* | Two `INegativeReceiver` implementers on the player, byte-identical bodies, **both on `PlayerTest.prefab`** — which one runs depends on collider layout. Player-side twin of BUG-053. Also: `EnemyPrefab.prefab` carries the *player* `NegativeReciver` | [ResourceReceiver.cs:29](Assets/Script/Character/Player/CoreComponent/ResourceReceiver.cs#L29) |
  | BUG-082 | LOGIC | ✅ **FIXED — committed 2026-09-25.** The last residual closed in `c0067f4`: `AbilityEffectDefinition.SubConditions` / `.SubEffects` / `.Costs` all carry `= new()` (`:7-9`). Every list on both ability SO classes now has an initialiser, which also makes `AbilityInstance.PayCost():131-137` safe *(was: ✅ **FIXED 2026-09-22** (owner, working tree) — operands swapped at `AbilityEffectDefinition.cs:26` so the null test runs before the dereference; the second copy at `AbilityDefinition.cs:67-75` deleted outright along with its zero-caller method. ⚠️ One site of the same shape remains elsewhere and is not this bug: `AbilityEffectDefinition.SubConditions` (`:7`) has no `= new()` and is dereferenced at `:13`. *Original entry:*)* | Null check after dereference: `if (Costs.Count == 0 \|\| Costs == null)` — `\|\|` short-circuits so `.Count` throws first; the null test is unreachable. Latent only because Unity's serializer materialises empty lists | [AbilityEffectDefinition.cs:26](Assets/Script/System/Abilities/Core/AbilityEffectDefinition.cs#L26) |
  | BUG-083 | LOGIC | ⚠️ **OPEN (new 2026-09-22)** | `AbilityContext.HoldTime` / `.HoldRatio` are plain fields snapshotted by `BuildContext()` *before* `StartHold()` zeroes the counter, and never rebuilt — both are always `0f`, so every charge-scaling effect is dead. Masked today: all four Paladin abilities have `MaxHoldTime = 0` | [AbilityContext.cs:11](Assets/Script/System/Abilities/Core/AbilityContext.cs#L11) |
  | BUG-084 | BUILD | ⚠️ **OPEN (new 2026-09-22)** | **Zero `.asmdef` under `Assets/`**, and `tests/EditMode` + `tests/PlayMode` sit *outside* `Assets/` — Unity compiles nothing there and Test Runner cannot discover a test. This is the precondition of TD-014, not a symptom: "write the first EditMode test" cannot start | — |
  | BUG-085 | LOGIC | ✅ **FIXED 2026-09-22 by `2a83469`** — `NotDeadCondition.cs` and its `.meta` deleted; `Assets/Script/System/Abilities/Conditions/` is now empty, so the `[CreateAssetMenu]` hazard is gone. *Original entry:* | `NotDeadCondition.IsMet()` body is commented out, always returns `true`. Dormant — no asset references it — but it carries `[CreateAssetMenu]`, so a designer can author a silent no-op gate at any time | [NotDeadCondition.cs:6](Assets/Script/System/Abilities/Conditions/NotDeadCondition.cs#L6) |
  | BUG-086 | LOGIC | ⚠️ **OPEN (new 2026-09-22)** | `PlayerDeathState.LogicUpdate()` never consumes `Status` and never changes state → `Emit(ON_PLAYER_DEATH)` fires **every frame**, unbounded. Violates the durable-`Status` contract in `manager-event-code.md`. Was only a parenthetical inside BUG-065 | [PlayerDeathState.cs:14](Assets/Script/Character/Player/States/PlayerDeathState.cs#L14) |
  | BUG-087 | LOGIC | ⚠️ **OPEN (new 2026-09-22)** | `ON_PLAYER_DEATH` has **zero subscribers**; `grep GameManager Assets --include=*.cs` = 0 hits; player-side `VitalStatsComponent` has no `Reborn()`; `ON_REALOAD_GAME` has 0 emitters and 0 subscribers. **Death is a permanent hard lock.** Closes the open half of Bug #6 / S10-08. Enemy side has the pattern to copy (`EntityVitalStats.Reborn()`, called from `Start()` and `OnEnable()`) | [PlayerDeathState.cs:18](Assets/Script/Character/Player/States/PlayerDeathState.cs#L18) |
  | BUG-088 | BUILD | ✅ **FIXED 2026-09-25.** `379c267` made the divisor a float literal (`Random.Range(10, 100)/100f`) and `b0b1337` tuned the range to `Random.Range(30, 75)`, so `rangeToCheck` lands in `[0.30, 0.74]` and `Vector2.Lerp` returns a real intermediate point. The related `EntityNegativeReciver.cs:27` line was removed by the ADR-0005 rewrite of that file *(was: ⚠️ **OPEN — PARTIAL** (S1 → S3 once committed))* | ✅ Fixed in the working tree: `Random.range`→`Random.Range`, `trasnform.postion`→`transform.position`, `Vector3.Lerp`→`Vector2.Lerp`. **The project compiles again.** ❌ Still open: `Random.Range(10, 100)/100` binds the `int` overload, so it is **integer division and evaluates to 0 for every draw** (max numerator 99). `Vector2.Lerp(a, b, 0)` returns `a`, so the method assigns the entity its own position and the knockback-target feature is silently dead — no error, no log. Fix: `/ 100f`. Also open: `EntityNegativeReciver.cs:27` unguarded dereference. *Original entry:* | `EntityMovement.SetPositionToCheck()` (`:161-165`, added in `723fab1`) calls `Random.range` — the member is `Random.Range` — and `trasnform.postion`, two typos naming nothing. CS0117 + CS0103: `Assembly-CSharp` does not build, so no Play Mode, no Test Runner, no verification of any other bug. Also, even once it compiles, `Random.Range(0, 100)/100` is **integer division** and is always `0`, so the method returns the entity’s own position. Related, same commit: `EntityNegativeReciver.cs:27` dereferences the `GetCoreComponent` result unguarded on the live damage path | [EntityMovement.cs:161](Assets/Script/Character/Entity/CoreComponent/EntityMovement.cs#L161) |
  | BUG-089 | LOGIC | ✅ **CLOSED 2026-09-22 — by design. ⏭️ REOPEN at demo/release phase** (owner decision which). Closed as *scaffolding*, not as resolved: the gain fields do not exist and no live content exercises the path. Both change the moment the project leaves development — the serialized `Costs` field is visible on every effect asset with no in-code guard, and force-release has never actually run (the only `Hold` ability carries no effect costs). The bug file keeps a seven-point re-check list, numbered to match the original gap audit | Per-effect `Costs` is the price of an **upgrade tier**: cannot afford → the effect still runs at its **default** level; can afford → charge and run at the **gained** level. So `Apply()` being unconditional is **correct**, and on a `Hold` ability a refusal additionally **force-releases** the ability — `CancelHold()` is that mechanism, not a failed abort. The flow is deliberate scaffolding; the gain fields are not designed yet. Unaffected today: three of the four live Paladin abilities are `Active` (`ActivationType: 0`) and no effect asset carries `SubConditions` or `Costs`. ⚠️ **Do not author a per-effect `Costs` list until the tier fields exist** — it would take resources and give nothing back. `BUG-089.md` holds the specification. Effect list order promoted to a rule in `weapon-skill-code.md`: it sets both execution order and resource priority. *Original entry:* | Owner stated the design: a per-effect `Costs` entry is the price of an **upgrade tier** — cannot afford → the effect still runs at its **default** level; can afford → charge, and run at the **gained** level. Under that design `Apply()` being unconditional is **correct**, so the earlier "a refused effect is applied anyway" claim is **withdrawn**. The implementation does not deliver the design. Seven gaps; the first is decisive: **paying buys nothing.** `Apply(AbilityContext)` receives no record that a cost was paid, and every concrete effect applies one fixed serialized value (`SpawnProjectileEffect.baseDamage`, `SpawnSummonEffect.baseDamage + PhysicalDamage`, `StatsEffectBase.statModifier`, `BuffDebuffStatsForDuration.statModifierGroup`) — there is exactly one power level in the code, so affording the tier is strictly worse than not affording it. Also: a refusal calls `CancelHold()`, which terminates the **whole channel** for **all** effects rather than dropping one effect to default; and `Costs` is a single all-or-nothing bundle, not a tier ladder. Dormant — no effect asset carries `SubConditions` or `Costs`. Treat the bug file as the feature spec, not a patch list. Dormant either way: every live effect asset has `SubConditions: []` and no serialized `Costs`. *Original entry:* | A refused `TryCast()` does not stop the cast. `Casting():79` calls `CancelHold()` — which clears `IsHolding` and returns nothing — and `CastInstant():52` then reads that cleared flag, does **not** return, and falls through to `ChangeState(Do)`. `DoInstant()` runs `Execute()`, which applies **every** effect unguarded, including the one that just refused, and starts the cooldown. Also desynchronises `AbilityInstance.IsHolding` from `AbilityHolder.IsHolding` | [AbilityInstance.cs:68](Assets/Script/System/Abilities/Core/AbilityInstance.cs#L68) |
  | BUG-090 | DATA | ⚠️ **OPEN (new 2026-09-22, S3)** | `Assets/SO/Skill/Conditions/Has Enough Mana Condition.asset` survived the deletion of its script. Its `m_Script` guid `6f896bb258601fc4cb5fc183399618ea` resolves to **0** `.meta` files; `ShootSpirit.asset` and `SpiritBomd.asset` still reference the asset, and both are reachable from `PlayerTest.prefab`. Second missing-script reference in that pair — bundle with BUG-073 | — |
  | BUG-091 | LOGIC | ✅ **FIXED — committed 2026-09-25** in `379c267` (was: working tree). Null-element guards at `AbilityDefinition.cs:58,63`; `CheckPayCostValid()` deleted; all three lists carry `= new()`, which makes the `&&` early-return case and the `GetCostValues()` walk unreachable. No residual *(was: ✅ **FIXED 2026-09-22** (owner, working tree) — null-element guards added to both `TryStart()` loops; `CheckPayCostValid()` deleted; operand order corrected (BUG-082); `Costs` (`:25`) given `= new()`, which also makes the `&&` early-return case and the unguarded `GetCostValues()` walk **unreachable** — all three lists on this class now carry initialisers, so neither loop can meet a null. Residual tidy-up only: the `&&` block at `:52-55` is now redundant and reads as a null guard it no longer needs to be. *Original entry:*)* | ✅ Owner added `if (condition == null) continue` and `if (cost == null) continue` to `TryStart()` in the working tree, restoring the null-**element** guard (and giving the `Costs` loop one the original never had). `StatCost` is a class, so `cost == null` is valid. ❌ Still open, and re-stated after owner review: the combined early return is joined by **`&&`**, so it fires only when *both* lists are empty — `Conditions == null` with a non-empty `Costs` (or the reverse) does **not** return, and the corresponding `foreach` then walks a null reference. ⚠️ A fifth site was found: `GetCostValues()` (`:74-77`) iterates `Costs` with no list guard and no element guard at all. Also still open: `CheckPayCostValid()` (`:67-75`) still has zero callers; the `||` operand order (BUG-082) is still at two sites; `Costs` (`:25`) still lacks `= new()`. Deleting `CheckPayCostValid()` closes two of those at once. *Original entry:* | Three defects in the new `AbilityDefinition`: `TryStart()` (`:50-64`) lost the `if (condition == null) continue` guard the deleted `ValidateConditions()` had (BUG-090’s asset is what would trigger it); its `&&` early-return lets a null `Costs` or null `Conditions` reach a `foreach`; and `CheckPayCostValid()` (`:67-75`) is a zero-caller copy carrying the BUG-082 null-check-after-dereference verbatim | [AbilityDefinition.cs:50](Assets/Script/System/Abilities/Core/AbilityDefinition.cs#L50) |
  | BUG-092 | BUILD | 🔴 **OPEN — NEW 2026-09-25, S1 BLOCKER** | `c0067f4` deleted `public float perTime;` from `RecoveryReductionPerTimeForDuration.cs` and left both uses (`:14`, `:17`) — **`error CS0103` twice**, so `Assembly-CSharp` does not build and **no bug in this register can be verified in the Editor**. Second compile break committed in four days (BUG-088 was the first); the absence of any pre-push compile check is **TD-048**, with this recurrence filed as **TD-051**. ⚠️ **A second defect survives the compile fix:** `timeCount` (`:8`) is `private` with no `[SerializeField]`, so Unity never serializes it and `[Range(1, 10)]` draws nothing — it is `0` on every asset, and `VitalStatsBase.RecoveryPerTime` decrements before testing, so **every HoT/DoT runs exactly one instant tick, silently**. `duration` (`:6`) is now read by nothing. Fix: serialize `perTime` and `timeCount`, delete `duration` | [RecoveryReductionPerTimeForDuration.cs:14](Assets/Script/System/Abilities/Effects/RecoveryReductionPerTimeForDuration.cs#L14) |
  | NEW-1 | LOGIC | ✅ FIXED | `EntityInput` target detection restored; `EntityFindTarget` performs FOV + range + obstacle checks | [EntityFindTarget.cs](Assets/Script/Character/Entity/CoreComponent/EntityFindTarget.cs) |
  | NEW-2 | LOGIC | ✅ FIXED | `EntityStatsSO.ModifiersAmor` getter/setter recursion → `StackOverflowException`; entire `EntityStatsSO.cs` deleted, entities use `EnemyStatSO : BaseStatsSO` | — |
  | NEW-3 | LOGIC | ✅ FIXED | `RecalculateDerived()` skip-guard used `\|\|` where it needed `&&` — fixed on `sprint-10`, carried into `BaseStatsSO` | [BaseStatsSO.cs](Assets/Script/System/StatSystem/BaseStatsSO.cs) |
  | NEW-4 | DATA | ❌ **REGRESSED → BUG-063** | Previously "FIXED — `Stat.modifiers` no longer serialized". No longer true; see BUG-063 | [Stat.cs:63-65](Assets/Script/System/StatSystem/Stat.cs#L63) |

  ### Orphan directories (cleanup, not bugs)

  `Assets/Script/Character/Boss/` and `Assets/Script/Handler/` contain only `.meta` files and no
  `.cs`. They are the residue of the boss feature that was added in `ffe1976` and reverted in
  `4421fdc` / `ff67f4d`. No document recorded that revert until now.

  ---

  ## Coding Conventions

  - **No comments** unless the WHY is non-obvious (hidden constraint, workaround, surprising invariant).
  - **ScriptableObject-first**: game data (abilities, enemies, weapons, items) lives in SO assets, not hardcoded values.
  - **No new singletons** — `MazeController` and `EnemyManager` (ratified exception, ADR-0002) are the only permitted singletons; `LevelManager` is a standing unratified violation (TD-023). Cross-system services now go through **VContainer** (ADR-0004); sibling components still use `Core.GetCoreComponent<T>()`.
  - **State machine for all characters**: new behaviour = new `PlayerState` / `EntityState` subclass, never inline `if/else` in `Update`.
  - **Skills**: new *player* abilities are authored as `AbilityDefinition` SO assets composed from effects + conditions (Abilities v2). Do **not** add new `ActivateSkill` subclasses — v1 is in maintenance for the weapon/enemy path only.
  - **Damage**: `INegativeReceiver.TakeDamage(float amountDamage, Vector2 attackPosition)` — note `float`, not `int`.
  - **Layer masks** must be set in Inspector; never hardcode layer indices.
  - **Preserve intentional typos** — `Resgister`, `INegativeReciver.cs`, `attackDamege`, `Modal`, `ENEBLE`, `CaculateIndex`, `currrentSA`, `deplayTime`, `IObjecPoolService`, `amoutDamage` are real contracts in code and in serialized assets.
  - **Known file/class name mismatches** (do not "fix" casually — prefab and scene references depend on the file name): `PlayerInputHandle.cs` → `PlayerInputHandler`, `TalentManager.cs` → `TalentManagger`, `VitalComponent.cs` → `VitalStatsComponent`, `CoreCompoment.cs` → `CoreComponent<T>`.

  ---

  ## Demo Completion Checklist

  1. ~~**Fix map compile errors**~~ ✅ Done — bugs 1-3 SUPERSEDED.
  2. ~~**Dungeon navigation + random room load**~~ ✅ Done.
  3. ~~**Level editor tool**~~ ✅ Done — `LevelManager` + `LevelManagerEditor`.
  4. ~~**Fix EventManager build break**~~ ✅ Done (Bug #10).
  5. ~~**Fix player melee damage**~~ ✅ Done (Bug #4).
  6. **Player death** ⚠️ (Bug #6 / S10-08) — ✅ `PlayerDeathState` constructed and emitting `ON_PLAYER_DEATH`; ✅ health routes through `VitalStatsComponent`. **Re-verified 2026-09-22: the remaining half is worse than recorded.** `ON_PLAYER_DEATH` has **zero subscribers**, `grep GameManager Assets --include=*.cs` returns **0 hits**, `VitalStatsComponent` has no reset path at all (`currentStats` is filled once in `Start()`), and `ON_REALOAD_GAME` has 0 emitters and 0 subscribers — so **death is a permanent hard lock with no recovery** (**BUG-087**). Also needs **BUG-086** (the event currently fires every frame) and **BUG-065** (stop `PlayerMovement` on death). The enemy side already has the pattern to copy: `EntityVitalStats.Reborn()`, called from both `Start()` and `OnEnable()`.
  7. ~~**Deploy enemy** (Bugs #5, #7, #8)~~ ✅ Done.
  8. ~~**Room clear condition**~~ ✅ Done — `RoomCell.EnemyCount` → `ON_CLEAR_ENEMY`.
  9. **HUD** ⚠️ — `UIManager` is still an empty stub. `StatsUIController`, `StatsScreenUIController` and UI Toolkit menus exist but have no GDD; the **player** health/mana bar is still displayed nowhere (enemies now have one via `EntityUIController`).
  10. **Between-room upgrade** ⚠️ — after room clear: pause, offer 3 stat cards, apply through `BaseStatsSO.AddModifiersFromSource()`. `StatPointAllocator` provides the allocation-session mechanics.
  11. ~~**Fix AnimationPlayerController**~~ ✅ Done (Bug #9).
  12. ~~**Combo attack**~~ ✅ Done — `WeaponStats.AttackStages` + `Weapon.OnAttackEnter()`.
  13. **Fix start-room teleport** ⚠️ (Bug #13).
  14. **Build-safe room JSON loading** ⚠️ (Bug #15) — replace `File.ReadAllText(Application.dataPath…)` with `TextAsset` refs or StreamingAssets.
  15. **Enemy spawn system** ⚠️ — GDD + ADR-0002/0003 exist. ✅ BUG-033 fixed; ✅ unblocked now that BUG-053 is closed. **Open:** BUG-ES-2 (two parallel spawn drivers), `overflowPercent` declared but never read, and the `retry > 4` fallback in `SetListCandidate()` that breaks ADR-0003's budget guarantee.
  16. ~~**Enemy targeting**~~ ✅ Done (NEW-1) — `EntityFindTarget` performs FOV + range + obstacle checks.
  17. ~~**Enemy damage/death chain**~~ ✅ Done (BUG-042/043/046/053, NEW-2) — one `INegativeReceiver` per side, Defense applied, health bar updated. ⚠️ Residual: BUG-066 key guard, BUG-043 `EntityAttack` duplicate.
  18. **Reconcile the two ability frameworks** ⚠️ NEW — decide whether `ActivateSkill` (v1) migrates to `AbilityDefinition` (v2) or stays as the weapon/enemy path permanently. Needs an ADR; blocks `design/gdd/skill-ability-system.md` from being authoritative again.
  19. **Fix BUG-063** ⚠️ NEW — one-line removal of the `#if UNITY_EDITOR [SerializeField]` on `Stat.modifiers`, before more runtime buffs are committed into `.asset` files.
  20. **Zero tests** ⚠️ (TD-014) — `tests/EditMode/`, `tests/PlayMode/` still contain only `.gitkeep`. **Re-verified 2026-09-22: this is not merely unstarted, it is currently impossible** (**BUG-084**). There is **no `.asmdef` anywhere under `Assets/`** (`find Assets -name "*.asmdef"` = 0), so no assembly can reference `UnityEngine.TestRunner`; and `tests/` is a *sibling* of `Assets/`, so Unity compiles nothing in it regardless. The root `GameRPG.Combat.EditModeTests.csproj` is an untracked, stale IDE artifact pointing at an `Assets/Tests/` directory that does not exist. Every sprint story estimating "write the first EditMode test" at 0.3d is mis-scoped until BUG-084 is done.
  21. **Make Abilities v2 actually work** ⚠️ (raised 2026-09-21, re-scoped three times; latest
      2026-09-25 post-pull at HEAD `c0067f4`) — **fix BUG-092 first: the project does not compile, so
      none of this can be tested.** Then: **(1) BUG-072** — set `layerMask` on `Lightning.prefab`;
      the code has been complete since 2026-09-22 and this is one Inspector field. **(2) a Play Mode
      smoke on all four Paladin abilities**, which converts BUG-075 and BUG-072 from static analysis
      into verified behaviour and closes S14-01 / S14-14. **(3) BUG-071 defect 3** (the tick chain
      cannot be stopped) **/ BUG-083** (`HoldRatio` always `0f`) **/ BUG-068 `:28` / BUG-079.**
      ✅ No longer on this list: BUG-074, BUG-075, BUG-076, BUG-077, BUG-078, BUG-082, BUG-085,
      BUG-091 — all closed; BUG-089 closed by design, reopen at demo/release.
      ⚠️ **A GDD for v2 is still missing** and is still the real blocker for anything new.
      *Superseded ordering, kept for the record:* **fix BUG-088 first.** Then **(1) BUG-075** — one
      word, `OnTriggerEnter2D(Collider2D)`, restores projectile damage on its own. **(2) BUG-072** —
      implement `LightningController`’s overlap query, assigning the receiver and invoking the
      callback once per target; restores summon damage, independent of (1). **(3) BUG-089** — a
      refused `TryCast()` must stop the cast. **(4) BUG-071 / BUG-074 / BUG-082+BUG-091 / BUG-083.**
      **The design question that remains is
      narrow**: whether a `TryCast()` override may have side effects (Consecrate says yes and is
      correct to), and whether a channelled cost should meter per dispatch or per second. Neither
      blocks (1)-(4). *Superseded ordering, kept for the record:*
      player's only ability path, and at HEAD it **deals no damage of any kind**. Fix in this order:
      (1) **BUG-072 + BUG-075 together** — the 3D `OnTriggerEnter` is the only writer of
      `Services.NegativeReceiver`, so fixing the signature is necessary but not sufficient; the
      callback contract (`Action<AbilityContext>`, which has no parameter for the thing that was hit)
      is the actual defect. (2) **BUG-077** — one-line deletion of two write-only serialized fields,
      stops runtime state reaching a committed asset today. (3) **BUG-071** — HoT/DoT never starts;
      fix all three defects in that block at once, not just the `StartCoroutine`. (4) **BUG-076 (a)**
      — Hold abilities re-charge per-effect cost every dispatch, live on Blessed Slash.
      (5) **BUG-078** double spawn and **BUG-074** delta-vs-total, both of which gate Consecrate.
      Downgraded and no longer demo-blocking: BUG-068 (dead code), BUG-079 (layering, not a lock).
      Closed: BUG-069. **A GDD for v2 is still the real blocker** — `Casting()`'s contract, the cost
      model, the cooldown model and the spawn-callback contract are all undefined, so BUG-072,
      BUG-076 and BUG-078 cannot be fixed without a design decision first.
  22. **Author the remaining Paladin ability animations** ⚠️ NEW (2026-09-21) — Consecrate now has all
      8 directions × 3 states wired into `Paladin Consecrate.overrideController`. Blessed Slash,
      Blessing and Avatar of Light still have no per-direction clips.
  23. **Unlock the test pipeline** ⚠️ NEW (2026-09-22, **BUG-084**) — prerequisite for TD-014 and for
      every "write the first EditMode test" story in sprints 14 and 15. No `.asmdef` exists anywhere
      under `Assets/`, and `tests/EditMode` / `tests/PlayMode` sit outside `Assets/` where Unity
      never compiles them. Needs: a decision on where tests live, a runtime `.asmdef` for gameplay
      code (a breaking change in its own right — it splits `Assembly-CSharp` and surfaces every
      implicit cross-directory dependency at once), a test `.asmdef`, and a correction to
      `.claude/rules/test-standards.md`, which currently points at the non-compiling paths.
  24. **Give player death a recovery path** ⚠️ NEW (2026-09-22, **BUG-087** + **BUG-086**) — split out
      of item 6 because it is a feature, not a bug fix: a `GameManager` (scene component registered in
      `GameLifetimeScope`, **not** a singleton), a `VitalStatsComponent.Reborn()` mirroring
      `EntityVitalStats.cs:31-35`, and a subscriber for `ON_PLAYER_DEATH` — which must not be added
      until BUG-086 stops the event firing every frame.

  ---

  ## Enemy Definitions

  `EnemySO` (spawning/drop data): name, level, speedMove, fieldOfViewRange, rateAttack, attackRange, damage, powerShoot, projectile, layerMask, depotItem. ⚠️ Not consumed by `Entity` (TD-030).

  `EntityData` SO (AI runtime): statsSO, layerMask, aima (AnimatorOverrideController), rangeCheckFieldOfView, idleDurationTime, moveDurationTime, movementVelocities, rangeCheckAttack, weaponSO.

  `EnemyStatSO : BaseStatsSO` — the stat profile an entity's `EntityStatsHandler` reads.
  Assets: `Assets/SO/Stat/Enemy/{Assasin,TrashMelee,FastSwarm,RangedCaster,Tank}Stats.asset` + `Enemy/Boss/BossStats.asset`.

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
