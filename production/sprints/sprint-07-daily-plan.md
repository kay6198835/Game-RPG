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

## Status Verdict: 🔴 DAY 0 — not started, 9 S1 (compile-blocking or functionally-dead) bugs open

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

| Task | Est. | Notes |
|------|------|-------|
| S7-00 (BUG-024, `CoreComponentBase.cs:5`) | 0.15d | CS0592 — auto-property `[SerializeField]`, blocks every Core/EntityCore build |
| S7-05 (BUG-029, `EventManager.cs:42`) | 0.1d | CS0102 — duplicate `ON_PLAYER_DEATH`, quick fix, do early |
| S7-01 (BUG-025, `PlayerDisadvantageState.cs:20`) | 0.1d | CS0103 — bare identifier |
| S7-02 (BUG-026, `PlayerDeathState.cs:17,21`) | 0.15d | CS0029 — enum as bool |
| S7-06 (BUG-030, `Core.cs:7` / `EntityCore.cs:17`) | 0.3d | `Awake()` must `override`, not hide — start once S7-00 lands |

Goal: clear the standalone compile errors first (S7-00/01/02/05 have no dependencies), then start the
`Awake()` override fix which everything else gates on.

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

(Populated by `/daily-standup` each morning — no entries yet, sprint not started.)

---

## Carry-Over Watch List (re-verify every standup)

- Bug #6 — 8th carry cycle, regressed twice; S7-11 is the first attempt scoped with a mandatory
  EditMode test. If this slips again, escalate to a dedicated spike rather than a 3rd opportunistic fix.
- Off-plan work — 3 consecutive cycles. S7-D4 is scheduled specifically to break the pattern, not
  just re-flag it. If Thu/Fri produces another unplanned architecture commit, that itself is the
  clearest evidence the root-cause conversation hasn't landed.
- QA plan — 5 consecutive cycles with none. Flagged in `sprint-07.md`, deferred to owner.
