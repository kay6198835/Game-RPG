# Items — Changelog

Newest first. Entries reconstructed from `CLAUDE.md` history, `docs/CHANGELOG-DOCS.md` and `git log`.

## 2026-11-07 — Item effect data restored; coin effect not authorable (doc-truth re-sync)
- **Commit:** `fbab752a` "fix(data): move values from renamed serialized keys into the new fields" (2026-11-05) — `New Stat Modifier Effect Definition.asset` and its copy; earlier `1370136c`.
- **Changed (README):** Open issues — BUG-121 marked FIXED (awaiting runtime) per the 2026-11-07 triage; BUG-123 added.
- **Why:** commit title; triage 2026-11-07.
- **Bugs:** BUG-121, BUG-123, BUG-120 (unchanged).

## 2026-10-31 — Item effect files replaced (doc-truth re-sync)
- **Commit:** `3d768b3d` "add regen system", `343a3830` "coding" (recorded by doc-sync on `origin/sprint-20` `e8baf251`)
- **Changed:** `RecoveryEffectDefinition`, `StatModifierEffectDefinition`, `CurrencyEffectDefinition` deleted; `StatModifierEffect` (abstract), `ModifierMaxStatsEffect`, `ModifierCurrentStatsEffect`, `CoinModifierEffect` added. Effects reach the character through `IEffectReceiver` / `IWallet`.
- **From → To:** README listed the three deleted files → README lists the four current ones; BUG-100 row re-pointed at `EffectReceiverBase.cs:86-91`; BUG-112, BUG-120, BUG-121 rows added.
- **Why:** not recorded in the commit.
- **Bugs:** BUG-120, BUG-121, BUG-112; inbox NOTE-20261031-10 (`CoinModifierEffect` cannot be authored).

## 2026-10-20 — Cleanup + GDD
- **Commit:** this change (branch `origin/feature/ui-flow-maingameplay`)
- **Changed:** `ItemEffectDefinition` class renamed `ItemEffectDefinitionSO` (matches its file; `.meta` GUID kept, so assets are unaffected) and its five references updated; `ItemSO.StyleItem` / `Style` removed (no reader) and the stale `<Style>k__BackingField` key stripped from `Comsuable/BaseOrds.asset` and `Test.asset`; `PrefabRandomItem.cs` deleted (fully commented out, unreferenced). README corrected: pickup key is **Q**, not G. New GDD `design/gdd/item-system.md`.
- **From → To:** undesigned system with dead fields → reverse-documented GDD with owner design direction (no inventory, Wallet, coin-paid primary stats)
- **Why:** owner decisions 2026-10-20; BUG-052.

## 2026-10-19 — Pool interface rename
- **Commit:** `cae7531` ("add asset")
- **Changed:** `ItemSpawner` field and `Construct()` parameter type `IObjecPoolService` → `IObjectPoolService`
- **Why:** project-wide interface rename (inbox NOTE-20261019-5). Not recorded until 2026-10-20.

## 2026-09-24 — Items apply to the owning character
- **Commits:** `8c3c350a`, `5007d3f6`
- **Changed:** `ItemController`, `ItemEffectDefinition` and subclasses; `ResourceReceiver` gutted; `IResourceReceiver.cs` deleted
- **From → To:** effects applied through the player's `ResourceReceiver : IResourceReceiver` → `ItemEffectDefinition.Apply(ICharacter)`, called by `ItemController.Interact()` on `GetComponentInParent<ICharacter>()`
- **Why:** ADR-0005 — items work for any character; removed a second `INegativeReceiver` implementer.
- **Bugs:** closes BUG-080, BUG-081.

## 2026-09-04 … 09-08 — Item system added
- **Commits:** `853fe39b`, `9f1258cd`
- **Changed:** `ItemSO` (renamed from `ItemOS`), `DepotItem`, `ItemController`, `ItemSpawner`, `PrefabRandomItem`, four effect SOs, `DepotItemEditor`, `PlayerResourceReceiverState`, `ON_DROP_ITEM` / `ON_COLLECT_ITEM`
- **From → To:** no item system → weighted drops on enemy death + pickup
- **Why:** run progression drops.
- **Bugs:** none at the time.
