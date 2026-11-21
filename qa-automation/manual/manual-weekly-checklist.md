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
| MAN-ENT-09 | Pooled enemy reset on reuse (`b33dedf9`) | MainGamePlay, New Game | Clear two combat rooms in a row | Enemies of the second room spawn with full HP bars, idle / walk normally (no death pose, no instant death); they wander near where they spawned (BUG-124) | | |
| MAN-ABL-05 | Archer Rain Arrows on its own prefab (`285e6900`) | CombatTest, Archer, full Mana, 3 enemies spread in front | Press 4 toward 4 different directions | Arrows fall in a pattern that matches the cast direction; enemies standing where arrows land lose HP; the effect disappears at the end of the clip and the next cast works | | |
| MAN-ITEM-02 | Item drop look (added 2026-11-20) | MainGamePlay, a combat room | Kill enemies until 3 items drop | Each drop pops in (scale tween), shows its sprite and is readable on every floor tile; no invisible drop (inbox NOTE-20261120-4: item `Test` has no sprite) | | |
| MAN-ITEM-03 | Pickup feel (added 2026-11-20) | An item on the floor | Walk up, press Q once; then press Q again with nothing in reach | The pickup animation faces the item; the item vanishes on the pickup frame, not before; one press takes one item; Q with nothing near does nothing and the player is not locked (AUTO QA-ITEM-03 / 04 / 12 check the numbers) | | |
| MAN-ITEM-04 | Item bonus shown to the player (added 2026-11-20) | Pick up `BaseOrds` | Open the Stats panel (C) | STR +3 and INT +1 show, and the derived stats (HP, damage) move with them; nothing reverts on room change (AUTO QA-ITEM-05 checks the values) | | |
| MAN-VIT-01 | HP bar on a max-HP change (added 2026-11-20) | Take some damage, then pick up a STR / VIT item | Watch the HP bar | The bar re-scales to the new max without jumping to full and without a wrong number (AUTO QA-VIT-10 checks the clamp) | | |
| MAN-UI-06 | Game Over per class (added 2026-11-20) | Each class in turn | Let enemies kill the player | Death animation plays to the end, then the Game Over screen. Expected to FAIL on every class until BUG-122 / NOTE-20261120-3 are fixed — record what you see | | |

## Part 2 — New this week

Filled in by the Friday QA task from `weekly-change-impact.md` (section 4) when a new feature needs eyes.
One row per new visual / feel / layout / audio behaviour. Clear the table at the start of each week after
moving any row that should stay into Part 1.

Week of 2026-11-17 (run `20261117-1620`, `sprint-23` `077ebd31`). Last week's rows were cleared: MAN-ENT-09 (BUG-124) and MAN-ABL-05 (BUG-109 hand check) moved to Part 1; MAN-CMB-06 is covered by AUTO QA-CMB-01 (PASS, BUG-125 fixed), MAN-CMB-07 by AUTO QA-CMB-04.Mage (KNOWN BUG-127), MAN-ENT-08 by QA-ENT-08, MAN-STAT-01 dropped.
Run `20261119-1616` (`sprint-23` `c613e641`) added MAN-ENT-11..13 and MAN-MAP-06..07; the 2026-11-17 rows are from the same sprint week and stay.

