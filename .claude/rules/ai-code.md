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
- All AI tuning values (FOV range, idle duration, move duration, attack range) live in `EntityData` ScriptableObject
- Never hardcode distances or timers in state classes — always read from `entity.entityData`

## Null Safety
- Always null-check `target` before calling `.position`, `.transform`, or distance calculations
- `EntityMoveState`: guard `if (target == null) { stateMachine.ChangeState(idle); return; }` at top of Update
- `EntityInput`: check `detectedPlayer != null` before assigning to `inputTarget`

## State Machine Contract
- All Entity states extend `EntityState` (NOT `MonoBehaviour`)
- `EntityDeathState` must extend `EntityState` — fix the base class bug before shipping
- State `Enter()` caches needed data; `LogicUpdate()` runs transitions; `Exit()` cleans up
