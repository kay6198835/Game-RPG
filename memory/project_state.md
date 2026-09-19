# Project State

Updated **2026-09-11** (full documentation/code re-synchronisation; every claim below re-verified
against `Assets/Script/` at HEAD `6d6a8e4`). Previous update was 2026-08-21 — **three sprints
stale**, and its entire "Open bugs" table had been overtaken by events.

Snapshot of actual code state. Source of truth for "what is really implemented" —
`CLAUDE.md` carries the same facts in long form. Per-document change reasons live in
`docs/CHANGELOG-DOCS.md`.

---

## Structural changes since the last update (2026-08-21 → 2026-09-11)

Four changes landed with **no documentation entry at the time**. They are the root cause of
nearly every stale reference found in this audit.

| # | Change | Commit | Impact on docs |
|---|--------|--------|----------------|
| R1 | Seven top-level directories moved under `Assets/Script/System/` — `Enemy/`, `Pathfinding/`, `Poolable/`→`PoolableService/`, `StatSystem/`, `Skill_Ability/`, `Item/`, `LifetimeScope/`. 77 renames | `1c0742e` (2026-09-03) | Every path in every doc written before this date is wrong |
| R2 | **VContainer 1.19.0** dependency injection adopted | `aa4e620` (2026-08-22) | No ADR existed until ADR-0004 (2026-09-11); contradicts `engine-code.md` as written |
| R3 | `StatsSO.cs` deleted → `BaseStatsSO` + `EnemyStatSO`; `StatPointAllocator` added | `b0512f4`, `1c0742e` | `stat-system.md`, `adr-0001`, `combat-balance` all reference a deleted class |
| R4 | `prototypes/skill-enhance-abilities/Scripts/` promoted into `Assets/Script/System/Abilities/` | `9b8d40f`, `5c7afba` (2026-09-09) | Prototype README, ability diagrams and the skill GDD all inverted |

Also unrecorded until now: a boss system was added (`ffe1976`) and reverted (`4421fdc`, `ff67f4d`),
leaving `Assets/Script/Character/Boss/` and `Assets/Script/Handler/` as `.meta`-only orphans.

---

## Systems completed since the last doc update

| System | Notes |
|--------|-------|
| **Dependency injection** | `GameLifetimeScope : LifetimeScope` registers 9 scene components; `IObjecPoolService` / `IPlayerService` / `IPlayerStatService`. `[Inject] Construct()` on `AbilityHolder`, `ItemSpawner`, `ObjectPoolManager`. **ADR-0004 (2026-09-11)** |
| **Enemy damage/death chain** | **BUG-053 CLOSED.** `EntityNegativeReciver` is now the single enemy `INegativeReceiver`: `DamageCalculate()` applies `StatType.Defense`, writes `EntityVitalStats`, refreshes `EntityUIController`. `EntityCore.TakeDamage()` removed (BUG-042), `EntityWeaponMelee.cs` deleted (BUG-043/046), `EntityStatsSO.cs` deleted (NEW-2) |
| **Player stat / vitals split** | `StatHandler : IPlayerStatService` (max values) + `VitalStatsComponent : IVitalComponent` (current values) + `ResourceReceiver : INegativeReceiver, IResourceReceiver`. `NegativeReciver` now delegates into `VitalStatsComponent` instead of owning a private `currentHealth` |
| **Abilities v2** | Composition framework promoted out of `prototypes/`: `AbilityDefinition` (SO) → `AbilityInstance` → `SkillState` (None/Start/Cast/Do/Exit), bound per `AbilitySlot`. **Now has live SO assets** (`SO/Skill/ShootSpirit/*.asset`, `SO/Skill/Conditions/*.asset`) — it is wired and running |
| **Item system** | `ItemSO`, `DepotItem` (weighted drop table by `RarityTierItem`), `ItemController`, `ItemSpawner`, `PrefabRandomItem`, 4 `ItemEffectDefinition` SOs, `DepotItemEditor`. `PlayerResourceReceiverState` handles pickup. **No GDD, no ADR** |
| **Enemy target detection** | **NEW-1 CLOSED.** `EntityFindTarget` performs FOV + range + obstacle-mask checks and feeds `EntityInput` |
| **Stat allocation UI** | `StatPointAllocator` (accept / revert / restore session) + `StatsScreenUIController`; drives `ON_RESET_STATS_UI_SESSION` |
| **Enemy health bar** | `EntityUIController` — per-enemy HP bar driven off the damage chain |

---

## Open bugs (verified against source 2026-09-11)

