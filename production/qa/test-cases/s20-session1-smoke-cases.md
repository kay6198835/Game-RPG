# Sprint 20 — Session 1: Smoke — Manual Test Cases

**Stories**: S20-01 (weekly playtest / Play Mode smoke), S20-F1 (land `feature/add-effect-component`)
**Story type**: Integration | **Evidence**: documented playtest (this sheet + `production/qa/playtests/playtest-2026-10-24.md`) | **Gate**: BLOCKING
**Source**: `tests/smoke/critical-paths.md`, `production/sprints/sprint-20.md`, `production/qa/qa-plan-sprint-20-2026-10-31.md`

## Common preconditions (all cases)

- Unity 2022.3.62f3 LTS; branch `sprint-20` with `feature/add-effect-component` merged. Record the commit hash: `________`
- Working tree clean before the session (`git status` shows no modified `.cs` / `.asset`), so results can be attributed to a commit.
- Console: Clear on Play ON, Collapse OFF, Error Pause OFF.
- Start every Play Mode run from `Assets/Scenes/Main/MainGamePlay.unity` (build index 0), not from `LoadRandomMap`.

Note: an accepted-risk note signed in `sprint-20.md` instead of running this sheet is recorded by QA as "no manual evidence", not as PASS.

---

### S1-01 — Merged branch compiles with 0 errors
- **Story**: S20-F1
- **Preconditions**: `feature/add-effect-component` (`786c031b`) merged into `sprint-20`; Editor closed.
- **Steps**:
  1. Open the project in the Editor and wait for the import / recompile to finish.
  2. Open the Console, filter to Errors.
  3. Search `Editor.log` (`%LOCALAPPDATA%\Unity\Editor\Editor.log`) for `error CS`.
- **Expected Result**: 0 `error CS` lines in Console and in `Editor.log`; Play button is enabled.
- **Actual Result**:
- **Pass/Fail**:

### S1-02 — Menu loads (critical path row 1)
- **Story**: S20-01
- **Preconditions**: S1-01 PASS.
- **Steps**:
  1. Enter Play Mode on `MainGamePlay`.
  2. Wait for splash to finish.
- **Expected Result**: Main menu shows; 0 exceptions in Console.
- **Actual Result**:
- **Pass/Fail**:

### S1-03 — New Game reaches dungeon + HUD (row 2)
- **Story**: S20-01
- **Preconditions**: S1-02 PASS, on main menu.
- **Steps**:
  1. Click New Game (select/create a save slot if asked).
  2. Wait for the Loading screen to finish.
- **Expected Result**: `LoadRandomMap` and `GameplayUI` are both loaded (Hierarchy); HUD shows HP bar, Mana bar and a 4-slot hotbar; 0 exceptions.
- **Actual Result**:
- **Pass/Fail**:

### S1-04 — Champion confirm opens first room (row 3)
- **Story**: S20-01
- **Preconditions**: S1-03 PASS, player in the first room with champion monuments.
- **Steps**:
  1. Walk to a champion monument; press G.
  2. On the confirm panel press Confirm.
- **Expected Result**: Confirm panel opens; player sprite / animator changes to the chosen champion; doors of the first room open; 0 exceptions.
- **Actual Result**:
- **Pass/Fail**:

### S1-05 — Melee kill (row 4)
- **Story**: S20-01
- **Preconditions**: Champion picked (S1-04); enemy present in a combat room.
- **Steps**:
  1. Attack an enemy with LMB until it dies.
- **Expected Result**: Hit reaction on each hit, enemy HP bar drops, death animation plays, enemy disappears; 0 exceptions.
- **Actual Result**:
- **Pass/Fail**:

### S1-06 — Abilities 1-4 cast (row 5)
- **Story**: S20-01
- **Preconditions**: Champion picked; Mana full.
- **Steps**:
  1. Press `1`, wait for the cast to end; repeat for `2`, `3`, `4`.
  2. Watch the hotbar and the HP/Mana bars after each.
