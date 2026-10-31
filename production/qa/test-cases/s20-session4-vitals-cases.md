# Sprint 20 — Session 4: Vitals / Death — Manual Test Cases

**Stories**: S20-08 (BUG-065, BUG-086), S20-09 (BUG-092, BUG-066/070), S20-10 (BUG-115)
**Story type**: Logic (+ Integration for S20-08) | **Evidence**: automated test required but impossible (BUG-084) — this sheet is the manual substitute; gap reported in sign-off | **Gate**: BLOCKING
**Run only after S20-F1 is merged** — it rewrites `VitalStatsBase` and the stat effects; results taken before the merge do not count.

## Common preconditions

- Session 1 PASS on the merged commit. Commit: `________`
- Before the session: `git status` shows no modified `Assets/SO/Stat/*.asset` (record the output).
- Debug aids allowed: debugger breakpoints with hit count, Inspector Debug mode. No committed code or asset changes for testing; any temporary asset is created under a scratch folder and deleted before `git status` checks.

---

### S4-01 — No movement during death animation
- **Story**: S20-08 (BUG-065)
- **Preconditions**: Player in a combat room, HP low.
- **Steps**:
  1. Hold W (or any direction) continuously.
  2. Let an enemy reduce HP to 0 while still holding the key.
  3. Watch the player Transform position and `Rigidbody2D.velocity` (Inspector) until the game-over panel shows.
- **Expected Result**: From death-state enter to game-over panel, position delta = 0 and `Rigidbody2D.velocity` = (0, 0).
- **Actual Result**:
- **Pass/Fail**:

### S4-02 — ON_PLAYER_DEATH emitted once
- **Story**: S20-08 (BUG-086)
- **Preconditions**: IDE debugger attached; breakpoint (log-only, count hits) on the `EventManager.Emit(EventID.ON_PLAYER_DEATH)` line in `PlayerDeathState`.
- **Steps**:
  1. Let the player die.
  2. Wait 10 s on the game-over panel; read the hit count.
- **Expected Result**: Hit count = 1; game-over panel shown once; Console has no line repeating per frame.
- **Actual Result**:
- **Pass/Fail**:

### S4-03 — Respawn returns a playable state
- **Story**: S20-08
- **Preconditions**: S4-02 on the game-over panel.
- **Steps**:
  1. Press Respawn.
  2. Move, attack, cast `1`.
- **Expected Result**: Scene reloads; HUD bound (HP/Mana full, hotbar populated); player moves, attacks and casts; 0 exceptions.
- **Actual Result**:
- **Pass/Fail**:

### S4-04 — Second death after respawn
- **Story**: S20-08
- **Preconditions**: S4-03 PASS, breakpoint counter reset.
- **Steps**:
  1. Die again; wait 10 s.
  2. Press Respawn.
- **Expected Result**: Hit count = 1 for the second death; one game-over panel; Respawn works again.
- **Actual Result**:
- **Pass/Fail**:

### S4-05 — Per-time effect fields are serialized
- **Story**: S20-09 (BUG-092)
- **Preconditions**: Merged commit.
- **Steps**:
  1. Find every asset of type "Recovery - Reduction Per Time For Duration" (Project search `t:RecoveryReductionPerTimeForDuration`). Record the list (may be empty).
  2. For each, read `Per Time` and `Time Count` in the Inspector.
- **Expected Result**: Both fields visible and editable; `Time Count` ≥ 1 on every asset. If the list is empty, record it and use S4-06's scratch asset.
- **Actual Result**:
- **Pass/Fail**:

### S4-06 — DoT / HoT applies N ticks at interval T
- **Story**: S20-09 (BUG-092)
- **Preconditions**: A live ability using the effect, or a scratch copy of an ability with the effect set to `Per Time` = 1, `Time Count` = 3 (not committed).
- **Steps**:
  1. Record the target stat value (HP for a DoT/HoT; the max value if the effect adjusts max).
  2. Apply the effect once; record the value every second for N+2 s.
- **Expected Result**: The value changes exactly `Time Count` times, about `Per Time` s apart, then stops. Not one instant change.
  | t (s) | 0 | 1 | 2 | 3 | 4 | 5 |
  |---|---|---|---|---|---|---|
  | value | | | | | | |
- **Actual Result**:
- **Pass/Fail**:

### S4-07 — No KeyNotFoundException across the vitals paths
- **Story**: S20-09 (BUG-066/070)
- **Preconditions**: Run in progress; Console filtered to Errors.
- **Steps**:
  1. Take damage from an enemy.
  2. Damage and kill an enemy.
  3. Pick up a recovery item (Q), if one drops.
  4. Cast a Mana-cost ability and Avatar of Light (HP cost).
  5. Let a timed buff (Blessing) expire.
- **Expected Result**: 0 `KeyNotFoundException`, 0 NRE from `VitalStatsBase` / `EffectReceiverBase`; each bar changes in the expected direction.
- **Actual Result**:
- **Pass/Fail**:

### S4-08 — HP seeded to max on spawn and respawn
- **Story**: S20-09 (BUG-066/070 `Reborn()` audit)
- **Preconditions**: New run; also one respawn (S4-03).
- **Steps**:
  1. Before any hit, read player current HP (Inspector) and max HP (stats panel C).
  2. Take one hit of known damage; read current HP.
  3. Repeat after a respawn, and for one pooled enemy on its second spawn.
- **Expected Result**: Current HP = max HP before the first hit; after the hit, current = max − damage (not max again, not 0). Same after respawn and for the re-spawned enemy.
- **Actual Result**:
- **Pass/Fail**:

### S4-09 — Play Mode leaves stat assets clean
- **Story**: S20-10 (BUG-115)
- **Preconditions**: `git status` clean on `Assets/SO/Stat/`.
- **Steps**:
  1. Play: pick each champion once (Paladin, Archer, Mage across runs), equip/pick up a weapon, spend stat points in the stats panel (C), cast Blessing, die and respawn.
  2. Exit Play Mode; File → Save Project.
  3. Run `git status Assets/SO/Stat`.
- **Expected Result**: No modified file under `Assets/SO/Stat/`.
- **Actual Result**:
- **Pass/Fail**:

### S4-10 — Stats do not accumulate across runs
- **Story**: S20-10 (BUG-115)
- **Preconditions**: New run, Paladin picked, no points spent.
- **Steps**:
  1. Open stats panel (C); record STR and PhysicalDamage.
  2. Die → Respawn, pick Paladin; record again.
  3. Return to menu → New Game, pick Paladin; record again.
- **Expected Result**: STR and PhysicalDamage identical in all three readings.
- **Actual Result**:
- **Pass/Fail**:

### S4-11 — Leaked modifiers removed from player stat assets
- **Story**: S20-10 (BUG-115)
- **Preconditions**: Fix committed.
- **Steps**:
  1. Open `PaladinStats.asset`, `ArcherStats.asset`, `MageStats.asset`, `BaseStats.asset` (YAML or Inspector Debug mode).
  2. Check every stat's `modifiers` list.
- **Expected Result**: No runtime modifiers (e.g. STR / PhysicalDamage from `SnS_Stat.asset`) stored in any of the four assets.
- **Actual Result**:
- **Pass/Fail**:
