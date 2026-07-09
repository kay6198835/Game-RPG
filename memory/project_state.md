# Project State

cập nhật 2026-07-02

Snapshot of actual code state, synced by /doc-sync. Source of truth for "what is
really implemented" — CLAUDE.md carries the same facts in long form.

---

## Systems completed since last doc update (2026-06-04)

| System | Commit(s) | Notes |
|--------|-----------|-------|
| Combo attack | a654831, 2eb0765, 6452127 (06-19 → 06-22) | Multi-stage `AttackState` list on `WeaponMeleeStats`; `WeaponMelee.SetAnimation()` cycles states; `DurationNextAttack()` paces combo. Damage output still blocked by Bug #4 |
| StatSystem | 9d1ecbf, 0bf02f0, e69a485 (06-30 → 07-01) | `Assets/Script/StatSystem/` — StatType (primary STR/DEX/INT/VIT/LUK, derived 100+), Stat/StatModifier, DerivedStatFormula, StatsSO ("Game/Stats Profile"). NOT yet consumed by any gameplay code. `ToolExcel/stat_system.xlsx` = formula emulator |
| Minimap | (pre-existing, verified 07-02) | `MapGridController.Move/OnLoadMap` implemented with DOTween — Bug #11 CLOSED |
| RoomCell.GetDoor fix | 05b76cc (07-01) | No longer instantiates DoorController via `new`; returns null + warning |

## Open bugs (verified against source 2026-07-02)

| # | Sev | Description | Location |
|---|-----|-------------|----------|
| 4 | LOGIC | `WeaponMelee.Attack()` foreach body empty — no damage | WeaponMelee.cs:29 |
| 5 | LOGIC | `EntityMoveState` derefs `Input.Target` (line 30) before null check (line 34) | EntityMoveState.cs:30 |
| 6 | LOGIC | Player damage broken — `Core.TakeDamage()` removed; `NegativeReciver.TakeDamage()` throws NotImplementedException | NegativeReciver.cs:8 |
| 7 | LOGIC | `EntityDeathState` extends MonoBehaviour, not EntityState | EntityDeathState.cs |
| 8 | LOGIC | `EntityBasicState` Health<=0 block empty — no death transition | EntityBasicState.cs:20 |
| 9 | LOGIC | `AnimationPlayerController` registers StartAnimation twice (OnEnable:21, OnDisable:29) — EndAnimation never fires | AnimationPlayerController.cs:21 |
| 12 | ARCH | `LevelManager` singleton violates no-new-singletons rule | LevelManager.cs |
| 13 | LOGIC | Start-room teleport commented out; `RoomGeneraterController.OnDoneLoadRoomGrid()` never called | RoomGridController.cs:56 |
| 14 | LOGIC | `MazeController.Awake` missing `return` after `Destroy(gameObject)` | MazeController.cs:17 |
| 15 | BUILD | Room JSON via `File.ReadAllText(Application.dataPath...)` — Editor-only, breaks builds | RoomGeneraterController.cs:57 |
| 16 | LOGIC | `RoomType` never read at runtime; start/end rooms picked by list position | RoomGeneraterController.cs:43 |
| 17 | ARCH | Dead code: `DoorController.OpenDoor()/CheckCanBeOpened()`, `RoomCell.UpdateStatusDoor()` — no-ops; real gating is `OpenDoors()/CloseDoor()` | DoorController.cs:29 |

## EventID enum (current)

`ON_PLAYER_ON_DOOR`, `ON_LOAD_MAZE_DONE`, `ON_LOAD_MAP`, `ON_CLEAR_ENEMY`, `ON_TEST`

Missing (needed for demo): `ON_ENEMY_DEATH`, `ON_PLAYER_DEATH`, `ON_PLAYER_TAKE_DAMAGE`, `ON_ROOM_CLEAR`

Note: `ON_CLEAR_ENEMY` has a consumer (RoomGridController → open doors) but its only
producer is the editor debug button in `LevelManagerEditor` — no gameplay emitter exists.

## Stubs / unimplemented

- `NegativeReciver.TakeDamage` — throws NotImplementedException (player damage endpoint)
- `EntityDeathState` — empty MonoBehaviour template
- `UIManager` — empty stub
- `Handler/EventHandler/` — empty folder
- `GameConstants.TileName.SPAWN` ("Tile_Spawn") — defined, never referenced, absent from all room JSONs
- `NewEnemy` / `NewEnemyState` / `NewEnemyStateMachine` — empty/stub
- StatSystem — implemented but not wired into Player/Entity

## Demo fix priority

1. Player damage + death chain — Bug #6 (NegativeReciver) + `ON_PLAYER_DEATH`
2. Enemy death chain — Bugs #5, #7, #8 (prerequisite for any room-clear loop)
3. `WeaponMelee.Attack()` damage — Bug #4
4. Start-room teleport — Bug #13
5. Enemy spawn system — GDD approved (`enemy-spawn-system.md`) + ADR-0002 (EnemyManager
   singleton). Prototype built: `RoomModel`/`MapModel`/`EnemyModal` (`Database-SO/Modal/`) +
   `RoomModel.GetSpawnSet()`, driven by `LevelManager.SpawnRoomEnemies()` Editor button.
   PLANNED: `EnemyManager` lifecycle + `Tile_Spawn` markers + `ON_ENEMY_DEATH`/`ON_ROOM_CLEAR`.
   (Superseded the earlier `EncounterSO`/`RoomEnemySpawner` sketch.)
6. Build-safe JSON loading — Bug #15 (required before first standalone build)
