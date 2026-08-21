# Project State

Updated 2026-08-21 (full documentation audit — every claim below re-verified against
`Assets/Script/`; previous update was 2026-07-09). Re-checked against `sprint-10` HEAD
`10023f0`, which landed a StatSystem UI prototype after the audit began.

Snapshot of actual code state. Source of truth for "what is really implemented" —
CLAUDE.md carries the same facts in long form.

---

## Systems completed since the last doc update (2026-07-09 → 2026-08-20)

| System | Notes |
|--------|-------|
| Weapon framework rewrite | `Weapon.cs` now owns the stage machine: `CanAttack()` / `CanChain()` / `OnAttackEnter(player)` / `OnActivate()` / `OnDeactivate()`. `WeaponStats` holds `AttackStages`, `AbilityWeapon`, `SkillWeapon`, `LayerMask` and a `StatModifierGroup`. `PlayerAttackState` never branches on `WeaponType`. Files renamed: `WeaponMelee.cs`→`MeleeWeapon.cs`, `WeaponMeleeStats.cs`→`MeleeWeaponStats.cs` |
| Player melee damage | `MeleeWeapon.OnActivate()` — `OverlapCircleNonAlloc` into a cached buffer + `INegativeReceiver.TakeDamage()`. **Bug #4 CLOSED** |
| Ranged weapons | `RangeWeapon` + `RangeAttackSO` + `RangeWeaponStats`: pooled projectiles, spread fan, per-stage `RecoveryTime`, `AutoFire`. `Shooting.cs` deleted |
| Shared base layer | New `Character/Base/`: `BaseEntity`, `CoreBase`, `CoreComponentBase`, `StateMachine<T>`, `IState`, `StatusAnimation`, `DirectionResolver`, `ICore`/`ICoreComponent`/`ICharacter`. Both `Core` and `EntityCore` now sit on top of it. **No ADR — BUG-052** |
| Animation handoff | Boolean flags replaced by the `StatusAnimation` enum + `SetAnimationStatus()`. `AnimationPlayerController` registration fixed — **Bug #9 CLOSED** |
| Enemy death chain (state side) | `EntityDeathState : EntityBasicState`, emits `ON_ENEMY_DEATH`; `EntityBasicState` transitions on `Health <= 0`. **Bugs #7 and #8 CLOSED** |
| Enemy move null-safety | `EntityMoveState.LogicUpdate()` guards `entityInput.TargetTransform` first. **Bug #5 SUPERSEDED** |
| Player damage endpoint | `NegativeReciver.TakeDamage()` implemented, emits `ON_PLAYER_DEATH`. **Bug #6 downgraded to PARTIAL** — it writes its own `currentHealth`, not `PlayerData.currentHealth` |
| Pathfinding | New `Assets/Script/Pathfinding/`: A*, Heuristic, PriorityQueue, Node/Path/PathRequest/SearchNode, GridBuilder, PathfindingGrid, PathRequestManager. Driven through `EnemyManager`. **No GDD, no ADR, absent from `systems-index.md`** |
| Object pooling | New `Assets/Script/Poolable/`: `ObjectPoolManager`, `Pool`, `PoolMember`, `IPoolable`. Supersedes the deleted `Pooling/ObjectPooling.cs` (TD-033). Consumed by `RangeWeapon`, `EnemySpawner`, `StatsUIController` |
| Room clear condition | `RoomCell.EnemyCount` + `OnDoneSpawnEnemy` / `OnSpawnExtraEnemy` / `OnEnemyDeath` → emits `ON_CLEAR_ENEMY` at zero. `RoomGridController` opens the doors. **Demo checklist item 8 DONE** |
| Enemy spawn pipeline | `RoomGeneraterController` parses `Tile_Spawn_Enemy` markers → `ON_GET_SPAWN_POSITIONS` → `EnemySpawner` draws a `RoomModel` from `MapModel`'s shuffle-bag, calls `GetSpawnSet()`, spawns pooled prefabs, emits `ON_DONE_SPAWN_ENEMY`. All 13 room JSONs now carry markers |
| Spawn selection algorithm | `RoomModel.GetSpawnSet()` rewritten to candidate-pool + `RarityTier` roll + retry-fallback (ADR-0003 Option C shape). `randomRatio`, `selectionWeight` and the Phase-2 fill are gone |
| EnemyManager | No longer a stub — it is now the **pathfinding service** (`SetPathfindingGrid`, `RequestPath`, `GetNodeByPositionWorld`). Its `Awake()` guard has the correct `return`. ⚠️ This is not the spawn-lifecycle role ADR-0002 describes |
| StatSystem wired into gameplay | `Player.cs` holds a `StatsSO`; `Weapon.Equid()` applies `stats.modifiers.ApplyTo(Player.Stats, this)` and `UnEquid()` removes by source. `StatsUIController` + `StatSlot` render the profile |
| UI Toolkit menus | `UI/UIController.cs` — runtime MainMenu / Settings / Pause from `.uxml`. **No GDD, no ADR, and `VERSION.md` currently advises against runtime UI Toolkit** |
| Editor tooling | `Assets/Editor/StatModifierTesterEditor.cs` added alongside `LevelManagerEditor.cs` |