| # | Sev | Description | Location |
|---|-----|-------------|----------|
| BUG-063 | **CRITICAL** | `Stat.modifiers` re-serialized via `#if UNITY_EDITOR [SerializeField]` — reopens the data-corruption bug `f5de65a` closed. Runtime buffs can again be committed into `.asset` files. One-line fix, carried 24+ cycles | `Stat.cs:63-65` |
| BUG-064 | HIGH | Entity refactor callers sweep — sub-items 1-6 fixed, **sub-item 7 (`RangeWeapon` DI wiring) open** | `RangeWeapon.cs` |
| BUG-066 | HIGH | `EntityVitalStats` indexes `currentStats[statType]` with no key guard → `KeyNotFoundException` on the live damage chain | `EntityVitalStats.cs` |
| BUG-065 | MEDIUM | `PlayerDeathState.Enter()` only calls `base.Enter()` — player keeps sliding through the death animation | `PlayerDeathState.cs:10` |
| 6 | MEDIUM | Player death chain narrowed but not closed — `PlayerData.currentHealth` never written, `Reborn()` has no caller, no `GameManager` | `NegativeReciver.cs` |
| BUG-043 | MEDIUM | `EntityAttack.Attack()` still duplicates `EntityWeapon` and hardcodes `TakeDamage(10, …)` | `EntityAttack.cs:33` |
| — | MEDIUM | `RoomModel.SetListCandidate()` `retry > 4` fallback skips the weight filter, breaking ADR-0003's budget guarantee. `overflowPercent` serialized, never read | `RoomModel.cs` |
| 12 | MEDIUM | `LevelManager` singleton (bare `public static` field); `RoomGeneraterController.Setting()` reaches through it | `LevelManager.cs:10` |
| 13 | MEDIUM | Start-room teleport commented out; `RoomGeneraterController.OnDoneLoadRoomGrid()` has no caller | `RoomGridController.cs:82` |
| 14 | MEDIUM | `MazeController.Awake()` missing `return` after `Destroy(gameObject)` | `MazeController.cs:17` |
| 15 | BUILD | Room JSON via `File.ReadAllText(Application.dataPath…)` — Editor-only, breaks Player builds | `RoomGeneraterController.cs:69` |
| 16 | MEDIUM | `RoomType` never read at runtime; start/end rooms picked by list position | `RoomGeneraterController.cs:47` |
| 17 | LOW | Dead code: `DoorController.OpenDoor()`/`CheckCanBeOpened()`, `RoomCell.UpdateStatusDoor()` | `DoorController.cs:29` |
| BUG-052 | DOC | Live subsystems with no ADR — now also covers the Item system, Abilities v2 and the UI layer. VContainer left this set when ADR-0004 landed | — |
| — | DOC | **Two ability frameworks coexist** with no ADR deciding the endgame: `ActivateSkill` (v1, weapon/enemy) and `AbilityDefinition` (v2, player) | `System/Skill_Ability/` vs `System/Abilities/` |

**Closed since the last update:** BUG-042, BUG-046, BUG-033, BUG-053, NEW-1, NEW-2 (all verified
against source this pass). BUG-044 confirmed fixed but its scope was overstated — the
"stops PlayerMovement" half became BUG-065. **NEW-4 has REGRESSED** and is now BUG-063.

---

## EventID enum (current — 23 values)

> **Count history — read this before assuming a value was deleted.** The 2026-08-20 audit wrote
> "19 values", a miscount; the real figure was **18**, corrected 2026-08-21. On 2026-08-22 the
> StatsScreen UI added `ON_REVERT_STATS_BY_UI` + `ON_RESTORE_STATS_BY_UI` → **20**. Between
> 2026-08-22 and 2026-09-07 the allocator and Item systems added `ON_RESET_STATS_UI_SESSION`,
> `ON_DROP_ITEM` and `ON_COLLECT_ITEM` → **23**, recorded 2026-09-11.
> Nothing has ever been removed from `EventManager.cs`.

`ON_PLAYER_ON_DOOR`, `ON_PLAYER_DEATH`, `ON_REALOAD_GAME`, `ON_LOAD_MAZE_DONE`, `ON_LOAD_MAP`,
`ON_CLEAR_ENEMY`, `ON_GET_SPAWN_POSITIONS`, `ON_DONE_SPAWN_ENEMY`, `ON_SPAWN_EXTRA_ENEMY`,
`ON_TEST`, `ON_ENEMY_DEATH`, `ON_ROOM_CLEAR`, `ON_OPEN_STATS_PLAYER_UI`,
`ON_CLOSE_STATS_PLAYER_UI`, `ON_INCREASE_STATS_BY_UI`, `ON_DECREASE_STATS_BY_UI`,
`ON_CHANGE_STATS_BY_UI_RUN_TIME`, `ON_UPDATE_STATS_BY_UI`, `ON_REVERT_STATS_BY_UI`,
`ON_RESTORE_STATS_BY_UI`, **`ON_RESET_STATS_UI_SESSION`**, **`ON_DROP_ITEM`**, **`ON_COLLECT_ITEM`**

