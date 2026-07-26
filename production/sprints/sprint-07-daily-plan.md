# Sprint 7 — Daily Plan & Progress Tracker

> **Sprint**: 2026-07-27 (Mon) → 2026-07-31 (Fri)
> **Companion to**: `sprint-07.md` (formal plan) — this file is the day-by-day breakdown + live tracker
> **Maintained by**: PM assistant — updated each session/standup
> **Routines (Claude Code)**:
>   - **Mon–Fri 10:00** → `/daily-standup`
>   - **Sat 22:00** → `/weekly-wrapup`
>   - **Sun 22:00** → `/weekly-kickoff`
> **Opened**: 2026-07-26 (Sun 22:00 kickoff) — autonomous scheduled run, no user present. Branch
> `sprint-07` created from `sprint-06` tip (`a27cb34`).

---

## Status Verdict: 🟡 DAY 1 (2026-07-27) — branch still red, but 3/9 S1 bugs confirmed fixed in working tree (uncommitted)

Sprint 6 closed **CONCERNS**: Must-Have bug list from its own scope mostly landed (8-9/9 task count),
but late-week off-plan work (Base/CoreBase hub refactor + Pathfinding) shipped uncompiled — 6
parallel code-review agents found 6 independent compile-blocking errors (BUG-024–029) plus 2 more
S1 findings (BUG-030/031) that make `Core.GetCoreComponent<T>()` functionally dead even past the
syntax errors. This is the **3rd consecutive sprint** with this pattern (Sprint 5 retro flagged it,
Sprint 6 retro flagged it again). Sprint 7 is scoped narrow and entirely bug-fix — no new feature
work until the branch compiles and the component hub is verified in Play Mode (S7-08 gate).

⚠️ Working tree had uncommitted changes at kickoff time (`EventManager.cs` modified,
`ICharacter.cs.meta` / `PlayerDeathState.cs.meta` untracked) — check whether this is a partial fix for
BUG-026/BUG-029 already in progress before starting S7-00/S7-05.

---

## Day-by-Day Plan

### Mon 2026-07-27 — Compile errors, batch 1 (Core/Base hub)

| Task | Est. | Notes | Status (verified via code read, EOD Mon) |
|------|------|-------|-------------------------------------------|
| S7-00 (BUG-024, `CoreComponentBase.cs:5`) | 0.15d | CS0592 — auto-property `[SerializeField]`, blocks every Core/EntityCore build | ✅ Done (uncommitted) — backing field `[SerializeField] private T core` + `Core { get; set; }` property, no more CS0592 |
| S7-05 (BUG-029, `EventManager.cs:42`) | 0.1d | CS0102 — duplicate `ON_PLAYER_DEATH`, quick fix, do early | ❌ Not started — `EventManager.cs` untouched, `ON_PLAYER_DEATH` still duplicated at lines 42 and 53 |
| S7-01 (BUG-025, `PlayerDisadvantageState.cs:20`) | 0.1d | CS0103 — bare identifier | ❌ Not started — `if (Status == EndRangeTrigger)` unchanged |
| S7-02 (BUG-026, `PlayerDeathState.cs:17,21`) | 0.15d | CS0029 — enum as bool | ❌ Not started — `if (StatusAnimation.Start)` / `.End` unchanged |
| S7-06 (BUG-030, `Core.cs:7` / `EntityCore.cs:17`) | 0.3d | `Awake()` must `override`, not hide — start once S7-00 lands | ✅ Done (uncommitted) — both `Core.cs` and `EntityCore.cs` now `protected override void Awake()` calling `base.Awake()` |

**Bonus, pulled forward from Tue (uncommitted, unplanned-but-in-scope):**
- S7-07 (BUG-031) ✅ — `CoreComponentBase.Setup()` override restored, `Core` back-ref populated in `Awake()`
- S7-03 (BUG-027) ✅ — `EntityMovement.cs`: `if (entityInput.TargetTransform.position)` → `if (entityInput.TargetTransform != null)`, plus waypoint bounds guard
- S7-04 (BUG-028) ⚠️ Partial — `EntityInput.cs` operator bug fixed (`Transform - Vector2` → `.position` math corrected), but `GetTargetInRange()` method body was **entirely commented out** rather than rewired. Net effect: compiles, but enemy auto target-acquisition (FOV) and `isAttack` range detection are now dead code — this does **not** meet S7-04's acceptance criteria ("FOV/target math verified in Play Mode") yet. Needs `entityFind.FindTargetMethod(...)` wired back in using `Core.Entity.Data` (the removed `entity` field, not restored) before this can be marked done.

