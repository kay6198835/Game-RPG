## Retrospective: Sprint 11
Period: 2026-08-24 -- 2026-08-28 (scheduled), wrap-up run 2026-08-30
Generated: 2026-08-30 (Saturday weekly wrap-up, `pm-weekly-wrapup` scheduled task, autonomous)

### Metrics

| Metric | Planned | Actual | Delta |
|--------|---------|--------|-------|
| Must-Have tasks | 5 (S11-01..S11-05) | 2 fully met (S11-04, S11-05), 1 partial (S11-02), 2 not met (S11-01, S11-03) | -2.5 |
| Should-Have tasks | 9 (S11-06..S11-14) | 1 confirmed done (S11-13/BUG-062 DI migration) | -8 |
| Nice-to-Have tasks | 4 (S11-N1..N4) | 0 confirmed | -4 |
| Completion Rate (Must-Have, strict acceptance criteria) | -- | 40% (2/5) | -- |
| Story Points / Effort Days (Must-Have) | 0.75d | ~0.35d landed clean (S11-04+S11-05), S11-02 partial | -0.4d |
| Bugs Fixed this week (per CLAUDE.md 2026-08-28 sync) | -- | BUG-042, BUG-043, BUG-044, BUG-046, BUG-033, NEW-1, NEW-2 (7) | +7 |
| Bugs still open, verified against current source (not doc claims) | -- | BUG-063 (S11-01), pre-push gate (S11-03), S4-05/S4-06 decision, ADR-0002 still Proposed | -- |
| Commits (`de2ed0f..HEAD`) | -- | 22 | -- |

### Velocity Trend

| Sprint | Must-Have Planned | Must-Have Completed | Rate |
|--------|---------|-----------|------|
| Sprint 9 | 1.05d | BUG-041 + BUG-059 closed | first real close in 3 cycles |
| Sprint 10 | 1.0d | 0d | 0% |
| Sprint 11 (current) | 0.75d | ~2/5 items clean, 1 partial | ~40-50% |

**Trend**: Recovering. Sprint 11 is the first cycle in three where the sprint's own literal top-priority
item (S11-02, the enemy `TakeDamage()` chain / BUG-042+053+054) received real code movement instead of a
9th-12th consecutive zero. Per the daily plan, this happened specifically because Thursday night
(`eb8c9a0`, `f666e00`) saw an actual uninterrupted session touch `EntityCore.cs`, `EntityNegativeReciver.cs`,
and the new `EntityStatsHandler`/`EntityVitalStats` pair.

