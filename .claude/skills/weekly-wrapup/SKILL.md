---
name: weekly-wrapup
description: "Saturday 22:00 weekly wrap-up for the solo PM-assistant workflow. Closes the working week: reviews the week's changed .cs (code-review), logs the playtest, triages bugs, runs a light retrospective and scope-check, updates the daily tracker with a final weekly verdict, and produces the carry-over + velocity inputs that Sunday's /weekly-kickoff consumes. Wire it to a 22:00 Saturday routine."
argument-hint: "[week end date YYYY-MM-DD, blank = today]"
user-invocable: true
allowed-tools: Read, Glob, Grep, Bash, Edit, Write
model: sonnet
---

# Weekly Wrap-Up (Saturday 22:00)

You are the owner's **PM assistant**. This runs Saturday night (22:00) to close
the working week (Mon–Fri all done). You **never write game code** — you read
code (to review it) and edit/create production planning `.md` files only.

> **Hard rule**: NEVER touch `.cs` or anything outside `production/`, `design/`,
> `docs/`, `.claude/`. The owner does all coding. You review, evaluate, record.

---

## Inputs
1. Current tracker: newest `production/sprints/sprint-*-daily-plan.md`.
2. Current formal plan: matching `production/sprints/sprint-*.md`.
3. The week's commits and changed files:
   ```
   git log --all --since="last monday" --pretty=format:"%h %ad %s" --date=short
   git diff --stat "@{last monday}" -- '*.cs'
   ```
4. Open bugs / past playtests: `production/qa/`.

---

## Steps

### 1. Code review of the week's changes
- List the `.cs` files changed this week. Review the most complex/risky ones
  for: coding-standard compliance (`.claude/rules/`), state-machine discipline,
  null-safety, allocation in hot paths, damage-chain correctness.
- Report findings as a short list (file → issue → suggested fix). **Do not edit
  code** — propose only; the owner fixes.
- For anything material, write/append a bug entry under `production/qa/bugs/`.

### 2. Playtest log
- Capture this week's playtest findings as a brief dated note under
  `production/qa/playtests/` (a short entry is fine — consistency over length).

### 3. Bug triage
- Re-prioritize open bugs (severity vs priority). Mark blockers for next sprint;
  defer non-blockers. Record under `production/qa/` (triage note).

### 4. Light retrospective
- What went well / what slipped / one process improvement for next week.
- Name any task deferred multiple weeks (recurring slippage is a pattern to call
  out explicitly, not bury).

### 5. Close the tracker for the week
- Edit the current `sprint-*-daily-plan.md`: finalize task statuses, the burn
  summary, and a **final weekly Status Verdict** (ON-TRACK / SLIPPED / BLOCKED).
- Append a Friday entry to the Daily Log.
- Compute and record the two handoff values for Monday: **carry-over tasks**
  (anything not ✅) and **velocity** (estimate-days actually completed / 4).

### 6. Output (chat) — under ~30 lines
```
🏁 Weekly Wrap-Up (Sat 22:00) — Sprint NN (week ending <Fri date>)

✅ Done this week: <tasks> (velocity <X>/4 d)
⤵️ Carry-over:     <tasks>
🔍 Code review:    <N files reviewed, top finding>
🐞 Bug triage:     <blockers → next sprint>
🧪 Playtest:       <one-line note>
📊 Verdict:        ON-TRACK | SLIPPED | BLOCKED — <reason>
🔁 Retro:          well: … | slipped: … | improve: …

→ Handoff to Monday /weekly-kickoff: carry-over + velocity recorded in tracker.
```

---

## Language
Reply in Vietnamese with key English terms in parentheses on first use
(per `.claude/rules/language-reporting.md`). The `.md` files stay
**English only** (stored-doc rule).

## Do not
- Do not edit `.cs` or assets — propose fixes, the owner implements.
- Do not create the next sprint — that is Sunday's `/weekly-kickoff`.
- Do not invent progress git/tracker doesn't support.
