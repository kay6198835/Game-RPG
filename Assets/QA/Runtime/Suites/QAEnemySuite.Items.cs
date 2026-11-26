#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;

namespace QA.Automation
{
    // Item effects on the player (added 2026-11-20, coverage review qa-automation/cases/coverage-gap-review-2026-11-20.md).
    // QA-ITEM-02..04 cover the drop and the Q pickup; these cases judge what a pickup GIVES. A drop is spawned
    // from the game's dropped-item prefab through the pool (as ItemSpawner.DropItem does) and collected through
    // ItemController.Interact(ResourceReceiver), the call PlayerResourceReceiverState makes on AnimationOnAction.
    // Synthetic items are built with CreateInstance and destroyed in Cleanup(); committed assets are only read.
    // Every case removes the stat modifiers it added, so later cases see the class's own stats.
    public partial class QAEnemySuite
    {
        private const BindingFlags AnyInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private readonly List<Object> _createdItems = new();

        private void DestroyCreatedItems()
        {
            foreach (Object o in _createdItems)
                if (o != null) Destroy(o);
            _createdItems.Clear();
        }

        #region Item helpers

        private static FieldInfo FindField(Type type, string name)
        {
            for (Type t = type; t != null; t = t.BaseType)
            {
                FieldInfo field = t.GetField(name, AnyInstance | BindingFlags.DeclaredOnly);
                if (field != null) return field;
            }
            return null;
        }

        private static bool SetField(object target, string name, object value)
        {
            FieldInfo field = FindField(target.GetType(), name);
            if (field == null) return false;
            field.SetValue(target, value);
            return true;
        }

        private ItemController DroppedItemPrefab =>
            itemSpawner != null ? FindField(typeof(ItemSpawner), "droppedItemPrefab")?.GetValue(itemSpawner) as ItemController : null;

        // Null when a field the QA relies on was renamed; the caller fails with that reason.
        private ItemSO MakeItem(string itemName, params ItemEffectDefinitionSO[] effects)
        {
            ItemSO item = ScriptableObject.CreateInstance<ItemSO>();
            item.name = itemName;
            _createdItems.Add(item);
            bool ok = SetField(item, "<ItemName>k__BackingField", itemName)
                      && SetField(item, "<effects>k__BackingField", effects.ToList());
            return ok ? item : null;
        }

        private ModifierMaxStatsEffect MakeMaxStatsEffect(StatType stat, float flat)
        {
            ModifierMaxStatsEffect effect = ScriptableObject.CreateInstance<ModifierMaxStatsEffect>();
            _createdItems.Add(effect);
            StatModifierGroup group = MakeGroup(new StatModifier(stat, flat, ModifierType.Flat, StatImpactType.Recovery), null);
            return group != null && SetField(effect, "statModifierGroup", group) ? effect : null;
        }

        private ModifierCurrentStatsEffect MakeCurrentStatsEffect(VitalStatType stat, float value)
        {
            ModifierCurrentStatsEffect effect = ScriptableObject.CreateInstance<ModifierCurrentStatsEffect>();
            _createdItems.Add(effect);
            StatModifierGroup group = MakeGroup(null, VitalMod(stat, value));
            return group != null && SetField(effect, "statModifierGroup", group) ? effect : null;
        }

        private CoinModifierEffect MakeCoinEffect(int amount)
        {
            CoinModifierEffect effect = ScriptableObject.CreateInstance<CoinModifierEffect>();
            _createdItems.Add(effect);
            return SetField(effect, "coinAmount", amount) ? effect : null;
        }

        private ItemController SpawnDrop(ItemSO data, Vector2 at)
        {
            ItemController prefab = DroppedItemPrefab;
            if (prefab == null) return null;
            GameObject drop = pool.Spawn(prefab.gameObject, at, Quaternion.identity);
            ItemController item = drop != null ? drop.GetComponent<ItemController>() : null;
            if (item != null) item.SetDataItem(data);
            return item;
        }

        private ResourceReceiver PlayerPickup => _rig.Player.GetComponentInChildren<ResourceReceiver>(true);

        // Collects the drop the way PlayerResourceReceiverState does; an exception fails the case instead of the run.
        private bool Collect(ItemController item, out string error)
        {
            error = null;
            try
            {
                return item.Interact(PlayerPickup);
            }
            catch (Exception e)
            {
                error = $"{e.GetType().Name}: {e.Message}";
                return false;
            }
        }

