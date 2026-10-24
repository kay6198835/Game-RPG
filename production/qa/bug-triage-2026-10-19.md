# Bug Triage — 2026-10-19 (weekly wrap-up, late)

> Run: `/weekly-wrapup --auto`, scheduled Sat 2026-10-17 22:00, fired **Mon 2026-10-19 ≈08:40** — after
> the Monday kickoff (`2634dbb9`, opened `sprint-19`) and the Monday standup (`1a9fa5e2`). Code read at
> `origin/sprint-18` `0def62ab` (owner merge of `cae75317`, 2026-10-19 08:44), the newest code on any
> sprint branch; `sprint-19` does not contain it yet.
> Evidence tier: `COMPILED` (standup batchmode on `0def62ab`) + `LOG` + `STATIC`. No `RUNTIME`:
> `playtest-2026-10-17.md` is empty.

## Inbox outcomes

| Note | Outcome | Result |
|---|---|---|
| NOTE-20261013-1 | Resolved in week | `cae75317` / merge `0def62ab` — compiles; still open on `sprint-19` (S19-02) |
| NOTE-20261013-2 | Resolved in week | `cae75317` — injects `IObjectPoolService` |
| NOTE-20261013-3 | Confirmed | **BUG-106** S2 |
| NOTE-20261015-1 | Needs owner | carried — needs Console stack trace |
| NOTE-20261019-1 | Confirmed | **BUG-107** S2 |
| NOTE-20261019-2 | Needs owner | carried — Editor-time, source unclear |
| NOTE-20261019-3 (new, wrap-up review) | Confirmed | **BUG-108** S1 |
| NOTE-20261019-4 (new, wrap-up review) | Confirmed | **BUG-109** S2 |
| NOTE-20261019-5 (new, wrap-up review) | Not a bug | renames, doc drift → `/doc-sync` |

Totals: 9 notes → 4 confirmed (BUG-106…109), 2 resolved, 0 duplicate, 1 not a bug, 2 needs owner.

ID allocation: `bash .claude/scripts/bug-id.sh next` returned `BUG-106` (max file `BUG-105` in both
the owner folder and the PM worktree). The script counts files in its working directory — the owner's
folder — so re-running it after each new file in the PM worktree cannot advance; IDs 107-109 were
assigned consecutively by hand after checking that no `BUG-106…109.md` exists in either folder.

## Priority vs severity — new bugs

| Bug | Sev | Pri | Why this priority | Suggested slot |
|---|---|---|---|---|
| BUG-108 | S1 | 1 | Every Knight enemy throws on its first stat read once `0def62ab` reaches `sprint-19`; data still recoverable on disk | Must — before or with S19-02 (the merge brings it in) |
| BUG-106 | S2 | 1 | Enemies never attack; player unarmed until Champion pick | Must — same session as BUG-107 (same two files) |
| BUG-107 | S2 | 1 | NRE on second Champion pick / Mage pick | Must — two-line fix, bundle with BUG-106 |
| BUG-109 | S2 | 2 | New Archer ability only; not on the demo path | Should — with S19-F1 |

## Blockers for the current sprint (Sprint 19)

1. **S19-02 merge** brings both the build fix *and* BUG-108 / BUG-106 / BUG-107 onto `sprint-19`.
   Merge and fix in one session, or the first smoke after the merge will see enemies throw and never
   attack — S19-01 would then record FAILs that are already known.
2. S19-01 playtest (16th carry) — no runtime evidence exists for any `FIXED in code` bug.

## Systemic trend

- **Serialized-field renames without `[FormerlySerializedAs]` — second occurrence** (BUG-105 sword
  stages, BUG-108 enemy data). Both renamed in code and re-saved only some assets. The check exists as
  review-flow R3.5 but nothing runs it before a push. Candidate for the S19-08 process gate.
- **Fourth sprint-branch build break** (BUG-088, BUG-092, `f552f701`, and `sprint-19` cut from a broken
  tip). Same root cause: no pre-push compile check (TD-048 / TD-051).
- Owner work keeps landing as large mixed commits (`cae75317`: 137 files, 20 `.cs`, assets + fixes +
  feature) on a branch literally named `origin/feature/ui-flow-maingameplay`, then merged straight to
  the sprint branch.

## Fix survival (30 d, `fix-survival.sh` at `0def62ab`)

14 PRESENT / 5 PARTIAL / 1 GONE. All non-PRESENT rows were triaged before (2026-10-09: BUG-076/078
`73ab8e77`, BUG-093/094 `b7a0af5e` superseded rewrites; BUG-096 `40d2c793` reverted by `ac13ee4f`).
New since last week: **BUG-082** `c0067f47` at 77% PARTIAL — re-read at `0def62ab`: all six lists in
`AbilityDefinition` / `AbilityEffectDefinition` still carry `= new()`, so the fix's substance is
present; no note. BUG-064's fix (`5b035b73`) survives `cae75317`'s `WeaponHolderBase` rewrite:
`Equid_UnEquid()` still calls `resolver?.InjectGameObject`.

## Status changes

None. No playtest sheet was filled, so no section-B bug can move.
