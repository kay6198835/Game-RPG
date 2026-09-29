using System;
using System.Collections.Generic;

namespace UIFlow
{
    /// <summary>
    /// Cầu nối tới nhân vật thật. Hiện mới nối đúng một thứ: sự kiện chết của gameplay.
    ///
    /// Vì sao chỉ ĐĂNG KÝ nghe EventManager mà không sửa gì bên gameplay?
    /// EventManager là bus tĩnh có sẵn; nghe một event là thao tác chỉ đọc, không đổi hành vi cũ.
    /// Lưu ý: PlayerDeathState hiện bắn ON_PLAYER_DEATH MỖI FRAME (BUG-086) → cờ _deathReported
    /// đảm bảo UI chỉ phản ứng một lần.
    /// </summary>
    public class RealPlayerDataProvider : IPlayerDataProvider, IDisposable
    {
        private readonly PlayerStatsData _stats = new() { characterName = "?", className = "?", level = 1, maxHP = 1, maxMana = 1 };
        private readonly List<CharacterClassInfo> _classes = new();
        private readonly List<SkillSlotData> _hotbar = new();
        private readonly List<SkillNodeData> _skillTree = new();
        private bool _deathReported;

        // TODO: nối logic thật — bắn sự kiện này khi máu/mana của Player đổi. Hiện chưa nối nên để accessor rỗng.
        public event Action<PlayerStatsData> StatsChanged { add { } remove { } }
        public event Action PlayerDied;

        public int SkillPoints => 0;

        public RealPlayerDataProvider()
        {
            EventManager.Resgister(EventID.ON_PLAYER_DEATH, OnPlayerDeath);
        }

        public void Dispose()
        {
            EventManager.UnResgister(EventID.ON_PLAYER_DEATH, OnPlayerDeath);
        }

        // TODO: nối logic thật — lấy IVitalComponent (máu/mana hiện tại) và IPlayerStatService (máu/mana tối đa)
        // của Player, rồi gọi StatsChanged khi chúng đổi. Hiện trả về số giữ chỗ.
        public PlayerStatsData GetStats() => _stats;

        // TODO: nối logic thật — danh sách class chơi được (hiện project chỉ có Paladin).
        public IReadOnlyList<CharacterClassInfo> GetAvailableClasses() => _classes;

        // TODO: nối logic thật — đọc PlayerData.AbilityBindings để dựng hotbar.
        public IReadOnlyList<SkillSlotData> GetHotbar() => _hotbar;

        // TODO: nối logic thật — project chưa có cây kỹ năng (chỉ có TalentManager hardcode).
        public IReadOnlyList<SkillNodeData> GetSkillTree() => _skillTree;

        public bool TryUnlockSkill(string skillId) => false;

        public void Respawn()
        {
            // Gameplay hiện không có đường hồi sinh (BUG-087), nên "hồi sinh" = load lại scene gameplay.
            // TODO: nối logic thật — gọi Reborn() của gameplay khi có, và đưa nhân vật về checkpoint.
            _deathReported = false;
            SceneFlow.EnterGameplay();
        }

        private void OnPlayerDeath(object _)
        {
            if (_deathReported) return;
            _deathReported = true;
            PlayerDied?.Invoke();
        }
    }
}
