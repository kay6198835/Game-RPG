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
| `Assets/QA/Scenes/Test/CombatTest.unity` | Open, press Play. Walk to a monument, press G, Confirm (or F1/F2/F3 to switch class) | Player, combat, abilities, enemy |
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
| MAN-MAP-05 | Bonfire room (`BonfireController`, `Bonfire.prefab`, `NormalRoom_20`, `Tile_Spawn_Bonfire`) (moved from Part 2, 2026-11-27) | MainGamePlay, New Game, reach a room with a bonfire | Walk up to the bonfire; press the key the prompt shows; walk away | A prompt "Press <key> to open stats" appears near / on the HUD when close and hides when you leave; the key opens the Stats panel. Inbox NOTE-20261115-6 predicts the prompt never shows (trigger callbacks not wired) — record what you see | | |
| MAN-ENT-11 | Hit from out of view: turn, look, investigate (`c613e641`, `EntityInvestigateState`) (moved from Part 2, 2026-11-27) | CombatTest, Paladin (or Archer for range), an enemy walking away from you | Hit it from behind once (non-lethal); then repeat with a wall between you after the hit; then hit from far behind with the Archer | After the hit clip the enemy turns its sprite AND view toward you: sees you → chases; wall in the way → stands and looks around for its full idle time; out of view → walks toward where the hit came from with the Move animation, chases if it spots you, else stops and idles. No snap, jitter or walking into walls (AUTO QA-ENT-11 checks the state only) | | |
| MAN-PLY-08 | Dash invincibility (`be973605` `InvicisibleHurtbox`, `9e5921c0` Paladin dash state, `67c188b3`) (moved from Part 2, 2026-11-27) | CombatTest, Paladin, an enemy attacking you (after MAN-CMB-09 passes) | Dash through the enemy while its attack lands; then stand still and take a hit | No HP lost while dashing; the next hit after the dash lowers HP (hurtbox back on); dash plays and returns to Idle / walk (AUTO QA-PLY-06 is KNOWN BUG-130, QA-VIT-16 checks the toggle) | | |
| MAN-CMB-10 | Knockback direction (`19540dff`) (moved from Part 2, 2026-11-27) | CombatTest, Paladin, an enemy | Hit the enemy from its left, then from its right | The enemy is pushed away from you, a short readable step. BUG-133 predicts it moves toward you — record the direction | | |

## Part 2 — New this week

Filled in by the Friday QA task from `weekly-change-impact.md` (section 4) when a new feature needs eyes.
One row per new visual / feel / layout / audio behaviour. Clear the table at the start of each week after
moving any row that should stay into Part 1.

Week of 2026-11-24 (run `20261127-0141`, `sprint-24` `075711d9`). Last week's rows were cleared: MAN-MAP-05 (bonfire prompt), MAN-ENT-11 (investigate after a hit), MAN-PLY-08 (dash invincibility, BUG-130 open) and MAN-CMB-10 (knockback, BUG-133 open) moved to Part 1. Dropped: MAN-PLY-06 (covered by Part 1 MAN-PLY-05 and AUTO QA-PLY-06, KNOWN BUG-130), MAN-PLY-07, MAN-UI-05, MAN-ITEM-01 (AUTO QA-ENT-16 / QA-ITEM-09 PASS), MAN-ENT-10 (owner intent, NOTE-20261117-4), MAN-ENT-12 (AUTO QA-PATH-04..06), MAN-ENT-13, MAN-MAP-06 (AUTO QA-MAP-07), MAN-MAP-07, MAN-CMB-09 (hits land again: QA-ENT-08 x10 and QA-VIT-06 PASS), MAN-PLY-09 (BUG-129 fixed), MAN-ANI-01.

