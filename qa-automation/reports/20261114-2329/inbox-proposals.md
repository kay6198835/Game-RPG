# Inbox proposals — QA run 20261114-2329

Sprint `origin/sprint-22` at `a8725a77d2`. Written to `production/qa/bug-inbox.md` (Open notes) on
`sprint-22`. No bug ID is allocated here; `/weekly-wrapup` triages these rows.

| Note | Source | Sev guess | Where | What | Evidence tier |
|---|---|---|---|---|---|
| NOTE-20261114-1 | qa-auto | S2 | `Assets/Script/Weapons/Weapon.cs:67-70` (`38d40925`); `Assets/Script/Character/Base/WeaponHolderBase.cs:90-98`; `Assets/Script/Weapons/MeleeWeapon/MeleeWeapon.cs:33` | QA-CMB-01.Paladin (1 of 1 runs, new FAIL): a stage-1 sword hit dealt 72.4; PhysicalDamage 56.4 + stage 8 = 64.4 expected. `WeaponHolderBase.CalculateCurrentDamage()` already adds `CurrentStage.attackDamage`, then `Weapon.FinalDamage()` (new in `38d40925`) adds it again before `TakeDamage()`, so every melee hit (player and enemy) gets the stage damage twice, and the crit bonus is computed before the second add. Report `qa-automation/reports/20261114-2329/report.md` | RUNTIME |
| NOTE-20261114-2 | qa-auto | S2 | `Assets/Script/System/Item/ItemController.cs:37-40`; item effect assets bound to the abstract `StatModifierEffect` script | Relates to BUG-120 (Open). QA-ITEM-03 (1 of 1 runs, new case): pressing Q on the dropped item throws `NullReferenceException` at `ItemController.cs:39` every frame Q is held (90 in the run); the item is never collected and `ON_COLLECT_ITEM` never fires. The item's one effect loads as null (BUG-120 asset bound to an abstract script, reachable since `094ec9df` migrated the `ItemSO` keys) and `Interact()` has no null-element guard, so one bad effect blocks the whole pickup. Case mapped KNOWN to BUG-120. Report `qa-automation/reports/20261114-2329/report.md` | RUNTIME |
| NOTE-20261114-3 | qa-auto | S3 | `Assets/SO/Weapons/MageWeapon/Mage Attack SO State 1.asset:16,24-26` (and States 2, 3; `0f3644bd`) | Mage basic attack: `ProjectileConfig.speed: 1`, `lifetime: 4`, `attackRange: 10`, so a MagicBolt travels at most 4 units and needs 2.5 s for a target 2.5 units away. QA-CMB-04.Mage (1 of 1 runs): no bolt reached the target within 2.5 s, one bolt still active 2 s later; QA-CMB-05.Mage: 0 shots landed in 2 s. Both were reported KNOWN under BUG-095 / BUG-103 (Archer `Arrow.prefab`), which does not apply: `MagicBolt.prefab` has a trigger CircleCollider2D. The BUG-095 mapping is narrowed to the Archer in this run's `known-issues.json`. Report `qa-automation/reports/20261114-2329/report.md` | RUNTIME |
| NOTE-20261114-4 | qa-auto | S3 | `Assets/TextMesh Pro/Examples & Extras/Scripts/TMP_TextInfoDebugTool.cs:4` | QA-STATIC-05 (1 of 1 runs): `using UnityEditor;` outside `#if UNITY_EDITOR` (the class body is guarded from `:14`). The file compiles into `Assembly-CSharp`, which ships, so a Player build is expected to fail on it. It was hidden under the BUG-116 mapping until BUG-116 was set Fixed. Fix: move the `using` inside the `#if`, or delete `Examples & Extras`. Not checked with a Player build | STATIC |
| NOTE-20261114-5 | qa-auto | S4 | 17 `EntityData` assets: `Assets/SO/Enemy/Role/*/*EntityData.asset`, `Assets/SO/Enemy/Enemy - Knight*.asset` (`a0e250fb`) | QA-STATIC-03 (1 of 1 runs): 58 new orphaned keys — `idleDurationTime`, `moveDurationTime`, `rangeView` (16 assets each), `angleView` (10). `a0e250fb` removed these fields on purpose ("view/timer values are computed from the enemy's own stats"), so nothing loads into them any more. Check that the computed values match what was authored, then re-save the assets to drop the keys | STATIC |

## Added to an existing note

- **NOTE-20261107-4** (QA-STATIC-01, Needs owner: "needs the full finding list"): run `20261114-2329` lists 19
  findings outside the baseline — 18 capture scenes `Assets/Sprite/{Maw,Vampire,Warrok}/Scenes/*_8dir_*.unity`
  (`*_8dir_pixel.unity` and `*_{idle,move,attack,takedamage,death}_8dir_capture.unity`) that reference a script
  that is not in the repo (the capture tool), plus `Assets/SO/Skill/ShootSpirit/MainEffect/ShootSpirit.asset`
  (BUG-073). Full list in `results.json` of the run.

## Mapped to an open bug (`known-issues.json`), no note

- QA-VIT-07 → BUG-122 (still FAIL: `ON_PLAYER_DEATH` not emitted, Die clip carries only `AnimationOnAction`).
- QA-ITEM-03 → BUG-120 (see NOTE-20261114-2 for the pickup NRE).

## Test defects fixed (no note)

- QA-ABL-01.Archer.Ultimate: Rain Arrows (`DamagesInManyPos`) only hits authored points of radius 0.3, so one
  target 2.5 units ahead can sit between points. The case now records a note for scattered summons; the pattern
  is checked by hand (MAN-ABL-05).
- QA-STATIC-05: `Assets/Tests/EditMode/TestSupport.cs` is in the Editor-only `GameRPG.EditModeTests` assembly;
  `Assets/Tests/` is now skipped.