### What Went Well
- **Sprint 9 retro's Action Item #1 finally landed after 2 sprints at zero.** `EntityCore.TakeDamage()`
  no longer throws `NotImplementedException` (verified: `EntityCore.cs` now has no `TakeDamage` method at
  all — health routes through `EntityVitalStats` instead). `EntityNegativeReciver.cs` was rewritten
  (not literally deleted, but functionally consolidated) to call
  `EntityVitalStats.ReceiveReduction(StatType.HP, amount)` + `EntityInput.OnTakeDamage(pos)` — it no
  longer resolves `PlayerInputHandler` off `EntityCore` and no longer emits `ON_PLAYER_DEATH`. Verified
  directly: only one `INegativeReceiver` implementer remains on the enemy side
  (`EntityAttack.cs` only *calls* the interface, it doesn't implement it). **This means BUG-053 as
  described in `CLAUDE.md`'s Known Bugs table (still marked OPEN as of the 2026-08-28 doc-sync commit) is
  now stale — the underlying symptom appears fixed in code that landed the same day the doc was written.**
  Flagging for `/doc-sync` next run.
- S11-04 (BUG-033, `EnemySpawner.cs`) and S11-05 (BUG-044, `PlayerDeathState` construction) both closed
  clean and match their written acceptance criteria exactly — verified against current file content, not
  just commit messages.
- S11-13 (BUG-062, `StatsUIController.cs` DI migration) also appears done — no direct `StatsSO` field
  remains, only `IPlayerStatService`/`IObjecPoolService` via `Construct()`. This was Action Item #3 from
  the Sprint 10 retro; if confirmed, that's 2 of 5 Sprint 10 action items now closed.
- Two extra bugs (BUG-043, BUG-046) closed as a side effect of the entity-side rewrite — deleting
  `EntityWeaponMelee.cs` entirely rather than patching it, consistent with `weapon-skill-code.md`'s
  "MeleeWeapon.OnActivate() is the reference implementation" guidance.

### What Went Poorly
- **CRITICAL — the project does not currently compile at HEAD.** Confirmed by this week's code-review
  pass: `EntityData.cs:8` still references `EntityStatsSO`, deleted this week; `Entity.cs` had its
  `EntityData data` field and `Data` getter removed, but `LoadEntity()`, `LoadState()`,
  `SetDataEntity()`, and external callers (`EntityInput.cs:81,83`, `EntityAttack.cs:68`,
  `EntityEffectStats.cs:20`) still reference them; `EntityInput.cs:57,61` still references
  `EntityFindTarget`, also deleted this week with no replacement. This is at least 4 distinct compile
  errors introduced by the same entity-refactor session that fixed BUG-042/053. **Nothing else this
  cycle — S11-07 Play Mode verify, the demo, any smoke check — can even be attempted until this is
  fixed.** This supersedes the "recovering velocity" read below: functional logic improved, but the
  build itself regressed to broken. Filed as the top item in this cycle's bug triage.
- **Even past the compile break, the enemy-death chain would still crash on contact.**
  `EntityDeathState.cs` emits `ON_ENEMY_DEATH` with a `Vector3` position; both subscribers
  (`EnemySpawner.ReleaseEnemy`, `ItemSpawner.DropItem`) cast the event payload to `GameObject` —
  `InvalidCastException` on every single enemy death once the build compiles again. S11-02's fix is real
  but incomplete one step further down the chain than this retro's earlier draft credited it for.
- **`EnemySpawner.OnDisable()` calls `EventManager.Resgister` instead of `UnResgister`** for
  `ON_ENEMY_DEATH` — a direct violation of `.claude/rules/manager-event-code.md`'s pairing rule, causing
  duplicate subscriptions on every enable/disable cycle.
- **`RangeWeapon.cs` has a `[SerializeField]` interface field (`IObjecPoolService poolManager`)** with no
  `[Inject]` Construct method, unlike the correctly-migrated `EnemySpawner`/`ItemSpawner`. Unity cannot
  serialize an interface via Inspector, so this field is always null and ranged combat silently never
  fires — no exception, no log, just dead input. Inconsistent application of the new VContainer pattern
  introduced this week.
- **S11-01 (BUG-063) — the cheapest item in the entire backlog (0.05d) — is still open.** `Stat.cs:63-65`
  still has `#if UNITY_EDITOR` / `[SerializeField]` above `modifiers`, unchanged from kickoff. This is now
  an 18th+ consecutive-cycle carry on a one-line fix with a comment in the file itself explaining exactly
  what to do.
- **S11-03 (pre-push hook) did not land — 14th carry.** `.git/hooks/pre-push` still does not exist. This
  was Sprint 10 retro's Action Item #2, marked Critical, and it carried through Sprint 11 untouched again.
- **S11-02's letter-of-acceptance-criteria was not fully met**, even though the functional symptom looks
  fixed: the acceptance criteria explicitly said "delete `EntityNegativeReciver.cs`" — it was rewritten,
  not deleted. Harmless in outcome, but worth naming: a future review should verify this isn't hiding a
  subtler issue (e.g., duplicate legacy code paths elsewhere still referencing the old class shape).
- **Zero movement again on S11-06 (S4-05/S4-06 forced decision) — now a 15th carry**, the oldest
  unresolved item in the project (originally Sprint 4). No autonomous cycle can close this; it strictly
  requires the owner to make a call.
- **ADR-0002 still `Proposed`** (S11-08, 10th carry) despite `EnemyManager`'s role having been re-scoped
  and re-verified multiple times since the amendment.
- **Undocumented architecture kept growing**: `Assets/Script/LifetimeScope/` now contains a *second*
  `ObjectPoolManager`/`Pool`/`PoolMember` set (`LifetimeScope/Service/PoolableService/`) alongside the
  original `Assets/Script/Poolable/`. S11-14 (DI ADR) did not land. This is the same undocumented-subsystem
  pattern already tracked by BUG-052, now compounding with a live duplicate-implementation risk.
- **S11-11 (individual `BUG-NNN.md` files) did not progress** — still 3 of 9 P1 items filed
  (BUG-052, BUG-053, BUG-063).
- **No QA plan again — the pattern is now old enough that "Nth consecutive cycle" stopped being a useful
  counter; recommend the next kickoff treat this as a standing risk acceptance rather than a fresh flag.**

### Blockers Encountered

| Blocker | Duration | Resolution | Prevention |
|---------|----------|------------|------------|
| No owner-present session on Tue 2026-08-25 | 1 full day | None — daily standup logged "zero dev commits, no session appears to have happened" | Same recurring gap; no automated fix possible |
| No Unity CLI in this environment | All of Sprint 8-11 | S11-07 Play Mode verify again unreached (5th consecutive sprint) | Requires an owner-in-Editor session; cannot close autonomously |
| No pre-push gate | All of Sprint 11 | S11-03 not landed again | Land even a placeholder hook — recommended 3 sprints running now |

### Estimation Accuracy

| Task | Estimated | Actual | Variance | Likely Cause |
|------|-----------|--------|----------|--------------|
| S11-01 (BUG-063) | 0.05d | 0d landed | -0.05d (100% short) | Deprioritized every session in favor of S11-02, despite being flagged "land it first" twice in the daily plan |
| S11-02 (BUG-042/053/054) | 0.3d | ~0.25d landed (functional fix, literal file-delete step skipped) | Close to estimate once the Thursday session actually happened | 11 prior cycles at zero meant the real work was compressed into one late session |
| S11-04 (BUG-033) | 0.1d | 0.1d, landed clean | 0 | — |
| S11-05 (BUG-044) | 0.15d | 0.15d, landed clean (across two partial commits) | 0 | — |
| S11-03 (pre-push hook) | 0.15d | 0d landed | -0.15d (100% short) | Producer/owner-only task, no autonomous path to close it |

**Overall estimation accuracy**: 2 of 5 Must-Have tasks (S11-04, S11-05) landed exactly on estimate; S11-02
landed functionally close to estimate but missed a literal acceptance-criteria clause; S11-01 and S11-03
(the two cheapest, most mechanical items) are the ones that stalled — the bottleneck is not sizing, it's
that trivial-but-not-urgent items keep losing every session to whatever else is in flight.

### Carryover Analysis

| Task | Original Sprint | Times Carried | Reason | Action |
|------|----------------|---------------|--------|--------|
| S11-01 / BUG-063 | Sprint 10 (NEW-4 regression) | 18th+ cycle | Consistently deprioritized behind S11-02 despite being the cheapest item in the backlog | Should be the literal first commit of Sprint 12 — nothing else should be attempted first |
| S11-03 / pre-push hook | Sprint 6 (S9-00) | 14th carry | Requires owner/producer action, not agent time | Land a minimal placeholder (even a no-op `exit 0` script with a TODO) to stop the silent carry count, then iterate |
| S4-05/S4-06 decision | Sprint 4 | 15th carry | Decision-avoidance — no technical blocker | Force a one-line written decision in Sprint 12's kickoff file regardless of owner availability that week |
| ADR-0002 → Accepted | Sprint 9 (S10-09) | 10th carry | Low urgency, no consequence for staying Proposed | Batch with the S4-05/S4-06 decision as a single "owner sign-off" pass |
| Individual `BUG-NNN.md` files | Sprint 7 | 6th+ cycle | Consistently deprioritized behind code work | qa-lead to batch-generate remaining 6 files in one session — flagged again this cycle in the parallel bug-triage report |
| DI/VContainer ADR (S11-14) | Sprint 10 | 2nd carry | New architecture keeps landing (now a 2nd `ObjectPoolManager` implementation) faster than it gets documented | Escalate: recommend this blocks further `LifetimeScope/` work until filed, per `engine-code.md`'s "Core.cs and EntityCore.cs are the ONLY permitted component hubs" concern about competing DI/hub patterns |

### Technical Debt Status
- TODO/FIXME/HACK inline comments: not tracked in this codebase by convention (per Sprint 10 retro) —
  `production/qa/bugs/` + bug-triage report remain the real debt ledger.
- **New concern this cycle**: two coexisting `ObjectPoolManager` implementations
  (`Assets/Script/Poolable/ObjectPoolManager.cs` vs
  `Assets/Script/LifetimeScope/Service/PoolableService/ObjectPoolManager.cs`). Full duplication risk
  assessment deferred to the parallel code-review pass (`production/qa/bug-triage-2026-08-30.md` and this
  run's code-review notes) — needs a decision on which one prefabs/scenes actually reference before
  Sprint 12.
- `CLAUDE.md`'s Known Bugs table is showing early drift again: BUG-053 marked OPEN as of its own
  2026-08-28 sync commit, but the code landed that same day appears to have fixed the described symptom.
  Recommend `/doc-sync` run before Sprint 12 kickoff.

### Previous Action Items Follow-Up (from Sprint 10 retro)

| Action Item (from Sprint 10 retro) | Status | Notes |
|---|---|---|
| 1. Open Sprint 11 with BUG-042/BUG-053/BUG-054 as the ONLY Must-Have item until it lands | **Partially Followed** | Sprint plan sequenced it as task #2 (after the 0.05d BUG-063 fix), not the sole item — but it was prioritized correctly in practice per the daily standups, and it landed functionally this cycle |
| 2. Land a minimal pre-push hook, no further deferral | **Not Started** | 14th carry, `.git/hooks/pre-push` still absent |
| 3. Finish `StatsUIController.cs` DI migration (BUG-062) | **Done** | Verified: no direct `StatsSO` reference remains, only DI-injected services |
| 4. Decide whether the sprint-planning process needs to change re: in-flight branch work | **Not Explicitly Decided** | No written decision found, but this sprint's plan did account for the prior week's in-flight state more explicitly at kickoff |
| 5. Force the S4-05/S4-06 decision | **Not Started** | 15th carry |

**2 of 5 action items from the previous retrospective were completed or meaningfully followed** — an
improvement over Sprint 10's 0/6, but the two hardest-to-avoid items (pre-push gate, the forced decision)
remain exactly where they were.

### Action Items for Next Iteration

| # | Action | Owner | Priority | Deadline |
|---|--------|-------|----------|----------|
| 0 | **FIX THE COMPILE BREAK before anything else** — `EntityData.cs`/`Entity.cs`/`EntityInput.cs`/`EntityAttack.cs`/`EntityEffectStats.cs` reference types/fields deleted this week (`EntityStatsSO`, `Entity.Data`, `EntityFindTarget`). Then fix the `Vector3`→`GameObject` cast crash on `ON_ENEMY_DEATH` and the `Resgister`/`UnResgister` mismatch in `EnemySpawner.OnDisable()`. Nothing else in Sprint 12 can be verified in-Editor until the project builds again | ai-programmer / gameplay-programmer | **Blocking** | Sprint 12, before any other task |
| 1 | **Land S11-01/BUG-063 as the literal first commit of Sprint 12** — one line, 18+ cycles carried, no technical blocker exists | gameplay-programmer | Critical | Sprint 12, Day 1 |
| 2 | **Land the pre-push hook as a bare placeholder if nothing else** — even `exit 0` with a TODO stops the 14-cycle carry and gives something to iterate on | Owner (Kay) / producer | Critical | Sprint 12 kickoff |
| 3 | **Force the S4-05/S4-06 decision AND flip ADR-0002 to Accepted in the same owner sign-off pass** — both are pure decision-avoidance items with no remaining technical content | Owner (Kay) | High | Sprint 12 kickoff |
| 4 | **File an ADR for the VContainer/DI layer before any more `LifetimeScope/` code lands** — a second `ObjectPoolManager` implementation now exists undocumented; this is actively compounding, not just stale | technical-director | High | Sprint 12, Day 1-2 |
| 5 | **Run `/doc-sync` to reconcile `CLAUDE.md`'s BUG-053 status against the code that shipped 2026-08-28** — the doc and the code disagree as of the same commit | lead-programmer | Medium | Sprint 12 kickoff |
| 6 | Batch-generate the remaining 6 `BUG-NNN.md` files (S11-11) in one qa-lead session | qa-lead | Medium | Sprint 12, Day 1-2 |

### Process Improvements
- **A daily standup that verifies against file contents (not commit messages) caught a real doc/code
  disagreement this cycle** (BUG-053) — this verification habit is working as intended and should
  continue.
- **Trivial, zero-dependency items (S11-01, S11-03) are structurally the ones that never win a session**,
  even when explicitly flagged "land it first" — because they're never urgent enough to interrupt
  whatever larger item is mid-flight. Recommend Sprint 12 schedule the two cheapest open items as a
  dedicated 10-minute block at the very start of Day 1, before any other file is opened.
- **Acceptance criteria should distinguish "functionally fixed" from "criteria met verbatim."** S11-02 is
  a good example: the symptom is gone, but "delete the file" as written was not literally done. Future
  story acceptance criteria should separate the observable behavior requirement from any specific
  implementation instruction, so partial-but-correct fixes aren't ambiguous to grade.

### Summary
Sprint 11 shows real, verifiable Must-Have logic progress after two sprints of zero movement on the
enemy damage chain — but it closes with **the project not compiling at HEAD**, which overrides the
positive read. The same entity-refactor session that finally fixed BUG-042/053's health routing also
deleted `EntityStatsSO`/`Entity.Data`/`EntityFindTarget` without updating every caller, and even past that
break the enemy-death event chain has a live cast-exception bug and a duplicate-subscription bug behind
it. Two other Must-Have items closed clean (BUG-033, BUG-044) and a confirmed DI migration (BUG-062)
carried Sprint 10's action item through — genuine wins. But the trivial-item and decision-avoidance
backlog (BUG-063, the pre-push hook, S4-05/S4-06) is now 14-18+ cycles deep with zero technical blocker on
any of them. **Sprint 12 must open with the compile break as a hard blocking task before anything else is
attempted** — Play Mode verification, the demo, and every other bug's true status are all unknowable until
the build works again — and should clear the cheap backlog items immediately after, since leaving them
open is now costing more attention in retros and triage than fixing them ever would.
