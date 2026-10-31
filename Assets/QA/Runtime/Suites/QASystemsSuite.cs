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

            yield return Case("QA-STAT-01", "STAT", QAPriority.P0, "Flat modifier adds its value to FinalValue", CaseStatFlat());
            yield return Case("QA-STAT-02", "STAT", QAPriority.P1, "PercentAdd modifiers sum before multiplying", CaseStatPercentAdd());
            yield return Case("QA-STAT-03", "STAT", QAPriority.P1, "PercentMult modifier multiplies", CaseStatPercentMult());
            yield return Case("QA-STAT-04", "STAT", QAPriority.P0, "RemoveModifiersFromSource restores the value", CaseStatRemove());
            yield return Case("QA-STAT-05", "STAT", QAPriority.P1, "Removing one source keeps another source's modifier", CaseStatSourceIsolation());
            yield return Case("QA-STAT-06", "STAT", QAPriority.P0, "Working on an Instantiate copy leaves the stat asset unchanged", CaseStatAssetUntouched());

            yield return Case("QA-POOL-01", "POOL", QAPriority.P0, "Spawn returns an active instance at the requested position", CasePoolSpawn());
            yield return Case("QA-POOL-02", "POOL", QAPriority.P0, "Release deactivates; the next Spawn reuses the same instance", CasePoolReuse());
            yield return Case("QA-POOL-03", "POOL", QAPriority.P1, "Two spawns without release give two distinct instances", CasePoolDistinct());

            yield return Case("QA-MAP-01", "MAP", QAPriority.P0, "Room clear: ON_OPEN_DOOR fires once, when the last spawned enemy dies", CaseRoomCounter());
            yield return Case("QA-MAP-02", "MAP", QAPriority.P1, "Room clear counts an extra spawned enemy", CaseRoomExtraEnemy());
            yield return Case("QA-MAP-03", "MAP", QAPriority.P1, "MapModel.GetRandomRoom deals every room once per cycle", CaseMapShuffleBag());
            yield return Case("QA-MAP-04", "MAP", QAPriority.P1, "RoomModel.GetSpawnSet stays within weight budget + overflow (20 seeds per room)", CaseSpawnBudget());
            yield return Case("QA-MAP-05", "MAP", QAPriority.P1, "Every room JSON parses with matching tiles / poses / layers counts", CaseRoomJson());
            yield return Case("QA-MAP-06", "MAP", QAPriority.P2, "Combat rooms carry a spawn marker; other room types report theirs", CaseRoomSpawnMarkers());

            yield return Case("QA-PATH-01", "PATH", QAPriority.P0, "A* finds a path of walkable, adjacent waypoints to the farthest reachable tile", CasePathFound());
            yield return Case("QA-PATH-02", "PATH", QAPriority.P1, "A* to a wall tile fails cleanly", CasePathBlocked());
            yield return Case("QA-PATH-03", "PATH", QAPriority.P1, "Same query twice gives the same path (search state cleared)", CasePathRepeat());

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

        // FinalValue = (Base + LevelUp, then modifiers in Order) + EquipmentByPrimary (Stat.CalculateFinalValue).
        private static float Expected(Stat stat, float flat, float percentAdd, float percentMult)
            => ((stat.BaseValue + stat.LevelUpValue + flat) * (1f + percentAdd)) * (1f + percentMult) + stat.EquipmentByPrimaryValue;

        private IEnumerator CaseStatFlat()
        {
            if (!StatReady()) yield break;
            BaseStatsSO stats = Copy(statsAsset);
            Stat stat = stats.Get(Probe);
            float before = stat.FinalValue;
            object source = new();
            stats.AddModifiersFromSource(source, new[] { new StatModifier(Probe, 5f, ModifierType.Flat) });
            yield return null;
            float expected = Expected(stat, 5f, 0f, 0f);
            Check(Near(stat.FinalValue, expected), $"{Probe} {before:0.##} -> {stat.FinalValue:0.##}", $"{Probe} {before:0.##} -> {stat.FinalValue:0.##}, expected {expected:0.##}");
        }

        private IEnumerator CaseStatPercentAdd()
        {
            if (!StatReady()) yield break;
            BaseStatsSO stats = Copy(statsAsset);
            Stat stat = stats.Get(Probe);
            stat.BaseValue = Mathf.Max(stat.BaseValue, 10f);
            stats.AddModifiersFromSource(new object(), new[]
            {
                new StatModifier(Probe, 0.1f, ModifierType.PercentAdd),
                new StatModifier(Probe, 0.2f, ModifierType.PercentAdd),
            });
            yield return null;
            float expected = Expected(stat, 0f, 0.3f, 0f);
            Check(Near(stat.FinalValue, expected), $"+10% +20% -> {stat.FinalValue:0.##} (x1.3)", $"FinalValue {stat.FinalValue:0.##}, expected {expected:0.##} (sum 30% then multiply once)");
        }

        private IEnumerator CaseStatPercentMult()
        {
            if (!StatReady()) yield break;
            BaseStatsSO stats = Copy(statsAsset);
            Stat stat = stats.Get(Probe);
            stat.BaseValue = Mathf.Max(stat.BaseValue, 10f);
            stats.AddModifiersFromSource(new object(), new[]
            {
                new StatModifier(Probe, 4f, ModifierType.Flat),
                new StatModifier(Probe, 0.5f, ModifierType.PercentMult),
            });
            yield return null;
            float expected = Expected(stat, 4f, 0f, 0.5f);
            Check(Near(stat.FinalValue, expected), $"(base+4) x1.5 -> {stat.FinalValue:0.##}", $"FinalValue {stat.FinalValue:0.##}, expected {expected:0.##}");
        }

        private IEnumerator CaseStatRemove()
        {
            if (!StatReady()) yield break;
            BaseStatsSO stats = Copy(statsAsset);
            Stat stat = stats.Get(Probe);
            float before = stat.FinalValue;
            object source = new();
            stats.AddModifiersFromSource(source, new[] { new StatModifier(Probe, 7f, ModifierType.Flat), new StatModifier(Probe, 0.25f, ModifierType.PercentAdd) });
            stats.RemoveModifiersFromSource(source);
            yield return null;
            Check(Near(stat.FinalValue, before) && stat.Modifiers.Count == 0, $"back to {before:0.##}, 0 modifiers",
                $"FinalValue {stat.FinalValue:0.##} (was {before:0.##}), {stat.Modifiers.Count} modifiers left");
        }

        private IEnumerator CaseStatSourceIsolation()
        {
            if (!StatReady()) yield break;
            BaseStatsSO stats = Copy(statsAsset);
            Stat stat = stats.Get(Probe);
            object item = new(), buff = new();
            stats.AddModifiersFromSource(item, new[] { new StatModifier(Probe, 3f, ModifierType.Flat) });
            stats.AddModifiersFromSource(buff, new[] { new StatModifier(Probe, 8f, ModifierType.Flat) });
            stats.RemoveModifiersFromSource(buff);
            yield return null;
            float expected = Expected(stat, 3f, 0f, 0f);
            Check(Near(stat.FinalValue, expected) && stat.Modifiers.Count == 1,
                $"item modifier kept: {stat.FinalValue:0.##}", $"FinalValue {stat.FinalValue:0.##}, expected {expected:0.##}; {stat.Modifiers.Count} modifiers left");
        }

        private IEnumerator CaseStatAssetUntouched()
        {
            if (!StatReady()) yield break;
            Dictionary<StatType, float> before = statsAsset.FullStatsValue();
            int modifiersBefore = statsAsset.Get(Probe)?.Modifiers.Count ?? 0;
            BaseStatsSO stats = Copy(statsAsset);
            stats.AddModifiersFromSource(new object(), new[] { new StatModifier(Probe, 50f, ModifierType.Flat) });
            stats.AddPrimaryPoint(StatType.STR, 3f);
            yield return null;
            Dictionary<StatType, float> after = statsAsset.FullStatsValue();
            List<string> changed = before.Where(p => !after.TryGetValue(p.Key, out float v) || !Mathf.Approximately(v, p.Value)).Select(p => p.Key.ToString()).ToList();
            Check(changed.Count == 0 && (statsAsset.Get(Probe)?.Modifiers.Count ?? 0) == modifiersBefore,
                $"{statsAsset.name} unchanged", $"{statsAsset.name} changed: {string.Join(", ", changed)}");
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
            pool.Release(spawned);
        }

        private IEnumerator CasePoolReuse()
        {
            if (pool == null) { Fail("scene not wired: no ObjectPoolManager"); yield break; }
            GameObject first = pool.Spawn(Template(), Vector2.zero, Quaternion.identity);
            pool.Release(first);
            yield return null;
            Check(!first.activeSelf, "released instance inactive", "released instance still active");
            GameObject second = pool.Spawn(Template(), Vector2.one, Quaternion.identity);
            yield return null;
            Check(second == first && second.activeSelf, "same instance reused and re-activated", "a new instance was created instead of reusing the released one");
            pool.Release(second);
        }

        private IEnumerator CasePoolDistinct()
        {
            if (pool == null) { Fail("scene not wired: no ObjectPoolManager"); yield break; }
            GameObject a = pool.Spawn(Template(), Vector2.zero, Quaternion.identity);
            GameObject b = pool.Spawn(Template(), Vector2.zero, Quaternion.identity);
            yield return null;
            Check(a != null && b != null && a != b, "two distinct instances", "the same instance was handed out twice");
            if (a != null) pool.Release(a);
            if (b != null && b != a) pool.Release(b);
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
            Path path = AStar.FindPath(_grid, start.WorldPosition, goal.WorldPosition);
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
            Path path = null;
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
            Path a = AStar.FindPath(_grid, start.WorldPosition, goal.WorldPosition);
            Path b = AStar.FindPath(_grid, start.WorldPosition, goal.WorldPosition);
            Check(a.Success == b.Success && a.Waypoints.SequenceEqual(b.Waypoints), "identical results", $"first {a.Waypoints.Count} waypoints, second {b.Waypoints.Count}");
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
