# Cross-GDD Review Report

**Date**: 2026-05-19
**GDDs Reviewed**: 4 system GDDs + game-concept + systems-index
**Systems Covered**: Character, Weapons, Skills, Map

---

## Consistency Issues

### Blocking
**None.** The 4 system GDDs were authored from the same source (the codebase) in one session and reference each other coherently. All flagged gaps are real code-level issues, not document-level contradictions.

### Warnings

**⚠️ Dependency: Animation system referenced but never authored**
- `character-system.md`, `weapons-system.md`, `skill-ability-system.md` all list **Animation System** as a dependency
- `systems-index.md` lists it as **MVP, Not Started** (no GDD file)
- The combat damage chain depends on `AnimationTrigger` events — undocumented timing rules
- **Recommendation**: write `design/gdd/animation-system.md` before architecture, or fold timing rules into character-system.md

**⚠️ Stale reference: `EntityData.rangeCheckChase` field**
- `character-system.md` lists this as a tuning knob (the field intended to replace the hardcoded 10f chase range)
- The field does not exist in the current `EntityData` SO
- No other GDD references it — unilateral promise to a field that hasn't been added

**⚠️ Player damage hit-count asymmetry**
- `weapons-system.md`: `WeaponMelee.Attack()` should use `OverlapCircleAll` (multi-hit AoE)
- `weapons-system.md`: `EntityWeaponMelee.Attack()` uses `OverlapCircle` (single hit)
- Stated intentionally in the same doc — consistent design, but worth confirming: is multi-hit a deliberate player buff, or should both sides be symmetrical?

---

## Game Design Issues

### Blocking
**None.**

### Warnings

**⚠️ No `game-pillars.md` defined**
- All 4 GDDs describe player fantasy ("agile, snappy", "rhythmic combat", "tense room locking") but no document codifies these as binding pillars
- Without pillars, future feature additions cannot be checked against intent
- **Recommendation**: extract pillars from `game-concept.md` Core Fantasy section into a standalone `design/gdd/game-pillars.md`

**⚠️ No difficulty progression rule**
- `map-system.md`: 4×4 = 16 rooms, but enemy difficulty per room is not specified
- No GDD answers "do later rooms have stronger or more enemies?"
- Room-clear loop has no escalation rule — every room is theoretically the same
- **Recommendation**: add a "Difficulty progression" section to `map-system.md` — even just "linear: +10% enemy health per room"

**⚠️ Talent card distribution undefined**
- `skill-ability-system.md`: "3 `InternalSkillSO` cards from the pool" — pool size, weighting, rarity not specified
- Without rules, card selection has no shape — could be 3 garbage cards or 3 game-winning cards
- **Recommendation**: define pool composition and selection algorithm

### Info

**ℹ️ Player attention budget is healthy** — ~4 active systems during the core loop (Movement, Combat direction, Skills cooldown, Enemy tracking). Within the 3–4 comfortable limit.

**ℹ️ No competing progression loops** — Talent cards are the only progression system in demo scope. Single dominant loop confirmed.

**ℹ️ No economic loops in scope** — No currency, shop, or inventory. Economic balance N/A for demo.

**ℹ️ Player fantasy is coherent** — All 4 GDDs reinforce the same identity: agile melee combatant, weapon-first, run-based growth. No conflict.

---

## Cross-System Scenario Issues

**Scenarios walked:** 4

### Blockers

**🔴 Player kills final enemy in room → room clear → upgrade → next room**
- Systems: Character → Weapons → Map → Skill (talents)
- **Step where it breaks**: Enemy death
  - `character-system.md`: `EntityDeathState` extends MonoBehaviour (broken); `EntityBasicState` death block empty
  - `map-system.md`: Assumes `ON_ENEMY_DEATH` event fires to decrement `RoomCell.enemyCount`
  - `EventID` enum has neither `ON_ENEMY_DEATH` nor `ON_ROOM_CLEAR`
- **Failure mode**: Entire core loop cannot complete. Enemy can't die → room can't clear → no talent cards → no progression
- **Resolution**: All 3 GDDs already specify the fix — implementation work, not design work

**🔴 Player health reaches 0 → death → restart**
- Systems: Character → Map (scene reload)
- **Step where it breaks**: `Core.TakeDamage` decrements health but no `ON_PLAYER_DEATH` event fires; no `GameManager` listens
- **Failure mode**: Player health can reach 0 but nothing happens — game enters unrecoverable broken state
- **Resolution**: Bug #6 from CLAUDE.md — add event to enum, emit on death, listener reloads `StartScene`

**🔴 Player uses skill twice in a row**
- Systems: Skill → Character
- **Step where it breaks**: Cooldown reset
  - `skill-ability-system.md` flags: "no concrete `ActivateSkill.Exit()` override calls `SetCanUseAbility(true)` — ability locks permanently after first use"
- **Failure mode**: First skill use sets `canUseAbility = false` and is never re-enabled
- **Resolution**: Implementation gap in `DashAbility.Exit()`, `SlashAbility.Exit()`, `BlockAbility.Exit()`

### Warnings

**⚠️ Slash projectile fires with enemy inside the 3-unit spawn radius**
- Systems: Skill → Weapons → Character
- `skill-ability-system.md`: Slash spawns at `player.position + DirectionMouseVector × 3 units`
- If an enemy is between the player and the 3-unit mark, the projectile spawns **inside or beyond** the enemy and never hits them
- **Outcome**: Slash misses point-blank targets. Not broken, but feels bad
- **Resolution**: Spawn offset should be smaller, OR projectile should raycast back toward player to detect overshoot

---

## GDDs Flagged for Revision

| GDD | Reason | Type | Priority |
|-----|--------|------|----------|
| *None* — all flagged issues are code-level gaps the GDDs already document accurately | — | — | — |

The GDDs themselves are internally consistent and accurately describe both current state and intended design. No revisions to documents needed.

---

## Verdict: **CONCERNS**

**Why not PASS:** Three blocking cross-system scenarios cannot complete end-to-end in current code. The GDDs document this honestly, but the demo cannot ship without these fixes.

**Why not FAIL:** All blockers are code-implementation gaps, not GDD contradictions. The GDDs themselves are sound enough to drive architecture and stories. Nothing in the documents themselves blocks `/create-architecture`.

### Required actions before demo (not before architecture)
1. Implement `ON_ENEMY_DEATH`, `ON_PLAYER_DEATH`, `ON_ROOM_CLEAR` in `EventID` enum
2. Fix `EntityDeathState` to extend `EntityState`; wire transition in `EntityBasicState`
3. Add `Core.TakeDamage` death check + emit event; create `GameManager` listener
4. Each `ActivateSkill.Exit()` must call `SetCanUseAbility(true)` after `cooldownTime` elapses

### Recommended GDD additions before architecture
- `design/gdd/game-pillars.md` — codify the binding pillars
- `design/gdd/animation-system.md` — currently a phantom dependency
- Difficulty progression rule in `map-system.md`
- Talent card pool & selection rules in `skill-ability-system.md`