Result: **branch still will not compile** — BUG-025/026/029 (all "quick, do early" items) are the 3 remaining blockers. Recommend committing the 3 confirmed fixes (S7-00/06/07/03) as their own small commits today to lock in progress, separate from the BUG-028 partial fix.

### Tue 2026-07-28 — Compile errors, batch 2 + hub verification

| Task | Est. | Notes |
|------|------|-------|
| S7-07 (BUG-031, `CoreComponentBase.cs:17-21`) | 0.3d | Depends on S7-06 landing first |
| S7-03 (BUG-027, `EntityMovement.cs:53`) | 0.15d | Independent, can run parallel to S7-06/07 |
| S7-04 (BUG-028, `EntityInput.cs:80,82,99,103`) | 0.25d | Independent, can run parallel |
| S7-08 (Play Mode verify `GetCoreComponent<T>()`) | 0.2d | **Gate** — do not start S7-09/S7-11 until this passes |

Goal: by end of Tue, zero Console errors and the component hub confirmed live for Player + Entity.

### Wed 2026-07-29 — Post-gate fixes

| Task | Est. | Notes |
|------|------|-------|
| S7-09 (BUG-032, `EntityWeaponMelee.cs:26,49`) | 0.2d | Gated on S7-08 |
| S7-10 (BUG-033/ES-1, `EnemySpawner.cs:62`) | 0.15d | Independent of S7-08, can start any day |
| S7-11 (Bug #6 re-scope + EditMode test) | 0.4d | Gated on S7-08 — largest single item this sprint |
| S7-12 (ADR-0002 Accepted) | 0.1d | Independent, quick |

### Thu 2026-07-30 — Decisions + Should-Have

| Task | Est. | Notes |
|------|------|-------|
| S7-13 (S4-05/S4-06 forced decision) | 0.1d | 6th carry — must close this cycle, no more silent re-carry |
| S7-D4 (off-plan-work root-cause conversation) | 0.3d | Highest-value Should-Have — 3-cycle pattern, needs a real process fix not another observation |
| S7-D3 (individual `BUG-NNN.md` files) | 0.2d | Process change, low effort |
| Buffer / catch-up | — | 1-day buffer reserved for Must-Have slippage |

### Fri 2026-07-31 — Should-Have stretch + wrap prep

| Task | Est. | Notes |
|------|------|-------|
| S7-D1 (Bug #13 start-room teleport) | 0.25d | If Must-Have closed clean |
| S7-D2 (Bug #15 build-safe JSON load) | 0.5d | If Must-Have closed clean |
| S7-N1 (first playtest) | — | Only if S7-08/09/11 all confirmed stable |
| Friday wrap-up prep | — | Feeds into Sat 22:00 `/weekly-wrapup` |

---

## Standup Log

### 2026-07-27 (Mon) — Day 1 standup (autonomous scheduled run)

**Yesterday (2026-07-26, Sun kickoff):** Only the kickoff commits landed (`a27cb34` wrap-up,
`79f5057` open sprint-07). No dev work committed yet on `sprint-07` itself — this is effectively Day 0→1.

**Working tree assessment (read-only, no `.cs` edited by this run):** substantial uncommitted work
already sits on top of the kickoff commit, touching `Base/CoreBase.cs`, `Base/CoreComponentBase.cs`,
`EntityCore.cs`, `Core.cs`, `EntityMovement.cs`, `EntityInput.cs`, `EnemySpawner.cs`, `EntityFindTarget.cs`,
`EntityWeaponHolder.cs`, plus Pathfinding (`GridBuilder.cs`, `PathfindingGrid.cs`) and Maze/Room files.
Verified against the 9 tracked S1 bugs by reading current file contents:

| Bug | Status |
|-----|--------|
| BUG-024 (S7-00) | ✅ Fixed, uncommitted |
| BUG-030 (S7-06) | ✅ Fixed, uncommitted |
| BUG-031 (S7-07, Tue item) | ✅ Fixed early, uncommitted |
| BUG-027 (S7-03, Tue item) | ✅ Fixed early, uncommitted |
| BUG-028 (S7-04, Tue item) | ⚠️ Partial — operator bug fixed, but replacement target-acquisition logic left fully commented out (new functional gap, not just "not yet started") |
| BUG-029 (S7-05) | ❌ Untouched — still duplicate `ON_PLAYER_DEATH` |
| BUG-025 (S7-01) | ❌ Untouched — still bare `EndRangeTrigger` |
| BUG-026 (S7-02) | ❌ Untouched — still enum-as-bool |
| BUG-032 (S7-09, Wed item) | ❌ Untouched — `input` field assignment still commented out in `EntityWeaponMelee.cs:26` |
| BUG-033 (S7-10, Wed item) | ❌ Untouched — `EnemySpawner.cs` null-check order still `set.Count == 0 || set == null` (wrong order) |

**Net:** branch does not compile yet. 3 confirmed fixes ready to commit; 1 partial fix needs finishing
before commit (or commit with a follow-up task, not silently left half-done); 3 of Monday's own planned
items not started; Wed's two items also untouched.

**Today's plan (remaining):**
| Task | Est. | Rationale |
|------|------|-----------|
| S7-05 (BUG-029) | 0.1d | Trivial dedupe, unblocks compile fastest — do first |
| S7-01 (BUG-025) | 0.1d | Independent, trivial |
| S7-02 (BUG-026) | 0.15d | Independent, trivial |
| Finish S7-04 (BUG-028) | ~0.15d remaining | Rewire `GetTargetInRange()` using `entityFind.FindTargetMethod(...)` + `Core.Entity.Data` instead of leaving it commented out |
| Commit checkpoint | — | Split into scoped commits per bug (S7-00/06/07/03 as one group, S7-05/01/02 as another) rather than one large commit — the recurring off-plan-work pattern (S7-D4) has partly been about oversized, hard-to-review commits |
| S7-08 (Play Mode verify) | 0.2d | Attempt once S7-05/01/02 land and branch compiles clean — S7-06/07/03 prerequisites already done |

**Blockers:**
- Branch still non-compiling (BUG-025/026/029 open) — nothing downstream of the hub can be verified in Play Mode yet.
- No Unity Editor CLI in this environment — compile status and Play Mode checks in this report are from static code reading, not an actual build; owner must confirm in-Editor.

**Emerging risks:**
- BUG-028's partial fix (commented-out logic instead of a real rewire) is the same failure pattern Sprint 6's wrap-up flagged ("compiles but functionally dead") — flag for extra scrutiny at S7-08 gate, don't let it slide through as "done" once compile succeeds.
- Large uncommitted diff spanning both in-scope (S7-03/04/06/07) and adjacent Pathfinding/Maze files (`GridBuilder.cs`, `PathfindingGrid.cs`, `MazeController.cs`, `MazeGenerator.cs`) not tied to any open bug ID — worth a quick sanity check that this isn't S7-D4's "off-plan work" pattern recurring on Day 1. `MazeController.cs`'s change is a harmless test-tuning value (Rows/Columns 3→2), and Pathfinding wiring in `EntityMovement.cs`/`EnemyManager.cs` predates this sprint (shipped in Sprint 6), so this reads as pre-existing rather than new scope creep — but worth the owner's eyes.
- Confirmed still open, not in Sprint 7's tracked list: Bug #14 (`MazeController.Awake()` missing `return` after `Destroy(gameObject)`, duplicate-instance still overwrites `Instance`) — recommend `/bug-triage` re-add it rather than let it stay silently dropped.
- ADR-0002 (S7-12) still reads `Status: Proposed` — not flipped yet.
- QA plan still missing (6th consecutive cycle as of today) — still deferred to owner per sprint doc.

(Prior: no entries yet, sprint not started.)

---

## Carry-Over Watch List (re-verify every standup)

- Bug #6 — 8th carry cycle, regressed twice; S7-11 is the first attempt scoped with a mandatory
  EditMode test. If this slips again, escalate to a dedicated spike rather than a 3rd opportunistic fix.
- Off-plan work — 3 consecutive cycles. S7-D4 is scheduled specifically to break the pattern, not
  just re-flag it. If Thu/Fri produces another unplanned architecture commit, that itself is the
  clearest evidence the root-cause conversation hasn't landed.
- QA plan — 5 consecutive cycles with none. Flagged in `sprint-07.md`, deferred to owner.
