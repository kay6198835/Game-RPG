# Bug Triage — 2026-10-09 (weekly wrap-up, Sprint 17)

> Autonomous `/weekly-wrapup --auto` run, fired Fri 2026-10-09 17:25 local. Window: commits since the
> previous wrap-up (`96c65fa6`, 2026-10-05), all branches. Sprint HEAD at triage: `origin/sprint-17`
> `9706c6f4`. Owner code tip read for review: `221d54be` (feature branches, not on `sprint-17`).
> Evidence tier: `COMPILED` (standup batchmode compile of `221d54be`) + `LOG`. **No `RUNTIME`** — the
> Saturday playtest sheet `production/qa/playtests/playtest-2026-10-10.md` is empty.

## Inbox outcomes

| Note | Outcome | Result |
|---|---|---|
| NOTE-20261009-1 | Duplicate | BUG-096 (evidence appended) |
| NOTE-20261009-2 | Needs owner | NRE @ `PlayerState.cs:35` from an uncommitted file — carry, re-check log |
| NOTE-20261009-3 | Resolved in week | CS0246 transient; compile PASS |
| NOTE-20261009-4 | Not a bug | Doc drift → `/doc-sync` |
| NOTE-20261009-5 | Confirmed | **BUG-102** (new, S3) |
| NOTE-20261009-6 | Needs owner | Champion select swaps `PlayerData` but nothing re-reads it — design decision |
| NOTE-20261009-7 | Not a bug | `GetCoreComponent` convention |
| NOTE-20261009-8 | Not a bug | fix-survival PARTIAL rows are superseded rewrites |

Totals: 1 confirmed · 1 resolved · 1 duplicate · 3 not a bug · 2 needs owner. New ID allocated: **BUG-102**.

### BUG-102 ID note

An interactive project-review session wrote `production/qa/bugs/BUG-102.md` into the owner's working
tree (uncommitted) before this triage, outside `.claude/rules/bug-inbox.md`. `bug-id.sh next` on
`sprint-17` also returned `BUG-102`, and the defect is the same, so the ID was kept. **Owner action:**
delete the local untracked copy before pulling `sprint-17`, otherwise Git refuses the merge
("untracked working tree file would be overwritten"). The owner's uncommitted edit to `BUG-097.md` was
not touched.

## Priority vs severity — open bugs that gate Sprint 18

| Bug | Sev | Priority | Why it gates |
|---|---|---|---|
| BUG-096 | S1 | 1 | Rest / Shop / Buff rooms still sealed on `sprint-17`; start-room half only on a feature branch |
| BUG-097 | S2 | 1 | Run starts in the Boss room; Champion monuments live in the Boss room JSON |
| BUG-072 + BUG-095 (Arrow) | S2 | 1 | Inspector fields; the smoke is meaningless without them |
| BUG-092 residual | S2 | 2 | Every HoT/DoT runs one tick |
| BUG-066 = BUG-070 | S2 | 2 | Unguarded dictionary on the live death path |
| BUG-065 + BUG-086 | S2 | 2 | Death path; precondition for BUG-087 |
| BUG-102 | S3 | 2 | NRE on Champion interact when `GameplayUI` is not loaded; bundle with the start-room work |

No new S1. No bug status was changed by this triage (the status lifecycle is pending an owner
decision; no playtest result to record).

## Systemic trends

- Fourth week running where a fix (BUG-096 / `40d2c79`) or a gate (S17-04 smoke) is undone or bypassed
  by a large feature commit with no Play Mode record.
- New bug IDs are again being written outside the Saturday triage (BUG-102 in the owner's folder) — the
  inbox rule adopted today is not yet known to interactive sessions.
- Static `event Action` bus (`UIEvents`) is growing without the `?.Invoke()` and `ResetStatics()`
  discipline the older members follow (R3.6).

## Fix survival (`fix-survival.sh 30`)

13 PRESENT · 4 PARTIAL · 1 GONE (18 fix rows). The GONE row (BUG-096 / `40d2c793`) was already noted and is BUG-096
itself. PARTIAL rows triaged as superseded rewrites (NOTE-20261009-8).

## Doc / ID drift (report only — fixes in `/doc-sync`)

- `bug-id.sh audit`: 5 historical IDs cited without files (BUG-033…046, pre-register, allowed); 0 bug
  files without `**Status**`; 5 `CLAUDE.md` Known-Bugs rows disagree with their bug file (BUG-064, 087,
  093, 094, 095 — 093…095 are the BUG-101 numbering collision).
- `doc-claims.sh`: 567 claims checked, 45 STALE.
