# Melee Combat Code Review — Summary

**Branch:** `claude/melee-attack-damage-review-urd4l0`
**Date:** 2026-07-30
**Scope:** Player melee damage (`WeaponMelee.Attack()`) + enemy AI/damage changes on this branch (diff vs `origin/feature/enemy-control`).
**Status:** 0 of 16 issues fixed — all re-verified against local HEAD `7995066` (no gameplay code changed since the review). Both melee directions currently deal no reliable damage (player path unwired; enemy path NullRefs).

Priority legend: 🔴 blocking · 🟠 should-fix · 🟡 defensive/minor · ⚪ decided-skip

---

## A. Player melee — `WeaponMelee.Attack()`

The `Attack()` body itself (OverlapCircle → `INegativeReceiver.TakeDamage`) is content-correct.
`GetComponentInChildren<INegativeReceiver>()` is correct for the current hierarchy
(collider on parent/root, receiver on child) and stays correct if the hurtbox collider
is co-located on the receiver's GameObject.

| # | Prio | Issue | Location | Fix |
|---|------|-------|----------|-----|
| A1 | 🔴 | `Attack()` is never invoked. `AnimationPlayerController.Attack(object)` is an empty method and no state calls the weapon's `Attack()`. Player deals no melee damage. | `AnimationPlayerController.cs:43`, `PlayerAttackState.cs` | Call `weaponHolder.Weapon.Attack()` on `Status == StartRangeTrigger` (mirror `EntityAttackState`), or implement `AnimationPlayerController.Attack`. |
| A2 | 🟠 | `OverlapCircleAll` allocates a `Collider2D[]` per swing — violates project standard. | `WeaponMelee.cs:28` | Use `OverlapCircleNonAlloc` with a cached `Collider2D[]` field. |
| A3 | 🟡 | `currrentSA` dereferenced with no null guard; NullRef possible if an attack animation event fires before the first `SetAnimation` (e.g. event on frame 0 at spawn). | `WeaponMelee.cs:28,34` | Add `if (currrentSA == null) return;` at top of `Attack()`. Place the hit event mid-clip, not frame 0. |
| A4 | 🟠 | Combo index leaks between separate attacks: the time-based reset in `SetAnimation` is commented out, so `currentStateIndex` only resets after the full chain. Interrupting a combo and attacking later resumes mid-chain instead of restarting at swing 0. Reference `EntityWeaponMelee.CheckCanAttack` keeps this reset. | `WeaponMelee.cs:71` | Re-enable the time check: reset `currentStateIndex = 0` when `lastClickTime + deplayTime < Time.time`. |
| A5 | 🟡 | `lastClickTime` assigned twice in `SetAnimation`. | `WeaponMelee.cs:76,85` | Remove the redundant assignment. |
| A6 | ⚪ | Double-hit (same enemy with multiple colliders → multiplied damage) — **decided to skip**: current design uses a single damage-only collider per enemy; overlaps between distinct enemies are separate receivers (correct AoE). Revisit only if a body+hurtbox or compound collider is added (then dedupe by `attachedRigidbody`/receiver via `HashSet`). | — | No change now. |

**Hurtbox placement (design note):** placing the hurtbox `BoxCollider2D` on the same
GameObject as the `INegativeReceiver` component is the cleanest option — every lookup
(`GetComponent`/`TryGetComponent`/`GetComponentInChildren`) resolves on self. Requirements:
(1) that child must NOT have its own `Rigidbody2D` (compound collider stays owned by the
root RB, so `rb.MovePosition` still moves it and `attachedRigidbody` resolves to root);
(2) the child must be on the layer included in the weapon's `LayerMask`;
(3) adjust the collider offset so the hurtbox stays centered on the body.

---

## B. Enemy AI / damage (branch diff)

