# Sprint 1 -- 2026-06-08 to 2026-06-12

## Sprint Goal
Make the core combat loop playable: player attacks deal damage, enemies die and trigger room-clear, and player death triggers restart -- breaking the 2-week streak of zero progress on the 5 S1/S2 bugs blocking the demo.

## Capacity
- Total days: 5 (Mon-Fri)
- Buffer (20%): 1 day reserved for unplanned work
- Available: 4 days

## Tasks

### Must Have (Critical Path)
| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|-------------|-------------------|
| S1-1 | Fix `AnimationPlayerController` double registration (Bug #9 / TD-009) | gameplay-programmer | 0.5 | -- | `EndAnimation` event fires; `OnEnable`/`OnDisable` register matching pairs; `PlayerUseWeaponState` subclasses exit cleanly via `animFinish` in Play Mode |
| S1-2 | Fix `WeaponMelee.Attack()` empty foreach (Bug #4 / TD-008) | gameplay-programmer | 0.5 | S1-1 | Melee attack reduces enemy `EntityStatsSO.ModifiersHealth` via `INegativeReceiver.TakeDamage(currrentSA.attackDamege, transform.position)`; mirrors `EntityWeaponMelee.Attack()`; verified in Play Mode |
| S1-3 | Fix `EntityStatsSO.ModifiersAmor` self-referencing getter -> StackOverflowException (TD-011, newly found) | gameplay-programmer | 0.25 | -- | Getter returns backing field, not itself; no `StackOverflowException` when armor is read or damage is applied |
| S1-4 | Fix Enemy AI death chain atomically (Bugs #5, #7, #8) | ai-programmer | 1.0 | S1-3 | `EntityMoveState` null-guards `Target` before dereference; `EntityDeathState` extends `EntityState` and is wired into `EntityStateMachine`; `EntityBasicState.LogicUpdate()` transitions to `EntityDeathState` on `Health <= 0`; emits `ON_ENEMY_DEATH` on entry |
| S1-5 | Implement player death + restart (Bug #6) | gameplay-programmer | 0.75 | S1-1, S1-2 | `Core.TakeDamage()` checks `currentHealth <= 0`, emits new `ON_PLAYER_DEATH` (added to `EventID` enum); new `GameManager` subscriber calls `PlayerData.Reborn()` + reloads `StartScene`; verified end-to-end |
| S1-6 | Wire room-clear condition | gameplay-programmer | 0.75 | S1-4 | `RoomCell` tracks live enemy count; doors lock on room entry, unlock via `ON_CLEAR_ENEMY` (emitted from `EntityDeathState`) when count = 0 |

**Must Have total: 3.75 days** (fits the 4-day available capacity)

### Should Have
| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|-------------|-------------------|
| S1-7 | Health bar HUD via `UIManager` | ui-programmer | 0.5 | S1-5 | `UIManager` subscribes to `ON_PLAYER_TAKE_DAMAGE` in `OnEnable`/unsubscribes in `OnDisable`; slider reflects `currentHealth/maxHealth`; no per-frame polling |
| S1-8 | End-to-end combat playtest session | qa-tester | 0.5 | S1-2, S1-4, S1-5, S1-6 | Documented session in `production/qa/playtests/` covering: enter room -> fight -> kill enemy -> room clear -> death -> restart |

### Nice to Have
| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|-------------|-------------------|
| S1-9 | Between-room upgrade picker stub (3 stat cards) | gameplay-programmer | 1.0 | S1-6 | After `ON_CLEAR_ENEMY`, game pauses and shows 3 `InternalSkillSO` cards; selecting one applies a stat boost to `PlayerData` |

## Carryover from Previous Sprint
| Task | Reason | New Estimate |
|------|--------|-------------|
| -- | No prior formal sprint existed (this is Sprint 1) | -- |

## Risks
| Risk | Probability | Impact | Mitigation |
|------|------------|--------|------------|
| Enemy AI death chain touches 3 interconnected files -- regression risk | Medium | High | Fix S1-4 atomically in one PR; code review before merge (per `ai-code.md` rules) |
| No unit tests exist for damage formula / state machines -- fixes may regress silently | High | Medium | Add EditMode test for `Core.TakeDamage()` -> `ON_PLAYER_DEATH` chain (required by `test-standards.md` for Logic stories -- BLOCKING gate) |
| Team has spent 2 weeks polishing the Map system instead of these blockers -- pattern may repeat | Medium | High | Defer ALL map-system work this sprint; `/sprint-status` mid-week to catch drift early |

## Dependencies on External Factors
- None -- all fixes are internal C# changes, testable via Unity Editor Play Mode. No external content or asset dependencies.

## Definition of Done for this Sprint
- [ ] All Must Have tasks (S1-1 -> S1-6) completed
- [ ] All tasks pass acceptance criteria
- [ ] QA plan exists (`production/qa/qa-plan-sprint-1.md`)
- [ ] All Logic/Integration stories (S1-2, S1-4, S1-5, S1-6) have passing EditMode/PlayMode tests
- [ ] Smoke check passed (`/smoke-check sprint`)
- [ ] QA sign-off report: APPROVED or APPROVED WITH CONDITIONS (`/team-qa sprint`)
- [ ] No S1 or S2 bugs in delivered features
- [ ] Design documents updated for any deviations (`weapons-system.md`, `skill-ability-system.md` per cross-review CONCERNS)
- [ ] Code reviewed and merged

---

> **Producer feasibility gate**: skipped -- review mode is `lean` (`production/review-mode.txt`).

> **Scope check:** If this sprint includes stories added beyond the original epic scope, run `/scope-check [epic]` to detect scope creep before implementation begins.

> ⚠️ **No QA Plan**: This sprint was started without a QA plan. Run `/qa-plan sprint`
> before the last story is implemented. The Production -> Polish gate requires a QA
> sign-off report, which requires a QA plan.
