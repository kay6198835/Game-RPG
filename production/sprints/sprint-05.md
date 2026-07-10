# Sprint 5 — 2026-07-13 to 2026-07-17

## Sprint Goal

**Unblock the combat loop end-to-end: player death, enemy death chain, and the Enemy-Spawn System's Data + Algorithm phase — in parallel, since the two tracks are independent until Sprint 6's `EnemyManager` runtime wiring.**

Sprint 4 closed 100% on its Must-Have P1 block (first in 9 sprints) and fully designed the Enemy-Spawn System (GDD approved, ADR-0002 written). Sprint 5 spends that unblock: finish the two remaining combat-loop bugs (player death, enemy death chain) that have been carried since the original bug sweep, fix the one live NullReferenceException risk in the spawn prototype (BUG-ES-1), and build out `EnemyManager`'s actual lifecycle (BUG-EM-1) — currently a 7-line scaffold that a live event chain already calls into.

> **Context**: Sprint 4's own retro flagged an "off-plan work" pattern (design-only pivot broken same night, 9 commits of unplanned spawn-prototype code). That code is real and mostly good, but it bypasses `EnemyManager` entirely (BUG-ES-2) and has a null-safety gap (BUG-ES-1). Sprint 5 treats it as in-scope now — reconciling it toward the GDD/ADR rather than re-litigating whether it should exist.

---

## Capacity

- Total days: 5
- Buffer (20%): 1 day
- Available: 4 days

---

## Carry-Over From Sprint 4

| Item | Type | Priority | Origin |
|------|------|----------|--------|
| `cb099ee` "random padding position" (`EnemySpawner.cs`, `LoadRandomMap.unity`) — landed on `origin/feature/spawn-enemy` after Sprint 4's Saturday wrap-up, not present on the `sprint-04` branch tip this branch forked from | Carry (uncommitted-to-sprint) | P1 | S4 (post-wrapup) |
| Stashed WIP on `EnemySpawner.cs` + `LoadRandomMap.unity` (`git stash` entry `wip-before-sprint-kickoff-2026-07-13`) — 2-file, 4-line diff, uncommitted at kickoff time | Carry (stashed) | P1 | S4 (post-wrapup) |
| Fix BUG-06 — `NegativeReciver.TakeDamage()` throws `NotImplementedException`, player cannot take damage or die | Bug (S1) | P1 | Original sweep → S4→S5 |
| Fix BUG-05/07/08 cluster — `EntityMoveState` null-deref, `EntityDeathState` wrong base class, `EntityBasicState` empty death block | Bug (S1) | P1 | Original sweep → S4→S5 |
| Fix BUG-ES-1 — `RoomModel.GetSpawnSet()` can return `null`, `EnemySpawner` doesn't guard it → NRE on first empty-enemy-list room | Bug (S1, new S4) | P1 | S4 |
| Build `EnemyManager` lifecycle (BUG-EM-1) — alive-count tracking, per-room reset, `ON_ENEMY_DEATH`/`ON_CLEAR_ENEMY` emission per ADR-0002 | Task | P1 | S4 |
| S4-05 — Fix BUG-PIH-1, `CancelInvoke` pairing in `PlayerInputHandle.cs:264` | Bug | P2 | S2→S3→S4→S5 (4th carry) |
| S4-06 — `TalentManager` prototype → SO-driven | Task | P2 | S3→S4→S5 (3rd carry) |
| BUG-ES-2 — duplicate spawn drivers (`EnemySpawner` vs `LevelManager.SpawnRoomEnemies()`), neither routes through `EnemyManager` | Bug | P2 | S4 |
| BUG-ES-3 — `EventID` missing `ON_ENEMY_DEATH`/`ON_ROOM_CLEAR` | Bug | P2 | S4 |
| ADR-0002 (`EnemyManager` singleton) still `Status: Proposed`, needs explicit owner Accept before Sprint 6 hardens it further | Decision | P2 | S4 |
| ADR: Skill Enhance vs `ActivateSkill` pipeline — still not started | Decision | P3 | S3→S4→S5 (3rd carry) |
| Quick cleanups: BUG-AH-2 (dead import), BUG-EM-2 (dead `System.Numerics` import), BUG-WM-2 (duplicate assignment), BUG-LM-1 (dead null-check on non-nullable int) | Bug (P2, low effort) | P3 | S4 |

---

## Tasks

