# Stats — Changelog

Newest first. Entries before 2026-10-05 reconstructed from `CLAUDE.md` history, `docs/CHANGELOG-DOCS.md` and `git log`.

## 2026-11-09 — Stat split: data-only asset + StatSheet
- **Commit:** none yet (local working tree, owner request: no commit / push).
- **Changed:** `BaseStatsSO`, `Stat`, `DerivedStatFormula`, `StatHandlerBase`, `StatHandler`, `CharacterData`,
  `PlayerData`, `EntityData` (+ `EntityFindTarget`, `EntityBasicState`, `EntityIdleState`, `EntityMoveState`),
  `StatModifierTester` (+ editor), QA suites and static checks; new `StatSheet`, `PrimaryStatEntry`,
  `Assets/Editor/BaseStatsSOEditor.cs`, `Assets/Editor/EntityDataEditor.cs`.
- **From → To:** `BaseStatsSO` stored and calculated everything (six stored tiers per stat, dirty flags,
  lookup, DTOs, events, point allocation), and each character `Instantiate`d a clone →
  `BaseStatsSO` holds only `level`, `primaryStats` (was `stats`) and `derivedStats` (was `statFormulas`);
  a plain `StatSheet` per character computes every value in `RecalculateAll()`. All 15 profile assets under
  `Assets/SO/Stat/` were migrated in YAML by a one-off script (primary base values and formulas copied
  verbatim and verified against HEAD; derived rows, level-up / final tiers, `statUnusedBonus`, `isDevMode`
  dropped), so no `[FormerlySerializedAs]` is kept. `Test.asset` loses its STR `levelUpValue: 1` (spent
  points are runtime-only now). `CharacterData.CreateRuntimeStats()`, `BaseStatsSO.SeedAllStats()`,
  `PlayerData.ResetData()` and `DerivedStatFormula.Evaluate()` removed. `EntityData` view / timer values are
  now methods taking the enemy's `IStatService` (buffs apply) instead of fields cached by `OnValidate`.
- **Why:** god asset — runtime state leaked into committed assets, enemy assets read 0 before a recalc,
  and the calculation was spread over many methods, hard to debug.
- **Bugs:** addresses BUG-063 and the spent-points part of BUG-115 in code (statuses left for triage).
  Numbers are unchanged by design (same formula); confirm in Play Mode.

## 2026-10-31 — Player stat asset paths (doc-truth re-sync)
- **Commit:** `ada2c1a8` (rename), `e6bf8fc3` (Archer / Mage)
- **Changed:** README asset list.
- **From → To:** `Assets/SO/Stat/PlayerStats.asset` → `Assets/SO/Stat/Player/{Paladin,Archer,Mage}Stats.asset`.
- **Why:** the 2026-10-19 doc-truth pass listed this fix but was interrupted before it landed (`2433b6ee`).
- **Bugs:** BUG-113, BUG-114, BUG-115, BUG-121 (filed 2026-10-25 / 10-31) — not yet described in the README. ⚠️ README not re-verified against `StatPointSession`, `VitalModifier`, `StatModifierGroup.statsModifiers` this pass.

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
