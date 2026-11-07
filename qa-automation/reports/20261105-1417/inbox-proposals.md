# Inbox proposals - run 20261105-1417

Sprint `origin/sprint-21` at `8140a56de7`. One unknown FAIL, already covered by an open note.

## New notes

None.

## Evidence added to existing notes

| Note | Case | Evidence | Tier |
|---|---|---|---|
| NOTE-20261101-1 | QA-VIT-07 | FAIL again - 9 of 9 runs since 2026-11-01. `ON_PLAYER_DEATH` not emitted within 4 s of death; death clip `Knight_Die_dir 6`, events `AnimationOnAction` only, while `PlayerDeathState` emits on `StatusAnimation.EndRangeTrigger`. Report `qa-automation/reports/20261105-1417/report.md` | RUNTIME |

## Resolved since the previous run

- QA-ENT-04 PASS (was FAIL in `20261105-1327`): the test now releases the dead enemy like
  `EnemySpawner.ReleaseEnemy()` (owner decision, NOTE-20261105-3 by design).
