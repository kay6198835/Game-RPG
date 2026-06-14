---
name: weekly-sprint
description: "Runs the project's full weekly sprint ritual end to end. 'kickoff' (Monday) reports last week's status and checks scope; 'wrapup' (Friday) reviews the week's changed code, triages bugs, optionally logs a playtest, creates next week's sprint plan, and commits the Friday wrap-up. Operationalizes production/review-schedule.md. Pass --auto for unattended scheduled runs."
argument-hint: "[kickoff|wrapup] [--auto] [--review full|lean|solo]"
user-invocable: true
allowed-tools: Read, Glob, Grep, Write, Edit, Bash, Task, AskUserQuestion
context: |
  !cat production/review-mode.txt 2>/dev/null
  !ls production/sprints/ 2>/dev/null
  !git log --oneline -8 2>/dev/null
---

# Weekly Sprint Ritual

This skill is the single entry point for the project's weekly sprint cadence,
defined in [`production/review-schedule.md`](../../../production/review-schedule.md).
It does not reimplement the sub-skills — it sequences them in the right order and
handles the Friday wrap-up commit.

- **`kickoff`** (Monday, ~15 min) — close out last week: status + scope check.
- **`wrapup`** (Friday, ~20 min) — review changed code, triage bugs, optional
  playtest, create next week's sprint plan, and commit the wrap-up.

> **Sprint creation lives in Friday wrap-up**, not Monday. The new sprint is
> drafted after the week's reports are done, so it reflects what actually shipped.

---

## Phase 0: Parse Arguments, Autonomy & Review Mode

1. **Mode** — `kickoff` or `wrapup`. If omitted:
   - Autonomous run (`--auto` present): infer from the day in the `currentDate`
     context (Mon → kickoff, Fri → wrapup); if ambiguous, default to `wrapup`.
   - Interactive: ask the user which mode to run via `AskUserQuestion`.

2. **Autonomy** — if `--auto` is passed (scheduled cloud routine), run in
   **autonomous** mode: never call `AskUserQuestion`; use the documented defaults
   at each decision point. Otherwise run **interactive**: ask the user at each
   decision point.

3. **Review mode** — resolve once and reuse for any gate this run:
   `--review [full|lean|solo]` flag → else `production/review-mode.txt` → else
   `lean`. See `.claude/docs/director-gates.md` for the gate pattern.

4. **Date** — take today's date from the `currentDate` context. Never call
   `Date.now()` or shell `date` for the report/commit date.

---

## Phase 1: KICKOFF (Monday)

Run only when mode = `kickoff`. This phase does **not** create a sprint.

1. **Last week's status** — run the `/sprint-status` logic: read the most recent
   plan in `production/sprints/`, and `production/sprint-status.yaml` if it exists.
   Summarize completion % and what carries over.

2. **Scope check** — run `/scope-check` to detect scope creep against last week's
   plan.

3. **Summary** — print a short status digest and remind the user that the new
   sprint is created at Friday wrap-up. No file is written in this phase.

---

## Phase 2: WRAPUP (Friday)

Run only when mode = `wrapup`. Execute steps in this order.

### 2.1 — Find the week's changed `.cs` files

Use `Bash` + `git`:
1. Find the most recent Friday wrap-up commit: `git log --grep="Friday wrap-up" --format=%H -n 1`.
2. Diff its range to HEAD: `git diff --name-only <that-commit>..HEAD -- "Assets/Script/**/*.cs"`.
3. Fallback if no prior wrap-up commit: `git diff --name-only "@{7.days.ago}"..HEAD -- "Assets/Script/**/*.cs"` (or the last 8 commits).

Report the file list. If empty, note "no code changes this week" and skip 2.2.

### 2.2 — Code review

Delegate `/code-review` over the files from 2.1, prioritizing the most complex
changes. Surface findings; do not auto-fix (fixes are separate sprint work).

### 2.3 — Bug triage

Delegate `/bug-triage sprint`. This writes `production/qa/bug-triage-<date>.md`
and re-prioritizes the open backlog against the current sprint.

