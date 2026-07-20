# Sprint 6 — 2026-07-21 to 2026-07-25

**Status: OPEN.** Branch `sprint-06`, created from `sprint-05` tip (`cc543ba`) at 2026-07-20 22:00
kickoff. `gh` CLI unavailable in this environment — draft PR (`--base sprint-05 --head sprint-06`,
title `Sprint 6`) was **not** auto-created; run manually if desired.

## Sprint Goal

Close the combat death loop and spawn-stabilization pillars carried from Sprint 5 (0% landed on
Track B, partial on Track C), verify no build break from Thu's pooling-system work, and resolve the
two open architecture questions (`EnemyModal` SO-vs-plain-class, ADR-0002/0003 status) blocking
further spawn-system work. This is a **stabilization sprint** — no new features — so Sprint 7 can
start clean on `EnemyManager` lifecycle + room-clear.

---

## Capacity

- Total days: 5
- Buffer (20%): 1 day
- Available: 4 days

Must-Have load ≈ 2.4d — comfortable fit against 4 days, leaving room for Should-Have carry cleanup.
Deliberately light after Sprint 5 closed at 34% Must-Have — prioritizing actually landing the death
chain over adding new scope.

---

## Carry-Over From Sprint 5

| Item | Type | Priority | Origin |
|------|------|----------|--------|
| BUG-05/07/08 — enemy death chain (`EntityMoveState` NRE, `EntityDeathState` base class, `EntityBasicState` empty death block) | Bug (S1) | P1 | 7th carry cycle |
| BUG-ES-1/ES-4 — spawn null-guard / index-guard | Bug | P1 | 2nd–3rd carry |
| **NEW-1** — `RoomModel.GetSpawnSet()` infinite-loop risk if any `EnemyModal.weight == 0` | Bug (S1, new) | P0 | Found in 2026-07-20 wrap-up code review |
| **Possible build break** — `PoolMember.cs:9` `[SerializeField]` on an auto-property (CS0592 pattern), unverified against a real Unity compile | Risk (new) | P0 | Found in 2026-07-20 wrap-up code review — **verify first, before any other task** |
| BUG-06 "Done" is a partial fix — `NegativeReciver` doesn't write through to `PlayerData.currentHealth`, breaks `Reborn()` reset contract | Bug (reopened) | P1 | Found in 2026-07-20 wrap-up code review |
| `EnemyModal` SO-vs-plain-class regression — no owner decision yet | Decision | P1 | 3rd carry (flagged Tue/Wed/Fri of Sprint 5) |
| ADR-0002 (`EnemyManager` singleton) Proposed→Accepted | Decision | P2 | 3rd carry |
| ADR-0003 (Option C spawn design) still Proposed despite S5-A2 marked Done | Decision | P2 | New this close-out |
| S5-C3 — dedupe spawn driver (`EnemySpawner` vs `LevelManager.SpawnRoomEnemies()`) | Task | P2 | 2nd carry |
| S5-D3 — `CancelInvoke` pairing (`PlayerInputHandle.cs:264`) | Task | P2 | 3rd carry |
| S5-D4 — quick cleanup batch (BUG-AH-2/EM-2/WM-2/LM-1) | Task | P2 | 3rd carry |
| S4-05/S4-06 — carried tasks needing an explicit keep-or-cut call | Decision | P3 | 4th/3rd carry |
| First playtest — 6th cycle with zero movement | Milestone | P1 | Tied to death-chain landing |
| `origin/feature/spawn-enemy` 2 commits ahead of old `sprint-05` (`dce9be1`/`d653654`) | Verify | P1 | Must confirm merged into `sprint-06` |

---

## Tasks