        private IEnumerable<StatType> AllStats => Enum.GetValues(typeof(StatType)).Cast<StatType>();

        private HashSet<object> PlayerModifierSources() =>
            new(AllStats.Select(t => _rig.Stats.GetStat(t)).Where(s => s != null).SelectMany(s => s.Modifiers).Select(m => m.Source).Where(s => s != null));

        private List<object> NewSources(HashSet<object> before) => PlayerModifierSources().Where(s => !before.Contains(s)).ToList();

        // Leaves the player with the modifiers it had before the case (item buffs are permanent by design).
        private void RemoveNewSources(HashSet<object> before)
        {
            foreach (object source in NewSources(before)) _rig.Stats.RemoveModifiersFromSource(source);
        }

        private static bool OnlyFlat(Stat stat) => stat.Modifiers.All(m => m.Type == ModifierType.Flat);

        private IWallet PlayerWallet
        {
            get
            {
                _rig.Player.Core.TryGetCapability(out IWallet wallet);
                return wallet;
            }
        }

        // Shared start of every item-effect case: a live player, the spawner on (it releases collected items).
        private IEnumerator ItemCaseReady(Action<bool> ready)
        {
            if (itemSpawner == null || pool == null)
            {
                Fail("scene not wired: ItemSpawner or ObjectPoolManager missing");
                ready(false);
                yield break;
            }
            if (DroppedItemPrefab == null)
            {
                Fail("ItemSpawner.droppedItemPrefab not found or not set (field renamed?)");
                ready(false);
                yield break;
            }
            bool playerReady = false;
            yield return EnsurePlayer(ok => playerReady = ok);
            if (!playerReady)
            {
                ready(false);
                yield break;
            }
            if (PlayerPickup == null)
            {
                Fail("player has no ResourceReceiver (pickup interactor)");
                ready(false);
                yield break;
            }
            if (_enemy != null && _enemy.activeSelf) pool.Despawn(_enemy);
            SetItemSpawner(true);
            PlacePlayer(ParkPosition);
            _rig.Vital.ReloadComponent();
            yield return Frames(1);
            ready(true);
        }

        #endregion

        #region Cases — item effects