### 2.4 — Playtest report (OPTIONAL — runs after triage, before sprint creation)

- **Interactive**: `AskUserQuestion` — "Log a playtest report for this week?"
  - Yes → delegate `/playtest-report new` → writes
    `production/qa/playtests/playtest-<date>-weekly-wrapup.md`.
  - No → skip, note "playtest skipped".
- **Autonomous (`--auto`)**: skip by default. Note "playtest skipped — run
  `/playtest-report` manually if a session happened this week."

### 2.5 — Create NEXT week's sprint plan

Delegate the `/sprint-plan new` flow (read milestone, previous sprint, GDDs, risk
register; build the plan using the template in
[`sprint-plan/SKILL.md`](../sprint-plan/SKILL.md)). Respect the review mode and the
QA-plan gate (sprint-plan Phase 5).

- **Interactive**: present the proposed plan and **ask before writing**.
  **Carryover is a decision point** — `AskUserQuestion`: (a) defer carryover to a
  later sprint, or (b) pull it in and cut a task. Never silently re-defer. On
  approval, write `production/sprints/sprint-<N>.md`.
- **Autonomous (`--auto`)**: write a **draft** to
  `production/sprints/sprint-<N>.draft.md` with a "⚠️ DRAFT — carryover decision
  required" header listing the carryover choices. Do **not** overwrite the official
  `sprint-<N>.md`, and do **not** commit the sprint as final (see 2.6).

### 2.6 — Commit the Friday wrap-up

Stage the generated reports plus the sprint file, then commit with the ritual
message and co-author trailer. Date from `currentDate`.

- **Interactive**: stage `production/qa/bug-triage-<date>.md`, the playtest report
  (if written), and the official `production/sprints/sprint-<N>.md`.
  ```
  chore: Friday wrap-up — bug triage[, playtest report] + sprint-<N> plan <YYYY-MM-DD>

  Co-Authored-By: Claude Opus 4.8 <noreply@anthropic.com>
  ```
- **Autonomous**: stage the bug-triage report and `sprint-<N>.draft.md` only.
  ```
  chore: Friday wrap-up — bug triage + sprint-<N> draft <YYYY-MM-DD>

  Co-Authored-By: Claude Opus 4.8 <noreply@anthropic.com>
  ```
  Leave a summary note telling the user to review the draft, decide carryover, and
  promote it to the official sprint file.

**Rules:** never stage code changes opportunistically — only the wrap-up
artifacts. If on the default branch (`main`), create a branch first (per global git
rules); normally the repo is on a feature branch, so commit directly. Report the
commit hash. Never `git push` — leave that to the user.

---

## Phase 3: Collaborative Protocol

- **Autonomous never asks.** With `--auto`, every "ask the user" step uses its
  documented default (skip playtest; draft-not-final sprint; commit reports + draft).
- **The sprint plan is a commitment, not a suggestion.** In interactive mode, the
  carryover decision is always the user's — surface the options, recommend, but let
  them choose.
- **Stay in scope.** This skill orchestrates existing skills; it does not duplicate
  their logic or change their output formats.
- **Reporting language** — chat output is English with Vietnamese keyword glosses in
  parentheses on first use (per `.claude/rules/language-reporting.md`). Files written
  under `production/` follow each sub-skill's own English-only format.
- **Final summary** — always end with a one-line digest: mode, what was produced,
  the commit hash (if any), and the next manual action (e.g. "promote the draft
  sprint", "run /qa-plan sprint").

---

## Scheduling

This skill is driven by two cloud routines (managed via `/schedule`):

| Routine | Cron | Command |
|---------|------|---------|
| Monday kickoff | `0 10 * * 1` (10:00 Mon) | `/weekly-sprint kickoff --auto` |
| Friday wrap-up | `0 15 * * 5` (15:00 Fri) | `/weekly-sprint wrapup --auto` |

Both run autonomously. The Friday routine commits the reports and a sprint draft;
promoting the draft to the official sprint plan is a manual review step.
