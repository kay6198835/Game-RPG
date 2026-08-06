---
name: module-quality-audit
description: "Monthly per-module quality audit: scores each system module (bug density, test evidence, tech debt, code-vs-GDD drift), then checks whether recent changes touched an already-approved/designed module and flags it for re-verification. Combines bug-triage, consistency-check, content-audit, test-evidence-review, tech-debt, and propagate-design-change into one monthly cycle instead of running them ad hoc."
argument-hint: "[audit | quick | module:<name>]"
user-invocable: true
allowed-tools: Read, Glob, Grep, Write, Edit, Bash, Task, AskUserQuestion
---

# Module Quality Audit

Monthly health check across every system module, plus a change-impact pass that
catches when new work silently touches a module whose design was already
locked in. Two questions this skill exists to answer every month:

1. **How healthy is each module right now?** (bug density, test coverage, tech
   debt, drift from its GDD)
2. **Did anything land this month that reaches into an already-approved
   module without anyone re-checking it?**

Run at the start of each month. Can also be run ad hoc (`module:<name>`) when a
single module needs a spot check outside the normal cycle.

---

## Autonomous vs Interactive Mode

This skill runs two ways:
- **Interactive** (user invokes `/module-quality-audit` directly): use
  `AskUserQuestion` at the decision points below.
- **Autonomous** (invoked by the `pm-monthly-module-audit` scheduled task, no
  user present): skip every `AskUserQuestion`, make the stated default choice,
  and note the choice made in the final report instead of asking. Never edit
  `.cs` files or game assets in this mode — read-only on code, write-only under
  `production/`.

Detect which mode applies from how the skill was invoked (a scheduled/cron
context vs. a live chat turn).

---

## Phase 1: Load Module Registry

1. Read `design/gdd/systems-index.md`. Take every system row that has a
   standalone GDD file (currently 7: `animation-system.md`,
   `character-system.md`, `weapons-system.md`, `skill-ability-system.md`,
   `map-system.md`, `stat-system.md`, `enemy-spawn-system.md`) — these are the
   audit's module units. Skip rows with `Status: Not Started` (no GDD, no code
   to audit yet).

2. For each module, resolve its **Status** from the index:
   - `Approved` → **locked/approved module** — any future change touching it
     needs explicit re-verification (this is the module the user's change-impact
     ask is about).
   - `Designed` → **reviewed baseline** — treat as approved-equivalent for
     drift purposes, but don't require the same re-verification weight.
   - `In Progress` → active development, drift checks are informational only,
     not a flag.

3. Map each module to its owning code path using `CLAUDE.md`'s Repository
   Layout section (e.g. `skill-ability-system.md` → `Assets/Script/Skill_Ability/`,
   `enemy-spawn-system.md` → `Assets/Script/Enemy/` + `Assets/Script/Database-SO/Modal/`).
   If a module has no doc-gap note already, use `CLAUDE.md`'s existing "Checking
   for Documentation Gaps" output as a cross-check.

4. Find the previous audit: `Glob` for `production/qa/module-health-*.md`,
   take the most recent by filename date. If none exists, this is the first
   run — use `git log --since="1 month ago"` as the change window instead of
   diffing against a prior audit date.

---

## Phase 2: Per-Module Quality Scorecard

Spawn one `Task` (subagent) per module **in parallel** — this is the expensive
part, keep each agent scoped to just its module's files. Each agent should
produce a verdict: **HEALTHY** / **AT RISK** / **CRITICAL**, using:

| Axis | Source |
|------|--------|
| Bug density | Grep `production/qa/bugs/*.md` and the latest `production/qa/bug-triage-*.md` for the module's file names / system name; also check `CLAUDE.md`'s Known Bugs table for entries pointing into this module's paths |
| Tech debt | Grep `docs/tech-debt-register.md` for entries tagged to this module |
| Test evidence | Check `tests/EditMode/` and `tests/PlayMode/` for files matching the module's classes; note ADVISORY vs BLOCKING per `.claude/rules/test-standards.md` |
| GDD-vs-code drift | Spot-check the GDD's Formulas / Detailed Rules section against current code (field names, values) — same check `/consistency-check` does, scoped to just this module instead of all GDDs |
| Code quality | Read the module's core files (2-4 files max, not the whole tree) for obvious violations of the relevant `.claude/rules/*.md` file for that domain (e.g. `gameplay-code.md`, `ai-code.md`) |

Verdict thresholds:
- **CRITICAL**: any open S1 bug traced to this module, OR a compile-blocking issue, OR test evidence missing on a Logic/Integration story that requires it (BLOCKING per test-standards.md)
- **AT RISK**: 2+ open S2/S3 bugs, OR 3+ open tech-debt items, OR confirmed GDD-vs-code drift not yet reconciled
- **HEALTHY**: none of the above

