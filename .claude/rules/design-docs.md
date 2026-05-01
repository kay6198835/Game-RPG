---
description: Design document standards for all GDDs in design/gdd/
globs: ["design/gdd/**/*.md"]
---

# Design Document Standards

## Required 8 Sections
Every GDD in `design/gdd/` MUST have all 8 sections. Pre-commit hook validates this.

1. **Overview** — one-paragraph summary of the system
2. **Player Fantasy** — the feeling and experience the mechanic creates
3. **Detailed Rules** — unambiguous mechanics description
4. **Formulas** — all math defined with named variables (e.g., `finalDamage = baseDamage * multiplier - blockDMG`)
5. **Edge Cases** — unusual situations explicitly handled
6. **Dependencies** — other systems this one depends on or affects
7. **Tuning Knobs** — which ScriptableObject fields control the feel
8. **Acceptance Criteria** — testable conditions that define "done"

## Scope
- One system = one GDD file: `combat-system.md`, `room-progression.md`, `upgrade-system.md`
- Do not put multiple unrelated systems in one file
- Cross-system interactions go in the GDD of the system that initiates them

## Living Documents
- When implementation deviates from the GDD, update the GDD — code is not the source of truth
- Mark sections as `[IMPLEMENTED]` or `[PLANNED]` for clarity during development
- Run `/design-review` before handing a GDD to programmers

## Formulas Format
```
finalDamage = attackDamege - target.blockDMG
healAmount  = maxHealth * 0.15
dashDistance = movementVelocities.dash * Time.deltaTime * dashDuration
```
- Use variable names that match the SO field names exactly
- Define every variable used — no implicit values
