using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using VContainer;

/// <summary>
/// UI Toolkit port of the UGUI stats panel (Stats_UI_Controller.prefab in LoadRandomMap).
/// Display layer only: it reads StatsSO.statViewDTOs and emits intent events. Every write
/// into StatsSO is done by StatPointAllocator (see .claude/rules/ui-code.md).
/// </summary>
public class StatsScreenUIController : MonoBehaviour
{
    /// <summary>Per-row view state. SessionGain is a click counter, not gameplay state.</summary>
    private class StatRow
    {
        public VisualElement Container;
        public Label Name;
        public Label Final;
        public Label Bonus;
        public Button Increase;
        public Button Decrease;
        public int SessionGain;
    }

    [Header("Screen templates (.uxml)")]
    [SerializeField] private VisualTreeAsset _statsScreenAsset;
    [SerializeField] private VisualTreeAsset _openBarAsset;
    [SerializeField] private VisualTreeAsset _primaryRowAsset;
    [SerializeField] private VisualTreeAsset _derivedRowAsset;

    [Header("Panel")]
    [SerializeField] private PanelSettings _panelSettings;
    [SerializeField] private int _sortingOrder = 10;   // above the menu UIDocument in UISample

    // Injected by GameLifetimeScope, not serialized: the screen reads stats through the
    // service and never holds the StatsSO asset.
    private IPlayerStatService _playerStatService;

    private UIDocument _document;

    private VisualElement _statsRoot;
    private VisualElement _openBarRoot;
    private Label _pointsLabel;
    private Button _updateButton;
    private Button _revertButton;
    private Button _restoreButton;

    private readonly Dictionary<StatType, StatRow> _rows = new();

