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
| MAN-ENT-02 | Enemy attack | Combat room | Stand next to an enemy for 5 s | Enemy attack animation plays and the player loses HP in step with the animation | | |
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

Week of 2026-11-05 (run `20261105-1002`, `sprint-21` `04cfb53d`).

| ID | Feature (commit) | Preconditions | Steps | Expected | Actual | Pass/Fail |
|---|---|---|---|---|---|---|
| MAN-ENT-04 | 10 enemy roles, animations (`6094aa10`) | MainGamePlay, enter combat rooms until you have met Assasin, Brute, Elite, FastSwarm, Tank, TrashMelee, RangedCaster, Scatter, Sniper (Boss if reachable) | Watch each role idle, walk, attack and die, from several directions | Each role uses its own sprites; animation faces the move / attack direction in all 8 directions; no T-pose, no frozen frame, no missing clip | | |
| MAN-ENT-05 | Role move speed and attack range from stats (`6094aa10`) | Same rooms | Walk toward and away from each role | FastSwarm / Assasin visibly faster than Tank / Brute; melee roles close in and swing at contact range; ranged roles stop at a distance and back off when you walk into them (MinRangeAttack) | | |
| MAN-ENT-06 | Ranged role projectiles (`6094aa10`) | Rooms with RangedCaster, Scatter, Sniper | Stand in their line of fire, then sidestep | Projectile leaves the enemy toward you, points along its flight, hits and disappears; Scatter fires a spread; a dodged projectile disappears at a wall (Arrow-style no-collider defect = BUG-095) | | |
| MAN-ENT-07 | Enemy death in a real room (`6094aa10`) | Combat room with 2+ enemies | Kill the enemies one by one; kill the last one with a single big hit if possible | Each death animation plays once, the body goes away, one item drop at most per enemy; doors open only after the **last** enemy (QA-ENT-04 saw `ON_ENEMY_DEATH` twice from a looping death clip) | | |
| MAN-CMB-05 | Crit as a percent (`6094aa10`) | Any class, a trash enemy | Take 20 hits from the enemy | No single hit takes most of your HP; crits look like a moderate bump, not a one-shot (was NOTE-20261104-2) | | |
| MAN-MAP-04 | Role room sets (`6094aa10`, `RoomModel_Roles`, `RoomModel_T1…T4`) | MainGamePlay, New Game | Clear 4–5 combat rooms | Enemy mix changes between rooms and gets harder deeper in; no room spawns 0 enemies and then stays locked | | |

## Open design questions (decide the expected result of some rows)

1. An unpicked player cannot attack or cast — intended or a bug?
2. Mage has no default weapon — does a Mage have a basic attack, and what is it?
3. Abilities with an empty Effects list (Archer 1–3, all four Mage) — SKIP or FAIL?
4. Avatar of Light at HP ≤ its HP cost — refuse the cast, or allow it?
