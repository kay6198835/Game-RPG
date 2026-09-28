using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UIFlow
{
    /// <summary>
    /// Menu chính. "Tiếp tục" chỉ hiện khi đã có save, và khi đó nó là nút nổi bật nhất:
    /// to hơn, màu nhấn, được chọn sẵn (Enter là vào game ngay).
    /// Panel chỉ báo sự kiện; MainMenuFlow quyết định mở màn nào.
    /// </summary>
    public class MainMenuPanel : UIPanel
    {
        [SerializeField] private Button continueButton;
        [SerializeField] private TMP_Text continueDetailText;
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button loadGameButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;

        [Header("Nút nổi bật")]
        [SerializeField] private Color highlightColor = new(0.95f, 0.7f, 0.2f);
        [SerializeField] private Color normalColor = new(1f, 1f, 1f, 0.15f);
        [SerializeField] private float highlightScale = 1.12f;

        public event Action ContinueClicked;
        public event Action NewGameClicked;
        public event Action LoadGameClicked;
        public event Action SettingsClicked;

        protected override void Awake()
        {
            base.Awake();
            continueButton.onClick.AddListener(() => ContinueClicked?.Invoke());
            newGameButton.onClick.AddListener(() => NewGameClicked?.Invoke());
            loadGameButton.onClick.AddListener(() => LoadGameClicked?.Invoke());
            settingsButton.onClick.AddListener(() => SettingsClicked?.Invoke());
            quitButton.onClick.AddListener(Quit);
        }

        protected override void OnShown()
        {
            ISaveProvider saves = UIServices.Save;
            SaveSlotData recent = saves.GetMostRecent();
            bool hasSave = recent != null;

            continueButton.gameObject.SetActive(hasSave);
            loadGameButton.interactable = saves.HasAnySave;
            if (hasSave)
            {
                continueDetailText.text = $"{recent.characterName} · {recent.className} Lv.{recent.level} · {recent.locationName}";
            }

            // Nút quan trọng nhất: Tiếp tục (nếu có save), không thì Chơi mới.
            Button primary = hasSave ? continueButton : newGameButton;
            Emphasize(continueButton, primary == continueButton);
            Emphasize(newGameButton, primary == newGameButton);

            // Chọn sẵn để người chơi dùng bàn phím/tay cầm nhấn Enter là vào luôn.
            if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(primary.gameObject);
        }

        private void Emphasize(Button button, bool emphasized)
        {
            button.transform.localScale = Vector3.one * (emphasized ? highlightScale : 1f);
            if (button.targetGraphic != null) button.targetGraphic.color = emphasized ? highlightColor : normalColor;
        }

        private static void Quit()
        {
#if UNITY_EDITOR
            // Application.Quit() không làm gì trong Editor → dừng Play Mode thay thế.
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
