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
- All damage flows through `INegativeReciver.TakeDamage(int amount, Vector2 attackPosition)`
- No MonoBehaviour may directly mutate another entity's health field
- Health changes must go through the Core/EntityCore component hub

## Forbidden Patterns
- `GameObject.Find()`, `FindObjectOfType()`, `SendMessage()` — use Inspector refs or EventManager
- `public` fields on MonoBehaviours — use `[SerializeField] private` + properties
- Coroutines that can leak (no `StopCoroutine` pairing) — prefer state machine transitions
