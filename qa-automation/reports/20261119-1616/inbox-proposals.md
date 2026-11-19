# Inbox proposals - run 20261119-1616

Sprint `origin/sprint-23` at `c613e6417c`. Totals: PASS 163 / FAIL 3 / KNOWN 9 / SKIP 20 / OBSERVED 3; new FAIL 0.

## New notes

None. The only unknown FAILs (QA-PLY-06.Paladin / .Archer / .Mage) are the defect already open as
`NOTE-20261117-3` (no bug file yet, so no `known-issues.json` mapping is allowed).

## Evidence added to existing notes

| Note | Evidence appended |
|---|---|
| NOTE-20261117-3 | Still FAILs in run `20261119-1616` (`sprint-23` `c613e641`): QA-PLY-06.Paladin / .Archer / .Mage 3 of 3 classes, now 2 of 2 runs - player moves 0 and stays in `PlayerDashState` 4 s. The class controllers at `c613e641` still have no Dash state (`Paladin.controller`, `Archer.controller`, `Mage.controller` contain no "Dash"; only `Player.controller` and `Paladin_Dash_Skill.overrideController` do). Same soft lock also seen as carry-over in QA-CMB-04.Archer / .Mage. Report `qa-automation/reports/20261119-1616/report.md` |

## Not proposed (for the record)

- QA-VIT-07 stays KNOWN under BUG-122 at `c613e641`. Owner commits `49c58e35` "fix death flow" and `abdbb332` are on
  `feature/fix-bug-sprint-23`, not yet on `sprint-23`, so the next run after the merge should show whether it is fixed.
- `known-issues.json` hygiene: QA-STATIC-03 is still mapped to BUG-105 / BUG-121 and QA-STATIC-05 to BUG-116, whose
  files say Fixed; the cases report KNOWN through baseline debt / BUG-126 anyway. Left for `/weekly-wrapup`.
- Flaky suspects QA-ENT-08.Tank / .TrashMelee, QA-ENT-09, -12, -13, -15 (one F each in the last 5 runs, all from the
  2026-11-17 enemy rework runs); all PASS in this run. No action.
