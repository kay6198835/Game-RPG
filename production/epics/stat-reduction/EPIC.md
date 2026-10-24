# Epic: Stat Reduction (Debuff) Flow

> **Layer**: Foundation (Stat System) + Feature (Abilities v2 effects)
> **GDD**: design/gdd/stat-system.md (no debuff section yet — SD-00 adds it)
> **Architecture Module**: `Assets/Script/System/StatSystem/`, `Assets/Script/Character/Base/VitalStatsBase.cs`,
> `Assets/Script/System/Abilities/Effects/`
> **Status**: Ready — tasks defined, not started
> **Created**: 2026-10-22 from the stat-system `/code-review` and debuff-flow analysis (session)
> **Estimate**: ~2.4 days in total (see the task table)

## Overview

A debuff is anything that lowers a character's stats or resources: instant HP/Mana reduction,
damage over time (DoT), a timed stat reduction (armor break, weaken, crit down) and a slow. This
epic makes every one of those reach gameplay, for an enemy target and for the player.

## Current state (2026-10-22, HEAD `aac995d`, static analysis)

**Delivery works.** A spawned projectile or damage area sets `AbilityContext.Target`
(`SpawnProjectileBase.cs:40`, `DamageArea.cs:28`) and runs the effect's `SubEffects` through
`SpawnEffectBase.ApplyOnHitEffects()` (`:36`). A SubEffect with `recipient = Target` resolves the hit
character's `IVitalComponent`. A debuff placed in the ability's top-level `Effects` list has no
target (`context.Target == null`) and does nothing.

| Debuff kind | Effect class | Works? | Why |
|---|---|---|---|
| Instant HP/Mana reduction | `RecoveryReductionStatsEffect` (`Reduction`) | Yes | `Reduction()` lowers `currentStats`; `EntityBasicState:33` kills at HP ≤ 0. Enemy health bar is not refreshed |
| DoT | `RecoveryReductionPerTimeForDuration` (`Reduction`) | Yes | Ticks; cannot be stopped (BUG-071), unbounded stacking, health bar not refreshed |
| Defense down | `BuffDebuffStatsForDuration` | **No** | Modifier lands on the enemy's runtime clone, but `EntityNegativeReciver.cs:30` reads `GetStatValue()` = `AdjustedValue` (NOTE-20261021-1) |
| Damage / crit / attack-speed down | `BuffDebuffStatsForDuration` | **No** | Combat reads the `currentStats` snapshot taken at `Reborn()` (NOTE-20261021-2) |
| Slow | — | **No** | Enemy speed does not read `StatType.MoveSpeed`; `MovementBase.AddSpeedMultiplier()` exists but no effect calls it |
| Any debuff on the player | — | **Unsafe** | Modifiers land on the shared asset (NOTE-20261021-3); expiry strips item buffs (BUG-100); a scene reload kills the removal coroutine, so the debuff becomes permanent for the session |

**No content exercises the flow:** all seven stat-effect assets under `Assets/SO` are
`recipient: Caster` and `impactType: Recovery`.

## Tasks

Estimates are in days, on the scale used by `production/sprints/sprint-19.md`.