Still missing: **`ON_PLAYER_TAKE_DAMAGE`** — `.claude/rules/ui-code.md` instructs the health bar to
bind to it, but the value has never existed.

`ON_ROOM_CLEAR` exists in the enum but has no producer yet.

---

## Key API changes to be aware of

| Contract | Was | Is now |
|---|---|---|
| `INegativeReceiver.TakeDamage` | `(int amount, Vector2 pos)` | **`(float amountDamage, Vector2 attackPosition)`** |
| Player stat profile | `StatsSO` | **`BaseStatsSO`** (same API surface) |
| Enemy stat profile | `EntityStatsSO` (deleted) | **`EnemyStatSO : BaseStatsSO`** |
| Player ability lifecycle | `ActivateSkill.Enter/Activate/Cast/Do/Exit` | **`AbilityInstance` + `SkillState` enum** |
| Cross-system services | `GetComponent` / Inspector refs | **VContainer `[Inject]`** (sibling components still use `Core.GetCoreComponent<T>()`) |

---

## Stubs / unimplemented

- `UIManager` — empty stub (TD-017)
- `PlayerUserItemState` — extends `MonoBehaviour` instead of `PlayerState` (TD-001)
- `ICharacter` — empty interface, zero implementers, zero references
- `ICoreComponent` — memberless marker; `CoreBase.Setup()` blind-casts to `ICoreComponent<ICore>`
- `StatsCharacter` — legacy SO base, superseded by `BaseStatsSO`, no longer used by Player or Entity
- `SwordAndShield` — empty subclass of `MeleeWeapon`
- `DualAbility` — all code commented out
- `AnimationName.cs` — an empty `ScriptableObject` stub; real constants live in `GameConstants.AnimationName`
- `AnimationEventManager` — `Emit()` has zero callers; the whole class is dead
- `RoomModel.overflowPercent` — serialized, never read
- `PlayerData.Reborn()` — implemented, no caller
- `EnemySO` — not consumed by `Entity`, which reads `EntityData` (TD-030)
- `TalentManagger` — stats hardcoded in `Awake()`, not SO-driven (TD-018); overlaps StatSystem
- `Assets/Script/Character/Boss/`, `Assets/Script/Handler/` — `.meta`-only orphan directories
- `prototypes/skill-enhance-abilities/Scripts/` — `.meta`-only since the 2026-09-09 promotion
- `tests/EditMode/`, `tests/PlayMode/`, `tests/playtest/` — only `.gitkeep`; **zero tests exist** (TD-014)

---

## Undocumented systems (code exists, no design/architecture doc)

| System | Location | Gap |
|--------|----------|-----|
| Item / Depot | `Assets/Script/System/Item/` (8 files + 4 effect SOs) | No GDD, no ADR, absent from `systems-index.md` |
| Abilities v2 | `Assets/Script/System/Abilities/` (17 files) | No GDD of its own; `skill-ability-system.md` documents v1 only. `docs/diagrams/ability-system-diagrams.md` is the only accurate description |
| Pathfinding (A*) | `Assets/Script/System/Pathfinding/` (12 files) | No GDD, no ADR (BUG-052) |
| Shared hub layer | `Assets/Script/Character/Base/` (11 files) | No ADR (BUG-052) |
| Object pooling | `Assets/Script/System/PoolableService/` | No GDD |
| UI Toolkit menus + Stats UI | `Assets/Script/UI/` (4 files) | No GDD; `VERSION.md` used to advise against runtime UI Toolkit — corrected 2026-09-11 |

---

## Demo fix priority

1. **BUG-063** — remove the `#if UNITY_EDITOR [SerializeField]` on `Stat.modifiers` before more
   runtime buffs are committed into `.asset` files. One line, zero blockers, 24+ cycles carried.
2. **BUG-064 sub-item 7** — `RangeWeapon` DI wiring; the last piece of the Sprint 12 refactor.
3. **BUG-066** — add the key-existence guard in `EntityVitalStats`; it sits on the live damage chain.
4. **Play Mode confirmation** — every enemy-chain fix above has been "fixed pending Play Mode
   confirmation" for 6+ sprints. Console-clean + kill one enemy + fire the ranged weapon once.
5. **Player death** (Bug #6 + BUG-065) — stop `PlayerMovement` on death, write
   `PlayerData.currentHealth`, add a `GameManager` that calls `Reborn()` and reloads.
6. **Ability framework decision** — one ADR choosing whether v1 migrates to v2 or stays as the
   weapon/enemy path. Until then `skill-ability-system.md` cannot be made authoritative.
7. **Start-room teleport** (Bug #13), then **build-safe JSON loading** (Bug #15) before the first
   standalone build.
