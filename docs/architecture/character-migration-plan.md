# Character Architecture — Migration Plan

Implements [ADR-0005](adr-0005-unified-character-contract.md) on branch `demo-architeture-1`.

> **Status:** all steps are written. Step 1 (`2aa225e`), step 2 (`0b3d129`) and step 3 (`b489f94`)
> are committed separately. Steps 4-10 were written in the same working tree. **Nothing has been
> compiled in the Unity Editor yet** (no Editor in the authoring environment, no automated tests —
> BUG-084). Run the verification column in order before merging.
>
> **Order change from the plan:** "Core hub capability" moved from step 3 to step 2, because the
> shared vital base resolves its stat service through it. The two steps are otherwise independent.

## Smoke test S (run after every step on `Assets/Scenes/Main/Test/LoadRandomMap.unity`)

1. Console has no errors or `MissingReferenceException`.
2. Player moves.
3. Hitting an enemy lowers its health bar; the enemy dies.
4. An enemy hit lowers player HP.
5. Blessed Slash and Consecrate cast and deduct Mana/HP as before.
6. Picking up a recovery item heals.
7. The stats panel opens and shows values.
8. Clearing a room opens the doors and the next room loads.

## Steps

| # | Change | Files | Serialization | Verification |
|---|---|---|---|---|
| 1 | Add `IStatService`; `IPlayerStatService : IStatService` keeps only `AddPrimaryPoint`/`GetLevelUpStatsBonus`; `EntityStatsHandler : IStatService` | `Interface/IStatService.cs` (+meta), `LifetimeScope/Interface/IPlayerStatService.cs`, `EntityStatsHandler.cs` | none | compile; S; stats panel full |
| 2 | `ICore.TryGetCapability<T>` + `ICore.Character`, implemented in `CoreBase` | `ICore.cs`, `CoreBase.cs` | none | compile; S |
| 3 | `StatHandlerBase<TCore>` (template `ResolveProfile`) and `VitalStatsBase<TCore>`; `EntityVitalStats` implements `IVitalComponent` (`DebuffForDuration` → `BuffDebuffForDuration`, 0 external callers); `IVitalComponent.Reborn()` | `Character/Base/StatHandlerBase.cs`, `VitalStatsBase.cs` (+metas); `StatHandler.cs`, `EntityStatsHandler.cs`, `VitalComponent.cs`, `EntityVitalStats.cs`, `Interface/IVitalComponent.cs` | `statsSO` keeps its name | compile; `PlayerTest.prefab` → StatHandler still shows its field; S |
| 4 | Fill `ICharacter`/`ICharacter<TCore>`; add `CharacterBase<TCore>`; `Player`/`Entity` re-parented, their own `core` field removed | `ICharacter.cs`, `Character/Base/CharacterBase.cs` (+meta), `Player.cs`, `Entity.cs` | ⚠️ `core` moves to the base **under the same name** — no data loss; `Awake` reassigns it anyway | open `PlayerTest`, `EnemyPrefab`, `Bat` prefabs: no "Missing script"; S |
| 5 | `CharacterLookup`; one receiver per character: `ResourceReceiver` drops `INegativeReceiver`; stray player `NegativeReciver` removed from `SO/Database/EnemyPrefab.prefab` | `Character/Base/CharacterLookup.cs` (+meta), `ResourceReceiver.cs`, **prefab** `SO/Database/EnemyPrefab.prefab` | ⚠️ one component removed by YAML edit (GUID-checked: the prefab is referenced by no asset or scene). **Re-wire step:** open the prefab in the Editor and confirm it loads without warnings | enemy hits still lower player HP; S |
| 6 | Items: `ItemEffectDefinition.Apply(ICharacter)` ×4; `ItemController` resolves `interactor.Core.Character` | `ItemEffectDefinitionSO.cs`, `RecoveryEffectDefinition.cs`, `StatModifierEffectDefinition.cs`, `CurrencyEffectDefinition.cs`, `ItemController.cs` | none (assets serialize fields, not method signatures) | recovery item heals; stat item raises the stat; no NRE (BUG-080) |
| 7 | Weapon equip through `holder.Core.Character.Stats` with `StatModifierGroup.Apply/Remmove` (same semantics as `ApplyTo/RemoveFrom`: both call `Add/RemoveModifiersFromSource` with the same list and source) | `Weapons/Weapon.cs` | none | equip/unequip the sword: stats panel rises/falls exactly as before |
| 8 | Abilities: `IAbilityOwner.Character`; `AbilityContext.Target` + `CasterCharacter` + `ResolveRecipient`; `IAbilityServices` = `Pool` only; `EffectRecipient` on `StatsEffectBase`/`BuffDebuffStatsForDuration`; spawn objects set `ctx.Target` via lookup; spawn effects run `SubEffects` on hit; cost checks read `CasterCharacter.Vital` | `IAbilityOwner.cs`, `AbilityContext.cs`, `AbilityDefinition.cs`, `AbilityEffectDefinition.cs`, `AbilityHolder.cs`, `StatEffectBase.cs`, `RecoveryReduction*.cs`, `BuffDebuffStatsForDuration.cs`, `SpawnEffectBase.cs`, `SpawnProjectileEffect.cs`, `SpawnSummonEffect.cs`, `SpawnProjectileBase.cs`, `LightningController.cs` | new `recipient` field defaults to `Caster` (0) → Paladin assets unchanged; every existing `SubEffects` list is empty (checked) | the four Paladin abilities behave as before. **Acceptance test:** create a `RecoveryReductionStatsEffect` asset (Reduction, HP, recipient = Target), add it to Blessed Slash's projectile effect `SubEffects`, hit an enemy → enemy HP drops by the extra amount |
| 9 | Per-instance enemy stats: `EntityStatsHandler` clones `EntityData.StatsSO`, destroys the clone `OnDestroy`; `EntityVitalStats.Reborn()` clears runtime modifiers first; `BaseStatsSO.ClearRuntimeModifiers()` | `EntityStatsHandler.cs`, `EntityVitalStats.cs`, `BaseStatsSO.cs` | none (runtime only) | debuff one bat → other bats unchanged; after Play Mode `git status` shows no change under `Assets/SO/Stat/Enemy/` |
| 10 | Cleanup: `IResourceReceiver` deleted; `ResourceReceiver` reduced to an `Interact` subclass (class kept for the prefab GUID) | `Interface/IResourceReceiver.cs` (+meta, deleted), `ResourceReceiver.cs` | none | compile; S |

## Rollback

Each step is revertable on its own. Steps 4 and 5 are the only ones that touch serialized data; after
reverting either, reopen the listed prefabs and confirm no missing script.

## Follow-ups not in this migration

- Update `CLAUDE.md` and `.claude/rules/{gameplay,weapon-skill,ai,ui}-code.md` once the ADR is accepted
  (they still describe `Services.Vital`, `IResourceReceiver` and `Services.NegativeReceiver`).
- Move `bullet.cs` / `Projectile.cs` / `Spell.cs` to `CharacterLookup`.
- Route `EntityEffectStats` (Abilities v1) to the per-instance clone.
- Fix BUG-066/070 once at `VitalStatsBase` and BUG-071 there.
