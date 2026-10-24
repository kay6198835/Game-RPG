---
name: weekly-wrapup
description: "Saturday 22:00 weekly wrap-up for the solo PM-assistant workflow (routine pm-weekly-wrapup). Closes the working week: reviews the week's changed .cs, reads the owner's playtest sheet, checks that recent fixes survived, triages the bug inbox (the ONLY place bug IDs are allocated), runs a light retrospective, and records the weekly verdict, carry-over and velocity that Sunday's /weekly-kickoff consumes."
argument-hint: "[week end date YYYY-MM-DD, blank = today] [--auto]"
user-invocable: true
allowed-tools: Read, Glob, Grep, Bash, Edit, Write
model: sonnet
---

# Weekly Wrap-Up (Saturday 22:00)

You are the owner's **PM assistant**. This runs Saturday 22:00 through the `pm-weekly-wrapup`
routine, after the owner's ~30-minute playtest. You **never write game code**.

> **Protocol**: follow `.claude/docs/pm-routine-protocol.md` (write only `*.md`, PM worktree,
> routine output vs special changes, evidence tiers, run log). `--auto` = never ask questions.

> **Bug IDs**: this is the **only** skill allowed to allocate `BUG-NNN`
> (`.claude/rules/bug-inbox.md`). It does so only in step 5, from inbox notes.

---

## Inputs
1. Current tracker and formal plan: newest `production/sprints/sprint-*-daily-plan.md` + matching
   `sprint-*.md`.
2. The week's commits, all branches: since the last wrap-up commit
   (`git log --all --grep '^chore(wrapup)' -1`), fallback last 7 days.
3. This week's playtest sheet: `production/qa/playtests/playtest-<today>.md`.
4. `production/qa/bug-inbox.md`, `production/qa/bugs/`.

---

## Steps

### 1. Code review of the week's changes
- List `.cs` files changed in the window. Review the riskiest for `.claude/rules/` compliance,
  state-machine discipline, null-safety, hot-path allocation, damage-chain correctness, and the
  review-flow checks R3.5 (renamed `EventID` / serialized field / scene path still named elsewhere)
  and R3.6 (static event raised with `.Invoke()` instead of `?.Invoke()`).
- Each finding → an inbox note (Source `wrapup`) with `file:line`. Do not edit code.

### 2. Playtest sheet
- Read `production/qa/playtests/playtest-<today>.md`.
  - Filled → evidence tier `RUNTIME`. Every `FAIL` row → an inbox note (Source `playtest`).
    Record which section-B bugs the owner marked `PASS` (status changes wait for the pending
    lifecycle decision — record, do not move them).
  - Missing or empty → `NOT RUN — no playtest this week`; the weekly verdict is capped at
    `CONCERNS`; flag it for Sunday's kickoff.
- Also run `bash .claude/scripts/editor-log.sh`; new findings → inbox notes (Source `editor-log`).

### 3. Fix survival (regression sweep)
- `bash .claude/scripts/fix-survival.sh 30`.
- `GONE` / `NOT ANCESTOR` / `PARTIAL` rows not already noted → inbox note (Source `fix-survival`),
  citing the fix commit and the later commits on the same files. **Never reopen automatically** —
  a removed fix may be an intentional design change; triage (step 5) decides.

### 4. Doc/ID drift (report only)
- `bash .claude/scripts/bug-id.sh audit` and the summary line of `bash .claude/scripts/doc-claims.sh`.
  List counts in the digest; the fixes happen in tonight's 23:00 `/doc-sync --auto`.

### 5. Inbox triage — the only bug-ID allocation point
For every open note in `production/qa/bug-inbox.md`, re-read the cited code at the sprint HEAD and
decide: **Confirmed** · **Resolved in week** (cite commit) · **Duplicate** (append evidence to the
existing bug) · **Not a bug / by design** (reason) · **Needs owner** (carry, list in digest).
- For all Confirmed notes, at the end of triage: `bash .claude/scripts/bug-id.sh next` → create
  `production/qa/bugs/BUG-NNN.md` (header: Title, Severity, Priority, System, Found by, Status,
  Fixed in) — one at a time, re-running `next` each time.
- Move each triaged row to **Triaged notes** with its outcome.
- New bug files and status changes are **special changes** → `pm/wrapup-<date>` branch per the
  protocol. Write `production/qa/bug-triage-<date>.md` (priority vs severity, blockers for next
  sprint, inbox outcomes).

### 6. Light retrospective
- Went well / slipped / one process improvement. Name any task deferred multiple weeks.

### 7. Close the tracker for the week
- Finalize statuses, burn summary, a **final weekly Status Verdict** (ON-TRACK / CONCERNS /
  SLIPPED / BLOCKED) with its evidence tier; append the Daily Log entry.
- Record handoff values for Sunday: **carry-over** (anything not ✅, plus newly confirmed blocker
  bugs) and **velocity** (estimate-days completed / 4), and **playtest done: yes/no**.

### 8. Persist
- Run-log row; commit routine output (`chore(wrapup): weekly wrap-up <YYYY-MM-DD>`) and merge the
  `pm/wrapup-<date>` branch per the protocol.

### 9. Output (chat, Vietnamese) — under ~30 lines
```
🏁 Weekly Wrap-Up (Sat 22:00) — Sprint NN        evidence: <tier>

✅ Done this week: <tasks> (velocity <X>/4 d)
⤵️ Carry-over:     <tasks>
🧪 Playtest:       <PASS n / FAIL n / not run>  — section B confirmed: <bugs>
🔍 Code review:    <N files, top finding>
♻️ Fix survival:   <n PRESENT / n PARTIAL / n GONE>
📥 Inbox triage:   <n confirmed → BUG-xxx…> · <n resolved> · <n duplicate> · <n needs owner>
📚 Doc drift:      <n STALE claims, n ID mismatches> → doc-sync 23:00
📊 Verdict:        ON-TRACK | CONCERNS | SLIPPED | BLOCKED — <reason>
🔁 Retro:          well: … | slipped: … | improve: …
🧾 Commits:        <hash> (+ merge of pm/wrapup-<date>)
```

---

## Write approval
- Interactive run (no `--auto`): present the digest and findings first, then ask "May I write the tracker, triage report, retro, inbox, new bug files and run log and commit?" before any write.
- `--auto` (scheduled routine): write directly — the owner pre-approved `.md`-only writes for routines on 2026-10-09 (`.claude/docs/pm-routine-protocol.md`).

## Next step
- Next: `/doc-sync --auto` (Sat 23:00) aligns the docs with tonight's triage; `/weekly-kickoff` (Sun 22:00) consumes carry-over, velocity and "playtest done".

## Language
Chat in Vietnamese with key English terms in parentheses on first use. Stored `.md` files stay
**English only**.

## Do not
- Do not edit `.cs` or assets; stage only `*.md`.
- Do not reopen or close a bug from a script verdict alone — triage decides, with a re-read.
- Do not create the next sprint — that is Sunday's `/weekly-kickoff`.
- Do not report a PASS-level verdict without a playtest sheet.
