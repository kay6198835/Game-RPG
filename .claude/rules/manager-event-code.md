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
  `AnimationFinishTrigger`, `AnimationEnd`)
- Each of those does exactly one thing: `CurrentState.SetAnimationStatus(StatusAnimation.X)`
- Never add gameplay logic directly into an animation event method — branch on `Status` inside the
  state's `LogicUpdate()` instead
- `Status` is durable state, not a one-frame pulse: the state that acts on a value is responsible
  for writing a new one to consume it
- `AnimationEventManager` / `AnimationEventId` / `AnimationPlayerController` are unreachable today.
  Do not build on them without first resolving Open Question #1 in `design/gdd/animation-system.md`

## UIManager Completion
- `UIManager` is currently an empty stub — implement via EventManager subscriptions
- ⚠️ **`ON_PLAYER_TAKE_DAMAGE` does not exist and never has** (re-checked 2026-09-11 against the
  23-value enum). Whoever builds the player health bar must add it to `EventID` first — this rule
  has instructed binding to a non-existent value since it was written
- The **enemy** health bar is already solved and is the pattern to copy: `EntityUIController` is
  pushed a new ratio from `EntityNegativeReciver.TakeDamage()` — never polled

## EventID Count (added 2026-09-11)

`EventID` holds **23** values. Three were added between 2026-08-22 and 2026-09-07 without any doc
update: `ON_RESET_STATS_UI_SESSION` (the `StatPointAllocator` session reset) and `ON_DROP_ITEM` /
`ON_COLLECT_ITEM` (the Item system). `ON_ROOM_CLEAR` exists but still has no producer.
Nothing has ever been removed from the enum — see the count history in `CLAUDE.md` before
assuming a value was deleted.
