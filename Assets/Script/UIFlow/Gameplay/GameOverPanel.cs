using UnityEngine;
using UnityEngine.UI;

namespace UIFlow
{
    /// <summary>
    /// Màn Game Over: hồi sinh ở checkpoint hoặc về menu. closeOnEscape = false trong Inspector:
    /// người chơi phải chọn một trong hai, Esc không được đưa họ về HUD khi đang chết.
    /// </summary>
    public class GameOverPanel : UIPanel
    {
        [SerializeField] private Button respawnButton;
        [SerializeField] private Button mainMenuButton;

        protected override void Awake()
        {
            base.Awake();
            respawnButton.onClick.AddListener(Respawn);
            mainMenuButton.onClick.AddListener(SceneFlow.ReturnToMainMenu);
        }

        private void Respawn()
        {
            CloseSelf();
            // Load lại scene gameplay (xem RealPlayerDataProvider.Respawn).
            UIServices.Player.Respawn();
            UIEvents.Notify("Đã hồi sinh ở checkpoint.");
        }
    }
}