| ID | Task | Owner | Est. | Depends on | Acceptance criteria |
|---|---|---|---|---|---|
| SD-00 | **Design decision — debuff rules.** Decide: (a) stacking per source (refresh duration / stack / cap N); (b) how a slow is implemented — `IMovement.AddSpeedMultiplier` (recommended: already pooled-safe through `ClearImpacts()`) or reading `MoveSpeed` in `MovementBase`; (c) floor/cap per stat (e.g. Defense ≥ 0, MoveSpeed ≥ 10 % of base, CritChance 0–100 %); (d) whether percent debuffs scale the primary-derived part (`equipmentByPrimaryValue`) | owner + game-designer | 0.2 | — | Decisions written as a `Debuffs` subsection of `design/gdd/stat-system.md` (Detailed Rules + Formulas + Edge Cases), marked `[PLANNED]` |
| SD-01 | **One definition of max** (NOTE-20261021-1). `BaseStatsSO.GetStatValue()` returns `FinalValue`; add `GetAdjustedValue()` only if a caller needs the un-modified value. Re-check callers: `VitalStatsBase.Recovery`, `EntityNegativeReciver:25,30`, `RealPlayerDataProvider:153-154`, `RegenComponent` | gameplay-programmer | 0.15 | — | Defense modifier changes damage taken; heal with +HP gear never lowers HP; HUD max includes gear |
| SD-02 | **Vitals hold resources only** (NOTE-20261021-2). `currentStats` keeps HP and Mana; `GetCurrentStatValue()` reads every other stat live from `IStatService`. On `OnStatChanged` for HP/Mana, clamp current to the new max | gameplay-programmer | 0.3 | SD-01 | A timed PhysicalDamage/CritChance debuff changes weapon damage while active and stops at expiry; no `KeyNotFoundException` |
| SD-03 | **One modifier source per application** (BUG-100, = sprint-19 **S19-09**). Timed buff/debuff uses a per-application token; permanent item buffs use the item/effect instance. Adjust `IVitalComponent.ApplyBuffDebuff` / `BuffDebuffForDuration` signatures as needed | gameplay-programmer | 0.2 | — | Item buff survives a debuff expiry; two overlapping debuffs each last their full duration |
| SD-04 | **Debuff lifecycle** (BUG-071 residual, = **S19-10**). Keep coroutine handles per token; cancel timed modifiers and DoTs on death, disable and `Reborn()`; apply the SD-00 stacking rule | gameplay-programmer | 0.2 | SD-03, SD-00 | DoT and timed debuffs stop on death; a re-spawned pooled enemy starts clean; stacking follows the GDD |
| SD-05 | **Stat floors and caps** from SD-00 (c), applied in `Stat.CalculateFinalValue()` or a per-`StatType` clamp table in `BaseStatsSO` | gameplay-programmer | 0.15 | SD-00 | A −200 % Defense debuff yields the floor, not a negative value; CritChance never exceeds its cap |
| SD-06 | **Slow effect.** New `AbilityEffectDefinition` (e.g. `SpeedModifierForDuration`) calling `IMovement.AddSpeedMultiplier(token, x)` and removing it at expiry, resolved with `TryGetRecipientComponent<IMovement>(Target)`; or the SD-00 alternative | gameplay-programmer | 0.2 | SD-00, SD-03 | A slowed enemy moves at `x` × speed for the duration, then returns to normal; slow is cleared on pool re-spawn |
| SD-07 | **Health bar on any HP change.** `EntityUIController` subscribes to `IVitalComponent.CurrentStatsChanged` instead of relying on `EntityNegativeReciver.OnDamaged` | gameplay / ui-programmer | 0.1 | SD-02 | Enemy bar drops on instant reduction and every DoT tick |
| SD-08 | **Player runtime stat clone** (NOTE-20261021-3). `StatHandler.ResolveProfile()` instantiates the profile and assigns it to `statsSO` (inspectable as "… (Runtime)"); rebuild on `SetCharacterData()`; `ResignChangeStat` uses `StatsSO`; clean the leaked modifiers from `Assets/SO/Stat/Player/{Paladin,Archer,Mage,Base}Stats.asset` | gameplay-programmer + owner (asset clean-up) | 0.25 | — | After Play Mode + Save Project, `git status` shows no `Assets/SO/Stat` change; a debuff on the player does not survive death / New Game |
| SD-09 | **Debuff content.** Author at least: Armor Break (Defense −30 % PercentAdd, 3 s), Weaken (PhysicalDamage −20 %, 3 s), Slow (SD-06, ×0.6, 2 s), Poison (DoT 5 × 1 s). Attach as `SubEffects` with `recipient: Target` on one Paladin ability and one enemy ability | owner (Editor) + game-designer | 0.2 | SD-02…SD-07 | Each asset produces its effect in Play Mode on an enemy; the enemy ability one on the player |
| SD-10 | **Verification.** Play Mode checklist covering every row of the acceptance criteria above, recorded in the week's playtest sheet. EditMode tests for `Stat` floors/stacking once BUG-084 is resolved | qa-tester / owner | 0.15 | SD-09 | Checklist all PASS, or failures filed as inbox notes |
| SD-11 | **Docs.** Mark the GDD `Debuffs` subsection `[IMPLEMENTED]`; update `docs/systems/stats/README.md` + `CHANGELOG.md` and `docs/systems/abilities/README.md` (on-hit SubEffects + `recipient: Target`); ADR-0001 dictionary type | PM (`/doc-sync`) | 0.1 | SD-10 | Docs describe the shipped behaviour |

