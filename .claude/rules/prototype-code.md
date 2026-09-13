---
description: Relaxed standards for throwaway prototypes in prototypes/ directory
globs: ["prototypes/**/*.cs", "prototypes/**/*.md"]
---

# Prototype Code Standards

## Relaxed Rules
Prototypes deliberately skip production standards to enable rapid iteration.
The following are ALLOWED in `prototypes/`:

- Hardcoded values (no SO required)
- Public fields on MonoBehaviours
- `GameObject.Find()` and `FindObjectOfType()`
- Singleton pattern for quick wiring
- No unit tests required
- Commented-out code blocks

## Required (even in prototypes)
- `prototypes/[feature-name]/README.md` MUST exist explaining:
  - **Hypothesis**: what question this prototype answers
  - **Result**: `[VALIDATED]` / `[INVALIDATED]` / `[IN PROGRESS]`
  - **Decision**: what was decided based on the result
- Prototype scenes must NOT be in `Assets/Scenes/Main/` — use `Assets/Scenes/Test/` or `Assets/Scenes/` root

## Promotion Rules
Prototype code that gets promoted to `Assets/Script/` MUST be rewritten to production standards:
- Values moved to ScriptableObjects
- State machine pattern applied
- Null checks added
- No `Find()` calls

## Isolation
Prototype scripts must not be referenced by production scripts in `Assets/Script/`

---

## Promotion Log

> Added 2026-09-11. Recording promotions here is now part of the Promotion Rules above — an
> undocumented promotion is how the ability framework ended up running in production while three
> separate documents still described it as "never wired".

### `skill-enhance-abilities` → `Assets/Script/System/Abilities/` (2026-09-09)

- **Commits**: `9b8d40f`, `5c7afba`
- **What moved**: all 17 `.cs` files. `prototypes/skill-enhance-abilities/Scripts/` now holds only
  two orphan `.meta` files; the directory's `README.md` is kept as the record of the experiment
- **Now in production use**: `AbilityHolder` was rewritten as `: CoreComponent<Core>, IAbilityOwner`
  and drives this framework for the player. Live SO assets exist at `Assets/SO/Skill/ShootSpirit/`
  and `Assets/SO/Skill/Conditions/`
- ⚠️ **The Promotion Rules above were not applied at the time.** The promoted code still carries
  prototype-grade traits — an unresolved TODO comment in `AbilityHolder.HandleInput()`, an unguarded
  `currentAbility.Definition` dereference in `GetAbility()`, and no ADR deciding its relationship to
  the v1 `ActivateSkill` framework it was meant to replace. That cleanup is outstanding and is
  tracked as demo-checklist item 18 in `CLAUDE.md`
