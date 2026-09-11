# Epics Index

Last Updated: **2026-09-11** (full documentation/code re-synchronisation)
Engine: Unity 2022.3.62f3 LTS

| Epic | Layer | System | GDD | Stories | Status |
|------|-------|--------|-----|---------|--------|
| Enemy Spawn & Per-Room Management | Feature | Enemy Spawn & Per-Room Management | design/gdd/enemy-spawn-system.md | Not yet created | Ready (untraced reqs) — partially built ahead of stories |

> **Audit note (2026-08-20).** No story files were ever created for this epic, but a
> substantial part of it shipped anyway through un-storied commits: the `Tile_Spawn_Enemy`
> marker parser, `EnemySpawner` (event-driven, pooled), `RoomCell`'s alive-count and
> `ON_CLEAR_ENEMY` emission, and the candidate-pool + `RarityTier` rewrite of
> `RoomModel.GetSpawnSet()`. Remaining gaps: BUG-033 null-guard order, the ADR-0003 budget
> invariant broken by the `retry > 4` fallback, two parallel spawn drivers (BUG-ES-2), and
> the fact that `EnemyManager` never took the lifecycle role ADR-0002 assigns it. The epic
> is also blocked end-to-end by TD-036 — enemies currently cannot die.
>
> **Update 2026-09-11: the blocker is gone.** TD-036 is CLOSED — BUG-042, BUG-046, BUG-053 and
> NEW-2 are all fixed, and enemies now take damage, apply `StatType.Defense`, update a health bar
> and die. BUG-033's null-guard is also fixed. What remains open on this epic is unchanged: the
> ADR-0003 budget invariant broken by the `retry > 4` fallback, `overflowPercent` serialized but
> never read, two parallel spawn drivers (BUG-ES-2), and `EnemyManager` never taking the lifecycle
> role ADR-0002 originally assigned it.

## Systems with code but no epic

| System | Location | Note |
|--------|----------|------|
| Pathfinding (A*) | `Assets/Script/System/Pathfinding/` | 12 files, shipped, no GDD / no ADR / absent from `systems-index.md` (BUG-052) |
| Shared hub layer | `Assets/Script/Character/Base/` | 10 files, underlies both Player and Entity, no ADR (BUG-052) |
| Object pooling | `Assets/Script/System/PoolableService/` | Shipped and consumed by three systems; `systems-index.md` still says "Not Started" |
| UI Toolkit menus + Stats UI | `Assets/Script/UI/` | Shipped, no GDD. (`VERSION.md`'s advice against runtime UI Toolkit was corrected 2026-09-11 — it contradicted shipped code) |
| **Dependency Injection** | `Assets/Script/System/LifetimeScope/` | Shipped 2026-08-22 (`aa4e620`). ADR-0004 written retroactively 2026-09-11; still no epic, no stories |
| **Abilities v2** | `Assets/Script/System/Abilities/` | 17 files promoted out of `prototypes/` 2026-09-09. Drives the player. No GDD, no ADR, no epic |
| **Item / Depot / drop tables** | `Assets/Script/System/Item/` | Shipped 2026-09-04 to 09-07. No GDD, no ADR, no epic |

> **Audit note (2026-09-11).** The "code but no epic" list grew from four entries to seven in three
> weeks. All three new entries shipped through un-storied commits — the same pattern the
> 2026-08-20 note recorded for the enemy-spawn epic.