### Must Have (P0/P1)

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S6-00 | **Verify branch parity** — confirm `sprint-06` includes `origin/feature/spawn-enemy`'s `dce9be1`/`d653654` (`RoomGeneraterController.cs`/`RoomGridController.cs`); merge/cherry-pick if missing | lead-programmer | 0.1 | None | `git log --oneline sprint-06 \| grep -E "dce9be1\|d653654"` (or equivalent content check) confirms both present |
| S6-01 | **Verify `PoolMember.cs:9` compiles** — open Editor, confirm no CS0592 on the `[SerializeField]` auto-property; fix if it does (backing field or drop the attribute) | lead-programmer | 0.1 | None | Console shows zero compile errors after Editor reload |
| S6-02 | Fix **NEW-1** — `RoomModel.GetSpawnSet()` hang risk when `EnemyModal.weight == 0`; clamp/guard so a zero-weight entry can't stall the retry loop | gameplay-programmer | 0.25 | None | Room with a 0-weight `EnemyModal` in its pool spawns without hanging Play Mode |
| S6-03 | Fix **BUG-05** — `EntityMoveState.LogicUpdate()`: move `if (entity.Input.Target == null)` guard to the top, before the `Vector2.Distance`/position dereference | ai-programmer | 0.25 | None | Guard is first statement; losing target mid-chase → Idle, no NRE |
| S6-04 | Fix **BUG-07** — rewrite `EntityDeathState` to extend `EntityState` (not `MonoBehaviour`); wire into `EntityStateMachine` | ai-programmer | 0.5 | S6-03 | `EntityDeathState : EntityState`; compiles; reachable via transition |
| S6-05 | Fix **BUG-08** — fill `EntityBasicState`'s empty `Health <= 0` block → transition to `EntityDeathState`; emit `ON_ENEMY_DEATH` once on entry | ai-programmer | 0.25 | S6-04 | Enemy at 0 HP → death state in Play Mode; `ON_ENEMY_DEATH` fires exactly once per death |
| S6-06 | Fix **BUG-ES-1** — `RoomModel.GetSpawnSet()` return an empty list, not `null`, on an empty pool; guard both driver call sites | gameplay-programmer | 0.25 | None | Empty enemy pool → no `NullReferenceException` in either driver |
| S6-07 | Fix **BUG-ES-4** — guard `EnemySpawner.SpawnRoomEnemies()` line ~60 (`spawnPosition[Random.Range(...)]`) against an empty `spawnPosition` list | gameplay-programmer | 0.1 | S6-06 | Empty spawn-position list → no `IndexOutOfRangeException`; warning logged instead |
| S6-08 | Fix **BUG-06 partial-fix** — `NegativeReciver.TakeDamage()` write-through to `PlayerData.currentHealth` (not a local/duplicate HP field), so `Reborn()` resets the same source of truth | gameplay-programmer | 0.25 | None | `PlayerData.currentHealth` is the single value mutated by damage and restored by `Reborn()`; no dual HP state |
| S6-09 | **Decision** — `EnemyModal` SO-vs-plain-class: resolve whether it should stay a plain `[System.Serializable]` class (current) or return to an `EntityModel`-derived SO asset (Mon-locked Sprint-5 data model). Record the call and reconcile ADR-0003 with it. | producer / lead-programmer | 0.15 | None | Written decision + rationale recorded (this file or an ADR-0003 addendum); no code change required this sprint unless the decision reverts the regression |

Must-Have total ≈ **2.20d** (see individual ests: 0.1+0.1+0.25+0.25+0.5+0.25+0.25+0.1+0.25+0.15 = 2.20d)

### Should Have (P2/P3)

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S6-D1 | Flip ADR-0002 (`EnemyManager` singleton) `Proposed → Accepted` | producer | 0.1 | None | ADR-0002 Status reads Accepted; sign-off note recorded |
| S6-D2 | Flip ADR-0003 (Option C spawn design) `Proposed → Accepted`, reconciled against the S6-09 decision | producer / lead-programmer | 0.1 | S6-09 | ADR-0003 Status reads Accepted (or explicitly stays Proposed with a documented blocking reason) |
| S6-D3 | S5-C3 — dedupe spawn driver: pick canonical (`EnemySpawner` event-driven vs `LevelManager.SpawnRoomEnemies()` Editor button); delete the losing path | lead-programmer | 0.5 | S6-06 | Exactly one spawn driver remains in `Assets/Script/`; duplicate deleted, not left dead |
| S6-D4 | S5-D3 — `PlayerInputHandle.cs:264` `Invoke(nameof(ChangeIsTakeDamage))` paired with `CancelInvoke` in `OnDisable` | gameplay-programmer | 0.25 | None | Every `Invoke`/`InvokeRepeating` has a matching `CancelInvoke` in `OnDisable` |
| S6-D5 | S5-D4 — cleanup batch (BUG-AH-2 dead `using UnityEngine.UIElements;`, BUG-EM-2 dead `using System.Numerics;`, BUG-WM-2 dup `lastClickTime` assign, BUG-LM-1 dead null-check on non-nullable int) | gameplay-programmer | 0.25 | None | All 4 grep-confirmed clean; no behavior change |
| S6-D6 | **Decision** — S4-05/S4-06 keep-or-cut call (4th/3rd carry); if cut, remove from tracker; if kept, re-estimate into Sprint 7 | producer | 0.1 | None | Explicit written decision, no more silent re-carry |