        // QA-ITEM-05: every item the drop table can roll gives exactly its authored stat modifiers.
        private IEnumerator CaseItemEffectsExact()
        {
            bool ready = false;
            yield return ItemCaseReady(ok => ready = ok);
            if (!ready) yield break;
            if (depotItem == null)
            {
                Fail("scene not wired: DepotItem missing");
                yield break;
            }
            List<ItemSO> items = depotItem.listItems.Where(s => s != null && s.DepotItem != null).Select(s => s.DepotItem).Distinct().ToList();
            if (items.Count == 0)
            {
                Skip($"{depotItem.name} lists no item");
                yield break;
            }

            foreach (ItemSO data in items)
            {
                HashSet<object> sources = PlayerModifierSources();
                Dictionary<StatType, float> before = _rig.Stats.GetFullStat();
                float hp = _rig.Hp, mana = _rig.Mana;
                IWallet wallet = PlayerWallet;
                int coins = wallet?.GetCurrentCoin() ?? 0;

                // What the item should give, summed per target from its authored effects.
                Dictionary<StatType, float> flats = new();
                Dictionary<VitalStatType, float> vitals = new();
                int coinGain = 0, expectedModifiers = 0;
                List<string> unjudged = new();
                foreach (ItemEffectDefinitionSO effect in data.Effects)
                {
                    if (effect == null) { unjudged.Add("an empty effect slot"); continue; }
                    StatModifierGroup group = effect is StatModifierEffect ? FindField(effect.GetType(), "statModifierGroup")?.GetValue(effect) as StatModifierGroup : null;
                    switch (effect)
                    {
                        case ModifierMaxStatsEffect _ when group != null:
                            foreach (StatModifier m in group.GetStatsModifiers.Where(x => x != null))
                            {
                                expectedModifiers++;
                                // Value is a magnitude (A1): ImpactType gives the sign.
                                float signed = m.ImpactType == StatImpactType.Reduction ? -m.Value : m.Value;
                                if (m.Type == ModifierType.Flat) flats[m.TargetStatType] = (flats.TryGetValue(m.TargetStatType, out float total) ? total : 0f) + signed;
                                else unjudged.Add($"{m.TargetStatType} {m.Type} {signed:0.##}");
                            }
                            break;
                        case ModifierCurrentStatsEffect _ when group != null:
                            foreach (StatModifier vm in group.GetStatsModifiers.Where(x => x != null && Enum.IsDefined(typeof(VitalStatType), (int)x.TargetStatType)))
                                vitals[(VitalStatType)vm.TargetStatType] = (vitals.TryGetValue((VitalStatType)vm.TargetStatType, out float sum) ? sum : 0f) + (vm.ImpactType == StatImpactType.Reduction ? -vm.Value : vm.Value);
                            break;
                        case CoinModifierEffect _:
                            coinGain += (int)(FindField(typeof(CoinModifierEffect), "coinAmount")?.GetValue(effect) ?? 0);
                            break;
                        default:
                            unjudged.Add(effect.GetType().Name);
                            break;
                    }
                }
                // A heal is only visible below max.
                foreach (VitalStatType v in vitals.Keys.ToList())
                    if (vitals[v] > 0f) _rig.Vital.Reduction(v, vitals[v] * 2f);
                hp = _rig.Hp;
                mana = _rig.Mana;

                ItemController drop = SpawnDrop(data, ParkPosition + Vector2.right * 0.2f);
                if (drop == null)
                {
                    Fail($"{data.name}: the pool gave no dropped item");
                    continue;
                }
                _collected.Clear();
                bool collected = Collect(drop, out string error);
                yield return Frames(2);
                if (error != null)
                {
                    Fail($"{data.name}: ItemController.Interact threw {error}");
                    RemoveNewSources(sources);
                    if (drop.gameObject.activeSelf) pool.Despawn(drop.gameObject);
                    continue;
                }
                Check(collected && _collected.Count == 1 && _collected[0] == drop.gameObject,
                    $"{data.name}: collected, ON_COLLECT_ITEM once with the item", $"{data.name}: Interact returned {collected}, ON_COLLECT_ITEM {_collected.Count}x");
                Check(!drop.gameObject.activeSelf, $"{data.name}: released to the pool", $"{data.name}: still active after the pickup");

                Dictionary<StatType, float> after = _rig.Stats.GetFullStat();
                foreach (KeyValuePair<StatType, float> flat in flats)
                {
                    Stat stat = _rig.Stats.GetStat(flat.Key);
                    float gained = after[flat.Key] - before[flat.Key];
                    if (OnlyFlat(stat))
                        Check(Near(gained, flat.Value, 0.01f), $"{data.name}: {flat.Key} {before[flat.Key]:0.##} -> {after[flat.Key]:0.##} (+{flat.Value:0.##})",
                            $"{data.name}: {flat.Key} {before[flat.Key]:0.##} -> {after[flat.Key]:0.##}, expected +{flat.Value:0.##}");
                    else
                        Check(gained > 0f, $"{data.name}: {flat.Key} +{gained:0.##} (percent modifiers on the stat, exact value not judged)",
                            $"{data.name}: {flat.Key} changed by {gained:0.##}, expected an increase of about {flat.Value:0.##}");
                }
                int added = NewSources(sources).Count;
                if (expectedModifiers > 0)
                    Check(added == 1, $"{data.name}: its modifiers carry one source of their own", $"{data.name}: {added} new modifier source(s), expected 1 (the item cannot be removed as a unit)");
                if (vitals.TryGetValue(VitalStatType.HP, out float hpGain))
                    Check(Near(_rig.Hp, Mathf.Min(hp + hpGain, _rig.MaxHp), 0.05f), $"{data.name}: HP {hp:0.##} -> {_rig.Hp:0.##}", $"{data.name}: HP {hp:0.##} -> {_rig.Hp:0.##}, expected +{hpGain:0.##}");
                if (vitals.TryGetValue(VitalStatType.Mana, out float manaGain))
                    Check(Near(_rig.Mana, Mathf.Min(mana + manaGain, _rig.MaxMana), 0.05f), $"{data.name}: Mana {mana:0.##} -> {_rig.Mana:0.##}", $"{data.name}: Mana {mana:0.##} -> {_rig.Mana:0.##}, expected +{manaGain:0.##}");
                if (coinGain != 0)
                    Check(wallet != null && wallet.GetCurrentCoin() - coins == coinGain, $"{data.name}: +{coinGain} coin", $"{data.name}: coins {coins} -> {wallet?.GetCurrentCoin()}, expected +{coinGain}");
                List<string> unchanged = after.Where(p => !flats.ContainsKey(p.Key) && before.TryGetValue(p.Key, out float b) && !Mathf.Approximately(b, p.Value))
                    .Select(p => $"{p.Key} {before[p.Key]:0.##}->{p.Value:0.##}").ToList();
                if (unchanged.Count > 0) Note($"{data.name}: derived stats also moved: {string.Join(", ", unchanged)}");
                if (unjudged.Count > 0) Observe($"{data.name}: not judged: {string.Join(", ", unjudged)}");
                if (flats.Count == 0 && vitals.Count == 0 && coinGain == 0 && unjudged.Count == 0)
                    Fail($"{data.name}: the item has no effect to give (see QA-STATIC-09)");

                RemoveNewSources(sources);
                _rig.Vital.ReloadComponent();
            }
        }

