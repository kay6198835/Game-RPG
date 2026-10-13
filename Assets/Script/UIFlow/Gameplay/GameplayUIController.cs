using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

namespace UIFlow
{
    /// <summary>
    /// Bộ điều khiển của scene GameplayUI (được load Additive chồng lên scene gameplay).
    /// - Phím tắt: I = Túi đồ, K = Kỹ năng, J = Nhiệm vụ (đổi được trong Cài đặt). Esc do UIManager lo.
    /// - Nghe UIEvents (hộp thoại, cửa hàng, thông báo, cooldown) và sự kiện chết của người chơi.
    /// </summary>
    public class GameplayUIController : MonoBehaviour
    {
        // Thứ tự trang trong TabWindow — phải khớp với mảng pages trong Inspector.
        private const int InventoryTab = 0;
        private const int CharacterTab = 1;
        private const int SkillTab = 2;
        private const int QuestTab = 3;

        [SerializeField] private UIManager uiManager;
        [SerializeField] private HUDPanel hud;
        [SerializeField] private TabWindow tabWindow;
        [SerializeField] private DialoguePanel dialoguePanel;
        [SerializeField] private ShopPanel shopPanel;
        [SerializeField] private GameOverPanel gameOverPanel;
        [SerializeField] private ConfirnPanel confirnPanel;

        private void OnEnable()
        {
            UIEvents.DialogueRequested += OnDialogueRequested;
            UIEvents.ShopRequested += OnShopRequested;
            UIEvents.NotificationRequested += OnNotification;
            UIEvents.SkillUsed += OnSkillUsed;
            UIServices.Player.PlayerDied += OnPlayerDied;
            UIEvents.OpenConfirnPanel += OnConfirnPanel;
        }

        private void OnDisable()
        {
            UIEvents.DialogueRequested -= OnDialogueRequested;
            UIEvents.ShopRequested -= OnShopRequested;
            UIEvents.NotificationRequested -= OnNotification;
            UIEvents.SkillUsed -= OnSkillUsed;
            UIServices.Player.PlayerDied -= OnPlayerDied;
            UIEvents.OpenConfirnPanel -= OnConfirnPanel;
        }

        private void Start()
        {
            // Scene gameplay nào cũng nên có EventSystem riêng. Nếu thiếu (ví dụ mở thẳng scene GameplayUI),
            // tạo một cái để nút bấm vẫn hoạt động. Không tạo khi đã có → tránh cảnh báo "2 EventSystem".
            if (EventSystem.current == null)
            {
                GameObject eventSystem = new("EventSystem (GameplayUI)", typeof(EventSystem), typeof(InputSystemUIInputModule));
                eventSystem.transform.SetParent(transform, false);
            }
        }

        private void Update()
        {
            // Không mở cửa sổ đè lên hộp thoại / Game Over / menu tạm dừng.
            bool free = uiManager.IsAtRoot || uiManager.Top == tabWindow;
            if (!free) return;

            if (UIInput.WasPressed(KeyBindings.Inventory)) ToggleTab(InventoryTab);
            else if (UIInput.WasPressed(KeyBindings.SkillTree)) ToggleTab(SkillTab);
            else if (UIInput.WasPressed(KeyBindings.QuestLog)) ToggleTab(QuestTab);
        }

        /// <summary>Mở cửa sổ ở tab chỉ định; bấm lại đúng phím của tab đang mở thì đóng.</summary>
        public void ToggleTab(int tab)
        {
            if (uiManager.IsOpen(tabWindow) && tabWindow.CurrentTab == tab)
            {
                uiManager.Close(tabWindow);
                return;
            }
            uiManager.Open(tabWindow);
            tabWindow.SelectTab(tab);
        }

        public void OpenCharacter() => ToggleTab(CharacterTab);

        private void OnDialogueRequested(IReadOnlyList<DialogueLine> lines, Action onFinished)
        {
            uiManager.Open(dialoguePanel);
            dialoguePanel.Play(lines, onFinished);
        }

        private void OnShopRequested() => uiManager.Open(shopPanel);

        private void OnNotification(string message) => hud.Notifications.Show(message);

        private void OnSkillUsed(int index) => hud.Hotbar.TriggerCooldown(index);

        private void OnPlayerDied()
        {
            uiManager.CloseAll();
            uiManager.Open(gameOverPanel);
        }

        private void OnConfirnPanel(ConfirnData data)
        {
            confirnPanel.SetConfirm(data);
            uiManager.Open(confirnPanel);
        }
    }
}