### Must Have (P1 — combat loop + spawn null-safety)

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S5-01 | Fix BUG-06 — implement `NegativeReciver.TakeDamage(int amount, Vector2 attackPosition)`: decrement `PlayerData.currentHealth`; `if (currentHealth <= 0) EventManager.Emit(EventID.ON_PLAYER_DEATH)`. Add `ON_PLAYER_DEATH` to `EventID` enum. | gameplay-programmer | 0.5 | None | `NegativeReciver.TakeDamage()` no longer throws; hitting player in Play Mode reduces `PlayerData.currentHealth`; `ON_PLAYER_DEATH` emitted at 0 HP; no direct health mutation bypassing the interface |
| S5-02 | Fix BUG-05 — `EntityMoveState.LogicUpdate()`: move `if (entity.Input.Target == null) { stateMachine.ChangeState(idleState); return; }` guard to the top, before the existing dereference at line 30 | ai-programmer | 0.25 | None | Guard is the first statement in `LogicUpdate()`; losing target mid-chase transitions to Idle with no NullReferenceException |
| S5-03 | Fix BUG-07 — rewrite `EntityDeathState` to extend `EntityState` (not `MonoBehaviour`); wire into `EntityStateMachine` | ai-programmer | 0.5 | S5-02 | `EntityDeathState : EntityState`; compiles; reachable via state machine transition |
| S5-04 | Fix BUG-08 — fill `EntityBasicState.LogicUpdate()` empty `Health <= 0` block: transition to `EntityDeathState`; emit `ON_ENEMY_DEATH` (new, see S5-06) on entry | ai-programmer | 0.25 | S5-03, S5-06 | Enemy at 0 HP transitions to death state in Play Mode; `ON_ENEMY_DEATH` fires exactly once per death |
| S5-05 | Fix BUG-ES-1 — guard `RoomModel.GetSpawnSet()` / `EnemySpawner.GetRoomSpawnSet()` against a null/empty return before `SpawnRoomEnemies()` reads `.Count` | gameplay-programmer | 0.25 | None | Loading a room with an empty enemy list does not throw; `set` is null-checked or defaults to an empty list before use |
| S5-06 | Fix BUG-ES-3 — add `ON_ENEMY_DEATH` and `ON_ROOM_CLEAR` to `EventID` enum (`EventManager.cs`) | gameplay-programmer | 0.1 | None | Both values present in `EventID`; zero other code changed by this task (pure enum addition) |
| S5-07 | Build `EnemyManager` lifecycle (BUG-EM-1) — track alive count per room (subscribe `ON_ENEMY_DEATH`), reset on `ON_LOAD_MAP`, emit `ON_CLEAR_ENEMY` when count reaches 0, per ADR-0002's Migration Plan | gameplay-programmer / lead-programmer | 1.0 | S5-04, S5-06 | `EnemyManager` maintains a live alive-count; `ON_CLEAR_ENEMY` emitted exactly once when the last tracked enemy in the current room dies; no singleton pattern beyond the ADR-0002-ratified `Instance` |

### Should Have

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S5-08 | Resolve BUG-ES-2 — decide canonical spawn driver (`EnemySpawner` event-driven vs `LevelManager.SpawnRoomEnemies()` editor-button); delete the losing path; route the winner through `EnemyManager` (S5-07) | lead-programmer | 0.5 | S5-07 | Exactly one spawn driver remains in `Assets/Script/`; it calls into `EnemyManager` for alive-count registration; no dead duplicate method |
| S5-09 | S4-05 — `PlayerInputHandle.cs`: audit all `Invoke`/`InvokeRepeating`; pair `Invoke(nameof(ChangeIsTakeDamage), 0.2f)` (line 264) with `CancelInvoke` in `OnDisable` | gameplay-programmer | 0.25 | None | Every `Invoke`/`InvokeRepeating` has a matching `CancelInvoke(name)` in `OnDisable`; no leaked invocations across state transitions in Play Mode |
| S5-10 | Get explicit owner sign-off flipping ADR-0002 `Status: Proposed` → `Accepted` | producer | 0.1 | S5-07 (lifecycle exists to review against) | ADR-0002 `Status` field reads `Accepted`; sign-off note recorded in the ADR |
| S5-11 | Reapply carried WIP — cherry-pick/reapply `cb099ee` and the stashed `wip-before-sprint-kickoff-2026-07-13` diff onto `sprint-05`, reconciled against S5-05/S5-07 changes to the same files | gameplay-programmer | 0.25 | S5-05, S5-07 | `EnemySpawner.cs` and `LoadRandomMap.unity` reflect the carried padding-position fix without reintroducing BUG-ES-1; stash entry dropped after confirmed reapplied |

### Nice to Have

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S5-12 | Quick cleanup batch — BUG-AH-2 (remove dead `using UnityEngine.UIElements;` in `AbilityHolder.cs`), BUG-EM-2 (remove dead `using System.Numerics;` in `EnemyManager.cs`), BUG-WM-2 (remove duplicate `lastClickTime = Time.time` in `WeaponMelee.SetAnimation()`), BUG-LM-1 (fix dead null-check `index == null` on non-nullable `int` in `LevelManager.cs:273`) | gameplay-programmer | 0.25 | None | All 4 grep-confirmed clean; no behavior change |
| S5-13 | S4-06 — `TalentManager` prototype → SO-driven: remove hardcoded `Awake` literal assignments, wire to `StatsCharacter` SO instance, Inspector-assignable | gameplay-programmer / lead-programmer | 1.0 | None | No `Awake` literal assignments remain; SO uses `[SerializeField]` + `[Range]`; no singletons |

