# Items

> **Status:** live · **Last verified:** 2026-10-20, branch `origin/feature/ui-flow-maingameplay` · **GDD:** [design/gdd/item-system.md](../../../design/gdd/item-system.md) (reverse-documented 2026-10-20) · **No ADR**
> History: [CHANGELOG.md](CHANGELOG.md)

## Purpose

Enemy drops rolled from a weighted table; picked up by interaction; each item applies a list of
effects to the character that picked it up. There is **no inventory**: an item is used the moment it
is picked up (owner decision 2026-10-20 — see the GDD, *Design Direction*).

## Code — `Assets/Script/System/Item/` (8 files)

| File | Contents |
|------|----------|
| `ItemSO.cs` | SO "Item SO/Item": `ItemSprite`, `ItemName`, `Effects` (list of `ItemEffectDefinitionSO`), `Description`, auto-assigned GUID `Id`. *`StyleItem` / `Style` removed 2026-10-20 — it was read by nothing* |
| `DepotItem.cs` | SO "Item SO/Item Depot": weighted drop table by `RarityTierItem`; `TryRollItem(out ItemSO)`, `GetRandomItemByRarity()`; declares `DepotItemSlot`, `DropRateItemSlot`, `RarityTierItem` |
| `ItemController.cs` | `: InteractiveObjects`. `SetDataItem(ItemSO)`; `Interact(interactor)` → `GetComponentInParent<ICharacter>()` → `effect.Apply(character)` for each effect → `Emit(ON_COLLECT_ITEM, gameObject)` |
| `ItemSpawner.cs` | `[Inject] Construct(IObjectPoolService)` (renamed from `IObjecPoolService` in `cae7531`). `ON_ENEMY_DEATH` → `DropItem()` (roll, pool-spawn, DOTween scale-in); `ON_COLLECT_ITEM` → release to pool |
| `ItemEffectDefinition/ItemEffectDefinitionSO.cs` | Abstract SO `ItemEffectDefinitionSO` (renamed from `ItemEffectDefinition` 2026-10-20 to match its file), `Apply(ICharacter)` |
| `ItemEffectDefinition/RecoveryEffectDefinition.cs` | "Item SO/Effect/Recovery" — `IVitalComponent.Recovery(StatType.HP, amount)` |
| `ItemEffectDefinition/StatModifierEffectDefinition.cs` | "Item SO/Effect/StatModifier" — `IVitalComponent.ApplyBuffDebuff(group)` (permanent) |
| `ItemEffectDefinition/CurrencyEffectDefinition.cs` | "Item SO/Effect/Currency" — **no-op**: body is `// player.Wallet.Add(amount);`; no Wallet exists yet |
| `Assets/Editor/DepotItemEditor.cs` | Custom drop-table Inspector |
| Assets | `Assets/SO/Item/` (PlatiumOre 0-5, Depot Item 1, Comsuable/{BaseOrds,Test}, Effect/New Recovery Effect Definition) |

*`PrefabRandomItem.cs` deleted 2026-10-20 — its body was entirely commented out and no asset referenced it.*

## Flow

```
EntityDeathState → Emit(ON_ENEMY_DEATH, enemy GameObject)
  → ItemSpawner.DropItem: depotItem.TryRollItem → Pool.Spawn(droppedItemPrefab) → SetDataItem
Player presses Q → PlayerInputHandler.IsResourceReceiver
  → PlayerBasicState: ResourceReceiver.FindInteraction() (OverlapCircleNonAlloc on "Interactable", nearest to mouse)
  → PlayerResourceReceiverState → ResourceReceiver.Intertion() → ItemController.Interact
  → effects Apply(ICharacter) → Emit(ON_COLLECT_ITEM) → ItemSpawner releases the object
```

`ResourceReceiver` (`Character/Player/CoreComponent/`) is an empty `: Interact` subclass since
ADR-0005 (2026-09-24); the class is kept because `PlayerTest.prefab` references it by GUID. The NPC
key **G** (`Interactor` → `PlayerIntertorState`) runs the same `Interact.Intertion()` and also picks up an
item if one is the nearest interactable. *Corrected 2026-10-20: this page used to say pickup was G.*

Effects reach the character's components from the `ICharacter` root
(`GetComponentInChildren<IVitalComponent>()` etc.), per ADR-0005 Amendment 1.

## Open issues

| Issue | Summary |
|-------|---------|
| — | `ItemSpawner.CheckRate()` always returns `true` (stub) |
| — | `ItemController.effects` is an `IReadOnlyList` marked `[SerializeField]` — Unity cannot serialize it (runtime-only, harmless) |
| — | `ON_DROP_ITEM` declared, never used |
| — | `CurrencyEffectDefinition.Apply()` is a no-op until the Wallet exists (GDD, *Design Direction*) |
| BUG-100 | Permanent stat items (`StatModifierEffectDefinition`) share the modifier source `this` with timed buffs; a timed buff expiring strips them |
| TD-030 | One global `DepotItem` on `ItemSpawner`; `EnemySO.depotItem` is never read. Kept on purpose for now (owner, 2026-10-20) |
| TD-054 | `RarityTierItem` (drop table) and UIFlow `ItemRarity` are two rarity enums; unify later |
| inbox | `Assets/Prefab/NPC/Merchant.prefab` and `NPC.prefab` carry scripts that were never committed (Missing Script) — see `production/qa/bug-inbox.md` |
| BUG-052 | No ADR |
