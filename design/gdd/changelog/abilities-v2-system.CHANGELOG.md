# Changelog — `abilities-v2-system.md`

Change log for [`design/gdd/abilities-v2-system.md`](../abilities-v2-system.md). The document itself is the **current official version**;
this file records **when** it changed, **what** changed (**from → to**) and **why**. Newest first.
Append entries by hand (template in `docs/systems/README.md`).

## 2026-11-13 — Three follow-up confirmations on AB-G7 and AB-L7
- **Commit:** not committed (working tree on `sprint-22`)
- **Changed:** AB-L7, AB-G7, AB-CD1, the cooldown formula, AC-44, EC-8.
- **From → To:**
  - AB-L7: "an interrupted cast starts no cooldown *(inferred)*" → **interrupted after the cost was paid: the ability goes on cooldown for its full `Cooldown`; interrupted before the cost was paid: nothing spent and no cooldown** (second follow-up the same day); the inference is removed.
  - AB-L7: player only → **player and enemy**: an enemy hit while casting is interrupted too.
  - AB-G7: the reading "a held key pressed during another action does not start the cast on return to Idle / Move" is confirmed and written into the rule.
- **Why:** owner answers to the three points the previous entry had marked as the author's reading.
- **Bugs:** none created. Bug-inbox NOTE-20261113-13 amended.

## 2026-11-13 — Owner answered the remaining questions (second round)
- **Commit:** not committed (working tree on `sprint-22`)
- **Changed:** eleven rules re-tagged and reworded, one rule added (AB-S10), two OPEN rules annotated as parked; Open Questions rows struck through or marked pending / deferred; acceptance criteria section E emptied, section F extended (AC-42 … AC-48), AC-13 demoted to "build behaviour"; edge cases EC-6, EC-7, EC-8, EC-23; rule index; status line.
- **From → To:**
  - AB-G7 OPEN → CONFIRMED `[PLANNED]`: abilities activate only in Idle / Move; a press elsewhere does nothing (OQ-14).
  - AB-L9 OPEN → CONFIRMED: the player's animator controller is not restored after a cast (OQ-13). Matches code. The enemy restore (`EntityAbilityState.cs:74-80`) stays AS-BUILT; the owner asked to review it.
  - AB-L7 AS-BUILT → CONFIRMED `[PLANNED]`: a hit interrupts the cast; an unpaid cost is not charged, a paid cost stays spent (OQ-18). "No cooldown" is inferred from AB-CD1 and marked as such.
  - AB-K1 OPEN → CONFIRMED: Archer / Mage slots are empty on purpose in this phase (OQ-10).
  - AB-K2 OPEN → CONFIRMED: Paladin values are temporary test values (OQ-11).
  - AB-H4 AS-BUILT → CONFIRMED, AB-H5 OPEN → CONFIRMED `[PLANNED]`: hold as long as wanted; the value is the time held, capped at `MaxHoldTime`, measured at release (OQ-5). The missing implementation is BUG-083 (already open — no new note).
  - AB-H6 OPEN → CONFIRMED: `Hold` vs `Active` is the designer's choice per ability (OQ-6).
  - AB-G8 AS-BUILT → CONFIRMED `[PLANNED]`: aim is taken at key release (OQ-15).
  - AB-S10 new, CONFIRMED: attack position = the caster's position; the real impact point if it is not complicated (OQ-7b).
  - AB-S8 AS-BUILT → CONFIRMED: three despawn ways — on hit, on lifetime end, on an animation-end event (OQ-17).
  - AB-N7 OPEN → CONFIRMED `[PLANNED]`: an enemy's ability hits the player (OQ-8).
  - AB-E3 stays OPEN — pending: `TryCast()` is reserved for the stacking logic of held effects and sub-effects, design not settled (OQ-4).
  - AB-D6 stays OPEN — deferred: `Passive` = innate effect, not designed in this phase (OQ-12). OQ-9 (enemy hold) deferred until the demo is done.
  - Counts: 19 / 43 / 9 of 71 → **31 CONFIRMED (8 of them `[PLANNED]`) / 39 AS-BUILT / 2 OPEN of 72**; open questions 14 → **3**, all parked.
- **Why:** owner decisions in the review session, 2026-11-13.
- **Bugs:** none created. Bug-inbox NOTE-20261113-7, -8, -11 amended with the decisions; NOTE-20261113-13 added (AB-L7 and AB-G8 not in code; owner decision on hold values relates to BUG-083).

## 2026-11-13 — Owner decided OQ-1, OQ-2, OQ-3, OQ-7 and OQ-16
- **Commit:** not committed (working tree on `sprint-22`)
- **Changed:** five rules re-tagged and reworded; Open Questions rows struck through with the decision; acceptance criteria AC-O1 / AC-O2 / AC-O6 replaced by a new section F (AC-37 … AC-41); edge cases EC-3, EC-9, EC-22; rule index; a `[PLANNED]` note under "How to read".
- **From → To:**
  - AB-CD4 OPEN → CONFIRMED `[PLANNED]`: cooldowns and hold timers count in every state (OQ-1, option a). Code still counts only in Idle / Move / ability state.
  - AB-C3 OPEN → CONFIRMED `[PLANNED]`: an HP cost needs `HP > cost`; it can never bring HP to 0 (OQ-2, option a). Code still allows `HP == cost`.
  - AB-L8 OPEN → CONFIRMED: dying between Start and Do loses the cost, no refund (OQ-3, option a). Matches code.
  - AB-S7 OPEN → CONFIRMED `[PLANNED]`: `damage = baseDamage + coefficient × stat`, stat and coefficient authored per effect; Rain Arrows through the callback (OQ-7, option a). Fields do not exist yet. The hit-position sub-question is split out as OQ-7b.
  - AB-D5 AS-BUILT → CONFIRMED: cooldowns reset on class pick and respawn (OQ-16, option a). Matches code.
  - Counts: 14 / 44 / 13 → **19 CONFIRMED / 43 AS-BUILT / 9 OPEN**; open questions 18 → **14** (13 left + OQ-7b).
- **Why:** owner decision in the review session, 2026-11-13 ("OQ 1, 2, 3, 7, 16: as recommended").
- **Bugs:** none created. Bug-inbox NOTE-20261113-6 amended, NOTE-20261113-12 added (code differs from AB-CD4).

## 2026-11-13 — GDD created (reverse-documented from code)
- **Commit:** not committed (working tree on `sprint-22`, base `origin/sprint-22` `a8725a77`; owner asked for local changes only)
- **Changed:** new document. 71 tagged rules (14 CONFIRMED, 44 AS-BUILT, 13 OPEN), the ability catalogue per class as authored, 23 edge cases, 36 acceptance criteria plus 6 held back on open rules, 18 open questions with options and a recommendation each.
- **From → To:** (no GDD for Abilities v2; `skill-ability-system.md` described the deleted v1) → `abilities-v2-system.md` is the design record for abilities; `skill-ability-system.md` is marked superseded and kept as history.
- **Why:** QA needs a written, checkable source for test cases. Abilities v2 has been the only ability path since 2026-11-04 (`6094aa10`) and had no GDD (`systems-index.md` row 25, demo checklist item 21, BUG-052).
- **Sources read:** `Assets/Script/System/Abilities/**`, `Character/Base/AbilityHolderBase.cs`, `EffectReceiverBase.cs`, `VitalStatsBase.cs`, `PlayerSkillWeaponState.cs`, `PlayerBasicState.cs`, `PlayerInputHandle.cs`, `EntityAbilityState.cs`, `EntityBasicState.cs`; ability and effect assets under `Assets/SO/Skill/{Paladin,Archer,Mage}/`; `Paladin.asset`, `Archer.asset`, `Mage.asset`, the 11 `EntityData` assets; prefabs `Slash-VFX-GO`, `Lightning`, `RainArrows`, `RuneCircle`; bug files BUG-068, 069, 071, 072, 075, 076, 078, 079, 083, 089, 109, 117, 118; ADR-0005.
- **Evidence tier:** STATIC. Nothing was run in Play Mode; no `.cs`, scene, prefab or ScriptableObject was changed. Not verified: the animation-event layout of the ability clips, and what releases `RuneCircle` / `Lightning` (both stated in the document).
- **Bugs:** no bug ID created. Bug-inbox notes NOTE-20261113-6 … -11 added.
