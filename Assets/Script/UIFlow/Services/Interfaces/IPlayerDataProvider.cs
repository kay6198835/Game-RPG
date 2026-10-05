using System;
using System.Collections.Generic;

namespace UIFlow
{
    /// <summary>
    /// Dữ liệu nhân vật cho HUD, bảng Nhân vật, cây kỹ năng, hotbar và màn Game Over.
    /// UI chỉ ĐỌC; thay đổi duy nhất UI được yêu cầu là mở khóa kỹ năng và hồi sinh.
    /// </summary>
    public interface IPlayerDataProvider
    {
        PlayerStatsData GetStats();

        /// <summary>Bắn ra mỗi khi máu/mana/level đổi → HUD cập nhật, không cần poll trong Update.</summary>
        event Action<PlayerStatsData> StatsChanged;

        /// <summary>Bắn ra đúng MỘT lần khi nhân vật chết → mở màn Game Over.</summary>
        event Action PlayerDied;

        /// <summary>Danh sách ô hotbar đổi (ví dụ Player thật vừa xuất hiện) → HUD dựng lại hotbar.</summary>
        event Action HotbarChanged;

        void Respawn();

        IReadOnlyList<SkillSlotData> GetHotbar();

        IReadOnlyList<SkillNodeData> GetSkillTree();

        int SkillPoints { get; }

        bool TryUnlockSkill(string skillId);
    }
}
