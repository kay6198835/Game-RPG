---
description: Standards for all gameplay logic in Assets/Script/Character, Assets/Script/Weapons, Assets/Script/Skill_Ability
globs: ["Assets/Script/Character/**/*.cs", "Assets/Script/Weapons/**/*.cs", "Assets/Script/Skill_Ability/**/*.cs"]
---

# Gameplay Code Standards

## Data-Driven Values
- ALL numeric gameplay values (damage, health, speed, cooldown, range) MUST live in ScriptableObjects
- Never hardcode magic numbers in MonoBehaviour or state classes
- Use `[SerializeField] private float` for inspector-only values, ScriptableObject fields for shared data

## State Machine Discipline
- New behaviour = new `PlayerState` or `EntityState` subclass — never inline `if/else` chains in `Update()`
- `LogicUpdate()` is for state transitions and input polling only — no physics, no allocation
- `PhysicsUpdate()` is for `rb.velocity` and force application only

## No Allocation in Hot Paths
- Never call `new`, LINQ, or string concatenation in `Update()`, `LogicUpdate()`, or `PhysicsUpdate()`
- Use `Physics2D.OverlapCircleNonAlloc` — never `OverlapCircle` in per-frame code
- Cache `GetComponent<>()` results in `Awake()` — never call in `Update()`

## Damage and Health

> Signature corrected 2026-09-11. The parameter became `float` in the Sprint 12 stat refactor
> (`f3f5f08`); this rule said `int` for three sprints, so code written to it would not compile.

- All damage flows through `INegativeReceiver.TakeDamage(float amountDamage, Vector2 attackPosition)`
- No MonoBehaviour may directly mutate another entity's health field
- Health changes must go through the Core/EntityCore component hub
- **Max vs current values are separate components.** Max comes from the stat profile
  (`StatHandler` / `EntityStatsHandler`, backed by `BaseStatsSO`); current comes from the vitals
  component (`VitalStatsComponent` / `EntityVitalStats`). Never read a max value where you mean
  a current one
- Mitigation belongs in the receiver, not the attacker — see
  `EntityNegativeReciver.DamageCalculate()`, which subtracts `StatType.Defense` and clamps at 0

## Dependency Resolution

> Added 2026-09-11. VContainer was adopted in `aa4e620` (2026-08-22) and went three sprints
> undocumented. See ADR-0004.

Three mechanisms, and they are not interchangeable:

| Need | Use |
|---|---|
| A sibling core component on the same character | `Core.GetCoreComponent<T>(out var c)` |
| A cross-system service (pooling, player lookup, player stats) | VContainer `[Inject] public void Construct(IService s)` |
| Scene-authored data (SO assets, tilemaps, prefab refs) | `[SerializeField] private` Inspector ref |

- New services are registered in `GameLifetimeScope.Configure()` and exposed **behind an
  interface** (`IObjecPoolService`, `IPlayerService`, `IPlayerStatService`)
- `RegisterComponentInHierarchy<T>()` finds, never spawns — a component missing from the scene
  throws at `Awake()`. Do not register anything that is instantiated at runtime; pooled objects
  are injected by `Pool.Spawn()` instead
- Do not resolve the container manually (`IObjectResolver.Resolve<T>()` in gameplay code) — that
  is service-location, not injection

## Forbidden Patterns
- `GameObject.Find()`, `FindObjectOfType()`, `SendMessage()` — use Inspector refs, DI, or EventManager
- `public` fields on MonoBehaviours — use `[SerializeField] private` + properties
- Coroutines that can leak (no `StopCoroutine` pairing) — prefer state machine transitions

## Vector2 / Vector3 Convention
- Gameplay math (directions, offsets, velocities, distances) uses `Vector2`/`Vector2Int` — this is a 2D game
- `Vector3` is allowed ONLY at the Unity API boundary: `transform.position` assignment, `localScale`, Tilemap calls, camera follow (z offset), `Quaternion.AngleAxis` rotation axis
- Fields, properties, and method parameters that are logically 2D must be typed `Vector2`, even when fed from `transform.position` (implicit truncation is the intended semantic)
- Never use `Vector3.one` as a 2D offset — it silently writes z; use `Vector2.one` and cast at the assignment site
- `Vector3Int` is reserved for Tilemap cell coordinates (`LevelData.poses`, `GetTile`/`SetTile`)
- Use `transform.Position2D()` (VectorExtensions) instead of `(Vector2)transform.position` in new code; `v.WithZ(z)` for the rare camera/layering case
