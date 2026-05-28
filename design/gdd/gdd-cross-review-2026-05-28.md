# Cross-GDD Review Report

**Date**: 2026-05-28
**GDDs Reviewed**: 6 system GDDs + game-concept + systems-index
**Systems Covered**: Character, Weapons, Skills, Map, Animation
**Previous review**: gdd-cross-review-2026-05-19.md (verdict: CONCERNS)
**Entity registry**: absent — full GDD reads used

---

## Consistency Issues

### Blocking

**🔴 Block Skill Scope Contradiction — RESOLVED this session**
- `game-concept.md` MVP: *"Two active skills (Slash on E, Block on RMB)"* — Block listed as MVP
- `weapons-system.md` had: *"Block Mechanic — Out of scope for the demo"*
- **Fix applied**: `weapons-system.md` Block section updated to say Block is in MVP scope but
  unimplemented (`[GAP]`), consistent with `game-concept.md` and `skill-ability-system.md`

### Warnings

**⚠️ Dependency Asymmetry — Map ↔ Skills**
- `skill-ability-system.md` lists Map as a dependency (Map → Skills via ON_ROOM_CLEAR)
- `map-system.md` Dependencies did not list Skills as a downstream dependent
- **Fix applied**: Skills dependency added to `map-system.md`

---

## Game Design Issues

### Blocking
**None.**

### Warnings (all carried from 2026-05-19 — not yet resolved)

**⚠️ No `game-pillars.md` defined**
- All system GDDs share an implicit player fantasy but no binding pillars document exists
- Without pillars, future feature additions cannot be checked against intent
- **Recommendation**: extract from `game-concept.md` Core Fantasy into `design/gdd/game-pillars.md`

**⚠️ No difficulty progression rule**
- `map-system.md` defines 4×4 = 16 rooms but no GDD specifies enemy scaling per room
- All 16 rooms are theoretically identical in difficulty — no escalation
- **Recommendation**: add a brief "Difficulty progression" section to `map-system.md`

**⚠️ Talent card pool/selection undefined**
- `skill-ability-system.md`: "3 InternalSkillSO cards from the pool" — pool size, weighting,
  rarity not specified
- **Recommendation**: define pool composition and selection algorithm before UI implementation

---

## Cross-System Scenario Issues

**Scenarios walked**: 4

### Blockers (all carried from 2026-05-19 — no implementation progress this month)

**🔴 Enemy death → room clear → upgrade → next room**
- Systems: Character → Map → Skills
- Break: `EntityDeathState` extends `MonoBehaviour` (not `EntityState`); `EntityBasicState`
  death block empty; `ON_ENEMY_DEATH` not in `EventID` enum
- CLAUDE.md bugs: #7, #8
- Status: No code progress. Blocks entire core loop.

**🔴 Player health → 0 → death → restart**
- Systems: Character → EventManager → GameManager
- Break: `Core.TakeDamage()` decrements health but no `ON_PLAYER_DEATH` fires; no GameManager
- CLAUDE.md bug: #6
- Status: No code progress. Player can reach 0 HP with no consequence.

**🔴 Skill cooldown reset after use**
- System: Skills
- Break: No concrete `ActivateSkill.Exit()` override calls `SetCanUseAbility(true)` —
  ability locks permanently after first use
- Status: No code progress. All active skills are single-use per session.

### Info

**ℹ️ Map save/load changes (commits ca19ad4, e4fe519)**
- Two commits this month touched map save/load logic
- `map-system.md` documents a potential `JsonUtility` TileBase serialization bug
- New save/load code may interact with this risk — warrants a code-level check

---

## GDDs Flagged for Revision

| GDD | Reason | Type | Priority |
|-----|--------|------|----------|
| `weapons-system.md` | Block scope contradiction | Consistency | **Fixed this session** |
| `map-system.md` | Missing Skills as downstream dependent | Consistency | **Fixed this session** |
| `map-system.md` | Difficulty progression rule still absent | Design | Warning |
| `skill-ability-system.md` | Talent card pool/selection rules undefined | Design | Warning |

---

## Verdict: **CONCERNS**

**Resolved this session**: Block scope contradiction (weapons vs game-concept); map↔skills dependency asymmetry.

**Unresolved document warnings**: 3 design gaps (pillars, difficulty curve, talent pool) carried from last review.

**Unresolved code blockers**: 3 cross-system scenarios still broken at implementation level. GDDs accurately document all three. No document changes needed for code issues.

**No new architecture-blocking contradictions** introduced this month.

### Required code work before demo
1. Add `ON_ENEMY_DEATH`, `ON_PLAYER_DEATH`, `ON_ROOM_CLEAR` to `EventID` enum
2. Fix `EntityDeathState` to extend `EntityState`; fill `EntityBasicState` death block
3. Add death check + event to `Core.TakeDamage()`; create `GameManager` listener
4. Each `ActivateSkill.Exit()` must call `SetCanUseAbility(true)` after cooldown delay
