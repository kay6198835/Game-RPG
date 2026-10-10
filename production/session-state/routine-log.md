# Routine Run Log

> One line per run of a scheduled PM routine, appended by the routine at the end of its run.
> `/daily-standup` reads the last 7 days and reports any missed slot.
> Expected slots: standup Mon–Fri 02:00 · wrap-up Sat 22:00 · doc-sync Sat 23:00 · kickoff Sun 22:00 ·
> module audit first Monday 22:00.

| Date (local) | Routine | HEAD (`origin/sprint-NN`) | Status | Evidence tier | Commit | Note |
|---|---|---|---|---|---|---|
| 2026-10-09 | daily-standup | `ec44e9be` | OK | COMPILED | (this commit) | Fri; compile PASS on `221d54be`; +1 inbox note (NOTE-4); playtest sheet 2026-10-10 generated; Must-Have 0/8 |
| 2026-10-09 | weekly-wrapup | `9706c6f4` | PARTIAL | COMPILED + LOG (no RUNTIME) | `9706c6f4`, `386083ca` (merge `662383d9`), (this commit) | Fired Fri 17:25, not Sat; playtest sheet empty → SLIPPED, playtest done: no; 8 notes triaged → BUG-102; velocity 0.05 |
| 2026-10-09 | doc-sync (`pm-weekly-doc-truth`) | `cdf68555` | PARTIAL | STATIC | merge on `sprint-17` (see branch `pm/doc-sync-2026-10-09`) | Phase 0: 567 claims, 45 flagged → 13 STALE / 5 FALSE overall; CLAUDE.md bug table aligned to files (BUG-064/087/093-095, +096-102); BUG-099 claims fixed; +4 inbox notes (9-12); 3 left Out of date. PARTIAL: protocol + scripts read from unmerged `42a81260`, not on `sprint-17` |
| 2026-10-09 | weekly-kickoff | `d8962b8d` | PARTIAL | STATIC | `d8962b8d` (close S17 on `sprint-17`), (this commit) | Fired Fri, not Sun; Sprint 17 closed SLIPPED (0/8 Must, velocity 0.05); `sprint-18` created on remote from `sprint-17` tip; plan 1.3d Must + 1.0d Should + 0.5d feature cap; playtest first Mon; PARTIAL: `gh` missing → no draft PR, protocol read from unmerged `42a81260`, `tests/smoke/critical-paths.md` absent |
| 2026-10-10 | weekly-wrapup | `c6cbc57a` | OK | LOG + STATIC (no RUNTIME) | `ad0a7dd2` (branch `pm/wrapup-2026-10-10`, merged), (this commit) | Sat 22:00 slot; 0 `.cs` changes since Fri wrap-up; playtest sheet empty → SLIPPED, playtest done: no; 4 notes → BUG-103/104/105 + BUG-064/087 status; velocity 0.05 |
