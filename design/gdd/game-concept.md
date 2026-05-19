---
status: reverse-documented
source: CLAUDE.md project overview
date: 2026-05-19
verified-by: Kiet
---

# Game Concept

> **Note**: Reverse-documented from CLAUDE.md project overview and existing codebase.
> Run `/brainstorm` to expand this into a full concept document with player journey,
> market positioning, and visual identity sections.

**Status**: In Design

---

## Concept Summary

**Genre**: Action Roguelike RPG — top-down, real-time combat
**Engine**: Unity 2022.3.62f3 LTS
**Platform**: PC (Windows)
**Scope**: Demo (single dungeon run)

A dungeon crawler where each run is a fresh challenge. The player fights through
procedurally generated rooms, clears enemies to unlock doors, collects power-ups,
and dies — then starts again stronger. Inspired by **Cult of the Lamb**: snappy
directional melee, weapon-linked skills, and a run-based power curve that rewards
learning over repetition.

---

## Core Fantasy

**What does the player do?** Enter a dungeon, fight enemies with directional melee combos
and weapon-linked skills, clear rooms, grab upgrades, go deeper.

**How should it feel?** Fast, committed, rhythmic. Every attack has weight and direction.
Skills are on a delay — you earn them, then you spend them at the right moment. Death
feels fair, not random.

---

## Core Loop

```
Enter room
  → Fight enemies (directional melee + skills)
  → Clear room (all enemies dead)
  → Pick upgrade from 3 passive talent cards
  → Advance through door to next room
  → Repeat until death
```

**30-second loop:** Enter room → engage enemies → dodge/skill to survive → kill last enemy

**3-minute loop:** Clear several rooms → accumulate upgrade path → face harder enemies

**Run loop:** Die → restart from start menu → try again with knowledge

---

## Core Mechanics

1. **Directional melee combat** — 8-directional attack animation system; 3-hit combo chain; attack direction tied to mouse position
2. **Weapon-linked skills** — each weapon carries two skill slots (E key skill + RMB ability); equipping a weapon changes your available skills
3. **Procedural dungeon** — DFS maze generation; variable room layouts loaded from saved tilemap JSON files
4. **Room-clear progression** — doors locked on room entry; unlock when all enemies are dead
5. **Per-run upgrades** — `InternalSkillSO` talent cards offered after each room clear (Strength / Dexterity / Intelligence / Charisma stat boosts)
6. **Death/restart loop** — player death resets the run; `PlayerData.Reborn()` restores stats; game returns to start menu

---

## MVP Definition (Demo Target)

**Full game life cycle in a single session:**
- Start menu → enter dungeon
- Movement (WASD) + directional melee combat (LMB)
- Two active skills (Slash on E, Block on RMB)
- Enemies that detect, chase, and attack the player
- Room-clear condition (enemies dead → doors open)
- Between-room upgrade (pick 1 of 3 stat cards)
- Player death → restart flow

**Not in demo scope:** multiplayer, full talent tree depth, ranged weapons (stretch goal), boss encounters, narrative/story.

---

## Input Bindings

| Action | Input |
|--------|-------|
| Move | WASD |
| Attack | Left Mouse Button |
| Skill | E (hold/release) |
| Block / Ability | Right Mouse Button (hold) |
| Equip/Unequip | F |
| Interact | G |
| Dash | Space |

---

## Available Enemy Rigs

Bat, Crab, Golem (3 phases), Pebble, Rat, Skull, Spiked Slime

---

## Visual Style

Top-down 2D pixel art. Dark dungeon aesthetic. Sprite rigs already imported for all
characters. No defined art bible yet — run `/art-bible` to formalize visual direction.

---

## Technical Summary

- **State machine** for all characters (player + enemies)
- **ScriptableObject-first** data model (stats, weapons, abilities, enemies)
- **EventManager** static pub/sub bus for cross-system communication
- **INegativeReceiver** interface for all damage application
- **Procedural maze** via DFS; room tilemaps saved as JSON, loaded at runtime
