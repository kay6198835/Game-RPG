---
status: reverse-documented
source: Assets/Script/System/Item/
date: 2026-10-20
verified-by: Kiet (design direction decided 2026-10-20; code facts read from branch origin/feature/ui-flow-maingameplay)
---

# Item System Design

> 📜 Change log: [changelog/item-system.CHANGELOG.md](changelog/item-system.CHANGELOG.md)
> Technical page (code map, open issues): [docs/systems/items/README.md](../../docs/systems/items/README.md)

> **What this document is.** The item system shipped in `853fe39` / `9f1258c` (2026-09) with no design
> document (BUG-052). Sections 1–8 describe the system **as built**. Section 9, *Design Direction*,
> records the owner's decisions of 2026-10-20 that change where the system is going; where 1–8 and 9
> disagree, 1–8 is what the code does today and 9 is what it must become.

## Summary

Enemies drop items rolled from a weighted rarity table. The player picks an item up with **Q** and its
effects apply at once to the character who picked it up. There is no inventory, no currency storage and
no shop backing yet. Progression will be paid in **coin** held by a per-run **Wallet** (section 9).

## 1. Overview [IMPLEMENTED]

| In scope (built) | Planned (section 9) | Out of scope |
|---|---|---|
| Drop roll on enemy death | Wallet component + coin | Inventory / bag |
| Pickup by interaction | Coin-paid primary-stat upgrades | Equipment slots |
| Effect application to the picker | Info-only record of picked items / plugins in a UI | Level / EXP system |
| Pooling of dropped items | Shop pricing | Cross-run persistence of coin |

## 2. Player Fantasy [PLANNED]

*Proposed, not confirmed by the owner.* Each kill can pay out something small and immediate; coin earned
during a run is the one resource the player spends to grow, so every room is a trade-off between risk and
the coin it may yield. Losing the run loses the coin — the roguelike stake.

## 3. Detailed Design [IMPLEMENTED]

### 3.1 Core mechanics

**Drop.**
```
EntityDeathState → Emit(ON_ENEMY_DEATH, enemy GameObject)
  → ItemSpawner.DropItem()
      depotItem.TryRollItem(out ItemSO)        miss → nothing spawns
      Pool.Spawn(droppedItemPrefab, enemy position)
      ItemController.SetDataItem(item)          sprite, name, effects
      DOTween scale-in (0.25 s, OutBack)
```
One `DepotItem` is assigned on the scene's `ItemSpawner`; every enemy uses it. **Kept as is by owner
decision (2026-10-20).** `EnemySO.depotItem` exists but is not read (TD-030).

**Pickup.**
```
Q → PlayerInputHandler.IsResourceReceiver
  → PlayerBasicState: ResourceReceiver.FindInteraction()
      OverlapCircleNonAlloc(radius 0.5, layer "Interactable"), nearest collider to the mouse
  → PlayerResourceReceiverState → on the animation's OnActivate → ResourceReceiver.Intertion()
  → ItemController.Interact(interactor)
```
The NPC key **G** (`Interactor`) uses the same `Interact.Intertion()` and also picks up an item when the
item is the nearest interactable.

**Apply.**
```
ItemController.Interact
  → target = interactor.GetComponentInParent<ICharacter>()
  → foreach effect in item.Effects: effect.Apply(target)
  → Emit(ON_COLLECT_ITEM, item GameObject) → ItemSpawner releases it to the pool
```
Effects never receive a concrete `Player`; each one resolves the interface it needs from
`target.Transform` (ADR-0005 Amendment 1), so the same item works on any character.

### 3.2 State and data