---

## Deferred (Explicit)

| Task | Reason | Target |
|------|--------|--------|
| Sprint 6 runtime spawn wiring — `EnemyManager` listens `ON_LOAD_MAP` → `GetHybridEnemySet` → spawn at `Tile_Spawn` points; `Tile_Spawn` marker tile parser | Requires this sprint's `EnemyManager` lifecycle (S5-07) and enemy-death chain (S5-02→S5-04) as prerequisites, per the CLAUDE.md roadmap | Sprint 6 |
| Room-clear door lock/unlock UI feedback + HUD health bar | Blocked on this sprint's death-chain + `ON_CLEAR_ENEMY`/`ON_PLAYER_DEATH` landing first | Sprint 6 |
| Skill Enhance vs `ActivateSkill` pipeline ADR | Still not started, 3rd carry — no owner bandwidth allocated this sprint either | Sprint 6 |
| ADR-0002 code-review-gate revisit (per its own Review Trigger) | Only relevant once Sprint 6 hardens the singleton further | Sprint 6 |

---

## Risks

| Risk | Probability | Impact | Mitigation |
|------|------------|--------|------------|
| Off-plan work pattern recurs (flagged 9 consecutive sprints prior to S4) | Medium — S4 broke the streak on Must-Haves but still had heavy off-plan volume | High | S5-01→S5-07 are the critical path; hold new spawn-system feature work until they land |
| S5-07 (`EnemyManager` lifecycle) is the biggest single task and has two upstream dependents (S5-04, S5-08) | Medium | High | Start S5-07 immediately after S5-04/S5-06 land (Day 2 target); don't let it slip to Day 4 |
| Reapplying carried WIP (S5-11) conflicts with S5-05's null-guard changes in the same files | Medium | Low | Sequenced explicitly after S5-05; small 4-line diff, low conflict surface |
| No QA plan exists yet for this sprint | Confirmed | Medium | See QA Plan Gate note below — flagged, not silently skipped |
| Zero automated tests still open (TD-014, carried since S3) | High | Medium | Death-chain and `EnemyManager` changes are exactly the kind of regression surface EditMode tests should cover — consider pulling S4-08-style test task forward if capacity allows |

---

## Dependencies on External Factors

- No Unity CLI available in this environment — all Play Mode smoke checks require manual confirmation in-Editor by the owner, same constraint as Sprint 4.
- `gh` CLI unavailable in this environment — the draft PR for `sprint-05` (base `sprint-04`) was not created by the kickoff automation; run manually if desired: `gh pr create --draft --base sprint-04 --head sprint-05 --title "Sprint 5"`.

---

## Definition of Done for This Sprint

- [ ] S5-01 → S5-07 (Must Have) all completed and pass acceptance criteria
- [ ] Play Mode smoke: player takes damage → dies → `ON_PLAYER_DEATH` fires; enemy takes damage → dies → `ON_ENEMY_DEATH` fires → room's `ON_CLEAR_ENEMY` fires when last enemy dies
- [ ] Loading a room with an empty enemy list does not throw (BUG-ES-1 regression check)
- [ ] `EventID` contains `ON_PLAYER_DEATH`, `ON_ENEMY_DEATH`, `ON_ROOM_CLEAR`
- [ ] Carried WIP (`cb099ee` + stash) reconciled onto `sprint-05` — stash empty of sprint-05-relevant entries
- [ ] Working tree clean before end-of-sprint commit
- [ ] Carry-over decision recorded for Sprint 6
- [ ] `/qa-plan sprint` run before implementation proceeds past S5-01→S5-04 (see QA Plan Gate note)

---

## QA Plan

⚠️ **No QA Plan**: This sprint was opened without a QA plan (no `production/qa/qa-plan-sprint-05.md` found, and no interactive session available to run `/qa-plan sprint` during this automated kickoff). Run `/qa-plan sprint` before the last Must-Have story (S5-07) is implemented — the Production → Polish gate requires a QA sign-off report, which requires a QA plan. This is especially load-bearing this sprint given the death-chain and `EnemyManager` changes are exactly the kind of logic that needs explicit Logic/Integration test classification per `test-standards.md`.

---

## Next Sprint Outlook (Sprint 6 — tentative)

- **Runtime spawn wiring**: `EnemyManager.OnLoadMap()` → `GetHybridEnemySet` → spawn at `Tile_Spawn` points, contingent on S5-07 landing clean.
- **Room-clear UX**: lock doors on room enter, unlock on `ON_CLEAR_ENEMY` (event now exists as of S5-06/S5-07).
- **HUD**: `UIManager` health bar bound to player-damage events, now that BUG-06 (S5-01) makes those events real.
- **First full playtest session**: once player death, enemy death, and room-clear are all live — this has been "next sprint" for 2+ retros running.
