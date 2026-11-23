#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace QA.Automation
{
    // Logic-only systems, no player: event bus, stat modifiers, object pool, room clear counter, map and
    // room models, room JSON data, A* pathfinding, item drop table. Every ScriptableObject is used through
    // an Object.Instantiate copy, so no committed asset is touched.
    public class QASystemsSuite : QASuite
    {
        [SerializeField] private ObjectPoolManager pool;
        [SerializeField] private BaseStatsSO statsAsset;
        [SerializeField] private MapModel mapModel;
        [SerializeField] private DepotItem depotItem;
        [SerializeField] private string roomJsonFolder = "Assets/Data/Json/Room";
        [SerializeField] private string pathfindingRoom = "CombatRoom_GrandArena.json";

        private readonly List<Object> _created = new();

        public override void Cleanup()
        {
            foreach (Object o in _created)
                if (o != null) Destroy(o);
            _created.Clear();
        }

        private T Copy<T>(T asset) where T : Object
        {
            T copy = Instantiate(asset);
            _created.Add(copy);
            return copy;
        }

        public override IEnumerator Run()
        {
            yield return Case("QA-EVT-01", "EVT", QAPriority.P0, "Emit delivers the payload to a registered listener", CaseEventDeliver());
            yield return Case("QA-EVT-02", "EVT", QAPriority.P0, "UnResgister stops delivery", CaseEventUnregister());
            yield return Case("QA-EVT-03", "EVT", QAPriority.P1, "Two listeners on one event both receive it; Emit with no listener is safe", CaseEventMulti());
            yield return Case("QA-EVT-04", "EVT", QAPriority.P1, "A listener that unregisters itself during Emit: the others still receive it, it gets no later Emit", CaseEventSelfUnregister());
            yield return Case("QA-EVT-05", "EVT", QAPriority.P2, "A listener that throws: do the listeners after it still receive the event?", CaseEventThrowingListener());

            yield return Case("QA-STAT-01", "STAT", QAPriority.P0, "Flat modifier adds its value to FinalValue", CaseStatFlat());
            yield return Case("QA-STAT-02", "STAT", QAPriority.P1, "PercentAdd modifiers sum before multiplying", CaseStatPercentAdd());
            yield return Case("QA-STAT-03", "STAT", QAPriority.P1, "PercentMult modifier multiplies", CaseStatPercentMult());
            yield return Case("QA-STAT-04", "STAT", QAPriority.P0, "RemoveModifiersFromSource restores the value", CaseStatRemove());
            yield return Case("QA-STAT-05", "STAT", QAPriority.P1, "Removing one source keeps another source's modifier", CaseStatSourceIsolation());
            yield return Case("QA-STAT-06", "STAT", QAPriority.P0, "Working on an Instantiate copy leaves the stat asset unchanged", CaseStatAssetUntouched());
            yield return Case("QA-STAT-07", "STAT", QAPriority.P1, "StatSheet reads each PrimaryStatEntry as BaseValue; two sheets from one profile stay independent", CaseStatPrimaryEntries());

            yield return Case("QA-POOL-01", "POOL", QAPriority.P0, "Spawn returns an active instance at the requested position", CasePoolSpawn());
            yield return Case("QA-POOL-02", "POOL", QAPriority.P0, "Release deactivates; the next Spawn reuses the same instance", CasePoolReuse());
            yield return Case("QA-POOL-03", "POOL", QAPriority.P1, "Two spawns without release give two distinct instances", CasePoolDistinct());

            yield return Case("QA-MAP-01", "MAP", QAPriority.P0, "Room clear: ON_OPEN_DOOR fires once, when the last spawned enemy dies", CaseRoomCounter());
            yield return Case("QA-MAP-02", "MAP", QAPriority.P1, "Room clear counts an extra spawned enemy", CaseRoomExtraEnemy());
            yield return Case("QA-MAP-03", "MAP", QAPriority.P1, "MapModel.GetRandomRoom deals every room once per cycle", CaseMapShuffleBag());
            yield return Case("QA-MAP-04", "MAP", QAPriority.P1, "RoomModel.GetSpawnSet stays within weight budget + overflow (20 seeds per room)", CaseSpawnBudget());
            yield return Case("QA-MAP-05", "MAP", QAPriority.P1, "Every room JSON parses with matching tiles / poses / layers counts", CaseRoomJson());
            yield return Case("QA-MAP-06", "MAP", QAPriority.P2, "Combat rooms carry a spawn marker; other room types report theirs", CaseRoomSpawnMarkers());
            yield return Case("QA-MAP-08", "MAP", QAPriority.P1, "Room clear emits ON_ROOM_CLEARED once, before ON_OPEN_DOOR", CaseRoomClearedOrder());
            yield return Case("QA-MAP-09", "MAP", QAPriority.P1, "A death after the room is cleared emits nothing more", CaseRoomDeathAfterClear());
            yield return Case("QA-MAP-10", "MAP", QAPriority.P2, "A room that spawned 0 enemies: does its door open? (BUG-096 context)", CaseRoomZeroSpawn());

            yield return Case("QA-PATH-01", "PATH", QAPriority.P0, "A* finds a path of walkable, adjacent waypoints to the farthest reachable tile", CasePathFound());
            yield return Case("QA-PATH-02", "PATH", QAPriority.P1, "A* to a wall tile fails cleanly", CasePathBlocked());
            yield return Case("QA-PATH-03", "PATH", QAPriority.P1, "Same query twice gives the same path (search state cleared)", CasePathRepeat());
            yield return Case("QA-PATH-04", "PATH", QAPriority.P1, "Every node sits on its tile centre and maps back to itself", CasePathTileCentre());
            yield return Case("QA-PATH-05", "PATH", QAPriority.P1, "No diagonal neighbour cuts a wall corner; A* paths keep the rule", CasePathNoCornerCut());
            yield return Case("QA-PATH-06", "PATH", QAPriority.P2, "IsLineWalkable: true between adjacent floor tiles, false into a wall or off the grid", CasePathLineWalk());
            yield return Case("QA-MAP-07", "MAP", QAPriority.P1, "Room library has a StartRoom, a BossRoom and a room for every required group; every file exists", CaseRoomLibrary());

            yield return Case("QA-ITEM-01", "ITEM", QAPriority.P1, "DepotItem rolls only items from its table; same seed gives same roll", CaseDepotRoll());
        }

        #region EVT

        private IEnumerator CaseEventDeliver()
        {
            object received = null;
            int calls = 0;
            void Listener(object o) { received = o; calls++; }
            EventManager.Resgister(EventID.ON_TEST, Listener);
            object payload = new();
            EventManager.Emit(EventID.ON_TEST, payload);
            EventManager.UnResgister(EventID.ON_TEST, Listener);
            yield return null;
            Check(calls == 1 && ReferenceEquals(received, payload), "listener called once with the payload", $"calls={calls}, payload match={ReferenceEquals(received, payload)}");
        }

        private IEnumerator CaseEventUnregister()
        {
            int calls = 0;
            void Listener(object _) => calls++;
            EventManager.Resgister(EventID.ON_TEST, Listener);
            EventManager.UnResgister(EventID.ON_TEST, Listener);
            EventManager.Emit(EventID.ON_TEST);
            yield return null;
            Check(calls == 0, "no call after UnResgister", $"{calls} calls after UnResgister");
        }

        private IEnumerator CaseEventMulti()
        {
            int a = 0, b = 0;
            void A(object _) => a++;
            void B(object _) => b++;
            EventManager.Resgister(EventID.ON_TEST, A);
            EventManager.Resgister(EventID.ON_TEST, B);
            EventManager.Emit(EventID.ON_TEST);
            EventManager.UnResgister(EventID.ON_TEST, A);
            EventManager.UnResgister(EventID.ON_TEST, B);
            yield return null;
            Check(a == 1 && b == 1, "both listeners called once", $"A={a}, B={b}");
            bool threw = false;
            try { EventManager.Emit(EventID.ON_TEST); }
            catch (System.Exception e) { threw = true; Note(e.Message); }
            Check(!threw, "Emit with every listener removed does not throw", "Emit threw with no listener");
        }

        private IEnumerator CaseEventSelfUnregister()
        {
            int a = 0, b = 0;
            void A(object _) { a++; EventManager.UnResgister(EventID.ON_TEST, A); }
            void B(object _) => b++;
            EventManager.Resgister(EventID.ON_TEST, A);
            EventManager.Resgister(EventID.ON_TEST, B);
            try
            {
                EventManager.Emit(EventID.ON_TEST);
                Check(a == 1 && b == 1, "both listeners ran on the Emit during which A unregistered", $"A={a}, B={b} on the first Emit");
                EventManager.Emit(EventID.ON_TEST);
                Check(a == 1 && b == 2, "A got no later Emit, B still does", $"A={a}, B={b} after the second Emit");
            }
            finally
            {
                EventManager.UnResgister(EventID.ON_TEST, A);
                EventManager.UnResgister(EventID.ON_TEST, B);
            }
            yield return null;
        }

        // EventManager invokes one multicast delegate: an exception in one listener skips every listener after it.
        // ON_ENEMY_DEATH has several (RoomCell counter, ItemSpawner, spawner release), so this is a design question.
        private IEnumerator CaseEventThrowingListener()
        {
            int after = 0;
            void Throws(object _) => throw new System.InvalidOperationException("QA listener failure");
            void After(object _) => after++;
            EventManager.Resgister(EventID.ON_TEST, Throws);
            EventManager.Resgister(EventID.ON_TEST, After);
            bool threw = false;
            try { EventManager.Emit(EventID.ON_TEST); }
            catch (System.InvalidOperationException) { threw = true; }
            finally
            {
                EventManager.UnResgister(EventID.ON_TEST, Throws);
                EventManager.UnResgister(EventID.ON_TEST, After);
            }
            yield return null;
            Note($"Emit rethrew to the emitter: {threw}; listener after the throwing one called: {after}x");
            if (after == 0) Observe("a throwing listener stops every later listener of the same event (no isolation in EventManager.Emit)");
        }

        #endregion

        #region STAT

        // A derived stat: adding modifiers to it never triggers RecalculateDerived from a primary.
        private const StatType Probe = StatType.Defense;

        private bool StatReady()
        {
            if (statsAsset != null) return true;
            Fail("scene not wired: no BaseStatsSO asset");
            return false;
        }

        // FinalValue = (Base + LevelUp, then modifiers in Order) + PrimaryBonus (StatSheet.ApplyModifiers).
        private static float Expected(Stat stat, float flat, float percentAdd, float percentMult)
            => ((stat.BaseValue + stat.LevelUpValue + flat) * (1f + percentAdd)) * (1f + percentMult) + stat.PrimaryBonus;

        private IEnumerator CaseStatFlat()
        {
            if (!StatReady()) yield break;
            StatSheet stats = new StatSheet(statsAsset);
            Stat stat = stats.GetStat(Probe);
            float before = stat.FinalValue;
            object source = new();
            stats.AddModifiersFromSource(source, new[] { new StatModifier(Probe, 5f, ModifierType.Flat, StatImpactType.Recovery) });
            yield return null;
            float expected = Expected(stat, 5f, 0f, 0f);
            Check(Near(stat.FinalValue, expected), $"{Probe} {before:0.##} -> {stat.FinalValue:0.##}", $"{Probe} {before:0.##} -> {stat.FinalValue:0.##}, expected {expected:0.##}");
        }

        private IEnumerator CaseStatPercentAdd()
        {
            if (!StatReady()) yield break;
            StatSheet stats = new StatSheet(statsAsset);
            Stat stat = stats.GetStat(Probe);
            stats.AddModifiersFromSource(new object(), new[]
            {
                new StatModifier(Probe, 0.1f, ModifierType.PercentAdd, StatImpactType.Recovery),
                new StatModifier(Probe, 0.2f, ModifierType.PercentAdd, StatImpactType.Recovery),
            });
            yield return null;
            float expected = Expected(stat, 0f, 0.3f, 0f);
            Check(Near(stat.FinalValue, expected), $"+10% +20% -> {stat.FinalValue:0.##} (x1.3)", $"FinalValue {stat.FinalValue:0.##}, expected {expected:0.##} (sum 30% then multiply once)");
        }

        private IEnumerator CaseStatPercentMult()
        {
            if (!StatReady()) yield break;
            StatSheet stats = new StatSheet(statsAsset);
            Stat stat = stats.GetStat(Probe);
            stats.AddModifiersFromSource(new object(), new[]
            {
                new StatModifier(Probe, 4f, ModifierType.Flat, StatImpactType.Recovery),
                new StatModifier(Probe, 0.5f, ModifierType.PercentMult, StatImpactType.Recovery),
            });
            yield return null;
            float expected = Expected(stat, 4f, 0f, 0.5f);
            Check(Near(stat.FinalValue, expected), $"(base+4) x1.5 -> {stat.FinalValue:0.##}", $"FinalValue {stat.FinalValue:0.##}, expected {expected:0.##}");
        }

        private IEnumerator CaseStatRemove()
        {
            if (!StatReady()) yield break;
            StatSheet stats = new StatSheet(statsAsset);
            Stat stat = stats.GetStat(Probe);
            float before = stat.FinalValue;
            object source = new();
            stats.AddModifiersFromSource(source, new[] { new StatModifier(Probe, 7f, ModifierType.Flat, StatImpactType.Recovery), new StatModifier(Probe, 0.25f, ModifierType.PercentAdd, StatImpactType.Recovery) });
            stats.RemoveModifiersFromSource(source);
            yield return null;
            Check(Near(stat.FinalValue, before) && stat.Modifiers.Count == 0, $"back to {before:0.##}, 0 modifiers",
                $"FinalValue {stat.FinalValue:0.##} (was {before:0.##}), {stat.Modifiers.Count} modifiers left");
        }

        private IEnumerator CaseStatSourceIsolation()
        {
            if (!StatReady()) yield break;
            StatSheet stats = new StatSheet(statsAsset);
            Stat stat = stats.GetStat(Probe);
            object item = new(), buff = new();
            stats.AddModifiersFromSource(item, new[] { new StatModifier(Probe, 3f, ModifierType.Flat, StatImpactType.Recovery) });
            stats.AddModifiersFromSource(buff, new[] { new StatModifier(Probe, 8f, ModifierType.Flat, StatImpactType.Recovery) });
            stats.RemoveModifiersFromSource(buff);
            yield return null;
            float expected = Expected(stat, 3f, 0f, 0f);
            Check(Near(stat.FinalValue, expected) && stat.Modifiers.Count == 1,
                $"item modifier kept: {stat.FinalValue:0.##}", $"FinalValue {stat.FinalValue:0.##}, expected {expected:0.##}; {stat.Modifiers.Count} modifiers left");
        }

        private IEnumerator CaseStatAssetUntouched()
        {
            if (!StatReady()) yield break;
            // A fresh sheet from the asset must read the same numbers after another sheet was buffed.
            Dictionary<StatType, float> before = new StatSheet(statsAsset).GetFullStat();
            StatSheet stats = new StatSheet(statsAsset);
            stats.AddModifiersFromSource(new object(), new[] { new StatModifier(Probe, 50f, ModifierType.Flat, StatImpactType.Recovery) });
            stats.AddPrimaryPoint(StatType.STR, 3f);
            yield return null;
            Dictionary<StatType, float> after = new StatSheet(statsAsset).GetFullStat();
            List<string> changed = before.Where(p => !after.TryGetValue(p.Key, out float v) || !Mathf.Approximately(v, p.Value)).Select(p => p.Key.ToString()).ToList();
            Check(changed.Count == 0,
                $"{statsAsset.name} unchanged", $"{statsAsset.name} changed: {string.Join(", ", changed)}");
        }

        private IEnumerator CaseStatPrimaryEntries()
        {
            if (!StatReady()) yield break;
            // Stat split (a0e250fb): each authored PrimaryStatEntry is the BaseValue of its primary stat on a new sheet,
            // and a primary point spent on one sheet does not reach a second sheet built from the same profile.
            StatSheet a = new StatSheet(statsAsset);
            StatSheet b = new StatSheet(statsAsset);
            List<string> wrong = new();
            int rows = 0;
            foreach (PrimaryStatEntry entry in statsAsset.PrimaryStats)
            {
                if (entry == null) { wrong.Add("null PrimaryStatEntry row"); continue; }
                rows++;
                float sheetBase = a.GetStat(entry.Type).BaseValue;
                if (!Near(sheetBase, entry.BaseValue)) wrong.Add($"{entry.Type}: sheet {sheetBase:0.##}, authored {entry.BaseValue:0.##}");
            }
            Check(rows > 0, $"{rows} primary rows on {statsAsset.name}", $"{statsAsset.name} has no PrimaryStatEntry rows");
            Check(wrong.Count == 0, "every primary BaseValue matches its authored entry", string.Join("; ", wrong));

            float bBefore = b.GetStatValue(StatType.STR);
            a.AddPrimaryPoint(StatType.STR, 2f);
            yield return null;
            Check(Near(b.GetStatValue(StatType.STR), bBefore),
                $"second sheet STR unchanged ({bBefore:0.##}) after a point on the first",
                $"second sheet STR {b.GetStatValue(StatType.STR):0.##}, was {bBefore:0.##} — sheets share state");
        }

        #endregion

        #region POOL

        private GameObject _poolTemplate;

        private GameObject Template()
        {
            if (_poolTemplate != null) return _poolTemplate;
            _poolTemplate = new GameObject("QAPoolTemplate");
            _poolTemplate.transform.position = new Vector3(500f, 500f, 0f);
            _created.Add(_poolTemplate);
            return _poolTemplate;
        }

        private IEnumerator CasePoolSpawn()
        {
            if (pool == null) { Fail("scene not wired: no ObjectPoolManager"); yield break; }
            Vector2 at = new(3f, -2f);
            GameObject spawned = pool.Spawn(Template(), at, Quaternion.identity);
            yield return null;
            Check(spawned != null && spawned.activeInHierarchy, "active instance returned", "no active instance");
            if (spawned == null) yield break;
            Check(Vector2.Distance(spawned.transform.position, at) < 0.001f, $"at {at}", $"at {(Vector2)spawned.transform.position}, asked {at}");
            Check(spawned != _poolTemplate, "instance is not the template", "pool returned the template itself");
            pool.Despawn(spawned);
        }

        private IEnumerator CasePoolReuse()
        {
            if (pool == null) { Fail("scene not wired: no ObjectPoolManager"); yield break; }
            GameObject first = pool.Spawn(Template(), Vector2.zero, Quaternion.identity);
            pool.Despawn(first);
            yield return null;
            Check(!first.activeSelf, "released instance inactive", "released instance still active");
            GameObject second = pool.Spawn(Template(), Vector2.one, Quaternion.identity);
            yield return null;
            Check(second == first && second.activeSelf, "same instance reused and re-activated", "a new instance was created instead of reusing the released one");
            pool.Despawn(second);
        }

        private IEnumerator CasePoolDistinct()
        {
            if (pool == null) { Fail("scene not wired: no ObjectPoolManager"); yield break; }
            GameObject a = pool.Spawn(Template(), Vector2.zero, Quaternion.identity);
            GameObject b = pool.Spawn(Template(), Vector2.zero, Quaternion.identity);
            yield return null;
            Check(a != null && b != null && a != b, "two distinct instances", "the same instance was handed out twice");
            if (a != null) pool.Despawn(a);
            if (b != null && b != a) pool.Despawn(b);
        }

        #endregion

        #region MAP

        private int _doorEvents;
        private void OnOpenDoor(object _) => _doorEvents++;

        private RoomCell NewRoomCell()
        {
            GameObject go = new("QARoomCell");
            _created.Add(go);
            return go.AddComponent<RoomCell>();
        }

        private IEnumerator CaseRoomCounter()
        {
            RoomCell room = NewRoomCell();
            _doorEvents = 0;
            EventManager.Resgister(EventID.ON_OPEN_DOOR, OnOpenDoor);
            try
            {
                room.OnDoneSpawnEnemy(3);
                room.OnEnemyDeath();
                room.OnEnemyDeath();
                Check(_doorEvents == 0, "no door event after 2 of 3 deaths", $"ON_OPEN_DOOR fired {_doorEvents} times after 2 of 3 deaths");
                room.OnEnemyDeath();
                Check(_doorEvents == 1 && room.EnemyCount == 0, "ON_OPEN_DOOR once at the third death", $"ON_OPEN_DOOR fired {_doorEvents} times, EnemyCount {room.EnemyCount}");
            }
            finally
            {
                EventManager.UnResgister(EventID.ON_OPEN_DOOR, OnOpenDoor);
            }
            yield return null;
        }

        private IEnumerator CaseRoomExtraEnemy()
        {
            RoomCell room = NewRoomCell();
            _doorEvents = 0;
            EventManager.Resgister(EventID.ON_OPEN_DOOR, OnOpenDoor);
            try
            {
                room.OnDoneSpawnEnemy(1);
                room.OnSpawnExtraEnemy();
                room.OnEnemyDeath();
                Check(_doorEvents == 0, "door stays shut while the extra enemy lives", $"ON_OPEN_DOOR fired {_doorEvents} times with one enemy left");
                room.OnEnemyDeath();
                Check(_doorEvents == 1, "door opens after the extra enemy", $"ON_OPEN_DOOR fired {_doorEvents} times");
            }
            finally
            {
                EventManager.UnResgister(EventID.ON_OPEN_DOOR, OnOpenDoor);
            }
            yield return null;
        }

        private IEnumerator CaseRoomClearedOrder()
        {
            RoomCell room = NewRoomCell();
            List<string> order = new();
            void Cleared(object _) => order.Add("ON_ROOM_CLEARED");
            void Door(object _) => order.Add("ON_OPEN_DOOR");
            EventManager.Resgister(EventID.ON_ROOM_CLEARED, Cleared);
            EventManager.Resgister(EventID.ON_OPEN_DOOR, Door);
            try
            {
                room.OnDoneSpawnEnemy(2);
                room.OnEnemyDeath();
                Check(order.Count == 0, "nothing after 1 of 2 deaths", $"events after 1 of 2 deaths: {string.Join(", ", order)}");
                room.OnEnemyDeath();
                Check(order.SequenceEqual(new[] { "ON_ROOM_CLEARED", "ON_OPEN_DOOR" }), "ON_ROOM_CLEARED then ON_OPEN_DOOR, once each",
                    $"events at the last death: {string.Join(", ", order)}");
            }
            finally
            {
                EventManager.UnResgister(EventID.ON_ROOM_CLEARED, Cleared);
                EventManager.UnResgister(EventID.ON_OPEN_DOOR, Door);
            }
            yield return null;
        }

        private IEnumerator CaseRoomDeathAfterClear()
        {
            RoomCell room = NewRoomCell();
            _doorEvents = 0;
            EventManager.Resgister(EventID.ON_OPEN_DOOR, OnOpenDoor);
            try
            {
                room.OnDoneSpawnEnemy(1);
                room.OnEnemyDeath();
                room.OnEnemyDeath();
                room.OnEnemyDeath();
                Check(_doorEvents == 1, "the door opened once; later deaths emit nothing", $"ON_OPEN_DOOR fired {_doorEvents} times for 1 spawn and 3 deaths");
                Note($"EnemyCount after the extra deaths: {room.EnemyCount}");
                room.OnDoneSpawnEnemy(1);
                room.OnEnemyDeath();
                Check(_doorEvents == 2, "a new wave in the same cell clears normally after stray deaths", $"ON_OPEN_DOOR fired {_doorEvents} times; a new wave of 1 did not clear (EnemyCount {room.EnemyCount})");
            }
            finally
            {
                EventManager.UnResgister(EventID.ON_OPEN_DOOR, OnOpenDoor);
            }
            yield return null;
        }

        private IEnumerator CaseRoomZeroSpawn()
        {
            RoomCell room = NewRoomCell();
            _doorEvents = 0;
            EventManager.Resgister(EventID.ON_OPEN_DOOR, OnOpenDoor);
            try
            {
                room.OnDoneSpawnEnemy(0);
            }
            finally
            {
                EventManager.UnResgister(EventID.ON_OPEN_DOOR, OnOpenDoor);
            }
            yield return null;
            if (_doorEvents == 0) Observe("ON_DONE_SPAWN_ENEMY with 0 enemies never opens the door (a room whose spawn set came back empty stays sealed)");
            else Note($"ON_OPEN_DOOR fired {_doorEvents} times for a 0-enemy room");
        }

        private IEnumerator CaseMapShuffleBag()
        {
            if (mapModel == null) { Fail("scene not wired: no MapModel asset"); yield break; }
            MapModel map = Copy(mapModel);
            int rooms = map.fullRoomList.Count(r => r != null);
            if (rooms == 0) { Skip("MapModel has no rooms"); yield break; }
            List<RoomModel> cycle = new();
            for (int i = 0; i < rooms; i++) cycle.Add(map.GetRandomRoom());
            int distinct = cycle.Where(r => r != null).Distinct().Count();
            Check(distinct == rooms, $"{rooms} draws gave {rooms} distinct rooms", $"{rooms} draws gave {distinct} distinct rooms ({cycle.Count(r => r == null)} null)");
            RoomModel next = map.GetRandomRoom();
            Check(next != null, "a new cycle starts after the bag empties", "GetRandomRoom returned null after a full cycle");
            yield return null;
        }

        private IEnumerator CaseSpawnBudget()
        {
            if (mapModel == null) { Fail("scene not wired: no MapModel asset"); yield break; }
            foreach (RoomModel asset in mapModel.fullRoomList.Where(r => r != null).Distinct())
            {
                SerializedObject so = new(asset);
                int budget = so.FindProperty("weightBudget").intValue;
                float overflow = so.FindProperty("overflowPercent").floatValue;
                int limit = Mathf.FloorToInt(budget * (1f + overflow));
                int worst = 0, empty = 0;
                for (int seed = 0; seed < 20; seed++)
                {
                    Random.InitState(1000 + seed);
                    RoomModel room = Copy(asset);
                    List<EnemySpawnEntry> set = room.GetSpawnSet();
                    if (set == null || set.Count == 0) { empty++; continue; }
                    int weight = set.Sum(e => e.enemy.weight * e.count);
                    worst = Mathf.Max(worst, weight);
                }
                Check(empty == 0, $"{asset.name}: every seed spawned something", $"{asset.name}: {empty}/20 seeds returned an empty spawn set");
                Check(worst <= limit, $"{asset.name}: worst total weight {worst} <= {limit} (budget {budget} +{overflow:P0})",
                    $"{asset.name}: total weight {worst} exceeds budget {budget} +{overflow:P0} = {limit} (ADR-0003 budget guarantee)");
                yield return null;
            }
        }

        private List<(string name, LevelData data)> LoadRooms()
        {
            List<(string, LevelData)> rooms = new();
            string folder = System.IO.Path.Combine(QARunConfig.ProjectRoot, roomJsonFolder);
            foreach (string file in Directory.GetFiles(folder, "*.json").OrderBy(f => f))
                rooms.Add((System.IO.Path.GetFileNameWithoutExtension(file), JsonUtility.FromJson<LevelData>(File.ReadAllText(file))));
            return rooms;
        }

        private IEnumerator CaseRoomJson()
        {
            List<(string name, LevelData data)> rooms = LoadRooms();
            Check(rooms.Count > 0, $"{rooms.Count} room files", $"no *.json under {roomJsonFolder}");
            foreach ((string name, LevelData data) in rooms)
            {
                bool ok = data != null && data.tiles.Count > 0 && data.tiles.Count == data.poses.Count && data.poses.Count == data.layerIndices.Count;
                if (!ok) Fail($"{name}: tiles {data?.tiles.Count} / poses {data?.poses.Count} / layers {data?.layerIndices.Count}");
            }
            Note("rooms: " + string.Join(", ", rooms.Select(r => r.name)));
            yield return null;
        }

        private IEnumerator CaseRoomSpawnMarkers()
        {
            foreach ((string name, LevelData data) in LoadRooms())
            {
                int markers = data.tiles.Count(t => t == GameConstants.TileName.SPAWN);
                if (name.StartsWith("CombatRoom_") || name.StartsWith("BossRoom_"))
                {
                    if (markers == 0) Fail($"{name}: no {GameConstants.TileName.SPAWN} marker");
                }
                else if (markers == 0) Note($"{name}: 0 spawn markers (non-combat room; doors must open without enemies — see BUG-096)");
            }
            yield return null;
        }

        private const string RoomLibraryAsset = "Assets/SO/Dungeon/Maze_Storage.asset";

        // RoomGeneraterController.BuildMazeRooms (e2b7efee) picks the start / end room by RoomType and at least
        // one room per required group (Shop; Treasure / Rest / Puzzle / Secret). It gives up the run when the
        // library lacks one, so the committed library must carry them all.
        private IEnumerator CaseRoomLibrary()
        {
            DungeonRoomSO library = AssetDatabase.LoadAssetAtPath<DungeonRoomSO>(RoomLibraryAsset);
            if (library == null) { Fail($"room library missing: {RoomLibraryAsset}"); yield break; }
            List<RoomFile> rooms = Copy(library).room;
            Note("types: " + string.Join(", ", rooms.GroupBy(r => r.roomType).OrderBy(g => g.Key).Select(g => $"{g.Key} {g.Count()}")));
            Check(rooms.Count(r => r.roomType == RoomType.StartRoom) >= 1, "has a StartRoom", "no StartRoom in the library");
            Check(rooms.Count(r => r.roomType == RoomType.BossRoom) >= 1, "has a BossRoom", "no BossRoom in the library");
            Check(rooms.Any(r => r.roomType == RoomType.ShopRoom), "has a Shop room", "no ShopRoom (required group 'Shop')");
            RoomType[] support = { RoomType.TreasureRoom, RoomType.RestRoom, RoomType.PuzzleRoom, RoomType.SecretRoom };
            Check(rooms.Any(r => support.Contains(r.roomType)), "has a support / special room", "no Treasure / Rest / Puzzle / Secret room (required group 'Support / special')");
            foreach (RoomFile r in rooms)
            {
                string path = System.IO.Path.Combine(QARunConfig.ProjectRoot, "Assets" + r.filePath);
                if (!File.Exists(path)) Fail($"{r.roomName}: file missing Assets{r.filePath}");
            }
            yield return null;
        }

        #endregion

        #region PATH

        private PathfindingGrid _grid;
        private List<Node> _walkable;

        private bool BuildGrid()
        {
            if (_grid != null) return true;
            string file = System.IO.Path.Combine(QARunConfig.ProjectRoot, roomJsonFolder, pathfindingRoom);
            if (!File.Exists(file))
            {
                Fail($"room file missing: {roomJsonFolder}/{pathfindingRoom}");
                return false;
            }
            LevelData data = JsonUtility.FromJson<LevelData>(File.ReadAllText(file));
            _grid = new PathfindingGrid();
            _grid.BuildGrid(data, Vector2.zero);
            _walkable = new List<Node>();
            HashSet<Node> seen = new();
            for (int x = -150; x <= 150; x++)
            for (int y = -150; y <= 150; y++)
            {
                Node node = _grid.GetNode(new Vector2(x, y));
                if (node != null && node.Walkable && seen.Add(node)) _walkable.Add(node);
            }
            Note($"{pathfindingRoom}: {_walkable.Count} walkable tiles of {_grid.MaxSize}");
            return true;
        }

        private Node Farthest(Node start)
        {
            Dictionary<Node, int> depth = new() { [start] = 0 };
            Queue<Node> queue = new();
            queue.Enqueue(start);
            Node far = start;
            while (queue.Count > 0)
            {
                Node n = queue.Dequeue();
                if (depth[n] > depth[far]) far = n;
                foreach (Node next in n.Neighbors)
                    if (next.Walkable && !depth.ContainsKey(next)) { depth[next] = depth[n] + 1; queue.Enqueue(next); }
            }
            return far;
        }

        private IEnumerator CasePathFound()
        {
            if (!BuildGrid()) yield break;
            if (_walkable.Count < 2) { Fail("fewer than 2 walkable tiles"); yield break; }
            Node start = _walkable[0];
            Node goal = Farthest(start);
            NavPath path = AStar.FindPath(_grid, start.WorldPosition, goal.WorldPosition);
            Check(path.Success, $"path found, {path.Waypoints.Count} waypoints", $"no path from {start.WorldPosition} to reachable {goal.WorldPosition}");
            if (!path.Success) yield break;
            int bad = 0;
            for (int i = 0; i < path.Waypoints.Count; i++)
            {
                Node n = _grid.GetNode(path.Waypoints[i]);
                if (n == null || !n.Walkable) bad++;
                if (i > 0 && Vector2.Distance(path.Waypoints[i - 1], path.Waypoints[i]) > 1.5f) bad++;
            }
            Check(bad == 0, "every waypoint walkable and adjacent to the previous", $"{bad} waypoint problems (unwalkable or a jump > 1.5)");
            Check(Vector2.Distance(path.Waypoints[^1], goal.WorldPosition) < 0.6f, "ends at the target", $"ends at {path.Waypoints[^1]}, target {goal.WorldPosition}");
            yield return null;
        }

        private IEnumerator CasePathBlocked()
        {
            if (!BuildGrid()) yield break;
            Node wall = null;
            for (int x = -150; x <= 150 && wall == null; x++)
            for (int y = -150; y <= 150 && wall == null; y++)
            {
                Node n = _grid.GetNode(new Vector2(x, y));
                if (n != null && !n.Walkable) wall = n;
            }
            if (wall == null) { Skip("room has no unwalkable tile"); yield break; }
            NavPath path = null;
            bool threw = false;
            try { path = AStar.FindPath(_grid, _walkable[0].WorldPosition, wall.WorldPosition); }
            catch (System.Exception e) { threw = true; Fail($"FindPath threw {e.GetType().Name}: {e.Message}"); }
            if (!threw) Check(!path.Success, "no path to a wall tile", $"path to a wall tile reported success ({path.Waypoints.Count} waypoints)");
            yield return null;
        }

        private IEnumerator CasePathRepeat()
        {
            if (!BuildGrid()) yield break;
            if (_walkable.Count < 2) { Fail("fewer than 2 walkable tiles"); yield break; }
            Node start = _walkable[0];
            Node goal = Farthest(start);
            NavPath a = AStar.FindPath(_grid, start.WorldPosition, goal.WorldPosition);
            NavPath b = AStar.FindPath(_grid, start.WorldPosition, goal.WorldPosition);
            Check(a.Success == b.Success && a.Waypoints.SequenceEqual(b.Waypoints), "identical results", $"first {a.Waypoints.Count} waypoints, second {b.Waypoints.Count}");
            yield return null;
        }

        private Dictionary<Vector2Int, Node> AllNodes()
        {
            Dictionary<Vector2Int, Node> all = new();
            for (int x = -150; x <= 150; x++)
            for (int y = -150; y <= 150; y++)
            {
                Node n = _grid.GetNode(new Vector2(x, y));
                if (n != null) all[n.TileMapPosition] = n;
            }
            return all;
        }

        private static bool IsFloor(Dictionary<Vector2Int, Node> all, Vector2Int cell) => all.TryGetValue(cell, out Node n) && n.Walkable;

        private IEnumerator CasePathTileCentre()
        {
            if (!BuildGrid()) yield break;
            Dictionary<Vector2Int, Node> all = AllNodes();
            int bad = 0;
            string first = null;
            Vector2[] offsets = { Vector2.zero, new(0.45f, 0.45f), new(-0.45f, -0.45f), new(0.45f, -0.45f), new(-0.45f, 0.45f) };
            foreach (Node n in all.Values)
            foreach (Vector2 o in offsets)
            {
                if (_grid.GetNode(n.WorldPosition + o) == n) continue;
                bad++;
                first ??= $"node {n.TileMapPosition} at {n.WorldPosition}: point {n.WorldPosition + o} maps to {_grid.GetNode(n.WorldPosition + o)?.TileMapPosition.ToString() ?? "null"}";
            }
            Check(all.Count > 0 && bad == 0, $"{all.Count} nodes, each centre and its 4 inner corners (±0.45) map back to the node",
                $"{bad} point(s) map to another cell; first: {first}");
            yield return null;
        }

        private IEnumerator CasePathNoCornerCut()
        {
            if (!BuildGrid()) yield break;
            Dictionary<Vector2Int, Node> all = AllNodes();
            int diagonals = 0, cuts = 0;
            string first = null;
            foreach (Node n in all.Values)
            {
                if (!n.Walkable) continue;
                foreach (Node next in n.Neighbors)
                {
                    Vector2Int d = next.TileMapPosition - n.TileMapPosition;
                    if (d.x == 0 || d.y == 0) continue;
                    diagonals++;
                    if (IsFloor(all, n.TileMapPosition + new Vector2Int(d.x, 0)) && IsFloor(all, n.TileMapPosition + new Vector2Int(0, d.y))) continue;
                    cuts++;
                    first ??= $"{n.TileMapPosition} -> {next.TileMapPosition}";
                }
            }
            Check(cuts == 0, $"{diagonals} diagonal links, none past a wall corner", $"{cuts} diagonal link(s) cut a wall corner; first {first}");

            Node start = _walkable[0];
            NavPath path = AStar.FindPath(_grid, start.WorldPosition, Farthest(start).WorldPosition);
            if (!path.Success) { Fail("no A* path to check"); yield break; }
            int pathCuts = 0;
            for (int i = 1; i < path.Waypoints.Count; i++)
            {
                Node a = _grid.GetNode(path.Waypoints[i - 1]), b = _grid.GetNode(path.Waypoints[i]);
                if (a == null || b == null) { pathCuts++; continue; }
                Vector2Int d = b.TileMapPosition - a.TileMapPosition;
                if (d.x != 0 && d.y != 0 && !(IsFloor(all, a.TileMapPosition + new Vector2Int(d.x, 0)) && IsFloor(all, a.TileMapPosition + new Vector2Int(0, d.y))))
                    pathCuts++;
            }
            Check(pathCuts == 0, $"A* path of {path.Waypoints.Count} waypoints keeps the corner rule", $"{pathCuts} path step(s) cut a wall corner");
            yield return null;
        }

        private IEnumerator CasePathLineWalk()
        {
            if (!BuildGrid()) yield break;
            Dictionary<Vector2Int, Node> all = AllNodes();
            Node floor = null, floorNext = null, wall = null;
            foreach (Node n in all.Values)
            {
                if (!n.Walkable) continue;
                foreach (Vector2Int d in new[] { Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down })
                {
                    if (!all.TryGetValue(n.TileMapPosition + d, out Node m)) continue;
                    if (m.Walkable && floorNext == null) { floor = n; floorNext = m; }
                    if (!m.Walkable && wall == null) wall = m;
                }
                if (floorNext != null && wall != null) break;
            }
            if (floorNext == null) { Skip("no two adjacent floor tiles"); yield break; }
            Check(_grid.IsLineWalkable(floor.WorldPosition, floorNext.WorldPosition), $"{floor.TileMapPosition} -> {floorNext.TileMapPosition}: walkable",
                $"{floor.TileMapPosition} -> {floorNext.TileMapPosition}: two adjacent floor tiles reported not walkable");
            if (wall != null)
            {
                Node from = all.Values.Where(n => n.Walkable).OrderBy(n => (n.TileMapPosition - wall.TileMapPosition).sqrMagnitude).First();
                Check(!_grid.IsLineWalkable(from.WorldPosition, wall.WorldPosition), $"{from.TileMapPosition} -> wall {wall.TileMapPosition}: not walkable",
                    $"{from.TileMapPosition} -> wall {wall.TileMapPosition}: reported walkable");
            }
            else Note("room has no wall tile next to a floor tile; wall check not run");
            Check(!_grid.IsLineWalkable(floor.WorldPosition, new Vector2(10000f, 10000f)), "line off the grid: not walkable", "line off the grid reported walkable");
            yield return null;
        }

        #endregion

        #region ITEM

        private IEnumerator CaseDepotRoll()
        {
            if (depotItem == null) { Fail("scene not wired: no DepotItem asset"); yield break; }
            DepotItem depot = Copy(depotItem);
            HashSet<ItemSO> table = new(depot.listItems.Where(s => s != null && s.DepotItem != null).Select(s => s.DepotItem));
            int rolled = 0, foreign = 0;
            for (int i = 0; i < 200; i++)
            {
                if (!depot.TryRollItem(out ItemSO item)) continue;
                rolled++;
                if (!table.Contains(item)) foreign++;
            }
            Check(foreign == 0, $"{rolled}/200 rolls gave an item, all from the table", $"{foreign} rolled items are not in the table");
            Random.InitState(42);
            depot.TryRollItem(out ItemSO first);
            Random.InitState(42);
            depot.TryRollItem(out ItemSO second);
            Check(first == second, $"seed 42 -> {(first != null ? first.name : "nothing")} twice", "same seed gave different items");
            yield return null;
        }

        #endregion
    }
}
#endif
