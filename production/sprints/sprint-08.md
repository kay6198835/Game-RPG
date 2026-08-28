# Sprint 8 — 2026-08-03 to 2026-08-07

**Status: CLOSED — CONCERNS (2026-08-09 Sunday kickoff, autonomous close).** No `/weekly-wrapup`
commit exists for Sat 2026-08-08 (checked: no `chore(wrapup)` commit, no `retro-sprint-08-*.md` file) —
this kickoff run closes the sprint directly from the Fri 2026-08-07 standup state, byte-identical to
last observed (`git log sprint-08` shows no commits after `a29895b` "daily standup 2026-08-07").

**Final scorecard — 0 of 8 Must-Have tasks (S8-01–S8-07, S8-12) landed on `sprint-08` across all 5
scheduled days:**

| Item | Final status |
|------|--------------|
| S8-00 root-cause conversation | ❌ Never held — 5th unaddressed cycle (3 in Sprint 6-7 lineage + 3 this sprint). Escalation condition from the sprint's own risk table ("if it recurs anyway, escalate to a hard process gate") was met as of 2026-08-06 and never actioned. |
| S8-01 / BUG-041 (player attack unwired) | ❌ OPEN — `MeleeWeapon.Attack()` still empty; `MakeDamage()` still commented out |
| S8-02 / BUG-042 (enemy TakeDamage throws) | ❌ OPEN — `EntityCore.TakeDamage()` still `throw NotImplementedException` |
| BUG-053 (new, merge-introduced 2026-08-06) | ❌ OPEN — `EntityNegativeReciver.cs` duplicate calls wrong hub, emits `ON_PLAYER_DEATH` on enemy death |
| S8-03 / BUG-043 (divergent enemy attack paths) | ⚠️ PARTIAL — `EntityAttack.cs.nextAttackTime` now advances; two paths still unconsolidated |
| S8-04 / BUG-044 (PlayerDeathState orphaned) | ❌ OPEN, untouched |
| S8-05 / Bug #6 (8th carry, write-through HP) | ❌ Not started this sprint at all |
| S8-06 / BUG-032 (one-line fix) | ❌ OPEN, untouched |
| S8-07 / BUG-033 (one-line fix) | ❌ OPEN, untouched |
| S8-12 (Play Mode verify gate) | ❌ Not reached — blocked on S8-01/02 |
| S8-10 (ADR-0002 Accepted) | ❌ Still **Proposed** — verified directly, not flipped |
| S8-11 (S4-05/S4-06 forced decision) | ❌ Zero movement — 8th carry this sprint, becomes 9th |
| QA Plan | ❌ Still none — 8th consecutive cycle without one |
| `origin/feature/enemy-control` merge | ✅ Resolved 2026-08-06 (`c5f26b1`) — brought real AI/pathfinding progress but introduced BUG-053 |

**Root cause (this run's read, not a restated observation):** the sprint's only structural
countermeasure against off-plan work was a conversation requiring the owner in the room, scheduled 6
times total (Sprint 6, 7, and 3x within Sprint 8) and held 0 times. A conversation that depends on
synchronous owner availability is not a countermeasure an autonomous PM cycle can force to happen —
it needs to be replaced by something enforceable without the owner present.

**Carried forward to Sprint 9 (all Must-Have items, unchanged in substance):** BUG-041, BUG-042,
BUG-053, BUG-043 (partial), BUG-044, Bug #6/S7-11 (9th carry), BUG-032 (3rd carry), BUG-033 (6th
carry), ADR-0002 flip (5th carry), S4-05/S4-06 decision (9th carry), QA plan (8th cycle), S8-12
verification gate.

**Process recommendation for Sprint 9 (superseding the root-cause-conversation approach):** replace
the 6th scheduled conversation with an enforceable gate — e.g. a pre-push hook or PR check that runs
a compile check before any merge to `sprint-09`, or a written rule that AI/pathfinding work happens
only on a tracked, separately-scoped branch/epic rather than landing via drive-by merge. This is a
recommendation for the owner to adopt, not something this run can enforce unilaterally.

---


**Opened:** 2026-08-02 Sunday 22:00 kickoff (autonomous scheduled run). Branch `sprint-08`, created
from `sprint-07` tip (`7cc1f75`, includes the sprint-07 closure commit). `gh` CLI unavailable in this
environment — draft PR (`--base sprint-07 --head sprint-08`, title `Sprint 8`) was **not**
auto-created; run manually if desired:
`gh pr create --draft --base sprint-07 --head sprint-08 --title "Sprint 8"`

