---
paths:
  - "**/*.md"
---

# Bug Inbox and Bug IDs

Adopted 2026-10-09 (owner decision). Replaces every earlier practice of allocating a bug ID at the
moment a defect is noticed. Cause: two writers allocated BUG-093…095 independently (BUG-101).

## One rule

**A `BUG-NNN` ID is allocated only by `/weekly-wrapup` on Saturday night.** Every other writer —
`/daily-standup`, `/doc-sync`, `/module-quality-audit`, `/bug-triage`, `/bug-report`, `/code-review`,
any script, any interactive session, the owner — records a suspected defect as a **note** in
`production/qa/bug-inbox.md` and never creates a `production/qa/bugs/BUG-*.md` file.

Why: during the week an issue may be fixed by the owner before anyone triages it. Saturday is the
only point where the week's code, the playtest sheet and the inbox are read together, so it is the
only point where "is this still a bug?" can be answered.

## Writing a note (any day, any writer)

Append a row to the **Open notes** table of `production/qa/bug-inbox.md`:

| Field | Content |
|---|---|
| Note | `NOTE-YYYYMMDD-N` — date of writing + running number for that day. Never reused |
| Source | who wrote it: `standup`, `wrapup`, `doc-sync`, `module-audit`, `compile-check`, `editor-log`, `fix-survival`, `playtest`, `owner`, `session` |
| Severity guess | S1–S4 (a guess; triage decides) |
| Where | `file:line`, asset path, or playtest step |
| What | one or two sentences, evidence first |
| Evidence tier | `STATIC` (read code) · `LOG` (Editor.log) · `COMPILED` (batchmode) · `RUNTIME` (played) |

- Before adding, search the inbox and `production/qa/bugs/` for the same defect; if it is already
  there, add the new evidence to that note or bug instead.
- A compile failure or any S1 suspicion is still only a note, **and** it is the first line of that
  day's standup digest.
- A note may cite an existing bug ID (`relates to BUG-096`); it may never invent one.

## Saturday triage (`/weekly-wrapup` only)

For every open note, decide one outcome and move the row to **Triaged notes** with the decision:

| Outcome | Action |
|---|---|
| **Confirmed** | `bash .claude/scripts/bug-id.sh next` → create `production/qa/bugs/BUG-NNN.md` first, then cite the ID anywhere else |
| **Resolved in week** | the code no longer shows it; record the commit |
| **Duplicate** | append the evidence to the existing bug file |
| **Not a bug / by design** | record the reason |
| **Needs owner** | cannot be decided from code or playtest; carry to next week and list it in the wrap-up digest |

Allocate IDs in one pass at the end of triage, so two notes confirmed in the same run get consecutive
numbers. Three-digit format: `BUG-103`, never `BUG-0103`.

## Citing IDs elsewhere

- Cite only IDs whose file exists. `bash .claude/scripts/bug-id.sh audit` lists violations.
- When `CLAUDE.md` or a doc disagrees with a bug file, the bug file wins; fix the doc, never renumber
  the file.
- Historical IDs below BUG-052 (`#1–#17`, `BUG-033…046`) predate the bug-file register and stay as
  they are.

## Bug status

The status lifecycle (whether "Fixed" needs runtime confirmation before "Closed") is **pending an
owner decision** after the weekly playtest flow has run for a few weeks. Until then, keep the status
words already in use and put the `**Status**` line in the header of every bug file.
