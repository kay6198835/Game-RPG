---
description: Core engine/framework code standards — Core.cs, CoreComponent, EntityCore, state machine infrastructure
globs: ["Assets/Script/Character/Player/Core/**/*.cs", "Assets/Script/Character/Entity/Core/**/*.cs", "Assets/Script/Character/Player/CoreComponent/**/*.cs", "Assets/Script/Character/Entity/CoreComponent/**/*.cs"]
---

# Engine / Core Code Standards

## Stability Contract
- Public APIs on Core, EntityCore, and component hubs are STABLE — no breaking changes without a design review
- All public methods on Core components must have a one-line doc comment explaining the contract
- Core components may NOT depend on specific state implementations — only on interfaces

## Dependency Rules

> Amended 2026-09-11. VContainer (`aa4e620`, 2026-08-22) introduced a third resolution
> mechanism that this section did not acknowledge for three sprints. See ADR-0004.

- Core.cs and EntityCore.cs are the ONLY permitted **component hubs** — no new singleton hubs
- `MazeController` and `EnemyManager` (ratified exception — ADR-0002) are the only permitted global singletons
- Components discover **siblings** via `Core.GetCoreComponent<T>()`, never direct field references
- Components receive **cross-system services** via VContainer constructor injection
  (`[Inject] public void Construct(IService s)`), registered in `GameLifetimeScope.Configure()`
- `GameLifetimeScope` is a composition root, not a hub: it wires object graphs and holds no
  gameplay state and no gameplay logic. Adding logic to it is the same violation as adding a hub
- Every injected dependency is exposed **behind an interface** in
  `System/LifetimeScope/Interface/` — never inject a concrete MonoBehaviour type

## Interface-First
- New capabilities must be expressed as interfaces first (`INegativeReceiver`, `IEffectable`, `IInteractable`)
- MonoBehaviours implement interfaces; callers depend only on the interface, not the concrete type
- New interfaces go in `Assets/Script/Interface/`

## Thread Safety
- All Unity API calls must happen on the main thread — no background thread access to transforms, physics, or components
- `Physics2D` calls are main-thread-only; use NonAlloc variants in hot paths

## Zero-Alloc Hot Paths
- `OverlapCircleNonAlloc` with pre-allocated collider arrays — cache the array as a field
- Pre-allocate `RaycastHit2D[]` buffers in `Awake()`, reuse every frame
