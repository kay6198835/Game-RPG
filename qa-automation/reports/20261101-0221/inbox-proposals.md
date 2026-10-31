# Bug-inbox proposals — QA run 20261101-0221

Code at `origin/sprint-20` `0a8f24c0` (no runtime change since `09ca5d19`). No new defect: the three
unknown FAILs are the defects already noted in `production/qa/bug-inbox.md` from run `20261101-0150`.
No new row is proposed; the new evidence is added to the existing notes instead
(`.claude/rules/bug-inbox.md`: "if it is already there, add the new evidence to that note").

| Case | Existing note | New evidence |
|---|---|---|
| QA-VIT-07 | NOTE-20261101-1 (S1) | FAIL again, now 4 of 4 runs (`0104`, `0117`, `0150`, `0221`); same detail — death clip `Knight_Die_dir 7` carries only `AnimationOnAction`, `ON_PLAYER_DEATH` not emitted in 4 s |
| QA-ENT-04 | NOTE-20261101-2 (S1) | FAIL again, 4 of 4 runs; HP 0, state `EntityMoveState`, clip `Knight_Walk_dir1`; `EntityBasicState.cs:31-36` unchanged at `0a8f24c0` (HP check only inside `if (entityInput.IsTakeDamage)`) |
| QA-ABL-01.Paladin.Ultimate | NOTE-20261101-3 (S2) | FAIL again, 4 of 4 runs; enters `PlayerSkillWeaponState`, no cooldown event, no stat change in 2.5 s |

Not a test defect and not flaky: identical verdicts in all four runs at fixed 60 fps and fixed seed.
They stay FAIL (not KNOWN) until `/weekly-wrapup` gives them bug files.
