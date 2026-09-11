# ADR-0004: VContainer is the project's dependency-injection container

## Status
Accepted (retroactively documented)

> **⚠️ This ADR was written on 2026-09-11, 20 days after the decision shipped.** VContainer was
> added to `Packages/manifest.json` and `GameLifetimeScope` was authored in commit `aa4e620`
> (2026-08-22). No ADR, rule update, or `VERSION.md` entry accompanied it, so for three sprints
> `.claude/rules/engine-code.md` actively instructed contributors to use a resolution mechanism
> the codebase had already moved past. This document records the decision as implemented; it does
> not re-litigate it.

## Date
2026-08-22 (decision) · 2026-09-11 (recorded)

## Engine Compatibility

| Field | Value |
|-------|-------|
| Engine | Unity 2022.3.62f3 LTS |
| Domain | Core / Architecture / Composition |
| Knowledge Risk | LOW (pinned engine version is within training data) |
| References Consulted | `docs/engine-reference/unity/VERSION.md`; `.claude/rules/engine-code.md`; `.claude/rules/gameplay-code.md`; `Packages/manifest.json` |
| Post-Cutoff APIs Used | None |
| Verification Required | None for the engine. See **Risks** for the third-party supply-chain concern |

## ADR Dependencies

- **Amends ADR-0002** (EnemyManager singleton exception) — DI is now the preferred alternative to
  new singletons, which narrows the circumstances under which a future singleton exception could
  be argued for.
- **Interacts with ADR-0001** (StatSystem data structure) — `StatHandler` exposes the stat profile
  as `IPlayerStatService`, so consumers no longer hold a direct `BaseStatsSO` reference.

---

## Context

The project's dependency story before 2026-08-22 had three mechanisms and no rule about which to
use when:

1. `Core.GetCoreComponent<T>()` — a per-character pull-based component hub (`CoreBase.Setup()`).
   Works well *within* one character, but cannot reach across systems.
2. `[SerializeField]` Inspector references — fine for scene-authored data, but brittle across
   prefab boundaries and impossible for runtime-spawned objects.
3. Singletons — explicitly forbidden by `.claude/rules/engine-code.md` except for
   `MazeController` and `EnemyManager` (ADR-0002). In practice a third one, `LevelManager`, had
   already appeared as an unratified violation (TD-023), precisely because nothing else could
   serve a cross-system lookup.

The pressure point was the Sprint 12 work: `ObjectPoolManager` needed to be reachable from
`RangeWeapon`, `EnemySpawner`, `ItemSpawner` and the new ability effects; `StatHandler` needed to
be reachable from the stats UI and the ability cost checks; and `PlayerManager` needed to be
reachable from item pickup and camera follow. Every one of those was either going to become a
singleton or a chain of Inspector references through prefabs that are instantiated at runtime.

`FollowPlayer.cs` shows where that road ends: `GameObject.Find("PlayerTest(Clone)")` plus an
`Invoke(0.01f)` timing hack (TD-029).

## Decision

**Adopt VContainer 1.19.0 as the project's dependency-injection container, with a single
scene-level composition root, and expose every injected dependency behind an interface.**

Concretely:

1. `Assets/Script/System/LifetimeScope/GameLifetimeScope.cs` is the **only** composition root.
   It is a `LifetimeScope` MonoBehaviour and must be present in any playable scene.
2. Cross-system services are registered with `RegisterComponentInHierarchy<T>().As<IService>()`:

   | Concrete | Interface | Consumers |
   |---|---|---|
   | `ObjectPoolManager` | `IObjecPoolService` | `RangeWeapon`, `EnemySpawner`, `ItemSpawner`, ability effects |
   | `PlayerManager` | `IPlayerService` | item pickup, position queries |
   | `StatHandler` | `IPlayerStatService` | stats UI, ability cost checks |

   Six further components are registered as concrete types because they are consumed by the
   container rather than by each other: `Player`, `EnemySpawner`, `StatsUIController`,
   `ItemSpawner`, `RoomGeneraterController`, `AbilityHolder`.
3. MonoBehaviours receive dependencies through **method injection**:
   `[Inject] public void Construct(IObjecPoolService pool)`. Constructor injection is unavailable
   to MonoBehaviours.
4. **DI does not replace the Core hub.** Sibling components on the same character are still
   resolved with `Core.GetCoreComponent<T>(out …)`. The two mechanisms have disjoint scopes:
   DI crosses systems, the hub stays within one character.
5. Runtime-spawned objects are **not** registered. `Pool.Spawn()` injects into each new instance.

### Rules that follow

- New services are registered in `GameLifetimeScope.Configure()` and **must** be exposed behind an
  interface in `System/LifetimeScope/Interface/`.
- Never inject a concrete MonoBehaviour type.
- Never resolve the container manually in gameplay code (`IObjectResolver.Resolve<T>()`) — that is
  service location wearing a DI costume, and reintroduces the hidden-dependency problem this
  decision exists to remove.
