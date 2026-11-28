# Bug Triage — 2026-11-28 (weekly wrap-up, Sprint 24)

> Routine `pm-weekly-wrapup` (`--auto`), Saturday 22:00 slot. Code re-read at `origin/sprint-24`
> `d3e5418e`. Runtime evidence: automated QA run `20261127-0228` (Friday); the Saturday 00:26 run was
> SKIPPED (Editor open). Playtest sheet `playtest-2026-11-28.md` empty.
>
> Note: `.claude/` is no longer tracked (`c40da7b8`, 2026-11-23) and is not on disk in the owner folder
> or the PM worktree. The skill and protocol were read from git history (`c40da7b8^`); `bug-id.sh` and
> `fix-survival.sh` were run read-only from the copies still on disk in `D:/Fork/Game-RPG-pmko`
> with `REPO=D:/Fork/Game-RPG-pm`.

## Inbox outcomes (9 notes)

| Note | Outcome | Result |
|---|---|---|
| NOTE-20261126-1 | Confirmed | **BUG-136** (S3, P3) — `Trap` mask compare + never `Launch()`ed |
| NOTE-20261127-1 | Duplicate | Evidence appended to BUG-117 |
| NOTE-20261123-1 | Resolved in week (code) + Needs owner (data) | Code `83800891`; 8 of 9 modifier assets have no `impactType` key — kept open |
| NOTE-20261123-2 | Resolved in week | Transient mid-edit compile errors of `df9e98fe` |
| NOTE-20261127-2 | Not a bug / by design | Unbound GDD assets (OQ-10) + empty orphaned keys |
| NOTE-20261128-1 | Not a bug / by design | fix-survival GONE rows = path move / refactor (new note from this run) |
| NOTE-20261115-7 | Needs owner (3rd) | Stats key C |
| NOTE-20261117-4 | Needs owner (3rd) | Move-state spawn / death facing intent |
| NOTE-20261122-1 | Needs owner (1st) | `PlayerManager.Enable()` / `Disable()` uncalled — part of the BUG-087 design note |

Totals: 1 confirmed · 2 resolved (1 code-half) · 1 duplicate · 2 by design · 4 needs owner (incl. the data half of NOTE-20261123-1).

## Status changes

| Bug | Was | Now | Evidence |
|---|---|---|---|
| BUG-135 (S1) | Open | **FIXED** | `e47f5c38`; QA-ENT-08 x10, QA-VIT-06, QA-VIT-15, QA-CMB-01..03.Paladin, QA-ENT-17 PASS (run `20261127-0228`); fix-survival PRESENT |
| BUG-117 (S2) | Open | **FIXED** | `df9e98fe` + `83800891` redesign; QA-ENT-14 PASS (run `20261127-0228`) |
| BUG-136 (S3) | — | **Open** (new) | STATIC re-read |

S1 open count: 6 → **5**. `production/qa/open-bugs.md` regenerated from the bug files.
`CLAUDE.md` is untracked since `c40da7b8`, so its "Bugs:" line was not edited (owner's local copy).

## Priority vs severity — blockers for next sprint

| Bug | Sev | Why next |
|---|---|---|
| BUG-122 remainder + BUG-086 + BUG-065 | S1 | Death → Game Over still not closed; now testable (combat works again) |
| BUG-130 | S2 | Dash soft lock still FAILs at runtime: QA-PLY-06 KNOWN for all 3 classes in run `20261127-0228` despite `b0cfe539` |
| BUG-096 remainder | S1 | Marker-less rooms soft lock (9th carry) |
| BUG-136 | S3 | Only if the trap is meant for the demo |

## Fix survival (30 days, HEAD `d3e5418e`)

18 PRESENT · 10 PARTIAL · 3 GONE · 0 NOT ANCESTOR (rows per commit). PARTIAL rows sit on multi-file
or refactored files (`1370136c`, `0626dd59`, `b5fb3581`, `25d869fb`, `8236f9d1`) touched again by the
week's effect / status work — expected. GONE: BUG-120 `aa045f79` (already triaged 2026-11-21, by
design), BUG-109 `285e6900` and BUG-120 `f528c087` (NOTE-20261128-1, path move / refactor). No reopen.

## Known-issues entries to remove (QA branch)

`qa-automation/config/known-issues.json` has no entry mapped to BUG-135 or BUG-117 — nothing to remove.

## Doc / ID drift (report only)

`bug-id.sh audit`: next free ID `BUG-137` after this triage; 4 IDs cited in living docs without a bug
file (BUG-033, 042, 043, 044 — pre-file-era IDs in GDDs / ADR-0005); 0 files without Status; 0 bug rows
in `CLAUDE.md`; `open-bugs.md` was STALE, regenerated here. `doc-claims.sh` at `d3e5418e`: 440 claims,
385 TRUE, **55 STALE** (was 59 on 2026-11-22; mostly the carried `ON_CLEAR_ENEMY` / old-path set) —
left to doc-sync.

## Editor log

`editor-log.sh`: `Editor.log` 22:14 and `Editor-prev.log` 04:27 — 0 `error CS`, **0 Play Mode entries**,
no `Assets/Script/` exception. No inbox note. Confirms no Play Mode session today.
