# Sprint 12 — 2026-08-31 to 2026-09-04

**Opened:** 2026-08-30 (Sunday 22:00 `pm-weekly-kickoff`, on-slot autonomous run — no owner present).
Branch `sprint-12`, created from `sprint-11` tip (`6348dc6`, "chore(wrapup): weekly wrap-up 2026-08-30"),
after `git fetch origin sprint-11` confirmed the local ref matched `origin/sprint-11` exactly. `gh` CLI
unavailable in this environment (`gh: command not found`) — draft PR (`--base sprint-11 --head sprint-12`,
title `Sprint 12`) was **not** auto-created; run manually if desired:
`gh pr create --draft --base sprint-11 --head sprint-12 --title "Sprint 12"`

**Review mode:** lean (from `production/review-mode.txt`) — producer feasibility gate (PR-SPRINT)
skipped per lean-mode rule.

**Note on working tree at kickoff:** `feature/fix-player-control` (the branch this run started on)
carried one uncommitted modification to
`Assets/Animation/Player/Sword and Shield/Knight_ComboAttack/Knight_ComboAttack_State1/Knight_ComboAttack_State1_dir 0.anim`
— asset files are outside this run's write scope. Stashed rather than discarded or carried:
`git stash` entry `"wip: uncommitted anim change on feature/fix-player-control before sprint kickoff"`
on that branch. Owner should `git stash list` / `git stash pop` on `feature/fix-player-control` to
recover it — not touched on `sprint-12`.

---

## Sprint Goal

**Fix the compile break (BUG-064) as the literal first commit, then re-verify last week's claimed-fixed
bugs against a build that actually compiles.** Sprint 11 closed FAIL specifically because the session that
finally moved the long-stuck enemy `TakeDamage()` chain (BUG-042+053) also deleted `EntityStatsSO.cs` and
`EntityFindTarget` without sweeping every caller — the project does not compile at `HEAD`
(`production/qa/bugs/BUG-064.md`). Nothing else this sprint — Play Mode verification, re-checking
BUG-042/043/044/046/033, the demo, QA — is possible until this lands. Immediately after, land the two
oldest pure-decision-avoidance items (S4-05/S4-06, ADR-0002 Accept) in a single owner sign-off pass, since
they have zero technical content blocking them and have carried 15+ and 10 cycles respectively.

---

## Capacity

- Total days: 5
- Buffer (20%): 1 day
- Available: 4 days

Must-Have load ≈ 1.1d — wider than Sprint 11's 0.75d because BUG-064 bundles five sub-fixes
(`production/qa/bug-triage-2026-08-30.md` recommended assignment, items a–e) that must land together to
restore a compiling build; splitting them across sessions would leave the project non-compiling in
between.

---

## Carryover from Previous Sprint

Full detail: `production/sprints/sprint-11.md` (closure block at top),
`production/retros/retro-sprint-11-2026-08-30.md`, `production/qa/bug-triage-2026-08-30.md`,
`production/qa/bugs/BUG-064.md`.

Sprint 11 closed **CLOSED — FAIL**: 2 of 5 Must-Have tasks fully met (S11-04/BUG-033, S11-05/BUG-044),
1 partial (S11-02), 2 not met (S11-01/BUG-063, S11-03). The FAIL verdict is driven by a build-breaking
regression discovered in code review, not by the Must-Have count alone. Verified directly against file
contents at this kickoff (not commit messages) — every item below is carried exactly as the Sprint 11
retro and bug-triage described it:

