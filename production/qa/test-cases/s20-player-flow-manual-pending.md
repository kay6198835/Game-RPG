# Player Flow — Manual Cases (PENDING owner)

**Scope**: spawn → pick class → basic attack → abilities, for Paladin / Archer / Mage.
**Automated part**: `Assets/Scenes/Test/QAPlayerFlow.unity` + `QAPlayerFlowProbe` (Tools > QA > Player Flow >
Build and Run). Reports land in `production/qa/evidence/auto/player-flow-*.md`. The cases below cannot
be judged by numbers and are left for a human pass. **Status: PENDING** for every row.

**How to run**: open `QAPlayerFlow.unity` (or `CombatTest.unity`), press Play. Disable the
`QAPlayerFlowProbe` object first if you want to play by hand; otherwise it drives the player.
Record the commit hash: `__________`. Tester: `__________`. Date: `__________`.

| ID | Class | Preconditions | Steps | Expected | Actual | Pass/Fail |
|---|---|---|---|---|---|---|
| MQ-01 | all | Play Mode, no class picked | Walk around the spawn point in 8 directions | Idle and walk animations match the movement direction; no sliding, no flicker | | PENDING |
| MQ-02 | all | Stand at a monument | Press G | Confirm panel shows the class name; title, message and both button labels sit in the right fields (BUG-102) | | PENDING |
| MQ-03 | all | Confirm panel open | Click **Cancel** | Panel closes; class not applied; door stays closed | | PENDING |
| MQ-04 | all | Class just confirmed | Look at the character and the HUD | Sprite and animator switch to the class look; hotbar icons are the class's 4 abilities, in slot order 1–4 | | PENDING |
| MQ-05 | Paladin | Paladin picked | LMB toward each of the 8 directions | Attack animation faces the mouse in every direction; hit flash / damage number on the dummy | | PENDING |
| MQ-06 | Paladin | Paladin picked | Click repeatedly | Combo stages flow into each other without a visible pause or snap back to idle | | PENDING |
| MQ-07 | Archer | Archer picked | LMB toward each of the 8 directions | Draw/shoot animation faces the mouse; arrow sprite leaves the bow and points along its flight | | PENDING |
| MQ-08 | Mage | Mage picked | LMB | What a Mage basic attack should look like is undecided (QA question 2) — note what happens | | PENDING |
| MQ-09 | Paladin | Full Mana | 1 Blessed Slash (hold, release) | Slash VFX travels toward the mouse; cast animation reads clearly | | PENDING |
| MQ-10 | Paladin | Full Mana | 2 Consecrate | Rune circle appears at the mouse, then lightning strikes it; timing feels like telegraph then hit | | PENDING |
| MQ-11 | Paladin | Full Mana | 3 Blessing, 4 Avatar of Light | Buff / heal VFX on the player; Stats panel (C) shows the change and it reverts when the buff ends | | PENDING |
| MQ-12 | Archer | Full Mana | 4 Rain Arrows | Arrows fall over the target area at the mouse, not at a previous cast's position (BUG-109) | | PENDING |
| MQ-13 | Archer, Mage | Full Mana | 1–3 (Archer), 1–4 (Mage) | Effects are empty in data — note any animation / VFX that plays and whether it feels like a skill | | PENDING |
| MQ-14 | all | Any ability on cooldown | Watch the hotbar | Cooldown overlay starts at cast and empties in step with the real cooldown | | PENDING |
| MQ-15 | all | After the session | `git status` | No `Assets/SO/Stat/*.asset` modified (BUG-115) | | PENDING |

## Design questions that decide some expected results

1. An unpicked player cannot attack or cast (BUG-106 area) — intended or a bug?
2. Mage has no default weapon — does a Mage have a basic attack, and what is it?
3. Abilities with an empty Effects list (Archer Hawk Eye / Kick-back Trap / Volley, all four Mage) — SKIP or FAIL?
4. Avatar of Light at HP ≤ its HP cost — refuse the cast, or allow it?
5. Rain Arrows uses `Paladin Spawn Consecrat Effect.asset` — wiring mistake or placeholder?
