# Items — Changelog

Newest first. Entries reconstructed from `CLAUDE.md` history, `docs/CHANGELOG-DOCS.md` and `git log`.

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