---

## Open bugs (verified against source 2026-08-20)

| # | Sev | Description | Location |
|---|-----|-------------|----------|
| NEW-1 | BLOCKER | `EntityInput.Update()` has `//GetTargetInRange();` commented out — the only writer of `targetTransform`. Enemies never detect the player; `EntityAttackState` would NullRef if reached | EntityInput.cs:67 |
| BUG-042 | BLOCKER | `EntityCore.TakeDamage()` throws `NotImplementedException` | EntityCore.cs:11 |
| BUG-053 | BLOCKER | `EntityNegativeReciver` runs player logic on an enemy: resolves `PlayerInputHandler` off `EntityCore` (→ NRE) and emits `ON_PLAYER_DEATH` on enemy death | EntityNegativeReciver.cs:10 |
| — | BLOCKER | Enemy health has two disconnected stores: damage lands on `EntityNegativeReciver.currentHealth`, the death check reads `EntityStatsSO.Health`. Enemies cannot die | EntityBasicState.cs:30 |
| NEW-2 | HIGH | `EntityStatsSO.ModifiersAmor` getter and setter recurse into themselves → `StackOverflowException` (TD-011, open since 2026-05-31) | EntityStatsSO.cs:47 |
| 6 | HIGH | Player death chain incomplete — `PlayerData.currentHealth` never written, `Reborn()` has no caller, no `GameManager`, `PlayerDeathState` never constructed | NegativeReciver.cs:6 |
| BUG-044 | HIGH | `PlayerDeathState.LogicUpdate()` body fully commented out; state absent from `Player.Awake()` | PlayerDeathState.cs:17 |
| BUG-043 | MEDIUM | Two divergent enemy attack paths: `EntityWeaponMelee.Attack()` and `EntityAttack.Attack()` (the latter hardcodes damage `10`) | EntityAttack.cs:33 |
| BUG-033 | MEDIUM | `EnemySpawner.SpawnRoomEnemies()` — `set.Count == 0 \|\| set == null` dereferences before the null test | EnemySpawner.cs:62 |
| BUG-046 | MEDIUM | `EntityWeaponMelee.Attack()` uses allocating `Physics2D.OverlapCircle` | EntityWeaponMelee.cs:29 |
| — | MEDIUM | `RoomModel.SetListCandidate()` `retry > 4` fallback skips the weight filter, breaking ADR-0003's "overspend is structurally impossible" guarantee. `overflowPercent` is declared but never read (a literal `0.1f` is used) | RoomModel.cs:55 |
| 12 | MEDIUM | `LevelManager` singleton (`public static Instance`, a bare field); `RoomGeneraterController.Setting()` reaches through it | LevelManager.cs:10 |
| 13 | MEDIUM | Start-room teleport commented out; `RoomGeneraterController.OnDoneLoadRoomGrid()` has no caller | RoomGridController.cs:82 |
| 14 | MEDIUM | `MazeController.Awake()` missing `return` after `Destroy(gameObject)` | MazeController.cs:17 |
| 15 | BUILD | Room JSON via `File.ReadAllText(Application.dataPath…)` — Editor-only, breaks Player builds | RoomGeneraterController.cs:63 |
| 16 | MEDIUM | `RoomType` never read at runtime; start/end rooms picked by list position | RoomGeneraterController.cs:47 |
| 17 | LOW | Dead code: `DoorController.OpenDoor()`/`CheckCanBeOpened()`, `RoomCell.UpdateStatusDoor()` — no-ops | DoorController.cs:29 |
| BUG-052 | DOC | `Character/Base/`, `Pathfinding/`, `Poolable/` have no ADR. CLAUDE.md's Repository Layout now lists them; the ADR decision is still owed | — |

**Closed since the last update:** Bugs #4, #5, #7, #8, #9, NEW-3 (fixed on `sprint-10`) and NEW-4
(fixed 2026-08-21 — `Stat.modifiers` no longer serialized). Plus #10, #11 previously.

---

## EventID enum (current — 19 values)

