# Bug Triage — 2026-11-07 (weekly wrap-up)

> Routine `pm-weekly-wrapup` (`/weekly-wrapup --auto`), unattended. Sprint 21, `origin/sprint-21`
> `acacdf6c`. Evidence tier: **RUNTIME (automated QA only) + LOG + STATIC**. No owner playtest sheet
> was filled (`production/qa/playtests/playtest-2026-11-07.md` is blank).

## Inputs

| Input | Result |
|---|---|
| Week's commits | 12 owner / interactive game commits since the 2026-11-01 kickoff; newest `d2d45973` "coding need check" (2026-11-07, auto-committed from the owner's tree by QA run `20261107-1437`) |
| Playtest sheet | NOT RUN — sheet present, every Result cell empty |
| Automated QA | Run `20261107-1437` on `5a3256e7` (Sat 14:37, later than the Friday slot): PASS 76 / FAIL 2 / KNOWN 9 / SKIP 20 / OBSERVED 4 (111). New FAIL 1 (QA-STATIC-01), still failing 1 (QA-VIT-07), fixed 1 (QA-ABL-01.Paladin.Utility), flaky 0 |
| `editor-log.sh` | 0 `error CS`; 21 Play Mode entries (owner's uncommitted tree); NRE @ `PlayerState.cs:42` (5×), `AttackFlowDebugger.cs:162/241` (7×), `BaseStatsSO.cs:243` (3×) → NOTE-20261107-2 / -3 |
| `fix-survival.sh 30` | 1 row: BUG-107 `3d768b3d` PARTIAL (49%) — later rewrites; substance present (BUG-107 now FIXED in code) |
| `bug-id.sh audit` | Next ID before triage `BUG-122`; 61 files; 6 historical pre-register IDs cited (expected); 0 files without `**Status**`; 0 CLAUDE.md status mismatches |
| `doc-claims.sh` | 699 claims, 593 TRUE, 106 STALE (mostly history entries, `ON_CLEAR_ENEMY` in GDDs / ADR-0002) → `/doc-sync` 23:00 |

## Inbox outcomes (24 notes: 20 open + 4 written tonight)

| Outcome | Count | Notes |
|---|---|---|
| Confirmed → new bug | 3 | NOTE-20261101-1 → **BUG-122** (S1) · NOTE-20261031-10 → **BUG-123** (S3) · NOTE-20261107-1 → **BUG-124** (S3) |
| Resolved in week | 7 | NOTE-20261101-2, -3, NOTE-20261104-1, -2, -5, -6, NOTE-20261105-4 |
| Duplicate | 4 | → BUG-108, BUG-066, BUG-102, BUG-121 |
| Not a bug / by design | 4 | NOTE-20261103-3 (doc drift), NOTE-20261105-1/-2 (triage hints, acted on), NOTE-20261105-3 (owner decision) |
| Not reproduced — closed | 1 | NOTE-20261103-1 |
| Needs owner (carried) | 5 | NOTE-20261104-3, -4 (balance / damage model, owner deferred), NOTE-20261107-2, -3 (stack traces), NOTE-20261107-4 (QA finding list) |

ID allocation note: `bug-id.sh next` reads `production/qa/bugs/` relative to the working directory
(the owner's folder), which does not yet contain the new files, so it returned `BUG-122` each time.
BUG-123 and BUG-124 were allocated by hand as the next consecutive numbers, after checking that no
`BUG-12[2-9].md` exists in either the owner's folder or `origin/sprint-21`.

## Bug status changes (code / data re-read at `acacdf6c`)

| Bug | Was | Now | Evidence |
|---|---|---|---|
| BUG-105 | Open | FIXED in data — awaiting runtime | `fbab752a`: `SnS_State1-3.asset:17` `attackDamage: 55` |
| BUG-106 | Open | PARTIAL | Enemy half `6094aa10`, QA-ENT-02 / QA-ENT-08 PASS; player half (Champion-only) open |
| BUG-107 | PARTIAL | FIXED in code — awaiting runtime | `1370136c`: `WeaponHolderBase.cs:46-61` |
| BUG-108 | Open | PARTIAL | Stats key restored `fc98fa2a`; Knight `weaponSO:` still stored, unread |
| BUG-115 | Open | PARTIAL | Runtime clone `1370136c`; QA-STATIC-04 still finds leaked modifiers in Archer / Mage stat assets |
| BUG-116 | Open | FIXED in code — awaiting Player build | `1370136c`: `CharacterData.cs:23` `RuntimeAnimatorController`, no `UnityEditor` |
| BUG-121 | Open | FIXED in code and data — awaiting runtime | `1370136c` + `fbab752a` |

Evidence appended without a status change: BUG-066, BUG-102. Statuses follow the existing wording;
the "Fixed → Closed needs runtime" lifecycle is still pending an owner decision
(`.claude/rules/bug-inbox.md`).

## Priority vs severity

| Bug | Sev | Priority | Why |
|---|---|---|---|
| **BUG-122** | S1 | P1 | Player death never reaches Game Over; blocks the death half of the demo loop and makes BUG-086 / BUG-087 unreachable. One clip-event or one-line state fix |
| BUG-096 | S1 | P1 | Boss room still sealed on load (Shop / Rest / Buff fixed in `1370136c`) |
| BUG-097 | S2 | P1 | Run still starts in the boss room (list-position start / end) |
| BUG-108 | S1→ residual | P2 | Remaining half is a dead key; legacy Knight assets |
| BUG-124 | S3 | P2 | One-line ordering fix; every pooled re-spawn |
| BUG-123 | S3 | P3 | Bundle with BUG-120 |

## Blockers for Sprint 22

1. **BUG-122** — player death event (S1). Without it there is no Game Over and no respawn test.
2. **S21-01 playtest** — 24th carry. Seven bugs now sit at "fixed in code, awaiting runtime" on top
   of the sheet's section B; only a hand playtest moves them.
3. BUG-096 (Boss room) / BUG-097 (start = boss room) — both still on the room path.

## QA automation follow-up

- No bug closed tonight is mapped in `qa-automation/config/known-issues.json` — nothing to remove.
- QA-VIT-07 should be mapped to **BUG-122** (it currently FAILs with no bug), and QA-VIT-08 stays SKIP
  until it is fixed. QA-side work on `qa/weekly-automation`, not done here.
- QA-STATIC-01: publish the 18 new findings (NOTE-20261107-4).
