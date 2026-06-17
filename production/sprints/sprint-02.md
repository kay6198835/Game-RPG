# Sprint 2 -- 2026-06-15 to 2026-06-19

## Sprint Goal
Pay down the Player combat architecture debt without leaving combat untestable again: decouple `Weapon` from `WeaponHolder`/`AbilityHolder` (push-on-equip ability flow) and introduce `Core.GetCoreComponent<T>()` (OCP fix), while folding in the two demo-blocker bugs that live inside the same code (Bug #9 animation-event, Bug #4 empty melee damage) so the equipped-weapon → attack → skill loop becomes playtestable by Friday.

> **Context**: This sprint is the tracked commitment for the Core/Weapon/Ability refactor planned this week (reference: `~/.claude/plans/check-code-h-th-ng-flickering-plum.md`). **Prioritization decision (2026-06-15)**: finish *this system* (weapon/ability architecture, incl. melee damage and ability Enter→Exit) first; the stats system is the next intended theme (likely Sprint 3); the remaining combat-loop blockers (player death, enemy death, room-clear) and other bugs/flows are sequenced *after* those two systems are stable. The two bugs kept in scope (#9, #4) are kept because the refactor cannot be *verified* without them — they are part of completing the weapon/ability system, not general combat cleanup.

## Capacity
- Total days: 5
- Buffer (20%): 1 day
- Available: 4 days
- *(Capacity assumed same as Sprint 1 — adjust if real availability differs.)*

## Tasks

### Must Have (Critical Path)
| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|-------------|-------------------|
| S2-01 | Stabilize + commit the in-progress `feature/refactor-state-system` working tree (28 uncommitted files: `PlayerInputHandle` moved `Input/`→`CoreComponent/`, all Player states, Core, weapons, abilities) | lead-programmer | 0.5 | None | Working tree compiles clean in Editor (no Console errors); `PlayerInputHandle.cs` + `.meta` relocation complete, old `Input/` copies removed; branch committed and pushed so later refactor edits start from a clean base |
| S2-02 | Refactor Part B — decouple `Weapon` ↔ `WeaponHolder`/`AbilityHolder` via push-on-equip; fixes the `UnityEditor` build-break for free | lead-programmer / gameplay-programmer | 1.0 | S2-01 | `Weapon` has no `holder`/`abilityHolder`/`currentAbilitySO`/`SetAbility`; exposes `Stats`/`OnEquip(Transform)`/`OnUnequip()`; `Interact()` calls `((WeaponHolder)interactor).Equip(this)`. `WeaponHolder.Equip()` pushes `Stats.AbilityWeapon`+`Stats.SkillWeapon` into `AbilityHolder.RegisterAbilities`; `Unequip()` clears. `AbilityHolder` is slot-based (`abilitySlot`/`specialSlot`+`SelectSlot`), **no `using UnityEditor.*` import**. Play Mode: equip → **E** fires Special, **RMB** fires Block; unequip clears both slots (per `weapon-skill-code.md`) |
| S2-03 | Refactor Part A — `Core.GetCoreComponent<T>()` + self-register + lazy-cache (OCP fix) | lead-programmer | 1.0 | S2-02 | `Core` holds `List<CoreComponent>` + `AddComponent` + `GetCoreComponent<T>()` (`foreach`, **no LINQ**) + `ref`-cache overload; named component properties removed; `CoreComponent.Awake` self-registers. Every call site uses lazy-cached accessor (no per-frame `GetCoreComponent` call); Console shows **no `"<T> not found"`** warnings. Grep `\.Core\.` / `core\.` confirms no missed site (e.g. `DashAbility` `player.Core.Movement`) per `engine-code.md` |
| S2-04 | Fix Bug #9 — `AnimationPlayerController` double-registration | gameplay-programmer | 0.25 | S2-01 | `OnEnable`/`OnDisable` register/unregister `StartAnimation` **and** `EndAnimation` distinctly (mirror in both); `PlayerUseWeaponState` exits reliably on `animFinish`; refactored ability state reaches `Exit()` (prevents ability getting stuck — must be verified together with S2-02) |
| S2-05 | Fix Bug #4 — `WeaponMelee.Attack()` empty foreach | gameplay-programmer | 0.25 | S2-02 | `Attack()` foreach calls `INegativeReceiver.TakeDamage(currrentSA.attackDamege, transform.position)`, mirroring `EntityWeaponMelee.Attack()`; verified hitting a live enemy reduces `EntityStatsSO.Health` in Play Mode (keep typo `attackDamege`) |

### Should Have
| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|-------------|-------------------|
| S2-06 | Add one EditMode test for the refactored equip→ability path | qa-tester | 0.5 | S2-02 | `WeaponHolder.Equip(weapon)` registers the weapon's two SOs into `AbilityHolder`; `SelectSlot(Ability)` vs `SelectSlot(Special)` resolves `ActiveAbility` to the correct SO; test uses its own `WeaponStats`/`AbilityHolder` instances, no project-asset mutation (per `test-standards.md`) |

## Deferred from Sprint 1 (DECIDED 2026-06-15 — defer, do not pull in)
Owner decision: complete the weapon/ability system (this sprint) and the stats system (next) before returning to the game-loop blockers. These stay parked, explicitly tracked so they are not lost:

| Task | Reason | Status | Target |
|------|--------|--------|--------|
| S1-03 (Bug #6) Player death + restart loop | Deferred behind system work | UNCHANGED since 2026-05-29 | Sprint 4+ (after stats) |
| S1-04 (Bugs #5/#7/#8) Enemy AI death chain | Deferred | UNCHANGED | Sprint 4+ |
| S1-05 Room-clear lock/unlock | Blocked on S1-04 | UNCHANGED (blocked) | Sprint 4+ |
| Map/Room bugs (BUG-RC-1, BUG-RGC-4, BUG-MC-1, BUG-DC-1) | Out of this system's scope | OPEN | Sprint 4+ / when Map work resumes |

> **Honesty note for the retrospective trail**: these combat blockers have now been on the board 5 consecutive weeks. Deferral here is a *deliberate, recorded* prioritization (finish architecture + stats first), not silent slippage. Revisit at Sprint 3 planning once the stats theme is scoped.

## Next Sprint Outlook (Sprint 3 — tentative)
- **Theme**: begin integrating the **stats system** (a prototype already exists: `0670ac0 add template stats system`, plus `TalentManager`). Promote the prototype to production standards per `prototype-code.md` (SO-driven, no `Find()`, state-machine where applicable).
- After stats stabilizes: return to the deferred Sprint-1 combat-loop blockers (death, enemy death, room-clear) and the remaining bug/flow backlog.

## Risks
| Risk | Probability | Impact | Mitigation |
|------|------------|--------|------------|
| **Recurring pattern (5th week)**: refactor work absorbs the whole sprint and combat stays unplayable | High | High | Bugs #9 + #4 are pulled INTO the refactor Must Haves (S2-04/S2-05) so even a refactor-heavy week leaves combat testable; time-box S2-03 (OCP) and cut it first if slipping |
| 28-file uncommitted working tree conflicts with refactor edits | High | Medium | S2-01 commits/pushes the clean base BEFORE any S2-02/03 edit starts — hard dependency |
| Refactor Part A (S2-03) touches many call sites — a missed site or Awake-order null | Medium | Medium | Use lazy-cache pattern (never cache in `Awake`); grep `\.Core\.`/`core\.` as a completion gate; watch Console for `"not found"` warnings |
| Bug #9 and S2-02 interact — ability may get stuck not exiting | Medium | High | Pair-test S2-04 with S2-02; do not mark S2-02 done until an ability completes Enter→Exit in Play Mode |
| Zero automated tests (TD-014) — state-machine regressions go unnoticed | High | Medium | S2-06 adds the first EditMode test on the refactored equip→ability seam (BLOCKING gate for the Logic story) |

## Dependencies on External Factors
None — all work is internal to the Player Core / Weapon / Ability framework and the EventManager. No remote/asset dependencies.

## Out of Scope (explicit)
- Interfaces (`IAbilityRunner`/`IWeaponHolder`) — rejected per YAGNI (single implementation); revisit only when a second consumer or a test seam needs it.
- Entity-side `EntityCore`/`EntityWeaponHolder` — separate hierarchy, untouched this sprint.
- The two future directions noted in the plan (active-skill slot limit + rarity/cost model; user-customizable input→skill mapping) — roadmap, not this sprint.

## Definition of Done for this Sprint
- [ ] All Must Have tasks completed and pass acceptance criteria
- [ ] Editor compiles clean; no `UnityEditor` import in runtime code; no `"<T> not found"` Console warnings
- [ ] Play Mode smoke: equip weapon → melee deals damage (S2-05) → E/RMB run the correct ability and Exit cleanly (S2-02 + S2-04)
- [ ] S2-06 EditMode test passes (`/smoke-check sprint`)
- [ ] Carryover decision recorded (Sprint 3 defer vs pull-in)
- [ ] Refactor deviations from the plan documented in the plan file
- [ ] Code reviewed and merged into `feature/refactor-state-system`

> **Scope check**: Must Haves trace to the refactor plan + Sprint-1 demo-blocker backlog (Bug #9, #4); no new feature scope. Run `/scope-check sprint-2` mid-sprint if stories get added.

---

## Added Note (2026-06-16) — Animation Control & Combo Attack Logic

> Source branch: `claude/keen-shannon-j91iry`. Full plan: `/root/.claude/plans/hi-n-t-i-game-c-a-cheeky-lecun.md`.
> Combat is Cult-of-the-Lamb style: top-down, 8-direction melee, one blend tree per action.
> **Status of this note**: design/decision context for upcoming weapon-animation work. Not yet pulled into the Sprint 2 Must Haves above — recorded here so it is not lost and can be scoped (likely a follow-up sprint, partly overlapping the S2-02/S2-05 weapon code).

### Problem 1 — Animation clip control (solution DECIDED, not yet coded)
Each action uses an 8-direction blend tree driven by a `Direction` parameter. Actions with multiple
variants (attack combo — one clip per hit) currently store one `AnimatorOverrideController` (AOC) **per
variant** in `AttackSO.directionAttackAnimatorOV`, which does not scale.

**Approved solution (Option 1):**
- Keep a single base Animator Controller.
- Instantiate one runtime AOC on weapon equip (stored on `WeaponMelee`), assign it to the `Animator` once.
- Each combo step overrides only the 8 directional clip slots (clips sourced from
  `AttackSO.directionClips : AnimationClip[8]`) — do **not** swap the whole controller.
- Keep the existing Animation Event → damage chain intact.
- Slot key = the original clip name in the blend-tree leaves → store as `string[8]` in
  `GameConstants.AnimationName` (no hardcoded literals).

**Files to change:** `AttackSO.cs` (replace the AOC field with `AnimationClip[8]`),
`WeaponMelee.cs` (`CheckCanAttack` + `DurationNextAttack`), `GameConstants.cs`.

**Caveat:** under Option 1, `AOC.clips` returns more than 8 pairs, so `DurationNextAttack` must read
`directionClips[0].length` directly and **drop the `/8` division**.

### Problem 2 — Combo not smooth between hit 1 → hit 2 (analyzed, not yet fixed)
Root causes:
1. **No combo-cancel window**: `PlayerAttackState` inherits `PlayerUseWeaponState` (not `PlayerBasicState`),
   so the `IsAttack → AttackState` transition (only present in `PlayerBasicState:29`) is unreachable while
   attacking → the flow is forced through Attack → Idle/Move → Attack → stutter.
2. **No input buffer**: `OnAttack` (`PlayerInputHandle.cs:165`) only sets `isAttack` from the held flag; it
   is read on exactly one frame after the clip ends → fast taps get dropped.
3. **`lastClickTime` wrong semantics**: `Weapon.CheckCanAttack:24` assigns it to `StartAttackTime` (the
   moment the state is entered), not the actual click time.
4. **(Not a bug)** the `/8` in `DurationNextAttack` is intentional: `totalDuration` sums 8 same-length
   directional clips, so `/8` yields the length of a single clip. (Note: superseded by Problem 1's caveat
   once `directionClips` lands — read `directionClips[0].length` instead.)

**Fix direction (not yet done):**
- Add a combo-cancel window: an Animation Event at ~60-70% of the clip opens a window that allows
  re-entering `AttackState` (incrementing `currentStateIndex`) instead of bouncing through Idle.
- Buffer attack input (store `lastAttackPressedTime` on press, valid ~0.2s).
- Separate the real `lastClickTime` from `StartAttackTime`; compute the reset window from the current
  clip length instead of `/8`.

**Environment caveat:** Unity Play Mode cannot run here → static analysis only. Smoothness is a
Visual/Feel concern (ADVISORY per `test-standards.md`) and must be confirmed by playtest.
