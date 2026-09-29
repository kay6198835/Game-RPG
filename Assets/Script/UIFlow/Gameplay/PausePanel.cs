using UnityEngine;
using UnityEngine.UI;

namespace UIFlow
{
    /// <summary>
    /// Menu tạm dừng (Esc khi đang ở HUD). Đặt pausesGame = true: UIManager tự đưa Time.timeScale về 0
    /// khi mở và về 1 khi đóng, nên panel này không tự đụng tới timeScale.
    /// </summary>
    public class PausePanel : UIPanel
    {
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button saveButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private SettingsPanel settingsPanel;

        protected override void Awake()
        {
            base.Awake();
            resumeButton.onClick.AddListener(CloseSelf);
            saveButton.onClick.AddListener(Save);
            settingsButton.onClick.AddListener(() => Manager.Open(settingsPanel));
            mainMenuButton.onClick.AddListener(SceneFlow.ReturnToMainMenu);
        }

        protected override void OnShown()
        {
            // Chưa chọn save (ví dụ bấm Play thẳng ở GameplayMock) thì không có gì để lưu.
            saveButton.interactable = UIServices.Save.SelectedSlot >= 0;
        }

        private void Save()
        {
            bool saved = UIServices.Save.SaveCurrent();
            UIEvents.Notify(saved ? "Đã lưu game." : "Lưu thất bại.");
        }
    }
}
