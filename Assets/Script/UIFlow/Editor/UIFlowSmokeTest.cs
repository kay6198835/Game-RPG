using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace UIFlow.EditorTools
{
    /// <summary>
    /// Smoke test tự động cho luồng UI: tự vào Play Mode ở MainGamePlay rồi đi hết luồng như người chơi,
    /// mỗi bước chờ điều kiện đúng (có giới hạn thời gian). Mọi Debug.LogError / exception trong lúc chạy đều tính là FAIL.
    /// Chạy: Tools > UI Flow > Run Smoke Test, hoặc dòng lệnh:
    ///   Unity.exe -batchmode -nographics -projectPath . -executeMethod UIFlow.EditorTools.UIFlowSmokeTest.RunBatch -logFile smoke.log
    /// Test chỉ dùng API public của UI (bấm nút, gọi UIEvents) — không sửa gì trong gameplay.
    /// </summary>
    [InitializeOnLoad]
    public static class UIFlowSmokeTest
    {
        private const string ActiveKey = "UIFlowSmoke.Active";
        private const string BatchKey = "UIFlowSmoke.Batch";
        private const float StepTimeout = 20f;

        private class Step
        {
            public string name;
            public Action action;
            public Func<bool> until;
            // Bước chạy code gameplay cũ (LoadRandomMap): lỗi của gameplay chỉ ghi nhận, không tính FAIL cho UI.
            public bool tolerateGameplayErrors;
        }

        private static readonly List<string> _gameplayErrors = new();
        private static bool[] _savedFlags;

        private static List<Step> _steps;
        private static int _index;
        private static bool _actionDone;
        private static double _stepStart;
        private static readonly List<string> _errors = new();
        private static readonly List<string> _passed = new();
        private static string _newCharacterName;

        // Chạy lại sau mỗi lần domain reload (vào Play Mode làm reload) → gắn lại vòng lặp nếu test đang dở.
        static UIFlowSmokeTest()
        {
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        [MenuItem("Tools/UI Flow/Run Smoke Test")]
        public static void Run() => Start(batch: false);

        public static void RunBatch() => Start(batch: true);

        private static void Start(bool batch)
        {
            SessionState.SetBool(ActiveKey, true);
            SessionState.SetBool(BatchKey, batch);
            EditorSceneManager.OpenScene(UIFlowBuilder.MainGamePlayPath);
            EditorApplication.isPlaying = true;
        }

        private static void OnPlayModeChanged(PlayModeStateChange state)
        {
            if (!SessionState.GetBool(ActiveKey, false)) return;

            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                _steps = BuildSteps();
                _index = 0;
                _actionDone = false;
                _stepStart = EditorApplication.timeSinceStartup;
                _errors.Clear();
                _passed.Clear();
                Application.logMessageReceived += OnLog;
                EditorApplication.update += Tick;
            }
            else if (state == PlayModeStateChange.EnteredEditMode)
            {
                bool batch = SessionState.GetBool(BatchKey, false);
                SessionState.SetBool(ActiveKey, false);
                if (batch) EditorApplication.Exit(SessionState.GetInt("UIFlowSmoke.Exit", 1));
            }
        }

        private static void OnLog(string message, string stackTrace, LogType type)
        {
            if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
            {
                bool tolerated = _index < _steps.Count && _steps[_index].tolerateGameplayErrors && !stackTrace.Contains("UIFlow.");
                if (tolerated)
                {
                    _gameplayErrors.Add($"{type}: {message}");
                    return;
                }
                _errors.Add($"[{(_index < _steps.Count ? _steps[_index].name : "?")}] {type}: {message}\n{stackTrace}");
            }
        }

        private static void Tick()
        {
            if (!EditorApplication.isPlaying) return;

            if (_index >= _steps.Count || _errors.Count > 0)
            {
                Finish();
                return;
            }

            Step step = _steps[_index];
            try
            {
                if (!_actionDone)
                {
                    step.action?.Invoke();
                    _actionDone = true;
                }
                if (step.until == null || step.until())
                {
                    _passed.Add(step.name);
                    Debug.Log($"[UIFlowSmoke] PASS {_passed.Count:00}: {step.name}");
                    _index++;
                    _actionDone = false;
                    _stepStart = EditorApplication.timeSinceStartup;
                    return;
                }
            }
            catch (Exception e)
            {
                _errors.Add($"[{step.name}] {e}");
                return;
            }

            if (EditorApplication.timeSinceStartup - _stepStart > StepTimeout)
            {
                _errors.Add($"[{step.name}] Hết {StepTimeout}s mà điều kiện chưa đúng.");
            }
        }

        private static void Finish()
        {
            EditorApplication.update -= Tick;
            Application.logMessageReceived -= OnLog;
            RestoreFlags();
            if (_gameplayErrors.Count > 0)
            {
                Debug.LogWarning($"[UIFlowSmoke] {_gameplayErrors.Count} lỗi từ gameplay cũ (không tính cho UI), mẫu:\n" +
                                 string.Join("\n", _gameplayErrors.GetRange(0, Math.Min(10, _gameplayErrors.Count))));
            }
            bool ok = _errors.Count == 0;
            if (ok)
            {
                Debug.Log($"[UIFlowSmoke] RESULT: PASS ({_passed.Count}/{_steps.Count} bước)");
            }
            else
            {
                // Log bằng Warning để chính dòng báo lỗi không bị OnLog bắt lại.
                Debug.LogWarning($"[UIFlowSmoke] RESULT: FAIL sau {_passed.Count}/{_steps.Count} bước\n" + string.Join("\n---\n", _errors));
            }
            SessionState.SetInt("UIFlowSmoke.Exit", ok ? 0 : 1);
            EditorApplication.isPlaying = false;
        }

        // ───────────────────────── Kịch bản ─────────────────────────

        private static List<Step> BuildSteps()
        {
            return new List<Step>
            {
                S("Logo tự chuyển sang menu chính (skipLogin bật)", null,
                  () => Active(SceneNames.MainGamePlay) && Visible<MainMenuPanel>()),
                S("Có save mock → nút Tiếp tục hiện và được làm nổi bật", null,
                  () => Find("ContinueButton").activeInHierarchy && Find("ContinueButton").transform.localScale.x > 1.01f),
                S("Bấm Tiếp tục → qua Loading → vào GameplayMock + gắn GameplayUI", () => Click("ContinueButton"),
                  () => Active(SceneNames.GameplayMock) && SceneManager.GetSceneByName(SceneNames.GameplayUI).isLoaded && Visible<HUDPanel>()),
                S("HUD hiện đúng nhân vật của save mới nhất (Aria)", null,
                  () => UIServices.Player.GetStats().characterName == "Aria"),
                S("Hotbar dựng đủ 5 ô kỹ năng", () => UIEvents.UseSkill(1),
                  () => Find("SkillHotbar").transform.childCount == 6),   // 5 ô + 1 ô mẫu đang tắt
                S("Mở túi đồ → 24 ô, game dừng (timeScale 0)", () => Object.FindObjectOfType<GameplayUIController>().ToggleTab(0),
                  () => Visible<TabWindow>() && Time.timeScale == 0f && CountActive<InventorySlot>() == 24),
                S("Kéo thả (SwapSlots) đổi chỗ ô 0 ↔ ô 10", () => UIServices.Inventory.SwapSlots(0, 10),
                  () => UIServices.Inventory.Items[10] != null && UIServices.Inventory.Items[10].id == "sword_iron" && UIServices.Inventory.Items[0] == null),
                S("Chuột phải mặc Kiếm thánh → Kiếm gỉ quay về túi", () => UIServices.Inventory.Equip(1),
                  () => UIServices.Inventory.GetEquipped(ItemSlotType.Weapon).id == "sword_holy" && UIServices.Inventory.Items[1].id == "sword_rusty"),
                S("Tooltip so sánh chỉ số hiện nội dung", () => Object.FindObjectOfType<ItemTooltip>(true).Show(UIServices.Inventory.Items[1], UIServices.Inventory.GetEquipped(ItemSlotType.Weapon), true),
                  () => Object.FindObjectOfType<ItemTooltip>(true).transform.Find("Body").GetComponent<TMP_Text>().text.Contains("(-")),
                S("Q/E đổi tab: sang tab Kỹ năng", () => Object.FindObjectOfType<TabWindow>().SelectTab(2),
                  () => Object.FindObjectOfType<TabWindow>().CurrentTab == 2 && Find("SkillPage").activeInHierarchy),
                S("Esc (Back) đóng cửa sổ → về HUD, game chạy lại", () => Manager().HandleEscape(),
                  () => !Visible<TabWindow>() && Manager().IsAtRoot && Time.timeScale == 1f),
                S("Esc ở HUD → mở menu tạm dừng, game dừng", () => Manager().HandleEscape(),
                  () => Visible<PausePanel>() && Time.timeScale == 0f),
                S("Lưu game từ menu tạm dừng", () => Click("SaveButton"), () => true),
                S("Mở Cài đặt từ menu tạm dừng", () => Click("SettingsButton", inScene: SceneNames.GameplayUI), () => Visible<SettingsPanel>()),
                S("Esc ×2 → đóng Cài đặt rồi đóng Tạm dừng", () => { Manager().HandleEscape(); Manager().HandleEscape(); },
                  () => Manager().IsAtRoot && Time.timeScale == 1f),
                S("NPC nói chuyện → hộp thoại hiện", () => UIEvents.RequestDialogue(MockCatalog.CreateNpcDialogue(), UIEvents.RequestShop),
                  () => Visible<DialoguePanel>()),
                S("Click tua nhanh + qua hết 4 câu → tự mở cửa hàng", () => { for (int i = 0; i < 8; i++) Click("Box"); },
                  () => !Visible<DialoguePanel>() && Visible<ShopPanel>()),
                S("Mua Bình máu → trừ 25 vàng", () => UIServices.Inventory.Buy(UIServices.Inventory.GetShopStock()[0], out _),
                  () => UIServices.Inventory.Gold == 475),
                S("Đóng cửa hàng", () => Manager().CloseAll(), () => Manager().IsAtRoot && Time.timeScale == 1f),
                S("Số damage world-space bay lên", () => UIEvents.ShowDamage(Vector3.zero, 42f, true),
                  () => CountActive<DamageNumber>() > 0),
                S("Hết máu → màn Game Over, Esc không đóng được", () =>
                  {
                      ((MockPlayerDataProvider)UIServices.Player).DebugChangeHP(-99999f);
                      Manager().HandleEscape();
                  },
                  () => Visible<GameOverPanel>()),
                S("Hồi sinh → đầy máu, về HUD", () => Click("RespawnButton"),
                  () => !Visible<GameOverPanel>() && UIServices.Player.GetStats().currentHP == UIServices.Player.GetStats().maxHP),
                S("Về menu chính qua Loading (bỏ qua logo lần 2)", SceneFlow.ReturnToMainMenu,
                  () => Active(SceneNames.MainGamePlay) && Visible<MainMenuPanel>() && Time.timeScale == 1f),
                S("Chơi mới → màn tạo nhân vật", () => Click("NewGameButton"), () => Visible<CharacterCreationPanel>()),
                S("Tên quá ngắn bị từ chối", () => { Input("NameInput").text = "A"; Click("ConfirmButton"); },
                  () => Visible<CharacterCreationPanel>() && Find("ErrorText").GetComponent<TMP_Text>().text.Length > 0),
                S("Nút Ngẫu nhiên + xoay preview", () => { Click("RandomButton"); Click("RotateRight"); },
                  () => Find("DirectionText").GetComponent<TMP_Text>().text.StartsWith("Hướng")),
                S("Tạo nhân vật hợp lệ → vào game với đúng tên", () =>
                  {
                      _newCharacterName = "Kay";
                      Input("NameInput").text = _newCharacterName;
                      Click("ConfirmButton");
                  },
                  () => Active(SceneNames.GameplayMock) && Visible<HUDPanel>() && UIServices.Player.GetStats().characterName == _newCharacterName),
                S("Về menu → Tải game liệt kê 4 save", () => SceneFlow.ReturnToMainMenu(),
                  () => Active(SceneNames.MainGamePlay) && Visible<MainMenuPanel>()),
                S("Mở màn chọn save", () => Click("LoadGameButton"),
                  () => Visible<SaveSelectPanel>() && CountActive<SaveSlotView>() == 4),
                S("Esc → quay lại menu chính", () => Manager().HandleEscape(), () => Visible<MainMenuPanel>() && Manager().IsAtRoot),

                // ── Tắt hết cờ: UI phải chạy với bản Real mà không sửa code UI
                S("Tắt hết cờ → provider chuyển sang bản Real", () => SetFlags(false, false, false, false, true),
                  () => UIServices.Player is RealPlayerDataProvider && UIServices.Save is RealSaveProvider && UIServices.Login is RealLoginService),
                S("Đăng nhập Real (Offline) → vào menu chính", () =>
                  {
                      Manager().SetRoot(Object.FindObjectOfType<LoginPanel>());
                      Click("LoginButton");
                  },
                  () => Visible<MainMenuPanel>()),
                G("skipGameplayInit tắt + bypass bật → load LoadRandomMap thật, KHÔNG gắn GameplayUI", SceneFlow.EnterGameplay,
                  () => Active(SceneNames.GameplayReal) && !SceneManager.GetSceneByName(SceneNames.GameplayUI).IsValid()),
                G("Từ gameplay thật về menu", SceneFlow.ReturnToMainMenu, () => Active(SceneNames.MainGamePlay) && Visible<MainMenuPanel>()),
                G("bypass tắt → LoadRandomMap thật + gắn GameplayUI (HUD hiện)", () =>
                  {
                      UIServices.Config.bypassLoadRandomLogic = false;
                      SceneFlow.EnterGameplay();
                  },
                  () => Active(SceneNames.GameplayReal) && SceneManager.GetSceneByName(SceneNames.GameplayUI).isLoaded && Visible<HUDPanel>()),
                G("Esc trên map thật → menu tạm dừng, game dừng", () => Manager().HandleEscape(),
                  () => Visible<PausePanel>() && Time.timeScale == 0f),
                G("Về menu từ map thật → timeScale trả về 1", () => Click("MainMenuButton", inScene: SceneNames.GameplayUI),
                  () => Active(SceneNames.MainGamePlay) && Visible<MainMenuPanel>() && Time.timeScale == 1f),
            };
        }

        private static Step G(string name, Action action, Func<bool> until) =>
            new() { name = name, action = action, until = until, tolerateGameplayErrors = true };

        /// <summary>Đổi cờ trên chính asset config trong RAM (không SetDirty nên không ghi ra đĩa) và reset provider.</summary>
        private static void SetFlags(bool mock, bool skipLogin, bool skipInit, bool skipSave, bool bypass)
        {
            UIDebugConfig config = UIServices.Config;
            _savedFlags ??= new[] { config.useMockData, config.skipLogin, config.skipGameplayInit, config.skipSaveLoad, config.bypassLoadRandomLogic };
            config.useMockData = mock;
            config.skipLogin = skipLogin;
            config.skipGameplayInit = skipInit;
            config.skipSaveLoad = skipSave;
            config.bypassLoadRandomLogic = bypass;
            UIServices.RebuildProviders();
        }

        private static void RestoreFlags()
        {
            if (_savedFlags == null) return;
            UIDebugConfig config = AssetDatabase.LoadAssetAtPath<UIDebugConfig>(UIFlowBuilder.ConfigPath);
            config.useMockData = _savedFlags[0];
            config.skipLogin = _savedFlags[1];
            config.skipGameplayInit = _savedFlags[2];
            config.skipSaveLoad = _savedFlags[3];
            config.bypassLoadRandomLogic = _savedFlags[4];
            _savedFlags = null;
        }

        private static Step S(string name, Action action, Func<bool> until) => new() { name = name, action = action, until = until };

        private static bool Active(string sceneName) => SceneManager.GetActiveScene().name == sceneName;

        private static bool Visible<T>() where T : UIPanel
        {
            T panel = Object.FindObjectOfType<T>();
            return panel != null && panel.IsVisible;
        }

        private static UIManager Manager() => Object.FindObjectOfType<UIManager>();

        private static int CountActive<T>() where T : Component
        {
            int count = 0;
            foreach (T component in Object.FindObjectsOfType<T>()) count++;   // FindObjectsOfType chỉ trả vật đang active
            return count;
        }

        private static GameObject Find(string name)
        {
            foreach (Transform t in Object.FindObjectsOfType<Transform>(true))
            {
                if (t.name == name && t.gameObject.scene.IsValid()) return t.gameObject;
            }
            throw new Exception("Không tìm thấy GameObject: " + name);
        }

        private static TMP_InputField Input(string name) => Find(name).GetComponent<TMP_InputField>();

        private static void Click(string name, string inScene = null)
        {
            // Cùng tên có thể có nhiều nút (ví dụ "MainMenuButton" ở Tạm dừng và Game Over) → chọn nút đang bấm được,
            // giống người chơi chỉ bấm được nút của panel đang hiện. IsInteractable() tính cả CanvasGroup của panel cha.
            bool found = false;
            foreach (Transform t in Object.FindObjectsOfType<Transform>(true))
            {
                if (t.name != name || (inScene != null && t.gameObject.scene.name != inScene)) continue;
                Button button = t.GetComponent<Button>();
                if (button == null) continue;
                found = true;
                if (!button.IsInteractable() || !button.gameObject.activeInHierarchy) continue;
                button.onClick.Invoke();
                return;
            }
            throw new Exception(found ? $"Nút {name} có nhưng đang bị khóa / ẩn." : "Không tìm thấy nút: " + name);
        }
    }
}