`ON_PLAYER_ON_DOOR`, `ON_PLAYER_DEATH`, `ON_REALOAD_GAME`, `ON_LOAD_MAZE_DONE`, `ON_LOAD_MAP`,
`ON_CLEAR_ENEMY`, `ON_GET_SPAWN_POSITIONS`, `ON_DONE_SPAWN_ENEMY`, `ON_SPAWN_EXTRA_ENEMY`,
`ON_TEST`, `ON_ENEMY_DEATH`, `ON_ROOM_CLEAR`, `ON_OPEN_STATS_PLAYER_UI`,
`ON_CLOSE_STATS_PLAYER_UI`, `ON_INCREASE_STATS_BY_UI`, `ON_DECREASE_STATS_BY_UI`,
`ON_CHANGE_STATS_BY_UI_RUN_TIME`, `ON_UPDATE_STATS_BY_UI`

Still missing: **`ON_PLAYER_TAKE_DAMAGE`** — `.claude/rules/ui-code.md` instructs the health bar to
bind to it, but the value has never existed.

`ON_ROOM_CLEAR` exists in the enum but has no producer yet.
`ON_CLEAR_ENEMY` is now produced by `RoomCell.OnEnemyDeath()` at zero alive, not only by an Editor button.

Register/UnRegister pairing is clean: all six subscriber files balance exactly
(`StatsUIController` 3/3, `StatSlot` 1/1, `RoomGridController` 6/6, `MapGridController` 2/2,
`EnemySpawner` 2/2, `AnimationPlayerController` 5/5).

---

## Stubs / unimplemented

- `UIManager` — empty stub (TD-017)
- `PlayerUserItemState` — extends `MonoBehaviour` instead of `PlayerState` (TD-001)
- `ICharacter` — empty interface, zero implementers, zero references
- `ICoreComponent` — memberless marker; `CoreBase.Setup()` blind-casts to `ICoreComponent<ICore>`
- `SwordAndShield` — empty subclass of `MeleeWeapon`
- `DualAbility` — all code commented out
- `AnimationName.cs` — an empty `ScriptableObject` stub; the real constants live in `GameConstants.AnimationName` (TD-016 describes this inaccurately)
- `EnemySpawner.Spawn()` — dead empty method
- `RoomModel.overflowPercent` — serialized, never read
- `PlayerData.Reborn()` — implemented, no caller
- `EnemySO` — not consumed by `Entity`, which reads `EntityData` (TD-030)
- `TalentManagger` — stats hardcoded in `Awake()`, not SO-driven (TD-018)
- `tests/EditMode/`, `tests/PlayMode/`, `tests/playtest/` — only `.gitkeep`; zero tests exist (TD-014)
- `prototypes/` — does not exist, so `.claude/rules/prototype-code.md`'s isolation rule is unenforceable as written

---

## Undocumented systems (code exists, no design/architecture doc)

| System | Location | Gap |
|--------|----------|-----|
| Pathfinding (A*) | `Assets/Script/Pathfinding/` (12 files) | No GDD, no ADR, missing from `systems-index.md` |
| Shared hub layer | `Assets/Script/Character/Base/` (10 files) | No ADR; changes the hub contract `engine-code.md` declares closed to `Core`/`EntityCore` |
| Object pooling | `Assets/Script/Poolable/` | `systems-index.md` still says "Not Started" |
| UI Toolkit runtime menus | `Assets/Script/UI/UIController.cs` | No GDD, no ADR; conflicts with `VERSION.md` guidance |
| Stats UI | `Assets/Script/UI/StatsUIController.cs`, `StatSlot.cs` | No GDD; six `ON_*_STATS_*_UI` events undocumented |

---

## Demo fix priority

1. **Enemy targeting** (NEW-1) — re-enable `EntityInput.GetTargetInRange()` with a null guard and
   NonAlloc queries. Nothing in enemy AI works until this lands.
2. **Enemy damage/death chain** (BUG-042 + BUG-053, story S10-01) — pick one `INegativeReceiver`
   implementer for the enemy, route health through `EntityStatsSO`, delete the duplicate.
3. **Player death** (Bug #6 + BUG-044, story S10-08) — write `PlayerData.currentHealth`, construct
   `PlayerDeathState`, restore its body, add a `GameManager` that calls `Reborn()` and reloads.
4. **StatSystem correctness** (NEW-2) — the recursive `ModifiersAmor` property, a silent failure that
   kills the Editor on first access. The `||`/`&&` guard in `RecalculateDerived()` was fixed on
   `sprint-10`; the `Stat.modifiers` serialization leak was fixed on 2026-08-21 (C1).
5. **Start-room teleport** (Bug #13).
6. **Enemy spawn hardening** — BUG-033 null-guard, the ADR-0003 budget-invariant fallback, and the
   two-parallel-drivers question (BUG-ES-2).
7. **Build-safe JSON loading** (Bug #15) — required before the first standalone build.
