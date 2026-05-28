# Technical Debt Register

Last updated: 2026-05-28
Total items: 21 | Estimated total effort: XL (if all fixed) | Sprint to pay first: demo-critical items

---

## Register

| ID | Category | Description | Files | Effort | Impact | Priority | Added |
|----|----------|-------------|-------|--------|--------|----------|-------|
| TD-001 | Architecture | `WeaponMelee.Attack()` foreach body empty — no damage applied to enemies. Player melee is non-functional. | Assets/Script/Weapons/MeleeWeapon/WeaponMelee.cs:29 | S | Critical | 1 | 2026-05-28 |
| TD-002 | Architecture | `EntityDeathState` extends MonoBehaviour instead of EntityState — not in state machine. Enemy cannot die. | Assets/Script/Character/Entity/States/EntityDeathState.cs | S | Critical | 1 | 2026-05-28 |
| TD-003 | Architecture | `EntityBasicState.LogicUpdate()` health ≤ 0 block is empty — no transition to death state. | Assets/Script/Character/Entity/States/EntityBasicState.cs:21 | S | Critical | 1 | 2026-05-28 |
| TD-004 | Architecture | `Core.TakeDamage()` has no death check — no `ON_PLAYER_DEATH` event fired. Player cannot die. | Assets/Script/Character/Player/Core/Core.cs:20 | S | Critical | 1 | 2026-05-28 |
| TD-005 | Architecture | `AnimationPlayerController.OnEnable()` line 21 registers StartAnimation twice; EndAnimation never fires. All use-weapon states permanently stuck. | Assets/Script/Character/Player/Animation/AnimationPlayerController.cs:21 | S | Critical | 1 | 2026-05-28 |
| TD-006 | Architecture | Skill cooldown broken — no `ActivateSkill.Exit()` override calls `SetCanUseAbility(true)`. All active skills lock permanently after first use. | Assets/Script/Skill_Ability/DashAbility.cs, SlashAbility.cs, BlockAbility.cs | S | Critical | 1 | 2026-05-28 |
| TD-007 | Architecture | `EntityMoveState.LogicUpdate()` dereferences `entity.Input.Target.transform.position` (line 30) before null check (line 34). NullRef when target lost mid-chase. | Assets/Script/Character/Entity/States/EntityMoveState.cs:30 | XS | High | 2 | 2026-05-28 |
| TD-008 | Architecture | `TalentManager.Awake()` hardcodes all stats (str=dex=int=cha=skillPoint=10). Violates ScriptableObject-first rule. | Assets/Script/Character/Player/CoreComponent/TalentManager.cs:20-24 | S | High | 2 | 2026-05-28 |
| TD-009 | Architecture | `AnimationName.cs` is an empty ScriptableObject — should be a static class with string constants, as animation-system.md specifies. All animation state names are currently magic strings. | Assets/Script/Character/Player/Animation/AnimationName.cs | S | High | 2 | 2026-05-28 |
| TD-010 | Performance | `Physics2D.OverlapCircle` (allocating) in `EntityFindTarget.cs` — called in `EntityInput.Update()` every frame per enemy. Should be `OverlapCircleNonAlloc` with cached buffer. | Assets/Script/Character/Entity/CoreComponent/EntityFindTarget.cs:21 | S | High | 2 | 2026-05-28 |
| TD-011 | Performance | `Physics2D.OverlapCircle` (allocating) in `EntityWeaponMelee.Attack()`. Should be NonAlloc variant. | Assets/Script/Character/Entity/EntityWeaponMelee.cs:33 | S | Medium | 3 | 2026-05-28 |
| TD-012 | Code Quality | `Shooting.Update()` reads `Input.GetMouseButton(0)` directly — bypasses state machine. Fires outside `PlayerAttackState`. | Assets/Script/Weapons/RangeWeapon/Shooting.cs:16 | M | High | 2 | 2026-05-28 |
| TD-013 | Code Quality | `FastMovement.cs` uses `Input.GetAxisRaw()` (legacy Input API). Should use New Input System. Note: this is a dev utility for map testing, not player input. | Assets/Script/Utility/FastMovement.cs:16 | S | Low | 4 | 2026-05-28 |
| TD-014 | Dead Code | `PlayerCombat.cs` — 129-line file, 93 lines commented out. CLAUDE.md: "Legacy class — all code commented out, do not use." No references remain. Safe to delete. | Assets/Script/Weapons/MeleeWeapon/PlayerCombat.cs | XS | Low | 4 | 2026-05-28 |
| TD-015 | Dead Code | `DualAbility.cs` — 82-line file, ~60 lines commented out. CLAUDE.md: "all code commented out (WIP)". Active redesign pending. | Assets/Script/Skill_Ability/DualAbility.cs | — | Low | 5 | 2026-05-28 |
| TD-016 | Dead Code | `Map/Legacy/Door.cs` and `Map/Legacy/Room.cs` — superseded by DoorController and RoomController. Not referenced. | Assets/Script/Map/Legacy/Door.cs, Room.cs | XS | Low | 4 | 2026-05-28 |
| TD-017 | Dead Code | `NewEnemyState.cs` and `NewEnemyStateMachine.cs` — both extend MonoBehaviour instead of EntityState; body empty. Stub files with wrong base class. | Assets/Script/Enemy/NewEnemyState.cs, NewEnemyStateMachine.cs | XS | Low | 4 | 2026-05-28 |
| TD-018 | Dead Code | `PlayerUserItemState.cs` — extends MonoBehaviour (wrong base class). Stub. | Assets/Script/Character/Player/States/PlayerUserItemState.cs | XS | Low | 4 | 2026-05-28 |
| TD-019 | Architecture | `UIManager.cs` — empty stub. Health bar, HUD, and upgrade UI not implemented. Blocks demo completion (Demo Checklist item #9). | Assets/Script/Manager/UI/UIManager.cs | L | High | 2 | 2026-05-28 |
| TD-020 | Architecture | `RangeWeapon.Attack()` empty stub; `bullet.TakeDamage()` commented out; `RangeWeapon.CheckCanAttack()` no cooldown logic. Ranged combat non-functional. | Assets/Script/Weapons/RangeWeapon/RangeWeapon.cs, bullet.cs | M | Medium | 3 | 2026-05-28 |
| TD-021 | Test Debt | Zero automated tests across the entire codebase. No EditMode or PlayMode tests in `tests/` directory. Blocking gate per test-standards.md (Logic stories require EditMode unit tests). | tests/ (empty) | XL | High | 2 | 2026-05-28 |

---

## Sprint Priority Grouping

### Must fix before demo (Priority 1 — Critical blockers)
- TD-001 WeaponMelee.Attack() damage
- TD-002 EntityDeathState base class
- TD-003 EntityBasicState death transition
- TD-004 Core.TakeDamage() death check
- TD-005 AnimationPlayerController EndAnimation bug
- TD-006 Skill cooldown reset

### Should fix for demo quality (Priority 2 — High impact)
- TD-007 EntityMoveState null guard
- TD-008 TalentManager SO-driven
- TD-009 AnimationName constants
- TD-010 EntityFindTarget NonAlloc
- TD-012 Shooting.Update() legacy input
- TD-019 UIManager implementation
- TD-021 Minimum test coverage (damage chain + state transitions)

### Fix after demo (Priority 3-5 — Low urgency)
- TD-011, TD-013, TD-014, TD-015, TD-016, TD-017, TD-018, TD-020

---

## Debt Trends

| Month | Items | Critical | High | Note |
|-------|-------|----------|------|------|
| 2026-05-28 | 21 | 6 | 8 | First scan |
