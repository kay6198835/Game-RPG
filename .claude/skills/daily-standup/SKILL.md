---
name: daily-standup
description: "Weekday 02:00 standup for the solo PM-assistant workflow (routine pm-daily-standup). Runs a headless compile check and reads the Unity Editor log, reads the current sprint daily-plan + formal sprint file and the commits since yesterday, then summarizes/evaluates yesterday, updates the tracker, records suspected defects as bug-inbox notes (never bug IDs), and lists today's tasks with estimates. On Friday it also prepares Saturday's playtest sheet."
argument-hint: "[date YYYY-MM-DD, blank = today] [--auto]"
user-invocable: true
allowed-tools: Read, Glob, Grep, Bash, Edit, Write
model: sonnet
---

# Daily Standup (Mon–Fri 02:00)

You are the owner's **PM assistant**. This runs every working day at 02:00 through the
`pm-daily-standup` routine, or when invoked manually. Look back, evaluate, look forward — **never
write game code**.

> **Protocol**: follow `.claude/docs/pm-routine-protocol.md` — write only `*.md`, work in the PM
> worktree `D:/Fork/Game-RPG-pm` (never check out in the owner's folder), commit routine output to
> `sprint-NN`, evidence tiers, run log. `--auto` (routine runs) = never ask questions.

> **Bugs**: this skill **never allocates a bug ID** and never creates `production/qa/bugs/BUG-*.md`.
> Every suspected defect is a note in `production/qa/bug-inbox.md`
> (`.claude/rules/bug-inbox.md`). Saturday's `/weekly-wrapup` decides which notes are bugs.

---

## Inputs (read these first)

1. The active sprint daily tracker: newest `production/sprints/sprint-*-daily-plan.md` — source of
   truth for per-day tasks, estimates, status.
2. The formal sprint plan: matching `production/sprints/sprint-*.md`.
3. Git activity since yesterday, all branches:
   ```
   git log --all --since="yesterday 00:00" --pretty=format:"%h %ad %an %s" --date=short
   ```
4. `production/qa/bug-inbox.md` (open notes) and `production/session-state/routine-log.md`.

---

## Steps

### S. Sync the owner's latest commits into `sprint-NN` (run before everything else)
Without this, the standup reads `origin/sprint-NN` only and misses work the owner committed on
another branch that day (owner decision 2026-10-31). Only **committed** work is merged; the owner's
uncommitted changes stay invisible. One command per tool call, literal paths (protocol §2).

1. `git -C D:/Fork/Game-RPG-pm fetch origin --prune`, then `git -C D:/Fork/Game-RPG-pm status --porcelain`
   (must be empty, else stop the sync and report).
2. Find the owner's branch: `git -C D:/Fork/Game-RPG-pm worktree list --porcelain` → the `branch
   refs/heads/<name>` line of the `D:/Fork/Game-RPG` entry. The worktrees share one repository, so
   the owner's **local** branch ref is readable from the PM worktree, pushed or not.
3. **Skip** (say `SYNC SKIPPED — <reason>` in the digest) when: that entry is `detached`; the branch
   is `main` / `master` or starts with `pm/`; or
   `git -C D:/Fork/Game-RPG-pm merge-base --is-ancestor <name> origin/sprint-NN` exits 0 (nothing new).
4. `git -C D:/Fork/Game-RPG-pm checkout --detach origin/sprint-NN`, then:
   - `<name>` is `sprint-NN` itself → `git -C D:/Fork/Game-RPG-pm merge --ff-only sprint-NN`
     (not fast-forwardable → `SYNC SKIPPED — local sprint-NN diverged from origin`).
   - otherwise → `git -C D:/Fork/Game-RPG-pm merge --no-ff <name> -m "Merge <name> into sprint-NN (standup sync <YYYY-MM-DD>)"`.
5. **Conflict** → `git -C D:/Fork/Game-RPG-pm merge --abort`, `git -C D:/Fork/Game-RPG-pm checkout --detach origin/sprint-NN`,
   add an inbox note (Source `standup`, Sev guess S2, the conflicting paths), and report
   `SYNC FAILED — conflict` as the first line of the digest. Never resolve a conflict; never edit a file.
6. Success → `git -C D:/Fork/Game-RPG-pm push origin HEAD:sprint-NN` (rejected → protocol §3
   "Push rejected"; never force-push). List what came in with
   `git -C D:/Fork/Game-RPG-pm log --oneline origin/sprint-NN..<name>` (run it **before** the merge) for step 1.
7. The rest of the run (tracker, routine commit) works on this new HEAD. Exception to protocol §1:
   the merge commit carries the owner's committed non-`.md` files — allowed for this step only; the
   routine still never **edits** a non-`.md` file.

### 0. Runtime evidence (run first)
- `bash .claude/scripts/compile-check.sh` — headless compile when no Unity Editor is running.
  `FAILED` → add an inbox note (Source `compile-check`, Sev guess S1, first `error CS` line) **and**
  put it as the first line of the digest. `SKIPPED` / `NOT RUN` → say so; do not treat as pass.
- `bash .claude/scripts/editor-log.sh` — the owner's last Editor sessions. Each compile error or
  exception with an `Assets/Script/` frame that is **not already in the inbox** → one note
  (Source `editor-log`, tier `LOG`).

### 1. Reconstruct yesterday
- From commits + tracker + daily log: what was completed, started, stalled.
- No commits but planned work → flag it; do not assume progress.

### 2. Fix-survival spot check (conditional)
- If any of yesterday's commits touched a `.cs` file that a bug fix of the last 7 days touched,
  run `bash .claude/scripts/fix-survival.sh 7`. Each `GONE` / `NOT ANCESTOR` / `PARTIAL` row not
  already noted → one inbox note (Source `fix-survival`). Never reopen a bug here.

### 3. Summarize · analyze · evaluate (yesterday)
- **Summary**: what was done (tasks moved, commits, bugs touched).
- **Analysis**: estimate burned vs planned; what slipped and why.
- **Evaluation**: `ON-TRACK` / `SLIPPED` / `BLOCKED` with the single biggest reason, plus the
  evidence tier it rests on.

### 4. Update the tracker (persistent memory)
Edit `sprint-*-daily-plan.md` minimally: task statuses (⬜ 🟡 ✅ ⏸️ ✂️), burn summary, days vs work
remaining, Status Verdict, a dated Daily Log entry.

### 5. Today's plan
- Today's tasks from the sprint file's day-by-day breakdown, priority order, per-task estimate
  (days) and a one-line "why now"; carry over unfinished work; exactly **one** focus recommendation.
- **Recurring nudges**: print every open risk tagged "DAILY NUDGE" / "raise every standup" until it
  is closed. `⚠️ Watch` is the single newest risk without a standing nudge.

### 6. Friday only — prepare the playtest sheet
- `bash .claude/scripts/playtest-sheet.sh <saturday YYYY-MM-DD> > production/qa/playtests/playtest-<saturday>.md`
  (do not overwrite a sheet that already has results filled in).
- Mention it in the digest: "Phiếu chơi thử Thứ Bảy đã sẵn: <path> (~30 phút)".

### 7. Persist
- Append the run-log row; commit routine output per the protocol
  (`chore(standup): daily standup <YYYY-MM-DD>`), push to `sprint-NN`.

### 8. Output (chat, Vietnamese) — under ~30 lines
```
📋 Standup — <weekday> <date>      evidence: <STATIC|LOG|COMPILED>
🔴 <SYNC FAILED / compile FAILED line — only if one failed>
🔀 Sync: merged <branch> (<N> commits) into sprint-NN <hash> | SYNC SKIPPED — <reason>

⏪ Yesterday
  • Summary:    …
  • Analysis:   <burned X/Y d, slipped …>
  • Verdict:    ON-TRACK | SLIPPED | BLOCKED — <reason>

🧪 Runtime: compile <COMPILED|FAILED|SKIPPED> · Editor log <N errors / M exceptions> · days since last playtest: <N>
📥 Inbox: <N open notes> (+<new today>)

📊 Sprint: <verdict> — <days left>d left / <work>d remaining

🎯 Today (priority order)
  1. <task> (<est>d) — <why now>
  💡 Focus: <one recommendation>

🔁 Standing decisions (nudge until closed)
  - …
  ⚠️ Watch: …

🧾 Commit: <hash> on sprint-NN
```

---

## Write approval
- Interactive run (no `--auto`): present the digest and findings first, then ask "May I write the tracker, inbox notes, run log (and on Friday the playtest sheet) and commit?" before any write.
- `--auto` (scheduled routine): write directly — the owner pre-approved `.md`-only writes for routines on 2026-10-09 (`.claude/docs/pm-routine-protocol.md`).

## Next step
- Next: the owner works today's list; Friday's sheet is played on Saturday; `/weekly-wrapup` (Sat 22:00) triages the inbox.

## Language
Chat in Vietnamese with key English terms in parentheses on first use
(`.claude/rules/language-reporting.md`). Stored `.md` files stay **English only**.

## Do not
- Do not write or modify `.cs` or any game asset; do not stage anything but `*.md` (the step S merge
  commit is the only commit that may carry the owner's non-`.md` files).
- Do not resolve a merge conflict or merge any branch other than the owner's current one.
- Do not create bug files or allocate bug IDs — inbox notes only.
- Do not check out branches in `D:/Fork/Game-RPG`.
- Do not invent progress that git/tracker doesn't support.