        // QA-ITEM-06: a coin item adds exactly its coinAmount to the wallet, every pickup.
        private IEnumerator CaseItemCoin()
        {
            bool ready = false;
            yield return ItemCaseReady(ok => ready = ok);
            if (!ready) yield break;
            IWallet wallet = PlayerWallet;
            if (wallet == null)
            {
                Fail("the player has no IWallet (WalletComponent) under its Core");
                yield break;
            }
            const int amount = 7;
            CoinModifierEffect coin = MakeCoinEffect(amount);
            ItemSO item = coin != null ? MakeItem("QA Coin", coin) : null;
            if (item == null)
            {
                Fail("QA cannot build a coin item: CoinModifierEffect.coinAmount or an ItemSO backing field was renamed");
                yield break;
            }
            HashSet<object> sources = PlayerModifierSources();
            Dictionary<StatType, float> stats = _rig.Stats.GetFullStat();
            int start = wallet.GetCurrentCoin();
            for (int i = 1; i <= 2; i++)
            {
                ItemController drop = SpawnDrop(item, ParkPosition + Vector2.right * 0.2f);
                bool collected = drop != null && Collect(drop, out string error) && error == null;
                yield return Frames(1);
                Check(collected && wallet.GetCurrentCoin() == start + amount * i, $"pickup {i}: coins {start} -> {wallet.GetCurrentCoin()}",
                    $"pickup {i}: coins {start} -> {wallet.GetCurrentCoin()}, expected {start + amount * i}");
            }
            Dictionary<StatType, float> now = _rig.Stats.GetFullStat();
            Check(stats.All(p => Mathf.Approximately(now[p.Key], p.Value)) && NewSources(sources).Count == 0,
                "a coin item changes no stat", "a coin item changed the player's stats");
            Check(Near(_rig.Hp, _rig.MaxHp) && Near(_rig.Mana, _rig.MaxMana), "a coin item changes no HP / Mana", $"HP {_rig.Hp:0.#}/{_rig.MaxHp:0.#}, Mana {_rig.Mana:0.#}/{_rig.MaxMana:0.#}");
            RemoveNewSources(sources);
        }

        // QA-ITEM-07: an HP / Mana recovery item heals exactly its value and never above max.
        private IEnumerator CaseItemRecovery()
        {
            bool ready = false;
            yield return ItemCaseReady(ok => ready = ok);
            if (!ready) yield break;
            float hpValue = Mathf.Max(1f, _rig.MaxHp * 0.1f);
            float manaValue = Mathf.Max(1f, _rig.MaxMana * 0.1f);
            ModifierCurrentStatsEffect hpEffect = MakeCurrentStatsEffect(VitalStatType.HP, hpValue);
            ModifierCurrentStatsEffect manaEffect = MakeCurrentStatsEffect(VitalStatType.Mana, manaValue);
            ItemSO hpItem = hpEffect != null ? MakeItem("QA Potion HP", hpEffect) : null;
            ItemSO manaItem = manaEffect != null ? MakeItem("QA Potion Mana", manaEffect) : null;
            if (hpItem == null || manaItem == null)
            {
                Fail("QA cannot build a recovery item: StatModifierEffect.statModifierGroup or an ItemSO backing field was renamed");
                yield break;
            }

            _rig.Vital.Reduction(VitalStatType.HP, hpValue * 3f);
            _rig.Vital.Reduction(VitalStatType.Mana, manaValue * 3f);
            float hp = _rig.Hp, mana = _rig.Mana;
            Collect(SpawnDrop(hpItem, ParkPosition), out string e1);
            Collect(SpawnDrop(manaItem, ParkPosition), out string e2);
            yield return Frames(1);
            if (e1 != null || e2 != null)
            {
                Fail($"Interact threw: {e1 ?? e2}");
                yield break;
            }
            Check(Near(_rig.Hp, hp + hpValue, 0.05f), $"HP item healed {_rig.Hp - hp:0.##} of {hpValue:0.##}", $"HP {hp:0.##} -> {_rig.Hp:0.##}, expected +{hpValue:0.##}");
            Check(Near(_rig.Mana, mana + manaValue, 0.05f), $"Mana item restored {_rig.Mana - mana:0.##} of {manaValue:0.##}", $"Mana {mana:0.##} -> {_rig.Mana:0.##}, expected +{manaValue:0.##}");

            _rig.Vital.ReloadComponent();
            yield return Frames(1);
            Collect(SpawnDrop(hpItem, ParkPosition), out _);
            yield return Frames(1);
            Check(_rig.Hp <= _rig.MaxHp + 0.01f, $"at full HP the item leaves HP at max {_rig.MaxHp:0.##}", $"HP {_rig.Hp:0.##} above max {_rig.MaxHp:0.##} after a heal at full HP");
            _rig.Vital.ReloadComponent();
        }

