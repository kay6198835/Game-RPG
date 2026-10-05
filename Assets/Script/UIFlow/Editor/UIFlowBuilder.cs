using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UIFlow.EditorTools.UIKit;

namespace UIFlow.EditorTools
{
    /// <summary>
    /// Tools > UI Flow > Build All: dựng lại toàn bộ scene + prefab + config của luồng UI và cập nhật Build Settings.
    /// Vì sao dựng bằng code thay vì kéo thả tay?
    /// - Hàng trăm tham chiếu [SerializeField] được nối đúng, không sót ô nào.
    /// - Chạy lại bao nhiêu lần cũng ra cùng kết quả; sửa bố cục ở đây rồi chạy lại là xong.
    /// ⚠️ Chạy lại sẽ GHI ĐÈ 4 scene UIFlow. Sửa tay trong scene thì đừng chạy lại (hoặc sửa luôn ở đây).
    /// Không bao giờ đụng tới LoadRandomMap hay scene gameplay nào khác.
    /// </summary>
    public static partial class UIFlowBuilder
    {
        public const string MainGamePlayPath = "Assets/Scenes/Main/MainGamePlay.unity";
        public const string LoadingPath = "Assets/Scenes/Main/Loading.unity";
        public const string GameplayUIPath = "Assets/Scenes/Main/GameplayUI.unity";
        public const string GameplayMockPath = "Assets/Scenes/Test/GameplayMock.unity";
        public const string GameplayRealPath = "Assets/Scenes/Main/Test/LoadRandomMap.unity";
        public const string ConfigPath = "Assets/SO/UIFlow/UIDebugConfig.asset";
        public const string HealthBarPrefabPath = "Assets/Prefab/UIFlow/WorldHealthBar.prefab";
        public const string DamageNumberPrefabPath = "Assets/Prefab/UIFlow/DamageNumber.prefab";

        [MenuItem("Tools/UI Flow/Build All")]
        public static void BuildAllMenu()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            bool exists = File.Exists(MainGamePlayPath) || File.Exists(GameplayUIPath);
            if (exists && !EditorUtility.DisplayDialog("UI Flow",
                    "Dựng lại sẽ GHI ĐÈ 4 scene: MainGamePlay, Loading, GameplayUI, GameplayMock.\n" +
                    "Không đụng tới LoadRandomMap hay scene gameplay khác. Tiếp tục?", "Dựng lại", "Hủy"))
            {
                return;
            }
            BuildAll();
            EditorSceneManager.OpenScene(MainGamePlayPath);
            EditorUtility.DisplayDialog("UI Flow", "Xong. Mở MainGamePlay và bấm Play.", "OK");
        }

        /// <summary>Gọi từ dòng lệnh: Unity.exe -batchmode -quit -executeMethod UIFlow.EditorTools.UIFlowBuilder.BuildAll</summary>
        public static void BuildAll()
        {
            UIDebugConfig config = EnsureConfig();
            WorldHealthBar healthBarPrefab = BuildHealthBarPrefab();
            DamageNumber damageNumberPrefab = BuildDamageNumberPrefab();

            BuildMainGamePlay(config);
            BuildLoading(config);
            BuildGameplayUI(config, damageNumberPrefab);
            BuildGameplayMock(config, healthBarPrefab);
            UpdateBuildSettings();

            AssetDatabase.SaveAssets();
            Debug.Log("[UIFlowBuilder] Đã dựng xong MainGamePlay, Loading, GameplayUI, GameplayMock và cập nhật Build Settings.");
        }

        [MenuItem("Tools/UI Flow/Update Build Settings Only")]
        public static void UpdateBuildSettings()
        {
            // Thứ tự: menu (0) → loading → gameplay thật → UI in-game → gameplay giả, rồi các scene cũ giữ nguyên.
            string[] wanted = { MainGamePlayPath, LoadingPath, GameplayRealPath, GameplayUIPath, GameplayMockPath };
            List<EditorBuildSettingsScene> scenes = new();
            foreach (string path in wanted)
            {
                if (File.Exists(path)) scenes.Add(new EditorBuildSettingsScene(path, true));
                else Debug.LogWarning("[UIFlowBuilder] Không tìm thấy scene: " + path);
            }
            foreach (EditorBuildSettingsScene existing in EditorBuildSettings.scenes)
            {
                if (!scenes.Exists(s => s.path == existing.path)) scenes.Add(existing);
            }
            EditorBuildSettings.scenes = scenes.ToArray();
        }