| ID | Feature (commit) | Preconditions | Steps | Expected | Actual | Pass/Fail |
|---|---|---|---|---|---|---|
| MAN-PLY-06 | Dash (`91c42d79`, `PlayerDashState`) | CombatTest, each class in turn | Aim the mouse in 8 directions, press Space each time; also Space while walking | Dash animation plays and faces the dash; the player travels toward the mouse, does not pass through walls, returns to Idle / walk cleanly. Only Paladin has dash clips — Archer / Mage: note what happens (AUTO QA-PLY-06 records whether they get stuck) | | |
| MAN-PLY-07 | Dash then hit (`PlayerBasicState` checks `IsDash` before `IsDeath`) | CombatTest, Paladin, an enemy, low HP (take hits until about 10 % HP) | Dash into the enemy and take the killing hit during or right after the dash | Player still dies: death animation plays and Game Over follows (or BUG-122 behaviour, no Game Over); the player is never left alive at 0 HP | | |
| MAN-MAP-05 | Bonfire room (`BonfireController`, `Bonfire.prefab`, `NormalRoom_20`, `Tile_Spawn_Bonfire`) | MainGamePlay, New Game, reach a room with a bonfire | Walk up to the bonfire; press the key the prompt shows; walk away | A prompt "Press <key> to open stats" appears near / on the HUD when close and hides when you leave; the key opens the Stats panel. Inbox NOTE-20261115-6 predicts the prompt never shows (trigger callbacks not wired) — record what you see | | |
| MAN-UI-05 | HUD interact prompt (`InteractiveNotifi`) | CombatTest and MainGamePlay | Stand next to a class monument, then next to a bonfire | Prompt text and key label readable at 1920×1080 and 1280×720, not overlapping the hotbar or bars; disappears when out of reach | | |
| MAN-ITEM-01 | Item stat buffs stack (`25d869fb`, `cfb6a230`) | MainGamePlay, kill enemies until two stat items drop (BaseOrds / Test) | Pick up both with Q, open C | Both bonuses show in the Stats panel and add up; casting Paladin Blessing (3) and letting it expire leaves the item bonuses in place (AUTO QA-ENT-16 checks the numbers on an enemy) | | |
| MAN-ENT-10 | Enemies spawn walking (`3b02df7d`: spawn state is Move, not Idle) | MainGamePlay, enter a combat room | Watch enemies for the first 3 s | Enemies start wandering at once; no pop, slide or wrong facing on the first frames; owner to confirm this is the intended spawn behaviour | | |
| MAN-ENT-11 | Hit from out of view: turn, look, investigate (`c613e641`, `EntityInvestigateState`) | CombatTest, Paladin (or Archer for range), an enemy walking away from you | Hit it from behind once (non-lethal); then repeat with a wall between you after the hit; then hit from far behind with the Archer | After the hit clip the enemy turns its sprite AND view toward you: sees you → chases; wall in the way → stands and looks around for its full idle time; out of view → walks toward where the hit came from with the Move animation, chases if it spots you, else stops and idles. No snap, jitter or walking into walls (AUTO QA-ENT-11 checks the state only) | | |
| MAN-ENT-12 | Enemy paths on tile centres, no corner cutting (`0e0dc874`) | MainGamePlay, a combat room with pillars or diagonal walls (e.g. FourPillars, DiagonalWalls) | Let an enemy chase you around a pillar and along a wall | The enemy walks through the middle of floor tiles, never drawn over a wall or pillar corner, never stuck next to a wall while you stand beside it (AUTO QA-PATH-04/05/06 check the grid) | | |
| MAN-ENT-13 | Death clips play once (`d840f0d0`, 88 death `.anim` files) | MainGamePlay, a combat room with several enemy roles | Kill one enemy of each role from different directions | Death animation plays once to the last frame and the body is removed; no loop, no restart, no pop back to idle | | |
| MAN-MAP-06 | Start / boss room by type and required rooms (`e2b7efee`, `a92a0c7b`, `3015e9cf`) | MainGamePlay, New Game, 3 runs | Look at the first room; walk the map to the boss room; note the room types on the way | First room has the Champion monuments and the Bonfire; the boss room spawns the boss and its doors open after it dies; every run contains a Shop and at least one Treasure / Rest / Puzzle / Secret room (AUTO QA-MAP-07 checks the library data). Also note any room whose doors never open (inbox NOTE-20261119-1 shape) | | |
| MAN-MAP-07 | LevelEditorTool room CRUD (`0b959b97`, `d840f0d0`) | Editor, `Assets/Scenes/SetLevel.unity` | Inspector of LevelEditorTool: load an existing room, change one tile, save as a new room, load it back, delete it | Each button does what it says; the room list updates; the room JSON appears / disappears under `Assets/Data/Json/Room/`; no Console error; revert every file afterwards with `git checkout` / `git clean` | | |
## Open design questions (decide the expected result of some rows)

1. An unpicked player cannot attack or cast — intended or a bug?
2. Mage has no default weapon — does a Mage have a basic attack, and what is it?
3. Abilities with an empty Effects list (Archer 1–3, all four Mage) — SKIP or FAIL?
4. Avatar of Light at HP ≤ its HP cost — refuse the cast, or allow it?
5. Should `EventManager.Emit` isolate listeners, so one listener that throws does not stop the others (AUTO QA-EVT-05)?
6. A room whose spawn set came back empty (0 enemies): should its door open (AUTO QA-MAP-10)?
7. Do item stat bonuses last the whole run, including death and New Game, or reset? (No AUTO case until decided.)
