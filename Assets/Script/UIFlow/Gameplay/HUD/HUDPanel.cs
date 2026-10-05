using TMPro;
using UnityEngine;

namespace UIFlow
{
    /// <summary>
    /// HUD = panel gốc của scene GameplayUI. Neo (anchor) theo góc để không vỡ ở mọi độ phân giải:
    /// máu/mana góc trên-trái, minimap trên-phải, hotbar dưới-giữa, nhiệm vụ bên phải, thông báo trái-giữa.
    /// Cập nhật theo sự kiện StatsChanged, không đọc dữ liệu mỗi frame.
    /// </summary>
    public class HUDPanel : UIPanel
    {
        [SerializeField] private StatBar hpBar;
        [SerializeField] private StatBar manaBar;
        [SerializeField] private StatBar expBar;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private SkillHotbar hotbar;
        [SerializeField] private NotificationFeed notifications;

        public SkillHotbar Hotbar => hotbar;
        public NotificationFeed Notifications => notifications;

        private void OnEnable()
        {
            UIServices.Player.StatsChanged += Refresh;
            UIServices.Player.HotbarChanged += RebuildHotbar;
        }

        private void OnDisable()
        {
            UIServices.Player.StatsChanged -= Refresh;
            UIServices.Player.HotbarChanged -= RebuildHotbar;
        }

        private void Start()
        {
            RebuildHotbar();
            Refresh(UIServices.Player.GetStats());
        }

        private void RebuildHotbar() => hotbar.Build(UIServices.Player.GetHotbar());

        private void Refresh(PlayerStatsData stats)
        {
            hpBar.SetValue(stats.currentHP, stats.maxHP);
            manaBar.SetValue(stats.currentMana, stats.maxMana);
            // Gameplay thật chưa có EXP (expToNextLevel = 0) → ẩn thanh thay vì hiện "0 / 0".
            bool hasExp = stats.expToNextLevel > 0;
            expBar.gameObject.SetActive(hasExp);
            if (hasExp) expBar.SetValue(stats.exp, stats.expToNextLevel);
            nameText.text = $"{stats.characterName}  <size=75%><color=#BBB>{stats.className}</color></size>";
            levelText.text = stats.level.ToString();
        }
    }
}