| Type | Kind | Fields |
|---|---|---|
| `ItemSO` | SO "Item SO/Item" | `ItemSprite`, `ItemName`, `Effects` (`List<ItemEffectDefinitionSO>`), `Description`, `Id` (GUID assigned once in `OnValidate`) |
| `DepotItem` | SO "Item SO/Item Depot" | `listItems` (`ItemSO` + `RarityTierItem`), `listTierValidItems` (`TierRate` % per tier), read-only `rateDepotDropItem` |
| `RarityTierItem` | enum | `Common`, `Rare`, `Epic`, `Legendary` — numeric values are labels kept for serialization |
| `ItemEffectDefinitionSO` | abstract SO | `Apply(ICharacter)` |

**Effect types.**

| Effect | Menu | Does | Duration |
|---|---|---|---|
| `RecoveryEffectDefinition` | Item SO/Effect/Recovery | `IVitalComponent.Recovery(HP, amount)` | instant |
| `StatModifierEffectDefinition` | Item SO/Effect/StatModifier | `IVitalComponent.ApplyBuffDebuff(group)` | permanent for the run (see BUG-100) |
| `CurrencyEffectDefinition` | Item SO/Effect/Currency | **nothing** — body is a commented `Wallet.Add(amount)` | — |

No runtime state is held by the system itself: a dropped item lives only as a pooled
`ItemController` until it is picked up.

### 3.3 Integration points

| System | Contract |
|---|---|
| Event bus | consumes `ON_ENEMY_DEATH`, emits/consumes `ON_COLLECT_ITEM`; `ON_DROP_ITEM` is declared and unused |
| Object pool | `IObjectPoolService` injected into `ItemSpawner` (VContainer, ADR-0004) |
| Stats / vitals | effects call `IVitalComponent` |
| Player input + state machine | Q → `PlayerResourceReceiverState`; G → `PlayerIntertorState` |
| Character contract | `ICharacter` root (ADR-0005) |
| UIFlow | `ItemData` / `IInventoryProvider` exist in the UI; **not connected** to items (`RealInventoryProvider` is a stub) |

## 4. Formulas [IMPLEMENTED]

**Drop roll** (`DepotItem.TryRollItem`):

```
rollable  = tiers with TierRate > 0 that have ≥ 1 item in listItems
TotalDropRate = Σ TierRate(rollable)
roll ∈ [0, 100)
walk rollable in list order, acc += TierRate; first tier with roll < acc wins
roll ≥ TotalDropRate → no drop
inside the winning tier: each item equally likely (reservoir sampling, no allocation)
```

*Worked example.* Common 50, Epic 10, Rare 5 → `TotalDropRate = 65`. Roll 0–49.99 → Common,
50–59.99 → Epic, 60–64.99 → Rare, 65–99.99 → no drop. A tier declared at 20 with no item is dropped
from the walk and from the total.

**Recovery:** `HP_current += amount`, clamped by the vitals component.

## 5. Edge Cases [IMPLEMENTED]

| Case | Behaviour today |
|---|---|
| Tier has a rate but no item | Excluded from roll and total; Editor warning in `OnValidate` |
| Declared rates sum > 100 | Tail tiers can never be hit; Editor warning |
| Roll misses | Nothing spawns (guard against `SetDataItem(null)`) |
| `ItemSpawner.CheckRate()` | Stub, always `true` |
| Timed buff expires after a permanent stat item | Both use source `this`; expiry strips the item's modifiers too (**BUG-100**) |
| Currency item picked up | No effect (Wallet does not exist) |
| Two interactables in range | Nearest to the **mouse**, not to the player, is chosen |

## 6. Dependencies [IMPLEMENTED]

- **Upstream:** enemy death state, object pool, DI container, player input / state machine, `ICharacter`.
- **Downstream:** stat / vitals system; (planned) Wallet, coin-paid upgrades, shop, HUD list of
  picked items / plugins.

## 7. Tuning Knobs [IMPLEMENTED]

| Knob | Where | Current |
|---|---|---|
| `TierRate` per tier | `DepotItem` asset | `Depot Item 1.asset` |
| Items per tier | `DepotItem.listItems` | — |
| Recovery `amount` | effect asset | — |
| Stat modifier values | `StatModifierEffectDefinition.statModifierGroup` | — |
| Pickup radius | `ResourceReceiver.intertionPointRadius` (prefab) | 0.5 |
| Drop scale-in | `ItemSpawner.DropItem` | 0.25 s (hardcoded — should move to data) |