### Side item (same path, not a debuff)

| ID | Task | Owner | Est. | Acceptance criteria |
|---|---|---|---|---|
| SD-12 | `Assets/SO/Skill/Paladin/Ability/Avatar of Light/Effect/Gain For Time Effect.asset` has `duration: 0`, so its +15 HP buff is removed about one frame after it is applied. Set the intended duration | owner (Editor) | 0.02 | Buff lasts the authored duration |

## Stories

Created 2026-10-22 by `/create-stories stat-reduction` (review mode `lean`, so QL-STORY-READY was
skipped; test cases come from `/qa-plan`). Nine stories are **Blocked** only because ADR-0001 and
ADR-0005 are still `Proposed` — the code they describe is already in the project; accept them with
`/architecture-decision accept`. No TR-ID exists for the stat system yet (`TR-stat-???`).

| # | Story | Task | Type | Status | ADR |
|---|-------|------|------|--------|-----|
| 001 | [Debuff rules design](story-001-debuff-rules-design.md) | SD-00 | Config/Data | Ready | N/A |
| 002 | [One definition of max](story-002-one-max-definition.md) | SD-01 | Logic | Blocked | ADR-0001 |
| 003 | [Vitals hold resources only](story-003-vitals-resources-only.md) | SD-02 | Logic | Blocked | ADR-0005, ADR-0001 |
| 004 | [One modifier source per application](story-004-modifier-source-per-application.md) | SD-03 | Logic | Blocked | ADR-0005 |
| 005 | [Debuff lifecycle](story-005-debuff-lifecycle.md) | SD-04 | Logic | Blocked | ADR-0005 |
| 006 | [Stat floors and caps](story-006-stat-floors-caps.md) | SD-05 | Logic | Blocked | ADR-0001 |
| 007 | [Slow effect](story-007-slow-effect.md) | SD-06 | Integration | Blocked | ADR-0005 |
| 008 | [Health bar on any HP change](story-008-health-bar-any-hp-change.md) | SD-07 | UI | Blocked | ADR-0005 |
| 009 | [Player runtime stat clone](story-009-player-runtime-stat-clone.md) | SD-08 | Integration | Blocked | ADR-0005, ADR-0001 |
| 010 | [Debuff content assets](story-010-debuff-content-assets.md) | SD-09 | Config/Data | Ready | N/A |
| 011 | [Verification and docs](story-011-debuff-verification-and-docs.md) | SD-10 + SD-11 | Integration | Blocked | ADR-0005 |
| 012 | [Avatar of Light buff duration](story-012-avatar-of-light-buff-duration.md) | SD-12 | Config/Data | Ready | N/A |

## Order

1. SD-00 (design) in parallel with SD-01, SD-03, SD-08 (no dependency between them).
2. SD-02 → SD-07; SD-04, SD-05, SD-06 once SD-00 is decided.
3. SD-09 content → SD-10 verification → SD-11 docs.

SD-03 and SD-04 duplicate sprint-19 tasks S19-09 and S19-10; finishing those closes them here. The
remaining tasks are not in any sprint yet — `/weekly-kickoff` can pull them into sprint-20.

## Related

NOTE-20261021-1, NOTE-20261021-2, NOTE-20261021-3, NOTE-20261021-4 (`production/qa/bug-inbox.md`),
NOTE-20261020-3 (`RegenComponent`), BUG-063, BUG-071, BUG-084, BUG-100, ADR-0001, ADR-0005.
