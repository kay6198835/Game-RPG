using UnityEngine;

namespace UIFlow
{
    /// <summary>
    /// Điều phối luồng trong scene MainGamePlay:
    ///   Logo → (Đăng nhập nếu skipLogin tắt) → Menu chính → Chơi mới / Chọn save / Cài đặt → vào game.
    /// Không có màn tạo nhân vật: "Chơi mới" tạo ngay một save mặc định (class Paladin — class duy nhất
    /// của game) rồi vào gameplay.
    /// Các panel chỉ báo sự kiện ("người chơi bấm Chơi mới"); lớp này quyết định mở màn nào.
    /// Tách vậy để panel dùng lại được và luồng nằm gọn ở một chỗ.
    /// </summary>
    public class MainMenuFlow : MonoBehaviour
    {
        [SerializeField] private UIManager uiManager;
        [SerializeField] private SplashPanel splashPanel;
        [SerializeField] private LoginPanel loginPanel;
        [SerializeField] private MainMenuPanel mainMenuPanel;
        [SerializeField] private SaveSelectPanel saveSelectPanel;
        [SerializeField] private SettingsPanel settingsPanel;

        private const string DefaultClassName = "Paladin";

        // Chỉ hiện logo ở lần mở game đầu tiên; quay về menu từ gameplay thì vào thẳng menu chính.
        private static bool _splashShown;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics() => _splashShown = false;

        private void OnEnable()
        {
            splashPanel.Finished += OnSplashFinished;
            loginPanel.LoggedIn += OnLoggedIn;
            mainMenuPanel.ContinueClicked += OnContinue;
            mainMenuPanel.NewGameClicked += OnNewGame;
            mainMenuPanel.LoadGameClicked += OnLoadGame;
            mainMenuPanel.SettingsClicked += OnSettings;
            saveSelectPanel.SaveChosen += OnSaveChosen;
        }

        private void OnDisable()
        {
            splashPanel.Finished -= OnSplashFinished;
            loginPanel.LoggedIn -= OnLoggedIn;
            mainMenuPanel.ContinueClicked -= OnContinue;
            mainMenuPanel.NewGameClicked -= OnNewGame;
            mainMenuPanel.LoadGameClicked -= OnLoadGame;
            mainMenuPanel.SettingsClicked -= OnSettings;
            saveSelectPanel.SaveChosen -= OnSaveChosen;
        }

        private void Start()
        {
            if (_splashShown)
            {
                uiManager.SetRoot(mainMenuPanel);
                return;
            }
            _splashShown = true;
            uiManager.SetRoot(splashPanel);
        }

        private void OnSplashFinished()
        {
            if (UIServices.Config.skipLogin) uiManager.SetRoot(mainMenuPanel);
            else uiManager.SetRoot(loginPanel);
        }

        private void OnLoggedIn() => uiManager.SetRoot(mainMenuPanel);

        private void OnContinue()
        {
            SaveSlotData recent = UIServices.Save.GetMostRecent();
            if (recent == null) return;
            UIServices.Save.SelectedSlot = recent.slotIndex;
            UIServices.OnSaveSelected();
            SceneFlow.EnterGameplay();
        }

        private void OnNewGame()
        {
            ISaveProvider saves = UIServices.Save;
            saves.CreateNewSave(new NewCharacterRequest
            {
                characterName = $"{DefaultClassName} {saves.GetSlots().Count + 1}",
                classId = DefaultClassName,
            });
            UIServices.OnSaveSelected();
            SceneFlow.EnterGameplay();
        }

        private void OnLoadGame() => uiManager.Open(saveSelectPanel);

        private void OnSettings() => uiManager.Open(settingsPanel);

        private void OnSaveChosen(SaveSlotData save) => SceneFlow.EnterGameplay();
    }
}