| ID | Feature (commit) | Preconditions | Steps | Expected | Actual | Pass/Fail |
|---|---|---|---|---|---|---|
| MAN-ABL-06 | Ability hits through `DamageEffect` (`b001d78e`, `85899a52` Delivery / Payload split) | CombatTest, Paladin, full Mana, an enemy 2-3 tiles ahead | Cast 1 (Blessed Slash) and 2 (Consecrate) at the enemy, 3 times each | Each hit lowers the enemy HP bar, the enemy shows its hit reaction (flash / take-damage clip / short knockback) and dies after enough hits; no hit lands on the caster (AUTO QA-ENT-17 checks the number on an enemy, QA-ABL-01 on a probe) | | |
| MAN-ABL-07 | Spawn effect clips play once and despawn (`6dd66791`, `fa206f82`: Exit transition removed on 4 effect controllers) | CombatTest, Paladin then Archer, full Mana | Cast Blessed Slash, Consecrate and Rain Arrows twice each, a few seconds apart | Each VFX plays once to its end and disappears; it never loops, never freezes on screen, never pops back to frame 1; the second cast plays the same as the first | | |
| MAN-ABL-08 | Consecrate cast clip (`43bb14be`: player clips play once with a padded end, Activate -> Cast exit time 1) | CombatTest, Paladin, full Mana | Press 2 in 4 different directions | The cast animation faces the mouse, plays once, holds briefly on the last pose and returns to Idle / walk without a snap or a second play | | |
| MAN-ABL-09 | Rain Arrows summon (`e52431d6`: summon spawns once, RainArrows clips hold the last frame; `a9398fc5`) | CombatTest, Archer, full Mana, 2 enemies spread in front | Press 4 at the enemies, then press 4 again elsewhere after the cooldown | One rain per cast, at the mouse; arrows hold their landed frame until the effect ends, then vanish; enemies under the arrows lose HP; the second cast is not doubled | | |
| MAN-ABL-10 | Self-apply buffs and status effects (`df9e98fe` add status effect, `85899a52` `SelfApplyEffect`, `83800891` A1 sign from impactType) | CombatTest, Paladin, full Mana; Stats panel from the Bonfire (or C) | Take some damage, then press 3 (Blessing) and 4 (Avatar of Light); open the Stats panel during and after the buff | The buff VFX shows on the player; the Stats panel shows the raised stats during the buff and the old values after it ends; a recovery heals (HP bar up), never lowers HP (inbox NOTE-20261123-1: assets without `impactType` may load as the wrong kind) | | |
| MAN-ABL-11 | Trap object (`f2a580b2` add trap object, `Trap.prefab`, `Trap.cs`) | Editor: drop `Assets/Prefab/Interactable Object/Trap.prefab` into CombatTest near an enemy path, Play | Lure an enemy across the trap; walk over it yourself | The trap fires on the enemy (visible effect, enemy HP drops) and does nothing to the player. Inbox NOTE-20261126-1 predicts it never fires (layer mask compared with a layer index) — record what you see; revert the scene afterwards | | |
| MAN-PLY-10 | Dash smoke and dash on every class (`1e5afb3c` add effect dash, `DashSmoke.prefab`; `b0cfe539` dash state in Paladin, Mage, Archer controllers) | CombatTest, each class in turn | Press Space in 4 directions while walking | A smoke puff appears at the start of each dash, faces / trails the dash and disappears; Archer and Mage now play a dash clip too. AUTO QA-PLY-06 is KNOWN BUG-130 (dash moves 0) — record the distance you see | | |
| MAN-MAP-08 | Rooms load in a Player build (`7a41d7c4` room JSON from TextAsset, `070bc0a5` build blockers removed) | `File > Build And Run` (Windows) | New Game, walk through 3 rooms | The build starts without errors, every room loads its tiles, doors and enemies like in the Editor | | |

## Open design questions (decide the expected result of some rows)

1. An unpicked player cannot attack or cast — intended or a bug?
2. Mage has no default weapon — does a Mage have a basic attack, and what is it?
3. Abilities with an empty Effects list (Archer 1–3, all four Mage) — SKIP or FAIL?
4. Avatar of Light at HP ≤ its HP cost — refuse the cast, or allow it?
5. Should `EventManager.Emit` isolate listeners, so one listener that throws does not stop the others (AUTO QA-EVT-05)?
6. A room whose spawn set came back empty (0 enemies): should its door open (AUTO QA-MAP-10)?
7. Do item stat bonuses last the whole run, including death and New Game, or reset? (No AUTO case until decided.)
