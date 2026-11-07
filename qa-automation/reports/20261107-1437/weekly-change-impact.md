# Weekly change impact

- Range: `8140a56de7` (`8140a56de7`) .. `origin/sprint-21` (`5a3256e7dc`)
- 12 commits (+5 merge commits), 1591 files changed
- Systems that get a deep (P2) run this week: **ABL, CMB, ITEM, MAP, PLY, STATIC**
- The test results for these systems are in `report.md` next to this file (section "Changed this week").

## 1. What landed

| Commit | Date | Author | Subject | Files |
|---|---|---|---|---|
| `48994e3c` | 2026-11-07 | Kay | chore(qa-auto): weekly QA run 20261107-1434 - 0 inbox notes 2026-11-07 | 1 |
| `0fa95816` | 2026-11-07 | Kay | test(qa): weekly automation run 20261107-1434 - SKIPPED | 1 |
| `4c1f25d8` | 2026-11-06 | Kay | chore(standup): daily standup 2026-11-06 | 3 |
| `95ae2d03` | 2026-11-05 | Kay | chore(qa-auto): enemy-attack cases trial - 1 inbox note 2026-11-05 | 2 |
| `3e50d206` | 2026-11-05 | Kay | test(qa): QA-ENT-08 enemy attacks the player per role, QA-STATIC-08 enemy attack wiring | 5 |
| `6f5322f6` | 2026-11-05 | Kay | fix(player): body collider no longer on the Player layer (needs owner check) | 1 |
| `dd1ec12d` | 2026-11-05 | Kay | update asset | 298 |
| `06204474` | 2026-11-05 | Kay | add asset | 1262 |
| `fbab752a` | 2026-11-05 | Claude | fix(data): move values from renamed serialized keys into the new fields | 9 |
| `7c80806b` | 2026-11-05 | Kay | chore(qa-auto): weekly QA run 20261105-1417 - 0 inbox notes 2026-11-05 | 2 |
| `d27f8221` | 2026-11-05 | Kay | test(qa): part C and inbox proposals for run 20261105-1417 | 2 |
| `16786a6f` | 2026-11-05 | Kay | test(qa): weekly automation run 20261105-1417 on origin/sprint-21 - FAIL | 11 |

## 2. Systems touched

| System | Files | Examples |
|---|---|---|
| ABL | 2 | `Assets/SO/Skill/Paladin/Ability/Blessed Slash/Effect/Paladin Recovery Reduction Stats Effect.asset`<br>`Assets/SO/Skill/Paladin/Ability/Blessing/Effect/Paladin Buff - Debuff Stat Effect.asset` |
| CMB | 4 | `Assets/SO/Weapons/PaladinWeapon/SnS_Stat.asset`<br>`Assets/SO/Weapons/PaladinWeapon/SnS_State1.asset`<br>`Assets/SO/Weapons/PaladinWeapon/SnS_State2.asset`<br>`Assets/SO/Weapons/PaladinWeapon/SnS_State3.asset` |
| DOCS | 6 | `docs/systems/enemy/CHANGELOG.md`<br>`docs/systems/enemy/README.md`<br>`production/qa/bug-inbox.md`<br>`production/qa/playtests/playtest-2026-11-07.md`<br>... +2 |
| ITEM | 2 | `Assets/SO/Item/Effect/New Stat Modifier Effect Definition - Copy.asset`<br>`Assets/SO/Item/Effect/New Stat Modifier Effect Definition.asset` |
| MAP | 8 | `Assets/SO/Database/Room/Role/RoomModel_Roles.asset`<br>`Assets/SO/Database/Room/Role/RoomModel_T1_Outpost.asset`<br>`Assets/SO/Database/Room/Role/RoomModel_T1_SwarmNest.asset`<br>`Assets/SO/Database/Room/Role/RoomModel_T2_Ambush.asset`<br>... +4 |
| PLY | 1 | `Assets/Prefab/Player/PlayerTest.prefab` |
| QA | 18 | `Assets/QA/Editor/QASceneBuilder.cs`<br>`Assets/QA/Editor/QAStaticChecks.cs`<br>`Assets/QA/Runtime/Suites/QAEnemySuite.cs`<br>`qa-automation/cases/case-catalog.md`<br>... +14 |
| VISUAL | 1550 | `Assets/Animation/Enemy/Role/Assasin/Activate/Assasin_SpinAttack_State1_dir 0.anim`<br>`Assets/Animation/Enemy/Role/Assasin/Activate/Assasin_SpinAttack_State1_dir 1.anim`<br>`Assets/Animation/Enemy/Role/Assasin/Activate/Assasin_SpinAttack_State1_dir 2.anim`<br>`Assets/Animation/Enemy/Role/Assasin/Activate/Assasin_SpinAttack_State1_dir 3.anim`<br>... +1546 |

## 3. Interaction risks (re-checked by the deep run)

| Changed | Why it can break something else | Also checked |
|---|---|---|
| `Assets/SO/Database/Room/Role/RoomModel_Roles.asset`<br>`Assets/SO/Database/Room/Role/RoomModel_T1_Outpost.asset`<br>`Assets/SO/Database/Room/Role/RoomModel_T1_SwarmNest.asset`<br>... +13 | Asset edits: missing script / orphaned key / saved-modifier checks | STATIC |

## 4. New and deleted

**New runtime scripts (0)** - a script not named in `qa-automation/cases/case-catalog.md` is marked *no case yet*:

- none

**New assets (0)**:

- none

**Deleted (0)**:

- none

## 5. Suggested new cases

Filled in by the Friday Claude task (part C). Until then: one case per *no case yet* script above, AUTO when
the result is a number, state or event, MANUAL when it needs eyes (animation, VFX, layout, feel).
