---
description: Enemy AI code standards — Entity states, EntityInput, EntityMovement, EntityFindTarget
globs: ["Assets/Script/Character/Entity/**/*.cs", "Assets/Script/Enemy/**/*.cs"]
---

# AI / Enemy Code Standards

## Performance Budget
- `EntityInput.Update()` runs every frame for every enemy — keep it under 0.1ms per entity
- Use `Physics2D.OverlapCircleNonAlloc` with a cached `Collider2D[]` buffer — never allocate in Update
- AI state transitions must be driven by cached data, not fresh physics queries each frame

## Debuggability
- Every state transition must be readable from the Animator or a custom debug gizmo
- Draw `OnDrawGizmosSelected` for field-of-view range, attack range, and current target
- Log state changes in DEBUG builds only: `#if UNITY_EDITOR`

## Data-Driven Parameters
- All AI tuning values (FOV range, idle duration, move duration, attack range) live in the `EntityData` ScriptableObject, read via `entity.Data`
- Stat values (HP, Defense, MoveSpeed…) live in an `EnemyStatSO : BaseStatsSO` profile, read via `EntityStatsHandler` (max) and `EntityVitalStats` (current) — never via `EntityData` directly
- Never hardcode distances, timers or damage in state classes. ⚠️ `EntityAttack.Attack()` still hardcodes `TakeDamage(10, …)` (BUG-043) — that is a known violation, not a pattern to follow

## Null Safety
- Always null-check `target` before calling `.position`, `.transform`, or distance calculations
- `EntityMoveState`: guard `if (target == null) { stateMachine.ChangeState(idle); return; }` at top of Update
- `EntityInput`: check `detectedPlayer != null` before assigning to `inputTarget`

## State Machine Contract
- All Entity states extend `EntityState` (NOT `MonoBehaviour`)
- State `Enter()` caches needed data; `LogicUpdate()` runs transitions; `Exit()` cleans up

> The old instruction "`EntityDeathState` must extend `EntityState` — fix the base class bug
> before shipping" was removed on 2026-09-11: that bug (#7) was fixed long ago.
> `EntityDeathState : EntityBasicState` and emits `ON_ENEMY_DEATH` on `EndRangeTrigger`.

## Damage Reception (added 2026-09-11)

- `EntityNegativeReciver` is the **single** `INegativeReceiver` implementer on an enemy. Do not add
  a second one — that was BUG-053, and it shipped player-only logic on an enemy for six sprints
- Mitigation lives in `DamageCalculate()`: subtract `StatType.Defense`, clamp at 0
- Write damage to `EntityVitalStats.ReceiveReduction(StatType.HP, …)`, never to a private field
- ⚠️ `EntityVitalStats` currently indexes `currentStats[statType]` with no key guard (BUG-066) —
  guard any new `StatType` you read through it
