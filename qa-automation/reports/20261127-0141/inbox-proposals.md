# Inbox proposals - QA run 20261127-0141

Sprint `origin/sprint-24` at `075711d9ad`, baseline `bcedaebb88`. Written by the Friday QA task on 2026-11-27.
Rows below are appended to the "Open notes" table of `production/qa/bug-inbox.md` on `sprint-24`.

## Verdicts on the 7 new FAILs

| Case | Verdict | Action |
|---|---|---|
| QA-ABL-01.Paladin.Primary, QA-ABL-01.Paladin.Secondary, QA-ABL-01.Archer.Ultimate | Test defect | Since `b001d78e` / `85899a52` ability hit damage is a `DamageEffect` that calls `IEffectReceiver.ApplyDamage` on the hit collider (`AbilityContext.TryGetRecipientComponent`). `QAProbeTarget` implemented only `INegativeReceiver`, so the spawned object selected it as `ctx.Target` but the damage found no receiver. Fixed: the probe implements `IEffectReceiver` too. Re-run `20261127-0218`: both Paladin cases PASS. QA-ABL-01.Archer.Ultimate also needed `IsScatteredSummon` to follow `SpawnEffectBase.Prefab` (CircleSummon -> RainArrows, `DamagesInManyPos`): it read the `summonPrefab` field removed in `8377ebaf`, so the scattered-point summon was judged on a single target. Fixed; re-run `-Deep ABL` |
| QA-STATIC-12 | Test defect (baseline) | The 2 "new findings" are info lines: Blessing and Avatar of Light now have 1 effect (a `SelfApplyEffect` wrapping the two old ones, `85899a52`) instead of 2. The 7 "no effect" findings are baseline (NOTE-20261113-8, by design). Baseline lines updated |
| QA-ITEM-10 | Existing bug | `ItemController.Interact()` has no null-element guard (NOTE-20261120-5, triaged into BUG-120, PARTIAL). Mapped in `known-issues.json` |
| QA-STATIC-03 | Real defect (2 notes) | 20 new orphaned keys, see NOTE-20261127-1 and NOTE-20261127-2 |
| QA-STATIC-09 | Real defect | 1 new finding: the recovery item effect lost its modifier, NOTE-20261127-1. The 2 `Comsuable/Test.asset` lines stay mapped to BUG-132 |

## Rows

| Note | Date | Source | Sev guess | Where | What | Evidence tier |
|---|---|---|---|---|---|---|
| NOTE-20261127-1 | 2026-11-27 | qa-auto | S4 | `Assets/Script/System/StatSystem/StatModifierGroup.cs` (`df9e98fe` removed `vitalModifiers`); `Assets/SO/Item/Effect/New Recovery Effect Definition.asset` | QA-STATIC-03 + QA-STATIC-09, 1 of 1 runs: `df9e98fe` "add status effect" removed the serialized list `StatModifierGroup.vitalModifiers` without a replacement key, so 3 item effect assets keep an orphaned `vitalModifiers` key (`New Recovery Effect Definition`, `New Stat Modifier Effect Definition`, `... - Copy`). The two stat ones hold an empty list (no data lost), but `New Recovery Effect Definition.asset` (`ModifierCurrentStatsEffect`) loses its only modifier (HP +10, `targetStatType: 100`, `value: 10`): QA-STATIC-09 now reports "no modifier (the effect changes nothing)" where the previous run read "HP +10". No item or drop table references the asset (guid `7b391542…` grep), so latent today. Relates to BUG-117 (vital modifiers). Fix direction: re-author the heal as a `statsModifiers` row on the HP vital with `impactType` Recovery (A1, `83800891`), or delete the asset. Report `qa-automation/reports/20261127-0141/report.md` | STATIC |
| NOTE-20261127-2 | 2026-11-27 | qa-auto | S4 | `Assets/SO/Skill/{Archer/Ability/Rain Arrows (GDD),Mage/Ability/Chain Lightning (GDD),Mage/Ability/Meteor (GDD)}/Effect/*Summon Effect.asset`; `Assets/Script/System/Abilities/Effects/SpawnSummonEffect.cs` (`8377ebaf` removed `summonPrefab`) | QA-STATIC-03, 1 of 1 runs: (1) the 3 unbound GDD summon effects keep an orphaned `summonPrefab` key that pointed at `Lightning.prefab`; the field is gone and the effect now spawns `Prefab` only, which these assets set to `RuneCircle.prefab` — once bound (NOTE-20261113-8 / OQ-10) they would show the rune circle without the lightning strike. (2) 15 payload effect assets (`DamageEffect`, `BuffDebuffStatsForDuration`, `RecoveryReductionStatsEffect`) keep a `SubEffects: []` key from before the Delivery / Payload split (`85899a52`); every one is empty, so no data is lost — a re-save clears them. Fix direction: re-point (1) when the GDD abilities are bound; re-save (2). Report `qa-automation/reports/20261127-0141/report.md` | STATIC |