Each agent returns: verdict, top 3 findings (one line each), bug/debt counts.

---

## Phase 3: Change-Impact Analysis

This is the check the user specifically asked for: **did an approved module
get touched by work that wasn't about it?**

1. `git log --since=[last audit date or 1 month ago] --name-only --pretty=format:"%H|%s"` across
   the current sprint branch(es) touched this month (check `production/sprints/`
   for which sprint branches were active in the window).

2. Build a changed-files list, then map each file to a module using the same
   path mapping from Phase 1.

3. For every **Approved** or **Designed** module that appears in the changed-files
   list:
   - Read the commit message(s) that touched it. If the commit's own scope
     (per its sprint story ID, e.g. `S8-03`) is genuinely about that module,
     this is expected — note it as "in-scope change" and move on.
   - If the commit's scope is about a *different* module or untracked/off-plan
     work (the pattern flagged repeatedly in `production/sprints/sprint-07-daily-plan.md`
     and `sprint-08-daily-plan.md` — off-plan work bleeding into unrelated
     files), flag it as **cross-module impact** and determine the right follow-up:
     - The module's **GDD file itself changed** → recommend/run
       `/propagate-design-change [gdd-file]`
     - Only **code changed**, GDD untouched → recommend/run `/consistency-check`
       scoped to this module — likely silent drift
     - An **ADR governing this module** changed status or content → recommend/run
       `/architecture-review`

4. Produce a Change-Impact table:

   | Module | Status | Files Touched | Commit(s) | In-Scope? | Recommended Follow-Up | Follow-Up Run This Cycle? |
   |--------|--------|----------------|-----------|-----------|------------------------|---------------------------|

**Interactive mode**: use `AskUserQuestion` per flagged cross-module impact —
"Run the recommended follow-up now, or defer and just record the flag?"
**Autonomous mode**: default to recording the flag without auto-running the
follow-up skill (running `/propagate-design-change` or `/architecture-review`
unattended could produce a document rewrite with no one to approve it) — note
in the report "Follow-up not run — requires owner decision" for each flagged row.

---

## Phase 4: Aggregate Report

Write `production/qa/module-health-[YYYY-MM].md`:

```markdown
# Module Quality Audit — [Month YYYY]

**Run**: [date] ([autonomous scheduled run | interactive])
**Window**: [last audit date or "1 month"] → [today]

## Module Scorecard

| Module | Status (GDD) | Verdict | Bugs | Tech Debt | Test Evidence | Top Finding |
|--------|---------------|---------|------|-----------|----------------|-------------|

## Change-Impact: Approved/Designed Modules Touched This Cycle

| Module | Status | Files Touched | Commit(s) | In-Scope? | Recommended Follow-Up | Run This Cycle? |
|--------|--------|----------------|-----------|-----------|------------------------|-------------------|

## Trend vs. Previous Audit

[If a prior module-health-*.md exists: which modules improved / regressed verdict tier since last month. If first run: "No prior audit — this is the baseline."]

## New Bugs / Debt Found This Cycle

[Any new issue found while auditing MUST be filed as its own `production/qa/bugs/BUG-NNN.md`
via /bug-report — never written here as prose only. List filed IDs.]

## Recommended Actions Next Cycle

[Bulleted, concrete — not "keep monitoring."]
```

---

## Phase 5: New Findings → Individual Bug Files

Per the process gap already flagged across Sprints 6-8 (`S7-D3`/`S8-D1`,
never completed): any new bug or drift found during this audit must be filed
as its own file via `/bug-report`, output to `production/qa/bugs/BUG-NNN.md`
(increment `NNN` from the highest existing file). Do **not** fold new findings
into the module-health report as prose only — that repeats the exact mistake
this audit exists partly to catch.

---

## Phase 6: Persist

**Interactive mode**: ask "May I write `production/qa/module-health-[YYYY-MM].md`
[+ any new `BUG-NNN.md` files] and commit?" before writing/committing.

**Autonomous mode**: write the report + any new bug files directly (no
approval gate — matches the existing `pm-weekly-kickoff`/`pm-weekly-wrapup`
autonomous convention), then:

```
git add production/qa/module-health-[YYYY-MM].md production/qa/bugs/*.md
git commit -m "chore(module-audit): monthly module quality audit [YYYY-MM]

Co-Authored-By: Claude Opus 4.8 <noreply@anthropic.com>"
git push
```

Never stage `.cs` files or assets. If the current branch is a sprint branch,
commit there; do not create a new branch for this audit.

---

## Output

A summary: per-module verdict table, count of CRITICAL/AT RISK/HEALTHY modules,
count of cross-module impact flags found, count of new bugs filed, and the
report file path.

Verdict: **COMPLETE** — audit written and (if autonomous) committed.
Verdict: **BLOCKED** — could not resolve module registry or git history for the window; partial report produced.