**Review mode:** lean (from `production/review-mode.txt`) — producer feasibility gate (PR-SPRINT)
skipped per lean-mode rule.

---

## Sprint Goal

**Recovery sprint — restore basic combat function, then stop the off-plan-work pattern before any new
scope starts.** Sprint 7 closed CONCERNS: the component-hub refactor is now structurally sound
(BUG-024/025/027/029/030/031 confirmed fixed), but combat regressed to non-functional in **both**
directions by the sprint's own last day — player attacks are never invoked (BUG-041) and the enemy
damage receiver throws `NotImplementedException` on every hit (BUG-042, now duplicated in a second
class). No other verification (death chain, room-clear, playtest) is possible until these two land.
This is also the **3rd consecutive sprint** with unplanned Pathfinding/hub work landing outside the
sprint's stated scope — the root-cause conversation (S7-D4) was scheduled twice and held zero times.
It is scheduled again here as the sprint's first item, not its last.

---

## Capacity

- Total days: 5
- Buffer (20%): 1 day
- Available: 4 days

Must-Have load ≈ 1.75d — deliberately light, mirroring Sprint 7's own (unmet) assumption that a narrow
bug-fix scope would be "comfortable." The gap this time is the root-cause conversation running as
Monday's first task, before any code work, specifically to test whether that changes the outcome.

---

## Carry-Over From Sprint 7

Full detail: `production/sprints/sprint-07-daily-plan.md` (Closure section) and
`production/retros/retro-sprint-07-2026-08-02.md`.

| Item | Type | Priority | Origin |
|------|------|----------|--------|
| BUG-041 — `WeaponMelee.Attack()` never called (`AnimationPlayerController.Attack()` empty stub) — player deals zero damage | Bug (S1, regressed) | P0 | New/regressed, Sprint 7 close |
| BUG-042 — `EntityCore.TakeDamage()` throws `NotImplementedException`; duplicate `EntityNegativeReciver.TakeDamage()` does the same | Bug (S1, new) | P0 | New, Sprint 7 close |
| BUG-043 — `EntityAttack.cs` and `EntityWeaponMelee.cs` are two divergent, only-partially-wired damage paths for enemies | Bug (S1, new) | P1 | New, Sprint 7 close |
| BUG-044 — `PlayerDeathState.LogicUpdate()` body fully commented out, orphaned from Bug #6's write-through fix | Bug (S1, new) | P1 | New, Sprint 7 close — fold into S8-05 |
| Bug #6 / S7-11 — `NegativeReciver.currentHealth` still a separate field from `PlayerData.currentHealth`; no `ON_PLAYER_DEATH` listener; no EditMode test | Bug (S1, 8th carry) | P0 | 8th carry |
| BUG-032 / S7-09 — `EntityWeaponMelee.cs:26` `input` field assignment still commented out — enemy skill NullRef | Bug (S1, 2nd carry, one-line fix) | P1 | 2nd carry |
| BUG-033 / S7-10 — `EnemySpawner.cs:62` null-check order still wrong (`set.Count == 0 \|\| set == null`) | Bug (S1, 5th carry, one-line fix) | P1 | 5th carry |
| Bug #14 — `MazeController.Awake()` missing `return` after `Destroy(gameObject)` | Bug (S1, carried) | P2 | Carried, untouched |
| Bug #15 — `File.ReadAllText(Application.dataPath...)` breaks Player builds | Bug (S1, carried) | P2 | Carried, untouched |
| ADR-0002 (`EnemyManager` singleton) Proposed→Accepted | Decision | P2 | 4th carry |
| S4-05/S4-06 keep-or-cut call | Decision | P2 | 8th carry — force this cycle, no further silent carry |
| Off-plan-work root-cause conversation | Process | **P0** | 3rd scheduling, 0 times held — run **first**, Monday AM |
| S7-D3 — individual `production/qa/bugs/BUG-NNN.md` files | Process | P3 | Not started |
| QA plan | Risk | P1 | 7 consecutive cycles with none |
| First playtest | Milestone | P2 | 9th cycle attempt, gated on BUG-041/042 |

---

## Tasks