        // QA-ITEM-08: two pickups of the same stat item stack, and each pickup can be removed on its own.
        private IEnumerator CaseItemStack()
        {
            bool ready = false;
            yield return ItemCaseReady(ok => ready = ok);
            if (!ready) yield break;
            const StatType stat = StatType.Defense;
            const float bonus = 4f;
            ModifierMaxStatsEffect effect = MakeMaxStatsEffect(stat, bonus);
            ItemSO item = effect != null ? MakeItem("QA Armour Shard", effect) : null;
            if (item == null)
            {
                Fail("QA cannot build a stat item: StatModifierEffect.statModifierGroup or an ItemSO backing field was renamed");
                yield break;
            }
            HashSet<object> sources = PlayerModifierSources();
            float before = _rig.Stats.GetStatValue(stat);
            bool flatOnly = OnlyFlat(_rig.Stats.GetStat(stat));
            Collect(SpawnDrop(item, ParkPosition), out string e1);
            Collect(SpawnDrop(item, ParkPosition + Vector2.up * 0.3f), out string e2);
            yield return Frames(1);
            if (e1 != null || e2 != null)
            {
                Fail($"Interact threw: {e1 ?? e2}");
                RemoveNewSources(sources);
                yield break;
            }
            float twice = _rig.Stats.GetStatValue(stat);
            Check(flatOnly ? Near(twice, before + 2f * bonus, 0.01f) : twice > before,
                $"two pickups: {stat} {before:0.##} -> {twice:0.##} (+{2f * bonus:0.##})", $"two pickups: {stat} {before:0.##} -> {twice:0.##}, expected {before + 2f * bonus:0.##}");
            List<object> added = NewSources(sources);
            Check(added.Count == 2, "each pickup has its own source", $"{added.Count} new source(s) for two pickups");
            if (added.Count >= 1)
            {
                _rig.Stats.RemoveModifiersFromSource(added[0]);
                float one = _rig.Stats.GetStatValue(stat);
                Check(flatOnly ? Near(one, before + bonus, 0.01f) : one > before && one < twice,
                    $"removing one pickup's source keeps the other: {stat} {one:0.##}", $"after removing one source {stat} is {one:0.##}, expected {before + bonus:0.##}");
            }
            RemoveNewSources(sources);
            Check(Near(_rig.Stats.GetStatValue(stat), before, 0.01f), "removing both restores the stat", $"{stat} {_rig.Stats.GetStatValue(stat):0.##}, was {before:0.##}");
        }

