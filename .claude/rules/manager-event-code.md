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
- `MazeController` is the only permitted singleton
- Managers that need cross-scene access use ScriptableObject events or `DontDestroyOnLoad` with a scene manager pattern — not `Instance` singletons
- `UIManager` must be wired via Inspector reference, not found at runtime

## GameConstants
- All direction vectors and input axis names live in `GameConstants.cs`
- New project-wide constants go here — not in individual MonoBehaviours
- No magic strings for Input axis names — always use `GameConstants.*`

## AnimationEventManager
- Animation events from Unity Animator call methods on `AnimationEventManager`
- `AnimationEventManager` fires through `EventManager` — it does NOT directly call state methods
- Never add gameplay logic directly into animation event callback methods

## UIManager Completion
- `UIManager` is currently an empty stub — implement via EventManager subscriptions
- Health bar binds to `EventID.ON_PLAYER_TAKE_DAMAGE` (or equivalent) — never polls `PlayerData` in Update