### Nice to Have

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S6-N1 | First full playtest session — once S6-03→S6-08 (death chain + spawn guards) land | producer / owner | — | S6-03..S6-08 | `/playtest-report` filed; first session since `playtest-2026-06-12` |
| S6-N2 | Start `EnemyManager` lifecycle body (alive-count, lock/unlock, `ON_CLEAR_ENEMY`/`ON_ROOM_CLEAR` emit) — only if Must-Have closes early | ai-programmer | 1.0 | S6-05, S6-09 | Stretch only; defer cleanly to Sprint 7 if not started |

---

## Definition of Done for This Sprint

- [ ] Branch parity verified (`origin/feature/spawn-enemy` commits present in `sprint-06`)
- [ ] `PoolMember.cs` compile risk verified/fixed — zero Console errors
- [ ] NEW-1 hang risk fixed — no zero-weight stall in `GetSpawnSet()`
- [ ] Death chain: Play Mode — enemy takes damage → dies → `EntityDeathState` reached → `ON_ENEMY_DEATH` fires once
- [ ] Spawn stabilization: empty-pool room and empty spawn-position list both load without throwing
- [ ] BUG-06 fully closed — single HP source of truth, `Reborn()` contract intact
- [ ] `EnemyModal` SO-vs-plain-class decision recorded; ADR-0002 and ADR-0003 status resolved
- [ ] Carry-over decision recorded for Sprint 7 if anything slips
- [ ] QA Plan gate resolved (see below)

---

## QA Plan

⚠️ **No QA Plan exists** — `production/qa/qa-plan-sprint-06.md` does not exist, and none has existed
for **3 consecutive sprint cycles** (`qa-plan-sprint-05.md` was never created either). This gate has
been silently re-carried each cycle; flagging explicitly again rather than repeating that pattern.

**Recommended next action**: run `/qa-plan sprint` before implementation starts — the death-chain
fixes (S6-03→S6-05) and the `Reborn()` write-through fix (S6-08) are exactly the Logic/Integration
surface `test-standards.md` requires explicit test classification for. This sprint was opened
autonomously (scheduled kickoff, no user present) — the QA plan gate is deferred to the owner's next
session rather than run unattended, since it involves judgment calls about test scope.

> ⚠️ This sprint was started without a QA plan. Run `/qa-plan sprint` before the last story is
> implemented. The Production → Polish gate requires a QA sign-off report, which requires a QA plan.

---

## Risks

| Risk | Probability | Impact | Mitigation |
|------|------------|--------|------------|
| Off-plan work recurs a 5th time (4 consecutive days in Sprint 5) | Medium-High | High | Sprint scope deliberately narrow (2.2d Must-Have vs 4d capacity) — if off-plan work recurs, the Must-Have list is small enough that even a partial week should close it. Flag explicitly at each standup if new unplanned architecture work appears. |
| `EnemyModal` decision (S6-09) blocks ADR-0003 flip (S6-D2) and any further spawn-system work | Medium | Medium | Decision task is P1 Must-Have, scheduled early in the week |
| No QA plan — 3rd consecutive cycle | Confirmed | Medium | Flagged explicitly above; not silently dropped |
| `PoolMember.cs` build break unverified | Unknown (untested) | High if real | First task Monday — verify before anything else builds on top of it |
| No Unity CLI in this environment | Known constraint | Low | All Play Mode smoke checks are manual in-Editor by the owner |

---

## Dependencies on External Factors

- No Unity CLI — Play Mode smoke checks require manual in-Editor confirmation.
- `gh` CLI unavailable — draft PR for `sprint-06` (base `sprint-05`) not auto-created; run manually:
  `gh pr create --draft --base sprint-05 --head sprint-06 --title "Sprint 6"`.

---

## Next Sprint Outlook (Sprint 7)

- `EnemyManager` lifecycle → room-clear: doors lock on entry, alive-count via `ON_ENEMY_DEATH`,
  unlock + `ON_CLEAR_ENEMY`/`ON_ROOM_CLEAR` at zero alive (per ADR-0002, pending S6-D1 acceptance).
- Placement polish (round-robin/entry-safety/jitter) + author `Tile_Spawn_Enemy` markers into the
  other 12 room JSONs.
- HUD health bar + between-room upgrade cards, consuming Sprint 5/6's death/room-clear events.
- First full playtest, if not already run in S6-N1.