- `GameLifetimeScope` holds no gameplay state and no gameplay logic. Adding either makes it a hub,
  which `engine-code.md` forbids.

## Alternatives Considered

| Option | Why not |
|---|---|
| **Keep going with singletons** | Directly contradicts `engine-code.md`. `LevelManager` (TD-023) already demonstrated the failure mode: a bare mutable `public static` field any script can null, mixing editor authoring, runtime loading and spawning in one class. |
| **`ScriptableObject` as a runtime service locator** | A popular Unity pattern and genuinely tempting here, since the project is already SO-heavy. Rejected because SO assets persist edits across Play Mode sessions — the project had *already* been burned by exactly that (`Stat.modifiers` leaking runtime buffs into committed `.asset` files, TD-038, now regressed as BUG-063). Putting runtime service references into SOs invites the same class of bug. |
| **Zenject / Extenject** | Larger, slower to compile, heavier reflection cost, and its maintenance cadence has been irregular. VContainer is faster at resolution, allocation-light, and small enough to read end to end. |
| **Pure constructor injection with a hand-rolled root** | No framework dependency, which is a real advantage. Rejected because MonoBehaviours cannot take constructor arguments, so it would mean hand-writing the same `Construct()` plumbing VContainer already provides, plus its lifecycle handling. |
| **`FindObjectOfType` at `Start()`** | Already a Forbidden Pattern in `gameplay-code.md`, and `FollowPlayer.cs` (TD-029) is the standing example of why. |

## Consequences

### Positive

- The forbidden-pattern list in `gameplay-code.md` becomes enforceable — there is finally a
  sanctioned answer for "how do I reach another system".
- Cross-system dependencies are declared in one readable file instead of being discovered at
  runtime.
- Interfaces make the services substitutable, which is a precondition for the EditMode tests that
  TD-014 says the project still has zero of.
- Removes the pressure that produced `LevelManager`'s unratified singleton.

### Negative / costs

- **Fail-fast at `Awake()`, not at first use.** `RegisterComponentInHierarchy<T>()` finds but never
  spawns. A component missing from the scene throws before any gameplay runs. This is the correct
  trade, but it means scene setup is now load-bearing and a mis-authored scene fails loudly and
  early. The source comments in `GameLifetimeScope` flag this for `Player` specifically.
- **Two registrations are commented out** (`StatsScreenUIController`, `StatPointAllocator`)
  because they are not reliably in the hierarchy. That is a known rough edge, not a finished design.
- **An incomplete migration is worse than none.** `RangeWeapon`'s DI wiring is still open as
  BUG-064 sub-item 7 — it is the last component reaching for a service the old way.
- A third-party dependency now sits on the project's critical path.

### Risks

- **Supply chain.** VContainer is pinned by **git URL** to tag `1.19.0`
  (`jp.hadashikick.vcontainer`), not by the Unity package registry. If the upstream repository
  moves or disappears, package resolution fails for the entire project. Mitigation if this ever
  matters: vendor the package under `Packages/` as an embedded package.
- **Undocumented adoption is the failure this ADR exists to close.** The gap between `aa4e620`
  and this document is the direct cause of `engine-code.md` giving wrong instructions for three
  sprints. Future framework-level decisions get an ADR in the same commit.

## Verification

Checked directly against source at HEAD `6d6a8e4` on 2026-09-11:

- `Packages/manifest.json` — `"jp.hadashikick.vcontainer": "…#1.19.0"` ✅
- `Assets/Script/System/LifetimeScope/GameLifetimeScope.cs` — `: LifetimeScope`, 9 registrations,
  3 behind interfaces, 2 commented out with a reason ✅
- `Assets/Script/System/LifetimeScope/Interface/` — `IObjecPoolService`, `IPlayerService`,
  `IPlayerStatService` ✅ (note the intentional `Objec` typo, preserved per coding conventions)
- `[Inject] public void Construct(...)` present on `AbilityHolder`, `ItemSpawner`,
  `ObjectPoolManager` ✅
- `StatHandler : CoreComponent<Core>, IPlayerStatService`;
  `PlayerManager : MonoBehaviour, IPlayerService` ✅

## Open Questions

1. **Is `GameLifetimeScope` the only scope the project needs?** A per-room child scope could own
   room-scoped services (the spawner, the pathfinding grid) and dispose them on room transition.
   Not needed today; revisit if room transitions start leaking state.
2. **Should `EnemyManager` be re-expressed as an injected `IPathfindingService`?** ADR-0002's
   singleton exception was granted before DI existed. The exception is no longer *necessary* —
   but `EntityMovement.Start()` reads `EnemyManager.Instance.Grid` on runtime-spawned enemies,
   which is precisely the case DI handles worst. Deliberately deferred.
3. **What happens to the two commented-out registrations?** Either guarantee those components are
   in the scene, or move them to spawn-time injection. Currently neither.