        // QA-ITEM-09: on the PLAYER, a permanent item buff survives the expiry of a timed buff on the same stat
        // (BUG-100 pattern; cfb6a230 gives every application its own SourceEffect). QA-ENT-16 is the enemy twin.
        private IEnumerator CaseItemBuffSurvivesTimedBuff()
        {
            bool ready = false;
            yield return ItemCaseReady(ok => ready = ok);
            if (!ready) yield break;
            PlayerEffectReceiver receiver = _rig.Player.GetComponentInChildren<PlayerEffectReceiver>(true);
            if (receiver == null)
            {
                Fail("the player has no PlayerEffectReceiver");
                yield break;
            }
            const StatType stat = StatType.STR;
            const float bonus = 5f;
            ModifierMaxStatsEffect effect = MakeMaxStatsEffect(stat, bonus);
            ItemSO item = effect != null ? MakeItem("QA Strength Ring", effect) : null;
            StatModifierGroup timed = MakeGroup(new StatModifier(stat, bonus, ModifierType.Flat, StatImpactType.Recovery), null);
            if (item == null || timed == null)
            {
                Fail("QA cannot build the item or the timed buff group (field renamed?)");
                yield break;
            }
            HashSet<object> sources = PlayerModifierSources();
            float before = _rig.Stats.GetStatValue(stat);
            Collect(SpawnDrop(item, ParkPosition), out string error);
            yield return Frames(1);
            if (error != null)
            {
                Fail($"Interact threw {error}");
                yield break;
            }
            float withItem = _rig.Stats.GetStatValue(stat);
            const float duration = 0.5f;
            receiver.AdjustMaxForDuration(timed, duration, new AbilityContext { Caster = new QAEffectCaster(_rig.Player.transform) });
            yield return Frames(2);
            float withBoth = _rig.Stats.GetStatValue(stat);
            Check(withBoth > withItem, $"timed buff on top: {stat} {withItem:0.##} -> {withBoth:0.##}", $"timed buff did not apply: {stat} {withItem:0.##} -> {withBoth:0.##}");
            yield return Wait(duration + 0.5f);
            float after = _rig.Stats.GetStatValue(stat);
            Check(Near(after, withItem, 0.01f), $"after the timed buff expired the item buff remains: {stat} {after:0.##}",
                $"{stat} is {after:0.##} after the timed buff expired, expected {withItem:0.##} (item buff lost with the timed buff)");
            Check(withItem > before, $"item gave {stat} +{withItem - before:0.##}", $"item gave {stat} nothing");
            RemoveNewSources(sources);
        }

        // QA-ITEM-10: a damaged item (an empty effect slot, or no effect at all) is still collected and released,
        // and no exception reaches the player's pickup state. ItemController.Interact has no null guard today.
        private IEnumerator CaseItemBrokenData()
        {
            bool ready = false;
            yield return ItemCaseReady(ok => ready = ok);
            if (!ready) yield break;
            ModifierMaxStatsEffect good = MakeMaxStatsEffect(StatType.Defense, 1f);
            ItemSO holed = good != null ? MakeItem("QA Holed Item", good, null) : null;
            ItemSO empty = MakeItem("QA Empty Item");
            if (holed == null || empty == null)
            {
                Fail("QA cannot build the test items (field renamed?)");
                yield break;
            }
            HashSet<object> sources = PlayerModifierSources();
            foreach (ItemSO data in new[] { holed, empty })
            {
                ItemController drop = SpawnDrop(data, ParkPosition);
                _collected.Clear();
                bool collected = Collect(drop, out string error);
                yield return Frames(1);
                Check(error == null, $"{data.name}: no exception", $"{data.name}: ItemController.Interact threw {error}");
                Check(collected && _collected.Count == 1, $"{data.name}: still collected (ON_COLLECT_ITEM once)",
                    $"{data.name}: Interact returned {collected}, ON_COLLECT_ITEM {_collected.Count}x — the item stays on the floor and Q retries it forever");
                if (drop != null && drop.gameObject.activeSelf) pool.Despawn(drop.gameObject);
            }
            RemoveNewSources(sources);
        }

