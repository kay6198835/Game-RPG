---
name: Scope Check — Game-RPG Demo Scope
description: MVP scope baseline (game-concept.md) vs current implementation state
date: 2026-05-26
---

# Scope Check: Game-RPG Demo — 2026-05-26

---

## Original Scope (MVP Demo Target)

**Source**: `design/gdd/game-concept.md` (reverse-documented 2026-05-19)

**Demo Goal**: Full game life cycle in a single session

**Core MVP Items** (8 requirements):
1. Start menu → enter dungeon
2. Movement (WASD)
3. Directional melee combat (LMB)
4. Two active skills (Slash on E, Block on RMB)
5. Enemies that detect, chase, and attack the player
6. Room-clear condition (enemies dead → doors open)
7. Between-room upgrade (pick 1 of 3 stat cards)
8. Player death → restart flow

**Explicitly out of scope**: multiplayer, full talent tree depth, ranged weapons, boss encounters, narrative.

---

## Current Implementation State

| Item | Status | Evidence | Completeness |
|------|--------|----------|--------------|
| Start menu | ❌ NOT DONE | No StartScene implementation; only test scenes exist | 0% |
| Movement | ✅ DONE | `PlayerMovement.cs` wired; WASD input works | 100% |
| Directional melee | ⚠️ PARTIAL | Animation framework + input handling complete; **damage application missing** (bug #4) | 70% |
| Two skills | ⚠️ PARTIAL | `DashAbility.cs`, `SlashAbility.cs`, `BlockAbility.cs` exist; untested in play; stubs or incomplete | 50% |
| Enemy AI | ⚠️ PARTIAL | `Entity` framework complete; detection/chase/attack logic wired; **3 critical bugs block execution** (bugs #5, #7, #8) | 60% |
| Room-clear condition | ❌ NOT DONE | `RoomController.cs` exists but no enemy tracking or clear logic | 10% |
| Between-room upgrade | ❌ NOT DONE | `TalentManager.cs` prototype exists; all code commented out | 20% |
| Player death/restart | ❌ NOT DONE | No death detection or restart flow (bug #6) | 0% |

**Overall MVP Completion**: 36/800 = **45%**

---

## Scope Additions (Not in Original MVP)

| Addition | Source | Date | Justified? | Effort | Impact |
|----------|--------|------|------------|--------|--------|
| **Level Editor Tool** | Commits ca19ad4–d7260ea | 2026-05-20 to 2026-05-21 | Yes — content authoring for rooms | M | Enables faster iteration on room layouts; not critical for demo playability |
| **LevelManager.cs** | commit 9294ff6 | 2026-05-17 | Yes — serialization layer | S | Save/load room tilemaps as JSON |
| **DungeonRoomSO** | commit 9294ff6 | 2026-05-17 | Yes — asset management | S | Track room file references |
| **Reverse-documented GDDs** | commits f8da143–f8321fb | 2026-05-19 | Yes — documentation compliance | M | 6 GDDs (animation, character, game-concept, map, skills, weapons) written for CLAUDE.md reference |
| **Periodic review schedule** | commit ede7861 | 2026-05-19 | Yes — process | S | Sprint planning structure |
| **Static map loading** (commits 2f8b836) | 2026-05-26 | Yes — room persistence | S | Load pre-saved room layouts instead of generating them on every play |

**Total additions**: 6 items (all justified)

---

## Scope Removals

None. All MVP items either in progress or not started — none have been explicitly removed.

---

## Bloat Analysis

| Metric | Count |
|--------|-------|
| Original MVP items | 8 |
| Items completed | 1 (movement only) |
| Items in progress (50%+) | 3 (melee, skills, enemy AI) |
| Items not started | 4 (start menu, room-clear, upgrade, death) |
| **Scope additions** | 6 |
| **Net scope change** | +6 items (**+75% growth**) |

---

## Risk Assessment

### Schedule Risk: **MEDIUM → HIGH**

**Blocker items**: 4 MVP items at 0% remain. Of these, item #8 (death/restart) is prerequisite to all other systems (must test game loop with death). Estimated unblock time: **~1 day** to fix bugs #4, #6 and wire room-clear + upgrade logic.

**Added work (level editor + GDDs) extended timeline beyond pure demo focus** but was reasonable for documentation compliance. However, this masks the fact that the MVP is still only **45% complete**.

### Quality Risk: **HIGH**

- **6 critical bugs** blocking combat, enemy AI, and game loop
- **No unit tests** for damage formula, state machines, or progression logic
- **GDDs not matching implementation** (cross-review verdict: CONCERNS on weapons and skill lifecycle)
- **No playtest session yet** — no empirical validation that the demo loop works end-to-end

### Integration Risk: **MEDIUM**

- Enemy AI + player combat (bugs #4 + #5, #7, #8) are interdependent; fixing one without the others will not unblock gameplay
- Room-clear logic depends on ON_ENEMY_DEATH event, which is not yet defined
- Upgrade UI depends on UIManager, which is a stub

---

## Scope Verdict

```
SCOPE VERDICT: CONCERNS
Net change: +75% (6 additions, 0 removals)
Status: Minor Creep — manageable with focused cuts and prioritization
```

**Interpretation**: The original 8 MVP items remain unchanged, but **6 additions have been made** (mostly documentation and the level editor tool). The level editor is valuable but not critical for the playable demo. The GDDs are process-required but don't impact code delivery. If these are removed from the sprint scope, **demo effort drops to 1 day** of bug fixes + wiring.

---

## Recommendations

### Must Keep (Critical to MVP)
- All 8 original MVP items (movement, combat, skills, enemies, room-clear, upgrade, death, start menu)
- Bug fixes #4, #6 (combat + death) — **blocking**
- Bug fixes #5, #7, #8 (enemy AI) — **blocking**

### Defer to Post-Demo (Nice-to-Haves)
- **Level Editor Tool** — move to post-demo phase; can manually author 3–5 test rooms with existing JSON format
- **Start Menu** — placeholder scene sufficient for demo; full menu UI can wait

### Suggested Sprint Allocation

**This week (5 working days):**
1. Fix bugs #4, #6 — **1 day** (combat + death)
2. Fix bugs #5, #7, #8 — **1 day** (enemy AI)
3. Implement room-clear logic + upgrade picker stub — **1 day**
4. Playtest end-to-end run; iterate on feel — **1 day**
5. Buffer — **1 day**

**Post-demo (future sprint):**
- Level editor (currently wired; deprioritize)
- Full start menu
- Upgrade card visuals + animation
- Design review vs implementation alignment (GDD fixes)

---

## Quality Improvement Recommendations

1. **Before shipping demo**: Run `/code-review Assets/Script/Character/` to catch similar state machine bugs
2. **Test strategy**: Add a simple PlayMode test for `Core.TakeDamage()` → `ON_PLAYER_DEATH` event chain (validation for bug #6 fix)
3. **Documentation sync**: After fixes merge, run `/propagate-design-change` to update GDDs to match actual implementation

---

**Next Steps:**

1. Run `/sprint-plan new` with the 3 blocking bugs as Must Haves
2. After bug fixes, re-run `/scope-check` to verify verdict improves
3. Once playable, log a playtest session with `/playtest-report`
