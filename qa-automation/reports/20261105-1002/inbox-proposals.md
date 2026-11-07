# Inbox proposals - QA run 20261105-1002

Weekly QA run on `origin/sprint-21` `04cfb53d` (re-runs `20261105-1053` at `e22cd6ab`, `20261105-1102` at `4b2133fb`
after part C; no runtime code changed between them). Written to `production/qa/bug-inbox.md` on `sprint-21`.
No bug ID is allocated here.

## New note

| Note | Date | Source | Sev guess | Where | What | Evidence tier |
|---|---|---|---|---|---|---|
| NOTE-20261105-3 | 2026-11-05 | qa-auto | S3 | `Assets/Script/Character/Entity/States/EntityDeathState.cs:13-19`; enemy death clips (e.g. `Assets/Animation/Enemy/Role/*/Death/*_Die_dir 0-7.anim`, `Knight_Die_dir 0.anim`: `m_LoopTime: 1`, one `AnimationOnAction` event) | QA-ENT-04 (3 of 3 runs, `sprint-21` `04cfb53d` / `4b2133fb`): a lethal hit now sends the enemy to `EntityDeathState` and emits `ON_ENEMY_DEATH` (the NOTE-20261101-2 fix works at runtime), but the event is emitted **2 times** within 1 s while the enemy is not released. `LogicUpdate()` emits on every `StatusAnimation.OnActivate`, and the death clips loop, so the event repeats once per loop (0.9 s). In a dungeon room it is masked only while `EnemySpawner.ReleaseEnemy()` releases the enemy synchronously on the first emit; any enemy not released that way (CombatTest spawner, a failed release) decrements `RoomCell.EnemyCount` and drops items again. Enemy-side twin of BUG-086. Report `qa-automation/reports/20261105-1002/report.md` | RUNTIME + STATIC |

## Evidence added to existing notes

| Note | Evidence |
|---|---|
| NOTE-20261101-1 | QA-VIT-07 FAIL again, 3 of 3 runs on `sprint-21` (7 of 7 since 2026-11-01): HP 0, `PlayerDeathState` entered, no `ON_PLAYER_DEATH` within 4 s; Paladin die clip still carries only `AnimationOnAction`; `PlayerDeathState.cs` unchanged in `0a8f24c0..04cfb53d` |
| NOTE-20261101-2 | QA-ENT-04 now emits `ON_ENEMY_DEATH` (3 of 3 runs) — the lethal-hit fix in `6094aa10` works at runtime; what remains is the double emission, NOTE-20261105-3 |
| NOTE-20261101-3 | QA-ABL-01.Paladin.Ultimate (Avatar of Light) PASS 3 of 3 runs on `sprint-21` — candidate "Resolved in week" |
| NOTE-20261105-1 | Runtime confirmation: QA-ENT-07 PASS (enemy MoveSpeed 2.79 = stat, MaxRange 1.5 = `MaxRangeAttack`, detection 5 ≥ MaxRange) and QA-STATIC-07 PASS (10 role `EntityData`: weapon set, MoveSpeed > 0, MinRange < MaxRange ≤ FOV) — NOTE-20261104-1 / -6 fixes hold |
| NOTE-20261105-2 | BUG-106 enemy half: QA-ENT-02 PASS when the enemy is spawned the way `EnemySpawner.ApplyData()` does (`TrashMeleeWeapon` equipped); the suite was updated to that path (test defect fixed, `8b6f6721`) |

## Not written as notes

- QA-STATIC-03 new findings (35): orphan keys of fields deleted on purpose in `6094aa10` (values migrated) — added to `config/static-baseline.txt`.
- QA-MAP-06: Boss room has no spawn marker — covered by BUG-096 (Open); mapped in `config/known-issues.json`.