        private static UIDebugConfig EnsureConfig()
        {
            UIDebugConfig config = AssetDatabase.LoadAssetAtPath<UIDebugConfig>(ConfigPath);
            if (config != null) return config;   // Đã có → giữ nguyên cờ người dùng đã chỉnh
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(ConfigPath));
            config = ScriptableObject.CreateInstance<UIDebugConfig>();
            AssetDatabase.CreateAsset(config, ConfigPath);
            return config;
        }

        private static Scene NewScene() => EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        private static void Save(Scene scene, string path)
        {
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
            EditorSceneManager.SaveScene(scene, path);
        }

        private static void Bootstrap(UIDebugConfig config, bool applySavedSettings)
        {
            GameObject go = new("UIBootstrap");
            UIBootstrap bootstrap = go.AddComponent<UIBootstrap>();
            Wire(bootstrap, ("config", config), ("applySavedSettings", applySavedSettings));
        }

        // ═════════════════════════ MainGamePlay ═════════════════════════

        private static void BuildMainGamePlay(UIDebugConfig config)
        {
            Scene scene = NewScene();
            CameraObject(new Color(0.05f, 0.05f, 0.08f), 5f);
            EventSystemObject();
            Bootstrap(config, applySavedSettings: true);

            Canvas canvas = ScreenCanvas("MenuCanvas", 0);
            UIManager manager = canvas.gameObject.AddComponent<UIManager>();
            Image background = Img(canvas.transform, "Background", new Color(0.07f, 0.08f, 0.12f));
            Stretch(background.gameObject);

            SplashPanel splash = BuildSplash(canvas.transform);
            LoginPanel login = BuildLogin(canvas.transform);
            MainMenuPanel mainMenu = BuildMainMenu(canvas.transform);
            SaveSelectPanel saveSelect = BuildSaveSelect(canvas.transform);
            SettingsPanel settings = BuildSettings(canvas.transform);

            // Panel gốc đặt ở runtime bởi MainMenuFlow (logo → đăng nhập → menu), không gán ở đây.
            Wire(manager, ("rootPanel", null), ("escapeFallbackPanel", null), ("controlsTimeScale", false));

            GameObject flowObject = new("MainMenuFlow");
            MainMenuFlow flow = flowObject.AddComponent<MainMenuFlow>();
            Wire(flow, ("uiManager", manager), ("splashPanel", splash), ("loginPanel", login), ("mainMenuPanel", mainMenu),
                 ("saveSelectPanel", saveSelect), ("settingsPanel", settings));

            Save(scene, MainGamePlayPath);
        }

        private static SplashPanel BuildSplash(Transform canvas)
        {
            SplashPanel panel = Panel<SplashPanel>(canvas, "SplashPanel", new Color(0.02f, 0.02f, 0.04f), closeOnEscape: false);
            GameObject logo = Node("Logo", panel.transform);
            Place(logo, Center, Center, new Vector2(0, 40), new Vector2(1200, 300));
            TextMeshProUGUI title = Txt(logo.transform, "Title", "GAME RPG", 140, TextAlignmentOptions.Center, AccentColor);
            title.fontStyle = FontStyles.Bold;
            Stretch(title.gameObject, 0, 0, 0, 90);
            TextMeshProUGUI subtitle = Txt(logo.transform, "Subtitle", "Dungeon of the Lamb", 40, TextAlignmentOptions.Center, MutedColor);
            Place(subtitle.gameObject, BottomCenter, BottomCenter, Vector2.zero, new Vector2(1200, 70));
            TextMeshProUGUI hint = Txt(panel.transform, "Hint", "Click hoặc nhấn Space để bỏ qua", 24, TextAlignmentOptions.Center, MutedColor);
            Place(hint.gameObject, BottomCenter, BottomCenter, new Vector2(0, 60), new Vector2(800, 40));

            Wire(panel, ("logo", Rect(logo)));
            return panel;
        }

        private static LoginPanel BuildLogin(Transform canvas)
        {
            LoginPanel panel = Panel<LoginPanel>(canvas, "LoginPanel", new Color(0, 0, 0, 0), closeOnEscape: false);
            RectTransform window = Window(panel.transform, "Window", new Vector2(760, 820), "Đăng nhập");

            GameObject body = Node("Body", window);
            Stretch(body, 40, 40, 100, 40);
            VLayout(body, 14);

            TMP_InputField user = Input(body.transform, "UserNameInput", "Tên đăng nhập");
            Size(user, preferredHeight: 60);
            TMP_InputField password = Input(body.transform, "PasswordInput", "Mật khẩu");
            Size(password, preferredHeight: 60);

            TextMeshProUGUI serverLabel = Txt(body.transform, "ServerLabel", "Chọn server", 26, TextAlignmentOptions.Left, MutedColor);
            Size(serverLabel, preferredHeight: 40);
            Transform serverList = ScrollList(body.transform, "ServerList");
            Size(serverList.parent.parent.GetComponent<RectTransform>(), preferredHeight: 300);
            Button rowTemplate = Btn(serverList, "ServerRowTemplate", "Server", 24);
            rowTemplate.GetComponentInChildren<TMP_Text>().alignment = TextAlignmentOptions.MidlineLeft;
            Size(rowTemplate, preferredHeight: 56);

            TextMeshProUGUI status = Txt(body.transform, "StatusText", "", 24, TextAlignmentOptions.Center, MutedColor);
            Size(status, preferredHeight: 60);
            Button loginButton = Btn(body.transform, "LoginButton", "Đăng nhập", 30, AccentColor);
            Size(loginButton, preferredHeight: 70);

            Wire(panel, ("userNameInput", user), ("passwordInput", password), ("serverListContainer", serverList),
                 ("serverRowTemplate", rowTemplate), ("loginButton", loginButton), ("statusText", status));
            return panel;
        }

        private static MainMenuPanel BuildMainMenu(Transform canvas)
        {
            MainMenuPanel panel = Panel<MainMenuPanel>(canvas, "MainMenuPanel", new Color(0, 0, 0, 0), closeOnEscape: false);

            TextMeshProUGUI title = Txt(panel.transform, "Title", "GAME RPG", 110, TextAlignmentOptions.Left, AccentColor);
            title.fontStyle = FontStyles.Bold;
            Place(title.gameObject, TopLeft, TopLeft, new Vector2(160, -120), new Vector2(1000, 150));

            // Cột nút neo vào giữa-trái: màn rộng hay hẹp thì cột vẫn cách mép trái 160 px (đã scale).
            GameObject column = Node("Buttons", panel.transform);
            Place(column, MiddleLeft, MiddleLeft, new Vector2(160, -60), new Vector2(560, 620));
            VLayout(column, 18, null, TextAnchor.UpperLeft);

            Button continueButton = Btn(column.transform, "ContinueButton", "Tiếp tục", 38);
            Size(continueButton, preferredHeight: 110);
            TMP_Text continueLabel = continueButton.GetComponentInChildren<TMP_Text>();
            Stretch(continueLabel.gameObject, 20, 20, 8, 44);
            continueLabel.alignment = TextAlignmentOptions.MidlineLeft;
            TextMeshProUGUI continueDetail = Txt(continueButton.transform, "Detail", "", 22, TextAlignmentOptions.MidlineLeft, new Color(0.15f, 0.12f, 0.05f));
            Stretch(continueDetail.gameObject, 20, 20, 62, 8);

            Button newGame = MenuButton(column.transform, "NewGameButton", "Chơi mới");
            Button loadGame = MenuButton(column.transform, "LoadGameButton", "Tải game");
            Button settings = MenuButton(column.transform, "SettingsButton", "Cài đặt");
            Button quit = MenuButton(column.transform, "QuitButton", "Thoát");

            TextMeshProUGUI version = Txt(panel.transform, "Version", "UI Flow · dữ liệu mock khi UIDebugConfig bật", 20, TextAlignmentOptions.Right, MutedColor);
            Place(version.gameObject, new Vector2(1, 0), new Vector2(1, 0), new Vector2(-30, 20), new Vector2(900, 40));

            Wire(panel, ("continueButton", continueButton), ("continueDetailText", continueDetail), ("newGameButton", newGame),
                 ("loadGameButton", loadGame), ("settingsButton", settings), ("quitButton", quit));
            return panel;
        }

        private static Button MenuButton(Transform parent, string name, string label)
        {
            Button button = Btn(parent, name, label, 32);
            button.GetComponentInChildren<TMP_Text>().alignment = TextAlignmentOptions.MidlineLeft;
            Stretch(button.GetComponentInChildren<TMP_Text>().gameObject, 20, 20, 4, 4);
            Size(button, preferredHeight: 76);
            return button;
        }

        private static SaveSelectPanel BuildSaveSelect(Transform canvas)
        {
            SaveSelectPanel panel = Panel<SaveSelectPanel>(canvas, "SaveSelectPanel", DimColor);
            RectTransform window = Window(panel.transform, "Window", new Vector2(1300, 860), "Chọn file save");

            Transform list = ScrollList(window, "SaveList", 12);
            Stretch(list.parent.parent.gameObject, 40, 40, 100, 150);

            // Dòng mẫu
            Image row = Img(list, "SaveSlotTemplate", new Color(1, 1, 1, 0.07f), Rounded);
            Size(row, preferredHeight: 130);
            SaveSlotView view = row.gameObject.AddComponent<SaveSlotView>();
            TextMeshProUGUI rowTitle = Txt(row.transform, "Title", "Tên", 34);
            Stretch(rowTitle.gameObject, 24, 360, 14, 64);
            TextMeshProUGUI rowDetail = Txt(row.transform, "Detail", "", 22, TextAlignmentOptions.Left, MutedColor);
            Stretch(rowDetail.gameObject, 24, 360, 70, 12);
            Button load = Btn(row.transform, "LoadButton", "Chơi", 28, AccentColor);
            Place(load.gameObject, MiddleRight, MiddleRight, new Vector2(-190, 0), new Vector2(150, 70));
            Button delete = Btn(row.transform, "DeleteButton", "Xóa", 26, new Color(0.7f, 0.25f, 0.25f, 0.8f));
            Place(delete.gameObject, MiddleRight, MiddleRight, new Vector2(-24, 0), new Vector2(140, 70));
            Wire(view, ("titleText", rowTitle), ("detailText", rowDetail), ("loadButton", load), ("deleteButton", delete));

            TextMeshProUGUI empty = Txt(window, "EmptyText", "Chưa có file save nào.", 30, TextAlignmentOptions.Center, MutedColor);
            Place(empty.gameObject, Center, Center, Vector2.zero, new Vector2(800, 60));
            TextMeshProUGUI message = Txt(window, "MessageText", "", 24, TextAlignmentOptions.Left, AccentColor);
            Place(message.gameObject, BottomLeft, BottomLeft, new Vector2(40, 50), new Vector2(900, 60));
            Button back = Btn(window, "BackButton", "Quay lại", 28);
            Place(back.gameObject, new Vector2(1, 0), new Vector2(1, 0), new Vector2(-40, 40), new Vector2(240, 70));

            Wire(panel, ("listContainer", list), ("slotTemplate", view), ("emptyText", empty), ("messageText", message), ("backButton", back));
            return panel;
        }

        /// <summary>Dùng chung cho scene menu và scene GameplayUI.</summary>
        private static SettingsPanel BuildSettings(Transform canvas)
        {
            SettingsPanel panel = Panel<SettingsPanel>(canvas, "SettingsPanel", DimColor);
            RectTransform window = Window(panel.transform, "Window", new Vector2(1300, 880), "Cài đặt");

            GameObject tabs = Node("Tabs", window);
            Place(tabs, TopCenter, TopCenter, new Vector2(0, -100), new Vector2(900, 64));
            HLayout(tabs, 16);
            Button audioTab = Btn(tabs.transform, "AudioTab", "Âm thanh", 28);
            Size(audioTab, 260, 60);
            Button graphicsTab = Btn(tabs.transform, "GraphicsTab", "Đồ họa", 28);
            Size(graphicsTab, 260, 60);
            Button keysTab = Btn(tabs.transform, "KeysTab", "Gán phím", 28);
            Size(keysTab, 260, 60);

            // ── Trang Âm thanh
            GameObject audioPage = Page(window, "AudioPage");
            (Slider master, TMP_Text masterValue) = SliderRow(audioPage.transform, "Master", "Âm lượng tổng");
            (Slider music, TMP_Text musicValue) = SliderRow(audioPage.transform, "Music", "Nhạc nền");
            (Slider sfx, TMP_Text sfxValue) = SliderRow(audioPage.transform, "Sfx", "Hiệu ứng");

            // ── Trang Đồ họa
            GameObject graphicsPage = Page(window, "GraphicsPage");
            TMP_Dropdown resolution = DropdownRow(graphicsPage.transform, "Resolution", "Độ phân giải");
            TMP_Dropdown quality = DropdownRow(graphicsPage.transform, "Quality", "Chất lượng");
            Toggle fullscreen = Tgl(graphicsPage.transform, "FullscreenToggle", "Toàn màn hình", 28);
            Size(fullscreen, preferredHeight: 56);
            Toggle vsync = Tgl(graphicsPage.transform, "VSyncToggle", "VSync", 28);
            Size(vsync, preferredHeight: 56);

            // ── Trang Gán phím
            GameObject keysPage = Node("KeysPage", window);
            Stretch(keysPage, 60, 60, 190, 130);
            Transform keyList = ScrollList(keysPage.transform, "KeyList", 6);
            Stretch(keyList.parent.parent.gameObject, 0, 0, 0, 70);
            GameObject keyRow = Node("KeyRowTemplate", keyList);
            HLayout(keyRow, 16, new RectOffset(16, 16, 0, 0), TextAnchor.MiddleLeft);
            Size(Rect(keyRow), preferredHeight: 56);
            KeyBindingRow keyRowView = keyRow.AddComponent<KeyBindingRow>();
            TextMeshProUGUI actionLabel = Txt(keyRow.transform, "Action", "Hành động", 26);
            Size(actionLabel, preferredHeight: 52, flexibleWidth: 1);
            Button keyButton = Btn(keyRow.transform, "KeyButton", "-", 26);
            Size(keyButton, 240, 50);
            Wire(keyRowView, ("actionLabel", actionLabel), ("keyButton", keyButton), ("keyLabel", keyButton.GetComponentInChildren<TMP_Text>()));
            Button resetKeys = Btn(keysPage.transform, "ResetKeysButton", "Mặc định", 24);
            Place(resetKeys.gameObject, BottomLeft, BottomLeft, Vector2.zero, new Vector2(220, 56));
            TextMeshProUGUI keyMessage = Txt(keysPage.transform, "KeyMessage", "", 22, TextAlignmentOptions.MidlineLeft, AccentColor);
            Place(keyMessage.gameObject, BottomLeft, BottomLeft, new Vector2(240, 0), new Vector2(900, 56));

            Button back = Btn(window, "BackButton", "Xong", 28, AccentColor);
            Place(back.gameObject, BottomCenter, BottomCenter, new Vector2(0, 36), new Vector2(260, 70));

            Wire(panel,
                ("tabButtons", new Object[] { audioTab, graphicsTab, keysTab }),
                ("pages", new Object[] { audioPage, graphicsPage, keysPage }),
                ("masterSlider", master), ("masterValue", masterValue), ("musicSlider", music), ("musicValue", musicValue),
                ("sfxSlider", sfx), ("sfxValue", sfxValue),
                ("resolutionDropdown", resolution), ("qualityDropdown", quality), ("fullscreenToggle", fullscreen), ("vsyncToggle", vsync),
                ("keyListContainer", keyList), ("keyRowTemplate", keyRowView), ("resetKeysButton", resetKeys), ("keyMessageText", keyMessage),
                ("backButton", back));
            return panel;
        }

        private static GameObject Page(Transform window, string name)
        {
            GameObject page = Node(name, window);
            Stretch(page, 100, 100, 200, 130);
            VLayout(page, 24, null, TextAnchor.UpperLeft);
            return page;
        }

        private static (Slider, TMP_Text) SliderRow(Transform parent, string name, string label)
        {
            GameObject row = Node(name + "Row", parent);
            HLayout(row, 20, null, TextAnchor.MiddleLeft);
            Size(Rect(row), preferredHeight: 60);
            Size(Txt(row.transform, "Label", label, 28), 320, 56);
            Slider slider = Sld(row.transform, name + "Slider");
            Size(slider, 560, 30);
            TextMeshProUGUI value = Txt(row.transform, "Value", "100%", 28, TextAlignmentOptions.Right);
            Size(value, 120, 56);
            return (slider, value);
        }

        private static TMP_Dropdown DropdownRow(Transform parent, string name, string label)
        {
            GameObject row = Node(name + "Row", parent);
            HLayout(row, 20, null, TextAnchor.MiddleLeft);
            Size(Rect(row), preferredHeight: 60);
            Size(Txt(row.transform, "Label", label, 28), 320, 56);
            TMP_Dropdown dropdown = Drop(row.transform, name + "Dropdown");
            Size(dropdown, 480, 52);
            return dropdown;
        }

        // ═════════════════════════ Loading ═════════════════════════

        private static void BuildLoading(UIDebugConfig config)
        {
            // Không có Camera / EventSystem: Canvas Overlay tự vẽ không cần camera, và scene đích sẽ mang
            // camera + EventSystem của nó vào → không bị 2 AudioListener / 2 EventSystem lúc chuyển scene.
            Scene scene = NewScene();
            Bootstrap(config, applySavedSettings: false);

            Canvas canvas = ScreenCanvas("LoadingCanvas", 100);
            LoadingScreen loading = canvas.gameObject.AddComponent<LoadingScreen>();

            Image background = Img(canvas.transform, "Background", new Color(0.09f, 0.1f, 0.16f));
            Stretch(background.gameObject);
            background.preserveAspect = false;
            Image shade = Img(canvas.transform, "BottomShade", new Color(0, 0, 0, 0.55f));
            Place(shade.gameObject, BottomCenter, BottomCenter, Vector2.zero, new Vector2(0, 320));
            Rect(shade).anchorMin = new Vector2(0, 0);
            Rect(shade).anchorMax = new Vector2(1, 0);

            TextMeshProUGUI target = Txt(canvas.transform, "TargetText", "Đang tải…", 34, TextAlignmentOptions.Left, AccentColor);
            Place(target.gameObject, BottomLeft, BottomLeft, new Vector2(160, 190), new Vector2(1000, 50));
            TextMeshProUGUI tip = Txt(canvas.transform, "TipText", "", 28, TextAlignmentOptions.Left);
            Place(tip.gameObject, BottomLeft, BottomLeft, new Vector2(160, 110), new Vector2(1400, 70));

            // Thanh tiến trình kéo giãn theo chiều ngang: anchor trái-phải, cách mép 160 px ở mọi độ phân giải.
            Image barBackground = Img(canvas.transform, "ProgressBar", new Color(1, 1, 1, 0.12f), White);
            RectTransform barRect = Rect(barBackground);
            barRect.anchorMin = new Vector2(0, 0);
            barRect.anchorMax = new Vector2(1, 0);
            barRect.pivot = new Vector2(0.5f, 0f);
            barRect.offsetMin = new Vector2(160, 70);
            barRect.offsetMax = new Vector2(-160, 86);
            Image fill = Filled(barBackground.transform, "Fill", AccentColor, Image.FillMethod.Horizontal);
            Stretch(fill.gameObject);
            fill.fillAmount = 0f;
            TextMeshProUGUI percent = Txt(canvas.transform, "PercentText", "0%", 30, TextAlignmentOptions.Right);
            Place(percent.gameObject, new Vector2(1, 0), new Vector2(1, 0), new Vector2(-160, 100), new Vector2(200, 50));

            GameObject error = Node("ErrorGroup", canvas.transform);
            Place(error, Center, Center, Vector2.zero, new Vector2(1100, 260));
            error.AddComponent<Image>().color = WindowColor;
            TextMeshProUGUI errorText = Txt(error.transform, "ErrorText", "", 30, TextAlignmentOptions.Center, new Color(1f, 0.5f, 0.45f));
            Stretch(errorText.gameObject, 30, 30, 30, 110);
            Button back = Btn(error.transform, "BackToMenuButton", "Về menu chính", 28);
            Place(back.gameObject, BottomCenter, BottomCenter, new Vector2(0, 24), new Vector2(320, 70));

            Wire(loading, ("progressFill", fill), ("percentText", percent), ("tipText", tip), ("targetText", target),
                 ("backgroundImage", background), ("errorGroup", error), ("errorText", errorText), ("backToMenuButton", back));

            // EventSystem nằm TRONG ErrorGroup: nhóm lỗi bị tắt lúc bình thường nên EventSystem cũng tắt theo →
            // không trùng với EventSystem của scene đích. Chỉ bật khi hiện lỗi, lúc đó mới cần bấm nút "Về menu".
            GameObject errorEvents = new("EventSystem (chỉ dùng khi lỗi)", typeof(UnityEngine.EventSystems.EventSystem),
                                         typeof(UnityEngine.InputSystem.UI.InputSystemUIInputModule));
            errorEvents.transform.SetParent(error.transform, false);
            error.SetActive(false);

            Save(scene, LoadingPath);
        }
    }
}