- **Expected Result**: Each bound key casts (visible effect); hotbar slot shows a cooldown; Mana (or HP, for HP-cost abilities) bar drops by the cost; 0 exceptions.
- **Actual Result**:
- **Pass/Fail**:

### S1-07 — Ranged weapon hit (row 6)
- **Story**: S20-01
- **Preconditions**: A ranged weapon is equipped (Archer default or floor pickup). If none is reachable, mark SKIP with reason.
- **Steps**:
  1. Aim at an enemy and attack with LMB.
- **Expected Result**: Projectile travels, hits the enemy, enemy HP bar drops.
- **Actual Result**:
- **Pass/Fail**:

### S1-08 — Combat room clear opens doors (row 7)
- **Story**: S20-01
- **Preconditions**: Player in an uncleared combat room.
- **Steps**:
  1. Kill every enemy in the room.
- **Expected Result**: Doors open within 1 s of the last death.
- **Actual Result**:
- **Pass/Fail**:

### S1-09 — Door transition (row 8)
- **Story**: S20-01
- **Preconditions**: S1-08 PASS (an open door).
- **Steps**:
  1. Walk through the open door.
- **Expected Result**: Next room loads; player is placed at the entry door of the new room; minimap avatar moves to the new cell.
- **Actual Result**:
- **Pass/Fail**:

### S1-10 — Player death (row 9)
- **Story**: S20-01
- **Preconditions**: Player in a room with enemies.
- **Steps**:
  1. Stop attacking and let enemies reduce HP to 0.
- **Expected Result**: Death animation plays; game-over panel appears; Console does not repeat the same line every frame.
- **Actual Result**:
- **Pass/Fail**:

### S1-11 — Respawn (row 10)
- **Story**: S20-01
- **Preconditions**: S1-10 reached the game-over panel.
- **Steps**:
  1. Press Respawn (or return to menu → New Game).
- **Expected Result**: Scene reloads; player can move, attack and cast; HP/Mana bars full; 0 exceptions.
- **Actual Result**:
- **Pass/Fail**:

### S1-12 — Archer abilities cast after merge
- **Story**: S20-F1
- **Preconditions**: New run; pick the Archer champion.
- **Steps**:
  1. Press `1`, `2`, `3`, `4` in turn near an enemy, waiting for each cast to finish.
  2. Note which ability is on each slot (Hawk Eye / Kick-back Trap / Piercing Shot / Rain Arrow / Volley).
- **Expected Result**: Each bound slot casts its ability with a visible effect and starts a cooldown; 0 exceptions. (Rain Arrow NRE / pooled `Destroy` is known BUG-109, deferred — record but do not fail S20-F1 on it alone.)
- **Actual Result**:
- **Pass/Fail**:

### S1-13 — Mage abilities cast after merge
- **Story**: S20-F1
- **Preconditions**: New run; pick the Mage champion.
- **Steps**:
  1. Confirm the Mage pick; note any exception.
  2. Press `1`, `2`, `3`, `4` in turn near an enemy (Chain Lightning / Fireball / Frost Nova / Meteor).
- **Expected Result**: Mage confirm raises no exception; each bound slot casts with a visible effect and starts a cooldown; 0 exceptions.
- **Actual Result**:
- **Pass/Fail**:

### S1-14 — Paladin abilities regression after merge
- **Story**: S20-F1
- **Preconditions**: New run; pick the Paladin champion.
- **Steps**:
  1. Press `1`, `2`, `3`, `4` in turn (Consecrate / Blessed Slash / Blessing / Avatar of Light).
  2. For Blessing, watch the stats panel (C) before, during and after the buff.
- **Expected Result**: Each casts; costs applied; Blessing raises the buffed stat and returns it to the pre-cast value on expiry; 0 exceptions.
- **Actual Result**:
- **Pass/Fail**:
