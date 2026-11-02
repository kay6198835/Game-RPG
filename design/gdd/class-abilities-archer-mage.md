---
status: in-design
source: design-first (Phase 1 of the Archer/Mage ability task, 2026-11-02)
date: 2026-11-02
verified-by: pending owner approval
---

# Class Abilities — Archer and Mage

> **Status**: Approved by owner 2026-11-02 (Q1, Q2, Q3, Q5 decided as recommended) · **[PLANNED]** — temporary ability assets exist for 5 of 8 abilities (see the task log); no new behaviour is implemented in code yet
> **Author**: Kay (owner) + Claude (design-system skill, systems math)
> **Last Updated**: 2026-11-02
> **Last Verified**: 2026-11-02 (branch `sprint-21`, HEAD `5f70bfd9`)
> **Implements Pillar**: Fast, committed, rhythmic combat; skills are earned and spent at the right moment (`game-concept.md` › Core Fantasy)
> **Creative Director Review (CD-GDD-ALIGN)**: NOT ASSESSED 2026-11-02 — skipped, `review_mode: lean`

## Summary

This document designs the eight abilities of the two non-Paladin player classes: four for the
**Archer** (ranged kiting, distance burst, mobility) and four for the **Mage** (area damage,
crowd control, mana-hungry, scales with `MagicDamage`). Each ability is specified as player-facing
behaviour first — timeline, targeting, hitbox, effect, cost, feedback, edge cases — and is balanced
against the Paladin kit so the three classes land within ±15% of each other in sustained damage and
differ in range, safety and control instead.

> **Quick reference** — Layer: `Feature` · Priority: `MVP` · Key deps: `Abilities v2 (undesigned)`, `Stat System`, `Damage & Health`, `Input System`, `Animation System`, `Character / Player Controller`, `Enemy AI`

> **Design-first scope.** Per the task brief, this GDD is written against the intended player
> experience, **not** against what the Abilities v2 code can do today. Mapping each behaviour onto
> existing effects, runtimes and prefabs is Phase 2 and is recorded in
> `production/qa/archer-mage-abilities-log-2026-11-02.md`. Several behaviours here (dash, knockback,
> slow, root, freeze, chain, armour penetration, true damage) have no implementation yet.

---

## Overview

A player picks a class at the Champion pedestal in the start room. The class decides the basic
attack (weapon) and the four ability slots bound to keys `1`/`2`/`3`/`4`
(Primary / Secondary / Utility / Ultimate). The Paladin already owns the melee, self-sustaining
role. This system adds two kits that make a run *play* differently, not just look different:

- **Archer** — wins by *distance*. Every ability either rewards standing far away (Hawk Eye) or
  buys distance back (Kick-back Trap, Volley's slow, Rain Arrows' zone). It is mana-light and leans
  on its free basic arrows.
- **Mage** — wins by *area and control*. It has no weapon; its Primary (Fireball) is free and is its
  basic attack. Its three mana spells clear packs and lock them down. It runs out of mana in long
  fights and must choose which spell to keep.

The abilities are authored as data (one ability asset per ability, composed of effect assets) so
every number in this document is a tuning knob. Without this system the Archer and Mage classes
exist only as stat profiles and animations: they can be selected but play like a Paladin with a
different sprite.

**Class names already in the project.** The owner has already created named, animated shells for
these abilities (`Assets/SO/Skill/{Archer,Mage}/Ability/*`, all with 8-direction × 3-state animator
overrides). This GDD keeps those names and their slot bindings from `Archer.asset` / `Mage.asset`:

| Class | Primary (`1`) | Secondary (`2`) | Utility (`3`) | Ultimate (`4`) | Unbound shell |
|---|---|---|---|---|---|
| Archer | Hawk Eye | Kick-back Trap | Volley | Rain Arrows | Piercing Shot (see Open Questions) |
| Mage | Fireball | Chain Lightning | Frost Nova | Meteor | — |

---

## Player Fantasy

**Archer — "the room is a clock and I am always one step ahead of it."** The Archer player reads
the room, pulls enemies into a line, backs off, and lets a fully drawn shot fly across the whole
screen. The power moment is the *long* shot: a charged Hawk Eye from the far wall that punches
through two enemies and nearly kills the Tank behind them. The fear moment is a Fast Swarm closing
in — answered by Kick-back Trap, a hop backwards that leaves a snare where you stood. Reference
feel: Hades' Heart-Seeking Bow charge; Enter the Gungeon's dodge-and-reposition rhythm.

**Mage — "I don't fight the room, I rearrange it."** The Mage player is fragile and slow and knows
it. They stand at the edge of the fight, throw cheap fireballs to soften the pack, then freeze
everything that gets close and drop a meteor on the frozen cluster. The power moment is the
cascade: Frost Nova → Meteor on six frozen enemies → screen shake → an empty room. The tension is
the mana bar: every spell is a decision about the next 20 seconds. Reference feel: Diablo III
Wizard's Frost Nova + Meteor combo; Magicka's "I made a mistake and everything is on fire".

Both fantasies serve the concept's *"skills are on a delay — you earn them, then you spend them at
the right moment"*: the Ultimates are 35–40 s cooldowns with a visible telegraph, and the Mage's
spells compete for one mana pool.

**What the Paladin keeps.** Melee, self-heal (Blessed Slash), self-buff (Blessing, Avatar of
Light), standing in the middle of the pack. Neither new kit heals, and neither new kit buffs its
own damage stats — that keeps the Paladin the only sustain class.

---

## Detailed Design

### Core Rules

#### Rules shared by all eight abilities

1. **Input.** Keys `1`/`2`/`3`/`4` map to Primary / Secondary / Utility / Ultimate. An `Active`
   ability fires on key **press**. The one `Hold` ability (Hawk Eye) charges while held and fires on
   **release**.
2. **Aim.** "Aim direction" means the vector from the caster to the mouse cursor at the moment the
   ability enters its Do phase. "Cursor point" means the cursor's world position, clamped to the
   ability's max range along the aim direction.
3. **Phases.** Every ability runs `Start → Cast → Do → Exit`.
   `Start` = wind-up animation; `Cast` = the committed moment (cost is paid here);
   `Do` = the payload spawns; `Exit` = recovery animation. The caster cannot start another ability
   until `Exit` ends.
4. **Cost** is paid on entering `Cast`, never on press. A press that cannot pay plays the
   "not enough mana" feedback and does nothing (no cooldown, no animation).
   *Hold exception (Hawk Eye only):* cost is paid on release, at the start of `Do`.
5. **Cooldown** starts when `Exit` ends (the whole ability is over), not when the key is pressed.
   An ability's full cycle is therefore `Start + Cast + Do + Exit + Cooldown`. The balance
   simulation uses exactly this model.
6. **Cancel.** During `Start` only, pressing Dash (`Space`) cancels the ability: no cost, no
   cooldown. Once `Cast` is reached the ability is committed. *Hold exception (Hawk Eye only):*
   Dash cancels during the whole draw (`Start` and `Cast`), since cost is not paid until release.
   Being hit does **not** cancel any ability (no flinch during abilities) — this keeps ranged
   classes readable and avoids cost-without-effect frustration.
