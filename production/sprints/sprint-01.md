# Sprint 1 -- 2026-06-08 to 2026-06-12

## Sprint Goal
Turn the navigable dungeon skeleton into a playable demo loop: player attacks deal damage, enemies die and unlock doors, and the player can die and restart — the minimum slice needed to playtest combat for the first time.

## Capacity
- Total days: 5
- Buffer (20%): 1 day
- Available: 4 days

## Tasks

### Must Have (Critical Path)
| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|-------------|-------------------|
| S1-01 | Fix `WeaponMelee.Attack()` empty foreach (Bug #4 / TD-008) | gameplay-programmer | 0.25 | None | Foreach calls `INegativeReceiver.TakeDamage(currrentSA.attackDamege, transform.position)`, mirroring `EntityWeaponMelee.Attack()`; verified hitting a live enemy reduces `EntityStatsSO.Health` in Play Mode |
| S1-02 | Fix `AnimationPlayerController` double-registration (Bug #9 / TD-009 — rated Critical, "all combat states permanently stuck") | gameplay-programmer | 0.25 | None | `OnEnable`/`OnDisable` register/unregister `StartAnimation` and `EndAnimation` distinctly (mirror fix in both); `PlayerUseWeaponState` reliably exits on `animFinish` |
| S1-03 | Player death + restart loop (Bug #6) | gameplay-programmer | 0.5 | S1-01 | `Core.TakeDamage()` emits new `EventID.ON_PLAYER_DEATH` when `currentHealth <= 0`; `GameManager` subscribes → `PlayerData.Reborn()` → reload `StartScene`; verified end-to-end death → restart in Play Mode |
| S1-04 | Enemy AI death chain — fix atomically in one PR (Bugs #5, #7, #8 / TD-001, TD-012) | ai-programmer | 0.5 | None (parallel) | `EntityMoveState` null-checks `Target` at top of `LogicUpdate` (before line 30 deref); `EntityDeathState` extends `EntityState` and is wired into `EntityStateMachine`; `EntityBasicState` Health≤0 block transitions to death state and emits `ON_CLEAR_ENEMY` on last enemy death; no NullRef on target loss |
| S1-05 | Room-clear condition — lock/unlock doors on enemy clear | gameplay-programmer | 0.5 | S1-04 | `RoomCell` tracks live enemy count; doors lock on room entry, unlock via `EventManager.Emit` when count reaches 0 (never call `DoorController.OpenDoor()` directly from enemy death, per map-code.md) |

### Should Have
| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|-------------|-------------------|
| S1-06 | HUD health bar (`UIManager`) | ui-programmer | 0.5 | S1-03 | `UIManager` subscribes to new `ON_PLAYER_TAKE_DAMAGE` event in `OnEnable`/unsubscribes in `OnDisable`; slider reflects `PlayerData.currentHealth`; no `Update()` polling; wired via Inspector ref (no singleton) |
| S1-07 | Between-room upgrade picker (3 stat cards) | gameplay-programmer + ui-programmer | 1.0 | S1-05 | On room clear, game pauses, shows 3 `InternalSkillSO` cards; chosen card applies stat boost to `PlayerData`; cards pooled, no per-spawn `Instantiate` |

### Nice to Have
| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|-------------|-------------------|
| S1-08 | Start menu placeholder scene | ui-programmer | 0.5 | None | `StartScene` has a functional "Start" button loading `RandomMaze`; styling not required |

## Carryover from Previous Sprint
| Task | Reason | New Estimate |
|------|--------|-------------|
| (No formal previous sprint existed) — but Bugs #4, #6, #5/#7/#8, #9 have been flagged as demo-blockers in 3 consecutive weekly reviews (5/26, 5/29, 6/05) without a fix | No sprint structure existed to track them | Folded into S1-01 → S1-04 above |

## Risks
| Risk | Probability | Impact | Mitigation |
|------|------------|--------|------------|
| Map/Room system (10 open bugs per bug-triage 5/29) destabilizes combat testing mid-sprint | Medium | High | Freeze new Map/Room work for the sprint; fix only what blocks testing the Must Haves |
| S1-04 (enemy AI) and S1-05 (room-clear) are tightly coupled — fixing one without the other yields no testable loop | High | Medium | Sequence S1-04 → S1-05 explicitly; pair-test together before marking either done |
| Zero automated tests exist (TD-014) — state machine regressions go unnoticed | High | Medium | Add at least one EditMode test for `Core.TakeDamage → ON_PLAYER_DEATH` per test-standards.md (BLOCKING gate for Logic stories) |
| Pattern risk: 3 straight weeks of effort went to Map refactors instead of these exact blockers | High | High | This sprint file becomes the tracked commitment — `/sprint-status` at next Monday's review will show directly whether the pattern broke |

## Dependencies on External Factors
None — all tasks are internal fixes within existing Player/Entity state machine and EventManager frameworks.

## Definition of Done for this Sprint
- [ ] All Must Have tasks completed
- [ ] All tasks pass acceptance criteria
- [ ] QA plan exists (`production/qa/qa-plan-sprint-1.md`)
- [ ] All Logic/Integration stories have passing unit/integration tests
- [ ] Smoke check passed (`/smoke-check sprint`)
- [ ] QA sign-off report: APPROVED or APPROVED WITH CONDITIONS (`/team-qa sprint`)
- [ ] No S1 or S2 bugs in delivered features
- [ ] Design documents updated for any deviations
- [ ] Code reviewed and merged

> **Note on review mode**: Review mode is `lean` — the producer feasibility gate (PR-SPRINT) was skipped per the lean-mode rule.

> **Scope check:** This sprint pulls 5 must-have items directly from the existing demo-blocker backlog (no new scope beyond what was already identified in review 5/26, bug-triage 5/29, and the tech-debt register). Run `/scope-check sprint-1` mid-sprint if any new stories get added.