### Must Have (P0/P1)

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S8-00 | **Hold off-plan-work root-cause conversation** — producer-facilitated, written outcome with a concrete process change (e.g., pre-push compile+smoke check, or moving "enemy control" work to its own tracked epic instead of drive-by merges) | producer | 0.1 | None | Written root-cause doc exists; concrete process change named, not a restated observation. **Owner action required — cannot be fully autonomous; if unheld by Wed standup, escalate as the sprint's top risk.** |
| S8-01 | Fix BUG-041 — wire `WeaponMelee.Attack()` call site: call `weaponHolder.Weapon.Attack()` from `AnimationPlayerController.Attack()` (or the correct anim-trigger site), mirroring `EntityAttackState` | gameplay-programmer | 0.2 | None | Player melee attack in Play Mode registers a hit via `OverlapCircleNonAlloc` + `INegativeReceiver.TakeDamage()` |
| S8-02 | Fix BUG-042 — implement `EntityCore.TakeDamage()` for real (health decrement + `ON_ENEMY_DEATH`/death-state hookup, mirroring `NegativeReciver.cs`'s pattern); delete or finish the duplicate `EntityNegativeReciver.cs` — do not leave both | ai-programmer | 0.2 | None | Player hit on enemy in Play Mode: no `NotImplementedException`; health decrements; exactly one `INegativeReceiver` implementer on the enemy prefab |
| S8-03 | Fix BUG-043 — consolidate `EntityAttack.cs` and `EntityWeaponMelee.cs` into one enemy damage path (or explicitly document why both exist); allocate `EntityAttack.hitBuffer` as a real array, not `[]`; fix `nextAttackTime` to actually advance after an attack | ai-programmer | 0.3 | S8-02 | One clear enemy-attack code path; `hitBuffer` non-empty; cooldown gate functionally (not just structurally) gates repeat attacks |
| S8-04 | Fix BUG-044 — restore `PlayerDeathState.LogicUpdate()` body (emit `ON_PLAYER_DEATH`/`ON_REALOAD_GAME`) | gameplay-programmer | 0.15 | None | `PlayerDeathState` emits on death; no commented-out logic left in place of a real fix |
| S8-05 | **Bug #6 / S7-11, 8th carry** — write-through `NegativeReciver`→`PlayerData.currentHealth` (single HP source of truth, drop the separate int field), confirm `ON_PLAYER_DEATH` listener fires (fold in S8-04), EditMode test `TakeDamage_BelowZero_TriggersDeathState` | gameplay-programmer | 0.4 | S8-04 | Test passes; single HP source of truth; `Reborn()` contract intact; listener confirmed firing — **do not close this without the test**, per Sprint 7's explicit warning against opportunistic fixes |
| S8-06 | Fix BUG-032, 2nd carry — `EntityWeaponMelee.cs:26` uncomment `Core.GetCoreComponent(out input)` | ai-programmer | 0.1 | None | Enemy skill use in Play Mode: no NullReferenceException |
| S8-07 | Fix BUG-033, 5th carry — `EnemySpawner.cs:62` swap to `set == null \|\| set.Count == 0` | gameplay-programmer | 0.1 | None | Empty-pool room spawns without NullReferenceException |
| S8-12 | **Verify** — Play Mode smoke check: player deals damage to enemy AND enemy deals damage to player, both without exceptions | lead-programmer | 0.2 | S8-01, S8-02, S8-03 | Owner confirms in-Editor; this is the sprint's real Definition of Done gate, learned from Sprint 7's S7-08 never being confirmed |

Must-Have total ≈ **1.75d** (0.1+0.2+0.2+0.3+0.15+0.4+0.1+0.1+0.2) — light on purpose; the sprint goal
is recovery, not new scope. S8-00 is sequenced first, ahead of any code task.

### Should Have (P2/P3)

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S8-08 | Fix Bug #14 — add `return` after `Destroy(gameObject)` in `MazeController.Awake()` | lead-programmer | 0.1 | None | Duplicate `MazeController` instance no longer overwrites `Instance` or re-runs the generator |
| S8-09 | Fix Bug #15 — replace `File.ReadAllText(Application.dataPath...)` with `TextAsset` refs or StreamingAssets for room JSON load | lead-programmer | 0.5 | None | Room load works from a Player build, not just Editor |
| S8-10 | Flip ADR-0002 (`EnemyManager` singleton) `Proposed → Accepted`, 4th carry | producer | 0.1 | None | ADR-0002 Status reads Accepted; sign-off note recorded |
| S8-11 | **Forced decision** — S4-05/S4-06 keep-or-cut, 8th carry, zero movement any cycle | producer | 0.1 | None | Written decision recorded; removed from tracker if cut, re-estimated into Sprint 9 if kept — **make the call, do not re-carry a 9th time** |
| S8-D1 | Process change — file individual `production/qa/bugs/BUG-NNN.md` reports (S7-D3, still not started) | qa-lead | 0.2 | None | At least BUG-041/042/043/044 have individual files |

### Nice to Have

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S8-N1 | First full playtest session — once S8-01/02/03/12 land | producer / owner | — | S8-01, S8-02, S8-03, S8-12 | `/playtest-report` filed; first session since `playtest-2026-06-12`, 9th cycle attempt |
| S8-N2 | Start `EnemyManager` lifecycle body (aliveCount, `ON_CLEAR_ENEMY`/`ON_ROOM_CLEAR`) — only if Must-Have closes early | ai-programmer | 1.0 | S8-12, S8-10 | Stretch only; defer cleanly to Sprint 9 if not started |

---

## Definition of Done for This Sprint

- [ ] Off-plan-work root-cause conversation held with a written, concrete outcome (S8-00)
- [ ] Player attack deals damage to enemy in Play Mode, verified (BUG-041)
- [ ] Enemy `TakeDamage()` no longer throws; single `INegativeReceiver` implementer on enemy prefab (BUG-042)
- [ ] Enemy attack path consolidated and functional (BUG-043)
- [ ] Bug #6 closed with a passing EditMode test, not an opportunistic fix (S8-05)
- [ ] BUG-032, BUG-033 confirmed fixed in Play Mode
- [ ] S8-12 Play Mode gate actually confirmed by the owner (unlike S7-08, never confirmed across all of Sprint 7)
- [ ] ADR-0002 status resolved to Accepted
- [ ] S4-05/S4-06 forced decision recorded
- [ ] QA Plan gate resolved (see below)

---

## QA Plan

⚠️ **No QA plan exists** for Sprint 8 (`production/qa/qa-plan-sprint-08.md` not found) — **7th
consecutive sprint cycle** without one. This kickoff ran autonomously (no user present); per the QA
plan gate, the choice requiring judgment (full plan now vs. defer) is deferred to the owner rather
than decided unattended.

> ⚠️ This sprint was started without a QA plan. Run `/qa-plan sprint` before the last story is
> implemented. The Production → Polish gate requires a QA sign-off report, which requires a QA plan.
> Given S8-05 is EditMode-test-gated and S8-12 is a Play Mode verification gate, a QA plan run early
> would meaningfully de-risk sign-off later — this recommendation has now repeated 7 times.

---

## Risks

| Risk | Probability | Impact | Mitigation |
|------|------------|--------|------------|
| Off-plan work recurs a 4th time (3 consecutive prior cycles, 2 skipped root-cause conversations) | High | High | S8-00 scheduled first, before any code task, specifically to test whether sequencing it earlier changes the outcome. If it recurs anyway, escalate to a hard process gate (e.g., branch protection / pre-push check) next cycle rather than a 4th conversation. |
| BUG-041/042/043 fixes uncover further combat-path issues not caught by static review | Medium | High | S8-12 is an explicit Play-Mode verification gate on both attack directions, not just "compiles" |
| Bug #6 fails a 4th time (regressed twice already) | Medium | Medium | Re-scoped with a mandatory EditMode test (S8-05), same as Sprint 7's approach — if it slips again, escalate to a dedicated spike per the Sprint 7 carry-over watch list |
| No QA plan — 7th consecutive cycle | Confirmed | Medium | Flagged explicitly above; deferred to owner rather than silently dropped |
| No Unity CLI in this environment | Known constraint | Low | All Play Mode smoke checks (S8-12 and combat verification) are manual in-Editor by the owner |

---

## Dependencies on External Factors

- No Unity CLI — Play Mode smoke checks require manual in-Editor confirmation by the owner.
- `gh` CLI unavailable — draft PR for `sprint-08` (base `sprint-07`) not auto-created; run manually:
  `gh pr create --draft --base sprint-07 --head sprint-08 --title "Sprint 8"`.

---

## Next Sprint Outlook (Sprint 9)

- If Must-Have closes clean: first playtest (S8-N1, if not already run), `EnemyManager` lifecycle
  (S8-N2, if not already started), resume Pathfinding correctness/perf work only once combat is
  confirmed stable end-to-end.
- Bug #14/#15 and S7-D3/QA-plan process items, if not completed as Should-Have here.
- HUD health bar + between-room upgrade cards, once the death chain (both directions) is confirmed stable.