    private int _pointsAtOpen;
    private int _pointsRemaining;
    private bool _isOpen;
    [Inject]
    public void Construct(IPlayerStatService playerStatService)
    {
        _playerStatService = playerStatService;
    }
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
        _document.sortingOrder = _sortingOrder;
    }

    private void OnEnable()
    {
        EventManager.Resgister(EventID.ON_CHANGE_STATS_BY_UI_RUN_TIME, OnPointsChanged);
        EventManager.Resgister(EventID.ON_RESET_STATS_UI_SESSION, OnSessionReset);
    }

    private void OnDisable()
    {
        EventManager.UnResgister(EventID.ON_CHANGE_STATS_BY_UI_RUN_TIME, OnPointsChanged);
        EventManager.UnResgister(EventID.ON_RESET_STATS_UI_SESSION, OnSessionReset);
    }

    private void Start()
    {
        VisualElement root = _document.rootVisualElement;
        root.Clear();

        _statsRoot = AddScreen(root, _statsScreenAsset);
        _openBarRoot = AddScreen(root, _openBarAsset);

        _pointsLabel = _statsRoot.Q<Label>("label-points");
        _updateButton = _statsRoot.Q<Button>("btn-update");
        _revertButton = _statsRoot.Q<Button>("btn-revert");
        _restoreButton = _statsRoot.Q<Button>("btn-restore");

        _statsRoot.Q<Button>("btn-close").clicked += CloseStats;
        _openBarRoot.Q<Button>("btn-open-stats").clicked += OpenStats;
        _updateButton.clicked += () => { EventManager.Emit(EventID.ON_UPDATE_STATS_BY_UI); CloseStats(); };
        _revertButton.clicked += () => EventManager.Emit(EventID.ON_REVERT_STATS_BY_UI);
        _restoreButton.clicked += () => EventManager.Emit(EventID.ON_RESTORE_STATS_BY_UI);

        BuildRows();
        CloseStats();
    }

    private void Update()
    {
        if (Keyboard.current == null) return;   // null when activeInputHandler is legacy-only
        // C, not Esc: UIController already owns Esc for pause, and both would fire the same frame.
        if (!Keyboard.current.cKey.wasPressedThisFrame) return;
        if (_isOpen) CloseStats(); else OpenStats();
    }

    // ---------- construction ----------

    private static VisualElement AddScreen(VisualElement root, VisualTreeAsset asset)
    {
        VisualElement screen = asset.Instantiate();
        // TemplateContainer does not fill its parent by default.
        screen.style.position = Position.Absolute;
        screen.StretchToParentSize();
        root.Add(screen);
        return screen;
    }

    private void BuildRows()
    {
        if (_playerStatService == null) return;

        ScrollView primaryList = _statsRoot.Q<ScrollView>("list-primary");
        ScrollView derivedList = _statsRoot.Q<ScrollView>("list-derived");

        // Enum order, not dictionary order, so the panel reads the same every run.
        foreach (StatType type in Enum.GetValues(typeof(StatType)))
        {
            bool isPrimary = type.IsPrimary();
            StatRow row = MakeRow(type, isPrimary);
            (isPrimary ? primaryList : derivedList).Add(row.Container);
            _rows[type] = row;
        }
        RefreshValues();
    }

    private StatRow MakeRow(StatType type, bool isPrimary)
    {
        VisualElement container = (isPrimary ? _primaryRowAsset : _derivedRowAsset).Instantiate();
        container.style.flexShrink = 0;   // TemplateContainer would otherwise collapse in a ScrollView

        StatRow row = new StatRow
        {
            Container = container,
            Name = container.Q<Label>("label-name"),
            Final = container.Q<Label>("label-final"),
            Bonus = container.Q<Label>("label-bonus"),
        };

        if (isPrimary)
        {
            row.Increase = container.Q<Button>("btn-increase");
            row.Decrease = container.Q<Button>("btn-decrease");
            row.Increase.clicked += () =>
            {
                row.SessionGain++;
                EventManager.Emit(EventID.ON_INCREASE_STATS_BY_UI, type);
            };
            row.Decrease.clicked += () =>
            {
                if (row.SessionGain <= 0) return;
                row.SessionGain--;
                EventManager.Emit(EventID.ON_DECREASE_STATS_BY_UI, type);
            };
        }

        row.Name.text = GameConstants.StatTypeName.TryGetValue(type, out string label) ? label : type.ToString();
        return row;
    }

    // ---------- event handlers ----------

    private void OnPointsChanged(object obj)
    {
        _pointsRemaining = obj is int remaining ? remaining : 0;
        RefreshValues();
    }

    private void OnSessionReset(object obj = null)
    {
        foreach (StatRow row in _rows.Values) row.SessionGain = 0;
        if (_playerStatService != null) _pointsAtOpen = _playerStatService.GetLevelUpStatsBonus();
    }

    // ---------- view ----------

    private void RefreshValues()
    {
        if (_playerStatService == null || _pointsLabel == null) return;   // event can fire before Start built the tree

        // Visibility is decided by the pool the session opened with, never by the live
        // remaining count: a button that vanishes the moment it stops being usable reflows
        // the panel on every click, and hiding Decrease at zero remaining would strand the
        // player with no way to undo. The per-click conditions dim instead.
        bool hasPointsThisSession = _pointsAtOpen > 0;

        foreach (var (type, row) in _rows)
        {
            if (!_playerStatService.GetFullViewStats().TryGetValue(type, out StatsViewDTO dto)) continue;
            row.Final.text = dto.FinalValue.ToString("0.##");
            row.Bonus.text = "(+" + dto.EquipmentValue.ToString("0.##") + ")";

            if (row.Increase == null) continue;
            SetVisible(row.Increase, hasPointsThisSession);
            SetVisible(row.Decrease, hasPointsThisSession);
            row.Increase.SetEnabled(_pointsRemaining > 0);
            row.Decrease.SetEnabled(row.SessionGain > 0);
        }

        _pointsLabel.text = "Points: " + _pointsRemaining;
        SetVisible(_restoreButton, _playerStatService.GetLevel() > 1);
        SetVisible(_revertButton, _pointsAtOpen > _pointsRemaining);
        SetVisible(_updateButton, _pointsAtOpen != _pointsRemaining);
    }

    private void OpenStats()
    {
        _isOpen = true;
        SetVisible(_statsRoot, true);
        SetVisible(_openBarRoot, false);
        if (_playerStatService != null)
        {
            _pointsAtOpen = _playerStatService.GetLevelUpStatsBonus();
            _pointsRemaining = _pointsAtOpen;
        }
        foreach (StatRow row in _rows.Values) row.SessionGain = 0;
        EventManager.Emit(EventID.ON_OPEN_STATS_PLAYER_UI);
        RefreshValues();
    }

    private void CloseStats()
    {
        // Uncommitted points are handed back, matching the UGUI panel's close behaviour.
        if (_isOpen && _pointsAtOpen != _pointsRemaining) EventManager.Emit(EventID.ON_REVERT_STATS_BY_UI);
        _isOpen = false;
        SetVisible(_statsRoot, false);
        SetVisible(_openBarRoot, true);
        EventManager.Emit(EventID.ON_CLOSE_STATS_PLAYER_UI);
    }

    private static void SetVisible(VisualElement element, bool visible)
    {
        if (element == null) return;
        element.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
    }

    /// <summary>
    /// Editor-only safety net: if an Inspector slot is empty the asset is pulled from its
    /// known path so the sample scene always plays. Assign the slots for a real build.
    /// </summary>
    private void ResolveMissingAssets()
    {
#if UNITY_EDITOR
        const string screens = "Assets/UI/Screens/";
        if (_statsScreenAsset == null) _statsScreenAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(screens + "StatsScreen.uxml");
        if (_openBarAsset == null) _openBarAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(screens + "StatsOpenBar.uxml");
        if (_primaryRowAsset == null) _primaryRowAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(screens + "StatRowPrimary.uxml");
        if (_derivedRowAsset == null) _derivedRowAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(screens + "StatRowDerived.uxml");
        if (_panelSettings == null) _panelSettings = UnityEditor.AssetDatabase.LoadAssetAtPath<PanelSettings>("Assets/UI Toolkit/PanelSettings.asset");
#endif
    }
}