| # | Prio | Issue | Location | Fix |
|---|------|-------|----------|-----|
| B1 | 🔴 | `EntityAttackState.entityAttack` is declared but never assigned → `entityAttack.Attack()` throws NullReferenceException on every enemy attack. | `EntityAttackState.cs:10,23` | Resolve in `Enter()`: `entity.Core.GetCoreComponent(out entityAttack);` |
| B2 | 🔴 | `EntityAttack.Start()` caches the receiver via `entityInput.TargetTransform.gameObject.GetComponent<INegativeReceiver>()`. `TargetTransform` is null at Start (only set by the commented-out `GetTargetInRange`), so `.gameObject` NullRefs; and `GetComponent` on the player root cannot find `NegativeReciver` (a child CoreComponent) → null. | `EntityAttack.cs:13` | Resolve lazily inside `Attack()`, null-check the target, use `GetComponentInChildren` (or `attachedRigidbody`). |
| B3 | 🟠 | `EntityAttack.Attack()` hardcodes damage `10` and applies it to the cached player with no `OverlapCircle` range/facing test — enemy hits the player even when out of range. Bypasses `AttackSO` and the reference `EntityWeaponMelee` path. | `EntityAttack.cs:17` | Read damage from `AttackSO`/`EntityData`; range-check before applying, or route through the weapon. |
| B4 | 🟠 | Enemy attack has no cooldown: the `weaponHolder.Weapon.CheckCanAttack(...)` gate is commented out; the transition is purely `entityFindTarget.IsInRange()`, so `AttackState` re-enters every in-range frame. | `EntityBasicState.cs` | Reinstate a rate/cooldown check before transitioning to `AttackState`. |
| B5 | 🟠 | Public mutable fields on MonoBehaviours: `EntityFindTarget.minRange/maxRange`, `Entity.stateMachine`. Tuning values should live in `EntityData` SO. | `EntityFindTarget.cs`, `Entity.cs` | `[SerializeField] private` + property; move range values to `EntityData`. |
| B6 | 🟡 | `EntityIdleState` reads `entityData.IdleDurationTime` then overwrites it with `idleDurationTime = 3;` (self-noted debt). | `EntityIdleState.cs` | Drop the hardcode; use the SO value. |
| B7 | 🟡 | Magic numbers: waypoint threshold `0.7f` (×2), `time >= 10` timeout. | `EntityMovement.cs`, `EntityMoveState.cs` | Move to constants/SO. |
| B8 | 🟡 | `EntityFindTarget.IsInRange()`/`IsNearPlayer()` call `DistanceToPlayer()` twice per invocation, each recomputing the distance and re-writing the serialized `distanceToPlayer` field (query with a side effect), in per-frame state code. | `EntityFindTarget.cs` | Compute once, compare cached value; avoid mutating a serialized field as a side effect. |
| B9 | 🟡 | `PathRequestManager.path` promoted from a loop local to a field for no functional reason — widens scope and retains the last `Path` across frames. | `PathRequestManager.cs:14` | Revert to a local. |
| B10 | 🟠 | Base/derived double-transition: `base.LogicUpdate()` (EntityBasicState) may `ChangeState`, but its `return` only exits the base method — the derived state continues and can `ChangeState` again, overwriting the base transition within the same frame. Same pattern in `EntityMoveState`. | `EntityIdleState.cs:24-31`, `EntityMoveState.cs` | After `base.LogicUpdate()`, bail if the state already changed (check `stateMachine.CurrentState` or a changed-state flag) before running further transitions. |

---

## C. Testing gate

| # | Prio | Issue | Fix |
|---|------|-------|-----|
| C1 | 🟠 | No EditMode unit test for the damage path. `.claude/rules/test-standards.md` marks damage-formula / state-machine logic as **BLOCKING**. | Add a `TakeDamage`-style EditMode test (e.g. `TakeDamage_ReducesHealth`, `TakeDamage_BelowZero_EmitsDeath`). |

---

## Positive observations

- Added `return` after `ChangeState(...)` across `PlayerBasicState`, `PlayerMoveState`,
  `EntityBasicState`, `EntityIdleState` — fixes multiple transitions in one `LogicUpdate`.
- `EntityBasicState` death/take-damage branches now transition and return correctly;
  `EntityTakeDamageState` now calls `StopMove()`.
- `DistanceToPlayer()` null-guards `TargetTransform`; `PhysicsUpdate` guards `entityMovement != null`.
- `input`→`entityInput` migration cleanly removes the deleted `Entity.Input`/`.Target`
  properties — no dangling references remain (compile-safe).
- `WeaponMelee.Attack()` correctly uses `GetComponentInChildren` and reads damage from
  `AttackSO` (the enemy-side `EntityAttack` regressed on both points).
- `AnimationPlayerController.OnEnable` now registers five distinct callbacks — the old
  double-`StartAnimation` registration (CLAUDE.md Bug #9) is no longer present.

---

## Suggested fix order

1. B1, B2 (blocking NullRefs — enemy damage is dead until fixed)
2. A1 (wire player `Attack()` — player damage is dead until fixed)
3. B3, B4 (enemy damage correctness: data-driven + range + cooldown)
4. A2, A4, B10 (player: NonAlloc + combo reset; enemy: double-transition guard)
5. A3, A5, B5–B9 (defensive + cleanup)
6. C1 (damage unit test — required by test-standards)
