---
description: Testing standards for the project — Unity PlayMode/EditMode tests, manual QA evidence
globs: ["tests/**/*", "production/qa/**/*.md"]
---

# Testing Standards

## Test Evidence by Story Type

| Story Type | Required Evidence | Gate |
|---|---|---|
| Logic (damage formula, state machine) | Unity EditMode unit test — must pass | BLOCKING |
| Integration (multi-system) | PlayMode test OR documented playtest session | BLOCKING |
| Visual/Feel (animation, VFX, juice) | Screenshot + lead sign-off | ADVISORY |
| UI (menus, HUD, health bar) | Manual walkthrough doc | ADVISORY |
| Config/Data (SO tuning) | Smoke check — enter Play, verify behaviour | ADVISORY |

## Unity Test Naming
- File: `[System]Tests.cs` in `tests/EditMode/` or `tests/PlayMode/`

> ⚠️ **Those two paths do not work, and no test can be written to them today (BUG-084, verified
> 2026-09-22).** `tests/` is a *sibling* of `Assets/`, and Unity compiles only what is under
> `Assets/` and `Packages/` — nothing in `tests/EditMode` or `tests/PlayMode` is ever seen by the
> compiler. Separately, `find Assets -name "*.asmdef"` returns **0**: with no assembly definition,
> nothing can reference `UnityEngine.TestRunner` / `UnityEditor.TestRunner`, so Unity Test Runner
> cannot discover a test even if one were placed under `Assets/`.
>
> The location above is therefore **not** a settled convention — it is an unvalidated assumption
> this file has carried since it was written. Resolve BUG-084 (decide the location, add a runtime
> `.asmdef` and a test `.asmdef`) and correct this line to whatever is decided, before estimating
> any story that involves writing a test.
- Method: `[MethodName]_[Scenario]_[ExpectedResult]()` e.g. `TakeDamage_BelowZero_TriggersDeathState()`

## Determinism
- No random values in tests — fix seeds or use deterministic inputs
- No `Time.time` or frame-dependent assertions — use explicit tick counts

## Isolation
- Each test sets up its own `PlayerData` SO instance — never mutate the project asset
- Destroy all GameObjects in `TearDown()` — no state leakage between tests

## What NOT to Automate
- Visual fidelity (shader output, particle systems)
- "Feel" quality (animation weight, perceived responsiveness)
- Full dungeon run — covered by playtest sessions logged in `production/qa/playtests/`
