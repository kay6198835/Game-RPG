# Items — Changelog

Newest first. Entries reconstructed from `CLAUDE.md` history, `docs/CHANGELOG-DOCS.md` and `git log`.

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
