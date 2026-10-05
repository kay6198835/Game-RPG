# Items

> **Status:** live · **Last verified:** 2026-10-05, HEAD `93ba6d8e` (no code change since 2026-09-24) · **No GDD, no ADR**
> History: [CHANGELOG.md](CHANGELOG.md)

## Purpose

Enemy drops rolled from a weighted table; picked up by interaction; each item applies a list of
effects to the character that picked it up.

## Code — `Assets/Script/System/Item/` (9 files)

| File | Contents |
|------|----------|
| `ItemSO.cs` | SO "Item SO/Item": `ItemSprite`, `ItemName`, `StyleItem`, `Effects` (list of `ItemEffectDefinition`), `Description` |
| `DepotItem.cs` | SO "Item SO/Item Depot": weighted drop table by `RarityTierItem`; `TryRollItem(out ItemSO)`, `GetRandomItemByRarity()`; declares `DepotItemSlot`, `DropRateItemSlot`, `RarityTierItem` |
| `ItemController.cs` | `: InteractiveObjects`. `SetDataItem(ItemSO)`; `Interact(interactor)` → `GetComponentInParent<ICharacter>()` → `effect.Apply(character)` for each effect → `Emit(ON_COLLECT_ITEM, gameObject)` |
| `ItemSpawner.cs` | `[Inject] Construct(IObjecPoolService)`. `ON_ENEMY_DEATH` → `DropItem()` (roll, pool-spawn, DOTween scale-in); `ON_COLLECT_ITEM` → release to pool |
| `PrefabRandomItem.cs` | Random item prefab helper |
| `ItemEffectDefinition/` | `ItemEffectDefinition` (abstract SO, `Apply(ICharacter)`), `RecoveryEffectDefinition`, `StatModifierEffectDefinition`, `CurrencyEffectDefinition` |
| `Assets/Editor/DepotItemEditor.cs` | Custom drop-table Inspector |
| Assets | `Assets/SO/Item/` (PlatiumOre 0-5, Depot Item 1, Comsuable/, Effect/) |

## Flow

```
EntityDeathState → Emit(ON_ENEMY_DEATH, enemy GameObject)
  → ItemSpawner.DropItem: depotItem.TryRollItem → Pool.Spawn(droppedItemPrefab) → SetDataItem
Player Interact (G) → PlayerIntertorState / PlayerResourceReceiverState → ItemController.Interact
  → effects Apply(ICharacter) → Emit(ON_COLLECT_ITEM) → ItemSpawner releases the object
```

Effects reach the character's components from the `ICharacter` root
(`GetComponentInChildren<IVitalComponent>()` etc.), per ADR-0005 Amendment 1.

## Open issues

| Issue | Summary |
|-------|---------|
| — | `ItemSpawner.CheckRate()` always returns `true` (stub) |
| — | `ItemController.effects` is an `IReadOnlyList` marked `[SerializeField]` — Unity cannot serialize it (runtime-only, harmless) |
| — | `ON_DROP_ITEM` declared, never used |
| BUG-052 | No GDD / ADR |
