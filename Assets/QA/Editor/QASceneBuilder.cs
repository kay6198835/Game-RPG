using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace QA.Automation.Editor
{
    // Builds the QA scenes from code into Assets/QA/Scenes/, one per group. Every build starts from an empty
    // scene, so a scene is always in sync with the code; nothing outside Assets/QA/ is written.
    // Assets are referenced read-only by path; a renamed asset fails the build with a clear message.
    public static class QASceneBuilder
    {
        public const string SceneFolder = "Assets/QA/Scenes";
        private const string PlayerPrefabPath = "Assets/Prefab/Player/PlayerTest.prefab";
        private const string EnemyPrefabPath = "Assets/Prefab/Enemy/EnemyPrefab.prefab";
        private const string MonumentPathFormat = "Assets/Prefab/Champion Monument/Champion_Monument_{0}.prefab";
        private const string PlayerDataPathFormat = "Assets/SO/Player/{0}/{0}.asset";
        private const string UIDebugConfigPath = "Assets/SO/UIFlow/UIDebugConfig.asset";
        private const string StatsAssetPath = "Assets/SO/Stat/Player/PaladinStats.asset";
        private const string MapModelPath = "Assets/SO/Database/Map/MapModel.asset";
        private const string DepotItemPath = "Assets/SO/Item/Depot Item 1.asset";
        private static readonly string[] Classes = { "Paladin", "Archer", "Mage" };

        public static readonly string[] PlayModeGroups = { "SYSTEMS", "VITALS", "ENEMY", "PLAYER" };

        public static string ScenePath(string group) => $"{SceneFolder}/QA_{Title(group)}.unity";

        private static string Title(string group) => group switch
        {
            "SYSTEMS" => "Systems",
            "VITALS" => "Vitals",
            "ENEMY" => "Combat",
            "PLAYER" => "PlayerFlow",
            _ => group,
        };

        public static string Build(string group)
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            CreateCamera();
            GameObject harnessObject = new("QAHarness");
            QAHarness harness = harnessObject.AddComponent<QAHarness>();
            Set(harness, "group", group);

            switch (group)
            {
                case "SYSTEMS": BuildSystems(harnessObject); break;
                case "VITALS": BuildVitals(harnessObject); break;
                case "ENEMY": BuildEnemy(harnessObject); break;
                case "PLAYER": BuildPlayer(harnessObject); break;
                default: throw new System.ArgumentException($"[QA] unknown group {group}");
            }

            string path = ScenePath(group);
            Directory.CreateDirectory(SceneFolder);
            EditorSceneManager.SaveScene(scene, path);
            Debug.Log($"[QA] Built {path}");
            return path;
        }

        private static void BuildSystems(GameObject harness)
        {
            ObjectPoolManager pool = PlayerInfrastructure(out _, out _);
            QASystemsSuite suite = harness.AddComponent<QASystemsSuite>();
            Wire(suite, "pool", pool);
            Wire(suite, "statsAsset", Require<BaseStatsSO>(StatsAssetPath));
            Wire(suite, "mapModel", Require<MapModel>(MapModelPath));
            Wire(suite, "depotItem", Require<DepotItem>(DepotItemPath));
        }

        private static void BuildVitals(GameObject harness)
        {
            PlayerInfrastructure(out PlayerManager playerManager, out _);
            QAVitalsSuite suite = harness.AddComponent<QAVitalsSuite>();
            Wire(suite, "playerManager", playerManager);
            Wire(suite, "playerClass", Require<PlayerData>(string.Format(PlayerDataPathFormat, "Paladin")));
        }

        private static void BuildEnemy(GameObject harness)
        {
            ObjectPoolManager pool = PlayerInfrastructure(out _, out Transform playerSpawn);
            playerSpawn.position = new Vector3(-8f, 0f, 0f);
            new GameObject("EnemyManager").AddComponent<EnemyManager>();
            GameObject enemySpawn = new("EnemySpawnPoint");
            enemySpawn.transform.position = new Vector3(4f, 0f, 0f);
            QAEnemySuite suite = harness.AddComponent<QAEnemySuite>();
            Wire(suite, "enemyPrefab", Require<GameObject>(EnemyPrefabPath));
            Wire(suite, "pool", pool);
            Wire(suite, "spawnPoint", enemySpawn.transform);
            // EntityMovement.Start reads EnemyManager.Instance.Grid; an empty grid answers every query with a
            // failure, which the movement code handles (same as CombatTestEnemySpawner).
            harness.AddComponent<QAEmptyPathGrid>();
        }

        private static void BuildPlayer(GameObject harness)
        {
            Sprite square = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            GameObject uiBootstrap = new("UIBootstrap");
            Wire(uiBootstrap.AddComponent<UIFlow.UIBootstrap>(), "config", AssetDatabase.LoadAssetAtPath<Object>(UIDebugConfigPath));
            ObjectPoolManager _ = PlayerInfrastructure(out PlayerManager playerManager, out Transform spawnPoint);

            PlayerData[] classData = new PlayerData[Classes.Length];
            Transform[] monuments = new Transform[Classes.Length];
            for (int i = 0; i < Classes.Length; i++)
            {
                classData[i] = Require<PlayerData>(string.Format(PlayerDataPathFormat, Classes[i]));
                GameObject monument = (GameObject)PrefabUtility.InstantiatePrefab(Require<GameObject>(string.Format(MonumentPathFormat, Classes[i])));
                monument.transform.position = new Vector3((i - 1) * 4f, 4f, 0f);
                monuments[i] = monument.transform;
            }

            // Loads GameplayUI additively: the monument's confirm panel and the hotbar live there.
            CombatTestBootstrap bootstrap = new GameObject("CombatTestBootstrap").AddComponent<CombatTestBootstrap>();
            Wire(bootstrap, "playerManager", playerManager);
            WireArray(bootstrap, "classes", new Object[0]);

            GameObject targetObject = new("QAProbeTarget") { layer = LayerMask.NameToLayer("Enemy") };
            targetObject.transform.position = new Vector3(3f, -3f, 0f);
            targetObject.transform.localScale = new Vector3(1f, 1.6f, 1f);
            SpriteRenderer targetRenderer = targetObject.AddComponent<SpriteRenderer>();
            targetRenderer.sprite = square;
            targetRenderer.color = new Color(0.85f, 0.7f, 0.45f);
            targetObject.AddComponent<BoxCollider2D>();
            QAProbeTarget target = targetObject.AddComponent<QAProbeTarget>();

            QAPlayerFlowSuite suite = harness.AddComponent<QAPlayerFlowSuite>();
            Wire(suite, "playerManager", playerManager);
            Wire(suite, "spawnPoint", spawnPoint);
            Wire(suite, "target", target);
            WireArray(suite, "classes", classData);
            WireArray(suite, "monuments", monuments);
        }

        // The services the player chain injects (CombatTestLifetimeScope is the sprint-20 composition root for
        // test scenes), plus a floor. PlayerManager spawns the player on first access to PlayerManager.Player.
        private static ObjectPoolManager PlayerInfrastructure(out PlayerManager playerManager, out Transform spawnPoint)
        {
            new GameObject("CombatTestLifetimeScope").AddComponent<CombatTestLifetimeScope>();
            ObjectPoolManager pool = new GameObject("ObjectPooling").AddComponent<ObjectPoolManager>();
            spawnPoint = new GameObject("PlayerSpawnPoint").transform;
            playerManager = new GameObject("PlayerManager").AddComponent<PlayerManager>();
            Wire(playerManager, "playerPrefab", Require<Player>(PlayerPrefabPath));
            Wire(playerManager, "spawnPoint", spawnPoint);

            Sprite square = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            GameObject floor = new("Floor");
            floor.transform.localScale = new Vector3(30f, 20f, 1f);
            SpriteRenderer floorRenderer = floor.AddComponent<SpriteRenderer>();
            floorRenderer.sprite = square;
            floorRenderer.color = new Color(0.25f, 0.27f, 0.25f);
            floorRenderer.sortingOrder = -100;
            return pool;
        }

        private static void CreateCamera()
        {
            GameObject cameraObject = new("Main Camera") { tag = "MainCamera" };
            cameraObject.transform.position = new Vector3(0f, 0f, -10f);
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 7f;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.19f, 0.3f, 0.47f);
            camera.nearClipPlane = 0.01f;
            cameraObject.AddComponent<AudioListener>();
        }

        private static T Require<T>(string path) where T : Object
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset == null) throw new FileNotFoundException($"[QA] Missing {typeof(T).Name} at {path}");
            return asset;
        }

        private static SerializedProperty Find(SerializedObject serialized, Object target, string field) =>
            serialized.FindProperty(field) ?? throw new System.ArgumentException($"{target.GetType().Name} has no field \"{field}\"");

        private static void Wire(Object target, string field, Object value)
        {
            SerializedObject serialized = new(target);
            Find(serialized, target, field).objectReferenceValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void Set(Object target, string field, string value)
        {
            SerializedObject serialized = new(target);
            Find(serialized, target, field).stringValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void WireArray(Object target, string field, Object[] values)
        {
            SerializedObject serialized = new(target);
            SerializedProperty property = Find(serialized, target, field);
            property.arraySize = values.Length;
            for (int i = 0; i < values.Length; i++) property.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
