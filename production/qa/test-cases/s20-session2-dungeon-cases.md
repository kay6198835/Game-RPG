# Sprint 20 — Session 2: Dungeon Loop — Manual Test Cases

**Stories**: S20-02 (BUG-108), S20-03 (BUG-096), S20-05 (BUG-097)
**Story type**: S20-02 Config/Data (ADVISORY, smoke); S20-03 Logic (BLOCKING — automated test impossible, BUG-084; manual substitute); S20-05 Logic or Config/Data by fix route
**Evidence**: this sheet; automated-test gap recorded in the sign-off

## Common preconditions

- Session 1 smoke PASS (or PASS WITH WARNINGS) on the same commit. Commit: `________`
- Start from `MainGamePlay` → New Game unless a case says otherwise.
- To identify the current room: select `RoomGridController` / `RoomGeneraterController` in the Hierarchy and read the loaded room JSON name (or the room name logged on load). A room type not reached in 3 runs is BLOCKED (not reached), not FAIL.

---

### S2-01 — Knight assets migrated on disk
- **Story**: S20-02 (BUG-108)
- **Preconditions**: Fix committed; Editor open.
- **Steps**:
  1. Open each of `Assets/SO/Enemy/Enemy - Knight.asset`, `Enemy - Knight 1..4.asset` in the Inspector.
  2. Read the `Stats` and weapon fields.
  3. Run `git diff` / open the YAML of each asset.
- **Expected Result**: All 5 assets show a non-null `Stats`; YAML key is `stats:` (not only `statsSO:`); weapon field assigned (or the re-author decision is recorded in BUG-108).
- **Actual Result**:
- **Pass/Fail**:

### S2-02 — Knights spawn without stat exceptions
- **Story**: S20-02 (BUG-108)
- **Preconditions**: S2-01 PASS; a run in progress.
- **Steps**:
  1. Enter a combat room that spawns Knights.
  2. Filter Console to Errors.
- **Expected Result**: Each Knight shows an HP bar; 0 `ArgumentException` ("The Object you want to instantiate is null") and 0 NRE from `EntityStatsHandler` / `EntityEffectStats`.
- **Actual Result**:
- **Pass/Fail**:

### S2-03 — Knights take damage and die; room clears
- **Story**: S20-02 (BUG-108)
- **Preconditions**: S2-02 in the same room.
- **Steps**:
  1. Attack each Knight until it dies.
- **Expected Result**: HP bar drops per hit; each Knight plays death and despawns; doors open after the last one; 0 exceptions.
- **Actual Result**:
- **Pass/Fail**:

### S2-04 — Start room doors open
- **Story**: S20-03 (BUG-096) / S20-05 (BUG-097)
- **Preconditions**: New run; first room loaded.
- **Steps**:
  1. Record whether the first room has champion monuments.
  2. Do not attack. Wait 3 s and check the doors.
  3. If doors are still closed and monuments exist, confirm a champion (G → Confirm).
- **Expected Result**: Doors open on load, or — if the S20-03/S20-05 fix records "start room opens on champion confirm" — no later than champion confirm. Record which rule applied. The player is never sealed in.
- **Actual Result**:
- **Pass/Fail**:

### S2-05 — Rest room opens on load
- **Story**: S20-03 (BUG-096)
- **Preconditions**: Run in progress; reach `RestRoom_Campfire`.
- **Steps**:
  1. Enter the room through a door; do nothing for 3 s.
- **Expected Result**: Doors open without any kill; no enemy spawns.
- **Actual Result**:
- **Pass/Fail**:

### S2-06 — Shop room opens on load
- **Story**: S20-03 (BUG-096)
- **Preconditions**: Reach `ShopRoom_Merchant`.
- **Steps**:
  1. Enter the room; do nothing for 3 s.
- **Expected Result**: Doors open without any kill; no enemy spawns.
- **Actual Result**:
- **Pass/Fail**:

### S2-07 — Buff room opens on load
- **Story**: S20-03 (BUG-096)
- **Preconditions**: Reach `BuffRoom_PowerShrine`.
- **Steps**:
  1. Enter the room; do nothing for 3 s.
- **Expected Result**: Doors open without any kill; no enemy spawns.
- **Actual Result**:
- **Pass/Fail**:

### S2-08 — Boss room opens on load
- **Story**: S20-03 (BUG-096)
- **Preconditions**: Reach `BossRoom_ThroneArena` (end cell after S20-05).
- **Steps**:
  1. Enter the room; do nothing for 3 s.
- **Expected Result**: Doors open (room has no spawn marker), or the owner-recorded boss-room rule in BUG-096 applies; player is never sealed in. Record which.
- **Actual Result**:
- **Pass/Fail**:

### S2-09 — Combat room still sealed until cleared (regression)
- **Story**: S20-03 (BUG-096)
- **Preconditions**: Enter any `CombatRoom_*`.
- **Steps**:
  1. Do not attack for 3 s; check doors.
  2. Kill all enemies; check doors.
- **Expected Result**: Doors closed in step 1; open after the last kill in step 2.
- **Actual Result**:
- **Pass/Fail**:

### S2-10 — Revisited cleared room stays open
- **Story**: S20-03 (BUG-096)
- **Preconditions**: At least one non-combat room and one cleared combat room visited.
- **Steps**:
  1. Walk back into each previously visited room.
- **Expected Result**: Doors open on entry; no enemies respawn; 0 exceptions.
- **Actual Result**:
- **Pass/Fail**:

### S2-11 — Start / end room order across 3 runs
- **Story**: S20-05 (BUG-097)
- **Preconditions**: Fix committed (reordered `Maze_Storage.asset` or selection by `RoomType`).
- **Steps**:
  1. New Game; record the first room's JSON name.
  2. Walk the maze to the end cell (or read the end cell's room from `RoomGeneraterController`); record its name.
  3. Record every other room name loaded.
  4. Exit Play Mode; repeat steps 1-3 twice more (3 runs).
- **Expected Result**: All 3 runs: first room = `StartRoom_Entrance`, end cell = `BossRoom_ThroneArena`; neither name appears in any other cell.
  | Run | First room | End room | Start/Boss elsewhere? |
  |---|---|---|---|
  | 1 | | | |
  | 2 | | | |
  | 3 | | | |
- **Actual Result**:
- **Pass/Fail**:

### S2-12 — Champion monuments reachable at run start
- **Story**: S20-05 (BUG-097)
- **Preconditions**: Run 1 of S2-11.
- **Steps**:
  1. In the first room, look for the three champion monuments (Paladin / Archer / Mage).
- **Expected Result**: Champion select is available in the first room (monuments moved to `StartRoom_Entrance`, or an equivalent owner-recorded path). Not only in the boss room at the end.
- **Actual Result**:
- **Pass/Fail**:
