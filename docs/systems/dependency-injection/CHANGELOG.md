# Dependency Injection — Changelog

Newest first. Entries before 2026-10-05 reconstructed from `CLAUDE.md` history, ADR-0004 and `git log`.

## 2026-10-02 — LevelManager and RoomGridController registered
- **Commit:** `0bc36406`
- **Changed:** `GameLifetimeScope`
- **From → To:** `LevelManager.Instance` singleton → `RegisterComponentInHierarchy<LevelManager>()`; `RoomGridController` registered so it can receive `PlayerManager`
- **Why:** remove the last unratified singleton (TD-023); start-room teleport needs the player service.
- **Bugs:** closes Bug #12 / TD-023.

## 2026-09-28 — Player spawned by PlayerManager
- **Commit:** `5b035b73`
- **Changed:** `GameLifetimeScope`, `PlayerManager`, `WeaponHolderBase`
- **From → To:**
  - `RegisterComponentInHierarchy<Player>()`, `<StatHandler>().As<IPlayerStatService>()`, `<AbilityHolder>()` → removed; `IPlayerStatService` = factory over `PlayerManager.StatService`
  - `PlayerManager.Construct(Player)` + manual camera parenting in `Start()` → `Construct(IObjectResolver)`, lazy `Player` property spawning via `resolver.Instantiate`
  - Weapons never injected → injected on equip
- **Why:** player prefab no longer has to be hand-placed in every scene; weapons need the pool service.
- **Bugs:** closes BUG-064 sub-item 7.

## 2026-09-23 — IStatService split
- **Commit:** `2aa225e4`
- **Changed:** `IPlayerStatService`
- **From → To:** standalone interface → `IPlayerStatService : IStatService`
- **Why:** ADR-0005.
- **Bugs:** none.

## 2026-08-22 — VContainer adopted
- **Commit:** `aa4e620c`
- **Changed:** `GameLifetimeScope` added with 9 registrations
- **From → To:** `GetComponent` / Inspector refs / singletons → VContainer method injection
- **Why:** cross-system services without singletons (ADR-0004, written 2026-09-11).
- **Bugs:** none.
