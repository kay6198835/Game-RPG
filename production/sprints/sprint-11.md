# Sprint 11 — 2026-08-24 to 2026-08-28

**Opened:** 2026-08-24 (Sunday 22:00 `pm-weekly-kickoff`, on-slot autonomous run — no owner present).
Branch `sprint-11`, created from `sprint-10` tip (`de2ed0f`, "chore(wrapup): weekly wrap-up 2026-08-22"),
after `git fetch origin sprint-10` confirmed the local ref matched `origin/sprint-10` exactly (process
carried from `retro-sprint-09-2026-08-15.md` — never branch off an unfetched local ref). `gh` CLI
unavailable in this environment (`gh: command not found`) — draft PR (`--base sprint-10 --head sprint-11`,
title `Sprint 11`) was **not** auto-created; run manually if desired:
`gh pr create --draft --base sprint-10 --head sprint-11 --title "Sprint 11"`

**Review mode:** lean (from `production/review-mode.txt`) — producer feasibility gate (PR-SPRINT)
skipped per lean-mode rule.

**Note on working tree:** `Assets/SO/Stat/PlayerStats.asset` carried onto this branch with an
uncommitted modification (pre-existing at kickoff time, not made by this run — asset files are outside
this run's write scope per hard constraint). Flagging for owner review; `git diff` on it returned no
line-level changes despite `git status` showing modified, which itself is worth a look (possible
line-ending or metadata-only touch) before assuming it is unrelated to the BUG-063 serialization
regression below.

---

## Sprint Goal

**Land BUG-063 first (one line, highest risk-to-cost ratio in the backlog), then give S10-01/BUG-042+053+054
(enemy `TakeDamage()` chain) a single dedicated, uninterrupted session** — it has now received zero code
movement for 8 consecutive standup cycles across two full sprints (Sprint 9 + Sprint 10), and every
prior retro's Action Item #1 says the same thing: distributed autonomous check-ins are not enough
pressure to move it. This sprint opens with both as the literal first two tasks, in that order.

---

## Capacity

- Total days: 5
- Buffer (20%): 1 day
- Available: 4 days

Must-Have load ≈ 0.75d — narrower than Sprint 10's 1.0d. Sprint 10 closed 0/6 Must-Have despite a
similarly narrow scope; widening further would repeat the overcommitment pattern without first proving
a session can be protected for S11-02.

---

## Carryover from Previous Sprint

Full detail: `production/sprints/sprint-10.md` (closure block at top),
`production/sprints/sprint-10-daily-plan.md` (Status Verdict + Carry-Over Watch List),
`production/retros/retro-sprint-10-2026-08-22.md`, `production/qa/bug-triage-2026-08-22.md`.

Sprint 10 closed **CLOSED — FAIL**: 0 of 6 Must-Have tasks completed, 1 of 6 Should-Have completed
(landed ungated). 0 of 6 action items from Sprint 9's retro were completed. Verified directly against
file contents at this kickoff (not commit messages) — every item below is still open exactly as
described:

| Task | Reason | New Estimate |
|------|--------|-------------|
| BUG-063 (`Stat.cs:63-65`, `[SerializeField]` reintroduced under `#if UNITY_EDITOR`) | Confirmed still present at this kickoff — regression undoing the `NEW-4`/`f5de65a` fix. Cheapest, highest-risk item in the backlog per Sprint 10's own closing recommendation | 0.05d |
| S10-01 → S11-02 (`EntityCore.TakeDamage()` chain, BUG-042+053+054) | Confirmed still `throw new System.NotImplementedException();` at `EntityCore.cs:11`; `EntityNegativeReciver.cs` still present, not deleted. **9th consecutive cycle, 0 movement across two full sprints** | 0.3d |
| S10-02 → S11-03 (process gate, enforced pre-push hook) | Confirmed still no `.git/hooks/pre-push` (only `.sample`). 10th carry | 0.15d |
| S10-04 → S11-04 (BUG-033, `EnemySpawner.cs:62` null-guard order) | Confirmed still `set.Count == 0 \|\| set == null` (wrong order) at this kickoff. 13th carry | 0.1d |
| S10-05 → S11-05 (BUG-044, `PlayerDeathState.LogicUpdate()` orphaned) | Confirmed still fully commented out at this kickoff (lines 17-24); state still never constructed in `Player.Awake()`. 9th carry | 0.15d |
| S10-06 → S11-06 (S4-05/S4-06 forced decision) | 12th carry, zero movement any cycle — decision-avoidance, not estimation | 0.1d |
| S10-03 → S11-07 (Play Mode verify gate) | Unreached 4 consecutive sprints (S7-08, S8-12, S9-12, S10-03). Depends on S11-02 | 0.2d |
| S10-09 → S11-08 (ADR-0002 → Accepted) | Still `Status: Proposed`. 9th carry, trivial | 0.1d |
| S10-07 → S11-09 (BUG-043 consolidation) | 6th carry, Should-Have, depends on S11-02 | 0.3d |
| S10-08 → S11-10 (Bug #6, player HP write-through) | 12th carry, Should-Have | 0.4d |
| S10-10 → S11-11 (individual `BUG-NNN.md` files) | Should-Have, only 3 of 9 P1 items filed (`BUG-052`, `BUG-053`, `BUG-063`) | 0.2d |
| S10-12 → S11-12 (BUG-046, `OverlapCircle`→`OverlapCircleNonAlloc`) | Should-Have, independent, quick | 0.15d |
| BUG-062 (`StatsUIController.cs` mid-migration, old `StatsSO` pattern vs new DI service) | New finding from Sprint 10 close, unaddressed | 0.2d |
| VContainer/DI ADR retrofit | Sprint 10 landed a full DI layer (`Assets/Script/LifetimeScope/`, `IPlayerStatService`) with no governing ADR | 0.3d |
| First playtest | 12th consecutive cycle attempt, gated on S11-02 + S11-07 | — |
| QA plan | 11th consecutive cycle missing | — see QA Plan Gate below |

---

## Tasks

### Must Have (Critical Path)

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S11-01 | Fix BUG-063 — remove the `#if UNITY_EDITOR` / `[SerializeField]` block reintroduced on `Stat.cs:63-65` above `private List<StatModifier> modifiers`; field must stay bare (no attribute at all, per the comment already in the file explaining why) | gameplay-programmer | 0.05 | None | `Stat.modifiers` carries no `[SerializeField]` under any build symbol; a Play Mode session no longer leaves `PlayerStats.asset`/`Test.asset` modified after exit |
| S11-02 | Fix BUG-042 + BUG-053 + BUG-054 together — implement `EntityCore.TakeDamage()` for real (health decrement via `EntityStatsSO`, death-state hookup); **delete** `EntityNegativeReciver.cs` rather than patching it. **Must be the first task attempted in the first session this sprint, before any other work, per two consecutive retros' Action Item #1** | ai-programmer | 0.3 | S11-01 | Player hit on enemy in Play Mode: no `NotImplementedException`; health decrements; exactly one `INegativeReceiver` implementer exists on the enemy prefab; no `ON_PLAYER_DEATH` emitted from enemy code |
| S11-03 | Land the process gate as an **enforced pre-push hook** (not another written-policy draft) — 10th carry, and its absence let Sprint 10's S10-11 WIP-reconciliation land ungated | producer / owner (Kay) | 0.15 | None | A working `.git/hooks/pre-push` (or committed equivalent script, documented as required) exists and actually blocks a failing push |
| S11-04 | Fix BUG-033, 13th carry — `EnemySpawner.cs:62` swap to `set == null \|\| set.Count == 0` | gameplay-programmer | 0.1 | None | Empty-pool room spawns without `NullReferenceException` |
| S11-05 | Fix BUG-044, 9th carry — restore `PlayerDeathState.LogicUpdate()` body and construct `PlayerDeathState` in `Player.Awake()` | gameplay-programmer | 0.15 | None | `PlayerDeathState` emits on death; no commented-out logic left; state reachable from `Player.Awake()` |

Must-Have total ≈ **0.75d** (0.05+0.3+0.15+0.1+0.15).

### Should Have

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S11-06 | **Forced decision** — S4-05/S4-06 keep-or-cut, 12th carry, zero movement across every prior cycle | owner (Kay) | 0.1 | None | Written decision recorded in this file or a linked doc — make the call, do not carry a 13th time |
| S11-07 | **Verify** — Play Mode smoke check: (a) player melee damage lands, (b) enemy `TakeDamage()` chain works end to end (S11-02's fix, live), (c) `statusAnimation` buffer-gate through a full combo | owner (Kay) / lead-programmer | 0.2 | S11-02 | Owner confirms in-Editor; 5th attempt after S7-08/S8-12/S9-12/S10-03 all went unreached |
| S11-08 | Flip ADR-0002 (`EnemyManager` singleton) `Proposed → Accepted`, 9th carry | producer | 0.1 | None | ADR-0002 Status reads Accepted; sign-off note recorded |
| S11-09 | Fix BUG-043 — consolidate `EntityAttack.cs` and `EntityWeaponMelee.cs` into one enemy damage path | ai-programmer | 0.3 | S11-02 | One clear enemy-attack code path; cooldown gate functionally gates repeat attacks |
| S11-10 | Bug #6 / S7-11, 12th carry — write-through `NegativeReciver`→`PlayerData.currentHealth`, confirm `ON_PLAYER_DEATH` listener fires, EditMode test `TakeDamage_BelowZero_TriggersDeathState` | gameplay-programmer | 0.4 | None | Test passes; single HP source of truth; `Reborn()` contract intact |
| S11-11 | File individual `production/qa/bugs/BUG-NNN.md` reports for remaining P1 items — only `BUG-052`/`BUG-053`/`BUG-063` exist today | qa-lead | 0.2 | None | All open P1 items have individual files |
| S11-12 | Fix BUG-046 — `EntityWeaponMelee.cs:29` allocating `Physics2D.OverlapCircle` → `OverlapCircleNonAlloc` | gameplay-programmer | 0.15 | None | Matches `engine-code.md`'s zero-alloc rule, cached buffer |
| S11-13 | Fix BUG-062 — finish `StatsUIController.cs` migration to the new `IPlayerStatService` DI pattern, remove the mixed old/new access | ui-programmer | 0.2 | None | `StatsUIController.cs` uses only the DI service, no direct `StatsSO` reach-through |
| S11-14 | Write an ADR for the VContainer/DI layer landed in Sprint 10 (`Assets/Script/LifetimeScope/`, `IPlayerStatService`) — currently undocumented architecture | technical-director | 0.3 | None | ADR filed under `docs/architecture/`, Status Accepted or Proposed with rationale |

### Nice to Have

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S11-N1 | Fix Bug #14 — add `return` after `Destroy(gameObject)` in `MazeController.Awake()` | lead-programmer | 0.1 | None | Duplicate `MazeController` instance no longer overwrites `Instance` |
| S11-N2 | Fix Bug #15 — replace `File.ReadAllText(Application.dataPath...)` with `TextAsset` refs or StreamingAssets | lead-programmer | 0.5 | None | Room load works from a Player build |
| S11-N3 | First full playtest session — once S11-02/S11-07 land | producer / owner | — | S11-02, S11-07 | `/playtest-report` filed — 12th cycle attempt |
| S11-N4 | Review the `PlayerStats.asset` uncommitted diff found at this kickoff — confirm it isn't a live symptom of BUG-063 before S11-01 lands | owner (Kay) | 0.05 | None | Diff explained (stale WIP, line-ending noise, or actual leak) and resolved |

---

## Risks

| Risk | Probability | Impact | Mitigation |
|------|------------|--------|------------|
| S11-02 (BUG-042/053/054) receives zero movement a 9th consecutive cycle | High — 8 consecutive cycles at zero so far, worst-yet pattern | High | Sequenced as literal task #2 (after the 0.05d BUG-063 fix) in the very first session; no other Must-Have item scheduled ahead of it except the cheap regression fix |
| No owner-in-Editor session happens, so S11-07 (Play Mode verify) goes unreached a 5th consecutive sprint | Medium-High — pattern across S7-08/S8-12/S9-12/S10-03 | High | Flagged as owner-only in the task table; depends on S11-02 landing first |
| S11-03 (process gate) stays a written-policy-only version instead of an enforced hook | High — 10th consecutive carry at this exact framing | Medium | Explicitly scoped as "enforced hook, not another draft" — same wording used and missed in Sprint 10 |
| Unrelated StatSystem/UI/tooling work again absorbs all session time, repeating Sprint 10's pattern (0d Must-Have movement across 4 of 5 scheduled days) | High — this exact pattern closed Sprint 10 | High | Sprint Goal explicitly names "before any other work" for S11-02; recommend the owner block the first session solely on S11-01+S11-02 |
| No QA plan — 11th consecutive cycle | Confirmed | Medium | Flagged explicitly below; deferred to owner rather than silently dropped |
| No Unity CLI / no `gh` CLI in this environment | Known constraint | Low | Play Mode smoke checks (S11-07) are manual in-Editor by the owner; PR creation noted as a manual follow-up |

---

## Dependencies on External Factors

- No Unity CLI — Play Mode smoke checks require manual in-Editor confirmation by the owner (S11-07).
- `gh` CLI unavailable — draft PR for `sprint-11` (base `sprint-10`) not auto-created; run manually:
  `gh pr create --draft --base sprint-10 --head sprint-11 --title "Sprint 11"`.

---

## Definition of Done for This Sprint

- [ ] S11-01: BUG-063 fixed — `Stat.modifiers` has no `[SerializeField]` under any build symbol
- [ ] S11-02: Enemy `TakeDamage()` no longer throws; single `INegativeReceiver` implementer on enemy prefab; `EntityNegativeReciver.cs` deleted
- [ ] S11-03: Process gate landed as an enforced check, not just a written note
- [ ] S11-04 / S11-05: BUG-033, BUG-044 confirmed fixed
- [ ] S11-06: S4-05/S4-06 decision recorded
- [ ] QA Plan gate resolved (see below)

Everything else (S11-07 Play Mode verify, BUG-043, Bug #6, ADR-0002, BUG-062, DI ADR, BUG-046) is
Should-Have — carried but not blocking sprint close, consistent with prior sprints' Definition of Done
scoping. Note: S11-07 is listed Should-Have here only because it structurally depends on S11-02 landing
first and cannot be guaranteed within the window — if S11-02 lands early, treat S11-07 as effectively
Must-Have for the remainder of the week.

---

## QA Plan

⚠️ **No QA plan exists** for Sprint 11 (`production/qa/qa-plan-sprint-11.md` not found) — **11th
consecutive sprint cycle** without one. This kickoff ran autonomously (no owner present); per the QA
plan gate, the choice requiring judgment (full plan now vs. defer) is deferred to the owner rather than
decided unattended, consistent with every prior cycle's handling of this same gate.

> ⚠️ This sprint was started without a QA plan. Run `/qa-plan sprint` before the last story is
> implemented. The Production → Polish gate requires a QA sign-off report, which requires a QA plan.
> Given S11-07 is an explicit Play Mode verification gate and S11-10 is EditMode-test-gated, a QA plan
> run early would meaningfully de-risk sign-off later — this recommendation has now repeated 11 times.

---

## Next Sprint Outlook (Sprint 12)

- If Must-Have closes clean: BUG-043 consolidation (if not done as Should-Have here), Bug #6 (if not
  done here), the enemy-death → room-clear condition confirmation in Play Mode, `EnemyManager`
  lifecycle body review, first playtest (if not already run).
- Bug #14/#15 and QA-plan process items, if not completed as Should-Have here.
- HUD health bar + between-room upgrade cards, once the death chain (both directions) is confirmed
  stable in Play Mode.
- DI/VContainer ADR retrofit (S11-14), if not completed here — architecture debt should not compound
  further without it.
