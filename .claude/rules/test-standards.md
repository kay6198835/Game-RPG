---
# Testing standards for the project — Unity PlayMode/EditMode tests, manual QA evidence
paths:
  - "tests/**"
  - "Assets/Tests/**"
  - "production/qa/**/*.md"
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
- File: `[System]Tests.cs` in `Assets/Tests/EditMode/` (PlayMode tests: `Assets/Tests/PlayMode/`, not created yet)

> **Changed 2026-11-12 (branch `chore/asmdef-test-setup`, BUG-084).** Tests live under `Assets/Tests/`,
> because Unity compiles only `Assets/` and `Packages/`. Gameplay code is the `GameRPG` assembly
> (`Assets/Script/GameRPG.asmdef`; `Assets/SO/Dungeon/` joins it through `GameRPG.asmref`), and
> `Assets/Tests/EditMode/GameRPG.EditModeTests.asmdef` references it and the Test Runner assemblies.
> First test: `StatSheetTests.cs`. `tests/EditMode/` and `tests/PlayMode/` outside `Assets/` are never
> compiled — do not put test code there (`tests/playtest/` and `tests/smoke/` hold manual evidence only).
> ⚠️ Written without a Unity Editor: the assemblies were not compiled when this note was written. Until
> the owner confirms a clean compile and a green Test Runner run, treat the setup as unverified.
> A package newly used by gameplay code must be added to `references` in `GameRPG.asmdef`.
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
