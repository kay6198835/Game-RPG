# Weekly change impact

- Range: `76a60d2fda` (`76a60d2fda`) .. `origin/sprint-23` (`bcedaebb88`)
- 3 commits (+0 merge commits), 3 files changed
- Systems that get a deep (P2) run this week: **PLY**
- The test results for these systems are in `report.md` next to this file (section "Changed this week").

## 1. What landed

| Commit | Date | Author | Subject | Files |
|---|---|---|---|---|
| `bcedaebb` | 2026-11-21 | Kay | chore(qa-auto): weekly QA run 20261121-2029 - 0 inbox notes 2026-11-21 | 1 |
| `67c188b3` | 2026-11-21 | Kay | update | 1 |
| `9e5921c0` | 2026-11-21 | Kay | coding need check | 1 |

## 2. Systems touched

| System | Files | Examples |
|---|---|---|
| DOCS | 1 | `production/session-state/routine-log.md` |
| PLY | 1 | `Assets/Script/Character/Player/States/PlayerDashState.cs` |
| VISUAL | 1 | `Assets/AnimationController/Player/Paladin/Paladin.controller` |

## 3. Interaction risks (re-checked by the deep run)

None matched.

## 4. New and deleted

**New runtime scripts (0)** - a script not named in `qa-automation/cases/case-catalog.md` is marked *no case yet*:

- none

**New assets (0)**:

- none

**Deleted (0)**:

- none

## 5. Suggested new cases

Filled in by the Friday Claude task (part C), 2026-11-21. The range of this run is small (3 commits); the
week's code changes (`19540dff`, `be973605`, `8005d27e`, `9e5921c0`, `67c188b3`) were scanned in run
`20261121-1931`, whose results were never triaged, so part C covers both ranges.

| ID | Kind | Covers | Why |
|---|---|---|---|
| QA-VIT-15 | AUTO (P0, `QAVitalsSuite`) | `NegativeReceiverBase` / `INegativeReceiver` split (`8005d27e`) | Weapons, projectiles and abilities look up `INegativeReceiver`; the receivers now implement only `INegativeReceiver<TCore>`. Guards the damage contract by type, without depending on an enemy attack landing |
| QA-VIT-16 | AUTO (P1, `QAVitalsSuite`) | `InvicisibleHurtbox(bool)` (`be973605`, dash invincibility) | The toggle result is a state (collider enabled) |
| QA-VIT-06 / 13 / 14 | test fix | `QAVitalsSuite` | They threw `NullReferenceException` when the player has no `INegativeReceiver`; now a clear FAIL message |
| MAN-CMB-09 | MANUAL | `8005d27e` | Do hits land at all, player and enemy |
| MAN-PLY-08 | MANUAL | `be973605`, `9e5921c0`, `67c188b3` | Dash invincibility feel |
| MAN-PLY-09 | MANUAL | `19540dff` | Champion confirm applies the class (BUG-129) |
| MAN-CMB-10 | MANUAL | `19540dff` | Knockback direction (BUG-133) |
| MAN-ANI-01 | MANUAL | `be973605`, `8005d27e` | New Archer clips and retimed enemy clips |

Config: `known-issues.json` maps QA-PLY-06.* to BUG-130, QA-ENT-11 to BUG-133, QA-STATIC-09 / QA-UNIT
ItemEffectDataTests (Test.asset) to BUG-132, QA-STATIC-13 Archer / Mage to BUG-122, and widens the BUG-128
pattern to the Erika / Nightshade / Paladin capture scenes. `static-baseline.txt` accepts the QA-STATIC-12
Archer / Mage empty ability slots (owner OQ-10, NOTE-20261120-2 triaged Not a bug) and the info lines of
QA-STATIC-09 / 12 / 13.