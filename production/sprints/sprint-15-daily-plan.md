# Sprint 15 — Daily Plan & Progress Tracker

> **Sprint**: 2026-09-21 (Mon) -> 2026-09-25 (Fri)
> **Companion to**: `sprint-15.md` — day-by-day breakdown + live tracker
> **Routines**: Mon-Fri 10:00 `/daily-standup` · Sat 22:00 `/weekly-wrapup` · Sun 22:00 `/weekly-kickoff`
> **Opened**: 2026-09-20 (Sunday 22:00 kickoff, autonomous). Branch `sprint-15` from `sprint-14` tip (`15242e6`).
> Sprint 14 closed FAIL (0/9 Must-Have). See `sprint-14.md`, `retro-sprint-14-2026-09-19.md`.

---

## Opening Block (checked first at every standup, before any other diff)

| # | Task | Est. | Status |
|---|------|------|--------|
| 1 | S15-01 BUG-063 | 0.05d | NOT STARTED (re-verified 2026-09-23, `Stat.cs:63-65` still `#if UNITY_EDITOR [SerializeField]`) |
| 2 | S15-02 BUG-064 item 7 | 0.1d | NOT STARTED (`RangeWeapon.cs:7` still `[SerializeField] private IObjecPoolService poolManager`, no `[Inject]`) |
| 3 | S15-03 BUG-065 | 0.1d | NOT STARTED (`PlayerDeathState.Enter()` still only `base.Enter()`) |
| 4 | S15-04 BUG-066 + BUG-070 | 0.2d | NOT STARTED (both `currentStats[statType]` indexers still unguarded, 7+7 sites) |

Combined 0.45d. Rule: no feature merge into `sprint-15` until all four are committed.
⚠️ **Rule broken 3 days running (Mon/Tue/Wed).** 8 commits landed on `sprint-15` since the Monday
standup — none of them the opening block. Risk table in `sprint-15.md` ("Opening block loses to
off-plan work a 5th time") has materialized, except the off-plan work itself was justified
(BUG-088 was a compile break — nothing else could be verified until it was fixed).

## Day-by-Day Plan

### Mon 2026-09-21 — Opening block + decisions
| Task | Est. | Status | Notes |
|------|------|--------|-------|
| S15-01, S15-02, S15-03 | 0.25d | ❌ NOT STARTED (confirmed EOD) | Three isolated commits — never landed |
| S15-04 | 0.2d | ❌ NOT STARTED (confirmed EOD) | Guard both vitals components — never landed |
| S15-06 / S15-07 decisions | 0.2d | ❌ NOT WRITTEN | No decision text in `sprint-15.md` |

### Tue 2026-09-22 — Abilities v2 S2 bugs (actual: off-plan work instead)
| Task | Est. | Status | Notes |
|------|------|--------|-------|
| S15-05 BUG-073 | 0.15d | ❌ NOT STARTED | GUID `ac9ac7c0...` on `ShootSpirit.asset` still resolves to 0 files |
| S15-08 BUG-071 | 0.15d | ❌ NOT STARTED | `RecoveryPerTime`/`ReductionPerTime` in `VitalComponent.cs` still never `StartCoroutine`d |
| S15-09 hook placeholder | 0.1d | ❌ NOT STARTED | No `.git/hooks/pre-push` |
| *(off-plan, not on the tracker)* | — | ✅ done | BUG-088 compile break fixed (`723fab1`/`2a83469`), BUG-075/072/089 (v2 damage path) fixed and **committed** `8b23174`, BUG-076/077/082/085/091 closed. See `production/qa/open-issues-2026-09-22.md` |

### Wed 2026-09-23 — Summon damage + first test
| Task | Est. | Status | Notes |
|------|------|--------|-------|
| S15-10 BUG-072 | 0.2d | ✅ CODE DONE (2a83469/8b23174, 2026-09-22) | `LightningController.Execute()` now does `OverlapCircleNonAlloc` + assign + `base.Execute()` per target. Residual: `layerMask` serializes as `0` (Nothing) — needs one Inspector set on `Lightning.prefab`, owner-in-Editor only |
| S15-12 first EditMode test | 0.3d | BLOCKED | Dep S15-04 not started; also blocked project-wide by BUG-084 (no `.asmdef` under `Assets/`) |

### Thu 2026-09-24 — Decisions and docs
| Task | Est. | Status | Notes |
|------|------|--------|-------|
| S15-11 TD-040 ADR | 0.3d | NOT STARTED | |
| S15-13 sign-off pass | 0.15d | NOT STARTED | |

### Fri 2026-09-25 — Doc sync + buffer
| Task | Est. | Status | Notes |
|------|------|--------|-------|
| S15-14 `/doc-sync` | 0.2d | NOT STARTED | |
| Nice-to-Have S15-N1..N3 | 0.3d | NOT STARTED | Buffer day |

---

## Standup Log

### Mon 2026-09-21 - Daily Standup (autonomous, no owner present)

Branch `sprint-15` (HEAD `d08a7d1`). Zero commits since the Sunday kickoff; working tree clean. The
weekend window (Sat 18:44 -> Mon) had no code activity.

**Opening Block re-verified against source: 0/4.**
- S15-01 BUG-063 - `Stat.cs:63-65` still `#if UNITY_EDITOR [SerializeField]` - NOT STARTED
- S15-02 BUG-064 item 7 - `RangeWeapon.cs` still no `[Inject]` - NOT STARTED
- S15-03 BUG-065 - `PlayerDeathState.Enter()` still only `base.Enter()` - NOT STARTED
- S15-04 BUG-066+070 - no `TryGetValue` in `EntityVitalStats.cs` or `VitalComponent.cs` - NOT STARTED

**Today (Mon), estimates:**

| Task | Est. | Complexity / Risk |
|------|------|-------------------|
| S15-01, S15-02, S15-03 (3 isolated commits) | 0.25d | Low; pattern exists (`ItemSpawner.cs:8-10`) |
| S15-04 guard both vitals components | 0.2d | Low-Med; 2 files, ~7 sites |
| S15-06 / S15-07 written decisions | 0.2d | Owner-dependent |

Blockers: none technical; S15-06/07 need owner sign-off. Risks: opening block loses to feature work a 5th
time; `feature/fix-player-control` remains the source of off-plan merges; ~230 binary assets merged last
week (size/LFS decision pending, S15-N3); `gh` unavailable so the sprint-15 draft PR is still not created.

---

### Wed 2026-09-23 - Daily Standup (autonomous, no owner present)

Branch `sprint-15`, HEAD `85bd612` (local branch fast-forwarded to match; it had drifted 8 commits
behind the actual working HEAD on `origin/feature/fix-player-control` since Monday — corrected this
run, no history rewritten). Working tree has one uncommitted, non-code change:
`Assets/SO/Skill/Paladin/Ability/Consecrate/Consecrate.asset` (`ActivationType: 0 -> 1`, Active ->
Hold) — left as-is, not a standup action item, flagged below for owner awareness.

**Yesterday (Tue 2026-09-22) — assessed against both the plan and source:**
- Planned (S15-05/08/09): 0/3 landed.
- Off-plan, but necessary and high-value: **BUG-088** (project did not compile — `Random.range` /
  `trasnform.postion` typos in `EntityMovement.SetPositionToCheck()`) fixed in `723fab1`/`2a83469`,
  restoring Play Mode capability project-wide. Same push: **BUG-072** and **BUG-075** (the two
  independent gaps blocking all Abilities v2 damage) fixed and committed `8b23174`; **BUG-089**
  closed as deliberate scaffolding (`549b35e`, reopen trigger recorded `85bd612`); **BUG-076/077/
  082/085/091** closed. Full evidence: `production/qa/open-issues-2026-09-22.md`.
- Net effect: real progress, but the sprint's own hard-gate rule (opening block before any other
  merge) was not honored a 3rd straight day, and none of Tuesday's actual planned tasks moved.

