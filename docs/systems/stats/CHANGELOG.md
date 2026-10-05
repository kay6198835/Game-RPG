# Stats — Changelog

Newest first. Entries before 2026-10-05 reconstructed from `CLAUDE.md` history, `docs/CHANGELOG-DOCS.md` and `git log`.

## 2026-09-25 — Modifier delta
- **Commit:** `b0b13379`
- **Changed:** `Utility.ModifierStatsCalculate`
- **From → To:** returned the modifier total → returns `addedValue - baseValue` (the delta)
- **Why:** HP 100 + `PercentAdd 0.10` healed 110 instead of 10.
- **Bugs:** closes BUG-074.

## 2026-09-23 / 09-24 — IStatService split out
- **Commits:** `2aa225e4`, `b489f94f`, `8c3c350a`
- **Changed:** `IStatService` extracted from `IPlayerStatService`; `StatHandlerBase<T>` shared by player and enemy
- **From → To:** player-only stat façade → `IStatService` for any character; `IPlayerStatService : IStatService`
- **Why:** ADR-0005 — one stat contract for player and enemy.
- **Bugs:** none.

## 2026-09-22 — BUG-063 accepted
- **Commit:** none (owner decision)
- **Changed:** status only
- **From → To:** "one-line fix carried 29+ cycles" → deliberately deferred to demo prep
- **Why:** Inspector view of live modifiers is useful during development.
- **Bugs:** BUG-063 ACCEPTED.

## 2026-08-28 … 09-03 — StatsSO replaced by BaseStatsSO
- **Commits:** `b0512f4`, `1c0742e`
- **Changed:** `StatsSO.cs` deleted; `BaseStatsSO` + `EnemyStatSO` + `StatPointAllocator` added; folder moved to `System/StatSystem/`
- **From → To:** `StatsSO` (player) + `EntityStatsSO` (enemy, deleted for NEW-2) → `BaseStatsSO` base with `EnemyStatSO` subclass, same API surface
- **Why:** Sprint 12 stat refactor — one profile type for both sides.
- **Bugs:** closes NEW-2; NEW-4 regresses as BUG-063.