| Task | Reason | New Estimate |
|------|--------|-------------|
| **BUG-064** (project does not compile — `EntityData.cs`/`Entity.cs`/`EntityInput.cs`/`EntityAttack.cs`/`EntityEffectStats.cs` reference types/fields deleted this week: `EntityStatsSO`, `Entity.Data`, `EntityFindTarget`) | New this cycle, found by code review during Sprint 11 wrap-up. Blocks every other verification | 0.4d |
| S11-01 → S12-02 (BUG-063, `Stat.cs:63-65` `[SerializeField]` regression) | 18th+ consecutive carry on a one-line fix with a comment in the file explaining exactly what to do — deprioritized every session behind S11-02 | 0.05d |
| S11-03 → S12-05 (pre-push hook) | 14th carry, requires owner/producer action not agent time. Retro recommends a bare placeholder to stop the silent carry count | 0.15d |
| S11-06 → S12-06 (S4-05/S4-06 forced decision) | 15th carry, oldest unresolved item in the project (originally Sprint 4), zero movement any cycle | 0.1d |
| S11-08 → S12-07 (ADR-0002 → Accepted) | 10th carry, trivial sign-off-only change | 0.1d |
| S11-14 → S12-08 (DI/VContainer ADR for `LifetimeScope/`) | 2nd carry, now escalated — a *second* `ObjectPoolManager` implementation exists undocumented (`LifetimeScope/Service/PoolableService/` vs `Assets/Script/Poolable/`), actively compounding | 0.3d |
| S11-11 → S12-09 (individual `BUG-NNN.md` files) | 6th+ cycle, only 3 of 9 open P1 items have individual files | 0.2d |
| BUG-053/BUG-054 (enemy health routing, `EntityNegativeReciver` player-only logic) | Blocked (not just delayed) by BUG-064 — sequence directly after it lands | 0.2d |
| Re-verify BUG-042/043/044/046/033/NEW-1/NEW-2/NEW-3/NEW-4 | All "claimed fixed" against a build that did not compile — bug-triage explicitly says do not treat CLAUDE.md's tags as ground truth until re-checked with a working compile | 0.2d |
| doc-sync (CLAUDE.md BUG-053 status disagrees with code as of the same 2026-08-28 commit) | Flagged by both retro and triage | 0.1d |
| QA plan | 14th+ consecutive cycle missing | — see QA Plan Gate below |
| First playtest | Unreached, last log 2026-06-12 | — blocked on BUG-064 |

---

## Tasks