**Opening Block re-verified against source: still 0/4** (see table above, checked line-by-line
2026-09-23).

**Today (Wed 2026-09-23), estimates:**

| Task | Est. | Complexity / Risk |
|------|------|-------------------|
| S15-01 BUG-063 | 0.05d | Trivial — delete 3 lines. Zero deps, zero risk. Should ship first, alone |
| S15-02 BUG-064-7 | 0.1d | Low — pattern already proven at `ItemSpawner.cs:8-10` |
| S15-03 BUG-065 | 0.1d | Low — resolve `PlayerMovement` via `Core.GetCoreComponent`, call `Stop()`/zero velocity in `Enter()` |
| S15-04 BUG-066+070 | 0.2d | Low-Med — 2 files, 7 sites each, mechanical `TryGetValue` swap, but touch both player and entity paths so worth a careful pass |
| S15-05 BUG-073 | 0.15d | Low code / Med process — GUID relink needs an owner-in-Editor step (Inspector drag), can't be done headless |
| S15-08 BUG-071 | 0.15d | Low-Med — same file as S15-04, do together; also fix the `count`/`duration` mix-up noted in the bug file, not just add `StartCoroutine` |
| S15-09 hook placeholder | 0.1d | Trivial |
| BUG-072 Inspector step | ~5min | Owner-only — set `layerMask` on `Lightning.prefab` in Unity Editor; code side is done |

Reordered priority for today: **S15-01 -> S15-02 -> S15-03 -> S15-04 -> S15-08 -> S15-05 -> S15-09**,
opening block strictly first per the sprint's own rule, since it is now 3 days overdue.

**Blockers:**
- S15-12 (first EditMode test) blocked twice over: needs S15-04, and needs BUG-084 (no `.asmdef`
  anywhere under `Assets/`) resolved first — that is a precondition, not a task, and nothing this
  sprint addresses it.
- S15-06 (Play Mode decision) and S15-07 (review-gate decision) still need owner/producer sign-off;
  neither can be closed autonomously.
- No `gh` CLI in this environment — sprint-15 draft PR still not opened (carried since kickoff).

**Risks:**
- Opening block is now the single most-carried item in the sprint's own history (BUG-063 sits at
  29th+ total carry across sprints) and has missed its own hard gate 3 days into a 5-day sprint —
  at this rate Sprint 15 repeats Sprint 14's 0/9 Must-Have close unless it lands tomorrow.
- BUG-072's Inspector step is a single point of failure for all summon-type v2 abilities and needs
  an owner Unity session — same class of risk as S14-07's 8-sprint-carried Play Mode ask.
- Uncommitted `Consecrate.asset` change (Active -> Hold) is in-progress, undocumented design intent —
  if it lands without a matching effect-asset review it risks a new BUG-083-class defect (HoldTime/
  HoldRatio are still always `0f`, so a Hold-type Consecrate would charge nothing).

---

_(appended by `/daily-standup`)_

## Carry-over Watchlist

- BUG-063 29th+ carry; BUG-064-7 / 065 / 066+070 5th carry — **all 4 now also 3 days overdue within Sprint 15 itself**
- BUG-073 / BUG-071 / hook placeholder — planned Tue, 0/3, now 1 day overdue within Sprint 15
- S14-07 Play Mode: 8th sprint — decision required (S15-06); BUG-072's Inspector step adds a second, smaller Editor-only ask
- QA plan 32nd+ cycle — owner decision
- TD-040 ADR (v1 vs v2) and ADR-0002 Accepted — both still open, needed before more v2 content ships
