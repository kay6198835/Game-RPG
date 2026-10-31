using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// Builds Assets/Scenes/Test/QAPlayerFlow.unity from code and runs QAPlayerFlowProbe in Play Mode.
// Menu: Tools > QA > Player Flow. Command line (Editor closed, no -batchmode, no -quit):
//   Unity.exe -projectPath <project> -executeMethod QAPlayerFlowRunner.RunFromCommandLine -logFile <log>
// The Editor exits when the probe finishes: exit code 0 = no FAIL, 1 = at least one FAIL.
[InitializeOnLoad]
public static class QAPlayerFlowRunner
{
    public const string ScenePath = "Assets/Scenes/Test/QAPlayerFlow.unity";
    private const string PlayerPrefabPath = "Assets/Prefab/Player/PlayerTest.prefab";
    private const string MonumentPathFormat = "Assets/Prefab/Champion Monument/Champion_Monument_{0}.prefab";
    private const string PlayerDataPathFormat = "Assets/SO/Player/{0}/{0}.asset";
    private const string UIDebugConfigPath = "Assets/SO/UIFlow/UIDebugConfig.asset";
    private static readonly string[] Classes = { "Paladin", "Archer", "Mage" };

    static QAPlayerFlowRunner()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    [MenuItem("Tools/QA/Player Flow/Build Scene")]
    public static void BuildSceneMenu()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        Build();
        EditorSceneManager.OpenScene(ScenePath);
    }

    [MenuItem("Tools/QA/Player Flow/Build and Run")]
    public static void RunMenu()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        Build();
        EditorSceneManager.OpenScene(ScenePath);
        EditorApplication.isPlaying = true;
    }

    public static void RunFromCommandLine()
    {
        SessionState.SetBool(QAPlayerFlowProbe.CliFlagKey, true);
        SessionState.SetInt(QAPlayerFlowProbe.ExitCodeKey, 2);
        Build();
        EditorSceneManager.OpenScene(ScenePath);
        EditorApplication.isPlaying = true;
    }

    private static void OnPlayModeChanged(PlayModeStateChange change)
    {
        if (change != PlayModeStateChange.EnteredEditMode || !SessionState.GetBool(QAPlayerFlowProbe.CliFlagKey, false)) return;
        SessionState.SetBool(QAPlayerFlowProbe.CliFlagKey, false);
        int code = SessionState.GetInt(QAPlayerFlowProbe.ExitCodeKey, 2);
        Debug.Log($"[QA] Play Mode finished, exiting with code {code}");
        EditorApplication.Exit(code);
    }

    public static void Build()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        Sprite square = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");

        CreateCamera();

        GameObject uiBootstrap = new("UIBootstrap");
        Wire(uiBootstrap.AddComponent<UIFlow.UIBootstrap>(), "config", AssetDatabase.LoadAssetAtPath<Object>(UIDebugConfigPath));

        new GameObject("CombatTestLifetimeScope").AddComponent<CombatTestLifetimeScope>();
        new GameObject("ObjectPooling").AddComponent<ObjectPoolManager>();

        GameObject spawnPoint = new("PlayerSpawnPoint");
        PlayerManager playerManager = new GameObject("PlayerManager").AddComponent<PlayerManager>();
        Wire(playerManager, "playerPrefab", Require<Player>(PlayerPrefabPath));
        Wire(playerManager, "spawnPoint", spawnPoint.transform);

        GameObject floor = new("Floor");
        floor.transform.localScale = new Vector3(30f, 20f, 1f);
        SpriteRenderer floorRenderer = floor.AddComponent<SpriteRenderer>();
        floorRenderer.sprite = square;
        floorRenderer.color = new Color(0.25f, 0.27f, 0.25f);
        floorRenderer.sortingOrder = -100;

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

        QAPlayerFlowProbe probe = new GameObject("QAPlayerFlowProbe").AddComponent<QAPlayerFlowProbe>();
        Wire(probe, "playerManager", playerManager);
        Wire(probe, "spawnPoint", spawnPoint.transform);
        Wire(probe, "target", target);
        WireArray(probe, "classes", classData);
        WireArray(probe, "monuments", monuments);

        Directory.CreateDirectory(System.IO.Path.GetDirectoryName(ScenePath));
        EditorSceneManager.SaveScene(scene, ScenePath);
        Debug.Log($"[QA] Built {ScenePath}");
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

    private static void Wire(Object target, string field, Object value)
    {
        SerializedObject serialized = new(target);
        SerializedProperty property = serialized.FindProperty(field)
            ?? throw new System.ArgumentException($"{target.GetType().Name} has no field \"{field}\"");
        property.objectReferenceValue = value;
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void WireArray(Object target, string field, Object[] values)
    {
        SerializedObject serialized = new(target);
        SerializedProperty property = serialized.FindProperty(field)
            ?? throw new System.ArgumentException($"{target.GetType().Name} has no field \"{field}\"");
        property.arraySize = values.Length;
        for (int i = 0; i < values.Length; i++) property.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }
}