### Must Have (Critical Path)

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S12-01 | **Fix BUG-064 — restore a compiling build.** (a) Resolve `EntityData`/`Entity.Data`/`EntityStatsSO` dangling refs in `EntityData.cs:8`, `Entity.cs` (`LoadEntity()`, `LoadState()`, `SetDataEntity()`), `EntityInput.cs:81,83`, `EntityAttack.cs:68`, `EntityEffectStats.cs:20` — needs a 2-minute architecture confirmation (does `EntityData` get a new `Stats` SO field, or does `Entity.cs` get its `data`/`Data` pattern back with the SO type updated) before patching. (b) Remove/reimplement `EntityFindTarget` refs in `EntityInput.cs:57,61` (also NEW-1's unfinished half). (c) Fix `ON_ENEMY_DEATH` `Vector3`/`GameObject` cast mismatch in `EnemySpawner.ReleaseEnemy` and `ItemSpawner.DropItem` — pick one payload type, fix both subscribers. (d) Fix `EnemySpawner.OnDisable()`'s `Resgister`→`UnResgister` typo. (e) Wire `RangeWeapon.cs`'s `poolManager` via `[Inject] Construct(IObjecPoolService)` matching the `EnemySpawner`/`ItemSpawner` pattern, or an Inspector-resolvable fallback | lead-programmer / ai-programmer | 0.4 | None — must be the literal first commit | Unity Console shows zero compile errors; Play Mode smoke pass succeeds (enter `LoadRandomMap`, kill one enemy, fire the ranged weapon once) — per `test-standards.md` this is an Integration story, BLOCKING evidence required (PlayMode test or documented playtest) |
| S12-02 | Fix BUG-063 — remove the `#if UNITY_EDITOR` / `[SerializeField]` block on `Stat.cs:63-65` above `private List<StatModifier> modifiers`; field must stay bare, per the comment already in the file | gameplay-programmer | 0.05 | None | `Stat.modifiers` carries no `[SerializeField]` under any build symbol; a Play Mode session no longer leaves `PlayerStats.asset`/`Test.asset` modified after exit |
| S12-03 | Fix BUG-053/BUG-054 — `EntityNegativeReciver` still resolves `PlayerInputHandler` off `EntityCore` and the enemy health-routing chain needs confirmation against the now-compiling build | ai-programmer | 0.2 | S12-01 | Enemy takes damage and dies in Play Mode with no NRE; no `ON_PLAYER_DEATH` emitted from enemy code |
| S12-04 | Re-verify BUG-042/043/044/046/033/NEW-1/NEW-2/NEW-3/NEW-4 against the now-compiling build — do not trust CLAUDE.md's "FIXED" tags until confirmed live | qa-lead / lead-programmer | 0.2 | S12-01 | Each item's written acceptance criteria checked directly against current source + one Play Mode pass; CLAUDE.md updated to match actual verified state |
| S12-05 | Land the process gate as an **enforced pre-push hook**, even a bare placeholder (`exit 0` + TODO) — 14th carry, and its absence is exactly what let last week's build-breaking refactor land ungated | producer / owner (Kay) | 0.15 | None | A working `.git/hooks/pre-push` (or committed equivalent, documented as required) exists | 

Must-Have total ≈ **1.0d** (0.4+0.05+0.2+0.2+0.15).

### Should Have

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S12-06 | **Forced decision** — S4-05/S4-06 keep-or-cut, 15th carry, zero movement across every prior cycle | owner (Kay) | 0.1 | None | Written decision recorded in this file or a linked doc — make the call |
| S12-07 | Flip ADR-0002 (`EnemyManager` singleton) `Proposed → Accepted`, 10th carry | producer | 0.1 | None | ADR-0002 Status reads Accepted; sign-off note recorded |
| S12-08 | File an ADR for the VContainer/DI layer (`Assets/Script/LifetimeScope/`) — a second `ObjectPoolManager` now exists undocumented alongside `Assets/Script/Poolable/`, actively compounding | technical-director | 0.3 | None | ADR filed under `docs/architecture/`; decides which `ObjectPoolManager` implementation prefabs/scenes should reference |
| S12-09 | Batch-generate remaining individual `production/qa/bugs/BUG-NNN.md` reports — only 3 of 9+ open P1 items have files today | qa-lead | 0.2 | None | All open P1 items have individual files |
| S12-10 | Run `/doc-sync` — reconcile `CLAUDE.md`'s BUG-053 status (marked OPEN as of 2026-08-28) against the code that shipped the same day | lead-programmer | 0.1 | S12-04 | `CLAUDE.md` Known Bugs table matches verified current source state |
| S12-11 | Write first EditMode/PlayMode tests for the Entity damage chain while it's being touched for S12-01/S12-03 — `tests/EditMode/`/`tests/PlayMode/` have been `.gitkeep`-only for the entire project history (TD-014) | ai-programmer / qa-lead | 0.3 | S12-01 | At least one passing EditMode test for `EntityVitalStats.ReceiveReduction` / death threshold |

### Nice to Have

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S12-N1 | First full playtest session — once S12-01/S12-03 confirm stable | producer / owner | — | S12-01, S12-03 | `/playtest-report` filed — last log 2026-06-12 |
| S12-N2 | Decide which `ObjectPoolManager` implementation prefabs actually reference, delete the unused one | tools-programmer | 0.2 | S12-08 | One `ObjectPoolManager` implementation remains, all references confirmed live |
| S12-N3 | Fix Bug #14 — add `return` after `Destroy(gameObject)` in `MazeController.Awake()` | lead-programmer | 0.1 | None | Duplicate `MazeController` instance no longer overwrites `Instance` |

---

## Risks

| Risk | Probability | Impact | Mitigation |
|------|------------|--------|------------|
| BUG-064 fix introduces a new architecture decision (EntityData/Stats SO shape) without review, repeating the exact "deletion without a reference sweep" pattern that caused BUG-064 itself | High — same root cause fired twice in two weeks per retro's Systemic Trends #1 | High | Explicit "2-minute architecture confirmation before typing" called out in S12-01; recommend a full `grep` for any deleted symbol before considering the fix done |
| No owner-in-Editor session happens, so the Play Mode smoke gate on S12-01 goes unreached, blocking everything sequenced after it | Medium-High — same pattern that stalled S11-07 across 5 consecutive sprints | High | S12-01 is the sprint's sole hard blocker — flagged as needing an owner-present session same as prior sprints flagged S11-02 |
| S12-05 (pre-push hook) again stays undone — 14 consecutive carries at the exact same framing | High | Medium | Retro explicitly recommends a bare placeholder (`exit 0` + TODO) instead of a full solution, to stop the silent carry count |
| Trivial/decision-avoidance items (S12-02, S12-06, S12-07) lose every session to the larger BUG-064 fix again, repeating the exact pattern retro flagged for S11-01/S11-03 | High — named explicitly in retro's Process Improvements | Medium | Retro recommends scheduling the cheapest open items as a dedicated block at the very start of Day 1, before any other file is opened — carried into this sprint's daily plan |
| No QA plan — 14th+ consecutive cycle | Confirmed | Medium | Flagged explicitly below, deferred to owner per every prior cycle's handling |
| No Unity CLI / no `gh` CLI in this environment | Known constraint | Low | Play Mode smoke checks (S12-01, S12-03) are manual in-Editor by the owner; PR creation noted as a manual follow-up |

---

## Dependencies on External Factors

- No Unity CLI — the Play Mode smoke gate on S12-01 (and everything sequenced after it) requires manual
  in-Editor confirmation by the owner.
- `gh` CLI unavailable — draft PR for `sprint-12` (base `sprint-11`) not auto-created; run manually:
  `gh pr create --draft --base sprint-11 --head sprint-12 --title "Sprint 12"`.

---

## Definition of Done for This Sprint

- [ ] S12-01: BUG-064 fixed — Unity Console zero errors, Play Mode smoke pass confirmed (kill enemy, fire ranged weapon)
- [ ] S12-02: BUG-063 fixed — `Stat.modifiers` has no `[SerializeField]` under any build symbol
- [ ] S12-03: BUG-053/BUG-054 confirmed fixed against the compiling build
- [ ] S12-04: BUG-042/043/044/046/033 re-verified against the compiling build, CLAUDE.md corrected if any claim doesn't hold
- [ ] S12-05: Process gate landed as at least a placeholder enforced hook
- [ ] QA Plan gate resolved (see below)

Everything else (S12-06 through S12-11, all Nice-to-Have) is Should-Have — carried but not blocking
sprint close, consistent with prior sprints' Definition of Done scoping.

---

## QA Plan

⚠️ **No QA plan exists** for Sprint 12 (`production/qa/qa-plan-sprint-12.md` not found) — **14th+
consecutive sprint cycle** without one. This kickoff ran autonomously (no owner present); consistent
with every prior cycle's handling of this gate, the choice requiring judgment (full plan now vs. defer)
is deferred to the owner rather than decided unattended.

> ⚠️ This sprint was started without a QA plan. Run `/qa-plan sprint` before the last story is
> implemented. The Production → Polish gate requires a QA sign-off report, which requires a QA plan.
> Given S12-01 is a hard blocking compile fix and S12-03/S12-04 are Play Mode re-verification gates, a
> QA plan run early would meaningfully de-risk sign-off later — this recommendation has now repeated
> 14+ times.

---

## Next Sprint Outlook (Sprint 13)

- If Must-Have closes clean: first playtest (S12-N1, if not run here), BUG-062-class DI-migration
  follow-through, HUD health bar work (blocked on the death chain being confirmed stable — now finally
  possible once S12-01/S12-03 land).
- S12-06/S12-07 (owner sign-off pass) and S12-08 (DI ADR), if not completed here — architecture debt
  should not compound further without them.
- QA-plan process item, if not completed as Should-Have here — 15th+ cycle would be the next flag point.
- Between-room upgrade cards (`StatsSO.AddModifiersFromSource()`), once combat is confirmed stable in
  Play Mode for a full sprint running.
