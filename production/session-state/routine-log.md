# Routine Run Log

> One line per run of a scheduled PM routine, appended by the routine at the end of its run.
> `/daily-standup` reads the last 7 days and reports any missed slot.
> Expected slots: standup Mon–Fri 02:00 · wrap-up Sat 22:00 · doc-sync Sat 23:00 · kickoff Sun 22:00 ·
> module audit first Monday 22:00.

| Date (local) | Routine | HEAD (`origin/sprint-NN`) | Status | Evidence tier | Commit | Note |
|---|---|---|---|---|---|---|
| 2026-10-09 | daily-standup | `ec44e9be` | OK | COMPILED | (this commit) | Fri; compile PASS on `221d54be`; +1 inbox note (NOTE-4); playtest sheet 2026-10-10 generated; Must-Have 0/8 |
| 2026-10-09 | weekly-wrapup | `9706c6f4` | PARTIAL | COMPILED + LOG (no RUNTIME) | `9706c6f4`, `386083ca` (merge `662383d9`), (this commit) | Fired Fri 17:25, not Sat; playtest sheet empty → SLIPPED, playtest done: no; 8 notes triaged → BUG-102; velocity 0.05 |
