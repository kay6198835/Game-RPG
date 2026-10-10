# Bug Triage — 2026-10-10 (weekly wrap-up, Saturday)

> Routine: `pm-weekly-wrapup` (`/weekly-wrapup --auto`). Branch `sprint-18`, HEAD `c6cbc57a`.
> Evidence tier: `LOG` + `STATIC` (no playtest sheet filled; no compile check in this routine).
> Previous triage: `bug-triage-2026-10-09.md` (that wrap-up fired on Friday; this is the scheduled
> Saturday run).

## Context

- No `.cs` commit on any branch since the 2026-10-09 wrap-up (`cdf68555`). The only new commits are PM
  routine output and the kickoff merge of `42a81260` / `221d54be` into `sprint-18` (`c6cbc57a`).
- Playtest sheet `production/qa/playtests/playtest-2026-10-10.md` exists but is **empty** →
  `NOT RUN — no playtest this week`. No section-B bug can move on runtime evidence.
- `editor-log.sh`: two Editor sessions today (16:58, 22:18), one Play Mode entry each, 0 compile errors,
  no exception with an `Assets/Script/` frame.

## Inbox outcomes (4 open notes + 2 carried)

| Note | Outcome | Result |
|---|---|---|
| NOTE-20261009-9 | Resolved in week (status update) | **BUG-064** → FIXED in code, awaiting runtime (`5b035b73`). **BUG-087** → PARTIAL (`cb0de496`: Game Over subscriber + scene-reload respawn; no in-place reset, never run) |
| NOTE-20261009-10 | Confirmed | **BUG-103** S3 — `RangeWeapon.nextFireTime` never read, `RecoveryTime` dead |
| NOTE-20261009-11 | Confirmed | **BUG-104** S4 — unguarded `Debug.Log` concat in `PlayerState.Enter()` |
| NOTE-20261009-12 | Confirmed | **BUG-105** S2 — `attackDamege` → `attackDamage` rename without `[FormerlySerializedAs]`; three sword stages load 0 |
| NOTE-20261009-2 | Not reproduced — closed | No NRE in today's two Editor sessions |
| NOTE-20261009-6 | Needs owner (carried) | Champion-select data swap — design decision |

IDs allocated with `bash .claude/scripts/bug-id.sh next`, one at a time: BUG-103, BUG-104, BUG-105.
These close the three "unfiled defects" recorded in BUG-101.

## Priority vs severity — new and changed

| Bug | Sev | Priority | Why |
|---|---|---|---|
| BUG-105 | S2 | **1** | One attribute restores the player's main damage source; affects every sword hit in every playtest. Recommend adding to Sprint 18 Must-Have beside S18-04 (same Editor session: open and re-save `SnS_State1-3.asset`) |
| BUG-103 | S3 | 3 | Tuning knob dead; ranged weapon only. Bundle with S18-04 (Arrow / ranged checks) |
| BUG-104 | S4 | 4 | Console flood hides real warnings during the playtest — cheap; bundle with any `PlayerState` edit |
| BUG-064 | S1→ fixed in code | — | Needs playtest step A6 to close |
| BUG-087 | S1 (partial) | 1 | Remaining scope is S18-14 (design note) after S18-07 (BUG-065 + BUG-086) |

## Blockers for next sprint (Sprint 18, starts 2026-10-12)

1. **No runtime evidence for 15 weeks** — S18-01 playtest first, Monday. Every "fixed in code" bug
   (BUG-064, 082, 085, 088, 091-095) waits on it.
2. BUG-096 (S1, rooms without spawn markers stay sealed) — start-room half now on `sprint-18`
   (`221d54be` merged in `c6cbc57a`); zero-spawn branch still missing in `LoadRoom()`.
3. BUG-105 (S2) — new, one line, should ride with S18-04.

## Fix survival (`fix-survival.sh 30`)

16 PRESENT · 4 PARTIAL (BUG-076/078 on `73ab8e77`, BUG-093/094 on `b7a0af5e`) · 1 GONE (BUG-096 on
`40d2c793`). All already triaged on 2026-10-09 (NOTE-1 → BUG-096 duplicate; NOTE-8 → superseded
rewrites). No new notes.

## ID / doc drift (report only — fixed by tonight's `/doc-sync --auto`)

- `bug-id.sh audit`: next free `BUG-106` after this triage; 0 files without a Status line; 0 CLAUDE.md
  status mismatches. IDs cited without a file: BUG-010, 033, 042, 043, 044, 046 (historical, below
  BUG-052 — allowed) and BUG-103 in `.claude/rules/bug-inbox.md` (an example; the file now exists).
- `doc-claims.sh`: 587 claims, 495 TRUE, **92 STALE**.
- For doc-sync: CLAUDE.md Known Bugs needs rows for BUG-103/104/105 and the new BUG-064 / BUG-087
  statuses; `.claude/rules/scriptableobject-data.md` and the doc-sync skill should cite BUG-105 where
  they say "BUG-095" for the `attackDamege` lesson.
