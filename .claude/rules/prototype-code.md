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
