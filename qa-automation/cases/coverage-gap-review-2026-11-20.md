# QA coverage gap review — 2026-11-20

Scope: every system in `case-catalog.md`, checked against the code on `sprint-23` (`4a2237e`) and the latest
run `20261117-1705` (178 cases: 151 PASS, 4 FAIL, 9 KNOWN, 13 SKIP, 1 OBSERVED). Evidence tier: STATIC. **None
of the code added here has been compiled or run.** No Unity Editor was available, so the first run is also the
first compile check.

## 1. What was added

| System | New AUTO cases | Where |
|---|---|---|
| ITEM | QA-ITEM-05…12: exact effect of every droppable item, coin item, HP / Mana recovery item, two pickups stacking, item buff vs timed buff on the player, damaged item data, pooled drop reuse, two items in reach with real Q input | `Assets/QA/Runtime/Suites/QAEnemySuite.Items.cs` |
| VIT | QA-VIT-09…14: recovery event, max-HP lowered / raised, Mana 0 is not death, unseeded vital (Coin), boundary damage (HP − 0.5), take-damage state round trip | `QAVitalsSuite.cs` |
| EVT | QA-EVT-04 (listener unregisters itself during Emit), QA-EVT-05 (throwing listener, OBSERVED) | `QASystemsSuite.cs` |
| MAP | QA-MAP-08 (`ON_ROOM_CLEARED` before `ON_OPEN_DOOR`), QA-MAP-09 (deaths after clear), QA-MAP-10 (0-enemy room, OBSERVED); QA-MAP-07 was taken by the room-library case of 2026-11-19 | `QASystemsSuite.cs` |
| STATIC | QA-STATIC-09 items, 10 drop tables, 11 class data, 12 bound abilities, 13 class death clips | `Assets/QA/Editor/QAStaticChecks.cs` |
| UNIT | `StatPointSessionTests` (10), `AbilityDefinitionTests` (8), `ItemEffectDataTests` (13) | `Assets/Tests/EditMode/` |
| MANUAL | MAN-ITEM-02…04, MAN-VIT-01, MAN-UI-06; design questions 5–7 | `manual/manual-weekly-checklist.md` |

### Expected results on the first run (static prediction)

| Case | Expected | Why |
|---|---|---|
| QA-STATIC-09, `ItemEffectDataTests.CommittedDropTables_…` | FAIL | item `Test` has no name and no sprite (NOTE-20261120-4) |
| QA-STATIC-12 | FAIL | 7 placeholder abilities bound to Archer / Mage (NOTE-20261120-2; open design question 3) |
| QA-STATIC-13 | FAIL on Archer and Mage (Paladin passes since `49c58e35`) | NOTE-20261120-3 |
| QA-ITEM-10 | FAIL on the empty slot | NOTE-20261120-5 |
| QA-ITEM-12 | may FAIL | NOTE-20261120-6 (stale collider buffer) |
| QA-EVT-05, QA-MAP-10 | OBSERVED | design open (checklist questions 5, 6) |
| everything else new | PASS | |

## 2. Gaps found but not automated yet

Each item needs a scene, an owner decision, or a feature that does not exist yet. Listed so the next QA update
can pick them up.

| System | Missing case | Blocked by |
|---|---|---|
| PLY | Interact (G) with nothing in reach; interact while attacking | small `QAPlayerFlowSuite` addition, not done this round |
| PLY | Movement speed = `MoveSpeed` stat; movement frozen during attack / ability / pickup | needs a free floor in `QA_PlayerFlow` |
| PLY | Dash cooldown, dash during attack / cast, dash into a wall | dash soft lock first (NOTE-20261117-3) |
| CMB | Crit: rate follows `CritChance`, crit adds `CritDamage` | needs a seeded crit roll in the case |
| CMB | Melee hits every target in range once (multi-target), never the same target twice per swing | needs a second `QAProbeTarget` |
| CMB | Attack input buffered during a stage starts the next combo stage; combo resets after a pause | timing case, P2 |
| ABL | Hold abilities: hold time, release, `MaxHoldTime` (BUG-083) | no live Hold ability with `MaxHoldTime` > 0 |
| ABL | Cooldown keeps running in states other than Basic / SkillWeapon (BUG-069 residual) | case design |
| ABL | HP cost equal to current HP kills the caster (NOTE-20261117-1 behaviour change) | owner decision (checklist question 4) |
| VIT | `ON_PLAYER_DEATH` → Game Over → Respawn reload; `ReloadComponent` after death | death re-entry loop (NOTE-20261120-1) |
| ENT | Enemy knockback direction on hit; enemy separation (no stacking); enemy stuck on walls | needs a walled arena |
| ENT | Enemy abilities (`EntityAbilityHolder`) | owner: no enemy abilities this phase |
| STAT | Negative max-stat modifier is a debuff, not a buff (NOTE-20261117-1, `StatModifier.Value` abs) | add once the sign rule is decided; a unit test would FAIL today |
| STAT | `PercentAdd` / `PercentMult` item modifiers | no committed item uses them |
| ITEM | Item bonuses across death and New Game | design question 7 |
| ITEM | Coin shown on the HUD; spending coins (shop) | no coin UI, `ShopController` empty |
| MAP | Door walk-through loads the next room at the matching door; minimap follows | needs `LoadRandomMap` in a QA scene |
| MAP | Start / Boss room open conditions | BUG-096 |
| UI | HP / Mana bars follow `CurrentStatsChanged`; hotbar cooldown fill; stats panel Accept / Revert through the UI | GameplayUI is loaded in `QA_PlayerFlow`, cases not written |
| UI | Pause, Settings, Save / Continue, Loading flow | menu scenes not in any QA group |
| POOL | Releasing an object twice; releasing an object the pool never spawned | owner decision on the expected behaviour |

## 3. Notes for triage

Five inbox notes were written for the defects this review found: NOTE-20261120-2…6 in
`production/qa/bug-inbox.md`. No bug ID was allocated (bug-inbox rule). No known-issue mapping was added.
