# PM Routine Protocol

Shared by the project-own PM skills (`/daily-standup`, `/weekly-wrapup`, `/weekly-kickoff`,
`/doc-sync --auto`, `/module-quality-audit`) whenever they run as a scheduled routine
(`~/.claude/scheduled-tasks/pm-*`) or with `--auto`. Adopted 2026-10-09 (owner decision).

## 1. Scope

- **Write only `*.md` files.** Never create, edit, stage or delete `.cs`, scenes, prefabs,
  ScriptableObjects, `ProjectSettings/`, `Packages/`, `.claude/settings*.json` or `.claude/hooks/`.
  The project settings allow `.md` edits without a prompt and nothing else, so an attempt to touch
  anything else stalls the run instead of acting — treat that stall as a defect in the routine.
- Read code freely. Never fix code; a suspected defect becomes a bug-inbox note
  (`.claude/rules/bug-inbox.md`).
- Run fully unattended: never ask a question. Make the conservative choice, state it in the digest.

## 2. Where to work — the PM worktree

The owner codes in `D:/Fork/Game-RPG` and may have uncommitted work on any branch. A routine never
checks out, stages or commits there (2026-10-06: a standup commit landed on the owner's feature
branch and had to be moved by hand).

Routines work in a separate git worktree, `D:/Fork/Game-RPG-pm`, on a **detached HEAD** so the owner
can still check out `sprint-NN` in their own folder.

**Command form — this is what lets a routine run without approval prompts.** The project settings
allow exact command prefixes only. A command that does not start with one of them stops the run and
waits for an approval nobody gives (kickoff 2026-10-12 stalled this way for over an hour). So:

- **Use the Bash tool for every shell command. Never use the PowerShell tool** — no allow rule
  covers it.
- **One command per tool call.** No `&&`, `;`, `|` chains, no `cd`, no shell variables
  (`$PM`, `$NN`), no quotes around the path. Write the literal path every time:
  `git -C D:/Fork/Game-RPG-pm <subcommand> …`.
- Substitute the sprint number yourself (`origin/sprint-18`), never `sprint-$NN`.
- Run scripts as `bash .claude/scripts/<name>.sh …` from the project root.

```bash
git -C D:/Fork/Game-RPG-pm fetch origin --prune
git -C D:/Fork/Game-RPG-pm status --porcelain          # must be empty; if not, stop and report
git -C D:/Fork/Game-RPG-pm checkout --detach origin/sprint-18
```

If `D:/Fork/Game-RPG-pm` does not exist: `git worktree add --detach D:/Fork/Game-RPG-pm origin/sprint-18`.
If `fetch` does not return within a minute (credential prompt), stop it, continue on the last fetched
state, and say so in the digest.

Read files for the report from `D:/Fork/Game-RPG-pm` (sprint state). Read **commits** with `git log --all` so feature
branches count. Uncommitted owner work is invisible to the routine, by design.

The only routine step that touches the owner's folder is `.claude/scripts/compile-check.sh`, which
runs Unity headless there only when no Editor is running, and never commits or reverts.

The `rtk` hook may report a stale branch or status (seen 2026-10-06); use
`rtk proxy git …` or `git -C …` when the answer matters.

## 3. Committing

**Routine output** (tracker, standup/triage/retro/module-health reports, playtest sheet, bug inbox
notes, routine log) → commit on the detached HEAD and push straight to the sprint branch:

```bash
git -C D:/Fork/Game-RPG-pm add -A -- ':(glob)**/*.md'
git -C D:/Fork/Game-RPG-pm diff --cached --name-only        # every line must end in .md; if not, git -C D:/Fork/Game-RPG-pm reset -q and stop
git -C D:/Fork/Game-RPG-pm commit -m "chore(<routine>): <summary> <YYYY-MM-DD>" -m "Co-Authored-By: Claude <noreply@anthropic.com>"
git -C D:/Fork/Game-RPG-pm push origin HEAD:sprint-18
```

**Special changes** go through a short branch, so they can be reviewed or reverted as one unit:
(a) edits to an existing living doc by `/doc-sync` (`CLAUDE.md`, GDDs, ADRs, `docs/systems/**`,
`.claude/rules/**`); (b) any bug-status change or new bug file; (c) any `.claude/**/*.md` edit;
(d) anything the routine flags "owner should look".

```bash
git -C D:/Fork/Game-RPG-pm checkout -b pm/<routine>-<YYYY-MM-DD> origin/sprint-18
# … edit, add *.md, commit (same checks as above) …
git -C D:/Fork/Game-RPG-pm push origin pm/<routine>-<YYYY-MM-DD>
git -C D:/Fork/Game-RPG-pm checkout --detach origin/sprint-18
git -C D:/Fork/Game-RPG-pm merge --no-ff pm/<routine>-<YYYY-MM-DD> -m "Merge pm/<routine>-<YYYY-MM-DD> into sprint-18"
git -C D:/Fork/Game-RPG-pm push origin HEAD:sprint-18
```

(`sprint-18` stands for the current sprint; write the real number.)

**Push rejected** (the owner pushed meanwhile): `fetch`, `rebase origin/sprint-NN` (`.md`-only
commits rebase cleanly), push again. Never force-push. If the rebase conflicts, abort it, leave the
commit on a `pm/` branch, push that branch, and report it in the digest.

## 4. Evidence tiers

Every verdict a routine writes carries the strongest tier it rests on:

| Tier | Source |
|---|---|
| `STATIC` | reading code and docs |
| `LOG` | `.claude/scripts/editor-log.sh` (Unity Editor.log of the owner's sessions) |
| `COMPILED` | `.claude/scripts/compile-check.sh` passed |
| `RUNTIME` | a filled playtest sheet in `production/qa/playtests/` |

A step that could not run says so by name (`SKIPPED — Editor open`, `NOT RUN — no sheet`), never
silently (`.claude/rules/skill-authoring.md`).

## 5. Run log

Every run, including a skipped one, appends one row to `production/session-state/routine-log.md`:
date, routine, HEAD of `origin/sprint-NN`, status (`OK` / `PARTIAL` / `SKIPPED` / `FAILED`),
evidence tier, commit hash, one-line note.

## 6. Shared scripts (`.claude/scripts/`, read-only)

| Script | Used by |
|---|---|
| `bug-id.sh next` | `/weekly-wrapup` only (Saturday triage) |
| `bug-id.sh audit` | `/weekly-wrapup`, `/doc-sync` |
| `fix-survival.sh [days]` | `/weekly-wrapup` (30), `/daily-standup` (7, conditional) |
| `doc-claims.sh` | `/doc-sync` Phase 0, `/weekly-wrapup` (count only) |
| `editor-log.sh` | `/daily-standup`, `/weekly-wrapup` |
| `compile-check.sh` | `/daily-standup` |
| `playtest-sheet.sh [saturday]` | `/daily-standup` on Friday |