7. **Damage.** Every ability hit is `raw = Base + Scale × Stat` (see Formulas), then the target's
   mitigation. Abilities **do not crit** (crit stays a basic-attack property). Damage marked
   **True** ignores Defense.
8. **One hit per instance.** A single projectile, wave or explosion damages a given target at most
   once. Different projectiles of one cast (Volley's arrows) are different instances.
9. **Control effects** (slow, root, freeze, stun, knockback) are new status effects owned by this
   GDD until a status-effect GDD exists. Their resolution rules are in *States and Transitions*.
   **Bosses** take every damage component but convert root/freeze/stun into a 0.3 s stagger and
   ignore knockback; **Tank**-archetype enemies take half duration of root/freeze/stun.
10. **Friendly fire:** none. Abilities never damage the caster.

#### Archer kit — scales with `PhysicalDamage`

##### A1 — Hawk Eye (Primary, `1`, Hold)

| Item | Specification |
|---|---|
| **Fantasy** | "Hold your breath, draw to the cheek, loose." A charged sniper arrow whose damage grows with charge time **and** with distance flown. |
| **Role in kit** | Main single-target damage and the Archer's only answer to high-Defense targets. |
| **Synergy** | Kick-back Trap creates the distance Hawk Eye wants and roots the target so the long shot lands; Volley's slow keeps targets in the far band; Rain Arrows' slow lets you charge fully inside the zone's edge. |
| **Activation** | `Hold`. Press = start drawing (Start); hold = charge (Cast); release = fire (Do). Max charge time 1.2 s; holding past 1.2 s keeps full charge, no auto-fire. Releasing before 0.25 s fires a "snap shot" at charge 0. |
| **Movement / facing** | Can move at 50% move speed while drawing; can rotate aim freely (the sprite turns among the 8 directions). |
| **Cancel** | Dash during draw cancels: no cost, no cooldown. |
| **Timeline** | Start 0.15 s (nock) → Cast = hold, 0–1.2 s → Do 0.10 s (release, projectile spawns) → Exit 0.25 s. **Cost paid on release**. **Cooldown 2.0 s starts when Exit ends.** |
| **Targeting** | Aim direction at release. Spawn 0.8 u in front of the caster. Max range 11 u. No target needed — fires into empty space. **"Distance flown"** = distance from the spawn point to the hit point. |
| **Hitbox** | Projectile, circle radius 0.2 u. |
| **Projectile** | Speed 20 u/s, lifetime 0.55 s (≈11 u). 1 arrow. Charge < 1.0: stops at first enemy. **Full charge (≥ 1.2 s): pierces up to 3 enemies.** No bounce, no homing. Hits a wall → stops, despawns, plays an impact puff. |
| **Effect** | Damage `(25 + 0.5 × PhysicalDamage) × (1 + 0.8 × charge) × distanceMult`, where `charge` = hold time / 1.2 clamped to [0, 1] and `distanceMult` = 1.0 at ≤ 3 u flown, rising linearly to 1.3 at ≥ 7 u. Ignores `50% × charge` of the target's Defense (armour penetration). No status effect. |
| **Cost / CD** | 8 Mana, 2.0 s. *Why:* cheap enough to be the default button; the 2 s gap plus the 1.2 s draw keeps it at roughly one shot every 3.5 s, so free basic arrows still fill most of the time. |
| **Feedback** | 8-direction draw/hold/release animation. A thin line from the bow to the cursor grows brighter with charge; at full charge a white flash on the bow and a "ping" SFX. Arrow trail grows longer with `distanceMult`. Full-charge hit: 0.05 s hit-stop, small screen shake. |
| **Tuning knobs** | Base 25 (15–40), Scale 0.5 (0.3–0.8), MaxHoldTime 1.2 s (0.8–1.6), charge bonus 0.8 (0.5–1.2), distance band 3→7 u, max distance mult 1.3 (1.1–1.5), pierce count 3 (1–5), armour pen 50% (25–75%), cost 8 (5–15), CD 2.0 s (1.5–3.0). |

##### A2 — Kick-back Trap (Secondary, `2`, Active)

| Item | Specification |
|---|---|
| **Fantasy** | "Get off me." The Archer kicks off the ground and hops back, leaving a spring trap where they stood. |
| **Role in kit** | Disengage + peel. The Archer's only movement ability and its only hard control. |
| **Synergy** | Trap root holds the chaser in place for a full-charge Hawk Eye from the new distance; trap + Volley slow chains a Fast Swarm; drop it at a door before Rain Arrows to keep a pack inside the zone. |
| **Activation** | `Active`, on press. |
| **Movement / facing** | The hop is the movement: 3.5 u **away from the aim direction** (i.e. backwards relative to the cursor) over 0.25 s. If the path is blocked by a wall the hop stops at the wall. Invulnerable during the 0.25 s hop. Cannot steer the hop. |
| **Cancel** | Not cancellable (it is the escape). |
| **Timeline** | Start 0.10 s (crouch) → Cast 0.05 s (**cost paid**, trap placed at the caster's start position) → Do 0.25 s (hop, i-frames) → Exit 0.15 s (land). **Cooldown 8 s starts when Exit ends.** Trap arms 0.3 s after placement. |
| **Targeting** | Trap: at the caster's position when Cast begins. Hop: opposite of aim direction. |
| **Hitbox** | Trap trigger: circle radius 1.0 u, triggered by any enemy inside it at the moment it arms or at any later moment (an enemy already standing on it when it arms triggers it at once). Explosion: circle radius 2.0 u, hits all enemies inside (max 8). |
| **Projectile** | None. |
| **Effect** | Explosion damage `30 + 0.6 × PhysicalDamage`. **Root 1.2 s** (cannot move, can still attack if in range) and **knockback 3 u** away from the trap centre — knockback resolves first, then root. An enemy exactly on the centre is pushed along the hop direction (away from the Archer). Trap lifetime 8 s; max 2 traps alive, placing a third removes the oldest. A trap that expires untriggered does nothing. |
| **Cost / CD** | 15 Mana, 8 s. *Why:* the escape must be available once per engagement but not twice in a row. The hop + knockback + root buys ≈ 3.5 s (6.5 u at TrashMelee 2.79 u/s + 1.2 s root); the remaining ≈ 4.5 s is the Archer's window to shoot before the escape is back. |
| **Feedback** | Crouch-and-hop animation (8 directions). Trap: small metal jaw sprite with a faint pulsing ring showing the 1.0 u trigger radius once armed. Trigger: snap SFX, dust burst, 0.08 s hit-stop, enemy sprite tinted brown while rooted. |
| **Tuning knobs** | Hop distance 3.5 u (2.5–5), hop time 0.25 s, i-frames on/off, arm delay 0.3 s (0.1–0.6), trigger radius 1.0 u, explosion radius 2.0 u (1.5–3), damage base 30 / scale 0.6, root 1.2 s (0.6–2.0), knockback 3 u (1–4), max traps 2 (1–3), trap lifetime 8 s, cost 15 (10–25), CD 8 s (6–12). |

##### A3 — Volley (Utility, `3`, Active)

| Item | Specification |
|---|---|
| **Fantasy** | A quick spread of five arrows — a shotgun at close range, a slowing net at mid range. |
| **Role in kit** | Pack damage at close-mid range and the Archer's sustained slow. |
| **Synergy** | Point-blank Volley after Kick-back's root = 5 arrows on one target; the slow keeps targets inside Rain Arrows; slowed enemies stay in Hawk Eye's far band longer. |
| **Activation** | `Active`, on press. |
| **Movement / facing** | Rooted for the 0.5 s cast; faces the aim direction. |
| **Cancel** | Dash during Start (0.15 s) only. |
| **Timeline** | Start 0.15 s → Cast 0.05 s (**cost paid**) → Do 0.05 s (5 arrows spawn together) → Exit 0.25 s. **Cooldown 6 s starts when Exit ends.** |
| **Targeting** | Aim direction; 5 arrows fanned evenly across 40° total (−20°, −10°, 0°, +10°, +20°). Spawn 0.6 u in front. Max range 7 u. |
| **Hitbox** | Each arrow: circle radius 0.15 u. |
| **Projectile** | Speed 14 u/s, lifetime 0.5 s (7 u), 5 arrows, spread 40°, no pierce, no bounce, no homing. Wall: arrow stops and despawns. |
| **Effect** | Per arrow `12 + 0.3 × PhysicalDamage`. Each arrow that hits applies **Slow 30% for 1.5 s**; re-applying refreshes the duration (does not stack strength). One target can be hit by several arrows (at ≤ 1.5 u all five usually connect). |
| **Cost / CD** | 20 Mana, 6 s. |
| **Feedback** | Quick 3-arrow-nock animation (8 directions). Five thin arrow trails. Slowed enemies show a blue-grey footprint particle. Light screen shake only when ≥ 3 arrows hit one target. |
| **Tuning knobs** | Arrow count 5 (3–7), fan 40° (25–60°), per-arrow base 12 / scale 0.3, range 7 u (5–9), slow 30% (15–45%), slow 1.5 s (1–2.5), cost 20 (12–25), CD 6 s (4–8). |

##### A4 — Rain Arrows (Ultimate, `4`, Active)

| Item | Specification |
|---|---|
| **Fantasy** | "Hold that line." Arrows fired high into the air fall on a marked zone for three seconds. |
| **Role in kit** | Zone denial and pack clear. The Archer keeps moving and shooting while the rain does the work. |
| **Synergy** | Traps at the zone edge keep enemies inside; Volley's slow + the zone's own slow pin a pack; charge a Hawk Eye while the zone holds them. |
| **Activation** | `Active`, on press. |
| **Movement / facing** | Rooted only for the 0.4 s cast; free to move, attack and use other abilities during the 3 s rain. |
| **Cancel** | Dash during Start only. |
| **Timeline** | Start 0.25 s (draw upward) → Cast 0.05 s (**cost paid**, zone marked) → Do 0.10 s (release) → Exit 0.10 s. **Cooldown 35 s starts when Exit ends.** Rain: telegraph 0.6 s, then 8 waves every 0.375 s (3.0 s). |
| **Targeting** | Cursor point, clamped to 8 u. If the cursor is inside a wall, the zone centres at the nearest walkable point along the aim line. No target needed. |
| **Hitbox** | Circle radius 2.5 u, re-checked each wave; up to 16 enemies per wave. |
| **Projectile** | Visual only (falling arrows), no physics projectile. |
| **Effect** | Each wave: `10 + 0.25 × PhysicalDamage` to every enemy inside. Enemies inside the zone are **slowed 20%**, lingering 0.5 s after they leave. |
| **Cost / CD** | 50 Mana, 35 s. |
| **Feedback** | Upward-draw animation (8 directions). Telegraph: a dashed ring on the ground for 0.6 s. Waves: arrows streaking down, dust puffs, a tight rhythmic thud SFX per wave. No screen shake (it would fire eight times). |
| **Tuning knobs** | Range 8 u (6–10), radius 2.5 u (2–3.5), telegraph 0.6 s (0.3–1.0), waves 8 (5–12), interval 0.375 s, per-wave base 10 / scale 0.25, slow 20% (0–35%), cost 50 (35–70), CD 35 s (25–45). |

#### Mage kit — scales with `MagicDamage`

> **The Mage has no weapon** (`Mage.asset` `Weapon` is empty). Fireball is therefore designed as the
> Mage's basic attack: free, short cooldown, always available. This is an assumption the owner must
> confirm (Open Question Q2).

##### M1 — Fireball (Primary, `1`, Active)

| Item | Specification |
|---|---|
| **Fantasy** | The bread-and-butter spell: a fist-sized fireball that bursts on impact and leaves enemies smouldering. |
| **Role in kit** | Free sustained damage; the Mage's "basic attack". Small splash that rewards hitting packs. |
| **Synergy** | Burning enemies + Frost Nova: Shatter makes the next Fireball hit +25%; Fireballs into a Meteor-stunned pack hit everything. |
| **Activation** | `Active`, on press. Holding the key re-casts as soon as the cooldown allows (auto-repeat). |
| **Movement / facing** | Can move at 70% speed while casting; faces the aim direction. |
| **Cancel** | Not cancellable (too short to matter). |
| **Timeline** | Start 0.20 s → Cast 0.05 s (cost paid — 0) → Do 0.05 s (projectile spawns) → Exit 0.15 s. **Cooldown 0.6 s starts when Exit ends.** Cycle = 0.45 + 0.6 = 1.05 s. |
| **Targeting** | Aim direction. Spawn 0.7 u in front. Max range 9 u. |
| **Hitbox** | Projectile circle radius 0.25 u; explosion circle radius 1.4 u, max 8 targets. |
| **Projectile** | Speed 11 u/s, lifetime 0.82 s (≈9 u), 1 projectile, no pierce, no bounce, no homing. **Explodes** on the first enemy, on a wall, or at max range. |
| **Effect** | Explosion `44 + 0.2 × MagicDamage` to all inside. Applies **Burn**: 3 s, ticks every 1 s for `4 + 0.05 × MagicDamage` **True** damage. Re-applying refreshes the duration; Burn does not stack. |
| **Cost / CD** | 0 Mana, 0.6 s. *Why:* with no weapon, a mana cost would leave a dry Mage with no way to deal damage at all — a dead state. |
| **Feedback** | One-hand cast (8 directions). Orange projectile with a short ember trail; explosion sprite + low "whump". Burning enemies: small flame particle above the head. No hit-stop (fires too often). |
| **Tuning knobs** | Base 44 (30–55), Scale 0.2 (0.1–0.4), explosion radius 1.4 u (1.0–2.0), speed 11 (8–15), range 9 u, burn tick 4 + 0.05 M, burn 3 s (2–5), CD 0.6 s (0.4–1.0), cast-move speed 70%. |

##### M2 — Chain Lightning (Secondary, `2`, Active)

| Item | Specification |
|---|---|
| **Fantasy** | Point at the nearest threat and let the lightning find the rest. |
| **Role in kit** | Multi-target burst that needs no precise aim, and a micro-interrupt. |
| **Synergy** | Packs held together by Frost Nova chain perfectly; interrupts a Ranged Caster mid-cast; a Shattered target takes the first jump at +25%. |
| **Activation** | `Active`, on press. |
| **Movement / facing** | Rooted for the 0.5 s cast. |
| **Cancel** | Dash during Start only. |
| **Timeline** | Start 0.20 s → Cast 0.05 s (**cost paid** — only if a first target exists) → Do 0.05 s (first strike; each jump follows 0.08 s later) → Exit 0.20 s. **Cooldown 4 s starts when Exit ends.** |
| **Targeting** | **Auto-target, chosen at key press:** the enemy nearest the caster inside a 60° cone around the aim direction and within 7 u. If no enemy qualifies the cast **fails**: no cost, no cooldown, a short "fizzle" spark and SFX. |
| **Hitbox** | Point-to-point (no area). Each jump picks the nearest enemy within 3.5 u of the last target that has not been hit by this cast. |
| **Projectile** | None (instant beam). |
| **Effect** | First target `40 + 0.5 × MagicDamage`; up to **3 jumps** (4 targets total), each jump ×0.8 of the previous hit. Each hit applies **Stun 0.2 s** (interrupts enemy attacks and casts). No target is hit twice. |
| **Cost / CD** | 22 Mana, 4 s. |
| **Feedback** | Two-hand forward cast (8 directions). Jagged white-blue beam between each target, 0.15 s. Crackle SFX pitched up per jump. 0.04 s hit-stop on the first target only. |
| **Tuning knobs** | Base 40 / Scale 0.5, jumps 3 (2–5), falloff 0.8 (0.6–1.0), jump range 3.5 u (2.5–5), acquire range 7 u, acquire cone 60° (30–120°), stun 0.2 s (0–0.5), cost 22 (15–30), CD 4 s (3–6). |

##### M3 — Frost Nova (Utility, `3`, Active)

| Item | Specification |
|---|---|
| **Fantasy** | "Back off." A ring of frost explodes from the Mage and freezes everything close. |
| **Role in kit** | The Mage's defensive panic button and setup tool. |
| **Synergy** | Frozen enemies are the ideal Meteor target (they cannot walk out of the telegraph); Shatter adds +25% to the next hit (Fireball, Chain Lightning first strike, or Meteor impact). |
| **Activation** | `Active`, on press. |
| **Movement / facing** | Rooted for the 0.5 s cast. No facing (centred on the caster). |
| **Cancel** | Dash during Start only. |
| **Timeline** | Start 0.25 s → Cast 0.05 s (**cost paid**) → Do 0.05 s (nova expands, hits instantly) → Exit 0.15 s. **Cooldown 10 s starts when Exit ends.** |
| **Targeting** | Centred on the caster. No target needed. |
| **Hitbox** | Circle radius 3.0 u around the caster, all enemies (max 16). |
| **Projectile** | None. |
| **Effect** | Damage `25 + 0.35 × MagicDamage`. **Freeze 2.0 s** (cannot move or attack). When Freeze ends (by time or Shatter) the target is **Chilled**: slowed 40% for 2 s. **Shatter:** the first **direct-hit** damage instance from the Mage on a Frozen target (Fireball impact, Chain Lightning hit, Meteor impact) deals +25% and ends the Freeze early (Chill still applies). Damage-over-time ticks (Burn, burning ground) never trigger Shatter. Tank: Freeze 1.0 s. Boss: no Freeze, Chill only. |
| **Cost / CD** | 30 Mana, 10 s. |
| **Feedback** | Arms-down slam animation (8 directions). White-blue ring expands to 3 u in 0.1 s; frozen enemies get an ice-block overlay and stop animating; Shatter plays a glass-break SFX and shards. Small screen shake on cast. |
| **Tuning knobs** | Radius 3.0 u (2.0–4.0), base 25 / scale 0.35, freeze 2.0 s (1.6–2.5; below 1.6 s the Nova→Meteor combo stops landing), chill 40% for 2 s, shatter bonus 25% (0–50%), cost 30 (20–40), CD 10 s (8–14). |

##### M4 — Meteor (Ultimate, `4`, Active)

| Item | Specification |
|---|---|
| **Fantasy** | Call down the sky. A huge telegraphed impact that stuns, scatters and sets the ground on fire. |
| **Role in kit** | Room-clear and boss-phase burst. |
| **Synergy** | Frost Nova → Meteor is the signature combo (frozen enemies cannot leave the telegraph; Freeze 2.0 s leaves ≈ 0.45 s margin at the fastest input); Chain Lightning's 0.2 s stun can hold a non-frozen enemy for the end of the fall; Fireballs keep Burn ticking on survivors in the fire. |
| **Activation** | `Active`, on press. |
| **Movement / facing** | Rooted for the 0.6 s cast; free to move during the 0.9 s fall. |
| **Cancel** | Dash during Start only. |
| **Timeline** | Start 0.40 s (raise arms) → Cast 0.05 s (**cost paid**, reticle locked) → Do 0.05 s (meteor spawns high) → Exit 0.10 s. **Cooldown 40 s starts when Exit ends.** Fall/telegraph 0.9 s → impact → burning ground 3 s. |
| **Targeting** | Cursor point, clamped to 9 u; same wall rule as Rain Arrows. No target needed. |
| **Hitbox** | Impact: circle radius 3.0 u, max 16. Burning ground: circle radius 2.5 u, ticks every 0.5 s. |
| **Projectile** | Visual falling meteor, no physics. |
| **Effect** | Impact `110 + 1.0 × MagicDamage`, **Stun 1.0 s**, **knockback 1.5 u** from the centre. Burning ground: 6 ticks of `5 + 0.1 × MagicDamage` **True** damage to enemies inside at the tick. |
| **Cost / CD** | 80 Mana, 40 s. *Why:* 80 is ~half of a level-1 Mage's pool — casting it is a commitment that takes Frost Nova away for a while. |
| **Feedback** | Both-arms-up animation (8 directions). Red circle telegraph that fills over 0.9 s; a growing shadow; impact: big flash, 0.1 s hit-stop, strong screen shake, rumble SFX, cracked-earth decal under the fire. |
| **Tuning knobs** | Range 9 u, radius 3.0 u (2.5–4.0), fall 0.9 s (0.6–1.2), base 110 / scale 1.0, stun 1.0 s (0.5–1.5), knockback 1.5 u, ground 3 s × 0.5 s ticks, ground tick 5 + 0.1 M, cost 80 (60–100), CD 40 s (30–60). |

### States and Transitions

#### Ability phase states (per ability instance)

| State | Enter when | Exit to | Notes |
|---|---|---|---|
| Ready | Cooldown ended | Start (press, enough mana) | Hotbar icon bright |
| Start | Key pressed and mana ≥ cost | Cast (timer) · Ready (Dash cancel) | No cost paid yet |
| Cast | Start timer done (Hold: while key held) | Do (timer / release) · Ready (Hawk Eye only: Dash cancel) | **Cost paid on enter** (except Hawk Eye); Hawk Eye stays here while held |
| Do | Cast done / released | Exit | Payload spawns (Hawk Eye: cost paid here) |
| Exit | Do done | Cooldown | Recovery animation; no new ability; **cooldown starts when Exit ends** |
| Cooldown | Exit done | Ready | Payload (rain, trap, ground fire) may still be live |

#### Status effects on enemies

| Effect | Source | Stacking | Interaction |
|---|---|---|---|
| Slow (x%) | Volley 30%, Rain 20%, Chill 40% | Strongest active slow applies; same source refreshes | Slows multiply with nothing — only the strongest counts |
| Root | Kick-back Trap 1.2 s (Tank 0.6 s, Boss → stagger) | Refresh, no stack | Enemy can attack if a target is in range |
| Freeze | Frost Nova 2.0 s (Tank 1.0 s, Boss none) | Refresh, no stack | Cannot move or attack; a direct Mage hit Shatters it |
| Stun | Chain Lightning 0.2 s, Meteor 1.0 s (Tank half, Boss → 0.3 s stagger, max once per 3 s) | Longest remaining wins | Cannot move or attack; interrupts a cast |
| Knockback | Kick-back Trap 3 u, Meteor 1.5 u (Boss immune) | Applied immediately; latest wins | Resolved before root/stun from the same hit |
| Burn | Fireball | Refresh, no stack | True damage |

Priority when several apply at once: Knockback → Stun/Freeze → Root → Slow.

### Interactions with Other Systems

| System | Data in | Data out | Owner |
|---|---|---|---|
| Stat System | `PhysicalDamage`, `MagicDamage`, `Mana`, `ManaRegen`, `MoveSpeed` of the caster | Mana cost deducted from current Mana | Stat System |
| Damage & Health | Target `Defense` | `TakeDamage(raw, origin)` per hit; True-damage hits bypass Defense | Damage & Health |
| Abilities v2 | Ability definition (phases, cost, cooldown, effects) | — | Abilities v2 |
| Input System | Keys `1`–`4` press / hold / release, cursor position, Dash | — | Input |
| Animation System | 8-direction × 3-state override per ability (already authored for all eight) | Animation events mark Start/Cast/Do/Exit | Animation |
| Enemy AI | — | Slow/root/freeze/stun/knockback states the AI must obey | Enemy AI |
| HUD | Ability icon, cooldown, mana | — | UIFlow `SkillHotbar` |
| Character / Champion | `Archer.asset` / `Mage.asset` `abilityBindings` | — | Character |

---

## Formulas

### Ability raw damage

The `ability_raw_damage` formula is defined as:

`ability_raw_damage = Base + Scale × ScalingStat`

**Variables:**
| Variable | Type | Range | Source | Description |
|---|---|---|---|---|
| Base | float | 5–110 | data file (effect asset) | Flat part of the hit |
| Scale | float | 0.1–1.0 | data file | Coefficient on the scaling stat |
| ScalingStat | float | 29–110 (L1–L10) | calculated (Stat System) | `PhysicalDamage` (Archer) or `MagicDamage` (Mage), caster's current value |

**Output Range (direct hits only):** 21.4 (one Volley arrow, L1) to 217.5 (Meteor impact, L10 INT build). Damage-over-time ticks are lower: Burn 5.5 and burning ground 7.9 per tick at L1.
**Example:** Chain Lightning, Mage L10 INT build (`MagicDamage` 107.5): 40 + 0.5 × 107.5 = 93.75.

### Hawk Eye damage

`hawk_eye_raw = (25 + 0.5 × PhysicalDamage) × (1 + 0.8 × charge) × distanceMult`

**Variables:**
| Variable | Type | Range | Source | Description |
|---|---|---|---|---|
| charge | float | 0–1 | calculated | `clamp(holdTime / 1.2, 0, 1)` |
| distanceMult | float | 1.0–1.3 | calculated | `1 + 0.3 × clamp((d − 3) / 4, 0, 1)`, d = distance flown from the spawn point (u) |
| penetration | float | 0–0.5 | calculated | `0.5 × charge` of target Defense ignored |

**Output Range:** 40.6 (snap shot, L1, point blank) to 132.9 (full charge, L10 DEX `PhysicalDamage` 63.6, ≥ 7 u flown: 56.8 × 1.8 × 1.3).
**Example:** L1 (`PhysicalDamage` 31.2), full charge, 5 u: (25 + 15.6) × 1.8 × 1.15 = 84.0; vs Tank (Defense 43.4, 50% ignored): 84.0 − 21.7 = 62.3.

### Damage taken (mitigation)

`final_damage = isTrue ? raw : max(0, raw − Defense × (1 − penetration))`

This is the **live** rule (`EntityNegativeReciver.DamageCalculate`). It makes any hit whose raw
damage is below the target's Defense deal **zero**. See Open Question Q1 for the proposed floor:

`final_damage_floor = max(raw − Defense_eff, 0.2 × raw)`

### Chain falloff

`chain_hit(n) = chain_raw × 0.8^n`, n = 0 (first target) … 3.
**Example:** L1 Mage (`MagicDamage` 29.2): first 54.6, then 43.7, 34.9, 27.9.

### Mana sustainability

`mana_demand = Σ(cost_i / cycle_i)` vs `ManaRegen`, where `cycle_i` = cooldown + lock time.

| Class | Demand at full uptime (Mana/s) | ManaRegen L1 / L10 | Result |
|---|---|---|---|
| Archer | 50/35.4 + 15/8.4 + 20/6.5 + 8/3.5 = **8.6** | 1.88 / 2.6 | Gated; free basic arrows fill the gaps |
| Mage | 80/40.6 + 30/10.5 + 22/4.5 + 0 = **9.7** | 1.88 / 4.76 (INT build) | Gated; free Fireball fills the gaps |

### Balance results (from the rotation simulation)

**Method.** A single-target rotation simulated at 0.05 s steps for 60 s, expected-value damage (no
RNG), starting at full mana. Abilities fire on cooldown in priority order Ultimate → Secondary →
Utility → Primary when mana allows; basic attacks fill the remaining time. TTK = enemy HP ÷ the
60 s DPS against that enemy's Defense. Script: session scratchpad `sim2.ps1` (not committed).

**Assumptions** (each is a knob that moves the result):
- Player L1: every primary 10, no points spent (`PhysicalDamage` 31.2, `MagicDamage` 29.2,
  Mana 143, ManaRegen 1.88, AttackSpeed 0.95, Crit 9% / +158 flat).
- Player L10 "specced": 27 points in the class stat — Paladin STR 23.5 + VIT 23.5
  (`PhysicalDamage` 71.7); Archer DEX 37 (`PhysicalDamage` 63.6, AttackSpeed 1.355);
  Mage INT 37 (`MagicDamage` 107.5, Mana 440, ManaRegen 4.76).
- Enemies from `Assets/SO/Stat/Enemy/*.asset`, at level 1 and grown to level 10 with their own
  `perLevel`. Boss is its asset as authored (level 30, HP 4450, Defense 112.5) at both points.
- Basic attacks: Paladin `PhysicalDamage + 55` with crit, 70% uptime (melee must close);
  Archer **stage damage only** (65 avg, no crit — what `RangeWeapon.OnHit` does today), 90% uptime;
  Mage none.
- Paladin reference: **PaladinBasic** = basic attacks only; **PaladinRef** = basics + Consecrate
  (CD 6 s) + Blessed Slash (CD 3 s), because the authored Paladin cooldowns (1 s) are placeholders.
  Blessing and Avatar of Light are excluded (Avatar carries a test value of +100 000 MagicDamage).
- Hit weights: Hawk Eye always full charge at ~5 u (×1.15); Kick-back Trap triggers 70% of casts;
  3 of 5 Volley arrows hit; target stays in 65% of Rain waves and 80% of Meteor fire ticks.

**Sustained single-target DPS (Defense 0) and 5 s burst:**

| Class | L1 DPS | vs Paladin mid (61.5) | L1 burst 5 s | L10 DPS | vs Paladin mid (83.1) | L10 burst 5 s |
|---|---|---|---|---|---|---|
| PaladinBasic | 64.3 | — | 351 | 90.6 | — | 494 |
| PaladinRef | 58.8 | — | 229 | 75.6 | — | 298 |
| **Archer** | **57.6** | **−6%** | 392 | **85.0** | **+2%** | 569 |
| **Mage** | **54.9** | **−11%** | 442 | **92.2** | **+11%** | 691 |

Both new classes sit inside the ±15% band at both levels. Damage per mana: Archer 25.2 / 30.4,
Mage 24.9 / 13.2, PaladinRef 25.2 / 26.7.

**TTK in seconds, live flat mitigation (single target):**

| Enemy (L1 → L10 HP / Def) | PaladinBasic L1 / L10 | Archer L1 / L10 | Mage L1 / L10 |
|---|---|---|---|
| TrashMelee (152/10.8 → 206/14.4) | 2.7 / 2.5 | 3.2 / 3.1 | 3.4 / 2.7 |
| FastSwarm (44/3.5 → 66.5/5.3) | 0.7 / 0.8 | 0.8 / 0.8 | 0.9 / 0.8 |
| Tank (482/43.4 → 590/52.4) | 13.1 / 10.4 | **23.7 / 26.0** | **36.9** / 18.4 |
| RangedCaster (65/4.5 → 96.5/6.3) | 1.1 / 1.1 | 1.2 / 1.2 | 1.3 / 1.1 |
| Assasin (62/4.4 → 93.5/6.2) | 1.0 / 1.1 | 1.2 / 1.2 | 1.2 / 1.1 |
| Boss L30 (4450/112.5) | 584 / 246 | **2384 / 730** | 710 / 373 |

**Same, with the proposed 20% damage floor (Q1):** Tank — Archer 22.8 / 24.8, Mage 28.7 / 18.2;
Boss — PaladinBasic 254 / 181, Archer 362 / 215, Mage 285 / 162. Other rows unchanged.

**Outliers and dominant strategies:**

1. **Flat Defense vs multi-hit kits (Tank, Boss).** Every Archer ability except Hawk Eye, and the
   Archer's basic arrow (65 raw, no `PhysicalDamage`), sits near or under a Tank's 43–52 Defense.
   Archer TTK on Tank is 1.8–2.5× the Paladin's; Mage at L1 is 2.8×. Against the authored Boss
   (Defense 112.5) the Archer's only non-zero damage is the crit-free full-charge Hawk Eye — the
   2384 s figure means "effectively immune". This is a property of the mitigation rule, not of
   this kit: the Paladin's basic attack also takes 584 s at L1. Fix candidates: the 20% floor (Q1)
   or the multiplicative armour already proposed in `design/balance/combat-balance-2026-07-07.md` §3.
2. **Mage scaling is the steepest.** `MagicDamage` grows 3.7× from L1 to an INT-37 build, versus
   2.3× for a STR build. Mage goes from −11% to +11%. At L15+ it will leave the band unless the
   `Scale` coefficients stay low. Watch this first in playtest.
3. **Ranged burst exceeds melee burst.** Mage 5 s burst is +26% (L1) / +40% (L10) over PaladinBasic;
   Archer +12% / +15%. This is the Ultimate opener (Meteor, Rain Arrows) and resets only every
   35–40 s. Accepted as class identity ("burst by distance", "Mage nukes"), but it means rooms that
   open with a pack die to a single Ultimate.
4. **Kiting dominance (Archer).** Archer `MoveSpeed` 3.2 (3.74 at DEX 37) beats TrashMelee 2.79 and
   Tank 1.84. Combined with an 8 s hop, the Archer can kite melee enemies indefinitely. Only
   FastSwarm (5.3) and Assasin (4.16) can catch it. Accepted for the class fantasy; enemy room
   composition (FastSwarm + RangedCaster) is the counter, not the kit.
5. **Frost Nova lock loop (Mage).** Freeze 2.0 s + Chill 2 s per 10.5 s cycle gives ~38% hard/soft-control
   uptime on a pack. Not a loop on its own; with a future cooldown-reduction upgrade it could
   become one — cap cooldown reduction on Frost Nova at 30%.
6. **Paladin's own kit is DPS-negative.** As authored (1 s cooldowns), PaladinAsIs deals 60.7 /
   80.2 DPS, below PaladinBasic. Paladin abilities do not scale with `PhysicalDamage`, so their
   lock time costs more than they deal. Out of scope here; recorded for the Paladin pass.

---

## Edge Cases

- **If the caster dies during Start or Cast**: the ability is dropped; cost already paid is not
  refunded; no payload spawns; cooldown does not start (Do never began).
- **If the caster dies after Do**: payloads that already exist (Rain zone, traps, Meteor fall,
  burning ground, Burn) keep running to completion. They are owned by the world, not the caster.
- **If the caster is hit during an ability**: no interrupt (Core Rule 6). Damage still applies.
- **If the player changes room while a payload is alive**: all of the caster's payloads (traps,
  Rain zone, Meteor, burning ground, projectiles) despawn on the room transition. Burn on enemies
  of the old room is irrelevant (those enemies are gone). Cooldowns keep counting.
- **If the target dies mid-ability**: Hawk Eye keeps flying (it is aimed, not homing); Chain
  Lightning continues jumping from the dead target's position; Meteor and Rain hit whatever is in
  the area at impact.
- **If Chain Lightning finds no first target**: the cast fails at press — no cost, no cooldown,
  fizzle feedback.
- **If Chain Lightning has fewer than 3 valid jumps**: it stops early. No double-hit.
- **If the Archer's hop path is blocked**: the hop stops at the obstacle; the trap is still placed
  at the start position; i-frames still last the full 0.25 s.
- **If a third trap is placed**: the oldest trap is removed silently.
- **If two traps overlap and one triggers**: only the triggered one explodes; the other stays.
- **If mana runs out while holding Hawk Eye**: cost is paid on release; if the Mana is no longer
  enough at release (spent by nothing else — only possible after a future mana-drain enemy),
  the shot fires as a snap shot at charge 0 and costs nothing.
- **If the cursor is out of range**: ground-targeted abilities (Rain, Meteor) clamp to max range
  along the aim line; inside a wall → nearest walkable point on that line.
- **If an enemy is already Frozen and Frost Nova hits again**: Freeze refreshes to 2.0 s; no
  Shatter is consumed by the nova's own damage.
- **If a Frozen enemy is hit by Rain Arrows / Volley / a trap**: those are not Mage damage — only
  Mage abilities trigger Shatter. Archer and Mage are never in the same run, so this cannot happen
  today; the rule is fixed so a future co-op or companion feature has an answer.
- **If several slows apply**: only the strongest counts; each keeps its own timer.
- **If Meteor knocks an enemy out of the burning ground**: no extra rule; the ground ticks only
  what is inside it.
- **If the boss is hit by root/freeze/stun**: replaced by a 0.3 s stagger, at most once per 3 s.
- **If an ability is pressed during another ability's Exit**: the input is buffered for 0.15 s and
  starts the next ability when Exit ends; any later press is dropped.

---

## Dependencies

| System | Direction | Hard / soft | Interface | GDD |
|---|---|---|---|---|
| Abilities v2 (composition) | this depends on it | Hard | Ability definition: activation type, cooldown, costs, max hold time, effect list, animator override; phase timeline | **None — undesigned** (systems index row 25) |
| Stat System | this depends on it | Hard | Reads `PhysicalDamage`, `MagicDamage`, `Mana`, `ManaRegen`, `MoveSpeed` | `stat-system.md` |
| Damage & Health | this depends on it | Hard | `TakeDamage(float, Vector2)`; mitigation rule; **True** damage needs a bypass path | `character-system.md` |
| Status effects (slow/root/freeze/stun/knockback/burn) | this depends on it | Hard | Apply / refresh / expire per *States and Transitions* | **None** — defined provisionally here |
| Input System | this depends on it | Hard | Keys `1`–`4` press/hold/release, cursor, Dash | in `character-system.md` |
| Animation System | this depends on it | Hard | 8-dir × 3-state clips per ability (authored for all 8) | `animation-system.md` |
| Enemy AI | depends on this | Hard | Must honour the status effects above | in `character-system.md` |
| Weapon System | sibling | Soft | Archer basic arrow (`ArcherWeapon.prefab`); Mage has none | `weapons-system.md` |
| HUD (UIFlow) | depends on this | Soft | Icon, cooldown, cost | `docs/ui/ui-ux-flow.md` |
| Character / Champion selection | depends on this | Hard | `abilityBindings` in `Archer.asset` / `Mage.asset` | `character-system.md` |

Bidirectional gaps: `stat-system.md`, `character-system.md` and `animation-system.md` do not list
this document as a dependent. Not updated here (this task may only create files); recorded in the
task log.

---

## Tuning Knobs

Every per-ability knob with its safe range is listed in that ability's table above. Cross-cutting
knobs:

| Knob | Default | Safe range | Too high | Too low | Interacts with |
|---|---|---|---|---|---|
| Ability crit | off | on/off | Archer burst spikes (+158 flat per crit) | — | Crit Damage flat rule |
| Damage floor (Q1) | 0% (live) | 0–30% | Defense stops mattering | Tank/Boss immune to small hits | All multi-hit abilities |
| Boss CC conversion | 0.3 s stagger / 3 s | 0.2–0.5 s | Boss is lockable | CC useless in boss room | Frost Nova, Meteor, Trap |
| Tank CC duration | 50% | 30–100% | Tank trivial | — | Trap, Frost Nova, Meteor |
| Ability input buffer | 0.15 s | 0.1–0.25 s | Accidental double cast | Dropped inputs feel unresponsive | All |
| Mana regen (class) | from stat asset | — | Mage never chooses | Mage stuck on Fireball | Mage identity |

---

## Visual/Audio Requirements

`art-director` not consulted — Lean mode. Review manually before production.

- Every ability has 8-direction Start/Cast/Do clips already authored (`Assets/Animation/Player/{Archer,Mage}/Ability-Skill/*`).
  ⚠️ The clips are 2.3–5.0 s long (Hawk Eye, Volley, Piercing Shot = 5 s). This design targets
  0.4–0.75 s per ability. The clips must be re-timed or played at a speed multiplier (Phase 2 log).
- Every ground-targeted or delayed hit has a **telegraph** visible to the player: Rain (dashed
  ring 0.6 s), Meteor (filling red circle 0.9 s), Trap (armed ring).
- Status readability: Slow = grey-blue footprints; Root = brown tint; Freeze = ice-block overlay;
  Stun = stars; Burn = small flame. One overlay per enemy at a time, priority as in the status table.
- SFX per ability: draw/charge ping (Hawk Eye), snap (Trap), triple twang (Volley), rhythmic thud
  (Rain), whump (Fireball), crackle (Chain), glass/ice (Nova, Shatter), rumble (Meteor).

📌 **Asset Spec** — after the art bible is approved, run `/asset-spec system:class-abilities-archer-mage`.

## Game Feel

| Ability | Input → visible response | Hit-stop | Screen shake |
|---|---|---|---|
| Hawk Eye | Draw line appears within 1 frame of press | 0.05 s on full charge | small, full charge only |
| Kick-back Trap | Hop starts ≤ 0.15 s after press | 0.08 s on trigger | none |
| Volley | Arrows out at 0.25 s | none | light, ≥ 3 arrows on one target |
| Rain Arrows | Ring at 0.3 s | none | none |
| Fireball | Projectile at 0.25 s | none | none |
| Chain Lightning | Beam at 0.25 s | 0.04 s first target | none |
| Frost Nova | Ring at 0.3 s | none | small |
| Meteor | Reticle at 0.45 s, impact at 1.35 s | 0.1 s | strong |

Target: no ability takes more than 0.45 s from press to the first visible payload, except the two
Ultimates (telegraphed by design).

## UI Requirements

- Hotbar (`SkillHotbar`) shows each slot's icon, a radial cooldown and a red tint when current
  Mana < cost. Fireball shows no cost.
- Hawk Eye shows a charge ring around the cursor while held.
- Rain Arrows and Meteor show the ground reticle following the cursor between press and Cast.
- Chain Lightning: the would-be first target gets a small highlight while the key is held (optional).

📌 **UX Flag — Class Abilities**: this system has UI requirements. Run `/ux-design` for the hotbar
states (cost-blocked, charging, ground-target) before writing stories.

## Cross-References

| This document references | Where | What |
|---|---|---|
| `design/gdd/stat-system.md` | Formulas | `DerivedStatFormula` shape, stat IDs |
| `design/balance/combat-balance-2026-07-07.md` | Formulas (outlier 1) | Multiplicative armour proposal, TTK bands |
| `design/gdd/character-system.md` | Dependencies | Damage & Health contract |
| `design/gdd/animation-system.md` | Visual/Audio | 8-direction clip convention |
| `design/gdd/weapons-system.md` | Formulas assumptions | Basic attack damage |
| `design/gdd/skill-ability-system.md` | Overview | v1 only; v2 has no GDD |
| Paladin ability assets `Assets/SO/Skill/Paladin/Ability/**` | Formulas | Baseline numbers |

---

## Acceptance Criteria

Each criterion is checked in Play Mode in `LoadRandomMap` or the `FastTest/CombatTest` scene with a
`TrainingDummy`, with the class picked at the Champion.

**Shared rules**
- **GIVEN** an Archer with 7 Mana, **WHEN** the player presses `2` (Kick-back Trap, cost 15), **THEN** no animation plays, Mana stays 7, the cooldown does not start and the hotbar shows the cost-blocked tint.
- **GIVEN** any `Active` ability in its Start phase, **WHEN** the player presses Space, **THEN** the ability stops, Mana is unchanged and the ability is Ready again.
- **GIVEN** any ability, **WHEN** its Exit phase ends, **THEN** the cooldown starts at that moment (not at the key press, not at the payload frame).
- **GIVEN** any ability, **WHEN** it hits, **THEN** no crit number is shown.

**Hawk Eye**
- **GIVEN** an Archer L1 (`PhysicalDamage` 31.2) whose dummy (Defense 0) is 7.8 u from the caster (7 u from the arrow spawn point), **WHEN** Hawk Eye is held 1.2 s and released, **THEN** the dummy takes 95.0 ± 1 (40.6 × 1.8 × 1.3).
- **GIVEN** three dummies in a line, **WHEN** a full-charge Hawk Eye is fired along the line, **THEN** all three take damage; **WHEN** a 0.5 s charge is fired, **THEN** only the first does.
- **GIVEN** the player holds `1`, **WHEN** they move, **THEN** move speed is 50% of normal until release.

**Kick-back Trap**
- **GIVEN** the Archer at position X aiming right, **WHEN** `2` is pressed, **THEN** the Archer ends ~3.5 u to the left of X within 0.25 s, takes no damage during the hop, and a trap is at X.
- **GIVEN** an armed trap, **WHEN** an enemy walks onto it, **THEN** enemies within 2.0 u take `30 + 0.6 × PhysicalDamage`, are pushed ~3 u, and cannot move for 1.2 s.
- **GIVEN** two traps placed, **WHEN** a third is placed, **THEN** the first one disappears.

**Volley**
- **GIVEN** a dummy 1 u in front, **WHEN** Volley is cast at it, **THEN** it takes 5 hits of `12 + 0.3 × PhysicalDamage` and moves at 70% speed for 1.5 s.

**Rain Arrows**
- **GIVEN** a dummy at the cursor ≤ 8 u away, **WHEN** Rain Arrows is cast, **THEN** a ring appears, and after 0.6 s the dummy takes 8 hits 0.375 s apart, and the caster can move during the waves.

**Fireball**
- **GIVEN** a Mage with 0 Mana, **WHEN** `1` is pressed, **THEN** Fireball fires.
- **GIVEN** three dummies within 1.4 u of each other, **WHEN** a Fireball hits the middle one, **THEN** all three take the impact damage and then 3 Burn ticks of `4 + 0.05 × MagicDamage` 1 s apart, unaffected by Defense.

**Chain Lightning**
- **GIVEN** no enemy within 7 u in the aim cone, **WHEN** `2` is pressed, **THEN** a fizzle plays, Mana and cooldown are unchanged.
- **GIVEN** four dummies spaced 3 u apart, **WHEN** Chain Lightning hits the first, **THEN** each dummy is hit exactly once with damage ×1.0, ×0.8, ×0.64, ×0.512.

**Frost Nova**
- **GIVEN** an enemy within 3 u, **WHEN** Frost Nova is cast, **THEN** it cannot move or attack for 2.0 s and then moves at 60% speed for 2 s.
- **GIVEN** a Frozen enemy with Burn ticking, **WHEN** a Burn tick lands, **THEN** the Freeze does not end.
- **GIVEN** a TrashMelee frozen by Frost Nova, **WHEN** the player presses `4` (Meteor) on it at the first frame Frost Nova allows (end of its Exit), **THEN** the Meteor impact lands while the target is still Frozen (expected margin ≈ 0.45 s), and the impact deals +25% (Shatter).
- **GIVEN** a Frozen enemy, **WHEN** a Fireball hits it, **THEN** the impact deals +25% and the Freeze ends at once.

**Meteor**
- **GIVEN** an enemy at the cursor, **WHEN** Meteor is cast, **THEN** a red circle fills for 0.9 s, then the enemy takes `110 + MagicDamage`, is stunned 1.0 s, and takes 6 fire ticks while inside 2.5 u.

**Balance (simulation acceptance — not a Play Mode test)**
- **GIVEN** the rotation spec in Formulas › Balance results (start at full Mana; priority Ultimate → Secondary → Utility → Primary on cooldown when affordable; basic attacks fill the rest; hit weights Hawk Eye ×1.15 full charge, Trap 0.7, Volley 3 of 5, Rain 0.65, Meteor fire 0.8) and the fixed Paladin baseline of **61.5 DPS at L1 and 83.1 DPS at L10** (mean of PaladinBasic and PaladinRef), **WHEN** the rotation is simulated for 60 s against Defense 0 at L1 and at L10-specced, **THEN** Archer and Mage DPS are each within ±15% of the baseline at both levels.
- **GIVEN** a Play Mode smoke on the TrainingDummy, **WHEN** each ability is cast once, **THEN** the logged damage of each hit matches its formula in this document within ±1 (checked per ability above); the 60 s DPS band is verified by the simulation, not by hand.

---

## Open Questions

| # | Question | Why it matters | Owner | Target |
|---|---|---|---|---|
| Q1 | Keep flat Defense subtraction, add a 20% damage floor, or adopt multiplicative armour (`raw × 100 / (100 + Def)`)? | Decides whether multi-hit kits can damage Tank/Boss at all (outlier 1). Changes Paladin too. **✅ Decided 2026-11-02 (owner): 20% floor** (smallest change, keeps Defense meaningful). Needs a code change + Paladin re-check; not implemented. | Kay | Before Phase 2 |
| Q2 | Mage has no weapon. Confirm "Fireball is the basic attack (free)", or give the Mage a staff weapon? | If a staff is added, Fireball must cost Mana again and the Mage numbers drop ~15%. **✅ Decided 2026-11-02 (owner): Fireball = free basic attack.** | Kay | Before Phase 2 |
| Q3 | `Piercing Shot` (animated, unbound) vs `Hawk Eye` (bound to Primary): merge (Hawk Eye full charge already pierces), swap names, or keep Piercing Shot for a future upgrade? | Avoids two shells for one fantasy. **✅ Decided 2026-11-02 (owner): merge — Hawk Eye full charge is the piercing shot; Hawk Eye uses the Piercing Shot clips.** | Kay | Before Phase 2 |
| Q4 | Archer basic arrow ignores `PhysicalDamage` and crit today (`RangeWeapon.OnHit`). Intended? | The whole Archer balance assumes it; if arrows start scaling, Archer L10 rises ~+35%. | Kay | Sprint 21 |
| Q5 | Should abilities ever crit (e.g. Hawk Eye full charge)? Crit adds a flat +158, which would double most ability hits. | Burst ceiling. **✅ Decided 2026-11-02 (owner): no ability crit** (Core Rule 7). | Kay | Before Phase 2 |
| Q6 | Status effects (slow/root/freeze/stun/knockback/burn) need their own GDD or a section in `character-system.md`. Which? | Six effects are specified here provisionally. | Kay | Sprint 22 |
| Q7 | Ability animation clips are 2.3–5.0 s; this design needs 0.4–0.75 s. Re-time clips or use a playback multiplier? | Without it every ability feels 5× too slow. | Kay | Phase 2 |
| Q8 | Class stat profiles: `ArcherStats` / `MageStats` are identical to `BaseStats` (all primaries 10). Should the Archer start DEX-leaning and the Mage INT-leaning? | Today class identity comes only from the ability kit. | Kay | Sprint 22 |
