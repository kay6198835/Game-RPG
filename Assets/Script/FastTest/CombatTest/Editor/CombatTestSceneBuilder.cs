using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// Tools > Combat Test > Build Scene: builds Assets/Scenes/Test/CombatTest.unity from code, so every
// [SerializeField] reference is wired the same way on every run. Re-running OVERWRITES the scene.
public static class CombatTestSceneBuilder
{
    public const string ScenePath = "Assets/Scenes/Test/CombatTest.unity";
    private const string PlayerPrefabPath = "Assets/Prefab/Player/PlayerTest.prefab";
    private const string MonumentPathFormat = "Assets/Prefab/Champion Monument/Champion_Monument_{0}.prefab";
    private const string PlayerDataPathFormat = "Assets/SO/Player/{0}/{0}.asset";
    private const string UIDebugConfigPath = "Assets/SO/UIFlow/UIDebugConfig.asset";
    private static readonly string[] Classes = { "Paladin", "Archer", "Mage" };

    [MenuItem("Tools/Combat Test/Build Scene")]
    public static void BuildSceneMenu()
    {
        if (File.Exists(ScenePath) && !EditorUtility.DisplayDialog("Combat Test",
                $"{ScenePath} already exists. Rebuild and overwrite it?", "Overwrite", "Cancel"))
            return;
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

        Build();
        EditorSceneManager.OpenScene(ScenePath);
    }

    [MenuItem("Tools/Combat Test/Open Scene")]
    public static void OpenSceneMenu()
    {
        if (!File.Exists(ScenePath))
        {
            EditorUtility.DisplayDialog("Combat Test", $"{ScenePath} does not exist yet. Run Tools > Combat Test > Build Scene first.", "OK");
            return;
        }
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        EditorSceneManager.OpenScene(ScenePath);
    }

    public static void Build()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        Sprite square = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");

        CreateCamera();

        GameObject uiBootstrap = new("UIBootstrap");
        UIFlow.UIBootstrap bootstrapUI = uiBootstrap.AddComponent<UIFlow.UIBootstrap>();
        Wire(bootstrapUI, "config", AssetDatabase.LoadAssetAtPath<Object>(UIDebugConfigPath));

        new GameObject("CombatTestLifetimeScope").AddComponent<CombatTestLifetimeScope>();
        new GameObject("ObjectPooling").AddComponent<ObjectPoolManager>();

        GameObject spawnPoint = new("PlayerSpawnPoint");
        GameObject playerManagerObject = new("PlayerManager");
        PlayerManager playerManager = playerManagerObject.AddComponent<PlayerManager>();
        Wire(playerManager, "playerPrefab", Require<Player>(PlayerPrefabPath));
        Wire(playerManager, "spawnPoint", spawnPoint.transform);

        CreateFloor(square);

        PlayerData[] classData = new PlayerData[Classes.Length];
        for (int i = 0; i < Classes.Length; i++)
        {
            classData[i] = AssetDatabase.LoadAssetAtPath<PlayerData>(string.Format(PlayerDataPathFormat, Classes[i]));
            GameObject monumentPrefab = Require<GameObject>(string.Format(MonumentPathFormat, Classes[i]));
            GameObject monument = (GameObject)PrefabUtility.InstantiatePrefab(monumentPrefab);
            monument.transform.position = new Vector3((i - 1) * 3f, 3.5f, 0f);
        }

        GameObject bootstrapObject = new("CombatTestBootstrap");
        CombatTestBootstrap bootstrap = bootstrapObject.AddComponent<CombatTestBootstrap>();
        Wire(bootstrap, "playerManager", playerManager);
        WireArray(bootstrap, "classes", classData);

        CreateDummy(square, "TrainingDummy", new Vector3(4f, 0f, 0f), 0f, new Color(0.85f, 0.7f, 0.45f));
        CreateDummy(square, "TrainingDummy_Armored", new Vector3(4f, -3f, 0f), 20f, new Color(0.55f, 0.6f, 0.7f));

        GameObject debugger = new("AttackFlowDebugger");
        debugger.AddComponent<AttackFlowDebugger>();
        debugger.SetActive(false);

        Directory.CreateDirectory(Path.GetDirectoryName(ScenePath));
        EditorSceneManager.SaveScene(scene, ScenePath);
        Debug.Log($"[CombatTest] Built {ScenePath}. Press Play, walk to a monument, press G to pick a class (or F1/F2/F3).");
    }

    private static void CreateCamera()
    {
        GameObject cameraObject = new("Main Camera") { tag = "MainCamera" };
        cameraObject.transform.position = new Vector3(0f, 0f, -10f);
        Camera camera = cameraObject.AddComponent<Camera>();
        camera.orthographic = true;
        camera.orthographicSize = 6f;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.19f, 0.3f, 0.47f);
        camera.nearClipPlane = 0.01f;
        cameraObject.AddComponent<AudioListener>();
    }

    private static void CreateFloor(Sprite square)
    {
        GameObject floor = new("Floor");
        floor.transform.localScale = new Vector3(24f, 16f, 1f);
        SpriteRenderer renderer = floor.AddComponent<SpriteRenderer>();
        renderer.sprite = square;
        renderer.color = new Color(0.25f, 0.27f, 0.25f);
        renderer.sortingOrder = -100;
    }

    private static void CreateDummy(Sprite square, string name, Vector3 position, float defense, Color color)
    {
        GameObject dummy = new(name) { layer = LayerMask.NameToLayer("Enemy") };
        dummy.transform.position = position;
        SpriteRenderer renderer = dummy.AddComponent<SpriteRenderer>();
        renderer.sprite = square;
        renderer.color = color;
        dummy.transform.localScale = new Vector3(1f, 1.6f, 1f);
        dummy.AddComponent<BoxCollider2D>();

        GameObject labelObject = new("Label");
        labelObject.transform.SetParent(dummy.transform, false);
        labelObject.transform.localPosition = new Vector3(0f, 0.9f, 0f);
        labelObject.transform.localScale = new Vector3(1f, 1f / 1.6f, 1f);
        TextMeshPro label = labelObject.AddComponent<TextMeshPro>();
        label.fontSize = 2.5f;
        label.alignment = TextAlignmentOptions.Center;
        label.rectTransform.sizeDelta = new Vector2(6f, 1.5f);
        label.sortingOrder = 50;

        TrainingDummy component = dummy.AddComponent<TrainingDummy>();
        Wire(component, "defense", defense);
        Wire(component, "spriteRenderer", renderer);
        Wire(component, "label", label);
    }

    private static T Require<T>(string path) where T : Object
    {
        T asset = AssetDatabase.LoadAssetAtPath<T>(path);
        if (asset == null) throw new FileNotFoundException($"[CombatTest] Missing {typeof(T).Name} at {path}");
        return asset;
    }

    private static void Wire(Object target, string field, object value)
    {
        SerializedObject serialized = new(target);
        SerializedProperty property = serialized.FindProperty(field)
            ?? throw new System.ArgumentException($"{target.GetType().Name} has no field \"{field}\"");
        switch (value)
        {
            case float f: property.floatValue = f; break;
            case Object o: property.objectReferenceValue = o; break;
            case null: property.objectReferenceValue = null; break;
        }
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void WireArray(Object target, string field, Object[] values)
    {
        SerializedObject serialized = new(target);
        SerializedProperty property = serialized.FindProperty(field);
        property.arraySize = values.Length;
        for (int i = 0; i < values.Length; i++) property.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }
}
