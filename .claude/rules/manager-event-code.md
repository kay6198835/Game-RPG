---
description: Manager and event bus code standards — EventManager, AnimationEventManager, GameConstants, UIManager
globs: ["Assets/Script/Manager/**/*.cs", "Assets/Script/GameConstants.cs"]
---

# Manager and Event Bus Standards

## EventManager (Static Bus)
- Use `EventManager.Resgister(EventID, callback)` and `EventManager.UnResgister` — typo is intentional, match it exactly
- Every `Register` call in `OnEnable()` MUST have a matching `UnRegister` in `OnDisable()`
- Pass typed data as the second argument — never pass raw `object` without casting
- New events: add to `EventID` enum only — never add `static Action` fields to individual classes

## No New Singletons
- `MazeController` and `EnemyManager` (ratified exception — ADR-0002) are the only permitted singletons
- Managers that need cross-scene access use ScriptableObject events or `DontDestroyOnLoad` with a scene manager pattern — not `Instance` singletons
- `UIManager` must be wired via Inspector reference, not found at runtime

## GameConstants
- All direction vectors and input axis names live in `GameConstants.cs`
- New project-wide constants go here — not in individual MonoBehaviours
- No magic strings for Input axis names — always use `GameConstants.*`

## Animation events

> Corrected 2026-08-21. This section previously said "Animation events from Unity Animator call
> methods on `AnimationEventManager`" and that it "fires through `EventManager`". Both are wrong:
> `AnimationEventManager` is its own static dictionary, unrelated to `EventManager`, and it is
> **dead** — `AnimationEventManager.Emit()` has zero callers anywhere in the repo.

- The real mechanism is Unity Animation Events calling methods **by name** on `Player` / `Entity`
  (`AnimationStart`, `AnimationTrigger`, `AnimationOnAction`, `AnimationOffAction`,
  `AnimtionFinishTrigger`, `AnimationEnd`)
- Each of those does exactly one thing: `CurrentState.SetAnimationStatus(StatusAnimation.X)`
- Never add gameplay logic directly into an animation event method — branch on `Status` inside the
  state's `LogicUpdate()` instead
- `Status` is durable state, not a one-frame pulse: the state that acts on a value is responsible
  for writing a new one to consume it
- `AnimationEventManager` / `AnimationEventId` / `AnimationPlayerController` are unreachable today.
  Do not build on them without first resolving Open Question #1 in `design/gdd/animation-system.md`

## UIManager Completion
- `UIManager` is currently an empty stub — implement via EventManager subscriptions
- Health bar binds to `EventID.ON_PLAYER_TAKE_DAMAGE` (or equivalent) — never polls `PlayerData` in Update
