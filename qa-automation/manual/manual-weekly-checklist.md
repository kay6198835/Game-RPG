# Manual weekly checklist

Cases a script cannot judge: how it looks, how it feels, how it sounds. Fill in **Actual** and **Pass/Fail**
(PASS / FAIL / BLOCKED / N/A). A FAIL is not a bug ID: add a `NOTE-YYYYMMDD-N` row to
`production/qa/bug-inbox.md` (Source `playtest`, Evidence tier `RUNTIME`) — see `.claude/rules/bug-inbox.md`.

The numbers behind these cases are already checked by the Friday automated run. Read
`qa-automation/reports/<latest>/report.md` first: a case whose AUTO twin FAILs is BLOCKED, not FAIL.

| Field | Value |
|---|---|
| Week / sprint | |
| Build (commit hash, `git rev-parse --short HEAD`) | |
| Automated report read (run id) | |
| Tester | |
| Date | |

## How to get into each scene

| Scene | How | Use for |
|---|---|---|
| `Assets/Scenes/Test/CombatTest.unity` | Open, press Play. Walk to a monument, press G, Confirm (or F1/F2/F3 to switch class) | Player, combat, abilities, enemy |
| `Assets/Scenes/Main/MainGamePlay.unity` | Open, press Play. New Game | Menus, loading, full dungeon |
| QA scenes (`Assets/QA/Scenes/QA_*.unity`) | Built by `Tools > QA > Automation`; they drive the player themselves | Do not play by hand |

## Part 1 — Regular (every week)

| ID | Area | Preconditions | Steps | Expected | Actual | Pass/Fail |
|---|---|---|---|---|---|---|
| MAN-PLY-01 | Movement | CombatTest, no class picked | Walk in all 8 directions (WASD combinations) | Idle / walk animation matches the direction; no sliding, no flicker, no snap | | |
| MAN-PLY-02 | Class pick | Stand at a monument | Press G | Confirm panel: class name in the title, message in the body, both button labels on the buttons (BUG-102) | | |
| MAN-PLY-03 | Class pick | Confirm panel open | Click **Cancel** | Panel closes; class not applied; door stays closed; game unpaused | | |
| MAN-PLY-04 | Class pick | Class just confirmed | Look at the character | Sprite and animations switch to the class look immediately, in every direction | | |
| MAN-PLY-05 | Dash | Any class | Space while moving in 4 directions | Dash covers distance in the move direction, animation plays, returns to idle cleanly | | |
| MAN-CMB-01 | Paladin melee | Paladin, enemy in CombatTest | LMB toward each of the 8 directions | Swing faces the mouse; hit flash / reaction on the enemy | | |
| MAN-CMB-02 | Paladin combo | Paladin | Click repeatedly | Stages flow into each other; no visible pause or snap to idle between stages | | |
| MAN-CMB-03 | Archer shot | Archer | LMB toward each of the 8 directions | Draw / shoot faces the mouse; the arrow leaves the bow and points along its flight | | |
| MAN-CMB-04 | Hit feel | Any class, enemy | Hit the enemy 5 times | Enemy health bar drops each hit; knockback reads clearly; enemy death animation plays fully | | |
| MAN-ABL-01 | Paladin 1 Blessed Slash | Full Mana | Hold 1, release toward the mouse | Slash VFX travels toward the mouse; cast animation is readable | | |
| MAN-ABL-02 | Paladin 2 Consecrate | Full Mana | Press 2 with the mouse on the enemy | Rune circle at the mouse first, then lightning strikes it (telegraph then hit) | | |
| MAN-ABL-03 | Paladin 3 / 4 | Full Mana | Press 3, then 4 | Buff / heal VFX on the player; Stats panel (C) shows the change and it reverts when the buff ends | | |
| MAN-ABL-04 | Archer 4 Rain Arrows | Full Mana | Cast twice at two different places | Arrows fall over the area at the mouse each time, not at the previous cast's position (BUG-109) | | |
| MAN-ENT-01 | Enemy chase | MainGamePlay, a combat room | Walk away from an enemy | Enemy follows around walls, does not walk through walls or get stuck on corners | | |
| MAN-ENT-02 | Enemy attack (look and timing; "the player loses HP" is AUTO in QA-ENT-08 / QA-STATIC-08) | Combat room | Stand next to an enemy for 5 s | Attack animation faces the player; the HP bar drops on the visible impact frame, not before or after; hit feedback (flash / knockback) shows | | |
| MAN-ENT-03 | Enemy death | Combat room | Kill an enemy | Death animation, body disappears, nothing left floating; HP bar removed | | |
| MAN-UI-01 | HUD | Any class picked | Look at the HUD | HP / Mana bars full and labelled; 4 hotbar icons in slot order 1–4 | | |
| MAN-UI-02 | Hotbar cooldown | Cast any ability | Watch the hotbar | Cooldown overlay starts at cast and empties in step with the real cooldown | | |
| MAN-UI-03 | Stats panel | Press C | Open, change a point, Accept / Revert, close | Values update; Revert restores; panel closes and the game resumes | | |
| MAN-UI-04 | Menus | MainGamePlay | Splash → menu → New Game → loading → gameplay | Every screen readable at 1920×1080 and 1280×720; no overlapping text; loading bar moves | | |
| MAN-MAP-01 | Doors | Combat room | Kill every enemy | Doors open visibly; walking through loads the next room and places the player at the matching door | | |
| MAN-MAP-02 | Minimap | Move through 3 rooms | Watch the minimap | Avatar moves to the current room; visited rooms revealed | | |
| MAN-MAP-03 | Non-combat rooms | Enter a Start / Rest / Shop / Buff room | Look at the doors | Doors open without enemies (BUG-096 — expected to FAIL until fixed) | | |
| MAN-AUD-01 | Audio | Any | Attack, cast, take a hit, open a menu | A sound for each, no clipping, volume consistent | | |
| MAN-HYG-01 | Asset hygiene | After the session | `git status` | No `Assets/SO/**/*.asset` modified by playing (BUG-115) | | |

