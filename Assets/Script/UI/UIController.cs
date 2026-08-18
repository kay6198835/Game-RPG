using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

/// <summary>
/// Sample UI Toolkit screen manager: clones MainMenu / Settings / PauseMenu into one
/// UIDocument root and toggles which one is visible. Display layer only - it owns no
/// gameplay state.
/// </summary>
public class UIController : MonoBehaviour
{
    private enum ScreenId { MainMenu, Settings, Pause, None }

    [Header("Screen templates (.uxml)")]
    [SerializeField] private VisualTreeAsset _mainMenuAsset;
    [SerializeField] private VisualTreeAsset _settingsAsset;
    [SerializeField] private VisualTreeAsset _pauseAsset;

    [Header("Panel")]
    [SerializeField] private PanelSettings _panelSettings;

    [Header("Config")]
    [SerializeField] private string _gameSceneName = "";   // empty = Play only hides the UI
    [SerializeField] private bool _startInGame;            // true = start hidden, Esc opens pause

    private UIDocument _document;

    // Cached root of each cloned screen.
    private VisualElement _mainMenuRoot;
    private VisualElement _settingsRoot;
    private VisualElement _pauseRoot;

    private ScreenId _current = ScreenId.None;
    private ScreenId _settingsCameFrom = ScreenId.MainMenu;   // where Back returns to

    private void Awake()
    {
        ResolveMissingAssets();

        // Self-wiring so the sample scene needs nothing but this component.
        _document = GetComponent<UIDocument>();
        if (_document == null) _document = gameObject.AddComponent<UIDocument>();
        if (_document.panelSettings == null)
        {
            // AddComponent already ran UIDocument.OnEnable with no panel; the off/on cycle
            // rebuilds the panel now that PanelSettings is assigned.
            _document.enabled = false;
            _document.panelSettings = _panelSettings;
            _document.enabled = true;
        }
    }

    private void Start()
    {
        VisualElement root = _document.rootVisualElement;
        root.Clear();

        _mainMenuRoot = AddScreen(root, _mainMenuAsset);
        _settingsRoot = AddScreen(root, _settingsAsset);
        _pauseRoot    = AddScreen(root, _pauseAsset);

        BindMainMenu();
        BindSettings();
        BindPause();

        ShowScreen(_startInGame ? ScreenId.None : ScreenId.MainMenu);
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;   // never leave the game frozen
    }

    private void Update()
    {
        // Esc toggles pause while in game. Settings screen ignores it (use Back).
        if (Keyboard.current == null || !Keyboard.current.escapeKey.wasPressedThisFrame) return;

        if (_current == ScreenId.None) ShowScreen(ScreenId.Pause);
        else if (_current == ScreenId.Pause) Resume();
    }

    // ---------- screen switching ----------

    /// <summary>Clones one .uxml as a full-screen overlay child of the document root.</summary>
    private static VisualElement AddScreen(VisualElement root, VisualTreeAsset asset)
    {
        VisualElement screen = asset.Instantiate();
        // TemplateContainer does not fill its parent by default.
        screen.style.position = Position.Absolute;
        screen.StretchToParentSize();
        root.Add(screen);
        return screen;
    }

    /// <summary>Shows exactly one screen (or none) and syncs the pause timescale.</summary>
    private void ShowScreen(ScreenId screen)
    {
        _current = screen;
        SetVisible(_mainMenuRoot, screen == ScreenId.MainMenu);
        SetVisible(_settingsRoot, screen == ScreenId.Settings);
        SetVisible(_pauseRoot,    screen == ScreenId.Pause);

        Time.timeScale = screen == ScreenId.None ? 1f : 0f;
    }

    private static void SetVisible(VisualElement element, bool visible)
    {
        element.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
    }

    // ---------- button wiring ----------

    private void BindMainMenu()
    {
        _mainMenuRoot.Q<Button>("btn-play").clicked += StartGame;
        _mainMenuRoot.Q<Button>("btn-settings").clicked += () => OpenSettings(ScreenId.MainMenu);
        _mainMenuRoot.Q<Button>("btn-quit").clicked += Quit;
    }

    private void BindSettings()
    {
        Slider volume = _settingsRoot.Q<Slider>("slider-volume");
        Label volumeLabel = _settingsRoot.Q<Label>("label-volume");
        Toggle fullscreen = _settingsRoot.Q<Toggle>("toggle-fullscreen");

        volume.value = AudioListener.volume;
        volumeLabel.text = Mathf.RoundToInt(volume.value * 100f) + "%";
        fullscreen.value = Screen.fullScreen;

        volume.RegisterValueChangedCallback(evt =>
        {
            AudioListener.volume = evt.newValue;
            volumeLabel.text = Mathf.RoundToInt(evt.newValue * 100f) + "%";
        });

        fullscreen.RegisterValueChangedCallback(evt => Screen.fullScreen = evt.newValue);

        _settingsRoot.Q<Button>("btn-back").clicked += () => ShowScreen(_settingsCameFrom);
    }

    private void BindPause()
    {
        _pauseRoot.Q<Button>("btn-resume").clicked += Resume;
        _pauseRoot.Q<Button>("btn-settings").clicked += () => OpenSettings(ScreenId.Pause);
        _pauseRoot.Q<Button>("btn-mainmenu").clicked += () => ShowScreen(ScreenId.MainMenu);
    }

    // ---------- actions ----------

    private void OpenSettings(ScreenId from)
    {
        _settingsCameFrom = from;
        ShowScreen(ScreenId.Settings);
    }

    private void Resume() => ShowScreen(ScreenId.None);

    private void StartGame()
    {
        ShowScreen(ScreenId.None);
        if (!string.IsNullOrEmpty(_gameSceneName)) SceneManager.LoadScene(_gameSceneName);
    }

    private void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    /// <summary>
    /// Editor-only safety net: if an Inspector slot is empty the asset is pulled from its
    /// known path so the sample scene always plays. Assign the slots for a real build.
    /// </summary>
    private void ResolveMissingAssets()
    {
#if UNITY_EDITOR
        const string screens = "Assets/UI/Screens/";
        if (_mainMenuAsset == null) _mainMenuAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(screens + "MainMenu.uxml");
        if (_settingsAsset == null) _settingsAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(screens + "Settings.uxml");
        if (_pauseAsset == null)    _pauseAsset    = UnityEditor.AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(screens + "PauseMenu.uxml");
        if (_panelSettings == null) _panelSettings = UnityEditor.AssetDatabase.LoadAssetAtPath<PanelSettings>("Assets/UI Toolkit/PanelSettings.asset");
#endif
    }
}
