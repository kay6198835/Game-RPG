# Architecture Review Report

**Date**: 2026-05-28
**Engine**: Unity 2022.3.62f3 LTS
**GDDs Reviewed**: 6 (character, weapons, skills, map, animation + game-concept)
**ADRs Reviewed**: 0 — architecture directory is empty
**TR Registry**: absent — IDs assigned fresh this review

---

## Traceability Summary

- Total requirements: **26**
- ✅ Covered (formal ADR): **0**
- ⚠️ Partial (informal in technical-preferences.md): **8**
- ❌ Gaps (no coverage): **18**

---

## Coverage Gaps — Full List

| TR-ID | System | Requirement | Suggested ADR | Priority |
|-------|--------|-------------|---------------|----------|
| TR-char-001 | Character | Hierarchical state machine (PlayerState / EntityState) | ADR: State Machine Architecture | Foundation |
| TR-char-002 | Character | Core/EntityCore component hubs only | ADR: State Machine Architecture | Foundation |
| TR-char-003 | Character | INegativeReceiver as sole damage path | ADR: Damage and Health Contract | Foundation |
| TR-char-004 | Character | SO-driven stats (PlayerData, EntityStatsSO) | ADR: ScriptableObject Data Model | Core |
| TR-char-005 | Character | OverlapCircleNonAlloc for enemy detection | ADR: Physics Performance Budget | Foundation |
| TR-char-006 | Character | ON_PLAYER_DEATH event → GameManager → Reborn() | ADR: Damage and Health Contract | Foundation |
| TR-char-007 | Character | 8-direction angle quantization | ADR: Damage and Health Contract | Foundation |
| TR-weap-001 | Weapons | OverlapCircleAll for player melee (multi-hit) | ADR: Weapon and Projectile System | Feature |
| TR-weap-002 | Weapons | AnimatorOverrideController per directional attack | ADR: Animation Event System | Foundation |
| TR-weap-003 | Weapons | Projectile pooling — no Instantiate in Update | ADR: Weapon and Projectile System | Feature |
| TR-weap-004 | Weapons | WeaponMeleeStats: 2 skill slots wired to AbilityHolder | ADR: ScriptableObject Data Model | Core |
| TR-skill-001 | Skills | ActivateSkill 5-phase lifecycle driven by AbilityHolder | ADR: ScriptableObject Data Model | Core |
| TR-skill-002 | Skills | runtimeAnimatorController swap for skill animations | ADR: Animation Event System | Foundation |
| TR-skill-003 | Skills | Per-skill cooldown; Exit() calls SetCanUseAbility(true) | ADR: ScriptableObject Data Model | Core |
| TR-skill-004 | Skills | TalentManager reads InternalSkillSO (no Awake hardcodes) | ADR: ScriptableObject Data Model | Core |
| TR-skill-005 | Skills | ON_ROOM_CLEAR → talent card UI | ADR: Room Clear and Progression | Feature |
| TR-map-001 | Map | MazeController Awake() order — runs before any room populated | ADR: Map and Dungeon Architecture | Core |
| TR-map-002 | Map | Dual grid: RoomMapController + CellMapController | ADR: Map and Dungeon Architecture | Core |
| TR-map-003 | Map | 5 EventID entries: ON_PLAYER_ON_DOOR, ON_LOAD_MAP, ON_LOAD_MAZE_DONE, ON_ENEMY_DEATH, ON_ROOM_CLEAR | ADR: EventManager Static Bus | Foundation |
| TR-map-004 | Map | JSON TileBase serialization via name lookup (not JsonUtility direct) | ADR: Map and Dungeon Architecture | Core |
| TR-map-005 | Map | RoomController enemy count; emits ON_ROOM_CLEAR at 0 | ADR: Room Clear and Progression | Feature |
| TR-anim-001 | Animation | AnimationEventManager as separate bus from EventManager | ADR: Animation Event System | Foundation |
| TR-anim-002 | Animation | Flag-based consumption (reset same frame after use) | ADR: Animation Event System | Foundation |
| TR-anim-003 | Animation | Animator depth contract: weapon=depth-1, skill=depth-0 | ADR: Animation Event System | Foundation |
| TR-sys-001 | System | 60 FPS, 16.7ms budget, 0.1ms per enemy | ADR: Physics Performance Budget | Foundation |
| TR-sys-002 | System | NonAlloc variants in all hot paths; no alloc in Update | ADR: Physics Performance Budget | Foundation |

---

## Required ADRs (Priority Order)

| Order | ADR Title | TR Coverage | Layer |
|-------|-----------|-------------|-------|
| 1 | State Machine Architecture | TR-char-001, TR-char-002 | Foundation |
| 2 | Damage and Health Contract | TR-char-003, TR-char-006, TR-char-007 | Foundation |
| 3 | EventManager Static Bus | TR-map-003, TR-sys-003 | Foundation |
| 4 | Animation Event System | TR-anim-001, TR-anim-002, TR-anim-003, TR-weap-002, TR-skill-002 | Foundation |
| 5 | Physics Performance Budget | TR-char-005, TR-sys-001, TR-sys-002 | Foundation |
| 6 | ScriptableObject Data Model | TR-char-004, TR-weap-004, TR-skill-001, TR-skill-003, TR-skill-004 | Core |
| 7 | Map and Dungeon Architecture | TR-map-001, TR-map-002, TR-map-004 | Core |
| 8 | Room Clear and Progression | TR-map-005, TR-skill-005 | Feature |
| 9 | Weapon and Projectile System | TR-weap-001, TR-weap-003 | Feature |

---

## Engine Compatibility

Engine: Unity 2022.3.62f3 LTS — within LLM training data, no breaking changes expected.

All GDD-stated patterns are valid for Unity 2022.3:
- `Physics2D.OverlapCircleNonAlloc` — ✅ available
- `AnimatorOverrideController` — ✅ stable
- `runtimeAnimatorController` swap — ✅ stable
- `JsonUtility` + TileBase — ⚠️ documented limitation in map-system.md; use name lookup table

No deprecated APIs identified in any GDD.

---

## Verdict: FAIL

**0 formal ADRs** — no requirement has formal architectural coverage. All 26 requirements are either gaps or covered only by informal bullet points in `technical-preferences.md`.

This does not necessarily mean code is wrong — `technical-preferences.md` enforces the correct patterns. But the architecture cannot be audited, onboarded, or validated without formal ADRs.

**Recommended immediate actions**:
1. `/architecture-decision state-machine-architecture`
2. `/architecture-decision damage-health-contract`
3. `/architecture-decision event-manager-bus`
