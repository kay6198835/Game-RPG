# Epics Index

Last Updated: 2026-08-20 (documentation audit)
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

## Systems with code but no epic

| System | Location | Note |
|--------|----------|------|
| Pathfinding (A*) | `Assets/Script/Pathfinding/` | 12 files, shipped, no GDD / no ADR / absent from `systems-index.md` (BUG-052) |
| Shared hub layer | `Assets/Script/Character/Base/` | 10 files, underlies both Player and Entity, no ADR (BUG-052) |
| Object pooling | `Assets/Script/Poolable/` | Shipped and consumed by three systems; `systems-index.md` still says "Not Started" |
| UI Toolkit menus + Stats UI | `Assets/Script/UI/` | Shipped, no GDD, and `VERSION.md` currently advises against runtime UI Toolkit |