        // QA-ITEM-11: a pooled drop that is reused shows and gives the NEW item only (SetDataItem replaces name,
        // sprite and effects).
        private IEnumerator CaseItemPoolReuse()
        {
            bool ready = false;
            yield return ItemCaseReady(ok => ready = ok);
            if (!ready) yield break;
            IWallet wallet = PlayerWallet;
            ModifierMaxStatsEffect statEffect = MakeMaxStatsEffect(StatType.Defense, 3f);
            CoinModifierEffect coinEffect = MakeCoinEffect(5);
            ItemSO first = statEffect != null ? MakeItem("QA Reuse A", statEffect) : null;
            ItemSO second = coinEffect != null ? MakeItem("QA Reuse B", coinEffect) : null;
            if (first == null || second == null || wallet == null)
            {
                Fail(wallet == null ? "the player has no IWallet" : "QA cannot build the test items (field renamed?)");
                yield break;
            }
            HashSet<object> sources = PlayerModifierSources();
            ItemController a = SpawnDrop(first, ParkPosition);
            Collect(a, out string e1);
            yield return Frames(1);
            float defense = _rig.Stats.GetStatValue(StatType.Defense);
            int coins = wallet.GetCurrentCoin();

            ItemController b = SpawnDrop(second, ParkPosition);
            if (b != a) Note("the pool handed out a different instance; reuse not exercised this run");
            Check(b != null && b.name == second.ItemName, $"reused drop shows the new name '{second.ItemName}'", $"reused drop is named '{(b != null ? b.name : "null")}'");
            Collect(b, out string e2);
            yield return Frames(1);
            if (e1 != null || e2 != null)
            {
                Fail($"Interact threw: {e1 ?? e2}");
                RemoveNewSources(sources);
                yield break;
            }
            Check(Near(_rig.Stats.GetStatValue(StatType.Defense), defense), "the second pickup does not re-apply the first item's stat buff",
                $"Defense {defense:0.##} -> {_rig.Stats.GetStatValue(StatType.Defense):0.##} on the second pickup (old effects kept on reuse)");
            Check(wallet.GetCurrentCoin() == coins + 5, "the second pickup gives the new item's coins", $"coins {coins} -> {wallet.GetCurrentCoin()}, expected +5");
            RemoveNewSources(sources);
        }

        // QA-ITEM-12: two items in reach — one Q collects exactly one; the next Q collects the other, not the first
        // again. Interact.FindNearestObject walks the whole collider buffer, including entries left from an earlier
        // search.
        private IEnumerator CaseItemTwoInReach()
        {
            bool ready = false;
            yield return ItemCaseReady(ok => ready = ok);
            if (!ready) yield break;
            if (_rig.Input == null)
            {
                Fail("player has no PlayerInputHandler");
                yield break;
            }
            IWallet wallet = PlayerWallet;
            CoinModifierEffect one = MakeCoinEffect(1);
            CoinModifierEffect ten = MakeCoinEffect(10);
            ItemSO small = one != null ? MakeItem("QA Coin 1", one) : null;
            ItemSO big = ten != null ? MakeItem("QA Coin 10", ten) : null;
            if (small == null || big == null || wallet == null)
            {
                Fail(wallet == null ? "the player has no IWallet" : "QA cannot build the test items (field renamed?)");
                yield break;
            }
            Vector2 at = ParkPosition;
            PlacePlayer(at);
            ItemController a = SpawnDrop(small, at + Vector2.right * 0.2f);
            ItemController b = SpawnDrop(big, at + Vector2.left * 0.2f);
            Physics2D.SyncTransforms();
            _input ??= new QAInputDriver();
            _input.Bind(_rig.Input);
            int start = wallet.GetCurrentCoin();
            List<GameObject> taken = new();

            for (int press = 1; press <= 2; press++)
            {
                bool idle = false;
                yield return WaitUntil(() => PlayerInBasicState, 3f, ok => idle = ok);
                if (!idle)
                {
                    Fail($"press {press}: player is in {PlayerState}, not Idle / Move");
                    break;
                }
                _collected.Clear();
                _input.KeyDown(Key.Q);
                bool got = false;
                yield return WaitUntil(() => _collected.Count > 0, 3f, ok => got = ok);
                _input.KeyUp();
                yield return Frames(2);
                Check(got && _collected.Count == 1, $"press {press}: one item collected", $"press {press}: ON_COLLECT_ITEM {_collected.Count}x");
                if (_collected.Count > 0)
                {
                    Check(!taken.Contains(_collected[0]), $"press {press}: a new item ({_collected[0].name})",
                        $"press {press}: collected {_collected[0].name} again after it was released (stale collider in Interact's buffer)");
                    taken.AddRange(_collected);
                }
            }
            int gained = wallet.GetCurrentCoin() - start;
            Check(gained == 11, "both items collected once each (+11 coins)", $"coins +{gained}, expected +11 (1 + 10)");
            Check(a == null || !a.gameObject.activeSelf, "first item released", "first item still on the floor");
            Check(b == null || !b.gameObject.activeSelf, "second item released", "second item still on the floor");
            if (a != null && a.gameObject.activeSelf) pool.Despawn(a.gameObject);
            if (b != null && b.gameObject.activeSelf) pool.Despawn(b.gameObject);
        }

        #endregion
    }
}
#endif