## 8. Acceptance Criteria [IMPLEMENTED]

1. **Given** a `DepotItem` with `TotalDropRate = 65`, **when** 1 000 enemies die, **then** 600–700 items drop.
2. **Given** a roll in the no-drop band, **when** an enemy dies, **then** nothing spawns and no error is logged.
3. **Given** an item on the ground within 0.5 of the player, **when** Q is pressed, **then** every effect
   applies exactly once and the object returns to the pool.
4. **Given** a Recovery item of `amount = 20` and the player below max HP by ≥ 20, **when** picked up,
   **then** current HP rises by exactly 20.
5. **Given** a tier with a rate and no item, **when** the asset is validated, **then** a warning names the tier.

## 9. Design Direction (owner decisions, 2026-10-20) [PLANNED]

These are decisions, not yet code. Each one changes sections 1–8 when implemented.

1. **No inventory.** Items are used on pickup. There is no bag, no equipment slot, nothing to sell.
   A UI will still **list what the player has** — skills, picked items and "plugins" — as
   information. Its shape is not decided.
2. **Stat pickups become information only.** Picking up a stat-related item will no longer change stats
   as `StatModifierEffectDefinition` does today; the item is recorded for display. Details not decided;
   current code is unchanged until they are.
3. **Wallet.** The player gets a `Wallet` component holding **coin**. Coin is the only progression
   currency.
4. **No level system, no EXP.** Every **primary stat** (STR / DEX / INT / VIT / LUK) is raised by
   paying coin. References: Elden Ring (runes spent on attributes), *Omniscient Reader's Viewpoint*
   (coins spent on stats).
5. **Coin is per run.** Kept until the run ends (death or victory), reset on every new game, never
   saved — roguelike rules.
6. **Enemy drops stay as they are** (one global `DepotItem`).
7. **`CurrencyEffectDefinition` stays as it is** (a no-op) for now.
8. **Two rarity enums stay separate.** `RarityTierItem` (drop table) and UIFlow `ItemRarity` (display)
   each have a use today; unifying them is TD-054.
9. **Removed (2026-10-20):** `ItemSO.StyleItem` / `Style` (read by nothing) and `PrefabRandomItem.cs`
   (fully commented out).

## Open Questions and Follow-Up Work

### Questions needing owner decision

| # | Question |
|---|---|
| Q1 | Wallet API and owner: player core component? events for the HUD (`CoinChanged`)? |
| Q2 | Coin sources: drops only, room clear, enemy kill directly, all three? |
| Q3 | Upgrade cost curve per primary point (flat, linear, exponential like Elden Ring)? Where is it paid (any time, rest room, shrine NPC)? |
| Q4 | What does the shop sell if there is no inventory — instant effects, stat points, plugins? `buyPrice` on `ItemSO`? `sellPrice` has no meaning without inventory |
| Q5 | What is a "plugin"? A passive modifier distinct from an item? |
| Q6 | Role of `Merchant.prefab` / `NPC.prefab` (their scripts were never committed) |
| Q7 | Does `CurrencyEffectDefinition` become the coin pickup once the Wallet exists? |

### Flagged follow-up work

- Wallet component + per-run reset (needs the missing `GameManager` / run reset, BUG-087).
- Replace the primary-stat allocation path (`StatPointAllocator`) with a coin-paid one.
- Info-only record of picked items; HUD list.
- TD-054 (rarity enums), TD-030 (per-enemy depots, deferred), BUG-100.
- Move the 0.25 s scale-in and pickup radius into data.
- An ADR for the drop/pickup contract (BUG-052).

## Version History

| Date | Change |
|---|---|
| 2026-10-20 | Created — reverse-documented from code; owner design direction recorded (section 9) |