## Part 2 — New this week

Filled in by the Friday QA task from `weekly-change-impact.md` (section 4) when a new feature needs eyes.
One row per new visual / feel / layout / audio behaviour. Clear the table at the start of each week after
moving any row that should stay into Part 1.

Week of 2026-11-14 (run `20261114-2329`, `sprint-22` `a8725a77`). Last week's rows (MAN-ENT-04…07, MAN-CMB-05, MAN-MAP-04) were cleared; the role look / speed checks are covered by AUTO QA-STATIC-07 / QA-ENT-07 / QA-ENT-08.

| ID | Feature (commit) | Preconditions | Steps | Expected | Actual | Pass/Fail |
|---|---|---|---|---|---|---|
| MAN-CMB-06 | Paladin 3-stage combo animations (`38d40925`) | CombatTest, Paladin, a dummy | Click repeatedly toward each of the 8 directions | All 3 stage animations play in every direction, each faces the mouse; the dummy's HP drops once per stage, by a bigger amount on stage 3 (AUTO QA-CMB-01 found the stage damage added twice, see the inbox) | | |
| MAN-CMB-07 | Mage basic attack, MagicBolt (`0f3644bd`) | CombatTest, Mage, a dummy 3 units and one 6 units away | LMB toward each dummy and toward 4 other directions | Cast animation faces the mouse; the bolt leaves the hand, points along its flight, reaches both dummies and disappears on hit (bolt speed is 1 with lifetime 4 — expected to FAIL on the 6-unit dummy, see the inbox) | | |
| MAN-ABL-05 | Archer Rain Arrows on its own prefab (`285e6900`) | CombatTest, Archer, full Mana, 3 enemies spread in front | Press 4 toward 4 different directions | Arrows fall in a pattern that matches the cast direction; enemies standing where arrows land lose HP; the effect disappears at the end of the clip and the next cast works | | |
| MAN-ENT-08 | Enemy attack clip event timing (`38d40925`, many `*_Attack_dir N.anim`) | MainGamePlay, rooms with Assasin, Boss (if reachable), Rat, Golem | Stand next to each enemy for 5 s | HP drops on the visible impact frame of each attack, not before or after | | |
| MAN-ENT-09 | Pooled enemy reset on reuse (`b33dedf9`) | MainGamePlay, New Game | Clear two combat rooms in a row | Enemies of the second room spawn with full HP bars, idle / walk normally (no death pose, no instant death); they wander near where they spawned (BUG-124) | | |
| MAN-STAT-01 | Stat split, data-only stat assets (`a0e250fb`) | CombatTest, each class | Pick the class, press C | Stats panel lists the same derived values as the class's stat asset Inspector preview; Paladin INT shows 10; spending a point and pressing Revert restores every value | | |
## Open design questions (decide the expected result of some rows)

1. An unpicked player cannot attack or cast — intended or a bug?
2. Mage has no default weapon — does a Mage have a basic attack, and what is it?
3. Abilities with an empty Effects list (Archer 1–3, all four Mage) — SKIP or FAIL?
4. Avatar of Light at HP ≤ its HP cost — refuse the cast, or allow it?
